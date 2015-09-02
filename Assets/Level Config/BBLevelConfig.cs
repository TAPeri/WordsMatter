/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class BBLevelConfig : ILevelConfig
{
	string bridgeWord;
	List<HighlightDesc> correctHighlightAreas;
	string description;

	bool useTTS;


	public int languageArea = -1;
	public int difficulty = -1;

	public BBLevelConfig(string para_bridgeWord, List<HighlightDesc> para_correctHighlightAreas, string para_description,TtsType ttsLevel,int languageArea,int difficulty)
	{

		this.difficulty = difficulty;
		this.languageArea = languageArea;

		bridgeWord = para_bridgeWord;
		correctHighlightAreas = para_correctHighlightAreas;
		correctHighlightAreas.Sort(new HighlightComparer());
		description = para_description;
		useTTS = (ttsLevel==TtsType.SPOKEN2WRITTEN);
	}

	public string getBridgeWord() { return bridgeWord; }
	public int getBridgeWordLength() { return bridgeWord.Length; }
	public List<HighlightDesc> getCorrectHighlightAreas() { return correctHighlightAreas; }
	public string getDescription() { return description; }
	public bool getUseTtsFlag() { return useTTS; }


	public int getTotHighlightPoints()
	{
		int totHighlightPoints = 0;
		for(int i=0; i<correctHighlightAreas.Count; i++)
		{
			totHighlightPoints += correctHighlightAreas[i].getRegionLength();
		}
		return totHighlightPoints;
	}



	public int[] getHighlightMismatch(List<HighlightDesc> para_playerHighlights)
	{
		IndexRegionTools irt = new IndexRegionTools();

		List<IndexRegion> baseList = convertHighlightListToRegionList(correctHighlightAreas);
		List<IndexRegion> inputList = convertHighlightListToRegionList(para_playerHighlights);


		List<IndexRegion> filteredRegions = irt.getFilter(baseList,inputList,0,this.getBridgeWordLength());
		string binArrStr = irt.createBinArr(filteredRegions,0,this.getBridgeWordLength());
		UnityEngine.Debug.Log("MisMatchStr: "+binArrStr);
		int[] retMismatchArr = irt.createCompactIntArr(binArrStr);

		return retMismatchArr;

		/*List<HighlightDesc> retHList = new List<HighlightDesc>();
		for(int i=0; i<filteredRegions.Count; i++)
		{
			retHList.Add(new HighlightDesc(filteredRegions[i].startIndex,filteredRegions[i].endIndex));
		}*

		return retHList;*/
	}

	public List<IndexRegion> convertHighlightListToRegionList(List<HighlightDesc> para_hList)
	{
		List<IndexRegion> retList = new List<IndexRegion>();

		for(int i=0; i<para_hList.Count; i++)
		{
			retList.Add((IndexRegion) para_hList[i]);
		}

		return retList;
	}


	// Sorts by start index.
	class HighlightComparer : System.Collections.Generic.IComparer<HighlightDesc>
	{
		public int Compare(HighlightDesc x, HighlightDesc y)
		{
			if(x.startIndex < y.startIndex)
			{
				return -1;
			}
			else if(x.startIndex > y.startIndex)
			{
				return 1;
			}
			else
			{
				// x.startIndex == y.startIndex
				return 0;
			}
		}
	}

}

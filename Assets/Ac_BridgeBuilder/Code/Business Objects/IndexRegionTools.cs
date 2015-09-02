/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using System.Collections.Generic;

public class IndexRegionTools
{


	public List<IndexRegion> getFilter(List<IndexRegion> para_base, List<IndexRegion> para_input, int para_firstPossibleIndexOnScale, int para_maxLength)
	{

		List<IndexRegion> baseRegions = para_base;
		baseRegions.Sort(new IndexRegionComparer());

		List<IndexRegion> inputRegions = para_input;
		inputRegions.Sort(new IndexRegionComparer());


		// This is a quick version but can be improved later.
		string binArr1 = createBinArr(baseRegions,para_firstPossibleIndexOnScale,para_maxLength);
		string binArr2 = createBinArr(inputRegions,para_firstPossibleIndexOnScale,para_maxLength);


				/*corr,inp,res
		0 0	0   
		0 1 0
		1 0 1
		1 1 0*/


		string resArr = "";
		for(int i=0; i<binArr1.Length; i++)
		{
			if((binArr1[i] == '1')&&(binArr2[i] == '0'))
			{
				resArr += "1";
			}
			else
			{
				resArr += "0";
			}
		}	

		List<IndexRegion> reqRList = convertStrToIndexRegionList(resArr,para_firstPossibleIndexOnScale);
		return reqRList;
	}


	public string createBinArr(List<IndexRegion> para_regionList, int para_firstPossibleIndexOnScale, int para_length)
	{
		string nwBinArr = "";

		for(int i=0; i<para_regionList.Count; i++)
		{
			IndexRegion currRegion = para_regionList[i];

			for(int j=currRegion.startIndex; j<(currRegion.endIndex+1); j++)
			{
				nwBinArr += "1";
			}

			if(i < (para_regionList.Count-1))
			{
				IndexRegion nxtRegion = para_regionList[i+1];

				for(int k=(currRegion.endIndex+1); k<nxtRegion.startIndex; k++)
				{
					nwBinArr += "0";
				}
			}
		}



		int paddingRequired = 0;
		if(para_firstPossibleIndexOnScale < para_regionList[0].startIndex)
		{
			paddingRequired = para_regionList[0].startIndex - para_firstPossibleIndexOnScale;
			nwBinArr = applyBinArrFrontPadding(nwBinArr,paddingRequired);
		}

		if(nwBinArr.Length < para_length)
		{
			paddingRequired = para_length - nwBinArr.Length;
			nwBinArr = applyBinArrBackPadding(nwBinArr,paddingRequired);
		}

		
		return nwBinArr;
	}

	public int[] createCompactIntArr(string para_binStr)
	{
		List<int> reqIndexes = new List<int>();
		for(int i=0; i<para_binStr.Length; i++)
		{
			if(para_binStr[i] == '1')
			{
				reqIndexes.Add(i);
			}
		}

		if(reqIndexes.Count == 0)
		{
			return null;
		}
		else
		{
			return reqIndexes.ToArray();
		}
	}

	private string applyBinArrFrontPadding(string para_binArr, int para_paddingSize)
	{
		return ((getPadding(para_paddingSize)) + (para_binArr));
	}

	private string applyBinArrBackPadding(string para_binArr, int para_paddingSize)
	{
		return ((para_binArr) + (getPadding(para_paddingSize)));
	}

	private string getPadding(int para_paddingSize)
	{
		string padding = "";
		for(int i=0; i<para_paddingSize; i++)
		{
			padding += "0";
		}
		return padding;
	}


	public List<IndexRegion> convertStrToIndexRegionList(string para_strArr, int para_firstPossibleIndexOnScale)
	{
		List<IndexRegion> retRegionList = new List<IndexRegion>();


		bool breakFlag = false;
		int startIndex;
		int endIndex = -1;

		do
		{
			breakFlag = false;

			if((endIndex+1) >= (para_strArr.Length))
			{
				breakFlag = true;
			}
			else
			{

				startIndex = searchForChar(para_strArr,endIndex+1,'1');
				if((startIndex < 0)||(startIndex >= (para_strArr.Length)))
				{
					breakFlag = true;
				}
				else
				{
					endIndex = (searchForChar(para_strArr,startIndex,'0') -1);
					if(endIndex < 0)
					{
						breakFlag = true;
					}
					else
					{
						if(endIndex == -1)
						{
							endIndex = (para_strArr.Length-1);
							breakFlag = true;
						}

						retRegionList.Add(new IndexRegion(startIndex,endIndex));
					}
				}
			}
		}
		while( ! breakFlag);


		return retRegionList;
	}

	private int searchForChar(string para_strArr, int para_startIndex, char para_reqChar)
	{

		for(int i=para_startIndex; i<para_strArr.Length; i++)
		{
			if(para_strArr[i] == para_reqChar)
			{
				return i;
			}
		}

		return -1;
	}

	/*public List<IndexRegion> convertStrToIndexRegionList(string para_strArr, int para_firstPossibleIndexOnScale)
	{
		List<IndexRegion> retRegionList = new List<IndexRegion>();

		bool hasStartedRegion = false;
		int tmpRegionStart = para_firstPossibleIndexOnScale;
		int tmpRegionEnd = para_firstPossibleIndexOnScale;
		int currIndex = para_firstPossibleIndexOnScale;
		for(int i=0; i<para_strArr.Length; i++)
		{
			if(para_strArr[i] == '1')
			{
				if( ! hasStartedRegion)
				{
					hasStartedRegion = true;
					tmpRegionStart = currIndex;
				}
				else
				{
					//
				}
			}
			else if(para_strArr[i] == '0')
			{
				if( hasStartedRegion)
				{
					hasStartedRegion = false;
					tmpRegionEnd = currIndex--;
					retRegionList.Add(new IndexRegion(tmpRegionStart,tmpRegionEnd));
				}
				else
				{
					//
				}
			}

			currIndex++;
		}

		return retRegionList;
	}*/
	


	// Sorts by start index.
	class IndexRegionComparer : System.Collections.Generic.IComparer<IndexRegion>
	{
		public int Compare(IndexRegion x, IndexRegion y)
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

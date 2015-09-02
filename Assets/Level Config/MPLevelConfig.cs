/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class MPLevelConfig : ILevelConfig
{
	public int[] boardSize; 
	public string[] correctItems;
	public string[] fillerItems;

	public bool useTTS;
	public string pattern;

	public int languageArea = -1;
	public int difficulty = -1;

	public MPLevelConfig(int[] para_boardSize,
	                     string[] para_correctItems,
	                     string[] para_fillerItems,
	                     string _pattern,
	                     bool isSound,int languageArea,int difficulty)
	{
		
		this.difficulty = difficulty;
		this.languageArea = languageArea;
		boardSize = para_boardSize;
		correctItems = para_correctItems;
		fillerItems = para_fillerItems;
		pattern = _pattern;
		useTTS = isSound;
	}

	public bool isCorrectItem(string para_testText)
	{


		bool retFlag = false;
		for(int i=0; i<correctItems.Length; i++)
		{
			if(correctItems[i] == para_testText)
			{
				retFlag = true;
				break;
			}
		}
		return retFlag;
	}
}

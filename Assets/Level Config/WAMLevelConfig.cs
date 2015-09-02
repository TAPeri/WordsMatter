/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class WAMLevelConfig : ILevelConfig
{
	public string descriptionLabel;
	public string[] correctItems;
	public string[] fillerItems;
	public bool useTTS;
	public int speed;

	public int languageArea = -1;
	public int difficulty = -1;

	public WAMLevelConfig(string para_descriptionLabel,
	                      string[] para_correctItems,
	                      string[] para_fillerItems,
	                      bool _TTSpattern,
	                      int _speed,int languageArea,int difficulty)
	{
		
		this.difficulty = difficulty;
		this.languageArea = languageArea;
		descriptionLabel = para_descriptionLabel;
		correctItems = para_correctItems;
		fillerItems = para_fillerItems;
		useTTS = _TTSpattern;
		speed = _speed;
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

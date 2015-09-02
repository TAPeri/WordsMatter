/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class SHeroLevelConfig : ILevelConfig
{
	public string[] sentenceWords;
	public int[] holeList;
	public string[] wordPoolWords;
	public bool isWord;
	public int speed;

	public int languageArea = -1;
	public int difficulty = -1;

	public SHeroLevelConfig(string[] para_sentenceWords,
	                        int[] para_holeList,
	                        string[] para_wordPoolWords,
	                        bool _isWord,
	                        int _speed,int languageArea,int difficulty)
	{

		this.difficulty = difficulty;
		this.languageArea = languageArea;
		sentenceWords = para_sentenceWords;
		holeList = para_holeList;
		wordPoolWords = para_wordPoolWords;
		isWord = _isWord;
		speed = _speed;
	}


	public string getLevelSentence(bool para_addEnclosure)
	{
		string retStr = "";
		if(para_addEnclosure) { retStr += "'"; }
		for(int i=0; i<sentenceWords.Length; i++)
		{
			retStr += (sentenceWords[i]);
			
			if(i < (sentenceWords.Length-1))
			{
				if(!isWord)
					retStr += " ";
			}
		}
		if(para_addEnclosure) { retStr += "'"; }
		return retStr;
	}

	public bool positionIsHole(int para_position)
	{
		bool retFlag = false;
		for(int i=0; i<holeList.Length; i++)
		{
			if(holeList[i] == para_position)
			{
				retFlag = true;
				break;
			}
		}
		return retFlag;
	}
}

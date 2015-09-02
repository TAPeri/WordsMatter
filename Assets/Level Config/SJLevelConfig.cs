/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class SJLevelConfig : ILevelConfig
{
	SJWordItem wordItem;
	bool useTTS = false;
	int speed = 0;



	public SJLevelConfig(SJWordItem para_wordItem,bool TTSon,int _speed)
	{

		wordItem = para_wordItem;
		speed = _speed;
		useTTS = TTSon;
	}

	public int getSpeed(){
		return speed;
	}

	public SJWordItem getWordItem()
	{
		return wordItem;
	}

	public bool getUseTtsFlag() { return useTTS; }

	public string createFormattedSplitString(string para_sourceString, List<int> para_splitPattern)
	{
		string retStr = "";
		
		if((para_splitPattern != null)&&(para_splitPattern.Count > 0))
		{
			int tmpCounter = 0;
			bool stopChecks = false;
			int nxtSplitPos = para_splitPattern[tmpCounter];
			
			for(int i=0; i<para_sourceString.Length; i++)
			{
				retStr += para_sourceString[i];
				
				if( ! stopChecks)
				{
					if(i == nxtSplitPos)
					{
						retStr += "-";
						tmpCounter++;
						if(tmpCounter < para_splitPattern.Count)
						{
							nxtSplitPos = para_splitPattern[tmpCounter];
						}
						else
						{
							stopChecks = true;
						}
					}
				}
				
			}
		}
		
		return retStr;
	}
}
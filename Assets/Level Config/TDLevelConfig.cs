/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class TDLevelConfig : ILevelConfig
{
	string targetWord;
	string[] carriageItems;

	public int languageArea = -1;
	public int difficulty = -1;
	int speed;

	public TDLevelConfig(string para_targetWord, string[] para_carriageItems,int languageArea,int difficulty,int speed)
	{
		
		this.difficulty = difficulty;
		this.languageArea = languageArea;
		targetWord = para_targetWord;
		carriageItems = para_carriageItems;
		this.speed = speed;
	}

	public string getTargetWord() { return targetWord; }
	public string[] getCarriageItems() { return carriageItems; }
	public int getNumberOfCarriages() { return carriageItems.Length; }
	public int getSpeed(){return speed;}

	public bool isCorrectTrainSetup(string[] para_playerAssignedCarriageItems)
	{
		/*if(para_playerAssignedCarriageItems.Length > carriageItems.Length)
		{
			// To long.
			return false;
		}*/

		if(para_playerAssignedCarriageItems.Length < carriageItems.Length)
		{
			// To short.
			return false;
		}

		for(int i=0; i<para_playerAssignedCarriageItems.Length; i++)
		{
			if(i >= carriageItems.Length)
			{
				if((para_playerAssignedCarriageItems[i] != null)
				&&(para_playerAssignedCarriageItems[i] != ""))
				{
					return false;
				}
			}
			else
			{
				if(para_playerAssignedCarriageItems[i] != carriageItems[i].ToLower())
				{
					// Mis-match with strings.
					return false;
				}
			}


			/*if(i >= carriageItems.Length)
			{
				// No corresponding position.
				return false;
			}
			else
			{
				if(para_playerAssignedCarriageItems[i] != para_playerAssignedCarriageItems[i])
				{
					// Mis-match with strings.
					return false;
				}
			}*/
		}

		return true;
	}

	public List<int> determineCorrectCarriages(string[] para_playerAssignedCarriageItems)
	{
		List<int> correctItems = new List<int>();

		for(int i=0; i<para_playerAssignedCarriageItems.Length; i++)
		{
			string tmpPlayerItem = para_playerAssignedCarriageItems[i];
			if(tmpPlayerItem != null)
			{
				if(i < carriageItems.Length)
				{
					if(tmpPlayerItem == carriageItems[i].ToLower())
					{
						correctItems.Add(i);
					}
				}
			}
		}

		return correctItems;
	}


	public string getFormattedSplitString()
	{
		string retStr = "";
		
		if(carriageItems != null)
		{
			for(int i=0; i<carriageItems.Length; i++)
			{
				if(carriageItems[i] != null)
				{
					if(i > 0)
					{
						retStr += "-";
					}
					
					retStr += carriageItems[i];
				}
			}
		}
		
		return retStr;
	}
}

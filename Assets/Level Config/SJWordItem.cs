/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */


public class SJWordItem
{
	int id;
	string word;
	public int[] syllSplitPositions;
	bool[] openSyllables;

	public int languageArea = -1;
	public int difficulty = -1;
	
	public SJWordItem(int para_id, string para_word, int[] para_syllSplitPositions,bool[] _openSyllables,int languageArea,int difficulty)
	{
		
		this.difficulty = difficulty;
		this.languageArea = languageArea;
		id = para_id;
		word = para_word;
		syllSplitPositions = para_syllSplitPositions;
		openSyllables = _openSyllables;
	}

	public int getID() { return id; }
	public string getWordAsString() { return word; }
	public int getWordLength() { return word.Length; }
	public bool[] getOpenSyllables(){return openSyllables;}
	public int[] getSyllSplitPositions()
	{
		//Debug.LogWarning("This should have into account the pattern that we are learning, e.g. only one split for prefixing");
		return syllSplitPositions;
	}
	public int getNumOfReqSplits() { return syllSplitPositions.Length; }



	public bool isValidSplitIndex(int para_splitIndex)
	{
		bool validFlag = false;
		for(int i=0; i<syllSplitPositions.Length; i++)
		{
			if(syllSplitPositions[i] == para_splitIndex)
			{
				validFlag = true;
				break;
			}
		}
		return validFlag;
	}
	




	// NOTE: Could be improved.
	public bool checkIfWordSegmentIsIndivisible(int para_segStartCharIndex, int para_segEndCharIndex)
	{

		
		bool isIndivisible = false;
		
		
		bool checkStart = true;
		bool checkEnd = true;
		
		int startSplitIndex = 0;
		int endSplitIndex = 0;
		
		if(para_segStartCharIndex == 0)
		{
			checkStart = false;	
			startSplitIndex = -1;
		}
		
		if(para_segEndCharIndex == (word.Length-1))
		{
			checkEnd = false;
			endSplitIndex = syllSplitPositions.Length;
		}
		
		
		
		
		
		for(int i=0; i<syllSplitPositions.Length; i++)
		{
			if(checkStart)
			{
				if(syllSplitPositions[i] == (para_segStartCharIndex-1))
				{
					startSplitIndex = i;	
				}
			}
			
			if(checkEnd)
			{
				if(syllSplitPositions[i] == (para_segEndCharIndex))
				{
					endSplitIndex = i;	
				}
			}
		}
		
		
		
		if(UnityEngine.Mathf.Abs(endSplitIndex - startSplitIndex) == 1)
		{
			isIndivisible = true;	
		}
		else
		{
			isIndivisible = false;	
		}
		
		return isIndivisible;
	}
}

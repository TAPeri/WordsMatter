/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class PDLevelConfig : ILevelConfig
{
	string[] parcelWord;
	int[] correctNumOfKnocks;

	public int[] languageArea;
	public int[] difficulty;
	int attempts;

	public PDLevelConfig(string[] para_parcelWord,
	                     int[] para_correctNumOfKnocks,int[] languageArea,int[] difficulty,int attempts)
	{
		
		this.difficulty = difficulty;
		this.languageArea = languageArea;
		parcelWord = para_parcelWord;
		correctNumOfKnocks = para_correctNumOfKnocks;
		this.attempts = attempts;
	}

	public bool isCorrectNumOfKnocks(int para_playerKnocks, int index)
	{
		return (correctNumOfKnocks[index] == para_playerKnocks);
	}

	public string[] getParcelWord()
	{
		return parcelWord;
	}

	public int[] getCorrectNumOfKnocks() { return correctNumOfKnocks; }
	public int getAttempts(){return attempts;}
}

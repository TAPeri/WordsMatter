/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class PDLevelOutcome : LevelOutcome
{
	int numCorrectWords;

	public PDLevelOutcome(bool para_isPositive, int correctWords)
		:base(para_isPositive)
	{
		numCorrectWords = correctWords;
	}

	public int getNumCorrectWords() { return numCorrectWords; }
}

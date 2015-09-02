/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class SJGameyResultData : GameyResultData
{
	public int numOfLines;
	public int numCorrectSplits;
	public int numWrongSplits;

	public SJGameyResultData(int para_numOfLines, int para_numCorrectSplits, int para_numWrongSplits)
	{
		numOfLines = para_numOfLines;
		numCorrectSplits = para_numCorrectSplits;
		numWrongSplits = para_numWrongSplits;
	}
}

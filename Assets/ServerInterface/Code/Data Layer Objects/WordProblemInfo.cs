/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
[System.Serializable]
public class WordProblemInfo
{
	public int category;
	public int index;
	public MatchedData[] matched;

	public WordProblemInfo()
	{
		// Empty constructor required for JSON converter.
	}
}

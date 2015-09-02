/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
[System.Serializable]
public class GraphemePhonemePair
{
	public string grapheme;
	public string phoneme;

	public GraphemePhonemePair()
	{
		// Empty constructor required for JSON converter.
	}
}

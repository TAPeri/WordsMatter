/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
[System.Serializable]
public class PackagedAnnotatedWord
{
	public AnnotatedWord annotatedWord;

	// Meta data.
	public bool filler;

	public PackagedAnnotatedWord()
	{
		// Empty constructor required for JSON converter.
	}

	public PackagedAnnotatedWord(AnnotatedWord a, bool f){ annotatedWord = a; filler = f;}

	public AnnotatedWord getAnnotatedWord() { return annotatedWord; }
	public bool isFiller() { return filler; }
}

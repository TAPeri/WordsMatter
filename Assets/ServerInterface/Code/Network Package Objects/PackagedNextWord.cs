/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */[System.Serializable]

public class PackagedNextWord{
	
	public AnnotatedWord annotatedWord;
	public AnnotatedSentence annotatedSentence;

	public bool filler;


	public PackagedNextWord()
	{
		// Empty constructor required for JSON converter.
	}
	

	public PackagedNextWord(AnnotatedWord a, bool f){ annotatedWord = a; annotatedSentence = null;filler = f;}
	public PackagedNextWord(AnnotatedSentence a, bool f){ annotatedWord = null; annotatedSentence = a;filler = f;}
	
	public AnnotatedWord getAnnotatedWord() { return annotatedWord; }
	public AnnotatedSentence getAnnotatedSentence() { return annotatedSentence; }

	public bool getFiller(){ return filler;}
	
	
}

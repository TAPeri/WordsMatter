/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

using System.Collections.Generic;

[System.Serializable]
public class AnnotatedWord : Word
{

	public List<WordProblemInfo> wordProblems;// = new List<WordProblemInfo>();

	public AnnotatedWord()
	{
		// Empty constructor required for JSON converter.
	}

	public AnnotatedWord(string _word, List<int> _ssPosIntList) : base(_word, _ssPosIntList){ 
		;
	}

	public AnnotatedWord(string _word,string _type) : base(_word, _type){ 
		;
	}

	public AnnotatedWord(string _word,string _type,string[] _syllables) : base(_word, _type,_syllables){ 
		;
	}

	public List<WordProblemInfo> getWordProblems() { return wordProblems; }

	/*public AnnotatedWord(Word w)
		:base()
	{
		base.word = w.getWord();
		base.wordUnmodified = w.getWordUnmodified();
		base.type = w.getType();
		base.syllables = w.getSyllables();
		base.cvForm = w.getCVForm();
		base.phonetics = w.getPhonetics();
		base.numSyllables = w.getNumberOfSyllables();
		base.languageCode = w.getLanguageCode();
		base.frequency = w.getFrequency();
		base.graphemesPhonemes = w.getGraphemesPhonemes();
		//this.wordProblems = getProblems(w);
	}

	public AnnotatedWord(Word w, int i, int j)
		:base()
	{
		base.word = w.getWord();
		base.wordUnmodified = w.getWordUnmodified();
		base.type = w.getType();
		base.syllables = w.getSyllables();
		base.cvForm = w.getCVForm();
		base.phonetics = w.getPhonetics();
		base.numSyllables = w.getNumberOfSyllables();
		base.languageCode = w.getLanguageCode();
		base.frequency = w.getFrequency();
		base.graphemesPhonemes = w.getGraphemesPhonemes();
		this.wordProblems = getProblems(w, i, j);
	}*/
}

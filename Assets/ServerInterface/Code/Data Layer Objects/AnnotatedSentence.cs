/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

using System.Collections.Generic;

[System.Serializable]
public class AnnotatedSentence
{

	public string theSentence;

	public int languageArea;
	public int difficulty;

	public List<string> fillerWords;
	
	public AnnotatedSentence()
	{
		// Empty constructor required for JSON converter.
	}
	
	public AnnotatedSentence(string _sentence, List<string> _filler){ 
		theSentence = _sentence;
		fillerWords = _filler;
	}

	public AnnotatedSentence(string _sentence, List<string> _filler, int _languageArea,int _difficulty){
		difficulty=_difficulty;
		languageArea = _languageArea;
		theSentence = _sentence;
		fillerWords = _filler;
	}


}

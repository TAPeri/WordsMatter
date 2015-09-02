/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using UnityEngine;

using System.Collections.Generic;

[System.Serializable]
public class Word
{

	public string suffix;
	public string suffixType;
	public string word;
	public string wordUnmodified;
	public string type;
	public string[] syllables;
	public string cvform;
	public string phonetics;
	public string stem;
	public int numSyllables;
	public string languageCode;
	public double frequency;
	public List<GraphemePhonemePair> graphemesPhonemes;



	public Word()
	{
		// Empty constructor required for JSON converter.
	}
	
	public Word(string word)
	{

		word = eraseInvalidCharsInString(word);

		this.wordUnmodified = word.Trim();
		this.word = word.ToLower().Trim();
		// all initializations must go here!!!
		
		numSyllables = 0;
		frequency = 0;
		type = "Unknown";//WordType.Unknown;
		cvform = "";
		phonetics = "";
		stem = "";
		languageCode = null;
		graphemesPhonemes = null;
	}
	
	public Word(string word, string wt) // (string word, WordType wt)
	{
		word = eraseInvalidCharsInString(word);


		this.wordUnmodified = word.Trim();
		this.word = word.ToLower().Trim();
		// all initializations must go here!!!
		
		numSyllables = 0;
		frequency = 0;
		type = wt;
		cvform = "";
		phonetics = "";
		languageCode = null;
		graphemesPhonemes = new List<GraphemePhonemePair>();
	}

	public Word(string word, string wt,string[] sys) // (string word, WordType wt)
	{
		word = eraseInvalidCharsInString(word);
		
		this.wordUnmodified = word.Trim();
		this.word = word.ToLower().Trim();
		// all initializations must go here!!!
		
		numSyllables = sys.Length;
		frequency = 0;
		type = wt;
		syllables = sys;
		cvform = "";
		phonetics = "";
		languageCode = null;
		graphemesPhonemes = new List<GraphemePhonemePair>();
	}

	// GETTERS
	public string getSuffix() {return this.suffix;}
	public string getWord() { return word; }
	public string getWordUnmodified() { return wordUnmodified; }
	public string getType() {	return type; }
	public string[] getSyllables() { 
		string[] output = new string[syllables.Length]; 

		for (int i =0;i<output.Length;i++) {
			output[i] = syllables[i];
		}
		return output; 
	}
	public int getNumberOfSyllables() { return numSyllables; }
	public string getCVForm() { return cvform; }
	public string getStem() { return stem; }
	public string getPhonetics() { return phonetics; }
	public int getLength() { return word.Length; }
	public double getFrequency() { return frequency; }
	public string getLanguageCode() {	return languageCode; }
	public List<GraphemePhonemePair> getGraphemesPhonemes() { return graphemesPhonemes; }
	
	public string getWordInToSyllables()
	{	
		string res = "-";
		for (int i = 0; i < syllables.Length; i++)
		{
			string tmp = syllables[i].ToUpper();
			res = res + tmp;
			res = res + "-";
		}

		return res;
	}

	// SETTERS
	public void setType(string x) { this.type = x; }
	public void setStem(string stem) { this.stem = stem; }



	// OVERRIDES
	public override bool Equals (object obj)
	{
		Word w = (Word) obj;
		return w.getWord().Equals(this.word);
	}

	public override string ToString()
	{
		return word;
	}

	public override int GetHashCode()
	{
		return word.GetHashCode();
	}


	
	private string eliminateStartingSymbols()
	{
		int i = 0;
		while (i < word.Length
		       && (word[i] == '\'' || word[i] == '`' || word[i] == '΄')) 
		{
			i++;
		}
		return word.Substring(i);
	}
	

	public int compareTo(System.Object o)
	{
		Word w = (Word) o;

		return (this.eliminateStartingSymbols()).CompareTo(w.eliminateStartingSymbols());
	}


	private string eraseInvalidCharsInString(string para_originalStr)
	{
		char[] invalidCharArr = { '«','*','»','(',')','{','}','[',']','<','>','=','%','€','$' };

		string tmpStr = para_originalStr;
		for(int i=0; i<invalidCharArr.Length; i++)
		{
			tmpStr = tmpStr.Replace(""+invalidCharArr[i],"");
		}

		return tmpStr;
	}


	public List<int> getSyllSplitPositions()
	{
		Debug.LogWarning("This should have into account the pattern that we are learning, e.g. only one split for prefixing");
		List<int> list = new List<int>();
		int accumulated_position = 0;
		
		for(int i = 0; i<syllables.Length-1;i++){
			accumulated_position +=syllables[i].Length;
			list.Add(accumulated_position-1);
		}


		return list;
		//return syllable_positions;
	}



	public Word(string para_textRep, List<int> para_syllablePos)
	{

		word = para_textRep;
		syllables = new string[para_syllablePos.Count+1];
		int last_position = 0;
		for(int i=0;i<para_syllablePos.Count;i++){
			syllables[i] = word.Substring(last_position,para_syllablePos[i]-last_position+1);
			last_position = para_syllablePos[i]+1;
			
		}
		syllables[para_syllablePos.Count] = word.Substring(last_position);

		//syllable_positions = para_syllablePos;
	}
}

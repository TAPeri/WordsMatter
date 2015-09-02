/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 *///using System;
[System.Serializable]


public class Difficulty {


	public string[] descriptions;
	public string problemType;

	public string humanReadableDescription;
	public int cluster;
	public string character;


	// Use this for initialization
	public Difficulty(){
		
	}

	public Difficulty(string[] d,DifficultyType p){ descriptions = d; problemType = p.ToString();}


	public bool isDescription(string x){
		for (int i=0; i<descriptions.Length; i++){
			if (string.Compare(descriptions[i],x, true)==0)
				return true;
		}
		return false;
	}

	public string getDescriptionsToString(){
		string res ="";
		for (int i=0; i<descriptions.Length-1; i++){
			res = res + descriptions[i]+", ";
		}
		res = res + descriptions[descriptions.Length-1];
		return res;
	}

	public DifficultyType getDifficultyType(){return (DifficultyType)System.Enum.Parse(typeof(DifficultyType), problemType);}
	
}
/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using System.Collections.Generic;

[System.Serializable]



public class LiteracyProfile {

	// Use this for initialization
	public LiteracyProfile(){

	}

	public LiteracyProfile(string language){
		trickyWords = new List<Word>();
		userSeverities = new UserSeverities(language);
		problems = new DifficultiesDescription(language);
	}


	public DifficultiesDescription problems;
	public UserSeverities userSeverities;
	public List<Word> trickyWords;

	public DifficultiesDescription getDifficultiesDescription(){return problems;}
	public UserSeverities getUserSeveritites(){return userSeverities;}
	public List<Word> getTrickyWords(){return trickyWords;}

}

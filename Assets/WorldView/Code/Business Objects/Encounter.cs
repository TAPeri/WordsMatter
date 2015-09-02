/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

[System.Serializable]
public class Encounter
{
	ApplicationID location;
	string level;
	int languageArea;
	int difficulty;

	string details;
	LanguageCode lc;
	
	public Encounter(ApplicationID _location,
	                 string _level,
	                 int _languageArea,
	                 int _difficulty,
	                 string _details,
	                 LanguageCode _lc)
	{
		location = _location;
		level = _level;
		languageArea = _languageArea;
		difficulty = _difficulty;
		details = _details;
		lc = _lc;
	}

	public ApplicationID getLocation() { return location; }
	public string getLevel() { return level; }
	public int getLanguageArea() { return languageArea; }
	public int getDifficulty() { return difficulty; }
	public string getDetails() { return details; }
	public LanguageCode getLangCode() { return lc; }
}
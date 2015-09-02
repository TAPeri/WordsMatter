/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */[System.Serializable]


public class LanguageArea {

	public string severityType, URI;
	public LanguageAreaCategory type;

	// Use this for initialization
	public LanguageArea(){
		
	}

	public string getSeverityType(){return severityType;}
	public string getURI(){return URI;}
	public LanguageAreaCategory getType(){return type;}
	
}
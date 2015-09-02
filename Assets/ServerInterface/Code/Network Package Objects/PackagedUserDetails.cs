/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */[System.Serializable]


public class PackagedUserDetails{
	// Use this for initialization
	public int id;
	public string username;
	public string password;
	public bool enabled;
	public string gender;
	public long birthdate;
	public string language;
	//private LanguageCode language_enum;

	public PackagedUserDetails()
	{
		// Empty constructor required for JSON converter.
	}
	
	public PackagedUserDetails(int _id, string _u, string _pass, bool _e, string _gender, long _bi, string l){  
		id = _id;
		username = _u;
		password = _pass;
		enabled = _e;
		gender = _gender;
		birthdate = _bi;
		language = l;
	}

	public string getID(){
		return id.ToString();
	}

	public string getUsername(){
		return username;
	}

	public string getGender(){
		return gender;
	}

	public LanguageCode getLanguage(){

		return (LanguageCode)System.Enum.Parse(typeof(LanguageCode),language);
	}

	public long getBirthdate(){
		return birthdate;
	}

}

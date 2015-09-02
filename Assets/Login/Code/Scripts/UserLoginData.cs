/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

[System.Serializable]
public class UserLoginData {

	public string username;
	public string password;
	public string teacher_username;
	public string teacher_password;
	public LanguageCode language;


	public UserLoginData(string u,string p,string tu,string tp,LanguageCode lang){

		username = u;
		password = p;
		teacher_username = tu;
		teacher_password = tp;
		language = lang;
	}
}

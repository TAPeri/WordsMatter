/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
[System.Serializable]


public class PackagedUserProfile {

	public  LiteracyProfile userProblems;
	public  UserPreferences preferences;
	public  string language;
		
	public PackagedUserProfile()
		{
			// Empty constructor required for JSON converter.
		}


	public PackagedUserProfile(string language)
	{
		this.language = language;
		userProblems = new LiteracyProfile(language);
		preferences = new UserPreferences();
	}

	public void autocorrect(){


		UnityEngine.TextAsset ta = UnityEngine.Resources.Load<UnityEngine.TextAsset>("Localisation_Files/"+language+ "/CharacterOverwrite"+language);
		

		if (ta!=null){
			
			string text = ta.text;
			foreach(string line in text.Split('\n')){
				string[] values = line.Split(',');

				int lA = System.Convert.ToInt32(values[0]);
				int diff = System.Convert.ToInt32(values[1]);

				userProblems.getDifficultiesDescription().getDifficulties()[lA][diff].character = values[2];
				UnityEngine.Debug.LogWarning("Fix: "+line);
			}
			
		}else{
			
			UnityEngine.Debug.LogError("Localisation_Files/"+language+ "/CharacterOverwrite"+language);
		}


	}

		
	public PackagedUserProfile(LiteracyProfile u, UserPreferences p, LanguageCode l){userProblems = u; preferences = p; language = l.ToString();}

	public LiteracyProfile getLiteracyProfile(){return userProblems;}
	public UserPreferences getUserPreferences(){return preferences;}
	public LanguageCode getLanguage(){return (LanguageCode)System.Enum.Parse(typeof(LanguageCode), language);}

}

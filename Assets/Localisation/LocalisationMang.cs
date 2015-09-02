/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class LocalisationMang
{
	public static LanguageCode langCode = LanguageCode.GR;
	public static LocalisationSet loadedGenericFile;
	public static BranchStringCollection loadedBioFile;
	private static Dictionary<string,string> translations;



	// Initialisation. Load the generic file as default.
	public static void init(LanguageCode para_langCode)
	{
		langCode = para_langCode;
		loadedGenericFile = null;
		loadedGenericFile = loadLocalFromJsonFile(para_langCode);

		translations = new Dictionary<string,string>();
		
		TextAsset ta = Resources.Load<TextAsset>("Localisation_Files/EN2GR");
		if(ta!=null){
			string[] tmpSS = ta.text.Split('\n');
			
			foreach(string pair in tmpSS)
				if(para_langCode==LanguageCode.GR)
					translations.Add (pair.Split(',')[0], pair.Split(',')[1]);
			else
				translations.Add (pair.Split(',')[0], pair.Split(',')[0]);
			
		}
	}



	public static Sprite getIntroSlide(int currSlideID){

		return Resources.Load<Sprite>("Localisation_Files/"+langCode.ToString()+ "/IntroNarrativeSlides/Page" + currSlideID);
	}

	public static List<AudioClip> getIntroSound(int page,bool isMale){
		
		TextAsset ta = (TextAsset) Resources.Load("Localisation_Files/"+langCode.ToString()+"/IntroStory"+langCode.ToString(),typeof(TextAsset));
		List<AudioClip> listSounds = new List<AudioClip>();

		if(ta!=null){
			string text = ta.text;
			foreach(string line in text.Split('\n')){
				string[] values = line.Split(',');
				if(values[0].Equals("Page"+page)){

					for(int i = 1;i<values.Length;i++){
						AudioClip tmpClip = null;
						if(langCode==LanguageCode.EN){

							if(isMale){
								tmpClip = Resources.Load<AudioClip>("Localisation_Files/"+langCode.ToString()+"/IntroNarrativeRecordings/Boy/EthanIntro_"+values[i]);
							}else{
								tmpClip = Resources.Load<AudioClip>("Localisation_Files/"+langCode.ToString()+"/IntroNarrativeRecordings/Girl/LilyIntro_"+values[i]);

							}
						}else{

						}

						if(tmpClip!=null){
							listSounds.Add(tmpClip);
						}

					}

					return listSounds;
				}
			}
		}
		
		
		return listSounds;
	}



	public static string instructions(int languageArea,string level,ApplicationID appID){
		LevelParameters given = new LevelParameters(level);
		string instructions = "";

		TextAsset ta = (TextAsset) Resources.Load("Localisation_Files/"+langCode.ToString()+"/Instructions_"+langCode.ToString()+"_"+appID,typeof(TextAsset));
		if(ta!=null){
			string text = ta.text;
			foreach(string line in text.Split('\n')){
				string[] values = line.Split(',');
				int la = 	int.Parse(values[0]);
				//int mode = 	int.Parse(values[1]);


				LevelParameters a = new LevelParameters(values[3]);
					
				if(la==languageArea)	
				if((a.mode==given.mode)){
					instructions = values[4];
						if((a.ttsType==given.ttsType)){
						instructions = values[4];
							
							if (values[3].Equals(level))
								return values[4];
							
						}
				}
					
					

			}


		}


		return instructions;

	}

	static private List<string> notFound;

	public static string translate(string word){

		if(translations == null){
			Debug.Log("Offline translations");
			init(langCode);
		}

		if(translations.ContainsKey(word))
			return translations[word];
		else{
			if(notFound==null)
				notFound = new List<string>();
			if(!notFound.Contains(word)){
				UnityEngine.Debug.Log("Not found "+ word);
				notFound.Add(word);
			}
		}
		return word;

	}


	// **** SET 1: Generic Localisation File 1. ****

	public static string getAlphabet(string para_lowerOrUpper)
	{
		string fixedLOrU = para_lowerOrUpper.ToLower();

		if(fixedLOrU == "upper")
		{
			return loadedGenericFile.getAlphabetUpper();
		}
		else
		{
			return loadedGenericFile.getAlphabetLower();
		}
	}

	public static List<string> getKeyboardLayout(string para_baseType, string para_lowerOrUpper)
	{
		string fixedBType = para_baseType.ToLower();
		string fixedLOrU = para_lowerOrUpper.ToLower();

		if(loadedGenericFile == null)
		{
			init(LanguageCode.EN);
		}

		if((fixedBType == "tablet")&&(fixedLOrU == "lower"))
		{
			return loadedGenericFile.getKeyboardLayoutTabLower();
		}
		else if((fixedBType == "tablet")&&(fixedLOrU == "upper"))
		{
			return loadedGenericFile.getKeyboardLayoutTabUpper();
		}
		else if((fixedBType == "alpha")&&(fixedLOrU == "lower"))
		{
			return loadedGenericFile.getKeyboardLayoutAlphaLower();
		}
		else if((fixedBType == "alpha")&&(fixedLOrU == "upper"))
		{
			return loadedGenericFile.getKeyboardLayoutAlphaUpper();
		}
		else
		{
			return loadedGenericFile.getKeyboardLayoutTabLower();
		}
	}

	public static string getString(string para_key)
	{
		if(loadedGenericFile == null)
		{
			init(LanguageCode.EN);
		}

		return loadedGenericFile.getLocalisedString(para_key);
	}

	public static LocalisationSet loadLocalFromJsonFile(LanguageCode para_langID)
	{
		LocalisationSet retSet = null;

		string path = "Localisation_Files/"+System.Enum.GetName(typeof(LanguageCode),para_langID)+"/";

		string localFileName = "Local_EN";
		switch(para_langID)
		{
			case LanguageCode.EN: localFileName = "Local_EN"; break;
			case LanguageCode.GR: localFileName = "Local_GR"; break;
			default: localFileName = "Local_EN"; break;
		}
		path += localFileName;

		TextAsset ta = (TextAsset) Resources.Load(path,typeof(TextAsset));
		//string[] lines = ta.text.Split(new string[] { "\r\n" },System.StringSplitOptions.None);
		string jsonStr = ta.text;
		retSet = JsonHelper.deserialiseObject<LocalisationSet>(jsonStr);
		return retSet;
	}



	// **** SET 2: Bio Localisation File ****

	public static bool loadBioFile(int para_charID)
	{
		//BranchStringCollection testObj = new BranchStringCollection();
		//testObj.strBaseCollections.Add("MetaData",new BaseStringCollection(new Dictionary<string,string>() { {"CharID","0"},{"MaxBioSections","4"}

		bool successFlag = false;

		string fileName = "Bio_Char"+para_charID;
		string path = "Localisation_Files/"+System.Enum.GetName(typeof(LanguageCode),langCode)+"/Bio/";
		path+= fileName;

		try
		{
			TextAsset ta = (TextAsset) Resources.Load(path,typeof(TextAsset));

			string jsonStr = ta.text;
			loadedBioFile = JsonHelper.deserialiseObject<BranchStringCollection>(jsonStr);
			successFlag = true;
		}
		catch(System.Exception ex)
		{
			Debug.LogError(para_charID+" -> "+ex.Message);
		}

		return successFlag;
	}

	public static int getMaxOfBioSectionsForChar(int para_charID)
	{
		bool loadSuccess = true;
		if(needToLoadBioFile(para_charID)) { loadSuccess = loadBioFile(para_charID); }

		if(loadSuccess)
		{
			string searchKey = "MetaData*MaxBioSections";
			string maxAsStr = loadedBioFile.getString(0,searchKey.Split('*'));
			return int.Parse(maxAsStr);
		}
		else
		{
			return 0;
		}
	}


	/*public static string getLongBioString(int para_charID){

		bool loadSuccess = true;
		if(needToLoadBioFile(para_charID)) { loadSuccess = loadBioFile(para_charID); }
		
		if(loadSuccess)
		{
			Debug.Log();
			return loadedBioFile.getString(0,para_key.Split('*'));
		}
		else
		{
			return "NO BIO";
		}



		,"FullBioMap"
	}*/

	public static string getBioString(int para_charID, string para_key)
	{
		bool loadSuccess = true;
		if(needToLoadBioFile(para_charID)) { loadSuccess = loadBioFile(para_charID); }

		if(loadSuccess)
		{
			return loadedBioFile.getString(0,para_key.Split('*'));
		}
		else
		{
			return "NO BIO";
		}
	}

	public static bool needToLoadBioFile(int para_charID)
	{
		bool retFlag = false;

		if(loadedBioFile == null) { retFlag = true; }
		else
		{
			int loadedBioCharID = int.Parse(loadedBioFile.getString(0,new string[2] {"MetaData","CharID"}));
			if(loadedBioCharID != para_charID) 
			{
				retFlag = true;
			}
		}

		return retFlag;
	}

	public static List<string> getAllLoadedBioSections(int para_charID)
	{
		List<string> retSections = new List<string>();

		int counterLimit = 0;
		counterLimit = getMaxOfBioSectionsForChar(para_charID);
		
		for(int i=0; i<counterLimit; i++)
		{
			string tmpStr = LocalisationMang.getBioString(para_charID,"BioSections*"+i);
			retSections.Add(tmpStr);
		}

		return retSections;
	}


	public static string[] getBioSection(int para_charID,int sectionIndex){
		string[] longDescription = LocalisationMang.getFullExtensiveBio(para_charID).Split('\n');
		
		string title = longDescription[2+(sectionIndex*3)];
		string text = longDescription[3+(sectionIndex*3)].Replace("[]","");

		return new string[]{title,text};



	}

	public static string getFullExtensiveBio(int para_charID)
	{

		//if(langCode==LanguageCode.GR)
		//	return "?\n\n?:\n[] ?\n\n?:\n[] ?\n\n?:\n[] ?\n\n?:\n[] ?\n\n?:\n[] ?\n\n?:\n[] ?\n\n?:\n[] ?";
		string retStr = "";
		retStr = LocalisationMang.getBioString(para_charID,"FullBioMap*Full");
		return retStr;
	}





	public static List<string[]> getBothNPCnames(){
		
		TextAsset ta = Resources.Load<TextAsset>("Localisation_Files/"+langCode+"/NPC_"+langCode+"_mapping");
		
		
		List<string[]> names = new List<string[]>();
		
		if (ta!=null){
			
			string text = ta.text;
			foreach(string line in text.Split('\n')){
				string[] values = line.Split(',');
			//	Debug.Log(line);
				names.Add(new string[]{values[1],values[2]});

			}
			
		}else{
			
			Debug.LogError("NOT FOUND: Localisation_Files/"+langCode+"/NPC_"+langCode+"_mapping");
		}
		
		return names;
		
	}



	public static List<string> getNPCnames(){

		TextAsset ta = Resources.Load<TextAsset>("Localisation_Files/"+langCode+"/NPC_"+langCode+"_mapping");


		List<string> names = new List<string>();

		if (ta!=null){

			string text = ta.text;
			foreach(string line in text.Split('\n')){
				string[] values = line.Split(',');

				names.Add(values[1]);
			}

		}else{

			Debug.LogError("NOT FOUND: Localisation_Files/"+langCode+"/NPC_"+langCode+"_mapping");
		}

		return names;

	}


	public static ApplicationID getMainApplicationID(int npcID){

		//Debug.LogError("Mapping probably not correct");

		switch(npcID){
			case 0: return ApplicationID.SERENADE_HERO ;
			case 1: return ApplicationID.DROP_CHOPS ;
			case 2: return ApplicationID.MOVING_PATHWAYS ;
			case 3: return ApplicationID.HARVEST ;
			case 4: return ApplicationID.WHAK_A_MOLE ;
			case 5: return ApplicationID.MAIL_SORTER ;
			case 6: return ApplicationID.EYE_EXAM ;
			case 7: return ApplicationID.TRAIN_DISPATCHER ;
			case 8: return ApplicationID.ENDLESS_RUNNER ;
			default: return ApplicationID.APP_ID_NOT_SET_UP ;

		}

	}

	public static int getOwnerNpcOfActivity(ApplicationID appID){


		switch(appID){
			case ApplicationID.SERENADE_HERO: 	return 0 ;
			case ApplicationID.DROP_CHOPS: 		return 1 ;
			case ApplicationID.MOVING_PATHWAYS: return 2 ;
			case ApplicationID.HARVEST:			return 3;
			case ApplicationID.WHAK_A_MOLE:		return 4;
			case ApplicationID.MAIL_SORTER:		return 5;
			case ApplicationID.EYE_EXAM :		return 6;
			case ApplicationID.TRAIN_DISPATCHER:return 7;
			case ApplicationID.ENDLESS_RUNNER:	return 8;
			default: return -1 ;
			
		}


	}

	/*public static ApplicationID getMainActivityKeyForNpc(int charID){
		
		Debug.LogError("Mapping not implemented");
		return ApplicationID.SERENADE_HERO;
	}*/
	

	public static string getActivityShorthand(ApplicationID appID){

		switch(appID){
		case ApplicationID.SERENADE_HERO: return translate("Music hall");
		case ApplicationID.DROP_CHOPS: return translate("Junkyard");
		case ApplicationID.MOVING_PATHWAYS: return translate("Town square");
		case ApplicationID.HARVEST: return translate("Garden");
		case ApplicationID.WHAK_A_MOLE: return translate("Monkey hotel");
		case ApplicationID.MAIL_SORTER: return translate("Post office");
		case ApplicationID.EYE_EXAM: return translate("Bridge");
		case ApplicationID.TRAIN_DISPATCHER: return translate("Train station");
		case ApplicationID.ENDLESS_RUNNER: return translate("Bike shed");
		default: return "";


		}

	}


	public static List<ApplicationID> getAllApplicableActivitiesForLangArea(int langArea){
		
		
		List<ApplicationID> output = new List<ApplicationID>();
		
		foreach (ApplicationID appID in System.Enum.GetValues(typeof(ApplicationID)))
		{
			
			string levelDBFilePath = "Localisation_Files/"+langCode+"/Instructions_"+langCode+"_"+appID;
			
			TextAsset ta = (TextAsset) Resources.Load(levelDBFilePath,typeof(TextAsset));
			if(ta==null)
				continue;
			string text = ta.text;
			string[] lineArr = text.Split('\n');
			foreach(string line in lineArr)
			{	
				string[] values = line.Split(',');
				
				if(System.Convert.ToInt32(values[0]) == langArea)
				{
					
					output.Add(appID);
					break;
					
				}else if(System.Convert.ToInt32(values[0])> langArea){
					break;
				}
			}
			
			
		}
		
		return output;
		
	}

	
	
	public static ApplicationID getRandomApplicableActivityPKeyForLangArea(int lA,List<ApplicationID> exclude){

		List<ApplicationID> candidates = getAllApplicableActivitiesForLangArea(lA);

		foreach(ApplicationID app in exclude){
			if(candidates.Contains(app))
				candidates.Remove(app);
		}


		if(candidates.Count==0)
			return ApplicationID.APP_ID_NOT_SET_UP;

		int index = UnityEngine.Random.Range(0,candidates.Count);

		return candidates[index];
	}

	

	public static bool checkIfNpcIsMainChar(int charID){

		return charID<9;

	}





	public static AvailableOptions workOutOptionsForActivityAndLangArea(ApplicationID para_appID, int para_langArea)
	{
		
		//int appID = (int) para_appID;
		
		
		string levelDBFilePath = "Localisation_Files/"+langCode+"/Instructions_"+langCode+"_"+para_appID;

		Dictionary<string,List<int>> validKeysAndCodes = new Dictionary<string, List<int>>();
		List<string> listKeys = new List<string>();
		
		TextAsset ta = (TextAsset) Resources.Load(levelDBFilePath,typeof(TextAsset));

		if(ta==null){
			Debug.LogError("Localisation_Files/"+langCode+"/Instructions_"+langCode+"_"+para_appID);

		}
		string text = ta.text;
		string[] lineArr = text.Split('\n');
		foreach(string line in lineArr)
		{	
			string[] values = line.Split(',');
			
			// values[0] == application ID
			// values[1] == language area
			// values[3] == index
			// values[4] == default level string.
			

			if(System.Convert.ToInt32(values[0]) == para_langArea)
			{
				// Check for valid attributes.
				
				string lvlStr = values[3];
				string[] lvlStrParts = lvlStr.Split('-');
				
				for(int i=0; i<lvlStrParts.Length; i++)
				{
					string tmpPart = lvlStrParts[i];
					string charKey = ""+tmpPart[0];
					int numCode = int.Parse(tmpPart.Substring(1));
					
					if( ! validKeysAndCodes.ContainsKey(charKey))
					{
						validKeysAndCodes.Add(charKey,new List<int>());
						listKeys.Add (charKey);
					}
					
					if( ! (validKeysAndCodes[charKey].Contains(numCode)))
					{
						validKeysAndCodes[charKey].Add(numCode);
					}
				}
			}
		}
		
		
		// Sort items.
		List<string> tmpKeys = new List<string>(validKeysAndCodes.Keys);
		for(int i=0; i<tmpKeys.Count; i++)
		{
			validKeysAndCodes[tmpKeys[i]].Sort();
		}
		
		
		
		
		
		Dictionary<string,string[]> validKeysAndReadableOptions = new Dictionary<string, string[]>();
		Dictionary<string,string> validKeysAndTitles = new Dictionary<string, string>();
		
		foreach(string key in listKeys)
		{
			
			List<string> nxtReadableOptions = new List<string>();
			
			//if(wvServCom.language==LanguageCode.EN){
			// Special override for text to speech. (Displays Off and On instead of 0 and 1).
			if(key == "T")
			{
				if (validKeysAndCodes[key].Count==1)
					continue;
				
				validKeysAndTitles.Add(key,LocalisationMang.translate("Text-to-speech"));
				
				nxtReadableOptions.AddRange(new string[] {LocalisationMang.translate("Off"),LocalisationMang.translate("On")});
				for(int k=2; k<validKeysAndCodes[key].Count; k++)
				{
					nxtReadableOptions.Add("?");
				}
				validKeysAndReadableOptions.Add(key,nxtReadableOptions.ToArray());
				
			}
			else if((key == "A"))
			{
				if (validKeysAndCodes[key].Count==1)
					continue;
				
				
				string title = LocalisationMang.translate("Accuracy");
				
				if(para_appID==ApplicationID.MAIL_SORTER){
					title = LocalisationMang.translate("Packages");
				}else if(para_appID==ApplicationID.SERENADE_HERO){
					title = LocalisationMang.translate("Alternatives");
				}else if(para_appID==ApplicationID.WHAK_A_MOLE){
					title = LocalisationMang.translate("Distractors");
				}else if(para_appID==ApplicationID.MOVING_PATHWAYS){
					title = LocalisationMang.translate("Number of paths");
				}else if(para_appID==ApplicationID.ENDLESS_RUNNER){
					title = LocalisationMang.translate("Words per door");
				}
				
				validKeysAndTitles.Add(key,title);
				
				if(para_appID==ApplicationID.MAIL_SORTER){
					for(int k=0; k<validKeysAndCodes[key].Count; k++)
					{
						nxtReadableOptions.Add(""+(validKeysAndCodes[key][k]+1));
					}
					
				}else{
					
					for(int k=0; k<validKeysAndCodes[key].Count; k++)
					{
						nxtReadableOptions.Add(""+validKeysAndCodes[key][k]);
					}
					
				}
				validKeysAndReadableOptions.Add(key,nxtReadableOptions.ToArray());
				
				
			}else if(key == "S")
			{
				if (validKeysAndCodes[key].Count==1)
					continue;
				
				string title = LocalisationMang.translate("Speed");
				
				if(para_appID==ApplicationID.MAIL_SORTER){
					title = LocalisationMang.translate("Rotating speed");
				}else if(para_appID==ApplicationID.WHAK_A_MOLE){
					title = LocalisationMang.translate("Monkey speed");
				}else if(para_appID==ApplicationID.MOVING_PATHWAYS){
					title = LocalisationMang.translate("Square size");
				}else if(para_appID==ApplicationID.ENDLESS_RUNNER){
					title = LocalisationMang.translate("Monkey speed");
				}else if(para_appID==ApplicationID.DROP_CHOPS){
					title = LocalisationMang.translate("Split time");
				}else if(para_appID==ApplicationID.SERENADE_HERO){
					title = LocalisationMang.translate("Speed");
				}else if(para_appID==ApplicationID.TRAIN_DISPATCHER){
					title = LocalisationMang.translate("Attempts");
				}
				
				validKeysAndTitles.Add(key,title);

				if(para_appID==ApplicationID.DROP_CHOPS){//Invert the labels for solomon 
					for(int k=0; k<validKeysAndCodes[key].Count; k++)
					{
						nxtReadableOptions.Add(new string[]{LocalisationMang.translate("High"),LocalisationMang.translate("Medium"),LocalisationMang.translate("Low")}[validKeysAndCodes[key][k]]);
					}

				}else{
				for(int k=0; k<validKeysAndCodes[key].Count; k++)
				{
					nxtReadableOptions.Add(new string[]{LocalisationMang.translate("Low"),LocalisationMang.translate("Medium"),LocalisationMang.translate("High")}[validKeysAndCodes[key][k]]);
				}
				}
				validKeysAndReadableOptions.Add(key,nxtReadableOptions.ToArray());
				
				
			}else if(key == "M")
			{
				if (validKeysAndCodes[key].Count==1)
					continue;
				
				string title = LocalisationMang.translate("Mode");
				
				validKeysAndTitles.Add(key,title);
				
				if(langCode==LanguageCode.EN){
					
					if(para_langArea==6){
						nxtReadableOptions.Add("Letter to letter");
						nxtReadableOptions.Add("Letter to word");
						
					}else if(para_langArea==3){
						nxtReadableOptions.Add("Category");
						nxtReadableOptions.Add("Count");
						
					}else{
						
						for(int k=0; k<validKeysAndCodes[key].Count; k++)
						{
							nxtReadableOptions.Add(""+validKeysAndCodes[key][k]);
						}
					}
					
				}else{
					
					//if(para_appID==ApplicationID.MOVING_PATHWAYS){
					
					nxtReadableOptions.Add(LocalisationMang.translate("Letter to letter"));
					nxtReadableOptions.Add(LocalisationMang.translate("Letter to word"));
					
					//}else{
					
					
					//}
				}
				validKeysAndReadableOptions.Add(key,nxtReadableOptions.ToArray());
				
			}else if(key == "W")
			{
				
				validKeysAndTitles.Add(key,LocalisationMang.translate("Word difficulty"));
				
				if (validKeysAndCodes[key].Count==1){
					
					List<int> options = new List<int>();
					for(int k=0; k<3; k++)
					{
						nxtReadableOptions.Add(new string[]{LocalisationMang.translate("Easy"),LocalisationMang.translate("Medium"),LocalisationMang.translate("Hard")}[k]);
						options.Add((k+1)*3);
					}
					
					validKeysAndCodes[key] = options;
				}else{
					for(int k=0; k<validKeysAndCodes[key].Count; k++)
					{
						//Debug.Log(validKeysAndCodes[key][k]);
						nxtReadableOptions.Add(new string[]{LocalisationMang.translate("Easy"),"","","",LocalisationMang.translate("Medium"),"","","","",LocalisationMang.translate("Hard")}[validKeysAndCodes[key][k]]);
					}
					
				}
				validKeysAndReadableOptions.Add(key,nxtReadableOptions.ToArray());
				
				
				
			}else if(key == "B")
			{
				if (validKeysAndCodes[key].Count==1)
					continue;
				
				string title = LocalisationMang.translate("Number of words");
				
				if(para_appID==ApplicationID.MAIL_SORTER){
					title = LocalisationMang.translate("Number of rounds");
				}else if(para_appID==ApplicationID.MOVING_PATHWAYS){
					title = LocalisationMang.translate("Words per difficulty");
				}
				
				validKeysAndTitles.Add(key,title);
				
				if (validKeysAndCodes[key].Count==1){
					
					List<int> options = new List<int>();
					for(int k=1; k<10; k++)
					{
						nxtReadableOptions.Add(""+k);
						options.Add((k));
					}
					
					validKeysAndCodes[key] = options;
					
				}else{
					
					if(para_appID==ApplicationID.MAIL_SORTER){
						for(int k=0; k<validKeysAndCodes[key].Count; k++)
						{
							nxtReadableOptions.Add(""+validKeysAndCodes[key][k]/4);
						}
					}else{
						for(int k=0; k<validKeysAndCodes[key].Count; k++)
						{
							nxtReadableOptions.Add(""+validKeysAndCodes[key][k]);
						}
					}
					
				}
				validKeysAndReadableOptions.Add(key,nxtReadableOptions.ToArray());
				
				
			}else if(key=="X"){
				
				if (validKeysAndCodes[key].Count==1)
					continue;
				
				
				validKeysAndTitles.Add(key,LocalisationMang.translate("Tricky words"));
				for(int k=0; k<validKeysAndCodes[key].Count; k++)
				{
					nxtReadableOptions.Add(new string[]{"0/4","1/4","2/4","3/4"}[validKeysAndCodes[key][k]]);
				}
				validKeysAndReadableOptions.Add(key,nxtReadableOptions.ToArray());
				
			}else if(key=="D"){
				if (validKeysAndCodes[key].Count==1)
					continue;
				
				validKeysAndTitles.Add(key,LocalisationMang.translate("Distractors"));
				for(int k=0; k<validKeysAndCodes[key].Count; k++)
				{
					nxtReadableOptions.Add(new string[]{"0/4","1/4","2/4","3/4"}[validKeysAndCodes[key][k]]);
				}
				validKeysAndReadableOptions.Add(key,nxtReadableOptions.ToArray());
				
				
			}
			
			
			
		}
		
		
		// Construct the AvailableOptions data structure.
		AvailableOptions avOps = null;
		Dictionary<string,OptionInfo> opInfoLookup = new Dictionary<string, OptionInfo>();
		
		foreach(KeyValuePair<string,string[]> pair in  validKeysAndReadableOptions)
		{
			if( ! opInfoLookup.ContainsKey(pair.Key))
			{
				OptionInfo nxtOpInf = new OptionInfo(pair.Key,validKeysAndCodes[pair.Key],validKeysAndReadableOptions[pair.Key],validKeysAndTitles[pair.Key]);
				opInfoLookup.Add(pair.Key,nxtOpInf);
			}
		}
		
		avOps = new AvailableOptions(new List<string>(validKeysAndReadableOptions.Keys),opInfoLookup);
		return avOps;
	}


	public static string getFirstLevelConfiguration (ApplicationID appID, int languageArea,int difficulty){
		
		
		//Debug.LogWarning("Still in use?");
		string levelDBFilePath = "";

		levelDBFilePath = "Localisation_Files/"+langCode+"/Instructions_"+langCode+"_"+appID; 
			TextAsset ta = (TextAsset) Resources.Load(levelDBFilePath,typeof(TextAsset));
			string text = ta.text;
			string[] lineArr = text.Split('\n');
			
			
			foreach(string line in lineArr){
				
				string[] values = line.Split(',');
				
				
				if(System.Convert.ToInt32(values[0])==languageArea){
					
					return values[3];
					
				}
			}
			
		return "";
			
		//TextAsset ta = (TextAsset) Resources.Load("Localisation_Files/"+language.ToString()+"/AllLevels_"+language.ToString(),typeof(TextAsset));
		
		
	}


}

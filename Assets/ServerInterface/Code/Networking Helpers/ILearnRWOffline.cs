/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Net;
//using System.Text;
//using System.IO;
//using System;

public class ILearnRWOffline : MonoBehaviour,IServerServices
{
	

	List<UserLogNoTimestamp> logs = new List<UserLogNoTimestamp>();
	
	//private string student_name = "";
	//private string userID = "";

	public string language = "EN";

	string app;
	//int number_words;
	//int difficultyID;
	//int languageArea;
	//int evaluation_mode;
	//string challenge;
	ApplicationID appID;
	string[] last_tags_requested = new string[0];

	bool dual = false;

	public GetRoutine requestLevel(string language,ApplicationID appID, int languageArea,int difficulty, int index){

		this.appID = appID;
		return new ConnectionError("",RoutineStatus.READY);
	}

	public string loadLevel(){

		switch(appID){

		case ApplicationID.MAIL_SORTER: return "M1-A2-S0-B5-W0-F3-T0";	//MS
		case ApplicationID.WHAK_A_MOLE: return "M2-A10-S0-B10-W0-F3-T0";//WAM
		case ApplicationID.ENDLESS_RUNNER: return "M0-A0-S2-B5-W1-F0-T0";	//PD
		case ApplicationID.HARVEST: return "M0-A0-S0-B5-W1-F1-T0";	//HARVEST
		case ApplicationID.SERENADE_HERO: return "M0-A3-S0-B10-W0-F3-T0";	//SH
		case ApplicationID.MOVING_PATHWAYS: return "M1-A1-S0-B10-W0-F3-T0";	//MP
		case ApplicationID.EYE_EXAM: return "M0-A3-S0-B10-W1-F0-T0";	//BB
		case ApplicationID.TRAIN_DISPATCHER: return "M0-A0-S0-B5-W1-F0-T0";	//TD
		case ApplicationID.DROP_CHOPS: return "M0-A2-S0-B10-W0-F0-T1";	//SJ
		}

		return "M0-A0-F0-W0-T0-F0-B0";
	}
	
	public PackagedServerInfo getServerInfo(){
		
		return new PackagedServerInfo(0);
	}
	

	
	public PackagedUserProfile getUserProfile(){


		TextAsset ta = (TextAsset) Resources.Load("Localisation_Files/EN/Offline_profile_EN",typeof(TextAsset));
		
		return JsonHelper.deserialiseObject<PackagedUserProfile>(ta.text);
		



		//return new PackagedUserProfile(language);
	}
	
	public PackagedUserDetails getUserDetails(){
		
		return new PackagedUserDetails(14, "joe_t", "", true, "male", 0000001, language);//System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")
	}
	
	public PackagedUserLogs getUserLogs(){
		
		List<UserLog> list = new List<UserLog>();
		foreach(UserLogNoTimestamp ul in logs){
			foreach(string t in last_tags_requested){
				
				if (ul.getTag().ToString()==t){
					list.Add(null);
				}
			}
		}
		
		return new PackagedUserLogs(1, list.ToArray(),1 );
	}

	public PackagedUserLogs getNewsFeed(){
		return new PackagedUserLogs(1,new UserLog[0],1);
	}
	
	public List<PackagedNextActivity> getNextActivity(){
		
		List<PackagedNextActivity> list = new List<PackagedNextActivity>();
		Debug.LogError("Wrong levels!");
		float rand = UnityEngine.Random.Range(0.0f,1.0f);
		
		if (rand<0.3){
			list.Add(  new PackagedNextActivity(new string[]{"DROP_CHOPS","SERENADE_HERO","EYE_EXAM"}, 0 , 0, new string[]{"M0-A2-S0-B5-W0-F3-T0","M0-A2-S0-B5-W0-F3-T0","M0-A2-S0-B5-W0-F3-T0"} )   );
		}else if(rand < 0.6){
			list.Add(  new PackagedNextActivity(new string[]{"DROP_CHOPS","SERENADE_HERO","EYE_EXAM"}, 0 , 0, new string[]{"M0-A2-S0-B5-W0-F3-T0","M0-A2-S0-B5-W0-F3-T0","M0-A2-S0-B5-W0-F3-T0"})   );
			
		}else{
			list.Add(  new PackagedNextActivity(new string[]{"DROP_CHOPS","SERENADE_HERO","EYE_EXAM"}, 0 , 0, new string[]{"M0-A2-S0-B5-W0-F3-T0","M0-A2-S0-B5-W0-F3-T0","M0-A2-S0-B5-W0-F3-T0"} )   );
			
		}
		
		return list;
		
	}
	public List<PackagedNextWord> getNextWords(){


		string file = "Localisation_Files/EN/Offline_";

		switch(app){
		case "DROP_CHOPS": file+="SJ_EN";break;

		case "SERENADE_HERO": file+="SH_EN";break;

		case "MAIL_SORTER": file+="MS_EN";break;

		case "MOVING_PATHWAYS": file+="MP_EN";break;

		case "WHAK_A_MOLE": file+="WAM_EN";break;

		case "HARVEST": file+="HARVEST_EN";break;

		case "TRAIN_DISPATCHER": file+="TD_EN";break;

		case "EYE_EXAM": file+="BB_EN";break;

		case "ENDLESS_RUNNER": file+="PD_EN";break;

		default: Debug.Log("Hard Coded for " +app+" not available");break;


	}

		
		TextAsset ta = (TextAsset) Resources.Load(file,typeof(TextAsset));
		
		return JsonHelper.deserialiseObject<List<PackagedNextWord>>(ta.text);





	}

	public bool connectedWithTeacher(){
		return dual;
	}

	public GetRoutine requestServerVersion(){
		return ready ();
	}

	
	public GetRoutine ready(){
		return new ConnectionError("Offline mode, no need to wait",RoutineStatus.READY);
	}
	
	public GetRoutine requestAuthentication(string _student_name,string student_pass){
		dual = false;
		//student_name = _student_name;
		return ready ();
	}
	
	public GetRoutine requestAuthentication(string _student_name,string student_pass,string teacher_name,string teacher_pass){
		dual = true;
		//student_name = _student_name;
		return ready ();
	}
	
	public GetRoutine requestRefreshAuthentication(){
		return ready ();
	}
	
	public GetRoutine requestUserProfile(string _userID){
		//userID = _userID;
		return ready ();
	}
	
	public GetRoutine requestUserDetails(){
		return ready ();
	}
	
	public GetRoutine requestNextActivity(string userID){
		return ready ();
	}

	public GetRoutine requestNextActivity(string userID,int languageArea,int difficulty){
		
		return ready ();

	}

	public GetRoutine requestNextActivity(string userID,string character){
		
		return ready ();
		
	}
	public GetRoutine requestNextActivity(string userID,string character,string game){
		
		return ready ();
		
	}
	
	public GetRoutine requestNextActivity(string userID,int languageArea,int difficulty,string game){
		
		return ready ();
	}


	public GetRoutine requestNextWords(string _app, int _number_words,int _difficultyID,int _languageArea,string _userID,int _evaluation_mode,string _challenge){

		Debug.Log("Words requested for "+_app);
		app = _app; 
		//number_words = _number_words; 
		//difficultyID = _difficultyID;
		//languageArea = _languageArea;
		//userID = _userID;
		//evaluation_mode = _evaluation_mode;
		//challenge = _challenge;

		return ready();
	}
	
	
	
	
	public GetRoutine requestUserLogs(
		string para_timestart,
		string para_timeend,
		int para_page,
		string[] para_tags,
		string para_applicationID,
		string para_sessionID){
		last_tags_requested = para_tags;
		return ready ();
	}


	public GetRoutine requestNewsFeed(
		string para_timestart,
		string para_timeend,
		int para_page,
		string[] para_tags,
		string para_applicationID,
		string para_sessionID){
		last_tags_requested = para_tags;
		return ready ();
	}
	public GetRoutine errorHandler(GetRoutine error_message){
		
		return error_message;
		
	}
	
	public void logData(UserLogNoTimestamp log){
		logs.Add(log);
		return;
		
	}
	
	public GetRoutine flushLogs(){
		return ready();
	}
	
	public GetRoutine requestProfileUpdate(int difficulty, int languageArea,string userID){
		return ready ();
	}
	
	public List<PackagedProfileUpdate> getProfileUpdate(){
		return new List<PackagedProfileUpdate>();
	}



	public GetRoutine requestSavefile(){
		return ready();
	}

	public PackagedUserLogs getSaveFiles(){
		return null;
	}

	
}

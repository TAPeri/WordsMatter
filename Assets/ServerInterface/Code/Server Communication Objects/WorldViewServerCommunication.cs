/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WorldViewServerCommunication : GameServerCommunication {
	
	
	public GetRoutine saved_data_query;
	public GetRoutine profile_query;
	static public PackagedUserProfile userProfile = null;
	


	public static bool error = false;
	public static string errorMessage = "";

	public static  void setError(string message){
		error = true;

		if(!errorMessage.Contains(message))
			errorMessage += message+"\n\n";

		Debug.LogError(message);
	}

	public static void clearError(){
		error = false;
		errorMessage = "";
	}


	private GetRoutine next_activity_query = new ConnectionError("Activity not requested",RoutineStatus.IDLE);
	
	
	private string game_state;
	
	


	GetRoutine saveRoutine;

	public void saveProgress(string game_state){
		
		Debug.Log("Save game state!");
		UserLogNoTimestamp log = basicLog(Tag.SAVEFILE);
		log.setValue(game_state);
		server.logData(log);
		saveRoutine = server.flushLogs();
		
	}
	
	
	
	private string processSaveFile(UserLog[] logs){
		
		//PackagedUserLogs pul = server.getUserLogs();
		
		//UserLog[] logs = pul.getLogs();
		
		//string game_state = "";
		
		if (logs.Length>0){
			
			/*NEWif (logs[0].getValue()!=null){
				if (logs[0].getValue()!=null){
					Debug.Log("Goooood");
				Debug.Log(logs[0].getValue());
				return logs[0].getValue();
			}*/



			Debug.Log("Retrieved "+logs.Length+" save files");

			if (logs[0].getValue()!=null){
				//if (logs[logs.Length-1].getValue()!=null){
				Debug.Log("Save file processed: "+logs[0].getUsername()+" "+logs[0].getTimestamp()+" "+logs[0].getTimestampRaw());
				//return logs[logs.Length-1].getValue();
				return logs[0].getValue();


			}
		}else{
			return "";
		}
		return "";


	}
	
	
	
	
	public void startActivity(int difficulty,int category, ApplicationID activity,string level){
		
		UserLogNoTimestamp log = new UserLogNoTimestamp(username,activity,Tag.APP_SESSION_START);
		log.setDifficulty(difficulty,category);
		log.setMode(Mode.ADVENTURE);
		log.setLevel(level);
		
		server.logData(log);
		
	}
	
	public void endActivity(int difficulty,int category, ApplicationID activity,string level){
		
		UserLogNoTimestamp log = new UserLogNoTimestamp(username,activity,Tag.APP_SESSION_END);
		log.setDifficulty(difficulty,category);
		log.setMode(Mode.ADVENTURE);
		log.setLevel(level);
		
		server.logData(log);
		
	}
	
	
	
	public void activityProposed(int difficulty,int category, ApplicationID activity,string level){

	//	Debug.Log("ACTIVITY PROPOSED: "+activity+" "+level);

		UserLogNoTimestamp log = new UserLogNoTimestamp(username,activity,Tag.ACTIVITY_PROPOSED);
		log.setDifficulty(difficulty,category);
		log.setMode(Mode.PLAY);
		log.setLevel(level);
		
		server.logData(log);
		
	}
	
	
	
	public GetRoutine requestSuggestion(){
		
		next_activity_query = server.requestNextActivity(userID);
		return next_activity_query;
		
	}
	

	public GetRoutine requestSuggestion(int languageArea,int difficulty){
		
		next_activity_query = server.requestNextActivity(userID, languageArea, difficulty);
		return next_activity_query;
		
	}

	public GetRoutine requestSuggestion(int languageArea,int difficulty,ApplicationID game){
		
		next_activity_query = server.requestNextActivity(userID, languageArea, difficulty,game.ToString());
		return next_activity_query;
		
	}
	public GetRoutine requestSuggestion(string character){
		
		next_activity_query = server.requestNextActivity(userID, character);
		return next_activity_query;
		
	}
	public GetRoutine requestSuggestion(string character,ApplicationID game){
		
		next_activity_query = server.requestNextActivity(userID, character,game.ToString());
		return next_activity_query;
		
	}





	
	public List<PackagedNextActivity> loadDifficultyAndActivity(){
		
		return server.getNextActivity();
		
	}
	
	
	public GetRoutine requestLevel(ApplicationID appID, int languageArea,int difficulty, int index){
		
		return server.requestLevel( userProfile.language, appID,  languageArea, difficulty,  index);
		
	}




	/*public string getPathToStaticLevelFile()
	{
		string levelDBFilePath = "";
		bool isOffline = false;
		if(server != null) { if((server is ILearnRWServer)) 	{ isOffline = false;	} else { isOffline = true;	} }
		else { isOffline = true; }
		
		if(isOffline) { levelDBFilePath = "Localisation_Files/"+language.ToString()+"/OfflineAllLevels"+language.ToString(); }
		else { levelDBFilePath = "Localisation_Files/"+language.ToString()+"/Instructions_"+language.ToString(); }

		return levelDBFilePath;
	}*/
	
	public string loadLevel(){
		
		return server.loadLevel();
	}
	



	GetRoutine newsfeed_query;

	public GetRoutine requestNewsfeed(int page){

		newsfeed_query = server.requestNewsFeed(
		null,
		null,
		page,
		new string[]{Tag.APP_ROUND_SESSION_END.ToString()} ,
		null,
		null);

		return newsfeed_query;

	}


	public UserLog[] getNewsfeed(){

		PackagedUserLogs pul = server.getNewsFeed();
		
		return pul.getLogs();

	}

	
	void Update(){
		
		if (loading_status.status()==RoutineStatus.ERROR){
			
			Debug.LogError(loading_status.getError());
			return;
			
		}
		
		switch(state){
			
		case 0://Loading user details
			
			state = loadingUserDetails();
			break;		
			
		case 1:
			
			Debug.Log("Request user profile");
			profile_query = server.requestUserProfile(userID);
			state = 2;
			break;

			
		case 2:
			
			if(profile_query.status()== RoutineStatus.READY){
				Debug.Log("Get user profile");

				userProfile = server.getUserProfile();


				if (Application.platform == RuntimePlatform.Android){
					tts = gameObject.AddComponent<TTS_android>();
				}else{
					tts = gameObject.AddComponent<TTS_googletranslate>();
				}
				
				tts.init(9,9,language.ToString());
				
				Debug.Log("Request save logs");
				
				saved_data_query = server.requestSavefile();

				state = 3;
				break;


			}else if(profile_query.status()== RoutineStatus.ERROR){
				
				Debug.LogError(profile_query.getError());
				state = 1;
				//userProfile = new PackagedUserProfile();

				break;
				
			}else{
				break;
			}
			

			
		case 3:
			
			if(saved_data_query.status() == RoutineStatus.READY){
				
				PackagedUserLogs pul = server.getSaveFiles();
				//OLD//PackagedUserLogs pul = server.getUserLogs();

				int number_pages = pul.getTotalPages();
				Debug.Log("Get save logs: "+pul.page+"/"+number_pages);
				if (number_pages>0){
					/*saved_data_query = server.requestUserLogs(
						null,
						null,
						number_pages,
						new string[]{Tag.SAVEFILE.ToString()} ,
					ApplicationID.GAME_WORLD.ToString(),
					null);//Request last page
					*/

					game_state = processSaveFile(pul.getLogs());
					loading_status = new ConnectionError("Savefile loaded",RoutineStatus.READY);

					state = 5;
					
				}else{
					
					game_state = "";
					loading_status = new ConnectionError("New savefile",RoutineStatus.READY);
					state = 5;
					
					
				}
				
				
			}else if(saved_data_query.status() == RoutineStatus.ERROR){
				
				loading_status = new ConnectionError("Couldn't load first page of save file. "+saved_data_query.getError(),RoutineStatus.ERROR);
				//state = -1;
				state = 5;
				Debug.Log("Safenet?");
				loading_status = new ConnectionError("Could not load first page",RoutineStatus.READY);
				
			}
			
			break;

		}


		if (saveRoutine!=null){

			if(saveRoutine.status()==RoutineStatus.READY){


				List<int> result = ((GetRoutine<List<int>>)saveRoutine).getPackage();

				string output = "Flush success!";

				foreach(int a in result)
					output+=", "+a;

				Debug.Log(output);



			}else if(saveRoutine.status()==RoutineStatus.ERROR){

				Debug.Log("Flush error!");
				saveRoutine = null;
			}
		}

	}
	
	static public TTSinterface tts;
	
	
	public string loadSavefile(){
		return game_state;
	}
	
}

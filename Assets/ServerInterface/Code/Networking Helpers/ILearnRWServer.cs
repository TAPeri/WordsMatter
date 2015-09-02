/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Net;
//using System.Text;
//using System.IO;
//using System;

public class ILearnRWServer : MonoBehaviour,IServerServices
{
	WWW www;
	
	string auth_username = "api";
	string auth_password = "api";
	public static string server_url = "https://ssl.ilearnrw.eu/ilearnrw";
	//public static string server_url = "http://localhost:8080/test";

	public static bool debugging = false;
	//public static bool debugging = true;
	
	string student_name = "";
	string teacher_name = "";
	
	GetRoutine connectionStatus = null;
	
	private PackagedAuthentication authentication;
	
	private GetRoutine<PackagedServerInfo> serverInfoService;
	
	private GetRoutine<PackagedAuthentication> authenticationService;
	private GetRoutine<PackagedAuthentication> refreshService;
	private GetRoutine<PackagedUserDetails> userDetailsService;
	private GetRoutine<PackagedUserProfile> userProfileService;
	private GetRoutine<PackagedUserLogs> userLogsService;
	private GetRoutine<PackagedUserLogs> newsFeedService;

	private GetRoutine<PackagedUserLogs> savefileService;
	
	//private GetRoutine<List<int>> postUserLogsService;
	//private GetRoutine<UserLog> postUserLogsService;


	
	private GetRoutine<List<PackagedNextActivity>> nextActivityService;
	private GetRoutine<List<PackagedNextWord>> nextWordsService;
	
	private GetRoutine<List<PackagedProfileUpdate>> profileUpdateService;

	private GetRoutine<LevelJSON> levelService;

	public bool connectedWithTeacher(){
		return dual;
	}


	public string loadLevel(){
		return levelService.getPackage().level;
	}



	public PackagedUserProfile getUserProfile(){

		PackagedUserProfile aux = userProfileService.getPackage();

		aux.autocorrect();

		return aux;
	}
	
	public PackagedServerInfo getServerInfo(){
		
		return serverInfoService.getPackage();
	}
	
	public PackagedUserDetails getUserDetails(){
		
		return userDetailsService.getPackage();
	}
	
	public PackagedUserLogs getUserLogs(){
		
		return userLogsService.getPackage();
	}

	public PackagedUserLogs getNewsFeed(){
		
		return newsFeedService.getPackage();
	}

	public PackagedUserLogs getSaveFiles(){
		
		return savefileService.getPackage();
	}


	public List<PackagedNextActivity> getNextActivity(){
		
		return nextActivityService.getPackage();
		
	}
	
	public List<PackagedNextWord> getNextWords(){
		
		List<PackagedNextWord> list = nextWordsService.getPackage();
		return list;
		
	}
	
	static List<string> requests;
	static List<string> responses;





	Vector2 scrollPosition = Vector2.zero;
	void OnGUI(){

		if(debugging){

			//windowRect = GUI.Window(2, windowRect, window, "Debugger");
			string text1 = "";
			
			//int count = 5;
			for(int i=requests.Count-1;i>-1;i--){
				
			//	if(count==0)break;
				if (i<responses.Count)
					text1+=i+":"+responses[i].Replace("\n","")+"\n";
				else
					text1+=i+": ... waiting response ...\n";		

				text1+=i+":"+requests[i].Replace("\n","")+"\n\n";

				
			}
			int window_width = 20000;
			int window_height = 5000;

			scrollPosition = GUI.BeginScrollView (new Rect (0f,0.0f,Screen.width,Screen.height*0.3f),scrollPosition, new Rect (0, 0, window_width, window_height));  

			GUI.Label(new Rect(0,0,window_width,window_height),text1);
			
			GUI.EndScrollView ();  
		}


	}

	//public ILearnRWServer(){
	void Awake(){
		Debug.Log("Server Awake");
		if (debugging){
			if(requests==null)
				requests = new List<string>();
			if(responses==null)
				responses = new List<string>();

		}
		
		authenticationService = new GetRoutine<PackagedAuthentication>(server_url + "/user/auth",requests,responses);
		refreshService = new GetRoutine<PackagedAuthentication>(server_url + "/user/newtokens",requests,responses);
		userDetailsService = new GetRoutine<PackagedUserDetails>(server_url + "/user/details",requests,responses);
		userProfileService = new GetRoutine<PackagedUserProfile>(server_url + "/profile",requests,responses);
		
		userLogsService = new GetRoutine<PackagedUserLogs>(server_url + "/lastLogs",requests,responses);
		newsFeedService = new GetRoutine<PackagedUserLogs>(server_url + "/lastLogs",requests,responses);
		//		userLogsService = new GetRoutine<PackagedUserLogs>(server_url + "/logs",requests,responses);
		savefileService = new GetRoutine<PackagedUserLogs>(server_url + "/lastLogs",requests,responses);

		//postUserLogsService= new GetRoutine<UserLog>(server_url + "/logs");
		//postUserLogsService= new GetRoutine<List<int>>(server_url + "/logs_array",requests,responses);

		nextActivityService = new GetRoutine<List<PackagedNextActivity>>(server_url + "/activity/next_UoM",requests,responses);
		nextWordsService = new GetRoutine<List<PackagedNextWord>>(server_url + "/activity/new_data_UoM",requests,responses);
		levelService = new GetRoutine<LevelJSON>(server_url+"/activity/next_test/",requests,responses);

		
		profileUpdateService = new GetRoutine<List<PackagedProfileUpdate>>(server_url+"/profile/update",requests,responses);
		
		serverInfoService = new GetRoutine<PackagedServerInfo>(server_url+"/info/version/",requests,responses);


		logs = new List<UserLogNoTimestamp>();
		
	}
	
	
	
	
	private bool validateCredentials(){
		
		if  (authenticationService.status()==RoutineStatus.READY){
			authentication = authenticationService.getPackage();
		}else if(refreshService.status()==RoutineStatus.READY){
			authentication = refreshService.getPackage();
		}else if(authenticationService.status()==RoutineStatus.ERROR){
			int errorCode =0;
			try{
				errorCode = System.Convert.ToInt32(authenticationService.getError().Split(' ')[0]);
			}catch{};

			if (errorCode==403){//Wrong password
				connectionStatus = new ConnectionError(errorCode+" Wrong username or password",RoutineStatus.ERROR);
				return false;
			}else{
				connectionStatus = new ConnectionError(errorCode+" Unexpected connection error",RoutineStatus.ERROR);
				return false;
			}
		}else if(refreshService.status()==RoutineStatus.ERROR){
			int errorCode =0;
			try{
				errorCode = System.Convert.ToInt32(authenticationService.getError().Split(' ')[0]);
			}catch{};			if (errorCode==403){//Wrong refresh token
				connectionStatus = new ConnectionError(errorCode+" Wrong refresh token; authenticate again",RoutineStatus.ERROR);
				return false;
			}else{
				connectionStatus = new ConnectionError(errorCode+" Unexpected connection error",RoutineStatus.ERROR);
				return false;
			}
		}else if(authentication==null){
			connectionStatus = new ConnectionError("400 Authenticate token not available; authenticate",RoutineStatus.ERROR);
			return false;
		}
		connectionStatus = new ConnectionError("0 Connection is ok",RoutineStatus.READY);;
		return true;
		
	}
	
	private Hashtable buildHeader()
	{
		Hashtable headers = new Hashtable();
		headers.Add("Authorization","Basic "+System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(auth_username+":"+auth_password)));
		headers.Add("content-type","application/json");
		return headers;
	}
	
	

	bool dual = false;

	public GetRoutine requestAuthentication(string _student_name,string student_pass){
		Debug.Log("Request authentication");
		dual = false;
		student_name = _student_name;
		StartCoroutine(authenticationService.get("?username="+student_name+"&pass="+student_pass));
		return authenticationService;
	}
	
	public GetRoutine requestAuthentication(string _student_name,string student_pass,string _teacher_name,string teacher_pass){
		Debug.Log("Request authentication");

		dual = true;

		student_name = _student_name;
		teacher_name = _teacher_name;
		StartCoroutine(authenticationService.get("?username="+student_name+"&pass="+student_pass+"&teacher="+teacher_name+"&teacher_pass="+teacher_pass));
		return authenticationService;
	}
	
	public GetRoutine requestRefreshAuthentication(){
		Debug.Log("Request refresh");

		if (validateCredentials()){
			StartCoroutine(refreshService.get("?refresh="+authentication.getRefresh()));
			return refreshService;
		}else{
			return connectionStatus;
		}
	}
	

	public GetRoutine requestLevel(string language,ApplicationID appID, int languageArea,int difficulty, int index){
	
		if (validateCredentials()){
			StartCoroutine(levelService.get("?language="+ language +"&languageArea="+languageArea+"&difficulty="+difficulty+"&index="+index+"&appID="+(int)appID+"&token="+authentication.getAuth()));
			return levelService;
		}else{
			return connectionStatus;
		}

	}


	public GetRoutine requestUserDetails(){
		if (validateCredentials()){
			StartCoroutine(userDetailsService.get("/"+student_name+"/?token="+authentication.getAuth()));
			return userDetailsService;
		}else{
			return connectionStatus;
		}
	}
	
	public GetRoutine requestServerVersion(){
		//if (validateCredentials()){
		StartCoroutine(serverInfoService.get(""));///?token="+authentication.getAuth()));
			return serverInfoService;
		//}else{
		//	return connectionStatus;
		//}
	}
	
	public GetRoutine requestUserProfile(string userID){
		
		if (validateCredentials()){
			StartCoroutine(userProfileService.get("?userId="+userID+"&token="+authentication.getAuth()));
			return userProfileService;
		}else{
			return connectionStatus;
		}
		
	}
	
	public GetRoutine requestNextActivity(string userID){
		
		if (validateCredentials()){
			StartCoroutine(nextActivityService.get("?userId="+userID+"&token="+authentication.getAuth()));
			return nextActivityService;
		}else{
			return connectionStatus;
		}
		
	}



	public GetRoutine requestNextActivity(string userID,int languageArea,int difficulty){
		
		if (validateCredentials()){
			StartCoroutine(nextActivityService.get("?userId="+userID+"&languageArea="+languageArea+"&difficulty="+difficulty+"&token="+authentication.getAuth()));
			return nextActivityService;
		}else{
			return connectionStatus;
		}
		
	}
	
	public GetRoutine requestNextActivity(string userID,int languageArea,int difficulty,string game){
		
		if (validateCredentials()){
			StartCoroutine(nextActivityService.get("?userId="+userID+"&languageArea="+languageArea+"&difficulty="+difficulty+"&game="+game+"&token="+authentication.getAuth()));
			return nextActivityService;
		}else{
			return connectionStatus;
		}
		
	}

	public GetRoutine requestNextActivity(string userID,string character){
		
		if (validateCredentials()){
			StartCoroutine(nextActivityService.get("?userId="+userID+"&character="+character.Replace(" ","%20")+"&token="+authentication.getAuth()));
			return nextActivityService;
		}else{
			return connectionStatus;
		}
		
	}

	public GetRoutine requestNextActivity(string userID,string character,string game){
		
		if (validateCredentials()){
			StartCoroutine(nextActivityService.get("?userId="+userID+"&character="+character.Replace(" ","%20")+"&game="+game+"&token="+authentication.getAuth()));
			return nextActivityService;
		}else{
			return connectionStatus;
		}
		
	}

	
	public GetRoutine requestNextWords(string app, int number_words,int difficultyID,int languageArea,string userID,int evaluation_mode,string challenge){
		
		if (validateCredentials()){
			
			StartCoroutine(nextWordsService.get("?token="+authentication.getAuth()+
			                                    "&count="+number_words+
			                                    "&evaluation_mode="+evaluation_mode+
			                                    "&probId="+languageArea+"_"+difficultyID+
			                                    "&difficultyLevel="+challenge+
			                                    "&userId="+userID+
			                                    "&activity="+app)
			               //           "&activity="+app.ToString())
			               );
			return nextWordsService;
		}else{
			return connectionStatus;
		}
		
	}


	public GetRoutine requestSavefile(){
		if (validateCredentials()){

			string parameters = "/"+student_name+"?tags=SAVEFILE&token="+authentication.getAuth();

			StartCoroutine(savefileService.get(parameters));
		
			return savefileService;

		}else{
			return connectionStatus;
		}
	}

	
	public GetRoutine requestUserLogs(
		string para_timestart,
		string para_timeend,
		int para_page,
		string[] para_tags,
		string para_applicationID,
		string para_sessionID){
		
		if (validateCredentials()){
			string parameters = "";
			if (para_sessionID != null){
				parameters = "/session/"+para_sessionID;
			}else{
				parameters = "/"+student_name+"?";
				
				if(para_timestart != null) 
					parameters += "timestart="+para_timestart+"&";
				if(para_timeend != null) 
					parameters += "timeend="+para_timeend+"&";
				if(para_page >= 0) 
					parameters += "page="+para_page+"&";
				if(para_applicationID != null)
					parameters += "applicationId="+para_applicationID+"&";
				if(para_tags != null) {
					parameters += "tags="+string.Join (";",para_tags)+"&";
					/*string concatStr = "";
				for(int i=0; i<para_tags.Count; i++)
				{
					concatStr += para_tags[i];
					
					if(i < (para_tags.Count-1))
					{
						concatStr += ";";	
					}
				}*/
				}
			}
			
			parameters += "token="+authentication.getAuth();
			
			
			StartCoroutine(userLogsService.get(parameters));
			
			return userLogsService;
			
		}else{
			return connectionStatus;
		}
		
	}


	public GetRoutine requestNewsFeed(
		string para_timestart,
		string para_timeend,
		int para_page,
		string[] para_tags,
		string para_applicationID,
		string para_sessionID){
		
		if (validateCredentials()){
			string parameters = "";
			if (para_sessionID != null){
				parameters = "/session/"+para_sessionID;
			}else{
				parameters = "/"+student_name+"?";
				
				if(para_timestart != null) 
					parameters += "timestart="+para_timestart+"&";
				if(para_timeend != null) 
					parameters += "timeend="+para_timeend+"&";
				if(para_page >= 0) 
					parameters += "page="+para_page+"&";
				if(para_applicationID != null)
					parameters += "applicationId="+para_applicationID+"&";
				if(para_tags != null) {
					parameters += "tags="+string.Join (";",para_tags)+"&";
					/*string concatStr = "";
				for(int i=0; i<para_tags.Count; i++)
				{
					concatStr += para_tags[i];
					
					if(i < (para_tags.Count-1))
					{
						concatStr += ";";	
					}
				}*/
				}
			}
			
			parameters += "token="+authentication.getAuth();
			
			
			StartCoroutine(newsFeedService.get(parameters));
			
			return newsFeedService;
			
		}else{
			return connectionStatus;
		}
		
	}
	
	
	List<UserLogNoTimestamp> logs; 
	
	public void logData(UserLogNoTimestamp log){

		log.setSupervisor(teacher_name);
		logs.Add(log);
		
		if (logs.Count>20)
			flushLogs();
		
		
	}
	
	public GetRoutine flushLogs(){

		if (validateCredentials()){
			//Debug.Log(JsonHelper.serialiseObject<List<UserLog>>(logs));
			//StartCoroutine(userLogsService.post("token="+authentication.getAuth(),JsonHelper.serialiseObject<List<UserLog>>(logs)));

			if (logs.Count==0)
				return null;//nothing to do here

			GetRoutine<List<int>> individualCall = new GetRoutine<List<int>>(server_url + "/logs_array",requests,responses);

			StartCoroutine(individualCall.post("?token="+authentication.getAuth(),JsonHelper.serialiseObject<List<UserLogNoTimestamp>>(logs)));



			/*foreach(UserLogNoTimestamp ul in logs){
				Debug.Log("Flush: "+ul.word+" "+ul.problem_category+" "+ul.problem_index+" "+ul.getTag()+" "+ul.getUsername());

			}*/

			logs = new List<UserLogNoTimestamp>();
			return individualCall;
		}else{
			return connectionStatus;
		}
		
	}


	bool profileUpdateCollected = true;
	List<PackagedProfileUpdate> extraUpdates;
	
	public GetRoutine requestProfileUpdate(int difficulty,int languageArea,string userID){


		if(!profileUpdateCollected){

			//An update was requested but never collected, typically during a replay
			if(profileUpdateService.status()==RoutineStatus.READY){

				if(extraUpdates==null)
					extraUpdates = new List<PackagedProfileUpdate>();

				List<PackagedProfileUpdate> newUpdates = profileUpdateService.getPackage();
				foreach(PackagedProfileUpdate update in newUpdates){
					extraUpdates.Add(update);
				}
				profileUpdateCollected = true;


			}else{

				Debug.LogError("Too late, the previous profile update will be ignored");
			}
		}
		
		if (validateCredentials()){
			//Debug.Log(JsonHelper.serialiseObject<List<UserLog>>(logs));
			//StartCoroutine(userLogsService.post("token="+authentication.getAuth(),JsonHelper.serialiseObject<List<UserLog>>(logs)));
			
			//			StartCoroutine(profileUpdateService.get("?userId="+userID+
			//			                                         "&category="+languageArea+
			//			                                         "&index="+difficulty+
			//			                                         "&token="+authentication.getAuth()));
			
			StartCoroutine(profileUpdateService.post("?userId="+userID+
			                                         "&category="+languageArea+
			                                         "&index="+difficulty+
			                                         "&token="+authentication.getAuth(),"{userID:"+userID+"}"));



			profileUpdateCollected = false;

			return profileUpdateService;
		}else{
			return connectionStatus;
		}
		
		
	}


	public List<PackagedProfileUpdate> getProfileUpdate(){


		List<PackagedProfileUpdate> newUpdates = profileUpdateService.getPackage();

		if(extraUpdates!=null)		
			foreach(PackagedProfileUpdate update in extraUpdates)
				newUpdates.Add(update);

		extraUpdates = null;
		profileUpdateCollected = true;

		return newUpdates;
		
	}
	
	
	
	
	public GetRoutine errorHandler(GetRoutine error){
		
		
		if (error.getError().Contains("401")){
			//Need to refresh token
			return requestRefreshAuthentication();
			
		}else if (error.getError().Contains("403")){
			//Need to refresh token
			return new ConnectionError("Wrong username or password",RoutineStatus.ERROR);
			
		}else if (error.getError().Contains("Could not resolve host" )||(error.getError().Contains("couldn't connect to host" ))){
			
			return new ConnectionError("No internet connection",RoutineStatus.ERROR);
			
		}else{
			
			return error;
		}
		
	}
	
	
	public IEnumerator ping()
	{
		
		www = new WWW(server_url+"/ping",null,buildHeader());
		yield return www;
		
		if(www.error == null){
			Debug.Log("Ping succeded! "+www.text);
		}else{
			Debug.LogError("Ping failed: "+www.error);		
		}	
	}
	
	
}
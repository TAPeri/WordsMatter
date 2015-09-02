/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class GameServerCommunication : MonoBehaviour {



	public LanguageCode language = LanguageCode.EN;
	
	public IServerServices server;
	
	protected GetRoutine user_details_query;	

	public int getStatus(){
		return state;
	}

	protected string username;	
	static public string userID;

	protected ApplicationID appID = ApplicationID.APP_ID_NOT_SET_UP;

	protected int state = 0;

	public string getUsername(){
		
		return username;
	}

	public string getUserID(){

		return userID;
	}
	public void Awake() {

		GameObject poRef = PersistentObjMang.getInstance();
		Transform serverTrans = poRef.transform.Find("Server");
		GameObject server_object = null;
		if(serverTrans != null){
			server_object = serverTrans.gameObject;
		}

		
		if (server_object == null){

			Debug.Log("Server object not instanciated; Offline session started");

			GameObject nwS = new GameObject();
			nwS.name = "Server";
			nwS.transform.parent = poRef.transform;
			server = nwS.AddComponent<ILearnRWOffline>();
			server_object = nwS;

		}else{


			server = (IServerServices)server_object.GetComponent(typeof(IServerServices));
		}


	}


	
	public void setUserDetails(string un,string ID,LanguageCode l){
		username = un;
		userID = ID;
		language = l;
		state = 1;
		loading_status = new ConnectionError("Data assigned",RoutineStatus.IDLE);
	}



	protected int loadingUserDetails(){


		if(user_details_query == null){
			Debug.Log("Requesting user details");
			user_details_query = server.requestUserDetails();

		}

		if (user_details_query.status()==RoutineStatus.READY){

			Debug.Log("Get user details");

			PackagedUserDetails pud = server.getUserDetails();

			username = pud.getUsername();
			userID = pud.getID();

			language = pud.getLanguage();

			LocalisationMang.init(language);





			return state+1;

		}else if( user_details_query.status()==RoutineStatus.ERROR ){

			Debug.LogError("Processing error: "+user_details_query.getError());

			GetRoutine  server_response = server.errorHandler(user_details_query);


			loading_status = server_response;
			return -1;
		
		}else{
			return state;
		}

	}



	protected 	UserLogNoTimestamp basicLog(Tag tag){
		return new UserLogNoTimestamp(username,ApplicationID.GAME_WORLD,tag);
	}



	protected 	UserLogNoTimestamp basicLog(Tag tag, ApplicationID app){
		return new UserLogNoTimestamp(username,app,tag);
	}


	protected GetRoutine loading_status = new ConnectionError("Loading",RoutineStatus.WAIT);

	public GetRoutine Loading(){

			return loading_status;

	}
	




}

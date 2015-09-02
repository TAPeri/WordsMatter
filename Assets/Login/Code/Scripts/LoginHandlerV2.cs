/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;

public class LoginHandlerV2 : ILearnRWUIElement {
	

	private GetRoutine connection = null;
	private GetRoutine connectionTest = null;
	
	private GameObject poRef;
	private GameObject server;
	
	private IServerServices server_script;
	private IServerServices connection_test_script;
	
	//bool loading = false;
	bool connectingInProgressFlag = false;
	//bool connectedFlag = false;
	//bool askedForSaveFile = false;
	//bool loadedSaveFlag = false;
	
	int loginPageID = 0;

	int selectedOption = 0;
	int prevSelOption = 0;
	//string[] optionTitles = new string[]{"",""};
	//string[] optionDescriptions = new string[] {"Singleplayer mode selected","Player & Teacher mode selected"};
	Texture2D[] optionTextures;

	string error = "";

	GUIStyle btnStyle;
	GUIStyle flagLabelStyle;
	GUIStyle selectionGridStyle;
	GUIStyle sectionTitleStyle;
	GUIStyle buttonOkStyle;
	GUIStyle fieldTitleStyle;
	GUIStyle opDescStyle;
	GUIStyle textFieldStyle;
	GUIStyle passFieldStyle;
	new bool hasInitGUIStyles = false;

	WorldViewServerCommunication wvServCom;
	ActivityServerCommunication acServCom; 

	UserLoginData userLoginData;

	string obbError = "";

	void Start()
	{
		gameObject.AddComponent<AudioSource>();

		if(File.Exists(Application.persistentDataPath + "/username.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/username.gd", FileMode.Open);
			try{
				userLoginData = (UserLoginData)bf.Deserialize(file);
			}catch(System.Exception ex){
				Debug.LogError(ex.Message);
				userLoginData = new UserLoginData("SmartEnglish","test","englishteacher","test",LanguageCode.EN);
			}
			file.Close();
		}else{

			userLoginData = new UserLoginData("SmartEnglish","test","englishteacher","test",LanguageCode.EN);
			//userLoginData = new UserLoginData("SmartGreek","SmartGreek","GreekTeacher2","password");

		}

		LocalisationMang.langCode = userLoginData.language;

		poRef = PersistentObjMang.getInstance();
		
		connectionTest=new ConnectionError("",RoutineStatus.IDLE);
		//testConnection();

		prepUIElements();

		downloader(true);

	}
	

	//bool fetching = false;
	TTSinterface tts;


	private void downloader(bool first){


		#if UNITY_ANDROID && !UNITY_EDITOR
		
		if(GooglePlayDownloader.RunningOnAndroid())//Only on Android
		{
			if(first)
				new GooglePlayDownloader();

			string expPath = GooglePlayDownloader.GetExpansionFilePath();
			if (expPath == null)
			{
				
				obbError = LocalisationMang.translate("Could not download OBB file: external storage is not available");
				connection = null;
				return;
				
			}
			else
			{
				string mainPath = GooglePlayDownloader.GetMainOBBPath(expPath);
				string patchPath = GooglePlayDownloader.GetPatchOBBPath(expPath);
				
				if (mainPath == null || patchPath == null){
					/*if(!fetching){
								fetching = true;
								GooglePlayDownloader.FetchOBB();
							}*/
					return;
				}
			}
		}
		#endif

	}



	private void  initGUI(){

		float targetWidth = 1280f;
		float targetHeight = 800f;
		Vector3 scaleForCurrRes = new Vector3((Screen.width * 1.0f)/targetWidth,(Screen.height * 1.0f)/targetHeight,1f);
		
		Texture2D clearTex = new Texture2D(1,1);
		clearTex.SetPixel(0,0,ColorUtils.convertColor(0,0,0,20));
		clearTex.Apply();
		
		btnStyle = new GUIStyle(GUI.skin.button);
		btnStyle.fontSize = (int) (40 * scaleForCurrRes.x);
		
		flagLabelStyle = new GUIStyle(GUI.skin.label);
		flagLabelStyle.fontSize = (int) (40 * scaleForCurrRes.x);
		flagLabelStyle.alignment = TextAnchor.LowerCenter;
		
		opDescStyle = new GUIStyle(GUI.skin.label);
		opDescStyle.fontSize = (int) (30 * scaleForCurrRes.x);
		opDescStyle.alignment = TextAnchor.MiddleLeft;
		//opDescStyle.alignment = TextAnchor.MiddleCenter;
		
		sectionTitleStyle = new GUIStyle(GUI.skin.label);
		sectionTitleStyle.fontSize = (int) (30 * scaleForCurrRes.x);
		sectionTitleStyle.alignment = TextAnchor.MiddleLeft;
		sectionTitleStyle.normal.textColor = Color.red;
		//sectionTitleStyle.bold.textColor = Color.red;
		//sectionTitleStyle.alignment = TextAnchor.MiddleCenter;
		
		
		buttonOkStyle = new GUIStyle(GUI.skin.label);
		buttonOkStyle.fontSize = (int) (30 * scaleForCurrRes.x);
		buttonOkStyle.alignment = TextAnchor.MiddleCenter;
		
		fieldTitleStyle = new GUIStyle(GUI.skin.label);
		fieldTitleStyle.fontSize = (int) (30 * scaleForCurrRes.x);
		fieldTitleStyle.alignment = TextAnchor.MiddleLeft;
		
		selectionGridStyle = new GUIStyle(GUI.skin.button);
		selectionGridStyle.normal.background = clearTex;
		selectionGridStyle.normal.textColor = Color.black;
		
		textFieldStyle = new GUIStyle(GUI.skin.textField);
		textFieldStyle.fontSize = (int) (30 * scaleForCurrRes.x);
		
		passFieldStyle = new GUIStyle(GUI.skin.textField);
		passFieldStyle.fontSize = (int) (30 * scaleForCurrRes.x);
		
		
		hasInitGUIStyles = true;

	}

	void OnGUI()
	{

		/*if(GUI.Button(new Rect(10,10,50,50),"TTS")){


			if(tts==null){
			if (Application.platform == RuntimePlatform.Android){
				tts = gameObject.AddComponent<TTS_android>();
			}else{
				tts = gameObject.AddComponent<TTS_googletranslate>();
			}
			
			tts.init(9,9,LocalisationMang.langCode.ToString());
			
			tts.fetch(new string[]{"dyslexia"});

			}
			AudioClip word = tts.say("dyslexia");

			if(word==null)
				error = "TTS didnt work";
			else
				audio.PlayOneShot(word);

		}*/


		if( ! hasInitGUIStyles)
		{
			initGUI();
		}


		//By default if connectionTest is disabled
		if (connectionTest.status()==RoutineStatus.IDLE){

			if(loginPageID == 0)
			{

				GUI.color = Color.white;

				int sectionCount = (selectedOption+1);

				for(int i=0; i<sectionCount; i++)
				{
					string sectionSuffix = "-S"+i;

					string sectionTitleName = "SectionTitle"+sectionSuffix;
					string sectionUsernameTitleName = "UsernameTitle"+sectionSuffix;
					string sectionUsernameFieldName = "UsernameField"+sectionSuffix;
					string sectionPasswordTitleName = "PasswordTitle"+sectionSuffix;
					string sectionPasswordFieldName = "PasswordField"+sectionSuffix;

					GUI.color = Color.red;
					if (i==0)
						if(error!="")
							GUI.Label(uiBounds[sectionTitleName],LocalisationMang.translate(error),sectionTitleStyle);
						else if(obbError!="")
							GUI.Label(uiBounds[sectionTitleName],LocalisationMang.translate(obbError),sectionTitleStyle);

					GUI.color = Color.black;

					GUI.Label(uiBounds[sectionUsernameTitleName],textContent[sectionUsernameTitleName],fieldTitleStyle);
					GUI.Label(uiBounds[sectionPasswordTitleName],textContent[sectionPasswordTitleName],fieldTitleStyle);
					GUI.color = Color.white;


					if(i == 0)
					{
						// Student.
						userLoginData.username = GUI.TextField(uiBounds[sectionUsernameFieldName],userLoginData.username,25,textFieldStyle);
						userLoginData.password = GUI.PasswordField(uiBounds[sectionPasswordFieldName],userLoginData.password,'*',25,passFieldStyle);
					}
					else
					{
						// Teacher
						userLoginData.teacher_username = GUI.TextField(uiBounds[sectionUsernameFieldName],userLoginData.teacher_username,25,textFieldStyle);
						userLoginData.teacher_password = GUI.PasswordField(uiBounds[sectionPasswordFieldName],userLoginData.teacher_password,'*',25,passFieldStyle);
					}
				}



				if( ! connectingInProgressFlag)
				{
					selectedOption = GUI.SelectionGrid(uiBounds["LoginOptionButtonsArea"],selectedOption,optionTextures,2,selectionGridStyle);
					
					if((prevSelOption != selectedOption))
					{
						if(selectedOption == 0)
						{
							transform.FindChild("StudentLoginArea").gameObject.SetActive(true);
							transform.FindChild("TeacherLoginArea").gameObject.SetActive(false);
						}
						else
						{
							transform.FindChild("StudentLoginArea").gameObject.SetActive(true);
							transform.FindChild("TeacherLoginArea").gameObject.SetActive(true);
						}
						
						prevSelOption = selectedOption;
					}else{
						selectedOption = prevSelOption;
					}


					GUI.color = Color.black;
					GUI.color = Color.clear;

					if(GUI.Button(uiBounds["DoneButtonTop"],""))
					{
						triggerSoundAtCamera("BubbleClick",1f,false);
						// Trigger login procedures.
						if(selectedOption == 1)
						{
							if(((userLoginData.teacher_username != "")&&(userLoginData.teacher_password != ""))&&
							((userLoginData.username != "")&&(userLoginData.password != "")))
							{
								setUpOnlineServerDual();
							}
						}
						else
						{
							if((userLoginData.username != "")&&(userLoginData.password != ""))
							{
								setUpOnlineServer();
							}
						}

						setConnectButtonVisibility(false);
					}
					GUI.color = Color.white;
				}else{

					if((error=="")&&(obbError=="")){
						if(wvServCom!=null){
							switch(wvServCom.getStatus()){

								case 0: message = "Checking username and password"; break;
								case 1: message = "Loading user details"; break;
								case 2: message = "Waiting for user profile"; break;
								case 3: message = "Waiting for saved data"; break;
								case 4: message = "..."; break;
								//case 5: message = "Server has sent all the data"; break;

							}

						}

						GUI.Label(uiBounds["SectionTitle-S0"],LocalisationMang.translate(message),sectionTitleStyle);
						//GUI.Label(uiBounds["SectionTitle-S1"],LocalisationMang.translate(message),sectionTitleStyle);
					}

				}

				GUI.color = Color.white;
			}


		}else{//USed only when testConnection is active
			
			GUI.Label(new Rect(20,220,500,20), "Testing connection!");
			
			if (connectionTest.status()==RoutineStatus.ERROR){
				GUI.Label(new Rect(20,280,500,20), "Connection Error: ");
				GUI.Label(new Rect(20,320,600,20), connectionTest.getError());
				if(GUI.Button(new Rect(20,360,80,40), "Continue")) {
					Destroy((Component)connection_test_script);
					connectionTest = new ConnectionError("Ignore error for now", RoutineStatus.IDLE);
				}
				
			}else if(connectionTest.status()==RoutineStatus.READY){
				GUI.Label(new Rect(20,280,500,20), "Server version is: "+connection_test_script.getServerInfo().version);
				Destroy((Component)connection_test_script);
			}
			
		}
	}

	string message = "Checking username and password";


	//int status = 0;
	void Update(){

		if (connection!=null){
			//server_script
			if((connection.status()==RoutineStatus.READY ) ){//||(connection.status()==RoutineStatus.IDLE)

				UserLogNoTimestamp log = new UserLogNoTimestamp(userLoginData.username,ApplicationID.LOGGING_SCREEN,Tag.LOGIN);
				log.setValue("Connected");
				server_script.logData(log);//Log successfull log in//also change the status of the connection to IDLE//Maybe too late
				connection = new ConnectionError("Connected",RoutineStatus.IDLE);
				wvServCom =  server.AddComponent<WorldViewServerCommunication>();//This script will launch on Awake a request User Details
				acServCom = server.AddComponent<ActivityServerCommunication>();



			}else if(connection.status()==RoutineStatus.ERROR){
				
				connectingInProgressFlag = false;

				setConnectButtonVisibility(true);
				
				GetRoutine  server_response = server_script.errorHandler(connection);
				
				UserLogNoTimestamp log = new UserLogNoTimestamp(userLoginData.username,ApplicationID.LOGGING_SCREEN,Tag.LOGIN);
				log.setValue(server_response.getError());
				
				server_script.logData(log);//Log fail log in
				
				error = server_response.getError();
				connection = null;
				return;
			}else if(connection.status()==RoutineStatus.WAIT){
				//GUI.Label(new Rect(20,220,500,20), "Connecting...");
				return;
			}
		}else
			return;


		downloader(false);

		if(wvServCom.Loading().status()==RoutineStatus.READY){

			message = "Loading saved data";
			SaveFileManager.loadSaveFromPlayerDownload(wvServCom.loadSavefile(),wvServCom.language,WorldViewServerCommunication.userProfile.getLiteracyProfile());//status changes to IDLE

			acServCom.setUserDetails(wvServCom.getUsername(),wvServCom.getUserID(),wvServCom.language);

			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create (Application.persistentDataPath + "/username.gd");
			userLoginData.language = LocalisationMang.langCode;
			bf.Serialize(file, userLoginData);
			file.Close();
			
			Debug.LogWarning("Removed the flip between Milton and Monica");

			WorldViewServerCommunication.clearError();

			Application.LoadLevel("PlayerCustomisationScene");


		}else if(wvServCom.Loading().status()==RoutineStatus.WAIT){
			return;
		}else if(wvServCom.Loading().status()==RoutineStatus.ERROR){

			error = wvServCom.Loading().getError();
			connectingInProgressFlag = false;
			
			//loading = false;
			//askedForSaveFile = false;
			//loadedSaveFlag = false;
			setConnectButtonVisibility(true);
			connection = null;
		}


	}
	
	
	void testConnection(){
		setUp();
		connection_test_script = server.AddComponent<ILearnRWServer>();
		connectionTest = connection_test_script.requestServerVersion();
		
		
	}

	void setUp(){
		
		// 		   "Server" object is now a child of the "PersistentObj" in order to keep only one object which caters for all persistence.
		//		   Always access the persistent object by calling PersistentObjMang.getInstance().
		
		if (server!=null){
			server.transform.parent = null;
			DestroyImmediate(server);
		}
		
		GameObject nwS = new GameObject();
		nwS.name = "Server";
		nwS.transform.parent = poRef.transform;
		
		server = nwS;
	}
	
	void setUpOnlineServer(){
		setUp();
		server_script = server.AddComponent<ILearnRWServer>();
		connection = server_script.requestAuthentication(userLoginData.username,userLoginData.password);
	}
	
	void setUpOnlineServerDual(){
		setUp();
		server_script = server.AddComponent<ILearnRWServer>();
		connection = server_script.requestAuthentication(userLoginData.username,userLoginData.password,userLoginData.teacher_username,userLoginData.teacher_password);
	}

	
	void setUpOfflineServer(){
		setUp();
		server_script = server.AddComponent<ILearnRWOffline>();
		ILearnRWOffline castVersion = (ILearnRWOffline) server_script;
		castVersion.language = LocalisationMang.langCode.ToString();
		connection = server_script.requestAuthentication(userLoginData.username,userLoginData.password);
		
	}
	
	void setUpOfflineServerDual(){
		setUp();
		server_script = server.AddComponent<ILearnRWOffline>();
		connection = server_script.requestAuthentication(userLoginData.username,userLoginData.password,userLoginData.teacher_username,userLoginData.teacher_password);
		
	}
	
	void OnDestroy(){
		
		if(server_script!=null)
			server_script.flushLogs();
		
	}

	private void prepUIElements()
	{
		optionTextures = new Texture2D[2];
		optionTextures[0] = Resources.Load<Texture2D>("Textures/singleplayericon");
		optionTextures[1] = Resources.Load<Texture2D>("Textures/dualloginicon");

		// Init text items.
		string[] elementNames = new string[]{"LoginOptionButtonsArea","LoginOptionButtonsDescription"};//,"DoneButton"
		string[] elementContent = new string[]{"LoginOptionButtonsArea","LoginOptionButtonsDescription"};//,"Ok"
		bool[] destroyGuideArr = new bool[]{true,true};//,false};
		int[] textElementTypeArr = new int[]{0,0};//,0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);

		elementNames = new string[]{"DoneButtonTop","DoneButtonShadow"};//,"DoneButton"
		elementContent = new string[]{"DoneButtonTop","DoneButtonShadow"};//,"Ok"
		destroyGuideArr = new bool[]{false,false};//,false};
		textElementTypeArr = new int[]{0,0};//,0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,"DoneButton");


		// Init section 0 text items.
		elementNames = new string[]{"SectionTitle-S0","UsernameTitle-S0","UsernameField-S0","PasswordTitle-S0","PasswordField-S0"};
		elementContent = new string[]{LocalisationMang.translate("Student"),LocalisationMang.translate("Student username"),"Username Field",LocalisationMang.translate("Student password"),"Password Field"};
		destroyGuideArr = new bool[]{true,true,true,true,true};
		textElementTypeArr = new int[]{0,0,0,0,0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,"StudentLoginArea");

		// Init section 1 text items.
		elementNames = new string[]{"SectionTitle-S1","UsernameTitle-S1","UsernameField-S1","PasswordTitle-S1","PasswordField-S1"};
		elementContent = new string[]{LocalisationMang.translate("Teacher"),LocalisationMang.translate("Teacher username"),"Username Field",LocalisationMang.translate("Teacher password"),"Password Field"};
		destroyGuideArr = new bool[]{true,true,true,true,true};
		textElementTypeArr = new int[]{0,0,0,0,0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,"TeacherLoginArea");

		transform.FindChild("StudentLoginArea").gameObject.SetActive(true);
		transform.FindChild("TeacherLoginArea").gameObject.SetActive(false);
	}

	private void setConnectButtonVisibility(bool para_visState)
	{
		connectingInProgressFlag = ! para_visState;
		transform.FindChild("DoneButton").gameObject.SetActive(para_visState);
		transform.FindChild("ProcessWaitSmallIcon").gameObject.SetActive(!para_visState);
	}


	Transform sfxPrefab = null;

	private void triggerSoundAtCamera(string para_soundFileName, float para_volume, bool para_loop)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		GameObject potentialOldSfx = GameObject.Find(para_soundFileName);
		if(potentialOldSfx != null) { Destroy(potentialOldSfx); }
		
		if(sfxPrefab == null) { sfxPrefab = Resources.Load<Transform>("Prefabs/SFxBox"); }
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		nwSFX.name = para_soundFileName;
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = para_volume;
		audS.loop = para_loop;
		if(para_loop) { Destroy(nwSFX.GetComponent<DestroyAfterTime>()); }
		audS.Play();
	}


}

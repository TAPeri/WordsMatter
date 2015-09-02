/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

using UnityEngine;
using System.Collections.Generic;


/**
 * 
 * This class controls basic state of the game
 * It is initialised by the WorldViewScript and added to the persistent object
 * 
 **/

public class ProgressScript : MonoBehaviour, CustomActionListener
{
	bool[] upAxisArr = new bool[3] { false, true, false };

	WorldViewScript worldView = null;
	GhostBookDisplay gbDisp = null;
	GhostbookManagerLight gbMang = null;

	GetRoutine suggestionStatus = new ConnectionError("",RoutineStatus.IDLE);
	GetRoutine newsfeedDownloadStatus = new ConnectionError("",RoutineStatus.IDLE);

	int loading = 0;

	WorldViewServerCommunication wvServCom = null;
	ActivityServerCommunication acServCom = null; 



	Transform questBar;
	Rect questBarUiBounds;
	Rect questBarAbortBtnUiBounds;
	bool showQuestBar = false;
	
	//bool showNarrativeIcon = false;
	//Rect narrativeIconUIBounds;
	//Transform narrativeIcon;

	Transform ghostbookIcon;
	bool showGBIcon = false;
	Rect GBIconUIBounds;
	Rect PauseIconUIBounds;
	bool GBOpen = false;
	Texture2D closeGB;

	bool unlockingCharacters = false;
	bool unlockingLocations = false;
	bool unlockingDifficulty = false;

	Mode launcherMode;
	string launcherDetails;
	EventSlot selectedEvent;
	

	private GameObject getServerObject()
	{
		GameObject poRef = PersistentObjMang.getInstance();
		Transform serverTrans = poRef.transform.Find("Server");
		GameObject server_object = null;
		if(serverTrans != null){
			server_object = serverTrans.gameObject;
		}

		if(server_object == null)
		{			
			Debug.LogError("Start Offline version");
			GameObject nwS = new GameObject();
			nwS.name = "Server";
			nwS.transform.parent = poRef.transform;
			nwS.AddComponent<ILearnRWOffline>();
			server_object = nwS;	
		}
		
		return server_object;
	}

	


	void Awake()	{

		Debug.Log("ProgressScript is Awake");

		GameObject servComObj = getServerObject();


		wvServCom = servComObj.GetComponent<WorldViewServerCommunication>();
		acServCom = servComObj.GetComponent<ActivityServerCommunication>();


		if((wvServCom == null)||(acServCom==null))
		{
			Debug.LogError("Something went wrong; WorldViewServerCommunication should be ready");
			wvServCom = servComObj.AddComponent<WorldViewServerCommunication>();
			acServCom = servComObj.AddComponent<ActivityServerCommunication>();

			loading = -1;
		}else{

			loading = 0;
		}

	}


	GetRoutine recommendationRequests = null;
	bool events,news;

	void Update()
	{

		switch(loading){

			case -1:
				if(wvServCom.Loading().status()==RoutineStatus.READY){
				
				SaveFileManager.loadSaveFromPlayerDownload(wvServCom.loadSavefile(),wvServCom.language,WorldViewServerCommunication.userProfile.getLiteracyProfile());//status changes to IDLE
					acServCom.setUserDetails(wvServCom.getUsername(),wvServCom.getUserID(),wvServCom.language);

					loading = 0;
				}else if(wvServCom.Loading().status()!=RoutineStatus.WAIT){
					Debug.LogWarning("TODO: Server could not be initialised");
				}
				break;
			case 0://WAIT UNTIL UPDATE POP UPS ARE DO


				break;

			case 1:
				SaveFileManager.saveGameStateToServer(wvServCom);
				events = false;
				news = false;
				suggestionStatus = wvServCom.requestSuggestion();
				newsfeedDownloadStatus = wvServCom.requestNewsfeed(1);
			
				loading = 2;
				
				break;
		
			case 2://WAIT for suggestions

			//Keeps track of suggestions
				if(suggestionStatus.status()==RoutineStatus.READY)
				{
				
					createEvents(wvServCom.loadDifficultyAndActivity());//status changes to IDLE
					events = true;

				}else if(suggestionStatus.status()==RoutineStatus.ERROR){
					events = true;

				
				}


				//Keep track of newsfeed
				if(newsfeedDownloadStatus.status() == RoutineStatus.READY)
				{
				// Fill up the local newsfeed.
					fillupLocalNewsfeedWithServNewsfeed(wvServCom.getNewsfeed());//Status is changed to IDLE
					news = true;
				}else if(newsfeedDownloadStatus.status() == RoutineStatus.ERROR){

					//Could not get news feed
					fillupLocalNewsfeedWithServNewsfeed(new UserLog[0]);
					news = true;

				}

				if(news&events)
					loading = 3;
					
				break;

			case 3:


				if(unlockingCharacters ||  unlockingLocations ||unlockingDifficulty)//wait until unlock pop ups are done
					break;


				//longInititalisation();


				activateWVInterface(false);

				worldView.setLoadingIconVisibility(false);

				loading = 4;

				break;


		}


		if(loading>3){//Loaded

			if(recommendationRequests!=null){//Need data for surprise activity (waiting for recommendation)

				if(recommendationRequests.status()==RoutineStatus.READY){

					worldView.setLoadingIconVisibility(false);

					List<PackagedNextActivity> activity = wvServCom.loadDifficultyAndActivity();
					List<ApplicationID> availableAcsInWorld = gbMang.getAvailableActivities();
						
					if (activity.Count>0){
							
						foreach(PackagedNextActivity a in activity){

							for(int i=0;i<a.activity.Length;i++){//Different ouptions
									
								ApplicationID activityID = a.getActivities()[i];
								if(availableAcsInWorld.Contains(activityID)){
									
									int activityOwner = LocalisationMang.getOwnerNpcOfActivity(activityID);
							
									ExternalParams extParams = new ExternalParams(activityID, gbMang.getNpcIDForLangAreaDifficulty(a.category,a.index) ,a.category,a.index,a.level[i],false);

									this.launchQuest(extParams,activityOwner ,launcherDetails,launcherMode);

									recommendationRequests = null;
									return;
											
								}
							}
						}
					}
						
					//WorldViewServerCommunication.setError("Server could not recommend an activity");
					activateWVInterface();
					recommendationRequests = null;

					/*ApplicationID reqAcPKey = LocalisationMang.getRandomApplicableActivityPKeyForLangArea(surpriseActivity[0],new List<ApplicationID>());
					int activityOwnerDefault = LocalisationMang.getOwnerNpcOfActivity(reqAcPKey);
						
					ExternalParams extParamsDefault = new ExternalParams(reqAcPKey,gbMang.getNpcIDForLangAreaDifficulty(surpriseActivity[0],surpriseActivity[1]),surpriseActivity[0],surpriseActivity[1],LocalisationMang.requestLevelStatic(reqAcPKey, surpriseActivity[0],surpriseActivity[1], 0),false);
					*/

				}else if(recommendationRequests.status()==RoutineStatus.ERROR){

					UnityEngine.Debug.LogError("Server error when retrieving recommendation");//Error window should appear automatically

					worldView.setLoadingIconVisibility(false);
					activateWVInterface();
					recommendationRequests = null;
					
				}
				
			}

			if(startRequested){

				if(acServCom.Loading().status() == RoutineStatus.READY){
					startRequested = false;
					worldView.setLoadingIconVisibility(false);
					handleImmediateActivityStart();

				}else if(acServCom.Loading().status() == RoutineStatus.ERROR){

					//Error message automatic
					abortQuest();
				}

			}



		}
	}

	public void move(){

		if(showDialog){

			Destroy(dialogue);
			worldView.clearUnclickable();
			worldView.addUnclickable(GBIconUIBounds);
			worldView.addUnclickable(PauseIconUIBounds);

			showDialog = false;
		}

	}


	/**
	 * 
	 * Function that is called after an activity has been played (or the game just started)
	 * 
	 * 
	 */

	public void attemptActivityDebrief(WorldViewScript wv)
	{
		

		worldView = wv;
		worldView.setLoadingIconVisibility(true);


		if(WorldViewServerCommunication.tts!=null)
			WorldViewServerCommunication.tts.clearCache();


		gbMang = GhostbookManagerLight.getInstance();

		ghostbookIcon = Camera.main.transform.FindChild("GUI_Ghostbook");
		ghostbookIcon.GetComponent<Animator>().enabled = true;
		ghostbookIcon.GetComponent<Animator>().Play("GB_ButtonShine");
		closeGB = Resources.Load<Texture2D>("Textures/Ghostbook/UI/ghostbookiconclose");
		
		GBIconUIBounds = WorldSpawnHelper.getWorldToGUIBounds(ghostbookIcon.FindChild("ButtonMain").renderer.bounds,upAxisArr);
		PauseIconUIBounds = WorldSpawnHelper.getWorldToGUIBounds(ghostbookIcon.FindChild("GUI_Pause").FindChild("ButtonMain").renderer.bounds,upAxisArr);

		ghostbookIcon.gameObject.SetActive(showGBIcon);
		GBOpen = false;


		
		worldView.displayWorldCharacters(gbMang.visibleCharacters());

		questBar = Camera.main.transform.FindChild("QuestBar");
		questBarUiBounds = WorldSpawnHelper.getWorldToGUIBounds(questBar.gameObject.renderer.bounds,upAxisArr);
		questBarAbortBtnUiBounds = WorldSpawnHelper.getWorldToGUIBounds(questBar.FindChild("AbortQuestBtn").renderer.bounds,upAxisArr);
		showQuestBar = false;


		GameObject tmp = new GameObject();
		gbDisp = tmp.AddComponent<GhostBookDisplay>();
		gbDisp.init(this,gbMang);

		deactivateWVInterface();



		GameObject poRef = PersistentObjMang.getInstance();
		DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
		
		if(ds.containsData("ActivityOutcome"))//An activity was played before
		{

			IServerServices server = (IServerServices)getServerObject().GetComponent(typeof(IServerServices));

			List<PackagedProfileUpdate> update = server.getProfileUpdate();

			loading = 0;//Wait for the pop-ups


			ActivityExternalDebriefer aed = transform.gameObject.AddComponent<ActivityExternalDebriefer>();
			aed.debrief(
				(ApplicationID) ((System.Object[])ds.getData("ActivityOutcome"))[0], 
				(bool) ((System.Object[])ds.getData("ActivityOutcome"))[1], 
			        (bool)((System.Object[]) ds.getData("ActivityOutcome"))[2], 
				(ActivitySessionMetaData) ds.getData("AcMetaData"), 
				gbMang,
				update,
				WorldViewServerCommunication.userProfile.getLiteracyProfile().getUserSeveritites().getSeverities(),
				this);

			ds.removeData("ActivityOutcome");
			ds.removeData("AcMetaData");

		}else{
			loading = 1;
			
		}


		if(!gbMang.getAvailableActivities().Contains(ApplicationID.TRAIN_DISPATCHER)){

				Debug.Log("Unlock Train");

				int newMainCharacter = LocalisationMang.getOwnerNpcOfActivity(ApplicationID.TRAIN_DISPATCHER);
				gbMang.unlockCharacterForLocation(newMainCharacter);
				worldView.displayWorldCharacters(new List<int>{newMainCharacter});
		}

		if(!gbMang.getAvailableActivities().Contains(ApplicationID.MOVING_PATHWAYS)){
				Debug.Log("Unlock Moving Pathways");

				int newMainCharacter = LocalisationMang.getOwnerNpcOfActivity(ApplicationID.MOVING_PATHWAYS);
				gbMang.unlockCharacterForLocation(newMainCharacter);
				worldView.displayWorldCharacters(new List<int>{newMainCharacter});
		}

		if(!gbMang.getAvailableActivities().Contains(ApplicationID.WHAK_A_MOLE)){
				Debug.Log("Unlock Whack");

				int newMainCharacter = LocalisationMang.getOwnerNpcOfActivity(ApplicationID.WHAK_A_MOLE);
				gbMang.unlockCharacterForLocation(newMainCharacter);
				worldView.displayWorldCharacters(new List<int>{newMainCharacter});
		}


		this.enabled = true;
		
	}

	public void updatesPopUpsDone(){

		loading = 1;

	}

	public void findCharacter(int charID){

		gbDisp.closeGhostbook();
		GBOpen = false;
		activateWVInterface();

		worldView.moveToCharacter(charID);
	}
	
	public void initiateOwnActivity(int charID){
		
		launcherMode = Mode.ADVENTURE;
		launcherDetails = "ActivityOwner";

		
//		recommendationRequests = wvServCom.requestSuggestion(lA,diff,LocalisationMang.getMainApplicationID(charID));
		recommendationRequests = wvServCom.requestSuggestion(gbMang.getNPCserverName(charID),LocalisationMang.getMainApplicationID(charID));
		deactivateWVInterface();
		worldView.setLoadingIconVisibility(true);
		
		//int diff = gbMang.getDiffForNPCID(diagPersonID)[UnityEngine.Random.Range(0,gbMang.getDiffForNPCID(diagPersonID).Count)  ];
		//activitySurpriseLaunched(lA,diff);
		
	}


	public void initiateActivityForChar(int charID){

		launcherMode = Mode.ADVENTURE;
		launcherDetails = "Quest";

		recommendationRequests = wvServCom.requestSuggestion(gbMang.getNPCserverName(charID));
		deactivateWVInterface();
		worldView.setLoadingIconVisibility(true);


	}

	public void characterUnlockDone(){

		unlockingCharacters = false;
		//worldView.displayWorldCharacters(popUpCharacters);

	}

	//Request recommendation from the server and call actiivtyLaunced
	public void launchSurpriseQuest(int la, int diff){

		launcherMode = Mode.PLAY;
		launcherDetails = "Album";

		recommendationRequests = wvServCom.requestSuggestion(la,diff);
		deactivateWVInterface();
		worldView.setLoadingIconVisibility(true);

	}

	
	public void launchQuest(EventSlot e,string source,Mode m){

		//Debug.Log("LA: "+e.getEncounter().getLanguageArea()+" DF:"+e.getEncounter().getDifficulty());

		gbDisp.closeGhostbook();
		GBOpen = false;
		activateWVInterface();

		if(checkLevelCorrectness(e.getEncounter().getLevel(),e.getApplicationID(),e.getEncounter().getLanguageArea())){

			launcherMode = m;
			launcherDetails = source;
			selectedEvent = e;
			
			acServCom.setActivityParameters(selectedEvent.getEncounter().getLocation(),
			                                selectedEvent.getEncounter().getDifficulty(),
			                                selectedEvent.getEncounter().getLanguageArea(),
			                                wvServCom.getUserID(),
			                                0,
			                                selectedEvent.getEncounter().getLevel(),
			                                launcherMode);
			
			acServCom.load();


			if(m ==Mode.ADVENTURE){//walk
				activateQuestBar(selectedEvent);
				worldView.sendQuestGiverToCharacter(selectedEvent.getQuestGiverCharID(),selectedEvent.getQuestReceiverCharID());
				worldView.moveToCharacter(selectedEvent.getQuestReceiverCharID());
			}else{//start immediate

				deactivateWVInterface();
				worldView.setLoadingIconVisibility(true);
				startRequested = true;

			}
			
		}else{
			WorldViewServerCommunication.setError("Problem on difficulty ("+e.getEncounter().getLanguageArea()+","+e.getEncounter().getDifficulty()+")\nLevel "+e.getEncounter().getLevel()+" no supported");

		}


	}


	//Activity should start with the given parameters
	public void launchQuest(ExternalParams builtExtParams,int destChar,string source,Mode m){

		EventSlot selectedEvent = new EventSlot(-1,
		                                        builtExtParams.questGiverIdOverride,
		                                        destChar,
		                                        builtExtParams.acIdOverride,
		                                        new Encounter(
														builtExtParams.acIdOverride,
														builtExtParams.levelOverride,
														builtExtParams.langAreaOverride,
														builtExtParams.difficultyOverride,
		              									"",
		              									LocalisationMang.langCode)
		                              			);
		launchQuest(selectedEvent,source,m);

	}



	private void activateQuestBar(EventSlot selectedEvent){

		questBar.gameObject.SetActive(true);
		QuestBarScript qBarScript = questBar.gameObject.AddComponent<QuestBarScript>();
		qBarScript.init(selectedEvent.getQuestGiverCharID(),selectedEvent.getQuestReceiverCharID(),selectedEvent.getApplicationID());//selectedEvent.getApplicationID()
		
		showQuestBar = true;
		
		showGBIcon = false;
		ghostbookIcon.gameObject.SetActive(showGBIcon);
		
		//if(showNarrativeIcon){
			//narrativeIcon.gameObject.SetActive(false);
		//}
		

		worldView.clearUnclickable();
		worldView.addUnclickable(questBarUiBounds);
		worldView.addUnclickable(questBarAbortBtnUiBounds);

	}

	
	public void abortQuest()
	{

		launcherDetails = "";
		worldView.resetQuestGiverPosition(selectedEvent.getQuestGiverCharID());
		selectedEvent = null;

		worldView.cancelMove();

		questBar.gameObject.SetActive(false);

		showQuestBar = false;
		
		activateWVInterface();

	}





	private bool checkLevelCorrectness(string challenge,ApplicationID reqAcID,int languageArea){

		TextAsset ta = (TextAsset) Resources.Load("Localisation_Files/"+LocalisationMang.langCode+"/Instructions_"+LocalisationMang.langCode+"_"+reqAcID,typeof(TextAsset));
		string text = ta.text;
		string[] lineArr = text.Split('\n');


		string initialChallenge = "";
		
		bool found = false;
		
		
		LevelParameters given = new LevelParameters(challenge);
		
		foreach(string line in lineArr){
			
			string[] values = line.Split(',');

			if(System.Convert.ToInt32(values[0])==languageArea){
				
				if(initialChallenge.Equals(""))
					initialChallenge = values[3];
				
				LevelParameters a = new LevelParameters(values[3]);
				
				
				if((a.mode==given.mode)){
					initialChallenge = values[3];
					if((a.ttsType==given.ttsType)){
						initialChallenge = values[3];
						
						if((a.accuracy == given.accuracy)&(a.amountDistractors == given.amountDistractors)&(a.speed==given.speed)){
							
							found = true;
							break;
						}
						
					}
					
				}
				
				
			}
			//}
		}

		return found;

	}


	bool startRequested = false;
	public void startActivity(){

		worldView.setLoadingIconVisibility(true);
		startRequested = true;
	}

	private void handleImmediateActivityStart()
	{
		//deactivatePersistentObj();
		ILevelConfigGenerator builtLvlGen = null;
		string nextSceneName = "";

		switch(selectedEvent.getApplicationID())
		{				
			// Server Based.
		case ApplicationID.SERENADE_HERO : builtLvlGen = new SHLevelConfigGeneratorServer(acServCom); nextSceneName = "SerenadeScene"; break;
		case ApplicationID.DROP_CHOPS : builtLvlGen = new SJLevelConfigGeneratorServer(acServCom); nextSceneName = "SolomonScene";break;
		case ApplicationID.MOVING_PATHWAYS : builtLvlGen = new MPLevelConfigGeneratorServer(acServCom); nextSceneName = "MovingPathwaysScene-Auto";  break;
		case ApplicationID.HARVEST : builtLvlGen = new HarvestLevelConfigGeneratorServer(acServCom);nextSceneName = "HarvestScene";  break;
		case ApplicationID.WHAK_A_MOLE: builtLvlGen = new WAMLevelConfigGeneratorServer(acServCom);nextSceneName = "WhackaMonkeyScene";  break;
		case ApplicationID.MAIL_SORTER : builtLvlGen = new MSLevelConfigGeneratorServer(acServCom);nextSceneName = "MailSorter";  break;
		case ApplicationID.EYE_EXAM: builtLvlGen = new BBLevelConfigGeneratorServer(acServCom); nextSceneName = "BridgeBuilder"; break;
		case ApplicationID.TRAIN_DISPATCHER: builtLvlGen = new TDLevelConfigGeneratorServer(acServCom); nextSceneName = "TrainDispatcher"; break;
		case ApplicationID.ENDLESS_RUNNER: builtLvlGen = new PDLevelConfigGeneratorServer(acServCom); nextSceneName = "PackageDelivery";break;
		}


		if(!WorldViewServerCommunication.error)//Ready to go!
		{
			ActivitySessionMetaData acSessionMetaData = new ActivitySessionMetaData(
															LocalisationMang.getNPCnames()[selectedEvent.getQuestGiverCharID()],
															selectedEvent.getApplicationID(),
															gbMang.getNameForLangArea(selectedEvent.getEncounter().getLanguageArea()),
															gbMang.createDifficultyShortDescription(selectedEvent.getEncounter().getLanguageArea(),selectedEvent.getEncounter().getDifficulty()),
															selectedEvent.getQuestGiverCharID(),
															selectedEvent.getEncounter().getLanguageArea(),
															selectedEvent.getEncounter().getDifficulty(),
			                                                launcherMode,
			                                                launcherDetails);

			
			GameObject poRef = PersistentObjMang.getInstance();
			DatastoreScript dataStore = poRef.GetComponent<DatastoreScript>();
			dataStore.insertData("LevelGen",builtLvlGen);
			dataStore.insertData("ActivityReturnSceneName",Application.loadedLevelName);
			dataStore.insertData("AcMetaData",acSessionMetaData);
			dataStore.insertData("NextSceneToLoad",nextSceneName);
			
			Application.LoadLevel("LoadingScene");
			Debug.Log("Progress Script goes to sleep here");

			this.enabled = false;
		}
		else
		{
			Debug.Log("Level wrong");
			abortQuest();
		}






	}


	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{

		Debug.Log("Progress script: "+para_sourceID+" "+para_eventID);
		if(para_sourceID == "Popup")
		{
			if(para_eventID == "OK")
			{

				Debug.Log("Who are you?");

				activateWVInterface();
			}
		}else if(para_sourceID == "IntroNarrativeWindow")
		{
			if(para_eventID == "AllDone")
			{
				Resources.UnloadUnusedAssets();
				System.GC.Collect();

				activateWVInterface();
				pause = false;

			}
		}else if(para_sourceID == "GamePauseWindow"){

			if(para_eventID == "ReturnToGame")
			{
				Destroy(GameObject.Find("GamePauseWindow"));
				activateWVInterface();
				pause = false;

			}else if(para_eventID == "Story"){

				deactivateWVInterface();
				Destroy(GameObject.Find("GamePauseWindow"));
				triggerIntroNarrativeWindow();

			}else if(para_eventID == "ExitGame"){

				SaveFileManager.saveGameStateToServer(wvServCom);
				Application.Quit();
			}

		}else if(para_sourceID == "DialogueManager"){

			if(para_eventID == "DialogueAbruptExit")
			{
			// Abrupt exit. (Via the exit button on the diag bar).
				showDialog = false;

				Destroy(transform.gameObject.GetComponent<DialogueManager>());
				activateWVInterface();
				worldView.leavingCharacter();
			
			}else if(para_eventID == "DialogueEnded")
			{

				Debug.Log("DEPRECATED");
				// Abrupt exit. (Via the exit button on the diag bar).
				Destroy(transform.gameObject.GetComponent<DialogueManager>());
				showDialog = false;

			}else if(para_eventID == "PlayOwn")
			{
				// Abrupt exit. (Via the exit button on the diag bar).
				Destroy(transform.gameObject.GetComponent<DialogueManager>());
				showDialog = false;
				initiateOwnActivity(targetCharacterQuest);
				
				
			}
		}

	}



	void errorMessageWindow(string errorMessageStr){
		
		InitStyles();
		
		
		GUI.Box(new Rect(Screen.width/2-210, Screen.height/2-150, 420, 300),"",boxStyle);
		
		

		if (errorMessageStr.Contains("Could not resolve host" )||errorMessageStr.Contains("couldn't connect to host" )||errorMessageStr.Contains("No address")||errorMessageStr.Contains("timed out")){
			
			errorMessageStr = LocalisationMang.translate("No internet connection");
			
		}
		
		Color defaultColor = GUI.color;
		
		GUI.color = Color.black;
		GUI.Label(new Rect(Screen.width/2-200, Screen.height/2-140, 400, 240),LocalisationMang.translate("An error has occurred.")+"\n\n"+errorMessageStr);
		GUI.color = defaultColor;
		
		if(GUI.Button(new Rect( Screen.width/2-40, Screen.height/2+110, 80, 30    ),LocalisationMang.translate("Ok"))){
			
			WorldViewServerCommunication.clearError();
		}
	}


	bool pause = false;

	void OnGUI(){

		if(pause)
			return;

		if(WorldViewServerCommunication.error){
			
			errorMessageWindow(WorldViewServerCommunication.errorMessage);
		}


		if(showQuestBar)
		{
			if(selectedEvent != null)
			{
				GUI.color = Color.clear;
				if(GUI.Button(questBarUiBounds,""))
				{
					worldView.moveToCharacter(selectedEvent.getQuestReceiverCharID());
				}
				
				if(GUI.Button(questBarAbortBtnUiBounds,""))
				{
					abortQuest();
				}
				GUI.color = Color.white;
			}
		}



		if(GBOpen){

			if(GUI.Button(GBIconUIBounds, closeGB))
			{
				gbDisp.closeGhostbook();
				GBOpen = false;
				activateWVInterface();
						
			}


		}else{
			if(showGBIcon){

				GUI.color = Color.clear;
			
				if(GUI.Button(GBIconUIBounds,""))//No texture because GBIcon is an object
				{
					worldView.cancelMove();
					deactivateWVInterface();
					if(showDialog){
						showDialog = false;
						DestroyImmediate(dialogue);
					}
					GBOpen = true;
					gbDisp.openGhostbook();
				}

				if(GUI.Button(PauseIconUIBounds,""))//No texture because GBIcon is an object
				{
					deactivateWVInterface();
					if(showDialog){
						showDialog = false;
						DestroyImmediate(dialogue);
					}
					pause = true;
					worldView.cancelMove();

					Transform gamePauseWindow = Resources.Load<Transform>("Prefabs/GamePauseWindow");
					
					GameObject mainCamera = GameObject.Find("Main Camera");
					Vector3 windowSpawnPos = new Vector3(mainCamera.transform.position.x,
					                                     mainCamera.transform.position.y,
					                                     mainCamera.transform.position.z + 3f);
					
					Transform nwAcPauseWindow = (Transform) Instantiate(gamePauseWindow,windowSpawnPos,Quaternion.identity);
					nwAcPauseWindow.name = "GamePauseWindow";
					GamePauseWindow apwScript = nwAcPauseWindow.gameObject.AddComponent<GamePauseWindow>();
					apwScript.registerListener("AcScen",this);

				}


				GUI.color = Color.white;

			}
		}

		if(showDialog){
			if(dialogue!=null){

				GUI.color = Color.clear;


				if(GUI.Button( WorldSpawnHelper.getWorldToGUIBounds(dialogue.transform.FindChild("BioButton").renderer.bounds,upAxisArr),"")){
					showDialog = false;
					DestroyImmediate(dialogue);


					worldView.leavingCharacter();

					deactivateWVInterface();
					GBOpen = true;
					gbDisp.openGhostbook(targetCharacterQuest);
					
				}else if((dialogue.transform.FindChild("ErrandHelpIcon")!=null)&&GUI.Button( WorldSpawnHelper.getWorldToGUIBounds(dialogue.transform.FindChild("ErrandHelpIcon").FindChild("ButtonMain").renderer.bounds,upAxisArr),"")){
					showDialog = false;

					initiateActivityForChar(targetCharacterQuest);
					DestroyImmediate(dialogue);

						
				}else if((dialogue.transform.FindChild("InfoIcon")!=null)&&GUI.Button( WorldSpawnHelper.getWorldToGUIBounds(dialogue.transform.FindChild("InfoIcon").FindChild("ButtonMain").renderer.bounds,upAxisArr),"")){

					DestroyImmediate(dialogue);
					deactivateWVInterface();

					DialogueManager dm = transform.gameObject.AddComponent<DialogueManager>();
					dm.registerListener("ProgressScript",this);

					LocalisationMang.loadBioFile(targetCharacterQuest);
					List<string> bioSections = LocalisationMang.getAllLoadedBioSections(targetCharacterQuest);
					//npcName


					dm.narrativeDialogue(targetCharacterQuest,bioSections);



				}

				GUI.color = Color.white;

				
			}
		}



	}

	int targetCharacterQuest = -1;
	
	public void triggerAppropriateDialogue(int dialogTargetCharID)
	{


		if(showQuestBar &&(dialogTargetCharID == selectedEvent.getQuestReceiverCharID())){//Talking to destination character on a quest

			deactivateWVInterface();
			worldView.setLoadingIconVisibility(true);
			startRequested = true;

			worldView.talking2Character(dialogTargetCharID);

				
		}else if(!showQuestBar){//if talking to a character while not in quest

			showDialog = true;
			targetCharacterQuest = dialogTargetCharID;
			if(LocalisationMang.checkIfNpcIsMainChar(dialogTargetCharID)){

				openMainCharacterDialog(dialogTargetCharID);

			}else{

				openSecCharacterDialog(dialogTargetCharID);

			}

			worldView.addUnclickable(WorldSpawnHelper.getWorldToGUIBounds(dialogue.renderer.bounds,upAxisArr));

			worldView.talking2Character(dialogTargetCharID);


			
		}else{//Already on a quest, and shouldn't be talking to this guy, ignore!

			Debug.LogWarning("Quest bar! "+showQuestBar);
		}


	}

	bool showDialog = false;
	GameObject dialogue;

	private void openSecCharacterDialog(int charID){

		Vector3 position = new Vector3(Camera.main.transform.position.x+3.0f,Camera.main.transform.position.y-0.5f, 0.4f);
		dialogue = (GameObject)Instantiate(Resources.Load("Prefabs/Dialogue/SecCharDialog"),position,Quaternion.identity);

	}

	private void openMainCharacterDialog(int charID){
		Vector3 position = new Vector3(Camera.main.transform.position.x+3.0f,Camera.main.transform.position.y-0.5f, 0.4f);
		dialogue = (GameObject)Instantiate(Resources.Load("Prefabs/Dialogue/MainCharDialog"),position,Quaternion.identity);


		List<int> candidateDifficulties =	gbMang.getDiffForNPCID(charID);
		List<int> candidateUnlocked =	new List<int>();
		int lA = gbMang.getLangAreaForNPCID(charID);

		foreach(int d in candidateDifficulties)
			if(gbMang.difficultyUnlocked(lA,d))
				candidateUnlocked.Add(d);
		
		if(candidateUnlocked.Count==0){

			//Hide play errand; this character is only unlocked to play own activity

			DestroyImmediate(dialogue.transform.FindChild("ErrandHelpIcon").gameObject);

		}



		
	}

	void deactivateWVInterface(){

		worldView.togglePauseScene(true);

		showGBIcon = false;
		ghostbookIcon.gameObject.SetActive(showGBIcon);

		//if(showNarrativeIcon){

			//narrativeIcon.gameObject.SetActive(false);

		//}
	}

	void activateWVInterface(){

		activateWVInterface(true);
	}


	void activateWVInterface(bool ignoreClick){
		
		worldView.clearUnclickable();
		worldView.addUnclickable(GBIconUIBounds);

		showGBIcon = true;
		ghostbookIcon.gameObject.SetActive(showGBIcon);
		
		//if(showNarrativeIcon){
			
			//narrativeIcon.gameObject.SetActive(true);
			//worldView.addUnclickable(narrativeIconUIBounds);

		//}

		worldView.togglePauseScene(false,ignoreClick);

	}

	
	private GUIStyle boxStyle = null;
	
	
	private void InitStyles()
	{
		if( boxStyle == null )
		{
			boxStyle = new GUIStyle( GUI.skin.box );
			boxStyle.normal.background = MakeTex( 2, 2, new Color( 1f, 1f, 1f, 1f ) );
		}
	}
	private Texture2D MakeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	





	/**
	 * 
	 * This method adds the recommended games to the events list and unlocks the necessary characters
	 * 
	 * */
	private void createEvents(List<PackagedNextActivity> pna)
	{



		if(pna!=null){

			gbMang.clearEventList();
			
			
			List<ApplicationID> recommendedGames = new List<ApplicationID>();

			for(int i_recommendation = 0; i_recommendation< pna.Count;i_recommendation++)
			{

				int nextApp = -1;


				if(pna[i_recommendation].getActivities().Length==0){
					Debug.LogError("Empty recommendation: "+pna[i_recommendation].getLanguageArea()+" "+ pna[i_recommendation].getDifficulty());
					continue;
				}

				List<int> candidateGames = new List<int>();

				for (int j = 0; j<pna[i_recommendation].getActivities().Length;j++){//Find first game on the activity that is currently enabled

					if(gbMang.getContactStatus(  LocalisationMang.getOwnerNpcOfActivity(pna[i_recommendation].getActivities()[j]))==CharacterStatus.UNLOCKED){
						candidateGames.Add(j);

					}

				}
				
				if(candidateGames.Count==0){//If nothing is unlocked, go for the first app

					nextApp = 0;

					Debug.LogWarning("TODO: Unlock a new location!");
					//unlockingLocation = true;
					int newMainCharacter = LocalisationMang.getOwnerNpcOfActivity(pna[i_recommendation].getActivities()[nextApp]);
					gbMang.unlockCharacterForLocation(newMainCharacter);
					worldView.displayWorldCharacters(new List<int>{newMainCharacter});
				}else{

					foreach(int i in candidateGames){
						if(!recommendedGames.Contains(  pna[i_recommendation].getActivities()[i]   )){
								nextApp = i;
								break;
							}
					}
					if(nextApp==-1)//all candidates are repated
						nextApp = candidateGames[0];
				}

				ApplicationID selectedAppID = pna[i_recommendation].getActivities()[nextApp];

				//string eventDescription = LocalisationMang.getActivityShorthand(selectedAppID);
					
				Encounter e = new Encounter(selectedAppID, 
				                            pna[i_recommendation].getLevel()[nextApp], 
				                            pna[i_recommendation].getLanguageArea(), 
				                            pna[i_recommendation].getDifficulty(),
				                            "", 
				                            wvServCom.language);


				int acOwnerID = LocalisationMang.getOwnerNpcOfActivity(e.getLocation());
				int questGiverID = gbMang.getNpcIDForLangAreaDifficulty(e.getLanguageArea(),e.getDifficulty());

				gbMang.addEvent(questGiverID, acOwnerID,e.getLocation(),e);

			
				recommendedGames.Add(selectedAppID);

				wvServCom.activityProposed(pna[i_recommendation].getDifficulty(),pna[i_recommendation].getLanguageArea(),selectedAppID,pna[i_recommendation].getLevel()[nextApp]);
										

				List<int[]> difficultiesUnlocked = gbMang.unlockAllRelatedDifficulties(pna[i_recommendation].getLanguageArea(),pna[i_recommendation].getDifficulty());
						
				List<int> popUpCharacters = new List<int>();
				List<int[]> popUpDifficulties = new List<int[]>();

				foreach(int[] newPair in difficultiesUnlocked){
					int charID = gbMang.getNpcIDForLangAreaDifficulty(newPair[0],newPair[1]);
					if(gbMang.unlockCharacter(charID)){
						popUpCharacters.Add(charID);
					}else{
						if(!popUpCharacters.Contains(charID)){
							popUpDifficulties.Add(newPair);
						}
					}
				}

				if(popUpCharacters.Count>0){

					unlockingCharacters = true;
					worldView.displayWorldCharacters(popUpCharacters);
					List<string> charNames = new List<string>();

					foreach(int id in popUpCharacters)
						charNames.Add(LocalisationMang.getNPCnames()[id]);
										
					unlockingCharacters = true;

					ActivityExternalDebriefer aed = transform.gameObject.AddComponent<ActivityExternalDebriefer>();
					aed.unlockCharacters(
						popUpCharacters,
						charNames,
						this);

				}

				if(popUpDifficulties.Count>0){
					Debug.LogWarning("TODO: new difficulties for known characters have been unlocked");
					//unlockingDifficulty
				}
			}
		}
	}

	/*
	 * 
	 * Unpacks the news items and send them to GB
	 * 
	 * */
	private void fillupLocalNewsfeedWithServNewsfeed(UserLog[] para_servData)
	{
		
		gbMang.eraseNewsItems();
		
		for(int i=0; i<para_servData.Length; i++)
		{
			UserLog tmpLog = para_servData[i];
			if(tmpLog != null)
			{
				ApplicationID tmpAcID = tmpLog.getApplicationID();
				int tmpLangArea = tmpLog.getLanguageArea();
				int tmpDifficulty = tmpLog.getDifficulty();
				int tmpQuestGiverID = gbMang.getNpcIDForLangAreaDifficulty(tmpLangArea,tmpDifficulty);
				string level = tmpLog.getLevel();
				string date = tmpLog.getTimestamp().ToString();
				gbMang.addNewsItemPastActivity(tmpAcID,tmpLangArea,tmpDifficulty,tmpQuestGiverID,level,date);
			}
		}
	}



	private void triggerIntroNarrativeWindow()
	{

		Transform introNarrativeWindowPrefab = Resources.Load<Transform>("Prefabs/IntroNarrativeWindow");
		Rect origPrefab2DBounds = CommonUnityUtils.get2DBounds(introNarrativeWindowPrefab.FindChild("WindowBounds").renderer.bounds);
		GameObject nwIntroNarrativeWindow = WorldSpawnHelper.initWorldObjAndBlowupToScreen(introNarrativeWindowPrefab,origPrefab2DBounds);
		nwIntroNarrativeWindow.transform.position = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,Camera.main.transform.position.z + 3f);
		nwIntroNarrativeWindow.transform.parent = Camera.main.transform;
		
		//string langCodeAsStr = System.Enum.GetName(typeof(LanguageCode),LocalisationMang.langCode);
		
		IntroNarrativeWindow inwScript = nwIntroNarrativeWindow.AddComponent<IntroNarrativeWindow>();
		inwScript.registerListener("AcScen",this);
		inwScript.init("IntroNarrativeWindow",true);
	}
	

}

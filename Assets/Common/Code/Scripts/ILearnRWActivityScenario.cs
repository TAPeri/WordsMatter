/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public abstract class ILearnRWActivityScenario : ILearnRWScenario, CustomActionListener
{

	protected ApplicationID acID;
	protected ActivityServerCommunication serverCommunication;
	protected ILevelConfigGenerator lvlConfigGen;
	protected bool sceneNoLongerValid = false;
	protected bool isInitialised = false;
	private ActivitySessionMetaData acSessionMetaData;
	private ActivityResult acResultData;

	private bool hasAlreadyShownGoalOnIntro = false;


	private bool endAcServRequestLaunchSuccess = false;
	//private bool needsLocalSeverityUpdate = false;



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


	void Awake()
	{

		GameObject servComObj = getServerObject();
		serverCommunication = servComObj.GetComponent<ActivityServerCommunication>();

	}


	public void replay(){

		GameObject poRef = PersistentObjMang.getInstance();
		DatastoreScript dataStore = poRef.GetComponent<DatastoreScript>();
		
		//Original acSessionMetaData is still on the data store (is progress script who deletes it)
		//dataStore.insertData("AcMetaData",acSessionMetaData);
		//Get level generator from ILearnRWActivityScenario
		lvlConfigGen.reboot();
		dataStore.insertData("LevelGen",lvlConfigGen);
		//dataStore.insertData("ActivityReturnSceneName",Application.loadedLevelName);

		dataStore.insertData("NextSceneToLoad", Application.loadedLevelName);
		
		Application.LoadLevel("LoadingScene");



	}

	protected bool initLevelConfigGenerator()
	{
		try
		{
			GameObject poRef = PersistentObjMang.getInstance();
			DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
			if(ds.containsData("LevelGen"))
			{
				lvlConfigGen = (ILevelConfigGenerator) ds.getData("LevelGen");
				if(lvlConfigGen != null)
				{
					ds.removeData("LevelGen");
					return true;
				}
			}

			return false;
		}
		catch(System.Exception ex)
		{
			Debug.LogError(ex.Message);
			return false;
		}
	}

	protected bool loadActivitySessionMetaData()
	{
		try
		{
			GameObject poRef = PersistentObjMang.getInstance();
			DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
			if(ds.containsData("AcMetaData"))
			{
				acSessionMetaData = (ActivitySessionMetaData) ds.getData("AcMetaData");
				if(acSessionMetaData != null)
				{
					updateActivityProgressMetaData(acSessionMetaData.getProgress());
					return true;
				}
			}

			return false;
		}
		catch(System.Exception ex)
		{
			Debug.LogError(ex.Message);
			return false;
		}
	}

	// para_percCompletion: float between 0 and 1.
	protected void updateActivityProgressMetaData(float para_percCompletion)
	{
		if(acSessionMetaData != null)
		{
			// Update data structure.
			acSessionMetaData.setProgress(para_percCompletion);
		}

		// Update any relevant in-game graphic elements. Eg. the miniture portrait progress bar.
		GameObject personPortraitObj = GameObject.Find("PersonPortrait");
		if(personPortraitObj != null)
		{
			int percAsInt = (int) (para_percCompletion * 100f);
			PortraitHelper.updatePortraitProgressBar(personPortraitObj,para_percCompletion,Color.green);
			PortraitHelper.updatePortraitTextLabel(personPortraitObj,""+percAsInt+"%",0.04f);
		}
	}

	protected bool triggerServerFlushLogsNUpdateRequest(bool para_activityOutcome)
	{
		if( ! sceneNoLongerValid)
		{
			sceneNoLongerValid = true;
			if(serverCommunication != null)
			{
				List<GetRoutine> endRoutineRefs = serverCommunication.endRound();
				// Note: The window script will hide the window from view (since we are loading in the background here).
				showServerUpdateScreen(endRoutineRefs,new List<string>() { LocalisationMang.translate("Sending your progress to the server"), LocalisationMang.translate("Retrieving server update") });
				return true;
			}
			else
			{
				return false;
			}
		}
		return false;
	}

	protected void endActivity(ApplicationID para_acID, bool para_outcome, GameyResultData para_gameyData, bool para_activityIsIncomplete)
	{
		GameObject poRef = PersistentObjMang.getInstance();
		DatastoreScript ds = poRef.GetComponent<DatastoreScript>();


		if(ds.getData("ActivityOutcome")!=null){//this is a replay
			System.Object[] previousOutcome = (System.Object[])ds.getData("ActivityOutcome");
			ds.removeData("ActivityOutcome");

			System.Object[] dataArr = new System.Object[3];
			dataArr[0] = para_acID;
			dataArr[1] = para_outcome||((bool)previousOutcome[1]);//Either this or previous outcome were positive
			dataArr[2] = para_activityIsIncomplete&&((bool)previousOutcome[2]);//Both games, the activity was incomplete
			ds.insertData("ActivityOutcome",dataArr);

		}else{

		// Store basic info.
			System.Object[] dataArr = new System.Object[3];
			dataArr[0] = para_acID;
			dataArr[1] = para_outcome;
			dataArr[2] = para_activityIsIncomplete;
			ds.insertData("ActivityOutcome",dataArr);

		}


		// Store detailed activity result.
		if(acSessionMetaData != null)
		{
			acSessionMetaData.recordActivityEnd();
			if(acResultData != null)
			{
				acResultData.saveActivitySessionMetaData(acSessionMetaData,para_gameyData);
				//ds.insertData("ActivityResult",acResultData);
			}
		}
		

		if(( ! para_activityIsIncomplete)
		   &&
		   ((para_acID == ApplicationID.DROP_CHOPS)
		 	||(para_acID == ApplicationID.SERENADE_HERO)
		 	||(para_acID == ApplicationID.MOVING_PATHWAYS)
		 	||(para_acID == ApplicationID.HARVEST)
		 	||(para_acID == ApplicationID.WHAK_A_MOLE)
		 	||(para_acID == ApplicationID.MAIL_SORTER)
		 	||(para_acID == ApplicationID.EYE_EXAM)
		 	||(para_acID == ApplicationID.TRAIN_DISPATCHER)
		 	||(para_acID == ApplicationID.ENDLESS_RUNNER))
		  )
		{
			// Trigger results screen.

			Transform resultsWindowPrefab = Resources.Load<Transform>("Prefabs/ResultViewerWindow");

		
			Rect origPrefab2DBounds = CommonUnityUtils.get2DBounds(resultsWindowPrefab.FindChild("WindowBounds").renderer.bounds);
			GameObject nwResultWindow = WorldSpawnHelper.initWorldObjAndBlowupToScreen(resultsWindowPrefab,origPrefab2DBounds);
			nwResultWindow.name = "ResultViewerWindow";
			nwResultWindow.transform.position = new Vector3(Camera.main.transform.position.x,
			                                                Camera.main.transform.position.y,
			                                                Camera.main.transform.position.z + 5f);



			ActivityResultViewer resViewer = null;
			if(para_acID == ApplicationID.DROP_CHOPS)
			{
				resViewer = nwResultWindow.AddComponent<SJResultViewer>();
			}
			else if(para_acID == ApplicationID.SERENADE_HERO)
			{
				resViewer = nwResultWindow.AddComponent<SHResultViewer>();
			}
			else if(para_acID == ApplicationID.MOVING_PATHWAYS)
			{
				resViewer = nwResultWindow.AddComponent<MPResultViewer>();
			}
			else if(para_acID == ApplicationID.HARVEST)
			{
				resViewer = nwResultWindow.AddComponent<HarvestResultViewer>();
			}
			else if(para_acID == ApplicationID.WHAK_A_MOLE)
			{
				resViewer = nwResultWindow.AddComponent<WAMResultViewer>();
			}
			else if(para_acID == ApplicationID.MAIL_SORTER)
			{
				resViewer = nwResultWindow.AddComponent<MSResultViewer>();
			}
			else if(para_acID == ApplicationID.EYE_EXAM)
			{
				resViewer = nwResultWindow.AddComponent<BBResultViewer>();
			}
			else if(para_acID == ApplicationID.TRAIN_DISPATCHER)
			{
				resViewer = nwResultWindow.AddComponent<TDResultViewer>();
			}
			else if(para_acID == ApplicationID.ENDLESS_RUNNER)
			{
				resViewer = nwResultWindow.AddComponent<PDResultViewer>();
			}

			if(resViewer != null)
			{
				resViewer.registerListener("AcScen",this);
				resViewer.init(acResultData);
			}

			pauseScene(true);
		}
		else
		{
			exitActivityScene();
		}
	}
	
	private void exitActivityScene()
	{
		GameObject poRef = PersistentObjMang.getInstance();
		DatastoreScript ds = poRef.GetComponent<DatastoreScript>();

		string returnSceneName = (string) ds.getData("ActivityReturnSceneName");
		ds.insertData("NextSceneToLoad",returnSceneName);
		//ds.insertData("NeedsLocalSeverityUpdate",needsLocalSeverityUpdate);
		if(Application.CanStreamedLevelBeLoaded("LoadingScene"))
		{
			Application.LoadLevel("LoadingScene");
		}
		else
		{
			Application.Quit();
		}
		Destroy(this.gameObject);
	}

	protected void showInfoScreen(string para_goalAppend)
	{
		string acNameStr = "";
		string howToText = "";
		string goalText = "";

		string acStrKey = "";

		string[] keyArr = this.getActivityKeys();
		acStrKey = keyArr[0];
		acNameStr = keyArr[1];


		string unParsedHowToText = LocalisationMang.getString(acStrKey+"*"+"Intro");
		string[] unFormattedHowToSegments = unParsedHowToText.Split('*');

		for(int i=0; i<unFormattedHowToSegments.Length; i++)
		{
			howToText += ("* "+unFormattedHowToSegments[i]);

			if(i < (unFormattedHowToSegments.Length-1))
			{
				howToText += "\n";
			}
		}


		string unParsedGoalText = LocalisationMang.getString(acStrKey+"*"+"Goal");
		string[] unFormattedGoalSegments = unParsedGoalText.Split('*');

		for(int i=0; i<unFormattedGoalSegments.Length; i++)
		{
			goalText += ("* "+unFormattedGoalSegments[i]);

			if(i < (unFormattedGoalSegments.Length-1))
			{
				goalText += "\n";
			}
		}
		goalText += para_goalAppend;



		ActivityInfoWindow acWind = transform.gameObject.AddComponent<ActivityInfoWindow>();
		acWind.registerListener("AcScen",this);
		acWind.init(acNameStr,howToText,goalText);
		pauseScene(true);
	}

	public void showInfoSlideshow()
	{
		//string acNameStr = "";
		//string howToText = "";

		
		string acStrKey = "";
		
		string[] keyArr = this.getActivityKeys();
		acStrKey = keyArr[0];
		//acNameStr = keyArr[1];
		
		
		string unParsedHowToText = LocalisationMang.getString(LocalisationMang.getActivityShorthand(acID)+"*"+"Intro");
		string[] unFormattedHowToSegments = unParsedHowToText.Split('*');


		Transform tutorialWindowPrefab = Resources.Load<Transform>("Prefabs/TutorialWindow");
		Rect origPrefab2DBounds = CommonUnityUtils.get2DBounds(tutorialWindowPrefab.FindChild("WindowBounds").renderer.bounds);
		GameObject nwTutorialWindow = WorldSpawnHelper.initWorldObjAndBlowupToScreen(tutorialWindowPrefab,origPrefab2DBounds);
		nwTutorialWindow.transform.position = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,Camera.main.transform.position.z + 3f);
		TutorialInfoWindow tiwScript = nwTutorialWindow.AddComponent<TutorialInfoWindow>();
		tiwScript.registerListener("AcScen",this);
		tiwScript.init("SlideshowWindow","Textures/SlideshowSlides/"+LocalisationMang.langCode+"/"+acStrKey,unFormattedHowToSegments);

		pauseScene(true);
	}

	protected void showGoalWindow(string para_goalAppend)
	{
		string goalText = getGoalText();


		Transform goalWindowPrefab = Resources.Load<Transform>("Prefabs/GoalWindow");
		Rect origPrefab2DBounds = CommonUnityUtils.get2DBounds(goalWindowPrefab.FindChild("WindowBounds").renderer.bounds);
		GameObject nwGoalWindow = WorldSpawnHelper.initWorldObjAndBlowupToScreen(goalWindowPrefab,origPrefab2DBounds,new Rect(0,0,0.75f,0.75f));
		nwGoalWindow.transform.position = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,Camera.main.transform.position.z + 3f);
		GoalWindow gwScript = nwGoalWindow.AddComponent<GoalWindow>();
		gwScript.registerListener("AcScen",this);
		gwScript.init(goalText);

		pauseScene(true);
	}

	private string getGoalText()
	{
		string goalText = "Instructions N/A";

		if(lvlConfigGen != null)
		{
			goalText = lvlConfigGen.getInstruction();
		}

		return goalText;

		/*string acNameStr = "";
		string howToText = "";
		
		
		string acStrKey = "";
		
		string[] keyArr = this.getActivityKeys();
		acStrKey = keyArr[0];
		acNameStr = keyArr[1];
		
		
		string goalText = "";
		string unParsedGoalText = LocalisationMang.getString(acStrKey+"*"+"Goal");
		string[] unFormattedGoalSegments = unParsedGoalText.Split('*');
		
		for(int i=0; i<unFormattedGoalSegments.Length; i++)
		{
			goalText += (""+unFormattedGoalSegments[i]);
			
			if(i < (unFormattedGoalSegments.Length-1))
			{
				goalText += "\n";
			}
		}
		goalText += generateGoalTextAppend();

		return goalText;*/
	}

	protected void showSuccessScreen()
	{
		Destroy(GameObject.Find("ActivitySuccessWindow"));
		Transform activitySuccessWindow = Resources.Load<Transform>("Prefabs/ActivitySuccessWindow");
		
		GameObject mainCamera = GameObject.Find("Main Camera");
		Vector3 windowSpawnPos = new Vector3(mainCamera.transform.position.x,
		                                     mainCamera.transform.position.y,
		                                     mainCamera.transform.position.z + 3f);
		
		Transform nwAcSuccessWindow = (Transform) Instantiate(activitySuccessWindow,windowSpawnPos,Quaternion.identity);
		nwAcSuccessWindow.name = "ActivitySuccessWindow";
		ActivitySuccessWindow acwScript = nwAcSuccessWindow.gameObject.AddComponent<ActivitySuccessWindow>();
		acwScript.registerListener("AcScen",this);
		acwScript.init("");

		pauseScene(true);
	}

	protected void showFailScreen()
	{		
		Destroy(GameObject.Find("ActivityFailWindow"));
		Transform activityFailWindow = Resources.Load<Transform>("Prefabs/ActivityFailWindow");
		
		GameObject mainCamera = GameObject.Find("Main Camera");
		Vector3 windowSpawnPos = new Vector3(mainCamera.transform.position.x,
		                                     mainCamera.transform.position.y,
		                                     mainCamera.transform.position.z + 3f);
		
		Transform nwAcFailWindow = (Transform) Instantiate(activityFailWindow,windowSpawnPos,Quaternion.identity);
		nwAcFailWindow.name = "ActivityFailWindow";
		ActivityFailWindow acwScript = nwAcFailWindow.gameObject.AddComponent<ActivityFailWindow>();
		acwScript.registerListener("AcScen",this);
		acwScript.init("");
		pauseScene(true);
	}

	protected void showServerUpdateScreen(List<GetRoutine> para_waitList, List<string> para_readableTaskTextList)
	{

		Debug.Log("WAIT WINDOW");
		Transform processWaitWindowPrefab = Resources.Load<Transform>("Prefabs/ProcessWaitWindow");
		Rect origPrefab2DBounds = CommonUnityUtils.get2DBounds(processWaitWindowPrefab.FindChild("WindowBounds").renderer.bounds);
		GameObject nwProcessWaitWindow = WorldSpawnHelper.initWorldObjAndBlowupToScreen(processWaitWindowPrefab,origPrefab2DBounds,new Rect(0,0,0.75f,0.75f));
		nwProcessWaitWindow.name = "ProcessWaitWindow";
		nwProcessWaitWindow.transform.position = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,Camera.main.transform.position.z + 3f);
		ProcessWaitWindow pwwScript = nwProcessWaitWindow.AddComponent<ProcessWaitWindow>();
		pwwScript.registerListener("AcScen",this);
		pwwScript.init(para_waitList,para_readableTaskTextList);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(((para_sourceID == "ActivityInfoWindow")&&(para_eventID == "OK"))
		||((para_sourceID == "SlideshowWindow")&&(para_eventID == "Close")))
		{
			if( ! hasAlreadyShownGoalOnIntro)
			{
				showGoalWindow(generateGoalTextAppend());
				hasAlreadyShownGoalOnIntro = true;
			}
			else
			{
				pauseScene(false);
			}
		}
		else if(para_sourceID == "PersonHelperWindow")
		{
			if(para_eventID == "Close")
			{
				Destroy(GameObject.Find("PersonHelperWindow"));
				pauseScene(false);
			}
		}
		else if(para_sourceID == "ActivityPauseWindow")
		{
			if(para_eventID == "ReturnToActivity")
			{
				Destroy(GameObject.Find("ActivityPauseWindow"));
				pauseScene(false);
			}
			else if(para_eventID == "Tutorial")
			{
				Destroy(GameObject.Find("ActivityPauseWindow"));
				pauseScene(true);
				showInfoSlideshow();
			}
			else if(para_eventID == "ExitActivity")
			{
				endActivity(acID,false,buildGameyData(),true);
			}
		}
		else if(para_sourceID == "GoalWindow")
		{
			if(para_eventID == "Close")
			{
				isInitialised = true;
				if(serverCommunication != null)
				{
					serverCommunication.startRound(acSessionMetaData.getLauncherDetails());
				}
				pauseScene(false);

			}
		}
		else if(para_sourceID == "ResultViewerWindow")
		{
			if(para_eventID == "Close")
			{
				Destroy(GameObject.Find("ResultViewerWindow"));
				if(endAcServRequestLaunchSuccess)
				{
					// The server request to flush logs and obtain a profile update is sent as soon as the players hit OK in the activity success or fail windows.
					// If the server is not done by the time the player closes the result window, display a progress window indicating that the request is still going on.
					// If an error occurred while trying to launch the server requests then the else branch of this condition should display a relevant popup.

					GameObject.Find("ProcessWaitWindow").GetComponent<ProcessWaitWindow>().performPrep();
				}
				else
				{
					exitActivityScene();
				}
			}else if(para_eventID == "Replay"){

				Destroy(GameObject.Find("ResultViewerWindow"));

				//WAIT AND STORE SERVER UPDATE
				//


				replay();


			}
		}
		else if(para_sourceID == "ProcessWaitWindow")
		{
			if(para_eventID == "Close")
			{
				Destroy(GameObject.Find("ProcessWaitWindow"));
				//needsLocalSeverityUpdate = true;
				exitActivityScene();
			}
		}
		else if(para_sourceID == "ActivitySuccessWindow")
		{
			if(para_eventID == "OK")
			{
				endAcServRequestLaunchSuccess = triggerServerFlushLogsNUpdateRequest(true);
				endActivity(acID,true,buildGameyData(),false);
			}
		}
		else if(para_sourceID == "ActivityFailWindow")
		{
			if(para_eventID == "OK")
			{
				endAcServRequestLaunchSuccess = triggerServerFlushLogsNUpdateRequest(false);
				endActivity(acID,false,buildGameyData(),false);
			}
		}
	}

	protected void performDefaultWinProcedure()
	{
		showSuccessScreen();
		pauseScene(true);
	}

	protected void performDefaultLoseProcedure()
	{
		showFailScreen();
		pauseScene(true);
	}
	

	protected void showPersonHelperWindow()
	{
		bool successFlag = true;
		if(acSessionMetaData == null)
		{
			loadActivitySessionMetaData();
			if(acSessionMetaData == null)	{ successFlag = false;	}
		}

		if(successFlag)
		{
			Destroy(GameObject.Find("PersonHelperWindow"));
			Transform personHelperWindow = Resources.Load<Transform>("Prefabs/PersonHelperWindow");

			GameObject mainCamera = GameObject.Find("Main Camera");
			Vector3 windowSpawnPos = new Vector3(mainCamera.transform.position.x,
			                                     mainCamera.transform.position.y,
			                                     mainCamera.transform.position.z + 3f);


			Rect origPrefab2DBounds = CommonUnityUtils.get2DBounds(personHelperWindow.FindChild("WindowBounds").renderer.bounds);
			GameObject nwHelperWindow = WorldSpawnHelper.initWorldObjAndBlowupToScreen(personHelperWindow,origPrefab2DBounds,new Rect(0,0,0.75f,0.75f));
			nwHelperWindow.transform.position = windowSpawnPos;
			nwHelperWindow.transform.parent = mainCamera.transform;
			nwHelperWindow.name = "PersonHelperWindow";
			PersonHelperWindow phwScript = nwHelperWindow.gameObject.AddComponent<PersonHelperWindow>();
			phwScript.init(acSessionMetaData,getGoalText());
			phwScript.registerListener("AcScen",this);

			pauseScene(true);
		}
	}

	protected void showActivityPauseWindow()
	{
		Destroy(GameObject.Find("ActivityPauseWindow"));
		Transform activityPauseWindow = Resources.Load<Transform>("Prefabs/ActivityPauseWindow");

		GameObject mainCamera = GameObject.Find("Main Camera");
		Vector3 windowSpawnPos = new Vector3(mainCamera.transform.position.x,
		                                     mainCamera.transform.position.y,
		                                     mainCamera.transform.position.z + 3f);
		
		Transform nwAcPauseWindow = (Transform) Instantiate(activityPauseWindow,windowSpawnPos,Quaternion.identity);
		nwAcPauseWindow.name = "ActivityPauseWindow";
		nwAcPauseWindow.parent = mainCamera.transform;
		ActivityPauseWindow apwScript = nwAcPauseWindow.gameObject.AddComponent<ActivityPauseWindow>();
		apwScript.registerListener("AcScen",this);

		pauseScene(true);
	}

	protected void initPersonHelperPortraitPic()
	{
		if(acSessionMetaData != null)
		{
			GameObject personPortraitObj = GameObject.Find("PersonPortrait");
			if(personPortraitObj != null)
			{
				int questGiverID = acSessionMetaData.getQuestGiverID();
				PortraitHelper.replaceEntireDummyPortrait(personPortraitObj,questGiverID,0,"0%",0.04f);
			}
		}
	}



	protected void recordActivityStart()
	{
		bool successFlag = true;
		if(acSessionMetaData == null)
		{
			loadActivitySessionMetaData();
			if(acSessionMetaData == null)	{ successFlag = false;	}
		}
		
		if(successFlag)
		{
			acSessionMetaData.recordActivityStart();
			if(acResultData == null)
			{
				acResultData = new ActivityResult();
			}
		}
	}
	
	protected void recordPresentedConfig(ILevelConfig para_config)
	{
		if(acResultData == null) { acResultData = new ActivityResult(); }
		acResultData.addPresentedContent(para_config);
		Debug.Log("Registered Presented Content");
	}

	protected void recordOutcomeForConfig(LevelOutcome para_outcome)
	{
		if(acResultData == null) { acResultData = new ActivityResult(); }
		acResultData.addOutcomeEntry(para_outcome);
	}


	

	private string[] getActivityKeys()
	{
		string acStrKey = "NONE";
		string acNameStr = "NONE";

		switch(acID)
		{
			case ApplicationID.DROP_CHOPS:   acStrKey = "Ac_SJ";  acNameStr = "Solomon's Junkyard"; break;
			case ApplicationID.SERENADE_HERO: 		acStrKey = "Ac_SH";  acNameStr = "Serenade Hero"; 	   break;
			case ApplicationID.MOVING_PATHWAYS: 	acStrKey = "Ac_MP";  acNameStr = "Moving Pathways";    break;
			case ApplicationID.HARVEST: 			acStrKey = "Ac_H"; 	 acNameStr = "Harvest"; 		   break;
			case ApplicationID.WHAK_A_MOLE: 	acStrKey = "Ac_WAM"; acNameStr = "Whack-A-Monkey";	   break;
			case ApplicationID.MAIL_SORTER: 		acStrKey = "Ac_MS";  acNameStr = "Mail Sorter"; 	   break;
			case ApplicationID.EYE_EXAM : 	acStrKey = "Ac_BB";  acNameStr = "Bridge Builder"; 	   break;
			case ApplicationID.TRAIN_DISPATCHER : 	acStrKey = "Ac_TD";  acNameStr = "Train Dispatcher";   break;
			case ApplicationID.ENDLESS_RUNNER: 	acStrKey = "Ac_PD";  acNameStr = "Package Delivery";   break;
			default: 							acStrKey = "Ac_SJ";  acNameStr = "Solomon's Junkyard"; break;
		}

		string[] retData = new string[2];
		retData[0] = acStrKey;
		retData[1] = acNameStr;
		return retData;
	}

	protected abstract string generateGoalTextAppend();

	protected abstract void initWorld();
	protected abstract void genNextLevel();
	protected abstract void pauseScene(bool para_pauseState);
	protected abstract bool checkNHandleWinningCondition();
	protected abstract bool checkNHandleLosingCondition();
	protected abstract void buildNRecordConfigOutcome(System.Object[] para_extraParams);
	protected abstract GameyResultData buildGameyData();
}

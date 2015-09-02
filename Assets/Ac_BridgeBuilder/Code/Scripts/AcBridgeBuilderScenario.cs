/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;


public class AcBridgeBuilderScenario : ILearnRWActivityScenario, CustomActionListener
{
	public Transform backdropPrefab;
	public Transform leftCliffPrefab;
	public Transform rightCliffPrefab;
	public Transform cliffPaddingPrefab;
	public Transform scafoldingLeftPrefab;
	public Transform scafoldingRightPrefab;
	public Transform scafoldingPlankPrefab;
	public Transform letterTilePrefab;
	public Transform pipeBigPrefab;
	public Transform pipeMidPrefab;
	public Transform pipeSmallPrefab;
	public Transform connectorPrefab;
	public Transform vehiclePrefab;
	public Transform vehicleRearColliderPrefab;
	public Transform wordBoxPrefab;
	public Transform sparksPrefab;
	public Transform clampPrefab;
	public Transform wrenchPrefab;


	BBLevelConfig currLvlConfig;
	bool metaDataLoaded = false;
	bool[] upAxisArr = {false, true, false};

	int currBridgeID;

	Vector3 engineerStartPos;
	Vector3 engineerEndPos;
	
	bool isGoBtnVisible;

	Rect currMaxGameBounds;
	
	int[] latestMistakes;
	
	//bool showAutoResolveBtn;
	int currNumOfMistakes;

	bool firstLevel = true;

	bool showGUI = true;

	bool paused;

	GameObject tmpDescriptionBox;

	int numBridgesRepaired = 0;
	int reqBridgesForWin; // init when level gen is init.


	int numOfCompleteCorrectBridges = 0;
	int numBrokenBridges = 0;
	float startTimestamp = 0;
	float endTimestamp = 0;


	int descriptionType = 0;
	// 0 = text only. 1 = sound only.
	bool descriptionBoxVisible = false;
	bool hasInitIntroSequence = false;



	void Start()
	{
		acID = ApplicationID.EYE_EXAM;
		metaDataLoaded = loadActivitySessionMetaData();
		
		loadTextures();
		prepUIBounds();
		
		initWorld();
		this.initLevelConfigGenerator();
		reqBridgesForWin = lvlConfigGen.getConfigCount();
		genNextLevel();

		initPersonHelperPortraitPic();
		showInfoSlideshow();
		recordActivityStart();
	}



	

	void OnGUI()
	{

		if( ! hasInitGUIStyles)
		{
			availableGUIStyles = new Dictionary<string, GUIStyle>();

			Texture2D blackTex = new Texture2D(1,1);
			blackTex.SetPixel(0,0,Color.black);
			blackTex.Apply();

			GUIStyle autoCompleteBtnStyle = new GUIStyle(GUI.skin.button);
			autoCompleteBtnStyle.normal.background = blackTex;
			autoCompleteBtnStyle.normal.textColor = Color.white;

			availableGUIStyles.Add("AutoResolveBtn",autoCompleteBtnStyle);

			hasInitGUIStyles = true;
		}
		else
		{
			if(isInitialised)
			{
				if(!paused){

				GUI.color = Color.clear;
				if(showGUI)
				{
					if(uiBounds.ContainsKey("PauseBtn"))
					{
						if(GUI.Button(uiBounds["PersonPortrait"],""))
						{
							if(metaDataLoaded)
							{
								showPersonHelperWindow();
							}
						}
						
						if(GUI.Button(uiBounds["PauseBtn"],""))
						{
							showActivityPauseWindow();
						}
					}
				}


				if((isGoBtnVisible)&&(uiBounds.ContainsKey("GoBtn")))
				{
					if(GUI.Button(uiBounds["GoBtn"],"Go"))
					{
						// Trigger engineer to apply repairs.
						Debug.Log("Go!");
						attempts--;
						
						HighlightInputScript his = transform.GetComponent<HighlightInputScript>();
						his.setInputOnState(false);
						setGoBtnVisibility(false);

						//triggerRepairSequence();
						triggerVehicleCrossSequence();//Before on right Cliff

					}
				}

				GUI.color = Color.white;

				if((descriptionBoxVisible)&&(descriptionType == 1))
				{
					GUI.color = Color.clear;

					if(GUI.Button(uiBounds["DescriptionBox"],""))
					{
						try
						{
							if(gameObject.GetComponent<AudioSource>() == null) { gameObject.AddComponent<AudioSource>(); }
							audio.PlayOneShot(WorldViewServerCommunication.tts.say(currLvlConfig.getDescription()));
						}
						catch(System.Exception ex)
						{
							Debug.LogError(ex.Message);
							Debug.LogError("Failed to use TTS");
						}
					}

					GUI.color = Color.white;
				}

				}
			}
		}
	}



	protected override void initWorld()
	{
		// Init World Objects.

		Destroy(GameObject.Find("CliffRight"));
		Destroy(GameObject.Find("ScaffoldR"));
		Destroy(GameObject.Find("ScaffoldFloor"));
		Destroy(GameObject.Find("Bridge"));
		GameObject descriptionDisplayObj = GameObject.Find("Textdisplay");
		descriptionDisplayObj.transform.parent = Camera.main.transform;

		HighlightInputScript his = transform.gameObject.AddComponent<HighlightInputScript>();
		his.wrenchPrefab = wrenchPrefab;
		his.clampPrefab = clampPrefab;
		BridgeManagerScript bms = transform.gameObject.AddComponent<BridgeManagerScript>();
		bms.init(letterTilePrefab,pipeBigPrefab,pipeMidPrefab,pipeSmallPrefab,connectorPrefab,wordBoxPrefab,sparksPrefab);

		//isGoBtnVisible = true;



		// Auto Adjust.
		GameObject tmpPersonPortrait = GameObject.Find("PersonPortrait");
		GameObject tmpPauseButton = GameObject.Find("PauseButton");
		GameObject tmpGoBtnObj = GameObject.Find("GoBtn");
		GameObject tmpHighlightCountArea = GameObject.Find("HighlightCountArea");
		GameObject tmpBackdrop = GameObject.Find("BackdropCollection").transform.FindChild("Backdrop-0").gameObject;
		SpawnNormaliser.adjustGameObjectsToNwBounds(SpawnNormaliser.get2DBounds(tmpBackdrop.renderer.bounds),
		                                            WorldSpawnHelper.getCameraViewWorldBounds(tmpBackdrop.transform.position.z,true),
		                                            new List<GameObject>() { tmpPersonPortrait, tmpPauseButton, tmpGoBtnObj, tmpHighlightCountArea });

		tmpPersonPortrait.transform.parent = Camera.main.transform;
		tmpPauseButton.transform.parent = Camera.main.transform;
		tmpGoBtnObj.transform.parent = Camera.main.transform;
		tmpGoBtnObj.renderer.sortingOrder = 100;
		tmpHighlightCountArea.transform.parent = Camera.main.transform;

		tmpHighlightCountArea.renderer.enabled = false;

		uiBounds.Add("PersonPortrait",WorldSpawnHelper.getWorldToGUIBounds(tmpPersonPortrait.renderer.bounds,upAxisArr));
		uiBounds.Add("PauseBtn",WorldSpawnHelper.getWorldToGUIBounds(tmpPauseButton.renderer.bounds,upAxisArr));		 

		uiBounds.Add("GoBtn",WorldSpawnHelper.getWorldToGUIBounds(tmpGoBtnObj.transform.renderer.bounds,upAxisArr));
		uiBounds.Add("AutoResolveBtn",WorldSpawnHelper.getWorldToGUIBounds(tmpGoBtnObj.transform.renderer.bounds,upAxisArr));
		//showAutoResolveBtn = false;



		uiBounds.Add("DescriptionBox",WorldSpawnHelper.getWorldToGUIBounds(descriptionDisplayObj.renderer.bounds,upAxisArr));


		// Setup cars and drivers.
		setupCars();


		// Init Mechanics Variables.
		currBridgeID = -1;
		GameObject firstCliffLeft = GameObject.Find("CliffLeft");
		firstCliffLeft.name = "CliffLeft-0";
		GameObject firstScaffoldLeft = GameObject.Find("ScaffoldL");
		firstScaffoldLeft.name = "ScaffoldLeft-0";

		setGoBtnVisibility(false);

	}


	protected new bool initLevelConfigGenerator()
	{
		if( ! base.initLevelConfigGenerator())
		{
			// Fallback.
			lvlConfigGen = new BBLevelConfigGeneratorServer(null); //new BBLevelConfigGeneratorHardCoded();
			Debug.LogWarning("Warning: Using Level Gen Fallback");
		}
		return true;
	}



	int attempts;
	protected override void genNextLevel()
	{

		attempts = 2;
		currLvlConfig = (BBLevelConfig) lvlConfigGen.getNextLevelConfig(null);
		if(serverCommunication != null) { serverCommunication.wordDisplayed(currLvlConfig.getBridgeWord(),currLvlConfig.languageArea,currLvlConfig.difficulty); }
		recordPresentedConfig(currLvlConfig);


		// Reset highlights.
		if(firstLevel)
		{
			resetWrenchCounter();
		}


		// Create description text.
		descriptionType = currLvlConfig.getUseTtsFlag() ? 1 : 0;

		if(descriptionType==1){

			if(!WorldViewServerCommunication.tts.test(currLvlConfig.getDescription()))
				descriptionType = 0;

		}


		setupDescriptionBox();


		// Build the bridge.
		currBridgeID++;

		GameObject cliffLeftObj = GameObject.Find("CliffLeft-"+currBridgeID);
		//Bounds cliffLeftBounds = cliffLeftObj.renderer.bounds;

		GameObject scaffoldLeftObj = GameObject.Find("ScaffoldLeft-"+currBridgeID);
		//Bounds scaffoldLeftBounds = scaffoldLeftObj.renderer.bounds;


		Rect bridge2DBounds = transform.GetComponent<BridgeManagerScript>().constructBridge(currBridgeID,currLvlConfig.getBridgeWord());



		// Build bridge right cliff.
		Vector3 rightCliffSpawnPt = new Vector3(bridge2DBounds.x + bridge2DBounds.width - letterTilePrefab.renderer.bounds.size.x + (rightCliffPrefab.renderer.bounds.size.x/2f),
		                                        bridge2DBounds.y - (rightCliffPrefab.renderer.bounds.size.y/2f),
		                                        cliffLeftObj.transform.position.z);
		Transform nwRightCliff = (Transform) Instantiate(rightCliffPrefab,rightCliffSpawnPt,Quaternion.identity);
		nwRightCliff.name = "CliffRight-"+currBridgeID;


		// Build cliff padding.
		Vector3 cliffPaddingSpawnPt = new Vector3(rightCliffSpawnPt.x + (nwRightCliff.renderer.bounds.size.x/2f) + (cliffPaddingPrefab.renderer.bounds.size.x/2f) - 0.01f,rightCliffSpawnPt.y,rightCliffSpawnPt.z);
		Transform nwCliffPadding = (Transform) Instantiate(cliffPaddingPrefab,cliffPaddingSpawnPt,cliffPaddingPrefab.rotation);
		nwCliffPadding.name = "CliffPadding-"+currBridgeID;
		nwCliffPadding.renderer.sortingOrder = -40;
		//Color tmpCol = nwCliffPadding.renderer.material.color;


		// Build next bridge left cliff.
		Vector3 nxtBridgeLeftCliffSpawnPt = new Vector3(rightCliffSpawnPt.x + (rightCliffPrefab.renderer.bounds.size.x/2f) + nwCliffPadding.renderer.bounds.size.x + (leftCliffPrefab.renderer.bounds.size.x/2f) - 0.02f,
		                                                rightCliffSpawnPt.y,
		                                                rightCliffSpawnPt.z);
		Transform nxtBridgeLeftCliff = (Transform) Instantiate(leftCliffPrefab,nxtBridgeLeftCliffSpawnPt,Quaternion.identity);
		nxtBridgeLeftCliff.name = "CliffLeft-"+(currBridgeID+1);


		// Build scafolding right.
		Vector3 rightScaffoldingSpawnPt = new Vector3(bridge2DBounds.x + bridge2DBounds.width + (scafoldingRightPrefab.renderer.bounds.size.x),// - letterTilePrefab.renderer.bounds.size.x + (scafoldingRightPrefab.renderer.bounds.size.x/2f),
		                                              scaffoldLeftObj.transform.position.y,
		                                              scaffoldLeftObj.transform.position.z);
		Transform nwRightScaffold = (Transform) Instantiate(scafoldingRightPrefab,rightScaffoldingSpawnPt,Quaternion.identity);
		nwRightScaffold.name = "ScaffoldRight-"+currBridgeID;


		// Build next scafolding left.

		Vector3 nxtBridgeLeftScaffoldSpawnPt = new Vector3(nxtBridgeLeftCliffSpawnPt.x - (leftCliffPrefab.renderer.bounds.size.x/2f) + (scafoldingRightPrefab.renderer.bounds.size.x/2f), rightScaffoldingSpawnPt.y, rightScaffoldingSpawnPt.z);
		Transform nxtBridgeLeftScaffold = (Transform) Instantiate(scafoldingLeftPrefab,nxtBridgeLeftScaffoldSpawnPt,Quaternion.identity);
		nxtBridgeLeftScaffold.name = "ScaffoldLeft-"+(currBridgeID+1);




		// Extend backdrop
		GameObject backdropCollection = GameObject.Find("BackdropCollection");
		Bounds maxBounds = CommonUnityUtils.findMaxBounds(new List<GameObject>() { cliffLeftObj, nwRightCliff.gameObject });
		Rect max2DBounds = CommonUnityUtils.get2DBounds(maxBounds);

		GameObject rightMostBackdrop = backdropCollection.transform.FindChild("Backdrop-"+(backdropCollection.transform.childCount-1)).gameObject;
		Bounds rightMostBackdropBounds = rightMostBackdrop.renderer.bounds;

		if((max2DBounds.xMax) > (rightMostBackdropBounds.max.x))
		{
			float rem = (max2DBounds.xMax) - (rightMostBackdropBounds.max.x);
			int addBack = (int) (rem / backdropPrefab.renderer.bounds.size.x);
			float addBackDecChecker = (rem % backdropPrefab.renderer.bounds.size.x);

			if(addBackDecChecker != 0)
			{
				addBack++;
			}

			Vector3 nxtBackdropSpawnPt = new Vector3(rightMostBackdropBounds.center.x + backdropPrefab.renderer.bounds.size.x - 0.2f,
			                                         rightMostBackdropBounds.center.y,
			                                         rightMostBackdropBounds.center.z);
			for(int k=0; k<addBack; k++)
			{
				Transform nwBackdrop = (Transform) Instantiate(backdropPrefab,nxtBackdropSpawnPt,Quaternion.identity);
				nwBackdrop.name = "Backdrop-"+backdropCollection.transform.childCount;
				nwBackdrop.parent = backdropCollection.transform;

				nxtBackdropSpawnPt.x += backdropPrefab.renderer.bounds.size.x;
			}
		}


		// Extend scaffold floor.
		GameObject scaffoldFloorCollection = GameObject.Find("ScaffoldFloorCollection");
		
		GameObject rightMostScaffoldFloor = scaffoldFloorCollection.transform.FindChild("ScaffoldFloor-"+(scaffoldFloorCollection.transform.childCount-1)).gameObject;
		Bounds rightMostScaffoldFloorBounds = rightMostScaffoldFloor.renderer.bounds;
		
		if((max2DBounds.xMax) > (rightMostScaffoldFloorBounds.max.x))
		{
			float rem = (max2DBounds.xMax) - (rightMostScaffoldFloorBounds.max.x);
			int addBack = (int) (rem / scafoldingPlankPrefab.renderer.bounds.size.x);
			float addBackDecChecker = (rem % scafoldingPlankPrefab.renderer.bounds.size.x);
			
			if(addBackDecChecker != 0)
			{
				addBack++;
			}
			
			Vector3 nxtScaffoldingFloorSpawnPt = new Vector3(rightMostScaffoldFloorBounds.center.x + scafoldingPlankPrefab.renderer.bounds.size.x - 0.2f,
			                                         rightMostScaffoldFloorBounds.center.y,
			                                         rightMostScaffoldFloorBounds.center.z);
			for(int k=0; k<addBack; k++)
			{
				Transform nwScaffoldFloor = (Transform) Instantiate(scafoldingPlankPrefab,nxtScaffoldingFloorSpawnPt,Quaternion.identity);
				nwScaffoldFloor.name = "ScaffoldFloor-"+scaffoldFloorCollection.transform.childCount;
				nwScaffoldFloor.parent = scaffoldFloorCollection.transform;
				
				nxtScaffoldingFloorSpawnPt.x += scafoldingPlankPrefab.renderer.bounds.size.x;
			}
		}



		// Adjust scroll cam max bounds.

		DragNScroll scrCam = Camera.main.gameObject.GetComponent<DragNScroll>();
		if(scrCam == null) { scrCam = Camera.main.gameObject.AddComponent<DragNScroll>(); }
		scrCam.flipDir = true;

		Rect camWorldBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,false);
		//Rect letterGUIArea = WorldSpawnHelper.getWorldToGUIBounds(new Rect(camWorldBounds.x,bridge2DBounds.y,camWorldBounds.width,letterTilePrefab.renderer.bounds.size.y),1,upAxisArr);
		Rect bridgeTopGUIArea = WorldSpawnHelper.getWorldToGUIBounds(new Rect(camWorldBounds.x,camWorldBounds.y,camWorldBounds.width,camWorldBounds.y - bridge2DBounds.y),1,upAxisArr);
		Rect bridgeBottomGUIArea = WorldSpawnHelper.getWorldToGUIBounds(new Rect(camWorldBounds.x,bridge2DBounds.y - letterTilePrefab.renderer.bounds.size.y,camWorldBounds.width,(bridge2DBounds.y - letterTilePrefab.renderer.bounds.size.y) - (camWorldBounds.y - camWorldBounds.height)),1,upAxisArr);
		scrCam.addGUIScrollArea(bridgeTopGUIArea);
		scrCam.addGUIScrollArea(bridgeBottomGUIArea);

		Transform tmpBackD = backdropCollection.transform.GetChild(0);
		Vector3 worldTL = new Vector3(max2DBounds.x,tmpBackD.renderer.bounds.max.y,Camera.main.transform.position.z);
		Vector3 worldBR = new Vector3(max2DBounds.xMax,tmpBackD.renderer.bounds.min.y,Camera.main.transform.position.z);
		scrCam.setWorldExtents(worldTL,worldBR,3);
		scrCam.freezeY = true;

		currMaxGameBounds = new Rect(max2DBounds.x,tmpBackD.renderer.bounds.max.y,max2DBounds.width,tmpBackD.renderer.bounds.size.y);


		// Place camera at the start of the level bounds.
		Rect camWorld2DBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,false);
		Vector3 currCamPos = Camera.main.transform.position;
		Vector3 nwCamPos = new Vector3(currMaxGameBounds.x + (camWorld2DBounds.width/2f),currCamPos.y,currCamPos.z);

		if(currBridgeID == 0)
		{
			Camera.main.transform.position = nwCamPos;
		}
		else
		{
			MoveToLocation mtl = Camera.main.transform.gameObject.AddComponent<MoveToLocation>();
			mtl.registerListener("AcScen",this);
			mtl.initScript(nwCamPos,cameraSpeed);//4f
		}


		// Engineer stuff.
		Vector3 tmpEngPos = GameObject.Find("Engineer").transform.position;
		if(currBridgeID  == 0)
		{
			engineerStartPos = new Vector3(tmpEngPos.x,tmpEngPos.y,tmpEngPos.z);
		}
		else
		{
			engineerStartPos = new Vector3(cliffLeftObj.renderer.bounds.center.x,tmpEngPos.y,tmpEngPos.z);
			GameObject.Find("Engineer").GetComponent<EngiScript>().moveToNewLevel(currBridgeID);
		}
		engineerEndPos = new Vector3(nwRightCliff.renderer.bounds.center.x,engineerStartPos.y,engineerStartPos.z);


		firstLevel = false;



	}


	bool engiInPosition = false;
	bool carInPosition = false;
	bool carcadeDone = false;

	float cameraSpeed = 8f;


	bool prePauseVisibility = false;

	protected override void pauseScene(bool para_pauseState)
	{
		paused = para_pauseState;


		DragNScroll scrCam = Camera.main.gameObject.GetComponent<DragNScroll>();
		if(scrCam != null) { scrCam.enabled = !paused; }


		if(paused){

			prePauseVisibility = isGoBtnVisible;
			HighlightInputScript his = transform.GetComponent<HighlightInputScript>();

			his.setInputOnState(!paused);
			setGoBtnVisibility(!paused);

		}else{
			HighlightInputScript his = transform.GetComponent<HighlightInputScript>();

			his.setInputOnState(prePauseVisibility);
			setGoBtnVisibility(prePauseVisibility);


		}

	}

	private void toggleGUI(bool para_state)
	{
		showGUI = para_state;

		GameObject tmpPersonPortrait = GameObject.Find("PersonPortrait");
		GameObject tmpPauseButton = GameObject.Find("PauseButton");

		tmpPersonPortrait.renderer.enabled = showGUI;
		tmpPauseButton.renderer.enabled = showGUI;
	}

	protected override bool checkNHandleWinningCondition()
	{
		updateActivityProgressMetaData(numBridgesRepaired/(reqBridgesForWin*1.0f));
		bool playerHasWon = (numBridgesRepaired >= reqBridgesForWin);
		
		if(playerHasWon)
		{
			endTimestamp = Time.time;
			pauseScene(true);
			performDefaultWinProcedure();
		}

		return playerHasWon;
	}

	protected override bool checkNHandleLosingCondition()
	{
		bool playerHasLost = true;
		
		if(playerHasLost)
		{
			endTimestamp = Time.time;
			pauseScene(true);
			performDefaultLoseProcedure();
		}

		return playerHasLost;
	}

	protected override void buildNRecordConfigOutcome(System.Object[] para_extraParams)
	{
		// Build outcome object.
		bool positiveFlag = (bool) para_extraParams[0];

		BBLevelOutcome reqOutcomeObj = null;

		if(positiveFlag)
		{
			reqOutcomeObj = new BBLevelOutcome(true,null);
		}
		else
		{
			//HighlightInputScript his = GameObject.Find("GlobObj").GetComponent<HighlightInputScript>();
			// Add code here.
			reqOutcomeObj = new BBLevelOutcome(false,null);
		}
		
		// Trigger record outcome.
		recordOutcomeForConfig(reqOutcomeObj);
	}

	protected override GameyResultData buildGameyData()
	{
		float timeDiff = endTimestamp - startTimestamp;
		int minutes = (int) (timeDiff / 60f);
		int seconds = (int) (((timeDiff / 60f) - minutes) * 60f);

		BBGameyResultData reqData = new BBGameyResultData(numOfCompleteCorrectBridges,numBrokenBridges,minutes,seconds);
		return reqData;
	}

	protected override string generateGoalTextAppend()
	{
		return (" "+reqBridgesForWin);
	}

	public void destroyOldLevelItems()
	{
		transform.gameObject.GetComponent<BridgeManagerScript>().destroyOldBridge();
	}

	public int[] getPlayerSelections()
	{
		HighlightInputScript his = GameObject.Find("GlobObj").GetComponent<HighlightInputScript>();
		return his.getHighlightLocations();
	}

	public int[] getCorrectSelections()
	{
		List<IndexRegion> baseList = currLvlConfig.convertHighlightListToRegionList(currLvlConfig.getCorrectHighlightAreas());
		IndexRegionTools irt = new IndexRegionTools();
		string binArrStr = irt.createBinArr(baseList,0,currLvlConfig.getBridgeWordLength());
		return irt.createCompactIntArr(binArrStr);
	}

	public void setMistakeCount(int para_mistakeCount)
	{
		currNumOfMistakes = para_mistakeCount;
	}



	private void triggerRepairSequence()
	{

		Debug.Log("Engi goes for repairs (Go)!");

		//toggleGUI(false);


		IndexRegionTools irt = new IndexRegionTools();
		int[] correctPosArr = irt.createCompactIntArr(irt.createBinArr(currLvlConfig.convertHighlightListToRegionList(currLvlConfig.getCorrectHighlightAreas()),0,currLvlConfig.getBridgeWordLength()));
		
		RepairSequence rs = transform.gameObject.AddComponent<RepairSequence>();
		rs.registerListener("AcScen",this);
		rs.init(currMaxGameBounds,engineerStartPos,engineerEndPos,clampPrefab,correctPosArr,false);
	}

	private void triggerVehicleCrossSequence()
	{

		Debug.Log("Red truck go!!");

		GameObject vehicleObj = GameObject.Find("Vehicle");

		GameObject nxtCliffLeft = GameObject.Find("CliffLeft-"+(currBridgeID+1));

		Vector3 vehicleEndPos = new Vector3(nxtCliffLeft.transform.renderer.bounds.min.x,
		                                    vehicleObj.transform.position.y,
		                                    vehicleObj.transform.position.z);

		VehicleCrossScript vcs = transform.gameObject.AddComponent<VehicleCrossScript>();
		vcs.registerListener("AcScen",this);
		vcs.init(currMaxGameBounds,vehicleEndPos,vehicleRearColliderPrefab,letterTilePrefab.renderer.bounds.size.x);
	}

	private void triggerAutoResolveSequence()
	{

		Debug.Log("Engi goes for repairs (Auto REpair)!");
		HighlightInputScript his = transform.GetComponent<HighlightInputScript>();

		//toggleGUI(false);
		setGoBtnVisibility(false);
		his.setInputOnState(false);

		//showAutoResolveBtn = false;
		transform.GetComponent<BridgeManagerScript>().restoreEntireBridge();

		IndexRegionTools irt = new IndexRegionTools();
		his.clearHighlights();
		//his.resetHighlightStateArr(currLvlConfig.getBridgeWordLength(),
		int[] correctPosArr = irt.createCompactIntArr(irt.createBinArr(currLvlConfig.convertHighlightListToRegionList(currLvlConfig.getCorrectHighlightAreas()),0,currLvlConfig.getBridgeWordLength()));
		his.autoHighlightPositions(correctPosArr);

		Transform wrenchCollection = Camera.main.transform.FindChild("WrenchCollection");
		if(wrenchCollection!=null)
			Destroy(wrenchCollection.gameObject);
		else
			Debug.Log("WRENCH COLLECTION ALREADY GONE");

		RepairSequence rs = transform.gameObject.AddComponent<RepairSequence>();
		rs.registerListener("AcScen",this);
		rs.init(currMaxGameBounds,engineerStartPos,engineerEndPos,clampPrefab,correctPosArr,true);


	}

	public new void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		base.respondToEvent(para_sourceID,para_eventID,para_eventData);


		if((isInitialised == true)&&(hasInitIntroSequence == false))
		{
			startTimestamp = Time.time;
			descriptionBoxVisible = true;
			hasInitIntroSequence = true;
			resetWrenchCounter();
//			setGoBtnVisibility(true);//first time

		}


		if(para_sourceID == "Repair")
		{
			if(para_eventID == "EngiAtRightCliff")//Only when done
			{
				Debug.Log("Engi got to right (repair)");
				engiInPosition = true;
				

				//runCarcade();
				
				// Trigger vehicle cross sequence.
			}else if(para_eventID == "EngiAtLeftCliff"){
				
				Debug.Log("Engi got to left (repair)");
				engiInPosition = true;
			}
		}else if(para_sourceID == "Engineer")
		{
			if(para_eventID == "EngiAtRightCliff")//Only when done
			{
				Debug.Log("Engi got to right (Engin)");
				engiInPosition = true;

				// Trigger vehicle cross sequence.
			}else if(para_eventID == "EngiAtLeftCliff"){

				Debug.Log("Engi got to left (Engin)");
				//engiInPosition = true;
			}
		}
		else if(para_sourceID == "CrossingVehicle")
		{
			if(para_eventID == "VehicleCrossed")
			{
				Debug.Log("Vehicle has crossed (Truck?) Engineer should be on the left");
				carInPosition = true;
				engiInPosition = true;//Engineer doesnt move
				currNumOfMistakes = GameObject.Find("Vehicle").transform.FindChild("VehCol").GetComponent<VehicleDamageColliderScript>().getMistakeCount();

			}
		}
		else if(para_sourceID == "AngryReturnScript")
		{
			if(para_eventID == "AngryReturnDone")
			{
				// Trigger new vehicle.
				Debug.Log("DEPRECATED");
				NewVehicleEnterScript nves = transform.gameObject.AddComponent<NewVehicleEnterScript>();
				nves.registerListener("AcScen",this);
				nves.init(vehiclePrefab,currBridgeID);
			}
		}
		else if(para_sourceID == "NewVehicleEnterScript")
		{
			if(para_eventID == "NewVehicleDone")
			{

					resetWrenchCounter();
					transform.GetComponent<BridgeManagerScript>().restoreEntireBridge();

			}
		}
		else if(para_sourceID == Camera.main.name)
		{
			if(para_eventID == "MoveToLocation")
			{
				destroyOldLevelItems();
				maxExtra = 0;
				resetWrenchCounter();
				toggleDescriptionBox(true);

				//Advance the carcade, in case animation wasnt used
				GameObject carcadeObj = GameObject.Find("Carcade");
				Transform vehicleObj = GameObject.Find("Vehicle").transform.FindChild("VehicleBody");
				
				//GameObject nxtCliffRight = GameObject.Find("CliffLeft-"+(currBridgeID));
				//Debug.Log();
//				Vector3 vehicleEndPos = new Vector3(nxtCliffRight.transform.renderer.bounds.min.x-(vehicleObj.renderer.bounds.size.x/2f),
				Vector3 vehicleEndPos = new Vector3(vehicleObj.position.x - (vehicleObj.renderer.bounds.size.x/2f) - (carcadeObj.transform.renderer.bounds.size.x/2f),
				                                    carcadeObj.transform.position.y,
				                                    carcadeObj.transform.position.z);



				carcadeObj.transform.position = vehicleEndPos;




			}
		}
		else if(para_sourceID == "CarcadeScript")
		{
			if(para_eventID == "CarcadeDone")
			{
				carcadeDone = true;

			}
		}


		if(carcadeDone&engiInPosition){//Only after autocorrect

			carcadeDone = false;
			engiInPosition = false;
			//engiInRightPostion = false;
			Debug.Log("NEXT LEVEL (after autocrrect)");

			// Go to next stage.
			numBridgesRepaired++;
			bool winFlag = checkNHandleWinningCondition();
			
			if( ! winFlag)
			{
				genNextLevel();
			}

		}

		if(carInPosition&engiInPosition){
			carInPosition = false;
			engiInPosition= false;
			//Vehicle crossed
			if(currNumOfMistakes == 0)
			{
				Debug.Log("No mistakes, next level without carcade!");


				// Run carcade.
				if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(currLvlConfig.getBridgeWord(),true,currLvlConfig.getDescription(),currLvlConfig.languageArea,currLvlConfig.difficulty); }
				numOfCompleteCorrectBridges++;
				System.Object[] obArr = new object[1] { true };
				buildNRecordConfigOutcome(obArr);
				//runCarcade();

				//carcadeDone = true;
				//engiInPosition = true;

				carcadeDone = false;
				engiInPosition = false;
				//engiInRightPostion = false;
				Debug.Log("NEXT LEVEL");
				
				// Go to next stage.
				numBridgesRepaired++;
				bool winFlag = checkNHandleWinningCondition();
				
				if( ! winFlag)
				{
					genNextLevel();
				}

				//GameObject.Find("Engineer").GetComponent<EngiScript>().moveToRightCliff();


			}
			else
			{

				if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(currLvlConfig.getBridgeWord(),false,currLvlConfig.getDescription(),currLvlConfig.languageArea,currLvlConfig.difficulty); }
				
				System.Object[] obArr = new object[1] { false };
				buildNRecordConfigOutcome(obArr);
				
				transform.GetComponent<BridgeManagerScript>().createErrorOverlay();
				
				if(attempts>0){//Call a new Truck
					Debug.Log("Mistakes, bring a new red truck!");

					NewVehicleEnterScript nves = transform.gameObject.AddComponent<NewVehicleEnterScript>();
					nves.registerListener("AcScen",this);
					nves.init(vehiclePrefab,currBridgeID);
					
				}else{
					Debug.Log("Mistakes, autoresolve!");
					//if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(currLvlConfig.getBridgeWord(),true,currLvlConfig.getDescription(),currLvlConfig.languageArea,currLvlConfig.difficulty); }
					numBrokenBridges++;
					//System.Object[] obArr = new object[1] { true };
					buildNRecordConfigOutcome(obArr);
					triggerAutoResolveSequence();
					runCarcade();
				}
			}

		}

	}

	public void setGoBtnVisibility(bool para_state)
	{
		GameObject goBtnObj = Camera.main.transform.FindChild("GoBtn").gameObject;
		goBtnObj.renderer.enabled = para_state;
		isGoBtnVisible = para_state;
	}

	public void toggleDescriptionBox(bool para_visibilityState)
	{
		if(tmpDescriptionBox == null)
		{
			tmpDescriptionBox = Camera.main.transform.FindChild("Textdisplay").gameObject;
		}

		descriptionBoxVisible = para_visibilityState;
		tmpDescriptionBox.SetActive(para_visibilityState);
	}


	int maxExtra = 0;
	private void resetWrenchCounter()
	{
		HighlightInputScript his = transform.GetComponent<HighlightInputScript>();

		if(maxExtra==0){
		maxExtra = (currLvlConfig.getBridgeWord().Length-currLvlConfig.getTotHighlightPoints())/2;//half of the not used letters

		if(maxExtra>0)
			maxExtra = UnityEngine.Random.Range(1,maxExtra+1);
		else{
			maxExtra = 1;
		}

		}
		his.resetHighlightStateArr(currLvlConfig.getBridgeWordLength(),currLvlConfig.getTotHighlightPoints()+maxExtra);


		if(paused){
			prePauseVisibility = true;
		}else{
			his.setInputOnState(true);
			setGoBtnVisibility(true);
	
		}
	}

	private void runCarcade()
	{
		//teleportCamToStartEdge();
		//GameObject.Find("Engineer").GetComponent<EngiScript>().moveToRightCliff();
		GameObject carcadeObj = GameObject.Find("Carcade");
		GameObject vehicleObj = GameObject.Find("Vehicle");

		CarcadeScript cs = carcadeObj.AddComponent<CarcadeScript>();
		cs.registerListener("AcScen",this);
		cs.init(vehicleObj);



	}

	private void setupCars()
	{

		// Code to setup drivers.
		List<Transform> carList = new List<Transform>();
		carList.Add(GameObject.Find("Vehicle").transform);
		GameObject carcadeObj = GameObject.Find("Carcade");
		for(int i=0; i<carcadeObj.transform.childCount; i++)
		{
			carList.Add(carcadeObj.transform.GetChild(i));
		}

		List<string> availableDriverNames = new List<string>() { "SecChar-0","SecChar-1","SecChar-2" };
		int currIndex = Random.Range(0,availableDriverNames.Count);

		for(int i=0; i<carList.Count; i++)
		{
			Transform tmpCar = carList[i];
			Transform bodyChild = tmpCar.FindChild("VehicleBody");
			Transform reqDriverPrefab = Resources.Load<Transform>("Prefabs/Avatars/"+availableDriverNames[currIndex]);
			Transform nwDriver = (Transform) Instantiate(reqDriverPrefab,tmpCar.FindChild("DriverMarker").position,Quaternion.identity);
			nwDriver.parent = bodyChild;

			nwDriver.FindChild("Torso").FindChild("ShadowL").renderer.enabled = false;
			nwDriver.FindChild("Torso").FindChild("ShadowR").renderer.enabled = false;

			CommonUnityUtils.setSortingOrderOfEntireObject(nwDriver.gameObject,1);

			nwDriver.GetComponent<Animator>().Play("Idle_R");

			currIndex++;
			if(currIndex >= availableDriverNames.Count) { currIndex = 0; }
		}
	}

	private void teleportCamToStartEdge()
	{
			
		Rect camWorld2DBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,false);
		Vector3 currCamPos = Camera.main.transform.position;
		Vector3 camMoveToLoc = new Vector3(currCamPos.x,currCamPos.y,currCamPos.z);
		camMoveToLoc.x = (currMaxGameBounds.x + camWorld2DBounds.width/2f);
		
				
		if(Mathf.Abs(currCamPos.x - camMoveToLoc.x) != 0)
		{
			TeleportToLocation ttl = Camera.main.transform.gameObject.AddComponent<TeleportToLocation>();
			ttl.registerListener("RepairSequence",this);
			ttl.init(camMoveToLoc);
		}
	}

	private void setupDescriptionBox()
	{
		if(descriptionType == 1)
		{
			// Sound only.

			GameObject descriptionDisplayObj = Camera.main.transform.FindChild("Textdisplay").gameObject;
			Transform wordBoxChild = descriptionDisplayObj.transform.FindChild("WordBox");
			if(wordBoxChild != null) { Destroy(wordBoxChild.gameObject); }

			descriptionDisplayObj.transform.FindChild("TtsIcon").renderer.enabled = true;

			if(gameObject.GetComponent<AudioSource>() == null) { gameObject.AddComponent<AudioSource>(); }


			// Button is placed on the box which then plays the sound.
			// OnGUI handles this.
		}
		else 
		{
			// Text only.

			GameObject descriptionDisplayObj = Camera.main.transform.FindChild("Textdisplay").gameObject;
			Transform wordBoxChild = descriptionDisplayObj.transform.FindChild("WordBox");
			if(wordBoxChild != null) { Destroy(wordBoxChild.gameObject); }
			descriptionDisplayObj.transform.FindChild("TtsIcon").renderer.enabled = false;
			Transform textAreaChild = descriptionDisplayObj.transform.FindChild("TextArea");
			
			GameObject nwDescTextObj = WordBuilderHelper.buildWordBox(99,currLvlConfig.getDescription().Replace("/",""),CommonUnityUtils.get2DBounds(textAreaChild.renderer.bounds),textAreaChild.position.z,upAxisArr,wordBoxPrefab);
			nwDescTextObj.name = "WordBox";
			nwDescTextObj.transform.parent = descriptionDisplayObj.transform;
			
			WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() { nwDescTextObj },0.07f);
			
			Destroy(nwDescTextObj.transform.FindChild("Board").gameObject);
			nwDescTextObj.transform.FindChild("Text").renderer.sortingOrder = 20002;
		}

		if( ! firstLevel)
		{
			toggleDescriptionBox(false);
		}
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class AcTrainDispatcherScenario  : ILearnRWActivityScenario, CustomActionListener
{
	public Transform backdropPrefab;
	public Transform groundPrefab;
	public Transform trainFrontPrefab;
	public Transform trainCarriagePrefab;
	public Transform carraigeSiloPrefab;
	public Transform keyboardKeyPrefab;
	public Transform wordBoxPrefab;
	public Transform sfxPrefab;

	public Sprite[] availableSideHeads;


	TDLevelConfig currLvlConfig;
	bool metaDataLoaded = false;

	KeyboardScript ks;

	//bool paused;

	int numOfCorrectTrainsDispatched = 0;
	int reqTrainsForWin; // init when lvl gen init.

	int numCorrectTrains = 0;
	int numIncorrectAttempts = 0;
	float startTimestamp = 0;
	float endTimestamp = 0;


	bool[] upAxisArr = new bool[3] { false, true, false };

	Vector3 trainFrontParkPos;

	List<AbsInputDetector> inputDetectors;

	int currSelCarriage;

	int storageBayCount;

	//Rect fullWorldExtents;

	bool firstLevel = true;
	bool hasRegisteredInit = false;

	bool appendBtnOn;
	bool goBtnOn;
	bool nextWordBtnOn = false;

	//bool detachBtnOn;
	//bool attachBtnOn;
	//bool prevBtnOn;
	//bool nxtBtnOn;

	bool userInputState;

	HashSet<int> validLockedInCarriagesSoFar;
	int indexOfFarthestlockedCarriage = 0;

	bool hasMadeMistakeOnTrain = false;


	void Start()
	{
		acID = ApplicationID.TRAIN_DISPATCHER;
		metaDataLoaded = loadActivitySessionMetaData();
		
		loadTextures();
		prepUIBounds();
		
		initWorld();
		this.initLevelConfigGenerator();
		reqTrainsForWin = lvlConfigGen.getConfigCount();
		genNextLevel();

		initPersonHelperPortraitPic();
		showInfoSlideshow();
		recordActivityStart();
	}


	void OnGUI()
	{
		if(hasRegisteredInit)
		{

					
			if(uiBounds.ContainsKey("PrevCarBtn"))
			{
				GUI.color = Color.clear;

				if(GUI.Button(uiBounds["PrevCarBtn"],"<"))
				{
					if(userInputState)
					{
						performButtonPressEffect("PrevCarBtn");
						triggerMoveToPrevCarriage();
					}
				}

				if(GUI.Button(uiBounds["NxtCarBtn"],">"))
				{
					if(userInputState)
					{
						performButtonPressEffect("NxtCarBtn");
						triggerMoveToNextCarriage();
					}
				}

				if(GUI.Button(uiBounds["BackspaceBtn"],"Backspace"))
				{
					if(userInputState)
					{
						ks.applyBackspace();
					}
				}

				if(GUI.Button(uiBounds["DetachCarBtn"],"Detach\nCarriage"))
				{
					if(userInputState)
					{
						performButtonPressEffect("DetachCarBtn");
						triggerDetachCarriage();
					}
				}

				if(appendBtnOn)
				{
					if(GUI.Button(uiBounds["AttachCarBtn"],"Attach\nCarriage"))
					{
						if(userInputState)
						{
							performButtonPressEffect("AttachCarBtn");
							triggerAddCarriage();
						}
					}
				}

				if(goBtnOn)
				{
					if(GUI.Button(uiBounds["GoBtn"],"Go"))
					{
						if(userInputState)
						{
							performButtonPressEffect("GoBtn");
							PassengerHubController phc = transform.gameObject.GetComponent<PassengerHubController>();
							List<int> correctPlayerCarriages = getCorrectPlayerCarriages();
							HashSet<int> correctCarrSet = new HashSet<int>();
							for(int i=0; i<correctPlayerCarriages.Count; i++)
							{
								if( ! correctCarrSet.Contains(correctPlayerCarriages[i]))
								{
									correctCarrSet.Add(correctPlayerCarriages[i]);
								}
							}

							Vector3 firstCarriagePos = GameObject.Find("Train").transform.FindChild("Carriage-0").transform.position;
							Camera.main.transform.position = new Vector3(firstCarriagePos.x-1f,Camera.main.transform.position.y,Camera.main.transform.position.z);

							setGoButtonVisibility(false);
							setNextWordButtonVisibility(false);
							setUserInputState(false,true);
							phc.launchNewBoardingAttempt(correctCarrSet);
						}
					}
				}

				if(nextWordBtnOn){

					if(GUI.Button(uiBounds["NextWordBtn"],"Go"))
					{
						if(userInputState)
						{
							performButtonPressEffect("Harvest_NextWordButton");
							autocorrection();

						}
					}
				}


				GUI.color = Color.clear;


				GUI.color = Color.clear;

				if(uiBounds.ContainsKey("PauseBtn"))
				{
					if(GUI.Button(uiBounds["PersonPortrait"],""))
					{
						if(metaDataLoaded)
						{

							if(userInputState)
							{

							showPersonHelperWindow();
							}
						}
					}
					
					if(GUI.Button(uiBounds["PauseBtn"],""))
					{

						if(userInputState)
						{

						showActivityPauseWindow();
					
						}
					}
				}
				
				GUI.color = Color.white;

			}
		}
	}


	protected override void initWorld()
	{
		// Create Keyboard.
		Destroy(GameObject.Find("Keyboard"));
		GameObject keyboardGObj = new GameObject("Keyboard");
		//LocalisationMang.init(LanguageCode.GR);
		List<string> keyboardLayout = LocalisationMang.getKeyboardLayout("tablet","lower");

		GameObject backspaceBtnObj = GameObject.Find("BackspaceBtn");
		Rect guiBoundsForBackspaceBtn = WorldSpawnHelper.getWorldToGUIBounds(backspaceBtnObj.transform.renderer.bounds,upAxisArr);
		uiBounds.Add("BackspaceBtn",guiBoundsForBackspaceBtn);
		//backspaceBtnObj.transform.parent = keyboardGObj.transform;


		Rect maxWorldKeyboardBounds = CommonUnityUtils.get2DBounds(GameObject.Find("GroundCollection").transform.FindChild("Ground*0").renderer.bounds);
		//...
		ks = keyboardGObj.AddComponent<KeyboardScript>();
		ks.initKeyboard(keyboardLayout,maxWorldKeyboardBounds,keyboardKeyPrefab,wordBoxPrefab,backspaceBtnObj.transform);
		ks.disableKeyboard();



		// Create Additional Buttons.

		GameObject prevCarBtnObj = GameObject.Find("PrevCarBtn");
		Rect guiBoundsForPrevCarBtn = WorldSpawnHelper.getWorldToGUIBounds(prevCarBtnObj.transform.renderer.bounds,upAxisArr);
		uiBounds.Add("PrevCarBtn",guiBoundsForPrevCarBtn);

		GameObject nextCarBtnObj = GameObject.Find("NxtCarBtn");
		Rect guiBoundsForNextCarBtn = WorldSpawnHelper.getWorldToGUIBounds(nextCarBtnObj.transform.renderer.bounds,upAxisArr);
		uiBounds.Add("NxtCarBtn",guiBoundsForNextCarBtn);




		GameObject detachCarBtnObj = GameObject.Find("DetachCarBtn");
		Rect guiBoundsForDetachCarBtn = WorldSpawnHelper.getWorldToGUIBounds(detachCarBtnObj.transform.renderer.bounds,upAxisArr);
		uiBounds.Add("DetachCarBtn",guiBoundsForDetachCarBtn);

		GameObject attachCarBtnObj = GameObject.Find("AttachCarBtn");
		Rect guiBoundsForAttachCarBtn = WorldSpawnHelper.getWorldToGUIBounds(attachCarBtnObj.transform.renderer.bounds,upAxisArr);
		uiBounds.Add("AttachCarBtn",guiBoundsForAttachCarBtn);

		GameObject goBtnObj = GameObject.Find("GoBtn");
		Rect guiBoundsForGoBtn = WorldSpawnHelper.getWorldToGUIBounds(goBtnObj.transform.renderer.bounds,upAxisArr);
		uiBounds.Add("GoBtn",guiBoundsForGoBtn);


		GameObject nextWord = GameObject.Find("Harvest_NextWordButton");
		Rect guiBoundsForNextWordBtn = WorldSpawnHelper.getWorldToGUIBounds(nextWord.transform.renderer.bounds,upAxisArr);
		uiBounds.Add("NextWordBtn",guiBoundsForNextWordBtn);

		Transform nxtWordBtnTextAreaGuide = nextWord.transform.FindChild("TextArea");
		GameObject nxtWordBoxTextObj = WordBuilderHelper.buildWordBox(99, LocalisationMang.translate("Next word"),
		                                                              CommonUnityUtils.get2DBounds(nxtWordBtnTextAreaGuide.gameObject.renderer.bounds),
		                                                              nxtWordBtnTextAreaGuide.position.z,
		                                                              upAxisArr,
		                                                              wordBoxPrefab);
		nxtWordBoxTextObj.transform.parent = nextWord.transform;
		nxtWordBoxTextObj.transform.FindChild("Text").renderer.sortingOrder = 505;
		Destroy(nxtWordBoxTextObj.transform.FindChild("Board").gameObject);

		Destroy(nxtWordBtnTextAreaGuide.gameObject);
		//nxtWordButtonOrigPos = new Vector3(nextWord.transform.position.x,nextWord.transform.position.y,nextWord.transform.position.z);



		//GameObject carriageDisplayObj = GameObject.Find("CarriagesDisplay");
		//GameObject carriageStorageBayTextArea = carriageDisplayObj.transform.FindChild("TextArea").gameObject;
		//Rect guiBoundsForStorageBayArea = WorldSpawnHelper.getWorldToGUIBounds(carriageStorageBayTextArea.transform.renderer.bounds,upAxisArr);
		//uiBounds.Add("StorageBay",guiBoundsForStorageBayArea);


		prevCarBtnObj.transform.parent = Camera.main.transform;
		nextCarBtnObj.transform.parent = Camera.main.transform;

		detachCarBtnObj.transform.parent = Camera.main.transform;
		attachCarBtnObj.transform.parent = Camera.main.transform;
		goBtnObj.transform.parent = Camera.main.transform;
		nextWord.transform.parent = Camera.main.transform;


		// Auto Adjust.
		GameObject tmpPersonPortrait = GameObject.Find("PersonPortrait");
		GameObject tmpPauseButton = GameObject.Find("PauseButton");
		GameObject tmpBackdrop = GameObject.Find("BackdropCollection").transform.FindChild("Backdrop*0").gameObject;
		SpawnNormaliser.adjustGameObjectsToNwBounds(SpawnNormaliser.get2DBounds(tmpBackdrop.renderer.bounds),
		                                            WorldSpawnHelper.getCameraViewWorldBounds(tmpBackdrop.transform.position.z,true),
		                                            new List<GameObject>() { tmpPersonPortrait, tmpPauseButton });

		tmpPersonPortrait.transform.parent = Camera.main.transform;
		tmpPauseButton.transform.parent = Camera.main.transform;
		
		uiBounds.Add("PersonPortrait",WorldSpawnHelper.getWorldToGUIBounds(tmpPersonPortrait.renderer.bounds,upAxisArr));
		uiBounds.Add("PauseBtn",WorldSpawnHelper.getWorldToGUIBounds(tmpPauseButton.renderer.bounds,upAxisArr));




		// Fix Keyboard, Word Display and Carriage Display to the Camera.
		GameObject wDisplayObj = GameObject.Find("WordDisplay");
		keyboardGObj.transform.parent = Camera.main.transform;
		wDisplayObj.transform.parent = Camera.main.transform;

		//carriageDisplayObj.transform.parent = Camera.main.transform;


		// Input detectors.
		inputDetectors = new List<AbsInputDetector>();
		ClickDetector cd = transform.gameObject.AddComponent<ClickDetector>();
		cd.registerListener("AcScen",this);
		inputDetectors.Add(cd);


		// Set camera to start of the screen.
		Rect firstBackdropBounds = CommonUnityUtils.get2DBounds(tmpBackdrop.renderer.bounds);
		Rect camWorldBounds = WorldSpawnHelper.getCameraViewWorldBounds(2,true);
		Vector3 tmpCamPos = Camera.main.transform.position;
		tmpCamPos.x = firstBackdropBounds.x + (camWorldBounds.width * 0.5f);
		Camera.main.transform.position = tmpCamPos;
	


		// Mechanics Vars Init.
		//paused = false;
		trainFrontParkPos = GameObject.Find("Train").transform.FindChild("TrainFront").position;
		currSelCarriage = -1;
		appendBtnOn = true;
		userInputState = true;
	}

	protected new bool initLevelConfigGenerator()
	{
		if( ! base.initLevelConfigGenerator())
		{
			// Fallback.
			lvlConfigGen = new TDLevelConfigGeneratorServer(null); //new TDLevelConfigGeneratorHardCoded();
			Debug.LogWarning("Warning: Using Level Gen Fallback");
		}
		return true;
	}

	protected override void genNextLevel()
	{
		if( ! firstLevel)
		{
			buildNRecordConfigOutcome(null);
			firstLevel = false;
		}

		currLvlConfig = (TDLevelConfig) lvlConfigGen.getNextLevelConfig(null);
		minAttempts = currLvlConfig.getSpeed()+1;

		if(serverCommunication != null) { serverCommunication.wordDisplayed(currLvlConfig.getTargetWord(),currLvlConfig.languageArea,currLvlConfig.difficulty); }
		recordPresentedConfig(currLvlConfig);


		// Set target word.

		GameObject wDisplay = Camera.main.transform.FindChild("WordDisplay").gameObject;
		GameObject wDisplayTextArea = wDisplay.transform.FindChild("TextArea").gameObject;
		Transform prevRoundInfoBox = wDisplay.transform.FindChild("WordBox");
		if(prevRoundInfoBox != null) { Destroy(prevRoundInfoBox.gameObject); }

		GameObject nwDescWordBox = WordBuilderHelper.buildWordBox(99,currLvlConfig.getTargetWord(),CommonUnityUtils.get2DBounds(wDisplayTextArea.renderer.bounds),wDisplayTextArea.transform.position.z,upAxisArr,wordBoxPrefab);
		WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() {nwDescWordBox},0.1f);
		nwDescWordBox.name = "WordBox";
		Destroy(nwDescWordBox.transform.FindChild("Board").gameObject);
		nwDescWordBox.transform.FindChild("Text").renderer.sortingOrder = 40;
		nwDescWordBox.transform.parent = wDisplay.transform;



		// Init Train.
		Destroy(GameObject.Find("Train"));

		GameObject nwTrain = new GameObject("Train");
		TrainScript ts = nwTrain.AddComponent<TrainScript>();
		ts.registerListener("AcScen",this);
		TrainCreationRetData trainCreationData = ts.createTrain(currLvlConfig.getCarriageItems(),trainFrontParkPos,CommonUnityUtils.get2DBounds(GameObject.Find("BackdropCollection").transform.FindChild("Backdrop*0").renderer.bounds),new List<Transform>() { trainFrontPrefab, trainCarriagePrefab, wordBoxPrefab });

		Rect trainMaxWorldBounds = trainCreationData.trainMaxWorldBounds;
		//int numCarriagesUsed = trainCreationData.numCarriagesUsed;
		int numCarriagesInStorageBay = trainCreationData.numCarriagesInStorageBay;
		storageBayCount = numCarriagesInStorageBay;



		// Setup carriage storage bay display.
		//updateCarriageStorageBayDisplay();





		// Setup camera scroll script.
		DragNScroll scrCam = Camera.main.gameObject.GetComponent<DragNScroll>();
		if(scrCam == null) { scrCam = Camera.main.gameObject.AddComponent<DragNScroll>(); }
		//scrCam.setScrollSpeed(0.5f);
		scrCam.flipDir = true;

		Rect camWorldBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,false);
		GameObject groundCollection = GameObject.Find("GroundCollection");
		Transform firstGround = groundCollection.transform.FindChild("Ground*0");
		Rect firstGroundWorld2D = CommonUnityUtils.get2DBounds(firstGround.renderer.bounds);
		Rect scrollableWorldArea = new Rect(camWorldBounds.x,camWorldBounds.y,camWorldBounds.width,camWorldBounds.y - firstGroundWorld2D.y);
		scrCam.addGUIScrollArea(WorldSpawnHelper.getWorldToGUIBounds(scrollableWorldArea,1,upAxisArr));

		
		GameObject backdropCollection = GameObject.Find("BackdropCollection");
		Transform firstBkdrop = backdropCollection.transform.FindChild("Backdrop*0");
		Vector3 worldTL = new Vector3(firstBkdrop.renderer.bounds.min.x,firstBkdrop.renderer.bounds.max.y,Camera.main.transform.position.z);
		Vector3 worldBR = new Vector3(trainFrontParkPos.x + trainMaxWorldBounds.width + trainCarriagePrefab.renderer.bounds.size.x,firstBkdrop.renderer.bounds.min.y,Camera.main.transform.position.z);
		scrCam.setWorldExtents(worldTL,worldBR,3);
		scrCam.freezeY = true;


		//fullWorldExtents = new Rect(worldTL.x,worldTL.y,worldBR.x - worldTL.x,worldTL.y - worldBR.y);



		adjustBackdrops(trainMaxWorldBounds);


		if(!firstLevel)
		{
			ts.enterTrainStation();
		}

		if(! firstLevel)
		{
			// Passenger Hub Controller.
			PassengerHubController phc = transform.gameObject.GetComponent<PassengerHubController>();
			phc.noticeNewTrainSpawn(1,currLvlConfig.getNumberOfCarriages());
		}

		// User input will be turned on once the train is parked.
		setUserInputState(false);

		setGoButtonVisibility(false);

		setNextWordButtonVisibility(false);
		attemptsThisRound = 0;
		validLockedInCarriagesSoFar = new HashSet<int>();
		indexOfFarthestlockedCarriage = 0;

		hasMadeMistakeOnTrain = false;


		firstLevel = false;
	}

	int attemptsThisRound = 0;

	protected override void pauseScene(bool para_pauseState)
	{

		setUserInputState(!userInputState);
		//paused = para_pauseState;
	}

	protected override bool checkNHandleWinningCondition()
	{
		updateActivityProgressMetaData(numOfCorrectTrainsDispatched/(reqTrainsForWin*1.0f));
		bool playerHasWon = (numOfCorrectTrainsDispatched >= reqTrainsForWin);
		
		if(playerHasWon)
		{
			Camera.main.transform.FindChild("Keyboard").gameObject.SetActive(false);
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
			Camera.main.transform.FindChild("Keyboard").gameObject.SetActive(false);
			endTimestamp = Time.time;
			pauseScene(true);
			performDefaultLoseProcedure();
		}

		return playerHasLost;
	}

	protected override void buildNRecordConfigOutcome(System.Object[] para_extraParams)
	{
		// Build outcome object.
		
		// Trigger record outcome.
		TDLevelOutcome lvlOutcome = new TDLevelOutcome( ! hasMadeMistakeOnTrain);
		recordOutcomeForConfig(lvlOutcome);
	}

	protected override GameyResultData buildGameyData()
	{
		float timeDiff = endTimestamp - startTimestamp;
		int minutes = (int) (timeDiff / 60f);
		int seconds = (int) (((timeDiff / 60f) - minutes) * 60f);

		TDGameyResultData reqData = new TDGameyResultData(numCorrectTrains,numIncorrectAttempts,minutes,seconds);
		return reqData;
	}

	protected override string generateGoalTextAppend()
	{
		return (" "+reqTrainsForWin);
	}

	public new void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		base.respondToEvent(para_sourceID,para_eventID,para_eventData);


		if((!hasRegisteredInit)&&(isInitialised))
		{
			startTimestamp = Time.time;

			hasRegisteredInit = true;
			GameObject.Find("Train").GetComponent<TrainScript>().enterTrainStation();
			PassengerHubController phc = transform.gameObject.AddComponent<PassengerHubController>();
			phc.init(availableSideHeads);
			phc.registerListener("AcScen",this);
			phc.noticeNewTrainSpawn(0,currLvlConfig.getNumberOfCarriages());
			ks.enableKeyboard(null);
		}


		if(para_sourceID == "InputDetector")
		{
			if(para_eventID == "ClickEvent")
			{
				System.Object[] parsedEData = (System.Object[]) para_eventData;
				float[] clickPos = (float[]) parsedEData[0];

				RaycastHit hitInf;
				if(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(clickPos[0],clickPos[1],0)),out hitInf))
				{
					checkNHandleCarriageDirectSelect(hitInf);
				}
			}
		}
		else if(para_sourceID == "TrainScript")
		{
			if(para_eventID == "DetachmentStart")
			{
				adjustCameraMaxBounds();
			}
			else if(para_eventID == "DetachmentEnd")
			{
				int numOfCarriagesDetached = (int) para_eventData;
				storageBayCount += numOfCarriagesDetached;
				//updateCarriageStorageBayDisplay();

				currSelCarriage--;
				carriageFocusEffect(GameObject.Find("Train").transform.FindChild("Carriage-"+currSelCarriage).gameObject);

				setUserInputState(true);
				setGoButtonVisibility(true);

			}
			else if(para_eventID == "AppendEnd")
			{
				appendBtnOn = true;
				adjustCameraMaxBounds();
				setUserInputState(true);
				setGoButtonVisibility(true);

			}
			else if(para_eventID == "TrainParked")
			{
				carriageFocusEffect(GameObject.Find("Train").transform.FindChild("Carriage-0").gameObject);
				setUserInputState(true);
				setGoButtonVisibility(true);
				setNextWordButtonVisibility(true);
			}
			else if(para_eventID == "TrainLeftStation")
			{
				numOfCorrectTrainsDispatched++;
				numCorrectTrains++;
				bool winFlag = checkNHandleWinningCondition();
				if( ! winFlag)
				{
					genNextLevel();
					setUserInputState(true);
				}
				else
				{
					// Last outcome
					buildNRecordConfigOutcome(null);
				}
			}
		}
		else if(para_sourceID == "PHubController")
		{
			if(para_eventID == "CorrectCarriageFilled")
			{
				int nwLockedCarriage = (int) para_eventData;
				if( ! validLockedInCarriagesSoFar.Contains(nwLockedCarriage))
				{
					validLockedInCarriagesSoFar.Add(nwLockedCarriage);
				}

				if(nwLockedCarriage > indexOfFarthestlockedCarriage)
				{
					indexOfFarthestlockedCarriage = nwLockedCarriage;
				}
			}
			else if(para_eventID == "BoardingAttemptEnd")
			{
				//Debug.LogWarning("BOARDING ATTEMPT");
				bool success = triggerCompletionCheck();
				if(! success)
				{
					setUserInputState(true);
					setGoButtonVisibility(true);
					setNextWordButtonVisibility(true);
				}
			}
			else if(para_eventID == "FailedToAttemptBoarding")
			{
				triggerSoundAtCamera("Buzzer_wrong_split");
				hasMadeMistakeOnTrain = true;
				numIncorrectAttempts++;
				attemptsThisRound++;
				setUserInputState(true);
				setGoButtonVisibility(true);
				setNextWordButtonVisibility(true);

			}
		}
		else if(para_sourceID == Camera.main.transform.name)
		{
			if(para_eventID == "MoveToLocation")
			{
				setGoButtonVisibility(true);
				setNextWordButtonVisibility(true);

			}
		}
	}

	private void adjustCameraMaxBounds()
	{
		// Setup camera scroll script.
		DragNScroll scrCam = Camera.main.gameObject.GetComponent<DragNScroll>();
		if(scrCam == null) { scrCam = Camera.main.gameObject.AddComponent<DragNScroll>(); }
		
		//Rect camWorldBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,false);
		//scrCam.addGUIScrollArea(bridgeTopGUIArea);
		//scrCam.addGUIScrollArea(bridgeBottomGUIArea);

		Rect trainMaxWorldBounds = GameObject.Find("Train").GetComponent<TrainScript>().getMaxTrainBounds();
		
		GameObject backdropCollection = GameObject.Find("BackdropCollection");
		Transform firstBkdrop = backdropCollection.transform.FindChild("Backdrop*0");
		Vector3 worldTL = new Vector3(firstBkdrop.renderer.bounds.min.x,firstBkdrop.renderer.bounds.max.y,Camera.main.transform.position.z);
		Vector3 worldBR = new Vector3(trainFrontParkPos.x + trainMaxWorldBounds.width + trainCarriagePrefab.renderer.bounds.size.x,firstBkdrop.renderer.bounds.min.y,Camera.main.transform.position.z);
		scrCam.setWorldExtents(worldTL,worldBR,3);
		scrCam.freezeY = true;

		adjustBackdrops(trainMaxWorldBounds);
	}


	private void performButtonPressEffect(string name)
	{
		triggerSoundAtCamera("BubbleClick");
		
		GameObject nxtWordButtonObj = GameObject.Find(name);

		Vector3 originalScale = nxtWordButtonObj.transform.localScale;
		CustomAnimationManager aniMang = nxtWordButtonObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("InjectLocalScale",1,new List<System.Object>(){new float[3]{0.8f*originalScale.x,0.8f*originalScale.y,0.8f*originalScale.z}}));//new float[3]{0.8f,0.8f,0.8f}}));
		batch1.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>(){ 0.1f }));
		List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
		batch2.Add(new AniCommandPrep("InjectLocalScale",1,new List<System.Object>(){new float[3]{originalScale.x,originalScale.y,originalScale.z}}));//new float[3]{1,1,1}}));
		batchLists.Add(batch1);
		batchLists.Add(batch2);
		//aniMang.registerListener("AcScen",this);
		aniMang.init(name+"PressEffect",batchLists);
	}

	private void adjustBackdrops(Rect para_trainMaxWorldBounds)
	{
		Rect trainMaxWorldBounds = para_trainMaxWorldBounds;

		// Extend backdrop.
		GameObject backdropCollectionObj = GameObject.Find("BackdropCollection");
		GameObject rightMostBackdropObj = backdropCollectionObj.transform.FindChild("Backdrop*"+(backdropCollectionObj.transform.childCount-1)).gameObject;
		Rect rightMostBackdropBounds = CommonUnityUtils.get2DBounds(rightMostBackdropObj.renderer.bounds);
		
		GameObject groundCollectionObj = GameObject.Find("GroundCollection");
		GameObject rightMostGroundObj = groundCollectionObj.transform.FindChild("Ground*"+(groundCollectionObj.transform.childCount-1)).gameObject;
		Rect rightMostGroundBounds = CommonUnityUtils.get2DBounds(rightMostGroundObj.renderer.bounds);
		
		
		if(rightMostBackdropBounds.xMax < trainMaxWorldBounds.xMax)
		{
			float remX = trainMaxWorldBounds.xMax - rightMostBackdropBounds.xMax;
			
			int numBackdropsToAdd = (int) (remX / rightMostBackdropObj.renderer.bounds.size.x);
			
			if((remX % rightMostBackdropObj.renderer.bounds.size.x) != 0)
			{
				numBackdropsToAdd++;
			}

			// Tmp hack until fixed: Apply num of additions to 3;
			//numBackdropsToAdd = 3;
			
			
			Vector3 nxtBackdropSpawnPt = new Vector3(rightMostBackdropBounds.xMax + (backdropPrefab.renderer.bounds.size.x/2f) - 0.04f,
			                                         rightMostBackdropBounds.y - (backdropPrefab.renderer.bounds.size.y/2f),
			                                         rightMostBackdropObj.transform.position.z);
			
			Vector3 nxtGroundSpawnPt = new Vector3(rightMostGroundBounds.xMax + (groundPrefab.renderer.bounds.size.x/2f) - 0.04f,
			                                       rightMostGroundBounds.y - (groundPrefab.renderer.bounds.size.y/2f),
			                                       rightMostGroundObj.transform.position.z);
			
			for(int i=0; i<numBackdropsToAdd; i++)
			{
				Transform nwBkdrop = (Transform) Instantiate(backdropPrefab,nxtBackdropSpawnPt,backdropPrefab.rotation);
				nwBkdrop.name = "Backdrop*"+(backdropCollectionObj.transform.childCount);
				nwBkdrop.parent = backdropCollectionObj.transform;
				
				Transform nwGround = (Transform) Instantiate(groundPrefab,nxtGroundSpawnPt,groundPrefab.rotation);
				nwGround.name = "Ground*"+(groundCollectionObj.transform.childCount);
				nwGround.parent = groundCollectionObj.transform;
				
				nxtBackdropSpawnPt.x += backdropPrefab.renderer.bounds.size.x;
				nxtGroundSpawnPt.x += groundPrefab.renderer.bounds.size.x;
			}
		}
	}

	private void checkNHandleCarriageDirectSelect(RaycastHit para_hitInf)
	{
		if(para_hitInf.collider.gameObject.name.Contains("Carriage"))
		{
			GameObject carriageObj = para_hitInf.collider.gameObject;
			Debug.Log("Hit carriage: "+carriageObj.name);
			carriageFocusEffect(carriageObj);
		}
	}

	private void triggerMoveToPrevCarriage()
	{
		if(currSelCarriage > 0)
		{
			Debug.Log("Moving to prev carriage.");
			carriageFocusEffect(GameObject.Find("Train").transform.FindChild("Carriage-"+(currSelCarriage-1)).gameObject);
		}
	}

	private void triggerMoveToNextCarriage()
	{
		GameObject trainObj = GameObject.Find("Train");
		Transform reqCarriageTrans = trainObj.transform.FindChild("Carriage-"+(currSelCarriage+1));
		if(reqCarriageTrans != null)
		{
			carriageFocusEffect(reqCarriageTrans.gameObject);
		}
	}

	private void carriageFocusEffect(GameObject para_carriageObj)
	{
		GameObject carriageObj = para_carriageObj;

		// Restore original color of previously selected carriage.
		GameObject trainObj = GameObject.Find("Train");
		Transform prevCarriageTrans = trainObj.transform.FindChild("Carriage-"+currSelCarriage);
		if(prevCarriageTrans != null)
		{
			//SpriteRenderer sRend = prevCarriageTrans.gameObject.GetComponent<SpriteRenderer>();
			//sRend.color = Color.white;
			prevCarriageTrans.transform.FindChild("CarriageSelection").renderer.enabled = false;
		}

		/*// Color newly selected carriage.
		SpriteRenderer sRendNw = carriageObj.GetComponent<SpriteRenderer>();
		sRendNw.color = new Color(0.6f,0.6f,0.6f);*/

		// Switch on selection highlight.
		carriageObj.transform.FindChild("CarriageSelection").renderer.enabled = true;

		// Centre camera at carriage.
		Destroy(Camera.main.gameObject.GetComponent<MoveToLocation>());
		MoveToLocation mtl = Camera.main.gameObject.AddComponent<MoveToLocation>();
		mtl.registerListener("AcScen",this);
		mtl.initScript(new Vector3(carriageObj.transform.position.x,Camera.main.gameObject.transform.position.y,Camera.main.gameObject.transform.position.z),10f);


		// Teleport camera to carriage.
		//TeleportToLocation ttl = Camera.main.gameObject.AddComponent<TeleportToLocation>();
		//ttl.init(new Vector3(carriageObj.transform.position.x,Camera.main.gameObject.transform.position.y,Camera.main.gameObject.transform.position.z));

		currSelCarriage = int.Parse(carriageObj.name.Split('-')[1]);
		
		// Init text mode.
		if(validLockedInCarriagesSoFar.Contains(currSelCarriage))
		{
			ks.disableKeyboard();
		}
		else
		{
			TextMesh reqTextDest = carriageObj.transform.FindChild("TextInputObj").transform.Find("Text").GetComponent<TextMesh>();
			ks.enableKeyboard(reqTextDest);
		}
	}



	private void triggerDetachCarriage()
	{
		//Debug.LogWarning("Detach! "+currSelCarriage+" "+indexOfFarthestlockedCarriage);
		if((currSelCarriage > 0)&&(currSelCarriage > indexOfFarthestlockedCarriage))
		{
			//Debug.LogWarning("Detach! ");
			setGoButtonVisibility(false);
			setUserInputState(false);
			setNextWordButtonVisibility(false);
			GameObject.Find("Train").GetComponent<TrainScript>().detachCarriage(currSelCarriage);
		}
	}

	private void triggerAddCarriage()
	{
		if(storageBayCount > 0)
		{
			setGoButtonVisibility(false);
			setUserInputState(false);
			setNextWordButtonVisibility(false);

			Vector3 endPosForNewCarriage = GameObject.Find("Train").GetComponent<TrainScript>().addCarriage();

			Vector3 tmpPos = Camera.main.transform.position;
			tmpPos.x = endPosForNewCarriage.x;
			Camera.main.transform.position = tmpPos;

			storageBayCount--;
			//updateCarriageStorageBayDisplay();

			adjustCameraMaxBounds();
			appendBtnOn = false;
		}
	}

	public List<int> getCorrectPlayerCarriages()
	{
		TrainScript ts = GameObject.Find("Train").GetComponent<TrainScript>();
		string[] playerCarriageItems = ts.getCarriageItems();

		List<int> correctCarriages = currLvlConfig.determineCorrectCarriages(playerCarriageItems);
		return correctCarriages;
	}

	private void autocorrection(){

			TrainScript ts = GameObject.Find("Train").GetComponent<TrainScript>();
			//string[] playerCarriageItems = ts.getCarriageItems();

			Vector3 firstCarriagePos = GameObject.Find("Train").transform.FindChild("Carriage-0").transform.position;
			Camera.main.transform.position = new Vector3(firstCarriagePos.x-1f,Camera.main.transform.position.y,Camera.main.transform.position.z);
			
			// Trigger leave station.
			//if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(currLvlConfig.getTargetWord(),false,"",currLvlConfig.languageArea,currLvlConfig.difficulty); }
			setUserInputState(false);
			ts.leaveStation();

	}


	private bool triggerCompletionCheck()
	{
		TrainScript ts = GameObject.Find("Train").GetComponent<TrainScript>();
		string[] playerCarriageItems = ts.getCarriageItems();

		bool successFlag = currLvlConfig.isCorrectTrainSetup(playerCarriageItems);

		if(successFlag)
		{
			// Teleport camera to first carriage.
			Vector3 firstCarriagePos = GameObject.Find("Train").transform.FindChild("Carriage-0").transform.position;
			Camera.main.transform.position = new Vector3(firstCarriagePos.x-1f,Camera.main.transform.position.y,Camera.main.transform.position.z);

			// Trigger leave station.
			if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(currLvlConfig.getTargetWord(),true,"",currLvlConfig.languageArea,currLvlConfig.difficulty); }
			setUserInputState(false);
			ts.leaveStation();
		}
		else
		{
			if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(currLvlConfig.getTargetWord(),false,"",currLvlConfig.languageArea,currLvlConfig.difficulty); }
			triggerSoundAtCamera("Buzzer_wrong_split");
			hasMadeMistakeOnTrain = true;
			numIncorrectAttempts++;
			attemptsThisRound++;
		}

		return successFlag;
	}

	private void updateCarriageStorageBayDisplay()
	{
		Transform carriagesDisplayTrans = Camera.main.gameObject.transform.FindChild("CarriagesDisplay");
		Transform cdTextAreaTrans = carriagesDisplayTrans.FindChild("TextArea");
		for(int i=0; i<cdTextAreaTrans.childCount; i++)
		{
			Destroy(cdTextAreaTrans.GetChild(i).gameObject);
		}
		/*int numItemsPerRow = (int) ((storageBayCount * 1.0f)/2f);
		if(((storageBayCount * 1.0f) % 2f) != 0)
		{
			numItemsPerRow++;
		}*/

		int numItemsPerRow = 5;
		
		Rect ta2dWorldBounds = CommonUnityUtils.get2DBounds(cdTextAreaTrans.renderer.bounds);
		Rect nxtCarriageSilBounds = new Rect(ta2dWorldBounds.x,ta2dWorldBounds.y,ta2dWorldBounds.width / (numItemsPerRow * 1.0f),ta2dWorldBounds.height / 2.0f);
		
		for(int i=0; i<storageBayCount; i++)
		{
			int colID = ((int) (i % (numItemsPerRow * 1.0f)));
			int rowID = (int) (i / (numItemsPerRow * 1.0f));
			
			nxtCarriageSilBounds.x = ta2dWorldBounds.x + ((colID * 1.0f) * nxtCarriageSilBounds.width);
			nxtCarriageSilBounds.y = ta2dWorldBounds.y - ((rowID * 1.0f) * nxtCarriageSilBounds.height);
			
			GameObject nwSiloObj = WorldSpawnHelper.initObjWithinWorldBounds(carraigeSiloPrefab,("Silo-"+i),nxtCarriageSilBounds,-1,upAxisArr);
			nwSiloObj.renderer.sortingOrder = 500;
			nwSiloObj.transform.parent = cdTextAreaTrans;
		}
	}

	private void setUserInputState(bool para_state)
	{
		setUserInputState(para_state,para_state);
	}

	private void setUserInputState(bool para_state, bool para_scrollOverride)
	{
		userInputState = para_state;

		for(int i=0; i<inputDetectors.Count; i++)
		{
			inputDetectors[i].enabled = para_state;
		}

		ks.enabled = para_state;
		if(validLockedInCarriagesSoFar != null)
		{
			if(validLockedInCarriagesSoFar.Contains(currSelCarriage))
			{
				ks.disableKeyboard();
			}
		}

		DragNScroll scrCam = Camera.main.gameObject.GetComponent<DragNScroll>();
		if(scrCam != null) { scrCam.enabled = para_scrollOverride; }
	}

	private void setGoButtonVisibility(bool para_state)
	{
		goBtnOn = para_state;
		GameObject goBtn = GameObject.Find("GoBtn");
		goBtn.renderer.enabled = para_state;
	}


	int minAttempts = 3;
	private void setNextWordButtonVisibility(bool para_state){

		if((para_state==true)&&(attemptsThisRound<minAttempts))
				return;
		nextWordBtnOn = para_state;
		GameObject goBtn = GameObject.Find("Harvest_NextWordButton");
		goBtn.renderer.enabled = para_state;
		//Transform textArea = goBtn.transform.Find("TextArea");
		//textArea.renderer.enabled = para_state;

		goBtn.transform.Find("WordBox99").Find("Text").renderer.enabled = para_state;




	}


	private void setDetachButtonVisiblity(bool para_state)
	{
		//detachBtnOn = para_state;
		GameObject detachBtn = GameObject.Find("DetachCarBtn");
		//detachBtn.renderer.enabled = para_state;
		SpriteRenderer sRend = detachBtn.GetComponent<SpriteRenderer>();
		if(para_state) { sRend.color = Color.white; }
		else { sRend.color = Color.gray; }
	}

	private void setAttachButtonVisibility(bool para_state)
	{
		//attachBtnOn = para_state;
		GameObject attachBtn = GameObject.Find("AttachCarBtn");
		//attachBtn.renderer.enabled = para_state;
		SpriteRenderer sRend = attachBtn.GetComponent<SpriteRenderer>();
		if(para_state) { sRend.color = Color.white; }
		else { sRend.color = Color.gray; }
	}

	private void setPrevButtonVisibility(bool para_state)
	{
	//	prevBtnOn = para_state;
		GameObject prevBtn = GameObject.Find("PrevCarBtn");
		//prevBtn.renderer.enabled = para_state;
		SpriteRenderer sRend = prevBtn.GetComponent<SpriteRenderer>();
		if(para_state) { sRend.color = Color.white; }
		else { sRend.color = Color.gray; }
	}

	private void setNextButtonVisibility(bool para_state)
	{
		//nxtBtnOn = para_state;
		GameObject nextBtn = GameObject.Find("NxtCarBtn");
		//nextBtn.renderer.enabled = para_state;
		SpriteRenderer sRend = nextBtn.GetComponent<SpriteRenderer>();
		if(para_state) { sRend.color = Color.white; }
		else { sRend.color = Color.gray; }
	}

	private void triggerSoundAtCamera(string para_soundFileName)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		DontDestroyOnLoad(nwSFX);
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = 0.5f;
		audS.Play();
	}



}
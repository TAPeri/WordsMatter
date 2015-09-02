/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class AcHarvestScenario : ILearnRWActivityScenario, CustomActionListener
{
	
	public Transform wordBoxPrefab;
	public Transform holePrefab;
	public Transform[] flowerPrefabArr;
	public Transform essencePowderPrefab;
	public Transform sfxPrefab;

	
	HarvestLevelConfig currLvlConfig;
	bool[] upAxisArr = { false, true, false };



	Rect mainWordSpawnWorldBounds;
	float mainWordZVal;
	float flowerZVal;

	List<Vector3> machineTextCentres;

	//Vector3 startWordBoxSize;
	//Vector3 endWordBoxSize;
	//float startFontCharacterSize;
	//float endFontCharacterSize;


	

	Vector3 origWordPos;
	
	//float timeToCompleteMachineRun_inSec = 1f;
	//float timeToCompleteMoveBack_inSec = 1f;

	//float maxTimePerWord_Sec = 8;

	//string debugSolutionStr = "";


	string nameOfDragItem;
	GameObject currObjBeingDragged;
	//Vector3 dragRestPosition;




	List<string> harvestMachineGObjNames;



	float desiredMaxFontCharSize_harvestWord = 0.08f;
	//float desiredMaxFontCharSize_machineDesc = 0.08f;


	int currRoundAttempts;
	bool paused;


	int numHarvestedWords = 0;
	int reqHarvestsForWin; //init when level gen is init.


	List<string> unusableMachines;

	List<Vector3> flowerSpawnPts;
	List<Vector2> scrollPosList;

	bool renderSpecialCam = false;
	Camera flowerCam;
	string hoverMachine;

	Dictionary<string,string> machineFlowerLookup;
	Dictionary<string,bool> machineOutcomeLookup;

	bool nxtWordButtonEnabled;
	Vector3 nxtWordButtonOrigPos;

	bool showGUI = true;
	bool metaDataLoaded = false;

	Vector2 tmpVect = new Vector2();


	int numCorrectHarvests = 0;
	int numMachinesBroken = 0;
	float startTimestamp = 0;
	float endTimestamp = 0;

	List<int> machinesGivenGoodInput;
	List<int> machinesGivenBadInput;
	//bool hasRegisteredConfigDisplay = false;



	
	void Start()
	{
		acID = ApplicationID.HARVEST;
		metaDataLoaded = loadActivitySessionMetaData();

		loadTextures();
		prepUIBounds();

		initWorld();
		this.initLevelConfigGenerator();
		reqHarvestsForWin = lvlConfigGen.getConfigCount();
		genNextLevel();

		initPersonHelperPortraitPic();
		showInfoSlideshow();
		recordActivityStart();
	}


	void OnGUI()
	{
		if(showGUI)
		{
			if(renderSpecialCam)
			{
				if( ! hasInitGUIStyles)
				{
					this.prepGUIStyles();
					hasInitGUIStyles = true;
				}
				else
				{
					GUI.color = Color.clear;
					if(nxtWordButtonEnabled)
					{
						if(GUI.Button(uiBounds["NextWordBtn"],""))
						{
							if(! paused)
							{


								performNextWordButtonPressEffect();

								
								//if(currRoundAttempts >= currLvlConfig.correctMachines.Length)
								//{
								numHarvestedWords++;
								buildNRecordConfigOutcome(null);
								bool isWin = checkNHandleWinningCondition();
								if(isWin)
								{

									toggleNextWordButtonVisibility(true);
								}
								//	toggleNextWordButtonVisibility(true);
								//}



								//Vince//buildNRecordConfigOutcome(null);



								genNextLevel();
							}
						}
					}

					GUI.color = Color.black;
					if(uiBounds.ContainsKey("MachineTextArea-1"))
					{
						for(int i=1; i<currLvlConfig.machineDescriptions.Length+1; i++)
						{
							Rect reqArea = uiBounds["MachineTextArea-"+i];
							Vector2 reqScrollVar = scrollPosList[(i-1)];


							if((Application.platform == RuntimePlatform.Android)
							   ||(Application.platform == RuntimePlatform.IPhonePlayer))
							{
								if(Input.touchCount == 1)
								{
									tmpVect.x = Input.touches[0].position.x;
									tmpVect.y = Screen.height - Input.touches[0].position.y;
									
									if(reqArea.Contains(tmpVect))
									{
										reqScrollVar.y += (Input.touches[0].deltaPosition.y * 1f);
										scrollPosList[(i-1)] = reqScrollVar;
									}
								}
							}


							GUILayout.BeginArea(reqArea);
							scrollPosList[(i-1)] = GUILayout.BeginScrollView(scrollPosList[(i-1)],GUIStyle.none,GUIStyle.none,GUILayout.Width(reqArea.width),GUILayout.Height(reqArea.height));
							
							GUI.color = Color.black;
							GUILayout.FlexibleSpace();
							GUILayout.Label(currLvlConfig.machineDescriptions[i-1],availableGUIStyles["MachineDescription"]);
							GUILayout.FlexibleSpace();

							GUILayout.EndScrollView();
							GUILayout.EndArea();
						}
					}

					GUI.color = Color.clear;
					if(uiBounds.ContainsKey("PauseBtn"))
					{
						if(GUI.Button(uiBounds["PersonPortrait"],""))
						{
							if(metaDataLoaded)
							{
								showGUI = false;
								showPersonHelperWindow();
							}
						}
						
						if(GUI.Button(uiBounds["PauseBtn"],""))
						{
							showGUI = false;
							showActivityPauseWindow();
						}
					}
				
					GUI.color = Color.white;

					/*GUI.color = Color.black;
					GUI.Label(uiBounds["DebugSolutionStrLabel"],debugSolutionStr);
					GUI.color = Color.white;*/



				}


			}


			if(renderSpecialCam)
			{
				if(Event.current.type == EventType.Repaint)
				{
					flowerCam.Render();
				}
			}
		}
	}



	protected override void initWorld()
	{

		DragScript dScript = (GameObject.Find("GlobObj").GetComponent<DragScript>());
		dScript.registerListener("AcScen",this);


		// Get Main Word display panel.
		GameObject mainWordDisplayPanel = GameObject.Find("Harvest_WordTile");
		mainWordSpawnWorldBounds = new Rect(mainWordDisplayPanel.transform.position.x - (mainWordDisplayPanel.transform.renderer.bounds.size.x/2f),
		                                    mainWordDisplayPanel.transform.position.y + (mainWordDisplayPanel.transform.renderer.bounds.size.y/2f),
		                                	mainWordDisplayPanel.transform.renderer.bounds.size.x,
		                                    mainWordDisplayPanel.transform.renderer.bounds.size.y);
		origWordPos = new Vector3(mainWordSpawnWorldBounds.x + (mainWordSpawnWorldBounds.width/2f),
		                          mainWordSpawnWorldBounds.y - (mainWordSpawnWorldBounds.height/2f),
		                          mainWordDisplayPanel.transform.position.z);
		mainWordZVal = origWordPos.z;
		flowerZVal = mainWordZVal - (0.5f + 0.1f);
		Destroy(mainWordDisplayPanel);


		// Next word button text
		GameObject nxtWordBtnObj = GameObject.Find("Harvest_NextWordButton");
		Transform nxtWordBtnTextAreaGuide = nxtWordBtnObj.transform.FindChild("TextArea");
		GameObject nxtWordBoxTextObj = WordBuilderHelper.buildWordBox(99, LocalisationMang.translate("Next word"),
		                               CommonUnityUtils.get2DBounds(nxtWordBtnTextAreaGuide.gameObject.renderer.bounds),
		                               nxtWordBtnTextAreaGuide.position.z,
		                               upAxisArr,
		                               wordBoxPrefab);
		nxtWordBoxTextObj.transform.parent = nxtWordBtnObj.transform;
		nxtWordBoxTextObj.transform.FindChild("Text").renderer.sortingOrder = 2;
		Destroy(nxtWordBoxTextObj.transform.FindChild("Board").gameObject);
		Destroy(nxtWordBtnTextAreaGuide.gameObject);
		nxtWordButtonOrigPos = new Vector3(nxtWordBtnObj.transform.position.x,nxtWordBtnObj.transform.position.y,nxtWordBtnObj.transform.position.z);


		// Get Machine Panels.
		machineTextCentres = new List<Vector3>();
		Vector3 machineTASize = new Vector3(1,1,1);
		harvestMachineGObjNames = new List<string>();

		scrollPosList = new List<Vector2>();


		bool foundMachine = true;
		int counter = 1;
		while(foundMachine)
		{
			string nxtHarvestMachine = ("Harvest_Machine"+counter);
			GameObject harvestMachineObj = GameObject.Find(nxtHarvestMachine);
			if(harvestMachineObj == null)
			{
				foundMachine = false;
			}
			else
			{
				// Register name. 
				harvestMachineGObjNames.Add(harvestMachineObj.name);


				// Assign Hole Object.
				Transform tArea = harvestMachineObj.transform.FindChild("TextArea");
				Bounds tmpBounds = harvestMachineObj.transform.renderer.bounds;
				float holeWidth = tmpBounds.size.x;
				float holeHeight = tmpBounds.size.y * 0.1f;
				Rect holeObjWorldBounds = new Rect(tmpBounds.center.x - (holeWidth/2f),
				                                   tArea.renderer.bounds.min.y + (holeHeight/2f),
				                                   holeWidth,
				                                   holeHeight);

				GameObject nwHoleObj = WorldSpawnHelper.initObjWithinWorldBounds(holePrefab,1,1,"Hole-"+counter,holeObjWorldBounds,null,harvestMachineObj.transform.position.z + 0.1f,upAxisArr);
				nwHoleObj.transform.parent = harvestMachineObj.transform;
				BoxCollider bCol = nwHoleObj.GetComponent<BoxCollider>();
				Vector3 tmpBColSize = bCol.size;
				tmpBColSize.z = 10f;
				bCol.size = tmpBColSize;
				HoleScript hScript = nwHoleObj.GetComponent<HoleScript>();
				hScript.registerListener("AcScen",this);
				nwHoleObj.gameObject.collider.enabled = false;



				// Get Text Area Centre.
				tArea = harvestMachineObj.transform.FindChild("TextArea");
				if(tArea != null)
				{
					uiBounds.Add("MachineTextArea-"+counter,WorldSpawnHelper.getWorldToGUIBounds(tArea.renderer.bounds,upAxisArr));
					scrollPosList.Add(new Vector2());

					Vector3 machineTACentre = tArea.renderer.bounds.center;
					Vector3 squashedTACentre = new Vector3(machineTACentre.x,machineTACentre.y,machineTACentre.z);
					machineTextCentres.Add(squashedTACentre);

					machineTASize.x = tArea.renderer.bounds.size.x;
					machineTASize.y = tArea.renderer.bounds.size.y;
					machineTASize.z = 1;
				}
				tArea.collider.enabled = false;
				tArea.renderer.enabled = false;
				//Destroy(tArea.gameObject);


				// Anim set speed to 0.
				Animator machAni = harvestMachineObj.GetComponent<Animator>();
				machAni.speed = 0;
			}
			counter++;
		}




		//startWordBoxSize = new Vector3(mainWordSpawnWorldBounds.width,mainWordSpawnWorldBounds.height,1);
		//endWordBoxSize = new Vector3(machineTASize.x,machineTASize.y,machineTASize.z);
		//startFontCharacterSize = 10;	// Set individually within genNextLevel.
		//endFontCharacterSize = 10;		// Set individually within genNextLevel.


		// Set all machine glows off.
		foundMachine = true;
		counter = 1;
		while(foundMachine)
		{
			string nxtHarvestMachine = ("Harvest_Machine"+counter);
			GameObject harvestMachineObj = GameObject.Find(nxtHarvestMachine);
			if(harvestMachineObj == null)
			{
				foundMachine = false;
			}
			else
			{
				toggleMachineGlow(nxtHarvestMachine,false);
			}
			counter++;
		}

		// Setup Next Word Button.
		Rect nxtWordBtnGUIBounds = WorldSpawnHelper.getWorldToGUIBounds(GameObject.Find("Harvest_NextWordButton").renderer.bounds,upAxisArr);
		uiBounds.Add("NextWordBtn",nxtWordBtnGUIBounds);


		// Auto Adjust.
		GameObject tmpPersonPortrait = GameObject.Find("PersonPortrait");
		GameObject tmpPauseButton = GameObject.Find("PauseButton");
		GameObject tmpBackdrop = GameObject.Find("Harvest_background");
		SpawnNormaliser.adjustGameObjectsToNwBounds(SpawnNormaliser.get2DBounds(tmpBackdrop.renderer.bounds),
		                                            WorldSpawnHelper.getCameraViewWorldBounds(tmpBackdrop.transform.position.z,true),
		                                            new List<GameObject>() { tmpPersonPortrait, tmpPauseButton });


		tmpPersonPortrait.transform.parent = GameObject.Find("Main Camera").transform;
		tmpPauseButton.transform.parent = GameObject.Find("Main Camera").transform;

		uiBounds.Add("PersonPortrait",WorldSpawnHelper.getWorldToGUIBounds(tmpPersonPortrait.renderer.bounds,upAxisArr));
		uiBounds.Add("PauseBtn",WorldSpawnHelper.getWorldToGUIBounds(tmpPauseButton.renderer.bounds,upAxisArr));		 


		/*// Setup Timer.
		Rect timerGUIBounds = WorldSpawnHelper.getWorldToGUIBounds(GameObject.Find("TimerArea").renderer.bounds,upAxisArr);
		uiBounds.Add("Timer",timerGUIBounds);
		TimerScript timerScrpt = GameObject.Find("GlobObj").AddComponent<TimerScript>();
		timerScrpt.registerListener("AcScen",this);
		timerScrpt.init(timerGUIBounds,maxTimePerWord_Sec);
		timerScrpt.pauseTimer(true);*/


		
		unusableMachines = new List<string>();

		flowerCam = GameObject.Find("ExperimentalCam").GetComponent<Camera>();
		


		paused = false;
	}




	protected new bool initLevelConfigGenerator()
	{
		if( ! base.initLevelConfigGenerator())
		{
			// Fallback.
			lvlConfigGen = new HarvestLevelConfigGeneratorServer(null); //new HarvestLevelConfigGeneratorHardCoded();
			Debug.LogWarning("Warning: Using Level Gen Fallback");
		}
		return true;
	}

	protected override void genNextLevel()
	{

		currLvlConfig = (HarvestLevelConfig) lvlConfigGen.getNextLevelConfig(null);
		//debugSolutionStr = currLvlConfig.getSolutionString();



		if(numHarvestedWords<reqHarvestsForWin){
		recordPresentedConfig(currLvlConfig);
			if(serverCommunication != null) { serverCommunication.wordDisplayed(currLvlConfig.harvestingWord,currLvlConfig.languageArea,currLvlConfig.difficulty); }
		}

		// Destroy old World Objs.
		Destroy(GameObject.Find("WordToHarvest"));


		for(int i=0; i<4; i++)
		{
			GameObject reqMachineObj = GameObject.Find("Harvest_Machine"+(i+1));

			if(reqMachineObj!= null){
				if(i>=currLvlConfig.machineDescriptions.Length){
					Debug.Log("Destroy Harvest_Machine"+(i+1));
					Destroy(reqMachineObj);
				}else{
					Transform descObj = reqMachineObj.transform.FindChild("MachineDescription");
					if(descObj != null)
					{
						Destroy(descObj.gameObject);
					}
				}

			}
		}

		//bool foundFlower = true;
		for(int flowerCounter=0; flowerCounter<4; flowerCounter++)
		{

			GameObject flowerToDestroy = GameObject.Find("Flower-"+flowerCounter);
			if(flowerToDestroy == null)
			{
				//foundFlower = false;
			}
			else
			{
				Destroy(GameObject.Find("Flower-"+flowerCounter));
			}
		}




		// Create new World Objs.


		// Init Harvest Word.
		string wordStrToHarvest = currLvlConfig.harvestingWord;
		GameObject wordToHarvestObj = WordBuilderHelper.buildWordBox(0,wordStrToHarvest,mainWordSpawnWorldBounds,mainWordZVal,upAxisArr,wordBoxPrefab);
		Destroy(wordToHarvestObj.transform.FindChild("Board").gameObject);
		wordToHarvestObj.name = "WordToHarvest";
		wordToHarvestObj.layer = 0;

		TextNBoardScript tnbScript = wordToHarvestObj.GetComponent<TextNBoardScript>();
		tnbScript.setBoardColor(205,133,63,255);


		//startFontCharacterSize = WordBuilderHelper.getCharacterSizeForBounds(mainWordSpawnWorldBounds.width,wordStrToHarvest.Length);
		//endFontCharacterSize = WordBuilderHelper.getCharacterSizeForBounds(endWordBoxSize.x,wordStrToHarvest.Length);

		WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject> { wordToHarvestObj }, desiredMaxFontCharSize_harvestWord);






		// Init flowers.

		int numOfFlowers = currLvlConfig.correctMachines.Length;

		List<int> flowerPrefabSelections = new List<int>();
		float totalWidthForFlowers = 0;
		for(int i=0; i<numOfFlowers; i++)
		{
			int randPrefabIndex = Random.Range(0,flowerPrefabArr.Length);
			flowerPrefabSelections.Add(randPrefabIndex);
			totalWidthForFlowers += flowerPrefabArr[randPrefabIndex].renderer.bounds.size.x;
		}

		GameObject flowerTableObj = GameObject.Find("Harvest_Table");
		Rect flowerTableBounds = CommonUnityUtils.get2DBounds(flowerTableObj.renderer.bounds);

		float remWidthOnTable = flowerTableBounds.width - totalWidthForFlowers;
		float flowerPadding = 0;
		if(remWidthOnTable > 0)
		{
			flowerPadding = (remWidthOnTable / ((numOfFlowers-1) * 1.0f));
		}
		if(flowerPadding > (flowerTableBounds.width * 0.2f)) { flowerPadding = (flowerTableBounds.width * 0.1f); }

		float totalReqWidthFlowersNPadding = totalWidthForFlowers + (flowerPadding * (numOfFlowers-1));
		Vector3 nxtFlowerSpawnPt = new Vector3(flowerTableBounds.x + (flowerTableBounds.width/2f) - (totalReqWidthFlowersNPadding/2f) + (flowerPrefabArr[flowerPrefabSelections[0]].renderer.bounds.size.x/2f),
		                                       flowerTableBounds.y + (flowerPrefabArr[flowerPrefabSelections[0]].renderer.bounds.size.y/2f) - 1f,
		                                       flowerTableObj.transform.position.z - 0.1f);

		if(flowerSpawnPts != null) { flowerSpawnPts.Clear(); } else { flowerSpawnPts = new List<Vector3>(); }
		for(int i=0; i<flowerPrefabSelections.Count; i++)
		{
			Transform reqFlowerPrefab = flowerPrefabArr[flowerPrefabSelections[i]];
			Transform nwFlower = (Transform) Instantiate(reqFlowerPrefab,nxtFlowerSpawnPt,Quaternion.identity);
			nwFlower.localScale = new Vector3(nwFlower.localScale.x * 0.75f,nwFlower.localScale.y * 0.75f,nwFlower.localScale.z);
			nwFlower.name = ("Flower-"+i);

			flowerSpawnPts.Add(new Vector3(nxtFlowerSpawnPt.x,nxtFlowerSpawnPt.y,nxtFlowerSpawnPt.z));

			if(i < (flowerPrefabSelections.Count-1))
			{
				nxtFlowerSpawnPt.x += (reqFlowerPrefab.renderer.bounds.size.x/2f) + flowerPadding + (flowerPrefabArr[flowerPrefabSelections[i+1]].renderer.bounds.size.x/2f);
			}
		}






		for(int i=0; i<currLvlConfig.machineDescriptions.Length; i++)
		{
			GameObject reqMachineObj = GameObject.Find("Harvest_Machine"+(i+1));

			Transform reqHoleObj = reqMachineObj.transform.FindChild("Hole-"+(i+1));

			HoleScript hScript = reqHoleObj.GetComponent<HoleScript>();
			hScript.manualDisownProcedure();
			//hScript.registerListener("AcScen",this);
			//nwHoleObj.gameObject.collider.enabled = false;
		}


		
		/*// Init Machine Descriptions.
		List<GameObject> tmpMDescObjList = new List<GameObject>();
		for(int i=0; i<currLvlConfig.machineDescriptions.Length; i++)
		{
			GameObject reqMachineObj = GameObject.Find("Harvest_Machine"+(i+1));


			string machineDescStr = currLvlConfig.machineDescriptions[i];

			Vector3 reqMachineTACntre = machineTextCentres[i];
			Rect machineTABounds = new Rect(reqMachineTACntre.x - (endWordBoxSize.x/2f),reqMachineTACntre.y + (endWordBoxSize.y/2f),endWordBoxSize.x,endWordBoxSize.y);

			GameObject nwMachineDescBox = WordBuilderHelper.buildWordBox(9,machineDescStr,machineTABounds,reqMachineTACntre.z,upAxisArr,wordBoxPrefab);
			nwMachineDescBox.name = "MachineDescription";
			nwMachineDescBox.layer = 0;
			nwMachineDescBox.transform.FindChild("Board").gameObject.SetActive(false);
			nwMachineDescBox.transform.FindChild("Text").renderer.sortingOrder = 50;
			nwMachineDescBox.transform.parent = reqMachineObj.transform;
			tmpMDescObjList.Add(nwMachineDescBox);
		}
		WordBuilderHelper.setBoxesToUniformTextSize(tmpMDescObjList,desiredMaxFontCharSize_machineDesc);*/

		
		hoverMachine = null;
		if(machineFlowerLookup != null) { machineFlowerLookup.Clear(); } else { machineFlowerLookup = new Dictionary<string, string>(); }
		if(machineOutcomeLookup != null) { machineOutcomeLookup.Clear(); } else { machineOutcomeLookup = new Dictionary<string, bool>(); }


		for(int i=0; i<unusableMachines.Count; i++)
		{
			toggleMachineColor(unusableMachines[i],Color.white);
		}
		unusableMachines.Clear();


		machinesGivenGoodInput = new List<int>();
		machinesGivenBadInput = new List<int>();
		//hasRegisteredConfigDisplay = false;

		currRoundAttempts = 0;


		toggleNextWordButtonVisibility(true);


		// Log word displayed.
		//serverCommunication.wordDisplayed(wordStrToHarvest);


		//TimerScript timerScrpt = GameObject.Find("GlobObj").GetComponent<TimerScript>();
		//timerScrpt.resetTimer();
	}

	protected override void pauseScene(bool para_pauseState)
	{
		paused = para_pauseState;
		renderSpecialCam = ! para_pauseState;
		showGUI = ! para_pauseState;		
	}

	protected override bool checkNHandleWinningCondition()
	{
		updateActivityProgressMetaData(numHarvestedWords/(reqHarvestsForWin*1.0f));
		bool playerHasWon = (numHarvestedWords >= reqHarvestsForWin);
		
		if(playerHasWon)
		{
			endTimestamp = Time.time;
			pauseScene(true);
			showGUI = false;
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
			showGUI = false;
			performDefaultLoseProcedure();
		}

		return playerHasLost;
	}

	protected override void buildNRecordConfigOutcome(System.Object[] para_extraParams)
	{
		// Build outcome object.

		bool outcomeIsPositiveFlag = (machinesGivenBadInput.Count == 0);

		HarvestLevelOutcome reqOutcomeObj = null;

		//Debug.Log("Add "+outcomeIsPositiveFlag);
		if(outcomeIsPositiveFlag)
		{
			reqOutcomeObj = new HarvestLevelOutcome(outcomeIsPositiveFlag,null,null);
		}
		else
		{
			reqOutcomeObj = new HarvestLevelOutcome(outcomeIsPositiveFlag,machinesGivenGoodInput,machinesGivenBadInput);
		}

		// Trigger record outcome.
		recordOutcomeForConfig(reqOutcomeObj);
	}

	protected override GameyResultData buildGameyData()
	{
		float timeDiff = endTimestamp - startTimestamp;
		int minutes = (int) (timeDiff / 60f);
		int seconds = (int) (((timeDiff / 60f) - minutes) * 60f);

		HarvestGameyResultData reqData = new HarvestGameyResultData(numCorrectHarvests,numMachinesBroken,minutes,seconds);
		return reqData;
	}

	protected override string generateGoalTextAppend()
	{
		return (" "+reqHarvestsForWin);
	}

	public new void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{

		base.respondToEvent(para_sourceID,para_eventID,para_eventData);


		if((para_sourceID == "SlideshowWindow")&&(para_eventID == "Close"))
		{
			renderSpecialCam = true;
			startTimestamp = Time.time;
		}
		else if(para_eventID == "NextWordButtonPressEffect")
		{
			nxtWordButtonEnabled = true;
		}
		else if((para_sourceID == "ActivityPauseWindow")||
			((para_sourceID == "PersonHelperWindow")&&(para_eventID == "Close")))
		{
			showGUI = true;
		}
		else if(para_eventID == "DragStart")
		{
			triggerSoundAtCamera("sfx_ObjectGrab");

			nameOfDragItem = para_sourceID;
			currObjBeingDragged = GameObject.Find(para_sourceID);
			//dragRestPosition = currObjBeingDragged.transform.position;


			// Make sure the item is at the forefront.
			Vector3 tmpPos = currObjBeingDragged.transform.position;
			tmpPos.z = flowerZVal;
			currObjBeingDragged.transform.position = tmpPos;

			// Increase scale to have a popout effect when the player clicks on the flower.
			currObjBeingDragged.transform.localScale *= 1.1f;

			// Disable physics when being carried by the player.
			Rigidbody rBody = currObjBeingDragged.GetComponent<Rigidbody>();
			rBody.useGravity = false;
			rBody.isKinematic = true;

			// Make item swing.
			CustomAnimationManager caMang = currObjBeingDragged.AddComponent<CustomAnimationManager>();
			List<List<AniCommandPrep>> cmdBatchList = new List<List<AniCommandPrep>>();
			List<AniCommandPrep> cmdBatch1 = new List<AniCommandPrep>();
			cmdBatch1.Add(new AniCommandPrep("Swing",1,new List<System.Object>() { 45f, 0.5f }));
			cmdBatchList.Add(cmdBatch1);
			caMang.init("SwingAniTest",cmdBatchList);
		}
		else if(para_eventID == "DragRelease")
		{

			DragScript dScript = GameObject.Find("GlobObj").GetComponent<DragScript>();
			if(dScript.getNumPotentialOwnersForDragObj(para_sourceID) <= 0)
			{
				triggerSoundAtCamera("sfx_ObjectRelease");

				// Restore original scale when player lets go of the flower.
				currObjBeingDragged.transform.localScale *= 0.9f;

				if(hoverMachine == null)
				{
					// Teleport flower back to table.

					int flowerIndex = int.Parse(currObjBeingDragged.name.Split('-')[1]);
					Vector3 teleportPos = flowerSpawnPts[flowerIndex];
					currObjBeingDragged.transform.position = teleportPos;
					Destroy(currObjBeingDragged.GetComponent<CustomAnimationManager>());
					Destroy(currObjBeingDragged.GetComponent<Swing>());
					currObjBeingDragged.transform.rotation = Quaternion.identity;
					currObjBeingDragged = null;
				}
				else
				{
					// Fall into the machine.


					// Disable all machine holes except the hover machine.
					for(int i=0; i<currLvlConfig.machineDescriptions.Length; i++)
					{
						GameObject m = GameObject.Find(harvestMachineGObjNames[i]);
						Transform machineHoleChild = m.transform.FindChild("Hole-"+(i+1));

						if(harvestMachineGObjNames[i] != hoverMachine)
						{
							machineHoleChild.gameObject.collider.enabled = false;
						}
						else
						{
							machineHoleChild.gameObject.collider.enabled = true;
						}
					}

					// Disable next word button.
					toggleNextWordButtonVisibility(false);
					//Hector//if( ! hasRegisteredConfigDisplay)
					//{
					//	recordPresentedConfig(currLvlConfig);
					//	if(serverCommunication != null) { serverCommunication.wordDisplayed(currLvlConfig.harvestingWord); }
					//	hasRegisteredConfigDisplay = true;
					//}
					

					// Register machine flower assignment and switch off machine glow.
					machineFlowerLookup.Add(hoverMachine,currObjBeingDragged.name);
					unusableMachines.Add(hoverMachine);
					toggleMachineGlow(hoverMachine,false);
					hoverMachine = null;

					// Reset and remove Swing.
					currObjBeingDragged.layer = 0;
					currObjBeingDragged.GetComponent<SpriteRenderer>().sortingOrder = -1;

					Destroy(currObjBeingDragged.GetComponent<CustomAnimationManager>());
					Destroy(currObjBeingDragged.GetComponent<Swing>());

					currObjBeingDragged.collider.isTrigger = false;

					Rigidbody rBody = currObjBeingDragged.GetComponent<Rigidbody>();
					rBody.useGravity = true;
					rBody.isKinematic = false;
				}


				/*// Disable all machine holes.
				for(int i=0; i<harvestMachineGObjNames.Count; i++)
				{
					GameObject m = GameObject.Find(harvestMachineGObjNames[i]);
					Transform machineHoleChild = m.transform.FindChild("Hole-"+(i+1));
					machineHoleChild.gameObject.SetActive(false);
				}


				// Determine if necessary to switch on hole.
				string potentialMachine = checkIfItemCanLandIntoMachines();
				if(potentialMachine == null)
				{
					// Do not alter z val.
				}
				else
				{
					dScript.setInputFlag(false);
					paused = true;

					GameObject destMachine = GameObject.Find(potentialMachine);
					Transform machineHoleChild = destMachine.transform.FindChild("Hole-"+(destMachine.name[destMachine.name.Length-1]));
					machineHoleChild.gameObject.SetActive(true);

					Vector3 tmpPos = currObjBeingDragged.transform.position;
					tmpPos.z = destMachine.transform.position.z + 0.1f;
					currObjBeingDragged.transform.position = tmpPos;
				}



				//TeleportToLocation ttl = currObjBeingDragged.AddComponent<TeleportToLocation>();
				//ttl.init(dragRestPosition);

				// Reset and remove Swing.
				Destroy(currObjBeingDragged.GetComponent<CustomAnimationManager>());
				Destroy(currObjBeingDragged.GetComponent<Swing>());
				//Vector3 tmpAngles = currObjBeingDragged.transform.localEulerAngles;
				//tmpAngles.z = 0;
				//currObjBeingDragged.transform.localEulerAngles = tmpAngles;
				
				Rigidbody rBody = currObjBeingDragged.GetComponent<Rigidbody>();
				rBody.useGravity = true;
				rBody.isKinematic = false;*/
			}
			else
			{
				// Fail safe.

				// Teleport flower back to table.
				
				int flowerIndex = int.Parse(currObjBeingDragged.name.Split('-')[1]);
				Vector3 teleportPos = flowerSpawnPts[flowerIndex];
				currObjBeingDragged.transform.position = teleportPos;
				Destroy(currObjBeingDragged.GetComponent<CustomAnimationManager>());
				Destroy(currObjBeingDragged.GetComponent<Swing>());
				currObjBeingDragged.transform.rotation = Quaternion.identity;
				currObjBeingDragged = null;
			}





			currObjBeingDragged = null;
			//dragRestPosition = Vector3.zero;
		}
		else if(para_eventID == "HoleFilled")
		{
			
			
			string[] eData = (string[]) para_eventData;
			//string wordObjName = eData[0];
			string holeObjName = eData[1];
			int machineID = int.Parse(holeObjName.Split('-')[1]);
			
			

			
			string reqMachineName = "Harvest_Machine"+machineID;
			GameObject reqMachineObj = GameObject.Find(reqMachineName);

			if(machineFlowerLookup.ContainsKey(reqMachineName))
			{
				GameObject reqHarvestObj = GameObject.Find(machineFlowerLookup[reqMachineName]);
				//GameObject machDescObj = (reqMachineObj.transform.FindChild("TextArea")).gameObject;
			
				Destroy(reqHarvestObj);

				this.respondToEvent(nameOfDragItem,"MachineInputAnimation",reqMachineObj.name);
			}

		}
		else if(para_eventID == "TimeOver")
		{
			// TODO: Add wordSolvedCorrectly false to all remaining unmatched flowers.
			/*for all correct machines, a timeout error
			serverCommunication.wordSolvedCorrectly(word,false,"timeout "+missedMachinePattern);*/
			//serverCommunication.wordSolvedCorrectly("word?",false,"timeout");

			//genNextLevel();
		}
		else if(para_sourceID == nameOfDragItem)
		{


			if(para_eventID == "MachineInputAnimation")
			{


				GameObject reqHarvestObj = GameObject.Find(nameOfDragItem);
				Rigidbody rBody = reqHarvestObj.GetComponent<Rigidbody>();
				rBody.useGravity = false;
				rBody.isKinematic = true;

				//reqHarvestObj.collider.isTrigger = true;

				string reqMachineName = (string) para_eventData;
				int reqMachineID = int.Parse(reqMachineName.Split(new string[] {"Harvest_Machine"}, System.StringSplitOptions.None)[1]);
				GameObject reqMachine = GameObject.Find((string) para_eventData);
				Transform machineHoleChild = reqMachine.transform.FindChild("Hole-"+reqMachineID);
				HoleScript tmpHScript = machineHoleChild.GetComponent<HoleScript>();
				tmpHScript.manualDisownProcedure();
				machineHoleChild.collider.enabled = false;
				//machineHoleChild.gameObject.SetActive(false);

				string machineDescription = currLvlConfig.machineDescriptions[reqMachineID-1];




				bool tmpMachineOutcome = currLvlConfig.machineIsCorrect(reqMachineID-1);
				machineOutcomeLookup.Add(reqMachine.name,tmpMachineOutcome);
				if(tmpMachineOutcome == true)
				{
					if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(currLvlConfig.harvestingWord,true,machineDescription,currLvlConfig.languageArea,currLvlConfig.difficulty); }
					numCorrectHarvests++;
					machinesGivenGoodInput.Add(reqMachineID-1);

					triggerSoundAtCamera("HarvestMachineWhirl",0.3f);

					CustomAnimationManager aniMang = reqMachine.AddComponent<CustomAnimationManager>();
					List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
					List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
					batch1.Add(new AniCommandPrep("TriggerAnimation",2, new List<System.Object>() { "harvesting1", 1f }));
					batchLists.Add(batch1);
					aniMang.registerListener("AcScen",this);
					aniMang.init("FlowerProcessEffect-"+reqMachineID,batchLists);
				}
				else
				{
					if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(currLvlConfig.harvestingWord,false,machineDescription,currLvlConfig.languageArea,currLvlConfig.difficulty); }
					numMachinesBroken++;
					machinesGivenBadInput.Add(reqMachineID-1);

					triggerSoundAtCamera("HarvestMachineWhirl",0.3f);

					CustomAnimationManager aniMang = reqMachine.AddComponent<CustomAnimationManager>();
					List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
					List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
					batch1.Add(new AniCommandPrep("TriggerAnimation",2, new List<System.Object>() { "badHarvest", 1f }));
					batchLists.Add(batch1);
					aniMang.registerListener("AcScen",this);
					aniMang.init("FlowerProcessEffect-"+reqMachineID,batchLists);
				}

			}

		}
		else if(para_eventID.Contains("FlowerProcessEffect"))
		{
			//Destroy(GameObject.Find(nameOfDragItem));
			//nameOfDragItem = "";

			// Note: reqMachineID is 1 based.
			int reqMachineID = int.Parse(para_eventID.Split('-')[1]);			
			string reqMachineName = "Harvest_Machine"+reqMachineID;
			GameObject reqMachine = GameObject.Find(reqMachineName);
			Animator machAni = reqMachine.GetComponent<Animator>();
			machAni.speed = 0;
			
			if(machineOutcomeLookup[reqMachineName])
			{
				triggerSoundAtCamera("CorrectDoubleDing");
				toggleMachineColor(reqMachineName,ColorUtils.convertColor(66,255,98));
			}
			else
			{
				triggerSoundAtCamera("SystemError");
				toggleMachineColor(reqMachineName,ColorUtils.convertColor(255,90,71));
			}
			
			DragScript dScript = GameObject.Find("GlobObj").GetComponent<DragScript>();
			dScript.setInputFlag(true);
			paused = false;

			currRoundAttempts++;
			if(currRoundAttempts >= currLvlConfig.correctMachines.Length)
			{
				/*numHarvestedWords++;
				bool isWin = checkNHandleWinningCondition();
				if(isWin)
				{
					buildNRecordConfigOutcome(null);
				}*/
				toggleNextWordButtonVisibility(true);
			}
		}
	}


	private string checkIfItemCanLandIntoMachines()
	{	
		Bounds currObj3DBounds = currObjBeingDragged.renderer.bounds;
		Rect currObj2DBounds = new Rect(currObj3DBounds.center.x - (currObj3DBounds.size.x/2f),
		                                currObj3DBounds.center.y + (currObj3DBounds.size.y/2f),
		                                currObj3DBounds.size.x,
		                                currObj3DBounds.size.y);

		Vector2 item2DCentre = new Vector2(currObj2DBounds.x + (currObj2DBounds.width/2f),currObj2DBounds.y - (currObj2DBounds.height/2f));

		string destinationMachine = null;
		Vector2 destMachineCentre = new Vector2(1,1);
		Vector2 tmp2DCentre = new Vector2(1,1);

		for(int i=0; i<harvestMachineGObjNames.Count; i++)
		{
			GameObject tmpMachineObj = GameObject.Find(harvestMachineGObjNames[i]);
			if(tmpMachineObj != null)
			{
				Bounds tmpMachineBounds = tmpMachineObj.transform.FindChild("MachineDescription").FindChild("Board").renderer.bounds;		// TMP Hack.

				//Bounds tmpMachineBounds = tmpMachineObj.renderer.bounds;
				tmp2DCentre.x = tmpMachineBounds.min.x + (tmpMachineBounds.size.x/2f);
				tmp2DCentre.y = tmpMachineBounds.min.y + (tmpMachineBounds.size.y/2f);

				if((currObj3DBounds.center.x > tmpMachineBounds.min.x)&&(currObj3DBounds.center.x < tmpMachineBounds.max.x)
				&&(currObj3DBounds.min.y > tmpMachineBounds.max.y))
				{
					if((destinationMachine == null)
					||((destinationMachine != null)&&(Vector2.Distance(item2DCentre,tmp2DCentre) < Vector2.Distance(item2DCentre,destMachineCentre))))
					{
						destinationMachine = tmpMachineObj.name;
						destMachineCentre.x = tmp2DCentre.x;
						destMachineCentre.y = tmp2DCentre.y;
					}
				}
			}
		}

		if(unusableMachines.Contains(destinationMachine))
		{
			destinationMachine = null;
		}

		return destinationMachine;
	}

	public void receiveHoveringMachineName(string para_machineObjName, bool para_state)
	{
		hoverMachine = null;
		if( ! unusableMachines.Contains(para_machineObjName))
		{
			if(para_state == true)
			{
				hoverMachine = para_machineObjName;
				triggerSoundAtCamera("PotentialSelection");
			}
			toggleMachineGlow(para_machineObjName,para_state);
		}
	}

	private void toggleMachineGlow(string para_machineObjName, bool para_glowState)
	{
		GameObject machineObj = GameObject.Find(para_machineObjName);
		Transform selectorChild = machineObj.transform.FindChild("Selector");
		Transform pistonGlow1 = machineObj.transform.FindChild("Piston1_parent").FindChild("Piston1").FindChild("PistonGlow");
		Transform pistonGlow2 = machineObj.transform.FindChild("Piston2_parent").FindChild("Piston2").FindChild("PistonGlow");
		Transform mechUdderGlow = machineObj.transform.FindChild("MechUdder").FindChild("MechUdderGlow");

		selectorChild.renderer.enabled = para_glowState;
		pistonGlow1.renderer.enabled = para_glowState;
		pistonGlow2.renderer.enabled = para_glowState;
		mechUdderGlow.renderer.enabled = para_glowState;
	}

	private void toggleMachineColor(string para_machineObjName, Color para_color)
	{
		GameObject machineObj = GameObject.Find(para_machineObjName);
		Transform machineTop = machineObj.transform.FindChild("Harvest_MachineTop");
		Transform mechUdder = machineObj.transform.FindChild("MechUdder");
		List<Transform> tmpList = new List<Transform>() { machineObj.transform, machineTop, mechUdder };

		for(int i=0; i<tmpList.Count; i++)
		{
			tmpList[i].GetComponent<SpriteRenderer>().color = para_color;
		}
	}

	private void toggleNextWordButtonVisibility(bool para_visibilityState)
	{
		if(para_visibilityState == false)
		{
			GameObject nxtWordButtonObj = GameObject.Find("Harvest_NextWordButton");
			Vector3 tmpPosVect = nxtWordButtonObj.transform.position;
			tmpPosVect.x += 1000;
			nxtWordButtonObj.transform.position = tmpPosVect;
			nxtWordButtonEnabled = false;
		}
		else
		{
			GameObject nxtWordButtonObj = GameObject.Find("Harvest_NextWordButton");
			nxtWordButtonObj.transform.position = nxtWordButtonOrigPos;
			nxtWordButtonEnabled = true;
		}
	}

	private void performNextWordButtonPressEffect()
	{
		triggerSoundAtCamera("BubbleClick");

		GameObject nxtWordButtonObj = GameObject.Find("Harvest_NextWordButton");
		CustomAnimationManager aniMang = nxtWordButtonObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("InjectLocalScale",1,new List<System.Object>(){new float[3]{0.8f,0.8f,0.8f}}));
		batch1.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>(){ 0.1f }));
		List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
		batch2.Add(new AniCommandPrep("InjectLocalScale",1,new List<System.Object>(){new float[3]{1,1,1}}));
		batchLists.Add(batch1);
		batchLists.Add(batch2);
		aniMang.registerListener("AcScen",this);
		aniMang.init("NextWordButtonPressEffect",batchLists);
	}

	protected new void loadTextures()
	{
		availableTextures = new Dictionary<string,Texture2D>();


		Texture2D blackTex = new Texture2D(1,1);
		blackTex.SetPixel(0,0,Color.black);
		blackTex.Apply();
		
		availableTextures.Add("BlackTex",blackTex);
		availableTextures.Add("ExitIcon",Resources.Load<Texture2D>("Textures/Common/ExitIcon"));
	}

	protected new void prepUIBounds()
	{
		uiBounds = new Dictionary<string,Rect>();
		
		uiBounds.Add("DebugSolutionStrLabel",new Rect(0,0,Screen.width,100));
	}

	protected new void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string,GUIStyle>(); hasInitGUIStyles = true;

		float targetWidth = 1280f;
		float targetHeight = 800f;
		Vector3 scaleForCurrRes = new Vector3((Screen.width * 1.0f)/targetWidth,(Screen.height * 1.0f)/targetHeight,1f);

		GUIStyle machineDescriptionLabelStyle = new GUIStyle(GUI.skin.label);
		machineDescriptionLabelStyle.alignment = TextAnchor.MiddleCenter;
		machineDescriptionLabelStyle.fontSize = (int) (24 * scaleForCurrRes.x);

		availableGUIStyles.Add("MachineDescription",machineDescriptionLabelStyle);
	}

	private void triggerSoundAtCamera(string para_soundFileName)
	{
		triggerSoundAtCamera(para_soundFileName,1f);
	}

	private void triggerSoundAtCamera(string para_soundFileName, float para_volume)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		DontDestroyOnLoad(nwSFX);
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = para_volume;
		audS.Play();
	}
}

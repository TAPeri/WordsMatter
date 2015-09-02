/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;


public class AcPackageDeliveryScenario : ILearnRWActivityScenario, CustomActionListener
{

	public Transform graphNodePrefab;
	public Transform blankCollisionBox;
	public Transform monkeyPrefab;
	public Transform mainAvatarPrefab;
	public Transform movementMarkerPrefab;
	public Transform arrowMarkerPrefab;
	public Transform sfxPrefab;
	public Transform wordboxPrefab;
	public Transform bananaPrefab;

	public Transform[] doorPrefabs;
	public Transform[] packagePrefabs;

	public RuntimeAnimatorController avController_NonLooping;

	int currentWordIdx = 0;
	int attempts = 3;

	PDLevelConfig currLvlConfig;
	bool metaDataLoaded = false;

	int numDeliveries = 0;
	int reqDeliveriesForWin; // Init when lvl gen init.

	int numPackagesLostToMonkies = 0;
	int numWrongDeliveries = 0;
	float startTimestamp = 0;
	float endTimestamp = 0;


	bool[] upAxisArr = new bool[3] {false,true,false};
	bool paused;
	bool hasRegisteredInit = false;

		
	int[] mapSize = {48,48};
	GridProperties gPropMapWorldBounds;
	ColoredNavGraph worldNavGraph;
	ITerrainHandler terrainHandle;


	


	float playerWalkSpeed = 3f;



	List<AbsInputDetector> inputDetectors;

	int[] destCellCoords;


	bool inHouseViewMode;

	int currNumKnocks;


	float tripTimerWaitDelay_Sec = 1f;

	Vector3 packageSnapReturnPos;

	Camera mainCam;

	DropoffLocationData[] dropoffDataArr;
	int currDropOffDataID;


	int numOfPackagesInInventory;

	List<string[]> stolenPackageToMonkeyQueue;
	int stoleQueueIndexPointer;


	int numOfBananasInInventory;


	List<int> selectedPackagePrefabIndexes;

	//bool pkgInvEffectIsBusy = false;
	//bool bananaInvEffectIsBusy = false;

	bool hasMadeMistakeAtDoor = false;

	bool isFlashingEmptyPkInv = false;



	void Start()
	{
		acID = ApplicationID.ENDLESS_RUNNER;
		metaDataLoaded = loadActivitySessionMetaData();
		
		loadTextures();
		this.prepUIBounds();

		this.initLevelConfigGenerator();
		reqDeliveriesForWin = lvlConfigGen.getConfigCount();

		initWorld();
		genNextLevel();

		initPersonHelperPortraitPic();
		showInfoSlideshow();
		recordActivityStart();
	}


	void OnGUI()
	{


		GUI.color = Color.clear;
		if( ! paused)
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
		GUI.color = Color.white;
	}



	void Update()
	{
		/*if(Input.GetKeyDown(KeyCode.Minus))
		{
			deductPackageFromInventory(true);
		}

		if(Input.GetKeyDown(KeyCode.Return))
		{
			addPackageToInventory(true);
		}

		if(Input.GetKeyDown(KeyCode.W))
		{
			addBananaToInventory(true);
		}

		if(Input.GetKeyDown(KeyCode.S))
		{
			deductBananaFromInventory(true);
		}*/
	}


	protected override void initWorld()
	{

		// Auto Adjust.

		GameObject tmpPackageUIIcon = GameObject.Find("PackageUIIcon");
		GameObject tmpBananaUIIcon = GameObject.Find("BananaUIIcon");
		GameObject tmpPersonPortrait = GameObject.Find("PersonPortrait");
		GameObject tmpPauseButton = GameObject.Find("PauseButton");
		GameObject tmpBackdrop = GameObject.Find("Backdrop").gameObject;
		SpawnNormaliser.adjustGameObjectsToNwBounds(SpawnNormaliser.get2DBounds(tmpBackdrop.renderer.bounds),
		                                            WorldSpawnHelper.getCameraViewWorldBounds(tmpBackdrop.transform.position.z,true),
		                                            new List<GameObject>() { tmpPackageUIIcon, tmpBananaUIIcon, tmpPersonPortrait, tmpBackdrop });

		tmpPackageUIIcon.transform.parent = GameObject.Find("Main Camera").transform;
		tmpBananaUIIcon.transform.parent = GameObject.Find("Main Camera").transform;
		tmpPersonPortrait.transform.parent = GameObject.Find("Main Camera").transform;
		tmpPauseButton.transform.parent = GameObject.Find("Main Camera").transform;

		tmpPackageUIIcon.transform.FindChild("PackageUICounter").gameObject.renderer.sortingOrder = (tmpPackageUIIcon.renderer.sortingOrder+1);
		tmpBananaUIIcon.transform.FindChild("BananaUICounter").gameObject.renderer.sortingOrder = (tmpBananaUIIcon.renderer.sortingOrder+1);
		CommonUnityUtils.setSortingOrderOfEntireObject(tmpPersonPortrait,100);
		CommonUnityUtils.setSortingOrderOfEntireObject(tmpPauseButton,200);


		uiBounds.Add("PackageCounter",WorldSpawnHelper.getWorldToGUIBounds(tmpPackageUIIcon.renderer.bounds,upAxisArr));
		uiBounds.Add("PersonPortrait",WorldSpawnHelper.getWorldToGUIBounds(tmpPersonPortrait.renderer.bounds,upAxisArr));
		uiBounds.Add("PauseBtn",WorldSpawnHelper.getWorldToGUIBounds(tmpPauseButton.renderer.bounds,upAxisArr));		 
		Destroy(tmpBackdrop);



		// Init world bounds data.
		gPropMapWorldBounds = calculateWorldMapBounds(mapSize);
		WorldSpawnHelper.initObjWithinWorldBounds(blankCollisionBox,
		                                          blankCollisionBox.transform.renderer.bounds.size.x,
		                                          blankCollisionBox.transform.renderer.bounds.size.y,
		                                          "WorldColBox",
		                                          new Rect(gPropMapWorldBounds.x,gPropMapWorldBounds.y,gPropMapWorldBounds.totalWidth,gPropMapWorldBounds.totalHeight),
		                                          null,
		                                          gPropMapWorldBounds.z,
		                                          upAxisArr);




		// Init Nav Graph.
		string bnwMapFilePath = "Textures/AcDelivery/Map";
		ImgToWorldNavGraphBuilderV2 navBuilder = new ImgToWorldNavGraphBuilderV2(bnwMapFilePath,gPropMapWorldBounds);

		List<ColorGraphTypeInfo> graphRequirements = new List<ColorGraphTypeInfo>()
		{
			new ColorGraphTypeInfo(new string[] { "FreeSpace", "MonkeySpawn", "HouseDeliveryPoint", "PlayerSpawn" },
								   new Color[] { Color.white, ColorUtils.convertColor(153,76,0), Color.blue, Color.red }) 
		};

		List<ColoredNavGraph> graphList = navBuilder.constructColorGraphs(graphRequirements);
		worldNavGraph = graphList[0];

		
		List<System.Object> paramList = new List<System.Object>();
		paramList.Add(worldNavGraph);
		paramList.Add(gPropMapWorldBounds);
		terrainHandle = new BasicTerrainHandler();
		terrainHandle.constructTerrainStructures(paramList);


		// Render Nav Graph. (For Debug Purposes).
		//GameObject navGraphRenderObj = NavGraphUnityUtils.renderNavGraph("WorldNavGraphRender",worldNavGraph,graphNodePrefab);
		//navGraphRenderObj.SetActive(false);




		// Spawn Player.
		List<int> playerSpawnPtList = worldNavGraph.getNodesOfType(worldNavGraph.getTypeIDByName("PlayerSpawn"));
		WorldNode playerSpawnNode = (WorldNode) worldNavGraph.getNode(playerSpawnPtList[Random.Range(0,playerSpawnPtList.Count)]);
		Vector3 tmpV = playerSpawnNode.getWorldPt();
		Vector3 playerSpawnPt = new Vector3(tmpV.x,tmpV.y,-2f);
		Transform nwPlayerAvatar = (Transform) Instantiate(mainAvatarPrefab,playerSpawnPt,Quaternion.identity);
		nwPlayerAvatar.gameObject.name = "MainAvatar";
		NewCharacterNavMovement cnm = nwPlayerAvatar.gameObject.AddComponent<NewCharacterNavMovement>();
		cnm.faceDirection("S");

		// Apply player clothing.
		applyPlayerAvatarClothing();




		// Spawn MonkeyManager.
		PDMonkeyManager monkeyMang = transform.gameObject.AddComponent<PDMonkeyManager>();
		monkeyMang.init(monkeyPrefab,sfxPrefab,mapSize,gPropMapWorldBounds,worldNavGraph,terrainHandle);


		// Input detectors.
		inputDetectors = new List<AbsInputDetector>();
		ClickDetector cd = transform.gameObject.AddComponent<ClickDetector>();
		cd.setMaxDelayBetweenClicks(0.01f);
		cd.registerListener("AcScen",this);
		inputDetectors.Add(cd);


		// Mechanics variables.
		currNumKnocks = 0;
		mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
		numOfPackagesInInventory = reqDeliveriesForWin;
		numOfBananasInInventory = 30;
		refreshInventoryCounterDisplay();
		stolenPackageToMonkeyQueue = new List<string[]>();
		stoleQueueIndexPointer = 0;


		// Select prefabs for packages.
		selectedPackagePrefabIndexes = new List<int>();
		for(int i=0; i<reqDeliveriesForWin; i++)
		{
			selectedPackagePrefabIndexes.Add(Random.Range(0,packagePrefabs.Length));
		}
	}


	/*private void determineDropoffLocations(int para_totalDeliveries)
	{
		List<DropoffLocationData> tmpList = new List<DropoffLocationData>();
		List<int> deliveryNodeIDs = new List<int>(worldNavGraph.getNodesOfType(worldNavGraph.getTypeIDByName("HouseDeliveryPoint")));
		int tmpCounter = 0;

		while((tmpCounter < (para_totalDeliveries-1))&&(deliveryNodeIDs.Count > 0))
		{
			int randIndex = Random.Range(0,deliveryNodeIDs.Count);
			int chosenDeliveryNodeID = deliveryNodeIDs[randIndex];
			WorldNode chosenDeliveryNode = (WorldNode) worldNavGraph.getNode(chosenDeliveryNodeID);
			int[] tmpCellCoords = gPropMapWorldBounds.hashPointToCell(new float[2] { chosenDeliveryNode.getWorldPt().x, chosenDeliveryNode.getWorldPt().y }, true);

			tmpList.Add(new DropoffLocationData(chosenDeliveryNode.getWorldPt(),tmpCellCoords));
			deliveryNodeIDs.RemoveAt(randIndex);
			tmpCounter++;
		}

		dropoffDataArr = tmpList.ToArray();
		currDropOffDataID = 0;
	}*/


	protected override void genNextLevel()
	{
		currLvlConfig = (PDLevelConfig) lvlConfigGen.getNextLevelConfig(null);


		// Select destination.
		Destroy(GameObject.Find("DestinationCell"));

		List<int> deliveryNodeIDs;
		int chosenDeliveryNodeID;
		WorldNode chosenDeliveryNode;
		int[] tmpCellCoords;

		do
		{
			deliveryNodeIDs = worldNavGraph.getNodesOfType(worldNavGraph.getTypeIDByName("HouseDeliveryPoint"));
			chosenDeliveryNodeID = deliveryNodeIDs[Random.Range(0,deliveryNodeIDs.Count)];
			chosenDeliveryNode = (WorldNode) worldNavGraph.getNode(chosenDeliveryNodeID);
			tmpCellCoords = gPropMapWorldBounds.hashPointToCell(new float[2] { chosenDeliveryNode.getWorldPt().x, chosenDeliveryNode.getWorldPt().y }, true);
		}
		while((destCellCoords != null)&&(tmpCellCoords[0] == destCellCoords[0])&&(tmpCellCoords[1] == destCellCoords[1]));

		destCellCoords = tmpCellCoords;

		Vector3 debugDestinationCellPt = new Vector3(chosenDeliveryNode.getWorldPt().x,chosenDeliveryNode.getWorldPt().y, -0.7f);
		GameObject debugDestinationCell = GameObject.CreatePrimitive(PrimitiveType.Quad);
		debugDestinationCell.name = "DestinationCell";
		debugDestinationCell.transform.position = debugDestinationCellPt;
		debugDestinationCell.renderer.material.shader = Shader.Find("Transparent/Diffuse");
		debugDestinationCell.renderer.material.color = ColorUtils.convertColor(255,216,0,150);// Color.yellow;



		// Arrow Marker.
		if(GameObject.Find("ArrowMarker_Destination") != null) { Destroy(GameObject.Find("ArrowMarker_Destination")); }
		Transform nwArrowMarker = (Transform) Instantiate(arrowMarkerPrefab,new Vector3(0,-4,-3),Quaternion.identity);
		ArrowMarkerScript amScript = nwArrowMarker.GetComponent<ArrowMarkerScript>();
		amScript.name = "ArrowMarker_Destination";
		amScript.init(GameObject.Find("MainAvatar"),debugDestinationCell,3f,Color.blue);


		// Create Random House Door.
		createRandomHouseDoor();

		hasMadeMistakeAtDoor = false;
	}


	public new void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{

		base.respondToEvent(para_sourceID,para_eventID,para_eventData);

		if((!hasRegisteredInit)&&(isInitialised))
		{
			startTimestamp = Time.time;

			hasRegisteredInit = true;

			// Set Camera to Follow Player.
			FollowScript followScrpt = Camera.main.transform.gameObject.AddComponent<FollowScript>();
			followScrpt.init(GameObject.Find("MainAvatar"),new bool[3] { true, true, false });
		}


		if(para_sourceID == "InputDetector")
		{
			if(para_eventID == "ClickEvent")
			{

				System.Object[] unparsedData = (System.Object[]) para_eventData;
				float[] clickPtArr = (float[]) unparsedData[0];

				//Debug.Log("Screen Point Clicked: "+clickPtArr[0]+","+clickPtArr[1]);

				if( ! inHouseViewMode)
				{
					if( ! paused)
					{
						if(( ! (uiBounds["PersonPortrait"].Contains(new Vector2(clickPtArr[0],Screen.height-clickPtArr[1]))))
						   &&( ! (uiBounds["PauseBtn"].Contains(new Vector2(clickPtArr[0],Screen.height-clickPtArr[1])))))
						{

							Ray reqRay = mainCam.ScreenPointToRay(new Vector3(clickPtArr[0],clickPtArr[1],0));
							
							RaycastHit hitData;
							if(Physics.Raycast(reqRay,out hitData))
							{
								if(hitData.collider.gameObject.name.Contains("Monkey"))
								{
									if(numOfBananasInInventory > 0)
									{
										fireBanana(hitData.collider.gameObject);
									}
								}
								else
								{
									int[] clickCell = gPropMapWorldBounds.hashPointToCell(new float[2] { hitData.point.x, hitData.point.y },true);
									initPlayerMovement(clickCell,false);
								}
							}
						}
					}

				}
				else
				{
					// Check if player has clicked on the screen containing the door.
					Rect doorScreenSpace = new Rect(0,0,Screen.width/2f,Screen.height);

					if(doorScreenSpace.Contains(new Vector2(clickPtArr[0],clickPtArr[1])))
					{
						//Debug.Log("About to hit world");
					}
					else
					{
						//Debug.Log("About to hit door");

						Camera doorCamScript = GameObject.Find("DoorCam").GetComponent<Camera>();
						Ray reqRayCam2 = doorCamScript.ScreenPointToRay(new Vector3(clickPtArr[0],clickPtArr[1],0));

						RaycastHit hitDataCam2;
						if(Physics.Raycast(reqRayCam2,out hitDataCam2))
						{
							//Debug.Log("Raycast hit: "+hitDataCam2.collider.name);
							if(hitDataCam2.collider.name == "KnockArea")
							{
								handleDoorKnock();
							}
						}
					}
				}
			}
		}
		else if(para_sourceID == "MainAvatar")
		{
			if(para_eventID == "PathComplete")
			{
				Destroy(GameObject.Find("MovementMarker"));
				
				Transform footstepsSoundBox = mainCam.transform.FindChild("FootstepsSoundBox");
				if(footstepsSoundBox != null)
				{
					Destroy(footstepsSoundBox.gameObject);
				}



				if(destCellCoords != null)
				{
					int[] currPlayerCell = getCellForItem("MainAvatar");
					if((currPlayerCell[0] == destCellCoords[0])&&(currPlayerCell[1] == destCellCoords[1]))
					{
						// Player has reached destination cell. Trigger house stuff.
						if(numOfPackagesInInventory > 0)
						{
							Debug.Log("Player has reached house");
							transform.GetComponent<PDMonkeyManager>().informAllMonkeysOfPlayerStock(false);
							triggerHouseViewStart();
						}
						else
						{
							Debug.Log("Cannot enter house. No packages in inventory.");
							if( ! isFlashingEmptyPkInv)
							{
								isFlashingEmptyPkInv = true;
								emptyPackageInventoryEffect();
							}
						}
					}
				}

			}
		}
		else if(para_eventID == "AdjustCameraViewport")
		{

			if( ! inHouseViewMode)
			{
				GameObject doorCamGObj = GameObject.Find("DoorCam");
				doorCamGObj.GetComponent<Camera>().enabled = false;

				currNumKnocks = 0;
				GameObject knockCounter = GameObject.Find("HouseDoor").transform.FindChild("DoorKnockCounter").FindChild("Text").gameObject;
				knockCounter.renderer.material.color = Color.black;
				knockCounter.GetComponent<TextMesh>().text = " ";

				deductPackageFromInventory(true);

				bool winFlag = checkNHandleWinningCondition();
				if( ! winFlag)
				{
					genNextLevel();
				}
			}

			toggleInputDetectors(true);

		}
		else if(para_eventID == "TimerTripped")
		{
			handleKnockSequenceOutcome();
		}
		else if(para_eventID == "GoodKnockSequenceFeedback")
		{
			toggleInputDetectors(true);
			setKnockInput(false);
			startDoorHandover();
		}
		else if(para_eventID == "WrongKnockSequenceFeedback")
		{

			currNumKnocks = 0;
			GameObject knockCounter = GameObject.Find("HouseDoor").transform.FindChild("DoorKnockCounter").FindChild("Text").gameObject;
			knockCounter.renderer.material.color = Color.black;
			knockCounter.GetComponent<TextMesh>().text = " ";
			toggleInputDetectors(true);

			if(attempts==0){

				hasMadeMistakeAtDoor = true;
				triggerHouseViewEnd();
				//numOfPackagesInInventory--;//done in response to event AdjustCameraViewport
			}



		}
		else if(para_eventID == "DeliveryBagEnter")
		{
			DragScript ds = transform.gameObject.AddComponent<DragScript>();
			ds.setReqCamera(GameObject.Find("DoorCam").GetComponent<Camera>());
			ds.registerListener("AcScen",this);

			Transform package = GameObject.Find("DoorDeliveryBag").transform.FindChild("Package");
			package.parent = null;
			packageSnapReturnPos = new Vector3(package.position.x,package.position.y,package.position.z);
		}
		else if(para_eventID == "DragRelease")
		{
			DragScript ds = transform.gameObject.GetComponent<DragScript>();
			if(ds.getNumPotentialOwnersForDragObj(para_sourceID) <= 0)
			{
				TeleportToLocation ttl = GameObject.Find("Package").AddComponent<TeleportToLocation>();
				ttl.init(packageSnapReturnPos);
			}
		}
		else if(para_eventID == "HoleFilled")
		{

			//Destroy(transform.gameObject.GetComponent<DragScript>());
			triggerPackageAccept();
		}
		else if(para_eventID == "PackageAcceptSequence")
		{
			Destroy(GameObject.Find("Package"));
			retractDeliveryBag();
		}
		else if(para_eventID == "DeliveryBagExit")
		{
			endDoorHandover();
		}
		else if(para_eventID == "DoorHandoverEnd")
		{
			triggerHouseViewEnd();
		}

		if(para_eventID == "DeductInventoryEffect")
		{
			if(stolenPackageToMonkeyQueue.Count > 0)
			{
				if(stoleQueueIndexPointer > 0)
				{
					for(int i=stoleQueueIndexPointer; i>0; i--)
					{
						stolenPackageToMonkeyQueue.RemoveAt(0);
					}
					stoleQueueIndexPointer = 0;
				}

				string[] tmpArr = stolenPackageToMonkeyQueue[0];
				GameObject stolenPackageGObj = GameObject.Find(tmpArr[0]);
				GameObject reqMonkeyGObj = GameObject.Find(tmpArr[1]);
				//stolenPackageGObj.transform.parent = reqMonkeyGObj.transform;


				//GameObject monkeyItemHolderGObj = reqMonkeyGObj.transform.FindChild("WorldPivot").FindChild("ItemHolder").gameObject;

				CustomAnimationManager aniMang = stolenPackageGObj.AddComponent<CustomAnimationManager>();
				List<List<AniCommandPrep>> batchList = new List<List<AniCommandPrep>>();
				List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
				batch1.Add(new AniCommandPrep("HomeInOnTarget",1,new List<System.Object>() {reqMonkeyGObj.name,10f}));
				batchList.Add(batch1);
				aniMang.registerListener("AcScen",this);
				aniMang.registerListener(reqMonkeyGObj.name,reqMonkeyGObj.GetComponent<PDMonkeyScript>());
				aniMang.init("PackageStealAnimation",batchList);


				stoleQueueIndexPointer++;
			}
		}
		else if(para_eventID == "MonkeyDroppedItem")
		{
			string itemName = (string) para_eventData;

			numPackagesLostToMonkies++;

			// Create Arrow Marker.
			string arrowMarkerName = "ArrowMarker_"+itemName;
			if(GameObject.Find(arrowMarkerName) != null) { Destroy(GameObject.Find(arrowMarkerName)); }
			Transform nwArrowMarker = (Transform) Instantiate(arrowMarkerPrefab,new Vector3(0,-4,-3),Quaternion.identity);
			ArrowMarkerScript amScript = nwArrowMarker.GetComponent<ArrowMarkerScript>();
			amScript.name = arrowMarkerName;
			amScript.init(GameObject.Find("MainAvatar"),GameObject.Find(itemName),3f,Color.yellow);
		}
		else if(para_eventID == "EmptyPackageInventoryFlash")
		{
			isFlashingEmptyPkInv = false;
		}

	

	}


	public void initPlayerMovement(int[] para_mapCell, bool para_stopOneNodeShort)
	{
		Destroy(GameObject.Find("MovementMarker"));
		
		
		int[] cellForPlayerAvatar = getCellForItem("MainAvatar");
		int srcNavNodeID = terrainHandle.getNavNodeIDForCell(cellForPlayerAvatar);
		int destNavNodeID = terrainHandle.getNavNodeIDForCell(para_mapCell);
		
		
		if(srcNavNodeID != destNavNodeID)
		{
			float[] cellCentre = gPropMapWorldBounds.getCellCentre(para_mapCell,true);
			if(cellCentre != null)
			{

				
				
				List<NavNode> pathNodes = worldNavGraph.searchForPath(srcNavNodeID,destNavNodeID,new HashSet<int> {worldNavGraph.getTypeIDByName("MonkeySpawn")});
				
				if((pathNodes == null)||(pathNodes.Count == 0))
				{
					Debug.Log("No Path Available");
				}
				else
				{

					Transform nwMovementMarker = (Transform) Instantiate(movementMarkerPrefab,new Vector3(cellCentre[0],cellCentre[1],-1f),Quaternion.identity);
					nwMovementMarker.name = "MovementMarker";
										
					//NavGraphUnityUtils.clearAllNavNodeHighlights("WorldNavGraphRender");
					//NavGraphUnityUtils.highlightNavNodes("WorldNavGraphRender",pathNodes);
					GameObject playerAvatar = GameObject.Find("MainAvatar");
					NewCharacterNavMovement cnm = playerAvatar.GetComponent<NewCharacterNavMovement>();
					if(cnm == null)
					{
						cnm = playerAvatar.AddComponent<NewCharacterNavMovement>();
						cnm.registerListener("AcScen",this);
					}
					cnm.moveAlongPath(pathNodes,playerWalkSpeed,true,para_stopOneNodeShort);
					
					
					/*if(Camera.main.transform.FindChild("FootstepsSoundBox") == null)
					{
						SoundPlayerScript sps = transform.GetComponent<SoundPlayerScript>();
						sps.triggerSoundLoopAtCamera("FootstepsGravel","FootstepsSoundBox",true);
					}*/
				}
			}
		}
	}


	private int[] getCellForItem(string para_itemName)
	{
		GameObject itemGObj = GameObject.Find(para_itemName);
		
		if(itemGObj == null)
		{
			Debug.LogError("Failed to root item. Could not find '"+para_itemName+"'");
			return null;
		}
		else
		{
			int[] itemCell = gPropMapWorldBounds.hashPointToCell(new float[2] { itemGObj.transform.position.x, itemGObj.transform.position.y },true);
			return itemCell;
		}
	}




	protected override void pauseScene(bool para_pauseState)
	{
		paused = para_pauseState;
		toggleInputDetectors(!paused);
	}

	protected override bool checkNHandleWinningCondition()
	{
		updateActivityProgressMetaData((numDeliveries+numWrongDeliveries)/(reqDeliveriesForWin*1.0f));
		bool playerHasWon = ((numDeliveries+numWrongDeliveries) >= reqDeliveriesForWin);
		
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
		return false;
	}

	protected override string generateGoalTextAppend()
	{
		return (" "+reqDeliveriesForWin);
	}

	protected override void buildNRecordConfigOutcome(System.Object[] para_extraParams)
	{
		// Build outcome object.
		
		// Trigger record outcome.
		if(! hasMadeMistakeAtDoor){
			PDLevelOutcome reqOutcome = new PDLevelOutcome( true,currentWordIdx+1);
			recordOutcomeForConfig(reqOutcome);
		}else{
			PDLevelOutcome reqOutcome = new PDLevelOutcome( false,currentWordIdx);
			recordOutcomeForConfig(reqOutcome);
		}
	}

	protected override GameyResultData buildGameyData()
	{
		float timeDiff = endTimestamp - startTimestamp;
		int minutes = (int) (timeDiff / 60f);
		int seconds = (int) (((timeDiff / 60f) - minutes) * 60f);

		PDGameyResultData reqData = new PDGameyResultData(numPackagesLostToMonkies,numDeliveries,numWrongDeliveries,minutes,seconds);
		return reqData;
	}


	protected new bool initLevelConfigGenerator()
	{
		if( ! base.initLevelConfigGenerator())
		{
			// Fallback.
			lvlConfigGen = new PDLevelConfigGeneratorServer(null); //new PDLevelConfigGeneratorHardCoded();
			Debug.LogWarning("Warning: Using Level Gen Fallback");
		}
		return true;
	}



	public void pickupPackage()
	{
		triggerSoundAtCamera("Pickup");
		addPackageToInventory(true);
	}



	private void addPackageToInventory(bool para_performEffect)
	{
		numOfPackagesInInventory++;
		refreshInventoryCounterDisplay();
		if(numOfPackagesInInventory == 1)
		{
			GameObject tmpPackageUIIcon = GameObject.Find("PackageUIIcon");
			tmpPackageUIIcon.GetComponent<SpriteRenderer>().color = Color.white;
			transform.GetComponent<PDMonkeyManager>().informAllMonkeysOfPlayerStock(true);
		}
		if(para_performEffect) { addToInventoryEffect(mainCam.transform.Find("PackageUIIcon").gameObject,"AddInventoryEffect"); }
	}

	private void addBananaToInventory(bool para_performEffect)
	{
		numOfBananasInInventory++;
		refreshInventoryCounterDisplay();
		if(numOfBananasInInventory == 1)
		{
			GameObject.Find("BananaUIIcon").GetComponent<SpriteRenderer>().color = Color.white;
		}
		if(para_performEffect) { addToInventoryEffect(mainCam.transform.Find("BananaUIIcon").gameObject,"AddInventoryEffect"); }
	}

	private void addToInventoryEffect(GameObject para_IconGObj, string para_effectTitle)
	{
		Vector3 currLocalScale = new Vector3(1,1,1);//para_IconGObj.transform.localScale;
		if(para_IconGObj.name == "PackageUIIcon") { currLocalScale.x = 0.5f; currLocalScale.y = 0.5f; }

		Destroy(para_IconGObj.GetComponent<InjectLocalScale>());
		Destroy(para_IconGObj.GetComponent<GrowOrShrink>());
		Vector3 tmpScale = para_IconGObj.transform.localScale;
		tmpScale.x = 1;
		tmpScale.y = 1;
		tmpScale.z = 1;
		para_IconGObj.transform.localScale = tmpScale;

		CustomAnimationManager aniMang = para_IconGObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("InjectLocalScale",1,new List<System.Object>(){new float[3]{0,0,0}}));
		List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
		batch2.Add(new AniCommandPrep("GrowOrShrink",1,new List<System.Object>() { new float[3] {currLocalScale.x,currLocalScale.y,currLocalScale.z}, 3f }));
		
		batchLists.Add(batch1);
		batchLists.Add(batch2);
		aniMang.init(para_effectTitle,batchLists);
	}

	private void deductPackageFromInventory(bool para_performEffect)
	{
		if(numOfPackagesInInventory > 0)
		{
			numOfPackagesInInventory--;
			refreshInventoryCounterDisplay();
			if(numOfPackagesInInventory == 0)
			{
				GameObject tmpPackageUIIcon = GameObject.Find("PackageUIIcon");
				tmpPackageUIIcon.GetComponent<SpriteRenderer>().color = Color.gray;
				transform.GetComponent<PDMonkeyManager>().informAllMonkeysOfPlayerStock(false);
			}
			if(para_performEffect) { deductFromInventoryEffect(mainCam.transform.FindChild("PackageUIIcon").gameObject,"DeductInventoryEffect"); }
		}
	}

	private void deductBananaFromInventory(bool para_performEffect)
	{
		if(numOfBananasInInventory > 0)
		{
			numOfBananasInInventory--;
			refreshInventoryCounterDisplay();
			if(numOfBananasInInventory == 0)
			{
				GameObject.Find("BananaUIIcon").GetComponent<SpriteRenderer>().color = Color.gray;
			}
			if(para_performEffect) { deductFromInventoryEffect(mainCam.transform.FindChild("BananaUIIcon").gameObject,"DeductInventoryEffect"); }
		}
	}

	private void deductFromInventoryEffect(GameObject para_IconGObj, string para_effectTitle)
	{
		Vector3 currLocalScale = new Vector3(1,1,1);//para_IconGObj.transform.localScale;
		if(para_IconGObj.name == "PackageUIIcon") { currLocalScale.x = 0.5f; currLocalScale.y = 0.5f; }
		
		Destroy(para_IconGObj.GetComponent<InjectLocalScale>());
		Destroy(para_IconGObj.GetComponent<GrowOrShrink>());
		Vector3 tmpScale = para_IconGObj.transform.localScale;
		tmpScale.x = 1;
		tmpScale.y = 1;
		tmpScale.z = 1;
		para_IconGObj.transform.localScale = tmpScale;

		CustomAnimationManager aniMang = para_IconGObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("GrowOrShrink",1,new List<System.Object>() { new float[3] { 0,0,0 }, 3f }));
		List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
		batch2.Add(new AniCommandPrep("InjectLocalScale",1,new List<System.Object>(){new float[3] {currLocalScale.x,currLocalScale.y,currLocalScale.z}}));
		batchLists.Add(batch1);
		batchLists.Add(batch2);
		aniMang.registerListener("AcScen",this);
		aniMang.init(para_effectTitle,batchLists);
	}

	private void refreshInventoryCounterDisplay()
	{
		GameObject tmpPackageUIIcon = GameObject.Find("PackageUIIcon");
		GameObject packageCounter = tmpPackageUIIcon.transform.FindChild("PackageUICounter").gameObject;
		TextMesh packageCounterTMesh = packageCounter.GetComponent<TextMesh>();
		packageCounterTMesh.text = ""+numOfPackagesInInventory;
		packageCounter.renderer.material.color = Color.black;

		GameObject tmpBananaUIIcon = GameObject.Find("BananaUIIcon");
		GameObject bananaCounter = tmpBananaUIIcon.transform.FindChild("BananaUICounter").gameObject;
		TextMesh bananaCounterTMesh = bananaCounter.GetComponent<TextMesh>();
		bananaCounterTMesh.text = ""+numOfBananasInInventory;
		bananaCounter.renderer.material.color = Color.black; // Color.yellow;
	}

	private void emptyPackageInventoryEffect()
	{
		Transform iconGObj = Camera.main.transform.FindChild("PackageUIIcon");

		CustomAnimationManager aniMang = iconGObj.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] {1,0,0,1}, 0.5f }));
		List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
		batch2.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] { 0.5f,0.5f,0.5f,1 }, 0.5f}));
		List<AniCommandPrep> batch3 = new List<AniCommandPrep>();
		batch3.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] {1,0,0,1}, 0.5f }));
		List<AniCommandPrep> batch4 = new List<AniCommandPrep>();
		batch4.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] { 0.5f,0.5f,0.5f,1 }, 0.5f}));
		batchLists.Add(batch1);
		batchLists.Add(batch2);
		batchLists.Add(batch3);
		batchLists.Add(batch4);
		aniMang.registerListener("AcScen",this);
		aniMang.init("EmptyPackageInventoryFlash",batchLists);

		triggerSoundAtCamera("Buzzer_wrong_split");
	}

	public void handleMonkeyPackageSteal(string para_monkeyName)
	{
		deductPackageFromInventory(true);

		GameObject tmpPackageUIIcon = GameObject.Find("PackageUIIcon");
		Rect packageIcon2Dbounds = CommonUnityUtils.get2DBounds(tmpPackageUIIcon.renderer.bounds);
		Rect smallPackageIconBounds = new Rect(packageIcon2Dbounds.x + (packageIcon2Dbounds.width/2f) - 0.125f,
		                                       packageIcon2Dbounds.y - (packageIcon2Dbounds.height/2f) + 0.125f,
		                                       0.25f,
		                                       0.25f);

		Vector3 packageSpawnPt = new Vector3(smallPackageIconBounds.x + (smallPackageIconBounds.width/2f),smallPackageIconBounds.y - (smallPackageIconBounds.height/2f),-1);

		// Note: numOfPackagesInInventory can be used as an index without -1 because detuctPackageFromInventory() call above should have decremented it already.
		Transform reqPackagePrefab = packagePrefabs[selectedPackagePrefabIndexes[numOfPackagesInInventory]];
		//GameObject stolenPackageObj = WorldSpawnHelper.initObjWithinWorldBounds(reqPackagePrefab,"StolenPackage-"+(numOfPackagesInInventory+1),smallPackageIconBounds,-1,upAxisArr);
		GameObject stolenPackageObj = ((Transform) Instantiate(reqPackagePrefab,packageSpawnPt,Quaternion.identity)).gameObject;
		stolenPackageObj.name = "StolenPackage-"+(numOfPackagesInInventory);
		stolenPackageObj.transform.localScale = new Vector3(0.18f,0.18f,1f);
		if(stolenPackageObj.collider != null) { stolenPackageObj.collider.enabled = false; }

		stolenPackageToMonkeyQueue.Add(new string[] {stolenPackageObj.name,para_monkeyName});
	}



	private void triggerHouseViewStart()
	{  
		toggleInputDetectors(false);
		
		float totalEnterTime_Sec = 1f;
		
		AdjustCameraViewport acv = GameObject.Find("Main Camera").AddComponent<AdjustCameraViewport>();
		acv.init(new Vector4(0,0,0.5f,1),totalEnterTime_Sec);
		
		GameObject doorCamGObj = GameObject.Find("DoorCam");
		doorCamGObj.GetComponent<Camera>().enabled = true;
		AdjustCameraViewport acv2 = GameObject.Find("DoorCam").AddComponent<AdjustCameraViewport>();
		acv2.registerListener("AcScen",this);
		acv2.init(new Vector4(0.5f,0,1,1),totalEnterTime_Sec);


		if(serverCommunication != null) { serverCommunication.wordDisplayed(currLvlConfig.getParcelWord()[currentWordIdx],currLvlConfig.languageArea[currentWordIdx],currLvlConfig.difficulty[currentWordIdx]); }
		recordPresentedConfig(currLvlConfig);
		
		inHouseViewMode = true;

		transform.GetComponent<PDMonkeyManager>().findMonkeyCounter(attempts);


	}

	private void handleDoorKnock()
	{
		TripTimer tTimer = null;
		if(transform.gameObject.GetComponent<TripTimer>() == null)
		{
			tTimer = transform.gameObject.AddComponent<TripTimer>();
			tTimer.registerListener("AcScen",this);
			tTimer.init(tripTimerWaitDelay_Sec);
		}
		tTimer = transform.gameObject.GetComponent<TripTimer>();
		tTimer.interruptAndRestart();
		
		
		triggerSoundAtCamera("Knock");
		currNumKnocks++;
		
		GameObject houseDoorGObj = GameObject.Find("HouseDoor");
		TextMesh knockCounterTMesh = houseDoorGObj.transform.FindChild("DoorKnockCounter").FindChild("Text").GetComponent<TextMesh>();
		knockCounterTMesh.text = ""+currNumKnocks;
	}


	private void handleKnockSequenceOutcome()
	{
		toggleInputDetectors(false);

		GameObject knockCounter = GameObject.Find("HouseDoor").transform.FindChild("DoorKnockCounter").FindChild("Text").gameObject;




		if(currLvlConfig.isCorrectNumOfKnocks(currNumKnocks,currentWordIdx))
		{


			if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(currLvlConfig.getParcelWord()[currentWordIdx],true,"",currLvlConfig.languageArea[currentWordIdx],currLvlConfig.difficulty[currentWordIdx]); }


			if(currentWordIdx == (currLvlConfig.getParcelWord().Length-1)){//Done with this door

			// Open door and present letter.

				numDeliveries++;

				CustomAnimationManager aniMang = knockCounter.AddComponent<CustomAnimationManager>();
				List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
				List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
				batch1.Add(new AniCommandPrep("ColorTransitionNonSprite",0,new List<System.Object>() { new float[4] {0,0.5f,0,1}, 0.5f }));
				List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
				batch2.Add(new AniCommandPrep("ColorTransitionNonSprite",0,new List<System.Object>() { new float[4] {0,0,0,1}, 0.5f }));
				List<AniCommandPrep> batch3 = new List<AniCommandPrep>();
				batch3.Add(new AniCommandPrep("ColorTransitionNonSprite",0,new List<System.Object>() { new float[4] {0,0.5f,0,1}, 0.5f }));
				List<AniCommandPrep> batch4 = new List<AniCommandPrep>();
				batch4.Add(new AniCommandPrep("DelayForInterval",0,new List<System.Object>() { 1f }));
				batchLists.Add(batch1);
				batchLists.Add(batch2);
				batchLists.Add(batch3);
				batchLists.Add(batch4);
				aniMang.registerListener("AcScen",this);
				aniMang.init("GoodKnockSequenceFeedback",batchLists);

				triggerSoundAtCamera("SFX_SuccessScreen");

			}else{//Get a new word for this door




				currentWordIdx++;
				attempts--;
				setKnockInput(true);
				triggerSoundAtCamera("SFX_SuccessScreen");

				currNumKnocks = 0;
				knockCounter.renderer.material.color = Color.black;
				knockCounter.GetComponent<TextMesh>().text = " ";
				toggleInputDetectors(true);


				changeWord();

			}
		}
		else
		{

			attempts--;

			if(attempts==0){
			

				numWrongDeliveries++;

				GameObject tmpPackageUIIcon = GameObject.Find("PackageUIIcon");
				Rect packageIcon2Dbounds = CommonUnityUtils.get2DBounds(tmpPackageUIIcon.renderer.bounds);
				Rect smallPackageIconBounds = new Rect(packageIcon2Dbounds.x + (packageIcon2Dbounds.width/2f) - 0.125f,
				                                       packageIcon2Dbounds.y - (packageIcon2Dbounds.height/2f) + 0.125f,
				                                       0.25f,
				                                       0.25f);
				
				Vector3 packageSpawnPt = new Vector3(smallPackageIconBounds.x + (smallPackageIconBounds.width/2f),smallPackageIconBounds.y - (smallPackageIconBounds.height/2f),-1);
				
				Transform reqPackagePrefab = packagePrefabs[selectedPackagePrefabIndexes[numOfPackagesInInventory-1]];
				//GameObject stolenPackageObj = WorldSpawnHelper.initObjWithinWorldBounds(reqPackagePrefab,"StolenPackage-"+(numOfPackagesInInventory+1),smallPackageIconBounds,-1,upAxisArr);
				GameObject stolenPackageObj = ((Transform) Instantiate(reqPackagePrefab,packageSpawnPt,Quaternion.identity)).gameObject;
				stolenPackageObj.transform.localScale = new Vector3(0.18f,0.18f,1f);
				if(stolenPackageObj.collider != null) { stolenPackageObj.collider.enabled = false; }

				//stolenPackageObj.name = "StolenPackage-"+(numOfPackagesInInventory);

				Transform bckColor = stolenPackageObj.transform.Find("ParcelChild/Parcel"+(selectedPackagePrefabIndexes[numOfPackagesInInventory-1]+1)+"_Bottom");
				if(bckColor!=null)
					bckColor.gameObject.renderer.sortingOrder = 0;
				transform.GetComponent<PDMonkeyManager>().eatPackage(stolenPackageObj);//when attempts==0, monkey will eat the package



			}else{

				transform.GetComponent<PDMonkeyManager>().moveCounter(attempts);
			}
			// Wrong. Check if there are more attempts. Else remove door view and gen next level.
			if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(currLvlConfig.getParcelWord()[currentWordIdx],false,"",currLvlConfig.languageArea[currentWordIdx],currLvlConfig.difficulty[currentWordIdx]); }

			CustomAnimationManager aniMang = knockCounter.AddComponent<CustomAnimationManager>();
			List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
			List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
			batch1.Add(new AniCommandPrep("ColorTransitionNonSprite",0,new List<System.Object>() { new float[4] {0.5f,0,0,1}, 0.5f }));
			List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
			batch2.Add(new AniCommandPrep("ColorTransitionNonSprite",0,new List<System.Object>() { new float[4] {0,0,0,1}, 0.5f }));
			List<AniCommandPrep> batch3 = new List<AniCommandPrep>();
			batch3.Add(new AniCommandPrep("ColorTransitionNonSprite",0,new List<System.Object>() { new float[4] {0.5f,0,0,1}, 0.5f }));
			batchLists.Add(batch1);
			batchLists.Add(batch2);
			batchLists.Add(batch3);
			aniMang.registerListener("AcScen",this);
			aniMang.init("WrongKnockSequenceFeedback",batchLists);

			//numWrongDeliveries++;

			//triggerSoundAtCamera("Buzzer_wrong_split");

			//Continue on respond to event WrongKnockSequenceFeedback


		}
	}


	private void startDoorHandover()
	{
		// Open door. Init drag scripts. Present letter.

		GameObject houseDoor = GameObject.Find("HouseDoor");

		GameObject knockCounter = houseDoor.transform.FindChild("DoorKnockCounter").FindChild("Text").gameObject;
		knockCounter.renderer.material.color = Color.black;
		knockCounter.GetComponent<TextMesh>().text = " ";

		houseDoor.GetComponent<Animator>().Play("DoorOpen");
		GameObject doorHole = houseDoor.transform.FindChild("PDDragHole").gameObject;
		doorHole.SetActive(true);
		doorHole.GetComponent<HoleScript>().reset();
		doorHole.GetComponent<HoleScript>().registerListener("AcScen",this);
		triggerSoundAtCamera("DoorOpen");




		GameObject doorDeliveryBagGObj = GameObject.Find("DoorDeliveryBag");

		Vector3 packageSpawnPt = new Vector3(doorDeliveryBagGObj.transform.position.x,
		                                     doorDeliveryBagGObj.transform.position.y,
		                                     doorDeliveryBagGObj.transform.position.z - 0.1f);

		Transform reqPackagePrefab = packagePrefabs[selectedPackagePrefabIndexes[numOfPackagesInInventory-1]];
		Transform nwPackage = (Transform) Instantiate(reqPackagePrefab,packageSpawnPt,Quaternion.identity);
		nwPackage.name = "Package";
		nwPackage.parent = doorDeliveryBagGObj.transform;
		CommonUnityUtils.setSortingOrderOfEntireObject(nwPackage.gameObject,600);


		Vector3 deliveryBagEnterPosition = new Vector3(doorDeliveryBagGObj.transform.position.x,
		                                               doorDeliveryBagGObj.transform.position.y + (doorDeliveryBagGObj.renderer.bounds.size.y),
		                                               doorDeliveryBagGObj.transform.position.z);


		CustomAnimationManager aniMang = doorDeliveryBagGObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3] {deliveryBagEnterPosition.x,deliveryBagEnterPosition.y,deliveryBagEnterPosition.z},0.3f,true}));
		batchLists.Add(batch1);
		aniMang.registerListener("AcScen",this);
		aniMang.init("DeliveryBagEnter",batchLists);
	}

	public void triggerPackageAccept()
	{
		GameObject packageGObj = GameObject.Find("Package");

		GameObject doorDeliveryBagGObj = GameObject.Find("DoorDeliveryBag");
		doorDeliveryBagGObj.GetComponent<SpriteRenderer>().sortingOrder = 1000;

		packageGObj.rigidbody.isKinematic = false;
		packageGObj.rigidbody.useGravity = true;
		packageGObj.rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

		CustomAnimationManager aniMang = packageGObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("ColorTransition",0,new List<System.Object>() { new float[4] {0,0,0,0}, 1f }));
		batchLists.Add(batch1);
		aniMang.registerListener("AcScen",this);
		aniMang.init("PackageAcceptSequence",batchLists);
	}

	public void retractDeliveryBag()
	{
		GameObject doorDeliveryBagGObj = GameObject.Find("DoorDeliveryBag");
		doorDeliveryBagGObj.GetComponent<SpriteRenderer>().sortingOrder = 16;

		Vector3 deliveryBagExitPosition = new Vector3(doorDeliveryBagGObj.transform.position.x,
		                                              doorDeliveryBagGObj.transform.position.y - (doorDeliveryBagGObj.renderer.bounds.size.y),
		                                              doorDeliveryBagGObj.transform.position.z);
		
		
		CustomAnimationManager aniMang = doorDeliveryBagGObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3] {deliveryBagExitPosition.x,deliveryBagExitPosition.y,deliveryBagExitPosition.z},0.3f,true}));
		batchLists.Add(batch1);
		aniMang.registerListener("AcScen",this);
		aniMang.init("DeliveryBagExit",batchLists);
	}

	public void endDoorHandover()
	{
		GameObject.Find("HouseDoor").GetComponent<Animator>().Play("DoorClosed");
		GameObject doorHole = GameObject.Find("HouseDoor").transform.FindChild("PDDragHole").gameObject;
		doorHole.GetComponent<HoleScript>().reset();
		doorHole.SetActive(false);
		triggerSoundAtCamera("DoorClose");




		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>() { 1f }));
		batchLists.Add(batch1);
		aniMang.registerListener("AcScen",this);
		aniMang.init("DoorHandoverEnd",batchLists);
	}

	public void triggerHouseViewEnd()
	{

		transform.GetComponent<PDMonkeyManager>().informAllMonkeysOfPlayerStock((numOfPackagesInInventory > 0));
		

		toggleInputDetectors(false);
		
		float totalExitTime_Sec = 1f;
		
		AdjustCameraViewport acv = GameObject.Find("Main Camera").AddComponent<AdjustCameraViewport>();
		acv.init(new Vector4(0,0,1,1),totalExitTime_Sec);
		
		GameObject doorCamGObj = GameObject.Find("DoorCam");
		doorCamGObj.GetComponent<Camera>().enabled = true;
		AdjustCameraViewport acv2 = GameObject.Find("DoorCam").AddComponent<AdjustCameraViewport>();
		acv2.registerListener("AcScen",this);
		acv2.init(new Vector4(1,0,1,1),totalExitTime_Sec);

		buildNRecordConfigOutcome(null);
		
		inHouseViewMode = false;
	}


	public void fireBanana(GameObject para_targetObj)
	{
		deductBananaFromInventory(true);

		GameObject playerAvatar = GameObject.Find("MainAvatar");

		Vector3 bananaSpawnPos = new Vector3(playerAvatar.transform.position.x,
		                                     playerAvatar.transform.position.y,
		                                     playerAvatar.transform.position.z - 0.1f);

		Transform nwBanana = (Transform) Instantiate(bananaPrefab,bananaSpawnPos,Quaternion.identity);
		nwBanana.name = "BananaShot";

		Vector3 fireDirection = Vector3.Normalize(para_targetObj.transform.position - playerAvatar.transform.position);

		triggerSoundAtCamera("throw");

		CustomAnimationManager aniMang = nwBanana.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("ShootNDecelerate",1,new List<System.Object>() { new float[3] {fireDirection.x,fireDirection.y,fireDirection.z} ,0.5f,1f }));
		List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
		batch2.Add(new AniCommandPrep("TriggerAnimation",2,new List<System.Object>() { "Splat", 1f }));
		List<AniCommandPrep> batch3 = new List<AniCommandPrep>();
		batch3.Add(new AniCommandPrep("DestroyObject",1,new List<System.Object>()));
		batchLists.Add(batch1);
		batchLists.Add(batch2);
		batchLists.Add(batch3);
		aniMang.init("FireBanana",batchLists);
	}






	private void renderBackgroundTiles()
	{
		GameObject backgroundElements = new GameObject("BackgroundElements");

		List<string> para_typeNames = worldNavGraph.getAllTypeNames();

		for(int i=0; i<para_typeNames.Count; i++)
		{
			string tileSetName = para_typeNames[i];

			int reqTypeID = worldNavGraph.getTypeIDByName(tileSetName);
			List<int> reqNodeKeys = worldNavGraph.getNodesOfType(reqTypeID);

			for(int j=0; j<reqNodeKeys.Count; j++)
			{
				WorldNode tmpNode = (WorldNode) worldNavGraph.getNode(reqNodeKeys[j]);
				Vector3 worldPt = tmpNode.getWorldPt();

				GameObject nwBkElement = GameObject.CreatePrimitive(PrimitiveType.Quad);
				nwBkElement.transform.position = new Vector3(worldPt.x,worldPt.y,-0.5f);
				nwBkElement.transform.localScale = new Vector3(gPropMapWorldBounds.cellWidth/nwBkElement.transform.localScale.x,
				                                               gPropMapWorldBounds.cellHeight/nwBkElement.transform.localScale.y,
				                                               1);

				nwBkElement.renderer.material.color = worldNavGraph.getColorByTypeName(tileSetName);
				nwBkElement.transform.parent = backgroundElements.transform;
			}
		}
	}

	private void toggleInputDetectors(bool para_onStatus)
	{
		if(inputDetectors != null)
		{
			for(int i=0; i<inputDetectors.Count; i++)
			{
				inputDetectors[i].toggleInputStatus(para_onStatus);
			}
		}

	}

	private void setKnockInput(bool para_onStatus)
	{
		for(int i=0; i<inputDetectors.Count; i++)
		{
			if(inputDetectors[i] is ClickDetector)
			{
				inputDetectors[i].toggleInputStatus(para_onStatus);
			}
		}
	}

	private void triggerSoundAtCamera(string para_soundFileName)
	{
		GameObject camGObj = GameObject.Find("Main Camera");
		
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = 0.5f;
		audS.Play();
	}

	protected new void prepUIBounds()
	{
		uiBounds = new Dictionary<string, Rect>();
	}

	private GridProperties calculateWorldMapBounds(int[] para_mapDimensions)
	{
		int counter = 1;
		bool hasFoundAdditionalMapSegment = true;
		List<Bounds> segmentBoundsList = new List<Bounds>();
		while(hasFoundAdditionalMapSegment)
		{
			GameObject mapSegment = GameObject.Find("WorldView_Map"+counter);
			if(mapSegment == null)
			{
				hasFoundAdditionalMapSegment = false;
			}
			else
			{
				segmentBoundsList.Add(mapSegment.renderer.bounds);
			}
			counter++;
		}
		Bounds mapWorldBounds = findMaxBounds(segmentBoundsList);
		Vector3 mapWorldBounds_TL = (mapWorldBounds.center) + new Vector3(-(mapWorldBounds.size.x/2f),(mapWorldBounds.size.y/2f),0);
		GridProperties reqGProp = new GridProperties(new Rect(mapWorldBounds_TL.x,mapWorldBounds_TL.y,mapWorldBounds.size.x,mapWorldBounds.size.y),para_mapDimensions[0],para_mapDimensions[1],0,0);
		reqGProp.z = mapWorldBounds.center.z;	 
		
		return reqGProp;
	}

	private Bounds findMaxBounds(List<Bounds> para_objs)
	{
		Bounds firstBound = para_objs[0];
		float[] minPt = new float[3] { firstBound.min.x, firstBound.min.y, firstBound.min.z };
		float[] maxPt = new float[3] { firstBound.max.x, firstBound.max.y, firstBound.max.z };
		
		for(int i=0; i<para_objs.Count; i++)
		{
			Bounds tmpB = para_objs[i];
			
			if(tmpB.min.x < minPt[0]) { minPt[0] = tmpB.min.x; }
			if(tmpB.min.y < minPt[1]) { minPt[1] = tmpB.min.y; }
			if(tmpB.min.z < minPt[2]) { minPt[2] = tmpB.min.z; }
			if(tmpB.max.x > maxPt[0]) { maxPt[0] = tmpB.max.x; }
			if(tmpB.max.y > maxPt[1]) { maxPt[1] = tmpB.max.y; }
			if(tmpB.max.z > maxPt[2]) { maxPt[2] = tmpB.max.z; }
		}
		
		Vector3 minPtVect = new Vector3(minPt[0],minPt[1],minPt[2]);
		Vector3 maxPtVect = new Vector3(maxPt[0],maxPt[1],maxPt[2]);
		
		
		
		Vector3 sizeVect = new Vector3(maxPtVect.x - minPtVect.x,maxPtVect.y - minPtVect.y,maxPtVect.z - minPtVect.z);
		Vector3 centreVect = new Vector3(minPtVect.x + (sizeVect.x/2f),minPtVect.y + (sizeVect.y/2f),minPtVect.z + (sizeVect.z/2f));
		Bounds reqBounds = new Bounds(centreVect,sizeVect);
		return reqBounds;
	}

	private void applyPlayerAvatarClothing()
	{
		GameObject poRef = PersistentObjMang.getInstance();
		if(poRef != null)
		{
			DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
			if(ds != null)
			{
				GameObject mainAv = GameObject.Find("MainAvatar");
				if(mainAv != null)
				{
					if(ds.containsData("PlayerAvatarSettings"))
					{
						PlayerAvatarSettings playerAvSettings = (PlayerAvatarSettings) ds.getData("PlayerAvatarSettings");
						ClothingConfig storedClothingConfig = playerAvSettings.getClothingSettings();
						ClothingApplicator cApp = transform.gameObject.AddComponent<ClothingApplicator>();
						cApp.setSubject(mainAv,avController_NonLooping);
						cApp.applyClothingConfig(storedClothingConfig,ClothingSize.SMALL);
						Destroy(cApp);
						mainAv.GetComponent<Animator>().runtimeAnimatorController = avController_NonLooping;
					}
				}
			}
		}
	}



	private void changeWord(){

		Debug.Log("NEW WORD: "+currLvlConfig.getParcelWord()[currentWordIdx]+" "+currentWordIdx);

		GameObject nwHouseDoor = GameObject.Find("HouseDoor");

		if(nwHouseDoor.transform.FindChild("DoorWord") != null)
		{
			Destroy(nwHouseDoor.transform.FindChild("DoorWord").gameObject);
		}

		GameObject parcelWordGObj = WordBuilderHelper.buildWordBox(-99,currLvlConfig.getParcelWord()[currentWordIdx],CommonUnityUtils.get2DBounds(tmp),-5f,upAxisArr,wordboxPrefab);
		WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() {parcelWordGObj},0.08f);
		parcelWordGObj.name = "DoorWord";
		parcelWordGObj.transform.parent = nwHouseDoor.transform;
		Destroy(parcelWordGObj.transform.FindChild("Board").gameObject);
		parcelWordGObj.transform.FindChild("Text").renderer.sortingOrder = 500;// nwHouseDoor.renderer.sortingOrder+1;
		
		
		// Init knock counter on door.
		//if(nwHouseDoor.transform.FindChild("DoorKnockCounter") != null)
		//{
		//	Destroy(nwHouseDoor.transform.FindChild("DoorKnockCounter").gameObject);
		//}
		//GameObject doorKnockCounterAreaGuide = nwHouseDoor.transform.FindChild("KnockCounterArea").gameObject;
		
		/*GameObject knockCounterGObj = WordBuilderHelper.buildWordBox(-99," ",CommonUnityUtils.get2DBounds(doorKnockCounterAreaGuide.renderer.bounds),-5f,upAxisArr,wordboxPrefab);
		WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() {knockCounterGObj},0.09f);
		knockCounterGObj.name = "DoorKnockCounter";
		knockCounterGObj.transform.parent = nwHouseDoor.transform;
		Destroy(knockCounterGObj.transform.FindChild("Board").gameObject);
		knockCounterGObj.transform.FindChild("Text").renderer.sortingOrder = 500;*/


	}



	Bounds tmp;

	private void createRandomHouseDoor()
	{

		currentWordIdx = 0;
		attempts = currLvlConfig.getAttempts();



		GameObject houseDoorGObj = GameObject.Find("HouseDoor");
		Transform childKnockArea = houseDoorGObj.transform.FindChild("KnockArea");
		Transform childKnockCounterArea = houseDoorGObj.transform.FindChild("KnockCounterArea");
		Transform childPDDragHole = houseDoorGObj.transform.FindChild("PDDragHole");

		Transform randDoorPrefab = doorPrefabs[Random.Range(0,doorPrefabs.Length)];
		Transform nwHouseDoor = (Transform) Instantiate(randDoorPrefab,houseDoorGObj.transform.position,Quaternion.identity);
		nwHouseDoor.name = "HouseDoor";
		nwHouseDoor.localScale = new Vector3(houseDoorGObj.transform.localScale.x,houseDoorGObj.transform.localScale.y,houseDoorGObj.transform.localScale.z);

		if(childKnockArea != null) { childKnockArea.transform.parent = nwHouseDoor; }
		if(childKnockCounterArea != null) { childKnockCounterArea.transform.parent = nwHouseDoor; }
		if(childPDDragHole != null) { childPDDragHole.transform.parent = nwHouseDoor; }



		// Init word on door.

		if(nwHouseDoor.transform.FindChild("DoorWord") != null)
		{
			Destroy(nwHouseDoor.transform.FindChild("DoorWord").gameObject);
		}
		GameObject doorWordAreaGuide = nwHouseDoor.transform.FindChild("TextArea").gameObject;

		tmp = doorWordAreaGuide.renderer.bounds;
		
		GameObject parcelWordGObj = WordBuilderHelper.buildWordBox(-99,currLvlConfig.getParcelWord()[currentWordIdx],CommonUnityUtils.get2DBounds(tmp),-5f,upAxisArr,wordboxPrefab);
		WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() {parcelWordGObj},0.08f);
		parcelWordGObj.name = "DoorWord";
		parcelWordGObj.transform.parent = nwHouseDoor.transform;
		Destroy(parcelWordGObj.transform.FindChild("Board").gameObject);
		parcelWordGObj.transform.FindChild("Text").renderer.sortingOrder = 500;// nwHouseDoor.renderer.sortingOrder+1;
		
		
		// Init knock counter on door.
		if(nwHouseDoor.transform.FindChild("DoorKnockCounter") != null)
		{
			Destroy(nwHouseDoor.transform.FindChild("DoorKnockCounter").gameObject);
		}
		GameObject doorKnockCounterAreaGuide = nwHouseDoor.transform.FindChild("KnockCounterArea").gameObject;
		
		GameObject knockCounterGObj = WordBuilderHelper.buildWordBox(-99," ",CommonUnityUtils.get2DBounds(doorKnockCounterAreaGuide.renderer.bounds),-5f,upAxisArr,wordboxPrefab);
		WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() {knockCounterGObj},0.09f);
		knockCounterGObj.name = "DoorKnockCounter";
		knockCounterGObj.transform.parent = nwHouseDoor.transform;
		Destroy(knockCounterGObj.transform.FindChild("Board").gameObject);
		knockCounterGObj.transform.FindChild("Text").renderer.sortingOrder = 500;



		nwHouseDoor.GetComponent<Animator>().Play("DoorClosed");

		Destroy(doorWordAreaGuide);
		Destroy(houseDoorGObj);
	}
}

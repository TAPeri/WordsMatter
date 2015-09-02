/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AcMovingPathwaysScenario : ILearnRWActivityScenario, CustomActionListener
{
	public Transform boardCellPrefab;
	public Transform backgroundTileablePrefab;

	public Transform borderItemTopPrefab;
	public Transform borderItemLeftPrefab;
	public Transform borderItemRightPrefab;
	public Transform borderItemTLCornerPrefab;
	public Transform borderItemTRCornerPrefab;
	public Transform borderItemBLCornerPrefab;
	public Transform borderItemBRCornerPrefab;

	public Transform debugPrefab;
	public Transform wordBoxPrefab;
	public Transform playerPrefab;
	public Transform bystanderPrefab;
	public Transform outlinePrefab;
	public Transform gravelPrefab;
	public Transform arrowMarkerPrefab;

	public Transform bystanderBubblePrefab;
	public Transform talkBubblePrefab;
	public Transform thumbsUpBubblePrefab;

	public Transform hexProberPrefab;

	Transform sfxPrefab = null;
	

	MPLevelConfig currLvlConfig;
	bool[] upAxisArr = { false, true, false };

	bool metaDataLoaded = false;
	bool hasInitIntroSequence = false;

	bool firstLevel;
	string[,] boardContent;
	int[] boardSize;
	Dictionary<string,Rect> worldBoundsLookup;
	bool playerInputOn;
	
	BasicNavGraph boardNavGraph;
	Dictionary<string,int> cellNameToNodeIDMap;
	Dictionary<int,string> nodeIDToCellNameMap;


	int[] currPlayerCell;
	int[] currDestCell;

	string chosenGoalSide;
	Vector3 chosenBystanderGoalPt;

	string[] cellsSelectedForRotation;

	Dictionary<string,AbsInputDetector> inputDetectScripts;


	bool inAutoRotateMode;
	//int currAutoRotateIndex = 0;
	string[] cellsToAutoRotate;

	GameObject activatedCornerButtonObj;
	GameObject activatedGravelObj;
	bool rotationReturnFlag = true;

	List<GameObject> hiddenCornerButtons = null;

	bool playerIsEnroute = false;
	string playerWalkEndCell;

	bool isPauseButtonVisible = true;
	bool isPaused = false;
	
	
	//int numClicks;


	int numCompletedPaths = 0;
	int reqPathsForWin; // Init when lvl gen is init.

	int numCompletePaths = 0;
	int numMissteps = 0;
	int numRotations = 0;
	float startTimestamp = 0;
	float endTimestamp = 0;


	int descriptionType = 0;
	// 0 = text only. 1 = sound only.
	bool descriptionTileButtonOn = false;



	void Start()
	{

		acID = ApplicationID.MOVING_PATHWAYS;
		metaDataLoaded = loadActivitySessionMetaData();

		loadTextures();
		prepUIBounds();

		initWorld();
		this.initLevelConfigGenerator();
		reqPathsForWin = lvlConfigGen.getConfigCount();
		genNextLevel();

		initPersonHelperPortraitPic();
		showInfoSlideshow();
		recordActivityStart();
	}
	
	

	void OnGUI()
	{
		GUI.color = Color.clear;

		if( ! isPaused)
		{
			if(uiBounds.ContainsKey("PauseBtn"))
			{
				if(isPauseButtonVisible)
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

					//GUI.color = Color.white;
					if(descriptionTileButtonOn)
					{
						if(GUI.Button(uiBounds["InstructionTile"],""))
						{
							try
							{
								if(gameObject.GetComponent<AudioSource>() == null) { gameObject.AddComponent<AudioSource>(); }
								audio.PlayOneShot(WorldViewServerCommunication.tts.say(currLvlConfig.pattern));
							}
							catch(System.Exception ex)
							{
								Debug.LogError("Failed to use TTS. "+ex.Message);
							}
						}
					}
				}
			}
		}
		
		GUI.color = Color.white;
	}


	private bool checkNHandle_boardCellCentreClick(Vector2 para_clickPos)
	{
		bool successFlag = false;

		RaycastHit hitInf;
		if(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(para_clickPos.x,para_clickPos.y,0)),out hitInf))
		{
			// Check for board cell hit.
			
			if((hitInf.collider.gameObject.transform.parent != null)
			   &&(hitInf.collider.gameObject.transform.parent.name == "HexBoard"))
			{
				// Has hit board cell centre.
				successFlag = true;
				

				GameObject wholeCellObj = hitInf.collider.gameObject;
				Transform cellWBox = wholeCellObj.transform.FindChild("WordBox");
				Transform textChild = cellWBox.FindChild("Text");
				TextMesh childTMesh = textChild.GetComponent<TextMesh>();
				
				if(currLvlConfig.isCorrectItem(childTMesh.text))
				{
					int playerNodeID = cellNameToNodeIDMap["Cell("+currPlayerCell[0]+","+currPlayerCell[1]+")"];
					int hitNodeID = cellNameToNodeIDMap[wholeCellObj.name];
					
					if(boardNavGraph.edgeExists(playerNodeID,hitNodeID))
					{
						if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(childTMesh.text,true,currLvlConfig.pattern,currLvlConfig.languageArea,currLvlConfig.difficulty); }
						triggerAvatarWalkToCell(wholeCellObj.name);
					}
				}
				else
				{
					int playerNodeID = cellNameToNodeIDMap["Cell("+currPlayerCell[0]+","+currPlayerCell[1]+")"];
					int hitNodeID = cellNameToNodeIDMap[wholeCellObj.name];

					if(boardNavGraph.edgeExists(playerNodeID,hitNodeID))
					{
						triggerSoundAtCamera("Buzzer_wrong_split");

						numMissteps++;
						if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(childTMesh.text,false,currLvlConfig.pattern,currLvlConfig.languageArea,currLvlConfig.difficulty); }
					}
				}
			}
		}

		return successFlag;
	}


	private bool checkNHandle_triCellRotationClick(Vector2 para_clickPos)
	{

		// Attempt rotation check.
		bool successFlag = false;
		
		
		float probeLength_world = 1f;
		Vector3 camPos = Camera.main.transform.position;
		Vector3 projPos = (camPos + ((new Vector3(0,1,0)) * probeLength_world));
		float probeLength_screen = (Camera.main.WorldToScreenPoint(projPos) - Camera.main.WorldToScreenPoint(camPos)).magnitude;


		List<string> unmovableCells = new List<string>();
		unmovableCells.Add("Cell("+currPlayerCell[0]+","+currPlayerCell[1]+")");
		unmovableCells.Add("Cell("+currDestCell[0]+","+currDestCell[1]+")");

		
		// 2 possible formations:
		
		List<string> probeCellsHit = new List<string>();
		
		float[] form1_angles = { 45, 135, 270 };
		float[] form2_angles = { 90, 225, 315 };
		
		List<float[]> angleSets = new List<float[]>() { form1_angles, form2_angles };
		
		bool foundValidRotationInstance = false;
		int chosenSet = 0;
		for(int m=0; m<angleSets.Count; m++)
		{
			if(foundValidRotationInstance)
			{
				break;
			}
			
			float[] tmpAngleArr = angleSets[m];
			
			Vector2 northVect = new Vector2(0,1);
			
			Vector2 nxtProbeLoc2D;
			Vector2 movVect = new Vector2();
			for(int k=0; k<tmpAngleArr.Length; k++)
			{
				float thetaDeg = tmpAngleArr[k];
				float thetaRad = (thetaDeg * Mathf.PI) / 180f;
				float x1 = (northVect.x * Mathf.Cos(thetaRad)) - (northVect.y * Mathf.Sin(thetaRad));
				float y1 = (northVect.x * Mathf.Sin(thetaRad)) + (northVect.y * Mathf.Cos(thetaRad));
				
				movVect.x = -x1;
				movVect.y = y1;
				movVect *= probeLength_screen;
				
				nxtProbeLoc2D = para_clickPos + movVect;


				RaycastHit hitInf;
				if(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(nxtProbeLoc2D.x,nxtProbeLoc2D.y,0)),out hitInf))
				{
					// Check for board cell hit.
					
					if((hitInf.collider.gameObject.transform.parent != null)
					   &&(hitInf.collider.gameObject.transform.parent.name == "HexBoard"))
					{
						if((probeCellsHit.Contains(hitInf.collider.gameObject.name))
						||(unmovableCells.Contains(hitInf.collider.gameObject.name)))
						{
							probeCellsHit.Clear();
							break;
						}
						
						probeCellsHit.Add(hitInf.collider.gameObject.name);
						
						if(probeCellsHit.Count == 3)
						{
							foundValidRotationInstance = true;
							break;
						}
					}
					else
					{
						probeCellsHit.Clear();
						break;
					}
				}
				
			}
			
			
			if(foundValidRotationInstance)
			{
				break;
			}

			chosenSet++;
		}
		
		
		if(foundValidRotationInstance)
		{
			// Highlight cells marked for rotation.
			//GameObject hexBoardObj = GameObject.Find("HexBoard");
			
			for(int m=0; m<cellsSelectedForRotation.Length; m++)
			{
				if(cellsSelectedForRotation[m] == null)
				{
					break;
				}

			}


			for(int m=0; m<probeCellsHit.Count; m++)
			{
				
				cellsSelectedForRotation[m] = probeCellsHit[m];
			}


			// Adjust vertex button appearance.
			RaycastHit hitInf;
			if(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(para_clickPos.x,para_clickPos.y,0)),out hitInf))
			{
				if(hitInf.collider.name.Contains("CornerButton"))
				{
					activatedCornerButtonObj = hitInf.collider.gameObject;
					activatedCornerButtonObj.renderer.sortingOrder = 8;
					activatedCornerButtonObj.GetComponent<Animator>().Play("clicked");
				}
			}


			numRotations++;

			triggerRotationEffect(chosenSet + 1);




			successFlag = true;
		}


		return successFlag;
	}



	private void triggerRotationEffect(int para_rotOption)
	{
		rotationReturnFlag = false;

		List<GameObject> reqChildren = new List<GameObject>();

		GameObject hexBoardObj = GameObject.Find("HexBoard");

		bool attachPlayerAvatar = false;
		string currPlayerCellFullName = "Cell("+currPlayerCell[0]+","+currPlayerCell[1]+")";
		for(int i=0; i<cellsSelectedForRotation.Length; i++)
		{
			Transform reqChild = hexBoardObj.transform.FindChild(cellsSelectedForRotation[i]);
			reqChild.renderer.sortingOrder = 6;
			reqChild.FindChild("WordBox").FindChild("Text").renderer.sortingOrder = 7;


			if(currPlayerCellFullName == reqChild.name)
			{
				string nwCellName = cellsSelectedForRotation[(i+1)%cellsSelectedForRotation.Length];
				int[] nwPlayerCoords = extractCoordsFromCellName(nwCellName);
				currPlayerCell[0] = nwPlayerCoords[0];
				currPlayerCell[1] = nwPlayerCoords[1];
				attachPlayerAvatar = true;
			}



			reqChild.name = "Dummy"+i;
			reqChildren.Add(reqChild.gameObject);
		}

		for(int i=0; i<reqChildren.Count; i++)
		{
			string nwCellName = cellsSelectedForRotation[(i+1)%cellsSelectedForRotation.Length];
			reqChildren[i].name = nwCellName;
		}



		// Find Centroid.
		Vector3 centroidVect = new Vector3(0,0,0);
		for(int i=0; i<reqChildren.Count; i++)
		{
			centroidVect.x += reqChildren[i].transform.position.x;
			centroidVect.y += reqChildren[i].transform.position.y;
			centroidVect.z += reqChildren[i].transform.position.z;
		}
		centroidVect.x = centroidVect.x / (reqChildren.Count * 1.0f);
		centroidVect.y = centroidVect.y / (reqChildren.Count * 1.0f);
		centroidVect.z = centroidVect.z / (reqChildren.Count * 1.0f);


		//Bounds reqBounds = CommonUnityUtils.findMaxBounds(reqChildren);

		Vector3 rotCentre = centroidVect + new Vector3(0,0,-2f);//reqBounds.center + new Vector3(boardCellPrefab.renderer.bounds.size.x * 0.13f,0,-2);
		GameObject rotMasterObj = new GameObject("RotationObj");
		rotMasterObj.transform.position = rotCentre;

		Transform nwOutline = (Transform) Instantiate(outlinePrefab,rotCentre,Quaternion.identity);
		nwOutline.name = "Outline";
		nwOutline.renderer.sortingOrder = 500;
		nwOutline.parent = rotMasterObj.transform;

		activatedGravelObj = ((Transform) Instantiate(gravelPrefab,rotCentre,Quaternion.identity)).gameObject;
		activatedGravelObj.name = "Gravel";
		activatedGravelObj.renderer.sortingOrder = -1;

		if(para_rotOption == 2)
		{
			Vector3 eulerAngleVect = nwOutline.localEulerAngles;
			eulerAngleVect.z += 60f;
			nwOutline.localEulerAngles = eulerAngleVect;

			activatedGravelObj.transform.localEulerAngles = eulerAngleVect;
		}


		for(int i=0; i<reqChildren.Count; i++)
		{
			Vector3 tmpMasterPos = reqChildren[i].transform.position;
			tmpMasterPos.z += -2f;
			reqChildren[i].transform.position = tmpMasterPos;

			reqChildren[i].transform.parent = rotMasterObj.transform;
		}

		if(attachPlayerAvatar)
		{
			GameObject pAv = GameObject.Find("PlayerAvatar");
			Vector3 tmpPos = pAv.transform.position;
			tmpPos.z += -2f;
			pAv.transform.position = tmpPos;
			pAv.transform.parent = rotMasterObj.transform;
		}
		

		
		triggerSoundAtCamera("GravelGrind"+Random.Range(1,2),0.5f);
		
		CustomAnimationManager aniMang = rotMasterObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> cmdBatches = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("StabilisedSpin",1,new List<System.Object>() { 0.5f, 0.333333f, new string[] {"WordBox","PlayerAvatar"} }));
		cmdBatches.Add(batch1);
		aniMang.registerListener("AcScen",this);
		aniMang.init("RotationEffect",cmdBatches);
	}
	

	protected override void initWorld()
	{


		// Init Input Detectors.
		inputDetectScripts = new Dictionary<string, AbsInputDetector>();
		GameObject globObj = GameObject.Find("GlobObj");
		ClickDetector clickDetect = globObj.AddComponent<ClickDetector>();
		clickDetect.registerListener("AcScen",this);
		inputDetectScripts.Add("ClickDetector",clickDetect);



		// Auto Adjust.
		GameObject tmpPersonPortrait = GameObject.Find("PersonPortrait");
		GameObject tmpPauseButton = GameObject.Find("PauseButton");
		GameObject tmpInstructionTile = GameObject.Find("InstructionTile");
		GameObject tmpInvisiPlane = GameObject.Find("InvisiPlane");
		SpawnNormaliser.adjustGameObjectsToNwBounds(SpawnNormaliser.get2DBounds(tmpInvisiPlane.renderer.bounds),
		                                            WorldSpawnHelper.getCameraViewWorldBounds(tmpInvisiPlane.transform.position.z,true),
		                                            new List<GameObject>() { tmpPersonPortrait, tmpPauseButton, tmpInstructionTile });
		
		Destroy(tmpInvisiPlane);
		tmpPersonPortrait.transform.parent = GameObject.Find("Main Camera").transform;
		tmpPauseButton.transform.parent = GameObject.Find("Main Camera").transform;
		tmpInstructionTile.transform.parent = GameObject.Find("Main Camera").transform;
		
		uiBounds.Add("PersonPortrait",WorldSpawnHelper.getWorldToGUIBounds(tmpPersonPortrait.renderer.bounds,upAxisArr));
		uiBounds.Add("PauseBtn",WorldSpawnHelper.getWorldToGUIBounds(tmpPauseButton.renderer.bounds,upAxisArr));		 
		uiBounds.Add("InstructionTile",WorldSpawnHelper.getWorldToGUIBounds(tmpInstructionTile.renderer.bounds,upAxisArr));





		// Init Mechanics Attributes.
		firstLevel = true;
		worldBoundsLookup = new Dictionary<string, Rect>();
		playerInputOn = true;

		currPlayerCell = new int[2] { -1,-1 };
		currDestCell = new int[2] { -1,-1 };

		cellsSelectedForRotation = new string[3] { null,null,null };

		//numClicks = 0;


		// Ambient Sounds.
		triggerAmbienceSound("TownSquareAmbience",0.1f);

	}

	protected new bool initLevelConfigGenerator()
	{
		if( ! base.initLevelConfigGenerator())
		{
			// Fallback.
			lvlConfigGen = new MPLevelConfigGeneratorServer(null); //new  MPLevelConfigGeneratorRandom();
			Debug.LogWarning("Warning: Using Level Gen Fallback");
		}
		return true;
	}

	protected override void genNextLevel()
	{

		currLvlConfig = (MPLevelConfig) lvlConfigGen.getNextLevelConfig(null);
		if(serverCommunication != null) { serverCommunication.wordDisplayed(currLvlConfig.pattern,currLvlConfig.languageArea,currLvlConfig.difficulty); }

		// Indicates if tts should be used.
		descriptionType = (currLvlConfig.useTTS ? 1 : 0);


		// Initialise board objects. (Placed here instead of worldview because board size is within the level config.)
		int[] startGameCellCoords = new int[2] { -1,-1 };
		if(firstLevel)
		{
			initialiseEmptyBoardObjects(currLvlConfig.boardSize);
			boardSize = new int[2] { currLvlConfig.boardSize[0], currLvlConfig.boardSize[1] };

			startGameCellCoords = new int[2] { 0, (int) (boardSize[1]/2f) };
			currPlayerCell[0] = startGameCellCoords[0];
			currPlayerCell[1] = startGameCellCoords[1];
		}
		initBlankNavGraph();


		// Decide selected filler and good tiles.

		int boardWidth_inColumns = currLvlConfig.boardSize[0];
		int boardHeight_inRows = currLvlConfig.boardSize[1];

		
		boardContent = new string[boardWidth_inColumns,boardHeight_inRows];
		
		for(int r=0; r<boardContent.GetLength(1); r++)
		{
			for(int c=0; c<boardContent.GetLength(0); c++)
			{
				if((Random.Range(0.0f,1.0f)>0.4f)&(currLvlConfig.fillerItems.Length>0)){

					boardContent[c,r] = currLvlConfig.fillerItems[Random.Range(0,currLvlConfig.fillerItems.Length)];
				}else{
					boardContent[c,r] = currLvlConfig.correctItems[Random.Range(0,currLvlConfig.correctItems.Length)];
				}

			}
		}

		selectDestinationCell();

		boardContent[currPlayerCell[0],currPlayerCell[1]] = currLvlConfig.correctItems[Random.Range(0,currLvlConfig.correctItems.Length)];
		boardContent[currDestCell[0],currDestCell[1]] = currLvlConfig.correctItems[Random.Range(0,currLvlConfig.correctItems.Length)];


		int sourceNodeID = cellNameToNodeIDMap["Cell("+currPlayerCell[0]+","+currPlayerCell[1]+")"];
		int destNodeID = cellNameToNodeIDMap["Cell("+currDestCell[0]+","+currDestCell[1]+")"];



		GameObject hexBoardObj = GameObject.Find("HexBoard");
		List<int> allNodeKeys = boardNavGraph.getAllNodeKeys();
		for(int i=0; i<allNodeKeys.Count; i++)
		{
			string tmpCellName = nodeIDToCellNameMap[allNodeKeys[i]];
			Transform tmpChild = hexBoardObj.transform.FindChild(tmpCellName);
			if(tmpChild != null)
			{
				SpriteRenderer sRend = tmpChild.GetComponent<SpriteRenderer>();
				sRend.color = Color.white;
			}
		}



		Transform srcCell = hexBoardObj.transform.FindChild(nodeIDToCellNameMap[sourceNodeID]);
		(srcCell.GetComponent<SpriteRenderer>()).color = Color.white;//Color.blue;
		Transform destCell = hexBoardObj.transform.FindChild(nodeIDToCellNameMap[destNodeID]);
		(destCell.GetComponent<SpriteRenderer>()).color = Color.gray;// Color.red;


		// Add the words to the world as gameobjects.

		GameObject boardGObj = GameObject.Find("HexBoard");
		List<GameObject> tmpWordBoxList = new List<GameObject>();
		int  wordBoxID = 0;
		for(int r=0; r<boardContent.GetLength(1); r++)
		{
			for(int c=0; c<boardContent.GetLength(0); c++)
			{
				Transform boardTile = (boardGObj.transform.FindChild("Cell("+c+","+r+")"));
				if(boardTile != null)
				{
					if(boardTile.FindChild("WordBox") == null)
					{
						Transform wbplane = boardTile.transform.FindChild("WordBoundsPlane");
						GameObject nwWordBox =	WordBuilderHelper.buildWordBox(wordBoxID,
												                               boardContent[c,r],
												                               new Rect(wbplane.renderer.bounds.min.x,wbplane.renderer.bounds.max.y,wbplane.renderer.bounds.size.x,wbplane.renderer.bounds.size.y),
												                               wbplane.position.z,
												                               upAxisArr,
												                               wordBoxPrefab);
						nwWordBox.name = "WordBox";
						Transform boardChild = nwWordBox.transform.FindChild("Board");
						boardChild.renderer.enabled = false;
						TextNBoardScript tnbScript = nwWordBox.GetComponent<TextNBoardScript>();
						tnbScript.destroyAfterInit = true;

						boardChild.renderer.sortingOrder = boardTile.renderer.sortingOrder + 1;
						nwWordBox.transform.FindChild("Text").renderer.sortingOrder = boardTile.renderer.sortingOrder + 2;

						nwWordBox.transform.parent = boardTile;

						tmpWordBoxList.Add(nwWordBox);
					}
					else
					{
						Transform existingWBox = boardTile.FindChild("WordBox");
						Transform textChild = existingWBox.FindChild("Text");
						TextMesh childTMesh = textChild.GetComponent<TextMesh>();
						childTMesh.text = boardContent[c,r];
						tmpWordBoxList.Add(existingWBox.gameObject);
					}
				}
				else
				{
					Debug.Log("Could not find: Cell("+c+","+r+")");
				}
			}
		}

		WordBuilderHelper.setBoxesToUniformTextSize(tmpWordBoxList,0.035f);




		// Init instruction tile.
		setupInstructionTile();		




		// Initialise Player (i.e. spawn player).

		if(firstLevel)
		{
			GameObject hexBoardGObj = GameObject.Find("HexBoard");
			Transform startCellTrans = hexBoardGObj.transform.FindChild("Cell("+startGameCellCoords[0]+","+startGameCellCoords[1]+")");
			Vector3 playerSpawnPt = new Vector3(startCellTrans.position.x,startCellTrans.position.y,startCellTrans.position.z);

			Transform nwPlayer = (Transform) Instantiate(playerPrefab,playerSpawnPt,Quaternion.identity);
			nwPlayer.name = "PlayerAvatar";


		}


		hideIrrelevantCornerButtons();

		if( ! firstLevel)
		{
			startPreWaitProcedure(chosenGoalSide,chosenBystanderGoalPt);
		}


		firstLevel = false;
	}

	// Should be called by genNextLevel to start the bystander wait sequence.
	public void startPreWaitProcedure(string para_sideID, Vector3 para_ptByEdge)
	{
		// Tell the bystander manager to select a available bystander for this level.
		// Tell it to select but pause the bystander.
		// In the mean time, pan the camera to the location.
		setPlayerInputState(false);
		BystanderMangScript bms = GameObject.Find("GlobObj").GetComponent<BystanderMangScript>();
		bms.selectWaitBystander(para_sideID, para_ptByEdge);


		// Identify final cam bounds for pan.
		Rect fullEncaps = worldBoundsLookup["FullEncapsulation"];
		Rect cameraWorldBounds = WorldSpawnHelper.getCameraViewWorldBounds(2,true);
		Rect nwLookAtBounds = CommonUnityUtils.findLookatBoundsWithinLimitedArea(para_ptByEdge,cameraWorldBounds,fullEncaps);

		Vector3 camDestPos = new Vector3(nwLookAtBounds.x + (nwLookAtBounds.width/2f),
		                                 nwLookAtBounds.y - (nwLookAtBounds.height/2f),
		                                 Camera.main.transform.position.z);


		CustomAnimationManager aniMang = Camera.main.transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3]{camDestPos.x,camDestPos.y,camDestPos.z}, 2f, true }));
		batchLists.Add(batch1);
		aniMang.registerListener("AcScen",this);
		aniMang.init("CamPanToWaitBystander",batchLists);
	}

	public void startWaitProcedure()
	{
		// Once the camera is panned to the bystander location,
		// Trigger the bystander to begin the WaitAndCall procedure.
		BystanderMangScript bms = GameObject.Find("GlobObj").GetComponent<BystanderMangScript>();
		bms.runWaitBystander();
	}

	// Called by WaitAndCallStateScript.
	public void endWaitProcedure()
	{
		// Signal that the WaitAndCall procedure is done.

		if(descriptionType == 1)
		{
			try
			{
				if(gameObject.GetComponent<AudioSource>() == null) { gameObject.AddComponent<AudioSource>(); }
				audio.PlayOneShot(WorldViewServerCommunication.tts.say(currLvlConfig.pattern));
			}
			catch(System.Exception ex)
			{
				Debug.LogError("Failed to use TTS. "+ex.Message);
			}

			descriptionTileButtonOn = true;
		}

		// Trigger auto rotations.
		//triggerRandomAutoRotation();
		allowPlayerToStartLevel();
	}

	public void allowPlayerToStartLevel()
	{
		// The player can start moving towards the selected bystander.
		
		// Teleport camera to player avatar.
		GameObject playerAv = GameObject.Find("PlayerAvatar");
		Vector3 pointOfInterest =  new Vector3(playerAv.transform.position.x,playerAv.transform.position.y,Camera.main.transform.position.z);
		Rect camBounds = WorldSpawnHelper.getCameraViewWorldBounds(2,true);
		
		Rect nwBounds = CommonUnityUtils.findLookatBoundsWithinLimitedArea(pointOfInterest,camBounds,worldBoundsLookup["FullEncapsulation"]);
		Vector3 reqCamPos = new Vector3(nwBounds.x + (nwBounds.width/2f), nwBounds.y - (nwBounds.height/2f),Camera.main.transform.position.z);
		Camera.main.transform.position = reqCamPos;
		
		// Set arrow here.
		
		// Unfreeze player controls.
		setPlayerInputState(true);
	}

	public void sendBystanderReleaseMsg()
	{
		BystanderMangScript bms = GameObject.Find("GlobObj").GetComponent<BystanderMangScript>();
		bms.releaseWaitingBystander();
	}

	public void initialiseEmptyBoardObjects(int[] para_boardSize)
	{
		
		// Generate empty tiled board.
		GameObject gridBoardGObj = new GameObject("HexBoard");
		GameObject cornerButtonCollectionGObj = new GameObject("CornerButtonCollection");

		int boardWidth_inColumns = para_boardSize[0];
		int boardHeight_inRows = para_boardSize[1];
		float worldspace_cellWidth = boardCellPrefab.renderer.bounds.size.x;
		float worldspace_cellHeight = boardCellPrefab.renderer.bounds.size.y;

		float totalBoard_Width  = ((worldspace_cellWidth * (boardWidth_inColumns * 1.0f)) * 0.75f) + (worldspace_cellWidth * 0.25f);
		float totalBoard_Height = (worldspace_cellHeight * (boardHeight_inRows * 1.0f));

		Vector2 boardWorldCentre = new Vector2(0,0);
		Rect boardWorldBounds = new Rect(boardWorldCentre.x - (totalBoard_Width/2f), boardWorldCentre.y + (totalBoard_Height/2f),totalBoard_Width,totalBoard_Height);
		worldBoundsLookup.Add("Board",boardWorldBounds);



		Vector3 cellPos = new Vector3(boardWorldBounds.x + (worldspace_cellWidth/2f),boardWorldBounds.y - (worldspace_cellHeight/2f),0);
		for(int c=0; c<boardWidth_inColumns; c++)
		{
			cellPos.y = boardWorldBounds.y - (worldspace_cellHeight/2f);
			
			int numOfRowsForColumn = boardHeight_inRows;

			bool isOddColumn = ((c%2) != 0);
			if((c%2) != 0)
			{
				numOfRowsForColumn = boardHeight_inRows-1;	
				cellPos.y = boardWorldBounds.y - (worldspace_cellHeight);
			}
			

			for(int r=0; r<numOfRowsForColumn; r++)
			{
				// Create board piece.
				Transform nwCell = (Transform) Instantiate(boardCellPrefab,cellPos,Quaternion.identity);
				nwCell.name = "Cell("+c+","+r+")";
				nwCell.parent = gridBoardGObj.transform;

				Transform cornerButtonWest = nwCell.FindChild("CornerButton-W");
				Transform cornerButtonSouthWest = nwCell.FindChild("CornerButton-SW");
				cornerButtonWest.parent = cornerButtonCollectionGObj.transform.parent;
				cornerButtonSouthWest.parent = cornerButtonCollectionGObj.transform.parent;

				if((c == 0)&&(r == 0))
				{
					Destroy(cornerButtonWest.gameObject);
					Destroy(cornerButtonSouthWest.gameObject);
				}
				else if((r == (numOfRowsForColumn-1))&&(isOddColumn))
				{
					Destroy(cornerButtonSouthWest.gameObject);
				}
				else if(((c == 0)||(r == (numOfRowsForColumn-1)))
				   	 && (! ((r == 0)&&(!isOddColumn))))				
				{
					Destroy(cornerButtonWest.gameObject);
					Destroy(cornerButtonSouthWest.gameObject);
				}
				else if((r == 0)&&(!isOddColumn))
				{
					Destroy(cornerButtonWest.gameObject);
				}


				cellPos.y -= worldspace_cellHeight;
			}
			
			cellPos.x += (worldspace_cellWidth * 0.75f);
		}





		// Generate border walkways padding.

		float walkwaySize = 1f;
		//GameObject topWalkway = new GameObject("TopWalkway");
		Rect topWalkwayWorldBounds = new Rect(boardWorldBounds.x,boardWorldBounds.y + walkwaySize,boardWorldBounds.width,walkwaySize);
		Rect leftWalkwayWorldBounds = new Rect(boardWorldBounds.x - walkwaySize,boardWorldBounds.y,walkwaySize,boardWorldBounds.height);
		Rect rightWalkwayWorldBounds = new Rect(boardWorldBounds.x + boardWorldBounds.width,boardWorldBounds.y,walkwaySize,boardWorldBounds.height);


		worldBoundsLookup.Add("TopWalkway",topWalkwayWorldBounds);
		worldBoundsLookup.Add("LeftWalkway",leftWalkwayWorldBounds);
		worldBoundsLookup.Add("RightWalkway",rightWalkwayWorldBounds);



		// Calculate total rect area taken up by the hex board and the walkways.
		Rect areaUsedByBoardAndWalkways = new Rect(leftWalkwayWorldBounds.x,
		                                           topWalkwayWorldBounds.y,
		                                           (rightWalkwayWorldBounds.x + rightWalkwayWorldBounds.width) - leftWalkwayWorldBounds.x,
		                                           topWalkwayWorldBounds.y - (boardWorldBounds.y - (boardWorldBounds.height)));


		// Generate wall borders.
		GameObject bastionMainParentGObj = new GameObject("Bastions");

		// Generate Top Border Items.
		GameObject topBastions = new GameObject("TopWalls");

		float divRes_numTopBorderSegments = (areaUsedByBoardAndWalkways.width / borderItemTopPrefab.renderer.bounds.size.x);
		float modRes_numTopBorderSegments = (areaUsedByBoardAndWalkways.width % borderItemTopPrefab.renderer.bounds.size.y);
		int numTopBorderSegments = (int) divRes_numTopBorderSegments;
		if(modRes_numTopBorderSegments != 0) { numTopBorderSegments++; }

		float totalLengthOfTopBorder = (numTopBorderSegments * borderItemTopPrefab.renderer.bounds.size.x)
										+ (borderItemTLCornerPrefab.renderer.bounds.size.x)
										+ (borderItemTRCornerPrefab.renderer.bounds.size.x)
										- (numTopBorderSegments * 0.05f); // prevent cracks.

		Rect totalTopBorderBounds = new Rect(areaUsedByBoardAndWalkways.x + (areaUsedByBoardAndWalkways.width/2f) - (totalLengthOfTopBorder/2f),
		                                     					  areaUsedByBoardAndWalkways.y + borderItemTLCornerPrefab.renderer.bounds.size.y,
		                                     					  totalLengthOfTopBorder,
		                                     					  borderItemTLCornerPrefab.renderer.bounds.size.y);

		Vector3 nxtItemTopLeft = new Vector3(totalTopBorderBounds.x,totalTopBorderBounds.y,0);
		for(int i=0; i<numTopBorderSegments+2; i++)
		{
			Transform reqPrefab = borderItemTopPrefab;

			if(i == 0)
			{
				// Apply TL corner piece.
				reqPrefab = borderItemTLCornerPrefab;
			}
			else if(i == ((numTopBorderSegments+2)-1))
			{
				// Apply TR corner piece.
				reqPrefab = borderItemTRCornerPrefab;
			}
			else
			{
				// Apply regular top piece.
				reqPrefab = borderItemTopPrefab;
			}

			Vector3 itemSpawnPos = new Vector3(nxtItemTopLeft.x + (reqPrefab.renderer.bounds.size.x/2f),nxtItemTopLeft.y - (reqPrefab.renderer.bounds.size.y/2f),nxtItemTopLeft.z);
			Transform nwTopBorderItem = (Transform) Instantiate(reqPrefab,itemSpawnPos,Quaternion.identity);
			nwTopBorderItem.parent = topBastions.transform;

			nxtItemTopLeft.x += (reqPrefab.renderer.bounds.size.x - 0.05f); // -0.05f to prevent cracks due to floating point accuracy.
		}


		// Generate Left Border Items.
		GameObject leftBastions = new GameObject("LeftWalls");

		float divRes_numLeftBorderSegments = (areaUsedByBoardAndWalkways.height / borderItemLeftPrefab.renderer.bounds.size.y);
		float modRes_numLeftBorderSegments = (areaUsedByBoardAndWalkways.height % borderItemLeftPrefab.renderer.bounds.size.y);
		int numLeftBorderSegments = (int) divRes_numLeftBorderSegments;
		if(modRes_numLeftBorderSegments != 0) { numLeftBorderSegments++; }

		float totalLengthOfLeftBorder = (numLeftBorderSegments * borderItemLeftPrefab.renderer.bounds.size.y) + (borderItemBLCornerPrefab.renderer.bounds.size.y) - (numLeftBorderSegments * 0.05f);

		Rect totalLeftBorderBounds = new Rect(totalTopBorderBounds.x,totalTopBorderBounds.y - totalTopBorderBounds.height,borderItemTLCornerPrefab.renderer.bounds.size.x,totalLengthOfLeftBorder);

		nxtItemTopLeft = new Vector3(totalLeftBorderBounds.x,totalLeftBorderBounds.y + 0.05f,0);
		for(int i=0; i<numLeftBorderSegments+1; i++)
		{
			Transform reqPrefab = borderItemLeftPrefab;

			if(i == ((numLeftBorderSegments+1)-1))
			{
				// Apply BL corner piece.
				reqPrefab = borderItemBLCornerPrefab;
			}
			else
			{
				// Apply regular left piece.
				reqPrefab = borderItemLeftPrefab;
			}

			Vector3 itemSpawnPos = new Vector3(nxtItemTopLeft.x + (reqPrefab.renderer.bounds.size.x/2f),nxtItemTopLeft.y - (reqPrefab.renderer.bounds.size.y/2f),nxtItemTopLeft.z);
			Transform nwLeftBorderItem = (Transform) Instantiate(reqPrefab,itemSpawnPos,Quaternion.identity);
			nwLeftBorderItem.parent = leftBastions.transform;

			nxtItemTopLeft.y -= (reqPrefab.renderer.bounds.size.y - 0.05f);
		}


		// Generate Right Border Items.
		GameObject rightBastions = new GameObject("RightWalls");

		int numRightBorderSegments = numLeftBorderSegments;

		float totalLengthOfRightBorder = (numRightBorderSegments * borderItemRightPrefab.renderer.bounds.size.y) + (borderItemBRCornerPrefab.renderer.bounds.size.y) - (numRightBorderSegments * 0.05f);

		Rect totalRightBorderBounds = new Rect(totalTopBorderBounds.x + totalTopBorderBounds.width - (borderItemTRCornerPrefab.renderer.bounds.size.x),totalTopBorderBounds.y - totalTopBorderBounds.height,borderItemTRCornerPrefab.renderer.bounds.size.x,totalLengthOfRightBorder);

		Vector3 nxtItemTopRight = new Vector3(totalRightBorderBounds.x + borderItemTRCornerPrefab.renderer.bounds.size.x - 0.05f,totalRightBorderBounds.y + 0.05f,0);
		for(int i=0; i<numRightBorderSegments+1; i++)
		{
			Transform reqPrefab = borderItemRightPrefab;

			if(i == ((numRightBorderSegments+1)-1))
			{
				// Apply BR corner piece.
				reqPrefab = borderItemBRCornerPrefab;
			}
			else
			{
				// Apply regular right piece.
				reqPrefab = borderItemRightPrefab;
			}

			Vector3 itemSpawnPos = new Vector3(nxtItemTopRight.x - (reqPrefab.renderer.bounds.size.x/2f),nxtItemTopRight.y - (reqPrefab.renderer.bounds.size.y/2f),nxtItemTopRight.z);
			Transform nwRightBorderItem = (Transform) Instantiate(reqPrefab,itemSpawnPos,Quaternion.identity);
			nwRightBorderItem.parent = rightBastions.transform;
			
			nxtItemTopRight.y -= (reqPrefab.renderer.bounds.size.y - 0.05f);
		}



		topBastions.transform.parent = bastionMainParentGObj.transform;
		leftBastions.transform.parent = bastionMainParentGObj.transform;
		rightBastions.transform.parent = bastionMainParentGObj.transform;


	
		// Adjust walkways due to walls.

		topWalkwayWorldBounds = new Rect(topWalkwayWorldBounds.x,
		                                 totalTopBorderBounds.y - borderItemTopPrefab.renderer.bounds.size.y,
		                                 topWalkwayWorldBounds.width,
		                                 (totalTopBorderBounds.y - borderItemTopPrefab.renderer.bounds.size.y) - boardWorldBounds.y);

		leftWalkwayWorldBounds = new Rect(totalLeftBorderBounds.x + borderItemLeftPrefab.renderer.bounds.size.x,
		                                  leftWalkwayWorldBounds.y,
		                                  boardWorldBounds.x - (totalLeftBorderBounds.x + borderItemLeftPrefab.renderer.bounds.size.x),
		                                  leftWalkwayWorldBounds.height);

		rightWalkwayWorldBounds = new Rect(rightWalkwayWorldBounds.x,
		                                   rightWalkwayWorldBounds.y,
		                                   (totalRightBorderBounds.x + totalRightBorderBounds.width - borderItemRightPrefab.renderer.bounds.size.x) - rightWalkwayWorldBounds.x,
		                                   rightWalkwayWorldBounds.height);

		Rect bottomWalkwayWorldBounds = new Rect(topWalkwayWorldBounds.x,boardWorldBounds.y - boardWorldBounds.height,topWalkwayWorldBounds.width,topWalkwayWorldBounds.height);

		Rect bottomEntranceBounds = new Rect(bottomWalkwayWorldBounds.x,
		                                     bottomWalkwayWorldBounds.y - bottomWalkwayWorldBounds.height,
		                                     bottomWalkwayWorldBounds.width,
		                                     (bottomWalkwayWorldBounds.y - bottomWalkwayWorldBounds.height) - (totalLeftBorderBounds.y - (totalLeftBorderBounds.height + 1)));

		GameObject db1 = WorldSpawnHelper.initObjWithinWorldBounds(debugPrefab,"TopWalk",topWalkwayWorldBounds,0,upAxisArr);
		GameObject db2 = WorldSpawnHelper.initObjWithinWorldBounds(debugPrefab,"LeftWalk",leftWalkwayWorldBounds,0,upAxisArr);
		GameObject db3 = WorldSpawnHelper.initObjWithinWorldBounds(debugPrefab,"RightWalk",rightWalkwayWorldBounds,0,upAxisArr);
		GameObject db4 = WorldSpawnHelper.initObjWithinWorldBounds(debugPrefab,"BottomWalk",bottomWalkwayWorldBounds,0,upAxisArr);
		GameObject db5 = WorldSpawnHelper.initObjWithinWorldBounds(debugPrefab,"BottomEntrance",bottomEntranceBounds,0,upAxisArr);
		db1.renderer.enabled = false;
		db2.renderer.enabled = false;
		db3.renderer.enabled = false;
		db4.renderer.enabled = false;
		db5.renderer.enabled = false;
		
		worldBoundsLookup["TopWalkway"] = topWalkwayWorldBounds;
		worldBoundsLookup["LeftWalkway"] = leftWalkwayWorldBounds;
		worldBoundsLookup["RightWalkway"] = rightWalkwayWorldBounds;
		worldBoundsLookup.Add("BottomWalkway",bottomWalkwayWorldBounds);
		worldBoundsLookup.Add("BottomEntrance",bottomEntranceBounds);		                  





		// Generate floor background.
		GameObject floorBkgroundGObj = new GameObject("FoorBkgrnd");

		//Rect fullEncapsulatingBounds = CommonUnityUtils.findMaxBounds(new List<Rect>() { boardWorldBounds, topHouseBorderWorldBounds, leftHouseBorderWorldBounds, rightHouseBorderWorldBounds });
		Rect fullEncapsulatingBounds = new Rect(totalLeftBorderBounds.x,
		                                        totalTopBorderBounds.y,
		                                        (totalRightBorderBounds.x + totalRightBorderBounds.width) - (totalLeftBorderBounds.x),
		                                        (totalTopBorderBounds.y) - (totalLeftBorderBounds.y - totalLeftBorderBounds.height));
		worldBoundsLookup.Add("FullEncapsulation",fullEncapsulatingBounds);

		int[] backgroundTilingGridSize = new int[] { 1,1 };

		float divAnswer = (fullEncapsulatingBounds.width/backgroundTileablePrefab.renderer.bounds.size.x);
		float modAnswer = (fullEncapsulatingBounds.width%backgroundTileablePrefab.renderer.bounds.size.x);

		backgroundTilingGridSize[0] = ((int) divAnswer);
		if(modAnswer > 0) {	backgroundTilingGridSize[0]++; }

		divAnswer = (fullEncapsulatingBounds.height/backgroundTileablePrefab.renderer.bounds.size.y);
		modAnswer = (fullEncapsulatingBounds.height%backgroundTileablePrefab.renderer.bounds.size.y);

		backgroundTilingGridSize[1] = ((int) divAnswer);
		if(modAnswer > 0) { backgroundTilingGridSize[1]++; }


		for(int r=0; r<backgroundTilingGridSize[1]; r++)
		{
			for(int c=0; c<backgroundTilingGridSize[0]; c++)
			{
				Vector3 bkgrndTileSpawnPos = new Vector3(fullEncapsulatingBounds.x + (backgroundTileablePrefab.renderer.bounds.size.x/2f) + (c * (backgroundTileablePrefab.renderer.bounds.size.x)),
				                                         fullEncapsulatingBounds.y - (backgroundTileablePrefab.renderer.bounds.size.y/2f) - (r * (backgroundTileablePrefab.renderer.bounds.size.y)),
				                                         0);

				Transform nwBkgrndTile = (Transform) Instantiate(backgroundTileablePrefab,bkgrndTileSpawnPos,Quaternion.identity);
				nwBkgrndTile.parent = floorBkgroundGObj.transform;
			}
		}




		Destroy(Camera.main.gameObject.GetComponent<DragNScroll>());
		DragNScroll dragNScrollScript = Camera.main.gameObject.AddComponent<DragNScroll>();
		Vector3 reqWorld_TL = new Vector3(fullEncapsulatingBounds.x,fullEncapsulatingBounds.y,0);
		Vector3 reqWorld_BR = new Vector3(fullEncapsulatingBounds.x + fullEncapsulatingBounds.width,fullEncapsulatingBounds.y - fullEncapsulatingBounds.height,0);
		dragNScrollScript.setWorldExtents(reqWorld_TL,reqWorld_BR,3);
		dragNScrollScript.flipDir = true;


		//WorldSpawnHelper.initObjWithinWorldBounds(debugPrefab,"Board",boardWorldBounds,0,upAxisArr);
		//WorldSpawnHelper.initObjWithinWorldBounds(debugPrefab,"TopHouses",topHouseBorderWorldBounds,0,upAxisArr);
		//WorldSpawnHelper.initObjWithinWorldBounds(debugPrefab,"LeftHouses",leftHouseBorderWorldBounds,0,upAxisArr);
		//WorldSpawnHelper.initObjWithinWorldBounds(debugPrefab,"RightHouses",rightHouseBorderWorldBounds,0,upAxisArr);
		//WorldSpawnHelper.initObjWithinWorldBounds(debugPrefab,"FullEncapsulation",fullEncapsulatingBounds,0,upAxisArr);



	}

	private void initBlankNavGraph()
	{
		if(firstLevel)
		{
			// Create the datastructure.

			boardNavGraph = new BasicNavGraph();
			cellNameToNodeIDMap = new Dictionary<string,int>();
			nodeIDToCellNameMap = new Dictionary<int,string>();


			int boardWidth_inColumns = boardSize[0];
			int boardHeight_inRows = boardSize[1];


			int cellID = 0;
			GameObject hexBoardObj = GameObject.Find("HexBoard");
			for(int c=0; c<boardWidth_inColumns; c++)
			{
			
				int numOfRowsForColumn = boardHeight_inRows;

				bool isOddColumn = false;
				if((c%2) != 0)
				{
					numOfRowsForColumn = boardHeight_inRows-1;	
					isOddColumn = true;
				}
				
				
				for(int r=0; r<numOfRowsForColumn; r++)
				{
					Transform reqCell = hexBoardObj.transform.FindChild("Cell("+c+","+r+")");
					if(reqCell != null)
					{

						WorldNode nwNavNode = new WorldNode(cellID,1,reqCell.position);
						boardNavGraph.addNode(nwNavNode);
						cellNameToNodeIDMap.Add(reqCell.name,cellID);
						nodeIDToCellNameMap.Add(cellID,reqCell.name);


						// Get potential neighbours.
						List<int[]> potentialNeighbourCoords = new List<int[]>();
						potentialNeighbourCoords.Add(new int[2] {c,r-1});
						potentialNeighbourCoords.Add(new int[2] {c,r+1});

						if(! isOddColumn)
						{
							potentialNeighbourCoords.Add(new int[2] {c-1,r-1});
							potentialNeighbourCoords.Add(new int[2] {c-1,r});

							//potentialNeighbourCoords.Add(new int[2] {c+1,r-1});
							//potentialNeighbourCoords.Add(new int[2] {c+1,r});
						}
						else
						{
							potentialNeighbourCoords.Add(new int[2] {c-1,r});
							potentialNeighbourCoords.Add(new int[2] {c-1,r+1});

							//potentialNeighbourCoords.Add(new int[2] {c+1,r});
							//potentialNeighbourCoords.Add(new int[2] {c+1,r+1});
						}


						for(int i=0; i<potentialNeighbourCoords.Count; i++)
						{
							int[] tmpCoords = potentialNeighbourCoords[i];
							string potentialNeighbourName = "Cell("+tmpCoords[0]+","+tmpCoords[1]+")";

							Transform reqNeighbourCell = hexBoardObj.transform.FindChild(potentialNeighbourName);
							if(reqNeighbourCell != null)
							{
								if(cellNameToNodeIDMap.ContainsKey(potentialNeighbourName))
								{
									int neighbourNodeID = cellNameToNodeIDMap[potentialNeighbourName];
									boardNavGraph.addEdge(cellID,neighbourNodeID,new NavEdge(new int[2] {cellID,neighbourNodeID},1));
								}
							}
						}


						cellID++;
					}
				}
			}


			GameObject navRendering = NavGraphUnityUtils.renderNavGraph("MPBoardNavGraphRender",boardNavGraph,debugPrefab);
			navRendering.SetActive(false);

		}
		else
		{
			// If not first level. Simply reset all node types to type 1: i.e. reachable type.

			List<int> nodeKeys = boardNavGraph.getAllNodeKeys();
			for(int i=0; i<nodeKeys.Count; i++)
			{
				boardNavGraph.setNodeType(nodeKeys[i],1);
			}
		}
	}


	private void selectDestinationCell()
	{
		// There are 3 sides of the board.
		// Select destination which is on a side which is not the side you are currently on.

		List<int> availableSides = new List<int>() { 0,1,2 }; // 0=left, 1=top, 2=right.

		int tmpCurrSide = -1;
		if(currPlayerCell[0] == 0)
		{
			tmpCurrSide = 0;
			availableSides.Remove(0);
		}

		if(currPlayerCell[1] == 0)
		{
			tmpCurrSide = 1;
			availableSides.Remove(1);
		}

		if(currPlayerCell[0] == (boardSize[0]-1))
		{
			tmpCurrSide = 2;
			availableSides.Remove(2);
		}


		int randIndex = Random.Range(0,availableSides.Count);
		int chosenSide = availableSides[randIndex];


		if(chosenSide == 0)
		{
			chosenGoalSide = "L";
			int minRangeY = 0;
			int maxRangeY = boardSize[1];
			if(tmpCurrSide == 1)
			{
				if(currPlayerCell[0] <= ((int) (boardSize[0]/2f)))
				{
					minRangeY = (int) (boardSize[1] * 0.7f);
				}
			}
			int randY = Random.Range(minRangeY,maxRangeY);

			currDestCell[0] = 0;
			currDestCell[1] = randY;
		}
		else if(chosenSide == 1)
		{
			chosenGoalSide = "T";
			int minRangeX = 0;
			int maxRangeX = boardSize[0];
			if(tmpCurrSide == 0)
			{
				if(currPlayerCell[1] <= ((int) (boardSize[1]/2f)))
				{
					minRangeX = (int) (boardSize[0] * 0.7f);
				}
			}
			else if(tmpCurrSide == 2)
			{
				if(currPlayerCell[1] <= ((int) (boardSize[1]/2f)))
				{
					maxRangeX = (int) (boardSize[0] * 0.4f);
				}
			}

			int randX = Random.Range(minRangeX,maxRangeX);

			currDestCell[0] = randX;
			currDestCell[1] = 0;
		}
		else
		{
			chosenGoalSide = "R";
			// chosenSide == 2

			int maxYVal = boardSize[1]-1;
			if(((boardSize[1]-1) %2) != 0)
			{
				maxYVal--;
				//isOddColumn = true;
			}

			int minYVal = 0;
			if(tmpCurrSide == 1)
			{
				if(currPlayerCell[0] >= ((int) (boardSize[0]/2f)))
				{
					minYVal = (int) (boardSize[1] * 0.7f);
				}
			}

			int randY = Random.Range(minYVal,maxYVal);

			currDestCell[0] = (boardSize[0]-1);
			currDestCell[1] = randY;
		}


		// Setup bystander stop point next to the chosen destination cell.
		//chosenBystanderGoalPt
		GameObject hexBoardObj = GameObject.Find("HexBoard");
		GameObject destCellGObj = hexBoardObj.transform.FindChild("Cell("+currDestCell[0]+","+currDestCell[1]+")").gameObject;
		Rect destCellBounds = CommonUnityUtils.get2DBounds(destCellGObj.renderer.bounds);
		Rect boardWorldBounds = worldBoundsLookup["Board"];
		if(chosenGoalSide == "L")
		{
			chosenBystanderGoalPt = new Vector3(boardWorldBounds.x - 0.4f, destCellBounds.y - (destCellBounds.height/2f), destCellGObj.transform.position.z);
		}
		else if(chosenGoalSide == "T")
		{
			chosenBystanderGoalPt = new Vector3(destCellBounds.x + (destCellBounds.width/2f), boardWorldBounds.y + 0.4f, destCellGObj.transform.position.z);
		}
		else
		{
			// chosenGoalSide == "R"
			chosenBystanderGoalPt = new Vector3(boardWorldBounds.x + boardWorldBounds.width + 0.4f, destCellBounds.y - (destCellBounds.height/2f), destCellGObj.transform.position.z);
		}

	}


	private List<int> applyHiddenIslandOp()
	{

		int startNodeID = cellNameToNodeIDMap["Cell("+currPlayerCell[0]+","+currPlayerCell[1]+")"];
		int destNodeID = cellNameToNodeIDMap["Cell("+currDestCell[0]+","+currDestCell[1]+")"];



		List<int> nodeKeys = boardNavGraph.getAllNodeKeys();
		int totNodes = nodeKeys.Count;

		//3 islands --> 25nodes.
		//	 ?		--> totNodes.
		int reqNumIslands = (int) ((totNodes * 2f)/25f);
		if(reqNumIslands < 1) { reqNumIslands = 1; }


		List<int> effectedNodes = new List<int>();
		for(int i=0; i<reqNumIslands; i++)
		{
			int nxtIslandCentreNode = startNodeID;

			while((nxtIslandCentreNode == startNodeID)
			||(nxtIslandCentreNode == destNodeID))
			{
				int randIndex = Random.Range(0,nodeKeys.Count);
				nxtIslandCentreNode = nodeKeys[randIndex];
			}


			List<int> nodesToFlip = new List<int>();
			nodesToFlip.Add(nxtIslandCentreNode);
			NavNode islandCentreNode = boardNavGraph.getNode(nxtIslandCentreNode);
			List<int> neighbourIDs = new List<int>(islandCentreNode.getAllNeighbourIDs());

			int islandExpansionPts = 3; // (cannot be more than 6 since we are dealing with hexacon tiles which have a maximum of 6 neighbours each).
			if(neighbourIDs.Count < islandExpansionPts)
			{
				foreach(int tmpID in neighbourIDs)
				{
					nodesToFlip.Add(tmpID);
				}
			}
			else
			{
				for(int k=0; k<islandExpansionPts; k++)
				{
					int randIndex = Random.Range(0,neighbourIDs.Count);
					nodesToFlip.Add(neighbourIDs[randIndex]);
					neighbourIDs.RemoveAt(randIndex);
				}
			}



			for(int k=0; k<nodesToFlip.Count; k++)
			{
				if((nodesToFlip[k] != startNodeID)&&(nodesToFlip[k] != destNodeID))
				{
					boardNavGraph.setNodeType(nodesToFlip[k],0); // 0=blocked. 1=good.
					effectedNodes.Add(nodesToFlip[k]);
				}
			}
		}

		return effectedNodes;
	}





	public void setPlayerInputState(bool para_state)
	{
		playerInputOn = para_state;

		DragNScroll scrCam = Camera.main.transform.GetComponent<DragNScroll>();
		if(scrCam != null) { scrCam.enabled = playerInputOn; }

		foreach(KeyValuePair<string,AbsInputDetector> pair in inputDetectScripts)
		{
			pair.Value.toggleInputStatus(para_state);
		}

		togglePauseButtonVisibility(para_state);
	}


	public new void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		base.respondToEvent(para_sourceID,para_eventID,para_eventData);



		if((isInitialised == true)&&(hasInitIntroSequence == false))
		{
			// Init intro sequence script.
			setPlayerInputState(false);

			// Bystander graph init.
			BystanderMangScript bms = GameObject.Find("GlobObj").AddComponent<BystanderMangScript>();
			bms.init(debugPrefab,bystanderBubblePrefab,worldBoundsLookup);

			triggerSoundAtCamera("FootstepsMarbleLong");

			// Trigger Intro Script.
			AcMPIntroSequenceScript introScript = transform.gameObject.AddComponent<AcMPIntroSequenceScript>();
			introScript.registerListener("AcScen",this);
			introScript.init(worldBoundsLookup["FullEncapsulation"]);
			hasInitIntroSequence = true;
		}
	
		if(para_eventID == "IntroDone")
		{
			//setPlayerInputState(true);
			startTimestamp = Time.time;
			Destroy(ambientSound);
			startPreWaitProcedure(chosenGoalSide,chosenBystanderGoalPt);
		}
		else if(para_sourceID == "InputDetector")
		{
			if(para_eventID == "ClickEvent")
			{
				if( ! playerIsEnroute)
				{
					System.Object[] eventData = (System.Object[]) para_eventData;
					float[] clickPosAsFloatArr = (float[]) eventData[0];
					//int numOfClicks = (int) eventData[1];

					//numClicks = numOfClicks;

					

					Vector2 clickPos = new Vector2(clickPosAsFloatArr[0],clickPosAsFloatArr[1]);

					bool boardCellEventFlag = checkNHandle_boardCellCentreClick(clickPos);

					if( ! boardCellEventFlag)
					{
							if(rotationReturnFlag)
							{
								checkNHandle_triCellRotationClick(clickPos);
							}
					}
				}
			}
		}
		else if(para_eventID == "PathComplete")
		{
			int[] nxtNodeCoords = extractCoordsFromCellName(playerWalkEndCell);
			currPlayerCell[0] = nxtNodeCoords[0];
			currPlayerCell[1] = nxtNodeCoords[1];

			GameObject hexBoardObj = GameObject.Find("HexBoard");
			Transform reqCell = hexBoardObj.transform.FindChild("Cell("+currPlayerCell[0]+","+currPlayerCell[1]+")");
			SpriteRenderer sRend = reqCell.GetComponent<SpriteRenderer>();
			sRend.color = Color.green;

			hideIrrelevantCornerButtons();


			if((currPlayerCell[0] == currDestCell[0])&&(currPlayerCell[1] == currDestCell[1]))
			{
				GameObject playerAv = GameObject.Find("PlayerAvatar");
				NewCharacterNavMovement cnm = playerAv.GetComponent<NewCharacterNavMovement>();
				if(chosenGoalSide == "T") { cnm.faceDirection("N"); } else if(chosenGoalSide == "L") { cnm.faceDirection("W"); } else if(chosenGoalSide == "R") { cnm.faceDirection("E");	}
				BystanderMangScript bms = GameObject.Find("GlobObj").GetComponent<BystanderMangScript>();
				GameObject waitBystander = bms.getWaitBystander();

				Transform[] player1BubbleSequence = new Transform[] { talkBubblePrefab, talkBubblePrefab };
				Transform[] bystanderBubbleSequence = new Transform[] { talkBubblePrefab, thumbsUpBubblePrefab };

				setPlayerInputState(false);
				Vector3 conversationMidPt = playerAv.transform.position + ((waitBystander.transform.position - playerAv.transform.position)/2f);
				focusCameraAtPointOfInterest(conversationMidPt);
				MPConvScript convScript = transform.gameObject.AddComponent<MPConvScript>();
				convScript.registerListener("AcScen",this);
				convScript.init(player1BubbleSequence,bystanderBubbleSequence,playerAv,waitBystander);
			}

			playerIsEnroute = false;
		}
		else if(para_eventID == "RotationEffect")
		{
			triggerSoundAtCamera("StoneThud",0.5f);

			GameObject rotMasterObj = GameObject.Find("RotationObj");
			Vector3 tmpAngles = rotMasterObj.transform.localEulerAngles;
			tmpAngles.z = -120f;
			rotMasterObj.transform.localEulerAngles = tmpAngles;

			GameObject hexBoardObj = GameObject.Find("HexBoard");

			List<string> childNames = new List<string>();
			for(int i=0; i<rotMasterObj.transform.childCount; i++)
			{
				childNames.Add(rotMasterObj.transform.GetChild(i).name);
			}

			for(int i=0; i<childNames.Count; i++)
			{
				Transform tmpChild = rotMasterObj.transform.FindChild(childNames[i]);

				if(tmpChild.name == "Outline")
				{
					Destroy(tmpChild.gameObject);
				}
				else if(tmpChild.name == "PlayerAvatar")
				{
					tmpChild.transform.parent = null;

					Vector3 tmpMasterPos = tmpChild.position;
					tmpMasterPos.z += 2f;
					tmpChild.position = tmpMasterPos;
				}
				else
				{
					// Is a board cell.
					tmpChild.transform.parent = hexBoardObj.transform;
					tmpChild.renderer.sortingOrder = 0;
					tmpChild.FindChild("WordBox").FindChild("Text").renderer.sortingOrder = 1;

					Vector3 tmpMasterPos = tmpChild.position;
					tmpMasterPos.z += 2f;
					tmpChild.position = tmpMasterPos;
				}
			}

			if(activatedCornerButtonObj != null)
			{
				activatedCornerButtonObj.renderer.sortingOrder = 5;
				activatedCornerButtonObj.GetComponent<Animator>().Play("unclicked");
				activatedCornerButtonObj = null;

				Destroy(activatedGravelObj);
			}

			rotationReturnFlag = true;

			Destroy(rotMasterObj);


			/*if(inAutoRotateMode)
			{
				triggerRandomAutoRotation();
			}*/

		}
		else if(para_eventID == "CamPanToWaitBystander")
		{
			startWaitProcedure();
		}
		else if(para_eventID == "PlayerBystanderConvFinished")
		{
			// Trigger new level.
			setPlayerInputState(true);

			GameObject playerAv = GameObject.Find("PlayerAvatar");
			NewCharacterNavMovement cnm = playerAv.GetComponent<NewCharacterNavMovement>();
			if(cnm == null) { cnm = playerAv.AddComponent<NewCharacterNavMovement>(); }
			cnm.setToIdle();
			Destroy(cnm);

			sendBystanderReleaseMsg();
			numCompletedPaths++;
			numCompletePaths++;
			bool winFlag = checkNHandleWinningCondition();
			if( ! winFlag)
			{
				genNextLevel();
			}
		}
	}


	protected override void pauseScene(bool para_pauseState)
	{
		isPaused = para_pauseState;
		setPlayerInputState(!para_pauseState);
	}

	protected override bool checkNHandleWinningCondition()
	{
		updateActivityProgressMetaData(numCompletedPaths/(reqPathsForWin*1.0f));
		bool playerHasWon = (numCompletedPaths >= reqPathsForWin);
		
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
		
		// Trigger record outcome.
		recordOutcomeForConfig(null);
	}

	protected override GameyResultData buildGameyData()
	{
		float timeDiff = endTimestamp - startTimestamp;
		int minutes = (int) (timeDiff / 60f);
		int seconds = (int) (((timeDiff / 60f) - minutes) * 60f);

		MPGameyResultData reqData = new MPGameyResultData(numCompletePaths,numMissteps,numRotations,minutes,seconds);
		return reqData;
	}

	protected override string generateGoalTextAppend()
	{
		return (" "+reqPathsForWin);
	}


	private void triggerAvatarWalkToCell(string para_cellGObjName)
	{

		if(cellNameToNodeIDMap.ContainsKey(para_cellGObjName))
		{
			playerIsEnroute = true;
			playerWalkEndCell = para_cellGObjName;

			int nxtNodeID = cellNameToNodeIDMap[para_cellGObjName];
			NavNode nxtNode = boardNavGraph.getNode(nxtNodeID);
			int[] nxtNodeBoardCoords = extractCoordsFromCellName(para_cellGObjName);



			GameObject playerAvatar = GameObject.Find("PlayerAvatar");
			NewCharacterNavMovement cnm = playerAvatar.gameObject.GetComponent<NewCharacterNavMovement>();
			if(cnm == null)
			{
				cnm = playerAvatar.gameObject.AddComponent<NewCharacterNavMovement>();
				cnm.registerListener("AcScen",this);
			}
			bool faceDownWhenOver = true;
			if((nxtNodeBoardCoords[0] == currDestCell[0])&&(nxtNodeBoardCoords[1] == currDestCell[1]))
			{
				faceDownWhenOver = false;
			}

			cnm.moveAlongPath(new List<NavNode>() { nxtNode },2f,faceDownWhenOver,false);

			triggerSoundAtCamera("HealFootstepsTiles",1);
		}
	}

	private int[] extractCoordsFromCellName(string para_cellname)
	{
		string[] parts = para_cellname.Split(',');
		int[] retCoords = new int[2] { int.Parse(parts[0].Split('(')[1]), int.Parse(parts[1].Split(')')[0]) };
		return retCoords;
	}

	private void focusCameraAtPointOfInterest(Vector3 para_pointOfInterest)
	{
		Rect fullEncaps = worldBoundsLookup["FullEncapsulation"];
		Rect cameraWorldBounds = WorldSpawnHelper.getCameraViewWorldBounds(2,true);
		Rect nwLookAtBounds = CommonUnityUtils.findLookatBoundsWithinLimitedArea(para_pointOfInterest,cameraWorldBounds,fullEncaps);
		
		Vector3 camDestPos = new Vector3(nwLookAtBounds.x + (nwLookAtBounds.width/2f),
		                                 nwLookAtBounds.y - (nwLookAtBounds.height/2f),
		                                 Camera.main.transform.position.z);

		Camera.main.transform.position = camDestPos;
	}


	private void hideIrrelevantCornerButtons()
	{
		if(hiddenCornerButtons == null)
		{
			hiddenCornerButtons = new List<GameObject>();
		}

		//Show previously hidden buttons
		for(int i=0; i<hiddenCornerButtons.Count; i++)
		{
			GameObject tmpCornerButton = hiddenCornerButtons[i];
			if(tmpCornerButton != null)
			{
				tmpCornerButton.renderer.enabled = true;
				tmpCornerButton.collider.enabled = true;
			}
		}
		hiddenCornerButtons.Clear();

		GameObject hexBoardObj = GameObject.Find("HexBoard");
		Transform currCellGObj = hexBoardObj.transform.FindChild("Cell("+currPlayerCell[0]+","+currPlayerCell[1]+")");
	    Transform destCellGObj = hexBoardObj.transform.FindChild("Cell("+currDestCell[0]+","+currDestCell[1]+")");
		Transform walkEndCell = null;
		if(playerWalkEndCell!=null)
			hexBoardObj.transform.FindChild(playerWalkEndCell);


		GameObject tmpProber = GameObject.Find("HexTileProber");
		Transform nwProber = null;
		if(tmpProber != null)
		{
			nwProber = tmpProber.transform;
		}
		else
		{
			nwProber = (Transform) Instantiate(hexProberPrefab,new Vector3(9000,0,-2),Quaternion.identity);
			nwProber.name = "HexTileProber";
		}

		//Removed buttons around the cell where the avatar stands, the target cell and the cell where the avatar is walking towards
		List<Transform> candidateCells = new List<Transform>() { currCellGObj, destCellGObj, walkEndCell };
		for(int i=0; i<candidateCells.Count; i++)
		{
			Transform tmpCell = candidateCells[i];
			if(tmpCell != null)
			{
				Vector3 tmpPos = nwProber.position;
				tmpPos.x = tmpCell.position.x;
				tmpPos.y = tmpCell.position.y;
				nwProber.position = tmpPos;

				for(int k=0; k<nwProber.childCount; k++)
				{
					Transform tmpPChild = nwProber.GetChild(k);
					//Debug.LogWarning("("+nwProber.position.x+","+nwProber.position.y+") -> "+k+" ("+tmpPChild.position.x+","+tmpPChild.position.y+")");
					RaycastHit hitInf;
					if(Physics.Raycast(Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(tmpPChild.position)),out hitInf))
					{
						if(hitInf.collider.name.Contains("CornerButton"))
						{
							GameObject reqCornerBtn = hitInf.collider.gameObject;
							reqCornerBtn.renderer.enabled = false;
							reqCornerBtn.collider.enabled = false;
							hiddenCornerButtons.Add(reqCornerBtn);
						}
					}
				}
			}
		}

	}


	private void setupInstructionTile()
	{

		if(descriptionType == 1) // sound only.
		{

			Transform instructionTile = Camera.main.transform.FindChild("InstructionTile");
			Transform oldTextBanner = instructionTile.FindChild("InstructionText");
			if(oldTextBanner != null) { Destroy(oldTextBanner.gameObject); }

			Transform instructionTA = instructionTile.FindChild("TextArea");
			instructionTA.renderer.enabled = false;

			instructionTile.FindChild("TtsIcon").renderer.enabled = false;
			instructionTile.renderer.enabled = false;
		}
		else
		{
			// Text display.

			Transform instructionTile = Camera.main.transform.FindChild("InstructionTile");
			Transform oldTextBanner = instructionTile.FindChild("InstructionText");
			if(oldTextBanner != null) { DestroyImmediate(oldTextBanner.gameObject); }
			instructionTile.FindChild("TtsIcon").renderer.enabled = false;

			Transform instructionTA = instructionTile.FindChild("TextArea");
			Rect instructionTABounds = new Rect(instructionTA.renderer.bounds.center.x - (instructionTA.renderer.bounds.size.x/2f),
			                                    instructionTA.renderer.bounds.center.y + (instructionTA.renderer.bounds.size.y/2f),
			                                    instructionTA.renderer.bounds.size.x,
			                                    instructionTA.renderer.bounds.size.y);
			GameObject nwInstructionText = WordBuilderHelper.buildWordBox(-9,currLvlConfig.pattern,instructionTABounds,instructionTA.position.z,upAxisArr,wordBoxPrefab);
			nwInstructionText.name = "InstructionText";
			nwInstructionText.transform.parent = instructionTile;
			GameObject instBoardChild = nwInstructionText.transform.FindChild("Board").gameObject;
			instBoardChild.renderer.enabled = false;
			TextNBoardScript instTnbScript = nwInstructionText.GetComponent<TextNBoardScript>();
			instTnbScript.destroyAfterInit = true;
			WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() { nwInstructionText },0.035f);
			
			instBoardChild.renderer.sortingOrder = instructionTile.renderer.sortingOrder + 1;
			nwInstructionText.transform.FindChild("Text").renderer.sortingOrder = instructionTile.renderer.sortingOrder + 2;

			instructionTA.renderer.enabled = false;

			instructionTile.renderer.enabled = false;
			instructionTile.FindChild("InstructionText").FindChild("Text").renderer.enabled = false;
		}

		descriptionTileButtonOn = false;
	}
	

	private void togglePauseButtonVisibility(bool para_state)
	{
		isPauseButtonVisible = para_state;
		GameObject pauseBtn = GameObject.Find("PauseButton");
		pauseBtn.renderer.enabled = para_state;
	}


	private void triggerSoundAtCamera(string para_soundFileName)
	{
		triggerSoundAtCamera(para_soundFileName,1f);
	}
	
	private void triggerSoundAtCamera(string para_soundFileName, float para_volume)
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
		audS.Play();
	}


	GameObject ambientSound;

	private void triggerAmbienceSound(string para_soundFileName, float para_volume)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		if(sfxPrefab == null) { sfxPrefab = Resources.Load<Transform>("Prefabs/SFxBox"); }
		ambientSound = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		Destroy(ambientSound.GetComponent<DestroyAfterTime>());
		AudioSource audS = (AudioSource) ambientSound.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = para_volume;
		audS.loop = true;
		audS.Play();
	}
}

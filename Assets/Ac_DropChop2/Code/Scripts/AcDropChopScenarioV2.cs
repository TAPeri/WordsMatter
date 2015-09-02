/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class AcDropChopScenarioV2 : ILearnRWActivityScenario, CustomActionListener
{
	public Transform textPlanePrefab;
	public Transform splitDetectorPrefab;
	public Transform[] itemToSplitPrefabArr;
	public Transform[] crackPrefabArr;
	public Transform framePrefab;
	public Transform sfxPrefab;
	public Transform sparksPrefab;
	public Transform scalableGlowPrefab;
	public Transform planePrefab;

	public Transform skeletonSwipeHandPrefab;
	public Transform nettingPrefab;
	public Transform blockToObjectEffect;

	public Transform[] oneSyllPrefabs;
	public Transform[] twoSyllPrefabs;
	public Transform[] threeSyllPrefabs;
	public Transform[] fourSyllPrefabs;
	public Transform[] fiveSyllPrefabs;

	public bool showSparksEffect;



	SJLevelConfig currLvlConfig;
	SJWordItem currWordItem;
	Dictionary<int,SJWordItem> wordItemCache;
	
	
	
	int gridWidthInCells = 11;//17;
	int gridHeightInCells = 6;
	int numPrequelRows = 1;
	float borderThicknessPerc = 0f;//0.1f;
	GridProperties dropGrid_GuiGProp;
	GridProperties dropGrid_WorldGProp;
	
	
	
	string conveyorWordObjID;
	HashSet<string> conveyorWordGObjLookup;


	
	int nxtWordID = 1;
	int[,] gridState;
	string[,] debugGridState;
	string currConvWordBeingDropped;
	HashSet<string> droppingWords;
	HashSet<string> stationaryWords;
	Dictionary<string,TetrisInfo> tetrisInfMap;
	Dictionary<int,string> numIDToStrIDMap;
	Dictionary<string,string> blockToWDataMap;
	float wordDropStepTimerStart_Sec;
	float wordDropStepTime_Sec = 0.1f;
	bool conveyorOpen;
	bool updateStationaryBlocks;



	bool playItemCollideSound;
	
	bool performLineCheck = false;
	
	List<GameObject> cracksGObjs;
	float growScale;
		
	List<string> orderedFullySplitConvObjNames;
	int nxtIndexForOp;
	bool isPerformingPositioningSequence = false;
	
	bool isInAutoSplitMode;
	string itemForAutoSplit;
	List<int> syllsForAutoSplit;	
	List<int> splitsDoneByGame;

	Color unmovableBlockColor = new Color(0.2f,0.16f,0.16f,1f);
	
	bool isWaitingForConveyorRowClearence;
	bool playerHasLost = false;
	bool chuteBusy = false;

	float wordChopTimerDuration_Sec = 30;
	
	List<GameObject> objsToUnPause;
	bool pause = false;
	bool[] upAxisArr = new bool[3] {false,true,false};

	//int maxAttemptsPerConveyorWord = 3;
	bool[] conWordAttemptsStates;

	int currNumOfWordsDone = 0;
	int reqNumOfWordsDone; // Initialised when generator is initalised below.

	int numHooveredLines = 0;
	int numCorrectWordSplits = 0;
	int numIncorrectWordSplits = 0;
	List<int> playerSplitPattern;


	bool wordUnattempted = true;

	bool whistleVisible = false;


	bool firstTimeFlag = true;



	void Start()
	{
		acID = ApplicationID.DROP_CHOPS;
		loadActivitySessionMetaData();
		
		this.loadTextures();
		prepUIBounds();
		
		initWorld();
		this.initLevelConfigGenerator();
		reqNumOfWordsDone = lvlConfigGen.getConfigCount();
		genNextLevel();

		showInfoSlideshow();
		initPersonHelperPortraitPic();
		recordActivityStart();
	}



	void Update()
	{
		
		if(isInitialised)
		{
			
			
			
			if( ! pause)
			{
				// Move existing blocks down:
				//int nwLineToClear = -1;
				//bool moveMade = false;
				//bool cameToRestOccurred = false;
				HashSet<string> nwStationaryBlocks = new HashSet<string>();
				HashSet<string> nwDestroyedBlocks = new HashSet<string>();
				HashSet<string> nwDroppingBlocks = new HashSet<string>();
				if((Time.time - wordDropStepTimerStart_Sec) > wordDropStepTime_Sec)
				{
					List<string> blocksToCheck = new List<string>();
					blocksToCheck.AddRange(droppingWords);
					if(updateStationaryBlocks)
					{
						blocksToCheck.AddRange(stationaryWords);
					}
					
					
					bool changeOccurred = false;
					Vector3 movDownVect = new Vector3(0,-(dropGrid_WorldGProp.cellHeight + dropGrid_WorldGProp.borderThickness),0);
					for(int i=0; i<blocksToCheck.Count; i++)
					{
						
						//Debug.Log("ToCheck: "+blocksToCheck[i]);
						if(tetrisInfMap.ContainsKey(blocksToCheck[i]))
						{
							TetrisInfo tInfo = tetrisInfMap[blocksToCheck[i]];
							int nwRow = tInfo.coords[1]+1;
							
							if(nwRow < (gridState.GetLength(1)+1))
							{
									bool canMoveDown = true;
									
									if(nwRow >= gridState.GetLength(1))
									{
										canMoveDown = false;	
									}
									else
									{	
										for(int x=tInfo.coords[0]; x<(tInfo.coords[0]+tInfo.wordSize); x++)
										{
											if(gridState[x,nwRow] != 0)
											{
												canMoveDown = false;
												break;
											}
										}
									}
									
									
									if(canMoveDown)
									{
										GameObject reqGObj = GameObject.Find(blocksToCheck[i]);
										reqGObj.transform.Translate(movDownVect);
										
										for(int x=tInfo.coords[0]; x<(tInfo.coords[0]+tInfo.wordSize); x++)
										{
											gridState[x,tInfo.coords[1]] = 0;
											gridState[x,nwRow] = tInfo.id;
											
											debugGridState[x,tInfo.coords[1]] = "0";
											debugGridState[x,nwRow] = ""+tInfo.id;
										}
										
										tInfo.coords[1] = nwRow;
										tetrisInfMap[blocksToCheck[i]] = tInfo;
										
										
										
										
										if(stationaryWords.Contains(blocksToCheck[i]))
										{
											nwDroppingBlocks.Add(blocksToCheck[i]);
										}
										
										changeOccurred = true;
										//moveMade = true;
									}
									else
									{
										//cameToRestOccurred = true;
										
										if( ! stationaryWords.Contains(blocksToCheck[i]))
										{
											nwStationaryBlocks.Add(blocksToCheck[i]);
											performLineCheck = true;
											playItemCollideSound = true;
										}
									}
								}
						}
					}
					
					if( ! changeOccurred)
					{
						if(!isWaitingForConveyorRowClearence)
						{
							updateStationaryBlocks = false;
							if(!playerHasLost)
							{
								checkNHandleLosingCondition();
							}
						}
					}
					
					if( ! changeOccurred)
					{
						if(isPerformingPositioningSequence)
						{
							if(nxtIndexForOp < orderedFullySplitConvObjNames.Count)
							{
								if(isRowFree(0))
								{

									
									isWaitingForConveyorRowClearence = false;
									moveNextBlockToGrid();	
								}
							}
						}
					}
					
					wordDropStepTimerStart_Sec = Time.time;
				}
				
				
				foreach(string deletedItem in nwDestroyedBlocks)
				{
					droppingWords.Remove(deletedItem);
				}
				
				foreach(string dItem in nwDroppingBlocks)
				{
					if(!droppingWords.Contains(dItem))
					{
						droppingWords.Add(dItem);
					}
					stationaryWords.Remove(dItem);
				}
				
				foreach(string statItem in nwStationaryBlocks)
				{
					if(!stationaryWords.Contains(statItem))
					{
						stationaryWords.Add(statItem);
					}
					droppingWords.Remove(statItem);
				}
				
				
				if(performLineCheck)
				{
					int lineToClear = searchForLinesToClear();
					
					if(lineToClear != -1)
					{
						performClearLineEffect(lineToClear);
						updateStationaryBlocks = true;
						numHooveredLines++;
					}
					
					performLineCheck = false;
				}
				
				
				
				
				
				
				// Check if Conveyor is free.
				if(! chuteBusy)
				{
					if(conveyorWordGObjLookup.Count == 0)
					{
						if((isRowFree(0)))//&&(isRowFree(1)))
						{
							conveyorOpen = true;
						}
					}
				}
				
				
				
				// Conveyor Script. When the current block is at rest, send the next block down.
				if(conveyorOpen)
				{
					if( ! droppingWords.Contains(currConvWordBeingDropped))
					{
						if(conveyorOpen)
						{		
							currNumOfWordsDone++;
							genNextLevel();
						}
					}
				}

				

				
				
				if(playItemCollideSound) { triggerSoundAtCamera("Blop"); playItemCollideSound = false; }

				
				
				
				
			}// end of pause bracket.
			
			
		}// end of isInitialised bracket.
		
		
		
	}


	void OnGUI()
	{
		if( ! pause)
		{
			if(whistleVisible)
			{
				GUI.color = Color.clear;
				if(GUI.Button(uiBounds["WhistleIcon"],""))
				{
					triggerSoundAtCamera("whistle1");
					getDeliveryChuteScript().hurryUp();
					setWhistleVisibility(false);
				}
			}

			GUI.color = Color.clear;
			if(uiBounds.ContainsKey("PauseBtn"))
			{
				if(GUI.Button(uiBounds["PersonPortrait"],""))
				{
					showPersonHelperWindow();
				}

				if(GUI.Button(uiBounds["PauseBtn"],""))
				{
					showActivityPauseWindow();
				}
			}
		}


		/*// Debugging: Display Grid State:
		for(int r=0; r<debugGridState.GetLength(1); r++)
		{
			for(int c=0; c<debugGridState.GetLength(0); c++)
			{
				if(debugGridState[c,r] != "0") { GUI.color = Color.black; } else { GUI.color = Color.white;	}
				GUI.Label(new Rect((c * 20f),(r * 20),200,20),debugGridState[c,r]);	
				GUI.color = Color.white;
			}
		}*/

	}



	protected override void initWorld()
	{

		// Auto Adjust.
		GameObject tmpPersonPortrait = GameObject.Find("PersonPortrait");
		GameObject tmpPauseButton = GameObject.Find("PauseButton");
		GameObject tmpBackdrop = GameObject.Find("Background").gameObject;
		SpawnNormaliser.adjustGameObjectsToNwBounds(SpawnNormaliser.get2DBounds(tmpBackdrop.renderer.bounds),
		                                            WorldSpawnHelper.getCameraViewWorldBounds(tmpBackdrop.transform.position.z,true),
		                                            new List<GameObject>() { tmpPersonPortrait, tmpPauseButton });
		
		//tmpExitBtnObj.transform.parent = Camera.main.transform;
		tmpPersonPortrait.transform.parent = GameObject.Find("Main Camera").transform;
		tmpPauseButton.transform.parent = GameObject.Find("Main Camera").transform;

		uiBounds.Add("PersonPortrait",WorldSpawnHelper.getWorldToGUIBounds(tmpPersonPortrait.renderer.bounds,upAxisArr));
		uiBounds.Add("PauseBtn",WorldSpawnHelper.getWorldToGUIBounds(tmpPauseButton.renderer.bounds,upAxisArr));		 


		// Mechanics Vars Init:


		// (1) Junk Grid:


		int totalRows = (gridHeightInCells+numPrequelRows);
		gridState = new int[gridWidthInCells,totalRows];
		debugGridState = new string[gridWidthInCells,totalRows];
		for(int r=0; r<totalRows; r++)
		{
			for(int c=0; c<gridWidthInCells; c++)
			{
				gridState[c,r] = 0;	
				debugGridState[c,r] = "0";
			}
		}
		currConvWordBeingDropped = "";
		droppingWords = new HashSet<string>();
		stationaryWords = new HashSet<string>();
		tetrisInfMap = new Dictionary<string, TetrisInfo>();
		numIDToStrIDMap = new Dictionary<int, string>();
		blockToWDataMap = new Dictionary<string, string>();
		wordItemCache = new Dictionary<int, SJWordItem>();
		wordDropStepTimerStart_Sec = Time.time;
		conveyorOpen = true;
		updateStationaryBlocks = false;


		GameObject mainPitGObj = GameObject.Find("MainPit");
		//Vector3 mainPitTopLeft = new Vector3(mainPitGObj.transform.position.x - (mainPitGObj.renderer.bounds.size.x/2f),
		//                                     mainPitGObj.transform.position.y + (mainPitGObj.renderer.bounds.size.y/2f),
		//                                     mainPitGObj.transform.position.z);

		//dropGrid_WorldGProp = new GridProperties(dropMainGridBox_world,gridWidthInCells,gridHeightInCells,borderThicknessPerc,mainPitGObj.transform.position.z);
		//dropGrid_WorldGProp = new GridProperties(new float[] {mainPitTopLeft.x,mainPitTopLeft.y,mainPitTopLeft.z},borderThicknessPerc,0.72f,0.72f,gridWidthInCells,gridHeightInCells);


		// Height is fixed.
		float reqHeightForCell = mainPitGObj.renderer.bounds.size.y/(gridHeightInCells * 1.0f);
		float reqWidthForCell = reqHeightForCell;
		float reqTotalWidthForGrid = reqWidthForCell * (gridWidthInCells * 1.0f);
		float reqTotalHeightForGrid = reqHeightForCell * (gridHeightInCells * 1.0f);

		Vector3 tmpLocalScale = mainPitGObj.transform.localScale;
		tmpLocalScale.x *= reqTotalWidthForGrid/mainPitGObj.renderer.bounds.size.x;
		tmpLocalScale.y *= reqTotalHeightForGrid/mainPitGObj.renderer.bounds.size.y;
		//tmpLocalScale.z = tmpLocalScale.z;
		mainPitGObj.transform.localScale = tmpLocalScale;

		Rect dropMainGridBox_world = CommonUnityUtils.get2DBounds(mainPitGObj.renderer.bounds);
		dropGrid_WorldGProp = new GridProperties(dropMainGridBox_world,gridWidthInCells,gridHeightInCells,borderThicknessPerc,mainPitGObj.transform.position.z);



		GridRenderer.createGridRender(dropGrid_WorldGProp,planePrefab,upAxisArr);


		createRandomStartGrid();




		// (2) Other vars:


		conveyorWordObjID = null;
		conveyorWordGObjLookup = new HashSet<string>();
		
		cracksGObjs = new List<GameObject>();
		
		playItemCollideSound = false;
		conWordAttemptsStates = new bool[3] {false,false,false};

		isInAutoSplitMode = false;
		itemForAutoSplit = null;
		syllsForAutoSplit = null;
		splitsDoneByGame = new List<int>();

		playerSplitPattern = new List<int>();
		
		isWaitingForConveyorRowClearence = false;

		wordDropStepTimerStart_Sec = Time.time;
		getChopTimerScript().setVisibilityState(false);


		PlayerBlockMovementScript pbms = transform.gameObject.GetComponent<PlayerBlockMovementScript>();
		pbms.init();
		
		LineDragActiveNDraw ldand = (GameObject.Find("LineMang").GetComponent<LineDragActiveNDraw>());
		ldand.init(WorldSpawnHelper.getWorldToGUIBounds(GameObject.Find("Background").renderer.bounds,upAxisArr),
		           0.1f,dropGrid_WorldGProp.z,dropGrid_WorldGProp.z,"SplitDetect",0);
		ldand.registerListener("AcScen",this);
		ldand.setPause(true);

		ScoreBoardScript sbs = getScoreBoardScript();
		sbs.reset();

		updateLineCounterDisplay();
		setWhistleVisibility(false);
		
		SoundOptionsDisplay sod = GameObject.Find("GlobObj").GetComponent<SoundOptionsDisplay>();
		sod.registerListener("AcScen",this);
	}

	

	protected override void genNextLevel()
	{


		// Perhaps we will need to add a error message if the config list was empty.
		bool winFlag = checkNHandleWinningCondition();

		if( ! winFlag)
		{
			if( ! firstTimeFlag)
			{
				conveyorOpen = false;
				chuteBusy = true;

				currLvlConfig = (SJLevelConfig) lvlConfigGen.getNextLevelConfig(null);
				wordItemCache.Add(currLvlConfig.getWordItem().getID(),currLvlConfig.getWordItem());
				wordChopTimerDuration_Sec = 10f*(3-currLvlConfig.getSpeed());

				if(serverCommunication != null) { serverCommunication.wordDisplayed(currLvlConfig.getWordItem().getWordAsString(),currLvlConfig.getWordItem().languageArea,currLvlConfig.getWordItem().difficulty); }

				playerSplitPattern = new List<int>();

				openGate();
				resetAttempts();
				triggerChuteToFetchAndReturnWord();
			}
		}
	}

	protected new bool initLevelConfigGenerator()
	{
		if( ! base.initLevelConfigGenerator())
		{
			// Fallback.
			lvlConfigGen = new SJLevelConfigGeneratorServer(null); //new SJLevelConfigGeneratorHardCoded();
			Debug.LogWarning("Warning: Using Level Gen Fallback");
		}
		return true;
	}

	
	protected override bool checkNHandleWinningCondition()
	{
		updateActivityProgressMetaData(currNumOfWordsDone/(reqNumOfWordsDone*1.0f));
		if(currNumOfWordsDone >= reqNumOfWordsDone)
		{
			pauseScene(true);
			performDefaultWinProcedure();
			return true;
		}
		return false;
	}
	
	
	protected override bool checkNHandleLosingCondition()
	{
		playerHasLost = !(isRowFree(numPrequelRows-1,true));
		
		if(playerHasLost)
		{
			pauseScene(true);
			performDefaultLoseProcedure();
			return true;
		}
		return false;
	}

	protected override void buildNRecordConfigOutcome(System.Object[] para_extraParams)
	{
		// Build outcome object.
		bool positiveFlag = (bool) para_extraParams[0];

		// Trigger record outcome.
		SJLevelOutcome reqOutcomeObj = null;
		if(positiveFlag)
		{
			reqOutcomeObj = new SJLevelOutcome(true);
		}
		else
		{
			reqOutcomeObj = new SJLevelOutcome(false,playerSplitPattern);
		}

		recordOutcomeForConfig(reqOutcomeObj);
	}

	protected override GameyResultData buildGameyData()
	{
		SJGameyResultData reqData = new SJGameyResultData(numHooveredLines,numCorrectWordSplits,numIncorrectWordSplits);
		return reqData;
	}

	protected override string generateGoalTextAppend()
	{
		return (" "+reqNumOfWordsDone);
	}

	protected override void pauseScene(bool para_state)
	{
		if(para_state)
		{
			pause = true;
			//LineDragActiveNDraw swipeMang = GameObject.Find("LineMang").GetComponent<LineDragActiveNDraw>();
			//swipeMang.setPause(true);
			PlayerBlockMovementScript pbms = GameObject.Find("GlobObj").GetComponent<PlayerBlockMovementScript>();
			if(pbms != null) { pbms.enabled = false; }
			
			/*if(objsToUnPause == null) { objsToUnPause = new List<GameObject>(); } else { objsToUnPause.Clear(); }
			foreach(string convItem in conveyorWordGObjLookup)
			{
				GameObject tmpObj = GameObject.Find(convItem);
				if(tmpObj != null)
				{
					objsToUnPause.Add(tmpObj);
					tmpObj.SetActive(false);
				}
			}*/

			getDeliveryChuteScript().setPauseState(pause);
			CircleTimerScript chopTimerScript = getChopTimerScript();
			if(chopTimerScript != null) { chopTimerScript.setPause(pause); }
		}
		else
		{
			pause = false;

			if(firstTimeFlag)
			{
				firstTimeFlag = false;
				genNextLevel();
			}

			//LineDragActiveNDraw swipeMang = GameObject.Find("LineMang").GetComponent<LineDragActiveNDraw>();
			//swipeMang.setPause(false);
			PlayerBlockMovementScript pbms = GameObject.Find("GlobObj").GetComponent<PlayerBlockMovementScript>();
			if(pbms != null) { pbms.enabled = true; }
			
			/*for(int i=0; i<objsToUnPause.Count;i++)
			{
				objsToUnPause[i].SetActive(true);
			}
			objsToUnPause.Clear();*/
			
			//conveyorCurrentTime = Time.time;

			getDeliveryChuteScript().setPauseState(pause);
			CircleTimerScript chopTimerScript = getChopTimerScript();
			if(chopTimerScript != null) { chopTimerScript.setPause(pause); }
		}
	}
	

	public new void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{

		base.respondToEvent(para_sourceID,para_eventID,para_eventData);


		if(para_sourceID == "BigPipe")
		{
			if(para_eventID == "EnterStart")
			{
				setWhistleVisibility(true);
			}
			else if(para_eventID == "DeliveryChuteEnter")
			{
				DeliveryChuteScript dcs = getDeliveryChuteScript();
				GameObject nwWordBlock = dcs.getAttachedWordGObj();
				nwWordBlock.transform.parent = null;
				growScale = dcs.getGrowScale();

				registerNewWordBlock(nwWordBlock,currWordItem.getWordLength(),new int[] {0,-1},currWordItem.getID());
				registerNewConveyorWord(nwWordBlock);

				recordPresentedConfig(currLvlConfig);

				LineDragActiveNDraw swipeMang = GameObject.Find("LineMang").GetComponent<LineDragActiveNDraw>();
				swipeMang.setPause(false);
				SplittableBlockUtil.setStateOfSplitDetectsForWord(nwWordBlock,true);
				
				// Switch timer back on.
				//conveyorCurrentTime = Time.time;
				//conveyorTimerOn = true;

				ScoreBoardScript sbs = getScoreBoardScript();
				sbs.resetCrosses();
				getChopTimerScript().reset();
				closeGate();
			}
		}
		else if(para_eventID == "SwipeHit")
		{
			
			string[] swipeLocInfo = (string[]) para_eventData;
			handleSplitterHit(swipeLocInfo[0],swipeLocInfo[1]);
			
		}
		else if(para_eventID == "SwipeComplete")
		{
			
			if(GameObject.Find(para_sourceID).transform.parent != null)
			{
				if(GameObject.Find(para_sourceID).transform.parent.name == "SkeletonSwipeHand")//
				{
					if((isInAutoSplitMode)&&(GameObject.Find("SkeletonSwipeHand") != null))
					{
						Destroy(GameObject.Find("SkeletonSwipeHand"));
						
						if(syllsForAutoSplit.Count > 0)
						{
							performNextAutoSwipe();
						}
						else
						{
							isInAutoSplitMode = false;
							LineDragActiveNDraw swipeMang = GameObject.Find("LineMang").GetComponent<LineDragActiveNDraw>();
							swipeMang.setPause(true);
						}
					}
				}
			}
			
		}
		else if(para_eventID == "HighlightSequence")
		{
			if(nxtIndexForOp < orderedFullySplitConvObjNames.Count)
			{
				performNextHighlightSequence();
			}
			else
			{
				//
				if((!wordUnattempted)&&( ! hasGotWrongAttemptOnCurrentWord()))
				{
					ScoreBoardScript sbs = getScoreBoardScript();
					sbs.registerListener("AcScen",this);
					getScoreBoardScript().addStar();
				}
				else
				{
					respondToEvent("Scoreboard","ScoreboardUpdate",null);
				}
			}
		}
		else if(para_eventID == "ScoreboardUpdate")
		{
			// All clear to open the gate and start placing
			// the cut blocks automatically in the grid.
			openGate();
			isPerformingPositioningSequence = true;
			nxtIndexForOp = 0;
			isWaitingForConveyorRowClearence = false;
			moveNextBlockToGrid();
		}
		else if(para_eventID == "MoveToGridAndTransformSequence")
		{
			// Delete Crack.
			if(cracksGObjs.Count > 0)
			{
				Destroy(cracksGObjs[0]);
				cracksGObjs.RemoveAt(0);
			}
			
			performIndivisibleSpawn(GameObject.Find(para_sourceID));
			droppingWords.Add(para_sourceID);
			
			isWaitingForConveyorRowClearence = false;
			
			if(nxtIndexForOp == orderedFullySplitConvObjNames.Count)
			{
				// End of placement sequence.
				isPerformingPositioningSequence = false;
				chuteBusy = false;
				nxtIndexForOp = 0;
			}
		}
		else if(para_sourceID == "SkeletonSwipeHand")
		{
			if(para_eventID == "MoveToLocation")
			{
				LineFollowActiveNDraw lfand = GameObject.Find(para_sourceID).transform.FindChild("FingerTip").GetComponent<LineFollowActiveNDraw>();
				lfand.triggerSwipeComplete();
				
				Destroy(GameObject.Find(para_sourceID));
			}
		}
		else if(para_eventID == "GateOpenAni")
		{

		}
		else if(para_eventID == "GateCloseAni")
		{
			// If tts is needed then say the word.
			if(currLvlConfig.getUseTtsFlag())
			{
				try
				{
					if(gameObject.GetComponent<AudioSource>() == null) { gameObject.AddComponent<AudioSource>(); }
					audio.PlayOneShot(WorldViewServerCommunication.tts.say(currLvlConfig.getWordItem().getWordAsString()));
				}
				catch(System.Exception ex)
				{
					Debug.LogError("Failed to use TTS. "+ex.Message);
				}
			}

			// Restart Timer.
			getChopTimerScript().init(wordChopTimerDuration_Sec);
			getChopTimerScript().setVisibilityState(true);
		}
		else if(para_eventID == "CircleTimerFinished")
		{
			buildNRecordConfigOutcome(new object[] { false });

			triggerAutoSwipeSequence(conveyorWordObjID);
		}
	}





	// LOGIC FUNCTIONS.

	private void openGate()
	{
		getChopTimerScript().setVisibilityState(false);
		ScoreBoardScript sbs = getScoreBoardScript();
		sbs.resetCrosses();

		GateScript gs = transform.gameObject.GetComponent<GateScript>();
		if(gs == null) { gs = transform.gameObject.AddComponent<GateScript>(); } else { gs.enabled = true; }
		gs.registerListener("AcScen",this);
		gs.openGate();
	}
	private void closeGate()
	{
		setWhistleVisibility(false);

		GateScript gs = transform.gameObject.GetComponent<GateScript>();
		if(gs == null) { gs = transform.gameObject.AddComponent<GateScript>(); } else { gs.enabled = true; }
		gs.registerListener("AcScen",this);
		gs.closeGate();
	}

	private void triggerChuteToFetchAndReturnWord()
	{
		DeliveryChuteScript dcs = getDeliveryChuteScript();

		currWordItem = currLvlConfig.getWordItem();

		dcs.performFetchWordSequence(currWordItem.getWordAsString(),
		                             dropGrid_WorldGProp,
		                             textPlanePrefab,
		                             splitDetectorPrefab,
		                             itemToSplitPrefabArr,
		                             framePrefab,
		                             scalableGlowPrefab,
		                             nettingPrefab);
	}



	private void registerNewWordBlock(GameObject para_wordBlockGObj,
	                                  int para_wordLength,
	                                  int[] para_gridCoords,
	                                  int para_lvlID)
	{
		string suffix = (para_wordBlockGObj.name.Split(':'))[1];
		string wordBlockName = "Block-"+nxtWordID+":"+suffix;
		para_wordBlockGObj.name = wordBlockName;

		int assignedID = nxtWordID;
		nxtWordID++;
		
		tetrisInfMap.Add(wordBlockName,new TetrisInfo(assignedID,para_gridCoords,para_wordLength,false));
		numIDToStrIDMap.Add(assignedID,wordBlockName);
		blockToWDataMap.Add("Block-"+(assignedID), ""+para_lvlID);
	}


	// WARNING: Call registerNewWordBlock before this.
	private void registerNewConveyorWord(GameObject para_nwConveyorBlock)
	{
		// Clear items.
		conveyorWordObjID = null;
		conveyorWordGObjLookup.Clear();
		currConvWordBeingDropped = null;

		// Update items.
		conveyorWordObjID = para_nwConveyorBlock.name.Split(':')[0];
		conveyorWordGObjLookup.Add(para_nwConveyorBlock.name);
		currConvWordBeingDropped = para_nwConveyorBlock.name;
		splitsDoneByGame = new List<int>();
		//resetConveyorTimerAndAttempts();
		conveyorOpen = false;
	}


	private bool hasGotWrongAttemptOnCurrentWord()
	{
		bool flag = false;
		if(conWordAttemptsStates != null)
		{
			for(int i=0; i<conWordAttemptsStates.Length; i++)
			{
				if(conWordAttemptsStates[i])
				{
					flag = true;
					break;
				}
			}
		}
		return flag;
	}

	private void triggerWrongSplitConsequences(string para_parentObjName, ref GameObject para_hitSplitDetector)
	{
		triggerSoundAtCamera("Buzzer_wrong_split");

		ScoreBoardScript sbs = getScoreBoardScript();
		if(sbs != null) { sbs.addWrongCross(); }

		string prefix1 = para_parentObjName.Split(':')[0];
		string prefix2 = currConvWordBeingDropped.Split(':')[0];

		if(prefix1 == prefix2)
		{
			if(conWordAttemptsStates[conWordAttemptsStates.Length-2] == true)
			{
				// Has reached max amount of tries.
				conWordAttemptsStates[conWordAttemptsStates.Length-1] = true;

				// Record outcome for this config.
				numIncorrectWordSplits++;
				buildNRecordConfigOutcome(new object[] { false });


				// Trigger Auto Swipe.
				
				// Prevent User Swipes.
				LineDragActiveNDraw swipeMang = GameObject.Find("LineMang").GetComponent<LineDragActiveNDraw>();
				swipeMang.setPause(true);
				
				// Trigger Auto Swipes.
				triggerAutoSwipeSequence(prefix1);
				

			}
			else
			{
				for(int i=0; i<conWordAttemptsStates.Length; i++)
				{
					if(conWordAttemptsStates[i] == false)
					{
						conWordAttemptsStates[i] = true;
						break;
					}
				}
			}
		}
	}
	
	
	
	private List<GameObject> triggerCorrectSplitConsequences(string para_wordObjName, int para_splitID)
	{
		GameObject wordObj = GameObject.Find(para_wordObjName);
		Transform wOverlay = wordObj.transform.FindChild("WordOverlay");		
		GameObject reqSplitDetector = (wOverlay.FindChild("SplitDet-"+para_splitID)).gameObject;
		return triggerCorrectSplitConsequences(para_wordObjName,ref reqSplitDetector);
	}
	
	private List<GameObject> triggerCorrectSplitConsequences(string para_parentObjName, ref GameObject para_hitSplitDetector)
	{
		
		int splitID = int.Parse((para_hitSplitDetector.name.Split('-'))[1]);
		
		
		// Trigger Sparks:
		if(showSparksEffect)
		{
			Vector3 sparksLoc = new Vector3(para_hitSplitDetector.transform.position.x,para_hitSplitDetector.transform.position.y,Camera.main.transform.position.z + 1);
			Instantiate(sparksPrefab,sparksLoc,Quaternion.identity);
		}
		
		// Trigger Sound:
		triggerSoundAtCamera("sfx_Swipe");
		
		
		// Display Crack:
		Transform reqCrack = crackPrefabArr[Random.Range(0,crackPrefabArr.Length)];
		float cWidth = dropGrid_WorldGProp.cellWidth * growScale;
		//Vector3 crackLoc = new Vector3(para_hitSplitDetector.transform.position.x,para_hitSplitDetector.transform.position.y,transform.position.z - 1);
		GameObject nwCrack = WorldSpawnHelper.initObjWithinWorldBounds(reqCrack,1f,1f,
		                                                               "Crack"+splitID,
		                                                               new Rect(para_hitSplitDetector.transform.position.x - (cWidth/2f),
		         para_hitSplitDetector.transform.position.y + ((dropGrid_WorldGProp.cellHeight * growScale)/2f),
		         cWidth,
		         (dropGrid_WorldGProp.cellHeight * growScale)),
		                                                               null,
		                                                               para_hitSplitDetector.transform.position.z,
		                                                               upAxisArr);
		
		//Transform nwCrack = (Transform) Instantiate(reqCrack,crackLoc,Quaternion.identity);
		cracksGObjs.Add(nwCrack);
		
		
		
		
		// Split Word:
		// Switch layer type to default in order to prevent further input from this split detector.
		para_hitSplitDetector.layer = 0;
		
		// Color Green.
		MeshRenderer tmpRend = (MeshRenderer) para_hitSplitDetector.GetComponent(typeof(MeshRenderer));
		Material nwMat = new Material(Shader.Find("Diffuse"));
		nwMat.color = Color.green;
		tmpRend.material = nwMat;
		tmpRend.enabled = true;
		
		
		// Split and register child blocks.
		List<GameObject> splitObjs = WordFactoryYUp.performSplitOnWordBoard(ref para_hitSplitDetector,framePrefab,scalableGlowPrefab);
		for(int i=0; i<splitObjs.Count; i++)
		{
			string splitObjName = splitObjs[i].name;
			string prefix = splitObjName.Split(':')[0];
			string suffix = splitObjName.Split(':')[1];
			
			
			
			
			TetrisInfo parentTetInf = tetrisInfMap[para_parentObjName];
			int parentX = parentTetInf.coords[0];
			int parentY = parentTetInf.coords[1];
			int parentSuffixStart = int.Parse((para_parentObjName.Split(':')[1]).Split('-')[0]);
			string tmpWDataMapFullKey = blockToWDataMap[prefix];
			SJWordItem parentWData = findWordInCache(int.Parse(tmpWDataMapFullKey));
			
			
			
			
			
			
			
			//int sourceBlockID = int.Parse(prefix.Split('-')[1]);
			
			int childX1_FromParent = int.Parse(suffix.Split('-')[0]);
			int childX2_FromParent = int.Parse(suffix.Split('-')[1]);
			
			int[] childCoords = { (parentX + (childX1_FromParent - parentSuffixStart)), parentY };
			
			
			// Check if child is indivisible.
			bool isIndivisible = parentWData.checkIfWordSegmentIsIndivisible(childX1_FromParent,childX2_FromParent);
			
			
			// Decide whether to drop or not.
			if(prefix == conveyorWordObjID)
			{
				conveyorWordGObjLookup.Add(splitObjName);
				conveyorWordGObjLookup.Remove(para_parentObjName);
			}
			else
			{
				droppingWords.Add(splitObjName);
			}
			
			
			
			tetrisInfMap.Add(splitObjName,new TetrisInfo(nxtWordID, childCoords, (childX2_FromParent - childX1_FromParent + 1),isIndivisible));
			int assignedID = nxtWordID;
			nxtWordID++;
			
			numIDToStrIDMap.Add(assignedID,splitObjName);
		}
		
		
		// Delete parent block references.
		droppingWords.Remove(para_parentObjName);
		stationaryWords.Remove(para_parentObjName);
		tetrisInfMap.Remove(para_parentObjName);
		
		
		
		// Trigger block updates as some may have become free.
		updateStationaryBlocks = true;
		
		
		return splitObjs;
	}


	private void moveNextBlockToGrid()
	{
		if(nxtIndexForOp < orderedFullySplitConvObjNames.Count)
		{
			string blockName = orderedFullySplitConvObjNames[nxtIndexForOp];
			TetrisInfo tInf = tetrisInfMap[blockName];
			GameObject reqGObj = GameObject.Find(blockName);
			
			
			int gridColForBlock = ((int) (gridWidthInCells/2f)) - ((int) (tInf.wordSize/2f));
			if((gridColForBlock < 0)||(gridColForBlock >= gridWidthInCells))
			{
				gridColForBlock = 0;
			}
			int[] nwGridCoords = new int[2] {gridColForBlock,0};
			tInf.coords = nwGridCoords;
			tetrisInfMap[blockName] = tInf;
			
			
			
			if(conveyorWordGObjLookup.Contains(blockName))
			{
				conveyorWordGObjLookup.Remove(blockName);
				if(conveyorWordGObjLookup.Count == 0)
				{
					conveyorWordObjID = null;
					currConvWordBeingDropped = null;
				}
			}
			
			for(int c=tInf.coords[0]; c<(tInf.coords[0] + tInf.wordSize); c++)
			{
				if(c < (gridWidthInCells-1))
				{
					gridState[c,tInf.coords[1]] = tInf.id;
					debugGridState[c,tInf.coords[1]] = ""+tInf.id;
				}
			}
			
			
			Rect gridBoundsForBlock = new Rect(dropGrid_WorldGProp.x + (tInf.coords[0] * (dropGrid_WorldGProp.cellWidth + dropGrid_WorldGProp.borderThickness)) + dropGrid_WorldGProp.borderThickness,
			                                   dropGrid_WorldGProp.y + ((dropGrid_WorldGProp.cellHeight + dropGrid_WorldGProp.borderThickness)) + dropGrid_WorldGProp.borderThickness,
			                                   (tInf.wordSize * dropGrid_WorldGProp.cellWidth) + ((tInf.wordSize-1) * dropGrid_WorldGProp.borderThickness),
			                                   dropGrid_WorldGProp.cellHeight);
			
			
			
			Vector3 movDest = new Vector3(gridBoundsForBlock.x + (gridBoundsForBlock.width/2f),
			                              gridBoundsForBlock.y - (gridBoundsForBlock.height/2f),
			                              dropGrid_WorldGProp.z);
			
			
			float initWidthOfBlock = (dropGrid_WorldGProp.cellWidth * tInf.wordSize * growScale);
			float initHeightOfBlock = (dropGrid_WorldGProp.cellHeight * growScale);
			
			
			
			
			
			CustomAnimationManager caMang = reqGObj.AddComponent<CustomAnimationManager>();
			List<List<AniCommandPrep>> cmdBatchList = new List<List<AniCommandPrep>>();
			List<AniCommandPrep> cmdBatch1 = new List<AniCommandPrep>();
			cmdBatch1.Add(new AniCommandPrep("TeleportToLocation",1,new List<System.Object>() { new float[3] {movDest.x,movDest.y,movDest.z} }));
			cmdBatch1.Add(new AniCommandPrep("ResizeToWorldSize",1,new List<System.Object>() { new float[3] {gridBoundsForBlock.width,gridBoundsForBlock.height,1},
				new float[3] {initWidthOfBlock,initHeightOfBlock,1}}));
			List<AniCommandPrep> cmdBatch2 = new List<AniCommandPrep>();
			cmdBatch2.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] {0,1,0,1}, 2f }));
			cmdBatchList.Add(cmdBatch1);
			cmdBatchList.Add(cmdBatch2);
			caMang.init("MoveToGridAndTransformSequence",cmdBatchList);
			caMang.registerListener("AcScen",this);
			
			nxtIndexForOp++;
			
			isWaitingForConveyorRowClearence = true;
		}
	}


	private List<string> performClearLineEffect(int para_hooveLine)
	{
		HashSet<string> nwDestroyedBlocks = new HashSet<string>();
		
		float tmpY = dropGrid_WorldGProp.y - dropGrid_WorldGProp.borderThickness - (dropGrid_WorldGProp.cellHeight/2f);
		
		Vector3 hooveLineCentrePos = new Vector3(dropGrid_WorldGProp.x + (dropGrid_WorldGProp.totalWidth/2f),
		                                         tmpY - ((dropGrid_WorldGProp.cellHeight + dropGrid_WorldGProp.borderThickness) * (para_hooveLine-numPrequelRows)),
		                                         dropGrid_WorldGProp.z);
		
		
		
		Rect highlighterWorldBounds = new Rect(dropGrid_WorldGProp.x,hooveLineCentrePos.y + (dropGrid_WorldGProp.cellHeight/2f),dropGrid_WorldGProp.totalWidth,dropGrid_WorldGProp.cellHeight);
		GameObject rowHighlighter = WorldSpawnHelper.initObjWithinWorldBounds(planePrefab,1,1,"HooveRowHighlighter",highlighterWorldBounds,null,dropGrid_WorldGProp.z + 0.1f,upAxisArr);
		rowHighlighter.renderer.material.color = new Color(0,0.4f,0); //new Color(0.9f,0.7f,0.05f);
		
		
		GameObject hooveLineCollection = new GameObject("HooveLineCollection");
		hooveLineCollection.transform.position = hooveLineCentrePos;
		
		
		for(int i=0; i<gridWidthInCells; i++)
		{
			string blockName = numIDToStrIDMap[gridState[i,para_hooveLine]];
			int wordLength = tetrisInfMap[blockName].wordSize;
			
			GameObject tmpBlockToRemove = GameObject.Find(blockName);	
			tmpBlockToRemove.transform.parent = hooveLineCollection.transform;
			
			nwDestroyedBlocks.Add(blockName);
			tetrisInfMap.Remove(blockName);
			numIDToStrIDMap.Remove(gridState[i,para_hooveLine]);
			stationaryWords.Remove(blockName);
			droppingWords.Remove(blockName);
			
			for(int k=0; k<wordLength; k++)
			{
				gridState[i + k,para_hooveLine] = 0;
				debugGridState[i + k,para_hooveLine] = "0";
			}
			i += (wordLength-1); // -1 because the for loop will increment i once anyway.
		}
		
		pause = true;
		
		
		
		// Trigger hoove.
		
		GameObject solomonWholeGObj = GameObject.Find("SolomonWhole");
		GameObject hooveCntr = (solomonWholeGObj.transform.FindChild("HooveCentre")).gameObject;
		Vector3 hooveSpot = new Vector3(solomonWholeGObj.transform.position.x,
		                                hooveLineCentrePos.y + (solomonWholeGObj.transform.position.y - hooveCntr.transform.position.y),
		                                solomonWholeGObj.transform.position.z);
		float hooveMoveSpeed = 3f;
		float hooveSuctionSpeed = 15f;
		
		Vector3 hooveMovVectWithSpeed = (new Vector3(0,1,0)) * (dropGrid_WorldGProp.cellHeight * hooveMoveSpeed);
		Vector3 hooveSuctionVectWithSpeed = (new Vector3(-1,0,0)) * (dropGrid_WorldGProp.cellWidth * hooveSuctionSpeed);
		
		HooveScript hScript = solomonWholeGObj.AddComponent<HooveScript>();
		hScript.triggerHoove(hooveSpot,hooveMovVectWithSpeed,hooveSuctionVectWithSpeed,dropGrid_WorldGProp.x - (dropGrid_WorldGProp.totalWidth/2f),hooveLineCollection,sfxPrefab);
		
		LineDragActiveNDraw swipeMang = GameObject.Find("LineMang").GetComponent<LineDragActiveNDraw>();
		swipeMang.setPause(true);
		
		return (new List<string>(nwDestroyedBlocks));
	}








	private int searchForLinesToClear()
	{
		int rowID = -1;
		
		for(int r=(gridState.GetLength(1)-1); r>0; r--)
		{
			bool fullLineFormed = true;
			
			for(int c=0; c<gridWidthInCells; c++)
			{
				if(gridState[c,r] == 0)
				{
					c = gridWidthInCells; // tmp hack.
					fullLineFormed = false;
					break;
				}
				else
				{
					string blockName = numIDToStrIDMap[gridState[c,r]];
					TetrisInfo tmpTInf = tetrisInfMap[blockName];
					
					if((! tmpTInf.isIndivisible))
					{
						c = gridWidthInCells; // tmp hack.
						fullLineFormed = false;
						break;
					}
				}
			}
			
			if(fullLineFormed)
			{
				rowID = r;
				break;
			}
		}
		
		return rowID;
	}
	
	private bool isRowFree(int para_rowID)
	{
		bool retVal = true;
		for(int i=0; i<gridState.GetLength(0); i++)
		{
			if(gridState[i,para_rowID] != 0)
			{
				retVal = false;
				break;
			}
		}
		return retVal;
	}
	
	private bool isRowFree(int para_rowID, bool para_ignoreConveyorObjs)
	{
		bool retVal = true;
		for(int i=0; i<gridState.GetLength(0); i++)
		{
			if(gridState[i,para_rowID] != 0)
			{
				if(para_ignoreConveyorObjs)
				{
					if(conveyorWordGObjLookup.Contains(numIDToStrIDMap[gridState[i,para_rowID]]))
					{
						
					}
					else
					{
						retVal = false;
						break;
					}
				}
				else
				{
					retVal = false;
					break;
				}
			}
		}
		return retVal;
	}


	private void handleSplitterHit(GameObject para_splitDetectObj)
	{
		GameObject tmpOverlayObj = (para_splitDetectObj.transform.parent).gameObject;
		GameObject tmpMasterObj = (tmpOverlayObj.transform.parent).gameObject;
		handleSplitterHit(tmpMasterObj.name,para_splitDetectObj.name);
	}
	
	
	private void handleSplitterHit(string para_masterObjName, string para_splitDetectObjName)
	{
		// *** Check Where Split Event Occurred ***
		if( ! isInAutoSplitMode)
		{
			wordUnattempted = false;
		}
		
		GameObject tmpMasterObj = GameObject.Find(para_masterObjName);
		GameObject tmpOverlayObj = (tmpMasterObj.transform.FindChild("WordOverlay")).gameObject;
		GameObject hitSplitDetector = (tmpOverlayObj.transform.FindChild(para_splitDetectObjName)).gameObject;
		int splitIndex = int.Parse(hitSplitDetector.name.Split('-')[1]);
		
		
		string tmpWDatabaseWholeKey = blockToWDataMap[((tmpMasterObj.name).Split(':')[0])];
		SJWordItem reqWord = findWordInCache(int.Parse(tmpWDatabaseWholeKey));

		if( ! isInAutoSplitMode)
		{
			playerSplitPattern.Add(splitIndex);
		}

		
		if(! reqWord.isValidSplitIndex(splitIndex))
		{
			// Invalid split.
			triggerWrongSplitConsequences(tmpMasterObj.name,ref hitSplitDetector);
			if(serverCommunication != null)	{ serverCommunication.wordSolvedCorrectly(reqWord.getWordAsString(),false,"Invalid splits",reqWord.languageArea,reqWord.difficulty); }
		}
		else
		{
			// Valid split.
			// Perform split.
			triggerCorrectSplitConsequences(tmpMasterObj.name,ref hitSplitDetector);
			if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(reqWord.getWordAsString(),true,"",reqWord.languageArea,reqWord.difficulty); }

			
			// If this is the final split in the word then trigger the transformation sequence.
			List<List<string>> wordSegmentExistenceLists = getExistenceOfWordSegments(para_masterObjName);
			List<string> existingSegments = wordSegmentExistenceLists[0];
			List<string> nonExistingSegments = wordSegmentExistenceLists[1];
			
			bool completeStatus = (nonExistingSegments.Count == 0);
			if(completeStatus)
			{
				LineDragActiveNDraw swipeMang = GameObject.Find("LineMang").GetComponent<LineDragActiveNDraw>();
				swipeMang.setPause(true);
				
				
				
				orderedFullySplitConvObjNames = existingSegments;
				nxtIndexForOp = 0;
				
				// Tmp Switch Off Conveyor Timer.
				//conveyorTimerOn = false;

				getChopTimerScript().setPause(true);


				if( !isInAutoSplitMode)
				{
					// Record outcome for this config.
					numCorrectWordSplits++;
					buildNRecordConfigOutcome(new object[] { true });
				}

				performNextHighlightSequence();
			}
		}
	}
	
	
	private List<List<string>> getExistenceOfWordSegments(string para_masterObjName)
	{
		string wordID = para_masterObjName.Split(':')[0];
		string wDataFullKey = blockToWDataMap[wordID];
		SJWordItem reqWord = findWordInCache(int.Parse(wDataFullKey));
		string wordStr = reqWord.getWordAsString();
		
		int[] databaseSyllPositions = reqWord.getSyllSplitPositions();
		List<int> syllSplitPositions = new List<int>();
		for(int i=0; i<databaseSyllPositions.Length; i++)
		{
			syllSplitPositions.Add(databaseSyllPositions[i]);
		}
		syllSplitPositions.Add(wordStr.Length-1);
		
		
		List<string> existingSegments = new List<string>();
		List<string> nonExistingSegments = new List<string>();
		
		int startLetterIndex = 0;
		for(int i=0; i<syllSplitPositions.Count; i++)
		{
			int endLetterIndex = syllSplitPositions[i];
			
			string segmentUniqueID = wordID + ":" + "" + startLetterIndex + "-" + endLetterIndex;
			
			if(tetrisInfMap.ContainsKey(segmentUniqueID))
			{
				if(! existingSegments.Contains(segmentUniqueID))
				{
					existingSegments.Add(segmentUniqueID);
				}
			}
			else
			{
				if(! nonExistingSegments.Contains(segmentUniqueID))
				{
					nonExistingSegments.Add(segmentUniqueID);
				}
			}
			
			startLetterIndex = endLetterIndex + 1;
		}
		
		
		List<List<string>> retData = new List<List<string>>();
		retData.Add(existingSegments);
		retData.Add(nonExistingSegments);
		return retData;
	}
	
	private void performNextHighlightSequence()
	{
		string para_segmentID = orderedFullySplitConvObjNames[nxtIndexForOp];
		
		GameObject reqSeg = GameObject.Find(para_segmentID);
		
		CustomAnimationManager caMang = reqSeg.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> cmdBatchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> cmdBatch1 = new List<AniCommandPrep>();
		cmdBatch1.Add(new AniCommandPrep("ColorTransition",1, new List<System.Object>() { new float[4] {0,1,0,1}, 0.25f }));
		List<AniCommandPrep> cmdBatch2 = new List<AniCommandPrep>();
		cmdBatch2.Add(new AniCommandPrep("ColorTransition",1, new List<System.Object>() { new float[4] {1,1,1,1}, 0.25f }));
		cmdBatchList.Add(cmdBatch1);
		cmdBatchList.Add(cmdBatch2);
		caMang.init("HighlightSequence",cmdBatchList);
		caMang.registerListener("AcScen",this);
		
		nxtIndexForOp++;
	}



	private void createRandomStartGrid()
	{
		int rowsFromBottomToFill = 3;
		int maxSyllLength = 5;
		
		float[] whiteSpaceProbPerRow = new float[3] { 0.1f, 0.1f, 0.1f };
		
		for(int tmpRCounter=0; tmpRCounter<rowsFromBottomToFill; tmpRCounter++)
		{
			bool rowHasWhite = false;
			List<string> rowBlocks = new List<string>();
			for(int c=0; c<gridWidthInCells; c++)
			{
				int maxBound = maxSyllLength;
				if((c+maxBound) > gridWidthInCells)
				{
					maxBound = gridWidthInCells - c;
				}
				
				int nxtBlockSize = Random.Range(1,maxBound);
				if(nxtBlockSize > 0)
				{
					bool isWhiteSpace = (Random.Range(0,1.0f) <= whiteSpaceProbPerRow[tmpRCounter]);
					
					if(!isWhiteSpace)
					{
						//Debug.Log("Creating Brick at: {"+c+","+((gridHeightInCells+numPrequelRows)-1-tmpRCounter)+"}");
						string nwRBlock = createIndivisibleAtGridLocation(new int[2] {c,(gridHeightInCells+numPrequelRows)-1 - tmpRCounter},nxtBlockSize);
						rowBlocks.Add(nwRBlock);
					}
					else
					{
						rowHasWhite = true;
					}
					
					c += nxtBlockSize;
				}
			}
			
			if( ! rowHasWhite)
			{
				int randIndex = Random.Range(0,rowBlocks.Count);
				destroyAndDeregisterBlock(rowBlocks[randIndex]);
			}
		}
	}

	private void destroyAndDeregisterBlock(string para_existingBlockName)
	{
		GameObject reqObj = GameObject.Find(para_existingBlockName);
		if(reqObj != null)
		{
			// Destroy from world.
			Destroy(reqObj);
			
			
			// Update Datastructures.
			TetrisInfo tmpTInf = tetrisInfMap[para_existingBlockName];
			numIDToStrIDMap.Remove(tmpTInf.id);
			
			int[] gCoords = tmpTInf.coords;
			for(int c=gCoords[0]; c<(gCoords[0] + tmpTInf.wordSize); c++)
			{
				if((c >= 0)&&(c < gridWidthInCells))
				{
					gridState[c,gCoords[1]] = 0;
					debugGridState[c,gCoords[1]] = "0";
				}
			}
			tetrisInfMap.Remove(para_existingBlockName);
			
			if(droppingWords.Contains(para_existingBlockName))
			{
				droppingWords.Remove(para_existingBlockName);
			}
		}
	}


	private string createIndivisibleAtGridLocation(int[] para_startLetterCoords, int para_length)
	{
		// Create new name and key.
		int assignedID = nxtWordID;
		string blkName = "Block-"+assignedID+":"+"0-"+(para_length-1);
		nxtWordID++;
		
		
		// Spawn the object.
		float boundsWidth = (para_length * dropGrid_WorldGProp.cellWidth);
		float boundsHeight = dropGrid_WorldGProp.cellHeight;
		Rect spawnWorldBounds = new Rect(dropGrid_WorldGProp.x + ((dropGrid_WorldGProp.borderThickness + dropGrid_WorldGProp.cellWidth) * para_startLetterCoords[0]),
		                                 dropGrid_WorldGProp.y - ((dropGrid_WorldGProp.borderThickness + dropGrid_WorldGProp.cellHeight) * para_startLetterCoords[1])
		                         							   + ((dropGrid_WorldGProp.borderThickness + dropGrid_WorldGProp.cellHeight) * 1),
		                                 boundsWidth,
		                                 boundsHeight);
		
		Transform reqSyllPrefab = selectRandomPrefabForSyll(para_length);
		
		GameObject nwIndiObj = WorldSpawnHelper.initObjWithinWorldBounds(reqSyllPrefab,
		                                                                 reqSyllPrefab.renderer.bounds.size.x,
		                                                                 reqSyllPrefab.renderer.bounds.size.y,
		                                                                 blkName,
		                                                                 spawnWorldBounds,
		                                                                 null,
		                                                                 dropGrid_WorldGProp.z,
		                                                                 upAxisArr);

		//WorldSpawnHelper.initObjWithinWorldBounds(planePrefab,"DebugPlane",spawnWorldBounds,dropGrid_WorldGProp.z,upAxisArr);
		
		nwIndiObj.tag = "Indivisible";
		SpriteRenderer tmpSR = nwIndiObj.GetComponent<SpriteRenderer>();
		tmpSR.sortingOrder = 6;
		tmpSR.sprite.texture.wrapMode = TextureWrapMode.Clamp;
		
		applyMovableIndivisibleEffect(nwIndiObj);
		
		
		
		// Update Datastructures.
		tetrisInfMap.Add(blkName,new TetrisInfo(assignedID,new int[2] {para_startLetterCoords[0],para_startLetterCoords[1]},para_length,true));
		numIDToStrIDMap.Add(assignedID,blkName);
		
		for(int c=para_startLetterCoords[0]; c<(para_startLetterCoords[0] + para_length); c++)
		{
			gridState[c,para_startLetterCoords[1]] = assignedID;
			debugGridState[c,para_startLetterCoords[1]] = ""+assignedID;			
		}
		
		droppingWords.Add(blkName);
		
		return nwIndiObj.name;
	}


	private void performIndivisibleSpawn(GameObject para_objToChange)
	{

		TetrisInfo tInf = tetrisInfMap[para_objToChange.name];
		int syllLength = tInf.wordSize;

		if(syllLength >= gridWidthInCells)
		{
			//int overflowLength = gridWidthInCells - syllLength;
			//tInf.coords[0] = 3;
			syllLength = 6;
		}


		Vector3 objCentrePos = new Vector3(para_objToChange.transform.position.x,
		                                   					  para_objToChange.transform.position.y,
		                                   					  para_objToChange.transform.position.z);


		// Determine if this is a moveable or fixed block.
		
		string prefix = para_objToChange.name.Split(':')[0];
		string wDataFullKey = blockToWDataMap[prefix];
		SJWordItem reqFullWord = findWordInCache(int.Parse(wDataFullKey));
		
		
		string[] blockLetterRangeStrArr = (para_objToChange.name.Split(':')[1]).Split('-');
		int[] blockLetterRange = new int[2] { int.Parse(blockLetterRangeStrArr[0]), int.Parse(blockLetterRangeStrArr[1]) };
		
		int lastLetterIDForWord = blockLetterRange[1];
		int boundarySplitToCheck = lastLetterIDForWord;
		if(lastLetterIDForWord == (reqFullWord.getWordLength()-1))
		{
			boundarySplitToCheck = blockLetterRange[0] - 1;
		}

		bool isFixedBlock = (splitsDoneByGame.Contains(boundarySplitToCheck));

		destroyAndDeregisterBlock(para_objToChange.name);
		//Destroy(para_objToChange);



		// Create indivisibles to replace the object.

		int maxIndivisibleLength = 4;
		List<int[]> itemStartCoords = new List<int[]>();
		List<int> itemLengths = new List<int>();
		//List<string> itemNames = new List<string>();
		if(syllLength <= maxIndivisibleLength)
		{
			itemStartCoords.Add(tInf.coords);
			itemLengths.Add(tInf.wordSize);
		}
		else
		{
			int[] tmpCoords = new int[2];
			tmpCoords[0] = tInf.coords[0];
			tmpCoords[1] = 0;
			int remLength = syllLength;

			while(remLength > 0)
			{
				int maxL = 2;// maxIndivisibleLength;
				if(remLength < maxL) { maxL = remLength; }

				int chosenLength = Random.Range(1,(maxL+1));

				itemStartCoords.Add(new int[2] {tmpCoords[0],tmpCoords[1]});
				itemLengths.Add(chosenLength);

				tmpCoords[0] += chosenLength;
				remLength -= chosenLength;
			}
		}


		for(int i=0; i<itemStartCoords.Count; i++)
		{
			string nwItemName = createIndivisibleAtGridLocation(itemStartCoords[i],itemLengths[i]);
			GameObject nwIndiObj = GameObject.Find(nwItemName);

			if(isFixedBlock) { applyFixedIndivisibleEffect(nwIndiObj); }
			else { applyMovableIndivisibleEffect(nwIndiObj); }
		}	
		
		
		// Apply sound effect and cloud prefab.
		triggerSoundAtCamera("sfx_BlockToObject");
		if(blockToObjectEffect != null)
		{
			Vector3 effectSpawnPt = new Vector3(objCentrePos.x,objCentrePos.y,objCentrePos.z - 0.5f);
			Transform nwEffect = (Transform) Instantiate(blockToObjectEffect,effectSpawnPt,Quaternion.identity);
			nwEffect.localEulerAngles = new Vector3(blockToObjectEffect.localEulerAngles.x,
			                                        blockToObjectEffect.localEulerAngles.y,
			                                        blockToObjectEffect.localEulerAngles.z);
		}
		
		
	}

	private void applyMovableIndivisibleEffect(GameObject para_indiObj)
	{
		if(para_indiObj.GetComponent<BoxCollider>() == null) { para_indiObj.AddComponent<BoxCollider>(); }
	}
	
	private void applyFixedIndivisibleEffect(GameObject para_indiObj)
	{
		// Mark this object as wrong, i.e. immovable.
		SplittableBlockUtil.applyColorToBlock(para_indiObj,unmovableBlockColor);
		
		// Remove collider to make this object immovable by the player.
		if(para_indiObj.GetComponent<BoxCollider>() != null) { Destroy(para_indiObj.GetComponent<BoxCollider>()); }
		if(para_indiObj.GetComponent<MeshCollider>() != null) { Destroy(para_indiObj.GetComponent<MeshCollider>()); }
	}

	private Transform selectRandomPrefabForSyll(int para_syllLength)
	{
		bool isUsingDefault = false;
		Transform[] tmpArr = null;
		
		switch(para_syllLength)
		{
		case 1: tmpArr = oneSyllPrefabs; break;
		case 2: tmpArr = twoSyllPrefabs; break;
		case 3: tmpArr = threeSyllPrefabs; break;
		case 4: tmpArr = fourSyllPrefabs; break;
		case 5: tmpArr = fiveSyllPrefabs; break;
		default: isUsingDefault = true; break;
		}
		
		if(isUsingDefault)
		{
			return planePrefab;
		}
		else
		{
			int chosenTexIndex = Random.Range(0,tmpArr.Length);
			return tmpArr[chosenTexIndex];
		}
	}


	// Handlers for player block movement.
	public void intakeObjectDragStart()
	{
		LineDragActiveNDraw ldand = GameObject.Find("LineMang").GetComponent<LineDragActiveNDraw>();
		ldand.setPause(true);
	}
	
	public void intakeObjectDragCommand(ref GameObject para_indivisibleGObj, int para_dragCommand)
	{
		
		if(pause)
		{
			return;	
		}
		
		
		if( ! conveyorWordGObjLookup.Contains(para_indivisibleGObj.name))
		{
			
			TetrisInfo tInfo = tetrisInfMap[para_indivisibleGObj.name];
			
			bool canPerformOp = true;
			
			DPadDir cmd = (DPadDir) para_dragCommand;
			
			switch(cmd)
			{
			case DPadDir.NORTH:	// North.
				
				if(tInfo.coords[1] > 0)
				{
					int tmpX = tInfo.coords[0];
					int tmpY = tInfo.coords[1]-1;
					
					for(int i=tmpX; i<(tmpX + tInfo.wordSize); i++)
					{
						if((gridState[i,tmpY] != 0)||(tmpY < numPrequelRows))
						{
							canPerformOp = false;
							break;
						}
					}
					
					if(canPerformOp)
					{
						Vector3 movUpV = new Vector3(0,(dropGrid_WorldGProp.cellHeight + dropGrid_WorldGProp.borderThickness),0);
						para_indivisibleGObj.transform.position += movUpV;
						
						for(int x=tmpX; x<(tmpX + tInfo.wordSize); x++)
						{
							gridState[x,tInfo.coords[1]] = 0;
							gridState[x,tmpY] = tInfo.id;
							
							debugGridState[x,tInfo.coords[1]] = "0";
							debugGridState[x,tmpY] = ""+tInfo.id;
						}
						
						tInfo.coords[1] = tmpY;
						tetrisInfMap[para_indivisibleGObj.name] = tInfo;
						//updateStationaryBlocks = true;
					}
					else
					{
						break;
					}
				}
				break;
				
				
				
			case DPadDir.SOUTH: 
				
				if(tInfo.coords[1] < (gridState.GetLength(1)-1))
				{
					int tmpX = tInfo.coords[0];
					int tmpY = tInfo.coords[1]+1;
					
					for(int i=tmpX; i<(tmpX + tInfo.wordSize); i++)
					{
						if((gridState[i,tmpY] != 0)||(tmpY < numPrequelRows))
						{
							canPerformOp = false;
							break;
						}
					}
					
					if(canPerformOp)
					{
						Vector3 movDownV = new Vector3(0,-(dropGrid_WorldGProp.cellHeight + dropGrid_WorldGProp.borderThickness),0);
						para_indivisibleGObj.transform.position += movDownV;
						
						for(int x=tmpX; x<(tmpX + tInfo.wordSize); x++)
						{
							gridState[x,tInfo.coords[1]] = 0;
							gridState[x,tmpY] = tInfo.id;
							
							debugGridState[x,tInfo.coords[1]] = "0";
							debugGridState[x,tmpY] = ""+tInfo.id;
						}
						
						tInfo.coords[1] = tmpY;
						tetrisInfMap[para_indivisibleGObj.name] = tInfo;
						//updateStationaryBlocks = true;
					}
					else
					{
						break;
					}
				}
				break;
				
				
			case DPadDir.WEST:
				
				if(tInfo.coords[0] > 0)
				{
					int tmpX = tInfo.coords[0];
					int tmpY = tInfo.coords[1];
					
					if((gridState[tmpX-1,tmpY] != 0)||(tmpY < numPrequelRows))
					{
						canPerformOp = false;
					}
					
					
					
					if(canPerformOp)
					{
						Vector3 movWestV = new Vector3(-(dropGrid_WorldGProp.cellHeight + dropGrid_WorldGProp.borderThickness),0,0);
						para_indivisibleGObj.transform.position += movWestV;
						
						gridState[tmpX + tInfo.wordSize -1,tmpY] = 0;
						gridState[tmpX-1,tmpY] = tInfo.id;
						
						debugGridState[tmpX + tInfo.wordSize -1,tmpY] = "0";
						debugGridState[tmpX-1,tmpY] = ""+tInfo.id;
						
						
						tInfo.coords[0]--;
						tetrisInfMap[para_indivisibleGObj.name] = tInfo;
						//updateStationaryBlocks = true;
					}
					else
					{
						break;
					}
				}
				break;
				
			case DPadDir.EAST:
				
				int nwX = tInfo.coords[0]+tInfo.wordSize;
				
				if(nwX <= (gridState.GetLength(0)-1))
				{
					int tmpX = tInfo.coords[0];
					int tmpY = tInfo.coords[1];
					
					
					if((gridState[nwX,tmpY] != 0)||(tmpY < numPrequelRows))
					{
						canPerformOp = false;	
					}
					
					
					
					if(canPerformOp)
					{
						Vector3 movEastV = new Vector3((dropGrid_WorldGProp.cellHeight + dropGrid_WorldGProp.borderThickness),0,0);
						para_indivisibleGObj.transform.position += movEastV;
						
						gridState[tmpX,tmpY] = 0;
						gridState[nwX,tmpY] = tInfo.id;
						
						debugGridState[tmpX,tmpY] = "0";
						debugGridState[nwX,tmpY] = ""+tInfo.id;
						
						
						tInfo.coords[0]++;
						tetrisInfMap[para_indivisibleGObj.name] = tInfo;
						//updateStationaryBlocks = true;
					}
					else
					{
						break;
					}
				}
				break;
				
				
				
			}
			
			
		}
	}
	
	public void intakeObjectDragStop(string para_blockReleased)
	{
		if(pause)
		{
			return;	
		}
		
		stationaryWords.Remove(para_blockReleased);
		droppingWords.Add(para_blockReleased);
		
		LineDragActiveNDraw ldand = GameObject.Find("LineMang").GetComponent<LineDragActiveNDraw>();
		ldand.setPause(false);
		
		updateStationaryBlocks = true;
	}

	public void noticeHooveTermination()
	{
		Destroy(GameObject.Find("SolomonWhole").GetComponent(typeof(HooveScript)));
		Destroy(GameObject.Find("HooveRowHighlighter"));
		updateStationaryBlocks = true;
		pause = false;

		updateLineCounterDisplay();
		
		LineDragActiveNDraw swipeMang = GameObject.Find("LineMang").GetComponent<LineDragActiveNDraw>();
		swipeMang.setPause(false);
	}

	private void updateLineCounterDisplay()
	{
		TextMesh tMesh = getLineCounterTMesh();
		tMesh.text = ""+numHooveredLines;
		tMesh.renderer.sortingOrder = 100;
		tMesh.renderer.material.color = Color.black;
	}

	private void setWhistleVisibility(bool para_state)
	{
		whistleVisible = para_state;

		if(uiBounds == null) { uiBounds = new Dictionary<string, Rect>(); }
		GameObject whistleIcon = GameObject.Find("PitRight").transform.FindChild("WhistleIcon").gameObject;
		whistleIcon.renderer.enabled = para_state;
		whistleIcon.SetActive(para_state);
		Rect whistleIconGUIBounds = WorldSpawnHelper.getWorldToGUIBounds(whistleIcon.renderer.bounds,upAxisArr);
		if(uiBounds.ContainsKey("WhistleIcon")) { uiBounds["WhistleIcon"] = whistleIconGUIBounds; } else { uiBounds.Add("WhistleIcon",whistleIconGUIBounds); }
	}

	private void spawnSkeletonSwipeHand(Vector3 para_swipeStartPt, Vector3 para_swipeDestPt)
	{
		float skeleSwipeHand_Width = dropGrid_WorldGProp.cellWidth * 3;
		float skeleSwipeHand_Height = skeleSwipeHand_Width;
		float skeleSwipeHand_X = para_swipeStartPt.x;
		float skeleSwipeHand_Y = para_swipeStartPt.y;
		
		GameObject skeleSwipeHandObj = WorldSpawnHelper.initObjWithinWorldBounds(skeletonSwipeHandPrefab,
		                                                                         skeletonSwipeHandPrefab.renderer.bounds.size.x,
		                                                                         skeletonSwipeHandPrefab.renderer.bounds.size.y,
		                                                                         "SkeletonSwipeHand",
		                                                                         new Rect (skeleSwipeHand_X,skeleSwipeHand_Y,skeleSwipeHand_Width,skeleSwipeHand_Height),
		                                                                         null,
		                                                                         para_swipeStartPt.z,
		                                                                         upAxisArr);
		GameObject skeleFingerTipObj = (skeleSwipeHandObj.transform.FindChild("FingerTip")).gameObject;
		LineFollowActiveNDraw lfand = skeleFingerTipObj.GetComponent<LineFollowActiveNDraw>();
		lfand.init(WorldSpawnHelper.getWorldToGUIBounds(GameObject.Find("Background").renderer.bounds,upAxisArr),0.1f,dropGrid_WorldGProp.z - (dropGrid_WorldGProp.cellWidth * 1.75f),dropGrid_WorldGProp.z,"SplitDetect",0);
		lfand.registerListener("AcScen",this);
		MoveToLocation skeleMovScript = skeleSwipeHandObj.AddComponent<MoveToLocation>();
		skeleMovScript.initScript(new Vector3(para_swipeDestPt.x + (skeleSwipeHandObj.renderer.bounds.size.x/2f),
		                                      para_swipeDestPt.y - (skeleSwipeHandObj.renderer.bounds.size.y/2f),
		                                      para_swipeDestPt.z),
		                          2f);
		skeleMovScript.registerListener("AcScen",this);
	}
	
	
	private void triggerAutoSwipeSequence(string para_blockNamePrefix)
	{
		splitsDoneByGame = new List<int>();
		isInAutoSplitMode = true;
		itemForAutoSplit = para_blockNamePrefix;
		string wDatabaseFullKey = blockToWDataMap[itemForAutoSplit];
		SJWordItem reqWord = findWordInCache(int.Parse(wDatabaseFullKey));
		
		int[] tmpSyllSplitArr = reqWord.getSyllSplitPositions();
		if(syllsForAutoSplit == null) { syllsForAutoSplit = new List<int>(); } else { syllsForAutoSplit.Clear(); }
		for(int k=0; k<tmpSyllSplitArr.Length; k++)
		{
			syllsForAutoSplit.Add(tmpSyllSplitArr[k]);
		}
		if(syllsForAutoSplit.Count > 0)
		{
			performNextAutoSwipe();
		}
	}
	
	private void performNextAutoSwipe()
	{
		bool foundBlockToSplit = false;
		
		
		while((foundBlockToSplit == false)&&(syllsForAutoSplit.Count > 0))
		{
			int nxtTargetSplitDetID = syllsForAutoSplit[0];
			
			foreach(string tmpConvWordID in conveyorWordGObjLookup)
			{
				string[] suffix = (tmpConvWordID.Split(':')[1]).Split('-');
				int[] suffixNums = new int[2] { int.Parse(suffix[0]), int.Parse(suffix[1]) };
				
				if((nxtTargetSplitDetID >= suffixNums[0])&&(nxtTargetSplitDetID < suffixNums[1]))
				{
					Bounds splitDetBounds = SplittableBlockUtil.getSplitDetectWorldBounds(tmpConvWordID,nxtTargetSplitDetID);
					
					float zVal = Camera.main.transform.position.z + 2f;
					Rect topOfPitWorld = CommonUnityUtils.get2DBounds(GameObject.Find("TopOfPit").renderer.bounds);


					Vector3 swipeStartPos = new Vector3(splitDetBounds.min.x + (splitDetBounds.size.x * 0.25f),topOfPitWorld.y - topOfPitWorld.height,zVal);
					Vector3 swipeEndPos = new Vector3(splitDetBounds.min.x + (splitDetBounds.size.x * 0.75f),topOfPitWorld.y,zVal);
					spawnSkeletonSwipeHand(swipeStartPos,swipeEndPos);
					
					foundBlockToSplit = true;
					splitsDoneByGame.Add(nxtTargetSplitDetID);
					
					break;
				}
			}
			
			syllsForAutoSplit.RemoveAt(0);
		}
	}

	private void resetAttempts()
	{
		wordUnattempted = true;
		for(int i=0; i<conWordAttemptsStates.Length; i++)
		{
			conWordAttemptsStates[i] = false;
		}
	}

	private ScoreBoardScript getScoreBoardScript()
	{
		ScoreBoardScript sbs = transform.GetComponent<ScoreBoardScript>();
		if(sbs == null) { sbs = transform.gameObject.AddComponent<ScoreBoardScript>(); } else { sbs.enabled = true; }
		return sbs;
	}

	private	DeliveryChuteScript getDeliveryChuteScript()
	{
		DeliveryChuteScript dcs = transform.GetComponent<DeliveryChuteScript>();
		if(dcs == null) { dcs = transform.gameObject.AddComponent<DeliveryChuteScript>(); } else { dcs.enabled = true; }
		dcs.registerListener("AcScen",this);
		return dcs;
	}

	private CircleTimerScript getChopTimerScript()
	{
		CircleTimerScript reqScript = null;
		Transform wordChopTimer = GameObject.Find("WordChopTimer").transform;
		if(wordChopTimer != null)
		{
			reqScript = wordChopTimer.FindChild("Timer").GetComponent<CircleTimerScript>();
			if(reqScript != null)
			{
				reqScript.enabled = true;
				reqScript.registerListener("AcScen",this);
			}
		}
		return reqScript;
	}

	private TextMesh getLineCounterTMesh()
	{
		TextMesh retScript = null;
		GameObject solomonGObj = GameObject.Find("SolomonWhole");
		if(solomonGObj != null)
		{
			retScript = solomonGObj.transform.FindChild("LineCounter").GetComponent<TextMesh>();
		}
		return retScript;
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

	protected new void loadTextures()
	{
		availableTextures = new Dictionary<string, Texture2D>();
		
		Texture2D progressBarTex = new Texture2D(1,1);
		progressBarTex.SetPixel(1,1,Color.green);
		progressBarTex.Apply();

		Texture2D progressBarTexDark = new Texture2D(1,1);
		progressBarTexDark.SetPixel(1,1,new Color(0,0.2f,0));
		progressBarTexDark.Apply();
		
		availableTextures.Add("PortraitProgFore",progressBarTex);
		availableTextures.Add("PortraitProgForeDark",progressBarTexDark);
	}

	private SJWordItem findWordInCache(int para_id)
	{
		SJWordItem retData = null;
		if(wordItemCache.ContainsKey(para_id))
		{
			retData = wordItemCache[para_id];
		}
		return retData;
	}


	class TetrisInfo
	{
		public int id;
		public int[] coords;
		public int wordSize;
		public bool isIndivisible;
		
		
		public TetrisInfo(int para_id, int[] para_startingCoords, int para_wordSize, bool para_isIndivisible)
		{
			id = para_id;
			coords = para_startingCoords;
			wordSize = para_wordSize;
			isIndivisible = para_isIndivisible;
		}
	}
	
	class WordSegmentComparer : System.Collections.Generic.IComparer<string>
	{
		public int Compare(string x, string y)
		{
			string[] x_suffixParts = ((x.Split(':')[1]).Split('-'));
			string[] y_suffixParts = ((y.Split(':')[1]).Split('-'));
			
			int[] x_parsedLetterRange = new int[2] { int.Parse(x_suffixParts[0]), int.Parse(x_suffixParts[1]) };
			int[] y_parsedLetterRange = new int[2] { int.Parse(y_suffixParts[0]), int.Parse(y_suffixParts[1]) };
			
			
			if(x_parsedLetterRange[1] < y_parsedLetterRange[0])
			{
				return -1;
			}
			else if(x_parsedLetterRange[0] > y_parsedLetterRange[1])
			{
				return 1;
			}
			else
			{
				return 0;
			}
			
		}
	}


}

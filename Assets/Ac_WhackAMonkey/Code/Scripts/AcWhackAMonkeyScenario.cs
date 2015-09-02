/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class AcWhackAMonkeyScenario : ILearnRWActivityScenario, CustomActionListener
{

	public Transform wordBoxPrefab;
	public Transform bananaPrefab;


	WAMLevelConfig currLvlConfig;
	bool metaDataLoaded = false;


	int numHoles;
	int maxMolesAtATime;
	int numActiveMoles;
	List<int> availableHoles;

	int maxBananans = 5;
	
	int leftBananas;

	float correctWordProbability;

	int minStayDuration_Sec;
	int maxStayDuration_Sec;

	List<AbsInputDetector> inputDetectors;

	bool[] upAxisArr;


	bool paused;

	bool winLockDown = false;

	Dictionary<int,bool> holeIsCorrectMap;

	int numCorrectHits;
	int reqHitsForWin = 10;


	int numFedCorrectMonkies = 0;
	int numMissedCorrectMonkies = 0;
	int numFedWrongMonkies = 0;
	int numWalls = 0;
	float startTimestamp = 0;
	float endTimestamp = 0;

	WAMLevelOutcome lvlOutcome;

	bool isFirstConfig = true;

	int descriptionType = 0;
	// 0 = text only. 1 = sound only.
	bool descriptionTileButtonOn = false;
	bool hasRegisteredIntro = false;


	bool ignoreFirst = true;

	void Start()
	{
		acID = ApplicationID.WHAK_A_MOLE;
		metaDataLoaded = loadActivitySessionMetaData();

		loadTextures();
		prepUIBounds();
		
		initWorld();
		this.initLevelConfigGenerator();
		genNextLevel();

		initPersonHelperPortraitPic();
		showInfoSlideshow();
		recordActivityStart();
		pauseScene(true);

	}


	void Update()
	{
		if(isInitialised)
		{
			if( ! paused)
			{
				if((numActiveMoles < maxMolesAtATime)&&(availableHoles.Count > 0)&&(correctWords.Count>0))
				{
					spawnMonkey();
				}
			}
		}
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

				if((descriptionTileButtonOn)&&(descriptionType == 1))
				{
					//GUI.color = Color.white;
					if(GUI.Button(uiBounds["GoalBox"],""))
					{
						try
						{
							if(gameObject.GetComponent<AudioSource>() == null) { gameObject.AddComponent<AudioSource>(); }
							audio.PlayOneShot(WorldViewServerCommunication.tts.say(currLvlConfig.descriptionLabel));
						}
						catch(System.Exception ex)
						{
							Debug.LogError("Failed to use TTS. "+ex.Message);
						}
					}
				}
			}
		}
		
		GUI.color = Color.white;
	}
	
	GameObject tmpBananas;
	protected override void initWorld()
	{
		// Initialisation of necessary world items.


		// Input Detectors.
		inputDetectors = new List<AbsInputDetector>();
		ClickDetector cd = transform.gameObject.AddComponent<ClickDetector>();
		cd.registerListener("AcScen",this);
		inputDetectors.Add(cd);

		// Auto Adjust.
		GameObject tmpWordDisplay = GameObject.Find("WordDisplay");
		GameObject tmpPersonPortrait = GameObject.Find("PersonPortrait");
		GameObject tmpPauseButton = GameObject.Find("PauseButton");
		GameObject tmpBackdrop = GameObject.Find("WhackaMonkey_Backdrop");
		tmpBananas = GameObject.Find("Bananas");


		SpawnNormaliser.adjustGameObjectsToNwBounds(SpawnNormaliser.get2DBounds(tmpBackdrop.renderer.bounds),
		                                            WorldSpawnHelper.getCameraViewWorldBounds(tmpBackdrop.transform.position.z,true),
		                                            new List<GameObject>() { tmpWordDisplay, tmpPersonPortrait, tmpPauseButton,tmpBananas });


		tmpPersonPortrait.transform.parent = GameObject.Find("Main Camera").transform;
		tmpPauseButton.transform.parent = GameObject.Find("Main Camera").transform;
		
		uiBounds.Add("PersonPortrait",WorldSpawnHelper.getWorldToGUIBounds(tmpPersonPortrait.renderer.bounds,upAxisArr));
		uiBounds.Add("PauseBtn",WorldSpawnHelper.getWorldToGUIBounds(tmpPauseButton.renderer.bounds,upAxisArr));
		uiBounds.Add("GoalBox",WorldSpawnHelper.getWorldToGUIBounds(tmpWordDisplay.renderer.bounds,upAxisArr));


		// Initialisation of Mechanics Variables
		availableHoles = new List<int>();
		numHoles = 0;
		while(GameObject.Find("Monkey"+(numHoles+1)) != null)
		{
			GameObject tmpMonkey = GameObject.Find("Monkey"+(numHoles+1));
			MonkeyScript mScript = tmpMonkey.GetComponent<MonkeyScript>();
			mScript.registerListener("AcScen",this);

			numHoles++;
			availableHoles.Add(numHoles);
		}

		holeIsCorrectMap = new Dictionary<int, bool>();

		maxMolesAtATime = 1;

		correctWordProbability = 0.5f;
		minStayDuration_Sec = 5;
		maxStayDuration_Sec = 5;

		upAxisArr = new bool[] { false, true, false };


	}

	protected new bool initLevelConfigGenerator()
	{
		if( ! base.initLevelConfigGenerator())
		{
			// Fallback.
			lvlConfigGen = new WAMLevelConfigGeneratorServer(null); //new WAMLevelConfigGeneratorHardCoded();
			Debug.LogWarning("Warning: Using Level Gen Fallback");
		}
		return true;
	}

	protected override void genNextLevel()
	{
		if(! isFirstConfig)
		{
			buildNRecordConfigOutcome(null);
		}
		isFirstConfig = false;

		currLvlConfig = (WAMLevelConfig) lvlConfigGen.getNextLevelConfig(null);

		maxMolesAtATime = currLvlConfig.speed+1;
		minStayDuration_Sec = 5-currLvlConfig.speed;
		maxStayDuration_Sec = 5-currLvlConfig.speed;

		recordPresentedConfig(currLvlConfig);
		if(serverCommunication != null) { serverCommunication.wordDisplayed(currLvlConfig.descriptionLabel,currLvlConfig.languageArea,currLvlConfig.difficulty); }

		lvlOutcome = new WAMLevelOutcome(true);

		// Inidcates if tts should be used.
		descriptionType = (currLvlConfig.useTTS ? 1 : 0);

		leftBananas = maxBananans+currLvlConfig.correctItems.Length;

		if(leftBananas>36)
			leftBananas = 36;

		for(int i=0;i<leftBananas;i++){
			Transform banana = tmpBananas.transform.Find(i.ToString("D2"));
			banana.gameObject.renderer.enabled = true;
		}

		for(int i=leftBananas;i<36;i++){
			Transform banana = tmpBananas.transform.Find(i.ToString("D2"));
			banana.gameObject.renderer.enabled = false;
		}

		correctWords = new List<string>();
		foreach(string s in currLvlConfig.correctItems)
			correctWords.Add(s);

		wordsDisplayed = new List<string>();
		// Create goal board.
		setupGoalBoard();
	}	   

	List<string> correctWords;
	List<string> wordsDisplayed;

	private void spawnMonkey()
	{
		if((numActiveMoles < maxMolesAtATime)&&(availableHoles.Count > 0))
		{
			// Select Hole.
			int randIndex = Random.Range(0,availableHoles.Count);
			int reqHoleID = availableHoles[randIndex];
			string reqMonkeyName = "Monkey"+reqHoleID;
			availableHoles.RemoveAt(randIndex);


			//Select Word.
			string reqWord = "Dummy-Word";
			float randProb = Random.Range(0f,1f);
			bool wordIsCorrect = false;
			if(randProb < correctWordProbability)
			{
				wordIsCorrect = true;
				int idx = Random.Range(0,correctWords.Count);
				reqWord = correctWords[idx];
				correctWords.RemoveAt(idx);
				wordsDisplayed.Add(reqWord);
				//reqWord = currLvlConfig.correctItems[Random.Range(0,currLvlConfig.correctItems.Length)];
			}
			else
			{
				wordIsCorrect = false;
				reqWord = currLvlConfig.fillerItems[Random.Range(0,currLvlConfig.fillerItems.Length)];
			}
			if(holeIsCorrectMap.ContainsKey(reqHoleID)) { holeIsCorrectMap[reqHoleID] = wordIsCorrect; } else { holeIsCorrectMap.Add(reqHoleID,wordIsCorrect); }


			// Select Stay Duration.
			float reqStayDuration = Random.Range(minStayDuration_Sec,(maxStayDuration_Sec+1));


			// Trigger Monkey.
			GameObject reqMonkeyObj = GameObject.Find(reqMonkeyName);
			MonkeyScript ms = reqMonkeyObj.GetComponent<MonkeyScript>();

			ms.triggerMonkeyPopup(reqHoleID,reqWord,wordIsCorrect,reqStayDuration,upAxisArr,wordBoxPrefab);


			numActiveMoles++;
		}
	}

	private void spawnBanana(Vector2 para_clickPos)
	{
		lastBananaShot = Time.realtimeSinceStartup;
		leftBananas--;
		Transform banana = tmpBananas.transform.Find(leftBananas.ToString("D2"));
		banana.transform.renderer.enabled = false;

		Vector3 worldDestpt = Camera.main.ScreenToWorldPoint(new Vector3(para_clickPos.x,para_clickPos.y,1));
		worldDestpt.z = -0.01f;

		float destWidth = bananaPrefab.renderer.bounds.size.x * 0.2f;
		float destHeight = bananaPrefab.renderer.bounds.size.y * 0.2f;

		Vector3 bananaSpawnPt = new Vector3(worldDestpt.x,worldDestpt.y,Camera.main.transform.position.z + 0.1f);

		Transform nwBanana = (Transform) Instantiate(bananaPrefab,bananaSpawnPt,Quaternion.identity);
		nwBanana.renderer.sortingOrder = 900;
		BananaScript bScript = nwBanana.GetComponent<BananaScript>();
		bScript.registerListener("AcScen",this);
		bScript.init(bananaSpawnPt,worldDestpt,1f,new Rect(worldDestpt.x - (destWidth/2f),worldDestpt.y + (destHeight/2f),destWidth,destHeight));
	}

	public new void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		base.respondToEvent(para_sourceID,para_eventID,para_eventData);

		if((isInitialised)&&(! hasRegisteredIntro))
		{
			if(descriptionType == 1)
			{
				descriptionTileButtonOn = true;

			}
			startTimestamp = Time.time;
			hasRegisteredIntro = true;
		}

		if(para_sourceID == "InputDetector")
		{
			if(para_eventID == "ClickEvent")
			{
				if((leftBananas > 0)&&(!tooSoon())){

				System.Object[] eventData = (System.Object[]) para_eventData;
				float[] clickPosAsFloatArr = (float[]) eventData[0];
				//int numOfClicks = (int) eventData[1];

				Vector2 clickPos = new Vector2(clickPosAsFloatArr[0],clickPosAsFloatArr[1]);


				if((uiBounds["GoalBox"].Contains(new Vector2(clickPos.x,Screen.height-clickPos.y)) == false)&&
				   (uiBounds["PauseBtn"].Contains(new Vector2(clickPos.x,Screen.height-clickPos.y)) == false)&&
				    (uiBounds["PersonPortrait"].Contains(new Vector2(clickPos.x,Screen.height-clickPos.y)) == false))
				{
					
						if(ignoreFirst)
							ignoreFirst = false;
						else
							spawnBanana(clickPos);
				}
				}
			}
		}
		else if(para_eventID == "MonkeyReturn")
		{
			List<System.Object> eventDataList = (List<System.Object>) para_eventData;
			int holeID = (int) eventDataList[0];
			string monkeyHeldWord = (string) eventDataList[1];
			bool isAMissedMonkey = (bool) eventDataList[2];

			if(isAMissedMonkey)
			{
				numMissedCorrectMonkies++;
				lvlOutcome.addMissedCorrectMonkey(monkeyHeldWord);

				correctWords.Add(monkeyHeldWord);
				wordsDisplayed.Remove(monkeyHeldWord);

			}




			numActiveMoles--;
			availableHoles.Add(holeID);
			
			if((numActiveMoles < maxMolesAtATime)&&(availableHoles.Count > 0)&&(correctWords.Count>0))
			{
				if(!paused)
					spawnMonkey();
			}
		}
		else if(para_eventID == "MonkeyAteBanana")
		{
			List<System.Object> eventDataList = (List<System.Object>) para_eventData;
			//int holeID = (int) eventDataList[0];
			string monkeyHeldWord = (string) eventDataList[1];

			wordsDisplayed.Remove(monkeyHeldWord);

			if( ! winLockDown)
			{
				numFedCorrectMonkies++;
				lvlOutcome.addFedCorrectMonkey(monkeyHeldWord);

				numCorrectHits++;
				if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(monkeyHeldWord,true,currLvlConfig.descriptionLabel,currLvlConfig.languageArea,currLvlConfig.difficulty); }

				won = checkNHandleWinningCondition();
				if(won)
				{
					buildNRecordConfigOutcome(null);
				}else{

					bool loseFlag = checkNHandleLosingCondition();
					if(loseFlag)
					{
						buildNRecordConfigOutcome(null);
					}
				}



			}
		}
		else if(para_eventID == "MonkeyRejectedBanana")
		{
			List<System.Object> eventDataList = (List<System.Object>) para_eventData;
			//int holeID = (int) eventDataList[0];
			string monkeyHeldWord = (string) eventDataList[1];

			if(monkeyHeldWord!=null){
				numFedWrongMonkies++;
				lvlOutcome.addFedIncorrectMonkey(monkeyHeldWord);

				if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(monkeyHeldWord,false,currLvlConfig.descriptionLabel,currLvlConfig.languageArea,currLvlConfig.difficulty); }

			
			}else{//Hit a closed window
				numWalls++;
			}

			won = false;
				
				bool loseFlag = checkNHandleLosingCondition();
				if(loseFlag)
				{
					buildNRecordConfigOutcome(null);
				}

		}else if(para_eventID == "BananaHitWall")
		{

			numWalls++;

			won = false;

				bool loseFlag = checkNHandleLosingCondition();
				if(loseFlag)
				{
					buildNRecordConfigOutcome(null);
				}
		}
	}


	protected override void pauseScene(bool para_pauseState)
	{
		paused = para_pauseState;

		for(int i=0; i<inputDetectors.Count; i++)
		{
			inputDetectors[i].enabled = ( ! paused);
		}
	}


	protected override bool checkNHandleWinningCondition()
	{

		updateActivityProgressMetaData((currLvlConfig.correctItems.Length-correctWords.Count-wordsDisplayed.Count)/(currLvlConfig.correctItems.Length*1f));
		if((correctWords.Count+wordsDisplayed.Count)==0)
		{
			winLockDown = true;
			endTimestamp = Time.time;
			pauseScene(true);
			performDefaultWinProcedure();
			return true;
		}

		return false;
	}


	double lastBananaShot = 0;
	private bool tooSoon(){

		if( (Time.realtimeSinceStartup-lastBananaShot)<0.3)
			return true;
		else
			return false;


	}

	protected override bool checkNHandleLosingCondition()
	{
		bool playerHasLost = (leftBananas<=0);
		
		if(playerHasLost)
		{
			endTimestamp = Time.time;
			pauseScene(true);
			performDefaultLoseProcedure();
		}

		return playerHasLost;
	}

	private void setupGoalBoard()
	{
		if(descriptionType == 1)
		{
			// Sound only.

			GameObject descriptionGObj = GameObject.Find("WordDisplay");
			Transform tmpChildPresent = descriptionGObj.transform.FindChild("WordBox");
			if(tmpChildPresent != null) { Destroy(tmpChildPresent.gameObject);	}

			descriptionGObj.transform.FindChild("TtsIcon").renderer.enabled = true;

			if(hasRegisteredIntro == false)
			{
				descriptionTileButtonOn = false;
			}
			else
			{
				descriptionTileButtonOn = true;

			}
		}
		else
		{
			// Text only.

			GameObject descriptionGObj = GameObject.Find("WordDisplay");
			
			Transform tmpChildPresent = descriptionGObj.transform.FindChild("WordBox");
			if(tmpChildPresent != null) { Destroy(tmpChildPresent.gameObject);	}

			descriptionGObj.transform.FindChild("TtsIcon").renderer.enabled = false;
			descriptionTileButtonOn = false;
			
			GameObject txtArea = (descriptionGObj.transform.FindChild("TextArea")).gameObject;
			Rect wBounds = new Rect(txtArea.renderer.bounds.center.x - (txtArea.renderer.bounds.size.x/2f),
			                        txtArea.renderer.bounds.center.y + (txtArea.renderer.bounds.size.y/2f),
			                        txtArea.renderer.bounds.size.x,
			                        txtArea.renderer.bounds.size.y);
			
			GameObject wBox = WordBuilderHelper.buildWordBox(0,currLvlConfig.descriptionLabel.Replace("/",""),wBounds,txtArea.transform.position.z,upAxisArr,wordBoxPrefab);
			WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() {wBox},0.1f);

			wBox.name = "WordBox";
			wBox.transform.parent = descriptionGObj.transform;
			Destroy(wBox.transform.FindChild("Board").gameObject);
			wBox.transform.FindChild("Text").renderer.sortingOrder = 510;
		}
	}

	bool won = false;
	protected override void buildNRecordConfigOutcome(System.Object[] para_extraParams)
	{
		// Build outcome object.
		
		// Trigger record outcome.
		recordOutcomeForConfig(lvlOutcome);
		lvlOutcome = new WAMLevelOutcome(won);
	}

	protected override GameyResultData buildGameyData()
	{
		float timeDiff = endTimestamp - startTimestamp;
		int minutes = (int) (timeDiff / 60f);
		int seconds = (int) (((timeDiff / 60f) - minutes) * 60f);

		WAMGameyResultData reqData = new WAMGameyResultData(numFedCorrectMonkies,numMissedCorrectMonkies,numFedWrongMonkies,minutes,seconds);
		return reqData;
	}

	protected override string generateGoalTextAppend()
	{
		return (" "+reqHitsForWin);
	}
}

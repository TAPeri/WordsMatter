/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class AcSerenadeHeroScenario : ILearnRWActivityScenario, CustomActionListener
{


    
	public Transform wordBoxPrefab;
	public Transform wordPoolWordBoxPrefab;
	public Transform holePrefab;
	public Transform sfxPrefab;
	public Transform debugPrefab;



	SHeroLevelConfig currLvlConfig;
	bool[] upAxisArr = new bool[3] {false,true,false};

	//float depth_sentenceWords = 4;
	//float depth_wordPoolWords = 3;

	
	bool paused = false;

	bool pauseBtnVisible = true;
	bool continueBtnVisible = true;

	bool metaDataLoaded = false;

	
	const int wordPoolMaxCapacity = 4;


	List<string> currLvlItems;
	List<Vector3> songBannerItemPositions = null;
	List<Vector3> wordPoolItemPositions = null;


	GameObject currObjBeingDragged;
	//Vector3 dragRestPosition;
	Vector3 songWordWorldScale;
	Vector3 poolWordWorldScale;


	//string debugFullSentence;

	//int musicMarkerIndex;
	float musicMarkerSpeedPerWord_InSec = 8f;


	float sentenceWordWidth;
	float sentenceWordSpacerWidth;

	//int markerUpdateCounter = 0;



	string[] characters = new string[5]{"Ser_Bass","Ser_Trump1","Ser_Trump2","Ser_Perc","Ser_Viol2"};
	List<Vector3> characterScenePositions;
	Dictionary<string,GameObject> characterGObjMap;
	int currCharacterCount;

	string[] windowReveals = new string[11] {null,null,"Window1","Window2","Window3","Window4","Window5","Window6","Window7","Window8","Window9"};



	int mistakesInCurrSentence;
	int mistakesForCharacterExit = 0;
	int counterToCharacterEnter = 0;
	const int badAttemptsUntilCharacterExit = 3;
	const int sentencesUntilNewCharacterEnter = 1;


	float songBannerFontCharacterSize;
	float wordPoolFontCharacterSize;

	//bool girlVisible = false;

	bool isPerformingMusicMarkerStartDelay = false;
	bool hasInitIntroSequence = false;


	List<AudioSource> scenAudSList;

	Dictionary<int,string> playerSelectedHoleItems;

	//float desiredSongBannerCharacterSize = 0.05f;
	float desiredWordPoolCharacterSize = 0.055f;//0.07f;


	int numCorrectSentences = 0;
	int numIncorrectTries = 0;
	//int finalMusPeepCount = 0;
	float startTimestamp = 0;
	float endTimestamp = 0;

	bool playerHasWon = false;

	int sentencesCompleted;
	int numSentencesDisplayed = 0;
	int reqMaxSentencesForWin; // Set via the config generator.



	void Start()
	{
		acID = ApplicationID.SERENADE_HERO;
		metaDataLoaded = loadActivitySessionMetaData();

		this.loadTextures();
		this.prepUIBounds();

		initWorld();
		//this.initLevelConfigGenerator();
		//genNextLevel();

		SoundOptionsDisplay sod = GameObject.Find("GlobObj").GetComponent<SoundOptionsDisplay>();
		sod.registerListener("AcScen",this);

		initPersonHelperPortraitPic();
		showInfoSlideshow();
		recordActivityStart();
	}





	void OnGUI()
	{
		if(isInitialised)
		{
			if( ! paused)
			{

				if( ! hasInitGUIStyles)
				{
					this.prepGUIStyles();					
					hasInitGUIStyles = true;
				}
				else
				{
					GUI.color = Color.clear;

					if(continueBtnVisible)
					{
						if(GUI.Button(uiBounds["ContinueBtn"],"Complete"))
						{
							continueBtnVisible = false;
							GameObject.Find("AutoCompleteButtonLocation").renderer.enabled = false;

							MoveToLocation mtl = GameObject.Find("MusicMarker").GetComponent<MoveToLocation>();
							Debug.Log("Marker, go to the end!");
							mtl.changeETASpeed(10f);
						}
					}


					if(pauseBtnVisible)
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

						if(GUI.Button(uiBounds["MusicBtn"],"")){

							if(musicOn){
								musicOn = false;
								GameObject btn = GameObject.Find("MusicButton");
								btn.renderer.material.color = Color.green;

								for(int i = 0; i<characters.Length;i++){
									string soundBoxName = characters[i] + "_SoundChannel";
									GameObject reqSoundBox = GameObject.Find(soundBoxName);
									if(reqSoundBox!=null){
										AudioSource audS = (AudioSource) reqSoundBox.GetComponent(typeof(AudioSource));
										audS.mute = true;
									}
								}
								string soundBoxNamePerm = "Ser_Viol1_SoundChannel";
								GameObject reqSoundBoxPerm = GameObject.Find(soundBoxNamePerm);
								if(reqSoundBoxPerm!=null){
									AudioSource audS = (AudioSource) reqSoundBoxPerm.GetComponent(typeof(AudioSource));
									audS.mute = true;
								}



							}else{
								musicOn = true;
								GameObject btn = GameObject.Find("MusicButton");
								btn.renderer.material.color = Color.red;
								for(int i = 0; i<currCharacterCount;i++){
									string soundBoxName = characters[i] + "_SoundChannel";
									GameObject reqSoundBox = GameObject.Find(soundBoxName);
									if(reqSoundBox!=null){
										AudioSource audS = (AudioSource) reqSoundBox.GetComponent(typeof(AudioSource));
										audS.mute = false;
									}
								}

								string soundBoxNamePerm = "Ser_Viol1_SoundChannel";
								GameObject reqSoundBoxPerm = GameObject.Find(soundBoxNamePerm);
								if(reqSoundBoxPerm!=null){
									AudioSource audS = (AudioSource) reqSoundBoxPerm.GetComponent(typeof(AudioSource));
									audS.mute = false;
								}

							}

						}
					}


					GUI.color = Color.white;
				}


			}
		}
	}

	bool musicOn = true;

	protected override void pauseScene(bool para_state)
	{
		GameObject tmpMM = GameObject.Find("MusicMarker");
		if(tmpMM != null)
		{
			MoveToLocation mtl = tmpMM.GetComponent<MoveToLocation>();

			if(mtl != null)
			{
				mtl.enabled = !para_state;
			}
		}

		DragScript dScript = (GameObject.Find("GlobObj").GetComponent<DragScript>());
		if(dScript != null)
		{
			dScript.setInputFlag(!para_state);
		}

		paused = para_state;
	}

	private void toggleTextVisibility(bool para_state)
	{
		for(int i=0; i<currLvlItems.Count; i++)
		{
			GameObject tmpLvlObj = GameObject.Find(currLvlItems[i]);
			if(tmpLvlObj != null)
			{
				Transform textChildObj = tmpLvlObj.transform.FindChild("Text");
				if(textChildObj != null)
				{
					textChildObj.gameObject.SetActive(para_state);
				}
			}
		}
	}


	protected override void initWorld()
	{

		this.initLevelConfigGenerator();
		reqMaxSentencesForWin = lvlConfigGen.getConfigCount();
		currLvlConfig = null;

		currLvlItems = new List<string>();

		


		// Auto Adjust.

		GameObject personPortrait = GameObject.Find("PersonPortrait");
		GameObject pauseBtn = GameObject.Find("PauseButton");
		GameObject continueBtn = GameObject.Find("AutoCompleteButtonLocation");
		GameObject bkgrnd = GameObject.Find("Background");
		GameObject musicBtn = GameObject.Find("MusicButton");

		SpawnNormaliser.adjustGameObjectsToNwBounds(SpawnNormaliser.get2DBounds(bkgrnd.renderer.bounds),
		                                            WorldSpawnHelper.getCameraViewWorldBounds(bkgrnd.transform.position.z + 2,true),
		                                            new List<GameObject>() { personPortrait, pauseBtn, continueBtn,musicBtn });

		Transform mainCamObj = GameObject.Find("Main Camera").transform;
		continueBtn.transform.parent = mainCamObj;
		personPortrait.transform.parent = mainCamObj;
		pauseBtn.transform.parent = mainCamObj;
		musicBtn.transform.parent = mainCamObj;
		musicBtn.renderer.material.color = Color.red;

		uiBounds.Add("ContinueBtn",WorldSpawnHelper.getWorldToGUIBounds(continueBtn.renderer.bounds,upAxisArr));
		uiBounds.Add("PersonPortrait",WorldSpawnHelper.getWorldToGUIBounds(personPortrait.renderer.bounds,upAxisArr));
		uiBounds.Add("PauseBtn",WorldSpawnHelper.getWorldToGUIBounds(pauseBtn.renderer.bounds,upAxisArr));		 
		uiBounds.Add("MusicBtn",WorldSpawnHelper.getWorldToGUIBounds(musicBtn.renderer.bounds,upAxisArr));		 

		pauseBtnVisible = false;
		pauseBtn.renderer.enabled = false;

		continueBtnVisible = false;
		continueBtn.renderer.enabled = false;


		// Adjust the song banner elements.

		GameObject wordScrollObj = GameObject.Find("WordScroll");
		Transform wordScrollLeftObj = wordScrollObj.transform.FindChild("WordScroll_Left");
		Transform wordScrollMidObj = wordScrollObj.transform.FindChild("WordScroll_Middle");
		Transform wordScrollRightObj = wordScrollObj.transform.FindChild("WordScroll_Right");

		Rect cameraWorldBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,true);
		wordScrollLeftObj.position = new Vector3(cameraWorldBounds.x + (wordScrollLeftObj.renderer.bounds.size.x/2f), wordScrollLeftObj.position.y, wordScrollLeftObj.position.z);
		wordScrollRightObj.position = new Vector3(cameraWorldBounds.x + cameraWorldBounds.width - (wordScrollRightObj.renderer.bounds.size.x/2f), wordScrollRightObj.position.y, wordScrollRightObj.position.z);

		// Create merged and adjusted song banner text area.
		List<Bounds> scrollTAPartBounds = new List<Bounds>();
		scrollTAPartBounds.Add(wordScrollLeftObj.FindChild("TextArea").renderer.bounds);
		scrollTAPartBounds.Add(wordScrollMidObj.FindChild("TextArea").renderer.bounds);
		scrollTAPartBounds.Add(wordScrollRightObj.FindChild("TextArea").renderer.bounds);
		Bounds maxScrollPartBounds = CommonUnityUtils.findMaxBounds(scrollTAPartBounds);
		Rect maxScrollPartBoundsRect = CommonUnityUtils.get2DBounds(maxScrollPartBounds);
		maxScrollPartBoundsRect = CommonUnityUtils.rescaleRect(maxScrollPartBoundsRect,0.85f,1f);
		GameObject songWordDisplayArea = WorldSpawnHelper.initObjWithinWorldBounds(debugPrefab,"SongWordDisplayArea",maxScrollPartBoundsRect,-1,upAxisArr);
		songWordDisplayArea.renderer.enabled = false;
		songWordDisplayArea.transform.parent = wordScrollObj.transform;



		wordScrollLeftObj.FindChild("TextArea").renderer.enabled = false;
		wordScrollLeftObj.FindChild("MusicMarkerStart").renderer.enabled = false;
		wordScrollMidObj.FindChild("TextArea").renderer.enabled = false;
		wordScrollRightObj.FindChild("TextArea").renderer.enabled = false;
		wordScrollRightObj.FindChild("MusicMarkerEnd").renderer.enabled = false;




		// Adjust word pool size.

		GameObject wordPoolGObj = GameObject.Find("WordPool");

		Vector2 nwWordPoolTopLeft = new Vector2(personPortrait.transform.position.x + (personPortrait.renderer.bounds.size.x/2f), continueBtn.transform.position.y + (continueBtn.renderer.bounds.size.y/2f));
		float availableWidthForWPool = (continueBtn.transform.position.x - (continueBtn.renderer.bounds.size.x/2f)) - nwWordPoolTopLeft.x;
		Rect nwWordPoolBounds = new Rect(nwWordPoolTopLeft.x,nwWordPoolTopLeft.y,availableWidthForWPool, continueBtn.renderer.bounds.size.y);
		Vector3 nwWordPoolCentrePt = new Vector3(nwWordPoolTopLeft.x + (nwWordPoolBounds.width/2f), nwWordPoolTopLeft.y - (nwWordPoolBounds.height/2f), wordPoolGObj.transform.position.z);

		float desiredWordPoolWidth = availableWidthForWPool * 0.95f;
		float desiredWordPoolHeight = nwWordPoolBounds.height * 0.9f;
		nwWordPoolBounds = new Rect(nwWordPoolBounds.x + (nwWordPoolBounds.width/2f) - (desiredWordPoolWidth/2f),
		                            nwWordPoolBounds.y - (nwWordPoolBounds.height/2f) + (desiredWordPoolHeight/2f),
		                            desiredWordPoolWidth,
		                            desiredWordPoolHeight);

		wordPoolGObj.transform.position = nwWordPoolCentrePt;
		Vector3 tmpWPoolScale = wordPoolGObj.transform.localScale;
		tmpWPoolScale.x = (tmpWPoolScale.x * (nwWordPoolBounds.width/wordPoolGObj.renderer.bounds.size.x));
		tmpWPoolScale.y = (tmpWPoolScale.y * (nwWordPoolBounds.height/wordPoolGObj.renderer.bounds.size.y));
		wordPoolGObj.transform.localScale = tmpWPoolScale;

		wordPoolGObj.renderer.enabled = false;







		// Game band Characters.

		currCharacterCount = 0;
		characterGObjMap = new Dictionary<string, GameObject>();
		characterScenePositions = new List<Vector3>();

		Rect totWorldBnds = WorldSpawnHelper.getGuiToWorldBounds(new Rect(0,0,Screen.width,Screen.height),Camera.main.transform.position.z,upAxisArr);

		for(int i=0; i<characters.Length; i++)
		{
			GameObject charObj = GameObject.Find(characters[i]);
			if(charObj == null)
			{
				characterScenePositions.Add(Vector3.zero);
			}
			else
			{
				characterScenePositions.Add(new Vector3(charObj.transform.position.x,charObj.transform.position.y,charObj.transform.position.z));
			}

			charObj.transform.position = new Vector3(totWorldBnds.x - 7,charObj.transform.position.y,charObj.transform.position.z);
			charObj.SetActive(false);
			characterGObjMap.Add(characters[i],charObj);
		}


		// Hide the people in the windows.
		for(int i=1; i<10; i++)
		{
			hidePersonAtWindow("Window"+i);
		}



		// Sound Channel Objects from the Intro Script instead.
		//initSoundChannels();


		// Player action log.
		playerSelectedHoleItems = new Dictionary<int, string>();


		// Music Marker.
		GameObject musicMarkerStartObj = wordScrollLeftObj.transform.FindChild("MusicMarkerStart").gameObject;
		Vector3 musicMarkerStartPos = new Vector3(musicMarkerStartObj.transform.position.x,musicMarkerStartObj.transform.position.y,Camera.main.transform.position.z + 2);
		GameObject musicMarkerObj = GameObject.Find("MusicMarker");
		musicMarkerObj.transform.position = musicMarkerStartPos;


		// Init intro sequence script.
		transform.gameObject.AddComponent<AcSHIntroSequenceScript>();

		// Night Ambience Sound Effect.
		triggerSoundAtCamera("NightAmbience",true);
	}



	public void firstSentence(){
		genNextLevel();

	}

	protected override void genNextLevel()
	{
		// Destroy old objects.
		for(int i=0; i<currLvlItems.Count; i++)
		{
			DestroyImmediate(GameObject.Find(currLvlItems[i]));
		}


		// Generate new sentence.
		currLvlConfig = (SHeroLevelConfig) lvlConfigGen.getNextLevelConfig(wordPoolMaxCapacity);

		musicMarkerSpeedPerWord_InSec = (3-currLvlConfig.speed)*5f;

		//debugFullSentence = currLvlConfig.getLevelSentence(true);
		recordPresentedConfig(currLvlConfig);

		if(serverCommunication != null) { serverCommunication.wordDisplayed(currLvlConfig.getLevelSentence(false),currLvlConfig.languageArea,currLvlConfig.difficulty); }


		// Show new song sentence.
		renderSongSentence();

		// Populate word pool.
		renderWordPool();


		playerSelectedHoleItems = new Dictionary<int, string>();


		mistakesInCurrSentence = 0;

		//musicMarkerIndex = -1;
		triggerMusicMarkerStartDelay();
	}




	public new void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		base.respondToEvent(para_sourceID,para_eventID,para_eventData);


		if((isInitialised == true)&&(hasInitIntroSequence == false))
		{
			// Init intro sequence script.
			transform.gameObject.GetComponent<AcSHIntroSequenceScript>().initIntroSequence();	
			hasInitIntroSequence = true;
		}


		if(para_eventID == "DragStart")
		{
			//Debug.Log("Drag start!!"+para_sourceID);

			currObjBeingDragged = GameObject.Find(para_sourceID);
			Destroy(currObjBeingDragged.GetComponent<MoveToLocation>());
			//dragRestPosition = currObjBeingDragged.transform.position;
		}
		else if(para_eventID == "DragRelease")
		{

			//Debug.Log("Drag release!!"+para_sourceID);
			DragScript dScript = GameObject.Find("GlobObj").GetComponent<DragScript>();
			if(dScript.getNumPotentialOwnersForDragObj(para_sourceID) <= 0)
			{
				Vector3 restPos = wordPoolItemPositions[int.Parse(para_sourceID.Split('-')[1])];

				//Debug.Log("Checked: '"+para_sourceID+"' and Zeroed in");

				// Version 1: Slowly move back into position in the Word Pool.
				//MoveToLocation mtl = currObjBeingDragged.AddComponent<MoveToLocation>();
				//mtl.initScript(restPos,40f);
				//mtl.registerListener("AcScen",this);

				// Version 2: Immediately teleport back to position in the Word Pool.
				TeleportToLocation ttl = currObjBeingDragged.AddComponent<TeleportToLocation>();
				ttl.init(restPos);
			}
			else
			{
				GameObject songBannerObj = GameObject.Find("WordScroll");
				GameObject songWordDisplayAreaObj = songBannerObj.transform.FindChild("SongWordDisplayArea").gameObject;
				Vector3 tmpPosVect = currObjBeingDragged.transform.position;
				tmpPosVect.z = songWordDisplayAreaObj.transform.position.z;
				currObjBeingDragged.transform.position = tmpPosVect;
			}

			currObjBeingDragged = null;
			//dragRestPosition = Vector3.zero;
		}
		else if(para_eventID == "HoleFilled")
		{
			string[] eData = (string[]) para_eventData;
			//string wordObjName = eData[0];
			string holeObjName = eData[1];

			GameObject reqHoleGObj = GameObject.Find(holeObjName);
			reqHoleGObj.renderer.enabled = false;			
		}
		else if(para_eventID == "HoleEmptied")
		{
			string[] eData = (string[]) para_eventData;
			//string wordObjName = eData[0];
			string holeObjName = eData[1];
			
			GameObject reqHoleGObj = GameObject.Find(holeObjName);
			reqHoleGObj.renderer.enabled = true;
		}
		else if(para_eventID == "DelayEnd")
		{
			if(isPerformingMusicMarkerStartDelay)
			{
				isPerformingMusicMarkerStartDelay = false;
				launchMusicMarker();
			}
		}
		else if(para_sourceID == "MusicMarker")
		{
			if(para_eventID == "MoveIntervalUpdate")
			{
				int holeIndex = (int) para_eventData;
				probeHolePosForRequiredItem(holeIndex);
				MoveToLocation mtl = GameObject.Find("MusicMarker").GetComponent<MoveToLocation>();

				if(holeIndex<(currLvlConfig.holeList.Length-1)&(continueBtnVisible)){
					Debug.Log("Marker, go to "+holeIndex);
					mtl.changeETAintermediate(musicMarkerSpeedPerWord_InSec);
				}else{

					Debug.Log("Marker, go to end (do nothing)");

					//mtl.changeETASpeed(2f);
				}


			}
			else if(para_eventID == "MoveToLocation")
			{
				triggerEndOfWordSequence();
			}
		}
		else if((characterGObjMap != null)&&(characterGObjMap.ContainsKey(para_sourceID)))
		{
			if(para_eventID == "CharacterEnter")
			{
				Animator characterAni = characterGObjMap[para_sourceID].GetComponent<Animator>();
				string animPrefix = (para_sourceID.Split('_')[1]).ToLower();
				characterAni.Play(animPrefix+"_play");

				incrementMusicChannel();
			}
			else if(para_eventID == "CharacterExit")
			{
				characterGObjMap[para_sourceID].SetActive(false);
			}
		}
		else if(para_eventID == "SoundOptionDisplayClose")
		{
			toggleTextVisibility(true);
			pauseScene(false);
		}
	}


	private void probeHolePosForRequiredItem(int para_holeIndex)
	{

		int sentenceElementIndex = currLvlConfig.holeList[para_holeIndex];

		GameObject holeObj = GameObject.Find("Hole-"+sentenceElementIndex);
		HoleScript hScript = holeObj.GetComponent<HoleScript>();
		string parkerName = hScript.getParkerName();



		if((parkerName != null)&&(parkerName != ""))
		{
			if(hScript.isHoleInLimbo())
			{
				DragScript dScrpt = (GameObject.Find("GlobObj").GetComponent<DragScript>());
				if(dScrpt != null)
				{
					dScrpt.clearAllOwnerCounterForObj(parkerName);
				}

				triggerWrongItemPlacementConsequences(sentenceElementIndex);

				if(playerSelectedHoleItems.ContainsKey(sentenceElementIndex)) { playerSelectedHoleItems[sentenceElementIndex] = ""; }
				else { playerSelectedHoleItems.Add(sentenceElementIndex,""); }
			}
			else
			{
				GameObject wordObj = GameObject.Find(parkerName);
				TextNBoardScript tnbScript = wordObj.GetComponent<TextNBoardScript>();
				string wordText = tnbScript.getText();
				
				if(wordText == currLvlConfig.sentenceWords[sentenceElementIndex])
				{
					// Correct
					triggerCorrectItemPlacementConsequences(sentenceElementIndex);
				}
				else
				{
					// Incorrect
					triggerWrongItemPlacementConsequences(sentenceElementIndex);
				}

				if(playerSelectedHoleItems.ContainsKey(sentenceElementIndex)) { playerSelectedHoleItems[sentenceElementIndex] = wordText; }
				else { playerSelectedHoleItems.Add(sentenceElementIndex,wordText); }
			}
		}
		else
		{
			// Missing item.
			triggerWrongItemPlacementConsequences(sentenceElementIndex);

			if(playerSelectedHoleItems.ContainsKey(sentenceElementIndex)) { playerSelectedHoleItems[sentenceElementIndex] = ""; }
			else { playerSelectedHoleItems.Add(sentenceElementIndex,""); }
		}


	}


	private void triggerCorrectItemPlacementConsequences(int para_sentencePosIndex)
	{
		GameObject holeObj = GameObject.Find("Hole-"+para_sentencePosIndex);
		HoleScript hScript = holeObj.GetComponent<HoleScript>();
		string parkerName = hScript.getParkerName();
		
		if((parkerName != null)&&(parkerName != ""))
		{
			GameObject wordObj = GameObject.Find(parkerName);
			TextNBoardScript tnbScript = wordObj.GetComponent<TextNBoardScript>();
			tnbScript.setBoardColor(Color.green);

			// Prevent word from being draggable.
			wordObj.layer = 0;
		}

		// Remove hole ability.
		Destroy(hScript);
	}

	private void triggerWrongItemPlacementConsequences(int para_sentencePosIndex)
	{
		mistakesInCurrSentence++;
		numIncorrectTries++;

		GameObject holeObj = GameObject.Find("Hole-"+para_sentencePosIndex);
		HoleScript hScript = holeObj.GetComponent<HoleScript>();
		string parkerName = hScript.getParkerName();
		
		if((parkerName != null)&&(parkerName != "")&&( ! hScript.isHoleInLimbo()))
		{
			GameObject wordObj = GameObject.Find(parkerName);
			TextNBoardScript tnbScript = wordObj.GetComponent<TextNBoardScript>();
			tnbScript.setBoardColor(Color.red);

			// Prevent word from being draggable.
			wordObj.layer = 0;
		}
		else
		{
			holeObj.renderer.material.color = Color.red;
		}

		// Remove hole ability.
		Destroy(hScript);



		mistakesForCharacterExit++;
		if(mistakesForCharacterExit == badAttemptsUntilCharacterExit)
		{
			mistakesForCharacterExit = 0;
			//exitCharacter();
		}
	}

	private void triggerEndOfWordSequence()
	{
		numSentencesDisplayed++;

		if(mistakesInCurrSentence == 0)
		{
			sentencesCompleted++;
			numCorrectSentences++;
			buildNRecordConfigOutcome(new object[]{true});
			if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(currLvlConfig.getLevelSentence(false),true,"",currLvlConfig.languageArea,currLvlConfig.difficulty); }


			counterToCharacterEnter++;
			if(counterToCharacterEnter == sentencesUntilNewCharacterEnter)
			{
				counterToCharacterEnter = 0;
				introduceNewCharacter();
			}
		}
		else
		{
			buildNRecordConfigOutcome(new object[]{false});
			if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(currLvlConfig.getLevelSentence(false),false,"",currLvlConfig.languageArea,currLvlConfig.difficulty); }
			exitCharacter();
		}
		checkNHandleWinningCondition();


		if( ! playerHasWon)
		{
			genNextLevel();
		}
	}




	private void introduceNewCharacter()
	{
		bool incrementedCharCount = false;

		if(currCharacterCount < characters.Length)
		{
			currCharacterCount++;
			incrementedCharCount = true;
			string reqCharName = characters[currCharacterCount-1];
			if(characterGObjMap.ContainsKey(reqCharName))
			{
				GameObject reqCharObj = characterGObjMap[reqCharName];
				reqCharObj.transform.eulerAngles = new Vector3(0,0,0);
				reqCharObj.SetActive(true);

				Animator characterAni = reqCharObj.GetComponent<Animator>();
				string animPrefix = (reqCharName.Split('_')[1]).ToLower();
				characterAni.Play(animPrefix+"_walk");


				Vector3 destPtInScene = characterScenePositions[currCharacterCount-1];
				//float timeToCompleteEnter_inSec = 2f;
				float characterWalkSpeed = 4f;


				CustomAnimationManager caMang = reqCharObj.AddComponent<CustomAnimationManager>();
				List<List<AniCommandPrep>> cmdBatchList = new List<List<AniCommandPrep>>();
				List<AniCommandPrep> cmdBatch1 = new List<AniCommandPrep>();
				cmdBatch1.Add(new AniCommandPrep("MoveToLocation",1, new List<System.Object>() { new float[3] {destPtInScene.x,destPtInScene.y,destPtInScene.z}, characterWalkSpeed }));
				cmdBatchList.Add(cmdBatch1);
				caMang.init("CharacterEnter",cmdBatchList);
				caMang.registerListener("AcScen",this);
			}



		}

		if(currCharacterCount < (windowReveals.Length))
		{
			string nxtWindowRevealName = windowReveals[currCharacterCount];

			if(nxtWindowRevealName != null)
			{
				showPersonAtWindow(nxtWindowRevealName);
			}

			if( ! incrementedCharCount)
			{
				currCharacterCount++;
			}
		}

		//checkNHandleWinningCondition();
	}

	private void exitCharacter()
	{
		if(currCharacterCount > 0)
		{

			bool decrementedCount = false;

			if(currCharacterCount <= (characters.Length-1))
			{
				currCharacterCount--;
				decrementedCount = true;
				reduceMusicChannel();

				string reqCharName = characters[currCharacterCount];
				if(characterGObjMap.ContainsKey(reqCharName))
				{
					GameObject reqCharObj = characterGObjMap[reqCharName];
					reqCharObj.transform.eulerAngles = new Vector3(reqCharObj.transform.eulerAngles.x,180,reqCharObj.transform.eulerAngles.z);


					Animator characterAni = reqCharObj.GetComponent<Animator>();
					string animPrefix = (reqCharName.Split('_')[1]).ToLower();
					characterAni.Play(animPrefix+"_walk");


					Rect cam2DWorldBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,true);
					Vector3 outOfScenePt = new Vector3(cam2DWorldBounds.x - 5,reqCharObj.transform.position.y,reqCharObj.transform.position.z);
					//float timeToCompleteExit_inSec = 2f;
					float characterWalkSpeed = 4;


					CustomAnimationManager caMang = reqCharObj.AddComponent<CustomAnimationManager>();
					List<List<AniCommandPrep>> cmdBatchList = new List<List<AniCommandPrep>>();
					List<AniCommandPrep> cmdBatch1 = new List<AniCommandPrep>();
					cmdBatch1.Add(new AniCommandPrep("MoveToLocation",1, new List<System.Object>() { new float[3] {outOfScenePt.x,outOfScenePt.y,outOfScenePt.z}, characterWalkSpeed }));
					cmdBatchList.Add(cmdBatch1);
					caMang.init("CharacterExit",cmdBatchList);
					caMang.registerListener("AcScen",this);
				}
			}


			if(currCharacterCount < windowReveals.Length)
			{
				if( ! decrementedCount)
				{
					currCharacterCount--;
				}


				string nxtWindowRevealName = windowReveals[currCharacterCount+1];
				if(nxtWindowRevealName != null)
				{
					hidePersonAtWindow(nxtWindowRevealName);
				}
			}
		}
	}


	private void incrementMusicChannel()
	{
		string soundBoxName = characters[currCharacterCount-1] + "_SoundChannel";//TODO index out of bounds?
		GameObject reqSoundBox = GameObject.Find(soundBoxName);
		AudioSource audS = (AudioSource) reqSoundBox.GetComponent(typeof(AudioSource));
		audS.mute = !musicOn;
	}
	
	private void reduceMusicChannel()
	{
		string soundBoxName = characters[currCharacterCount] + "_SoundChannel";
		GameObject reqSoundBox = GameObject.Find(soundBoxName);
		AudioSource audS = (AudioSource) reqSoundBox.GetComponent(typeof(AudioSource));
		audS.mute = musicOn;
	}


	protected override bool checkNHandleWinningCondition()
	{
		updateActivityProgressMetaData(numSentencesDisplayed/(reqMaxSentencesForWin*1.0f));
		playerHasWon = (numSentencesDisplayed >= reqMaxSentencesForWin);

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

		// Trigger record outcome.
		SHLevelOutcome reqOutcomeObj = null;
		if(positiveFlag)
		{
			reqOutcomeObj = new SHLevelOutcome(true,null);
		}
		else
		{
			reqOutcomeObj = new SHLevelOutcome(false,playerSelectedHoleItems);
		}

		// Trigger record outcome.
		recordOutcomeForConfig(reqOutcomeObj);
	}

	protected override GameyResultData buildGameyData()
	{
		float timeDiff = endTimestamp - startTimestamp;
		int minutes = (int) (timeDiff / 60f);
		int seconds = (int) (((timeDiff / 60f) - minutes) * 60f);

		SHGameyResultData reqData = new SHGameyResultData(numCorrectSentences,numIncorrectTries,currCharacterCount,minutes,seconds);
		return reqData;
	}

	protected override string generateGoalTextAppend()
	{
		return (" "+reqMaxSentencesForWin);
	}







	private float determineCharacterSizeForRow(Rect para_boardWorldBounds, List<string> para_rowWords, float para_maxCharacterSize)
	{
		float retCharacterSize = -1;

		float minCharacterSize = -1;

		for(int i=0; i<para_rowWords.Count; i++)
		{
			float reqMaxWidth = para_boardWorldBounds.width;
			float normalTextWidth = 0.7f * para_rowWords[i].Length;
			float normalTextCharacterSize = 0.1f;			
			float reqTextCharacterSize = (reqMaxWidth * normalTextCharacterSize)/normalTextWidth;

			if((minCharacterSize == -1)||(reqTextCharacterSize < minCharacterSize))
			{
				minCharacterSize = reqTextCharacterSize;
			}
		}


		if(minCharacterSize > para_maxCharacterSize)
		{
			retCharacterSize = para_maxCharacterSize;
		}
		else
		{
			retCharacterSize = minCharacterSize;
		}

		return retCharacterSize;
	}


	



	private void launchMusicMarker()
	{
		GameObject musicMarkerStartObj = (GameObject.Find("WordScroll")).transform.FindChild("WordScroll_Left").transform.FindChild("MusicMarkerStart").gameObject;
		GameObject musicMarkerEndObj = (GameObject.Find("WordScroll")).transform.FindChild("WordScroll_Right").transform.FindChild("MusicMarkerEnd").gameObject;
		Vector3 musicMarkerStartPos = new Vector3(musicMarkerStartObj.transform.position.x,musicMarkerStartObj.transform.position.y,Camera.main.transform.position.z + 2);
		Vector3 musicMarkerEndPos = new Vector3(musicMarkerEndObj.transform.position.x,musicMarkerEndObj.transform.position.y,Camera.main.transform.position.z + 2);

		float totDistance = Vector3.Distance(musicMarkerStartPos,musicMarkerEndPos);

		int numHoles = currLvlConfig.holeList.Length;
		float[] musicMarkerNormIntervalPositions = new float[numHoles];
		for(int i=0; i<currLvlConfig.holeList.Length; i++)
		{
			GameObject holeObj = GameObject.Find("Hole-"+currLvlConfig.holeList[i]);
			Vector3 holeTopLeft = new Vector3(holeObj.transform.position.x - (holeObj.renderer.bounds.size.x/2f), holeObj.transform.position.y + (holeObj.renderer.bounds.size.y/2f), musicMarkerStartPos.z);
			float tmpDistFromStart = Vector3.Distance(musicMarkerStartPos,holeTopLeft);
			musicMarkerNormIntervalPositions[i] = tmpDistFromStart / totDistance;
		}


		GameObject musicMarkerObj = GameObject.Find("MusicMarker");
		musicMarkerObj.transform.position = musicMarkerStartPos;

		float sentenceLength = (currLvlConfig.sentenceWords.Length * 1.0f);
		float reqDuration = (sentenceLength * musicMarkerSpeedPerWord_InSec);




		MoveToLocation mtl = musicMarkerObj.AddComponent<MoveToLocation>();
		mtl.initScript(musicMarkerEndPos,reqDuration,false);
		mtl.requestIntervalUpdates(musicMarkerNormIntervalPositions);//Only these matter
		mtl.changeETAintermediate(musicMarkerSpeedPerWord_InSec);//Only these matter
		Debug.Log(musicMarkerSpeedPerWord_InSec);
		mtl.registerListener("AcScen",this);

		continueBtnVisible = true;
		GameObject.Find("AutoCompleteButtonLocation").renderer.enabled = true;

		if(paused)
		{
			mtl.enabled = false;
		}
	}
	
	private void triggerMusicMarkerStartDelay()
	{
		isPerformingMusicMarkerStartDelay = true;
		DelayForInterval dfi = transform.gameObject.AddComponent<DelayForInterval>();
		dfi.registerListener("AcScen",this);
		dfi.init(0.5f);
	}

	private AudioSource spawnBandMemberAudioObject(string para_soundBoxName, string para_instrumentShortHand)
	{
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,Camera.main.transform.position,Quaternion.identity)).gameObject;
		nwSFX.name = para_soundBoxName;
		Destroy(nwSFX.GetComponent<DestroyAfterTime>());
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/Serenade Band Tracks/Serenade_"+para_instrumentShortHand+"_01.00",typeof(AudioClip));
		audS.volume = 0.4f;
		audS.mute = true;
		audS.loop = true;

		return audS;
	}





	protected new bool initLevelConfigGenerator()
	{
		if( ! base.initLevelConfigGenerator())
		{
			// Fallback.
			lvlConfigGen = new SHLevelConfigGeneratorServer(null); //new SHeroLevelConfigGeneratorHardCoded();
			Debug.LogWarning("Warning: Using Level Gen Fallback");
		}
		return true;
	}


	

	private new void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string, GUIStyle>();
		
		GUIStyle goalTextStyle = new GUIStyle(GUI.skin.label);
		goalTextStyle.fontSize = 20;
		goalTextStyle.alignment = TextAnchor.MiddleCenter;
		
		availableGUIStyles.Add("GoalText",goalTextStyle);
	}

	private List<SentenceSegment> extractMinimalSentenceSegments(SHeroLevelConfig para_config)
	{
		List<SentenceSegment> retSegmentList = new List<SentenceSegment>();

		string fullSentence = para_config.getLevelSentence(true);
		int sentenceCharLengthWhite = fullSentence.Replace("'","").Length;
		//fullSentence = fullSentence.Replace(" ","");
		//int sentenceCharLength = fullSentence.Replace("'","").Length;


		//Debug.Log(fullSentence+" "+sentenceCharLengthWhite+" characters");

		SentenceSegment regSeg = new SentenceSegment("",0,currLvlConfig.positionIsHole(0),0);
		string[] sentenceWordArr = para_config.sentenceWords;

		int maxHoleCharacters = 0;
		List<int> holeSizes = new List<int>();

		float tmpTotPerc = 0;
		for(int i=0; i<sentenceWordArr.Length; i++)
		{
			string tmpSentenceWord = sentenceWordArr[i];
			bool currIsHole = currLvlConfig.positionIsHole(i);

			if((regSeg.isHole == false)&&(currIsHole == false))
			{
				regSeg.segmentText += (tmpSentenceWord);
				regSeg.sentenceCharPerc = (regSeg.segmentText.Length * 1.0f) / (sentenceCharLengthWhite * 1.0f);
				regSeg.isHole = false;
				//Debug.Log("Accumulated *"+regSeg.segmentText+"* "+regSeg.sentenceCharPerc+ " False");


			}
			else
				/*if(((regSeg.isHole == false)&&(currIsHole == true))
				||((regSeg.isHole == true)&&(currIsHole == false))
				||((regSeg.isHole = true)&&(currIsHole == true)))*/
			{
				//Debug.Log("Done");

				retSegmentList.Add(regSeg);

				if(regSeg.isHole){

					holeSizes.Add(regSeg.segmentText.Length);
					if(regSeg.segmentText.Length>maxHoleCharacters)
						maxHoleCharacters = regSeg.segmentText.Length;
				}

				tmpTotPerc += regSeg.sentenceCharPerc;
				regSeg = new SentenceSegment(tmpSentenceWord, ((tmpSentenceWord.Length) * 1.0f ) / (sentenceCharLengthWhite * 1.0f) ,currIsHole,tmpSentenceWord.Length);

				//Debug.Log("accumulated (hole) *"+regSeg.segmentText+"* "+regSeg.sentenceCharPerc+" "+currIsHole);

			}

			if(i == (sentenceWordArr.Length-1))
			{
				retSegmentList.Add(regSeg);
				tmpTotPerc += regSeg.sentenceCharPerc;
				if(regSeg.isHole){
					
					holeSizes.Add(regSeg.segmentText.Length);
					if(regSeg.segmentText.Length>maxHoleCharacters)
						maxHoleCharacters = regSeg.segmentText.Length;
				}

				//Debug.Log("Done");
			}
		}


		//Replaces the length of each hole for the maximum length, to avoid recognition of correct answer based on size
		foreach(int holeWidth in holeSizes){
			sentenceCharLengthWhite += (maxHoleCharacters-holeWidth);
		}

		if(retSegmentList[0].segmentText.Length == 0)
		{
			retSegmentList.RemoveAt(0);
		}

		tmpTotPerc = 0.0f;
		foreach(SentenceSegment segment in retSegmentList){
			if(segment.isHole)
				segment.numCharacters = maxHoleCharacters;
			else
				segment.numCharacters = segment.segmentText.Length;

			segment.sentenceCharPerc = segment.numCharacters/(1.0f*sentenceCharLengthWhite);
			tmpTotPerc+=segment.sentenceCharPerc;

		}
		


		//Debug.Log("Overflow? "+tmpTotPerc);
		// If we have a perc overflow due to floating point precision then fix to 1 and redistribute the remainder.
		/*if(tmpTotPerc > 1)
		{
			float remainder = 1 - tmpTotPerc;
			float removalFactor = remainder / (retSegmentList.Count * 1.0f);

			tmpTotPerc = 0;
			for(int i=0; i<retSegmentList.Count; i++)
			{
				retSegmentList[i].sentenceCharPerc -= removalFactor;
				tmpTotPerc += retSegmentList[i].sentenceCharPerc;
			}

			if(tmpTotPerc > 1)
			{
				float subTotal = 0;
				for(int i=0; i<retSegmentList.Count-1; i++)
				{
					subTotal += retSegmentList[i].sentenceCharPerc;
				}

				retSegmentList[retSegmentList.Count-1].sentenceCharPerc = 1 - subTotal;
			}
		}*/




		return retSegmentList;
	}



	public void renderSongSentence()
	{
		// Create world items representing the sentence.

		if(currLvlConfig == null)
		{
			//Debug.LogWarning("DEPRECATED");
			currLvlConfig = (SHeroLevelConfig) lvlConfigGen.getNextLevelConfig(wordPoolMaxCapacity);
			musicMarkerSpeedPerWord_InSec = (3-currLvlConfig.speed)*5f;

			recordPresentedConfig(currLvlConfig);
			if(serverCommunication != null) { serverCommunication.wordDisplayed(currLvlConfig.getLevelSentence(false),currLvlConfig.languageArea,currLvlConfig.difficulty); }
		}

		GameObject songBannerObj = GameObject.Find("WordScroll");
		GameObject songWordDisplayAreaObj = songBannerObj.transform.FindChild("SongWordDisplayArea").gameObject;
		Rect sentenceAreaBounds = CommonUnityUtils.get2DBounds(songWordDisplayAreaObj.renderer.bounds);

		
		List<SentenceSegment> sentenceSegments = extractMinimalSentenceSegments(currLvlConfig);

		float totalSentencePercForHoles = 0;
		float tmpTotal = 0;
		int numHoles = 0;
		for(int i=0; i<sentenceSegments.Count; i++)
		{
			SentenceSegment tmpSeg = sentenceSegments[i];
			if(tmpSeg.isHole)
			{
				numHoles++;
				totalSentencePercForHoles += tmpSeg.sentenceCharPerc;
			}
			tmpTotal += tmpSeg.sentenceCharPerc;
		}
		//Debug.Log("100%:"+tmpTotal);
		float allocatedWidthPercPerHole = totalSentencePercForHoles / (numHoles * 1.0f);
		
		List<GameObject> tmpWordBoxes = new List<GameObject>();
		List<GameObject> tmpWordBoxesAndHoles = new List<GameObject>();
		List<int> holeIdxs = new List<int>();
		songBannerItemPositions = new List<Vector3>();


		//Debug.Log("Total area: "+sentenceAreaBounds.x+" "+sentenceAreaBounds.y+" "+sentenceAreaBounds.height+" "+sentenceAreaBounds.width);

		Vector3 currSegWorldTopLeft = new Vector3(sentenceAreaBounds.x,sentenceAreaBounds.y,songWordDisplayAreaObj.transform.position.z);
		int nxtHoleIDArrIndex = 0;
		for(int i=0; i<sentenceSegments.Count; i++)
		{
			SentenceSegment tmpSeg = sentenceSegments[i];
			float blockWidth = sentenceAreaBounds.width * tmpSeg.sentenceCharPerc;
			if(tmpSeg.isHole) { 
				blockWidth = sentenceAreaBounds.width * allocatedWidthPercPerHole; }


			Rect blockBounds = new Rect(currSegWorldTopLeft.x,currSegWorldTopLeft.y,blockWidth,sentenceAreaBounds.height);


			if( ! tmpSeg.isHole)
			{
				// Create text block.

				//Debug.Log(tmpSeg.segmentText+" Width: "+blockBounds.width+" from "+currSegWorldTopLeft.x);

				GameObject nwWordBox = WordBuilderHelper.buildWordBox(99,tmpSeg.segmentText,blockBounds,currSegWorldTopLeft.z,upAxisArr,wordBoxPrefab);
				Destroy(nwWordBox.GetComponent<BoxCollider>());
				Destroy(nwWordBox.transform.FindChild("Board").gameObject);
				nwWordBox.transform.FindChild("Text").gameObject.renderer.sortingOrder = 1000;
				//Debug.Log("TextMeshWidth-"+i+"-"+(nwWordBox.transform.FindChild("Text").renderer.bounds.size.x));



				tmpWordBoxes.Add(nwWordBox);
				tmpWordBoxesAndHoles.Add(nwWordBox);
				songBannerItemPositions.Add(nwWordBox.transform.position);
				currLvlItems.Add(nwWordBox.name);
			}
			else
			{
				// Create hole block.

				//Debug.Log(tmpSeg.segmentText+" Width: "+blockBounds.width+" from "+currSegWorldTopLeft.x);

				string text =tmpSeg.segmentText;
				while(text.Length<tmpSeg.numCharacters)
					text+="*";
				//Debug.Log(text);
				GameObject nwWordBoxHole = WordBuilderHelper.buildWordBox(99,text,blockBounds,currSegWorldTopLeft.z,upAxisArr,wordBoxPrefab);
				Destroy(nwWordBoxHole.GetComponent<BoxCollider>());
				Destroy(nwWordBoxHole.transform.FindChild("Board").gameObject);
				nwWordBoxHole.transform.FindChild("Text").gameObject.renderer.sortingOrder = 1000;
				//Debug.Log("TextMeshWidth-"+i+"-"+(nwWordBox.transform.FindChild("Text").renderer.bounds.size.x));

				holeIdxs.Add(tmpWordBoxesAndHoles.Count);
				tmpWordBoxesAndHoles.Add(nwWordBoxHole);

			}

			currSegWorldTopLeft.x += blockBounds.width;
		}

		//songBannerFontCharacterSize = WordBuilderHelper.setBoxesToUniformTextSize(tmpWordBoxesAndHoles,0.10f);
		songBannerFontCharacterSize = WordBuilderHelper.setBoxesToTextSizeInRange(tmpWordBoxesAndHoles,0.04f,0.1f);


		Debug.Log("WorldBoxes font "+songBannerFontCharacterSize);


		float holeNewWidth = 0f;

		tmpTotal = 0;
		for(int i=0; i<tmpWordBoxesAndHoles.Count; i++)
		{
			GameObject nxtWordBox = tmpWordBoxesAndHoles[i];
			Transform textChild = nxtWordBox.transform.FindChild("Text");
			Transform boardChild = nxtWordBox.transform.FindChild("Board");
			
			float origBoardWidth = boardChild.renderer.bounds.size.x;
			
			float reqBoardWidth = textChild.renderer.bounds.size.x;

			if(sentenceSegments[i].isHole){

				if(reqBoardWidth>holeNewWidth)
					holeNewWidth = reqBoardWidth;
			}else{

			float reqXScale = reqBoardWidth / origBoardWidth;
			
			Vector3 tmpScaleVect = boardChild.localScale;
			tmpScaleVect.x *= reqXScale;
			boardChild.localScale = tmpScaleVect;
			
			tmpTotal += boardChild.renderer.bounds.size.x;
			//freedWidth += (origBoardWidth - reqBoardWidth);
			//Debug.Log("Adjust word "+nxtWordBox.transform.FindChild("Text").gameObject.GetComponent<TextMesh>().text+" by "+reqXScale+ " (size: "+ (boardChild.renderer.bounds.size.x) +")");
			}
		}


		for(int i=0; i<tmpWordBoxesAndHoles.Count; i++)
		{

			if(!sentenceSegments[i].isHole)
				continue;

			GameObject nxtWordBox = tmpWordBoxesAndHoles[i];
			Transform textChild = nxtWordBox.transform.FindChild("Text");

			float origTextWidth = textChild.renderer.bounds.size.x;

			float reqXScale = holeNewWidth / origTextWidth;
				
			Vector3 tmpScaleVect = textChild.localScale;
			tmpScaleVect.x *= reqXScale;
			textChild.localScale = tmpScaleVect;

			tmpTotal += textChild.renderer.bounds.size.x;
				//freedWidth += (origBoardWidth - reqBoardWidth);
			Debug.Log("Adjust hole "+nxtWordBox.transform.FindChild("Text").gameObject.GetComponent<TextMesh>().text+" by "+reqXScale+ " (size: "+ (textChild.renderer.bounds.size.x) +")");
		}

		//Destroy holeWords and replace them with holes of the same size
		for(int i = 0;i<holeIdxs.Count;i++){
			GameObject holeWord = tmpWordBoxesAndHoles[ holeIdxs[i] ];

			Bounds bounds = holeWord.transform.FindChild("Text").renderer.bounds;
			Rect blockBounds = CommonUnityUtils.get2DBounds(bounds);

			GameObject nwHole = WorldSpawnHelper.initObjWithinWorldBounds(holePrefab,1,1,"Hole-"+currLvlConfig.holeList[nxtHoleIDArrIndex],blockBounds,null,songWordDisplayAreaObj.transform.position.z,upAxisArr);
			nxtHoleIDArrIndex++;
			
			HoleScript hs = nwHole.GetComponent<HoleScript>();
			hs.registerListener("AcScen",this);
			
			nwHole.transform.renderer.material.color = Color.gray;
			nwHole.transform.GetComponent<BoxCollider>().size = new Vector3(1,1,50);
			
			songBannerItemPositions.Add(nwHole.transform.position);
			songWordWorldScale = nwHole.transform.renderer.bounds.size;
			
			currLvlItems.Add(nwHole.name);

			DestroyImmediate(holeWord);

		}


		/*float addedWidthPerHole = freedWidth / (numHoles * 1.0f);
		if((numHoles == 1)&&(currLvlConfig.isWord)) { addedWidthPerHole = 0; }
		for(int i=0; i<numHoles; i++)
		{
			GameObject nxtHoldObj = GameObject.Find("Hole-"+currLvlConfig.holeList[i]);

			float addition = addedWidthPerHole;
			float origHoleWidth = nxtHoldObj.renderer.bounds.size.x;
			
			if(i == (numHoles-1))
			{
				if((tmpTotal + addition) > sentenceAreaBounds.width)
				{
					addition = sentenceAreaBounds.width - tmpTotal;
				}
			}
			
			float reqHoleWidth = origHoleWidth + addition;
			
			Vector3 tmpScaleVect = nxtHoldObj.transform.localScale;
			tmpScaleVect.x *= (reqHoleWidth / origHoleWidth);
			nxtHoldObj.transform.localScale = tmpScaleVect;
			
			tmpTotal += reqHoleWidth;
		}*/

		
		//Position the boxes
		currSegWorldTopLeft = new Vector3(sentenceAreaBounds.x + (sentenceAreaBounds.width/2f) - (tmpTotal/2f),sentenceAreaBounds.y,songWordDisplayAreaObj.transform.position.z);
		int nxtWordBoxArrIndex = 0;
		int nxtHoleArrIndex = 0;
		
		for(int i=0; i<sentenceSegments.Count; i++)
		{
			SentenceSegment tmpSeg = sentenceSegments[i];
			
			float boxWidth = 0;
			
			if( ! tmpSeg.isHole)
			{
				GameObject reqBox = tmpWordBoxes[nxtWordBoxArrIndex];
				boxWidth = reqBox.transform.FindChild("Board").renderer.bounds.size.x;
				reqBox.transform.position = new Vector3(currSegWorldTopLeft.x + (boxWidth/2f),reqBox.transform.position.y,reqBox.transform.position.z);
				nxtWordBoxArrIndex++;
			}
			else
			{
				GameObject reqBox = GameObject.Find("Hole-"+currLvlConfig.holeList[nxtHoleArrIndex]);
				boxWidth = reqBox.renderer.bounds.size.x;
				reqBox.transform.position = new Vector3(currSegWorldTopLeft.x + (boxWidth/2f),reqBox.transform.position.y,reqBox.transform.position.z);
				nxtHoleArrIndex++;
			}
			
			currSegWorldTopLeft.x += boxWidth;
		}
		
		
		// Apply hole asthetics (shirnk slightly)
		/*for(int i=0; i<currLvlConfig.holeList.Length; i++)
		{
			GameObject holeObj = GameObject.Find("Hole-"+currLvlConfig.holeList[i]);
			
			Vector3 tmpScale = holeObj.transform.localScale;
			tmpScale.x *= 0.9f;
			tmpScale.y *= 0.8f;
			holeObj.transform.localScale = tmpScale;
			
			songWordWorldScale = holeObj.transform.localScale;
		}*/


		AcSHIntroSequenceScript introScript = transform.gameObject.GetComponent<AcSHIntroSequenceScript>();
		if(introScript != null)
		{
			introScript.respondToEvent("AcScen","SentenceDisplayed",null);
		}
	}


	public List<GameObject> renderWordPool()
	{
		// Populate Fragment Area.
		

		GameObject wordPoolObj = GameObject.Find("WordPool");
		GameObject wordPoolDisplayAreaObj = wordPoolObj;
		Rect fragmentAreaBounds = CommonUnityUtils.get2DBounds(wordPoolDisplayAreaObj.renderer.bounds);
		
		
		bool wordPoolArea_horizontal = (fragmentAreaBounds.width >= fragmentAreaBounds.height);
		
		float wordFragmentWidth = fragmentAreaBounds.width;
		float wordFragmentHeight = fragmentAreaBounds.height / (wordPoolMaxCapacity * 1.0f);
		float fragPaddingWidth = (fragmentAreaBounds.width * 0.1f) / (wordPoolMaxCapacity * 1.0f);
		if(wordPoolArea_horizontal)
		{
			wordFragmentWidth = ((fragmentAreaBounds.width * 0.9f) / (wordPoolMaxCapacity * 1.0f));
			wordFragmentHeight = fragmentAreaBounds.height;
		}
		Rect wordFragmentBounds = new Rect(fragmentAreaBounds.x,fragmentAreaBounds.y,wordFragmentWidth,wordFragmentHeight);
		
		
		wordPoolItemPositions = new List<Vector3>();
		DragScript dScript = (GameObject.Find("GlobObj").GetComponent<DragScript>());
		dScript.resetAllOwnerCounters();
		
		
		
		
		List<GameObject> tmpWordBoxes = new List<GameObject>();
		
		for(int i=0; i<currLvlConfig.wordPoolWords.Length; i++)
		{
			string nxtWordPoolWord = currLvlConfig.wordPoolWords[i];
			
			GameObject nwFragmentBox = WordBuilderHelper.buildWordBox(i,nxtWordPoolWord,wordFragmentBounds,Camera.main.transform.position.z + 4 - 2,upAxisArr,wordPoolWordBoxPrefab);
			nwFragmentBox.name = "WordPoolEntry-"+i;
			dScript.registerListener("AcScenScript",this);
			poolWordWorldScale = (nwFragmentBox.transform.FindChild("Board")).renderer.bounds.size;
			nwFragmentBox.transform.FindChild("Board").renderer.material.color = ColorUtils.convertColor(189,183,107);
			wordPoolItemPositions.Add(nwFragmentBox.transform.position);
			currLvlItems.Add(nwFragmentBox.name);
			
			
			tmpWordBoxes.Add(nwFragmentBox);
			
			if(! wordPoolArea_horizontal)
			{
				wordFragmentBounds.y -= wordFragmentBounds.height;
			}
			else
			{
				wordFragmentBounds.x += (wordFragmentBounds.width + fragPaddingWidth);
			}
		}
		
		float wordPoolCharSize = WordBuilderHelper.getMinFontCharSizeForWordBoxes(tmpWordBoxes);
		if(wordPoolCharSize > desiredWordPoolCharacterSize) { wordPoolCharSize = desiredWordPoolCharacterSize; }
		wordPoolFontCharacterSize = wordPoolCharSize;
		WordBuilderHelper.setBoxesToUniformTextSize(tmpWordBoxes,wordPoolCharSize);
		
		
		for(int i=0; i<tmpWordBoxes.Count; i++)
		{
			//Destroy(tmpWordBoxes[i].transform.FindChild("Board").gameObject);
			tmpWordBoxes[i].transform.FindChild("Board").gameObject.renderer.sortingOrder = 1001;
			tmpWordBoxes[i].transform.FindChild("Text").gameObject.renderer.sortingOrder = 1001;
			
			ActiveAutoResizeScript aarScript = tmpWordBoxes[i].AddComponent<ActiveAutoResizeScript>();
			aarScript.init(songBannerItemPositions,poolWordWorldScale,songWordWorldScale,wordPoolFontCharacterSize,songBannerFontCharacterSize,true);
		}

		return tmpWordBoxes;
	}

	private void initSoundChannels()
	{
		scenAudSList = new List<AudioSource>();
		AudioSource permanentViolinSource = spawnBandMemberAudioObject("Ser_Viol1_SoundChannel","Viol1");
		permanentViolinSource.mute = false;//permanentViolinSource.mute = false;
		scenAudSList.Add(permanentViolinSource);
		for(int i=0; i<characters.Length; i++)
		{   
			string soundBoxName = characters[i] + "_SoundChannel";
			string instrumentShortHand = characters[i].Split('_')[1];
			
			AudioSource audS = spawnBandMemberAudioObject(soundBoxName,instrumentShortHand);
			scenAudSList.Add(audS);
		}
		for(int i=0; i<scenAudSList.Count; i++)
		{
			scenAudSList[i].Play();
		}
	}


	public void startTheBandMusic()
	{
		Destroy(GameObject.Find("NightAmbience"));

		initSoundChannels();

		pauseBtnVisible = true;
		GameObject.Find("PauseButton").renderer.enabled = true;

		mistakesInCurrSentence = 0;
		//musicMarkerIndex = -1;
		launchMusicMarker();

		startTimestamp = Time.time;
	}

	public void showPersonAtWindow(string para_windowName)
	{
		GameObject houseGObj = GameObject.Find("House");
		GameObject reqWindow = houseGObj.transform.FindChild(para_windowName).gameObject;
		GameObject personHead = reqWindow.transform.FindChild("WindowHead").gameObject;
		GameObject personBody = reqWindow.transform.FindChild("WindowBody").gameObject;

		reqWindow.GetComponent<Animator>().enabled = true;
		personHead.renderer.enabled = true;
		personBody.renderer.enabled = true;
		reqWindow.GetComponent<SpriteRenderer>().color = Color.white;
	}

	public void hidePersonAtWindow(string para_windowName)
	{
		GameObject houseGObj = GameObject.Find("House");
		GameObject reqWindow = houseGObj.transform.FindChild(para_windowName).gameObject;
		GameObject personHead = reqWindow.transform.FindChild("WindowHead").gameObject;
		GameObject personBody = reqWindow.transform.FindChild("WindowBody").gameObject;

		reqWindow.GetComponent<Animator>().enabled = false;
		personHead.renderer.enabled = false;
		personBody.renderer.enabled = false;
		reqWindow.GetComponent<SpriteRenderer>().color = Color.gray;
	}




	private void triggerSoundAtCamera(string para_soundFileName, bool para_permanent)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		nwSFX.name = para_soundFileName;
		if(para_permanent)
		{
			Destroy(nwSFX.GetComponent<DestroyAfterTime>());
		}
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = 0.7f;
		audS.Play();
	}


	class SentenceSegment
	{
		public string segmentText;
		public float sentenceCharPerc;
		public bool isHole;
		public int numCharacters;

		public SentenceSegment(string para_segmentText, float para_sentenceCharPerc, bool para_isHole,int chars)
		{
			segmentText = para_segmentText;
			sentenceCharPerc = para_sentenceCharPerc;
			isHole = para_isHole;
			numCharacters = chars;
		}
	}
}
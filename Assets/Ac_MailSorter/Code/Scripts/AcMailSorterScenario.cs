/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;


public class AcMailSorterScenario : ILearnRWActivityScenario, CustomActionListener
{

	public Transform postmen3Prefab;
	public Transform postmen4Prefab;
	public Transform[] parcelPrefabs;
	public Transform wordBoxPrefab;
	public Transform basketBasePrefab;
	public Transform postChiefPrefab;


	MSLevelConfig currLvlConfig;
	bool metaDataLoaded = false;

	Vector3 postmenLevelDest;
	Vector3 postChiefLevelDest;
	float postmenDistPerSec = 5f;

	string[] jumbledParcelWords;
	string[] jumbledPostmenWords;
	int[] selectedParcelPrefabIndexes;

	List<AbsInputDetector> inputDetectors;

	bool[] upAxisArr = new bool[3] { false, true, false };


	HashSet<int> correctOccupiedBasketIDs;
	HashSet<int> occupiedBasketIDs;

	bool hasRegisteredInit = false;
	bool paused = false;

	int currNumOfPostmen = 0;

	int numOfFinishedBikes = 0;
	int reqBikesForWin; // init when lvl gen init.


	int numCorrectAttempts = 0;
	int numIncorrectAttemptsThisRound = 0;
	int numIncorrectAttempts = 0;
	float startTimestamp = 0;
	float endTimestamp = 0;

	bool isFirstLevel = true;

	MSLevelOutcome lvlOutcome;


	void Start()
	{
		acID = ApplicationID.MAIL_SORTER;
		metaDataLoaded = loadActivitySessionMetaData();

		loadTextures();
		prepUIBounds();
		
		initWorld();
		this.initLevelConfigGenerator();
		reqBikesForWin = lvlConfigGen.getConfigCount();
		//genNextLevel();

		initPersonHelperPortraitPic();
		showInfoSlideshow();
		recordActivityStart();
	}
	


	void OnGUI()
	{
		
		GUI.color = Color.clear;

		if(hasRegisteredInit)
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

	protected override void initWorld()
	{
		inputDetectors = new List<AbsInputDetector>();
		ClickDetector cd = transform.gameObject.AddComponent<ClickDetector>();
		cd.registerListener("AcScen",this);
		inputDetectors.Add(cd);
		
		GhostDragScript gds = transform.gameObject.GetComponent<GhostDragScript>();
		gds.registerListener("AcScen",this);
		
		
		GameObject postMenObj = GameObject.Find("PostMen");
		postmenLevelDest = new Vector3(postMenObj.transform.position.x,postMenObj.transform.position.y,postMenObj.transform.position.z);
		Destroy(postMenObj);
		
		GameObject postChiefObj = GameObject.Find("PostChief");
		postChiefLevelDest = new Vector3(postChiefObj.transform.position.x,postChiefObj.transform.position.y,postChiefObj.transform.position.z);
		Destroy(postChiefObj);
		
		GameObject platformObj = GameObject.Find("PlatformParent");
		Animator aniScript = platformObj.GetComponent<Animator>();
		aniScript.speed = 0;
		//CommonUnityUtils.setSortingOrderOfEntireObject(platformObj,-18);
		
		GameObject mainConveyorObj = GameObject.Find("ConveyerRight").gameObject;
		ConveyorScript cs1 = mainConveyorObj.GetComponent<ConveyorScript>();
		cs1.init(new Vector3(1,0,0),2f);
		cs1.toggleConveyorOnState(); // Switch off main conveyor by default.
		
		GameObject returnConveyorObj = GameObject.Find("ReturnConveyerLeft");
		ConveyorScript cs2 = returnConveyorObj.GetComponent<ConveyorScript>();
		cs2.init(new Vector3(-1,0,0),2f);//1.5
		
		ParcelAbsorberScript pas = GameObject.Find("Absorber").GetComponent<ParcelAbsorberScript>();
		pas.registerListener("AcScen",this);


		// Auto Adjust.
		GameObject tmpPersonPortrait = GameObject.Find("PersonPortrait");
		GameObject tmpPauseButton = GameObject.Find("PauseButton");
		GameObject tmpBackdrop = GameObject.Find("Backdrop");
		SpawnNormaliser.adjustGameObjectsToNwBounds(SpawnNormaliser.get2DBounds(tmpBackdrop.renderer.bounds),
		                                            WorldSpawnHelper.getCameraViewWorldBounds(tmpBackdrop.transform.position.z,true),
		                                            new List<GameObject>() { tmpPersonPortrait, tmpPauseButton });


		tmpPersonPortrait.transform.parent = GameObject.Find("Main Camera").transform;
		tmpPauseButton.transform.parent = GameObject.Find("Main Camera").transform;
		
		uiBounds.Add("PersonPortrait",WorldSpawnHelper.getWorldToGUIBounds(tmpPersonPortrait.renderer.bounds,upAxisArr));
		uiBounds.Add("PauseBtn",WorldSpawnHelper.getWorldToGUIBounds(tmpPauseButton.renderer.bounds,upAxisArr));		 


		correctOccupiedBasketIDs = new HashSet<int>();
		occupiedBasketIDs = new HashSet<int>();
	}

	protected void goaheadWithInit()
	{


		genNextLevel();
		isInitialised = true;
	}

	protected new bool initLevelConfigGenerator()
	{
		if( ! base.initLevelConfigGenerator())
		{
			// Fallback.
			lvlConfigGen = new MSLevelConfigGeneratorServer(null); //new MSLevelConfigGeneratorHardCoded(); 
			Debug.LogWarning("Warning: Using Level Gen Fallback");
		}
		return true;
	}

	protected override void genNextLevel()
	{

		numIncorrectAttemptsThisRound = 0;
		if(! isFirstLevel)
		{
			buildNRecordConfigOutcome(null);
		}
		isFirstLevel = false;

		currLvlConfig = (MSLevelConfig) lvlConfigGen.getNextLevelConfig(null);
	
		recordPresentedConfig(currLvlConfig);


		GameObject mainConveyorObj = GameObject.Find("ConveyerRight").gameObject;
		ConveyorScript cs1 = mainConveyorObj.GetComponent<ConveyorScript>();
		GameObject returnConveyorObj = GameObject.Find("ReturnConveyerLeft");
		ConveyorScript cs2 = returnConveyorObj.GetComponent<ConveyorScript>();

		XRayDetectorScript xray = GameObject.Find("XRayBack").transform.FindChild("XRayDetector").GetComponent<XRayDetectorScript>();

		xray.setTtsOn(false);

		switch(currLvlConfig.getSpeed()){

			case 1: 
				cs1.setNewSpeed(3f);
				cs2.setNewSpeed(3f);
				xray.setScanningTime(4f);

				break;

			case 2: 
				cs1.setNewSpeed(4f);
				cs2.setNewSpeed(4f);
				xray.setScanningTime(3f);

				break;

			default:
				cs1.setNewSpeed(2f);
				cs2.setNewSpeed(2f);
				xray.setScanningTime(5f);

				break;
		}



		// TTs is used for the baskets
		//GameObject.Find("XRayBack").transform.FindChild("XRayDetector").GetComponent<XRayDetectorScript>().setTtsOn(currLvlConfig.getUseTtsFlag());


		currNumOfPostmen = currLvlConfig.getPostmenWords().Length;


		jumbledParcelWords = currLvlConfig.getParcelWords();

		jumbledPostmenWords = jumbleAnArray(currLvlConfig.getPostmenWords());
		correctOccupiedBasketIDs.Clear();
		occupiedBasketIDs.Clear();


		// Select parcel prefabs.
		DispenserScript ds = GameObject.Find("Dispenser").GetComponent<DispenserScript>();
		ds.reset();

		List<int> availableIndexes = new List<int>();
		for(int i=0; i<parcelPrefabs.Length; i++)
		{
			availableIndexes.Add(i);
		}

		selectedParcelPrefabIndexes = new int[jumbledParcelWords.Length];
		for(int i=0; i<jumbledParcelWords.Length; i++)
		{
			int randIndex = Random.Range(0,availableIndexes.Count);
			int reqPrefabIndex = availableIndexes[randIndex];
			selectedParcelPrefabIndexes[i] = reqPrefabIndex;
			availableIndexes.RemoveAt(randIndex);

			ds.queueItem(parcelPrefabs[reqPrefabIndex],"Parcel-"+i);
		}


		// Spawn postmen.
		triggerPostmenEnter();


		lvlOutcome = new MSLevelOutcome(true);
		
		
		// Spawn boxes.
	}

	

	public new void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		base.respondToEvent(para_sourceID,para_eventID,para_eventData);

		if((!hasRegisteredInit)&&(isInitialised))
		{
			startTimestamp = Time.time;
			hasRegisteredInit = true;
			goaheadWithInit();
		}
		else if(para_eventID == "PostmenEnterSequence")
		{
			//Debug.Log("Carrier has arrived!");
			GameObject postmenObj = GameObject.Find("PostMen");
			Animator aniScript = postmenObj.transform.GetComponent<Animator>();
			aniScript.Play("IdleCyclists");
			postmenObj.GetComponent<BoxCollider>().enabled = false;

			for(int i=0; i<currNumOfPostmen; i++)
			{
				Transform tmpPostie = postmenObj.transform.FindChild("BigPostie-"+(i));
				Animator tmpPostiAnim = tmpPostie.GetComponent<Animator>();
				tmpPostiAnim.Play("IdleCylists");
			}

			// Turn main conveyor on.
			GameObject mainConveyorObj = GameObject.Find("ConveyerRight").gameObject;
			ConveyorScript cs1 = mainConveyorObj.GetComponent<ConveyorScript>();
			cs1.toggleConveyorOnState();

			// Release the first item.
			DispenserScript ds = GameObject.Find("Dispenser").GetComponent<DispenserScript>();
			ds.giveReleaseGoAhead();

			//triggerPostmenExit();
		}
		else if(para_eventID == "PostmenExitSequence")
		{
			//Debug.Log("Carrier lost!");
			Destroy(GameObject.Find("PostMen"));
			numOfFinishedBikes++;
			bool winFlag = checkNHandleWinningCondition();
			if( ! winFlag)
			{
				genNextLevel();
			}
			else
			{
				// Record last outcome.
				buildNRecordConfigOutcome(null);
			}
		}
		else if(para_eventID == "ClickEvent")
		{
			System.Object[] parsedEventData = (System.Object[]) para_eventData;
			float[] clickPos = (float[]) parsedEventData [0];

			RaycastHit hitInf;
			if(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(clickPos[0],clickPos[1],0)),out hitInf))
			{
				checkNHandlePostmanWordReveal(hitInf);
			}

		}
		else if(para_eventID == "DragStart")
		{
			triggerSoundAtCamera("Blip");
		}
		else if(para_eventID == "DragRelease")
		{
			System.Object[] parsedEventData = (System.Object[]) para_eventData;
			string relatedGhostName = (string) parsedEventData[0];
			Bounds finalGhostBounds = (Bounds) parsedEventData[1];

			triggerSoundAtCamera("Blop");

			GameObject postmenObj = GameObject.Find("PostMen");

			finalGhostBounds = new Bounds(new Vector3(finalGhostBounds.center.x,finalGhostBounds.center.y,postmenObj.transform.position.z),finalGhostBounds.size);

			
			for(int i=0; i<jumbledPostmenWords.Length; i++)
			{
				Transform reqBasket = postmenObj.transform.FindChild("Basket-"+i);
				Bounds reqBounds = reqBasket.renderer.bounds;

				if(finalGhostBounds.Intersects(reqBounds))
				{
					if(occupiedBasketIDs.Contains(i))
					{
						// Basket is already occupied.

					}
					else
					{
						occupiedBasketIDs.Add(i);

						ItemDropIntoBasketScript idibs = transform.gameObject.AddComponent<ItemDropIntoBasketScript>();
						idibs.registerListener("AcScen",this);
						idibs.init(GameObject.Find(relatedGhostName),reqBasket.gameObject,basketBasePrefab,false);
					}
				}
			}
		}
		else if(para_eventID == "BasketReceiveComplete")
		{
			// Check if the item is in the correct basket.

			System.Object[] parsedEventData = (System.Object[]) para_eventData;
			GameObject parcelObj = (GameObject) parsedEventData[0];
			GameObject basketObj = (GameObject) parsedEventData[1];

			int parcelID = int.Parse(parcelObj.name.Split('-')[1]);
			int basketID = int.Parse(basketObj.name.Split('-')[1]);

			bool successFlag = currLvlConfig.isCorrectParcelPostmanCombination(jumbledParcelWords[parcelID],jumbledPostmenWords[basketID]);

			if(successFlag)
			{
				// Perform good outcome.

				if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(jumbledParcelWords[parcelID],true,jumbledPostmenWords[basketID],currLvlConfig.languageAreas[parcelID],currLvlConfig.difficulties[parcelID] ); }
				numCorrectAttempts++;


				if( ! correctOccupiedBasketIDs.Contains(basketID))
				{
					correctOccupiedBasketIDs.Add(basketID);
				}

				SpriteRenderer sRend = parcelObj.GetComponent<SpriteRenderer>();
				if(sRend != null)
				{
					sRend.color = Color.green;
				}

				checkNHandleEndLevelCondition();
			}
			else
			{
				// Perform bad outcome.
				occupiedBasketIDs.Remove(basketID);

				if(serverCommunication != null) { serverCommunication.wordSolvedCorrectly(jumbledParcelWords[parcelID],false,jumbledPostmenWords[basketID],currLvlConfig.languageAreas[parcelID],currLvlConfig.difficulties[parcelID]); }
				numIncorrectAttempts++;
				numIncorrectAttemptsThisRound++;
				lvlOutcome.addIncorrectAttempt(currLvlConfig.getUnjumbledIndexForPostman(jumbledPostmenWords[basketID]));


				SpriteRenderer sRend = parcelObj.GetComponent<SpriteRenderer>();
				if(sRend != null)
				{
					sRend.color = Color.red;
				}

				//parcelObj.AddComponent<WitherNDestroy>();

				if(numIncorrectAttemptsThisRound>currNumOfPostmen){
					triggerAutocorrectSequence(parcelObj);
				}else
					triggerChiefSequence(parcelObj);
			}
		}else if(para_eventID == "BasketReceiveCompleteAutoCorrect"){


			System.Object[] parsedEventData = (System.Object[]) para_eventData;
			//GameObject parcelObj = (GameObject) parsedEventData[0];
			GameObject basketObj = (GameObject) parsedEventData[1];

			int basketID = int.Parse(basketObj.name.Split('-')[1]);

			if( ! correctOccupiedBasketIDs.Contains(basketID))
			{
				correctOccupiedBasketIDs.Add(basketID);
			}
			checkNHandleEndLevelCondition();


		}else if(para_eventID == "ChiefLeft")
		{
			string tmpName = (string) para_eventData;
			string parcelAbsorbedName = "Parcel-"+int.Parse(tmpName.Split('-')[1]);
			handleReabsorbtionNQueue(parcelAbsorbedName);
		}
		else if(para_eventID == "ItemAbsorbed")
		{
			string parcelAbsorbedName = (string) para_eventData;
			handleReabsorbtionNQueue(parcelAbsorbedName);
		}
		else if(para_sourceID == "XRay")
		{
			if(para_eventID == "XRayScannedWord")
			{

				string scannedWord = ((string) para_eventData).Split('-')[0];
				int parcelID = int.Parse(((string) para_eventData).Split('-')[1]);

				if(serverCommunication != null) { serverCommunication.wordDisplayed(scannedWord,currLvlConfig.languageAreas[parcelID],currLvlConfig.difficulties[parcelID]); }
			}
		}
	}

	public void dropAutocorrect(GameObject parcel,GameObject basket){
		//GameObject.Find(relatedGhostName)
		//reqBasket.gameObject

		SpriteRenderer sRend = parcel.GetComponent<SpriteRenderer>();
		if(sRend != null)
		{
			sRend.color = Color.white;
			sRend.sortingOrder = 5;
		}
		occupiedBasketIDs.Add(int.Parse(basket.name.Split('-')[1]));
		
		ItemDropIntoBasketScript idibs = transform.gameObject.AddComponent<ItemDropIntoBasketScript>();
		idibs.registerListener("AcScen",this);
		idibs.init(parcel,basket,basketBasePrefab,true);


	}


	private void handleReabsorbtionNQueue(string para_absorbedItemGObjName)
	{
		string parcelAbsorbedName = para_absorbedItemGObjName;

		GameObject mainConveyorObj = GameObject.Find("ConveyerRight").gameObject;
		ConveyorScript cs1 = mainConveyorObj.GetComponent<ConveyorScript>();
		cs1.detachObject(parcelAbsorbedName);
		
		GameObject returnConveyorObj = GameObject.Find("ReturnConveyerLeft");
		ConveyorScript cs2 = returnConveyorObj.GetComponent<ConveyorScript>();
		cs2.detachObject(parcelAbsorbedName);
		
		
		int parcelID = int.Parse(parcelAbsorbedName.Split('-')[1]);
		Transform reqPrefab = parcelPrefabs[selectedParcelPrefabIndexes[parcelID]];
		DispenserScript ds = GameObject.Find("Dispenser").GetComponent<DispenserScript>();
		ds.queueItem(reqPrefab,parcelAbsorbedName);
	}

	public string getWordAssociatedWithParcelID(int para_parcelID)
	{
		if((para_parcelID >= 0)&&(para_parcelID < jumbledParcelWords.Length))
		{
			return jumbledParcelWords[para_parcelID];
		}
		else
		{
			return "ERROR";
		}
	}

	public void triggerAutocorrectSequence(GameObject para_objToPickup)
	{
		GameObject backdropObj = GameObject.Find("Backdrop");
		Bounds backdropBounds = backdropObj.renderer.bounds;

		Vector3 chiefStartPos = new Vector3(backdropBounds.max.x + 3f,//(postChiefPrefab.renderer.bounds.size.x),
		                                    postChiefLevelDest.y,
		                                    postChiefLevelDest.z);
		
		Vector3 chiefDestPos = new Vector3(backdropBounds.min.x - 3f,//(postChiefPrefab.renderer.bounds.size.x/2f),
		                                   chiefStartPos.y,
		                                   chiefStartPos.z);


		
		GameObject basketTarget = null;
		GameObject postmenObj = GameObject.Find("PostMen");


		for(int i=0; i<jumbledPostmenWords.Length; i++)
		{

			if(currLvlConfig.isCorrectParcelPostmanCombination(jumbledParcelWords[int.Parse (para_objToPickup.name.Split('-')[1])],jumbledPostmenWords[i])){

				basketTarget = postmenObj.transform.FindChild("Basket-"+i).gameObject;
			}

		}


		PostChiefAutocorrect pcs = transform.gameObject.AddComponent<PostChiefAutocorrect>();
		pcs.registerListener("AcScen",this);

		if(para_objToPickup.transform.position.x>basketTarget.transform.position.x)
			pcs.init(para_objToPickup,basketTarget,chiefStartPos,chiefDestPos,postChiefPrefab,this);
		else
			pcs.init(para_objToPickup,basketTarget,chiefDestPos,chiefStartPos,postChiefPrefab,this);

	}


	public void triggerChiefSequence(GameObject para_objToPickup)
	{
		GameObject backdropObj = GameObject.Find("Backdrop");
		Bounds backdropBounds = backdropObj.renderer.bounds;

		Vector3 chiefStartPos = new Vector3(backdropBounds.max.x + 3f,//(postChiefPrefab.renderer.bounds.size.x),
		                                    postChiefLevelDest.y,
		                                    postChiefLevelDest.z);

		Vector3 chiefDestPos = new Vector3(backdropBounds.min.x - 3f,//(postChiefPrefab.renderer.bounds.size.x/2f),
		                                   chiefStartPos.y,
		                                   chiefStartPos.z);

		PostChiefScript pcs = transform.gameObject.AddComponent<PostChiefScript>();
		pcs.registerListener("AcScen",this);
		pcs.init(para_objToPickup,chiefStartPos,chiefDestPos,postChiefPrefab);
	}


	protected override void pauseScene(bool para_pauseState)
	{
		paused = para_pauseState;

		ClickDetector cd = transform.gameObject.GetComponent<ClickDetector>();
		if(cd!=null)
			cd.toggleInputStatus(!paused);

		GhostDragScript gds = transform.gameObject.GetComponent<GhostDragScript>();
		if(gds!=null)
			gds.setInputFlag(!paused);

	}
	
	protected override bool checkNHandleWinningCondition()
	{
		updateActivityProgressMetaData(numOfFinishedBikes/(reqBikesForWin*1.0f));
		bool playerHasWon = (numOfFinishedBikes >= reqBikesForWin);
		
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
		recordOutcomeForConfig(lvlOutcome);
		lvlOutcome = new MSLevelOutcome(true);
	}

	protected override GameyResultData buildGameyData()
	{
		float timeDiff = endTimestamp - startTimestamp;
		int minutes = (int) (timeDiff / 60f);
		int seconds = (int) (((timeDiff / 60f) - minutes) * 60f);

		MSGameyResultData reqData = new MSGameyResultData(numCorrectAttempts,numIncorrectAttempts,minutes,seconds);
		return reqData;
	}

	protected override string generateGoalTextAppend()
	{
		return (" "+reqBikesForWin);
	}

	private void checkNHandleEndLevelCondition()
	{
		if(correctOccupiedBasketIDs.Count == jumbledParcelWords.Length)
		{
			correctOccupiedBasketIDs.Clear();

			// Switch main conveyor off.
			GameObject mainConveyorObj = GameObject.Find("ConveyerRight").gameObject;
			ConveyorScript cs1 = mainConveyorObj.GetComponent<ConveyorScript>();
			cs1.toggleConveyorOnState();

			// Send postmen out.
			triggerPostmenExit();
		}
	}

	private bool checkNHandlePostmanWordReveal(RaycastHit para_hitInf)
	{
		bool successFlag = false;

		RaycastHit hitInf = para_hitInf;

		if((hitInf.collider.gameObject.name.Contains("Basket"))
		   ||(hitInf.collider.gameObject.name.Contains("Postie")))
		{
			Debug.Log("Hit: "+hitInf.collider.gameObject.name);

			successFlag = true;
		}

		if(successFlag)
		{
			int postmanID = int.Parse(hitInf.collider.gameObject.name.Split('-')[1]);

			Transform reqBasket = hitInf.collider.transform.parent.FindChild("Basket-"+postmanID);
			Transform reqPostman = hitInf.collider.transform.parent.FindChild("BigPostie-"+postmanID);

			PostmanItemRevealScript pirs = transform.gameObject.AddComponent<PostmanItemRevealScript>();
			pirs.registerListener("AcScen",this);
			pirs.init(reqPostman.gameObject,reqBasket.gameObject,jumbledPostmenWords[postmanID],wordBoxPrefab,currLvlConfig.getUseTtsFlag());
		}

		return successFlag;
	}

	private void triggerPostmenEnter()
	{
		Destroy(GameObject.Find("PostMen"));
		
		GameObject backdropObj = GameObject.Find("Backdrop");
		Bounds backdropBounds = backdropObj.renderer.bounds;
		
		Transform reqPostmenPrefab = postmen3Prefab;
		
		if(jumbledPostmenWords.Length == 4)	{ reqPostmenPrefab = postmen4Prefab; }
		else { reqPostmenPrefab = postmen3Prefab; }
		
		Vector3 reqPostmenSpawnPos = new Vector3(backdropBounds.max.x + (reqPostmenPrefab.GetComponent<BoxCollider>().size.x),
		                                         postmenLevelDest.y,
		                                         postmenLevelDest.z);
		
		Transform nwPostmen = (Transform) Instantiate(reqPostmenPrefab,reqPostmenSpawnPos,Quaternion.identity);
		nwPostmen.name = "PostMen";

		string parentCycleAnimName = "cycle";
		//if(currNumOfPostmen == 3) { parentCycleAnimName = "cycle2"; }
		nwPostmen.gameObject.GetComponent<Animator>().Play(parentCycleAnimName);
		for(int i=0; i<currNumOfPostmen; i++)
		{
			Transform tmpPostie = nwPostmen.transform.FindChild("BigPostie-"+(i));
			Animator tmpPostiAnim = tmpPostie.GetComponent<Animator>();
			tmpPostiAnim.Play("Cycling");
		}

		triggerSoundAtCamera("BicycleBell");
		
		CustomAnimationManager caMang = nwPostmen.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> cmdBatchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> cmdBatch1 = new List<AniCommandPrep>();
		cmdBatch1.Add(new AniCommandPrep("MoveToLocation",1, new List<System.Object>() { new float[3] {postmenLevelDest.x,postmenLevelDest.y,postmenLevelDest.z}, postmenDistPerSec }));
		cmdBatchList.Add(cmdBatch1);
		caMang.init("PostmenEnterSequence",cmdBatchList);
		caMang.registerListener("AcScen",this);
	}

	private void triggerPostmenExit()
	{
		GameObject backdropObj = GameObject.Find("Backdrop");
		Bounds backdropBounds = backdropObj.renderer.bounds;

		GameObject postmenObj = GameObject.Find("PostMen");
		Animator aniScript = postmenObj.transform.GetComponent<Animator>();
		aniScript.speed = 1;
		postmenObj.GetComponent<BoxCollider>().enabled = true;

		string parentCycleAnimName = "cycle";
		//if(currNumOfPostmen == 3) { parentCycleAnimName = "cycle2"; }
		postmenObj.GetComponent<Animator>().Play(parentCycleAnimName);
		for(int i=0; i<currNumOfPostmen; i++)
		{
			Transform tmpPostie = postmenObj.transform.FindChild("BigPostie-"+(i));
			Animator tmpPostiAnim = tmpPostie.GetComponent<Animator>();
			tmpPostiAnim.Play("Cycling");
		}

		Vector3 offScreenPostmenPos = new Vector3(backdropBounds.min.x - (postmenObj.GetComponent<BoxCollider>().bounds.size.x),
		                                          postmenObj.transform.position.y,
		                                          postmenObj.transform.position.z);

		triggerSoundAtCamera("BicycleBellLeave");

		CustomAnimationManager caMang = postmenObj.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> cmdBatchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> cmdBatch1 = new List<AniCommandPrep>();
		cmdBatch1.Add(new AniCommandPrep("MoveToLocation",1, new List<System.Object>() { new float[3] {offScreenPostmenPos.x,offScreenPostmenPos.y,offScreenPostmenPos.z}, postmenDistPerSec }));
		cmdBatchList.Add(cmdBatch1);
		caMang.init("PostmenExitSequence",cmdBatchList);
		caMang.registerListener("AcScen",this);
	}

	private string[] jumbleAnArray(string[] para_srcArr)
	{
		string[] srcArr = para_srcArr;
		List<string> tmpList = new List<string>();
		for(int i=0; i<srcArr.Length; i++)
		{
			tmpList.Add(srcArr[i]);
		}
		string[] destArr = new string[srcArr.Length];
		for(int i=0; i<srcArr.Length; i++)
		{
			int randIndex = Random.Range(0,tmpList.Count);
			destArr[i] = tmpList[randIndex];
			tmpList.RemoveAt(randIndex);
		}
		return destArr;
	}

	private void triggerSoundAtCamera(string para_soundFileName)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		Transform  sfxPrefab = Resources.Load<Transform>("Prefabs/SFxBox");
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = 1f;
		audS.Play();
	}
}

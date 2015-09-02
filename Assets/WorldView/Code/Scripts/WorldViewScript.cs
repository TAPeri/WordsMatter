/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

using UnityEngine;
using System.Collections.Generic;


public class WorldViewScript : MonoBehaviour, CustomActionListener
{
	public Transform graphNodePrefab;
	public Transform blankCollisionBox;
	public Transform movementMarkerPrefab;
	
	public Transform diagBubble_solomon;
	public Transform diagBubble_serenader;

	
	public RuntimeAnimatorController avController_NonLooping;
	
	public bool setupReady = false;
	

	int[] mapSize = {48,48};
	GridProperties gPropMapWorldBounds;
	ColoredNavGraph worldNavGraph;
	ITerrainHandler terrainHandle;

	ScrollCam scrCam;
	
	bool isPaused;
	bool playerNavigationFlag;
	
	float playerWalkSpeed = 2f;

	int[] currPlayerDestCell;
	
	bool dialogEventRequired;
	int dialogTargetCharID;
	GameObject dialogTargetGObj;
	ApplicationID selectedActivityID;
	
	GameObject meetingPersonGObj;

	float movementMarker_Z = -0.5f;
	
	bool[] upAxisArr = new bool[3] { false, true, false };
	


	List<AbsInputDetector> inputDetectors;

	
	//bool hasPerformedUnlockSequence = false;

	//EventSlot selectedEvent;
	Mode activityLauncher;
	string launcherDetails;

	//int intendedEventSlotID = -1;
	

	//int tmpQuestDestNpcID = -1;
	
	bool isAttemptingToReachPlayerHouse = false;
	bool transferingToHouse = false;
	
	//ExternalParams extParams = null;


	bool isStillPerformingUnlockSequence;


	GameObject[] walkers;

	List<int> visibleWalkers;


	Rect clickableArea;

	AudioSource ambientSound;
	void Start()
	{

		float iconifiedBounds_Width = Screen.width * 0.15f;
		float iconifiedBounds_Height = Screen.height * 0.15f;
		if(iconifiedBounds_Width > iconifiedBounds_Height) { iconifiedBounds_Width = iconifiedBounds_Height; }
		else { iconifiedBounds_Height = iconifiedBounds_Width; }
		float iconifiedBounds_X = Screen.width - iconifiedBounds_Width;
		float iconifiedBounds_Y = Screen.height - iconifiedBounds_Height;
		
		clickableArea = new Rect(iconifiedBounds_X,iconifiedBounds_Y,iconifiedBounds_Width,iconifiedBounds_Height);


		walkers = new GameObject[LocalisationMang.getNPCnames().Count];
		visibleWalkers = new List<int>();

		isPaused = false;
		currPlayerDestCell = new int[2] {-1,-1};
		playerNavigationFlag = true;

		dialogEventRequired = false;
		dialogTargetCharID = -1;
		
		
		// Disable any debug scene related things.
		makeAllClickablesTransparent();	
		
		// Apply Clothing Config to Main Avatar.
		applyPlayerAvatarClothing();
		clonePlayerAv();

		
		// Calculate World Bounds.
		gPropMapWorldBounds = calculateWorldMapBounds(mapSize);
		WorldSpawnHelper.initObjWithinWorldBounds(blankCollisionBox,
		                                          blankCollisionBox.transform.renderer.bounds.size.x,
		                                          blankCollisionBox.transform.renderer.bounds.size.y,
		                                          "WorldColBox",
		                                          new Rect(gPropMapWorldBounds.x,gPropMapWorldBounds.y,gPropMapWorldBounds.totalWidth,gPropMapWorldBounds.totalHeight),
		                                          null,
		                                          gPropMapWorldBounds.z,
		                                          upAxisArr);
		
		
		// Remove Scroll Cam and use Follow Cam.
		Destroy(GameObject.Find("GlobObj").GetComponent(typeof(ScrollCam)));		
		FollowScript followScrpt = Camera.main.transform.gameObject.AddComponent<FollowScript>();
		followScrpt.init(GameObject.Find("MainAvatar"),new bool[3] { true, true, false });
		
		
		
		
		// Init Nav Graph.
		string bnwMapFilePath = "Textures/WorldView/WorldView_B&W";
		ImgToWorldNavGraphBuilderV2 navBuilder = new ImgToWorldNavGraphBuilderV2(bnwMapFilePath,gPropMapWorldBounds);
		List<ColorGraphTypeInfo> graphRequirements = new List<ColorGraphTypeInfo>()
		{
			new ColorGraphTypeInfo(new string[] { "FreeSpace", "MainCharacterSpawns" },
			new Color[] { Color.white, Color.red }) 
		};
		List<ColoredNavGraph> graphList = navBuilder.constructColorGraphs(graphRequirements);
		worldNavGraph = graphList[0];
		
		
		
		
		List<System.Object> paramList = new List<System.Object>();
		paramList.Add(worldNavGraph);
		paramList.Add(gPropMapWorldBounds);
		terrainHandle = new BasicTerrainHandler();
		terrainHandle.constructTerrainStructures(paramList);

			

		
		
		Animator mainPlayerAvatarAni = GameObject.Find("MainAvatar").GetComponent<Animator>();
		//mainPlayerAvatarAni.Play("MainAvatar_walkDown");
		mainPlayerAvatarAni.speed = 0;
		
		
		SoundPlayerScript sps = transform.GetComponent<SoundPlayerScript>();
		//sps.triggerSoundLoopAtCamera("WindAmbient","WorldAmbientBox",0.3f,true);
		ambientSound = sps.triggerSoundLoopAtCamera("Serenade Band Tracks/Serenade_Bass_01.00","WorldAmbientBox",0.3f,true);
		
		// Init input detectors.
		inputDetectors = new List<AbsInputDetector>();
		ClickDetector cd = transform.gameObject.AddComponent<ClickDetector>();
		cd.registerListener("WorldScript",this);
		cd.setMaxDelayBetweenClicks(0.0001f);
		inputDetectors.Add(cd);
		
		

		// Persistent Obj Check.

		int index = 0;


		//Hide all recognised characters and destroy extra ones
		foreach(string para_namePrefix in new string[]{"AvatarChar-","SecChar-"}){

			int counter = 0;
			bool foundItem = true;
			while(foundItem)
			{
				GameObject tmpObj = GameObject.Find(para_namePrefix+""+counter);
				counter++;
				if(tmpObj == null)
				{
					foundItem = false;
				}
				else
				{
					if(index >= walkers.Length){
						Destroy(tmpObj);

					}else{
						rootItemToMapGrid(tmpObj);
						walkers[index++] = tmpObj;
						List<SpriteRenderer> sRends = CommonUnityUtils.getSpriteRendsOfChildrenRecursively(tmpObj);
						if(sRends != null)
						{
							for(int i=0; i<sRends.Count; i++)
							{
								sRends[i].enabled = false;
							}
						}
						tmpObj.collider.enabled = false;
					//tmpObj.SetActive(false);
					}
				}
				
			}
		}

		isStillPerformingUnlockSequence = false;


		GameObject poRef = PersistentObjMang.getInstance();
		control = poRef.GetComponent<ProgressScript>();
		if(control == null)
		{
			control = poRef.AddComponent<ProgressScript>();
		}/*else{
			Debug.Log("Enable progress script");
			control.enabled = true;
		}*/
		
		Debug.LogWarning("Position of avatar not initialised");
		//placeExistingItemInCell("MainAvatar",int[]{});//NEW
		control.attemptActivityDebrief(this);

		// Switch player controls off until the intro effect is over.
		togglePlayerInputState(false);
		setupReady = false;

		Debug.Log("Fading removed");

		respondToEvent("", "FadeEffectDone", null);

	}

			
	
	void OnDestroy()
	{
		if(Camera.main != null)
		{
			Transform ambientSoundBox = Camera.main.transform.FindChild("WorldAmbientBox");
			if(ambientSoundBox != null) { Destroy(ambientSoundBox);	}
			
			Transform footstepsSoundBox = Camera.main.transform.FindChild("FootstepsSoundBox");
			if(footstepsSoundBox != null) {	Destroy(footstepsSoundBox.gameObject); }
		}
	}
	

	
	private void initPlayerMovement(int[] para_mapCell, bool para_stopOneNodeShort, bool para_targetIsPerson)
	{
		Destroy(GameObject.Find("MovementMarker"));
		isAttemptingToReachPlayerHouse = false;
		
		
		int[] cellForPlayerAvatar = getCellForItem("MainAvatar");
		int srcNavNodeID = terrainHandle.getNavNodeIDForCell(cellForPlayerAvatar);
		int destNavNodeID = terrainHandle.getNavNodeIDForCell(para_mapCell);
		
		
		if(srcNavNodeID != destNavNodeID)
		{
			float[] cellCentre = gPropMapWorldBounds.getCellCentre(para_mapCell,true);
			if(cellCentre != null)
			{
				Transform nwMovementMarker = (Transform) Instantiate(movementMarkerPrefab,new Vector3(cellCentre[0],cellCentre[1],movementMarker_Z),Quaternion.identity);
				nwMovementMarker.name = "MovementMarker";
				
				HashSet<int> untraversableCellTypes = null;
				if( ! para_targetIsPerson)
				{
					untraversableCellTypes = new HashSet<int> {worldNavGraph.getTypeIDByName("MainCharacterSpawns")};
				}
				
				List<NavNode> pathNodes = worldNavGraph.searchForPath(srcNavNodeID,destNavNodeID,untraversableCellTypes);
				
				if((pathNodes == null)||(pathNodes.Count == 0))
				{
					Debug.Log("No Path Available");
				}
				else
				{
					currPlayerDestCell[0] = para_mapCell[0];
					currPlayerDestCell[1] = para_mapCell[1];
					
					NavGraphUnityUtils.clearAllNavNodeHighlights("WorldNavGraphRender");
					NavGraphUnityUtils.highlightNavNodes("WorldNavGraphRender",pathNodes);
					GameObject playerAvatar = GameObject.Find("MainAvatar");
					NewCharacterNavMovement cnm = playerAvatar.GetComponent<NewCharacterNavMovement>();
					if(cnm == null)
					{
						cnm = playerAvatar.AddComponent<NewCharacterNavMovement>();
						cnm.registerListener("AcScen",this);
					}
					cnm.moveAlongPath(pathNodes,playerWalkSpeed,true,para_stopOneNodeShort);
					
					
					if(Camera.main.transform.FindChild("FootstepsSoundBox") == null)
					{
						SoundPlayerScript sps = transform.GetComponent<SoundPlayerScript>();
						sps.triggerSoundLoopAtCamera("FootstepsGravel","FootstepsSoundBox",1f,true);
					}
				}
			}
		}
	}

	int[] questGiverStartingPoint;
	public void sendQuestGiverToCharacter(int walkerID,int targetID)
	{
		questGiverStartingPoint = gPropMapWorldBounds.hashPointToCell( new float[2] { walkers[walkerID].transform.position.x, walkers[walkerID].transform.position.y },true);

		if(walkerID!=targetID){

			int[] personCell = gPropMapWorldBounds.hashPointToCell( new float[2] { walkers[targetID].transform.position.x, walkers[targetID].transform.position.y },true);
			WVPedestrianScript wvps =walkers[walkerID].GetComponent<WVPedestrianScript>();

			wvps.runToWitnessMeeting((WorldNode) worldNavGraph.getNode(terrainHandle.getNavNodeIDForCell(personCell)));
		}

	}

	public void resetQuestGiverPosition(int walkerID){

		if(LocalisationMang.checkIfNpcIsMainChar(walkerID)){//Return to activity location
			WVPedestrianScript wvps =walkers[walkerID].GetComponent<WVPedestrianScript>();
			Debug.Log("Go home :"+questGiverStartingPoint[0]+" "+questGiverStartingPoint[1]);
			wvps.goBackHome((WorldNode) worldNavGraph.getNode(terrainHandle.getNavNodeIDForCell(questGiverStartingPoint)));
		}else{
			WVPedestrianScript wvps =walkers[walkerID].GetComponent<WVPedestrianScript>();
			Debug.Log("Abandon meeting :");

			wvps.goBackHome();
		}
		questGiverStartingPoint = null;

	}

	
	public void moveToCharacter(int para_charID)
	{

		dialogTargetGObj = walkers[para_charID];
		
		if(dialogTargetGObj != null)
		{
			dialogEventRequired = true;
			dialogTargetCharID = para_charID;
			
			meetingPersonGObj = dialogTargetGObj;
			WVPedestrianScript wvps = meetingPersonGObj.GetComponent<WVPedestrianScript>();
			if(wvps != null)
			{
				int[] personCell = gPropMapWorldBounds.hashPointToCell( new float[2] { dialogTargetGObj.transform.position.x, dialogTargetGObj.transform.position.y },true);
				wvps.goToMeetingCell((WorldNode) worldNavGraph.getNode(terrainHandle.getNavNodeIDForCell(personCell)));
			}
			
			initPlayerMovement(gPropMapWorldBounds.hashPointToCell( new float[2] { dialogTargetGObj.transform.position.x, dialogTargetGObj.transform.position.y },true),true,true);
		}
	}



	int targetTalker = -1;
	public void talking2Character(int charID){
		targetTalker = charID;
		if(walkers[charID]!=null){

			WVPedestrianScript wvps = walkers[charID].GetComponent<WVPedestrianScript>();
			if(wvps != null)
			{
				
				wvps.startConversationBehaviour();
			}


		}
	}


	public void leavingCharacter(){
		
		if(walkers[targetTalker]!=null){
			
			WVPedestrianScript wvps = walkers[targetTalker].GetComponent<WVPedestrianScript>();
			if(wvps != null)
			{
				
				wvps.endConversationBehaviour();
			}
			
			
		}
	}

	public void cancelMove(){

		GameObject playerAvatar = GameObject.Find("MainAvatar");
		if(playerAvatar.GetComponent<NewCharacterNavMovement>()!=null)
			playerAvatar.GetComponent<NewCharacterNavMovement>().setToIdle();
		
		if(meetingPersonGObj != null)
		{
			WVPedestrianScript wvps = meetingPersonGObj.GetComponent<WVPedestrianScript>();
			if(wvps != null)
			{
				wvps.abandonMeeting();
			}
			meetingPersonGObj = null;
		}

		Transform footstepsSoundBox = Camera.main.transform.FindChild("FootstepsSoundBox");
		if(footstepsSoundBox != null) {	Destroy(footstepsSoundBox.gameObject); }

	}


	List<Rect> unclickableAreas = new List<Rect>();

	public void addUnclickable(Rect r){
		unclickableAreas.Add(r);
	}

	public void clearUnclickable(){
		unclickableAreas.Clear();
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{

		//UnityEngine.Debug.Log("Event: "+para_sourceID+" "+para_eventID);

		if(para_sourceID == "InputDetector")
		{
			if(para_eventID == "ClickEvent")
			{
				if( ! isPaused)
				{
					if(playerNavigationFlag)
					{
						System.Object[] parsedEventData = (System.Object[]) para_eventData;
						float[] clickPos = (float[]) parsedEventData [0];

						if(( ! clickableArea.Contains(new Vector2(clickPos[0],Screen.height - clickPos[1])))){
							bool clickAllowed = true;
							foreach(Rect r in unclickableAreas)
								if (r.Contains(new Vector2(clickPos[0],Screen.height-clickPos[1]))){
									clickAllowed = false;
									break;
								}

							if(clickAllowed)
								handleWorldClick(new Vector3(clickPos[0],clickPos[1],0));
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
				
				Transform footstepsSoundBox = Camera.main.transform.FindChild("FootstepsSoundBox");
				if(footstepsSoundBox != null)
				{
					Destroy(footstepsSoundBox.gameObject);
				}

				if(isAttemptingToReachPlayerHouse)
				{
					UnityEngine.Debug.Log("Reached home");

					performPlayerHouseLoad();
				}
				else if(dialogEventRequired)
				{
					UnityEngine.Debug.Log("Reached dialog point");

					control.triggerAppropriateDialogue(dialogTargetCharID);
				}
			}

		}
		else if(para_eventID == "DelayEnd")
		{
			if( ! isStillPerformingUnlockSequence)
			{
				Debug.Log("Waky waky");

				//togglePlayerInputState(true);
				//setGhostbookVisibility(true);
			}else{
				Debug.Log("Not the delay");
			}
			
			//if( ! hasPerformedUnlockSequence)
			//{
			//displayUnlockedWorldCharacters();
			//hasPerformedUnlockSequence = true;
			//}
		}
		else if(para_sourceID == "UnlockSequenceEffectScript")
		{
			if(para_eventID == "AllDone")
			{
				isStillPerformingUnlockSequence = false;
				//togglePlayerInputState(true);
				//setGhostbookVisibility(true);
				
				GameObject.Find("MainAvatar").transform.FindChild("HuhBub").renderer.enabled = false;
				SoundPlayerScript sps = transform.GetComponent<SoundPlayerScript>();
				sps.triggerSoundAtCamera("ManHuh2");
				
				FollowScript followScrpt = Camera.main.transform.gameObject.AddComponent<FollowScript>();
				followScrpt.init(GameObject.Find("MainAvatar"),new bool[3] { true, true, false });
			}
		}
		else if(para_eventID == "FadeEffectDone")
		{
			if(setupReady == false)
			{

				setupReady = true;


			}
			else if(transferingToHouse)
			{
				GameObject poRef = PersistentObjMang.getInstance();
				DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
				ds.insertData("NextSceneToLoad","PlayerCustomisationScene");
				Application.LoadLevel("LoadingScene");
			}
		}

	}


	public void togglePauseScene(bool para_pauseState,bool ignoreFirst)
	{

		/*foreach(GameObject obj in walkers){
			Debug.Log(obj.name+" "+(!para_pauseState));
			obj.SetActive(!para_pauseState);
		}*/

		// Pause.
		isPaused = para_pauseState;

		if(isPaused)
			ambientSound.Pause();
		else
			ambientSound.Play();

		ignoreFirstClick = ignoreFirst;
		ignoreTime = Time.realtimeSinceStartup;
		togglePlayerInputState(!isPaused);
		
		ScrollCam scrCam = (ScrollCam) (GameObject.Find("GlobObj").GetComponent(typeof(ScrollCam)));
		if(scrCam != null) { scrCam.enabled = !isPaused; }
	}

	double ignoreTime = 0.0;

	void Update(){

		if(Time.realtimeSinceStartup-ignoreTime>0.5)
			ignoreFirstClick = false;

	}


	public void togglePauseScene(bool para_pauseState)
	{
		/*foreach(GameObject obj in walkers){
			Debug.Log(obj.name+" "+(!para_pauseState));
			obj.SetActive(!para_pauseState);
		}*/

		// Pause.
		isPaused = para_pauseState;

		if(isPaused)
			ambientSound.Pause();
		else
			ambientSound.Play();

		if(!para_pauseState)
			ignoreFirstClick = true;
		
		togglePlayerInputState(!isPaused);
		
		ScrollCam scrCam = (ScrollCam) (GameObject.Find("GlobObj").GetComponent(typeof(ScrollCam)));
		if(scrCam != null) { scrCam.enabled = !isPaused; }
	}
	
	public void togglePlayerInputState(bool para_state)
	{
		playerNavigationFlag = para_state;
		
		ClickDetector tmpCD = transform.gameObject.GetComponent<ClickDetector>();
		if(tmpCD != null)
		{
			tmpCD.enabled = para_state;
		}		
	}


	bool ignoreFirstClick = false;
	
	private void handleWorldClick(Vector3 para_pt)
	{

		if(ignoreFirstClick){

			ignoreFirstClick = false;
			return;
		}

		control.move();
		
		RaycastHit hitInf;
		if(Physics.Raycast(Camera.main.ScreenPointToRay(para_pt),out hitInf))
		{
			int[] clickCell = gPropMapWorldBounds.hashPointToCell(new float[2] { hitInf.point.x, hitInf.point.y },true);
			
			GameObject colGameObj = hitInf.collider.gameObject;
			

			//To close initiated dialaog by touching anywheere
			if(meetingPersonGObj != null)
			{
				WVPedestrianScript wvps = meetingPersonGObj.GetComponent<WVPedestrianScript>();
				if(wvps != null)
				{
					wvps.abandonMeeting();
				}
				meetingPersonGObj = null;
			}
			
			// Check what the player hit.
			if(colGameObj.layer == LayerMask.NameToLayer("WorldClickable"))
			{
				Debug.Log("Clicked Interactive World Object");
			}
			else if(colGameObj.name == "PlayerHouse")
			{
				int[] playerHousePorchCellCoords = getCellForItem("PlayerHousePorchCell");
				int[] currPlayerCell = getCellForItem("MainAvatar");
				
				//extParams = null;
				
				if((currPlayerCell[0] == playerHousePorchCellCoords[0])&&(currPlayerCell[1] == playerHousePorchCellCoords[1]))
				{
					isAttemptingToReachPlayerHouse = true;
					performPlayerHouseLoad();
				}
				else
				{
					initPlayerMovement(playerHousePorchCellCoords,false,false);
					isAttemptingToReachPlayerHouse = true;
				}
			}
			else if(colGameObj.name == "WorldColBox")
			{
				//Debug.Log("Clicked on World Col Box");
				
				
				if(clickCell != null)
				{
					bool traversibeFlag = terrainHandle.isCellTraversible(clickCell);
					if(traversibeFlag)
					{
						//Debug.Log("Hit traversible area");
						
						if( ! ((clickCell[0] == currPlayerDestCell[0])&&(clickCell[1] == currPlayerDestCell[1])))
						{
							dialogEventRequired = false;
							dialogTargetCharID = -1;
							dialogTargetGObj = null;
							
							//extParams = null;
							
							initPlayerMovement(clickCell,false,false);
						}
					}
					else
					{
						Debug.Log("Hit NON traversible area");
					}
				}
			}
			else if((colGameObj.name.Contains("AvatarChar"))||(colGameObj.name.Contains("SecChar")))
			{
				int charID = 0;
				if(colGameObj.name.Contains("AvatarChar"))
				{
					charID = int.Parse(colGameObj.name.Split('-')[1]);
				}
				else
				{
					charID = 9 + int.Parse(colGameObj.name.Split('-')[1]);
				}
				//extParams = null;
				
				// Instruct the character to move to the meeting point.
				meetingPersonGObj = colGameObj;
				WVPedestrianScript wvps = meetingPersonGObj.GetComponent<WVPedestrianScript>();
				if(wvps != null)
				{
					wvps.goToMeetingCell((WorldNode) worldNavGraph.getNode(terrainHandle.getNavNodeIDForCell(clickCell)));
				}
				
				// Make player move to the character.
				moveToCharacter(charID);
			}
			else
			{
				Debug.Log("Hit "+colGameObj.name);
			}
		}
	}
	
	
	private List<string> getAllWorldCharacterGObjNames()
	{
		List<string> retList = new List<string>();
		
		string prefix = "AvatarChar-";
		int counter = 0;
		
		bool continueFlag = true;
		while(continueFlag)
		{
			GameObject tmpAvObj = GameObject.Find(prefix + counter);
			
			if(tmpAvObj == null)
			{
				continueFlag = false;
			}
			else
			{
				retList.Add(tmpAvObj.name);
			}
			
			counter++;
		}
		
		return retList;
	}
	

	
	
	private void makeAllClickablesTransparent()
	{
		List<GameObject> reqGameObjs = getAllGameObjectsByLayer(LayerMask.NameToLayer("WorldClickable"));
		if(reqGameObjs != null)
		{
			for(int i=0; i<reqGameObjs.Count; i++)
			{
				Color currColor = reqGameObjs[i].renderer.material.GetColor("_TintColor");
				currColor.a = 0;
				reqGameObjs[i].renderer.material.SetColor("_TintColor",currColor);
			}
		}
	}	
	
	private List<GameObject> getAllGameObjectsByLayer (int layer) 
	{
		GameObject[] reqObjArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
		List<GameObject> reqObjList = new List<GameObject>();
		for(int i=0; i<reqObjArray.Length; i++) 
		{
			if (reqObjArray[i].layer == layer) 
			{
				reqObjList.Add(reqObjArray[i]);
			}
		}
		
		if(reqObjList.Count == 0) 
		{
			return null;
		}
		else
		{
			return reqObjList;
		}
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

	ProgressScript control;


	

	
	private void placeExistingItemInCell(string para_itemName, int[] para_cell)
	{
		GameObject itemGObj = GameObject.Find(para_itemName);
		
		if(itemGObj != null)
		{
			float[] cellCentre = gPropMapWorldBounds.getCellCentre(para_cell,true);
			itemGObj.transform.position = new Vector3(cellCentre[0],cellCentre[1],itemGObj.transform.position.z);
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


	private void rootItemToMapGrid(GameObject itemGObj)
	{
		
		if(itemGObj == null)
		{
			Debug.LogError("Failed to root item. Could not find '"+itemGObj+"'");
		}
		else
		{
			int[] itemCell = gPropMapWorldBounds.hashPointToCell(new float[2] { itemGObj.transform.position.x, itemGObj.transform.position.y },true);
			if(itemCell != null)
			{
				//Debug.Log("Item '"+para_itemName+"' is at cell: ("+itemCell[0]+","+itemCell[1]+")");
			}
			else
			{
				Debug.LogError("Error hashing cell for '"+itemGObj.name+"'");
			}
			
			float[] itemCellCentre = gPropMapWorldBounds.getCellCentre(itemCell,true);
			if(itemCellCentre != null)
			{
				itemGObj.transform.position = new Vector3(itemCellCentre[0],itemCellCentre[1],itemGObj.transform.position.z);
			}
			else
			{
				Debug.LogError("Could not centre '"+itemGObj.name+"' to grid cell");
			}
		}
	}
	
	
	// para_namePrefix example: AvatarChar-   ,  SecChar-






	// NOTE: Displaying world characters in the world does not necessarily mean that they are unlocked in Ghostbook.
	// After the player interacts with characters (completing one of their errands) for the first time, they are automatically unlocked in Ghostbook.
	public void displayWorldCharacters(List<int> para_npcIDsToDisplay)//, bool para_fastMode)
	{
		if(para_npcIDsToDisplay == null) { return; }
		if(para_npcIDsToDisplay.Count <= 0) { return; }

			if(para_npcIDsToDisplay != null)
			{

				for(int i=0; i<para_npcIDsToDisplay.Count; i++)
				{

					int tmpID = para_npcIDsToDisplay[i];

					bool personCanWalk = false;

					personCanWalk = !LocalisationMang.checkIfNpcIsMainChar(tmpID);


					if(walkers[tmpID] != null)
					{
						visibleWalkers.Add(tmpID);
						List<SpriteRenderer> sRends = CommonUnityUtils.getSpriteRendsOfChildrenRecursively(walkers[tmpID]);
						if(sRends != null)
						{
							for(int k=0; k<sRends.Count; k++)
							{
								sRends[k].enabled = true;
							}
						}
						walkers[tmpID].collider.enabled = true;
						
						// Add also a Pedestrian script (contains the pedestrian state machine).

						Destroy(walkers[tmpID].GetComponent<WVPedestrianScript>());
						WVPedestrianScript pedScript = walkers[tmpID].AddComponent<WVPedestrianScript>();
						pedScript.init(mapSize,gPropMapWorldBounds,worldNavGraph,terrainHandle,personCanWalk);

					}
				}
				
				if(Camera.main.transform.gameObject.GetComponent<FollowScript>() == null)
				{
					FollowScript followScrpt = Camera.main.transform.gameObject.AddComponent<FollowScript>();
					followScrpt.init(GameObject.Find("MainAvatar"),new bool[3] { true, true, false });
				}
			}
		//}
	}


	
	public bool isNpcVisibleInWorld(int para_npcID)
	{

		return visibleWalkers.Contains(para_npcID);

		/*GameObject persistantObj = PersistentObjMang.getInstance();
		DatastoreScript datastoreScript = persistantObj.GetComponent<DatastoreScript>();
		WorldStateData wsd = (WorldStateData) datastoreScript.getData("WorldViewState");
		return wsd.isNpcVisibleInWorld(para_npcID);*/
	}
	

	
	private void performPlayerHouseLoad()
	{
		//TODO
		//if(selectedEvent == null)
		//{
			if( ! transferingToHouse)
			{
				isAttemptingToReachPlayerHouse = false;
				transferingToHouse = true;
				togglePlayerInputState(false);
				
				// Activate fader.
				Vector3 camPos = Camera.main.transform.position;
				
				Transform faderPrefab = Resources.Load<Transform>("Prefabs/FaderScreen");
				Transform nwFader = (Transform) Instantiate(faderPrefab,new Vector3(camPos.x,camPos.y,camPos.z + 2f),Quaternion.identity);
				FaderScript fs = nwFader.GetComponent<FaderScript>();
				fs.registerListener("WorldViewScript",this);
				fs.init(Color.clear,Color.black,1f,false);
			}
		//}
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
	
	private void clonePlayerAv()
	{
		GameObject mainAv = GameObject.Find("MainAvatar");
		GameObject diagPlayerAv = ((Transform) Instantiate(mainAv.transform,new Vector3(mainAv.transform.position.x+3000,mainAv.transform.position.y,mainAv.transform.position.z),Quaternion.identity)).gameObject;
		diagPlayerAv.name = "MainAvDiagClone";
	}
	

	
	public void setLoadingIconVisibility(bool para_state)
	{
		Transform smallLoadIcon = Camera.main.transform.FindChild("ProcessWaitSmallIcon");
		if(smallLoadIcon != null)
		{
			smallLoadIcon.gameObject.SetActive(para_state);
			smallLoadIcon.FindChild("RotArrow").GetComponent<Animator>().Play("LoadingRotate");
		}
	}

	

	
	public void triggerDelayNShowSafeUI(int para_delaySeconds)
	{
		float tmpDelaySec = para_delaySeconds;
		if(para_delaySeconds < 0)
		{
			tmpDelaySec = 2;
		}
		DelayForInterval delayScript = transform.gameObject.AddComponent<DelayForInterval>();
		delayScript.registerListener("WorldViewScript",this);
		delayScript.init(tmpDelaySec);
	}
	

}

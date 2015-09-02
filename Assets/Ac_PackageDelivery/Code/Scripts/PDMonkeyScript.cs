/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class PDMonkeyScript : MonoBehaviour, CustomActionListener
{


	public MonkeyState currState;
	NewCharacterNavMovement cnm;
	GameObject monkeyDetectorGObj;

	//int[] mapSize;
	GridProperties gPropMapWorldBounds;
	ColoredNavGraph worldNavGraph;
	ITerrainHandler terrainHandle;
	GameObject playerAvatar;

	float monkeyWalkSpeed = 2f;
	float monkeyRunSpeed = 4f;
	float currMonkeySpeed;


	// IDLE params.
	float idle_timeOfLastFaceSwitch;
	float idle_faceSwitchDelaySec = 3f;
	string[] faceDirections = {"N","S","W","E"};


	string stolenPackageName;
	//int escapeDistanceInNodes = 7;


	NavNode dropOffNode;
	NavNode escapeNode;

	bool playerHasItems = true;

	Transform sfxPrefab;

	int currAlertCount = 0;


	public enum MonkeyState
	{
		IDLE,
		ALERTED,
		CHASING,
		STEALING_PACKAGE,
		ESCAPING_WITH_PACKAGE,
		RETREATING,
		EATING_BANANA,
		COUNTER
	}



	void Start()
	{
		setState(MonkeyState.IDLE);
		cnm = transform.gameObject.GetComponent<NewCharacterNavMovement>();
		if(cnm == null) { cnm = transform.gameObject.AddComponent<NewCharacterNavMovement>(); }
		cnm.registerListener("MonkeyScript",this);
		playerAvatar = GameObject.Find("MainAvatar");
		currMonkeySpeed = monkeyWalkSpeed;
	}

	void Update()
	{
		if(currState == MonkeyState.IDLE)
		{
			// Check for player detection.
			bool playerDetected = false;
			if(playerHasItems)
			{
				if(monkeyDetectorGObj == null) { monkeyDetectorGObj = transform.FindChild("WorldPivot").FindChild("MonkeyDetector").gameObject; }
				if(monkeyDetectorGObj.renderer.bounds.Contains(playerAvatar.transform.position))
				{
					Debug.Log("Monkey detected Player!");
					setState(MonkeyState.ALERTED);
					playerDetected = true;
				}
			}

			// Check if time to face another direction.
			if( ! playerDetected)
			{
				if((Time.time - idle_timeOfLastFaceSwitch) >= idle_faceSwitchDelaySec)
				{
					cnm.faceDirection(faceDirections[Random.Range(0,faceDirections.Length)]);
					idle_timeOfLastFaceSwitch = Time.time;
				}
			}
		}
		else if(currState == MonkeyState.ALERTED)
		{
			initCharacterMovement(getCellForItem("MainAvatar"),true,false);
			setState(MonkeyState.CHASING);
		}
	}


	private void setState(MonkeyState para_state)
	{
		currState = para_state;

		if(para_state == MonkeyState.IDLE)
		{
			transform.GetComponent<SpriteRenderer>().color = Color.white;
			currMonkeySpeed = monkeyWalkSpeed;
			idle_timeOfLastFaceSwitch = Time.time;
			currAlertCount = 0;
		}
		else if(para_state == MonkeyState.ALERTED)
		{
			currAlertCount++;
		}
		else if(para_state == MonkeyState.CHASING)
		{
			if(currAlertCount == 1)
			{
				triggerSoundAtCamera("MonkeyAngry");
				transform.GetComponent<SpriteRenderer>().color = Color.red;
			}
		}else if(para_state == MonkeyState.COUNTER){

		}else if(para_state == MonkeyState.STEALING_PACKAGE)
		{
			GameObject.Find("GlobObj").GetComponent<AcPackageDeliveryScenario>().handleMonkeyPackageSteal(transform.gameObject.name);
		}
		else if(para_state == MonkeyState.ESCAPING_WITH_PACKAGE)
		{
			// Select end point of monkey. The monkey should drop off the package about mid way.
			triggerSoundAtCamera("MonkeyLaugh");

			currMonkeySpeed = monkeyRunSpeed;

			GameObject stolenItem = GameObject.Find(stolenPackageName);
			Transform itemHolder = transform.FindChild("WorldPivot").FindChild("ItemHolder");
			itemHolder.parent = null;
			stolenItem.transform.position = new Vector3(itemHolder.position.x,itemHolder.position.y,itemHolder.position.z);
			itemHolder.parent = transform.FindChild("WorldPivot");
			stolenItem.transform.parent = itemHolder;


			List<NavNode> escapeNodes = null;
			escapeNodes = worldNavGraph.getChildNodesAtHopDistance(worldNavGraph.getNode(terrainHandle.getNavNodeIDForCell(getCellForItem(transform.gameObject.name))),
						                                         8,
						                                         new HashSet<int>() {worldNavGraph.getTypeIDByName("MonkeySpawn")},
																 new HashSet<int>() {worldNavGraph.getNode(terrainHandle.getNavNodeIDForCell(getCellForItem("MainAvatar"))).getNodeID()});

			if((escapeNodes == null)||(escapeNodes.Count > 0))
			{
				// Try again, this time allow passing through the player.

				escapeNodes = worldNavGraph.getChildNodesAtHopDistance(worldNavGraph.getNode(terrainHandle.getNavNodeIDForCell(getCellForItem(transform.gameObject.name))),
				                                                       8,
				                                                       new HashSet<int>() {worldNavGraph.getTypeIDByName("MonkeySpawn")},
																	   null);

				if((escapeNodes == null)||(escapeNodes.Count > 0))
				{
					// Select random location on the map.

					List<int> freeSpaceNodeIDs = worldNavGraph.getNodesOfType(worldNavGraph.getTypeIDByName("FreeSpace"));
					escapeNodes = new List<NavNode>();
					escapeNodes.Add(worldNavGraph.getNode(freeSpaceNodeIDs[Random.Range(0,freeSpaceNodeIDs.Count)]));
				}
			}


			// Select escape node out of potential escapes.
			NavNode chosenEscapeNode = escapeNodes[Random.Range(0,escapeNodes.Count)];

			List<NavNode> escapepath = worldNavGraph.searchForPath(terrainHandle.getNavNodeIDForCell(getCellForItem(transform.gameObject.name)),
			                           							   chosenEscapeNode.getNodeID());

			escapeNode = escapepath[escapepath.Count-1];


			// Select package drop off from the escape path.
			dropOffNode = escapepath[ (int) (escapepath.Count/2f) ];

			//List<NavNode> pathToDropOff = worldNavGraph.searchForPath(terrainHandle.getNavNodeIDForCell(getCellForItem(transform.gameObject.name)),
			//                                                          dropOffNode.getNodeID());


			initCharacterMovement(getCellForItem(((WorldNode) dropOffNode).getWorldPt()),false,false);
		}
		else if(para_state == MonkeyState.RETREATING)
		{
			Transform stolenItem = transform.FindChild("WorldPivot").FindChild("ItemHolder").FindChild(stolenPackageName);
			stolenItem.parent = null;
			stolenItem.position = new Vector3(transform.position.x,transform.position.y,transform.position.z + 0.1f);
			if(stolenItem.collider != null)
			{
				stolenItem.collider.enabled = true;
			}

			initCharacterMovement(getCellForItem(((WorldNode) escapeNode).getWorldPt()),false,true);

			GameObject.Find("GlobObj").GetComponent<AcPackageDeliveryScenario>().respondToEvent(transform.name,"MonkeyDroppedItem",stolenPackageName);
		}
		else if(para_state == MonkeyState.EATING_BANANA)
		{
			triggerSoundAtCamera("Eat");
			transform.GetComponent<SpriteRenderer>().color = Color.green;

			cnm.faceDirection("S");
			GameObject monkeyFoodGObj = transform.FindChild("Food").gameObject;
			//Color monkeyFoodColor = monkeyFoodGObj.GetComponent<SpriteRenderer>().color;

			CustomAnimationManager aniMang = monkeyFoodGObj.AddComponent<CustomAnimationManager>();
			List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
			List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
			batch1.Add(new AniCommandPrep("GrowOrShrink",1,new List<System.Object>() { new float[3] { 0,0,0 }, 0.05f }));
			//batch1.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] {monkeyFoodColor.r,monkeyFoodColor.g,monkeyFoodColor.b,monkeyFoodColor.a}, 2f }));
			batchLists.Add(batch1);
			aniMang.registerListener("Monkey",this);
			aniMang.init("EatingAni",batchLists);
		}
	}


	public void init(int[] para_mapSize,
			         GridProperties para_gPropMapWorldBounds,
					 ColoredNavGraph para_worldNavGraph,
	                 ITerrainHandler para_terrainHandle,
	                 Transform para_sfxPrefab)
	{
		//mapSize = para_mapSize;
		gPropMapWorldBounds = para_gPropMapWorldBounds;
		worldNavGraph = para_worldNavGraph;
		terrainHandle = para_terrainHandle;
		sfxPrefab = para_sfxPrefab;
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(currState == MonkeyState.CHASING)
		{
			if(para_eventID == "PathComplete")
			{

				// Check again for player detected.
				//bool playerDetected = false;
				if(monkeyDetectorGObj == null) { monkeyDetectorGObj = transform.FindChild("WorldPivot").FindChild("MonkeyDetector").gameObject; }
				if(monkeyDetectorGObj.renderer.bounds.Contains(playerAvatar.transform.position))
				{
					Debug.Log("Monkey detected Player!");
					setState(MonkeyState.ALERTED);
					//playerDetected = true;
				}
				else
				{
					setState(MonkeyState.IDLE);
				}
			}
		}
		else if(currState == MonkeyState.STEALING_PACKAGE)
		{
			if(para_eventID == "PackageStealAnimation")
			{
				// Source ID should hold the name of the stolen package.
				runAwayWithPackage(para_sourceID);
			}
		}
		else if(currState == MonkeyState.ESCAPING_WITH_PACKAGE)
		{
			if(para_eventID == "PathComplete")
			{
				setState(MonkeyState.RETREATING);
			}
		}
		else if(currState == MonkeyState.RETREATING)
		{
			if(para_eventID == "PathComplete")
			{
				setState(MonkeyState.IDLE);
			}
		}
		else if(currState == MonkeyState.EATING_BANANA)
		{
			if(para_eventID == "EatingAni")
			{
				Destroy(transform.FindChild("Food").gameObject);
				setState(MonkeyState.IDLE);
			}
		}
	}

	public void runAwayWithPackage(string para_packageName)
	{
		stolenPackageName = para_packageName;
		setState(MonkeyState.ESCAPING_WITH_PACKAGE);
	}

	public void receivePlayerItemStatus(bool para_playerHasItems)
	{
		playerHasItems = para_playerHasItems;
	}

	void OnTriggerStay(Collider collider)
	{
		if(currState == MonkeyState.CHASING)
		{
			if(collider.gameObject.name == "MainAvatar")
			{
				// Steal a package now!
				setState(MonkeyState.STEALING_PACKAGE);
			}
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if(currState == MonkeyState.CHASING)
		{
			if(collider.gameObject.name == "MainAvatar")
			{
				// Steal a package now!
				setState(MonkeyState.STEALING_PACKAGE);
			}
		}

		if((currState == MonkeyState.IDLE)||(currState == MonkeyState.ALERTED)||(currState == MonkeyState.CHASING))
		{
			if(collider.gameObject.name.Contains("Banana"))
			{
				Destroy(collider.gameObject.GetComponent<CustomAnimationManager>());
				Destroy(collider.gameObject.GetComponent<ShootNDecelerate>());
				collider.gameObject.GetComponent<Animator>().Play("BananaIdle");

				collider.gameObject.name = "Food";
				collider.gameObject.transform.position = transform.position;
				collider.gameObject.transform.parent = transform;

				setState(MonkeyState.EATING_BANANA);
			}
		}
	}



	public void eatPackage(GameObject package){

		package.name = "Food";
		package.transform.position = transform.position;
		package.transform.parent = transform;
			
		setState(MonkeyState.EATING_BANANA);


	}

	public void becomeCounter(int distance){

		if(currState!=MonkeyState.COUNTER)
			triggerSoundAtCamera("MonkeyAngry");
		else
			setState(MonkeyState.COUNTER);

		initCharacterMovement(getCellForItem("MainAvatar"),false,false,distance);
	}
	


	public int calculateDistanceToPlayer(){

		int[] para_mapCell = getCellForItem("MainAvatar");
		int[] cellForCharacter = getCellForItem(transform.gameObject.name);
		int srcNavNodeID = terrainHandle.getNavNodeIDForCell(cellForCharacter);
		int destNavNodeID = terrainHandle.getNavNodeIDForCell(para_mapCell);

		float[] cellCentre = gPropMapWorldBounds.getCellCentre(para_mapCell,true);
		if(cellCentre != null)
		{

			List<NavNode> pathNodes = worldNavGraph.searchForPath(srcNavNodeID,destNavNodeID,new HashSet<int> {worldNavGraph.getTypeIDByName("MonkeySpawn")});
			if(pathNodes != null)
				return pathNodes.Count;
		}



		return -1;
	}


	private void initCharacterMovement(int[] para_mapCell, bool para_stopOneNodeShort, bool para_faceDownWhenOver){

		initCharacterMovement(para_mapCell, para_stopOneNodeShort, para_faceDownWhenOver,-1);
	}


	private void initCharacterMovement(int[] para_mapCell, bool para_stopOneNodeShort, bool para_faceDownWhenOver,int distance)
	{

		
		
		int[] cellForCharacter = getCellForItem(transform.gameObject.name);
		int srcNavNodeID = terrainHandle.getNavNodeIDForCell(cellForCharacter);
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
					Debug.Log(pathNodes.Count+" nodes!");

					if (distance>0)
						if(distance<pathNodes.Count){
							pathNodes.RemoveRange(pathNodes.Count-distance,distance);
						}else{
							return;//WAIT
						}
					Debug.Log(pathNodes.Count+" nodes!");
					cnm.moveAlongPath(pathNodes,currMonkeySpeed,para_faceDownWhenOver,para_stopOneNodeShort);

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

	private int[] getCellForItem(Vector3 para_worldPos)
	{
		int[] itemCell = gPropMapWorldBounds.hashPointToCell(new float[2] { para_worldPos.x, para_worldPos.y },true);
		return itemCell;
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

}

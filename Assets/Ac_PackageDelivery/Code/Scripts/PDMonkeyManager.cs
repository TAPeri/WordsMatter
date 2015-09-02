/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class PDMonkeyManager : MonoBehaviour
{

	// References to variables instantiated by the AcPackageDeliveryScenario.
	Transform monkeyPrefab;
	Transform sfxPrefab;
	int[] mapSize;
	GridProperties gPropMapWorldBounds;
	ColoredNavGraph worldNavGraph;
	ITerrainHandler terrainHandle;

	// Init and use in this script.
	SpatialHasher monkeySpawnLocHasher;
	// Percentage of occupied monkey spawn points.
	float monkeyCoverage = 1f;	
	float monkeyWalkSpeed = 2f;
	//float monkeyRunSpeed = 5f;


	//GameObject playerAvatar;
	List<GameObject> idleMonkeys;
	//List<int> alertedMonkeys;
	//List<int> tmpList;


	void Start()
	{
		//tmpList = new List<int>();
	}


	void Update()
	{

	}



	public void init(Transform para_monkeyPrefab,
	                 Transform para_sfxPrefab,
						   int[] para_mapSize,
	                 	   GridProperties para_gPropMapWorldBounds,
	                 	   ColoredNavGraph para_worldNavGraph,
	                       ITerrainHandler para_terrainHandle)
	{
		monkeyPrefab = para_monkeyPrefab;
		sfxPrefab = para_sfxPrefab;
		mapSize = para_mapSize;
		gPropMapWorldBounds = para_gPropMapWorldBounds;
		worldNavGraph = para_worldNavGraph;
		terrainHandle = para_terrainHandle;
		//playerAvatar = GameObject.Find("MainAvatar");


		// Spatial hash monkey spawn pts.
		monkeySpawnLocHasher = new SpatialHasher(new GridProperties(new float[3] {gPropMapWorldBounds.x,gPropMapWorldBounds.y,gPropMapWorldBounds.z},0,gPropMapWorldBounds.totalWidth/4f,gPropMapWorldBounds.totalHeight/4f,4,4));
		List<int> monkeySpawnNodeIDs = worldNavGraph.getNodesOfType(worldNavGraph.getTypeIDByName("MonkeySpawn"));
		for(int i=0; i<monkeySpawnNodeIDs.Count; i++)
		{
			WorldNode tmpNode = (WorldNode) worldNavGraph.getNode(monkeySpawnNodeIDs[i]);
			Vector3 tmpWorldPt = tmpNode.getWorldPt();
			monkeySpawnLocHasher.insertItem(tmpNode.getNodeID(),new Vector2(tmpWorldPt.x,tmpWorldPt.y));
		}
		
		// Spawn Monkeys.
		spawnMonkeys();
	}

	private void spawnMonkeys()
	{
		// Spawn monkeys at locations.
		
		int numOfMonkeysToSpawn = (int) (monkeySpawnLocHasher.getTotalItemCount() * monkeyCoverage);
		List<int> monkeySpawnNodeIDs = new List<int>(worldNavGraph.getNodesOfType(worldNavGraph.getTypeIDByName("MonkeySpawn")));
		int quadrantsPerWidth = 2;
		int quadrantsPerHeight = 2;
		Rect quadrantBounds = new Rect(gPropMapWorldBounds.x,
		                               gPropMapWorldBounds.y,
		                               gPropMapWorldBounds.totalWidth/(quadrantsPerWidth * 1.0f),
		                               gPropMapWorldBounds.totalHeight/(quadrantsPerHeight * 1.0f));
		List<int> quadrantItems;
		
		List<int> selectedMonkeySpawnNodes = new List<int>();
		
		
		
		for(int r=0; r<quadrantsPerHeight; r++)
		{
			for(int c=0; c<quadrantsPerWidth; c++)
			{
				quadrantBounds.x = gPropMapWorldBounds.x + (c * quadrantBounds.width);
				quadrantBounds.y = gPropMapWorldBounds.y - (r * quadrantBounds.height);
				
				quadrantItems = new List<int>(monkeySpawnLocHasher.areaQuery(quadrantBounds));
				int numSelItemsFromLoc = (int) (quadrantItems.Count * monkeyCoverage);
				for(int i=0; i<numSelItemsFromLoc; i++)
				{
					int randIndex = Random.Range(0,quadrantItems.Count);
					int potentialItemID = quadrantItems[randIndex];
					if( ! selectedMonkeySpawnNodes.Contains(potentialItemID))
					{
						selectedMonkeySpawnNodes.Add(quadrantItems[randIndex]);
						numOfMonkeysToSpawn--;
					}
					
					monkeySpawnNodeIDs.Remove(quadrantItems[randIndex]);
					quadrantItems.RemoveAt(randIndex);
					
					if(numOfMonkeysToSpawn <= 0) 	{ break; }
				}
				
				if(numOfMonkeysToSpawn <= 0) 	{ break; }
			}
			
			if(numOfMonkeysToSpawn <= 0) 	{ break; }
		}
		
		
		if(numOfMonkeysToSpawn > 0)
		{
			while((numOfMonkeysToSpawn > 0)&&(monkeySpawnNodeIDs.Count > 0))
			{
				int randIndex = Random.Range(0,monkeySpawnNodeIDs.Count);
				selectedMonkeySpawnNodes.Add(monkeySpawnNodeIDs[randIndex]);
				monkeySpawnNodeIDs.RemoveAt(randIndex);
				numOfMonkeysToSpawn--;
			}
		}
		

		idleMonkeys = new List<GameObject>();
		GameObject monkeyCollectionGObj = new GameObject("MonkeyCollection");
		for(int i=0; i<selectedMonkeySpawnNodes.Count; i++)
		{
			WorldNode tmpNode = (WorldNode) worldNavGraph.getNode(selectedMonkeySpawnNodes[i]);
			Vector3 nodeCentre = tmpNode.getWorldPt();
			Vector3 monkeySpawnPt = new Vector3(nodeCentre.x,nodeCentre.y,-2f);
			Transform nwMonkeyTrans = (Transform) Instantiate(monkeyPrefab,monkeySpawnPt,Quaternion.identity);
			nwMonkeyTrans.gameObject.name = "Monkey-"+i;
			nwMonkeyTrans.parent = monkeyCollectionGObj.transform;

			NewCharacterNavMovement cnm = nwMonkeyTrans.gameObject.AddComponent<NewCharacterNavMovement>();
			cnm.doNotDestroy = true;
			cnm.faceDirection("S");

			PDMonkeyScript ms = nwMonkeyTrans.gameObject.AddComponent<PDMonkeyScript>();
			ms.init(mapSize,gPropMapWorldBounds,worldNavGraph,terrainHandle,sfxPrefab);

			idleMonkeys.Add(nwMonkeyTrans.gameObject);
		}

		
		//triggerRandomDestinationsForMonkeys();
	}

	
	
	private void triggerRandomDestinationsForMonkeys()
	{
		GameObject monkeyCollectionGObj = GameObject.Find("MonkeyCollection");
		
		List<int> allNodeKeys = worldNavGraph.getAllNodeKeys();
		
		for(int i=0; i<monkeyCollectionGObj.transform.childCount; i++)
		{
			GameObject tmpMonkey = monkeyCollectionGObj.transform.GetChild(i).gameObject;
			NewCharacterNavMovement cnm = tmpMonkey.GetComponent<NewCharacterNavMovement>();
			if(cnm == null)
			{
				cnm = tmpMonkey.AddComponent<NewCharacterNavMovement>();
				cnm.doNotDestroy = true;
				//cnm.registerListener("AcScen",this);
			}
			int srcNodeID = terrainHandle.getNavNodeIDForCell( gPropMapWorldBounds.hashPointToCell(new float[2] {tmpMonkey.transform.position.x,tmpMonkey.transform.position.y},true) );
			int destNodeID = Random.Range(0,allNodeKeys.Count);
			List<NavNode> pathNodes = worldNavGraph.searchForPath(srcNodeID,destNodeID,new HashSet<int> {worldNavGraph.getTypeIDByName("MonkeySpawn")} );
			cnm.moveAlongPath(pathNodes,monkeyWalkSpeed,true,false);
		}
	}



	PDMonkeyScript counterMonkey;

	public void moveCounter(int distance){

		counterMonkey.becomeCounter(distance);
	}

	public void eatPackage(GameObject package){

		counterMonkey.eatPackage(package);
		counterMonkey = null;
	}


	public void findMonkeyCounter(int distance){
		Transform monkeyCollection = GameObject.Find("MonkeyCollection").transform;


		int bestDistance = 10000;

		for(int i=0; i<monkeyCollection.childCount; i++)
		{
			Transform monkey = monkeyCollection.GetChild(i);
			PDMonkeyScript mScript = monkey.GetComponent<PDMonkeyScript>();
			int dist = mScript.calculateDistanceToPlayer()-distance;

			Debug.Log(dist);
			if (dist==0){
				counterMonkey = mScript;
				bestDistance = 0;
				break;
			}else if(dist<0){
				dist = -dist;
				if(dist<bestDistance){
					counterMonkey = mScript;
					bestDistance = dist;
				}
			}else{
				if(dist<bestDistance){
					counterMonkey = mScript;
					bestDistance = distance;
				}
			}
		}


		counterMonkey.becomeCounter(distance);


	}

	public void informAllMonkeysOfPlayerStock(bool para_playerHasItems)
	{
		Transform monkeyCollection = GameObject.Find("MonkeyCollection").transform;

		for(int i=0; i<monkeyCollection.childCount; i++)
		{
			Transform monkey = monkeyCollection.GetChild(i);
			PDMonkeyScript mScript = monkey.GetComponent<PDMonkeyScript>();
			mScript.receivePlayerItemStatus(para_playerHasItems);
		}
	}


}

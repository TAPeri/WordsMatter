/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class BystanderMangScript : MonoBehaviour, CustomActionListener
{

	BasicNavGraph bystanderNavGraph;
	int nxtAvailableBystanderID = 0;
	//float bystanderZPos = -2f;

	List<int> prefabIndexesMasterList = null;
	List<int> availablePrefabIndexes = null;
	List<Transform> bystanderPrefabList;

	List<int> bottomWalkwayNodeIDs;
	List<int> bottomEntranceNodeIDs;
	List<int> cornerNodeIDs;

	Dictionary<string,Rect> areas;
	List<int> topNodes;
	List<int> leftNodes;
	List<int> rightNodes;

	EnterStateFactory enterStateScpt;
	RegularBystanderFactory regularStateScrpt;
	LingerBystanderFactory lingerStateScrpt;
	ExitStateFactory exitStateScrpt;
	

	// Tmp Wait Sequence Params.
	//bool waitBystanderInvalidated = false;
	GameObject chosenWaitBystander;
	GameObject pastWaitBystander;
	int pastPreExitNode;
	Vector3 waitPosition;
	string waitSideID;
	int chosenPreExitNode;
	Transform exclamationMarkPrefab;


	public void init(Transform para_debugPrefab,
	                 Transform para_exclamationMarkPrefab,
	                 Dictionary<string,Rect> para_worldBoundsLookup)
	{
		initBystanderNavGraph(para_worldBoundsLookup,para_debugPrefab);

		enterStateScpt = transform.gameObject.AddComponent<EnterStateFactory>();
		regularStateScrpt = transform.gameObject.AddComponent<RegularBystanderFactory>();
		lingerStateScrpt = transform.gameObject.AddComponent<LingerBystanderFactory>();
		exitStateScrpt = transform.gameObject.AddComponent<ExitStateFactory>();
		exitStateScrpt.registerListener("BystanderMangScript",this);

		Dictionary<string,List<int>> areaToNodes = new Dictionary<string, List<int>>();
		areaToNodes.Add("T",topNodes);
		areaToNodes.Add("L",leftNodes);
		areaToNodes.Add("R",rightNodes);
		//Dictionary<string,Rect> areaWorldBounds = new Dictionary<string, Rect>();

		enterStateScpt.init(bystanderNavGraph,bottomEntranceNodeIDs,cornerNodeIDs);
		regularStateScrpt.init(bystanderNavGraph,cornerNodeIDs);
		lingerStateScrpt.init(bystanderNavGraph,areaToNodes);
		exitStateScrpt.init(bystanderNavGraph,bottomEntranceNodeIDs,cornerNodeIDs);


		// Create start bystanders.
		nxtAvailableBystanderID = 0;

		// Apply 2 lingerers to each of the 3 sides.
		foreach(KeyValuePair<string,List<int>> pair in areaToNodes)
		{
			for(int i=0; i<2; i++)
			{
				BystanderInitParams bParams = getNextBystanderInitParams();
				lingerStateScrpt.createLingerBystander(bParams.bName,bParams.bPrefab,pair.Key);
			}
		}

		// Apply 4 enter bystanders.
		for(int i=0; i<4; i++)
		{
			BystanderInitParams bParams = getNextBystanderInitParams();
			enterStateScpt.createEnteringBystander(bParams.bName,bParams.bPrefab);
		}


		exclamationMarkPrefab = para_exclamationMarkPrefab;
	}



	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "BystanderExit")
		{
			// Replenish scene if an agent exits.
			BystanderInitParams bParams = getNextBystanderInitParams();
			enterStateScpt.createEnteringBystander(bParams.bName,bParams.bPrefab);
		}
		else if(pastWaitBystander != null)
		{
			if((para_eventID == "PathComplete")&&(para_sourceID == pastWaitBystander.name))
			{
				pastWaitBystander.GetComponent<NewCharacterNavMovement>().unregisterListener("BystanderMangScript");
				exitStateScrpt.intakeBystander(pastWaitBystander,pastPreExitNode);
				//waitBystanderInvalidated = true;
			}
		}
	}


	public void selectWaitBystander(string para_sideID, Vector3 para_ptByEdge)
	{

		// Tell lingerstatefactory to select a bystander.
		// Remove this bystander from the linger state.
		GameObject reqObj = lingerStateScrpt.selectAndDeregisterBystander(para_sideID);
		if(reqObj == null)
		{
			BystanderInitParams bParams = getNextBystanderInitParams();
			lingerStateScrpt.createLingerBystander(bParams.bName,bParams.bPrefab,para_sideID);

			reqObj = lingerStateScrpt.selectAndDeregisterBystander(para_sideID);
		}


		// Pause selected bystander.
		//waitBystanderInvalidated = false;
		chosenWaitBystander = reqObj;
		waitSideID = para_sideID;
		waitPosition = para_ptByEdge;
		NewCharacterNavMovement cnm = chosenWaitBystander.GetComponent<NewCharacterNavMovement>();
		cnm.setToIdle();


		// Tell regularstatefactory to transfer a bystander to replace the one which was lingering.
		bool successFlag = regularStateScrpt.orderAnyBystanderToLinger(para_sideID);
		if( ! successFlag)
		{
			BystanderInitParams bParams = getNextBystanderInitParams();
			regularStateScrpt.createRegularBystander(bParams.bName,bParams.bPrefab);
			regularStateScrpt.orderAnyBystanderToLinger(para_sideID);
		}


		// Add a new bystander via the enterfactory. (This bystander will replace the missing regularstate bystander).
		BystanderInitParams bPParams = getNextBystanderInitParams();
		enterStateScpt.createEnteringBystander(bPParams.bName,bPParams.bPrefab);
	}

	public void runWaitBystander()
	{
		// Run selected bystander with wait state.
		WaitAndCallStateScript ws = chosenWaitBystander.AddComponent<WaitAndCallStateScript>();
		GameObject instructionTileGObj = Camera.main.transform.FindChild("InstructionTile").gameObject;
		Rect finalInstructionTileBounds = CommonUnityUtils.get2DBounds(instructionTileGObj.renderer.bounds);
		Vector3 tmpV = instructionTileGObj.transform.position;
		tmpV.x = 9001;
		instructionTileGObj.transform.position = tmpV;
		ws.init(waitSideID,waitPosition,exclamationMarkPrefab,instructionTileGObj,finalInstructionTileBounds);
	}

	public GameObject getWaitBystander()
	{
		return chosenWaitBystander;
	}

	public void releaseWaitingBystander()
	{
		Vector3 chosenItemPos = waitPosition;
		string sideKey = null;
		pastWaitBystander = GameObject.Find(chosenWaitBystander.name);
		pastPreExitNode = chosenPreExitNode;
		foreach(KeyValuePair<string,Rect> pair in areas)
		{
			Rect tmpRect = pair.Value;

			if((chosenItemPos.x >= tmpRect.x)
			&&(chosenItemPos.x <= (tmpRect.x + tmpRect.width))
			&&(chosenItemPos.y >= (tmpRect.y - tmpRect.height))
			&&(chosenItemPos.y <= tmpRect.y))
			{
				sideKey = pair.Key;
				break;
			}
		}

		if(sideKey != null)
		{
			if(sideKey == "T")
			{
				chosenPreExitNode = topNodes[0];
			}
			else if(sideKey == "L")
			{
				chosenPreExitNode = leftNodes[0];
			}
			else if(sideKey == "R")
			{
				chosenPreExitNode = rightNodes[0];
			}

			NewCharacterNavMovement cnm = chosenWaitBystander.GetComponent<NewCharacterNavMovement>();
			cnm.doNotDestroy = true;
			cnm.registerListener("BystanderMangScript",this);
			cnm.moveAlongPath(new List<NavNode> { bystanderNavGraph.getNode(chosenPreExitNode) },1f,false,false);
		}
		else
		{
			Destroy(chosenWaitBystander);
			BystanderInitParams bParams = getNextBystanderInitParams();
			enterStateScpt.createEnteringBystander(bParams.bName,bParams.bPrefab);
		}
	}




	
	private void initBystanderNavGraph(Dictionary<string,Rect> para_worldBoundsLookup, Transform para_debugPrefab)
	{
		Dictionary<string,Rect> worldBoundsLookup = para_worldBoundsLookup;

		bystanderNavGraph = new BasicNavGraph();
		
		GameObject lwalkwayObj = GameObject.Find("LeftWalk");
		float graphZPos = lwalkwayObj.transform.position.z;
		Rect leftWalkwayWorld = worldBoundsLookup["LeftWalkway"];
		Vector3 lw_topMid = new Vector3(leftWalkwayWorld.x + (leftWalkwayWorld.width/2f),leftWalkwayWorld.y,graphZPos);
		Vector3 lw_botMid = new Vector3(lw_topMid.x,leftWalkwayWorld.y - (leftWalkwayWorld.height),graphZPos);
		
		Rect topWalkwayWorld = worldBoundsLookup["TopWalkway"];
		Vector3 tw_leftMid = new Vector3(topWalkwayWorld.x,topWalkwayWorld.y - (topWalkwayWorld.height/2f),graphZPos);
		Vector3 tw_rightMid = new Vector3(topWalkwayWorld.x + topWalkwayWorld.width,tw_leftMid.y,graphZPos);
		
		Rect rightWalkwayWorld = worldBoundsLookup["RightWalkway"];
		Vector3 rw_topMid = new Vector3(rightWalkwayWorld.x + (rightWalkwayWorld.width/2f),rightWalkwayWorld.y,graphZPos);
		Vector3 rw_botMid = new Vector3(rw_topMid.x,rightWalkwayWorld.y - rightWalkwayWorld.height,graphZPos);

		Rect bottomWalkwayWorld = worldBoundsLookup["BottomWalkway"];
		Rect bottomEntranceWorld = worldBoundsLookup["BottomEntrance"];
		List<Vector3> bottomWalkwayPoints = new List<Vector3>();
		List<Vector3> bottomEntrancePoints = new List<Vector3>();

		float entranceSpacing = 2;
		int numEntrances = (int) ((bottomEntranceWorld.width - entranceSpacing) / entranceSpacing);
		Vector3 nxtBottomWalkwayPt = new Vector3(bottomWalkwayWorld.x + entranceSpacing,bottomWalkwayWorld.y - (bottomWalkwayWorld.height/2f),graphZPos);
		Vector3 nxtBottomEntrancePt = new Vector3(bottomEntranceWorld.x + entranceSpacing,bottomEntranceWorld.y - ( Random.Range(0,2)),graphZPos);
		//Vector3 nxtBottomEntrancePt = new Vector3(bottomEntranceWorld.x + entranceSpacing,bottomEntranceWorld.y - (bottomEntranceWorld.height + Random.Range(2,6)),graphZPos);

		for(int i=0; i<numEntrances; i++)
		{
			bottomWalkwayPoints.Add(nxtBottomWalkwayPt);
			bottomEntrancePoints.Add(nxtBottomEntrancePt);

			nxtBottomWalkwayPt.x += entranceSpacing;
			nxtBottomEntrancePt.x += entranceSpacing;
			nxtBottomEntrancePt.y = bottomEntranceWorld.y - (bottomEntranceWorld.height + 1);//Random.Range(2,6));
		}


		Vector3[] posList = new Vector3[6] { lw_botMid, lw_topMid, tw_leftMid, tw_rightMid, rw_topMid, rw_botMid };
		int nodeTicketID = 0;
		for(int i=0; i<posList.Length; i++)
		{
			bystanderNavGraph.addNode(new WorldNode(nodeTicketID,1,posList[i]));
			if(i > 0) {	bystanderNavGraph.addEdge(nodeTicketID-1,nodeTicketID,new NavEdge(new int[2] { (nodeTicketID-1), nodeTicketID },1)); }
			nodeTicketID++;
		}
		cornerNodeIDs = new List<int>() { 0,2,4,5 };
		topNodes = new List<int>() { 2,3 };
		leftNodes = new List<int>() { 0,1 };
		rightNodes = new List<int>() { 4,5 };

		bottomWalkwayNodeIDs = new List<int>();
		for(int i=0; i<bottomWalkwayPoints.Count; i++)
		{
			bystanderNavGraph.addNode(new WorldNode(nodeTicketID,1,bottomWalkwayPoints[i]));
			bottomWalkwayNodeIDs.Add(nodeTicketID);
			if(i > 0) { bystanderNavGraph.addEdge(nodeTicketID-1,nodeTicketID,new NavEdge(new int[2] { (nodeTicketID-1), nodeTicketID },1)); }
			nodeTicketID++;
		}
		bystanderNavGraph.addEdge(bottomWalkwayNodeIDs[0],0,new NavEdge(new int[2] { bottomWalkwayNodeIDs[0], 0 },1));
		bystanderNavGraph.addEdge(bottomWalkwayNodeIDs[bottomWalkwayNodeIDs.Count-1],5,new NavEdge(new int[2] { bottomWalkwayNodeIDs[bottomWalkwayNodeIDs.Count-1], 5 },1));

		bottomEntranceNodeIDs = new List<int>();
		for(int i=0; i<bottomEntrancePoints.Count; i++)
		{
			bystanderNavGraph.addNode(new WorldNode(nodeTicketID,2,bottomEntrancePoints[i]));
			bottomEntranceNodeIDs.Add(nodeTicketID);
			bystanderNavGraph.addEdge(nodeTicketID,bottomWalkwayNodeIDs[i],new NavEdge(new int[2] { nodeTicketID, bottomWalkwayNodeIDs[i]},1));
			nodeTicketID++;
		}


		areas = new Dictionary<string, Rect>();
		areas.Add("L",leftWalkwayWorld);
		areas.Add("T",topWalkwayWorld);
		areas.Add("R",rightWalkwayWorld);

		GameObject navGraphRender = NavGraphUnityUtils.renderNavGraph("MPBystanderNavGraphRender",bystanderNavGraph,para_debugPrefab);
		navGraphRender.SetActive(false);
	}

	private void loadBystanderPrefabs()
	{
		bystanderPrefabList = new List<Transform>();

		bool foundItem = true;
		int counter = 0;

		while(foundItem)
		{
			Transform tmpPrefab = Resources.Load<Transform>("Prefabs/Avatars/SecChar-"+counter);
			if(tmpPrefab == null)
			{
				foundItem = false;
			}
			else
			{
				bystanderPrefabList.Add(tmpPrefab);
			}
			counter++;
		}
	}

	private Transform getNextBystanderPrefab()
	{
		Transform reqPrefab = null;
		
		if(bystanderPrefabList == null)
		{
			loadBystanderPrefabs();

			prefabIndexesMasterList = new List<int>();
			availablePrefabIndexes = new List<int>();
			for(int i=0; i<bystanderPrefabList.Count; i++)
			{
				prefabIndexesMasterList.Add(i);
				availablePrefabIndexes.Add(i);
			}
		}
		
		int reqPrefabIndex = Random.Range(0,availablePrefabIndexes.Count);
		reqPrefab = bystanderPrefabList[availablePrefabIndexes[reqPrefabIndex]];
		availablePrefabIndexes.RemoveAt(reqPrefabIndex);
		if(availablePrefabIndexes.Count <= 0)
		{
			for(int k=0; k<prefabIndexesMasterList.Count; k++)
			{
				availablePrefabIndexes.Add(k);
			}
		}
		
		return reqPrefab;
	}

	private BystanderInitParams getNextBystanderInitParams()
	{
		string bName = "B-"+nxtAvailableBystanderID;
		Transform reqPrefab = getNextBystanderPrefab();
		nxtAvailableBystanderID++;

		BystanderInitParams nwParams = new BystanderInitParams(bName,reqPrefab);
		return nwParams;
	}

	class BystanderInitParams
	{
		public string bName;
		public Transform bPrefab;

		public BystanderInitParams(string para_bName, Transform para_bPrefab)
		{
			bName = para_bName;
			bPrefab = para_bPrefab;
		}
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class EnterStateFactory : BystanderStateFactory, CustomActionListener
{
	BasicNavGraph bystanderGraph;
	List<int> enterNodeList;
	List<int> cornerNodeIDs;
	int availabilityCounter;


	
	public void init(BasicNavGraph para_bystanderGraph,
	                 List<int> para_enterNodeList,
	                 List<int> para_cornerNodeIDs)
	{
		stateFactoryName = "EnterStateFactory";
		bystanderGraph = para_bystanderGraph;
		enterNodeList = para_enterNodeList;
		cornerNodeIDs = para_cornerNodeIDs;
		availabilityCounter = 0;
	}

	public BystanderCreationData createEnteringBystander(string para_bystanderName, Transform para_bystanderPrefab)
	{
		int enterNodeID = getNextAvailableEnterNode();
		WorldNode reqEnterNode = (WorldNode) bystanderGraph.getNode(enterNodeID);
		Vector3 worldPt = reqEnterNode.getWorldPt();

		GameObject nwBystander = spawnNewBystanderInWorld(para_bystanderName,para_bystanderPrefab,worldPt,enterNodeID);
		registerNewBystander(nwBystander.name,nwBystander,enterNodeID);

		triggerEnterTheTownSquare(nwBystander,enterNodeID);

		BystanderCreationData retData = new BystanderCreationData(nwBystander,enterNodeID);
		return retData;
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "PathComplete")
		{
			// If an enter bystander completes its path then transfer it over to the regular bystander state.

			GameObject bystanderObj = registeredBystanders[para_sourceID];
			transferBystander_ToRegularState(bystanderObj);
		}
	}

	private int getNextAvailableEnterNode()
	{
		int retNodeID = enterNodeList[availabilityCounter];
		availabilityCounter++;
		if(availabilityCounter >= enterNodeList.Count)
		{
			availabilityCounter = 0;
		}
		return retNodeID;
	}

	private void triggerEnterTheTownSquare(GameObject para_bystanderObj, int para_bystanderNodeID)
	{
		NewCharacterNavMovement cnm = para_bystanderObj.GetComponent<NewCharacterNavMovement>();
		if(cnm == null)
		{
			cnm = para_bystanderObj.gameObject.AddComponent<NewCharacterNavMovement>();
			cnm.doNotDestroy = true;
		}
		cnm.registerListener(stateFactoryName,this);

		int nxtNodeID = cornerNodeIDs[Random.Range(0,cornerNodeIDs.Count)];
		List<NavNode> pathNodes = bystanderGraph.searchForPath(para_bystanderNodeID,nxtNodeID,new HashSet<int>() { 2 });
		bystanderToNodeID[para_bystanderObj.name] = nxtNodeID;
		cnm.moveAlongPath(pathNodes,Random.Range(1f,1.4f),false,false);
	}

	private void transferBystander_ToRegularState(GameObject para_bystanderObj)
	{
		int bystanderNode = bystanderToNodeID[para_bystanderObj.name];

		// Deregister.
		unregisterBystander(para_bystanderObj.name);

		// Transfer.
		RegularBystanderFactory rbf = transform.GetComponent<RegularBystanderFactory>();
		rbf.intakeBystander(para_bystanderObj,bystanderNode);
	}
}

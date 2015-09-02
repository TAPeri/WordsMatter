/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ExitStateFactory : BystanderStateFactory, CustomActionListener, IActionNotifier
{
	BasicNavGraph bystanderGraph;
	List<int> enterNodeList;
	List<int> cornerNodeIDs;
	int availabilityCounter = 0;


	public void init(BasicNavGraph para_bystanderGraph,
	                 List<int> para_enterNodeList,
	                 List<int> para_cornerNodeIDs)
	{
		stateFactoryName = "ExitStateFactory";
		bystanderGraph = para_bystanderGraph;
		enterNodeList = para_enterNodeList;
		cornerNodeIDs = para_cornerNodeIDs;
		availabilityCounter = 0;
	}

	public BystanderCreationData createExitBystander(string para_bystanderName, Transform para_bystanderPrefab)
	{
		int reqSpawnNodeID = getNextAvailableCornerNode();
		WorldNode reqSpawnWorldNode = (WorldNode) bystanderGraph.getNode(reqSpawnNodeID);
		Vector3 worldPt = reqSpawnWorldNode.getWorldPt();

		GameObject nwBystander = spawnNewBystanderInWorld(para_bystanderName,para_bystanderPrefab,worldPt,reqSpawnNodeID);
		registerNewBystander(nwBystander.name,nwBystander,reqSpawnNodeID);
		
		triggerExitTheTownSquare(nwBystander,reqSpawnNodeID);
		
		BystanderCreationData retData = new BystanderCreationData(nwBystander,reqSpawnNodeID);
		return retData;
	}

	public void intakeBystander(GameObject para_bystander, int para_currNodeID)
	{
		registerNewBystander(para_bystander.name,para_bystander,para_currNodeID);
		triggerExitTheTownSquare(para_bystander,para_currNodeID);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "PathComplete")
		{
			GameObject bystanderObj = registeredBystanders[para_sourceID];
			unregisterBystander(bystanderObj.name);
			Destroy(bystanderObj);
			notifyAllListeners("ExitStateFactory","BystanderExit",null);
		}
	}
	
	private int getNextAvailableCornerNode()
	{
		int retNodeID = cornerNodeIDs[availabilityCounter];
		availabilityCounter++;
		if(availabilityCounter >= cornerNodeIDs.Count)
		{
			availabilityCounter = 0;
		}
		return retNodeID;
	}
	
	private void triggerExitTheTownSquare(GameObject para_bystanderObj, int para_bystanderNodeID)
	{
		NewCharacterNavMovement cnm = para_bystanderObj.GetComponent<NewCharacterNavMovement>();
		if(cnm == null)
		{
			cnm = para_bystanderObj.gameObject.AddComponent<NewCharacterNavMovement>();
			cnm.doNotDestroy = true;
		}
		cnm.registerListener(stateFactoryName,this);
		
		int nxtNodeID = enterNodeList[Random.Range(0,enterNodeList.Count)];
		List<NavNode> pathNodes = bystanderGraph.searchForPath(para_bystanderNodeID,nxtNodeID);
		bystanderToNodeID[para_bystanderObj.name] = nxtNodeID;
		cnm.moveAlongPath(pathNodes,2f,false,false);
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

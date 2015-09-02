/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class RegularBystanderFactory : BystanderStateFactory, CustomActionListener
{
	BasicNavGraph bystanderGraph;
	List<int> cornerNodeIDs;

	bool requestFlag_transToLinger;
	string requestedLingerSide;


	public void init(BasicNavGraph para_bystanderGraph,
	                 List<int> para_cornerNodeIDs)
	{
		stateFactoryName = "RegularBystanderFactory";
		bystanderGraph = para_bystanderGraph;
		cornerNodeIDs = para_cornerNodeIDs;
		requestFlag_transToLinger = false;
	}

	public BystanderCreationData createRegularBystander(string para_bystanderName, Transform para_bystanderPrefab)
	{
		int reqNodeID = cornerNodeIDs[Random.Range(0,cornerNodeIDs.Count)];
		WorldNode reqNode = (WorldNode) bystanderGraph.getNode(reqNodeID);
		Vector3 worldPt = reqNode.getWorldPt();

		GameObject nwBystander = spawnNewBystanderInWorld(para_bystanderName,para_bystanderPrefab,worldPt,reqNodeID);
		registerNewBystander(nwBystander.name,nwBystander,reqNodeID);

		triggerRandomHeading(nwBystander,reqNodeID);

		BystanderCreationData retData = new BystanderCreationData(nwBystander.gameObject,reqNodeID);
		return retData;
	}

	public void intakeBystander(GameObject para_bystander, int para_currNodeID)
	{
		registerNewBystander(para_bystander.name,para_bystander,para_currNodeID);
		triggerRandomHeading(para_bystander,para_currNodeID);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "PathComplete")
		{
			GameObject bystanderObj = registeredBystanders[para_sourceID];

			if(requestFlag_transToLinger == true)
			{
				requestFlag_transToLinger = false;
				transferBystander_ToLingerState(bystanderObj);
			}
			else
			{
				triggerRandomHeading(bystanderObj,bystanderToNodeID[para_sourceID]);					
			}
		}
	}

	public bool orderAnyBystanderToLinger(string para_lingerSide)
	{
		bool successFlag = false;
		if(registeredBystanders != null)
		{
			if(registeredBystanders.Count > 0)
			{
				requestFlag_transToLinger = true;
				requestedLingerSide = para_lingerSide;
				successFlag = true;
			}
		}
		return successFlag;
	}

	private void triggerRandomHeading(GameObject para_bystanderObj, int para_bystanderNodeID)
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
		cnm.moveAlongPath(pathNodes,Random.Range(1f,1.5f),false,false);
	}

	private void transferBystander_ToLingerState(GameObject para_bystanderObj)
	{
		int bystanderNode = bystanderToNodeID[para_bystanderObj.name];

		unregisterBystander(para_bystanderObj.name);

		LingerBystanderFactory lbf = transform.GetComponent<LingerBystanderFactory>();
		lbf.intakeBystander(para_bystanderObj,bystanderNode,requestedLingerSide);
	}
}

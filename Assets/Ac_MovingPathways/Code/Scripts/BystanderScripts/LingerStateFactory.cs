/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class LingerBystanderFactory : BystanderStateFactory, CustomActionListener
{
	BasicNavGraph bystanderGraph;
	Dictionary<string,List<int>> areaToNodes;
	//Dictionary<string,Rect> areaWorldBounds;

	Dictionary<string,string> bystanderToArea;
	Dictionary<string,int> bystanderToAreaNodeIndex;


	public void init(BasicNavGraph para_bystanderGraph,
	                 Dictionary<string,List<int>> para_areaToNodes)
	{
		stateFactoryName = "LingerBystanderFactory";
		bystanderGraph = para_bystanderGraph;
		areaToNodes = para_areaToNodes;
		bystanderToArea = new Dictionary<string, string>();
		bystanderToAreaNodeIndex = new Dictionary<string, int>();
	}

	public BystanderCreationData createLingerBystander(string para_bystanderName, Transform para_bystanderPrefab, string para_side)
	{
		List<int> nodeIDList = null;
		if((para_side == "T")||(para_side == "L")||(para_side == "R"))
		{
			nodeIDList = areaToNodes[para_side];
		}

		if(nodeIDList != null)
		{
			int randIndex = Random.Range(0,nodeIDList.Count);
			bystanderToAreaNodeIndex.Add(para_bystanderName,randIndex);
			bystanderToArea.Add(para_bystanderName,para_side);

			int reqNodeID = nodeIDList[randIndex];
			WorldNode reqNode = (WorldNode) bystanderGraph.getNode(reqNodeID);
			Vector3 worldPt = reqNode.getWorldPt();
			
			GameObject nwBystander = spawnNewBystanderInWorld(para_bystanderName,para_bystanderPrefab,worldPt,reqNodeID);
			registerNewBystander(nwBystander.name,nwBystander,reqNodeID);

			triggerLingerBehaviour(nwBystander,reqNodeID);
			
			BystanderCreationData retData = new BystanderCreationData(nwBystander.gameObject,reqNodeID);
			return retData;
		}

		return null;
	}

	public void intakeBystander(GameObject para_bystander, int para_currNodeID, string para_lingerSide)
	{
		registerNewBystander(para_bystander.name,para_bystander,para_currNodeID);

		if(bystanderToArea.ContainsKey(para_bystander.name))
		{
			bystanderToArea[para_bystander.name] = para_lingerSide;
		}
		else
		{
			bystanderToArea.Add(para_bystander.name,para_lingerSide);
		}

		if(bystanderToAreaNodeIndex.ContainsKey(para_bystander.name))
		{
			bystanderToAreaNodeIndex[para_bystander.name] = 0;
		}
		else
		{
			bystanderToAreaNodeIndex.Add(para_bystander.name,0);
		}

		triggerLingerBehaviour(para_bystander,para_currNodeID);
	}

	public GameObject selectAndDeregisterBystander(string para_sideID)
	{
		GameObject retObj = null;
		foreach(KeyValuePair<string,string> pair in bystanderToArea)
		{
			if(pair.Value == para_sideID)
			{
				retObj = registeredBystanders[pair.Key];
				unregisterBystander(pair.Key);
				bystanderToArea.Remove(pair.Key);
				bystanderToAreaNodeIndex.Remove(pair.Key);
				break;
			}
		}
		return retObj;
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "PathComplete")
		{
			if(registeredBystanders.ContainsKey(para_sourceID))
			{
				GameObject bystanderObj = registeredBystanders[para_sourceID];
				triggerLingerBehaviour(bystanderObj,bystanderToNodeID[para_sourceID]);					
			}
		}
	}

	private void triggerLingerBehaviour(GameObject para_bystanderObj, int para_bystanderNodeID)
	{
		NewCharacterNavMovement cnm = para_bystanderObj.GetComponent<NewCharacterNavMovement>();
		if(cnm == null)
		{
			cnm = para_bystanderObj.gameObject.AddComponent<NewCharacterNavMovement>();
			cnm.doNotDestroy = true;
		}
		cnm.registerListener(stateFactoryName,this);


		string reqSide = bystanderToArea[para_bystanderObj.name];
		List<int> reqNodesForSide = areaToNodes[reqSide];
		int currListIndex = bystanderToAreaNodeIndex[para_bystanderObj.name];
		currListIndex++;
		if(currListIndex >= reqNodesForSide.Count) { currListIndex = 0; }
		bystanderToAreaNodeIndex[para_bystanderObj.name] = currListIndex;


		int nxtNodeID = reqNodesForSide[currListIndex];
		List<NavNode> pathNodes = bystanderGraph.searchForPath(para_bystanderNodeID,nxtNodeID,new HashSet<int>() { 2 });
		bystanderToNodeID[para_bystanderObj.name] = nxtNodeID;
		cnm.moveAlongPath(pathNodes,Random.Range(1f,1.5f),false,false);
	}


}

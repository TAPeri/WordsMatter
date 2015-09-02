/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using UnityEngine;
using System.Collections.Generic;


public class BasicTerrainHandler : ITerrainHandler
{

	Dictionary<string,int> coordToNodeMap;
	

	public bool constructTerrainStructures(List<System.Object> para_dataToUse)
	{
		BasicNavGraph nGraph = (BasicNavGraph) para_dataToUse[0];
		GridProperties gProp = (GridProperties) para_dataToUse[1];
		
		coordToNodeMap = new Dictionary<string,int>();
		List<int> nodeKeys = nGraph.getAllNodeKeys();
		float[] tmpFArr = new float[2];
		for(int i=0; i<nodeKeys.Count; i++)
		{
			WorldNode tmpWNode = (WorldNode) nGraph.getNode(nodeKeys[i]);
			tmpFArr[0] = tmpWNode.getWorldPt().x;
			tmpFArr[1] = tmpWNode.getWorldPt().y;
			int[] cellCoords = gProp.hashPointToCell(tmpFArr,true);
			if(cellCoords != null)
			{
				coordToNodeMap.Add(""+cellCoords[0]+"-"+cellCoords[1],tmpWNode.getNodeID());
			}
		}

		return true;
	}

	public bool isCellTraversible(int[] para_cellCoords)
	{
		bool retFlag = false;
		
		if((coordToNodeMap != null)
		&&(coordToNodeMap.ContainsKey(""+para_cellCoords[0]+"-"+para_cellCoords[1])))
		{
			retFlag = true;
		}

		return retFlag;		
	}

	public int getNavNodeIDForCell(int[] para_cellCoords)
	{
		int nodeID = -1;

		string reqKey = ""+para_cellCoords[0]+"-"+para_cellCoords[1];
		if((coordToNodeMap != null)
		   &&(coordToNodeMap.ContainsKey(reqKey)))
		{
			nodeID = coordToNodeMap[reqKey];
		}

		return nodeID;
	}
}

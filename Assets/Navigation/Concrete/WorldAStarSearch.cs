/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

public class WorldAStarSearch : AStarSearch
{
	protected override float g_func(ref NavGraph para_graph, ref NavNode para_node1, ref NavNode para_node2)
	{
		NavEdge reqEdge = para_graph.getEdge(para_node1.getNodeID(),para_node2.getNodeID());

		float retGVal = 0;
		if(reqEdge == null)
		{
			retGVal = float.PositiveInfinity;
		}
		else
		{
			retGVal = reqEdge.getCost();
		}
		return retGVal;
	}

	protected override float h_func(ref NavGraph para_graph, ref NavNode para_nodeID, ref NavNode para_goalNodeID)
	{
		WorldNode wN1 = (WorldNode) para_nodeID;
		WorldNode wN2 = (WorldNode) para_goalNodeID;

		float retHVal = UnityEngine.Vector3.Distance(wN1.getWorldPt(),wN2.getWorldPt());
		return retHVal;
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public abstract class AStarSearch : INavAlgorithm
{



	public List<int> searchForPath(ref NavGraph para_graph, NavNode para_sourceNode, NavNode para_destNode)
	{
		return searchForPath(ref para_graph,para_sourceNode,para_destNode,null);
	}

	public List<int> searchForPath(ref NavGraph para_graph, NavNode para_sourceNode, NavNode para_destNode, HashSet<int> para_untraversibleTypes)
	{

		int sourceNodeID = para_sourceNode.getNodeID();
		int destNodeID = para_destNode.getNodeID();

		HashSet<int> closedSet = new HashSet<int>();
		HashSet<int> openSet = new HashSet<int>();
		Dictionary<int,int> cameFromMap = new Dictionary<int,int>();

		Dictionary<int,float> gScoreMap = new Dictionary<int, float>();
		Dictionary<int,float> fScoreMap = new Dictionary<int, float>();
		
		openSet.Add(sourceNodeID);
		gScoreMap.Add(sourceNodeID,0);
		fScoreMap.Add(sourceNodeID, (gScoreMap[sourceNodeID] + h_func(ref para_graph,ref para_sourceNode,ref para_destNode)));

		bool foundPath = false;
		while((openSet.Count > 0)&&( ! foundPath))
		{
			// Search for node in openSet which has the lowest fScore.
			int smallestFScoreNodeID = -1;
			float smallestFScoreInSet = -1;
			foreach(int openItemID in openSet)
			{
				float tmpFScoreForNode = fScoreMap[openItemID];

				if((smallestFScoreInSet == -1)||(tmpFScoreForNode < smallestFScoreInSet))
				{
					smallestFScoreNodeID = openItemID;
					smallestFScoreInSet = tmpFScoreForNode;
				}
			}


			int currentNodeID = smallestFScoreNodeID;
			NavNode currentNode = para_graph.getNode(currentNodeID);

			if(currentNodeID == destNodeID)
			{
				foundPath = true;
			}
			else
			{
				openSet.Remove(currentNodeID);
				closedSet.Add(currentNodeID);

				HashSet<int> neighboursToCurrent = currentNode.getAllNeighbourIDs();
				foreach(int neighbourID in neighboursToCurrent)
				{
					if(closedSet.Contains(neighbourID))
					{
						continue;
					}
					else
					{
						NavNode neighbourNode = para_graph.getNode(neighbourID);

						if((para_untraversibleTypes == null)||( ! para_untraversibleTypes.Contains(neighbourNode.getNodeType())))
						{
							float tentativeGScore = gScoreMap[currentNodeID] + g_func(ref para_graph,ref currentNode,ref neighbourNode);

							if(( ! openSet.Contains(neighbourID))
							||(tentativeGScore < gScoreMap[neighbourID]))
							{
								cameFromMap[neighbourID] = currentNodeID;
								gScoreMap[neighbourID] = tentativeGScore;
								fScoreMap[neighbourID] = gScoreMap[neighbourID] + h_func(ref para_graph,ref neighbourNode,ref para_destNode);

								if( ! openSet.Contains(neighbourID))
								{
									openSet.Add(neighbourID);
								}
							}
						}
					}
				}

			}

		}



		if(foundPath)
		{
			// Success.
			List<int> reqPath = reconstructPath(ref cameFromMap, destNodeID);
			return reqPath;
		}
		else
		{
			// Failure.
			return null;
		}
	}



	private List<int> reconstructPath(ref Dictionary<int,int> para_cameFromMap, int para_currNodeID)
	{
		List<int> retList = new List<int>();

		if( ! para_cameFromMap.ContainsKey(para_currNodeID))
		{
			retList.Add(para_currNodeID);
		}
		else
		{
			List<int> subPath = reconstructPath(ref para_cameFromMap, para_cameFromMap[para_currNodeID]);
			retList.AddRange(subPath);
			retList.Add(para_currNodeID);
		}

		return retList;
	}



	protected abstract float g_func(ref NavGraph para_graph, ref NavNode para_node1, ref NavNode para_node2);		 // G Cost Function.
	protected abstract float h_func(ref NavGraph para_graph, ref NavNode para_nodeID, ref NavNode para_goalNodeID);  // H (Heuristic) Cost Function.
}

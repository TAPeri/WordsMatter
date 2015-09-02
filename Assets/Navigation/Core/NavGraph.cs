/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public abstract class NavGraph : INodeCollection, IEdgeCollection
{
	INodeCollection vertices;
	IEdgeCollection edges;
	INavAlgorithm navAlgorithm;


	public NavGraph()
	{
		// Used For Serialization Only.
	}

	public NavGraph(INodeCollection para_vertices,
	                IEdgeCollection para_edges,
	                INavAlgorithm para_navAlg)
	{
		vertices = para_vertices;
		edges = para_edges;
		navAlgorithm = para_navAlg;
	}
	

	// Node Collection Relay.
	public bool nodeExists(int para_nodeID) { return vertices.nodeExists(para_nodeID); }
	public List<int> getAllNodeKeys() { return vertices.getAllNodeKeys(); }
	public NavNode getNode(int para_nodeID)	{ return vertices.getNode(para_nodeID); }
	public List<int> getNodesOfType(int para_nodeType) { return vertices.getNodesOfType(para_nodeType); }
	public bool addNode(NavNode para_nwNode) { return vertices.addNode(para_nwNode); }
	public bool setNodeType(int para_nodeID,int para_type) { return vertices.setNodeType(para_nodeID,para_type); }
	public bool addNeighbourReferences(int para_node1ID, int para_node2ID) { return vertices.addNeighbourReferences(para_node1ID,para_node2ID); }
	public bool removeNeighbourReferences(int para_node1ID, int para_node2ID) { return vertices.removeNeighbourReferences(para_node1ID,para_node2ID); }
	public bool removeNode(int para_nodeID)
	{
		if(vertices.nodeExists(para_nodeID))
		{
			NavNode nodeToRemove = vertices.getNode(para_nodeID);
			HashSet<int> neighbours = nodeToRemove.getAllNeighbourIDs();

			foreach(int neighID in neighbours)
			{
				NavNode neighbourNode = vertices.getNode(neighID);
				if(neighbourNode != null)
				{
					this.removeEdge(para_nodeID,neighID);
				}
			}

			vertices.removeNode(para_nodeID);
		}
		return true;
	}


	// Edge Collection Relay.
	public List<string> getAllEdgeKeys() { return edges.getAllEdgeKeys(); }
	public NavEdge getEdge(string para_edgeKey) { return edges.getEdge(para_edgeKey); }
	public NavEdge getEdge(int para_node1ID, int para_node2ID) { return edges.getEdge(para_node1ID,para_node2ID); }
	public bool addEdge(int para_node1ID, int para_node2ID, NavEdge para_edgeData)
	{ 
		bool successFlag = edges.addEdge(para_node1ID,para_node2ID,para_edgeData);

		if(successFlag)
		{
			// Automaticaly place internal neighbour references.
			vertices.addNeighbourReferences(para_node1ID,para_node2ID);
		}

		return successFlag;
	}
	public bool removeEdge(int para_node1ID, int para_node2ID)
	{
		bool successFlag = edges.removeEdge(para_node1ID,para_node2ID);

		if(successFlag)
		{
			vertices.removeNeighbourReferences(para_node1ID,para_node2ID);
		}

		return successFlag;
	}
	public bool edgeExists(int para_node1ID, int para_node2ID) { return edges.edgeExists(para_node1ID,para_node2ID); }



	// Graph Operators.
	public List<NavNode> searchForPath(int para_sourceNodeID, int para_destNodeID)
	{
		if((this.nodeExists(para_sourceNodeID))
		   &&(this.nodeExists(para_destNodeID)))
		{	
			return searchForPath(this.getNode(para_sourceNodeID),this.getNode(para_destNodeID));
		}

		return null;
	}

	public List<NavNode> searchForPath(int para_sourceNodeID, int para_destNodeID, HashSet<int> para_untraversibleTypes)
	{
		if((this.nodeExists(para_sourceNodeID))
		   &&(this.nodeExists(para_destNodeID)))
		{
			return searchForPath(this.getNode(para_sourceNodeID),this.getNode(para_destNodeID),para_untraversibleTypes);
		}

		return null;
	}
	
	public List<NavNode> searchForPath(NavNode para_sourceNode, NavNode para_destNode)
	{
		NavGraph thisObj = this;
		List<int> pathIDs = navAlgorithm.searchForPath(ref thisObj,para_sourceNode,para_destNode);
		if(pathIDs == null)
		{
			return null;
		}
		else
		{
			List<NavNode> reqData = convertIDListToNodeList(pathIDs);
			return reqData;
		}
	}

	public List<NavNode> searchForPath(NavNode para_sourceNode, NavNode para_destNode, HashSet<int> para_untraversibleTypes)
	{
		NavGraph thisObj = this;
		List<int> pathIDs = navAlgorithm.searchForPath(ref thisObj,para_sourceNode,para_destNode,para_untraversibleTypes);
		if(pathIDs == null)
		{
			return null;
		}
		else
		{
			List<NavNode> reqData = convertIDListToNodeList(pathIDs);
			return reqData;
		}
	}



	public List<NavNode> getChildNodesAtHopDistance(NavNode para_sourceNode,
	                                                int para_distanceFromSrc,
	                                                HashSet<int> para_untraversibleTypes,
	                                                HashSet<int> para_untraversibleNodes)
	{
		
		// Uses modified Breadth First Search.
		List<NavNode> retList = new List<NavNode>();
		
		int depthLevel = 0;
		List<int> candidateNodeIDs = new List<int>();
		List<int> nwCandidateNodeIDs = new List<int>();
		HashSet<int> seenNodes = new HashSet<int>();
		candidateNodeIDs.Add(para_sourceNode.getNodeID());
		
		if(para_distanceFromSrc > 0)
		{
			do
			{
				for(int i=0; i<candidateNodeIDs.Count; i++)
				{
					int cNodeID = candidateNodeIDs[i];

					if( ! seenNodes.Contains(cNodeID))
					{ 
						seenNodes.Add(cNodeID);
						NavNode cNode = getNode(cNodeID);
						HashSet<int> cNodeNeighbours = cNode.getAllNeighbourIDs();

						foreach(int neighbourID in cNodeNeighbours)
						{
							bool isValid = true;
							if(depthLevel == (para_distanceFromSrc-1))
							{
								if(seenNodes.Contains(neighbourID))
								{
									isValid = false;
								}
							}

							if(para_untraversibleNodes != null)
							{
								if(para_untraversibleNodes.Contains(neighbourID))
								{
									isValid = false;
								}
							}

							if(para_untraversibleTypes != null)
							{
								if(para_untraversibleTypes.Contains(getNode(neighbourID).getNodeType()))
								{
									isValid = false;
								}
							}

							if(isValid)
							{
								nwCandidateNodeIDs.Add(neighbourID);
							}
						}
					}				
				}
				
				candidateNodeIDs.Clear();
				candidateNodeIDs = nwCandidateNodeIDs;
				nwCandidateNodeIDs = new List<int>();
				
				depthLevel++;
			}
			while((depthLevel < para_distanceFromSrc)&&(candidateNodeIDs.Count > 0));
		}


		for(int i=0; i<candidateNodeIDs.Count; i++)
		{
			retList.Add(getNode(candidateNodeIDs[i]));
		}
		
		return retList;
	}




	// Graph Helpers.
	private List<NavNode> convertIDListToNodeList(List<int> para_nodeIDs)
	{
		List<NavNode> reqList = new List<NavNode>();
		
		for(int i=0; i<para_nodeIDs.Count; i++)
		{
			NavNode nodeData = this.getNode(para_nodeIDs[i]);
			if(nodeData == null)
			{
				
			}
			reqList.Add(nodeData);
		}
		
		return reqList;
	}
	
	private List<int> convertNodeListToIDList(List<NavNode> para_nodeList)
	{
		List<int> reqList = new List<int>();
		
		for(int i=0; i<para_nodeList.Count; i++)
		{
			reqList.Add(para_nodeList[i].getNodeID());
		}
		
		return reqList;
	}


}
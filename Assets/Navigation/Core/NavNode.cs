/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class NavNode
{
	int navNodeID;
	int nodeType;

	HashSet<int> neighbours;
	
	public NavNode(int para_nodeID, int para_nodeType)
	{
		navNodeID = para_nodeID;
		nodeType = para_nodeType;
		neighbours = new HashSet<int>();
	}

	public NavNode(int para_nodeID, int para_nodeType, HashSet<int> para_neighbours)
	{
		UnityEngine.Debug.LogWarning("New this and para_nodeID!");
		this.navNodeID = para_nodeID;
		nodeType = para_nodeType;
		neighbours = para_neighbours;
	}

	public int getNodeID()   { return navNodeID; }
	public int getNodeType() { return nodeType; }
	public HashSet<int> getAllNeighbourIDs() { return neighbours; }

	public void setNodeType(int para_type)
	{
		nodeType = para_type;
	}

	public void addNeighbourRef(int para_neighbourID)
	{
		if( ! neighbours.Contains(para_neighbourID))
		{
			neighbours.Add(para_neighbourID);
		}
	}

	public void removeNeighbourRef(int para_neighbourID)
	{
		if(neighbours.Contains(para_neighbourID))
		{
			neighbours.Remove(para_neighbourID);
		}
	}

	public bool isNeighbour(int para_testNodeID)
	{
		return (neighbours.Contains(para_testNodeID));
	}

	public bool isNeighbour(NavNode para_testNode)
	{
		return (neighbours.Contains(para_testNode.getNodeID()));
	}
}
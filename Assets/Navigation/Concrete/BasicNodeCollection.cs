/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class BasicNodeCollection : INodeCollection
{

	Dictionary<int,NavNode> vertices;



	public BasicNodeCollection()
	{
		vertices = new Dictionary<int, NavNode>();
	}





	public bool nodeExists(int para_nodeID)
	{
		return (vertices.ContainsKey(para_nodeID));
	}

	public List<int> getAllNodeKeys()
	{
		List<int> keys = new List<int>(vertices.Keys);
		return keys;
	}

	public NavNode getNode(int para_nodeID)
	{
		NavNode retData = null;
		if(vertices.ContainsKey(para_nodeID))
		{
			retData = vertices[para_nodeID];
		}
		return retData;
	}

	public List<int> getNodesOfType(int para_nodeType)
	{
		List<int> retKeys = new List<int>();

		foreach(KeyValuePair<int,NavNode> pair in vertices)
		{
			if(pair.Value.getNodeType() == para_nodeType)
			{
				retKeys.Add(pair.Value.getNodeID());
			}
		}

		return retKeys;
	}
	
	public bool addNode(NavNode para_nwNode)
	{
		bool successFlag = false;
		if( ! vertices.ContainsKey(para_nwNode.getNodeID()))
		{
			vertices.Add(para_nwNode.getNodeID(),para_nwNode);
			successFlag = true;
		}
		return successFlag;
	}

	public bool setNodeType(int para_nodeID,int para_type)
	{
		bool successFlag = false;
		if(vertices.ContainsKey(para_nodeID))
		{
			NavNode tmpNode = vertices[para_nodeID];
			tmpNode.setNodeType(para_type);
			vertices[para_nodeID] = tmpNode;
			successFlag = true;
		}
		return successFlag;
	}

	public bool addNeighbourReferences(int para_node1ID, int para_node2ID)
	{
		bool successFlag = false;
		if((vertices.ContainsKey(para_node1ID))
		&&(vertices.ContainsKey(para_node2ID)))
		{
			NavNode tmpNode1 = vertices[para_node1ID];
			NavNode tmpNode2 = vertices[para_node2ID];

			if((tmpNode1 != null)
			&&(tmpNode2 != null))
			{
				tmpNode1.addNeighbourRef(para_node2ID);
				tmpNode2.addNeighbourRef(para_node1ID);
				vertices[para_node1ID] = tmpNode1;
				vertices[para_node2ID] = tmpNode2;
				successFlag = true;
			}
		}
		return successFlag;
	}

	public bool removeNeighbourReferences(int para_node1ID, int para_node2ID)
	{
		bool successFlag = false;
		if((vertices.ContainsKey(para_node1ID))
		&&(vertices.ContainsKey(para_node2ID)))
		{
			NavNode tmpNode1 = vertices[para_node1ID];
			NavNode tmpNode2 = vertices[para_node2ID];

			if((tmpNode1 != null)
			&&(tmpNode2 != null))
			{
				tmpNode1.removeNeighbourRef(para_node2ID);
				tmpNode2.removeNeighbourRef(para_node1ID);
				vertices[para_node1ID] = tmpNode1;
				vertices[para_node2ID] = tmpNode2;
				successFlag = true;
			}
		}
		return successFlag;
	}


	public bool removeNode(int para_nodeID)
	{
		if(vertices.ContainsKey(para_nodeID))
		{
			vertices.Remove(para_nodeID);
		}
		return true;
	}

}

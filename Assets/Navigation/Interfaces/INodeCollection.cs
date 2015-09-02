/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public interface INodeCollection
{
	bool nodeExists(int para_nodeID);
	List<int> getAllNodeKeys();
	NavNode getNode(int para_nodeID);
	List<int> getNodesOfType(int para_nodeType);
	bool addNode(NavNode para_nwNode);
	bool setNodeType(int para_nodeID,int para_type);
	bool addNeighbourReferences(int para_node1ID, int para_node2ID);
	bool removeNeighbourReferences(int para_node1ID, int para_node2ID);
	bool removeNode(int para_nodeID);
}

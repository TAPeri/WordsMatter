/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public interface IEdgeCollection
{
	List<string> getAllEdgeKeys();
	NavEdge getEdge(string para_edgeKey);
	NavEdge getEdge(int para_node1ID, int para_node2ID);
	bool addEdge(int para_node1ID, int para_node2ID, NavEdge para_edgeData);
	bool removeEdge(int para_node1ID, int para_node2ID);
	bool edgeExists(int para_node1ID, int para_node2ID);
}

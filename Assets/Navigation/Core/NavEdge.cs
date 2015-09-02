/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class NavEdge
{
	int[] nodeIDs;
	float cost;

	public NavEdge(int[] para_nodeIDs, float para_cost)
	{
		nodeIDs = para_nodeIDs;
		cost = para_cost;
	}

	public int[] getNodeIDs() { return nodeIDs; }
	public float getCost() { return cost; }
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class BasicEdgeCollection : IEdgeCollection
{

	Dictionary<string,NavEdge> edges;
	string[] tmpEdgeKeys;


	public BasicEdgeCollection()
	{
		edges = new Dictionary<string, NavEdge>();
		tmpEdgeKeys = new string[2] {"",""};
	}

	public List<string> getAllEdgeKeys()
	{
		List<string> keys = new List<string>(edges.Keys);
		return keys;
	}

	public NavEdge getEdge(string para_edgeKey)
	{
		NavEdge retData = null;
		if(edges.ContainsKey(para_edgeKey))
		{
			retData = edges[para_edgeKey];
		}
		return retData;
	}

	public NavEdge getEdge(int para_node1ID, int para_node2ID)
	{
		NavEdge retData = null;

		producePotentialEdgeKeys(para_node1ID,para_node2ID);
		if((edges.ContainsKey(tmpEdgeKeys[0]))
		|| (edges.ContainsKey(tmpEdgeKeys[1])))
		{
			if(edges.ContainsKey(tmpEdgeKeys[0]))
			{
				retData = edges[tmpEdgeKeys[0]];
			}
			else
			{
				retData = edges[tmpEdgeKeys[1]];
			}		
		}

		return retData;		
	}

	public bool addEdge(int para_node1ID, int para_node2ID, NavEdge para_edgeData)
	{
		bool successFlag = false;

		producePotentialEdgeKeys(para_node1ID,para_node2ID);
		if(( ! edges.ContainsKey(tmpEdgeKeys[0]))
		 &&( ! edges.ContainsKey(tmpEdgeKeys[1])))
		{
			edges.Add(createEdgeKey(para_node1ID,para_node2ID),para_edgeData);
			successFlag = true;
		}

		return successFlag;
	}

	public bool removeEdge(int para_node1ID, int para_node2ID)
	{
		bool successFlag = false;

		producePotentialEdgeKeys(para_node1ID,para_node2ID);
		for(int i=0; i<tmpEdgeKeys.Length; i++)
		{
			if(edges.ContainsKey(tmpEdgeKeys[i]))
			{
				edges.Remove(tmpEdgeKeys[i]);
			}
		}

		successFlag = true;
		return successFlag;
	}

	public bool edgeExists(int para_node1ID, int para_node2ID)
	{
		producePotentialEdgeKeys(para_node1ID,para_node2ID);
		if((edges.ContainsKey(tmpEdgeKeys[0]))
		|| (edges.ContainsKey(tmpEdgeKeys[1])))
		{
			return true;
		}
		else
		{
			return false;
		}
	}



	private string createEdgeKey(int para_node1ID, int para_node2ID)
	{
		return (para_node1ID+"-"+para_node2ID);
	}

	private void producePotentialEdgeKeys(int para_node1ID, int para_node2ID)
	{
		tmpEdgeKeys[0] = para_node1ID+"-"+para_node2ID;
		tmpEdgeKeys[1] = para_node2ID+"-"+para_node1ID;
	}
}

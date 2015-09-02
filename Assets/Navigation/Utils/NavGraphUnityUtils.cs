/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class NavGraphUnityUtils
{
	


	public static GameObject renderNavGraph(string para_renderName, NavGraph para_graph, Transform para_graphNodePrefab)
	{
		GameObject navGraphRenderObj = new GameObject(para_renderName);



		// Extract color info if needed.
		Color defaultColor = Color.white;
		Dictionary<int,Color> nodeTypeIDToColorMap = null;
		if(para_graph is ColoredNavGraph)
		{
			ColoredNavGraph castGraph = (ColoredNavGraph) para_graph;
			nodeTypeIDToColorMap = castGraph.createTypeIDToColorMap();
		}



		
		// Render Nodes:
		GameObject navNodesObj = new GameObject("Nodes");
		
		List<int> nodeKeys = para_graph.getAllNodeKeys();
		for(int i=0; i<nodeKeys.Count; i++)
		{
			WorldNode reqNode = (WorldNode) para_graph.getNode(nodeKeys[i]);
			int reqNodeType = reqNode.getNodeType();
			/*int[] imgLocPt = reqNode.get2DLocationPt();

			Vector3 nodeWorldPt = new Vector3((gPropMapWorldBounds.x + (gPropMapWorldBounds.cellWidth/2f)) + (gPropMapWorldBounds.cellWidth * (imgLocPt[0] * 1.0f)),
			                                  (gPropMapWorldBounds.y - (gPropMapWorldBounds.cellHeight/2f)) - (gPropMapWorldBounds.cellHeight * (imgLocPt[1] * 1.0f)),
			                                  gPropMapWorldBounds.z);*/
			
			Vector3 nodeWorldPt = reqNode.getWorldPt();
			
			
			
			Transform nwNodePt = (Transform) Transform.Instantiate(para_graphNodePrefab,nodeWorldPt,Quaternion.identity);
			nwNodePt.name = "Node-"+reqNode.getNodeID();
			nwNodePt.parent = navNodesObj.transform;


			Color paintColor = defaultColor;
			if((nodeTypeIDToColorMap != null)
			&&(nodeTypeIDToColorMap.ContainsKey(reqNodeType)))
			{
				paintColor = nodeTypeIDToColorMap[reqNodeType];
			}
			nwNodePt.renderer.material.color = paintColor;			
		}
		
		
		
		// Render Edges:
		GameObject navEdgesObj = new GameObject("Edges");
		
		List<string> edgeKeys = para_graph.getAllEdgeKeys();
		for(int i=0; i<edgeKeys.Count; i++)
		{
			NavEdge reqEdge = para_graph.getEdge(edgeKeys[i]);
			int[] nodesOnEdge = reqEdge.getNodeIDs();
			
			Vector3 node1Pt = (GameObject.Find("Node-"+nodesOnEdge[0])).transform.position;
			Vector3 node2Pt = (GameObject.Find("Node-"+nodesOnEdge[1])).transform.position;
			
			GameObject nwEdge = new GameObject("Edge:"+nodesOnEdge[0]+"-"+nodesOnEdge[1]);
			LineRenderer lRend = nwEdge.AddComponent<LineRenderer>();

			lRend.castShadows = false;
			lRend.receiveShadows = false;
			lRend.SetVertexCount(2);
			lRend.SetWidth(0.1f,0.1f);
			lRend.SetPosition(0,node1Pt);
			lRend.SetPosition(1,node2Pt);
			lRend.SetColors(Color.yellow,Color.yellow);
			lRend.material = new Material(Shader.Find("Diffuse"));//"Particles/Additive"));
			
			nwEdge.transform.parent = navEdgesObj.transform;
		}
		
		
		
		// Finish Object.
		navNodesObj.transform.parent = navGraphRenderObj.transform;
		navEdgesObj.transform.parent = navGraphRenderObj.transform;
		

		return navGraphRenderObj;
	}


	public static void highlightNavNodes(string para_renderName, List<NavNode> para_nodes)
	{
		GameObject navGraphRenderObj = GameObject.Find(para_renderName);
		if(navGraphRenderObj != null)
		{
			GameObject nodesChildObj = (navGraphRenderObj.transform.FindChild("Nodes")).gameObject;
			
			for(int i=0; i<para_nodes.Count; i++)
			{
				GameObject reqNodeObj = (nodesChildObj.transform.FindChild("Node-"+para_nodes[i].getNodeID())).gameObject;
				reqNodeObj.renderer.material.color = Color.yellow;
			}
		}
	}

	public static void clearAllNavNodeHighlights(string para_renderName)
	{
		GameObject navGraphRenderObj = GameObject.Find(para_renderName);
		if(navGraphRenderObj != null)
		{
			GameObject nodesChildObj = (navGraphRenderObj.transform.FindChild("Nodes")).gameObject;
			
			for(int i=0; i<nodesChildObj.transform.childCount; i++)
			{
				GameObject reqNodeObj = nodesChildObj.transform.GetChild(i).gameObject;
				reqNodeObj.renderer.material.color = Color.white;
			}
		}
	}

}

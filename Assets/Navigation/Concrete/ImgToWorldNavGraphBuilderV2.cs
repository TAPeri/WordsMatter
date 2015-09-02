/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;


// V2 allows you to obtain multiple graphs from an image with just one call.
// Graphs can now support multiple colors.
public class ImgToWorldNavGraphBuilderV2 : INavGraphBuilder
{


	string imgPath;
	GridProperties worldGProp;
	Dictionary<Color,List<int>> colourDestinationGraphMap;
	int totNumOfGraphs;

	
	Color[] pixels;
	GraphTmpMetaData[] graphMetaDataArr;


	
	public ImgToWorldNavGraphBuilderV2(string para_imgPath,
	                                   GridProperties para_worldGProp)
	{
		imgPath = para_imgPath;
		worldGProp = para_worldGProp;
	}

	private void prepDatastructures(List<ColorGraphTypeInfo> para_requirements)
	{
		totNumOfGraphs = para_requirements.Count;
		colourDestinationGraphMap = new Dictionary<Color, List<int>>();
		graphMetaDataArr = new GraphTmpMetaData[totNumOfGraphs];


		// Identify which graphs will need which colours in order to make the process faster.
		// With this, we only need to read through the img file once in order to produce all the graphs.
		for(int i=0; i<para_requirements.Count; i++)
		{
			ColorGraphTypeInfo currReqs = para_requirements[i];
			//string[] typeNames = currReqs.typeNames;
			Color[] typeColors = currReqs.typeColors;

			for(int j=0; j<typeColors.Length; j++)
			{
				Color tmpColor = typeColors[j];

				if( ! colourDestinationGraphMap.ContainsKey(tmpColor))
				{
					colourDestinationGraphMap.Add(tmpColor,new List<int>());
				}
				colourDestinationGraphMap[tmpColor].Add(i);
			}

			graphMetaDataArr[i] = new GraphTmpMetaData(typeColors);
		}
	}



	public NavGraph constructGraph()
	{
		return null;
	}


	// Nodes are added by moving a single cell.
	// Edges are initially one length edges.
	// Post processing can reduce the number of nodes.
	//
	//		# # #
	//		# X #
	//		# # #
	//
	// X = Node checking.
	// # = Neighbour/Graph Edge checking.
	//
	public List<ColoredNavGraph> constructColorGraphs(List<ColorGraphTypeInfo> para_requirements)
	{

		List<ColoredNavGraph> retGraphList = new List<ColoredNavGraph>();


		// Prep phase.
		prepDatastructures(para_requirements);
		for(int i=0; i<totNumOfGraphs; i++)
		{
			retGraphList.Add(new ColoredNavGraph(para_requirements[i]));
		}





		// Go Time!
				
		Texture2D mapImg = Resources.Load<Texture2D>(imgPath);
		if(mapImg != null)
		{
			// NOTE: UNITY RETURNS THIS ARRAY WHICH STARTS WITH ROWS BOTTOM-UP.
			pixels = mapImg.GetPixels();
			int[] mapImgWNH = new int[2] { mapImg.width, mapImg.height };


			for(int r=0; r<mapImg.height; r++)
			{
				for(int c=0; c<mapImg.width; c++)
				{
					// (pixels.Length-mapImg.width) - This is to navigate the image top down while still abiding with Unity's returned pixel array which goes bottom up.
					Color tmpPixel = pixels[(pixels.Length - mapImg.width) - (r * mapImg.width) + c];
					

					if(colourDestinationGraphMap.ContainsKey(tmpPixel))
					{

						List<int> relevantGraphIDs = colourDestinationGraphMap[tmpPixel];
						for(int i=0; i<relevantGraphIDs.Count; i++)
						{
							int reqGraphID = relevantGraphIDs[i];
							includeNode(reqGraphID,retGraphList[reqGraphID],c,r,tmpPixel,mapImgWNH);
						}

					}


				}
			}
		}


		return retGraphList;
	}





	// By default, C# creates param: para_graph as a new reference to the object. It does not copy the object but it creates a new reference.
	// The ref keyword refers to the same object but prevents duplicating the reference.
	private void includeNode(int para_graphID,
	                         NavGraph para_graph,
	                         int para_nodeX,
	                         int para_nodeY,
	                         Color para_nodeColor,
	                         int[] para_imgWNH)	// img width and height
	{
		// PREP.
		NavGraph retGraph = para_graph;
		GraphTmpMetaData reqGraphMetaData = graphMetaDataArr[para_graphID];
		int c = para_nodeX;
		int r = para_nodeY;
		int nxtNodeID = reqGraphMetaData.nxtNodeID;
		int nxtEdgeID = reqGraphMetaData.nxtEdgeID;
		Dictionary<string,int> coordToNodeIDMap = reqGraphMetaData.coordToNodeIDMap;




		// GO.


		
		// Add new node.
		WorldNode nwNode = new WorldNode(nxtNodeID,
		                                 reqGraphMetaData.colorToNodeType[para_nodeColor],
		                                 new Vector3((worldGProp.x + (worldGProp.cellWidth/2f)) + (worldGProp.cellWidth * c),
										             (worldGProp.y - (worldGProp.cellWidth/2f)) - (worldGProp.cellHeight * r),
										             worldGProp.z));
		
		retGraph.addNode(nwNode);
		coordToNodeIDMap.Add((""+c+"-"+r),nwNode.getNodeID());
		nxtNodeID++;
		
		
		// Get valid neighbourhood pixels.
		List<Color> neighbourhoodPixels = new List<Color>();
		List<int[]> reqNPixCoords = new List<int[]>();
		
		
		
		
		
		//reqNPixCoords.Add(new int[2]{(c-1),(r-1)});
		reqNPixCoords.Add(new int[2]{(c),(r-1)});
		//reqNPixCoords.Add(new int[2]{(c+1),(r-1)});
		reqNPixCoords.Add(new int[2]{(c-1),(r)});
		List<int> validCoordIndexList = new List<int>();
		for(int k=0; k<reqNPixCoords.Count; k++)
		{
			int[] tmpCoords = reqNPixCoords[k];
			
			if((tmpCoords[0] >= 0)&&(tmpCoords[0] < para_imgWNH[0])            // mapImg.width)
			   &&(tmpCoords[1] >= 0)&&(tmpCoords[1] < para_imgWNH[1]))		   // mapImg.height))
			{
				// Valid coords.
				validCoordIndexList.Add(k);
				neighbourhoodPixels.Add( pixels[(pixels.Length - para_imgWNH[0]) - (tmpCoords[1] * para_imgWNH[0]) + tmpCoords[0]] );
			}
		}
		
		
		// Check for neighbours and create edges. (NavGraph will handle the internal creation of neighbour references, just run addEdge).
		for(int k=0; k<validCoordIndexList.Count; k++)
		{

			if(colourDestinationGraphMap.ContainsKey(neighbourhoodPixels[k]))
			{
				if(colourDestinationGraphMap[neighbourhoodPixels[k]].Contains(para_graphID))
				{
					int[] neighbourCoords = reqNPixCoords[k];
					int neighbourID = coordToNodeIDMap[(""+neighbourCoords[0]+"-"+neighbourCoords[1])];
								
					retGraph.addEdge(nwNode.getNodeID(),neighbourID,new NavEdge(new int[2] {nwNode.getNodeID(),neighbourID},1));
				}
			}
		}




		// POST.

		reqGraphMetaData.nxtNodeID = nxtNodeID;
		reqGraphMetaData.nxtEdgeID = nxtEdgeID;
		reqGraphMetaData.coordToNodeIDMap = coordToNodeIDMap;
		graphMetaDataArr[para_graphID] = reqGraphMetaData;
	}


	private class GraphTmpMetaData
	{
		public int nxtNodeID;
		public int nxtEdgeID;
		public Dictionary<string,int> coordToNodeIDMap;
		public Dictionary<Color,int> colorToNodeType;

		public GraphTmpMetaData(Color[] para_acceptedColors)
		{
			nxtNodeID = 0;
			nxtEdgeID = 0;
			coordToNodeIDMap = new Dictionary<string,int>();
			colorToNodeType = new Dictionary<Color, int>();
			for(int i=0; i<para_acceptedColors.Length; i++)
			{
				if( ! colorToNodeType.ContainsKey(para_acceptedColors[i]))
				{
					colorToNodeType.Add(para_acceptedColors[i],i);
				}
				else
				{
					// Error in Color spec if enters here.
					colorToNodeType[para_acceptedColors[i]] = i;
				}
			}
		}
	}


}

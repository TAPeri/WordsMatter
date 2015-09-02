/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ImgToWorldNavGraphBuilder : INavGraphBuilder
{

	string imgPath;
	//int[] mapDimensions;
	GridProperties worldGProp;
	Color traversibleColor;

	Color[] pixels;
	
	
	public ImgToWorldNavGraphBuilder(string para_imgPath,
	                                 int[] para_mapDimensions,
	                                 GridProperties para_worldGProp)
	{
		imgPath = para_imgPath;
		//mapDimensions = para_mapDimensions;
		worldGProp = para_worldGProp;
		traversibleColor = this.convertColor(new int[4] {0,0,0,255});
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
	public NavGraph constructGraph()
	{
		
		BasicNavGraph retGraph = new BasicNavGraph();
		
		int nxtNodeID = 0;
		//int nxtEdgeID = 0;
		Dictionary<string,int> coordToNodeIDMap = new Dictionary<string,int>();
		
		
		
		Texture2D mapImg = Resources.Load<Texture2D>(imgPath);
		if(mapImg != null)
		{
			// NOTE: UNITY RETURNS THIS ARRAY WHICH STARTS WITH ROWS BOTTOM-UP.
			pixels = mapImg.GetPixels();
			
			for(int r=0; r<mapImg.height; r++)
			{
				for(int c=0; c<mapImg.width; c++)
				{
					// (pixels.Length-mapImg.width) - This is to navigate the image top down while still abiding with Unity's returned pixel array which goes bottom up.
					Color tmpPixel = pixels[(pixels.Length - mapImg.width) - (r * mapImg.width) + c];





					if(tmpPixel.Equals(traversibleColor))
					{
						// Add new node.
						WorldNode nwNode = new WorldNode(nxtNodeID,1, new Vector3((worldGProp.x + (worldGProp.cellWidth/2f)) + (worldGProp.cellWidth * c),
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
							
							if((tmpCoords[0] >= 0)&&(tmpCoords[0] < mapImg.width)
							   &&(tmpCoords[1] >= 0)&&(tmpCoords[1] < mapImg.height))
							{
								// Valid coords.
								validCoordIndexList.Add(k);
								neighbourhoodPixels.Add( pixels[(pixels.Length - mapImg.width) - (tmpCoords[1] * mapImg.width) + tmpCoords[0]] );
							}
						}
						
						
						// Check for neighbours and create edges. (NavGraph will handle the internal creation of neighbour references, just run addEdge).
						for(int k=0; k<validCoordIndexList.Count; k++)
						{
							if(neighbourhoodPixels[k].Equals(traversibleColor))
							{
								int[] neighbourCoords = reqNPixCoords[k];
								int neighbourID = coordToNodeIDMap[(""+neighbourCoords[0]+"-"+neighbourCoords[1])];
								
								
								retGraph.addEdge(nwNode.getNodeID(),neighbourID,new NavEdge(new int[2] {nwNode.getNodeID(),neighbourID},1));
							}
						}
					}
				}
			}
		}
		
		return retGraph;
	}
	
	
	
	
	// Helpers.


	private void checkForSurroundingsAtPoint(int[] para_cellCoords, Color[] para_colorPattern)
	{
		List<int[]> posToCheckArr = new List<int[]>();
		posToCheckArr.Add(new int[2] { para_cellCoords[0]-1, para_cellCoords[1]-1 });

	}

	
	private Color convertColor(int[] para_vals)
	{
		if(para_vals.Length == 3)
		{
			return convertColor(para_vals[0],para_vals[1],para_vals[2],255); // a:255 = solid colour
		}
		else if(para_vals.Length == 4)
		{
			return convertColor(para_vals[0],para_vals[1],para_vals[2],para_vals[3]);
		}
		else
		{
			return Color.magenta;
		}							
	}
	
	private Color convertColor(int para_r, int para_g, int para_b, int para_a)
	{
		return (new Color(para_r/255.0f,para_g/255.0f,para_b/255.0f,para_a/255.0f)); 
	}

}

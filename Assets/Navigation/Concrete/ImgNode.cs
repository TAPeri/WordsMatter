/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class ImgNode : NavNode
{
	int[] location2D;
	
	public ImgNode(int para_nodeID, int para_nodeType, int[] para_loc)
		:base(para_nodeID,para_nodeType)
	{
		location2D = para_loc;
	}
	
	public int[] get2DLocationPt()	{ return location2D; }
}

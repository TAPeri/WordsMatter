/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class WorldNode : NavNode
{
	UnityEngine.Vector3 worldPt;
	
	public WorldNode(int para_nodeID, int para_nodeType, UnityEngine.Vector3 para_worldPt)
		:base(para_nodeID,para_nodeType)
	{
		worldPt = para_worldPt;
	}

	public UnityEngine.Vector3 getWorldPt()	{ return worldPt; }


	public string getDirectionFromForeignPt(UnityEngine.Vector3 para_ptLoc)
	{
		string strDir = getDirectionToForeignNode(para_ptLoc);
		if(strDir == "N") { strDir = "S"; }
		else if(strDir == "S") { strDir = "N"; }
		else if(strDir == "W") { strDir = "E"; }
		else if(strDir == "E") { strDir = "W"; }
		else if(strDir == "NW") { strDir = "SE"; }
		else if(strDir == "NE") { strDir = "SW"; }
		else if(strDir == "SW") { strDir = "NE"; }
		else if(strDir == "SE") { strDir = "NW"; }
		else { strDir = "NONE"; }

		return strDir;
	}

	public string getDirectionToForeignNode(WorldNode para_node2)
	{
		return getDirectionToForeignNode(para_node2.getWorldPt());
	}

	public string getDirectionToForeignNode(UnityEngine.Vector3 para_node2Loc)
	{
		UnityEngine.Vector3 nodeLoc = para_node2Loc;

		if((nodeLoc.x == worldPt.x)&&(nodeLoc.y > worldPt.y))       {  return "N";  } // NORTH
		else if((nodeLoc.x == worldPt.x)&&(nodeLoc.y < worldPt.y))  {  return "S";  } // SOUTH
		else if((nodeLoc.x < worldPt.x)&&(nodeLoc.y == worldPt.y))	{  return "W";  } // WEST
		else if((nodeLoc.x > worldPt.x)&&(nodeLoc.y == worldPt.y))	{  return "E";  } // EAST
		else if((nodeLoc.x < worldPt.x)&&(nodeLoc.y > worldPt.y))	{  return "NW"; } // NW
		else if((nodeLoc.x > worldPt.x)&&(nodeLoc.y > worldPt.y))	{  return "NE"; } // NE
		else if((nodeLoc.x < worldPt.x)&&(nodeLoc.y < worldPt.y))	{  return "SW"; } // SW
		else if((nodeLoc.x > worldPt.x)&&(nodeLoc.y < worldPt.y))	{  return "SE"; } // SE
		else { return "NONE"; }
	}


}

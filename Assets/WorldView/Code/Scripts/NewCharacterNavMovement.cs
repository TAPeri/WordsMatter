/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class NewCharacterNavMovement : MonoBehaviour, IActionNotifier, CustomActionListener
{
	
	List<NavNode> nodesToFollow;
	float charSpeed;
	bool faceDownWhenOver;
	bool stopOneNodeShort;
	

	public bool doNotDestroy;



	public void moveToWorldPoint(Vector3 para_worldPt,
	                             float para_charSpeed,
	                             bool para_faceDownWhenOver)
	{
		List<NavNode> dummyNodeList = new List<NavNode>();
		dummyNodeList.Add(new WorldNode(0,1,para_worldPt));
		moveAlongPath(dummyNodeList,para_charSpeed,para_faceDownWhenOver,false);
	}
	
	public void moveAlongPath(List<NavNode> para_nodes,
	                          float para_charSpeed,
	                          bool para_faceDownWhenOver,
	                          bool para_stopOneNodeShort)
	{
		if((para_nodes == null)||(para_nodes.Count == 0))
		{
			notifyAllListeners(transform.gameObject.name,"EmptyPathList",null);
			Destroy(this);
		}
		else
		{
			Destroy(transform.GetComponent<MoveToLocation>());
			
			Animator aniScript = transform.GetComponent<Animator>();
			aniScript.speed = 1;
			
			nodesToFollow = para_nodes;
			charSpeed = para_charSpeed;
			faceDownWhenOver = para_faceDownWhenOver;
			stopOneNodeShort = para_stopOneNodeShort;
			
			if((stopOneNodeShort)&&(nodesToFollow.Count == 1))
			{
				signalPathComplete();
			}
			else
			{
				moveToNextNode();
			}
		}
	}

	public void setToIdle()
	{



		//MoveToLocation a = transform.GetComponent<MoveToLocation>();



		Destroy(transform.GetComponent<MoveToLocation>());
		if(nodesToFollow != null) { nodesToFollow.Clear(); }

		Animator aniScript = transform.GetComponent<Animator>();
		aniScript.Play("Idle");//setTrigger("Idle");
	}
	
	public void faceDirection(string para_dirCode)
	{
		changeAnimDependingOnDirection(para_dirCode,false);
		Animator aniScript = transform.gameObject.GetComponent<Animator>();
		aniScript.speed = 1;
		//aniScript.speed = 0;
	}
	
	
	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_sourceID == transform.name)
		{
			if(para_eventID == "MoveToLocation")
			{
				if((nodesToFollow != null)&&(nodesToFollow.Count > 0)) // ?
				{
					nodesToFollow.RemoveAt(0);
					if((nodesToFollow.Count == 0)
					   ||((stopOneNodeShort)&&(nodesToFollow.Count == 1)))
					{
						signalPathComplete();
					}
					else
					{
						moveToNextNode();
					}
				}
			}
		}
	}
	
	
	
	
	private void signalPathComplete()
	{
		if(stopOneNodeShort)
		{
			string dirToFinalNode = ((WorldNode) nodesToFollow[0]).getDirectionFromForeignPt(transform.position);
			changeAnimDependingOnDirection(dirToFinalNode,false);
			
		}
		else
		{
			Animator aniScript = transform.gameObject.GetComponent<Animator>();
			if(faceDownWhenOver)
			{
				aniScript.Play("FrontWalk");//.setTrigger("FrontWalk");
				
				Transform worldPivotChild = transform.FindChild("WorldPivot");
				if(worldPivotChild != null) { worldPivotChild.eulerAngles = new Vector3(0,0,0); }
			}
		}
		Animator aniScript2 = transform.gameObject.GetComponent<Animator>();
		aniScript2.speed = 0;
		
		
		notifyAllListeners(transform.gameObject.name,"PathComplete",null);
		
		if(!doNotDestroy)
		{
			Destroy(this);
		}
	}
	
	
	private void moveToNextNode()
	{
		WorldNode firstNode = (WorldNode) nodesToFollow[0];
		Vector3 nodeLoc = firstNode.getWorldPt();
		
		
		// Change sprite animation.
		string dirToNextNode = firstNode.getDirectionFromForeignPt(transform.position);
		changeAnimDependingOnDirection(dirToNextNode,true);
		
		
		
		MoveToLocation mtl = transform.gameObject.AddComponent<MoveToLocation>();

		//MoveToLocation a = transform.GetComponent<MoveToLocation>();



		mtl.initScript(new Vector3(nodeLoc.x,nodeLoc.y,transform.position.z),charSpeed);
		mtl.registerListener("CharMov",this);
	}
	
	private void changeAnimDependingOnDirection(string para_dirCode, bool para_isMoving)
	{
		string dirToNextNode = para_dirCode;

		//transform.eulerAngles = new Vector3(0,0,0);
		Animator aniScript = transform.gameObject.GetComponent<Animator>();

		string[] animNames = null;
		if(para_isMoving) { animNames = new string[] {"BackWalk","FrontWalk","SideWalkL","SideWalkR"}; }
		else { animNames = new string[] {"Idle_Back","Idle","Idle_L","Idle_R"}; }
		
		if(dirToNextNode == "N") {	aniScript.Play(animNames[0]); }		// NORTH
		else if(dirToNextNode == "S")	{ aniScript.Play(animNames[1]);	} // SOUTH
		else if((dirToNextNode == "W")||(dirToNextNode == "NW")||(dirToNextNode == "SW")) { aniScript.Play(animNames[2]); } // WEST, NW, SW
		else if((dirToNextNode == "E")||(dirToNextNode == "NE")||(dirToNextNode == "SE")) 	// EAST, NE, SE
		{
			//Vector3 tmpEAngles = transform.eulerAngles;
			//tmpEAngles.y = 180;
			//transform.eulerAngles = tmpEAngles;
			aniScript.Play(animNames[3]);//transform.name+"_walkLeft");
		}
		else
		{
			// Idle.
		}
		
		
		// If the object contains a WorldPivot child then rotate this depending on the facing direction.
		// Used mainly for sprite based items. Assumes default face down.
		Transform worldPivotChild = transform.FindChild("WorldPivot");
		if(worldPivotChild != null)
		{
			if(dirToNextNode == "N") { worldPivotChild.eulerAngles = new Vector3(0,0,180); }
			else if(dirToNextNode == "S") { worldPivotChild.eulerAngles = new Vector3(0,0,0); }
			else if(dirToNextNode == "W") { worldPivotChild.eulerAngles = new Vector3(0,0,-90); }
			else if(dirToNextNode == "E") { worldPivotChild.eulerAngles = new Vector3(0,0,-90); }// Tmp fix. should be 0,0,90.
		}
	}
	

	public string getDirectionToForeignPoint(Vector3 para_node2Loc)
	{
		Vector3 nodeLoc = para_node2Loc;
		Vector3 worldPt = transform.position;

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

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

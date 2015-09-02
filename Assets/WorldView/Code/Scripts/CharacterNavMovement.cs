/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class CharacterNavMovement : MonoBehaviour, IActionNotifier, CustomActionListener
{

	List<NavNode> nodesToFollow;
	float charSpeed;
	bool faceDownWhenOver;
	bool stopOneNodeShort;



	string aniPrefix;

	public bool doNotDestroy;


	public void moveAlongPath(List<NavNode> para_nodes,
	                          float para_charSpeed,
	                          bool para_faceDownWhenOver,
	                          bool para_stopOneNodeShort,
	                          string para_aniPrefix)
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
			aniPrefix = "MainAvatar";//para_aniPrefix;
			if((para_aniPrefix != null)&&(para_aniPrefix != "")) { aniPrefix = para_aniPrefix; }

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

	public void faceDirection(string para_dirCode, string para_aniPrefix)
	{
		changeAnimDependingOnDirection(para_dirCode,para_aniPrefix);
		Animator aniScript = transform.gameObject.GetComponent<Animator>();
		aniScript.speed = 0;
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
			changeAnimDependingOnDirection(dirToFinalNode,aniPrefix);

		}
		else
		{
			Animator aniScript = transform.gameObject.GetComponent<Animator>();
			if(faceDownWhenOver)
			{
				aniScript.Play(aniPrefix+"_walkDown");// transform.name+"_walkDown");

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
		changeAnimDependingOnDirection(dirToNextNode,aniPrefix);



		MoveToLocation mtl = transform.gameObject.AddComponent<MoveToLocation>();
		mtl.initScript(new Vector3(nodeLoc.x,nodeLoc.y,transform.position.z),charSpeed);
		mtl.registerListener("CharMov",this);
	}

	private void changeAnimDependingOnDirection(string para_dirCode, string para_aniPrefix)
	{
		string dirToNextNode = para_dirCode;
		if(aniPrefix == null) { if(para_aniPrefix != null) {aniPrefix = para_aniPrefix; } else { aniPrefix = "MainAvatar"; }}

		//transform.eulerAngles = new Vector3(0,0,0);
		Animator aniScript = transform.gameObject.GetComponent<Animator>();
		
		if(dirToNextNode == "N")      														{  transform.eulerAngles = new Vector3(0,0,0); aniScript.Play(aniPrefix+"_walkUp"); }// transform.name+"_walkUp");   }// NORTH
		else if(dirToNextNode == "S") 														{  transform.eulerAngles = new Vector3(0,0,0); aniScript.Play(aniPrefix+"_walkDown"); }// transform.name+"_walkDown"); }// SOUTH
		else if((dirToNextNode == "W")||(dirToNextNode == "NW")||(dirToNextNode == "SW")) 	{  transform.eulerAngles = new Vector3(0,0,0); aniScript.Play(aniPrefix+"_walkLeft"); }// transform.name+"_walkLeft"); }// WEST, NW, SW
		else if((dirToNextNode == "E")||(dirToNextNode == "NE")||(dirToNextNode == "SE"))	// EAST, NE, SE
		{
			Vector3 tmpEAngles = transform.eulerAngles;
			tmpEAngles.y = 180;
			transform.eulerAngles = tmpEAngles;
			aniScript.Play(aniPrefix+"_walkLeft");//transform.name+"_walkLeft");
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
			else if(dirToNextNode == "E") { worldPivotChild.eulerAngles = new Vector3(0,0,90); }
		}
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

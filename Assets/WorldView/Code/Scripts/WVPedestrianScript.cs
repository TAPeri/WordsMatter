/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class WVPedestrianScript : MonoBehaviour, CustomActionListener
{
	PedestrianState currState;
	NewCharacterNavMovement cnm;

	//int[] mapSize;
	GridProperties gPropMapWorldBounds;
	BasicNavGraph worldNavGraph;
	ITerrainHandler terrainHandle;
	GameObject playerAvatar;
	
	float walkSpeed = 1f;
	float runSpeed = 4f;
	float currSpeed;
	
	// IDLE params.
	float idle_timeOfLastFaceSwitch;
	float idle_faceSwitchDelaySec = 3f;
	string[] faceDirections = {"N","S","W","E"};

	int faceSwitchesBeforeWalk;

	bool canWalk;
	int walkCounter = 0;

	public enum PedestrianState
	{
		IDLE,
		WALKING,
		TALKING,
		WAITING_FOR_MEETING,
		RUNNING_TO_MEETING,
		WALKING_HOME
	}

	void Start()
	{
		cnm = transform.gameObject.GetComponent<NewCharacterNavMovement>();
		if(cnm == null) { cnm = transform.gameObject.AddComponent<NewCharacterNavMovement>(); }
		cnm.doNotDestroy = true;
		cnm.registerListener("PedestrianState",this);
		if(canWalk) { setState(PedestrianState.WALKING); }
		else { setState(PedestrianState.IDLE); }
	}

	void Update()
	{
		if(currState == PedestrianState.IDLE)
		{
			if((Time.time - idle_timeOfLastFaceSwitch) >= idle_faceSwitchDelaySec)
			{
				cnm.faceDirection(faceDirections[Random.Range(0,faceDirections.Length)]);
				idle_timeOfLastFaceSwitch = Time.time;
				faceSwitchesBeforeWalk--;

				if(canWalk)
				{
					if(faceSwitchesBeforeWalk <= 0)
					{
						faceSwitchesBeforeWalk = 0;
						setState(PedestrianState.WALKING);
					}
				}
			}

			//TODO transparency

		}
	}

	public void startConversationBehaviour()
	{
		Debug.Log("Talk!");
		Vector3 playerLoc = GameObject.Find("MainAvatar").transform.position;
		cnm.faceDirection(cnm.getDirectionToForeignPoint(playerLoc));
		setState(PedestrianState.TALKING);
	}

	public void endConversationBehaviour()
	{
		Debug.Log("Fibish Talk!");

		cnm.faceDirection("S");
		setState(PedestrianState.IDLE);
	}


	public void goBackHome()
	{
		
		setState(PedestrianState.IDLE);

	}

	public void goBackHome(WorldNode para_meetingNode)
	{

		cnm.setToIdle();
		setState(PedestrianState.WALKING_HOME);
		initCharacterMovement(getCellForItem(para_meetingNode.getWorldPt()),false,false);
		//setState(PedestrianState.IDLE);
	}

	public void runToWitnessMeeting(WorldNode para_meetingNode)
	{
		setState(PedestrianState.RUNNING_TO_MEETING);
		cnm.setToIdle();
		currSpeed = runSpeed;
		initCharacterMovement(getCellForItem(para_meetingNode.getWorldPt()),2,false);
	}

	public void goToMeetingCell(WorldNode para_meetingNode)
	{
		if(canWalk)
		{
			//List<NavNode> walkPath = worldNavGraph.searchForPath(terrainHandle.getNavNodeIDForCell(getCellForItem(transform.gameObject.name)),
			//                                                     para_meetingNode.getNodeID());
			cnm.setToIdle();
			initCharacterMovement(getCellForItem(para_meetingNode.getWorldPt()),false,false);
		}

		setState(PedestrianState.WAITING_FOR_MEETING);
	}

	public void abandonMeeting()
	{
		Debug.Log("Bye meeting "+currState);
		if(currState!=PedestrianState.RUNNING_TO_MEETING){
			cnm.faceDirection("S");
			setState(PedestrianState.IDLE);
		}
	}

	private void setState(PedestrianState para_state)
	{

		if(para_state == PedestrianState.IDLE)
		{

			cnm.setToIdle();
			currSpeed = walkSpeed;
			idle_timeOfLastFaceSwitch = Time.time;
			faceSwitchesBeforeWalk = Random.Range(2,5);
		}
		else if(para_state == PedestrianState.WALKING)
		{
			int maxWalkRange = 10;
			int selectedWalkDistance = Random.Range(2,maxWalkRange);

			List<NavNode> walkDestNodes = null;
			walkDestNodes = worldNavGraph.getChildNodesAtHopDistance(worldNavGraph.getNode(terrainHandle.getNavNodeIDForCell(getCellForItem(transform.gameObject.name))),
			                                                       selectedWalkDistance,
			                                                         null,null);
			
			if((walkDestNodes == null)||(walkDestNodes.Count == 0))
			{
				// Return to idle.
				setState(PedestrianState.IDLE);
			}
			else
			{
				// Select dest node out of potential nodes.
				NavNode chosenDestNode = walkDestNodes[Random.Range(0,walkDestNodes.Count)];
				
				//List<NavNode> walkPath = worldNavGraph.searchForPath(terrainHandle.getNavNodeIDForCell(getCellForItem(transform.gameObject.name)),
				//                                                       chosenDestNode.getNodeID());

				initCharacterMovement(getCellForItem(((WorldNode) chosenDestNode).getWorldPt()),false,true);
			}

		}
		else if(para_state == PedestrianState.TALKING)
		{
			// Should have been set via the startConversationBehaviour.
		}

		currState = para_state;

	}


	public void init(int[] para_mapSize,
	                 GridProperties para_gPropMapWorldBounds,
	                 BasicNavGraph para_worldNavGraph,
	                 ITerrainHandler para_terrainHandle,
	                 bool para_canWalk)
	{
		//mapSize = para_mapSize;
		gPropMapWorldBounds = para_gPropMapWorldBounds;
		worldNavGraph = para_worldNavGraph;
		terrainHandle = para_terrainHandle;
	
		currSpeed = walkSpeed;
		canWalk = para_canWalk;
	}
	
	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(currState == PedestrianState.IDLE)
		{

		}
		else if(currState == PedestrianState.WALKING)
		{
			if(para_eventID == "PathComplete")
			{
				walkCounter++;

					setState(PedestrianState.IDLE);
			}
		}else if(currState==PedestrianState.RUNNING_TO_MEETING){

			Debug.Log("Still running"+para_eventID);
		}else if(currState==PedestrianState.WALKING_HOME){

			Debug.Log("Got home "+para_eventID);
			setState(PedestrianState.IDLE);

		}
	}


	private void initCharacterMovement(int[] para_mapCell, bool para_stopOneNodeShort, bool para_faceDownWhenOver){

		if(para_stopOneNodeShort)
			initCharacterMovement( para_mapCell, 0, para_faceDownWhenOver);
		else
			initCharacterMovement( para_mapCell, 1, para_faceDownWhenOver);

	}


	private void initCharacterMovement(int[] para_mapCell, int stopShort, bool para_faceDownWhenOver)
	{
		int[] cellForCharacter = getCellForItem(transform.gameObject.name);
		int srcNavNodeID = terrainHandle.getNavNodeIDForCell(cellForCharacter);
		int destNavNodeID = terrainHandle.getNavNodeIDForCell(para_mapCell);
		
		if(srcNavNodeID != destNavNodeID)
		{
			float[] cellCentre = gPropMapWorldBounds.getCellCentre(para_mapCell,true);
			if(cellCentre != null)
			{
				List<NavNode> pathNodes = worldNavGraph.searchForPath(srcNavNodeID,destNavNodeID);



				if((pathNodes == null)||(pathNodes.Count == 0))
				{
					Debug.Log("No Path Available");
				}
				else
				{

					if(stopShort==0)
						cnm.moveAlongPath(pathNodes,currSpeed,para_faceDownWhenOver,false);
					else if (stopShort==1)
						cnm.moveAlongPath(pathNodes,currSpeed,para_faceDownWhenOver,true);
					else{
						for(int i=0;i<stopShort-1;i++){
							if(pathNodes.Count>1)
								pathNodes.RemoveAt(pathNodes.Count-1);

						}
						cnm.moveAlongPath(pathNodes,currSpeed,para_faceDownWhenOver,true);


					}

				}
			}
		}
	}


	private int[] getCellForItem(string para_itemName)
	{
		GameObject itemGObj = GameObject.Find(para_itemName);
		
		if(itemGObj == null)
		{
			Debug.LogError("Failed to root item. Could not find '"+para_itemName+"'");
			return null;
		}
		else
		{
			int[] itemCell = gPropMapWorldBounds.hashPointToCell(new float[2] { itemGObj.transform.position.x, itemGObj.transform.position.y },true);
			return itemCell;
		}
	}
	
	private int[] getCellForItem(Vector3 para_worldPos)
	{
		int[] itemCell = gPropMapWorldBounds.hashPointToCell(new float[2] { para_worldPos.x, para_worldPos.y },true);
		return itemCell;
	}

}

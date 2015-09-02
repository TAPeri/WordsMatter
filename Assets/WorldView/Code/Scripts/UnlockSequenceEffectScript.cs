/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class UnlockSequenceEffectScript : MonoBehaviour, CustomActionListener, IActionNotifier
{
	List<int> charIDsToUnlockInWorld;
	GameObject currentTargetObj;

	GameObject endObj;
	bool isWaitingForInitialDelay = true;
	bool isWaitingForCharacterPopDelay = true;

	int[] mapSize;
	GridProperties gPropMapWorldBounds;
	ColoredNavGraph worldNavGraph;
	ITerrainHandler terrainHandle;


	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if((para_eventID == "MoveToLocation")||(para_eventID == "TeleportToLocation"))
		{
			DelayForInterval tmpDelay = transform.gameObject.AddComponent<DelayForInterval>();
			tmpDelay.registerListener("UnlockSequenceEffectScript",this);
			tmpDelay.init(1f);
		}
		else if(para_eventID == "DelayEnd")
		{
			if(isWaitingForInitialDelay)
			{
				isWaitingForInitialDelay = false;
				performNextUnlockEffectOnWorldCharacter();
			}
			else if(isWaitingForCharacterPopDelay)
			{
				isWaitingForCharacterPopDelay = false;
				performUnlockEffect();
			}
			else
			{
				charIDsToUnlockInWorld.RemoveAt(0);
				performNextUnlockEffectOnWorldCharacter();
			}
		}
	}

	public void init(List<int> para_charIDsToUnlockInWorld,
	                 int[] para_mapSize,
	                 GridProperties para_gPropMapWorldBounds,
	                 ColoredNavGraph para_worldNavGraph,
	                 ITerrainHandler para_terrHandle)
	{
		charIDsToUnlockInWorld = para_charIDsToUnlockInWorld;
		mapSize = para_mapSize;
		gPropMapWorldBounds = para_gPropMapWorldBounds;
		worldNavGraph = para_worldNavGraph;
		terrainHandle = para_terrHandle;

		performInitialDelay();
	}

	private void performNextUnlockEffectOnWorldCharacter()
	{
		if((charIDsToUnlockInWorld != null)&&(charIDsToUnlockInWorld.Count > 0))
		{
			int tmpID = charIDsToUnlockInWorld[0];
			string reqTargetName = "AvatarChar-"+tmpID;
			if(tmpID >= 9) { reqTargetName = "SecChar-"+(tmpID-9); }

			currentTargetObj = GameObject.Find(reqTargetName);
			if(currentTargetObj != null)
			{
				isWaitingForCharacterPopDelay = true;
				moveToPointOfInterest(currentTargetObj.transform.position);
			}
			else
			{
				charIDsToUnlockInWorld.RemoveAt(0);
				performNextUnlockEffectOnWorldCharacter();
			}
		}
		else
		{
			notifyAllListeners("UnlockSequenceEffectScript","AllDone",null);
			Destroy(this);
		}
	}

	private void performInitialDelay()
	{
		DelayForInterval tmpDelay = transform.gameObject.AddComponent<DelayForInterval>();
		tmpDelay.registerListener("UnlockSequenceEffectScript",this);
		tmpDelay.init(2f);
	}

	private void moveToPointOfInterest(Vector3 para_ptOfInterest)
	{
		Vector3 destPt = new Vector3(para_ptOfInterest.x,para_ptOfInterest.y,transform.position.z);

		/*MoveToLocation mtl = transform.gameObject.AddComponent<MoveToLocation>();
		mtl.registerListener("UnlockSequenceEffectScript",this);
		mtl.initScript(destPt,14f);*/

		TeleportToLocation ttl = transform.gameObject.AddComponent<TeleportToLocation>();
		ttl.registerListener("UnlockSequenceEffectScript",this);
		ttl.init(destPt);
	}

	private void performUnlockEffect()
	{
		List<SpriteRenderer> sRends = CommonUnityUtils.getSpriteRendsOfChildrenRecursively(currentTargetObj);
		if(sRends != null)
		{
			for(int i=0; i<sRends.Count; i++)
			{
				sRends[i].enabled = true;
			}
		}
		currentTargetObj.collider.enabled = true;


		int tmpID = charIDsToUnlockInWorld[0];

		// Add also a Pedestrian script (contains the pedestrian state machine).
		bool isOneOfTheMainChars = (tmpID < 9);
		bool personCanWalk = !isOneOfTheMainChars;
		Destroy(currentTargetObj.GetComponent<WVPedestrianScript>());
		WVPedestrianScript pedScript = currentTargetObj.AddComponent<WVPedestrianScript>();
		pedScript.init(mapSize,gPropMapWorldBounds,worldNavGraph,terrainHandle,personCanWalk);

		// Final delay.
		DelayForInterval tmpDelay = transform.gameObject.AddComponent<DelayForInterval>();
		tmpDelay.registerListener("UnlockSequenceEffectScript",this);
		tmpDelay.init(1f);
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

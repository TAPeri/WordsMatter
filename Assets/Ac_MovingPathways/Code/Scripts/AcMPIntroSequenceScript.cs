/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class AcMPIntroSequenceScript : MonoBehaviour, CustomActionListener, IActionNotifier
{

	// STEP 1: Place camera next to entrance.
	// STEP 2: Delay for a few seconds (to see the people entering).
	// STEP 3: Pan to the main character.

	Rect fullEncapsBounds;


	public void init(Rect para_fullEncapsBounds)
	{
		fullEncapsBounds = para_fullEncapsBounds;
		placeCameraAtEntrace();
	}


	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "CamEntranceViewAni")
		{
			panToMainCharacter();
		}
		else if(para_eventID == "PanToMainCharacterAni")
		{
			notifyAllListeners("AcMPIntroSequenceScript","IntroDone",null);
			Destroy(this);
		}
	}
	
	private void placeCameraAtEntrace()
	{
		GameObject bottomEntranceGObj = GameObject.Find("BottomEntrance");
		Rect bottomEntranceBounds = CommonUnityUtils.get2DBounds(bottomEntranceGObj.renderer.bounds);
		Vector3 pointOfInterest = new Vector3(bottomEntranceBounds.x + (bottomEntranceBounds.width/2f), bottomEntranceBounds.y - bottomEntranceBounds.height,Camera.main.transform.position.z);
		Rect camBounds = WorldSpawnHelper.getCameraViewWorldBounds(2,true);

		Rect nwBounds = CommonUnityUtils.findLookatBoundsWithinLimitedArea(pointOfInterest,camBounds,fullEncapsBounds);
		Vector3 reqCamPos = new Vector3(nwBounds.x + (nwBounds.width/2f), nwBounds.y - (nwBounds.height/2f),Camera.main.transform.position.z);

		CustomAnimationManager aniMang = Camera.main.transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("TeleportToLocation",1,new List<System.Object>() { new float[3]{reqCamPos.x,reqCamPos.y,reqCamPos.z}}));
		batch1.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>() { 1f }));
		batchLists.Add(batch1);
		aniMang.registerListener("AcMPIntro",this);
		aniMang.init("CamEntranceViewAni",batchLists);
	}

	private void panToMainCharacter()
	{
		GameObject playerAv = GameObject.Find("PlayerAvatar");

		Vector3 pointOfInterest = playerAv.transform.position;
		Rect camBounds = WorldSpawnHelper.getCameraViewWorldBounds(2,true);
		Rect nwBounds = CommonUnityUtils.findLookatBoundsWithinLimitedArea(pointOfInterest,camBounds,fullEncapsBounds);
		Vector3 reqCamPos = new Vector3(nwBounds.x + (nwBounds.width/2f), nwBounds.y - (nwBounds.height/2f),Camera.main.transform.position.z);

		CustomAnimationManager aniMang = Camera.main.transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3]{reqCamPos.x,reqCamPos.y,reqCamPos.z}, 2f, true}));
		batchLists.Add(batch1);
		aniMang.registerListener("AcMPIntro",this);
		aniMang.init("PanToMainCharacterAni",batchLists);
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

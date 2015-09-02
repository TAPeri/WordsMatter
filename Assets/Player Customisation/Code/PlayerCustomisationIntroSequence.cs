/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class PlayerCustomisationIntroSequence : MonoBehaviour, CustomActionListener, IActionNotifier
{
	// STEP 1: Make player avatar enter the scene through the left door.
	// STEP 2: Toggle visibility of base portraits to ON.
	// STEP 3: Toggle visibility of arrow sets 1-3 to ON in sequential fashion.
	// STEP 4: Make done button enter from below.

	Vector3 playerAvDestPos;
	int currArrowSetToSwitchOn = 0;

	public void init(Vector3 para_playerAvDestPos)
	{
		playerAvDestPos = para_playerAvDestPos;

		// Activate fader.
		Transform faderPrefab = Resources.Load<Transform>("Prefabs/FaderScreen");
		Transform nwFader = (Transform) Instantiate(faderPrefab,new Vector3(0,0,0),Quaternion.identity);
		nwFader.GetComponent<FaderScript>().init(Color.black,Color.clear,1f,true);

		// Make player enter.
		makePlayerEnter();
	}

	private void makePlayerEnter()
	{
		GameObject mainAv = GameObject.Find("MainAvatar");

		Animator playerAni = mainAv.GetComponent<Animator>();
		playerAni.Play("BigAVWalk");
		playerAni.speed = 1;

		Vector3 tmpEAngles = mainAv.transform.localEulerAngles;
		tmpEAngles.x = 0;
		tmpEAngles.y = 180;
		tmpEAngles.z = 0;
		mainAv.transform.localEulerAngles = tmpEAngles;


		CustomAnimationManager aniMang = mainAv.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",1,new List<System.Object>() { new float[3]{playerAvDestPos.x,playerAvDestPos.y,playerAvDestPos.z}, 3f }));
		batchLists.Add(batch1);
		aniMang.registerListener("PCIntroSeq",this);
		aniMang.init("PlayerEnter",batchLists);
	}

	private void setBasePortraitsOn()
	{
		GameObject baseAvatar0 = GameObject.Find("BaseAvatar0");
		GameObject baseAvatar1 = GameObject.Find("BaseAvatar1");

		Vector3 tmppos1 = baseAvatar0.transform.position;
		tmppos1.y -= 1000;
		baseAvatar0.transform.position = tmppos1;

		Vector3 tmppos2 = baseAvatar1.transform.position;
		tmppos2.y -= 1000;
		baseAvatar1.transform.position = tmppos2;

		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>() { 0.3f }));
		batchLists.Add(batch1);
		aniMang.registerListener("PCIntroSeq",this);
		aniMang.init("SetBasePortraitsOn",batchLists);
	}

	private void setNextArrowSetOn()
	{
		GameObject.Find("set"+currArrowSetToSwitchOn+"PrevBtn").renderer.enabled = true;
		GameObject.Find("set"+currArrowSetToSwitchOn+"NextBtn").renderer.enabled = true;

		currArrowSetToSwitchOn++;

		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>() { 0.3f }));
		batchLists.Add(batch1);
		aniMang.registerListener("PCIntroSeq",this);
		aniMang.init("SetNextArrowSetOn",batchLists);
	}

	private void makeDoneButtonEnter()
	{
		GameObject doneBtn = GameObject.Find("DoneButton");

		CustomAnimationManager aniMang = doneBtn.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",1,new List<System.Object>() { new float[3]{doneBtn.transform.position.x,
																								  doneBtn.transform.position.y + 3,
																								  doneBtn.transform.position.z}, 3f }));
		batchLists.Add(batch1);
		aniMang.registerListener("PCIntroSeq",this);
		aniMang.init("DoneButtonEnter",batchLists);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "PlayerEnter")
		{
			GameObject mainAv = GameObject.Find("MainAvatar");

			Vector3 tmpEAngles = mainAv.transform.localEulerAngles;
			tmpEAngles.x = 0;
			tmpEAngles.y = 0;
			tmpEAngles.z = 0;
			mainAv.transform.localEulerAngles = tmpEAngles;

			Animator playerAni = mainAv.GetComponent<Animator>();

			//playerAni.Play("SideWalkR",-1,0);
			playerAni.Play("AVPhotoPose_3");
			Destroy(GameObject.Find("FootstepsWood"));


			setBasePortraitsOn();
		}
		else if(para_eventID == "SetBasePortraitsOn")
		{
			//setNextArrowSetOn();
			this.respondToEvent("","SetNextArrowSetOn",null);
		}
		else if(para_eventID == "SetNextArrowSetOn")
		{


			GameObject.Find("set0PrevBtn").renderer.enabled = true;
			GameObject.Find("set0NextBtn").renderer.enabled = true;
			GameObject.Find("set1PrevBtn").renderer.enabled = true;
			GameObject.Find("set1NextBtn").renderer.enabled = true;
			GameObject.Find("set2PrevBtn").renderer.enabled = true;
			GameObject.Find("set2NextBtn").renderer.enabled = true;

			GameObject.Find("DoneButton").transform.FindChild("DoneButtonTop").renderer.enabled = true;
			GameObject.Find("DoneButton").transform.FindChild("DoneButtonShadow").renderer.enabled = true;

			//GameObject doneBtn = GameObject.Find("DoneButton");
			//doneBtn.renderer.enabled = true;
			/*Vector3 position = doneBtn.transform.position;
			position.y += 3;
			doneBtn.transform.position = position;*/

			notifyAllListeners("PCIntroSeq","IntroSequenceDone",null);
			Destroy(this);
		}
			/*if(currArrowSetToSwitchOn < 3)
			{
				setNextArrowSetOn();
			}
			else
			{
				makeDoneButtonEnter();
			}
		}
		else if(para_eventID == "DoneButtonEnter")
		{
			notifyAllListeners("PCIntroSeq","IntroSequenceDone",null);
			Destroy(this);
		}*/
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

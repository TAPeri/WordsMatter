/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class PlatformScript : MonoBehaviour, CustomActionListener, IActionNotifier
{
	bool retractedState;
	//bool extendedState;

	void Start()
	{
		retractedState = true;

		//extendedState = false;
	}

	void OnCollisionEnter(Collision collision)
	{
		if(retractedState)
		{
			if(collision.collider.name.Contains("Parcel"))
			{
				//Debug.Log("Parcel has landed!");


				Rigidbody rb = collision.collider.gameObject.GetComponent<Rigidbody>();
				if(rb != null)
				{
					rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
				}

				GameObject currItem = collision.collider.gameObject;
				currItem.transform.parent = transform;

				DelayForInterval delayScript = transform.gameObject.AddComponent<DelayForInterval>();
				delayScript.registerListener("Platform",this);
				delayScript.init(0.5f);//1.5f
			}
		}
	}

	
	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "DelayEnd")
		{
			movePlatformUp();
		}
		else if(para_eventID == "PlatformMoveUp")
		{
			//Debug.Log("Platform has moved up!");
			retractedState = false;
			//extendedState = true;

			//GameObject platformParentObj = GameObject.Find("PlatformParent");
			//platformParentObj.GetComponent<Animator>().speed = 0;

			for(int i=0; i<transform.childCount; i++)
			{
				Transform currItem = transform.GetChild(i);
				currItem.parent = null;
			}

			//movePlatformDown();
			GameObject topPiston = GameObject.Find("TopPiston");
			TopPistonScript tps = topPiston.GetComponent<TopPistonScript>();
			tps.registerListener("Platform",this);
			tps.expandPiston();
		}
		else if(para_eventID == "TopPistonExpand")
		{
			GameObject topPiston = GameObject.Find("TopPiston");
			TopPistonScript tps = topPiston.GetComponent<TopPistonScript>();
			tps.relaxPiston();
			movePlatformDown();
		}
		else if(para_eventID == "PlatformMoveDown")
		{
			//Debug.Log("Platform has moved down!");
			retractedState = true;
			//extendedState = false;
		}
	}

	public void movePlatformUp()
	{
		GameObject platformParentObj = GameObject.Find("PlatformParent");
		CustomAnimationManager caMang = platformParentObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> cmdBatchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> cmdBatch1 = new List<AniCommandPrep>();
		cmdBatch1.Add(new AniCommandPrep("TriggerAnimation",1, new List<System.Object>() { "elevate" }));
		cmdBatchList.Add(cmdBatch1);
		caMang.init("PlatformMoveUp",cmdBatchList);
		caMang.registerListener("PlatformScript",this);
	}

	public void movePlatformDown()
	{
		GameObject platformParentObj = GameObject.Find("PlatformParent");
		CustomAnimationManager caMang = platformParentObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> cmdBatchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> cmdBatch1 = new List<AniCommandPrep>();
		cmdBatch1.Add(new AniCommandPrep("TriggerAnimation",2, new List<System.Object>() { "elevate", -1f }));
		cmdBatchList.Add(cmdBatch1);
		caMang.init("PlatformMoveDown",cmdBatchList);
		caMang.registerListener("PlatformScript",this);
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}
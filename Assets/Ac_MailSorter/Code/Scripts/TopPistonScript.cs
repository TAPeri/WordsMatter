/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class TopPistonScript : MonoBehaviour, CustomActionListener, IActionNotifier
{
	void Start()
	{
		transform.GetComponent<Animator>().speed = 0;
	}

	public void expandPiston()
	{
		CustomAnimationManager caMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> cmdBatchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> cmdBatch1 = new List<AniCommandPrep>();
		cmdBatch1.Add(new AniCommandPrep("TriggerAnimation",1, new List<System.Object>() { "elevate" }));
		cmdBatchList.Add(cmdBatch1);
		caMang.init("TopPistonExpand",cmdBatchList);
		caMang.registerListener("PlatformScript",this);
	}

	public void relaxPiston()
	{
		CustomAnimationManager caMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> cmdBatchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> cmdBatch1 = new List<AniCommandPrep>();
		cmdBatch1.Add(new AniCommandPrep("TriggerAnimation",2, new List<System.Object>() { "elevate", -1f }));
		cmdBatchList.Add(cmdBatch1);
		caMang.init("TopPistonRelax",cmdBatchList);
		caMang.registerListener("PlatformScript",this);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "TopPistonExpand")
		{
			Debug.Log("Top piston has pushed");
			notifyAllListeners(transform.name,"TopPistonExpand",null);
		}
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

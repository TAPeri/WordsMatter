/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class AngryReturnScript : MonoBehaviour, CustomActionListener, IActionNotifier
{

	void Start()
	{
		// Trigger Angry Engi returning to left cliff.

		GameObject engi = GameObject.Find("Engineer");
		EngiScript engiScript = engi.GetComponent<EngiScript>();
		engiScript.registerListener("AngryReturnScript",this);
		engiScript.moveToLeftCliff();
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "EngiAtLeftCliff")
		{
			//GameObject.Find("Engineer").GetComponent<EngiScript>().unregisterListener("AngryReturnScript");
			notifyAllListeners("AngryReturnScript","AngryReturnDone",null);
			Destroy(this);
		}
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

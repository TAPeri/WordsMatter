/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class TripTimer : MonoBehaviour, IActionNotifier
{

	bool isDone;
	float fixedReqTime;
	float remainingTime;




	void Update()
	{
		if( ! isDone)
		{
			remainingTime -= Time.deltaTime;

			if(remainingTime <= 0)
			{
				remainingTime = 0;
				isDone = true;
				notifyAllListeners("TripTimer","TimerTripped",null);
				Destroy(this);
			}
		}
	}

	public void init(float para_remainingTime)
	{
		isDone = false;
		fixedReqTime = para_remainingTime;
		remainingTime = para_remainingTime;
	}

	public void interruptAndRestart()
	{
		if( ! isDone)
		{
			remainingTime = fixedReqTime;
		}
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class CircleTimerScript : MonoBehaviour, IActionNotifier
{
	Vector3 origPos;

	bool isInitialised;
	bool isPaused;
	float totalTime;
	float timerStart;

	//float pauseTimestamp;
	float elapsedTimeAtPause;


	void Start()
	{
		origPos = new Vector3(transform.parent.position.x,
		                      transform.parent.position.y,
		                      transform.parent.position.z);
		reset();
	}

	void Update()
	{
		if(isInitialised)
		{
			if(!isPaused)
			{
				float elapsedTime = Time.time - timerStart;
				if(elapsedTime > totalTime)
				{
					notifyAllListeners(transform.name,"CircleTimerFinished",null);
					isInitialised = false;
				}

				renderer.material.SetFloat("_Cutoff", Mathf.InverseLerp(0, totalTime, totalTime - elapsedTime)); 
			}
		}
	}

	public void init(float para_totTimeInSeconds)
	{
		totalTime = para_totTimeInSeconds;
		timerStart = Time.time;
		isInitialised = true;
	}

	public void setPause(bool para_pauseOn)
	{
		isPaused = para_pauseOn;
		if(isPaused)
		{
			//pauseTimestamp = Time.time;
			elapsedTimeAtPause = Time.time - timerStart;
		}
		else
		{
			// Quick Hack.
			//float timeSpentPaused = Time.time - pauseTimestamp;
			timerStart = Time.time - elapsedTimeAtPause;// timeSpentPaused ;
		}
	}

	public void reset()
	{
		totalTime = 0;
		timerStart = 0;
		isInitialised = false;
		isPaused = false;
		renderer.material.SetFloat("_Cutoff", Mathf.InverseLerp(0, 1, 1));
	}

	public void setVisibilityState(bool para_isVisible)
	{
		// Tmp.
		if(para_isVisible)
		{
			transform.parent.position = new Vector3(origPos.x,origPos.y,origPos.z);
		}
		else
		{
			Vector3 tmpPos = transform.parent.position;
			tmpPos.y += 9001;
			transform.parent.position = tmpPos;
		}
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

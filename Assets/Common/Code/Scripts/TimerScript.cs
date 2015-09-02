/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class TimerScript : MonoBehaviour, IActionNotifier
{

	Rect timerGUIBounds;
	GUIStyle timerGUIStyle;
	bool hasInitGUIStyle;


	bool isOn;
	bool isPaused;
	//float startTimeStamp;
	float durationSec;
	float secondsRemaining;
	int intValOfRemTime;


	void Start()
	{
		//isOn = false;
		//isPaused = false;
		//hasInitGUIStyle = false;
		/*timerGUIBounds = new Rect(0,0,0,0);
		startTimeStamp = Time.time;
		durationSec = 0;
		secondsRemaining = 0;
		intValOfRemTime = 0;*/
	}


	void Update()
	{
		if(isOn)
		{
			if(( ! isPaused)&&(secondsRemaining > 0))
			{
				secondsRemaining -= Time.deltaTime;
				intValOfRemTime = (int) secondsRemaining;

				if(secondsRemaining <= 0)
				{
					notifyAllListeners("TimerScript","TimeOver",null);
					secondsRemaining = 0;
					intValOfRemTime = 0;
				}
			}
		}
	}

	void OnGUI()
	{
		if( ! hasInitGUIStyle)
		{
			timerGUIStyle = new GUIStyle(GUI.skin.label);
			timerGUIStyle.alignment = TextAnchor.MiddleCenter;
			timerGUIStyle.fontSize = 30;

			hasInitGUIStyle = true;
		}
		else if(isOn)
		{
			GUI.color = Color.black;
			GUI.Label(timerGUIBounds,""+intValOfRemTime,timerGUIStyle);
			GUI.color = Color.white;
		}
	}


	public void init(Rect para_guiBounds, float para_totalDurationSec)
	{
		timerGUIBounds = para_guiBounds;
		durationSec = para_totalDurationSec;
		secondsRemaining = para_totalDurationSec;
		intValOfRemTime = (int) secondsRemaining;
	}


	public void startTimer()
	{
		isOn = true;
		secondsRemaining = durationSec;
		intValOfRemTime = (int) secondsRemaining;
		//startTimeStamp = Time.time;
	}

	public void pauseTimer(bool para_pauseState)
	{
		isPaused = para_pauseState;
	}

	public void resetTimer()
	{
		this.startTimer();
	}

	public void destroyTimer()
	{
		Destroy(this);
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

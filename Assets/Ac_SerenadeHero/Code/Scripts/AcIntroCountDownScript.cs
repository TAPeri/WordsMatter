/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class AcIntroCountDownScript : MonoBehaviour, IActionNotifier
{
	int countDownValue;
	float timePerInteval;
	float timeOfLastInterval;

	bool hasPreparedGUIStyles = false;
	Rect countDownBounds;
	GUIStyle countDownGUIStyle = null;

	//bool playSound;

	Transform sfxPrefab;


	void Update()
	{
		if((Time.time - timeOfLastInterval) >= timePerInteval)
		{
			if(countDownValue == 1)
			{
				notifyAllListeners("IntroCountdown","CountdownEnd",null);
				Destroy(this);
			}
			else
			{
				countDownValue--;
				timeOfLastInterval = Time.time;
			}
		}
	}

	void OnGUI()
	{
		if( ! hasPreparedGUIStyles)
		{
			float targetWidth = 1280f;
			float targetHeight = 800f;
			Vector3 scaleForCurrRes = new Vector3((Screen.width * 1.0f)/targetWidth,(Screen.height * 1.0f)/targetHeight,1f);

			countDownGUIStyle = new GUIStyle(GUI.skin.label);
			countDownGUIStyle.alignment = TextAnchor.MiddleCenter;
			countDownGUIStyle.fontSize = (int) (300 * scaleForCurrRes.x);
			hasPreparedGUIStyles = true;
		}
		else
		{
			GUI.color = Color.black;
			GUI.Label(countDownBounds,""+countDownValue,countDownGUIStyle);
			GUI.color = Color.white;
		}
	}

	public void init(int para_countDownMax, float para_timePerInterval, bool para_playSound)
	{
		countDownValue = para_countDownMax;
		timePerInteval = para_timePerInterval;
		timeOfLastInterval = Time.time;
		//playSound = para_playSound;

		float boundsWidth = Screen.width * 0.7f;
		float boundsHeight = boundsWidth;
		countDownBounds = new Rect((Screen.width/2f) - (boundsWidth/2f),(Screen.height/2f) - (boundsHeight/2f),boundsWidth,boundsHeight);

		sfxPrefab = Resources.Load<Transform>("Prefabs/SFxBox");
	}

	private void triggerSoundAtCamera(string para_soundFileName)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		DontDestroyOnLoad(nwSFX);
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = 0.5f;
		audS.Play();
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ActivitySuccessWindow : ILearnRWUIElement
{

	string windowName = "ActivitySuccessWindow";
	Transform sfxPrefab = null;
	new bool hasInitGUIStyles = false;


	void Start()
	{
		// Switch on dimer object.
		GameObject dimScreen = transform.FindChild("DimScreen").gameObject;
		dimScreen.renderer.enabled = true;
		dimScreen.renderer.material.color = Color.black;
		
		// Init text items.
		string[] elementNames   = {"OkButton"};
		string[] elementContent = {LocalisationMang.translate("Success!")};
		bool[] destroyGuideArr = {false};
		int[] textElementTypeArr = {0};
		
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);
	}

	void OnGUI()
	{
		if( ! hasInitGUIStyles)
		{
			prepGUIStyles();
			hasInitGUIStyles = true;
		}
		else
		{
			GUI.color = Color.black;
			GUI.Label(uiBounds["OkButton"],textContent["OkButton"],availableGUIStyles["FieldTitle"]);
			GUI.color = Color.clear;
			if(GUI.Button(uiBounds["OkButton"],""))
			{
				notifyAllListeners(windowName,"OK",null);
				Destroy(transform.gameObject);
			}
			GUI.color = Color.white;
		}
	}
	
	public void init(string para_text)
	{
		triggerSoundAtCamera("SFX_SuccessScreen");
	}
	
	protected new void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string, GUIStyle>();
		
		float targetWidth = 1280f;
		float targetHeight = 800f;
		Vector3 scaleForCurrRes = new Vector3((Screen.width * 1.0f)/targetWidth,(Screen.height * 1.0f)/targetHeight,1f);
		
		GUIStyle fieldTitleStyle = new GUIStyle(GUI.skin.label);
		fieldTitleStyle.alignment = TextAnchor.MiddleCenter;
		fieldTitleStyle.fontSize = (int) (40 * scaleForCurrRes.x);
		
		GUIStyle fieldContentStyle = new GUIStyle(GUI.skin.label);
		fieldContentStyle.alignment = TextAnchor.MiddleCenter;
		fieldContentStyle.fontSize = (int) (30 * scaleForCurrRes.x);
		
		GUIStyle slideTextStyle = new GUIStyle(GUI.skin.label);
		slideTextStyle.alignment = TextAnchor.MiddleLeft;
		slideTextStyle.fontSize = (int) (30 * scaleForCurrRes.x);
		
		GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
		btnStyle.wordWrap = true;

		availableGUIStyles.Add("FieldTitle",fieldTitleStyle);
		availableGUIStyles.Add("FieldContent",fieldContentStyle);
		availableGUIStyles.Add("SlideText",slideTextStyle);
		availableGUIStyles.Add("Button",btnStyle);
		hasInitGUIStyles = true;
	}

	public void triggerSoundAtCamera(string para_soundFileName)
	{
		GameObject camGObj = Camera.main.gameObject;

		if(sfxPrefab == null) { sfxPrefab = Resources.Load<Transform>("Prefabs/SFxBox"); }

		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = 1f;
		audS.Play();
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}
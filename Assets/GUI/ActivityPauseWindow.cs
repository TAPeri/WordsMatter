/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ActivityPauseWindow : ILearnRWUIElement, IActionNotifier
{


	void Start()
	{		
		triggerSoundAtCamera("BubbleClick");

		// Correct render order.
		//fixRenderSortingForElements(new List<string[]>() { (new string[]{"DimScreen"}),(new string[]{"ContentPane"})});
		
		// Switch on dimer object.
		transform.FindChild("DimScreen").renderer.enabled = true;
		
		// Init text items.
		string[] elementNames   = {"Title","ReturnToActivity","Tutorial","Settings","ExitActivity","ReturnToActivity_Label","Tutorial_Label","Settings_Label","ExitActivity_Label"};
		string[] elementContent = {
			LocalisationMang.translate("Activity paused"),
			LocalisationMang.translate("Return to activity"),
			LocalisationMang.translate("Tutorial"),
			LocalisationMang.translate("Settings"),
			LocalisationMang.translate("Exit activity"),
			LocalisationMang.translate("Return to activity"),
			LocalisationMang.translate("Tutorial"),
			LocalisationMang.translate("Settings"),
			LocalisationMang.translate("Exit activity")
		};
		bool[] destroyGuideArr = {true,true,true,true,true,true,true,true,true};
		int[] textElementTypeArr = {0,0,0,0,0,0,0,0,0};
		
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);
	}

	Transform sfxPrefab;

	void OnGUI()
	{
		if( ! hasInitGUIStyles)
		{
			this.prepGUIStyles();
			hasInitGUIStyles = true;
		}
		else
		{
			GUI.color = Color.black;
			GUI.Label(uiBounds["Title"],textContent["Title"],availableGUIStyles["Title"]);
			GUI.Label(uiBounds["ReturnToActivity_Label"],textContent["ReturnToActivity_Label"],availableGUIStyles["FieldContent"]);
			GUI.Label(uiBounds["Tutorial_Label"],textContent["Tutorial_Label"],availableGUIStyles["FieldContent"]);
			GUI.Label(uiBounds["Settings_Label"],textContent["Settings_Label"],availableGUIStyles["FieldContent"]);
			GUI.Label(uiBounds["ExitActivity_Label"],textContent["ExitActivity_Label"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.white;

			GUI.color = Color.clear;
			GUI.skin.button.wordWrap = true;
			if(GUI.Button(uiBounds["ReturnToActivity"],textContent["ReturnToActivity"],availableGUIStyles["Button"]))
			{
				triggerSoundAtCamera("BubbleClick");

				notifyAllListeners("ActivityPauseWindow","ReturnToActivity",null);
			}

			if(GUI.Button(uiBounds["Tutorial"],textContent["Tutorial"],availableGUIStyles["Button"]))
			{
				triggerSoundAtCamera("BubbleClick");
				notifyAllListeners("ActivityPauseWindow","Tutorial",null);
			}

			if(GUI.Button(uiBounds["Settings"],textContent["Settings"],availableGUIStyles["Button"]))
			{

			}

			if(GUI.Button(uiBounds["ExitActivity"],textContent["ExitActivity"],availableGUIStyles["Button"]))
			{
				triggerSoundAtCamera("BubbleClick");

				notifyAllListeners("ActivityPauseWindow","ExitActivity",null);
			}
		}
	}


	protected new void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string, GUIStyle>();

		float targetWidth = 1280f;
		float targetHeight = 800f;
		Vector3 scaleForCurrRes = new Vector3((Screen.width * 1.0f)/targetWidth,(Screen.height * 1.0f)/targetHeight,1f);

		GUIStyle fieldTitleStyle = new GUIStyle(GUI.skin.label);
		fieldTitleStyle.alignment = TextAnchor.MiddleCenter;
		fieldTitleStyle.fontSize = (int) (50 * scaleForCurrRes.x);
		
		GUIStyle fieldContentStyle = new GUIStyle(GUI.skin.label);
		fieldContentStyle.alignment = TextAnchor.MiddleCenter;
		fieldContentStyle.fontSize = (int) (30 * scaleForCurrRes.x);

		GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
		btnStyle.wordWrap = true;
		btnStyle.normal.textColor = Color.black;

		availableGUIStyles.Add("Title",fieldTitleStyle);
		availableGUIStyles.Add("FieldContent",fieldContentStyle);
		availableGUIStyles.Add("Button",btnStyle);
		hasInitGUIStyles = true;
	}


	private void triggerSoundAtCamera(string para_soundFileName)
	{
		triggerSoundAtCamera(para_soundFileName,1f,false);
	}
	
	private void triggerSoundAtCamera(string para_soundFileName, float para_volume, bool para_loop)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		GameObject potentialOldSfx = GameObject.Find(para_soundFileName);
		if(potentialOldSfx != null) { Destroy(potentialOldSfx); }
		
		if(sfxPrefab == null) { sfxPrefab = Resources.Load<Transform>("Prefabs/SFxBox"); }
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		nwSFX.name = para_soundFileName;
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = para_volume;
		audS.loop = para_loop;
		if(para_loop) { Destroy(nwSFX.GetComponent<DestroyAfterTime>()); }
		audS.Play();
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class GamePauseWindow : ILearnRWUIElement, IActionNotifier
{
	
	
	void Start()
	{		
		// Correct render order.
		//fixRenderSortingForElements(new List<string[]>() { (new string[]{"DimScreen"}),(new string[]{"ContentPane"})});
		
		// Switch on dimer object.
		transform.FindChild("DimScreen").renderer.enabled = true;
		
		// Init text items.
		string[] elementNames   = {"ReturnToGame","Story","ExitGame","ReturnToGame_Label","Story_Label","ExitGame_Label"};
		string[] elementContent = {
			LocalisationMang.translate("Return to game"),
			LocalisationMang.translate("Story"),
			LocalisationMang.translate("Exit game"),
			LocalisationMang.translate("Return to game"),
			LocalisationMang.translate("Story"),
			LocalisationMang.translate("Exit game")
		};
		bool[] destroyGuideArr = {true,true,true,true,true,true};
		int[] textElementTypeArr = {0,0,0,0,0,0};
		
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);
	}
	
	
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
			GUI.Label(uiBounds["ReturnToGame_Label"],textContent["ReturnToGame_Label"],availableGUIStyles["FieldContent"]);
			GUI.Label(uiBounds["Story_Label"],textContent["Story_Label"],availableGUIStyles["FieldContent"]);
			GUI.Label(uiBounds["ExitGame_Label"],textContent["ExitGame_Label"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.white;
			
			GUI.color = Color.clear;
			GUI.skin.button.wordWrap = true;
			if(GUI.Button(uiBounds["ReturnToGame"],textContent["ReturnToGame"],availableGUIStyles["Button"]))
			{
				notifyAllListeners("GamePauseWindow","ReturnToGame",null);
			}
			
			if(GUI.Button(uiBounds["Story"],textContent["Story"],availableGUIStyles["Button"]))
			{
				notifyAllListeners("GamePauseWindow","Story",null);
			}
			
			if(GUI.Button(uiBounds["ExitGame"],textContent["ExitGame"],availableGUIStyles["Button"]))
			{
				notifyAllListeners("GamePauseWindow","ExitGame",null);

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
	
	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

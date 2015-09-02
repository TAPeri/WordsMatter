/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class MessageWindow : ILearnRWUIElement
{
	
	string windowName = "MessageWindow";
	new bool hasInitGUIStyles = false;

	string titleText;
	string messageText;

	Vector2 messageScrollPos;
	Vector2 tmpVect;
	
	
	void Start()
	{
		messageScrollPos = new Vector2();
		tmpVect = new Vector2();
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
			GUI.Label(uiBounds["TitleTA"],textContent["TitleTA"],availableGUIStyles["FieldTitle"]);


			if((Application.platform == RuntimePlatform.Android)
			   ||(Application.platform == RuntimePlatform.IPhonePlayer))
			{
				if(Input.touchCount == 1)
				{
					tmpVect.x = Input.touches[0].position.x;
					tmpVect.y = Screen.height - Input.touches[0].position.y;
					
					if(uiBounds["MessageTA"].Contains(tmpVect))
					{
						messageScrollPos.y += (Input.touches[0].deltaPosition.y * 1f);
					}
				}
			}


			GUILayout.BeginArea(uiBounds["MessageTA"]);
			messageScrollPos = GUILayout.BeginScrollView(messageScrollPos);
			GUILayout.BeginVertical();
			GUILayout.Label(textContent["MessageTA"],availableGUIStyles["MessageText"]);
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.EndArea();

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
	
	public void init(string para_title, string para_message)
	{

		titleText = para_title;
		messageText = para_message;
		if(titleText == null) { titleText = "Missing Title"; }
		if(messageText == null) { messageText = "Missing Message"; }

		// Switch on dimer object.
		GameObject dimScreen = transform.FindChild("DimScreen").gameObject;
		dimScreen.renderer.enabled = true;
		dimScreen.renderer.material.color = Color.black;
		
		// Init text items.
		string[] elementNames   = {"TitleTA","MessageTA","OkButton"};
		string[] elementContent = {titleText,messageText,"OK"};
		bool[] destroyGuideArr = {true,true,false};
		int[] textElementTypeArr = {0,0,0};
		
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);
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
		
		GUIStyle messageTextStyle = new GUIStyle(GUI.skin.label);
		messageTextStyle.alignment = TextAnchor.MiddleLeft;
		messageTextStyle.fontSize = (int) (35 * scaleForCurrRes.x);
		
		GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
		btnStyle.wordWrap = true;

		availableGUIStyles.Add("FieldTitle",fieldTitleStyle);
		availableGUIStyles.Add("FieldContent",fieldContentStyle);
		availableGUIStyles.Add("MessageText",messageTextStyle);
		availableGUIStyles.Add("Button",btnStyle);
		hasInitGUIStyles = true;
	}
	

	
	
	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}
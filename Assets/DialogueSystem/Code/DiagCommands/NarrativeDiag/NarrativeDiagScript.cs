/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class NarrativeDiagScript : ILearnRWUIElement, CustomActionListener, IActionNotifier
{

	NarrativeDiagCommand commandData;
	Vector2 scrollPos;
	Vector2 tmpVect;

	TextAudioManager taMang;

	GameObject currentBubble; 



	bool initialised = false;

	void OnGUI()
	{
		if( ! hasInitGUIStyles)
		{
			prepGUIStyles();
			hasInitGUIStyles = true;
		}
		else if(initialised)
		{

			Rect narrativeGUIArea = uiBounds["TextArea"];

			if((Application.platform == RuntimePlatform.Android)
			   ||(Application.platform == RuntimePlatform.IPhonePlayer))
			{
				if(Input.touchCount == 1)
				{
					tmpVect.x = Input.touches[0].position.x;
					tmpVect.y = Screen.height - Input.touches[0].position.y;
					
					if(narrativeGUIArea.Contains(tmpVect))
					{
						scrollPos.y += (Input.touches[0].deltaPosition.y * 1f);
					}
				}
			}

			//GUI.Box(uiBounds["TextArea"],"");


			GUILayout.BeginArea(narrativeGUIArea);
			scrollPos = GUILayout.BeginScrollView(scrollPos,GUILayout.Width(narrativeGUIArea.width),GUILayout.Height(narrativeGUIArea.height));
			GUI.color = Color.black;
			GUILayout.Label(textContent["TextArea"],availableGUIStyles["NarrativeFieldContent"]);
			GUILayout.EndScrollView();
			GUILayout.EndArea();


			/*GUI.color = Color.white;
			scrollPos = GUI.BeginScrollView(narrativeGUIArea,scrollPos,new Rect(0,0,narrativeGUIArea.width + 100,narrativeGUIArea.height + 100),true,true);
			GUI.color = Color.black;
			GUI.Label(new Rect(0,0,narrativeGUIArea.width,narrativeGUIArea.height),textContent["TextArea"],availableGUIStyles["NarrativeFieldContent"]);
			GUI.EndScrollView();*/







			GUI.color = Color.white;
			
		}
	}
	//bool messageOne = false;

	protected new void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string, GUIStyle>();

		float targetWidth = 1280f;
		float targetHeight = 800f;
		Vector3 scaleForCurrRes = new Vector3((Screen.width * 1.0f)/targetWidth,(Screen.height * 1.0f)/targetHeight,1f);

		GUIStyle narrativeFieldStyle = new GUIStyle(GUI.skin.label);
		narrativeFieldStyle.alignment = TextAnchor.MiddleLeft;
		narrativeFieldStyle.fontSize = (int) (30 * scaleForCurrRes.x);

		GUIStyle fieldTitleStyle = new GUIStyle(GUI.skin.label);
		fieldTitleStyle.alignment = TextAnchor.MiddleCenter;
		fieldTitleStyle.fontSize = (int) (17 * scaleForCurrRes.x);
		
		GUIStyle fieldContentStyle = new GUIStyle(GUI.skin.label);
		fieldContentStyle.alignment = TextAnchor.MiddleCenter;
		fieldContentStyle.fontSize = (int) (17 * scaleForCurrRes.x);
		
		GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
		btnStyle.wordWrap = true;
		btnStyle.normal.textColor = Color.black;

		availableGUIStyles.Add("NarrativeFieldContent",narrativeFieldStyle);
		availableGUIStyles.Add("FieldTitle",fieldTitleStyle);
		availableGUIStyles.Add("FieldContent",fieldContentStyle);
		availableGUIStyles.Add("Button",btnStyle);
		hasInitGUIStyles = true;
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "NarrativeOkToContinue")
		{
			notifyAllListeners("NarrativeDiagScript","BubbleCreated",currentBubble);
		}
		else if(para_eventID == "DelayEnd")
		{

		}
		else if(para_sourceID == "TextAudioManager")
		{
			if(para_eventID == "RequestCompleted")
			{
				
				if(taMang != null)
				{
					taMang.unregisterListener("Narrative");
				}

				respondToEvent("NarrativeDiagScript","NarrativeOkToContinue",null);
			}
		}
	}

	public void init(NarrativeDiagCommand para_commandData)
	{

		commandData = para_commandData;
		tmpVect = new Vector2();
		scrollPos = new Vector2();

		Transform narrativeSpeechBubbleRight = transform.FindChild("SquareText");
		currentBubble = ((Transform) Instantiate(narrativeSpeechBubbleRight,narrativeSpeechBubbleRight.position,narrativeSpeechBubbleRight.rotation)).gameObject;
		currentBubble.name = "CurrentText";
		currentBubble.transform.parent = transform;
		currentBubble.SetActive(true);

		// Init and register items.
		string[] elementNames = new string[]{"TextArea"};
		string[] elementContent = new string[]{commandData.getNarrativeText()};
		bool[] destroyGuideArr = new bool[]{true};
		int[] textElementTypeArr = new int[]{0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,"CurrentText");


		taMang = GameObject.Find("DiagBar").GetComponent<TextAudioManager>();
		taMang.playVoiceOver(para_commandData.getVoiceOverSearchKey(),"Narrative",this);
		initialised = true;
	}



	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

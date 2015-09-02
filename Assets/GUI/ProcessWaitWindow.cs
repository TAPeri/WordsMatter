/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ProcessWaitWindow : ILearnRWUIElement, CustomActionListener, IActionNotifier
{
	//bool okBtnVisible;
	bool preppedOnScreen = false;

	Vector2 tmpVect;
	Vector2 messageScrollPos;

	List<GetRoutine> tmpWaitList;
	List<string> textElements;
	List<string> statusElements;


	void OnStart()
	{
		tmpVect = new Vector2();
		messageScrollPos = new Vector2();
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
			if(preppedOnScreen)
			{
				GUI.color = Color.black;
				GUI.Label(uiBounds["Title"],textContent["Title"],availableGUIStyles["NoticeContent"]);
				GUI.color = Color.white;

				if((Application.platform == RuntimePlatform.Android)
				   ||(Application.platform == RuntimePlatform.IPhonePlayer))
				{
					if(Input.touchCount == 1)
					{
						tmpVect.x = Input.touches[0].position.x;
						tmpVect.y = Screen.height - Input.touches[0].position.y;
						
						if(uiBounds["TextArea"].Contains(tmpVect))
						{
							messageScrollPos.y += (Input.touches[0].deltaPosition.y * 1f);
						}
					}
				}


				GUILayout.BeginArea(uiBounds["TextArea"]);
				messageScrollPos = GUILayout.BeginScrollView(messageScrollPos);
				GUILayout.BeginVertical();
				for(int i=0; i<textElements.Count; i++)
				{
					GUILayout.BeginHorizontal();
					GUI.color = Color.black;
					GUILayout.Label(statusElements[i],availableGUIStyles["ScrollLabel"]);
					GUILayout.Label(textElements[i],availableGUIStyles["ScrollLabel"]);
					GUI.color = Color.white;
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
				GUILayout.EndScrollView();
				GUILayout.EndArea();


				GUI.color = Color.white;
			}
		}
	}

	protected new void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string, GUIStyle>();
		
		float targetWidth = 1280f;
		float targetHeight = 800f;
		Vector3 scaleForCurrRes = new Vector3((Screen.width * 1.0f)/targetWidth,(Screen.height * 1.0f)/targetHeight,1f);
		
		GUIStyle noticeContentStyle = new GUIStyle(GUI.skin.label);
		noticeContentStyle.alignment = TextAnchor.MiddleCenter;
		noticeContentStyle.fontSize = (int) (30 * scaleForCurrRes.x);

		GUIStyle scrollTextStyle = new GUIStyle(GUI.skin.label);
		scrollTextStyle.alignment = TextAnchor.UpperLeft;
		scrollTextStyle.fontSize = (int) (28 * scaleForCurrRes.x);

		GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
		btnStyle.wordWrap = true;
		btnStyle.fontSize = (int) (16 * scaleForCurrRes.x);
		btnStyle.normal.textColor = Color.black;
		
		availableGUIStyles.Add("NoticeContent",noticeContentStyle);
		availableGUIStyles.Add("ScrollLabel",scrollTextStyle);
		availableGUIStyles.Add("Button",btnStyle);
		hasInitGUIStyles = true;
	}

	public void init(List<GetRoutine> para_waitList, List<string> para_readableTaskTextList)
	{
		//okBtnVisible = false;
		preppedOnScreen = false;
		transform.position = new Vector3(transform.position.x + 9000,transform.position.y,transform.position.z);

		tmpWaitList = para_waitList;

		textElements = para_readableTaskTextList;
		statusElements = new List<string>();
		for(int i=0; i<textElements.Count; i++)
		{
			statusElements.Add("");
		}
		if(statusElements.Count > 0)
		{
			statusElements[0] = "[...]";
		}
	}

	public void performPrep()
	{
		transform.position = new Vector3(transform.position.x - 9000,transform.position.y,transform.position.z);

		// Switch on dimer object.
		GameObject dimScreen = transform.FindChild("DimScreen").gameObject;
		dimScreen.renderer.enabled = true;
		dimScreen.renderer.material.color = Color.black;

		// Init text items.
		string[] elementNames   = {"Title","TextArea"};
		string[] elementContent = {"",""};
		bool[] destroyGuideArr = {true,false};
		int[] textElementTypeArr = {0,0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);

		WaiterScript wS = transform.gameObject.AddComponent<WaiterScript>();
		wS.registerListener("ProcessWaitWindow",this);
		wS.init(tmpWaitList);

		preppedOnScreen = true;
	}

	public void changeMessage(string para_msg)
	{
		textContent["OKBtn"] = para_msg;
	}

	public void showOKButton()
	{
		//okBtnVisible = true;
		Transform okBtnActual = transform.FindChild("OkBtnActual");
		if(okBtnActual != null)
		{
			okBtnActual.gameObject.SetActive(true);
		}
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_sourceID == "WaiterScript")
		{
			if(para_eventID == "SingleTaskDone")
			{
				List<System.Object> retData = (List<System.Object>) para_eventData;
				int taskID = (int) retData[0];
				bool taskSuccessFlag = (bool) retData[1];

				string nwStatString = "[?]";
				if(taskSuccessFlag == true) { nwStatString = LocalisationMang.translate("[Ok]"); } else { nwStatString = LocalisationMang.translate("[Failed]"); }
				statusElements[taskID] = nwStatString;
			}
			else if(para_eventID == "AllDone")
			{
				triggerDelayNClose();
			}
		}
		else if(para_eventID == "DelayEnd")
		{
			notifyAllListeners("ProcessWaitWindow","Close",null);
			Destroy(this);
		}
	}

	private void triggerDelayNClose()
	{
		DelayForInterval dfi = transform.gameObject.AddComponent<DelayForInterval>();
		dfi.registerListener("ProcessWaitWindow",this);
		dfi.init(0.1f);
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

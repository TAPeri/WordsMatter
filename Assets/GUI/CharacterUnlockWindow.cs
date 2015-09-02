/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class CharacterUnlockWindow : ILearnRWUIElement, CustomActionListener, IActionNotifier
{
	//int unlockedCharID;
	//string unlockedCharName;

	bool preped = false;

	void Start()
	{
		DelayForInterval dfi = transform.gameObject.AddComponent<DelayForInterval>();
		dfi.registerListener("TMpWind",this);
		dfi.init(1);
	}

	void OnGUI()
	{
		if(preped)
		{
			if( ! hasInitGUIStyles)
			{
				prepGUIStyles();
				hasInitGUIStyles = true;
			}
			else
			{
				GUI.color = Color.black;
				GUI.Label(uiBounds["TextArea"],textContent["TextArea"],availableGUIStyles["NoticeContent"]);
				GUI.color = Color.white;
				if(GUI.Button(uiBounds["OkBtn"],textContent["OkBtn"],availableGUIStyles["Button"]))
				{
					notifyAllListeners("CharacterUnlockWindow","Close",null);
					Destroy(transform.gameObject);
				}
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
		noticeContentStyle.alignment = TextAnchor.MiddleLeft;
		noticeContentStyle.fontSize = (int) (30 * scaleForCurrRes.x);
		
		GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
		btnStyle.wordWrap = true;
		btnStyle.fontSize = (int) (16 * scaleForCurrRes.x);
		btnStyle.normal.textColor = Color.black;
		
		availableGUIStyles.Add("NoticeContent",noticeContentStyle);
		availableGUIStyles.Add("Button",btnStyle);
		hasInitGUIStyles = true;
	}

	public void init(int para_unlockedCharID, string para_unlockedCharName)
	{
		//unlockedCharID = para_unlockedCharID;
		//unlockedCharName = para_unlockedCharName;

		// Apply portrait.
		Transform dummyPortrait = transform.FindChild("Portrait");
		PortraitHelper.replaceEntireDummyPortrait(dummyPortrait.gameObject,para_unlockedCharID,0,para_unlockedCharName,0.04f);
		CommonUnityUtils.setSortingLayerOfEntireObject(dummyPortrait.gameObject,"SpriteGUI");
		CommonUnityUtils.setSortingOrderOfEntireObject(dummyPortrait.gameObject,6000);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "DelayEnd")
		{
			Vector3 parentPos = transform.parent.position;
			Vector3 tmpPos = transform.position;
			tmpPos.x = parentPos.x;
			tmpPos.y = parentPos.y;
			transform.position = tmpPos;
			applyPrep();
		}
	}

	private void applyPrep()
	{
		// Correct render order.
		fixRenderSortingForElements(new List<string[]>() { (new string[]{"DimScreen"}),
			(new string[]{"Frame"}),
			(new string[]{"ContentPane"}),
			(new string[]{"Portrait","TABack","TextArea","OkBtn"})});
		

		// Init text items.
		string[] elementNames = new string[]{"TextArea","OkBtn"};
		string[] elementContent = new string[]{LocalisationMang.getString("GhostB*CharacterUnlock"),LocalisationMang.translate("Ok")};
		bool[] destroyGuideArr = new bool[]{true,false};
		int[] textElementTypeArr = new int[]{0,0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);

		preped = true;
	}
	
	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

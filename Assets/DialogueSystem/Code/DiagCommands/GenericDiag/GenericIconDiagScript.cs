/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class GenericIconDiagScript : ILearnRWScenario, CustomActionListener, IActionNotifier
{

	GameObject currentBubble;
	List<string> iconNames;
	bool guiActive = false;

	GenericIconDiagCommand commandData;
	List<string> pressableIconNames;

	Color semiTransparentColor = new Color(1,1,1,0.6f);

	void OnGUI()
	{
		if(guiActive)
		{
			if(pressableIconNames != null)
			{
				GUI.color = semiTransparentColor;

				for(int i=0; i<pressableIconNames.Count; i++)
				{
					GUI.color = Color.clear;
					if(GUI.Button(uiBounds[pressableIconNames[i]],""))
					{
						notifyAllListeners(commandData.getCommandName(),"ButtonPressed-"+i,i);
						Destroy(this);
					}
					GUI.color = Color.white;
				}

				GUI.color = Color.white;
			}
		}
	}


	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if((para_sourceID == "DiagElementSequenceScript")&&(para_eventID == "BubbleCreated"))
		{
			checkEndCondition();
		}
	}


	public GameObject init(GenericIconDiagCommand para_buildCommand)
	{
		commandData = para_buildCommand;

		string bubbleGuideName = "";
		int speakerIndex = para_buildCommand.getSpeakerIndex();
		if(speakerIndex == 0) { bubbleGuideName = "SpeechBubbleLeft"; } else { bubbleGuideName = "SpeechBubbleRight"; }

		Transform bubbleGuide = transform.FindChild(bubbleGuideName);
		currentBubble = ((Transform) Instantiate(bubbleGuide,bubbleGuide.position,bubbleGuide.rotation)).gameObject;

		GameObject workAreaGuide = currentBubble.transform.FindChild("TextArea").gameObject;

		Transform[] iconPrefabs = para_buildCommand.getIconPrefabs();


		Rect workArea2Dbounds = CommonUnityUtils.get2DBounds(workAreaGuide.renderer.bounds);
	

		float cellWidth = (workArea2Dbounds.width/(iconPrefabs.Length * 1f));
		float cellHeight = cellWidth;
		float selectedPadding = 0;
		if(cellWidth >= workArea2Dbounds.height)
		{
			cellWidth = workArea2Dbounds.height;
			cellHeight = workArea2Dbounds.height;

			float remWidth = cellWidth * (iconPrefabs.Length * 1f);
			float regularSinglePadding = workArea2Dbounds.width * 0.05f;
			float regularTotalPadding = (regularSinglePadding * ((iconPrefabs.Length-1) * 1f));
			selectedPadding = regularSinglePadding;
			if(regularTotalPadding > remWidth) { selectedPadding = (remWidth / ((iconPrefabs.Length-1) * 1f)); }
		}
		else
		{
			cellHeight = cellWidth;
			selectedPadding = 0;
		}


		float totSpaceRequired = (cellWidth * (iconPrefabs.Length * 1f)) + (selectedPadding * ((iconPrefabs.Length-1) * 1f));
		
		float tmp_X = workArea2Dbounds.x + (workArea2Dbounds.width/2f) - (totSpaceRequired/2f);
		float tmp_Y = workArea2Dbounds.y;

		List<GameObject> createdIcons = new List<GameObject>();
		iconNames = new List<string>();
		uiBounds = new Dictionary<string, Rect>();
		bool[] upAxisArr = new bool[]{false,true,false};
		for(int i=0; i<iconPrefabs.Length; i++)
		{
			string nwIconName = "Icon-"+i;
			iconNames.Add(nwIconName);
			Rect nwIconWorldBounds = new Rect(tmp_X,tmp_Y,cellWidth,cellHeight);
			uiBounds.Add(nwIconName,WorldSpawnHelper.getWorldToGUIBounds(nwIconWorldBounds,1,upAxisArr));

			GameObject nwIcon = WorldSpawnHelper.initObjWithinWorldBounds(iconPrefabs[i],nwIconName,nwIconWorldBounds,workAreaGuide.transform.position.z,new bool[]{false,true,false});
			nwIcon.renderer.sortingOrder = 150;
			nwIcon.transform.parent = currentBubble.transform;
			nwIcon.SetActive(false);
			createdIcons.Add(nwIcon);

			tmp_X += (cellWidth + selectedPadding);
		}


		if(para_buildCommand.needsToPerformPopInSequence())
		{
			DiagElementSequenceScript dess = currentBubble.AddComponent<DiagElementSequenceScript>();
			dess.registerListener("GenericIconDiagScript",this);
			dess.init(currentBubble,createdIcons);
		}
		else
		{
			for(int i=0; i<createdIcons.Count; i++)
			{
				createdIcons[i].SetActive(true);
			}
			guiActive = true;
		}

		bool[] pressableIconFlagArr = commandData.getPressableIconsFlagArr();
		if(pressableIconFlagArr != null)
		{
			pressableIconNames = new List<string>();
			for(int i=0; i<pressableIconFlagArr.Length; i++)
			{
				if(pressableIconFlagArr[i])
				{
					pressableIconNames.Add("Icon-"+i);
				}
			}
		}

		Destroy(workAreaGuide);
		currentBubble.SetActive(true);

		return currentBubble;
	}

	private void checkEndCondition()
	{
		if(commandData.getPressableIconsFlagArr() == null)
		{
			notifyAllListeners(commandData.getCommandName(),"BubbleCreated",currentBubble);
			Destroy(this);
		}
		else
		{
			guiActive = true;
		}
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

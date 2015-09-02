/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class DiagElementSequenceScript : MonoBehaviour, CustomActionListener, IActionNotifier
{
	GameObject mainParentObj;
	List<GameObject> sequenceObjects;
	int counter;

	List<Vector3> destScales;

	float endSequenceDelay_Sec = 2f;


	public void init(GameObject para_mainParent, List<GameObject> para_sequenceObjects)
	{
		mainParentObj = para_mainParent;
		sequenceObjects = para_sequenceObjects;
		destScales = new List<Vector3>();
		counter = 0;


		for(int i=0; i<sequenceObjects.Count; i++)
		{
			destScales.Add(new Vector3(sequenceObjects[i].transform.localScale.x,
			                           sequenceObjects[i].transform.localScale.y,
			                           sequenceObjects[i].transform.localScale.z));
			sequenceObjects[i].transform.localScale = new Vector3(0.01f,0.01f,0.01f);
			sequenceObjects[i].SetActive(false);
		}

		triggerNextElementEffect();
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "ElementEffectAni")
		{
			if(counter >= sequenceObjects.Count)
			{
				notifyAllListeners("DiagElementSequenceScript","BubbleCreated",mainParentObj);
				Destroy(this);
			}
			else
			{
				triggerNextElementEffect();
			}
		}
	}

	private void triggerNextElementEffect()
	{
		GameObject reqObj = sequenceObjects[counter];
		reqObj.SetActive(true);
		Vector3 reqScaleObj = destScales[counter];
		counter++;

		CustomAnimationManager aniMang = reqObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("GrowOrShrink",1,new List<System.Object>() { new float[3] { reqScaleObj.x,reqScaleObj.y,reqScaleObj.z }, 5f }));
		batchLists.Add(batch1);

		if(counter >= sequenceObjects.Count)
		{
			List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
			batch2.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>() { endSequenceDelay_Sec }));
			batchLists.Add(batch2);
		}

		aniMang.registerListener("DiagElementSequenceScript",this);
		aniMang.init("ElementEffectAni",batchLists);
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class PostmanItemRevealScript : MonoBehaviour, CustomActionListener, IActionNotifier
{
	GameObject postmanGObj;
	GameObject basketGObj;
	GameObject createdWordBox;

	bool[] upAxisArr = new bool[3] { false, true, false };

	float displayDelayInSec = 3f;

	float desiredMaxFontCharSize = 0.04f;


	public void init(GameObject para_postmanGObj, GameObject para_basketGObj, string para_postmanWord, Transform para_wordBoxPrefab,bool para_useTts)
	{

		UnityEngine.Debug.LogWarning("INIT");
		postmanGObj = para_postmanGObj;
		basketGObj = para_basketGObj;




		int postmanID = int.Parse(postmanGObj.name.Split('-')[1]);

		float reqWidth = basketGObj.renderer.bounds.size.x * 1.5f;

		Rect reqBounds = new Rect(basketGObj.renderer.bounds.center.x - (reqWidth/2f),
		                          basketGObj.renderer.bounds.center.y + (basketGObj.renderer.bounds.size.y/2f),
		                          reqWidth,
		                          basketGObj.renderer.bounds.size.y);




		bool text = !para_useTts;

		if(para_useTts){
			if(!WorldViewServerCommunication.tts.test(para_postmanWord)){
				text=true;
			}else{

			try
			{
				if(gameObject.GetComponent<AudioSource>() == null) { gameObject.AddComponent<AudioSource>(); }
					audio.PlayOneShot(WorldViewServerCommunication.tts.say(para_postmanWord));
			}
			catch(System.Exception ex)
			{
				Debug.LogError("Failed to use TTS. "+ex.Message);
				text = true;
			}
			}
		}

		GameObject postmanWordBox;
		if(text){
			postmanWordBox = WordBuilderHelper.buildWordBox(1,para_postmanWord.Replace("/",""),reqBounds,basketGObj.transform.position.z + (-0.1f),upAxisArr,para_wordBoxPrefab);
		}else{
			postmanWordBox = WordBuilderHelper.buildWordBox(1,"",reqBounds,basketGObj.transform.position.z + (-0.1f),upAxisArr,para_wordBoxPrefab);

		}
		postmanWordBox.name = "PostmanWordBox"+ postmanID;
		DelayForInterval delayScript = postmanWordBox.AddComponent<DelayForInterval>();
		delayScript.registerListener("PostmanDisplayDelay",this);
		delayScript.init(displayDelayInSec);


		TextMesh tMesh = postmanWordBox.transform.FindChild("Text").GetComponent<TextMesh>();
		if(tMesh.characterSize > desiredMaxFontCharSize) { tMesh.characterSize = desiredMaxFontCharSize; }
		postmanWordBox.transform.FindChild("Text").renderer.sortingOrder = 600;

		Destroy(postmanWordBox.transform.FindChild("Board").gameObject);

		createdWordBox = postmanWordBox;
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "DelayEnd")
		{
			Destroy(createdWordBox);
			Destroy(this);
		}
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

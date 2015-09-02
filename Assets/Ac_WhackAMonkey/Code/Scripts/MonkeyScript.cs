/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class MonkeyScript : MonoBehaviour, IActionNotifier, CustomActionListener
{
	int holeID;
	float stayDuration_Sec;
	public bool isGoodMonkey;
	bool isEating;

	string copyOfWord;

	
	public void triggerMonkeyPopup(int para_holeID,
								   string para_word,
	                               bool para_isGoodMonkey,
	                               float para_stayDuration_Sec,
	                               bool[] para_upAxisArr,
	                               Transform para_wordBoxPrefab)
	{
		holeID = para_holeID;
		stayDuration_Sec = para_stayDuration_Sec;
		isGoodMonkey = para_isGoodMonkey;
		isEating = false;
		copyOfWord = para_word;



		// Spawn word box.
		Transform tmpChildPresent = transform.FindChild("WordBox");
		if(tmpChildPresent != null)
		{
			Destroy(tmpChildPresent.gameObject);
		}

		GameObject txtArea = (transform.FindChild("TextArea")).gameObject;
		Rect wBounds = new Rect(txtArea.renderer.bounds.center.x - (txtArea.renderer.bounds.size.x/2f),
		                        txtArea.renderer.bounds.center.y + (txtArea.renderer.bounds.size.y/2f),
		                        txtArea.renderer.bounds.size.x,
		                        txtArea.renderer.bounds.size.y);

		GameObject wBox = WordBuilderHelper.buildWordBox(0,para_word,wBounds,txtArea.transform.position.z,para_upAxisArr,para_wordBoxPrefab);
		WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() {wBox},0.08f);
		wBox.name = "WordBox";
		wBox.transform.parent = transform;
		Destroy(wBox.transform.FindChild("Board").gameObject);
		wBox.transform.FindChild("Text").renderer.sortingOrder = 512;


		// Trigger animation.

		triggerSoundAtCamera("DoorOpen");

		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("TriggerAnimation",1, new List<System.Object>() { "ShuttersOpen" }));
		batchList.Add(batch1);
		aniMang.registerListener("Monkey",this);
		aniMang.init("MonkeyEmergeAni",batchList);
	}

	private void performMonkeyStay()
	{
		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("DelayForInterval",1, new List<System.Object>() { stayDuration_Sec }));
		batchList.Add(batch1);
		aniMang.registerListener("Monkey",this);
		aniMang.init("MonkeyStayAni",batchList);
	}

	private void performMonkeyReturn()
	{
		transform.collider.enabled = false;
		Transform wordBoxChild = transform.FindChild("WordBox");
		if(wordBoxChild != null) { Destroy(wordBoxChild.gameObject); }

		triggerSoundAtCamera("DoorClose");

		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("TriggerAnimation",1, new List<System.Object>() { "ShuttersClosed" }));
		batchList.Add(batch1);
		aniMang.registerListener("Monkey",this);
		aniMang.init("MonkeyReturnAni",batchList);
	}

	private void performEatBanana()
	{
		transform.collider.enabled = false;
		Transform wordBoxChild = transform.FindChild("WordBox");
		if(wordBoxChild != null) { Destroy(wordBoxChild.gameObject); }

		triggerSoundAtCamera("Eat");

		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("TriggerAnimation",1, new List<System.Object>() { "MonkeyEating" }));
		batchList.Add(batch1);
		aniMang.registerListener("Monkey",this);
		aniMang.init("MonkeyEatAni",batchList);
	}

	private void performCooldown()
	{
		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("DelayForInterval",1, new List<System.Object>() { 2f }));
		batchList.Add(batch1);
		aniMang.registerListener("Monkey",this);
		aniMang.init("Cooldown",batchList);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "MonkeyEmergeAni")
		{
			// Perform stay animation.
			transform.collider.enabled = true;
			performMonkeyStay();
		}
		else if(para_eventID == "MonkeyStayAni")
		{
			// Perform return animation.
			if( ! isEating)
			{
				performMonkeyReturn();
			}
		}
		else if(para_eventID == "MonkeyReturnAni")
		{
			performCooldown();
		}
		else if(para_eventID == "MonkeyEatAni")
		{
			List<System.Object> dataToSend = new List<System.Object>();
			dataToSend.Add(holeID);
			dataToSend.Add(copyOfWord);

			notifyAllListeners(transform.name,"MonkeyAteBanana",dataToSend);
			performMonkeyReturn();
		}
		else if(para_eventID == "Cooldown")
		{
			List<System.Object> dataToSend = new List<System.Object>();
			dataToSend.Add(holeID);
			dataToSend.Add(copyOfWord);
			bool missedMonkey = ((isGoodMonkey)&&(isEating == false));
			dataToSend.Add(missedMonkey);

			notifyAllListeners(transform.name,"MonkeyReturn",dataToSend);	
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.name.Contains("Banana"))
		{
			if(isGoodMonkey)
			{
				if( ! isEating)
				{
					isEating = true;
					performEatBanana();
				}
			}
			else
			{
				List<System.Object> dataToSend = new List<System.Object>();
				dataToSend.Add(holeID);
				dataToSend.Add(copyOfWord);
				notifyAllListeners(transform.name,"MonkeyRejectedBanana",dataToSend);
			}
		}
	}

	private void triggerSoundAtCamera(string para_soundFileName)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		Transform  sfxPrefab = Resources.Load<Transform>("Prefabs/SFxBox");
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

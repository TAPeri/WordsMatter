/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class TextAudioManager : MonoBehaviour, IActionNotifier
{

	public delegate void AudioCallback();

	VoiceOverService voiceOverTool;
	//LocalTextToSpeechService localTextToSpeechTool;
	
	List<AudioClip> currSoundQueue;
	GameObject currSoundObj;



	void Start()
	{
		//prepTools();
	}

	void OnDestroy()
	{
		Destroy(currSoundObj);
	}

	public void init(){

		prepTools();

	}

	private void prepTools()
	{

		Debug.Log("Voice over ready");
		voiceOverTool = new VoiceOverService();
		//localTextToSpeechTool = new LocalTextToSpeechService();
		currSoundQueue = null;
		currSoundObj = null;
	}



	public bool isVoiceOverAvailable()
	{
		// TMP FIX.
		return (LocalisationMang.langCode == LanguageCode.EN);
	}

	public bool playVoiceOver(string para_reqParamStr,
	                          string para_listenerName,
	                          CustomActionListener para_listener)
	{
		bool validFlag = true;
		Debug.LogWarning("Replaced assignment with comparisson");
		if(validFlag == true)
		{
			GameObject poRef = PersistentObjMang.getInstance();
			DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
			bool isMaleSetting = true;
			if(ds.containsData("PlayerAvatarSettings"))
			{
				PlayerAvatarSettings playerAvSettings = (PlayerAvatarSettings) ds.getData("PlayerAvatarSettings");
				string genderStr = playerAvSettings.getGender();
				if(genderStr == "Female") { isMaleSetting = false; }
			}

			List<AudioClip> clipList = voiceOverTool.extractContent(para_reqParamStr,isMaleSetting);
			List<AudioClip> filteredClipList = new List<AudioClip>();
			if(clipList != null)
			{
				bool atLeastOneClip = false;
				for(int i=0; i<clipList.Count; i++)
				{
					AudioClip tmpClip = clipList[i];
					if(tmpClip != null)
					{
						atLeastOneClip = true;
						filteredClipList.Add(tmpClip);
					}
				}

				if( ! atLeastOneClip)
				{
					reportQueueProcessingCompleted();
				}
				else
				{
					currSoundQueue = filteredClipList;
					registerListener(para_listenerName,para_listener);
					playNextSoundInQueue();
				}
			}
		}


		return validFlag;
	}


	
	private void playNextSoundInQueue()
	{
		bool continueFlag = false;
		if(currSoundQueue != null)
		{
			if(currSoundQueue.Count > 0)
			{
				continueFlag = true;
			}
		}

		if(continueFlag)
		{
			triggerSoundAtCamera(currSoundQueue[0]);
		}
		else
		{
			reportQueueProcessingCompleted();
		}
	}

	private void reportQueueProcessingCompleted()
	{
		notifyAllListeners("TextAudioManager","RequestCompleted",null);
	}

	private void audioDoneCallback()
	{
		if(currSoundQueue != null)
		{
			if(currSoundQueue.Count > 0)
			{
				currSoundQueue.RemoveAt(0);
			}
		}
		playNextSoundInQueue();
	}

	private void triggerSoundAtCamera(AudioClip para_clip)
	{
		if(currSoundObj == null) { currSoundObj = new GameObject("SoundObj"); }

		float tmpClipLength = 1.5f;

		// We currently don't have good audio files for Greek or other languages besides English.
		//if(LocalisationMang.langCode == LanguageCode.EN)
		//{
			AudioSource audS = currSoundObj.GetComponent<AudioSource>();
			if(audS == null) { audS = currSoundObj.AddComponent<AudioSource>(); }
			audS.clip = para_clip;
			audS.volume = 1f;
			audS.PlayOneShot(para_clip);
			tmpClipLength = para_clip.length;
		//}
		//else
		//{
		//	Debug.LogWarning("No good audio files available");
		//}

		StartCoroutine(DelayedCallback(tmpClipLength,audioDoneCallback));
	}
		               	
	private System.Collections.IEnumerator DelayedCallback(float para_time, AudioCallback para_callback)
	{
		yield return  new WaitForSeconds(para_time);
		para_callback();
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}
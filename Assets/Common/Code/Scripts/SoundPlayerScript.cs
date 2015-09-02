/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class SoundPlayerScript : MonoBehaviour
{
	public Transform sfxPrefab;

	public void setSoundPrefab(Transform para_soundPrefab)
	{
		sfxPrefab = para_soundPrefab;
	}

	public void triggerSoundAtCamera(string para_soundFileName)
	{
		triggerSoundAtCamera(para_soundFileName,0.5f);
	}

	public void triggerSoundAtCamera(string para_soundFileName, float para_volume)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = para_volume;
		audS.Play();
	}

	public void triggerSoundAtCamera_OnlyDestroyWhenReady(string para_soundFileName)
	{

	}

	public AudioSource triggerSoundLoopAtCamera(string para_soundFileName, string para_soundObjName, float para_volume, bool para_moveWithCamera)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		nwSFX.name = para_soundObjName;
		Destroy(nwSFX.GetComponent<DestroyAfterTime>());
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = para_volume;
		audS.loop = true;
		audS.Play();

		if(para_moveWithCamera)
		{
			nwSFX.transform.parent = Camera.main.transform;
		}

		return audS;
	}
}
/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class CircleFadeScript : MonoBehaviour
{
	float fadeDuration_Sec;
	float startTime;
	bool fadeIn;


	void Update()
	{
		if((Time.time - startTime) >= fadeDuration_Sec)
		{
			Destroy(this);
		}
		else
		{
			float fadeValue = (Time.time - startTime) / fadeDuration_Sec;
			if(fadeIn)
			{
				fadeValue = 1 - fadeValue;
			}
			//float fadeValue = 0.5f;
			renderer.material.SetFloat("_Cutoff",fadeValue);
		}
	}

	public void init(float para_fadeDurationSec, bool para_fadeIn)
	{
		fadeDuration_Sec = para_fadeDurationSec;
		startTime = Time.time;
		fadeIn = para_fadeIn;
	}
}

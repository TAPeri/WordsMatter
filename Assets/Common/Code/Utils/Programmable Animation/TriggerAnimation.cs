/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class TriggerAnimation : AbsCustomAniCommand
{
	Animator aniScript;
	string aniName;
	bool initDone = false;

	// This delay was included because Unity's Animator cannot update its play parameters and run Update at the same time.
	// Without the delay, Update is called before the animator and TriggerAnimation would exit immediately due to normalizedTime not being reset in time by Unity.
	//float initCallTimestamp;
	float initDelay = 0.5f;
	
	void Update()
	{
		if(initDone)
		{
			if(((aniScript.speed > 0)&&(aniScript.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1))
			||((aniScript.speed < 0)&&(aniScript.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0)))
			{
				notifyAllListeners(transform.name,"AnimationEnd",null);
				Destroy(this);
			}
		}
		else
		{
			if((Time.time) >= initDelay)
			{
				initDone = true;
			}
		}
	}
	
	public void init(string para_aniName, float para_aniSpeed)
	{
		aniScript = transform.gameObject.GetComponent<Animator>();

		float startTime = 0;
		if(para_aniSpeed <= 0) { startTime = 1; }

		aniScript.Play(para_aniName,0,startTime);
		aniScript.speed = para_aniSpeed;
	}

	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		if(para_prep.mode == 1)
		{
			string p_aniName = (string) para_prep.parameters[0];
			this.init(p_aniName,1);
		}
		else
		{
			string p_aniName = (string) para_prep.parameters[0];
			float p_aniSpeed = (float) para_prep.parameters[1];
			this.init(p_aniName,p_aniSpeed);
		}
		return true;
	}
}

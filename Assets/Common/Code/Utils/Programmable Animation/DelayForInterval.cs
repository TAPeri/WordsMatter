/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class DelayForInterval : AbsCustomAniCommand
{
	
	float startTimestamp;
	float delayTimeInSec;


	void Update()
	{
		if((Time.time - startTimestamp) >= delayTimeInSec)
		{
			notifyAllListeners(transform.name,"DelayEnd",null);
			Destroy(this);
		}
	}
	
	public void init(float para_delayTimeInSec)
	{
		startTimestamp = Time.time;
		delayTimeInSec = para_delayTimeInSec;
	}


	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		float p_dTimeSec = (float) para_prep.parameters[0];
		this.init(p_dTimeSec);
		return true;
	}
}

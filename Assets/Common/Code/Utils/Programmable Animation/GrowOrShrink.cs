/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class GrowOrShrink : AbsCustomAniCommand
{
	Vector3 destScale;
	Vector3 dirVect;
	float speedPerSec;

	void Update()
	{
		float diffVal = speedPerSec * Time.deltaTime;

		if(Vector3.Distance(transform.localScale,destScale) <= diffVal)
		{
			transform.localScale = destScale;
			notifyAllListeners(transform.name,"GrowOrShrink",null);
			Destroy(this);
		}
		else
		{
			transform.localScale += (dirVect * diffVal);
		}
	}

	public void init(Vector3 para_destScale, float para_speedPerSec)
	{
		destScale = para_destScale;
		speedPerSec = para_speedPerSec;

		dirVect = Vector3.Normalize(destScale - transform.localScale);
	}

	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		float[] p_destScale = (float[]) para_prep.parameters[0];
		float p_speedPerSec = (float) para_prep.parameters[1];
		this.init(new Vector3(p_destScale[0],p_destScale[1],p_destScale[2]),p_speedPerSec);
		return true;
	}
}

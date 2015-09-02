/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class InjectLocalScale : AbsCustomAniCommand
{
	Vector3 destScale;

	void Update()
	{
		Vector3 tmpScale = transform.localScale;
		tmpScale.x = destScale.x;
		tmpScale.y = destScale.y;
		tmpScale.z = destScale.z;
		transform.localScale = tmpScale;

		notifyAllListeners(transform.gameObject.name,"InjectLocalScale",null);
		Destroy(this);
	}

	public void init(Vector3 para_destScale)
	{
		destScale = para_destScale;
	}
	
	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		float[] p_destScale = (float[]) para_prep.parameters[0];
		this.init(new Vector3(p_destScale[0],p_destScale[1],p_destScale[2]));
		return true;
	}
}

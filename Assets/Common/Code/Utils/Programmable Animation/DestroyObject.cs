/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class DestroyObject : AbsCustomAniCommand
{
	void Update()
	{
		notifyAllListeners(transform.name,"DestroyObject",null);
		Destroy(transform.gameObject);
	}
	
	public void init()
	{
	}	
	
	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		init();
		return true;
	}
}

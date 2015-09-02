/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

// DO NOT USE: WIP, INCOMPLETE.
public class FlashObject : AbsCustomAniCommand
{
	int currFlashID;
	int totNumFlashes;
	//float flashDelaySec;

	void Update()
	{
		if(currFlashID >= totNumFlashes)
		{
			notifyAllListeners(transform.name,"FlashObject",null);
			Destroy(this);
		}
		else
		{

		}
	}
	
	public void init(int para_numOfFlashes, float para_flashDelaySec)
	{
		currFlashID = 0;
		totNumFlashes = para_numOfFlashes;
		//flashDelaySec = para_flashDelaySec;
	}	
	
	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		int p_totNumFlashes = (int) para_prep.parameters[0];
		float p_flashDelaySec = (float) para_prep.parameters[1];
		init(p_totNumFlashes,p_flashDelaySec);
		return true;
	}
}

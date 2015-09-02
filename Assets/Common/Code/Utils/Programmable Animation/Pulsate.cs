/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class Pulsate : AbsCustomAniCommand
{
	

	int state;

	Vector3 smallState_Scale;
	Vector3 normalState_Scale;
	Vector3 largeState_Scale;

	Vector3 currDestScale;
	Vector3 changeVectPerSec;

	int endState;


	void Update()
	{

		if(Vector3.Distance(transform.localScale,currDestScale) < (changeVectPerSec.magnitude * Time.deltaTime))
		{
			if(state == endState)
			{
				terminate();
			}

			switch(state)
			{
				case 0: currDestScale = normalState_Scale;
						changeVectPerSec *= -1f;
						break;

				case 1: currDestScale = smallState_Scale;
						break;

				case 2: currDestScale = normalState_Scale;
						changeVectPerSec *= -1f;
						break;

				case 3:	terminate();
						break;
			}
			state++;
		}
		else
		{
			transform.localScale += (changeVectPerSec * Time.deltaTime);
		}
	}


	public void init(float para_growFactor, float para_durationSec, int para_endState)
	{
		normalState_Scale = new Vector3(transform.localScale.x,transform.localScale.y,transform.localScale.z);
		smallState_Scale = normalState_Scale / (para_growFactor);
		largeState_Scale = normalState_Scale * para_growFactor;

		changeVectPerSec = divVect((largeState_Scale - normalState_Scale),para_durationSec/((endState+2) * 1.0f) );  // 4 states.  norm->big, big->norm, norm->small, small->norm.

		endState = para_endState;

		state = 0;
		currDestScale = new Vector3(largeState_Scale.x,largeState_Scale.y,largeState_Scale.z);
	}
	
	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		float p_growFactor = (float) para_prep.parameters[0];
		float p_durationSec = (float) para_prep.parameters[1];
		int p_endState = (int) para_prep.parameters[2];
		this.init(p_growFactor,p_durationSec,p_endState);
		return true;
	}

	private Vector3 divVect(Vector3 para_vect, float para_divVal)
	{
		Vector3 nwVect = new Vector3(para_vect.x/para_divVal,para_vect.y/para_divVal,para_vect.z/para_divVal);
		return nwVect;
	}

	private void terminate()
	{
		notifyAllListeners(transform.name,"PulsateEnd",null);
		Destroy(this);
	}
}

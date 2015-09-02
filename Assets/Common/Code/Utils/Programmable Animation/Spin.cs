/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class Spin : AbsCustomAniCommand 
{

	float anglePerSec;
	float haltAtMaxRot;
	float numRotsSoFar;

	//float origAngle;
	float currRotAngle;

	float currRoundAngle;

	bool isInfiniteSpin;



	void Update()
	{

		if(( ! isInfiniteSpin)&&(numRotsSoFar >= haltAtMaxRot))
		{
			/*float rem = haltAtMaxRot % 2f;
			Vector3 angleVect = transform.localEulerAngles;
			angleVect.z = (int) (360f - (360f * rem));
			transform.localEulerAngles = angleVect;*/

			notifyAllListeners(transform.name,"SpinComplete",null);
			Destroy(this);
		}
		else
		{
			if(currRotAngle >= currRoundAngle)
			{
				currRotAngle = 360f - currRotAngle;

				if(currRoundAngle == 360f)
				{
					numRotsSoFar++;
				}
				else
				{
					numRotsSoFar += (currRoundAngle/360f);
				}

				if(( ! isInfiniteSpin)&&((haltAtMaxRot - numRotsSoFar) < 1))
				{
					currRoundAngle = (360f * (haltAtMaxRot - numRotsSoFar));
				}
			}
			else
			{
				currRotAngle += (anglePerSec * Time.deltaTime);

				Vector3 angleVect = transform.localEulerAngles;
				angleVect.z = 360f - currRotAngle;
				transform.localEulerAngles = angleVect;
			}
		}

	}


	// Finite spin mode.
	public void init(float para_rotationsPerSecond, float para_haltAtMaxRot)
	{
		anglePerSec = (para_rotationsPerSecond * 360.0f);
		haltAtMaxRot = para_haltAtMaxRot;
		numRotsSoFar = 0;
		currRotAngle = 0;
		//origAngle = transform.localEulerAngles.z;
		isInfiniteSpin = false;

		if(para_haltAtMaxRot >= 1f) { currRoundAngle = 360f; } else { currRoundAngle = 360f * para_haltAtMaxRot; } 
	}

	// Infinite spin mode.
	public void init(float para_rotationsPerSecond)
	{
		anglePerSec = (para_rotationsPerSecond * 360.0f);
		numRotsSoFar = 0;
		currRotAngle = transform.localEulerAngles.z;// 0;
		//origAngle = transform.localEulerAngles.z;
		currRoundAngle = 360f;
		isInfiniteSpin = true;
	}
	

	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		if(para_prep.mode == 1)
		{
			float p_rotationsPerSecond = (float) para_prep.parameters[0];
			float p_haltAtMaxRot = (float) para_prep.parameters[1];
			this.init(p_rotationsPerSecond,p_haltAtMaxRot);
		}
		else if(para_prep.mode == 2)
		{
			float p_rotationsPerSecond = (float) para_prep.parameters[0];
			this.init(p_rotationsPerSecond);
		}
		return true;
	}
}

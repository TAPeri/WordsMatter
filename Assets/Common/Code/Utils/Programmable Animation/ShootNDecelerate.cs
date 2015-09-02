/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class ShootNDecelerate : AbsCustomAniCommand
{
	Vector3 velocity;
	Vector3 normDecelVect;
	float deceleration;
	Vector3 decelVect;


	void Update()
	{
		decelVect = (normDecelVect * (deceleration * Time.deltaTime));

		if((velocity.magnitude - decelVect.magnitude) <= 0)
		{
			Vector3 endPos = transform.position + decelVect;
			transform.position = endPos;

			velocity = new Vector3(0,0,0);
			deceleration = 0;

			notifyAllListeners(transform.name,"ShootNDecelerate",null);
			Destroy(this);
		}
		else
		{
			velocity += decelVect;
			transform.position += velocity;
		}
	}



	public void initScript(Vector3 para_initialNormalisedDir, float para_initialSpeed, float para_deceleration)
	{
		velocity = para_initialNormalisedDir * para_initialSpeed;
		normDecelVect = Vector3.Normalize(-velocity);
		deceleration = para_deceleration;
	}


	
	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		float[] p_initialDir = (float[]) para_prep.parameters[0];
		float p_initialSpeed = (float) para_prep.parameters[1];
		float p_deceleration = (float) para_prep.parameters[2];
		this.initScript(new Vector3(p_initialDir[0],p_initialDir[1],p_initialDir[2]),p_initialSpeed,p_deceleration);
		
		return true;
	}


}

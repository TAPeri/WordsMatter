/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class GrowProportionateToDistance : AbsCustomAniCommand
{

	Vector3 worldInfluencePt;
	Vector3 finalScaleVect;
	float startDistanceFromTarget;
	Vector3 originalScaleVect;


	// Small optimisation.
	float prevDistToTarget;


	
	void Start()
	{
		startDistanceFromTarget = Vector3.Distance(worldInfluencePt,transform.position);
		originalScaleVect = new Vector3(transform.localScale.x,transform.localScale.y,transform.localScale.z);
		prevDistToTarget = startDistanceFromTarget;
	}
	

	void Update()
	{
		float currDistToTarget = Vector3.Distance(worldInfluencePt,transform.position);

		if(currDistToTarget != prevDistToTarget)
		{
			float percOfTotalDist = (1 - (currDistToTarget/startDistanceFromTarget));
			Vector3 changeV = (finalScaleVect - originalScaleVect) * percOfTotalDist;
			transform.localScale = originalScaleVect + changeV;//originalScaleVect + changeV;
			prevDistToTarget = currDistToTarget;
		}

		if(currDistToTarget == 0)
		{
			notifyAllListeners(transform.name,"GrowProportionateToDistance",null);
			Destroy(this);
		}
	}


	public void init(Vector3 para_worldInfluencePt, Vector3 para_initialWorldBounds, Rect para_finalWorldSize)
	{
		worldInfluencePt = para_worldInfluencePt;

		float dest_widthInWorldSpace = para_finalWorldSize.width;
		float dest_heightInWorldSpace = para_finalWorldSize.height;

		float curr_widthInWorldSpace = para_initialWorldBounds.x;
		float curr_heightInWorldSpace = para_initialWorldBounds.y;

		Vector3 origLocalScale = new Vector3(transform.localScale.x,transform.localScale.y,transform.localScale.z);
		Vector3 diffScale = new Vector3(dest_widthInWorldSpace/curr_widthInWorldSpace, dest_heightInWorldSpace/curr_heightInWorldSpace, 1);
		Vector3 destScale = new Vector3(origLocalScale.x * diffScale.x,origLocalScale.y * diffScale.y,origLocalScale.z * diffScale.z);

		finalScaleVect = destScale - origLocalScale;
	}

	public void init(Vector3 para_worldInfluencePt, Vector3 para_finalScaleVect)
	{
		worldInfluencePt = para_worldInfluencePt;
		finalScaleVect = para_finalScaleVect;
	}


	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{

		if(para_prep.mode == 1)
		{
			float[] p_worldInfluencePt = (float[]) para_prep.parameters[0];
			float[] p_finalScaleVect = (float[]) para_prep.parameters[1];
			this.init(new Vector3(p_worldInfluencePt[0],p_worldInfluencePt[1],p_worldInfluencePt[2]),
			          new Vector3(p_finalScaleVect[0],p_finalScaleVect[1],p_finalScaleVect[2]));
		}
		else
		{
			float[] p_worldInfluencePt = (float[]) para_prep.parameters[0];
			float[] p_initWorldBounds = (float[]) para_prep.parameters[1];
			float[] p_finalGUISize = (float[]) para_prep.parameters[2];
			this.init(new Vector3(p_worldInfluencePt[0],p_worldInfluencePt[1],p_worldInfluencePt[2]),
			          new Vector3(p_initWorldBounds[0],p_initWorldBounds[1],p_initWorldBounds[2]),
			          new Rect(p_finalGUISize[0],p_finalGUISize[1],p_finalGUISize[2],p_finalGUISize[3]));
		}

		return true;
	}
}

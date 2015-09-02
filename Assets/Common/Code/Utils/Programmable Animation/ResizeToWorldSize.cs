/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class ResizeToWorldSize : AbsCustomAniCommand
{

	bool isDone;
	float[] destWorldSize;
	float[] initObjSize;




	void Update()
	{
		if( ! isDone)
		{
			Vector3 destSize = new Vector3(destWorldSize[0],destWorldSize[1],destWorldSize[2]);
			Vector3 currSize = new Vector3(initObjSize[0],initObjSize[1],initObjSize[2]);
			Vector3 currScale = new Vector3(transform.localScale.x,transform.localScale.y,transform.localScale.z);

			Vector3 destScale = divideVectors(multiplyVectors(destSize,currScale),currSize);

			transform.localScale = destScale;

			isDone = true;
		}
		else
		{
			notifyAllListeners(transform.name,"ResizeToWorldSize",null);
			Destroy(this);
		}
	}


	public void init(float[] para_destWorldSize, float[] para_initObjSize)
	{
		isDone = false;
		destWorldSize = para_destWorldSize;
		initObjSize = para_initObjSize;
	}


	private Vector3 multiplyVectors(Vector3 para_v1, Vector3 para_v2)
	{
		return (new Vector3(para_v1.x * para_v2.x,
		                    para_v1.y * para_v2.y,
		                    para_v1.z * para_v2.z));
	}

	private Vector3 divideVectors(Vector3 para_v1, Vector3 para_v2)
	{
		return (new Vector3(para_v1.x / para_v2.x,
		                    para_v1.y / para_v2.y,
		                    para_v1.z / para_v2.z));
	}


	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		float[] p_destWorldSize = (float[]) para_prep.parameters[0];
		float[] p_initObjSize = (float[]) para_prep.parameters[1];
		this.init(p_destWorldSize,p_initObjSize);
		return true;
	}
}

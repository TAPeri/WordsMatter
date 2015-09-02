/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class AdjustCameraViewport : AbsCustomAniCommand
{

	bool isDone;
	Vector4 currVect;
	Vector4 destViewportVect;
	Vector4 changeVectPerSec;
	Camera camScript;



	void Update()
	{
		if(!isDone)
		{
			float deltaDist = (changeVectPerSec * Time.deltaTime).magnitude;
			if(Vector4.Distance(currVect,destViewportVect) <= deltaDist)
			{
				currVect = destViewportVect;
				isDone = true;
			}
			else
			{
				currVect += (changeVectPerSec * Time.deltaTime);
			}

			Rect tmpCRect = camScript.rect;
			tmpCRect.x = currVect.x;
			tmpCRect.y = currVect.y;
			tmpCRect.width = currVect.z;
			tmpCRect.height = currVect.w;
			camScript.rect = tmpCRect;
		}
		else
		{
			notifyAllListeners(transform.name,"AdjustCameraViewport",null);
			Destroy(this);
		}
	}


	public void init(Vector4 para_destViewportVect, float para_totalTransitionTimeSec)
	{
		isDone = false;
		camScript = transform.GetComponent<Camera>();
		currVect = new Vector4(camScript.rect.x,camScript.rect.y,camScript.rect.width,camScript.rect.height);
		destViewportVect = para_destViewportVect;

		changeVectPerSec = destViewportVect - currVect;
		float reqMag = (changeVectPerSec.magnitude / para_totalTransitionTimeSec);
		changeVectPerSec = (Vector4.Normalize(changeVectPerSec) * reqMag);
	}
	
	
	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		float[] p_destViewportVect = (float[]) para_prep.parameters[0];
		float p_totTransitionTimeSec = (float) para_prep.parameters[1];
		this.init(new Vector4(p_destViewportVect[0],p_destViewportVect[1],destViewportVect[2],destViewportVect[3]),
		          p_totTransitionTimeSec);
		return true;
	}

}

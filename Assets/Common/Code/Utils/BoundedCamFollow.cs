/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class BoundedCamFollow : MonoBehaviour
{

	GameObject itemToFollow;
	bool[] axisToFollow;
	Rect maxBounds2D;

	Rect maxBounds2DForCamPos;

	Vector3 tmpFuturePos;
	

	void Update()
	{


		if(axisToFollow[0])	{ tmpFuturePos.x = itemToFollow.transform.position.x; } else { tmpFuturePos.x = transform.position.x; }
		if(axisToFollow[1]) { tmpFuturePos.y = itemToFollow.transform.position.y; } else { tmpFuturePos.y = transform.position.y; }
		if(axisToFollow[2]) { tmpFuturePos.z = itemToFollow.transform.position.z; } else { tmpFuturePos.z = transform.position.z; }
	

		if((tmpFuturePos.x >= maxBounds2DForCamPos.x)&&(tmpFuturePos.x <= (maxBounds2DForCamPos.x + maxBounds2DForCamPos.width))
		&&(tmpFuturePos.y >= (maxBounds2DForCamPos.y - maxBounds2DForCamPos.height))&&(tmpFuturePos.y <= maxBounds2DForCamPos.y))
		{
			Vector3 tmpPos = transform.position;
			tmpPos.x = tmpFuturePos.x;
			tmpPos.y = tmpFuturePos.y;
			transform.position = tmpPos;
		}
	}


	public void init(GameObject para_itemToFollow, bool[] para_axisToFollow, Rect para_2DMaxBounds)
	{
		itemToFollow = para_itemToFollow;
		axisToFollow = para_axisToFollow;
		tmpFuturePos = new Vector2();

		Rect camWorld2DBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,false);

		// BROKEN.

		//if((camWorld2DBounds.xMin >= para_2DMaxBounds.xMin)&&(camWorld2DBounds.xMax <= para_2DMaxBounds.xMax)
		//&&(camWorld2DBounds.yMin >= para_2DMaxBounds.yMin)&&(camWorld2DBounds.yMax <= para_2DMaxBounds.yMax))
		//{
			// Cam bounds is smaller than total potential bounds.
		//}
		//else
		//{
			// Potential bounds is smaller and therefore no cam following is needed.
		//	Debug.LogWarning("No BoundedCamFollow needed. Potential bounds is smaller than cam bounds.");
		//	Destroy(this);
		//}


		Vector2 maxTL = new Vector2(para_2DMaxBounds.x + (camWorld2DBounds.width/2f),
		                            para_2DMaxBounds.y - (camWorld2DBounds.height/2f));

		Vector2 maxBR = new Vector2((para_2DMaxBounds.x + para_2DMaxBounds.width) - (camWorld2DBounds.width/2f),
		                            (para_2DMaxBounds.y - para_2DMaxBounds.height) + (camWorld2DBounds.height/2f));

		maxBounds2DForCamPos = new Rect(maxTL.x,maxTL.y,Mathf.Abs(maxBR.x - maxTL.x),Mathf.Abs(maxTL.y - maxBR.y));
	}
}

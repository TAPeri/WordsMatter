/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class ArrowMarkerScript : MonoBehaviour
{

	GameObject srcObj;
	GameObject destObj;
	float arrowDistFromSrc;
	Vector3 dirVect;

	Vector3 northVect = new Vector3(0,1,0);
	float angleAimDir;

	void Update()
	{

		if(destObj == null)
		{
			Destroy(this);
		}
		else
		{

			dirVect = destObj.transform.position - srcObj.transform.position;
			dirVect.z = 0;

			if(dirVect.magnitude <= arrowDistFromSrc)
			{
				transform.renderer.enabled = false;
			}
			else
			{
				transform.renderer.enabled = true;

				if(Vector3.Cross(northVect,Vector3.Normalize(dirVect)).z < 0)
				{
					angleAimDir = -1f;
				}
				else
				{
					angleAimDir = 1f;
				}

				transform.position = srcObj.transform.position + (Vector3.Normalize(dirVect) * arrowDistFromSrc);
				Vector3 tmpEAngles = transform.localEulerAngles;
				tmpEAngles.z = (Vector3.Angle(northVect,dirVect)) * angleAimDir;
				transform.localEulerAngles = tmpEAngles;
			}
		}
	}


	public void init(GameObject para_srcObj, GameObject para_destObj, float para_arrowDistFromSrc)
	{
		init(para_srcObj,para_destObj,para_arrowDistFromSrc,Color.white);
	}
	
	public void init(GameObject para_srcObj, GameObject para_destObj, float para_arrowDistFromSrc, Color para_arrowColor)
	{
		srcObj = para_srcObj;
		destObj = para_destObj;
		arrowDistFromSrc = para_arrowDistFromSrc;

		SpriteRenderer sRend = transform.gameObject.GetComponent<SpriteRenderer>();
		sRend.color = para_arrowColor;
	}
}

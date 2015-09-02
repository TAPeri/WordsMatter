/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ColorTransition : AbsCustomAniCommand
{
	
	bool isDone;
	Vector4 endColor;
	Vector4 changeVect;
	Vector4 tmpCurrCol;
	List<SpriteRenderer> sRendList;
	

	
	
	void Update()
	{
		for(int i=0; i<sRendList.Count; i++)
		{
			Color currCol = sRendList[i].color;
			
			Vector4 incVect = changeVect * Time.deltaTime;
			
			tmpCurrCol.x = currCol.r;
			tmpCurrCol.y = currCol.g;
			tmpCurrCol.z = currCol.b;
			tmpCurrCol.w = currCol.a;
			
			
			if(Vector4.Distance(endColor,tmpCurrCol) < incVect.magnitude)
			{
				currCol.r = endColor.x;
				currCol.g = endColor.y;
				currCol.b = endColor.z;
				currCol.a = endColor.w;
				isDone = true;
			}
			else
			{
				currCol.r += incVect.x;
				currCol.g += incVect.y;
				currCol.b += incVect.z;
				currCol.a += incVect.w;
			}
			
			sRendList[i].color = currCol;
		}
		
		
		if(isDone)
		{
			notifyAllListeners(transform.name,"ColorTransition",null);
			Destroy(this);
		}
	}
	
	
	public void init(Color para_endColor, float para_effectDurationInSec)
	{
		endColor = para_endColor;
		sRendList = getSpriteRendsOfChildrenRecursively(transform.gameObject);

		if(sRendList.Count > 0)
		{
			SpriteRenderer tmpSRend = sRendList[0];
			Vector4 currColor = new Vector4(tmpSRend.color.r,tmpSRend.color.g,tmpSRend.color.b,tmpSRend.color.a);
			changeVect = endColor - currColor;
			changeVect = changeVect/para_effectDurationInSec;
		}
		
		tmpCurrCol = new Vector4();
		isDone = false;
	}

	public List<SpriteRenderer> getSpriteRendsOfChildrenRecursively(GameObject para_tmpObj)
	{
		List<SpriteRenderer> localList = new List<SpriteRenderer>();

		SpriteRenderer tmpSRend = null;
		tmpSRend = para_tmpObj.GetComponent<SpriteRenderer>();
		if(tmpSRend != null)
		{
			localList.Add(tmpSRend);
		}

		for(int i=0; i<para_tmpObj.transform.childCount; i++)
		{
			List<SpriteRenderer> tmpRecList = getSpriteRendsOfChildrenRecursively((para_tmpObj.transform.GetChild(i)).gameObject);
			localList.AddRange(tmpRecList);
		}			

		return localList;
	}
	
	
	
	public override bool initViaCommandPrep (AniCommandPrep para_prep)
	{
		float[] p_endColor = (float[]) para_prep.parameters[0];
		float p_durationSec = (float) para_prep.parameters[1];
		this.init(new Color(p_endColor[0],p_endColor[1],p_endColor[2],p_endColor[3]),
		          p_durationSec);
		return true;
	}
	
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ColorTransitionNonSprite : AbsCustomAniCommand
{
	
	bool isDone;
	Vector4 endColor;
	Vector4 changeVect;
	Vector4 tmpCurrCol;
	List<Renderer> rendList;
	
	
	
	
	void Update()
	{
		for(int i=0; i<rendList.Count; i++)
		{
			Color currCol = rendList[i].material.color;
			
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
			
			rendList[i].material.color = currCol;
		}
		
		
		if(isDone)
		{
			notifyAllListeners(transform.name,"ColorTransitionNonSprite",null);
			Destroy(this);
		}
	}
	
	
	public void init(Color para_endColor, float para_effectDurationInSec)
	{
		endColor = para_endColor;
		rendList = getRenderersOfChildrenRecursively(transform.gameObject);
		
		if(rendList.Count > 0)
		{
			Renderer tmpRend = rendList[0];
			Color tmpCol = tmpRend.material.color;
			Vector4 currColor = new Vector4(tmpCol.r,tmpCol.g,tmpCol.b,tmpCol.a);
			changeVect = endColor - currColor;
			changeVect = changeVect/para_effectDurationInSec;
		}
		
		tmpCurrCol = new Vector4();
		isDone = false;
	}
	
	public List<Renderer> getRenderersOfChildrenRecursively(GameObject para_tmpObj)
	{
		List<Renderer> localList = new List<Renderer>();
		
		Renderer tmpRend = null;
		tmpRend = para_tmpObj.renderer;
		if(tmpRend != null)
		{
			localList.Add(tmpRend);
		}
		
		for(int i=0; i<para_tmpObj.transform.childCount; i++)
		{
			List<Renderer> tmpRecList = getRenderersOfChildrenRecursively((para_tmpObj.transform.GetChild(i)).gameObject);
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

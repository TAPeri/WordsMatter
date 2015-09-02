/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class CommonUnityUtils
{

	public static Bounds findMaxBounds(List<GameObject> paraObjs)
	{
		List<Bounds> boundsList = new List<Bounds>();
		for(int i=0; i<paraObjs.Count; i++)
		{
			boundsList.Add(paraObjs[i].renderer.bounds);
		}
		return findMaxBounds(boundsList);
	}

	public static Bounds findMaxBounds(List<Bounds> para_objs)
	{
		Bounds firstBound = para_objs[0];
		float[] minPt = new float[3] { firstBound.min.x, firstBound.min.y, firstBound.min.z };
		float[] maxPt = new float[3] { firstBound.max.x, firstBound.max.y, firstBound.max.z };
		
		for(int i=0; i<para_objs.Count; i++)
		{
			Bounds tmpB = para_objs[i];
			
			if(tmpB.min.x < minPt[0]) { minPt[0] = tmpB.min.x; }
			if(tmpB.min.y < minPt[1]) { minPt[1] = tmpB.min.y; }
			if(tmpB.min.z < minPt[2]) { minPt[2] = tmpB.min.z; }
			if(tmpB.max.x > maxPt[0]) { maxPt[0] = tmpB.max.x; }
			if(tmpB.max.y > maxPt[1]) { maxPt[1] = tmpB.max.y; }
			if(tmpB.max.z > maxPt[2]) { maxPt[2] = tmpB.max.z; }
		}
		
		Vector3 minPtVect = new Vector3(minPt[0],minPt[1],minPt[2]);
		Vector3 maxPtVect = new Vector3(maxPt[0],maxPt[1],maxPt[2]);
		
		
		
		Vector3 sizeVect = new Vector3(maxPtVect.x - minPtVect.x,maxPtVect.y - minPtVect.y,maxPtVect.z - minPtVect.z);
		Vector3 centreVect = new Vector3(minPtVect.x + (sizeVect.x/2f),minPtVect.y + (sizeVect.y/2f),minPtVect.z + (sizeVect.z/2f));
		Bounds reqBounds = new Bounds(centreVect,sizeVect);
		return reqBounds;
	}

	// Note: Assumes Rect.x and Rect.y indicate the top left of the Rectangle.
	// Note: Also assumes that the space origin is bottom left.
	public static Rect rescaleRect(Rect para_srcRect, float para_widthScale, float para_heightScale)
	{
		float nwWidth = para_srcRect.width * para_widthScale;
		float nwHeight = para_srcRect.height * para_heightScale;

		Rect retRect = new Rect(para_srcRect.x + (para_srcRect.width/2f) - (nwWidth/2f),
		                        para_srcRect.y - (para_srcRect.height/2f) + (nwHeight/2f),
		                        nwWidth,nwHeight);
		return retRect;
	}

	public static Rect get2DBounds(Bounds para_bounds)
	{
		Rect nwBounds = new Rect(para_bounds.min.x,
		                         para_bounds.max.y,
		                         para_bounds.size.x,
		                         para_bounds.size.y);
		
		return nwBounds;
	}


	public static Rect normaliseRect(Rect para_baseRect, Rect para_subRect)
	{
		Rect normRect = new Rect((para_subRect.x - para_baseRect.x)/para_baseRect.width,
		                         (para_subRect.y - para_baseRect.y)/para_baseRect.height,
		                         para_subRect.width/para_baseRect.width,
		                         para_subRect.height/para_baseRect.height);

		return normRect;
	}

	public static Rect findLookatBoundsWithinLimitedArea(Vector3 para_ptOfInterest, Rect para_lookatViewRect, Rect para_limitArea)
	{
		Vector3 ptOfInterest = para_ptOfInterest;
		Rect lookAtView = para_lookatViewRect;
		Rect limitArea = para_limitArea;
		
		Rect potentialBounds = new Rect(ptOfInterest.x - (lookAtView.width/2f), ptOfInterest.y + (lookAtView.height/2f), lookAtView.width, lookAtView.height);
		
		if(potentialBounds.x <= limitArea.x)
		{
			potentialBounds.x = (limitArea.x + 1);
		}
		else if((potentialBounds.x >= (limitArea.x + limitArea.width))
		        ||((potentialBounds.x + potentialBounds.width) >= (limitArea.x + limitArea.width)))
		{
			potentialBounds.x = (limitArea.x + limitArea.width - potentialBounds.width - 1);
		}
		
		if(potentialBounds.y >= limitArea.y)
		{
			potentialBounds.y = (limitArea.y - 1);
		}
		else if((potentialBounds.y <= (limitArea.y - limitArea.height))
		        ||((potentialBounds.y - potentialBounds.height) <= (limitArea.y - limitArea.height)))
		{
			potentialBounds.y = (limitArea.y - limitArea.height + potentialBounds.height + 1);
		}
		
		return potentialBounds;
	}


	public static void setSortingOrderOfEntireObject(GameObject para_gObj, int para_sLayer)
	{
		if(para_gObj == null) { return; }

		if(para_gObj.renderer != null)
		{
			para_gObj.renderer.sortingOrder += para_sLayer;
		}

		for(int i=0; i<para_gObj.transform.childCount; i++)
		{
			CommonUnityUtils.setSortingOrderOfEntireObject(para_gObj.transform.GetChild(i).gameObject,para_sLayer);
		}
	}

	public static void setSortingLayerOfEntireObject(GameObject para_gObj, string para_sLayer)
	{
		if(para_gObj == null) { return; }
		
		if(para_gObj.renderer != null)
		{
			para_gObj.renderer.sortingLayerName = para_sLayer;
		}
		
		for(int i=0; i<para_gObj.transform.childCount; i++)
		{
			CommonUnityUtils.setSortingLayerOfEntireObject(para_gObj.transform.GetChild(i).gameObject,para_sLayer);
		}
	}


	public static List<SpriteRenderer> getSpriteRendsOfChildrenRecursively(GameObject para_tmpObj)
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

	/*
	 *  BROKEN
	 * 
	 * public static Rect findMaxBounds(List<Rect> para_rects)
	{
		Rect firstBound = para_rects[0];
		float[] minPt = new float[2] { firstBound.xMin, firstBound.yMin };
		float[] maxPt = new float[2] { firstBound.xMax, firstBound.yMax };
		
		for(int i=0; i<para_rects.Count; i++)
		{
			Rect tmpR = para_rects[i];
			
			if(tmpR.xMin < minPt[0]) { minPt[0] = tmpR.xMin; }
			if(tmpR.yMin < minPt[1]) { minPt[1] = tmpR.yMin; }
			if(tmpR.xMax > maxPt[0]) { maxPt[0] = tmpR.xMax; }
			if(tmpR.yMax > maxPt[1]) { maxPt[1] = tmpR.yMax; }
		}
		
		Vector2 minPtVect = new Vector2(minPt[0],minPt[1]);
		Vector2 maxPtVect = new Vector2(maxPt[0],maxPt[1]);
		

		Rect reqBounds = new Rect(minPtVect.x,minPtVect.y,maxPtVect.x - minPtVect.x,maxPtVect.y - minPtVect.y);
		return reqBounds;
	}*/
}

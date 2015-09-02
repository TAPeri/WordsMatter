/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class SpawnNormaliser
{

	public static List<Vector3> convertWorldPoints(Rect para_srcBounds, Rect para_destBounds, List<Vector3> para_ptsSrcBounds)
	{
		List<Vector3> nwPts = new List<Vector3>();

		for(int i=0; i<para_ptsSrcBounds.Count; i++)
		{
			Vector3 tmpSrcPt = para_ptsSrcBounds[i];
			Vector2 normalisedPt = new Vector2((tmpSrcPt.x - para_srcBounds.x)/para_srcBounds.width,(para_srcBounds.y - tmpSrcPt.y)/para_srcBounds.height);

			Vector3 nwPt = new Vector3(para_destBounds.x + (para_destBounds.width * normalisedPt.x),
			                           para_destBounds.y - (para_destBounds.height * normalisedPt.y),
			                           tmpSrcPt.z);

			nwPts.Add(nwPt);
		}

		return nwPts;
	}

	public static void adjustGameObjectsToNwBounds(Rect para_srcBounds, Rect para_destBounds, List<GameObject> para_objList)
	{
		List<Vector3> initialPositions = new List<Vector3>();
		//List<bool> usesTopLeftPt = new List<bool>();
		for(int i=0; i<para_objList.Count; i++)
		{
			GameObject tmpObj = para_objList[i];
			//if(tmpObj.renderer != null)
			//{
			//	initialPositions.Add(new Vector3(tmpObj.transform.position.x - (tmpObj.renderer.bounds.size.x/2f),
			//	                                 tmpObj.transform.position.y + (tmpObj.renderer.bounds.size.y/2f),
			//	                                 tmpObj.transform.position.z));
			//	usesTopLeftPt.Add(true);
			//}
			//else
			//{
				initialPositions.Add(tmpObj.transform.position);
			//	usesTopLeftPt.Add(false);
			//}
		}

		List<Vector3> finalPositions = convertWorldPoints(para_srcBounds,para_destBounds,initialPositions);

		for(int i=0; i<para_objList.Count; i++)
		{
			GameObject tmpObj = para_objList[i];
			//Vector3 reqFinalPos = finalPositions[i];
			//if(usesTopLeftPt[i])
			//{
			//	tmpObj.transform.position = new Vector3(reqFinalPos.x + (tmpObj.renderer.bounds.size.x/2f),reqFinalPos.y - (tmpObj.renderer.bounds.size.y/2f),reqFinalPos.z);
			//}
			//else
			//{
				tmpObj.transform.position = finalPositions[i];
			//}
		}
	}

	public static Rect get2DBounds(Bounds para_bounds)
	{
		Rect nwBounds = new Rect(para_bounds.min.x,
		                         para_bounds.max.y,
		                         para_bounds.size.x,
		                         para_bounds.size.y);

		return nwBounds;
	}
}

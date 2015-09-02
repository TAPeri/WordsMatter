/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class WorldSpawnHelper : MonoBehaviour
{

	public static Rect getGuiToWorldBounds(Rect para_guiBounds, float para_depthCoord, bool[] para_upAxisArr)
	{
		Rect retWorldBounds = new Rect(0,0,1,1);
		
		Vector3 tl = Camera.main.ScreenToWorldPoint(new Vector3(para_guiBounds.x,Screen.height-para_guiBounds.y,para_depthCoord));
		Vector3 br = Camera.main.ScreenToWorldPoint(new Vector3(para_guiBounds.x + para_guiBounds.width,Screen.height - (para_guiBounds.y + para_guiBounds.height),para_depthCoord));
		
		
		if(para_upAxisArr[1])
		{
			retWorldBounds.x = tl.x;
			retWorldBounds.y = tl.y;
			retWorldBounds.width = Mathf.Abs(br.x - tl.x);
			retWorldBounds.height = Mathf.Abs(tl.y - br.y);
		}
		else
		{
			retWorldBounds.x = tl.x;
			retWorldBounds.y = tl.z;
			retWorldBounds.width = Mathf.Abs(br.x - tl.x);
			retWorldBounds.height = Mathf.Abs(tl.z - br.z);
		}
		
		return retWorldBounds;
	}

	public static Rect getWorldToGUIBounds(Bounds para_worldBounds, bool[] para_upAxisArr)
	{
		Vector3 tl = new Vector3(para_worldBounds.max.x - para_worldBounds.size.x,para_worldBounds.max.y,para_worldBounds.max.z);
		Vector3 br = new Vector3(para_worldBounds.max.x,para_worldBounds.min.y,para_worldBounds.max.z);

		Vector3 tl_guiPt = Camera.main.WorldToScreenPoint(tl);
		Vector3 br_guiPt = Camera.main.WorldToScreenPoint(br);

		Rect guiBounds = new Rect(tl_guiPt.x,Screen.height - tl_guiPt.y,br_guiPt.x - tl_guiPt.x,tl_guiPt.y - br_guiPt.y);
		return guiBounds;
	}

	public static Rect getWorldToGUIBounds(Rect para_worldBounds, float para_depthVal, bool[] para_upAxisArr)
	{
		Vector3 tl = new Vector3(para_worldBounds.x,para_worldBounds.y,para_depthVal);
		Vector3 br = new Vector3(para_worldBounds.x + para_worldBounds.width,para_worldBounds.y - para_worldBounds.height,para_depthVal);
		
		Vector3 tl_guiPt = Camera.main.WorldToScreenPoint(tl);
		Vector3 br_guiPt = Camera.main.WorldToScreenPoint(br);
		
		Rect guiBounds = new Rect(tl_guiPt.x,Screen.height - tl_guiPt.y,br_guiPt.x - tl_guiPt.x,tl_guiPt.y - br_guiPt.y);
		return guiBounds;
	}
	

	public static GameObject initObjAtGUIBounds(Transform para_prefab,
											    float para_prefabUnitWidth,
											    float para_prefabUnitHeight,
										        string para_name,
										        Rect para_guiBounds,
											    float[] para_rotAngles,
										        float para_depthCoord,
												bool[] para_upAxisArr)
	{
		Rect objWorldBounds = getGuiToWorldBounds(para_guiBounds,para_depthCoord,para_upAxisArr);
		return initObjWithinWorldBounds(para_prefab,para_prefabUnitWidth,para_prefabUnitHeight,para_name,objWorldBounds,para_rotAngles,para_depthCoord,para_upAxisArr);
	}


	public static GameObject initObjWithinWorldBounds(Transform para_prefab,
	                                                  string para_name,
	                                                  Bounds para_worldBounds,
	                                                  bool[] para_upAxisArr)
	{
		return initObjWithinWorldBounds(para_prefab,
		                                para_name,
		                                new Rect(para_worldBounds.center.x - (para_worldBounds.size.x/2f),
		         								 para_worldBounds.center.y + (para_worldBounds.size.y/2f),
		         							     para_worldBounds.size.x,
		         								 para_worldBounds.size.y),
		                                para_worldBounds.center.z,
		                                para_upAxisArr);
	}

	public static GameObject initObjWithinWorldBounds(Transform para_prefab,
	                                                  string para_name,
	                                                  Rect para_worldBounds,
	                                                  float para_depthCoord,
	                                                  bool[] para_upAxisArr)
	{
		return initObjWithinWorldBounds(para_prefab,
		                                para_prefab.renderer.bounds.size.x,
		                                para_prefab.renderer.bounds.size.y,
		                                para_name,
		                                para_worldBounds,
		                                null,
		                                para_depthCoord,
		                                para_upAxisArr);
	}

	public static GameObject initObjWithinWorldBounds(Transform para_prefab,
													  float para_prefabUnitWidth,
													  float para_prefabUnitHeight,
													  string para_name,
													  Rect para_worldBounds,
													  float[] para_rotAngles,
													  float para_depthCoord,
													  bool[] para_upAxisArr)
	{
		Transform nwObjTrans;
		
		if(para_upAxisArr[1])
		{
			nwObjTrans = (Transform) Instantiate(para_prefab,
											     new Vector3(para_worldBounds.x+(para_worldBounds.width/2f),
												   		     para_worldBounds.y-(para_worldBounds.height/2f),
														     para_depthCoord),
											     Quaternion.identity);
			
			nwObjTrans.localScale = new Vector3(para_worldBounds.width/para_prefabUnitWidth,para_worldBounds.height/para_prefabUnitHeight,1);
		}
		else
		{
			nwObjTrans = (Transform) Instantiate(para_prefab,
												 new Vector3(para_worldBounds.x+(para_worldBounds.width/2f),
														     para_depthCoord,
															 para_worldBounds.y-(para_worldBounds.height/2f)),
											     Quaternion.identity);
			
			nwObjTrans.localScale = new Vector3(para_worldBounds.width/para_prefabUnitWidth,1,para_worldBounds.height/para_prefabUnitHeight);
		}
		
		
		
		if((para_name != null)&&(para_name != ""))
		{
			nwObjTrans.name = para_name;
		}		
		
		if(para_rotAngles != null)
		{
			nwObjTrans.localEulerAngles = new Vector3(para_rotAngles[0],para_rotAngles[1],para_rotAngles[2]);
		}
		else
		{
			nwObjTrans.localEulerAngles = new Vector3(para_prefab.localEulerAngles[0],para_prefab.localEulerAngles[1],para_prefab.localEulerAngles[2]);	
		}
			
		return nwObjTrans.gameObject;
	}


	public static GameObject initWorldObjAndBlowupToScreen(Transform para_objPrefab, Rect para_originalObj2DWorldBounds)
	{
		return initWorldObjAndBlowupToScreen(para_objPrefab,para_originalObj2DWorldBounds,new Rect(0,0,1,1));
	}

	public static GameObject initWorldObjAndBlowupToScreen(Transform para_objPrefab, Rect para_originalObj2DWorldBounds, Rect para_normMaxScreenBounds)
	{
		Rect camera2DWorldBounds = getCameraViewWorldBounds(1,false);

		Rect maxConsideredWorldBounds = new Rect(0,0,camera2DWorldBounds.width,camera2DWorldBounds.height);
		maxConsideredWorldBounds.width *= para_normMaxScreenBounds.width;
		maxConsideredWorldBounds.height *= para_normMaxScreenBounds.height;


		float scaleFactor = Mathf.Min((maxConsideredWorldBounds.width/para_originalObj2DWorldBounds.width),
		                              				(maxConsideredWorldBounds.height/para_originalObj2DWorldBounds.height));

		Transform nwObj = (Transform) Instantiate(para_objPrefab,new Vector3(0,0,0),Quaternion.identity);
		Vector3 tmpScale = nwObj.localScale;
		tmpScale.x *= scaleFactor;
		tmpScale.y *= scaleFactor;
		tmpScale.z *= scaleFactor;
		nwObj.localScale = tmpScale;

		return nwObj.gameObject;
	}


	public static Vector3[] getCameraViewWorldBounds(float para_depthVal)
	{
		Vector3[] pts = new Vector3[4];

		Vector3 tl = Camera.main.ScreenToWorldPoint(new Vector3(0,Screen.height,para_depthVal));
		Vector3 tr = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,para_depthVal));
		Vector3 bl = Camera.main.ScreenToWorldPoint(new Vector3(0,0,para_depthVal));
		Vector3 br = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width,0,para_depthVal));



		pts[0] = tl;
		pts[1] = tr;
		pts[2] = bl;
		pts[3] = br;

		return pts;
	}

	public static Rect getCameraViewWorldBounds(float para_depthVal, bool para_dummyFlag)
	{
		Vector3[] pts = WorldSpawnHelper.getCameraViewWorldBounds(para_depthVal);

		Rect nwBounds = new Rect(pts[0].x,pts[0].y,pts[1].x - pts[0].x,pts[0].y - pts[3].y);
		return nwBounds;
	}
}

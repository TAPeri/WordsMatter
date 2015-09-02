/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class GridRenderer : MonoBehaviour
{

	public static void createGridRender(GridProperties para_gProp,
	                             		Transform para_planePrefab,
	                             		bool[] para_upAxisArr)
	{
		GridProperties worldGProp = para_gProp;
		Transform planePrefab = para_planePrefab;
		bool[] upAxisArr = para_upAxisArr;


		GameObject gridGObj = new GameObject("DropGrid");
		GameObject gridGObj_back = new GameObject("BackgroundGrid");
		GameObject gridGObj_frontBorder = new GameObject("FrontBorder");

		float depthVal_gridMajorOverlay = para_gProp.z;
		float depthVal_gridMinorOverlay = para_gProp.z - 0.5f;


		Texture2D majorGridLineTex = new Texture2D(1,1);
		majorGridLineTex.SetPixel(0,0,Color.black);
		majorGridLineTex.Apply();

		Texture2D minorGridLineTex = new Texture2D(1,1);
		minorGridLineTex.SetPixel(0,0,Color.gray);
		minorGridLineTex.Apply();



		//Instantiate(planePrefab,new Vector3(worldGProp.x,worldGProp.y,Camera.main.transform.position.z + depthVal_grid),Quaternion.identity);
		
		// Create minor grid lines.
		Rect tmpObjWorldBounds = new Rect(worldGProp.x,worldGProp.y,worldGProp.borderThickness,worldGProp.totalHeight);
		for(int c=0; c<(worldGProp.widthInCells+1); c++)
		{
			tmpObjWorldBounds.x = (worldGProp.x + (c * (worldGProp.cellWidth + worldGProp.borderThickness)));
			GameObject gridLineObj = WorldSpawnHelper.initObjWithinWorldBounds(planePrefab,1,1,"GridBorder_(x="+c+")",tmpObjWorldBounds,null,Camera.main.transform.position.z + depthVal_gridMinorOverlay,upAxisArr);
			gridLineObj.transform.renderer.material.mainTexture = minorGridLineTex;
			gridLineObj.transform.parent = gridGObj_back.transform;
		}
		
		tmpObjWorldBounds = new Rect(worldGProp.x,worldGProp.y,worldGProp.totalWidth,worldGProp.borderThickness);
		for(int r=0; r<(worldGProp.heightInCells+1); r++)
		{
			tmpObjWorldBounds.y = (worldGProp.y - (r * (worldGProp.cellHeight + worldGProp.borderThickness)));
			GameObject gridLineObj = WorldSpawnHelper.initObjWithinWorldBounds(planePrefab,1,1,"GridBorder_(y="+r+")",tmpObjWorldBounds,null,Camera.main.transform.position.z + depthVal_gridMinorOverlay,upAxisArr);
			gridLineObj.transform.renderer.material.mainTexture = minorGridLineTex; 
			gridLineObj.transform.parent = gridGObj_back.transform;
		}
		
		
		
		// Create major grid lines.
		List<Rect> majorGridLineBounds = new List<Rect>();
		majorGridLineBounds.Add( new Rect(worldGProp.x,worldGProp.y - worldGProp.totalHeight + worldGProp.borderThickness,worldGProp.totalWidth,worldGProp.borderThickness) );
		majorGridLineBounds.Add( new Rect(worldGProp.x,worldGProp.y,worldGProp.borderThickness,worldGProp.totalHeight) );
		majorGridLineBounds.Add( new Rect(worldGProp.x + worldGProp.totalWidth - worldGProp.borderThickness,worldGProp.y,worldGProp.borderThickness,worldGProp.totalHeight) );
		majorGridLineBounds.Add( new Rect(worldGProp.x,worldGProp.y,worldGProp.totalWidth,worldGProp.borderThickness) );
		
		for(int i=0; i<majorGridLineBounds.Count; i++)
		{
			GameObject tmpGridBorder = WorldSpawnHelper.initObjWithinWorldBounds(planePrefab,1,1,"GridBorderMain",majorGridLineBounds[i],null,Camera.main.transform.position.z + depthVal_gridMajorOverlay,upAxisArr);
			tmpGridBorder.transform.renderer.material.mainTexture = majorGridLineTex;
			tmpGridBorder.transform.parent = gridGObj_frontBorder.transform;
		}
 		
		gridGObj_back.transform.parent = gridGObj.transform;
		gridGObj_frontBorder.transform.parent = gridGObj.transform;
		
		
		
		gridGObj.SetActive(false);
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class DragNScroll : MonoBehaviour
{

	public bool isScrolling;
	public bool flipDir = true;
	public bool freezeX = false;
	public bool freezeY = false;

	Vector2 prevTouch;
	Vector2 currTouch;
	bool fingerDown;

	Vector3 fixedWorldPivotPt;
	Vector3 secondWorldPt;
	Vector3 tmpV3;

	int upAxisIndicator;

	List<Rect> guiScrollAreas;

	// Extent Variables.
	bool useCamExtents;
	Rect extentRect;



	void Start()
	{
		prevTouch = new Vector2();
		currTouch = new Vector2();
		tmpV3 = new Vector3();
		fingerDown = false;
	}


	void Update()
	{
		bool touchDetected = false;
		if(Input.touches.Length == 1)
		{
			currTouch = Input.touches[0].position;
			touchDetected = true;
		}
		else if(Input.GetMouseButton(0))
		{
			currTouch.x = Input.mousePosition.x;
			currTouch.y = Input.mousePosition.y;
			touchDetected = true;
		}
		
		
		// Selective touch.
		if((touchDetected)&&(guiScrollAreas != null)&&(guiScrollAreas.Count > 0))
		{
			bool foundValidArea = false;
			for(int i=0; i<guiScrollAreas.Count; i++)
			{
				Rect tmpArea = guiScrollAreas[i];
				if((currTouch.x >= tmpArea.x)&&(currTouch.x <= tmpArea.xMax)
				   &&(currTouch.y <= tmpArea.y)&&(currTouch.y >= (tmpArea.y - tmpArea.height)))
				{
					foundValidArea = true;
					break;
				}
			}
			
			if( ! foundValidArea)
			{
				touchDetected = false;
			}
		}
		
		
		if(touchDetected)
		{			
			if( ! fingerDown)
			{
				prevTouch = new Vector2(currTouch.x,currTouch.y);
				fingerDown = true;
			}
			else
			{
				fixedWorldPivotPt = Camera.main.ScreenToWorldPoint(new Vector3(prevTouch.x,prevTouch.y,2));

				secondWorldPt = Camera.main.ScreenToWorldPoint(new Vector3(currTouch.x,currTouch.y,2));
				Vector3 dirVect = secondWorldPt - fixedWorldPivotPt;
				if(flipDir)
				{
					dirVect.x *= -1;
					dirVect.y *= -1;
				}

				if(freezeX)	{ dirVect.x = 0; }
				if(freezeY)	{ dirVect.y = 0; }

				tmpV3 = (Camera.main.transform.position + dirVect);


				bool validMove = true;

				if(useCamExtents)
				{
					bool nwPosIsContainedInBounds = isPointWithinWorldExtents(tmpV3);
					if( ! nwPosIsContainedInBounds)
					{
						Vector3 tmpUpdatedDirVect = new Vector3(0,dirVect.y,0);
						tmpV3 = (Camera.main.transform.position + tmpUpdatedDirVect);
						nwPosIsContainedInBounds = isPointWithinWorldExtents(tmpV3);

						if( ! nwPosIsContainedInBounds)
						{
							tmpUpdatedDirVect.x = dirVect.x;
							tmpUpdatedDirVect.y = 0;
							tmpUpdatedDirVect.z = 0;
							tmpV3 = (Camera.main.transform.position + tmpUpdatedDirVect);
							nwPosIsContainedInBounds = isPointWithinWorldExtents(tmpV3);
						}
					}
					validMove = nwPosIsContainedInBounds;
				}

				if(validMove)
				{
					Camera.main.transform.position = tmpV3;// += dirVect;

					fixedWorldPivotPt.x = secondWorldPt.x;
					fixedWorldPivotPt.y = secondWorldPt.y;
					fixedWorldPivotPt.z = secondWorldPt.z;
					
					prevTouch.x = currTouch.x;
					prevTouch.y = currTouch.y;

					isScrolling = true;
				}
			}
		}
		else
		{
			fingerDown = false;
		}
	}



	
	// para_upAxisIndicator:  1=x-axis, 2=y-axis, 3=z-axis.
	// Also assumes that the camera viewport is smaller than the world bounds.
	public void setWorldExtents(Vector3 para_worldExtent_TL, Vector3 para_worldExtent_BR, int para_upAxisIndicator)
	{					
		if(para_upAxisIndicator == 2)
		{
			upAxisIndicator = 2;
			
			Vector3 viewport_TL = Camera.main.ViewportToWorldPoint(new Vector3(0,1,0));
			Vector3 viewport_BR = Camera.main.ViewportToWorldPoint(new Vector3(1,0,0));
			
			float viewport_WorldWidth = viewport_BR.x - viewport_TL.x;
			float viewport_WorldHeight = viewport_TL.z - viewport_BR.z;
			
			Vector3 extent_topLeft = new Vector3(para_worldExtent_TL.x + (viewport_WorldWidth/2f),Camera.main.transform.position.y,para_worldExtent_TL.z - (viewport_WorldHeight/2f));
			Vector3 extent_botRight = new Vector3(para_worldExtent_BR.x - (viewport_WorldWidth/2f),Camera.main.transform.position.y,para_worldExtent_BR.z + (viewport_WorldHeight/2f));
			
			extentRect = new Rect(extent_topLeft.x,extent_topLeft.z,extent_botRight.x - extent_topLeft.x,extent_topLeft.z - extent_botRight.z);
			
			useCamExtents = true;
		}
		else if(para_upAxisIndicator == 3)
		{
			upAxisIndicator = 3;
			
			Vector3 viewport_TL = Camera.main.ViewportToWorldPoint(new Vector3(0,1,0));
			Vector3 viewport_BR = Camera.main.ViewportToWorldPoint(new Vector3(1,0,0));
			
			float viewport_WorldWidth = viewport_BR.x - viewport_TL.x;
			float viewport_WorldHeight = viewport_TL.y - viewport_BR.y;
			
			Vector3 extent_topLeft = new Vector3(para_worldExtent_TL.x + (viewport_WorldWidth/2f),para_worldExtent_TL.y - (viewport_WorldHeight/2f),Camera.main.transform.position.z);
			Vector3 extent_botRight = new Vector3(para_worldExtent_BR.x - (viewport_WorldWidth/2f),para_worldExtent_BR.y + (viewport_WorldHeight/2f),Camera.main.transform.position.z);
			
			extentRect = new Rect(extent_topLeft.x,extent_topLeft.y,extent_botRight.x - extent_topLeft.x,extent_topLeft.y - extent_botRight.y);	
			
			useCamExtents = true;
		}
	}

	public bool isPointWithinWorldExtents(Vector3 para_nwCamPos)
	{
		bool nwPosIsContainedInBounds = true;
		Vector3 nwCamPos = para_nwCamPos;

		if(useCamExtents)
		{
			if(upAxisIndicator == 3)
			{
				bool clause1Flag = (nwCamPos.x >= extentRect.x);
				bool clause2Flag = (nwCamPos.x <= (extentRect.x + extentRect.width));
				bool clause3Flag = (nwCamPos.y <= extentRect.y);
				bool clause4Flag = (nwCamPos.y >= (extentRect.y - extentRect.height));
				nwPosIsContainedInBounds = (clause1Flag && clause2Flag && clause3Flag && clause4Flag);
			}
			
			if(upAxisIndicator == 2)
			{
				bool clause1Flag = (nwCamPos.x >= extentRect.x);
				bool clause2Flag = (nwCamPos.x <= (extentRect.x + extentRect.width));
				bool clause3Flag = (nwCamPos.z <= extentRect.y);
				bool clause4Flag = (nwCamPos.z >= (extentRect.y - extentRect.height));
				nwPosIsContainedInBounds = (clause1Flag && clause2Flag && clause3Flag && clause4Flag);
			}
		}

		return nwPosIsContainedInBounds;
	}

	public void addGUIScrollArea(Rect para_nwScrollArea)
	{
		// Note flip rect here since touch coordinates are origin based in bottom left rather than top left.
		if(guiScrollAreas == null) { guiScrollAreas = new List<Rect>(); }
		
		Rect tmpEfficientRect = new Rect(para_nwScrollArea.x,
		                                 Screen.height - para_nwScrollArea.y,
		                                 para_nwScrollArea.width,
		                                 para_nwScrollArea.height);
		
		guiScrollAreas.Add(tmpEfficientRect);
	}
}

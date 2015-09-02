/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ManualScrollCam : MonoBehaviour
{
	public Vector3 extent_topLeft;
	public Vector3 extent_botRight;
	bool useCamExtents;

	public float scroll_speed = 0.2f;

	public bool flipDir;
	public bool freezeX;
	public bool freezeZ;
	
	public bool isScrolling;
	
	Vector2 currTouch;
	Vector2 prevTouch;
	Vector3 firstWorldPt;
	Vector3 secondWorldPt;
	Vector3 tmpPt;

	
	Rect extentRect;
	
	int upAxisIndicator;

	List<Rect> guiScrollAreas;
	

	void Start()
	{
		currTouch = new Vector2(-1,-1);
		prevTouch = new Vector2(-1,-1);
		firstWorldPt = new Vector3(0,0,2);
		secondWorldPt = new Vector3(0,0,2);
		tmpPt = new Vector3(0,0,2);
		isScrolling = false;
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

			if((prevTouch.x == -1)&&(prevTouch.y == -1))
			{
				prevTouch = new Vector2(currTouch.x,currTouch.y);
			}	
			else
			{



				tmpPt.x = prevTouch.x;
				tmpPt.y = prevTouch.y;
				tmpPt.z = Camera.main.transform.position.z;
				firstWorldPt = Camera.main.ScreenToViewportPoint(tmpPt);
				tmpPt.x = currTouch.x;
				tmpPt.y = currTouch.y;
				secondWorldPt = Camera.main.ScreenToWorldPoint(tmpPt);



				//Vector3 dirVect = worldPt - Camera.main.transform.position;
				Vector3 dirVect = secondWorldPt - firstWorldPt;
				//float inputMag = dirVect.magnitude;
				
				if(flipDir)
				{
					dirVect.x *= -1;
				}
				dirVect.Normalize();
				
				
				
				if(freezeX)	{ dirVect.x = 0; }
				if(freezeZ)	{ dirVect.y = 0; }
				
				
				
				//dirVect = ((dirVect));





				Vector3 nwCamPos = Camera.main.transform.position + new Vector3(dirVect.x,dirVect.y,0);


				if(useCamExtents)
				{

					//if((nwCamPos.x < extentRect.x)||(nwCamPos.x > (extentRect.x + extentRect.width))) { nwCamPos.x = Camera.main.transform.position.x; }
					
					//if(extentRect.Contains(nwCamPos))
					bool nwPosIsContainedInBounds = false;
					
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
					
					if(nwPosIsContainedInBounds)
					{
						Camera.main.transform.position = nwCamPos;
						isScrolling = true;
					}

					if(upAxisIndicator == 2)
					{
						if(Camera.main.transform.position.z > extentRect.y)
						{
							Vector3 tmpPos = Camera.main.transform.position;
							tmpPos.z = extentRect.y;
							Camera.main.transform.position = tmpPos;
						}
						
						if(Camera.main.transform.position.z < (extentRect.y - extentRect.height))
						{
							Vector3 tmpPos = Camera.main.transform.position;
							tmpPos.z = (extentRect.y - extentRect.height);
							Camera.main.transform.position = tmpPos;
						}
					}

				}
				else
				{
					Camera.main.transform.position = nwCamPos;
					isScrolling = true;
				}

				prevTouch.x = currTouch.x;
				prevTouch.y = currTouch.y;
			}
			
		}
		else if(Input.touches.Length == 0)
		{
			currTouch.x = -1;
			currTouch.y = -1;
			prevTouch.x = -1;
			prevTouch.y = -1;
			isScrolling = false;
		}
	}
	
	public void initScrollCam(bool para_freezeX, bool para_freezeZ)
	{
		freezeX = para_freezeX;
		freezeZ = para_freezeZ;
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
			
			extent_topLeft = new Vector3(para_worldExtent_TL.x + (viewport_WorldWidth/2f),Camera.main.transform.position.y,para_worldExtent_TL.z - (viewport_WorldHeight/2f));
			extent_botRight = new Vector3(para_worldExtent_BR.x - (viewport_WorldWidth/2f),Camera.main.transform.position.y,para_worldExtent_BR.z + (viewport_WorldHeight/2f));
			
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
			
			extent_topLeft = new Vector3(para_worldExtent_TL.x + (viewport_WorldWidth/2f),para_worldExtent_TL.y - (viewport_WorldHeight/2f),Camera.main.transform.position.z);
			extent_botRight = new Vector3(para_worldExtent_BR.x - (viewport_WorldWidth/2f),para_worldExtent_BR.y + (viewport_WorldHeight/2f),Camera.main.transform.position.z);
			
			extentRect = new Rect(extent_topLeft.x,extent_topLeft.y,extent_botRight.x - extent_topLeft.x,extent_topLeft.y - extent_botRight.y);	
			
			useCamExtents = true;
		}
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
	
	public void setScrollSpeed(float para_speed)
	{
		scroll_speed = para_speed;
	}
}

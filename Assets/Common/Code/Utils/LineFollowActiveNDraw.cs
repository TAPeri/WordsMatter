/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class LineFollowActiveNDraw : MonoBehaviour, IActionNotifier
{
	//Rect areaOfEffect;
	Vector3 disp_startPos;
	Vector3 disp_endPos;
	Vector3 coll_startPos;
	Vector3 coll_endPos;
	
	bool penDown;
	bool sessionStart;
	bool inSession;
	
	LineRenderer localLRend;
	float lineThickness;
	//float disp_zCoord;
	float coll_zCoord;
	
	
	Vector3 movDir;
	float speed = 0.005f;
	
	int collisionLayerMask;
	
	
	
	HashSet<string> alreadyHitObjs;
	//int numOfParentLevelsToUniqueKey;
	
	
	bool isPaused;


	void Start()
	{
		penDown = true;
		sessionStart = true;
		inSession = false;
	}
	
	void Update()
	{
		
		if(!isPaused)
		{
			

					
			if(penDown)
			{
				Vector3 worldPt = transform.position;
				
				if(sessionStart)
				{
					localLRend.enabled = true;
					
					disp_startPos.x = worldPt.x;
					disp_startPos.y = worldPt.y;
					disp_startPos.z = worldPt.z;
					
					disp_endPos.x = worldPt.x;
					disp_endPos.y = worldPt.y;
					disp_endPos.z = worldPt.z;
					
					localLRend.SetPosition(0,disp_startPos);
					localLRend.SetPosition(1,disp_endPos);
					
					sessionStart = false;
					inSession = true;
				}
				else
				{
					disp_endPos.x = worldPt.x;
					disp_endPos.y = worldPt.y;
					disp_endPos.z = worldPt.z;
					localLRend.SetPosition(1,disp_endPos);
				}
				
				Vector3 tmpV = (disp_endPos - disp_startPos);
				movDir = Vector3.Normalize(disp_endPos-disp_startPos) * (tmpV.magnitude * speed);
				
				disp_startPos += movDir;
				localLRend.SetPosition(0,disp_startPos);
				
				
				// Actively and continuously check for slicing collisions while dragging occurs.
				coll_startPos.x = disp_startPos.x;
				coll_startPos.y = disp_startPos.y;
				coll_startPos.z = coll_zCoord;
				
				coll_endPos.x = disp_endPos.x;
				coll_endPos.y = disp_endPos.y;
				coll_endPos.z = coll_zCoord;
				
				
				if(tmpV.magnitude > 0)
				{
					RaycastHit hitInfo;
					Ray reqRay = new Ray(coll_startPos,Vector3.Normalize(coll_endPos-coll_startPos));
					if (Physics.Raycast(reqRay,out hitInfo,tmpV.magnitude,collisionLayerMask))
					{
						if(alreadyHitObjs == null)
						{
							alreadyHitObjs = new HashSet<string>();
						}
						
						string masterObjName = ((hitInfo.collider.gameObject.transform.parent).parent).name;
						string splitObjName = hitInfo.collider.gameObject.name;
						string parentNSplitObjCombo = masterObjName + "-" + splitObjName;
						
						if(! alreadyHitObjs.Contains(parentNSplitObjCombo))
						{
							string[] swipeLocInfo = new string[2] { masterObjName, splitObjName };
							notifyAllListeners("Swiper","SwipeHit",swipeLocInfo);
							
							alreadyHitObjs.Add(parentNSplitObjCombo);
						}
						
						
					}
				}
				
			}
			else
			{
				if(inSession)
				{
					localLRend.SetPosition(0,Vector3.zero);
					localLRend.SetPosition(1,Vector3.zero);
					
					inSession = false;
					triggerSwipeComplete();
				}
			}
			
		}
		
		
		
	}
	
	public void init(Rect para_guiAreaOfEffect,
	                 float para_lineThickness,
	                 float para_displayZCoord,
	                 float para_collisionZCoord,
	                 string para_collisionLayer,
	                 int para_numOfParentLayersToUniqueNameID)
	{
		//areaOfEffect = para_guiAreaOfEffect;
		lineThickness = para_lineThickness;
		//disp_zCoord = para_displayZCoord;
		coll_zCoord = para_collisionZCoord;
		collisionLayerMask = 1<<LayerMask.NameToLayer(para_collisionLayer);
		//numOfParentLevelsToUniqueKey = para_numOfParentLayersToUniqueNameID;
		
		disp_startPos = new Vector3(0,0,0);
		disp_endPos = new Vector3(0,0,0);
		coll_startPos = new Vector3(0,0,0);
		coll_endPos = new Vector3(0,0,0);
		
		
		localLRend = null;
		
		isPaused = false;
		
		reset();
	}
	
	
	public void reset()
	{
		disp_startPos.x = 0;
		disp_startPos.y = 0;
		disp_startPos.z = 0;
		
		disp_endPos.x = 0;
		disp_endPos.y = 0;
		disp_endPos.z = 0;
		
		coll_startPos.x = 0;
		coll_startPos.y = 0;
		coll_startPos.z = 0;
		
		coll_endPos.x = 0;
		coll_endPos.y = 0;
		coll_endPos.z = 0;
		
		penDown = true;
		sessionStart = true;
		inSession = false;
		
		
		localLRend = this.transform.gameObject.GetComponent<LineRenderer>();
		localLRend.material = new Material(Shader.Find("Transparent/Diffuse"));
		//localLRend.material.color = Color.white;
		localLRend.castShadows = false;
		localLRend.receiveShadows = false;
		localLRend.SetVertexCount(2);
		localLRend.SetWidth(lineThickness,lineThickness);
		//localLRend.SetColors(Color.white,Color.white);
		localLRend.material.mainTexture = Resources.Load<Texture2D>("Textures/Common/swipeTex");
		
		localLRend.SetPosition(0,Vector3.zero);
		localLRend.SetPosition(1,Vector3.zero);
		localLRend.enabled = false;
	}
	
	
	public void setPause(bool para_state)
	{
		isPaused = para_state;
		reset();
	}
	

	public void triggerSwipeComplete()
	{
		notifyAllListeners(transform.name,"SwipeComplete",null);
	}
	
	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
	
}
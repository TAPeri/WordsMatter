/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class LineDragActiveNDraw : MonoBehaviour, IActionNotifier
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
	float disp_zCoord;
	float coll_zCoord;


	Vector3 movDir;
	float speed = 0.2f;

	int collisionLayerMask;



	HashSet<string> alreadyHitObjs;
	//int numOfParentLevelsToUniqueKey;


	bool isPaused;
		
	
		
	void Update()
	{

		if(!isPaused)
		{

			Vector3 clickPos = Vector3.zero;
			
			if(Application.platform == RuntimePlatform.Android)
			{
				// Running on Android.
				// Use Touch and adjust coordinate system.
				
				
				
				if(Input.touches.Length == 1)
				{
					foreach(Touch t in Input.touches)
					{
						clickPos = new Vector3(t.position.x,
						                       t.position.y,
						                       0);
					}
				}
				
				
				if((!penDown)&&(Input.touches.Length > 0))
				{
					penDown = true;
					sessionStart = true;
				}
				else
				{
					// Detect if player has let go of the touch.
					if(Input.touches.Length == 0)
					{
						penDown = false;
						sessionStart = false;
					}
				}
			}
			else
			{
				// Running on PC.
				// Use Mouse Clicks.
				
				if(Input.GetMouseButton(0))
				{
					clickPos = Input.mousePosition;
					
					if(!penDown)
					{
						sessionStart = true;
					}
					
					penDown = true;
				}
				else
				{
					penDown = false;
					sessionStart = false;
				}
			}
			
			
			if(penDown)
			{
				clickPos.z = Mathf.Abs(Camera.main.transform.position.z - disp_zCoord);
				Vector3 worldPt = Camera.main.ScreenToWorldPoint(clickPos);
				
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
							notifyAllListeners(transform.name,"SwipeHit",swipeLocInfo);

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
					notifyAllListeners(transform.name,"SwipeComplete",null);
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
		disp_zCoord = para_displayZCoord;
		coll_zCoord = para_collisionZCoord;
		collisionLayerMask = 1<<LayerMask.NameToLayer(para_collisionLayer);
		//numOfParentLevelsToUniqueKey = para_numOfParentLayersToUniqueNameID;

		disp_startPos = new Vector3(0,0,0);
		disp_endPos = new Vector3(0,0,0);
		coll_startPos = new Vector3(0,0,0);
		coll_endPos = new Vector3(0,0,0);

		transform.renderer.sortingOrder = 9999;
		//transform.renderer.material.color = Color.red;
		
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

		penDown = false;
		sessionStart = false;
		inSession = false;
		
		
		localLRend = this.transform.gameObject.GetComponent<LineRenderer>();
		localLRend.material = new Material(Shader.Find("Transparent/Diffuse"));
		localLRend.material.color = ColorUtils.convertColor(125,0,255);
		localLRend.castShadows = false;
		localLRend.receiveShadows = false;
		localLRend.SetVertexCount(2);
		localLRend.SetWidth(lineThickness,lineThickness);
		//localLRend.SetColors(Color.red,Color.red);
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

	
	
	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}

}
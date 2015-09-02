/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class LineDragNDraw : MonoBehaviour, IActionNotifier
{
	//Rect areaOfEffect;
	Vector3 startPos;
	Vector3 endPos;

	bool penDown;
	bool sessionStart;
	bool inSession;

	LineRenderer localLRend;
	float lineThickness;
	float zCoord;

	



	void OnGUI()
	{
		GUI.color = Color.black;
		GUI.Label(new Rect(0,0,200,100),"In Session: "+inSession);
		GUI.color = Color.white;
	}

	void Update()
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
			clickPos.z = zCoord;
			Vector3 worldPt = Camera.main.ScreenToWorldPoint(clickPos);

			if(sessionStart)
			{
				localLRend.enabled = true;

				startPos.x = worldPt.x;
				startPos.y = worldPt.y;
				startPos.z = worldPt.z;

				endPos.x = worldPt.x;
				endPos.y = worldPt.y;
				endPos.z = worldPt.z;

				localLRend.SetPosition(0,startPos);
				localLRend.SetPosition(1,endPos);

				sessionStart = false;
				inSession = true;
			}
			else
			{
				endPos.x = worldPt.x;
				endPos.y = worldPt.y;
				endPos.z = worldPt.z;
				localLRend.SetPosition(1,endPos);
			}
		}
		else
		{
			if(inSession)
			{
				localLRend.SetPosition(0,Vector3.zero);
				localLRend.SetPosition(1,Vector3.zero);

				List<float[]> linePts = new List<float[]>();
				linePts.Add(new float[3] { startPos.x, startPos.y, startPos.z });
				linePts.Add(new float[3] { endPos.x, endPos.y, endPos.z });

				notifyAllListeners("LineDragNDraw","LineCompleted",linePts);

				inSession = false;
			}
		}


	}

	public void init(Rect para_guiAreaOfEffect, float para_lineThickness, float para_zCoord)
	{
		//areaOfEffect = para_guiAreaOfEffect;
		lineThickness = para_lineThickness;
		zCoord = para_zCoord;

		startPos = new Vector3(0,0,0);
		endPos = new Vector3(0,0,0);


		localLRend = null;

		reset();
	}


	public void reset()
	{
		startPos.x = 0;
		startPos.y = 0;
		startPos.z = 0;

		endPos.x = 0;
		endPos.y = 0;
		endPos.z = 0;

		penDown = false;
		sessionStart = false;
		inSession = false;


		localLRend = this.transform.gameObject.GetComponent<LineRenderer>();
		localLRend.material = new Material(Shader.Find("Diffuse"));
		localLRend.material.color = Color.black;
		localLRend.castShadows = false;
		localLRend.receiveShadows = false;
		localLRend.SetVertexCount(2);
		localLRend.SetWidth(lineThickness,lineThickness);
		localLRend.SetColors(Color.black,Color.black);

		localLRend.SetPosition(0,Vector3.zero);
		localLRend.SetPosition(1,Vector3.zero);
		localLRend.enabled = false;
	}

	


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}

}
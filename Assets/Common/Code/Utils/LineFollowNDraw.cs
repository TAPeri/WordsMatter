/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class LineFollowNDraw : MonoBehaviour, IActionNotifier
{
	//Rect areaOfEffect;
	Vector3 startPos;
	Vector3 endPos;

	GameObject objToFollow;

	LineRenderer localLRend;
	float lineThickness;
	float zCoord;

	bool inSession;





	void Update()
	{
		if(inSession)
		{
			if(objToFollow != null)
			{
				endPos.x = objToFollow.transform.position.x;
				endPos.y = objToFollow.transform.position.y;
				endPos.z = zCoord;

				localLRend.SetPosition(1,endPos);
			}
			else
			{
				localLRend.SetPosition(0,Vector3.zero);
				localLRend.SetPosition(1,Vector3.zero);
				
				List<float[]> linePts = new List<float[]>();
				linePts.Add(new float[3] { startPos.x, startPos.y, startPos.z });
				linePts.Add(new float[3] { endPos.x, endPos.y, endPos.z });
				
				notifyAllListeners("LineFollowNDraw","LineCompleted",linePts);
				
				inSession = false;
			}
		}
	}

	public void init(Rect para_areaOfEffect, float para_lineThickness, float para_zCoord, GameObject para_objToFollow)
	{
		if(para_objToFollow != null)
		{
			//areaOfEffect = para_areaOfEffect;
			lineThickness = para_lineThickness;
			zCoord = para_zCoord;
			objToFollow = para_objToFollow;

			startPos = new Vector3(para_objToFollow.transform.position.x,
			                       para_objToFollow.transform.position.y,
			                       zCoord);
			endPos = new Vector3(startPos.x,startPos.y,startPos.z);


			localLRend = this.transform.gameObject.GetComponent<LineRenderer>();
			localLRend.material = new Material(Shader.Find("Diffuse"));
			localLRend.material.color = Color.black;
			localLRend.castShadows = false;
			localLRend.receiveShadows = false;
			localLRend.SetVertexCount(2);
			localLRend.SetWidth(lineThickness,lineThickness);
			localLRend.SetColors(Color.black,Color.black);
			
			localLRend.SetPosition(0,startPos);
			localLRend.SetPosition(1,endPos);
			localLRend.enabled = false;

			inSession = true;
		}
	}

	public void haltSession()
	{
		objToFollow = null;
	}




	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}

}

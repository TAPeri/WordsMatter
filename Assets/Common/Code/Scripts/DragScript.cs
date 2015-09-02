/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class DragScript : MonoBehaviour, IActionNotifier
{

	Vector2 clickPos;
	Vector3 tmpVect;
	bool fingerDown;
	
	bool isDragging;
	GameObject currDragObj;
	Dictionary<string,int> potentialOwnerCounterLookup;

	Camera reqCam;

	bool takeInput;


	void Start()
	{
		clickPos = new Vector2(-1,-1);
		tmpVect = new Vector3(-1,-1,-1);
		fingerDown = false;
		isDragging = false;
		currDragObj = null;
		potentialOwnerCounterLookup = new Dictionary<string, int>();
		if(reqCam == null) { reqCam = Camera.main; }
		takeInput = true;
	}
	
	
	void Update()
	{

		if(takeInput)
		{

			bool clickDetected = false;
			if((Application.platform == RuntimePlatform.Android)
			   ||(Application.platform == RuntimePlatform.IPhonePlayer))
			{
				if(Input.touches.Length == 1)
				{
					clickPos.x = Input.touches[0].position.x;
					clickPos.y = Input.touches[0].position.y;
					clickDetected = true;
				}
			}
			else
			{
				if(Input.GetMouseButton(0))
				{
					clickPos.x = Input.mousePosition.x;
					clickPos.y = Input.mousePosition.y;
					clickDetected = true;
				}
			}
			
			
			if(clickDetected)
			{
				tmpVect.x = clickPos.x;
				tmpVect.y = clickPos.y;
				tmpVect.z = 0;


				if(!fingerDown)
				{


					RaycastHit hitInf;
					if(Physics.Raycast(reqCam.ScreenPointToRay(tmpVect),out hitInf))
					{
						if(hitInf.collider.gameObject.layer == LayerMask.NameToLayer("Draggable"))
						{
							isDragging = true;
							currDragObj = hitInf.collider.gameObject;

							notifyAllListeners(currDragObj.name,"DragStart",null);
							//Debug.Log("Drag Start");
						}
					}
					
					fingerDown = true;
				}
				else
				{

					if(isDragging)
					{
						if(currDragObj == null)
						{
							isDragging = false;
						}
						else
						{	
							Vector3 nwDragObjPos = reqCam.ScreenToWorldPoint(tmpVect);
							nwDragObjPos.z = currDragObj.transform.position.z;

							Rigidbody rb = null;
							if((rb = currDragObj.transform.GetComponent<Rigidbody>()) != null)
								rb.transform.Translate(nwDragObjPos - currDragObj.transform.position,Space.World);
							currDragObj.transform.position = nwDragObjPos;

							//Debug.Log("Is Dragging");
						}
					}
				}
			}
			else
			{

				if(isDragging)
				{						
					notifyAllListeners(currDragObj.name,"DragRelease",null);
					//Debug.Log("Drag Released");

					isDragging = false;
					currDragObj = null;
				}			

				fingerDown = false;
			}
		}



	}




	public void incrementDragObjOwnerAmount(string para_dragObjName)
	{
		if(potentialOwnerCounterLookup == null) { potentialOwnerCounterLookup = new Dictionary<string, int>(); }
		if( ! potentialOwnerCounterLookup.ContainsKey(para_dragObjName)) { potentialOwnerCounterLookup.Add(para_dragObjName,0); }
		potentialOwnerCounterLookup[para_dragObjName] = potentialOwnerCounterLookup[para_dragObjName] + 1;

		//Debug.Log("Drag Owner Counter for '"+para_dragObjName+"' = "+potentialOwnerCounterLookup[para_dragObjName]);
	}

	public void decrementDragObjOwnerAmount(string para_dragObjName)
	{
		if(potentialOwnerCounterLookup == null) { potentialOwnerCounterLookup = new Dictionary<string, int>(); }
		if( ! potentialOwnerCounterLookup.ContainsKey(para_dragObjName)) { potentialOwnerCounterLookup.Add(para_dragObjName,0); }
		potentialOwnerCounterLookup[para_dragObjName] = potentialOwnerCounterLookup[para_dragObjName] - 1;
		if(potentialOwnerCounterLookup[para_dragObjName] < 0) { potentialOwnerCounterLookup[para_dragObjName] = 0; }

		//Debug.Log("Drag Owner Counter for '"+para_dragObjName+"' = "+potentialOwnerCounterLookup[para_dragObjName]);
	}

	public void clearAllOwnerCounterForObj(string para_dragObjName)
	{
		if((potentialOwnerCounterLookup != null)&&(potentialOwnerCounterLookup.ContainsKey(para_dragObjName)))
		{
			potentialOwnerCounterLookup.Remove(para_dragObjName);
		}
	}

	public void resetAllOwnerCounters()
	{
		if(potentialOwnerCounterLookup != null)
		{
			potentialOwnerCounterLookup.Clear();
		}
	}

	public int getNumPotentialOwnersForDragObj(string para_dragObjName)
	{
		if(potentialOwnerCounterLookup == null)
		{
			return 0;
		}
		else
		{
			if( ! potentialOwnerCounterLookup.ContainsKey(para_dragObjName))
			{
				return 0;
			}
			else
			{
				int retCounter = potentialOwnerCounterLookup[para_dragObjName];
				//Debug.Log("Counter for '"+para_dragObjName+"' is "+retCounter);
				return retCounter;
			}
		}
	}

	public void setInputFlag(bool para_state)
	{
		takeInput = para_state;
	}

	public void setReqCamera(Camera para_camScript)
	{
		reqCam = para_camScript;
		if(reqCam == null) { reqCam = Camera.main; }
	}

	public string getNameOfDragObj()
	{
		string retName = null;
		if(currDragObj != null)
		{
			retName = currDragObj.name;
		}
		return retName;
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

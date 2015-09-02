/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class GhostDragScript : MonoBehaviour, IActionNotifier
{
	
	Vector2 clickPos;
	Vector3 tmpVect;
	bool fingerDown;
	
	bool isDragging;
	GameObject currDragObj;
	int potentialOwnerCounter;
	
	bool takeInput;


	public float ghostZVal;


	//GameObject ghostObj;
	string relatedGhostName;

	
	void Start()
	{
		clickPos = new Vector2(-1,-1);
		tmpVect = new Vector3(-1,-1,-1);
		fingerDown = false;
		isDragging = false;
		currDragObj = null;
		//ghostObj = null;
		potentialOwnerCounter = 0;
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
					if(Physics.Raycast(Camera.main.ScreenPointToRay(tmpVect),out hitInf))
					{
						if(hitInf.collider.gameObject.layer == LayerMask.NameToLayer("Draggable"))
						{
							isDragging = true;
							currDragObj = createGhost(hitInf.collider.gameObject);
							
							potentialOwnerCounter = 0;
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
							Vector3 nwDragObjPos = Camera.main.ScreenToWorldPoint(tmpVect);
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
					System.Object[] retData = new System.Object[2];
					retData[0] = relatedGhostName;
					retData[1] = new Bounds(currDragObj.renderer.bounds.center,currDragObj.renderer.bounds.size);

					notifyAllListeners(currDragObj.name,"DragRelease",retData);
					//Debug.Log("Drag Released");

					Destroy(currDragObj);
					
					isDragging = false;
					currDragObj = null;
				}			
				
				fingerDown = false;
			}
		}
		
		
		
	}
	
	
	public void incrementDragObjOwnerAmount()
	{
		potentialOwnerCounter++;
	}
	
	public void decrementDragObjOwnerAmount()
	{
		potentialOwnerCounter--;
	}
	
	public int getNumPotentialOwnersForDragObj()
	{
		return potentialOwnerCounter;
	}
	
	public void setInputFlag(bool para_state)
	{
		takeInput = para_state;
	}
	

	private GameObject createGhost(GameObject para_obj)
	{
		relatedGhostName = para_obj.name;

		Transform nwGhostObj = (Transform) Instantiate(para_obj.transform,new Vector3(para_obj.transform.position.x,para_obj.transform.position.y,ghostZVal),para_obj.transform.rotation);
		nwGhostObj.name = "GhostDragObj";
		Destroy(nwGhostObj.GetComponent<BoxCollider>());
		Destroy(nwGhostObj.GetComponent<Rigidbody>());
		SpriteRenderer sRend = nwGhostObj.GetComponent<SpriteRenderer>();
		sRend.sortingOrder = 200;
		sRend.color = Color.grey;

		return nwGhostObj.gameObject;
	}

	
	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

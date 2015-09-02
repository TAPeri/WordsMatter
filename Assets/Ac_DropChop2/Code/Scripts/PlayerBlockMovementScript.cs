/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class PlayerBlockMovementScript : MonoBehaviour
{
	
	public Transform sfxPrefab;
 	
	AcDropChopScenarioV2 scenScriptRef;
	
	Vector2 clickPos;
	Vector3 tmpClickPosV3;
	bool fingerDown;
	
	bool objDragOccurring;
	GameObject currObjBeingDragged;
	
	
	int chosenDirID;
	
	
	
		
	void Start()
	{
		//scenScriptRef = (AcDropChopScenario) this.GetComponent(typeof(AcDropChopScenario));	
		
		clickPos = new Vector2(-1,-1);
		tmpClickPosV3 = new Vector3(-1,-1,-1);
		objDragOccurring = false;
		currObjBeingDragged = null;
	}
	
	/*void OnGUI()
	{
		GUI.Label(new Rect(0,0,100,100),"Drag Axis: "+chosenDirID);	
	}*/
	
	
	void Update()
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
			if(!fingerDown)
			{
				RaycastHit hitInf;
				tmpClickPosV3.x = clickPos.x;
				tmpClickPosV3.y = clickPos.y;
				tmpClickPosV3.z = 0;
				if(Physics.Raycast(Camera.main.ScreenPointToRay(tmpClickPosV3),out hitInf))
				{
					//Debug.Log("Hit: "+hitInf.collider.gameObject.name);
					if(hitInf.collider.gameObject.tag == "Indivisible")
					{
						objDragOccurring = true;
						currObjBeingDragged = hitInf.collider.gameObject;
						triggerSoundAtCamera("sfx_ObjectGrab");
						scenScriptRef.intakeObjectDragStart();
					}
				}
			
				fingerDown = true;
			}
			else
			{
				if(objDragOccurring)
				{
					if(currObjBeingDragged == null)
					{
						objDragOccurring = false;
					}
					else
					{	
						tmpClickPosV3.x = clickPos.x;
						tmpClickPosV3.y = clickPos.y;
						tmpClickPosV3.z = Mathf.Abs(Camera.main.transform.position.z - currObjBeingDragged.transform.position.z);
						
						Vector3 wPoint = Camera.main.ScreenToWorldPoint(tmpClickPosV3);
						
						Vector2 v1 = new Vector2(currObjBeingDragged.transform.position.x,currObjBeingDragged.transform.position.y);
						Vector2 v2 = new Vector2(wPoint.x,wPoint.y);
						
						Vector2 dragVect = v2 - v1;
						
						
						if(dragVect.magnitude > currObjBeingDragged.transform.renderer.bounds.size.y)
						{
						
							Vector2 northVect = new Vector2(0,1);
							
							float angle = Vector2.Angle(northVect,dragVect);
							
							
							if(dragVect.x < 0)
							{
								angle = 360 - angle;
							}
							
							
							int numOfDirs = 4;
							float anglePerDir = 360f/(numOfDirs * 1.0f);
							
							chosenDirID = -1;
							
							float lowerBound = (360 - (anglePerDir/2f));
							float upperBound = (anglePerDir/2f);
							
							
							if((angle <= (anglePerDir/2f))
							||(angle > (360 - (anglePerDir/2f))))
							{
								chosenDirID = 0;	
							}
							
							if(chosenDirID == -1)
							{
								for(int k=0; k<numOfDirs; k++)
								{
									if((angle > lowerBound)
									&&(angle <= upperBound))
									{
										chosenDirID = k;	
									}
									
									
									if((lowerBound + anglePerDir) <= 360)
									{
										lowerBound = (lowerBound + anglePerDir);	
									}
									else
									{
										lowerBound = (lowerBound + anglePerDir) - 360;
									}
									
									if((upperBound + anglePerDir) <= 360)
									{
										upperBound = (upperBound + anglePerDir);	
									}
									else
									{
										upperBound = (upperBound + anglePerDir) - 360;	
									}
								}
							}
							
							
							if(chosenDirID != -1)
							{
								scenScriptRef.intakeObjectDragCommand(ref currObjBeingDragged,chosenDirID);	
							}
							
							
							
							
							
							/*tmpClickPosV3.x = clickPos.x;
							tmpClickPosV3.y = clickPos.y;
							tmpClickPosV3.z = 3;
							Vector3 nwDragObjPos = Camera.main.ScreenToWorldPoint(tmpClickPosV3);
							
							currObjBeingDragged.transform.position = nwDragObjPos;
							
							Debug.Log("Is Dragging");*/
						}
						
					}
				}
			}
		}
		else
		{
			// Notify that drag has stopped.
			if(currObjBeingDragged != null)
			{
				triggerSoundAtCamera("sfx_ObjectRelease");
				scenScriptRef.intakeObjectDragStop(currObjBeingDragged.name);
			}
			
			// Reset			
			fingerDown = false;	
			objDragOccurring = false;
			currObjBeingDragged = null;
		}
	}

	public void init()
	{
		scenScriptRef = transform.GetComponent<AcDropChopScenarioV2>();
	}
	
	
	private void triggerSoundAtCamera(string para_soundFileName)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = 0.5f;
		audS.Play();
	}
}

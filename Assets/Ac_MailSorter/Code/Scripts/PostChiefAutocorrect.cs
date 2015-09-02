/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class PostChiefAutocorrect : MonoBehaviour, CustomActionListener, IActionNotifier
{
	
	Vector3 finishPos;
	float chiefWalkSpeed = 4f;
	//float totalTimeForChiefSequence_Sec = 5f;
	int state = 0;
	AcMailSorterScenario scenario;
	GameObject chiefObj;
	GameObject reqObjToPickup;
	GameObject dropLocationObject;
	public void init(GameObject para_reqObjToPickup,
	                 GameObject para_dropLocationObject,
	                 Vector3 para_postChiefSpawnPt,
	                 Vector3 para_postChiefDestPt,
	                 Transform para_postChiefPrefab,
	                 AcMailSorterScenario para_scenario)
	{

		scenario = para_scenario;
		reqObjToPickup = para_reqObjToPickup;
		finishPos = para_postChiefDestPt;
		dropLocationObject = para_dropLocationObject;
		
		Transform nwChief = (Transform) Instantiate(para_postChiefPrefab,para_postChiefSpawnPt,Quaternion.identity);

		if(para_postChiefSpawnPt.x<para_postChiefDestPt.x)

			nwChief.Rotate (Vector3.up * 180);

		CommonUnityUtils.setSortingLayerOfEntireObject(nwChief.gameObject,"Pass_1");
		
		
		Vector3 pickupPosForChief = new Vector3(para_reqObjToPickup.transform.position.x + (3f * 0.2f),//(para_postChiefPrefab.renderer.bounds.size.x * 0.2f),
		                                        para_postChiefSpawnPt.y,
		                                        para_postChiefSpawnPt.z);
		
		
		
		
		MoveToLocation mtl = nwChief.gameObject.AddComponent<MoveToLocation>();
		mtl.registerListener("PostChiefAutocorrect",this);
		mtl.initScript(pickupPosForChief,chiefWalkSpeed);
		
		Animator characterAni = nwChief.gameObject.GetComponent<Animator>();
		characterAni.Play("BigSideWalkL");
		
		chiefObj = nwChief.gameObject;
	}
	
	
	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "MoveToLocation")
		{
			if(state == 0)
			{
				state++;
				
				string fixtureName = reqObjToPickup.GetComponent<SpriteRenderer>().sprite.name;
				Vector3 fixturePos = chiefObj.transform.FindChild(fixtureName).position;
				
				reqObjToPickup.transform.parent = null;
				Vector3 tmpObjPos = reqObjToPickup.transform.position;
				tmpObjPos.x = fixturePos.x;
				tmpObjPos.y = fixturePos.y;
				tmpObjPos.z = chiefObj.transform.position.z - 0.1f;
				reqObjToPickup.transform.position = tmpObjPos;
				reqObjToPickup.transform.parent = chiefObj.transform;
				
				
				SpriteRenderer sRend = reqObjToPickup.GetComponent<SpriteRenderer>();
				if(sRend != null)
				{
					sRend.sortingOrder = 500;
				}
				
				chiefObj.GetComponent<Animator>().Play("BigParcelPickupL");


				Vector3 dropPosForChief = new Vector3(dropLocationObject.transform.position.x + (3f * 0.2f),//(para_postChiefPrefab.renderer.bounds.size.x * 0.2f),
				                                        finishPos.y,
				                                        finishPos.z);
				
				MoveToLocation mtl = chiefObj.gameObject.AddComponent<MoveToLocation>();
				mtl.registerListener("PostChiefAutocorrect",this);
				mtl.initScript(dropPosForChief,chiefWalkSpeed);
				//mtl.initScript(finishPos,totalTimeForChiefSequence_Sec/2f,false);
			}else if(state == 1){

				state++;


				scenario.dropAutocorrect(reqObjToPickup, dropLocationObject);

				
				MoveToLocation mtl = chiefObj.gameObject.AddComponent<MoveToLocation>();
				mtl.registerListener("PostChiefAutocorrect",this);
				mtl.initScript(finishPos,chiefWalkSpeed);


			}else
			{
				//notifyAllListeners("ChiefScript","ChiefLeft",reqObjToPickup.name);
				Destroy(chiefObj);
				Destroy(this);
			}
		}
	}
	
	
	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

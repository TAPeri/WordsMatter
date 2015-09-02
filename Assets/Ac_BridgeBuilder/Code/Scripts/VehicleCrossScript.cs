/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class VehicleCrossScript : MonoBehaviour, CustomActionListener, IActionNotifier
{

	Rect maxGameArea;
	Vector3 vehicleEndPos;
	Transform vehicleRearColliderPrefab;
	float letterTileWidth;
	int state = 0;

	float truckSpeed = 7f;

	public void init(Rect para_maxGameArea, Vector3 para_vehicleEndPos, Transform para_vehicleRearColliderPrefab, float para_letterTileWidth)
	{
		maxGameArea = para_maxGameArea;
		vehicleEndPos = para_vehicleEndPos;
		vehicleRearColliderPrefab = para_vehicleRearColliderPrefab;
		letterTileWidth = para_letterTileWidth;

		Debug.Log("Move!");


		Rect camWorld2DBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,false);
		Vector3 currCamPos = Camera.main.transform.position;
		Vector3 camMoveToLoc = new Vector3(currCamPos.x,currCamPos.y,currCamPos.z);
		camMoveToLoc.x = (maxGameArea.x + camWorld2DBounds.width/2f);
		
		
		if(Mathf.Abs(currCamPos.x - camMoveToLoc.x) != 0)
		{

			TeleportToLocation ttl = Camera.main.transform.gameObject.AddComponent<TeleportToLocation>();
			ttl.registerListener("VehicleCrossScript",this);
			ttl.init(camMoveToLoc);
		}
		else
		{
			this.respondToEvent(Camera.main.name,"MoveToLocation",null);
		}


		// STEP 1: Move camera to vehicle location. (Cam is bounded to game area).
		
		//Rect camWorld2DBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,false);
		//Vector3 currCamPos = Camera.main.transform.position;
		//Vector3 camMoveToLoc = new Vector3(currCamPos.x,currCamPos.y,currCamPos.z);
		//camMoveToLoc.x = (maxGameArea.x + camWorld2DBounds.width/2f);
		
		//MoveToLocation mtl = Camera.main.transform.gameObject.AddComponent<MoveToLocation>();
		//mtl.registerListener("VehicleCrossScript",this);
		//mtl.initScript(camMoveToLoc,5f);//5f

		//this.respondToEvent("","MoveToLocation",null);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{

		Debug.Log(para_sourceID+" "+para_eventID);

		if(para_eventID=="TeleportToLocation"){
			this.respondToEvent("","MoveToLocation",null);

		}


		if(para_eventID == "MoveToLocation")
		{
			if(state == 0)
			{
				state++;

				GameObject vehicleObj = GameObject.Find("Vehicle");
				GameObject vehicleBody = vehicleObj.transform.FindChild("VehicleBody").gameObject;

				// STEP 2: Attach collider to vehicle rear.

				Vector3 colliderPos = new Vector3(vehicleObj.transform.position.x - (vehicleBody.renderer.bounds.size.x/2f) - (letterTileWidth/2f),
				                                  vehicleObj.transform.position.y - (vehicleBody.renderer.bounds.size.y/2f),
				                                  vehicleObj.transform.position.z);

				Transform nwCollider = (Transform) Instantiate(vehicleRearColliderPrefab,colliderPos,Quaternion.identity);
				nwCollider.name = "VehCol";
				nwCollider.parent = vehicleObj.transform;


				// STEP 3: Trigger Vehicle movement.

				BoundedCamFollow bcf = Camera.main.gameObject.AddComponent<BoundedCamFollow>();
				bcf.init(vehicleObj,new bool[3] { true, false, false }, maxGameArea);

				vehicleObj.GetComponent<Animator>().Play("VehicleMoving");

				MoveToLocation mtl = vehicleObj.AddComponent<MoveToLocation>();
				mtl.registerListener("VehicleCrossScript",this);
				mtl.initScript(vehicleEndPos,truckSpeed);

				Debug.Log("moving!");
			}
			else
			{
				// Vehicle has crossed over. Destroy the vehicle and report back to the main script.
				GameObject.Find("Vehicle").GetComponent<Animator>().Play("VehicleIdle");
				Destroy(Camera.main.gameObject.GetComponent<BoundedCamFollow>());
				Destroy(GameObject.Find("Vehicle").transform.FindChild("VehCol").gameObject);
				notifyAllListeners("CrossingVehicle","VehicleCrossed",null);
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

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class NewVehicleEnterScript : MonoBehaviour, CustomActionListener, IActionNotifier
{

	public void init(Transform para_vehiclePrefab, int para_currBridgeID)
	{
		DestroyImmediate(GameObject.Find("Vehicle"));

		GameObject leftCliff = GameObject.Find("CliffLeft-"+para_currBridgeID);
		Rect leftCliffBounds = CommonUnityUtils.get2DBounds(leftCliff.renderer.bounds);

		Transform vehBodyChild = para_vehiclePrefab.FindChild("VehicleBody");
		Transform vehWheelBackChild = para_vehiclePrefab.FindChild("WheelBack");

		Vector3 vehicleSpawnPt = new Vector3(leftCliffBounds.x - (vehBodyChild.renderer.bounds.size.x/2f),
		                                     leftCliffBounds.y + ((vehBodyChild.renderer.bounds.size.y + (vehWheelBackChild.renderer.bounds.size.y * 0.7f))/2f),
		                                     -0.1f);

		Transform nwVehicle = (Transform) Instantiate(para_vehiclePrefab,vehicleSpawnPt,Quaternion.identity);
		nwVehicle.name = "Vehicle";

		List<string> availableDriverNames = new List<string>() { "SecChar-0","SecChar-1","SecChar-2" };
		int currIndex = Random.Range(0,availableDriverNames.Count);

		Transform bodyChild = nwVehicle.FindChild("VehicleBody");
		Transform reqDriverPrefab = Resources.Load<Transform>("Prefabs/Avatars/"+availableDriverNames[currIndex]);
		Transform nwDriver = (Transform) Instantiate(reqDriverPrefab,nwVehicle.FindChild("DriverMarker").position,Quaternion.identity);
		nwDriver.parent = bodyChild;
		
		nwDriver.FindChild("Torso").FindChild("ShadowL").renderer.enabled = false;
		nwDriver.FindChild("Torso").FindChild("ShadowR").renderer.enabled = false;
		
		CommonUnityUtils.setSortingOrderOfEntireObject(nwDriver.gameObject,1);
		
		nwDriver.GetComponent<Animator>().Play("Idle_R");



		Vector3 destLocation = vehicleSpawnPt + new Vector3(vehBodyChild.renderer.bounds.size.x/2f,0,0);

		MoveToLocation mtl = nwVehicle.gameObject.AddComponent<MoveToLocation>();
		mtl.registerListener("NewVehicleScript",this);
		mtl.initScript(destLocation,2f);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "MoveToLocation")
		{
			notifyAllListeners("NewVehicleEnterScript","NewVehicleDone",null);
			Destroy(this);
		}
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

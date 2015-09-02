/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class CarcadeScript : MonoBehaviour, CustomActionListener, IActionNotifier
{
	float carSpeed = 5f;
	public void init(GameObject para_carLeader)
	{
		Transform vehBodyChild = para_carLeader.transform.FindChild("VehicleBody");




		Vector3 carcadeCentreDest = new Vector3(vehBodyChild.position.x - (vehBodyChild.renderer.bounds.size.x/2f) - (transform.renderer.bounds.size.x/2f),transform.position.y,transform.position.z);

		for(int i=0; i<transform.childCount; i++)
		{
			Transform tmpChild = transform.GetChild(i);
			tmpChild.GetComponent<Animator>().Play("VehicleMoving");
		}

		MoveToLocation mtl = transform.gameObject.AddComponent<MoveToLocation>();
		mtl.registerListener("CarcadeScript",this);
		mtl.initScript(carcadeCentreDest,carSpeed);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "MoveToLocation")
		{
			for(int i=0; i<transform.childCount; i++)
			{
				Transform tmpChild = transform.GetChild(i);
				tmpChild.GetComponent<Animator>().Play("VehicleIdle");
			}

			notifyAllListeners("CarcadeScript","CarcadeDone",null);
			Destroy(this);
		}
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

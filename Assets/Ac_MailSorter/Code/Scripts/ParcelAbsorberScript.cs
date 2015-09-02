/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class ParcelAbsorberScript : MonoBehaviour, IActionNotifier
{
	//DispenserScript linkToDispenser;

	void Start()
	{
		//linkToDispenser = GameObject.Find("Dispenser").GetComponent<DispenserScript>();
	}

	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.name.Contains("Parcel"))
		{
			GameObject tmpParcelObj = collision.gameObject;

			string reqName = tmpParcelObj.name;

			notifyAllListeners("Absorber","ItemAbsorbed",reqName);

			Destroy(tmpParcelObj);


			/*Destroy(tmpParcelObj.collider);
			Destroy(tmpParcelObj.rigidbody);
			SpriteRenderer sRend = tmpParcelObj.GetComponent<SpriteRenderer>();
			if(sRend != null) { sRend.color = Color.white; }
			tmpParcelObj.layer = LayerMask.NameToLayer("Default");
			tmpParcelObj.transform.rotation = Quaternion.identity;*/
		}
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

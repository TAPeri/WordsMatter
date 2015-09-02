/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class BasketBaseColliderScript : MonoBehaviour, IActionNotifier
{
	bool isLocked;

	void OnCollisionEnter(Collision collision)
	{
		if( ! isLocked)
		{
			if(collision.gameObject.name.Contains("DroppedP"))
			{
				isLocked = true;
				notifyAllListeners("BasketBase","BasketReceivedItem",null);
				Destroy(this.gameObject);
			}
		}
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

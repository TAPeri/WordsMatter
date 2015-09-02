/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class PDPlayerScript : MonoBehaviour
{

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.name.Contains("Package"))
		{
			GameObject.Find("GlobObj").GetComponent<AcPackageDeliveryScenario>().pickupPackage();
			Destroy(collider.gameObject);
		}
	}
}

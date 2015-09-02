/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class DetectorPlateScript : MonoBehaviour
{
	void Start()
	{
		renderer.enabled = false;
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.layer == LayerMask.NameToLayer("Draggable"))
		{
			AcHarvestScenario scenScript = GameObject.Find("GlobObj").GetComponent<AcHarvestScenario>();
			scenScript.receiveHoveringMachineName(transform.parent.name,true);
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if(collider.gameObject.layer == LayerMask.NameToLayer("Draggable"))
		{
			AcHarvestScenario scenScript = GameObject.Find("GlobObj").GetComponent<AcHarvestScenario>();
			scenScript.receiveHoveringMachineName(transform.parent.name,false);
		}
	}
}

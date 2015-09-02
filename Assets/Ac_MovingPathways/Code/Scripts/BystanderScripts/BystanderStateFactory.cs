/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class BystanderStateFactory : MonoBehaviour
{
	protected string stateFactoryName = "DefaultName";
	protected Dictionary<string,GameObject> registeredBystanders;
	protected Dictionary<string,int> bystanderToNodeID;

	protected GameObject spawnNewBystanderInWorld(string para_bystanderName,
	                                              Transform para_bystanderPrefab,
	                                              Vector3 para_spawnPt,
	                                              int para_nodeID)
	{
		Transform nwBystander = (Transform) Instantiate(para_bystanderPrefab,para_spawnPt,Quaternion.identity);
		nwBystander.name = para_bystanderName;
		CommonUnityUtils.setSortingLayerOfEntireObject(nwBystander.gameObject,"Pass_"+Random.Range(1,9));

		registerNewBystander(para_bystanderName,nwBystander.gameObject,para_nodeID);
		return nwBystander.gameObject;
	}

	protected void registerNewBystander(string para_bystanderName,
	                                    GameObject para_bystander,
	                                    int para_nodeID)
	{
		if(registeredBystanders == null)
		{
			registeredBystanders = new Dictionary<string, GameObject>();
			bystanderToNodeID = new Dictionary<string, int>();
		}

		if(registeredBystanders.ContainsKey(para_bystanderName))
		{
			registeredBystanders[para_bystanderName] = para_bystander;
			bystanderToNodeID[para_bystanderName] = para_nodeID;
		}
		else
		{
			registeredBystanders.Add(para_bystanderName,para_bystander);
			bystanderToNodeID.Add(para_bystanderName,para_nodeID);
		}
	}

	protected void unregisterBystander(string para_bystanderName)
	{
		GameObject reqBystanderObj = null;
		if((registeredBystanders != null)&&(registeredBystanders.ContainsKey(para_bystanderName)))
		{
			reqBystanderObj = registeredBystanders[para_bystanderName];
			NewCharacterNavMovement cnm = reqBystanderObj.GetComponent<NewCharacterNavMovement>();
			if(cnm != null) { cnm.unregisterListener(stateFactoryName); }
			Destroy(reqBystanderObj.GetComponent<MoveToLocation>());
			registeredBystanders.Remove(para_bystanderName);
			bystanderToNodeID.Remove(para_bystanderName);
		}
	}
}

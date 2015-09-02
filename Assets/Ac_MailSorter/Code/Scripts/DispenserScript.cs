/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class DispenserScript : MonoBehaviour
{
	List<Transform> queuedPrefabs;
	List<string> queuedObjNames;
	GameObject latestReleasedObj;
	bool canReleaseFlag;
	bool autoDispenseOn;
	bool goAheadGiven;

	void Start()
	{
		queuedPrefabs = new List<Transform>();
		queuedObjNames = new List<string>();
		latestReleasedObj = null;
		canReleaseFlag = false;
		autoDispenseOn = false;
		goAheadGiven = false;
	}

	void Update()
	{

		//Parcels are moved by the ConveyorScript class, which detects collition between parcel and conveyor game objects
		if(queuedPrefabs.Count > 0)
		{
			canReleaseFlag = false;

			if(autoDispenseOn)
			{
				if(latestReleasedObj == null)
				{
					canReleaseFlag = true;
				}
				else
				{
					float maxDistanceX = transform.position.x + (latestReleasedObj.transform.renderer.bounds.size.x);
					if(latestReleasedObj.transform.position.x >= maxDistanceX)
					{
						canReleaseFlag = true;
					}
				}
			}
			else
			{
				if(goAheadGiven)
				{
					canReleaseFlag = true;
					goAheadGiven = false;
				}
			}


			if(canReleaseFlag)
			{
				releaseNextItem();
				canReleaseFlag = false;
			}
		}
	}

	public void queueItem(Transform para_prefab, string para_name)
	{
		queuedPrefabs.Add(para_prefab);
		queuedObjNames.Add(para_name);
	}

	public void giveReleaseGoAhead()
	{
		goAheadGiven = true;
	}

	public void reset()
	{
		queuedPrefabs.Clear();
		queuedObjNames.Clear();
		latestReleasedObj = null;
		canReleaseFlag = false;
		autoDispenseOn = false;
		goAheadGiven = false;
	}

	private void releaseNextItem()
	{
		if(queuedPrefabs.Count > 0)
		{
			Transform nwItem = (Transform) Instantiate(queuedPrefabs[0],transform.position,Quaternion.identity);
			nwItem.name = queuedObjNames[0];
			latestReleasedObj = nwItem.gameObject;

			queuedPrefabs.RemoveAt(0);
			queuedObjNames.RemoveAt(0);
		}
	}
}

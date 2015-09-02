/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class BystanderCreationData
{
	GameObject nwBystander;
	int spawnNodeID;

	public BystanderCreationData(GameObject para_nwBystander, int para_spawnNodeID)
	{
		nwBystander = para_nwBystander;
		spawnNodeID = para_spawnNodeID;
	}

	public GameObject getBystander() { return nwBystander; }
	public int getSpawnNodeID() { return spawnNodeID; }
}

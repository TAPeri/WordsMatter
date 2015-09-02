/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;

public class DropoffLocationData
{
	public Vector3 worldPt;
	public int[] cellCoords;

	public DropoffLocationData(Vector3 para_worldPt,
	                           int[] para_cellCoords)
	{
		worldPt = para_worldPt;
		cellCoords = para_cellCoords;
	}
}

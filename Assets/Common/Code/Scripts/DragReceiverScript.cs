/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class DragReceiverScript : MonoBehaviour
{
	public GameObject gObjReceived;



	public void receiveDraggedGObj(GameObject para_gObjReleased)
	{
		gObjReceived = para_gObjReleased;
	}
}

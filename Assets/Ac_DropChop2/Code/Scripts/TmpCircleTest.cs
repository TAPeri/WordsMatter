/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class TmpCircleTest : MonoBehaviour
{


	void Start()
	{
		CircleFadeScript cfs = GameObject.Find("DimScreen").AddComponent<CircleFadeScript>();
		cfs.init(3,false);
	}
}

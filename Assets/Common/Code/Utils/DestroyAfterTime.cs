/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour
{
	public float delaySec;
	
	float startTime;
	
	void Start ()
	{
		startTime = Time.time;
	}
	
	
	void Update ()
	{
		if((Time.time - startTime) >= delaySec)
		{
			Destroy (this.gameObject);
		}
	}
}
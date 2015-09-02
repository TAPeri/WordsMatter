/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class WitherNDestroy : MonoBehaviour, CustomActionListener
{

	void Start()
	{
		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] {0,0,0,0}, 1f }));
		batchLists.Add(batch1);
		aniMang.registerListener("WitherNDestroy",this);
		aniMang.init("WitherAni",batchLists);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "WitherAni")
		{
			Destroy(this.gameObject);
		}
	}
}

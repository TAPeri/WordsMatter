/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class TriggerChildEnable : AbsCustomAniCommand
{
	string childName;
	bool reqEnableState;

	void Update()
	{
		Transform reqChild = transform.FindChild(childName);
		if(reqChild != null)
		{
			reqChild.gameObject.SetActive(reqEnableState);
		}
		notifyAllListeners(transform.name,"TriggerChildEnable",null);
		Destroy(this);
	}

	public void init(string para_childName, bool para_reqEnableState)
	{
		childName = para_childName;
		reqEnableState = para_reqEnableState;
	}

	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		string p_childName = (string) para_prep.parameters[0];
		bool p_reqEnableState = (bool) para_prep.parameters[1];
		this.init(p_childName,p_reqEnableState);
		return true;
	}
}

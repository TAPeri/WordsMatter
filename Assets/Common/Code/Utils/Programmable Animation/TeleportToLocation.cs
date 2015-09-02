/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class TeleportToLocation : AbsCustomAniCommand
{
	bool isDone;
	Vector3 destination;
	
	void Update()
	{
		if(!isDone)
		{
			transform.position = destination;
			isDone = true;
		}
		else
		{
			notifyAllListeners(transform.name,"TeleportToLocation",null);
			Destroy(this);
		}
	}

	public void init(Vector3 para_destination)
	{
		isDone = false;
		destination = para_destination;
	}


	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		float[] p_destinationPt = (float[]) para_prep.parameters[0];
		this.init(new Vector3(p_destinationPt[0],p_destinationPt[1],p_destinationPt[2]));
		return true;
	}

}

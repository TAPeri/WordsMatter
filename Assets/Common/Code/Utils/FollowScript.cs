/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class FollowScript : MonoBehaviour
{
	GameObject obToFollow;
	bool[] followAxes;
	Vector3 nwPos;


	void Update()
	{
		if(followAxes[0]) { nwPos.x = obToFollow.transform.position.x; }
		if(followAxes[1]) { nwPos.y = obToFollow.transform.position.y; }
		if(followAxes[2]) { nwPos.z = obToFollow.transform.position.z; }

		transform.position = nwPos;
	}

	public void init(GameObject para_objToFollow, bool[] para_followAxes)
	{
		obToFollow = para_objToFollow;
		followAxes = para_followAxes;
		nwPos = new Vector3(transform.position.x,transform.position.y,transform.position.z);
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class HomeInOnTarget : AbsCustomAniCommand
{

	GameObject targetGObj;
	float speed;
	float increment;
	Vector3 movVect;


	void Update()
	{
		increment = (speed * Time.deltaTime);
		
		if(Vector3.Distance(transform.position,targetGObj.transform.position) > increment)
		{
			movVect = ((Vector3.Normalize(targetGObj.transform.position - transform.position)) * speed);
			transform.position += (movVect * (Time.deltaTime));
		}
		else
		{
			performExit();
		}
	}


	public void initScript(string para_targetObjName, float para_distPerSec)
	{
		targetGObj = GameObject.Find(para_targetObjName);
		speed = para_distPerSec;
	}


	
	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		string p_targetObjName = (string) para_prep.parameters[0];
		float p_distPerSec = (float) para_prep.parameters[1];
		this.initScript(p_targetObjName,p_distPerSec);

		return true;
	}


	private void performExit()
	{
		transform.position = targetGObj.transform.position;
		notifyAllListeners(transform.name,"HomeInOnTarget",null);
		Destroy(this);
	}


}

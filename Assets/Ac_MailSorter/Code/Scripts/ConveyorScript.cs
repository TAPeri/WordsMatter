/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ConveyorScript : MonoBehaviour
{
	Dictionary<string,GameObject> conveyorObjects;
	Vector3 conveyorMovVect;
	bool conveyorOn;
	Vector3 normConveyorDirection;


	void Start()
	{
		conveyorObjects = new Dictionary<string,GameObject>();
	}
	

	void Update()
	{
		if(conveyorOn)
		{
			Vector3 changeVal = conveyorMovVect * Time.deltaTime;

			foreach(KeyValuePair<string,GameObject> pair in conveyorObjects)
			{
				if(pair.Value != null)
				{
					pair.Value.transform.position += changeVal;
				}
			}
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if(collision.collider.name.Contains("Parcel"))
		{
			GameObject colGObj = collision.collider.gameObject;
			if(colGObj != null)
			{
				if( ! conveyorObjects.ContainsKey(colGObj.name))
				{

					conveyorObjects.Add(colGObj.name,colGObj);
				}
			}
		}
	}

	void OnCollisionExit(Collision collision)
	{
		if(collision.collider.name.Contains("Parcel"))
		{
			GameObject colGObj = collision.collider.gameObject;
			if(colGObj != null)
			{
				if(conveyorObjects.ContainsKey(colGObj.name))
				{
					conveyorObjects.Remove(colGObj.name);
				}
			}
		}
	}


	public void init(Vector3 para_normConveyorDirection, float para_conveyorSpeed)
	{
		normConveyorDirection = para_normConveyorDirection;
		conveyorMovVect = (para_normConveyorDirection * para_conveyorSpeed);
		conveyorOn = true;
	}

	public void setNewSpeed(float speed){
		conveyorMovVect = (normConveyorDirection * speed);

	}


	public void toggleConveyorOnState()
	{
		conveyorOn = !conveyorOn;
		Animator aniScript = transform.gameObject.GetComponent<Animator>();
		if(aniScript != null)
		{
			if(conveyorOn) { aniScript.speed = 1; } else { aniScript.speed = 0; }
		}
	}

	public void setOnState(bool para_onState)
	{
		conveyorOn = para_onState;
		Animator aniScript = transform.gameObject.GetComponent<Animator>();
		if(aniScript != null)
		{
			if(conveyorOn) { aniScript.speed = 1; } else { aniScript.speed = 0; }
		}
	}

	public void detachObject(string para_gObjName)
	{
		if(conveyorObjects.ContainsKey(para_gObjName))
		{
			conveyorObjects.Remove(para_gObjName);
		}
	}
}

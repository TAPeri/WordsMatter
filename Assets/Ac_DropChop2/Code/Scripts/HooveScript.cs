/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class HooveScript : MonoBehaviour
{
	
	bool isInPosition;
	bool isHooving;
	bool isReturningToRest;
	bool isDone;
	
	Vector3 hooveRestPos;
	Vector3 hooveSpot;
	Vector3 hooveMovVect;
	Vector3 hooveSuctionVect;
	float endHooveX;
	GameObject hooveRowCollection;
	

	Transform sfxPrefab;
	bool spawnedClimbingSound;
	string tmpClimbSFXObjName;
	
	void Update()
	{
		if( ! isDone)
		{
			if(! isInPosition)
			{
				if((Vector3.Distance(transform.position,hooveSpot) <= (hooveMovVect.magnitude * 0.05f))
				||(transform.position.y > hooveSpot.y))
				{
					transform.position = hooveSpot;	
					isInPosition = true;
					isHooving = true;


					Destroy(GameObject.Find(tmpClimbSFXObjName));
					spawnedClimbingSound = false;
					//triggerSoundAtCamera("sfx_SolomonArrive",null,true);
					triggerSoundAtCamera("sfx_SolomonHoovering",null,true);

					GameObject solomonParent = GameObject.Find("SolomonWhole");
					Animator solomonAni = solomonParent.GetComponent<Animator>();
					solomonAni.Play("sol_vacuuming");
				}
				else
				{	
					if(!spawnedClimbingSound)
					{
						tmpClimbSFXObjName = triggerSoundAtCamera("sfx_SolomonClimb","sfx_SolomonClimb",false,0.4f);
						spawnedClimbingSound = true;
					}

					transform.position += (hooveMovVect * Time.deltaTime);
				}
			}
			else if(isHooving)
			{
				if(hooveRowCollection.transform.position.x < endHooveX)
				{
					Destroy(hooveRowCollection);
					isHooving = false;
					isReturningToRest = true;

					GameObject solomonParent = GameObject.Find("SolomonWhole");
					Animator solomonAni = solomonParent.GetComponent<Animator>();
					solomonAni.Play("sol_idle");
				}
				else
				{
					hooveRowCollection.transform.position += (hooveSuctionVect * Time.deltaTime);
				}
			}
			else if(isReturningToRest)
			{
				if((Vector3.Distance(transform.position,hooveRestPos) <= (hooveMovVect.magnitude * 0.05f))
				||(transform.position.y < hooveRestPos.y))
				{
					transform.position = hooveRestPos;
					isReturningToRest = false;
					isDone = true;
					
					// Report done to scenario script.
					AcDropChopScenarioV2 scenScript = (AcDropChopScenarioV2) GameObject.Find("GlobObj").GetComponent(typeof(AcDropChopScenarioV2));
					scenScript.noticeHooveTermination();
				}
				else
				{
					transform.position += ((-hooveMovVect) * Time.deltaTime);
				}
			}
		}
	}
	
	
	public void triggerHoove(Vector3 para_hooveSpot,
										 Vector3 para_hooveMovVectWithSpeed,
										 Vector3 para_hooveSuctionVectWithSpeed,
										 float para_endHooveX, GameObject para_hooveRowCollection,
	                         Transform para_sfxPrefab)
	{
		isInPosition = false;
		isHooving = false;
		isReturningToRest = false;
		isDone = false;
		
		hooveRestPos = transform.position;
		hooveSpot = para_hooveSpot;
		hooveMovVect = para_hooveMovVectWithSpeed;
		hooveSuctionVect = para_hooveSuctionVectWithSpeed;
		endHooveX = para_endHooveX;
		hooveRowCollection = para_hooveRowCollection;

		spawnedClimbingSound = false;
		sfxPrefab = para_sfxPrefab;
	}


	private string triggerSoundAtCamera(string para_soundFileName, string para_customObjName, bool para_autoDestroy)
	{
		return triggerSoundAtCamera(para_soundFileName,para_customObjName,para_autoDestroy,0.5f);
	}

	private string triggerSoundAtCamera(string para_soundFileName, string para_customObjName, bool para_autoDestroy, float para_volume)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		if((para_customObjName != null)&&(para_customObjName != ""))
		{
			nwSFX.name = para_customObjName;
		}
		DontDestroyOnLoad(nwSFX);
		if(!para_autoDestroy)
		{
			DestroyAfterTime datScript = nwSFX.GetComponent<DestroyAfterTime>();
			if(datScript != null)
			{
				Destroy(datScript);
			}
		}
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = para_volume;
		audS.Play();

		return nwSFX.name;
	}
}

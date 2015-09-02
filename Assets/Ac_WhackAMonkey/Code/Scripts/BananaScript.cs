/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class BananaScript : MonoBehaviour, CustomActionListener,IActionNotifier
{



	public void init(Vector3 para_startPos,
	                 Vector3 para_targetPos,
	                 float para_speedToTarget,
	                 Rect para_targetBounds)
	{
		Vector3 tmpEAngles = transform.localEulerAngles;
		tmpEAngles.z = Random.Range(0,360);
		transform.localEulerAngles = tmpEAngles;

		triggerSoundAtCamera("throw");

		//Spin spinScript = transform.gameObject.AddComponent<Spin>();
		//spinScript.init(1f);
		Animator tmpBAni = transform.GetComponent<Animator>();
		tmpBAni.Play("Spin");


		Rect currentBounds = CommonUnityUtils.get2DBounds(transform.renderer.bounds);
		Vector3 destScale = new Vector3(para_targetBounds.width/currentBounds.width,para_targetBounds.height/currentBounds.height,1);


		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",2, new List<System.Object>() { new float[3] {para_targetPos.x,para_targetPos.y,para_targetPos.z}, para_speedToTarget, false }));
		//batch1.Add(new AniCommandPrep("Spin",2, new List<System.Object>() { 1f }));
		batch1.Add(new AniCommandPrep("GrowProportionateToDistance",1, new List<System.Object>() { new float[3] {para_targetPos.x,para_targetPos.y,para_targetPos.z}, new float[3] {destScale.x,destScale.y,destScale.z} }));

		batchList.Add(batch1);
		aniMang.registerListener("BScript",this);
		aniMang.init("BananaThrow",batchList);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "BananaThrow")
		{
			// The banana has hit the back wall. Simply play the delete animation.
			triggerDeath();
			notifyAllListeners(transform.name,"BananaHitWall",null);

		}
		else if(para_eventID == "WitherAnimation")
		{
			Destroy(transform.gameObject);
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		//Debug.Log("Banna hit: "+collider.gameObject.name);

		bool performWither = true;

		if(collider.gameObject.name.Contains("Monkey"))
		{
			if(collider.gameObject.GetComponent<MonkeyScript>().isGoodMonkey)
			{
				performWither = false;
				Destroy(transform.gameObject);
			}
		}

		if(performWither)
		{
			Destroy(transform.GetComponent<Spin>());
			CustomAnimationManager caMang = transform.gameObject.GetComponent<CustomAnimationManager>();
			caMang.forceHalt();

			triggerDeath();
		}
	}	

	private void triggerDeath()
	{
		Destroy(transform.GetComponent<Spin>());

		transform.GetComponent<Animator>().speed = 0.05f;

		triggerSoundAtCamera("splat");

		CustomAnimationManager caMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("TriggerAnimation",1,new List<System.Object>() { "Splat" }));
		batchList.Add(batch1);
		caMang.registerListener("BScript",this);
		caMang.init("WitherAnimation",batchList);

		/*CustomAnimationManager caMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] {1,1,1,0}, 1f }));
		batchList.Add(batch1);
		caMang.registerListener("BScript",this);
		caMang.init("WitherAnimation",batchList);*/
	}

	private void triggerSoundAtCamera(string para_soundFileName)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		Transform  sfxPrefab = Resources.Load<Transform>("Prefabs/SFxBox");
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = 1f;
		audS.Play();
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}

}

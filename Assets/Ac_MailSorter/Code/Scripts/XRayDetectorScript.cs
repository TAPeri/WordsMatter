/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class XRayDetectorScript : MonoBehaviour, CustomActionListener
{
	public Transform wordBoxPrefab;
	GameObject nxtItem;

	Rect xRayDisplay_WorldBounds;
	float xRayDisplay_zVal;


	GameObject displayWordBox;

	float scanningTime_Sec = 5f;
	bool isScanning;

	bool playTtsSound = false;



	public void setScanningTime(float time){

		scanningTime_Sec = time;
	}

	void Start()
	{
		nxtItem = null;
		GameObject xRayDisplayAreaGObj = transform.parent.FindChild("XRayDisplayArea").gameObject;

		xRayDisplay_WorldBounds = new Rect(xRayDisplayAreaGObj.renderer.bounds.center.x - (xRayDisplayAreaGObj.renderer.bounds.size.x/2f),
				                            xRayDisplayAreaGObj.renderer.bounds.center.y + (xRayDisplayAreaGObj.renderer.bounds.size.y/2f),
				                            xRayDisplayAreaGObj.renderer.bounds.size.x,
				                            xRayDisplayAreaGObj.renderer.bounds.size.y);

		xRayDisplay_zVal = xRayDisplayAreaGObj.transform.position.z;

		xRayDisplayAreaGObj.renderer.enabled = false;

		isScanning = false;
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.name.Contains("Parcel"))
		{
			//Debug.Log("XRay detected: "+collider.name);
			nxtItem = collider.gameObject;
		}
	}

	void OnTriggerStay(Collider collider)
	{
		if( ! isScanning)
		{
			if(nxtItem != null)
			{
				if(collider.gameObject.name == nxtItem.name)
				{
					if(nxtItem.transform.position.x >= transform.position.x)
					{
						GameObject mainConveyorObj = GameObject.Find("ConveyerRight").gameObject;
						ConveyorScript cs1 = mainConveyorObj.GetComponent<ConveyorScript>();
						cs1.setOnState(false);

						Vector3 tmpPos = nxtItem.transform.position;
						tmpPos.x = transform.position.x;
						nxtItem.transform.position = tmpPos;

						this.scanItemWithXRay();
					}
				}
			}
		}
	}

	public void setTtsOn(bool para_useTts)
	{
		playTtsSound = para_useTts;
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{

		if(para_eventID == "PreScanEffect")
		{
			showScannedWord();
		}
		else if(para_eventID == "XRayScanning")
		{
			Destroy(displayWordBox);


			//SpriteRenderer sRend = nxtItem.GetComponent<SpriteRenderer>();
			/*if(sRend != null)
			{
				sRend.color = new Color(0.301f,0.301f,1);
			}*/
			nxtItem.layer = LayerMask.NameToLayer("Draggable");


			nxtItem = null;

			GameObject mainConveyorObj = GameObject.Find("ConveyerRight").gameObject;
			ConveyorScript cs1 = mainConveyorObj.GetComponent<ConveyorScript>();
			cs1.setOnState(true);

			DispenserScript ds = GameObject.Find("Dispenser").GetComponent<DispenserScript>();
			ds.giveReleaseGoAhead();

			isScanning = false;
		}

	}

	private void scanItemWithXRay()
	{
		isScanning = true;

		performPreScanEffect();
	}

	private void performPreScanEffect()
	{
		transform.parent.GetComponent<Animator>().Play("Scanning");

		CustomAnimationManager aniMang = transform.parent.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>() { 0.3f }));
		batchLists.Add(batch1);
		aniMang.registerListener("XRayDetectorScript",this);
		aniMang.init("PreScanEffect",batchLists);
	}

	private void showScannedWord()
	{
		transform.parent.GetComponent<Animator>().Play("XRayOff");

		AcMailSorterScenario acScen = GameObject.Find("GlobObj").GetComponent<AcMailSorterScenario>();
		string reqWord = acScen.getWordAssociatedWithParcelID(int.Parse(nxtItem.name.Split('-')[1]));
		
		UnityEngine.AudioClip sound = null;
		if(playTtsSound)
		{
			sound = WorldViewServerCommunication.tts.say(reqWord);

			try
			{
				if(gameObject.GetComponent<AudioSource>() == null) { gameObject.AddComponent<AudioSource>(); }
				audio.PlayOneShot(sound);
			}
			catch(System.Exception ex)
			{
				Debug.LogError("Failed to use TTS. "+ex.Message);
			}

		}

		if (sound!=null){
			displayWordBox = WordBuilderHelper.buildWordBoxWithSound(99,reqWord,xRayDisplay_WorldBounds,xRayDisplay_zVal,new bool[3] { false,true,false },wordBoxPrefab,sound ,true);
			
		}else{
			displayWordBox = WordBuilderHelper.buildWordBox(99,reqWord,xRayDisplay_WorldBounds,xRayDisplay_zVal,new bool[3] { false,true,false },wordBoxPrefab);
		}
		
		CustomAnimationManager caMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> cmdBatchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> cmdBatch1 = new List<AniCommandPrep>();
		cmdBatch1.Add(new AniCommandPrep("DelayForInterval",1, new List<System.Object>() { scanningTime_Sec }));
		cmdBatchList.Add(cmdBatch1);
		caMang.registerListener("XRay",this);
		caMang.init("XRayScanning",cmdBatchList);

		acScen.respondToEvent("XRay","XRayScannedWord",reqWord+"-"+int.Parse(nxtItem.name.Split('-')[1]));
	}
}

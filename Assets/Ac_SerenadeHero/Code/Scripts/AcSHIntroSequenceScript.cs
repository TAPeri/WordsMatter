/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class AcSHIntroSequenceScript : MonoBehaviour, CustomActionListener
{

	// 1. Song banner
	// 2. Sentence appear
	// 3. Music marker appear
	// 4. Violin usher.
	// 5. Word pool popup.
	// 6. 1,2,3 drum tone.
	// 7. Go.

	Vector3 songBannerDestPos;
	Vector3 firstViolinistDestPos;
	GameObject violinist1;
	GameObject serenader;
	Transform serenaderParticleSystem;

	List<GameObject> wordBlocks;
	int nxtBlockPopIndex = 0;


	void Start()
	{
		// Move song banner out of screen.
		GameObject wordScrollObj = GameObject.Find("WordScroll");
		Transform wordScrollLeftObj = wordScrollObj.transform.FindChild("WordScroll_Left");
		Rect cameraWorldBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,true);

		GameObject musicMarkerObj = GameObject.Find("MusicMarker");
		musicMarkerObj.transform.parent = wordScrollObj.transform;
		
		songBannerDestPos = new Vector3(wordScrollObj.transform.position.x,wordScrollObj.transform.position.y,wordScrollObj.transform.position.z);
		Vector3 tmpSongScrollPos = wordScrollObj.transform.position;
		tmpSongScrollPos.y = cameraWorldBounds.y + wordScrollLeftObj.renderer.bounds.size.y;
		wordScrollObj.transform.position = tmpSongScrollPos;

		// Hide first violinist.
		violinist1 = GameObject.Find("Ser_Viol1");
		firstViolinistDestPos = new Vector3(violinist1.transform.position.x,violinist1.transform.position.y,violinist1.transform.position.z);
		Vector3 tmpViolPos = violinist1.transform.position;
		tmpViolPos.x = cameraWorldBounds.x - 5;
		violinist1.transform.position = tmpViolPos;
		violinist1.SetActive(false);

		// Freeze serenader.
		serenader = GameObject.Find("Ser_Main");
		serenader.GetComponent<Animator>().enabled = false;
		serenaderParticleSystem = serenader.transform.FindChild("NoteParticleSytem");
		serenaderParticleSystem.gameObject.SetActive(false);
	}
	

	void Update()
	{
	
	}

	public void initIntroSequence()
	{
		// Move song banner out of screen.
		GameObject wordScrollObj = GameObject.Find("WordScroll");
		//Transform wordScrollLeftObj = wordScrollObj.transform.FindChild("WordScroll_Left");
		//Rect cameraWorldBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,true);
		
		//Vector3 songBannerDestPos = new Vector3(wordScrollObj.transform.position.x,wordScrollObj.transform.position.y - (wordScrollLeftObj.renderer.bounds.size.y * 2f),wordScrollObj.transform.position.z);

		triggerSoundAtCamera("MachinePowerUp",true);

		CustomAnimationManager aniMang = wordScrollObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>() { 0.5f }));
		List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
		batch2.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3]{songBannerDestPos.x,songBannerDestPos.y,songBannerDestPos.z}, 2f, true }));
		//batchLists.Add(batch1);
		batchLists.Add(batch2);
		aniMang.registerListener("IntroScript",this);
		aniMang.init("IntroSongBannerReveal",batchLists);
	}

	public void triggerShowFirstSentence()
	{
		GameObject.Find("GlobObj").GetComponent<AcSerenadeHeroScenario>().renderSongSentence();//.firstSentence();//
	}

	public void triggerPermanentViolinistEnter()
	{
		violinist1.SetActive(true);

		Animator characterAni = violinist1.GetComponent<Animator>();
		string animPrefix = (("Ser_Viol1").Split('_')[1]).ToLower();
		characterAni.Play(animPrefix+"_walk");

		Vector3 destPtInScene = firstViolinistDestPos;
		float characterWalkSpeed = 4f;

		triggerSoundAtCamera("RunningFootsteps",true);

		CustomAnimationManager caMang = violinist1.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> cmdBatchList = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> cmdBatch1 = new List<AniCommandPrep>();
		cmdBatch1.Add(new AniCommandPrep("MoveToLocation",1, new List<System.Object>() { new float[3] {destPtInScene.x,destPtInScene.y,destPtInScene.z}, characterWalkSpeed }));
		cmdBatchList.Add(cmdBatch1);
		caMang.registerListener("IntroScript",this);
		caMang.init("Violin1Enter",cmdBatchList);
	}

	public void triggerWordPoolRender()
	{
		wordBlocks = GameObject.Find("GlobObj").GetComponent<AcSerenadeHeroScenario>().renderWordPool();

		GameObject wordPoolObj = GameObject.Find("WordPool");
		GameObject wordPoolDisplayAreaObj = wordPoolObj;
		Rect fragmentAreaBounds = CommonUnityUtils.get2DBounds(wordPoolDisplayAreaObj.renderer.bounds);

		Rect cameraWorldBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,true);

		for(int i=0; i<wordBlocks.Count; i++)
		{
			GameObject reqWordBlock = wordBlocks[i];
			reqWordBlock.layer = 0;
			ActiveAutoResizeScript tmpResizeScript = reqWordBlock.GetComponent<ActiveAutoResizeScript>();
			if(tmpResizeScript != null) { tmpResizeScript.enabled = false; }

			Vector3 reqHiddenPos = new Vector3(reqWordBlock.transform.position.x,cameraWorldBounds.y - cameraWorldBounds.height - fragmentAreaBounds.height,reqWordBlock.transform.position.z);
			reqWordBlock.transform.position = reqHiddenPos;
		}
	}

	private void performNextWordPop()
	{
		if(nxtBlockPopIndex >= wordBlocks.Count)
		{
			respondToEvent("IntroSequence","WordPoolPopAllDone",null);
		}
		else
		{
			GameObject reqWordBlock = wordBlocks[nxtBlockPopIndex];

			GameObject wordPoolObj = GameObject.Find("WordPool");
			GameObject wordPoolDisplayAreaObj = wordPoolObj;

			Vector3 destPos = new Vector3(reqWordBlock.transform.position.x,wordPoolDisplayAreaObj.transform.position.y,reqWordBlock.transform.position.z);

			float popSpeed = 5f;

			CustomAnimationManager caMang = reqWordBlock.AddComponent<CustomAnimationManager>();
			List<List<AniCommandPrep>> cmdBatchList = new List<List<AniCommandPrep>>();
			List<AniCommandPrep> cmdBatch1 = new List<AniCommandPrep>();
			cmdBatch1.Add(new AniCommandPrep("MoveToLocation",1, new List<System.Object>() { new float[3] {destPos.x,destPos.y,destPos.z}, popSpeed }));
			cmdBatchList.Add(cmdBatch1);
			caMang.registerListener("IntroScript",this);
			caMang.init("WordBlockPop",cmdBatchList);

			nxtBlockPopIndex++;
		}
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "IntroSongBannerReveal")
		{
			Destroy(GameObject.Find("MachinePowerUp"));

			Transform musicMarkerObj = GameObject.Find("WordScroll").transform.FindChild("MusicMarker");
			musicMarkerObj.parent = null;

			triggerShowFirstSentence();
		}
		else if(para_eventID == "SentenceDisplayed")
		{
			triggerPermanentViolinistEnter();
		}
		else if(para_eventID == "Violin1Enter")
		{
			Destroy(GameObject.Find("RunningFootsteps"));

			Animator characterAni = violinist1.GetComponent<Animator>();
			string animPrefix = (("Ser_Viol1").Split('_')[1]).ToLower();
			characterAni.Play(animPrefix+"_play",0,0f);
			characterAni.speed = 0;

			triggerWordPoolRender();
			performNextWordPop();
		}
		else if(para_eventID == "WordBlockPop")
		{
			triggerSoundAtCamera("Pop",false);

			GameObject reqWordBlock = wordBlocks[nxtBlockPopIndex-1];
			reqWordBlock.layer = LayerMask.NameToLayer("Draggable");
			ActiveAutoResizeScript tmpResizeScript = reqWordBlock.GetComponent<ActiveAutoResizeScript>();
			if(tmpResizeScript != null) { tmpResizeScript.enabled = true; }

			performNextWordPop();
		}
		else if(para_eventID == "WordPoolPopAllDone")
		{
			AcIntroCountDownScript countDownScript = transform.gameObject.AddComponent<AcIntroCountDownScript>();
			countDownScript.registerListener("IntroScript",this);
			countDownScript.init(4,0.5f,false);
			triggerSoundAtCamera("DrumstickIntro",false);
		}
		else if(para_eventID == "CountdownEnd")
		{
			// Unfreeze violinist.
			Animator characterAni = violinist1.GetComponent<Animator>();
			string animPrefix = (("Ser_Viol1").Split('_')[1]).ToLower();
			characterAni.Play(animPrefix+"_play",0,0f);
			characterAni.speed = 1;

			// Unfreeze serenader.
			serenader.GetComponent<Animator>().enabled = true;
			serenaderParticleSystem.gameObject.SetActive(true);

			GameObject.Find("GlobObj").GetComponent<AcSerenadeHeroScenario>().startTheBandMusic();
			Destroy(this);
		}
	}


	private void triggerSoundAtCamera(string para_soundFileName, bool para_permanent)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		GameObject nwSFX = ((Transform) Instantiate(Resources.Load<Transform>("Prefabs/SFxBox"),camGObj.transform.position,Quaternion.identity)).gameObject;
		nwSFX.name = para_soundFileName;
		if(para_permanent)
		{
			Destroy(nwSFX.GetComponent<DestroyAfterTime>());
		}
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = 0.7f;
		audS.Play();
	}
}

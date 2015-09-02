/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class IntroNarrativeWindow : ILearnRWUIElement, CustomActionListener, IActionNotifier
{
	//string windowName = "IntroNarrativeWindow";

	//string pathDirectory;

	int currSlideID;
	Sprite currSlideSprite;
	Sprite nextSlideSprite;
	bool noMoreSlides;

	SpriteRenderer foreSlideRend;
	//SpriteRenderer backSlideRend;

	bool nxtBtnOn;
	bool exitBtnOn;


	List<AudioClip> clipList = new List<AudioClip>();
	int playing = 0;
	AudioSource audS = null;
	bool isMaleSetting = true;

	void Update(){

		if(playing < clipList.Count){
			if(!audS.isPlaying){
				audS.clip = clipList[playing];
				audS.volume = 1f;
				audS.Play();


//				audS.PlayOneShot(clipList[playing]);

				playing++;

				//yield return new WaitForSeconds(audS.clip.length);
			}


		}

	}
	
	
	void Start()
	{

		foreSlideRend = transform.FindChild("ForeSlide").GetComponent<SpriteRenderer>();
		//backSlideRend = transform.FindChild("BackSlide").GetComponent<SpriteRenderer>();
		audS = this.GetComponent<AudioSource>();
		if(audS == null) { audS = gameObject.AddComponent<AudioSource>(); }






		// Switch on dimer object.
		GameObject dimScreen = transform.FindChild("DimScreen").gameObject;
		dimScreen.renderer.enabled = true;
		dimScreen.renderer.material.color = Color.black;
		
		// Init text items.
		string[] elementNames   = {"NextButton","MapButton","ExitButton"};
		string[] elementContent = {"NextButton","MapButton","ExitButton"};
		bool[] destroyGuideArr = {false,false,false};
		int[] textElementTypeArr = {0,0,0};
		
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);
		
		Destroy(transform.FindChild("WindowBounds").gameObject);
		//nxtBtnOn = false;
		//transform.FindChild("NextButton").renderer.enabled = false;
	}
	
	
	void OnGUI()
	{
		if( ! hasInitGUIStyles)
		{
			prepGUIStyles();
			hasInitGUIStyles = true;
		}
		else
		{
			
			GUI.color = Color.clear;
			
			if(nxtBtnOn)
			{
				if( ! noMoreSlides)
				{
					if(GUI.Button(uiBounds["NextButton"],textContent["NextButton"]))
					{
						triggerFadeOutCurrentSlide();
					}
				}
			}

			if(noMoreSlides)
			{
				if(GUI.Button(uiBounds["MapButton"],textContent["MapButton"]))
				{
					// Done with images. Report back in order to transition to the worldview.
					notifyAllListeners("IntroNarrativeWindow","AllDone",null);
					Destroy(transform.gameObject);
				}
			}

			if(exitBtnOn)
			{
				if(GUI.Button(uiBounds["ExitButton"],textContent["ExitButton"]))
				{
					// Abrupt exit.
					notifyAllListeners("IntroNarrativeWindow","AllDone",null);
					Destroy(transform.gameObject);
				}
			}
			

		}
	}

	
	public void init(string para_windowName,  bool para_exitBtnAvailable)
	{


		//windowName = para_windowName;
		//pathDirectory = LocalisationMang.getSlideDirectory();;
		currSlideID = 0;

		GameObject poRef = PersistentObjMang.getInstance();
		DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
		
		if(ds.containsData("PlayerAvatarSettings"))
		{
			PlayerAvatarSettings playerAvSettings = (PlayerAvatarSettings) ds.getData("PlayerAvatarSettings");
			string genderStr = playerAvSettings.getGender();
			if(genderStr.Equals("Female")) { 
				isMaleSetting = false; 
				//Debug.LogWarning("** Female"+genderStr);
				
			}//else{
			//Debug.LogWarning("*WRONG* Female"+genderStr);
			
			//}
			
		}

		bool proceedureSuccess = false;

		bool foundFirstSlide = false;
		bool foundFirstNextSlide = false;

		if(getNextSlideSprite())
		{
			foundFirstSlide = true;
			currSlideSprite = nextSlideSprite;

			clipList = LocalisationMang.getIntroSound(currSlideID,isMaleSetting);
			playing = 0;

		}

		if(getNextSlideSprite())
		{
			foundFirstNextSlide = true;
		}

		proceedureSuccess = ((foundFirstSlide)&&(foundFirstNextSlide));


		if(proceedureSuccess)
		{
			nxtBtnOn = true;
			transform.FindChild("NextButton").gameObject.renderer.enabled = true;
			
			if(para_exitBtnAvailable)
			{
				exitBtnOn = true;
				transform.FindChild("ExitButton").gameObject.renderer.enabled = true;
			}



			
			foreSlideRend = transform.FindChild("ForeSlide").GetComponent<SpriteRenderer>();
			foreSlideRend.sprite = currSlideSprite;
		}
		else
		{
			// Abrupt exit.
			notifyAllListeners("IntroNarrativeWindow","AllDone",null);
			Destroy(transform.gameObject);
		}
	}

	private void triggerFadeOutCurrentSlide()
	{
		CustomAnimationManager aniMang = transform.FindChild("ForeSlide").gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] {1,1,1,0}, 1f }));
		batchLists.Add(batch1);
		aniMang.registerListener("IntroNarrativeWind",this);
		aniMang.init("FadeOutCurrentSlide",batchLists);

		nxtBtnOn = false;
		transform.FindChild("NextButton").gameObject.renderer.enabled = false;
	}

	private void triggerFadeInNextSlide()
	{
		foreSlideRend.sprite = nextSlideSprite;
		
		CustomAnimationManager aniMang = transform.FindChild("ForeSlide").gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] {1,1,1,1}, 1f }));
		batchLists.Add(batch1);
		aniMang.registerListener("IntroNarrativeWind",this);
		aniMang.init("FadeInNextSlide",batchLists);

		nxtBtnOn = false;
		transform.FindChild("NextButton").gameObject.renderer.enabled = false;
	}

	private bool getNextSlideSprite()
	{
		bool retFlag = false;

		currSlideID++;
		//string nxtSlideImgFileName = "Page" + currSlideID;

		Sprite nwImg = LocalisationMang.getIntroSlide(currSlideID);//Resources.Load<Sprite>(pathDirectory + "/" + nxtSlideImgFileName);
		if(nwImg != null)
		{
			nextSlideSprite = nwImg;
			Resources.UnloadUnusedAssets();
			System.GC.Collect();
			retFlag = true;
		}
		else
		{
			currSlideID--;
			noMoreSlides = true;
		}

		return retFlag;
	}


	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "FadeOutCurrentSlide")
		{
			triggerFadeInNextSlide();
		}
		else if(para_eventID == "FadeInNextSlide")
		{
			if(getNextSlideSprite())
			{
				nxtBtnOn = true;
				transform.FindChild("NextButton").gameObject.renderer.enabled = true;

				clipList = LocalisationMang.getIntroSound(currSlideID-1,isMaleSetting);
				playing = 0;

			}
			else
			{
				transform.FindChild("MapButton").gameObject.renderer.enabled = true;
			}
		}
	}

	
	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

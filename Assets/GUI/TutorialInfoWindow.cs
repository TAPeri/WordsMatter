/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class TutorialInfoWindow : ILearnRWUIElement, IActionNotifier
{
	string windowName = "SlideshowWindow";
	
	int currSlideID;
	Sprite[] slideshowImages;
	string[] slideshowText;

	Vector2 slideTextScrollPos;
	Vector2 tmpVect;

	Color darkGreen = ColorUtils.convertColor(0,192,0);

	SpriteRenderer tutorialSlideRend;

	bool showProgressBar = true;


    
	void Start()
	{
		this.loadTextures();

		currSlideID = 0;
		tutorialSlideRend = transform.FindChild("TutorialContentPane").GetComponent<SpriteRenderer>();
		slideTextScrollPos = new Vector2();
		tmpVect = new Vector2();


		/*// Correct render order.
		fixRenderSortingForElements(new List<string[]>() { (new string[]{"DimScreen"}),(new string[]{"TutorialPicFrame","PreviousButton","NextButton","ProgressBarArea","GoBtn"}),(new string[]{"TutorialContentPane"}) });*/
		
		// Switch on dimer object.
		GameObject dimScreen = transform.FindChild("DimScreen").gameObject;
		dimScreen.renderer.enabled = true;
		dimScreen.renderer.material.color = Color.black;

		// Init text items.
		string[] elementNames   = {"TutorialTitle","TutorialContentPane","SlideTextArea","PreviousButton","NextButton","ProgressBarArea","GoBtn","TTSButton"};
		string[] elementContent = {LocalisationMang.translate("Tutorial"),"Slides","SlideTextArea",
			"<",
			">",
			" ",
			LocalisationMang.translate("Ok"),
		""};
		bool[] destroyGuideArr = {true,false,false,false,false,true,false,false};
		int[] textElementTypeArr = {0,0,0,0,0,0,0,0};
		
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);

		Destroy(transform.FindChild("WindowBounds").gameObject);
		transform.FindChild("PreviousButton").renderer.enabled = false;
	}

	/*void OnDestroy(){

		if(WorldViewServerCommunication.tts!=null){
			WorldViewServerCommunication.tts.delete(slideshowText);
			
		}
	}*/


	void OnGUI()
	{
		if( ! hasInitGUIStyles)
		{
			prepGUIStyles();
			hasInitGUIStyles = true;
		}
		else
		{
			GUI.color = Color.black;

			GUI.Label(uiBounds["TutorialTitle"],textContent["TutorialTitle"],availableGUIStyles["FieldTitle"]);


			if((slideshowText != null)&&(currSlideID < slideshowText.Length))
			{
				if((Application.platform == RuntimePlatform.Android)
				   ||(Application.platform == RuntimePlatform.IPhonePlayer))
				{
					if(Input.touchCount == 1)
					{
						tmpVect.x = Input.touches[0].position.x;
						tmpVect.y = Screen.height - Input.touches[0].position.y;
						
						if(uiBounds["SlideTextArea"].Contains(tmpVect))
						{
							slideTextScrollPos.y += (Input.touches[0].deltaPosition.y * 1f);
						}
					}
				}

				GUILayout.BeginArea(uiBounds["SlideTextArea"]);
				slideTextScrollPos = GUILayout.BeginScrollView(slideTextScrollPos);
				GUILayout.BeginVertical();
				GUILayout.Label(slideshowText[currSlideID],availableGUIStyles["SlideText"]);
				GUILayout.EndVertical();
				GUILayout.EndScrollView();
				GUILayout.EndArea();
			}
			//GUI.DrawTexture(uiBounds["TutorialContentPane"],slideshowImages[currSlideID]);

			GUI.color = Color.white;

			if(showProgressBar)
			{
				float progPerc = (currSlideID+1)/(slideshowImages.Length * 1.0f);
				Rect fullProgBarArea = uiBounds["ProgressBarArea"];
				Rect progBounds = new Rect(fullProgBarArea.x, fullProgBarArea.y, progPerc * fullProgBarArea.width, fullProgBarArea.height);
				GUI.DrawTexture(progBounds,availableTextures["ProgressBarTex"]);
			}


			GUI.color = Color.clear;

			if(currSlideID > 0)
			{
				if(GUI.Button(uiBounds["PreviousButton"],textContent["PreviousButton"],availableGUIStyles["Button"]))
				{

					triggerSoundAtCamera("Blop");

					if(currSlideID > 0)
					{
						notifyAllListeners(windowName,"Previous",null);
						currSlideID--;
						tutorialSlideRend.sprite = slideshowImages[currSlideID];

						transform.FindChild("NextButton").gameObject.renderer.enabled = true;
						if(currSlideID <= 0)
						{
							transform.FindChild("PreviousButton").gameObject.renderer.enabled = false;
						}
					}
				}
			}


			if(currSlideID < (slideshowImages.Length-1))
			{
				if(GUI.Button(uiBounds["NextButton"],textContent["NextButton"],availableGUIStyles["Button"]))
				{

					triggerSoundAtCamera("Blip");

					if(currSlideID < (slideshowImages.Length-1))
					{
						notifyAllListeners(windowName,"Next",null);
						currSlideID++;
						tutorialSlideRend.sprite = slideshowImages[currSlideID];

						transform.FindChild("PreviousButton").gameObject.renderer.enabled = true;
						if(currSlideID >= (slideshowImages.Length-1))
						{
							transform.FindChild("NextButton").gameObject.renderer.enabled = false;
						}
					}
				}
			}

			//GUI.color = Color.clear;
			if(GUI.Button(uiBounds["GoBtn"],textContent["GoBtn"],availableGUIStyles["Button"]))
			{

				triggerSoundAtCamera("BubbleClick");

				notifyAllListeners(windowName,"Close",null);
				Destroy(transform.gameObject);
			}

			if(WorldViewServerCommunication.tts!=null){

				if(WorldViewServerCommunication.tts.loading()){
					setLoadingIconVisibility(true);
					GameObject.Find("TTSButton").renderer.enabled = false;
					GameObject.Find("GUI_VoiceIcon").renderer.enabled = false;
				}else{
					setLoadingIconVisibility(false);

					if(WorldViewServerCommunication.tts.test(slideshowText[currSlideID])){
						GameObject.Find("TTSButton").renderer.enabled = true;
						GameObject.Find("GUI_VoiceIcon").renderer.enabled = true;
						
						if(GUI.Button(uiBounds["TTSButton"],"",availableGUIStyles["Button"]))
						{
							
							triggerSoundAtCamera("BubbleClick");
							
							audio.clip = WorldViewServerCommunication.tts.say(slideshowText[currSlideID]);
							audio.volume = 1f;
							audio.Play();
							
						}
					}else{
						GameObject.Find("TTSButton").renderer.enabled = false;
						GameObject.Find("GUI_VoiceIcon").renderer.enabled = false;
					}

				}

			}else{
				GameObject.Find("TTSButton").renderer.enabled = false;
				GameObject.Find("GUI_VoiceIcon").renderer.enabled = false;
			}


		}
	}

	bool ttsLoading = false;
	public void setLoadingIconVisibility(bool para_state)
	{
		if(ttsLoading==para_state)
			return;
		else
			ttsLoading = para_state;

		Transform smallLoadIcon = transform.FindChild("ProcessWaitSmallIcon");
		if(smallLoadIcon != null)
		{
			smallLoadIcon.gameObject.SetActive(para_state);
			smallLoadIcon.FindChild("RotArrow").GetComponent<Animator>().Play("LoadingRotate");
		}
	}


	/*public void init(string para_windowName, string[] para_imgPaths, string[] para_slideTextArr)
	{
		windowName = para_windowName;
		slideshowText = para_slideTextArr;

		if(gameObject.GetComponent<AudioSource>() == null) { gameObject.AddComponent<AudioSource>(); }

		if(WorldViewServerCommunication.tts!=null)
			WorldViewServerCommunication.tts.fetch(para_slideTextArr);

		//}else{
		//	Debug.Log("TTS IS NULL");
		//}

		
		slideshowImages = new Sprite[para_imgPaths.Length];
		for(int i=0; i<para_imgPaths.Length; i++)
		{
			string reqPath = para_imgPaths[i];
			slideshowImages[i] = Resources.Load<Sprite>(reqPath);
		}

		if(slideshowImages.Length <= 1)
		{
			transform.FindChild("NextButton").gameObject.renderer.enabled = false;
			transform.FindChild("PreviousButton").gameObject.renderer.enabled = false;
		}

		tutorialSlideRend = transform.FindChild("TutorialContentPane").GetComponent<SpriteRenderer>();
		tutorialSlideRend.sprite = slideshowImages[currSlideID];
	}*/
	
	public void init(string para_windowName, string para_imgDirectory, string[] para_slideTextArr)
	{
		windowName = para_windowName;
		slideshowText = para_slideTextArr;

		if(gameObject.GetComponent<AudioSource>() == null) { gameObject.AddComponent<AudioSource>(); }

		
		if(WorldViewServerCommunication.tts!=null){
			WorldViewServerCommunication.tts.fetch(slideshowText);
			//foreach(string s in slideshowText)
			//	WorldViewServerCommunication.tts.fetch(new string[1]{s});
		}
		/*}else{
			Debug.Log("TTS IS NULL");
		}*/

		List<Sprite> tmpImages = new List<Sprite>();
		
		int nxtSlideID = 1;
		string nxtSlideImgFileName = "slide_";
		bool foundNewSlide = true;
		do
		{
			nxtSlideImgFileName = "slide_" + nxtSlideID;
			Sprite nwImg = Resources.Load<Sprite>(para_imgDirectory + "/" + nxtSlideImgFileName);
			if(nwImg != null)
			{
				tmpImages.Add(nwImg);
			}
			else
			{
				foundNewSlide = false;
			}
			
			nxtSlideID++;
		}
		while(foundNewSlide);
		
		slideshowImages = tmpImages.ToArray();

		if(slideshowImages.Length <= 1)
		{
			transform.FindChild("NextButton").gameObject.renderer.enabled = false;
			transform.FindChild("PreviousButton").gameObject.renderer.enabled = false;
		}

		tutorialSlideRend = transform.FindChild("TutorialContentPane").GetComponent<SpriteRenderer>();
		tutorialSlideRend.sprite = slideshowImages[currSlideID];
	}



	protected new void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string, GUIStyle>();

		float targetWidth = 1280f;
		float targetHeight = 800f;
		Vector3 scaleForCurrRes = new Vector3((Screen.width * 1.0f)/targetWidth,(Screen.height * 1.0f)/targetHeight,1f);
		
		GUIStyle fieldTitleStyle = new GUIStyle(GUI.skin.label);
		fieldTitleStyle.alignment = TextAnchor.MiddleCenter;
		fieldTitleStyle.fontSize = (int) (50 * scaleForCurrRes.x);
		
		GUIStyle fieldContentStyle = new GUIStyle(GUI.skin.label);
		fieldContentStyle.alignment = TextAnchor.MiddleCenter;
		fieldContentStyle.fontSize = (int) (30 * scaleForCurrRes.x);

		GUIStyle slideTextStyle = new GUIStyle(GUI.skin.label);
		slideTextStyle.alignment = TextAnchor.MiddleLeft;
		slideTextStyle.fontSize = (int) (30 * scaleForCurrRes.x);
		
		GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
		btnStyle.wordWrap = true;

		availableGUIStyles.Add("FieldTitle",fieldTitleStyle);
		availableGUIStyles.Add("FieldContent",fieldContentStyle);
		availableGUIStyles.Add("SlideText",slideTextStyle);
		availableGUIStyles.Add("Button",btnStyle);
		hasInitGUIStyles = true;
	}

	protected new void loadTextures()
	{
		availableTextures = new Dictionary<string, Texture2D>();

		Texture2D progBarTex = new Texture2D(1,1);
		progBarTex.SetPixel(0,0,darkGreen);
		progBarTex.Apply();

		availableTextures.Add("ProgressBarTex",progBarTex);
	}

	Transform sfxPrefab;

	private void triggerSoundAtCamera(string para_soundFileName)
	{
		triggerSoundAtCamera(para_soundFileName,1f,false);
	}
	
	private void triggerSoundAtCamera(string para_soundFileName, float para_volume, bool para_loop)
	{
		GameObject camGObj = Camera.main.gameObject;
		
		GameObject potentialOldSfx = GameObject.Find(para_soundFileName);
		if(potentialOldSfx != null) { Destroy(potentialOldSfx); }
		
		if(sfxPrefab == null) { sfxPrefab = Resources.Load<Transform>("Prefabs/SFxBox"); }
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		nwSFX.name = para_soundFileName;
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = para_volume;
		audS.loop = para_loop;
		if(para_loop) { Destroy(nwSFX.GetComponent<DestroyAfterTime>()); }
		audS.Play();
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

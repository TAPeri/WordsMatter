/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class PhotoAddedWindow : ILearnRWUIElement, CustomActionListener, IActionNotifier
{

	bool preped = false;

	Photo reqPhoto;
	bool okBtnVisible = false;

	
	void Start()
	{

		transform.FindChild("OkBtn").gameObject.SetActive(false);

		DelayForInterval dfi = transform.gameObject.AddComponent<DelayForInterval>();
		dfi.registerListener("TMpWind",this);
		dfi.init(1);
	}

	void OnGUI()
	{
		if(preped)
		{
			if( ! hasInitGUIStyles)
			{
				//Debug.Log(uiBounds.ContainsKey("OkBtn"));
				prepGUIStyles();
				hasInitGUIStyles = true;
				//Debug.Log(uiBounds.ContainsKey("OkBtn"));

			}
			else
			{
				GUI.color = Color.black;
				GUI.Label(uiBounds["Title"],textContent["Title"],availableGUIStyles["NoticeContent"]);
				GUI.color = Color.white;

				if(okBtnVisible)
				{
					GUI.color = Color.black;

					GUI.color = Color.clear;

					if(GUI.Button(uiBounds["OkBtn"],""))
					{
						triggerSoundAtCamera("BubbleClick");
						notifyAllListeners("PhotoAddedWindow","Close",null);
						Destroy(transform.gameObject);
					}

					//}
				}
				GUI.color = Color.white;
			}
		}
	}

	protected new void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string, GUIStyle>();
		
		float targetWidth = 1280f;
		float targetHeight = 800f;
		Vector3 scaleForCurrRes = new Vector3((Screen.width * 1.0f)/targetWidth,(Screen.height * 1.0f)/targetHeight,1f);
		
		GUIStyle noticeContentStyle = new GUIStyle(GUI.skin.label);
		noticeContentStyle.alignment = TextAnchor.LowerLeft;
		noticeContentStyle.fontSize = (int) (30 * scaleForCurrRes.x);
		
		GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
		btnStyle.wordWrap = true;
		btnStyle.fontSize = (int) (16 * scaleForCurrRes.x);
		btnStyle.normal.textColor = Color.black;
		
		availableGUIStyles.Add("NoticeContent",noticeContentStyle);
		availableGUIStyles.Add("Button",btnStyle);
		hasInitGUIStyles = true;
	}

	string questGiverName;
	string[] info;
	public void init(PDBCAddPhoto para_addPhotoParams,
	                 	   string para_questGiverName,
	                 		string[] text)
	{
		int questGiverID = para_addPhotoParams.questGiverID;
		questGiverName = para_questGiverName;
		info = text;

		
		// Apply portrait.
		Transform dummyPortrait = transform.FindChild("Portrait");
		PortraitHelper.replaceEntireDummyPortrait(dummyPortrait.gameObject,questGiverID,0,questGiverName,0.04f);
		CommonUnityUtils.setSortingLayerOfEntireObject(dummyPortrait.gameObject,"SpriteGUI");
		CommonUnityUtils.setSortingOrderOfEntireObject(dummyPortrait.gameObject,6000);

		GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
		reqPhoto = gbMang.getPhoto(questGiverID, para_addPhotoParams.langAreaID, para_addPhotoParams.diffIndexInLangArea, para_addPhotoParams.photoDiffPosition);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "DelayEnd")
		{
			Vector3 parentPos = transform.parent.position;
			Vector3 tmpPos = transform.position;
			tmpPos.x = parentPos.x;
			tmpPos.y = parentPos.y;
			transform.position = tmpPos;
			applyPrep();
			performFlashIntro();
		}
		else if(para_eventID == "PhotoFlashIntroAni")
		{
			drawPhoto();
			performFlashFade();
		}
		else if(para_eventID == "PhotoFlashFadeAni")
		{
			//Debug.Log(uiBounds.ContainsKey("OkBtn"));
			//Debug.Log("Visible");
			transform.FindChild("OkBtn").gameObject.SetActive(true);
			okBtnVisible = true;
		}
	}

	private void performFlashIntro()
	{
		//Debug.Log(uiBounds.ContainsKey("OkBtn"));

		Transform flashTrans = transform.FindChild("PhotoFlash");

		CommonUnityUtils.setSortingLayerOfEntireObject(flashTrans.gameObject,"SpriteGUI");
		CommonUnityUtils.setSortingOrderOfEntireObject(flashTrans.gameObject,7000);

		triggerSoundAtCamera("CameraFlash");

		CustomAnimationManager aniMang = flashTrans.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("ColorTransition",1, new List<System.Object>() { new float[4] {1,1,1,1}, 0.7f }));
		batchLists.Add(batch1);
		aniMang.registerListener("PhotoAddedWindow",this);
		aniMang.init("PhotoFlashIntroAni",batchLists);
	}

	private void performFlashFade()
	{

		//Debug.Log(uiBounds.ContainsKey("OkBtn"));
		CustomAnimationManager aniMang = transform.FindChild("PhotoFlash").gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("ColorTransition",1, new List<System.Object>() { new float[4] {1,1,1,0}, 4f }));
		batchLists.Add(batch1);
		aniMang.registerListener("PhotoAddedWindow",this);
		aniMang.init("PhotoFlashFadeAni",batchLists);
	}

	private void drawPhoto()
	{		//Debug.Log(uiBounds.ContainsKey("OkBtn"));

		// Apply photo render.
		PhotoVisualiser pVisualiser = new PhotoVisualiser();
		GameObject nwRenderObj = pVisualiser.producePhotoRender("NewPhotoRender",reqPhoto,0.05f,transform.FindChild("PhotoGuide").gameObject,true);
		CommonUnityUtils.setSortingLayerOfEntireObject(nwRenderObj,"SpriteGUI");
		CommonUnityUtils.setSortingOrderOfEntireObject(nwRenderObj,6000);
		nwRenderObj.transform.parent = transform;
	}

	private void applyPrep()
	{
		/*// Correct render order.
		fixRenderSortingForElements(new List<string[]>() { (new string[]{"DimScreen"}),
			(new string[]{"Frame"}),
			(new string[]{"ContentPane"}),
			(new string[]{"PortraitGuide","Title","OkBtn"})});*/
		
		
		// Init text items.
		string[] elementNames = new string[]{"Title","Portrait","PortraitGuide","OkBtn"};

		string information = "";
		if(info!=null){
			if(info.Length==2){
				if(info[0].Contains(":"))
					information+=info[0]+" "+ info[1];
				else
					information+=info[0]+": "+ info[1];
			}
		}

		if(information=="")
			information = LocalisationMang.getString("GhostB*PhotoAdd");

		string[] elementContent = new string[]{information,"Portrait","PortraitGuide",LocalisationMang.translate("Ok")};
		bool[] destroyGuideArr = new bool[]{true,false,true,false};
		int[] textElementTypeArr = new int[]{0,0,0,0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);
		//Debug.Log(uiBounds.ContainsKey("OkBtn"));
		preped = true;
	}

	private void triggerSoundAtCamera(string para_soundFileName)
	{
		GameObject camGObj = GameObject.Find("Main Camera");

		Transform sfxPrefab = Resources.Load<Transform>("Prefabs/SFxBox");
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

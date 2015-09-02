/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

using UnityEngine;
using System.Collections.Generic;

public class PlayerCustomisationScenario : ILearnRWUIElement, CustomActionListener
{

	public RuntimeAnimatorController charAniController_NonLooping;
	public RuntimeAnimatorController charAniController_Looping;
	Transform sfxPrefab = null;
	
	Transform[] baseAvatars;
	Transform workbenchAvatar;

	ClothingCatalog clothingCatalog;
	ClothingCatalogIterator[] catalogIters;

	ClothingConfig mainAvCConfig;
	bool isAnimatingMainAvatar;

	ClothingApplicator cApp;

	string genderSettingStr = "Male";

	bool playerInputState = false;

	bool doneButtonClickRegistered = false;

	Color inactiveColor = ColorUtils.convertColor(115,115,115,150);
	
	bool playerExiting = false;


	void Start()
	{

		performInitialUISetup();
		loadBaseAvatars();


		ClothingManager cMang = ClothingManager.getInstance(ClothingSize.BIG);
		clothingCatalog = cMang.loadNGetFullClothingCatalog(ClothingSize.BIG);

		// Start index with 0 will allow the specialised iterator to move over Male head parts.
		catalogIters = new ClothingCatalogIterator[3];
		catalogIters[0] = clothingCatalog.getSpecialisedHeadGearIterator(0,2);// getHeadGearIterator();
		catalogIters[1] = clothingCatalog.getBodyGearIterator();
		catalogIters[2] = clothingCatalog.getLegGearIterator();

		cApp = transform.gameObject.AddComponent<ClothingApplicator>();
		cApp.setSubject(GameObject.Find("MainAvatar"),charAniController_NonLooping);

		int initialBaseAvatar = 0;

		GameObject poRef = PersistentObjMang.getInstance();
		DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
		
		if(ds.containsData("PlayerAvatarSettings"))
		{
			PlayerAvatarSettings playerAvSettings = (PlayerAvatarSettings) ds.getData("PlayerAvatarSettings");
			string genderStr = playerAvSettings.getGender();
			if(genderStr.Equals("Female")) { 
				initialBaseAvatar = 1;
				
			}//else{
			//Debug.LogWarning("*WRONG* Female"+genderStr);
			
			//}
			
		}


		selectBaseAv(initialBaseAvatar);

		
		Rect backdrop2DBounds = CommonUnityUtils.get2DBounds(GameObject.Find("Backdrop").renderer.bounds);
		GameObject mainAv = GameObject.Find("MainAvatar");
		Vector3 mainAvRestPos = new Vector3(mainAv.transform.position.x,mainAv.transform.position.y,mainAv.transform.position.z);
		mainAv.transform.position = new Vector3(backdrop2DBounds.x - 3f,mainAv.transform.position.y,mainAv.transform.position.z);

		applyLoadedPlayerAvatarClothing();

		catalogIters[0].directAccessToItem(mainAvCConfig.getClothing("Head"));
		catalogIters[1].directAccessToItem(mainAvCConfig.getClothing("Body"));
		catalogIters[2].directAccessToItem(mainAvCConfig.getClothing("Leg"));
		refreshClothingIterArrowDisplays();

		isAnimatingMainAvatar = false;

		PlayerCustomisationIntroSequence introSeqScript = transform.gameObject.AddComponent<PlayerCustomisationIntroSequence>();
		introSeqScript.registerListener("AcScen",this);
		introSeqScript.init(mainAvRestPos);

		triggerSoundAtCamera("DoorOpen");
		triggerSoundAtCamera("FootstepsWood",0.9f,true);


	
	}

	void OnGUI()
	{
		if(!hasInitGUIStyles)
		{
			prepGUIStyles();
			hasInitGUIStyles = true;
		}
		else
		{
			if(playerInputState == true)
			{
				GUI.color = Color.clear;

				// BaseBoxes.
				for(int i=0; i<2; i++)
				{
					if(GUI.Button(uiBounds["BaseBox"+(i+1)],"",availableGUIStyles["Button"]))
					{
						selectBaseAv(i);
					}
				}




				for(int i=0; i<3; i++)
				{
					//string tmpClothingBoxName = "set"+i+"ClothingBox";
					string tmpPrevBtnName = "set"+i+"PrevBtn";
					string tmpNextBtnName = "set"+i+"NextBtn";


					/*if(GUI.Button(uiBounds[tmpClothingBoxName],"",availableGUIStyles["FieldContent"]))
					{
						applyDisplayBoxToMainAvatar(i);
					}*/
					if(GUI.Button(uiBounds[tmpPrevBtnName],textContent[tmpPrevBtnName],availableGUIStyles["Button"]))
					{
						if(catalogIters[i].hasPrevious())
						{
							triggerSoundAtCamera("Blop");
							string nextItem = catalogIters[i].getPrevious();
							if(nextItem != null) { applyDisplayBoxToMainAvatar(i); }
							if(! catalogIters[i].hasPrevious())
							{
								GameObject.Find(tmpPrevBtnName).GetComponent<SpriteRenderer>().color = inactiveColor;
							}
							GameObject.Find(tmpNextBtnName).GetComponent<SpriteRenderer>().color = Color.white;
							//if(nextItem != null){ refreshClothingDisplayBox(i); }
						}
					}
					if(GUI.Button(uiBounds[tmpNextBtnName],textContent[tmpNextBtnName],availableGUIStyles["Button"]))
					{
						if(catalogIters[i].hasNext())
						{
							triggerSoundAtCamera("Blip");
							string nextItem = catalogIters[i].getNext();
							if(nextItem != null) { applyDisplayBoxToMainAvatar(i); }
							if(! catalogIters[i].hasNext())
							{
								GameObject.Find(tmpNextBtnName).GetComponent<SpriteRenderer>().color = inactiveColor;
							}
							GameObject.Find(tmpPrevBtnName).GetComponent<SpriteRenderer>().color = Color.white;
							//if(nextItem != null){ refreshClothingDisplayBox(i); }
						}
					}
				}


				//GUI.Label(uiBounds["AvatarBox"],textContent["AvatarBox"],availableGUIStyles["FieldContent"]);
				//if(GUI.Button(uiBounds["AnimateBtn"],textContent["AnimateBtn"],availableGUIStyles["Button"]))
				//{
				//	toggleMainAvatarAnimate();
				//}



				if(GUI.Button(uiBounds["DoneButtonTop"],textContent["DoneButtonTop"],availableGUIStyles["Button"]))
				{
					if( ! doneButtonClickRegistered)
					{
						triggerDoneButtonClickEffect();
						doneButtonClickRegistered = true;
					}
				}

				GUI.color = Color.white;
			}
		}
	}

	private void triggerDoneButtonClickEffect()
	{
		//GameObject doneBtn = GameObject.Find("DoneButton");
		//CustomAnimationManager aniMang = doneBtn.AddComponent<CustomAnimationManager>();
		//List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		//List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		//batch1.Add(new AniCommandPrep("TriggerAnimation",1,new List<System.Object>() { "DoneButtonClicked" }));
		//List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
		//batch2.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>() { 0.2f }));
		//List<AniCommandPrep> batch3 = new List<AniCommandPrep>();
		//batch3.Add(new AniCommandPrep("TriggerAnimation",1,new List<System.Object>() { "DoneButtonIdle" }));
		//batchLists.Add(batch1);
		//batchLists.Add(batch2);
		//batchLists.Add(batch3);
		//aniMang.registerListener("AcScen",this);
		//aniMang.init("DoneButtonClickAni",batchLists);

		triggerSoundAtCamera("BubbleClick");

		storeMainAvatarConfigToPersistentMemory();

		playerInputState = false;
		respondToEvent("", "DoneButtonClickAni", null);

	}

	private void triggerPlayerLeaveThroughDoor()
	{
		GameObject mainAv = GameObject.Find("MainAvatar");




		Rect backdrop2DBounds = CommonUnityUtils.get2DBounds(GameObject.Find("Backdrop").renderer.bounds);

		Animator playerAni = mainAv.GetComponent<Animator>();
		playerAni.runtimeAnimatorController = charAniController_NonLooping;
		playerAni.Play("BigAVWalk");

		Vector3 tmpEAngles = mainAv.transform.localEulerAngles;
		tmpEAngles.x = 0;
		tmpEAngles.y = 180;
		tmpEAngles.z = 0;
		mainAv.transform.localEulerAngles = tmpEAngles;

		
		CustomAnimationManager aniMang = mainAv.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",1,new List<System.Object>() { new float[3]{backdrop2DBounds.x + backdrop2DBounds.width + 3f,mainAv.transform.position.y,mainAv.transform.position.z}, 3f }));
		batchLists.Add(batch1);
		aniMang.registerListener("AcScen",this);
		aniMang.init("PlayerExit",batchLists);

		playerExiting = true;
		triggerSoundAtCamera("FootstepsWood",0.9f,true);

		// Also activate fader.
		Transform faderPrefab = Resources.Load<Transform>("Prefabs/FaderScreen");
		Transform nwFader = (Transform) Instantiate(faderPrefab,new Vector3(0,0,0),Quaternion.identity);
		FaderScript fs = nwFader.GetComponent<FaderScript>();
		fs.registerListener("PCustomisation",this);
		fs.init(Color.clear,Color.black,1f,false);
	}

	void OnDestroy()
	{
		ClothingManager.disposeInstance();
	}



	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "IntroSequenceDone")
		{
			playerInputState = true;
		}
		else if(para_eventID == "DoneButtonClickAni")
		{


			// Hide all elements.
			GameObject.Find("set0PrevBtn").renderer.enabled = false;
			GameObject.Find("set0NextBtn").renderer.enabled = false;
			GameObject.Find("set1PrevBtn").renderer.enabled = false;
			GameObject.Find("set1NextBtn").renderer.enabled = false;
			GameObject.Find("set2PrevBtn").renderer.enabled = false;
			GameObject.Find("set2NextBtn").renderer.enabled = false;
			
			GameObject tmpDoneBtn = GameObject.Find("DoneButton");
			Destroy(tmpDoneBtn);

			// Trigger player leave scene.
			triggerPlayerLeaveThroughDoor();
		}
		else if(para_eventID == "FadeEffectDone")
		{
			if(playerExiting)
			{
				Destroy(GameObject.Find("FootstepsWood"));
				triggerSoundAtCamera("DoorClose");
				exitPlayerCustomisation();
			}
		}
		/*else if(para_eventID == "PlayerExit")
		{
			exitPlayerCustomisation();
		}*/
	}

	protected new void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string, GUIStyle>();
		
		GUIStyle fieldTitleStyle = new GUIStyle(GUI.skin.label);
		fieldTitleStyle.alignment = TextAnchor.MiddleCenter;
		fieldTitleStyle.fontSize = 17;
		
		GUIStyle fieldContentStyle = new GUIStyle(GUI.skin.label);
		fieldContentStyle.alignment = TextAnchor.MiddleCenter;
		fieldContentStyle.fontSize = 17;
		
		GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
		btnStyle.wordWrap = true;
		btnStyle.normal.textColor = Color.black;
		
		availableGUIStyles.Add("FieldTitle",fieldTitleStyle);
		availableGUIStyles.Add("FieldContent",fieldContentStyle);
		availableGUIStyles.Add("Button",btnStyle);
		hasInitGUIStyles = true;
	}


	private void selectBaseAv(int para_baseAvatarIndex)
	{
		Destroy(GameObject.Find("MainAvatar"));

		if(para_baseAvatarIndex == 0) { genderSettingStr = "Male"; } else { genderSettingStr = "Female"; }

		Debug.Log("Avatar gender: "+genderSettingStr);

		Transform tmpBaseAv = baseAvatars[para_baseAvatarIndex];
		Rect tmpWB = WorldSpawnHelper.getGuiToWorldBounds(uiBounds["AvatarBox"],1,upAxisArr);
		Vector3 tmpSpawnPt = new Vector3(tmpWB.x + (tmpWB.width/2f),tmpWB.y - (tmpWB.height/2f),-3);
		workbenchAvatar = (Transform) Instantiate(tmpBaseAv,tmpSpawnPt,tmpBaseAv.rotation);
		workbenchAvatar.name = "MainAvatar";
		workbenchAvatar.GetComponent<Animator>().runtimeAnimatorController = charAniController_NonLooping;

		//Animator tmpAni = workbenchAvatar.gameObject.GetComponent<Animator>();
		//tmpAni.speed = 0;
		//tmpAni.enabled = false;

		Vector3 tmpScale = workbenchAvatar.localScale;
		tmpScale.x = 1.5f;
		tmpScale.y = 1.5f;
		tmpScale.z = 1.5f;
		workbenchAvatar.localScale = tmpScale;

		//if(mainAvCConfig == null)
		//{
			mainAvCConfig = new ClothingConfig();
			string setPrefix = "AV0"+(para_baseAvatarIndex+1);
			mainAvCConfig.setClothing("Head",setPrefix);
			mainAvCConfig.setClothing("Body",setPrefix);
			mainAvCConfig.setClothing("Leg",setPrefix);
		//}

		int startIndexForSpecialHead = 0;
		if(para_baseAvatarIndex != 0) { startIndexForSpecialHead = 1; } 
		catalogIters = new ClothingCatalogIterator[3];
		catalogIters[0] = clothingCatalog.getSpecialisedHeadGearIterator(startIndexForSpecialHead,2); //clothingCatalog.getHeadGearIterator();
		catalogIters[1] = clothingCatalog.getBodyGearIterator();
		catalogIters[2] = clothingCatalog.getLegGearIterator();

		refreshClothingIterArrowDisplays();

		cApp.setSubject(workbenchAvatar.gameObject,charAniController_NonLooping);
	}

	private void refreshClothingIterArrowDisplays()
	{
		for(int i=0; i<catalogIters.Length; i++)
		{
			ClothingCatalogIterator tmpIter = catalogIters[i];
			
			string tmpPrevBtnName = "set"+i+"PrevBtn";
			string tmpNextBtnName = "set"+i+"NextBtn";

			SpriteRenderer tmpPrevSRend = GameObject.Find(tmpPrevBtnName).GetComponent<SpriteRenderer>();
			SpriteRenderer tmpNextSRend = GameObject.Find(tmpNextBtnName).GetComponent<SpriteRenderer>();
			if(! tmpIter.hasPrevious())	{ tmpPrevSRend.color = inactiveColor; } else { tmpPrevSRend.color = Color.white; }
			if(! tmpIter.hasNext())	{ tmpNextSRend.color = inactiveColor; } else { tmpNextSRend.color = Color.white; }
		}
	}

	// This applys clothing changes for the customisation screen.
	private void applyDisplayBoxToMainAvatar(int para_displayBoxIndex)
	{
		ClothingCatalogIterator reqIterator = catalogIters[para_displayBoxIndex];
		string iterName = reqIterator.getIteratorName();
		string iterCurrItem = reqIterator.getCurrentItem();

		string categoryName = iterName;
		string charSpritePrefix = iterCurrItem;
		//Big_
		string charSpritePrefixWithoutAddons = charSpritePrefix.Substring(4); // Removes the Big_ from Big_AVXX to give the AVXX.


		mainAvCConfig.setClothing(categoryName,charSpritePrefixWithoutAddons);
		cApp.applyClothingConfig(mainAvCConfig,ClothingSize.BIG);

		workbenchAvatar.GetComponent<Animator>().Play("AVPhotoPose_3");
	}

	// This attempts to apply a loaded clothing config file.
	private void applyLoadedPlayerAvatarClothing()
	{
		GameObject poRef = PersistentObjMang.getInstance();
		if(poRef != null)
		{
			DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
			if(ds != null)
			{
				GameObject mainAv = GameObject.Find("MainAvatar");
				if(mainAv != null)
				{
					if(ds.containsData("PlayerAvatarSettings"))
					{
						PlayerAvatarSettings playerAvSettings = (PlayerAvatarSettings) ds.getData("PlayerAvatarSettings");
						ClothingConfig storedClothingConfig = playerAvSettings.getClothingSettings();
						ClothingApplicator cApp = transform.gameObject.AddComponent<ClothingApplicator>();
						cApp.setSubject(mainAv,charAniController_NonLooping);
						cApp.applyClothingConfig(storedClothingConfig,ClothingSize.BIG);
						Destroy(cApp);
						mainAv.GetComponent<Animator>().runtimeAnimatorController = charAniController_NonLooping;
						mainAvCConfig = storedClothingConfig;


						int startIndexForHeadParts = 0;
						if((playerAvSettings.getGender().Contains("Male")) == false) { startIndexForHeadParts = 1; }
						catalogIters[0] = clothingCatalog.getSpecialisedHeadGearIterator(startIndexForHeadParts,2);
					}
				}
			}
		}
	}

	/*private void refreshClothingDisplayBox(int para_clothingDisplayBoxIndex)
	{
		ClothingCatalogIterator reqIterator = catalogIters[para_clothingDisplayBoxIndex];
		string iterName = reqIterator.getIteratorName();
		
		string displayBoxAvName = iterName+"Av";
		Destroy(GameObject.Find(displayBoxAvName));
		
		string iterCurrItem = reqIterator.getCurrentItem();
		
		Transform reqTrans = null;
		if(iterCurrItem.Contains("SC"))
		{
			int modelNum = int.Parse(iterCurrItem.Split(new string[]{"SC"},System.StringSplitOptions.None)[1]);
			reqTrans = Resources.Load<Transform>("Prefabs/Avatars/SecChar-"+(modelNum-1));
		}
		
		if(reqTrans != null)
		{
			Rect tmpWB = WorldSpawnHelper.getGuiToWorldBounds(uiBounds["set"+para_clothingDisplayBoxIndex+"ClothingBox"],1,upAxisArr);
			Vector3 tmpSpawnPt = new Vector3(tmpWB.x + (tmpWB.width/2f),tmpWB.y - (tmpWB.height/2f),-3);
			Transform nwDisplayObj = (Transform) Instantiate(reqTrans,tmpSpawnPt,reqTrans.rotation);
			nwDisplayObj.name = displayBoxAvName;
			
			Animator tmpAni = nwDisplayObj.GetComponent<Animator>();
			//tmpAni.speed = 0;

			cApp.setSubject(nwDisplayObj.gameObject,charAniController_NonLooping);
			cApp.toggleAllSpriteRends(nwDisplayObj,false);
			cApp.toggleClothingVisibility(iterCurrItem,iterName);
			cApp.setSubject(GameObject.Find("MainAvatar"),charAniController_NonLooping);
		}
	}*/

	private void toggleMainAvatarAnimate()
	{
		isAnimatingMainAvatar = !isAnimatingMainAvatar;
		GameObject mainAv = GameObject.Find("MainAvatar");
		Animator tmpAni = mainAv.GetComponent<Animator>();
		tmpAni.runtimeAnimatorController = null;

		if(isAnimatingMainAvatar)
		{
			tmpAni.runtimeAnimatorController = charAniController_Looping;
		}
		else
		{
			tmpAni.runtimeAnimatorController = charAniController_NonLooping;
		}
	}


	private void storeMainAvatarConfigToPersistentMemory()
	{
		GameObject poRef = PersistentObjMang.getInstance();
		DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
		if(ds != null)
		{
			PlayerAvatarSettings playerAvSettings = new PlayerAvatarSettings(genderSettingStr,mainAvCConfig);
			ds.insertData("PlayerAvatarSettings",playerAvSettings);
		}
	}

	private void exitPlayerCustomisation()
	{
		GameObject poRef = PersistentObjMang.getInstance();
		DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
		if(ds != null)
		{

			if(needsIntroNarrative())
			{
				ds.insertData("HasDoneFirstTimeIntro",true);
				Application.LoadLevel("IntroNarrativeScene");
			}
			else
			{
				string nxtSceneName = "WorldView";
				if(ds.containsData("NextSceneName"))
				{
					string tmpSceneName = (string) ds.getData("NextSceneName");
					if(tmpSceneName != null)
					{
						tmpSceneName = tmpSceneName.Trim();
						if(tmpSceneName != "")
						{
							nxtSceneName = tmpSceneName;
						}
					}
					ds.removeData("NextSceneName");
				}

				ds.insertData("NextSceneToLoad",nxtSceneName);
				Application.LoadLevel("LoadingScene");
			}
		}
	}



	



	private void performInitialUISetup()
	{
		// Auto Adjust.
		GameObject tmpBackdrop = GameObject.Find("Backdrop").gameObject;
		List<GameObject> itemsToAutoAdjust = new List<GameObject>();
		List<string> itemAdjNames = new List<string>() { "AvatarBox", //"DoneBtn",
			"set0PrevBtn", "set0NextBtn",
			"set1PrevBtn", "set1NextBtn",
			"set2PrevBtn", "set2NextBtn", "BaseBox1","BaseBox2" };
		
		for(int i=0; i<itemAdjNames.Count; i++)
		{
			GameObject tmpGObj = GameObject.Find(itemAdjNames[i]);
			if(tmpGObj != null)
			{
				itemsToAutoAdjust.Add(tmpGObj);
			}
		}
		
		SpawnNormaliser.adjustGameObjectsToNwBounds(SpawnNormaliser.get2DBounds(tmpBackdrop.renderer.bounds),
		                                            WorldSpawnHelper.getCameraViewWorldBounds(tmpBackdrop.transform.position.z,true),
		                                            itemsToAutoAdjust);
		
		for(int i=0; i<itemsToAutoAdjust.Count; i++)
		{
			itemsToAutoAdjust[i].transform.parent = transform;
		}
		
		// Correct render order.
		//fixRenderSortingForElements(new List<string[]>() { (new string[]{"DimScreen"}),
		//	(new string[]{"Frame"}),
		//	(new string[]{"ContentPane"}),
		//	(new string[]{"GoalImg","TextArea","OkBtn"})});
		
		
		// Init text items.
		string[] elementNames = itemAdjNames.ToArray();
		string[] elementContent = new string[]{"AvatarBox"//,""
			,"<",">","<",">","<",">","BB1","BB2"};
		bool[] destroyGuideArr = new bool[]{false,//false,
			false,false,false,false,false,false,false,false};
		int[] textElementTypeArr = new int[]{0,//0,
			0,0,0,0,0,0,0,0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);


		// Hide items.
		GameObject.Find("DoneButton").transform.FindChild("DoneButtonTop").renderer.enabled = false;
		GameObject.Find("DoneButton").transform.FindChild("DoneButtonShadow").renderer.enabled = false;
		GameObject.Find("set0PrevBtn").renderer.enabled = false;
		GameObject.Find("set0NextBtn").renderer.enabled = false;
		GameObject.Find("set1PrevBtn").renderer.enabled = false;
		GameObject.Find("set1NextBtn").renderer.enabled = false;
		GameObject.Find("set2PrevBtn").renderer.enabled = false;
		GameObject.Find("set2NextBtn").renderer.enabled = false;

		elementNames = new string[]{"DoneButtonTop","DoneButtonShadow"};//,"DoneButton"
		elementContent = new string[]{"DoneButtonTop","DoneButtonShadow"};//,"Ok"
		destroyGuideArr = new bool[]{false,false};//,false};
		textElementTypeArr = new int[]{0,0};//,0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,"DoneButton");


		/*GameObject tmpDoneBtn = GameObject.Find("DoneButton");
		Vector3 tmpDoneBtnPos = tmpDoneBtn.transform.position;
		tmpDoneBtnPos.y -= 3f;
		tmpDoneBtn.transform.position = tmpDoneBtnPos;*/


		// Add Done to the Done Button.
		//GameObject doneWordBox = WordBuilderHelper.buildWordBox(99,LocalisationMang.translate("Ok"),CommonUnityUtils.get2DBounds(tmpDoneBtn.renderer.bounds),1,upAxisArr,Resources.Load<Transform>("Prefabs/GenericWordBox"));
		//WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() { doneWordBox },0.09f);
		//doneWordBox.transform.parent = tmpDoneBtn.transform;
		//Vector3 tmpTextPos = doneWordBox.transform.position;
		//tmpTextPos.z = -1;
		//doneWordBox.transform.position = tmpTextPos;
		//Destroy(doneWordBox.transform.FindChild("Board").gameObject);
		//doneWordBox.transform.FindChild("Text").renderer.sortingOrder = 300;
	}

	private void loadBaseAvatars()
	{
		baseAvatars = new Transform[] { Resources.Load<Transform>("Prefabs/AvatarsBigVersions/Big_AVs/Big_AV01"),
										Resources.Load<Transform>("Prefabs/AvatarsBigVersions/Big_AVs/Big_AV02") };
		
		Rect tmpWB;
		Vector3 tmpSpawnPt;
		for(int i=0; i<baseAvatars.Length; i++)
		{
			Transform tmpBaseAv = baseAvatars[i];
			tmpWB = WorldSpawnHelper.getGuiToWorldBounds(uiBounds["BaseBox"+(i+1)],1,upAxisArr);
			tmpSpawnPt = new Vector3(tmpWB.x + (tmpWB.width/2f),tmpWB.y - (tmpWB.height/2f),-3);
			tmpSpawnPt.y += 1000;
			Transform nwBaseAv = (Transform) Instantiate(tmpBaseAv,tmpSpawnPt,tmpBaseAv.rotation);
			nwBaseAv.name = "BaseAvatar"+i;
			nwBaseAv.localScale = new Vector3(0.5f,0.5f,1f);
			nwBaseAv.GetComponent<Animator>().Play("AVPhotoPose_3");
			nwBaseAv.GetComponent<Animator>().speed = 0;
		}
	}



	private bool needsIntroNarrative()
	{
		bool retFlag = false;
		
		try
		{
			GameObject poRef = PersistentObjMang.getInstance();
			DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
			
			if(ds.containsData("FirstTime"))
			{
				if(((bool)ds.getData("FirstTime")) == true)
				{
					retFlag = true;
				}
			}

			if(ds.containsData("HasDoneFirstTimeIntro"))
			{
				retFlag = false;
			}
		}
		catch(System.Exception ex)
		{
			Debug.LogError(ex.StackTrace);
		}
		
		return retFlag;
	}

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
}

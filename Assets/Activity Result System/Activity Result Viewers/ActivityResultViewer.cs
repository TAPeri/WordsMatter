/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public abstract class ActivityResultViewer : ILearnRWUIElement, IActionNotifier
{
	protected ActivityResult acResultData;
	protected int currPage = 0;
	protected int maxPages = 0;

	//bool okIsActivated = false;

	protected bool guiOnStatus = true;

	bool previousPageAvailable = false;
	bool nextPageAvailable = true;
	

	protected void Start()
	{
		loadTextures();


		//Transform gameResultPage = transform.FindChild("ResultScreens_A");
		//Transform pedagogicalResultPage = transform.FindChild("ResultScreens_B");

		// Init Game Data Result Page Text Items.
		string[] elementNames   = {"TitleArea-0","ScrollArea-0"};
		string[] elementContent = {"",LocalisationMang.translate("Scroll area")};//LocalisationMang.translate("Game results")
		bool[] destroyGuideArr = {true,false};
		int[] textElementTypeArr = {0,0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,"ResultScreens_A");

		// Init Pedagogical Data Result Page Text Items.
		elementNames   = new string[]{"TitleArea-1","ScrollArea-1"};
		elementContent = new string[]{"",LocalisationMang.translate("Scroll area")};//LocalisationMang.translate("Pedagogical results")
		destroyGuideArr = new bool[]{true,false};
		textElementTypeArr = new int[]{0,0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,"ResultScreens_B");

		// Init Back to the World Page Text Items.
		elementNames   = new string[]{"TitleArea-2","ScrollArea-2","ExitText","ReplayText","GUI_Exit","ReplayButton"};
		elementContent = new string[]{"",LocalisationMang.translate("Scroll area"),LocalisationMang.translate("Back to the world"),LocalisationMang.translate("Play again"),"",""};//LocalisationMang.translate("Back to the world")
		destroyGuideArr = new bool[]{true,false,true,true,false,false};
		textElementTypeArr = new int[]{0,0,0,0,0,0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,"ResultScreens_C");

		// Init next and previous buttons.
		elementNames   = new string[]{"PreviousButton","NextButton"};
		elementContent = new string[]{LocalisationMang.translate("Previous"),LocalisationMang.translate("Next")};
		destroyGuideArr = new bool[]{false,false};
		textElementTypeArr = new int[]{0,0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);


		// Dim Screen ON.
		transform.FindChild("DimScreen").renderer.enabled = true;

		selectPage(pageRedirector(currPage,currPage));
	}

	protected void OnGUI()
	{
		if( ! hasInitGUIStyles)
		{
			prepGUIStyles();
			hasInitGUIStyles = true;
		}
		else
		{
			GUI.color = Color.black;

			if(guiOnStatus)
			{
				if(currPage == 0)
				{
					GUI.Label(uiBounds["TitleArea-0"],textContent["TitleArea-0"],availableGUIStyles["PageTitle"]);
				}
				else if(currPage == 1)
				{
					GUI.Label(uiBounds["TitleArea-1"],textContent["TitleArea-1"],availableGUIStyles["PageTitle"]);
				}
				else if(currPage == 2)
				{
					GUI.Label(uiBounds["TitleArea-2"],textContent["TitleArea-2"],availableGUIStyles["PageTitle"]);
					GUI.Label(uiBounds["ExitText"],textContent["ExitText"],availableGUIStyles["FieldTitle"]);
					GUI.Label(uiBounds["ReplayText"],textContent["ReplayText"],availableGUIStyles["FieldTitle"]);

					GUI.color = Color.clear;
					if(guiOnStatus)
					{
						if(GUI.Button(uiBounds["GUI_Exit"],""))
						{
							triggerSoundAtCamera("BubbleClick");

							exitResultScreen();
						}else if(GUI.Button(uiBounds["ReplayButton"],"")){
							triggerSoundAtCamera("BubbleClick");

							notifyAllListeners("ResultViewerWindow","Replay",null);

						}
					}
				}
			}


			GUI.color = Color.clear;

			if(guiOnStatus)
			{

				if(previousPageAvailable)
				{
					if(GUI.Button(uiBounds["PreviousButton"],""))
					{
						triggerSoundAtCamera("Blop");

						selectPage(pageRedirector(currPage,currPage-1));
					}
				}

				if(nextPageAvailable)
				{
					if(GUI.Button(uiBounds["NextButton"],""))
					{
						triggerSoundAtCamera("Blip");

						selectPage(pageRedirector(currPage,currPage+1));
					}
				}
			}

			GUI.color = Color.white;
		}
	}


	public void exitResultScreen()
	{
		notifyAllListeners("ResultViewerWindow","Close",null);
	}

	public virtual void init(ActivityResult para_acResult)
	{
		acResultData = para_acResult;

		//GhostbookManager gbMang = GhostbookManager.getInstance();
		//IGBActivityReference acRef = gbMang.getActivityReferenceMaterial();

		// Update the activity icon on the result screen.
		//Transform reqAcSymbol = Resources.Load<Transform>("Prefabs/Ghostbook/ActivitySymbolsBig/BigSymbols_"+acResultData.getAcID());
		//Transform activitySymbolHolder = transform.FindChild("ActivityButton").FindChild("Activity Symbol");
		//activitySymbolHolder.GetComponent<SpriteRenderer>().sprite = reqAcSymbol.GetComponent<SpriteRenderer>().sprite;
	}

	protected void selectPage(int para_pageID)
	{
		// Deactivate old stuff.
		transform.FindChild("ResultScreens_A").gameObject.SetActive(false);
		transform.FindChild("ResultScreens_B").gameObject.SetActive(false);
		transform.FindChild("ResultScreens_C").gameObject.SetActive(false);
		transform.FindChild("PreviousButton").gameObject.SetActive(true);
		transform.FindChild("NextButton").gameObject.SetActive(true);
		previousPageAvailable = true;
		nextPageAvailable = true;


		string[] pageLetterIDs = {"A","B","C"};
		string reqPageObjName = "ResultScreens_"+pageLetterIDs[para_pageID];
	
		Transform reqResultScreenPage = transform.FindChild(reqPageObjName);
		reqResultScreenPage.gameObject.SetActive(true);

		Transform subPagesCollection = reqResultScreenPage.FindChild("Pages");
		for(int i=0; i<subPagesCollection.childCount; i++)
		{
			subPagesCollection.GetChild(i).gameObject.SetActive(false);
		}
		subPagesCollection.gameObject.SetActive(true);

		buildPage(para_pageID,reqResultScreenPage.gameObject);
		// Note: inherited viewers are in charge of activating the necessary sub page when buildPage is called.

		
		if(para_pageID == 0)
		{
			previousPageAvailable = false;
			transform.FindChild("PreviousButton").gameObject.SetActive(false);
		}
		else if(para_pageID == 2)
		{
			nextPageAvailable = false;
			transform.FindChild("NextButton").gameObject.SetActive(false);
		}

		currPage = para_pageID;
	}

	protected new void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string, GUIStyle>();

		float targetWidth = 1280f;
		float targetHeight = 800f;
		Vector3 scaleForCurrRes = new Vector3((Screen.width * 1.0f)/targetWidth,(Screen.height * 1.0f)/targetHeight,1f);

		GUIStyle pageTitleStyle = new GUIStyle(GUI.skin.label);
		pageTitleStyle.alignment = TextAnchor.MiddleLeft;
		pageTitleStyle.fontSize = (int) (80 * scaleForCurrRes.x);

		GUIStyle fieldTitleStyle = new GUIStyle(GUI.skin.label);
		fieldTitleStyle.alignment = TextAnchor.MiddleLeft;
		fieldTitleStyle.fontSize = (int) (40 * scaleForCurrRes.x);
		
		GUIStyle fieldContentStyle = new GUIStyle(GUI.skin.label);
		fieldContentStyle.alignment = TextAnchor.MiddleLeft;
		fieldContentStyle.fontSize = (int) (40 * scaleForCurrRes.x);

		GUIStyle fieldContentLeftMidStyle = new GUIStyle(GUI.skin.label);
		fieldContentLeftMidStyle.alignment = TextAnchor.MiddleLeft;
		fieldContentLeftMidStyle.fontSize = (int) (40 * scaleForCurrRes.x);

		Texture2D clearBlackTex = new Texture2D(1,1);
		clearBlackTex.SetPixel(0,0,ColorUtils.convertColor(200,0,0,100));
		clearBlackTex.Apply();

		Texture2D blackTex = new Texture2D(1,1);
		blackTex.SetPixel(1,1,Color.black);
		blackTex.Apply();

		Texture2D reviseBtnBackground = clearBlackTex;


		GUIStyle reviseButtonStyle = new GUIStyle(GUI.skin.button);
		reviseButtonStyle.wordWrap = true;
		reviseButtonStyle.fontSize = (int) (30 * scaleForCurrRes.x);
		reviseButtonStyle.normal.background = reviseBtnBackground;
		reviseButtonStyle.normal.textColor = Color.black;
		reviseButtonStyle.active.background = reviseBtnBackground;
		reviseButtonStyle.active.textColor = Color.black;
		reviseButtonStyle.focused.background = reviseBtnBackground;
		reviseButtonStyle.focused.textColor = Color.black;
		reviseButtonStyle.hover.background = reviseBtnBackground;
		reviseButtonStyle.hover.textColor = Color.black;

		GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
		btnStyle.wordWrap = true;
		btnStyle.normal.textColor = Color.black;

		GUIStyle tableHorizSeparatorBoxStyle = new GUIStyle(GUI.skin.box);
		tableHorizSeparatorBoxStyle.normal.background = blackTex;

		availableGUIStyles.Add("PageTitle",pageTitleStyle);
		availableGUIStyles.Add("FieldTitle",fieldTitleStyle);
		availableGUIStyles.Add("FieldContent",fieldContentStyle);
		availableGUIStyles.Add("FieldContentML",fieldContentLeftMidStyle);
		availableGUIStyles.Add("SeparatorBox",tableHorizSeparatorBoxStyle);
		availableGUIStyles.Add("ReviseButton",reviseButtonStyle);
		availableGUIStyles.Add("Button",btnStyle);
		hasInitGUIStyles = true;
	}

	public virtual void buildPage(int para_pageID, GameObject para_pageParent)
	{
		// Extend this.
	}

	public virtual int pageRedirector(int para_fromPage, int para_toPage)
	{
		// Extend this if necessary.
		return para_toPage;
	}

	protected void createMessageWindow(string para_title, string para_message, string para_listenerName, CustomActionListener para_listener)
	{
		Transform messageWindowPrefab = Resources.Load<Transform>("Prefabs/MessageWindow");
		Rect origPrefab2DBounds = CommonUnityUtils.get2DBounds(messageWindowPrefab.FindChild("WindowPane").renderer.bounds);
		GameObject nwMessageWindow = WorldSpawnHelper.initWorldObjAndBlowupToScreen(messageWindowPrefab,origPrefab2DBounds);
		nwMessageWindow.name = "MessageWindow";
		nwMessageWindow.transform.position = new Vector3(Camera.main.transform.position.x,
		                                                Camera.main.transform.position.y,
		                                                Camera.main.transform.position.z + 5f);

		MessageWindow mwScript = nwMessageWindow.AddComponent<MessageWindow>();
		mwScript.init(para_title,para_message);
		mwScript.registerListener(para_listenerName,para_listener);
	}

	private void triggerSoundAtCamera(string para_soundFileName)
	{
		triggerSoundAtCamera(para_soundFileName,1f,false);
	}

	Transform sfxPrefab;

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
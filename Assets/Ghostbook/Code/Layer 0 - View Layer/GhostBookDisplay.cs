/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

using UnityEngine;
using System.Collections.Generic;

public class GhostBookDisplay: ILearnRWScenario, CustomActionListener
{


	private Transform ghostBookWordBoxPrefab;

	private Transform textBannerNarrowPrefab;
	//private Transform textBannerThickPrefab;
	private Transform newsIconPrefab;

	private Transform[] unlockedCharacterFaces;
	private Transform[] lockedCharacterFaces;
	private Transform[] characterSmallFaces;
	private Transform[] activitySymbols;

	private Sprite[] photoPageCompletionStatusIcons;

	//public Transform albumInnerPrefab;
	
	private Material maskShaderMaterial;
	private Material maskShaderMaterialForText;


	bool[] upAxisArr = new bool[3] { false,true,false };


	float[] tabZLevels;

	List<Rect> tmpContactGridItemBounds;
	List<Rect> contactNormTexBounds;
	List<Rect> lockedContactNormTexBounds;
	//List<Rect> smallFacesNormTexBounds;
	//List<Rect> activitySymbolsNormTexBounds;
	bool[] tabSelectionArr;
	int currSelectedTabID;
	int currSelectedPageID;
	//int prevSelectedTabID;
	//int prevSelectedPageID;
	Dictionary<int,int> tabToCurrPageMap;

	Stack<int[]> hiearchyStack;

	
	public Vector2 scrollPos;
	Vector2 detailedBioScrollPos;
	Vector2 eventsScrollPos;
	Vector2 newsFeedScrollPos;
	//Vector2 albumCollectionScrollPos;
	float totalHeightForContactsScroll;


	GhostbookManagerLight gbMang;

	bool isOpen;

	int sectionIndex = 0;

	ContactPageInfoSnippit playerSelectedCharSnippit;
	EventSummarySnippit playerSelectedEvent;
	NewsItem playerSelectedNewsItem;
	List<string> playerSelectedCharBioSections;
	PhotoAlbum playerSelectedCharAlbum;
	List<Rect> selectedCharAlbumUiBounds;
	List<Rect> selectedCharAlbumIconsUiBounds;
	List<Rect> selectedCharAlbumCaptionUiBounds;


	//float virtualWidth = 1920.0f;
	//float virtualHeight = 1080.0f;
	//Matrix4x4 originalTRSMat;
	//Matrix4x4 customTRSMat;



	GameObject ghostbookGObj;

	 
	// Tmp vars.
	//Rect tmpRect = new Rect(0,0,1,1);
	Vector2 tmpVect = new Vector2();
	//Rect noEventBannerNormBounds;
	Rect noNewsBannerNormBounds;
	//Rect newsIconNormTexBounds;
	List<ContactPortraitSnippit> cListByID;
	List<EventSummarySnippit> availableEvents;
	List<NewsItem> availableNews;
	Texture[] albumIconTextures;
	Rect[] albumIconNormTexCoords;

	bool fixToCamera = true; 

	bool isInAlbumInnerView = false;
	//bool isInPhotoCloseupView = false;
	//bool isInTeacherView = false;

	//GUIStyle oldVerticalScrollbarThumbStyle;
	//GUIStyle oldHorizontalScrollbarThumbStyle;

	Transform scrollableContactGrid;
	Rect scrollableGridZeroWorldBounds;
	Rect scrollableGridInnerGUIBounds;
	ClickDetector gridClickDetector;

	Transform scrollableEventsPane;
	Rect scrollableEventspaneZeroWorldBounds;
	Rect scrollableEventsPaneInnerGUIBounds;
	ClickDetector eventsPaneClickDetector;

	List<EventsPageInnerGUICollection> eventsSectionInnerGUIs;

	//bool performEventClickProcedureFlag = false;
	//int eventClickIndex = -1;

	public bool setupReady = false;

	
	ProgressScript control;


	public void init(ProgressScript c, GhostbookManagerLight gb)
	{

		unlockedCharacterFaces = new Transform[34] ;
		lockedCharacterFaces = new Transform[34] ;
		characterSmallFaces = new Transform[34] ;
		
		for(int i =0;i<34;i++){
			
			unlockedCharacterFaces[i] = ((GameObject)Resources.Load("Prefabs/Ghostbook/UnlockedProfilePics/UnlockedPortrait_"+i)).transform;
			//			unlockedCharacterFaces[i] = (Transform)Instantiate(Resources.Load("UnlockedPortrait_"+i));
			lockedCharacterFaces[i] = ((GameObject)Resources.Load("Prefabs/Ghostbook/LockedProfilePics/LockedPortrait_"+i)).transform;
			characterSmallFaces[i] = ((GameObject)Resources.Load("Prefabs/Ghostbook/SmallHeads/SmallHeads_"+i)).transform;
			
			
		}
		
		activitySymbols= new Transform[9];
		activitySymbols[0] = ((GameObject)Resources.Load("Prefabs/Ghostbook/ActivitySymbols/SmallSymbols_MAIL_SORTER")).transform;
		activitySymbols[1] = ((GameObject)Resources.Load("Prefabs/Ghostbook/ActivitySymbols/SmallSymbols_WHAK_A_MOLE")).transform;
		activitySymbols[2] = ((GameObject)Resources.Load("Prefabs/Ghostbook/ActivitySymbols/SmallSymbols_ENDLESS_RUNNER")).transform;
		activitySymbols[3] = ((GameObject)Resources.Load("Prefabs/Ghostbook/ActivitySymbols/SmallSymbols_HARVEST")).transform;
		activitySymbols[4] = ((GameObject)Resources.Load("Prefabs/Ghostbook/ActivitySymbols/SmallSymbols_SERENADE_HERO")).transform;
		activitySymbols[5] = ((GameObject)Resources.Load("Prefabs/Ghostbook/ActivitySymbols/SmallSymbols_MOVING_PATHWAYS")).transform;
		activitySymbols[6] = ((GameObject)Resources.Load("Prefabs/Ghostbook/ActivitySymbols/SmallSymbols_EYE_EXAM")).transform;
		activitySymbols[7] = ((GameObject)Resources.Load("Prefabs/Ghostbook/ActivitySymbols/SmallSymbols_TRAIN_DISPATCHER")).transform;
		activitySymbols[8] = ((GameObject)Resources.Load("Prefabs/Ghostbook/ActivitySymbols/SmallSymbols_DROP_CHOPS")).transform;
		
		photoPageCompletionStatusIcons = new Sprite[5] ;
		
		for(int i = 0;i<5;i++){
			photoPageCompletionStatusIcons[i] = Resources.Load<Sprite>("Prefabs/Ghostbook/PhotoIcon_V"+i);
			
		}
		
		maskShaderMaterial = (Material)Resources.Load("Prefabs/Ghostbook/ScrollMaskMat");
		maskShaderMaterialForText = (Material)Resources.Load("Prefabs/Ghostbook/ScrollMaskMatForText");
		
		ghostBookWordBoxPrefab = ((GameObject)Resources.Load("Prefabs/Ghostbook/GBookWordBox")).transform;
		
		textBannerNarrowPrefab = ((GameObject)Resources.Load("Prefabs/Ghostbook/TextBannerNarrow")).transform;
		//textBannerThickPrefab = ((GameObject)Resources.Load("Prefabs/Ghostbook/TextBannerThick")).transform;
		newsIconPrefab = ((GameObject)Resources.Load("Prefabs/Ghostbook/NewsBanner")).transform;


		this.loadTextures();
		this.prepUIBounds();

		//customTRSMat = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(Screen.width/virtualWidth,Screen.height/virtualHeight,1.0f));

		this.gbMang = gb;
		this.control = c;

		//albumCollectionScrollPos = new Vector2();

		//smallFacesNormTexBounds = extractNormTexBoundsForMultisprites(characterSmallFaces);
		//activitySymbolsNormTexBounds = extractNormTexBoundsForMultisprites(activitySymbols);

		//constructTabbedGhostbook();
		//loadContactGridPage();
		//ghostbookGObj.SetActive(false);

		setupReady = true;
		//this.enabled = false;
	}




	void initGUI(){

		//originalTRSMat = GUI.matrix;
		
		//GUI.skin.verticalScrollbar = GUIStyle.none;
		//GUI.skin.horizontalScrollbar = GUIStyle.none;
		
		float targetWidth = 1280f;
		float targetHeight = 800f;
		Vector3 scaleForCurrRes = new Vector3((Screen.width * 1.0f)/targetWidth,(Screen.height * 1.0f)/targetHeight,1f);
		
		
		GUIStyle midCentredSmallLabel = new GUIStyle(GUI.skin.label);
		midCentredSmallLabel.fontSize = (int) (30f  * scaleForCurrRes.x);
		midCentredSmallLabel.alignment = TextAnchor.MiddleCenter;
		
		GUIStyle midLeftSmallLabel = new GUIStyle(GUI.skin.label);
		midLeftSmallLabel.fontSize = (int) (30f  * scaleForCurrRes.x);
		midLeftSmallLabel.alignment = TextAnchor.MiddleLeft;
		
		GUIStyle midLeftTinyLabel = new GUIStyle(GUI.skin.label);
		midLeftTinyLabel.fontSize = (int) (25f  * scaleForCurrRes.x);
		midLeftTinyLabel.alignment = TextAnchor.MiddleLeft;
		
		GUIStyle midLeftMinisculeLabel = new GUIStyle(GUI.skin.label);
		midLeftMinisculeLabel.fontSize = (int) (20f  * scaleForCurrRes.x);
		midLeftMinisculeLabel.alignment = TextAnchor.MiddleLeft;
		
		/*GUIStyle midCentredLargeLabel = new GUIStyle(GUI.skin.label);
			midCentredLargeLabel.fontSize = (int) (20f  * scaleForCurrRes.x);
			midCentredLargeLabel.alignment = TextAnchor.MiddleCenter;*/
		
		GUIStyle topLeftSmallLabel = new GUIStyle(GUI.skin.label);
		topLeftSmallLabel.fontSize = (int) (20f * scaleForCurrRes.x);
		topLeftSmallLabel.alignment = TextAnchor.UpperLeft;
		
		GUIStyle noVerticalScrollbarStyle = new GUIStyle(GUI.skin.verticalScrollbar);
		noVerticalScrollbarStyle = GUIStyle.none;
		
		GUIStyle noHorizontalScrollbarStyle = new GUIStyle(GUI.skin.horizontalScrollbar);
		noHorizontalScrollbarStyle = GUIStyle.none;
		
		float scrollbarThickness = 6;
		
		GUIStyle verticalScrollbarStyle = new GUIStyle(GUI.skin.verticalScrollbar);
		verticalScrollbarStyle.fixedWidth = scrollbarThickness;
		
		GUIStyle horizontalScrollbarStyle = new GUIStyle(GUI.skin.horizontalScrollbar);
		horizontalScrollbarStyle.fixedHeight = scrollbarThickness;
		
		//oldVerticalScrollbarThumbStyle = new GUIStyle(GUI.skin.verticalScrollbarThumb);
		//oldHorizontalScrollbarThumbStyle = new GUIStyle(GUI.skin.horizontalScrollbarThumb);
		
		GUIStyle origVertScrollbarThumb = new GUIStyle(GUI.skin.verticalScrollbarThumb);
		GUIStyle origHorizScrollbarThumb = new GUIStyle(GUI.skin.horizontalScrollbarThumb);
		
		GUIStyle nwVertScrollbarThumb = new GUIStyle(GUI.skin.verticalScrollbarThumb);
		nwVertScrollbarThumb.fixedWidth = scrollbarThickness;
		GUIStyle nwHorizScrollbarThumb = new GUIStyle(GUI.skin.horizontalScrollbarThumb);
		nwHorizScrollbarThumb.fixedWidth = scrollbarThickness;
		
		availableGUIStyles = new Dictionary<string, GUIStyle>();
		
		availableGUIStyles.Add("OrigVertScrollbarThumb",origVertScrollbarThumb);
		availableGUIStyles.Add("OrigHorizScrollbarThumb",origHorizScrollbarThumb);
		availableGUIStyles.Add("NwVertScrollbarThumb",nwVertScrollbarThumb);
		availableGUIStyles.Add("NwHorizScrollbarThumb",nwHorizScrollbarThumb);
		
		GUI.skin.verticalScrollbarThumb = nwVertScrollbarThumb;
		GUI.skin.horizontalScrollbarThumb = nwHorizScrollbarThumb;
		GUI.skin.verticalScrollbar = verticalScrollbarStyle;
		GUI.skin.horizontalScrollbar = horizontalScrollbarStyle;
		
		
		availableGUIStyles.Add("LabelMidCSmall",midCentredSmallLabel);
		availableGUIStyles.Add("LabelMidLSmall",midLeftSmallLabel);
		availableGUIStyles.Add("LabelMidLTiny",midLeftTinyLabel);
		availableGUIStyles.Add("LabelMidLMiniscule",midLeftMinisculeLabel);
		availableGUIStyles.Add("LabelTopLSmall",topLeftSmallLabel);
		availableGUIStyles.Add("NoVerticalScrollBar",noVerticalScrollbarStyle);
		availableGUIStyles.Add("NoHorizScrollBar",noHorizontalScrollbarStyle);
		availableGUIStyles.Add("VerticalScrollBar",verticalScrollbarStyle);
		availableGUIStyles.Add("HorizontalScrollBar",horizontalScrollbarStyle);
		
		hasInitGUIStyles = true;

	}

	void OnGUI()
	{

		GUI.color = Color.white;

		if( ! hasInitGUIStyles)
		{
			initGUI();
		}
		else
		{

			if( isOpen)
			{

				if((isOpen)&&( ! isInAlbumInnerView))
				{
					//GUI.matrix = customTRSMat;
					//GUI.matrix = originalTRSMat;

					GUI.color = Color.clear;

					if(GUI.Button(uiBounds["ContactsTitle"],""))
					{
						Debug.Log("Selected Contacts Tab");

						//int pageToSelect = 0;

						if((currSelectedTabID == 0)&&(currSelectedPageID != 0))
						{
							tabToCurrPageMap[0] = 0;
							ghostbookGObj.transform.FindChild("ContactsTab").FindChild("Backdrop").FindChild("ContactsTabHeader").renderer.enabled = true;
							ghostbookGObj.transform.FindChild("ContactsTab").FindChild("Backdrop").FindChild("BackButton").renderer.enabled = false;

							goUpDirectory();
						}
						else
						{
							selectTab(0,0);//tabToCurrPageMap[0]);
						}

					}

					if(GUI.Button(uiBounds["EventsTitle"],""))
					{
						Debug.Log("Selected Events Tab");

						if((currSelectedTabID == 1)&&(currSelectedPageID != 0))
						{
							//ghostbookGObj.transform.FindChild("EventsTab").FindChild("Backdrop").FindChild("Title").FindChild("Text").GetComponent<TextMesh>().text = "Events";
							goUpDirectory();
						}
						else
						{
							//loadEventsPage();
							loadEventsPageUpgrade2();
							selectTab(1,0);
						}
					}

					if(GUI.Button(uiBounds["NewsFeedTitle"],""))
					{
						Debug.Log("Selected News Feed Tab");

						if((currSelectedTabID == 2)&&(currSelectedPageID != 0))
						{
							//ghostbookGObj.transform.FindChild("NewsFeedTab").FindChild("Backdrop").FindChild("Title").FindChild("Text").GetComponent<TextMesh>().text = "Newsfeed";
							goUpDirectory();
						}
						else
						{
							loadNewsFeedPage();
							selectTab(2,0);
						}
					}

					GUI.color = Color.white;

				
					if((currSelectedTabID == 0)&&(currSelectedPageID == 0))
					{
						// Contacts tab selected.

						Rect guiLocationBounds = uiBounds["ContactsGridScrollArea"];
						Rect innerScrollBounds = scrollableGridInnerGUIBounds;//new Rect(0,0,guiLocationBounds.width,totalHeightForContactsScroll);
						
								
						
						if((Application.platform == RuntimePlatform.Android)
						   ||(Application.platform == RuntimePlatform.IPhonePlayer))
						{
							if(Input.touchCount == 1)
							{
								tmpVect.x = Input.touches[0].position.x;
								tmpVect.y = Screen.height - Input.touches[0].position.y;
								
								if(guiLocationBounds.Contains(tmpVect))
								{
									scrollPos.y += (Input.touches[0].deltaPosition.y * 1f);
								}
							}
						}
						

						
						scrollPos = GUI.BeginScrollView(guiLocationBounds,scrollPos,innerScrollBounds,GUIStyle.none,availableGUIStyles["VerticalScrollBar"]);


						Vector3 tmpGridPos = scrollableContactGrid.position;
						tmpGridPos.y = ((scrollableGridZeroWorldBounds.y - (scrollableGridZeroWorldBounds.height/2f)) + ((scrollPos.y / scrollableGridInnerGUIBounds.height) * scrollableGridZeroWorldBounds.height));
						scrollableContactGrid.position = tmpGridPos;

						GUI.EndScrollView();
	
					}
					else if((currSelectedTabID == 0)&&(currSelectedPageID == 1))
					{
						// Contacts tab selected + show detailed contact profile.


						string[] sectionText = LocalisationMang.getBioSection(playerSelectedCharSnippit.getCharID(),sectionIndex);


						GUI.color = Color.black;
						GUI.Label(uiBounds["CharProfileBioTitle"],sectionText[0],availableGUIStyles["LabelMidLTiny"]);

						if(gbMang.getContactBioSectionStatus(playerSelectedCharSnippit.getCharID(),sectionIndex))
							GUI.Label(uiBounds["CharProfileBio"],sectionText[1],availableGUIStyles["LabelMidLTiny"]);
						else
							GUI.Label(uiBounds["CharProfileBio"],"?",availableGUIStyles["LabelMidLTiny"]);

						GUI.color = Color.clear;


						if(GUI.Button(uiBounds["RightButton"],"Go right")){
							sectionIndex = (sectionIndex+1)%7;
						}

						if(GUI.Button(uiBounds["LeftButton"],"Go left")){
							if (sectionIndex == 0)
								sectionIndex = 6;
							else
								sectionIndex--;
						}

						if(GUI.Button(uiBounds["CharProfileMap"],"Map View"))
						{

							control.findCharacter(playerSelectedCharSnippit.getCharID());
							return;
							//loadMapView();
							//selectTab(0,2);
						}


						GUI.color = Color.clear;
						if(GUI.Button(uiBounds["CharProfileAlbum"],""))//TODO
						{
							// Has clicked on the photo album.
														
							Vector3 innerAlbumObjSpawnPt = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,ghostbookGObj.transform.position.z - 0.4f);
							Transform nwInnerAlbumObj = ((GameObject)Instantiate(Resources.Load("Prefabs/Ghostbook/NewStuff/PhotoAlbumInner"),innerAlbumObjSpawnPt,Quaternion.identity)).transform;
//							Transform nwInnerAlbumObj = (Transform) Instantiate(Resources.Load("PhotoAlbumInner"),innerAlbumObjSpawnPt,Resources.Load("PhotoAlbumInner").rotation);
							nwInnerAlbumObj.gameObject.name = "AlbumInnerView";
							nwInnerAlbumObj.parent = Camera.main.transform;
							CommonUnityUtils.setSortingOrderOfEntireObject(nwInnerAlbumObj.gameObject,1000);
							AlbumInnerViewScript innerAVS = nwInnerAlbumObj.gameObject.AddComponent<AlbumInnerViewScript>();
							innerAVS.registerListener("GhostbookDisplay",this);
							innerAVS.init(playerSelectedCharAlbum,photoPageCompletionStatusIcons,control);
							
							SoundPlayerScript sps = GameObject.Find("GlobObj").GetComponent<SoundPlayerScript>();
							if(sps != null) { sps.triggerSoundAtCamera("ScrapbookOpen",1); }
							
							isInAlbumInnerView = true;
						}

						GUI.color = Color.white;
					}
					else if((currSelectedTabID == 0)&&(currSelectedPageID == 2))
					{
						// Map view, recent selections page for a character.

						GUI.color = Color.black;
						GUI.Label(uiBounds["MapViewCharName"],playerSelectedCharSnippit.getName(),availableGUIStyles["LabelMidCSmall"]);
						GUI.Label(uiBounds["MapViewSelectedLoc"],"Default Location",availableGUIStyles["LabelMidCSmall"]);
						if(GUI.Button(uiBounds["MapViewBtnVisit"],"Visit"))
						{
							control.findCharacter(playerSelectedCharSnippit.getCharID());

						}
						GUI.color = Color.white;
					}
					else if((currSelectedTabID == 0)&&(currSelectedPageID == 3))
					{
						// Detailed Bio View.
						GUI.color = Color.black;
						//GUI.Label(uiBounds["DetailedBioCharName"],playerSelectedCharSnippit.getName(),availableGUIStyles["LabelMidCSmall"]);


						Rect detailedBioTA = uiBounds["DetailedBioTextArea"];

						if((Application.platform == RuntimePlatform.Android)
						   ||(Application.platform == RuntimePlatform.IPhonePlayer))
						{
							if(Input.touchCount == 1)
							{
								tmpVect.x = Input.touches[0].position.x;
								tmpVect.y = Screen.height - Input.touches[0].position.y;
								
								if(detailedBioTA.Contains(tmpVect))
								{
									detailedBioScrollPos.y += (Input.touches[0].deltaPosition.y * 1f);
								}
							}
						}


						GUI.color = Color.white;
						GUILayout.BeginArea(detailedBioTA);
						detailedBioScrollPos = GUILayout.BeginScrollView(detailedBioScrollPos,GUIStyle.none,availableGUIStyles["VerticalScrollBar"],GUILayout.Width(detailedBioTA.width),GUILayout.Height(detailedBioTA.height));

						GUI.color = Color.black;
						if((playerSelectedCharBioSections == null)||(playerSelectedCharBioSections.Count == 0))
						{
							GUILayout.Label("No Bio Available",availableGUIStyles["LabelTopLSmall"]);
						}
						else
						{
							for(int k=0; k<playerSelectedCharBioSections.Count; k++)
							{
								GUILayout.Label(playerSelectedCharBioSections[k],availableGUIStyles["LabelTopLSmall"]);
							}
						}
					
						GUILayout.EndScrollView();
						GUILayout.EndArea();
						GUI.color = Color.white;
					}
					else if((currSelectedTabID == 1)&&(currSelectedPageID == 0))
					{
						// Events main page.


						int eventCount = availableEvents.Count;
						
						if(eventCount == 0)
						{

							Rect guiLocationBounds = uiBounds["EventsGUIScrollArea"];
							GUI.color = Color.black;
							GUI.Label(guiLocationBounds,LocalisationMang.translate("No available events.\nCome back later!"),availableGUIStyles["LabelMidCSmall"]);
							GUI.color = Color.white;
						}
						else
						{
							Rect guiLocationBounds = uiBounds["EventsGUIScrollArea"];
							Rect innerScrollBounds = scrollableEventsPaneInnerGUIBounds;
							
							
							
							if((Application.platform == RuntimePlatform.Android)
							   ||(Application.platform == RuntimePlatform.IPhonePlayer))
							{
								if(Input.touchCount == 1)
								{
									tmpVect.x = Input.touches[0].position.x;
									tmpVect.y = Screen.height - Input.touches[0].position.y;
									
									if(guiLocationBounds.Contains(tmpVect))
									{
										scrollPos.y += (Input.touches[0].deltaPosition.y * 1f);
									}
								}
							}
							
							
							
							scrollPos = GUI.BeginScrollView(guiLocationBounds,scrollPos,innerScrollBounds,GUIStyle.none,availableGUIStyles["VerticalScrollBar"]);
							
							
							Vector3 tmpPanePos = scrollableEventsPane.position;
							tmpPanePos.y = ((scrollableEventspaneZeroWorldBounds.y - (scrollableEventspaneZeroWorldBounds.height/2f)) + ((scrollPos.y / scrollableEventsPaneInnerGUIBounds.height) * scrollableEventspaneZeroWorldBounds.height));
							scrollableEventsPane.position = tmpPanePos;

							//GUIStyle evTextLabelStyleML = availableGUIStyles["LabelMidLSmall"];
							GUIStyle evTextLabelStyleMC = availableGUIStyles["LabelMidCSmall"];
							GUIStyle evTextLabelStyleMLTiny = availableGUIStyles["LabelMidLTiny"];

							GUI.color = Color.black;
							if(eventsSectionInnerGUIs != null)
							{
								for(int i=0; i<eventsSectionInnerGUIs.Count; i++)
								{
									EventsPageInnerGUICollection tmpInnerGUICol = eventsSectionInnerGUIs[i];
									EventSummarySnippit tmpEvSnip = availableEvents[i];

									GUI.Label(tmpInnerGUICol.friendlyShoutoutBounds,tmpEvSnip.getEventText(),evTextLabelStyleMLTiny);
									GUI.Label(tmpInnerGUICol.langAreaTextFieldBounds,tmpEvSnip.getReadableLA(),evTextLabelStyleMC);
									GUI.Label(tmpInnerGUICol.difficultyTextFieldBounds,tmpEvSnip.getReadableDiff(),evTextLabelStyleMC);
								}
							}
							GUI.color = Color.white;

							
							GUI.EndScrollView();
						}



						

					}
					else if((currSelectedTabID == 2)&&(currSelectedPageID == 0))
					{
						Rect newsfeedGUIScrollArea = uiBounds["NewsFeedGUIScrollArea"];
						
						
						int newsfeedCount = availableNews.Count;
						
						if(newsfeedCount == 0)
						{
							Rect noNewsBannerBounds = uiBounds["NoNewsBanner"];
							SpriteRenderer reqSRend = textBannerNarrowPrefab.GetComponent<SpriteRenderer>();
							GUI.DrawTextureWithTexCoords(noNewsBannerBounds,reqSRend.sprite.texture,noNewsBannerNormBounds);
							GUI.color = Color.black;
							GUI.Label(noNewsBannerBounds,"No available news",availableGUIStyles["LabelMidCSmall"]);
							GUI.color = Color.white;
						}
						else
						{


							if((Application.platform == RuntimePlatform.Android)
							   ||(Application.platform == RuntimePlatform.IPhonePlayer))
							{
								if(Input.touchCount == 1)
								{
									tmpVect.x = Input.touches[0].position.x;
									tmpVect.y = Screen.height - Input.touches[0].position.y;
									
									if(newsfeedGUIScrollArea.Contains(tmpVect))
									{
										newsFeedScrollPos.y += (Input.touches[0].deltaPosition.y * 1f);
									}
								}
							}



							
							Rect narrowBannerGuideBounds = uiBounds["GuideTextBannerNarrow"];
							//Rect newsIconGuideBounds = uiBounds["GuideNewsIcon"];

							Rect totalBoundsForOneRow = new Rect((newsfeedGUIScrollArea.width/2f) - ((narrowBannerGuideBounds.width)/2f),
							                                     					   10,
							                                     					   (narrowBannerGuideBounds.width),
							                                    					   narrowBannerGuideBounds.height);

							//Rect rowIconBounds = new Rect(totalBoundsForOneRow.x,totalBoundsForOneRow.y,newsIconGuideBounds.width,newsIconGuideBounds.height);
							Rect rowBannerBounds = new Rect(totalBoundsForOneRow.x,
							                                				   totalBoundsForOneRow.y + (totalBoundsForOneRow.height/2f) - (narrowBannerGuideBounds.height/2f),
							                                				   narrowBannerGuideBounds.width,
							                                				   narrowBannerGuideBounds.height);
							                              					

							Rect innerScrollBounds = new Rect(0,0,newsfeedGUIScrollArea.width,(rowBannerBounds.height * ((newsfeedCount+2) * 1.0f)));
							newsFeedScrollPos = GUI.BeginScrollView(newsfeedGUIScrollArea,newsFeedScrollPos,innerScrollBounds,GUIStyle.none,availableGUIStyles["VerticalScrollBar"]);
							
							
							for(int k=0; k<newsfeedCount; k++)
							{
								NewsItem tmpNewsItem = availableNews[k];



								//TODO replace newsitems with events
								/***


										EventsPageInnerGUICollection tmpInnerGUICol = eventsSectionInnerGUIs[i];
										EventSummarySnippit tmpEvSnip = availableEvents[i];
										
										GUI.Label(tmpInnerGUICol.friendlyShoutoutBounds,tmpEvSnip.getEventText(),evTextLabelStyleMLTiny);
										GUI.Label(tmpInnerGUICol.langAreaTextFieldBounds,tmpEvSnip.getReadableLA(),evTextLabelStyleMC);
										GUI.Label(tmpInnerGUICol.difficultyTextFieldBounds,tmpEvSnip.getReadableDiff(),evTextLabelStyleMC);
				

								******/

								//GUI.Box(rowBounds,"");
								GUI.color = Color.clear;
								if(GUI.Button(rowBannerBounds,""))
								{
									playerSelectedNewsItem = tmpNewsItem;
									
									if(playerSelectedNewsItem is NIPastActivity)
									{
										NIPastActivity castNI = (NIPastActivity) playerSelectedNewsItem;

										control.launchQuest(new ExternalParams(castNI.getActivityPlayed(),castNI.getQuestGiverID(),castNI.getLangArea(),castNI.getDifficulty(),castNI.getLevel(),false), LocalisationMang.getOwnerNpcOfActivity(castNI.getActivityPlayed()),"NewsFeed",Mode.PLAY);


									}
								}
								GUI.color = Color.white;
								


								SpriteRenderer reqSRend = newsIconPrefab.GetComponent<SpriteRenderer>();
								//GUI.DrawTextureWithTexCoords(rowIconBounds,reqSRend.sprite.texture,newsIconNormTexBounds);

								
								reqSRend = textBannerNarrowPrefab.GetComponent<SpriteRenderer>();
								GUI.DrawTextureWithTexCoords(rowBannerBounds,reqSRend.sprite.texture,noNewsBannerNormBounds);
								GUI.color = Color.black;
								GUI.Label(rowBannerBounds,tmpNewsItem.getNewsText(),availableGUIStyles["LabelMidLTiny"]);
								GUI.color = Color.white;
								

								//rowIconBounds.y += totalBoundsForOneRow.height + (20f);
								rowBannerBounds.y += totalBoundsForOneRow.height + (20f); // 20f == padding.
							}
							
							GUI.EndScrollView();
						}
					}
				}
			}
		}


	/*	if(performEventClickProcedureFlag)
		{
			performEventClickProcedure();
			eventClickIndex = -1;
			performEventClickProcedureFlag = false;
		}*/
	}

	void Update()
	{
		if(fixToCamera)
		{
			if((ghostbookGObj != null)
			&&(Camera.main.gameObject != null))
			{
				ghostbookGObj.transform.parent = Camera.main.gameObject.transform;
				fixToCamera = false;
			}
		}
	}


	public bool getIsOpenFlag() { return isOpen; }


	public void openGhostbook(int charID){

		openGhostbook();

		playerSelectedCharSnippit = gbMang.getContactPageInfoSnippit(charID);
		playerSelectedCharAlbum = playerSelectedCharSnippit.getPhotoAlbum();
		//Debug.Log("OWNER "+playerSelectedCharAlbum.ownerID);
		loadFriendPage();
		tabToCurrPageMap[0] = 1;
		selectTab(0,1);

	}

	
	public void openGhostbook(int lA,int diff,int charID){
		

		openGhostbook(charID);

		GUI.color = Color.clear;
				
		Vector3 innerAlbumObjSpawnPt = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,ghostbookGObj.transform.position.z - 0.4f);
		Transform nwInnerAlbumObj = ((GameObject)Instantiate(Resources.Load("Prefabs/Ghostbook/NewStuff/PhotoAlbumInner"),innerAlbumObjSpawnPt,Quaternion.identity)).transform;

		nwInnerAlbumObj.gameObject.name = "AlbumInnerView";
		nwInnerAlbumObj.parent = Camera.main.transform;
		CommonUnityUtils.setSortingOrderOfEntireObject(nwInnerAlbumObj.gameObject,1000);
		AlbumInnerViewScript innerAVS = nwInnerAlbumObj.gameObject.AddComponent<AlbumInnerViewScript>();
		innerAVS.registerListener("GhostbookDisplay",this);
		innerAVS.init(playerSelectedCharAlbum,photoPageCompletionStatusIcons,control);
			
		SoundPlayerScript sps = GameObject.Find("GlobObj").GetComponent<SoundPlayerScript>();
		if(sps != null) { sps.triggerSoundAtCamera("ScrapbookOpen",1); }
			
		isInAlbumInnerView = true;

		GUI.color = Color.white;

		
	}

	public void finalInit(){

		scrollPos.x = 0;
		scrollPos.y = 0;
		constructTabbedGhostbook();
		loadContactGridPage();
		ghostbookGObj.SetActive(false);


	}


	public void openGhostbook()
	{

		finalInit();
		isOpen = true;

		scrollPos.x = 0;
		scrollPos.y = 0;

		//finalInit();


		ghostbookGObj.SetActive(true);
		selectTab(0,0);

		
		SoundPlayerScript sps = GameObject.Find("GlobObj").GetComponent<SoundPlayerScript>();
		if(sps != null)
		{
			sps.triggerSoundAtCamera("pop");
		}

		GUI.skin.verticalScrollbarThumb = availableGUIStyles["NwVertScrollbarThumb"];
		GUI.skin.horizontalScrollbarThumb = availableGUIStyles["NwHorizScrollbarThumb"];
	}



	public void closeGhostbook()
	{
		isOpen = false;
		fixToCamera = true;


		Transform innerAlbum = Camera.main.transform.FindChild("AlbumInnerView");
		if(innerAlbum!=null)
			DestroyImmediate(innerAlbum.gameObject);
		isInAlbumInnerView = false;

		//ghostbookGObj.SetActive(false);

		Destroy(ghostbookGObj);
		ghostbookGObj = null;
//		UnityEngine.Debug.LogWarning("Removed some GUI stuff");
//		GUI.skin.verticalScrollbarThumb = oldVerticalScrollbarThumbStyle;
//		GUI.skin.horizontalScrollbarThumb = oldHorizontalScrollbarThumbStyle;
		hasInitGUIStyles = false;
		clearVariables();

		if(gridClickDetector != null) { DestroyImmediate(gridClickDetector); }
		if(eventsPaneClickDetector != null) { DestroyImmediate(eventsPaneClickDetector); }
//*/
		
		SoundPlayerScript sps = GameObject.Find("GlobObj").GetComponent<SoundPlayerScript>();
		if(sps != null)
		{
			sps.triggerSoundAtCamera("blop");
		}

//		GUI.skin.verticalScrollbarThumb = availableGUIStyles["OrigVertScrollbarThumb"];
//		GUI.skin.horizontalScrollbarThumb = availableGUIStyles["OrigHorizScrollbarThumb"];
	}


	private void constructTabbedGhostbook()
	{
		// Construct tabbed ghostbook.

		ghostbookGObj = new GameObject("GhostbookGObj");

		
		Rect camWorld2DBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,false);
		
		Vector3 ghostBookCentrePt = new Vector3(camWorld2DBounds.x + (camWorld2DBounds.width/2f),
		                                        camWorld2DBounds.y - (camWorld2DBounds.height/2f),
		                                        Camera.main.gameObject.transform.position.z + 1);
		
		float tabZSeparation = 0.1f;
		Vector3 nxtTabSpawnPt = new Vector3(ghostBookCentrePt.x,ghostBookCentrePt.y,ghostBookCentrePt.z);


		Transform nwContactsTab = ((GameObject) Instantiate(Resources.Load("Prefabs/Ghostbook/ContactsTab"),nxtTabSpawnPt,Quaternion.identity)).transform;
		nwContactsTab.name = "ContactsTab";
		nxtTabSpawnPt.z += tabZSeparation;

		Transform nwEventsTab = ((GameObject) Instantiate(Resources.Load("Prefabs/Ghostbook/EventsTab"),nxtTabSpawnPt,Quaternion.identity)).transform;
		nwEventsTab.name = "EventsTab";
		nxtTabSpawnPt.z += tabZSeparation;
		Transform nwNewsFeedTab = ((GameObject) Instantiate(Resources.Load("Prefabs/Ghostbook/NewsFeedTab"),nxtTabSpawnPt,Quaternion.identity)).transform;
		nwNewsFeedTab.name = "NewsFeedTab";

		tabZLevels = new float[3];
		tabZLevels[0] = nwContactsTab.position.z;
		tabZLevels[1] = nwEventsTab.position.z;
		tabZLevels[2] = nwNewsFeedTab.position.z;
		
		nwContactsTab.parent = ghostbookGObj.transform;
		nwEventsTab.parent = ghostbookGObj.transform;
		nwNewsFeedTab.parent = ghostbookGObj.transform;



		GameObject contactsTitle = nwContactsTab.FindChild("Backdrop").FindChild("Title").gameObject;
		GameObject eventsTitle = nwEventsTab.FindChild("Backdrop").FindChild("Title").gameObject;
		GameObject newsFeedTitle = nwNewsFeedTab.FindChild("Backdrop").FindChild("Title").gameObject;

		// Adjust text size.
		//float maxCharSizeForWordBox = 0.04f;
		//WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() { nwContactsTitleBox, nwEventsTitleBox, nwNewsFeedTitleBox },maxCharSizeForWordBox);

		// Extract UI bounds for tab titles.
		Rect contactsTitleGUIbounds = WorldSpawnHelper.getWorldToGUIBounds(contactsTitle.renderer.bounds,upAxisArr);
		Rect eventsTitleGUIbounds = WorldSpawnHelper.getWorldToGUIBounds(eventsTitle.renderer.bounds,upAxisArr);
		Rect newsFeedTitleGUIbounds = WorldSpawnHelper.getWorldToGUIBounds(newsFeedTitle.renderer.bounds,upAxisArr);
		uiBounds.Add("ContactsTitle",contactsTitleGUIbounds);
		uiBounds.Add("EventsTitle",eventsTitleGUIbounds);
		uiBounds.Add("NewsFeedTitle",newsFeedTitleGUIbounds);
		
		// Destroy old reference objects.
		Destroy(contactsTitle);
		Destroy(eventsTitle);
		Destroy(newsFeedTitle);

		
		tabSelectionArr = new bool[3];
		for(int i=0; i<tabSelectionArr.Length; i++)
		{
			tabSelectionArr[i] = false;
		}

		tabToCurrPageMap = new Dictionary<int, int>() { {0,0}, {1,0}, {2,0} };
		hiearchyStack = new Stack<int[]>();
	}




	private void loadContactGridPage()
	{

		// Access ghostbook gobj through global variable.
		GameObject contactsTab = ghostbookGObj.transform.FindChild("ContactsTab").gameObject;
		GameObject contentArea = contactsTab.transform.FindChild("Backdrop").FindChild("ContentArea").gameObject;
		contentArea.renderer.sortingOrder = 152;


		Rect contentArea2DBounds = CommonUnityUtils.get2DBounds(contentArea.renderer.bounds);
		float contentAreaZVal = contentArea.transform.position.z;
		
		Rect contactsGridScrollGUIArea = new Rect(0,0,1,1);
		if( ! uiBounds.ContainsKey("ContactsGridScrollArea"))
		{
			contactsGridScrollGUIArea = WorldSpawnHelper.getWorldToGUIBounds(contentArea.renderer.bounds,upAxisArr);
			uiBounds.Add("ContactsGridScrollArea",contactsGridScrollGUIArea);
		}
		contactsGridScrollGUIArea = uiBounds["ContactsGridScrollArea"];


		int numItemsPerRow = 4;
		Rect worldSizeForFace = CommonUnityUtils.get2DBounds(unlockedCharacterFaces[0].gameObject.renderer.bounds);
		
		float totalWidthForFacesInRow = worldSizeForFace.width * (numItemsPerRow * 1.0f);
		float unUsedWidth = contentArea2DBounds.width - totalWidthForFacesInRow;
		float paddingWidth = unUsedWidth/((numItemsPerRow + 1)*1.0f);
		float paddingHeight = paddingWidth;





		// Create Sections.

		Transform genericWordBox = ghostBookWordBoxPrefab;

		Transform sectionTemplate = contactsTab.transform.FindChild("Pages").FindChild("ContactsGridPage").FindChild("SectionTemplate");
		Rect sectionTemplate2DBounds = CommonUnityUtils.get2DBounds(sectionTemplate.FindChild("SectionBackground").renderer.bounds);

		Vector3 currFreeSpaceTL = new Vector3(contentArea2DBounds.x,contentArea2DBounds.y);
		List<ContactGridSection> contactGridSectionList = createContactGridSections();
		for(int i=0; i<contactGridSectionList.Count; i++)
		{
			// Clone section template and position it.
			ContactGridSection currSection = contactGridSectionList[i];

			Transform nwSection = (Transform) Instantiate(sectionTemplate,new Vector3(currFreeSpaceTL.x + (sectionTemplate2DBounds.width/2f),currFreeSpaceTL.y - (sectionTemplate2DBounds.height/2f),sectionTemplate.position.z),Quaternion.identity);
			nwSection.name = "Section-"+i;

			Transform titleAreaGuide = nwSection.FindChild("SectionTitleArea");
			GameObject nwSectionTitleBox = WordBuilderHelper.buildWordBox(i,currSection.getSectionName(),CommonUnityUtils.get2DBounds(titleAreaGuide.renderer.bounds),titleAreaGuide.position.z,upAxisArr,genericWordBox);
			nwSectionTitleBox.name = "SectionTitle";
			WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() {nwSectionTitleBox},0.09f);
			nwSectionTitleBox.transform.parent = nwSection;
			Destroy(nwSectionTitleBox.transform.FindChild("Board").gameObject);
			nwSectionTitleBox.transform.FindChild("Text").renderer.sortingOrder = 162;


			Transform portraitAreaGuide = nwSection.FindChild("PortraitArea");
			Rect portraitAreaBounds = CommonUnityUtils.get2DBounds(portraitAreaGuide.renderer.bounds);

			List<GameObject> wordBannerList = new List<GameObject>();

			List<int> charIDsForSection = currSection.getSectionCharIDs();

			int r=0;
			int c=0;
			GameObject lastPortraitInSection = null;
			for(int k=0; k<charIDsForSection.Count; k++)
			{
				int currCharID = charIDsForSection[k];

				c = (int) (k % (numItemsPerRow * 1.0f));
				r = (int) (k / (numItemsPerRow * 1.0f));

				Vector3 nwFaceSpawnPt = new Vector3(portraitAreaBounds.x + ((c+1) * paddingWidth) + (c * (worldSizeForFace.width)) + (worldSizeForFace.width/2f),
				                                    portraitAreaBounds.y - ((r+1) * paddingHeight) - (r * (worldSizeForFace.height)) - (worldSizeForFace.height/2f),
				                                    contentAreaZVal);

				Transform nwlyCreatedPortrait = null;
				if(gbMang.getContactStatus(currCharID) == CharacterStatus.LOCKED)
				{
					// Render locked character portrait.
					Transform reqLockedPortrait = lockedCharacterFaces[currCharID];
					nwlyCreatedPortrait = (Transform) Instantiate(reqLockedPortrait,nwFaceSpawnPt,Quaternion.identity);
				}
				else
				{
					// Render unlocked character portrait.
					Transform reqUnlockedPortrait = unlockedCharacterFaces[currCharID];
					nwlyCreatedPortrait = (Transform) Instantiate(reqUnlockedPortrait,nwFaceSpawnPt,Quaternion.identity);

					ContactPageInfoSnippit tmpSnip = gbMang.getContactPageInfoSnippit(currCharID);
					string charName = tmpSnip.getName();
					float percComp = tmpSnip.getPhotoAlbum().getNormalisedPercentageCompletion();
					PortraitHelper.replaceEntireDummyPortrait(nwlyCreatedPortrait.gameObject,currCharID,percComp,charName);
					wordBannerList.Add(nwlyCreatedPortrait.FindChild("NameBar").FindChild("TextBanner").gameObject);

					BoxCollider bCol = nwlyCreatedPortrait.gameObject.AddComponent<BoxCollider>();
					bCol.isTrigger = true;
				}
				if(nwlyCreatedPortrait != null)
				{
					nwlyCreatedPortrait.name = "Portrait-"+currCharID;
					nwlyCreatedPortrait.parent = nwSection;
					lastPortraitInSection = nwlyCreatedPortrait.gameObject;
				}
			}

			WordBuilderHelper.setBoxesToUniformTextSize(wordBannerList,0.04f);


			Transform endSectionSeparator = nwSection.FindChild("End-SectionSeparator");
			if(lastPortraitInSection != null)
			{
				Rect lastPortraitBounds = CommonUnityUtils.get2DBounds(lastPortraitInSection.renderer.bounds);

				Vector3 tmpSepPos = endSectionSeparator.position;
				tmpSepPos.y = lastPortraitBounds.y - lastPortraitBounds.height - 0.3f;
				endSectionSeparator.position = tmpSepPos;
			}
			Rect endSeparatorBounds = CommonUnityUtils.get2DBounds(endSectionSeparator.renderer.bounds);


			Transform sectionBackground = nwSection.FindChild("SectionBackground");
			Rect origSectionBounds = CommonUnityUtils.get2DBounds(sectionBackground.renderer.bounds);
			Rect adjustedSectionBounds = new Rect(origSectionBounds.x,origSectionBounds.y,origSectionBounds.width,origSectionBounds.y - (endSeparatorBounds.y + endSeparatorBounds.height));

			Vector3 tmpBkScale = sectionBackground.localScale;
			tmpBkScale.x *= (adjustedSectionBounds.width/origSectionBounds.width);
			tmpBkScale.y *= (adjustedSectionBounds.height/origSectionBounds.height);
			sectionBackground.localScale = tmpBkScale;

			Vector3 tmpBkPos = sectionBackground.position;
			tmpBkPos.x = adjustedSectionBounds.x + (adjustedSectionBounds.width/2f);
			tmpBkPos.y = adjustedSectionBounds.y - (adjustedSectionBounds.height/2f);
			sectionBackground.position = tmpBkPos;


			nwSection.parent = contactsTab.transform;

			currFreeSpaceTL.x = adjustedSectionBounds.x;
			currFreeSpaceTL.y = adjustedSectionBounds.y - adjustedSectionBounds.height;
		}



		Transform firstSection = contactsTab.transform.FindChild("Section-0");
		Transform lastSection = contactsTab.transform.FindChild("Section-"+(contactGridSectionList.Count-1));
		Rect firstSectionBounds = CommonUnityUtils.get2DBounds(firstSection.FindChild("SectionBackground").renderer.bounds);
		Rect lastSectionBounds = CommonUnityUtils.get2DBounds(lastSection.FindChild("SectionBackground").renderer.bounds);

		Transform blowupScrollPane = contactsTab.transform.FindChild("Pages").FindChild("ContactsGridPage").FindChild("BlowupBase");
		Rect blowupScrollPane2DBounds = CommonUnityUtils.get2DBounds(blowupScrollPane.renderer.bounds);
		Rect adjBoundsForScrollPane = new Rect(firstSectionBounds.x,firstSectionBounds.y,firstSectionBounds.width,(firstSectionBounds.y - (lastSectionBounds.y - lastSectionBounds.height)));

		Vector3 bScale = blowupScrollPane.localScale;
		bScale.x *= adjBoundsForScrollPane.width / blowupScrollPane2DBounds.width;
		bScale.y *= adjBoundsForScrollPane.height / blowupScrollPane2DBounds.height;
		blowupScrollPane.localScale = bScale;

		Vector3 bPos = blowupScrollPane.position;
		bPos.x = adjBoundsForScrollPane.x + (adjBoundsForScrollPane.width/2f);
		bPos.y = adjBoundsForScrollPane.y - (adjBoundsForScrollPane.height/2f);
		blowupScrollPane.position = bPos;


		for(int i=0; i<contactGridSectionList.Count; i++)
		{
			Transform tmpSection = contactsTab.transform.FindChild("Section-"+i);
			tmpSection.gameObject.SetActive(true);
			tmpSection.parent = blowupScrollPane;
		}


		scrollableContactGrid = blowupScrollPane;
		scrollableGridZeroWorldBounds = CommonUnityUtils.get2DBounds(blowupScrollPane.renderer.bounds);
		scrollableGridInnerGUIBounds = WorldSpawnHelper.getWorldToGUIBounds(blowupScrollPane.renderer.bounds,upAxisArr);
		scrollableGridInnerGUIBounds.x = 0;
		scrollableGridInnerGUIBounds.y = 0;


		// Add apply mask shader to everything on the blowupScrollPane.
		Vector3 validViewportTL = Camera.main.WorldToViewportPoint(new Vector3(contentArea2DBounds.x,contentArea2DBounds.y,1));
		Vector3 validViewportBR = Camera.main.WorldToViewportPoint(new Vector3(contentArea2DBounds.x + contentArea2DBounds.width,contentArea2DBounds.y - contentArea2DBounds.height,1));
		maskShaderMaterial.SetVector("_Clip",new Vector4(validViewportTL.x,validViewportTL.y,validViewportBR.x - validViewportTL.x,validViewportTL.y - validViewportBR.y));
		maskShaderMaterialForText.SetVector("_Clip",new Vector4(validViewportTL.x,validViewportTL.y,validViewportBR.x - validViewportTL.x,validViewportTL.y - validViewportBR.y));
		applyMaskShaderToAllChildren(blowupScrollPane);


		// Click detector for grid.
		gridClickDetector = transform.GetComponent<ClickDetector>();
		if(gridClickDetector == null)
		{
			gridClickDetector = transform.gameObject.AddComponent<ClickDetector>();
		}
		gridClickDetector.registerListener("GhostbookDisplay",this);
	}

	private void loadFriendPage()
	{
		// Access ghostbook gobj through global variable.
		GameObject contactGridPageGObj = ghostbookGObj.transform.FindChild("ContactsTab").FindChild("Pages").FindChild("ContactsGridPage").gameObject;
		GameObject friendsPageGObj = ghostbookGObj.transform.FindChild("ContactsTab").FindChild("Pages").FindChild("FriendPage").gameObject;

		ghostbookGObj.transform.FindChild("ContactsTab").transform.FindChild("Backdrop").FindChild("ContactsTabHeader").renderer.enabled = false;
		ghostbookGObj.transform.FindChild("ContactsTab").transform.FindChild("Backdrop").FindChild("BackButton").renderer.enabled = true;


		// Init profile pic.
		Transform oldProfilePic = friendsPageGObj.transform.Find("Portrait");
		int charID = playerSelectedCharSnippit.getCharID();
		float percCompletion = playerSelectedCharAlbum.getNormalisedPercentageCompletion();
		string charName = playerSelectedCharSnippit.getName();
		PortraitHelper.replaceEntireDummyPortrait(oldProfilePic.gameObject,charID,percCompletion,charName);

		GameObject portraitTextBanner = oldProfilePic.FindChild("NameBar").FindChild("TextBanner").gameObject;
		WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() {portraitTextBanner},0.04f);


		// Init bio.
		extractGUIBoundsAndAddToCache(friendsPageGObj.transform.FindChild("Info").FindChild("BioTextSection").gameObject,"CharProfileBio");
		extractGUIBoundsAndAddToCache(friendsPageGObj.transform.FindChild("Info").FindChild("TitleTextSection").gameObject,"CharProfileBioTitle");
		extractGUIBoundsAndAddToCache(friendsPageGObj.transform.FindChild("Info").FindChild("GoLeft").gameObject,"LeftButton");
		extractGUIBoundsAndAddToCache(friendsPageGObj.transform.FindChild("Info").FindChild("GoRight").gameObject,"RightButton");

		// Init recent locations.
		extractGUIBoundsAndAddToCache(friendsPageGObj.transform.FindChild("Map").gameObject,"CharProfileMap");
		//extractGUIBoundsAndAddToCache(friendsPageGObj.transform.FindChild("Map").FindChild("MiniMapIcon").gameObject,"CharProfileMap");

		// Init album section.
		extractGUIBoundsAndAddToCache(friendsPageGObj.transform.FindChild("Photos").gameObject,"CharProfileAlbum");
		//extractGUIBoundsAndAddToCache(friendsPageGObj.transform.FindChild("Photos").FindChild("PhotoAlbum").gameObject,"CharProfileAlbum");


		// Set appropriate album seal sprite.
		//PortraitHelper.applyCorrectSealSprite(friendsPageGObj.transform.FindChild("Photos").FindChild("Seal").gameObject,percCompletion);
		Debug.LogWarning("Seal missing");


		contactGridPageGObj.SetActive(false);
		friendsPageGObj.SetActive(true);
	}

	private void loadMapView()
	{
		// Access ghostbook gobj through global variable.
		GameObject mapViewPageGObj = ghostbookGObj.transform.FindChild("ContactsTab").FindChild("Pages").FindChild("MapLocationsPage").gameObject;



		// Init profile pic.
		Transform oldProfilePic = mapViewPageGObj.transform.Find("ProfilePic");
		Vector3 profilePicCentre = new Vector3(oldProfilePic.position.x,oldProfilePic.position.y,oldProfilePic.position.z);
		Vector3 profilePicScale = new Vector3(oldProfilePic.localScale.x,oldProfilePic.localScale.y,oldProfilePic.localScale.z);
		int charID = playerSelectedCharSnippit.getCharID();
		Transform reqCharProfilePicPrefab = unlockedCharacterFaces[charID];
		
		Destroy(oldProfilePic.gameObject);
		Transform nwCharProfilePic = (Transform) Instantiate(reqCharProfilePicPrefab,profilePicCentre,Quaternion.identity);
		nwCharProfilePic.name = "ProfilePic";
		nwCharProfilePic.localScale = profilePicScale;
		nwCharProfilePic.parent = mapViewPageGObj.transform;
		
		GameObject nameGuide = nwCharProfilePic.FindChild("Name").gameObject;
		nameGuide.renderer.enabled = false;

		extractGUIBoundsAndAddToCache(nameGuide,"MapViewCharName");


		// Init location selection area.
		extractGUIBoundsAndAddToCache(mapViewPageGObj.transform.FindChild("LocationSelection").gameObject,"MapViewSelectedLoc");

		// Init visit btn.
		extractGUIBoundsAndAddToCache(mapViewPageGObj.transform.FindChild("BtnVisit").gameObject,"MapViewBtnVisit");
	}


	private void loadDetailedBioView()
	{

		sectionIndex = 0;
		// Access ghostbook gobj through global variable.
		GameObject detailedBioViewPageGObj = ghostbookGObj.transform.FindChild("ContactsTab").FindChild("Pages").FindChild("DetailedBioPage").gameObject;
		

		// Init profile pic.
		Transform oldProfilePic = detailedBioViewPageGObj.transform.Find("Portrait");
		int charID = playerSelectedCharSnippit.getCharID();
		//float percCompletion = playerSelectedCharAlbum.getNormalisedPercentageCompletion();
		string charName = playerSelectedCharSnippit.getName();
		PortraitHelper.replaceEntireDummyPortrait(oldProfilePic.gameObject,charID,0,charName);
		
		GameObject portraitTextBanner = oldProfilePic.FindChild("NameBar").FindChild("TextBanner").gameObject;
		WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() {portraitTextBanner},0.04f);
		



		// Init Detailed Bio.
		extractGUIBoundsAndAddToCache(detailedBioViewPageGObj.transform.FindChild("TextArea").gameObject,"DetailedBioTextArea");

		playerSelectedCharBioSections = new List<string>() { LocalisationMang.getFullExtensiveBio(playerSelectedCharSnippit.getCharID()) };
	}

	private void loadEventsPage()
	{
		// Access ghostbook gobj through global variable.
		GameObject eventsPageGObj = ghostbookGObj.transform.FindChild("EventsTab").FindChild("Pages").FindChild("MainPage").gameObject;

		extractGUIBoundsAndAddToCache(eventsPageGObj.transform.FindChild("ContentArea").gameObject,"EventsGUIScrollArea");
		extractGUIBoundsAndAddToCache(eventsPageGObj.transform.FindChild("GuideTextBannerNarrow").gameObject,"GuideTextBannerNarrow");

		Rect totalContentArea = uiBounds["EventsGUIScrollArea"];
		Rect guideTextBannerNarrow = uiBounds["GuideTextBannerNarrow"];

		Rect centredTextBannerBounds = new Rect(totalContentArea.x + (totalContentArea.width/2f) - (guideTextBannerNarrow.width/2f),
		                                        					    totalContentArea.y + (totalContentArea.height/2f) - (guideTextBannerNarrow.height/2f),
		                                        						guideTextBannerNarrow.width,
		                                        						guideTextBannerNarrow.height);
		if( ! uiBounds.ContainsKey("NoEventsBanner"))
		{
			uiBounds.Add("NoEventsBanner",centredTextBannerBounds);
		}


		SpriteRenderer reqSRend = textBannerNarrowPrefab.GetComponent<SpriteRenderer>();
		Rect baseTexBounds = new Rect(0,0,0,0);
		baseTexBounds.width = reqSRend.sprite.texture.width;
		baseTexBounds.height = reqSRend.sprite.texture.height;
		//noEventBannerNormBounds = CommonUnityUtils.normaliseRect(baseTexBounds,reqSRend.sprite.textureRect);


		availableEvents = gbMang.getAvailableEvents();
	}


	private void loadEventsPageUpgrade2()
	{
		// Create Sections.
		
		//Transform genericWordBox = ghostBookWordBoxPrefab;
		GameObject eventsTab = ghostbookGObj.transform.FindChild("EventsTab").gameObject;
		GameObject eventsMainPage = eventsTab.transform.FindChild("Pages").FindChild("MainPage").gameObject;

		extractGUIBoundsAndAddToCache(eventsMainPage.transform.FindChild("ContentArea").gameObject,"EventsGUIScrollArea");
		GameObject contentArea = eventsTab.transform.FindChild("Backdrop").FindChild("ContentArea").gameObject;
		Rect contentArea2DBounds = CommonUnityUtils.get2DBounds(contentArea.renderer.bounds);
		

		Transform sectionTemplate = eventsMainPage.transform.FindChild("SectionTemplate");
		Rect sectionTemplate2DBounds = CommonUnityUtils.get2DBounds(sectionTemplate.FindChild("SectionBackground").renderer.bounds);
		
		Vector3 currFreeSpaceTL = new Vector3(contentArea2DBounds.x,contentArea2DBounds.y);
		availableEvents = gbMang.getAvailableEvents();

		bool pageContainsEntries = false;
		if(availableEvents != null)
		{
			if(availableEvents.Count > 0)
			{
				pageContainsEntries = true;
			}
		}


		if(pageContainsEntries)
		{
			for(int i=0; i<availableEvents.Count; i++)
			{
				// Clone section template and position it.
				EventSummarySnippit currEvSnip = availableEvents[i];
				
				Transform nwSection = (Transform) Instantiate(sectionTemplate,new Vector3(currFreeSpaceTL.x + (sectionTemplate2DBounds.width/2f),currFreeSpaceTL.y - (sectionTemplate2DBounds.height/2f),sectionTemplate.position.z),Quaternion.identity);
				nwSection.name = "Section-"+i;

				Transform smallHeadGuide = nwSection.FindChild("SmallHead");
				Transform activitySymbolGuide = nwSection.FindChild("SmallSymbol");

				smallHeadGuide.GetComponent<SpriteRenderer>().sprite = characterSmallFaces[currEvSnip.getQuestGiverCharID()].GetComponent<SpriteRenderer>().sprite;


				activitySymbolGuide.GetComponent<SpriteRenderer>().sprite = activitySymbols[(int) (currEvSnip.getApplicationID())].GetComponent<SpriteRenderer>().sprite;


				
				Transform sectionBackground = nwSection.FindChild("SectionBackground");
				Rect origSectionBounds = CommonUnityUtils.get2DBounds(sectionBackground.renderer.bounds);
				Rect adjustedSectionBounds = origSectionBounds;//new Rect(origSectionBounds.x,origSectionBounds.y,origSectionBounds.width,origSectionBounds.y - (endSeparatorBounds.y + endSeparatorBounds.height));
				
				nwSection.parent = eventsTab.transform;
				
				currFreeSpaceTL.x = adjustedSectionBounds.x;
				currFreeSpaceTL.y = adjustedSectionBounds.y - adjustedSectionBounds.height;
			}
			

			
			Transform firstSection = eventsTab.transform.FindChild("Section-0");
			Transform lastSection = eventsTab.transform.FindChild("Section-"+(availableEvents.Count-1));
			Rect firstSectionBounds = CommonUnityUtils.get2DBounds(firstSection.FindChild("SectionBackground").renderer.bounds);
			Rect lastSectionBounds = CommonUnityUtils.get2DBounds(lastSection.FindChild("SectionBackground").renderer.bounds);
			
			Transform blowupScrollPane = eventsMainPage.transform.FindChild("BlowupBase");
			Rect blowupScrollPane2DBounds = CommonUnityUtils.get2DBounds(blowupScrollPane.renderer.bounds);
			Rect adjBoundsForScrollPane = new Rect(firstSectionBounds.x,firstSectionBounds.y,firstSectionBounds.width,(firstSectionBounds.y - (lastSectionBounds.y - lastSectionBounds.height)));
			
			Vector3 bScale = blowupScrollPane.localScale;
			bScale.x *= adjBoundsForScrollPane.width / blowupScrollPane2DBounds.width;
			bScale.y *= adjBoundsForScrollPane.height / blowupScrollPane2DBounds.height;
			blowupScrollPane.localScale = bScale;
			
			Vector3 bPos = blowupScrollPane.position;
			bPos.x = adjBoundsForScrollPane.x + (adjBoundsForScrollPane.width/2f);
			bPos.y = adjBoundsForScrollPane.y - (adjBoundsForScrollPane.height/2f);
			blowupScrollPane.position = bPos;
			
			for(int i=0; i<availableEvents.Count; i++)
			{
				Transform tmpSection = eventsTab.transform.FindChild("Section-"+i);
				tmpSection.gameObject.SetActive(true);
				tmpSection.parent = blowupScrollPane;
			}
			

			scrollableEventsPane = blowupScrollPane;
			scrollableEventspaneZeroWorldBounds = CommonUnityUtils.get2DBounds(blowupScrollPane.renderer.bounds);
			scrollableEventsPaneInnerGUIBounds = WorldSpawnHelper.getWorldToGUIBounds(blowupScrollPane.renderer.bounds,upAxisArr);
			scrollableEventsPaneInnerGUIBounds.x = 0;
			scrollableEventsPaneInnerGUIBounds.y = 0;


			//Rect regPaneWBounds = scrollableGridZeroWorldBounds;
			Rect paneGUIBounds = uiBounds["EventsGUIScrollArea"];
			eventsSectionInnerGUIs = new List<EventsPageInnerGUICollection>();
			for(int i=0; i<availableEvents.Count; i++)
			{
				Transform tmpSection = blowupScrollPane.transform.FindChild("Section-"+i);

				//Rect tmpSectionGUIBounds = WorldSpawnHelper.getWorldToGUIBounds(tmpSection.FindChild("SectionBackground").renderer.bounds,upAxisArr);
				Rect tmpFShGUIBounds = WorldSpawnHelper.getWorldToGUIBounds(tmpSection.FindChild("FriendlyShoutoutArea").renderer.bounds,upAxisArr);
				Rect tmpLA2DGUIBounds = WorldSpawnHelper.getWorldToGUIBounds(tmpSection.FindChild("LanguageAreaField").renderer.bounds,upAxisArr);
				Rect tmpDiff2DGUIBounds = WorldSpawnHelper.getWorldToGUIBounds(tmpSection.FindChild("DifficultyField").renderer.bounds,upAxisArr);

				EventsPageInnerGUICollection tmpInnerGUIData = new EventsPageInnerGUICollection();
				tmpInnerGUIData.friendlyShoutoutBounds = new Rect(((tmpFShGUIBounds.x - paneGUIBounds.x)/paneGUIBounds.width) * paneGUIBounds.width,
				                                                  ((tmpFShGUIBounds.y - paneGUIBounds.y)/paneGUIBounds.height) * paneGUIBounds.height,
				                                                  tmpFShGUIBounds.width,tmpFShGUIBounds.height);

				tmpInnerGUIData.langAreaTextFieldBounds = new Rect(((tmpLA2DGUIBounds.x - paneGUIBounds.x)/paneGUIBounds.width) * paneGUIBounds.width,
				                                                   ((tmpLA2DGUIBounds.y - paneGUIBounds.y)/paneGUIBounds.height) * paneGUIBounds.height,
				                                                   tmpLA2DGUIBounds.width,tmpLA2DGUIBounds.height);

				tmpInnerGUIData.difficultyTextFieldBounds = new Rect(((tmpDiff2DGUIBounds.x - paneGUIBounds.x)/paneGUIBounds.width) * paneGUIBounds.width,
				                                                     ((tmpDiff2DGUIBounds.y - paneGUIBounds.y)/paneGUIBounds.height) * paneGUIBounds.height,
				                                                     tmpDiff2DGUIBounds.width,tmpDiff2DGUIBounds.height);

				eventsSectionInnerGUIs.Add(tmpInnerGUIData);



			}

					
			
			// Add apply mask shader to everything on the blowupScrollPane.
			Vector3 validViewportTL = Camera.main.WorldToViewportPoint(new Vector3(contentArea2DBounds.x,contentArea2DBounds.y,1));
			Vector3 validViewportBR = Camera.main.WorldToViewportPoint(new Vector3(contentArea2DBounds.x + contentArea2DBounds.width,contentArea2DBounds.y - contentArea2DBounds.height,1));
			maskShaderMaterial.SetVector("_Clip",new Vector4(validViewportTL.x,validViewportTL.y,validViewportBR.x - validViewportTL.x,validViewportTL.y - validViewportBR.y));
			maskShaderMaterialForText.SetVector("_Clip",new Vector4(validViewportTL.x,validViewportTL.y,validViewportBR.x - validViewportTL.x,validViewportTL.y - validViewportBR.y));
			applyMaskShaderToAllChildren(blowupScrollPane);
			
			
			// Click detector for grid.
			if(eventsPaneClickDetector == null)
			{
				eventsPaneClickDetector = transform.gameObject.AddComponent<ClickDetector>();
				eventsPaneClickDetector.registerListener("GhostbookDisplay",this);
			}
		}
	}



	private void loadNewsFeedPage()
	{
		// Access ghostbook gobj through global variable.
		GameObject newsPageGObj = ghostbookGObj.transform.FindChild("NewsFeedTab").FindChild("Pages").FindChild("MainPage").gameObject;
		
		extractGUIBoundsAndAddToCache(newsPageGObj.transform.FindChild("ContentArea").gameObject,"NewsFeedGUIScrollArea");
		extractGUIBoundsAndAddToCache(newsPageGObj.transform.FindChild("GuideTextBannerNarrow").gameObject,"GuideTextBannerNarrow");
		extractGUIBoundsAndAddToCache(newsPageGObj.transform.FindChild("GuideNewsIcon").gameObject,"GuideNewsIcon");
		
		Rect totalContentArea = uiBounds["NewsFeedGUIScrollArea"];
		Rect guideTextBannerNarrow = uiBounds["GuideTextBannerNarrow"];
		
		Rect centredTextBannerBounds = new Rect(totalContentArea.x + (totalContentArea.width/2f) - (guideTextBannerNarrow.width/2f),
		                                        totalContentArea.y + (totalContentArea.height/2f) - (guideTextBannerNarrow.height/2f),
		                                        guideTextBannerNarrow.width,
		                                        guideTextBannerNarrow.height);
		if( ! uiBounds.ContainsKey("NoNewsBanner"))
		{
			uiBounds.Add("NoNewsBanner",centredTextBannerBounds);
		}
		
		
		SpriteRenderer reqSRend = textBannerNarrowPrefab.GetComponent<SpriteRenderer>();
		Rect baseTexBounds = new Rect(0,0,0,0);
		baseTexBounds.width = reqSRend.sprite.texture.width;
		baseTexBounds.height = reqSRend.sprite.texture.height;
		noNewsBannerNormBounds = CommonUnityUtils.normaliseRect(baseTexBounds,reqSRend.sprite.textureRect);

		reqSRend = newsIconPrefab.GetComponent<SpriteRenderer>();
		baseTexBounds = new Rect(0,0,0,0);
		baseTexBounds.width = reqSRend.sprite.texture.width;
		baseTexBounds.height = reqSRend.sprite.texture.height;
		//newsIconNormTexBounds = CommonUnityUtils.normaliseRect(baseTexBounds,reqSRend.sprite.textureRect);


		
		availableNews = gbMang.getNewsItems();
	}

	private void selectTab(int para_tabIndex, int para_pageIndex)
	{
		scrollPos.x = 0;
		scrollPos.y = 0;

		// Access ghostbook gobj through global variable.
		List<GameObject> tabGObjs = new List<GameObject>();
		tabGObjs.Add(ghostbookGObj.transform.FindChild("ContactsTab").gameObject);
		tabGObjs.Add(ghostbookGObj.transform.FindChild("EventsTab").gameObject);
		tabGObjs.Add(ghostbookGObj.transform.FindChild("NewsFeedTab").gameObject);


		for(int i=0; i<tabSelectionArr.Length; i++)
		{
			if(i == para_tabIndex)
			{
				tabSelectionArr[i] = true;

				tabGObjs[i].transform.FindChild("Pages").gameObject.SetActive(true);

				if((i == 0)&&(para_pageIndex != 0))
				{
					tabGObjs[0].transform.FindChild("Backdrop").FindChild("ContactsTabHeader").renderer.enabled = false;
					tabGObjs[0].transform.FindChild("Backdrop").FindChild("BackButton").renderer.enabled = true;

				}
			}
			else
			{
				if(tabSelectionArr[i] == true)
				{
					tabGObjs[i].transform.FindChild("Pages").gameObject.SetActive(false);

					if(i == 0)
					{
						tabGObjs[0].transform.FindChild("Backdrop").FindChild("ContactsTabHeader").renderer.enabled = true;
						tabGObjs[0].transform.FindChild("Backdrop").FindChild("BackButton").renderer.enabled = false;
					}
				}

				tabSelectionArr[i] = false;
			}
		}





		List<int> tabIndexOrdering = new List<int>();
		tabIndexOrdering.Add(para_tabIndex);
		for(int i=0; i<tabSelectionArr.Length; i++)
		{
			if(i != para_tabIndex)
			{
				tabIndexOrdering.Add(i);
			}
		}

		Vector3 tmpPosVar = new Vector3();
		for(int i=0; i<tabIndexOrdering.Count; i++)
		{
			tmpPosVar = tabGObjs[tabIndexOrdering[i]].transform.position;
			tmpPosVar.z = tabZLevels[i];
			tabGObjs[tabIndexOrdering[i]].transform.position = tmpPosVar;
		}



		// Page selection.
		List<List<string>> pageNameCollection = new List<List<string>>();
		List<string> contactsTabPages = new List<string>() { "ContactsGridPage", "FriendPage", "MapLocationsPage", "DetailedBioPage", "SpecificPhotoAlbumPage" };
		List<string> eventsTabPages = new List<string>() {"MainPage"};
		List<string> newsfeedTabPages = new List<string>() {"MainPage"};
		pageNameCollection.Add(contactsTabPages);
		pageNameCollection.Add(eventsTabPages);
		pageNameCollection.Add(newsfeedTabPages);

		Transform reqTabPagesObj = tabGObjs[para_tabIndex].transform.FindChild("Pages");
		List<string> reqTabPageNames = pageNameCollection[para_tabIndex];
		for(int i=0; i<reqTabPagesObj.childCount; i++)
		{
			Transform tmpChildPage = reqTabPagesObj.GetChild(i);

			if(tmpChildPage.name == reqTabPageNames[para_pageIndex])
			{
				tmpChildPage.gameObject.SetActive(true);
			}
			else
			{
				tmpChildPage.gameObject.SetActive(false);
			}
		}

		if(! ((para_tabIndex == 0)&&(para_pageIndex == 0)))
		{
			if(gridClickDetector != null) {	gridClickDetector.enabled = false;	}
		}
		else { if(gridClickDetector != null) {	gridClickDetector.enabled = true;	} }

		if(! ((para_tabIndex == 1)&&(para_pageIndex == 0)))
		{
			if(eventsPaneClickDetector != null) { eventsPaneClickDetector.enabled = false; }
		}
		else { if(eventsPaneClickDetector != null) { eventsPaneClickDetector.enabled = true; }}


		//prevSelectedTabID = currSelectedTabID;
		//prevSelectedPageID = currSelectedPageID;

		currSelectedTabID = para_tabIndex;
		currSelectedPageID = para_pageIndex;

		// Page hiearchy
		if(hiearchyStack.Count > 0)
		{
			if(hiearchyStack.Peek()[0] != currSelectedTabID)
			{
				hiearchyStack.Clear();
			}
		}
		hiearchyStack.Push(new int[2] { currSelectedTabID, currSelectedPageID });

		//CommonUnityUtils.setSortingOrderOfEntireObject(ghostbookGObj,5000);
	}

	private void goUpDirectory()
	{
		hiearchyStack.Pop();
		int[] reqCombo = hiearchyStack.Peek();
		hiearchyStack.Pop();
		selectTab(reqCombo[0],reqCombo[1]);
	}

	
	private void extractGUIBoundsAndAddToCache(GameObject para_gObj, string para_boundName)
	{
		Rect reqGUIBounds = WorldSpawnHelper.getWorldToGUIBounds(para_gObj.renderer.bounds,upAxisArr);
		if(uiBounds.ContainsKey(para_boundName)) { uiBounds[para_boundName] = reqGUIBounds; } else { uiBounds.Add(para_boundName,reqGUIBounds); }
	}



	public void setVisibility(bool para_state)
	{
		// Access ghostbook gobj through global variable.
		if(ghostbookGObj != null)
		{
			ghostbookGObj.SetActive(para_state);
		}
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_sourceID == "AlbumInnerViewScript")
		{
			if(para_eventID == "Close")
			{
				isInAlbumInnerView = false;
				//isInPhotoCloseupView = false;
				//isInTeacherView = false;
			}
			else if(para_eventID == "IsInPhotoCloseupView")
			{
				//isInPhotoCloseupView = true;
			}
			else if(para_eventID == "IsInTeacherView")
			{
				//isInTeacherView = true;
			}

		}
		else if(para_eventID == "ClickEvent")
		{
			if((currSelectedTabID == 0)&&(currSelectedPageID == 0))//Contact list
			{
				System.Object[] parsedEventData = (System.Object[]) para_eventData;
				float[] clickPos = (float[]) parsedEventData [0];
				
				RaycastHit hitInf;
				if(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(clickPos[0],clickPos[1],0)),out hitInf))
				{
					if(hitInf.collider.name.Contains("Portrait"))
					{
						// Player has selected a portrait from the contact grid. Load that character's friend page.
						int reqCharID = int.Parse(hitInf.collider.name.Split('-')[1]);
						playerSelectedCharSnippit = gbMang.getContactPageInfoSnippit(reqCharID);
						playerSelectedCharAlbum = playerSelectedCharSnippit.getPhotoAlbum();
						//Debug.Log("OWNER "+playerSelectedCharAlbum.ownerID);
						loadFriendPage();
						tabToCurrPageMap[0] = 1;
						selectTab(0,1);
					}
				}
			}
			else if((currSelectedTabID == 1)&&(currSelectedPageID == 0))//Events
			{
				System.Object[] parsedEventData = (System.Object[]) para_eventData;
				float[] clickPos = (float[]) parsedEventData [0];
				
				RaycastHit hitInf;
				if(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(clickPos[0],clickPos[1],0)),out hitInf))
				{
					if(hitInf.collider.name.Contains("EventsBanner"))
					{
						int reqEventIndex = int.Parse(hitInf.collider.gameObject.transform.parent.name.Split('-')[1]);
						if(availableEvents != null)
						{
							if(availableEvents.Count > 0)
							{
								if(reqEventIndex < availableEvents.Count)
								{
									//eventClickIndex = reqEventIndex;

									//int reqEventIndex = eventClickIndex;
									
									EventSummarySnippit reqSnip = availableEvents[reqEventIndex];
									Encounter encounterDataForEvent = reqSnip.getRelatedEncData();
									//eventClickIndex = -1;
									
									control.launchQuest(
										new ExternalParams(
										reqSnip.getApplicationID(),
										reqSnip.getQuestGiverCharID(),
										encounterDataForEvent.getLanguageArea(),
										encounterDataForEvent.getDifficulty(),
										encounterDataForEvent.getLevel(),
										false),
										LocalisationMang.getOwnerNpcOfActivity(reqSnip.getApplicationID()),
										"Event",
										Mode.PLAY);
									//performEventClickProcedureFlag = true;
	
								}
							}
						}
					}
				}
			}
			
		}
	}

	private void clearVariables()
	{
		availableTextures.Clear();
		uiBounds.Clear();
		loadTextures();
		prepUIBounds();
	}

	private List<Rect> extractNormTexBoundsForMultisprites(Transform[] spriteArr)
	{
		List<Rect> retNormTexBoundsList = new List<Rect>();
		Rect baseTexBounds = new Rect(0,0,0,0);
		for(int i=0; i<spriteArr.Length; i++)
		{
			SpriteRenderer sRend = spriteArr[i].GetComponent<SpriteRenderer>();
			baseTexBounds.width = sRend.sprite.texture.width;
			baseTexBounds.height = sRend.sprite.texture.height;
			retNormTexBoundsList.Add(CommonUnityUtils.normaliseRect(baseTexBounds,sRend.sprite.textureRect));
		}
		return retNormTexBoundsList;
	}

	protected new void loadTextures()
	{
		availableTextures = new Dictionary<string, Texture2D>();
		availableTextures.Add("GhostBookIconOpen",Resources.Load<Texture2D>("Textures/Ghostbook/UI/ghostbookiconopen"));
		availableTextures.Add("GhostBookIconClose",Resources.Load<Texture2D>("Textures/Ghostbook/UI/ghostbookiconclose"));
	}


	protected  new void prepUIBounds()
	{
		uiBounds = new Dictionary<string, Rect>();

		Rect iconifiedGUIBounds = WorldSpawnHelper.getWorldToGUIBounds(Camera.main.transform.FindChild("GUI_Ghostbook").FindChild("ButtonMain").renderer.bounds,upAxisArr);
		//Used for the close button

		/*float iconifiedBounds_Width = Screen.width * 0.15f;
		float iconifiedBounds_Height = Screen.height * 0.15f;
		if(iconifiedBounds_Width > iconifiedBounds_Height) { iconifiedBounds_Width = iconifiedBounds_Height; }
		else { iconifiedBounds_Height = iconifiedBounds_Width; }
		float iconifiedBounds_X = Screen.width - iconifiedBounds_Width;
		float iconifiedBounds_Y = Screen.height - iconifiedBounds_Height;
		Rect iconifiedGUIBounds = new Rect(iconifiedBounds_X,iconifiedBounds_Y,iconifiedBounds_Width,iconifiedBounds_Height);*/

		uiBounds.Add("IconifiedBounds",iconifiedGUIBounds);
	}


	// Change this later.
	private List<ContactGridSection> createContactGridSections()
	{
		// Sectioned by Language Area.

		List<ContactGridSection> retSectionList = new List<ContactGridSection>();

		//IGBLangAreaReference langAreaRef = gbMang.getLangAreaReferenceMaterial();
		int languageAreas  = gbMang.numberOfLanguageAreas();

		for(int i=0; i<languageAreas; i++)
		{

			ContactGridSection tmpSection = new ContactGridSection(gbMang.getNameForLangArea(i));
			List<int> allNpcsForLA = gbMang.getAllNpcsForLangArea(i);
			for(int k=0; k<allNpcsForLA.Count; k++)
			{
				tmpSection.addChar(allNpcsForLA[k]);
			}

			retSectionList.Add(tmpSection);
		}

		return retSectionList;

	}

	class ContactGridSection
	{
		string sectionName;
		List<int> charIDsForSection;

		public ContactGridSection(string para_sectionName)
		{
			sectionName = para_sectionName;
			charIDsForSection = new List<int>();
		}

		public void addChar(int para_charID)
		{
			if( ! charIDsForSection.Contains(para_charID))
			{
				charIDsForSection.Add(para_charID);
			}
		}

		public string getSectionName() { return sectionName; }
		public List<int> getSectionCharIDs() { return charIDsForSection; }
	}

	class EventsPageInnerGUICollection
	{
		public Rect friendlyShoutoutBounds;
		public Rect langAreaTextFieldBounds;
		public Rect difficultyTextFieldBounds;

		public EventsPageInnerGUICollection() { }
	}

	private void applyMaskShaderToAllChildren(Transform para_srcObj)
	{
		if(para_srcObj != null)
		{
			if(para_srcObj.renderer != null)
			{
				if(para_srcObj.GetComponent<TextMesh>() == null)
				{
					Color tmpCol = para_srcObj.renderer.material.color;
					para_srcObj.renderer.material = maskShaderMaterial;


					SpriteRenderer sRend = para_srcObj.GetComponent<SpriteRenderer>();
					if(sRend != null)
					{
						para_srcObj.renderer.material.SetColor("_Color",sRend.color);	
					}
					else
					{
						para_srcObj.renderer.material.SetColor("_Color",tmpCol);	
					}
				}
				else
				{
					//Color tmpCol = para_srcObj.renderer.material.color;
					para_srcObj.renderer.material = maskShaderMaterialForText;
				}
		
			}

			for(int i=0; i<para_srcObj.childCount; i++)
			{
				applyMaskShaderToAllChildren(para_srcObj.GetChild(i));
			}
		}

		return;
	}




}
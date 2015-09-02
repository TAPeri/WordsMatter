/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class AlbumInnerViewScript : ILearnRWUIElement, IActionNotifier, CustomActionListener
{

	PhotoAlbum album;
	List<PhotoPage> availablePhotoPages;

	// Contents View Variables.
	bool isInContentsView = false;
	Transform contentsPageChild;
	bool contentsBoundsLoaded = false;
	Vector2 contentsScrollPos;
	Vector2 tmpVect;
	Sprite[] photoCompletionIcons;
	Rect templateContentsRow;
	Rect templateContentsRowText;
	Rect templateContentsRowImg;

	// Photo Page View Variables.
	bool isInPageView = false;
	Transform photoPageChild;
	bool photoPageBoundsLoaded = false;
	int numPhotosOnCurrPage = 0;
	PhotoPage currPhotoPage;
	int currPhotoPageIndex = 0;
	bool prevPhotoPageAvailable = true;
	bool nextPhotoPageAvailable = false;
	string humanReadableDescText = "";

	// Photo Closeup View Variables.
	bool isInCloseupView = false;
	bool isInTeacherFeatureView = false;


	Transform genericWordBoxPrefab;
	Transform sfxPrefab;



	void OnDestroy(){

		if(nwTeacherWindow!=null)
			DestroyImmediate(nwTeacherWindow);
	}




	void Start()
	{
		tmpVect = new Vector2();
		contentsScrollPos = new Vector2();
		genericWordBoxPrefab = (Transform) Resources.Load<Transform>("Prefabs/GenericWordBox");
	}

	void OnGUI()
	{
		if( ! hasInitGUIStyles)
		{
			this.prepGUIStyles();
			hasInitGUIStyles = true;
		}
		else
		{
			if(isInTeacherFeatureView)
			{
				
			}
			else if(isInContentsView)
			{
				GUI.color = Color.black;
				GUI.Label(uiBounds["ContentsBanner"],textContent["ContentsBanner"],availableGUIStyles["ContentsTitle"]);
				GUI.color = Color.white;

				float rowPadding = 10;
				Rect guiLocationBounds = uiBounds["ContentsScrollArea"];
				Rect innerScrollBounds = new Rect(0,0,guiLocationBounds.width,(templateContentsRow.height*(availablePhotoPages.Count+1)) + (rowPadding * (availablePhotoPages.Count)));
				
				if((Application.platform == RuntimePlatform.Android)
				   ||(Application.platform == RuntimePlatform.IPhonePlayer))
				{
					if(Input.touchCount == 1)
					{
						tmpVect.x = Input.touches[0].position.x;
						tmpVect.y = Screen.height - Input.touches[0].position.y;
						
						if(guiLocationBounds.Contains(tmpVect))
						{
							contentsScrollPos.y += (Input.touches[0].deltaPosition.y * 1f);
						}
					}
				}


				contentsScrollPos = GUI.BeginScrollView(guiLocationBounds,contentsScrollPos,innerScrollBounds,GUIStyle.none,GUIStyle.none);
			
				templateContentsRow.x = 0;
				templateContentsRow.y = 0;
				templateContentsRowText.x = 0;
				templateContentsRowText.y = 0;
				templateContentsRowImg.x = templateContentsRow.width * 0.7f;
				templateContentsRowImg.y = templateContentsRow.height * 0.1f;
				for(int i=0; i<availablePhotoPages.Count; i++)
				{
					PhotoPage tmpPage = availablePhotoPages[i];
					string pageName = tmpPage.getPageName();
					int numPhotos = tmpPage.getNumAvailablePhotos();

					GUI.color = Color.white;
					if(GUI.Button(templateContentsRow,""))
					{
						bool validClick = true;

						if((Application.platform == RuntimePlatform.Android)
						   ||(Application.platform == RuntimePlatform.IPhonePlayer))
						{
							validClick = false;

							GUI.Label(new Rect(0,0,200,200),"TouchPhase: "+Input.touches[0].phase);

							if((Input.touchCount == 1)&&(Input.touches[0].phase != TouchPhase.Moved))
							{
								validClick = true;
							}
						}

						if(validClick)
						{
							// Go to photo page.
							Debug.Log("Now transfering to page "+i+": "+pageName);
							displayPhotoPageView(i);
						}
					}

					GUI.color = Color.black;
					GUI.Label(templateContentsRowText,pageName,availableGUIStyles["TitleLabel"]);
					GUI.color = Color.white;
					GUI.DrawTexture(templateContentsRowImg,photoCompletionIcons[numPhotos].texture);

					float yInc = (templateContentsRow.height + rowPadding);
					templateContentsRow.y += yInc;
					templateContentsRowText.y += yInc;
					templateContentsRowImg.y += yInc;
				}

				GUI.EndScrollView();


				GUI.color = Color.clear;
				if((album != null)&&(availablePhotoPages != null)&&(availablePhotoPages.Count > 0))
				{
					if(GUI.Button(uiBounds["NextPageButton"],""))
					{
						// Go to first photo page.
						displayPhotoPageView(0);
					}
				}
				GUI.color = Color.white;
			}
			else if(isInPageView)
			{
				GUI.color = Color.clear;
				for(int i=0; i<numPhotosOnCurrPage; i++)
				{
					//Debug.Log("PhotoPosition"+i);
					if(GUI.Button(uiBounds["PhotoPosition"+i],""))
					{
						// Go to closeup view.
						Debug.Log("Will now show closeup photo view.");
						notifyAllListeners("AlbumInnerViewScript","IsInPhotoCloseupView",null);
						displayCloseupView(i,currPhotoPage.getAvailablePhotos()[i]);
					}
				}

				GUI.color = Color.black;
				GUI.Label(uiBounds["PhotoPageTitle"],currPhotoPage.getPageName(),availableGUIStyles["TitleLabel"]);
				GUI.color = Color.clear;

				if(GUI.Button(uiBounds["BookmarkTip"],""))
				{
					// Go to contents page.
					displayContentsView();
				}


				if(prevPhotoPageAvailable)
				{
					if(GUI.Button(uiBounds["PreviousPageButton"],""))
					{
						if((currPhotoPageIndex+1-1) <= 0)
						{
							// Go to contents page.
							displayContentsView();
						}
						else
						{
							// Go to previous photo page.
							displayPhotoPageView(currPhotoPageIndex-1);
						}
					}
				}

				if(nextPhotoPageAvailable)
				{
					if(GUI.Button(uiBounds["NextPageButton"],""))
					{
						// Go to next photo page.
						displayPhotoPageView(currPhotoPageIndex+1);
					}
				}


				if(GUI.Button(uiBounds["PlayPageButton"],""))
				{
					// Player wants to play this page. (I.e. this language area + difficulty combo.)
					// Go to the location/activity window if the teacher is logged in.

					bool isTeacherLoggedIn = false; 

					// Determine if teacher is logged in using IServerServices.
					try
					{
						GameObject poRef = PersistentObjMang.getInstance();
						Transform serverTrans = poRef.transform.Find("Server");
						GameObject server_object = null;
						if(serverTrans != null)
						{
							server_object = serverTrans.gameObject;
							IServerServices serverServ = (IServerServices)server_object.GetComponent(typeof(IServerServices));
							if(serverServ != null)
							{
								isTeacherLoggedIn = serverServ.connectedWithTeacher();
							}
						}
					}
					catch(System.Exception ex)
					{
						Debug.LogError("Error when checking if teacher logged in. "+ex.Message);
						isTeacherLoggedIn = false;
					}

					// Tmp commented override if necessary:
					//bool isTeacherLoggedIn = false;


					if(isTeacherLoggedIn)
					{
						// Teacher logged in. Trigger teacher options popup.

						Transform teacherWindowPopup = Resources.Load<Transform>("Prefabs/TeacherFeaturesWindow");

						if(teacherWindowPopup != null)
						{
							isInTeacherFeatureView = true;
							notifyAllListeners("AlbumInnerViewScript","IsInTeacherView",null);

							Rect origPrefab2DBounds = CommonUnityUtils.get2DBounds(teacherWindowPopup.FindChild("WindowBounds").renderer.bounds);
							nwTeacherWindow = WorldSpawnHelper.initWorldObjAndBlowupToScreen(teacherWindowPopup,origPrefab2DBounds);
							nwTeacherWindow.transform.position = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,Camera.main.transform.position.z + 3f);
							nwTeacherWindow.name = "TeacherFeaturesWindow";
							TeacherFeaturesWindow tfw = nwTeacherWindow.AddComponent<TeacherFeaturesWindow>();
							tfw.registerListener("AlbumInnerView",this);
							tfw.init(currPhotoPage.getLangArea(),currPhotoPage.getDifficulty(),album.getOwnerID());
						}
					}
					else
					{
						// Teacher not logged in. Go staight to launch.
						triggerPlayPhotoPage();
					}
				}


				GUI.color = Color.black;

				GUI.Label(uiBounds["NoteScrollArea"],humanReadableDescText,availableGUIStyles["HumanReadableExamples"]);
				


				GUI.color = Color.white;
			}
			else if(isInCloseupView)
			{

			}
		}
	}

	GameObject nwTeacherWindow;
	
	ProgressScript control;

	public void init(PhotoAlbum para_albumData, Sprite[] para_photoCompletionIcons,ProgressScript c)
	{

		control = c;
		//Debug.Log("START ALBUM");


		contentsPageChild = transform.FindChild("PhotoAlbum_Contents");
		photoPageChild = transform.FindChild("PhotoAlbum_PhotoPage");

		Rect contentsAreaGUIBounds = WorldSpawnHelper.getWorldToGUIBounds(contentsPageChild.FindChild("ContentsScrollArea").renderer.bounds,new bool[]{false,true,false});
		templateContentsRow = new Rect(0,0,contentsAreaGUIBounds.width,contentsAreaGUIBounds.height / (3.5f));
		templateContentsRowText = new Rect(0,0,templateContentsRow.width * 0.75f,templateContentsRow.height);
		templateContentsRowImg = new Rect(templateContentsRow.width * 0.7f,templateContentsRow.height * 0.1f,templateContentsRow.width * 0.25f,templateContentsRow.height - (2 * (templateContentsRow.height * 0.1f)));


		album = para_albumData;
		availablePhotoPages = album.getAllAvailablePages();
		photoCompletionIcons = para_photoCompletionIcons;

		displayContentsView();
	}

	private void displayContentsView()
	{
		photoPageChild.gameObject.SetActive(false);
		contentsPageChild.gameObject.SetActive(true);
		isInContentsView = true;
		isInPageView = false;
		isInCloseupView = false;


		if( ! contentsBoundsLoaded)
		{

			GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();

			string albumContentsHeader = "Contents";

			List<PhotoPage> albumPages = album.getAllAvailablePages();
			if((albumPages != null)&&(albumPages.Count > 0))
			{
				int langAreaForAlbum = albumPages[0].getLangArea();

				albumContentsHeader = gbMang.getNameForLangArea(langAreaForAlbum);
			}


			// Init text items.
			string[] elementNames   = {"ContentsBanner","ContentsScrollArea","NextPageButton"};
			string[] elementContent = {albumContentsHeader,"ContentsScrollArea","NextPageButton"};
			bool[] destroyGuideArr = {false,true,false};
			int[] textElementTypeArr = {0,0,0};
			prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,contentsPageChild.name);
			contentsBoundsLoaded = true;

			// Replace dummy portrait.

			GameObject albumOwnerPortraitDummy = contentsPageChild.FindChild("AlbumOwnerPortrait").gameObject;
			int ownerID = album.getOwnerID();

			string ownerName = LocalisationMang.getNPCnames()[ownerID];
			float percComp = album.getNormalisedPercentageCompletion();

			PortraitHelper.replaceEntireDummyPortrait(albumOwnerPortraitDummy,ownerID,percComp,ownerName);
			albumOwnerPortraitDummy.transform.FindChild("NameBar").FindChild("TextBanner").FindChild("Text").renderer.sortingOrder = 1500;

			Transform completionSeal = contentsPageChild.FindChild("CompletionSeal");
			completionSeal.FindChild("Text").GetComponent<TextMesh>().text = ""+((int) (percComp * 100))+"%";
			completionSeal.FindChild("Text").renderer.sortingOrder = 1004;


			PortraitHelper.applyCorrectSealSprite(completionSeal.gameObject,percComp);
		}
	}

	private void displayPhotoPageView(int para_photoPageIndex)
	{
		contentsPageChild.gameObject.SetActive(false);
		photoPageChild.gameObject.SetActive(true);
		isInContentsView = false;
		isInPageView = true;
		isInCloseupView = false;


		PhotoPage reqPage = availablePhotoPages[para_photoPageIndex];
		currPhotoPage = reqPage;
		currPhotoPageIndex = para_photoPageIndex;


			//int reqLA = currPhotoPage.getLangArea();
			//int reqDiff = currPhotoPage.getDifficulty();

			humanReadableDescText = currPhotoPage.getExplanation();



		// Adjust state of the previous and next buttons.
		Transform prevPageBtn = photoPageChild.FindChild("PreviousPageButton");
		Transform nextPageBtn = photoPageChild.FindChild("NextPageButton");
		prevPageBtn.renderer.enabled = true;
		nextPageBtn.renderer.enabled = false;
		if((availablePhotoPages.Count > 0)&&(currPhotoPageIndex > 0)&&(currPhotoPageIndex < availablePhotoPages.Count))
		{
			prevPhotoPageAvailable = true;
			prevPageBtn.renderer.enabled = true;
		}
		if((availablePhotoPages.Count > 0)&&(currPhotoPageIndex < (availablePhotoPages.Count-1))&&(currPhotoPageIndex >= 0))
		{
			nextPhotoPageAvailable = true;
			nextPageBtn.renderer.enabled = true;
		}



		// Setup page title, bookmark and prev, next buttons.
		if( ! photoPageBoundsLoaded)
		{
			string[] elementNames   = {"PhotoPageTitle","BookmarkTip","PreviousPageButton","NextPageButton","PlayPageButton","NoteScrollArea"};
			string[] elementContent = {"PhotoPageTitle","BookmarkTip","PreviousPage","NextPage","PlayPageButton","NoteScrollArea"};
			bool[] destroyGuideArr = {false,true,false,false,false,true};
			int[] textElementTypeArr = {0,0,0,0,0,0};
			prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,photoPageChild.name);
			photoPageBoundsLoaded = true;
		}


		// Setup page number text.
		Transform pageNumber = photoPageChild.FindChild("PageNumber");
		pageNumber.GetComponent<TextMesh>().text = " ";//""+(para_photoPageIndex+1);
		pageNumber.renderer.sortingOrder = 1110;




		// Setup photos on the page.

		PhotoVisualiser pVisualiser = new PhotoVisualiser();
		
		int numOfPhotos = reqPage.getNumAvailablePhotos();
		numPhotosOnCurrPage = numOfPhotos;
		Dictionary<int,Photo> availablePhotos = reqPage.getAvailablePhotos();

		for(int i=0; i<4; i++)
		{
			Transform tmpOldPhoto = photoPageChild.FindChild("PhotoPosition"+i);
			if(tmpOldPhoto != null) { 	Destroy(tmpOldPhoto.gameObject); }
			Transform tmpOldCaption = photoPageChild.FindChild("Caption"+i);
			if(tmpOldCaption != null) { Destroy(tmpOldCaption.gameObject); }
		}
				

		for(int i=0; i<4; i++)
		{
			Transform reqPhotoGuide = photoPageChild.FindChild("PhotoTemplate"+(i+1));
			Transform captionTextGuide = reqPhotoGuide.FindChild("CaptionText");
			captionTextGuide.renderer.enabled = false;

			Transform photoNumber = reqPhotoGuide.FindChild("PhotoNumber");
			photoNumber.GetComponent<TextMesh>().text = ""+ (((currPhotoPageIndex * 4) + i) + 1);
			photoNumber.renderer.sortingOrder = 2011;
			photoNumber.renderer.enabled = true;
		}

		for(int i=0; i<numOfPhotos; i++)
		{
			Transform reqPhotoGuide = photoPageChild.FindChild("PhotoTemplate"+(i+1));
			if((reqPhotoGuide != null)&&(availablePhotos.ContainsKey(i)))
			{
				Photo reqPhoto = availablePhotos[i];

				reqPhotoGuide.parent = null;
				Transform photoBlank = reqPhotoGuide.FindChild("PhotoBlank");
				photoBlank.renderer.enabled = false;
				if( ! uiBounds.ContainsKey("PhotoPosition"+i))
				{
					uiBounds.Add("PhotoPosition"+i,WorldSpawnHelper.getWorldToGUIBounds(photoBlank.renderer.bounds,upAxisArr));
				}
				GameObject nwPhotoRender = pVisualiser.producePhotoRender("PhotoPosition"+i,reqPhoto,0.025f,photoBlank.gameObject,false);
				CommonUnityUtils.setSortingOrderOfEntireObject(nwPhotoRender,1300);

				reqPhotoGuide.FindChild("SellotapeCorners").renderer.sortingOrder = 2000;
				Transform selloBanner = reqPhotoGuide.FindChild("SellotapeBanner");
				selloBanner.renderer.sortingOrder = 2010;


				Transform captionTextGuide = reqPhotoGuide.FindChild("CaptionText");
				GameObject nwCaptionObj = WordBuilderHelper.buildWordBox(99,reqPhoto.getDateTimeStampStr(),CommonUnityUtils.get2DBounds(captionTextGuide.renderer.bounds),2f,upAxisArr,genericWordBoxPrefab);
				nwCaptionObj.name = "Caption"+i;
				Destroy(nwCaptionObj.transform.FindChild("Board").gameObject);
				nwCaptionObj.transform.FindChild("Text").renderer.sortingOrder = 2011;
				nwCaptionObj.transform.parent = photoPageChild;


				Transform photoNumber = reqPhotoGuide.FindChild("PhotoNumber");
				photoNumber.renderer.enabled = false;


				nwPhotoRender.transform.parent = photoPageChild;
				reqPhotoGuide.parent = photoPageChild;
			}
		}

		triggerSoundAtCamera("PageTurn");
	}

	private void displayCloseupView(int para_photoPosID, Photo para_reqPhotoData)
	{
		isInContentsView = false;
		isInPageView = false;

		Transform closeUpPhotoElement = Resources.Load<Transform>("Prefabs/CloseUpPhotoElement");

		transform.parent = null;
		Transform nwCloseup = (Transform) Instantiate(closeUpPhotoElement,new Vector3(9000,transform.position.y,transform.position.z),Quaternion.identity);
		nwCloseup.name = "Closeup";
		//nwCloseup.renderer.sortingOrder = 1600;
		//nwCloseup.FindChild("PhotoWhiteArea").renderer.sortingOrder = 1601;
		GameObject photoGuideObj = nwCloseup.FindChild("PhotoGuide").gameObject;
		

		//GameObject reqPhotoObjToCopy = photoPageChild.FindChild("PhotoPosition"+para_photoPosID).gameObject;

		PhotoVisualiser pvis = new PhotoVisualiser();
		GameObject photoRendForCloseup = pvis.producePhotoRender("TmpCloseupPhoto",para_reqPhotoData,0.05f,photoGuideObj,true);


		//GameObject photoRendForCloseup = ((Transform) Instantiate(reqPhotoObjToCopy.transform,photoGuideObj.transform.position,photoGuideObj.transform.rotation)).gameObject;
		//GameObject photoRendBackground = photoRendForCloseup.transform.FindChild("PhotoBackground").gameObject;
		//Rect photoRendBackgroundBounds = CommonUnityUtils.get2DBounds(photoRendBackground.renderer.bounds);
		photoRendForCloseup.transform.localScale = new Vector3(0.5f,0.5f,1f);//new Vector3(photoGuideObj.renderer.bounds.size.x / photoRendBackgroundBounds.width, photoGuideObj.renderer.bounds.size.y / photoRendBackgroundBounds.height, 1);

		//fixAnimationsInCloseup(reqPhotoObjToCopy,photoRendForCloseup,new List<string>() { "PlayerAvatar","QuestGiver","ActivityOwner" }); 

		photoRendForCloseup.transform.parent = nwCloseup;
		CommonUnityUtils.setSortingLayerOfEntireObject(nwCloseup.gameObject,"SpriteGUI");
		CommonUnityUtils.setSortingOrderOfEntireObject(nwCloseup.gameObject,3600);
		nwCloseup.renderer.sortingOrder = 0;
		nwCloseup.FindChild("PhotoWhiteArea").renderer.sortingOrder = 1;
		nwCloseup.parent = transform;
		transform.parent = Camera.main.transform;

		PhotoVisualiser pVis = new PhotoVisualiser();
		pVis.makeAllCharactersStill(photoRendForCloseup);


		CustomAnimationManager aniMang = nwCloseup.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>() { 0.5f }));
		batch1.Add(new AniCommandPrep("TeleportToLocation",1,new List<System.Object>() { new float[3]{transform.position.x,transform.position.y,transform.position.z}}));
		batchLists.Add(batch1);
		aniMang.init(batchLists);

		// Manual Scaling. (Tmp fix).
		nwCloseup.localScale = new Vector3(2,2,1);
		isInCloseupView = true;
	}

	public void getOutOfCloseupView()
	{
		Transform closeupChild = transform.FindChild("Closeup");
		if(closeupChild != null) { Destroy(closeupChild.gameObject); }
		isInCloseupView = false;
		isInPageView = true;
	}


	private void triggerPlayPhotoPage()
	{
		triggerPlayPhotoPage(null);
	}

	
	private void triggerPlayPhotoPage(ExternalParams extParams)
	{

		if(extParams == null)// The child (not the teacher) invoked this photo page play.
		{	

			control.launchSurpriseQuest(currPhotoPage.getLangArea(),currPhotoPage.getDifficulty());
			DestroyImmediate(Camera.main.transform.FindChild("AlbumInnerView").gameObject);

		}
		else
		{
			control.launchQuest(extParams,LocalisationMang.getOwnerNpcOfActivity(extParams.acIdOverride),"Album",Mode.PLAY);

			if(Camera.main.transform.FindChild("AlbumInnerView")!=null)
				DestroyImmediate(Camera.main.transform.FindChild("AlbumInnerView").gameObject);
			//gbDisp.isInAlbumInnerView = false;

		}


	}


	private new void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string, GUIStyle>();

		float targetWidth = 1280f;
		float targetHeight = 800f;
		Vector3 scaleForCurrRes = new Vector3((Screen.width * 1.0f)/targetWidth,(Screen.height * 1.0f)/targetHeight,1f);

		GUIStyle midLabelStyle = new GUIStyle(GUI.skin.label);
		midLabelStyle.alignment = TextAnchor.MiddleCenter;

		GUIStyle midTitleLabelStyle = new GUIStyle(GUI.skin.label);
		midTitleLabelStyle.alignment = TextAnchor.MiddleCenter;
		midTitleLabelStyle.fontSize = (int) (30 * scaleForCurrRes.x);

		GUIStyle contentsTitleLabelStyle = new GUIStyle(GUI.skin.label);
		contentsTitleLabelStyle.alignment = TextAnchor.MiddleCenter;
		contentsTitleLabelStyle.fontSize = (int) (25 * scaleForCurrRes.x);

		GUIStyle humanReadableExamplesStyle = new GUIStyle(GUI.skin.label);
		humanReadableExamplesStyle.alignment = TextAnchor.MiddleLeft;
		humanReadableExamplesStyle.fontSize = (int) (30 * scaleForCurrRes.x);


		availableGUIStyles.Add("MidLabel",midLabelStyle);
		availableGUIStyles.Add("TitleLabel",midTitleLabelStyle);
		availableGUIStyles.Add("ContentsTitle",contentsTitleLabelStyle);
		availableGUIStyles.Add("HumanReadableExamples",humanReadableExamplesStyle);
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

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_sourceID == "TeacherFeaturesWindow")
		{
			if(para_eventID == "Ok")
			{
				triggerPlayPhotoPage((ExternalParams) para_eventData);
			}
			else if(para_eventID == "Close")
			{
				isInTeacherFeatureView = false;
			}
		}
	}
	

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

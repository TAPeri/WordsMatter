/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class PhotoRemovedWindow : ILearnRWUIElement, CustomActionListener, IActionNotifier
{
	bool showOkButton = false;
	bool preped = false;

	//GameObject photoObjToDelete;
	
	void Start()
	{
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
				prepGUIStyles();
				hasInitGUIStyles = true;
			}
			else
			{
				GUI.color = Color.black;
				GUI.Label(uiBounds["Title"],textContent["Title"],availableGUIStyles["NoticeContent"]);
				GUI.color = Color.white;
				if(showOkButton)
				{
					GUI.color = Color.black;
					GUI.Label(uiBounds["OkBtn"],textContent["OkBtn"],availableGUIStyles["NoticeContent"]);
					GUI.color = Color.clear;
					if(GUI.Button(uiBounds["OkBtn"],textContent["OkBtn"],availableGUIStyles["Button"]))
					{
						notifyAllListeners("PhotoRemovedWindow","Close",null);
						Destroy(transform.gameObject);
					}				
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
		noticeContentStyle.alignment = TextAnchor.MiddleCenter;
		noticeContentStyle.fontSize = (int) (30 * scaleForCurrRes.x);
		
		GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
		btnStyle.wordWrap = true;
		btnStyle.fontSize = (int) (16 * scaleForCurrRes.x);
		btnStyle.normal.textColor = Color.black;
		
		availableGUIStyles.Add("NoticeContent",noticeContentStyle);
		availableGUIStyles.Add("Button",btnStyle);
		hasInitGUIStyles = true;
	}
	
	public void init(PDBCRemovePhoto para_removePhotoParams, string para_questGiverName)
	{
		int questGiverID = para_removePhotoParams.questGiverID;
		string questGiverName = para_questGiverName;
		
		
		
		// Apply portrait.
		Transform dummyPortrait = transform.FindChild("Portrait");
		PortraitHelper.replaceEntireDummyPortrait(dummyPortrait.gameObject,questGiverID,0,questGiverName,0.04f);
		CommonUnityUtils.setSortingLayerOfEntireObject(dummyPortrait.gameObject,"SpriteGUI");
		CommonUnityUtils.setSortingOrderOfEntireObject(dummyPortrait.gameObject,6000);

		
		// Apply photo render.
		GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
		Photo reqPhoto = gbMang.getPhoto(questGiverID, para_removePhotoParams.langAreaID, para_removePhotoParams.diffIndexInLangArea, para_removePhotoParams.photoDiffPosition);
		PhotoVisualiser pVisualiser = new PhotoVisualiser();
		GameObject nwRenderObj = pVisualiser.producePhotoRender("NewPhotoRender",reqPhoto,0.05f,transform.FindChild("PhotoGuide").gameObject,true);
		CommonUnityUtils.setSortingLayerOfEntireObject(nwRenderObj,"SpriteGUI");
		CommonUnityUtils.setSortingOrderOfEntireObject(nwRenderObj,6000);
		nwRenderObj.transform.parent = transform;


		// Apply gradual fade effect on photo.
		applyFadeAnimationToPhoto(nwRenderObj);
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
		}
		else if(para_eventID == "PhotoFadeAni")
		{
			//Destroy(photoObjToDelete);
			transform.FindChild("warning").renderer.enabled = true;
			transform.FindChild("DoneButton").gameObject.SetActive(true);
			showOkButton = true;
		}
	}
	
	private void applyPrep()
	{
		/*// Correct render order.
		fixRenderSortingForElements(new List<string[]>() { (new string[]{"DimScreen"}),
			(new string[]{"Frame"}),
			(new string[]{"ContentPane"}),
			(new string[]{"Title","BlackBackground","OkBtn"}),
			(new string[]{"PhotoGuide"})});*/
		
		
		// Init text items.
		string[] elementNames = new string[]{"Title","Portrait","PortraitGuide","OkBtn"};
		string[] elementContent = new string[]{LocalisationMang.getString("GhostB*PhotoRemove"),"Portrait","PortraitGuide",LocalisationMang.translate("Ok")};
		bool[] destroyGuideArr = new bool[]{true,false,false,true};
		int[] textElementTypeArr = new int[]{0,0,0,0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);
		
		preped = true;
	}

	private void applyFadeAnimationToPhoto(GameObject para_photoGObj)
	{
		//photoObjToDelete = para_photoGObj;

		CustomAnimationManager aniMang = para_photoGObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("ColorTransition",1, new List<System.Object>() { new float[4] {1,1,1,0.1f}, 4f }));
		//List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
		//batch2.Add(new AniCommandPrep("DestroyObject",1,new List<System.Object>()));
		batchLists.Add(batch1);
		aniMang.registerListener("PhotoRemovedWindow",this);
		aniMang.init("PhotoFadeAni",batchLists);
	}
	
	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

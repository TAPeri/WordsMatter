/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class GoalWindow : ILearnRWUIElement, CustomActionListener, IActionNotifier
{

	string goalText;
	bool inScene = false;

	string[] elementNames;   
	string[] elementContent; 
	bool[] destroyGuideArr;
	int[] textElementTypeArr;

	Vector2 goalTextScrollPos;
	Vector2 tmpVect;
	



	void Start()
	{	
		//teleportToOutOfScene();

		/*// Correct render order.
		fixRenderSortingForElements(new List<string[]>() { (new string[]{"DimScreen"}),
														   (new string[]{"Frame"}),
														   (new string[]{"ContentPane"}),
														   (new string[]{"GoalImg","TextArea","OkBtn"})});*/
		
		transform.FindChild("DimScreen").GetComponent<SpriteRenderer>().enabled = true;

		
		// Init text items.
		elementNames = new string[]{"TextArea","OkBtn"};
		elementContent = new string[]{""+goalText,LocalisationMang.translate("Ok")};
		destroyGuideArr = new bool[]{false,false};
		textElementTypeArr = new int[]{0,0};
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);

		goalTextScrollPos = new Vector2();
		tmpVect = new Vector2();

		enterScene();
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
			if(inScene)
			{
				//GUI.color = Color.clear;
				GUI.color = Color.black;



				if((Application.platform == RuntimePlatform.Android)
				   ||(Application.platform == RuntimePlatform.IPhonePlayer))
				{
					if(Input.touchCount == 1)
					{
						tmpVect.x = Input.touches[0].position.x;
						tmpVect.y = Screen.height - Input.touches[0].position.y;
						
						if(uiBounds["TextArea"].Contains(tmpVect))
						{
							goalTextScrollPos.y += (Input.touches[0].deltaPosition.y * 1f);
						}
					}
				}

			

				GUILayout.BeginArea(uiBounds["TextArea"]);
				goalTextScrollPos = GUILayout.BeginScrollView(goalTextScrollPos);
				GUILayout.BeginVertical();
				GUILayout.Label(textContent["TextArea"],availableGUIStyles["GoalText"]);
				GUILayout.EndVertical();
				GUILayout.EndScrollView();
				GUILayout.EndArea();


				//GUI.Label(uiBounds["TextArea"],textContent["TextArea"],availableGUIStyles["FieldContent"]);
				GUI.Label(uiBounds["OkBtn"],LocalisationMang.translate("Ok"),availableGUIStyles["FieldContent"]);
				GUI.color = Color.clear;
				if(GUI.Button(uiBounds["OkBtn"],textContent["OkBtn"],availableGUIStyles["Button"]))
				{


					GameObject camGObj = Camera.main.gameObject;
					

					GameObject nwSFX = ((Transform) Instantiate(Resources.Load<Transform>("Prefabs/SFxBox"),camGObj.transform.position,Quaternion.identity)).gameObject;
					nwSFX.name = "BubbleClick";
					AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
					audS.clip = (AudioClip) Resources.Load("Sounds/BubbleClick",typeof(AudioClip));
					audS.volume = 1f;
					audS.loop = false;

					audS.Play();


					inScene = false;
					exitScene();
				}
			}
		}
	}

	public void init(string para_goalText)
	{
		goalText = para_goalText;
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		//if(para_sourceID == "GoalWindow")
		//{
			if(para_eventID == "EnterScene")
			{
				//elementNames = new string[]{"TextArea","OkBtn"};
				//elementContent = new string[]{""+goalText,"OK           "};
				//destroyGuideArr = new bool[]{false,false};
				//textElementTypeArr = new int[]{1,1};
				//prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);
				inScene = true;
			}
			else if(para_eventID == "ExitScene")
			{
				notifyAllListeners("GoalWindow","Close",null);
				Destroy(transform.gameObject);
			}
		//}
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

		GUIStyle goalTextStyle = new GUIStyle(GUI.skin.label);
		goalTextStyle.alignment = TextAnchor.MiddleLeft;
		goalTextStyle.fontSize = (int) (30 * scaleForCurrRes.x);
		
		GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
		btnStyle.wordWrap = true;
		btnStyle.normal.textColor = Color.black;
		
		availableGUIStyles.Add("FieldTitle",fieldTitleStyle);
		availableGUIStyles.Add("FieldContent",fieldContentStyle);
		availableGUIStyles.Add("GoalText",goalTextStyle);
		availableGUIStyles.Add("Button",btnStyle);
		hasInitGUIStyles = true;
	}

	private void teleportToOutOfScene()
	{
		Rect camera2DWorldBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,true);

		Vector3 teleportPos = new Vector3(transform.position.x,
		                                  camera2DWorldBounds.y - camera2DWorldBounds.height - (transform.FindChild("WindowBounds").renderer.bounds.size.y/2f),
		                                  transform.position.z);

		transform.position = teleportPos;
	}

	private void enterScene()
	{
		Vector3 destPos = new Vector3(transform.position.x,0,transform.position.z);

		destPos = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,Camera.main.transform.position.z + 3f);

		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		//batch1.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3] {destPos.x,destPos.y,destPos.z}, 1f, false }));
		batch1.Add(new AniCommandPrep("TeleportToLocation",1,new List<System.Object>() { new float[3] {destPos.x,destPos.y,destPos.z} }));
		batchLists.Add(batch1);
		aniMang.registerListener("GoalWindow",this);
		aniMang.init("EnterScene",batchLists);
	}

	private void exitScene()
	{
		respondToEvent("GoalWindow","ExitScene",null);

		/*Rect camera2DWorldBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,true);

		Vector3 destPos = new Vector3(transform.position.x,
		                                  camera2DWorldBounds.y - camera2DWorldBounds.height - (transform.FindChild("WindowBounds").renderer.bounds.size.y/2f),
		                                  transform.position.z);

		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		//batch1.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3] {destPos.x,destPos.y,destPos.z}, 1f, false }));
		batch1.Add(new AniCommandPrep("TeleportToLocation",1,new List<System.Object>() { new float[3] {destPos.x,destPos.y,destPos.z} }));
		batchLists.Add(batch1);
		aniMang.registerListener("GoalWindow",this);
		aniMang.init("ExitScene",batchLists);*/
	}



	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

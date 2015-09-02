/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class PersonHelperWindow : ILearnRWUIElement, IActionNotifier
{

	ActivitySessionMetaData acMetaData;
	string goalText;

	Vector2 langAreaScrollPos;
	Vector2 difficultyScrollPos;
	Vector2 activityGoalScrollPos;
	Vector2 tmpVect;

	

	void Start()
	{

		GameObject camGObj = Camera.main.gameObject;
		
		
		GameObject nwSFX = ((Transform) Instantiate(Resources.Load<Transform>("Prefabs/SFxBox"),camGObj.transform.position,Quaternion.identity)).gameObject;
		nwSFX.name = "BubbleClick";
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/BubbleClick",typeof(AudioClip));
		audS.volume = 1f;
		audS.loop = false;
		
		audS.Play();


		this.loadTextures();

		// Correct render order.
		//fixRenderSortingForElements(new List<string[]>() { (new string[]{"DimScreen"}),(new string[]{"ContentPane"}),(new string[]{"PersonPortrait"}) });

		// Switch on dimer object.
		transform.FindChild("DimScreen").renderer.enabled = true;

		// Init text items.
		string[] elementNames   = {"TitleBar","LangAreaTitle","LangAreaTA","DifficultyTitle","DifficultyTA","ActivityGoalTitle","ActivityGoalTA","OkBtn"};
		string[] elementContent = {LocalisationMang.translate("You are helping")+" "+acMetaData.getCharacterHelperName(),
							  	   LocalisationMang.translate("Language area"),
									acMetaData.getLanguageArea(),
									LocalisationMang.translate("Difficulty"),
									acMetaData.getDifficulty(),
									LocalisationMang.translate("Activity goal"),
									""+goalText,
									LocalisationMang.translate("Ok")};
		bool[] destroyGuideArr = {true,true,false,true,false,true,false,true};
		int[] textElementTypeArr = {0,0,0,0,0,0,0,0};

		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);

		// Setup progress bar foreground bounds.
		//Rect progressBarUIBounds = uiBounds["ProgressBar"];
		//Rect progressBarForegroundUIBounds = new Rect(progressBarUIBounds.x,progressBarUIBounds.y,progressBarUIBounds.width * acMetaData.getProgress(),progressBarUIBounds.height);
		//uiBounds.Add("ProgBarFore",progressBarForegroundUIBounds);

		langAreaScrollPos = new Vector2();
		difficultyScrollPos = new Vector2();
		activityGoalScrollPos = new Vector2();
		tmpVect = new Vector2();
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
			GUI.color = Color.black;
			GUI.Label(uiBounds["TitleBar"],textContent["TitleBar"],availableGUIStyles["Title"]);

			GUI.Label(uiBounds["LangAreaTitle"],textContent["LangAreaTitle"],availableGUIStyles["FieldTitle"]);
			GUI.Label(uiBounds["DifficultyTitle"],textContent["DifficultyTitle"],availableGUIStyles["FieldTitle"]);
			GUI.Label(uiBounds["ActivityGoalTitle"],textContent["ActivityGoalTitle"],availableGUIStyles["FieldTitle"]);
			//GUI.Label(uiBounds["ProgressBarTitle"],textContent["ProgressBarTitle"],availableGUIStyles["FieldTitle"]);


			if((Application.platform == RuntimePlatform.Android)
			   ||(Application.platform == RuntimePlatform.IPhonePlayer))
			{
				if(Input.touchCount == 1)
				{
					tmpVect.x = Input.touches[0].position.x;
					tmpVect.y = Screen.height - Input.touches[0].position.y;
					
					if(uiBounds["LangAreaTA"].Contains(tmpVect))
					{
						langAreaScrollPos.y += (Input.touches[0].deltaPosition.y * 1f);
					}
				}
			}

			//GUI.Label(uiBounds["LangAreaTA"],textContent["LangAreaTA"],availableGUIStyles["FieldContent"]);
			GUILayout.BeginArea(uiBounds["LangAreaTA"]);
			GUILayout.BeginVertical();
			langAreaScrollPos = GUILayout.BeginScrollView(langAreaScrollPos,GUI.skin.horizontalScrollbar,GUIStyle.none);
			GUILayout.Label(textContent["LangAreaTA"],availableGUIStyles["FieldContent"]);
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.EndArea();


			if((Application.platform == RuntimePlatform.Android)
			   ||(Application.platform == RuntimePlatform.IPhonePlayer))
			{
				if(Input.touchCount == 1)
				{
					tmpVect.x = Input.touches[0].position.x;
					tmpVect.y = Screen.height - Input.touches[0].position.y;
					
					if(uiBounds["DifficultyTA"].Contains(tmpVect))
					{
						difficultyScrollPos.y += (Input.touches[0].deltaPosition.y * 1f);
					}
				}
			}

			//GUI.Label(uiBounds["DifficultyTA"],textContent["DifficultyTA"],availableGUIStyles["FieldContent"]);
			GUILayout.BeginArea(uiBounds["DifficultyTA"]);
			GUILayout.BeginVertical();
			difficultyScrollPos = GUILayout.BeginScrollView(difficultyScrollPos,GUI.skin.horizontalScrollbar,GUIStyle.none);
			GUILayout.Label(textContent["DifficultyTA"],availableGUIStyles["FieldContent"]);
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.EndArea();


			if((Application.platform == RuntimePlatform.Android)
			   ||(Application.platform == RuntimePlatform.IPhonePlayer))
			{
				if(Input.touchCount == 1)
				{
					tmpVect.x = Input.touches[0].position.x;
					tmpVect.y = Screen.height - Input.touches[0].position.y;
					
					if(uiBounds["ActivityGoalTA"].Contains(tmpVect))
					{
						activityGoalScrollPos.y += (Input.touches[0].deltaPosition.y * 1f);
					}
				}
			}


			//GUI.Label(uiBounds["ActivityGoalTA"],textContent["ActivityGoalTA"],availableGUIStyles["FieldContent"]);
			GUILayout.BeginArea(uiBounds["ActivityGoalTA"]);
			GUILayout.BeginVertical();
			activityGoalScrollPos = GUILayout.BeginScrollView(activityGoalScrollPos,GUIStyle.none,GUI.skin.verticalScrollbar);
			GUILayout.Label(textContent["ActivityGoalTA"],availableGUIStyles["FieldContentML"]);
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.EndArea();


			GUI.Label(uiBounds["OkBtn"],textContent["OkBtn"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.clear;
			if(GUI.Button(uiBounds["OkBtn"]," ",availableGUIStyles["Button"]))
			{


				GameObject camGObj = Camera.main.gameObject;
				

				GameObject nwSFX = ((Transform) Instantiate(Resources.Load<Transform>("Prefabs/SFxBox"),camGObj.transform.position,Quaternion.identity)).gameObject;
				nwSFX.name = "BubbleClick";
				AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
				audS.clip = (AudioClip) Resources.Load("Sounds/BubbleClick",typeof(AudioClip));
				audS.volume = 1f;
				audS.loop = false;

				audS.Play();

				notifyAllListeners("PersonHelperWindow","Close",null);
			}

			GUI.color = Color.white;

			// Progress bar stuff.
			//GUI.DrawTexture(uiBounds["ProgBarFore"],availableTextures["ProgBarFore"]);
			//GUI.color = Color.black;
			//GUI.Label(uiBounds["ProgressBar"],textContent["ProgressBar"],availableGUIStyles["FieldContent"]);
			//GUI.color = Color.white;
		}
	}

	public void init(ActivitySessionMetaData para_acMetaData, string para_goalText)
	{
		acMetaData = para_acMetaData;
		goalText = para_goalText;

		// Update the person portrait to have the quest giver's portrait.
		int questGiverID = acMetaData.getQuestGiverID();
		Transform personPortraitObj = transform.FindChild("PersonPortrait");
		PortraitHelper.replaceEntireDummyPortrait(personPortraitObj.gameObject,questGiverID,0,acMetaData.getCharacterHelperName(),0.04f);
		CommonUnityUtils.setSortingLayerOfEntireObject(personPortraitObj.gameObject,"SpriteGUI");
		CommonUnityUtils.setSortingOrderOfEntireObject(personPortraitObj.gameObject,6000);
	}

	protected new void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string, GUIStyle>();

		float targetWidth = 1280f;
		float targetHeight = 800f;
		Vector3 scaleForCurrRes = new Vector3((Screen.width * 1.0f)/targetWidth,(Screen.height * 1.0f)/targetHeight,1f);

		GUIStyle windowTitleStyle = new GUIStyle(GUI.skin.label);
		windowTitleStyle.alignment = TextAnchor.MiddleCenter;
		windowTitleStyle.fontSize = (int) (40 * scaleForCurrRes.x);

		GUIStyle fieldTitleStyle = new GUIStyle(GUI.skin.label);
		fieldTitleStyle.alignment = TextAnchor.MiddleCenter;
		fieldTitleStyle.fontSize = (int) (20 * scaleForCurrRes.x);
		
		GUIStyle fieldContentStyle = new GUIStyle(GUI.skin.label);
		fieldContentStyle.alignment = TextAnchor.MiddleCenter;
		fieldContentStyle.fontSize = (int) (30 * scaleForCurrRes.x);

		GUIStyle fieldContentMLStyle = new GUIStyle(GUI.skin.label);
		fieldContentMLStyle.alignment = TextAnchor.MiddleLeft;
		fieldContentMLStyle.fontSize = (int) (30 * scaleForCurrRes.x);
		
		GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
		btnStyle.wordWrap = true;
		btnStyle.fontSize = (int) (40 * scaleForCurrRes.x);
		btnStyle.normal.textColor = Color.black;
		
		availableGUIStyles.Add("Title",windowTitleStyle);
		availableGUIStyles.Add("FieldTitle",fieldTitleStyle);
		availableGUIStyles.Add("FieldContent",fieldContentStyle);
		availableGUIStyles.Add("FieldContentML",fieldContentMLStyle);
		availableGUIStyles.Add("Button",btnStyle);
		hasInitGUIStyles = true;
	}

	protected new void loadTextures()
	{
		availableTextures = new Dictionary<string, Texture2D>();

		Texture2D progressBarTex = new Texture2D(1,1);
		progressBarTex.SetPixel(1,1,Color.green);
		progressBarTex.Apply();

		availableTextures.Add("ProgBarFore",progressBarTex);
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

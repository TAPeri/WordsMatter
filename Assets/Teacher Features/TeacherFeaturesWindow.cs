/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class TeacherFeaturesWindow : ILearnRWUIElement,  IActionNotifier
{

	Vector2 scrollPos;
	Vector2 tmpVect;

	//bool canChangeLanguageArea = false;
	//bool canChangeDifficulty = false;

	int langArea;
	int diff;
	int questGiverID;

//	IGBActivityReference acRefMat;
	List<ApplicationID> applicableActivities;
	int currAcListIndex;

	ApplicationID currAcID = ApplicationID.APP_ID_NOT_SET_UP;

	string defaultParamStrForConfig;
	Dictionary<string,string> optionParamStore;
	List<string> paramKeyOrder;
	

	AvailableOptions loadedOptions;
	Dictionary<ApplicationID,AvailableOptions> optionsCache = null;
	int[] selectorArr = null;





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

			GUI.Label(uiBounds["Title"],textContent["Title"],availableGUIStyles["FieldTitle"]);
			GUI.Label(uiBounds["LangAreaTitle"],textContent["LangAreaTitle"],availableGUIStyles["FieldContent"]);
			GUI.Label(uiBounds["DifficultyTitle"],textContent["DifficultyTitle"],availableGUIStyles["FieldContent"]);

			GUI.Label(uiBounds["LangAreaContent"],textContent["LangAreaContent"],availableGUIStyles["FieldContent"]);
			GUI.Label(uiBounds["DifficultyContent"],textContent["DifficultyContent"],availableGUIStyles["FieldContent"]);

			GUI.Label(uiBounds["ApplicableActivitiesTitle"],textContent["ApplicableActivitiesTitle"],availableGUIStyles["FieldContentTiny"]);
			GUI.Label(uiBounds["ActivityOptionsTitle"],textContent["ActivityOptionsTitle"],availableGUIStyles["FieldContent"]);


			GUI.color = Color.clear;

			if(GUI.Button(uiBounds["PrevActivityButton"],textContent["PrevActivityButton"]))
			{
				// See previous activity options.
				if(currAcListIndex > 0)
				{
					triggerSoundAtCamera("Blop",1f,false);

					currAcListIndex--;
					updateActivitySymbol();
				}
			}

			if(GUI.Button(uiBounds["NextActivityButton"],textContent["NextActivityButton"]))
			{
				// See next activity options.
				if(currAcListIndex < (applicableActivities.Count)-1)
				{
					triggerSoundAtCamera("Blip",1f,false);
					currAcListIndex++;
					updateActivitySymbol();
				}
			}


			if(currAcID != ApplicationID.APP_ID_NOT_SET_UP){

				GUI.color = Color.black;
				GUI.Label(uiBounds["OkButton"],textContent["OkButton"],availableGUIStyles["FieldContent"]);
				GUI.color = Color.clear;


				if(GUI.Button(uiBounds["OkButton"],""))
				{
				// Create level parameter string based on the options.
					string finalParamStr = buildParamString();
					ExternalParams eParams = new ExternalParams(currAcID,questGiverID,langArea,diff,finalParamStr,true);
					notifyAllListeners("TeacherFeaturesWindow","Ok",eParams);
					//Destroy(transform.gameObject);
				}

			}

			GUI.color = Color.white;



			if((Application.platform == RuntimePlatform.Android)
			   ||(Application.platform == RuntimePlatform.IPhonePlayer))
			{
				if(Input.touchCount == 1)
				{
					tmpVect.x = Input.touches[0].position.x;
					tmpVect.y = Screen.height - Input.touches[0].position.y;
					
					if(uiBounds["ActivityOpScrollArea"].Contains(tmpVect))
					{
						scrollPos.y += (Input.touches[0].deltaPosition.y * 1f);
					}
				}
			}


			GUILayout.BeginArea(uiBounds["ActivityOpScrollArea"]);
			scrollPos = GUILayout.BeginScrollView(scrollPos);

			// Write code about activity specific options here.


			GUIStyle selGridStyle = availableGUIStyles["SelectionGrid"];


			List<string> opKeys = loadedOptions.validKeys;
			Dictionary<string,OptionInfo> opLookup = loadedOptions.validOptions;

			for(int i=0; i<opKeys.Count; i++)
			{
				string currOpKey = opKeys[i];
				OptionInfo currOpInfo = opLookup[currOpKey];

				GUILayout.BeginVertical();
				GUI.color = Color.black;
				GUILayout.BeginHorizontal();
				GUILayout.Label(currOpInfo.opReadableTitle,availableGUIStyles["FieldContentML"]);
				GUI.color = Color.white;
				//GUILayout.BeginHorizontal();
				selectorArr[i] = GUILayout.SelectionGrid(selectorArr[i],currOpInfo.opReadableSelections,currOpInfo.opReadableSelections.Length,selGridStyle);
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}


			GUILayout.EndScrollView();
			GUILayout.EndArea();

		}
	}

	private new void prepGUIStyles()
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

		GUIStyle fieldContentMLStyle = new GUIStyle(GUI.skin.label);
		fieldContentMLStyle.alignment = TextAnchor.MiddleLeft;
		fieldContentMLStyle.fontSize = (int) (30 * scaleForCurrRes.x);

		GUIStyle fieldContentTinyStyle = new GUIStyle(GUI.skin.label);
		fieldContentTinyStyle.alignment = TextAnchor.MiddleCenter;
		fieldContentTinyStyle.fontSize = (int) (25 * scaleForCurrRes.x);


		Texture2D clearTex = new Texture2D(1,1);
		clearTex.SetPixel(0,0,ColorUtils.convertColor(0,0,0,20));
		clearTex.Apply();
	
		GUIStyle selectionGridStyle = new GUIStyle(GUI.skin.button);
		selectionGridStyle.normal.background = clearTex;
		selectionGridStyle.normal.textColor = Color.black;
		selectionGridStyle.fixedHeight = (int) (60 * scaleForCurrRes.x);



		GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
		btnStyle.wordWrap = true;
		
		availableGUIStyles.Add("FieldTitle",fieldTitleStyle);
		availableGUIStyles.Add("FieldContent",fieldContentStyle);
		availableGUIStyles.Add("FieldContentML",fieldContentMLStyle);
		availableGUIStyles.Add("FieldContentTiny",fieldContentTinyStyle);
		availableGUIStyles.Add("SelectionGrid",selectionGridStyle);
		availableGUIStyles.Add("Button",btnStyle);
		hasInitGUIStyles = true;
	}

	// Use this if language area and difficulty are fixed and cannot be altered by the user.
	// Eg. If a user has selected a langArea and Diff by selecting to play a ghostbook photo page.
	public void init(int para_langArea, int para_difficulty, int para_questGiverID)
	{
		//canChangeLanguageArea = false;
		//canChangeDifficulty = false;

		GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();

		//IGBLangAreaReference langAreaRef = gbMang.getLangAreaReferenceMaterial();
		//IGBDifficultyReference diffRefMat = gbMang.getDifficultyReferenceMaterial();
	//	acRefMat = gbMang.getActivityReferenceMaterial();

		string langAreaName = gbMang.getNameForLangArea(para_langArea);
		string diffName = gbMang.createDifficultyShortDescription(para_langArea,para_difficulty);


		langArea = para_langArea;
		diff = para_difficulty;
		questGiverID = para_questGiverID;



		// Switch on dimer object.
		GameObject dimScreen = transform.FindChild("DimScreen").gameObject;
		dimScreen.renderer.enabled = true;
		dimScreen.renderer.material.color = Color.black;
		
		// Init text items.
		string[] elementNames   = {"Title","LangAreaTitle","DifficultyTitle","LangAreaContent","DifficultyContent","ActivityOpScrollArea","PrevActivityButton","NextActivityButton","OkButton","ApplicableActivitiesTitle","ActivityOptionsTitle"};
		string[] elementContent = {LocalisationMang.translate("Teacher options"),LocalisationMang.translate("Language area"),LocalisationMang.translate("Difficulty"),langAreaName,diffName,"AcOpScrollArea","<",">",LocalisationMang.translate("Ok"),LocalisationMang.translate("Applicable activities"),LocalisationMang.translate("Activity options")};
		bool[] destroyGuideArr = {true,true,true,false,false,false,true,true,true,true,true};
		int[] textElementTypeArr = {0,0,0,0,0,0,0,0,0,0,0};
		
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);

		scrollPos = new Vector2();
		tmpVect = new Vector2();

		applicableActivities = LocalisationMang.getAllApplicableActivitiesForLangArea(langArea);
		
		currAcListIndex = 0;//applicableActivities.Count-1;
		
		currAcID = applicableActivities[currAcListIndex];//acRefMat.getAcIDEnum_byAcPKey();
		
		
		updateActivitySymbol();


	}

	private void updateActivitySymbol()
	{

		currAcID  = applicableActivities[currAcListIndex];

		Debug.Log(currAcID);

		if(currAcID == ApplicationID.APP_ID_NOT_SET_UP){

			Debug.LogWarning("needs something here?");
			if(optionsCache == null) { optionsCache = new Dictionary<ApplicationID, AvailableOptions>(); }

			updateAvailableOptions();


			return;
		}

		Transform reqActivitySymbolPrefab = Resources.Load<Transform>("Prefabs/Ghostbook/ActivitySymbols/SmallSymbols_"+currAcID);

		if(reqActivitySymbolPrefab != null)
		{
			Sprite reqSymbolSprite = reqActivitySymbolPrefab.GetComponent<SpriteRenderer>().sprite;
			transform.FindChild("ActivitySymbol").GetComponent<SpriteRenderer>().sprite = reqSymbolSprite;
		}


		if(optionParamStore == null) { 
			optionParamStore = new Dictionary<string, string>(); 
			paramKeyOrder = new List<string>(); 
		} else { 
			optionParamStore.Clear(); 
			paramKeyOrder.Clear(); 
		}

		defaultParamStrForConfig = LocalisationMang.getFirstLevelConfiguration(currAcID,langArea,diff);

		string[] splitParams = defaultParamStrForConfig.Split('-');

		for(int i=0; i<splitParams.Length; i++)
		{
			string tmpItem = splitParams[i];
			if((tmpItem != null)&&(tmpItem != ""))
			{
				string firstChar = ""+tmpItem[0];
				paramKeyOrder.Add(firstChar);
				optionParamStore.Add(firstChar,tmpItem);
			}
		}

		updateAvailableOptions();
	}


	private void updateAvailableOptions()
	{
		ApplicationID reqAppIDToSearch = currAcID;

		// Warning. Cache usage should be invalidated if the language area and difficulty combo changes.
		if(optionsCache == null) { optionsCache = new Dictionary<ApplicationID, AvailableOptions>(); }
		if(optionsCache.ContainsKey(reqAppIDToSearch))
		{
			loadedOptions = optionsCache[reqAppIDToSearch];
		}
		else
		{
			loadedOptions = LocalisationMang.workOutOptionsForActivityAndLangArea(reqAppIDToSearch,langArea);
			optionsCache.Add(reqAppIDToSearch,loadedOptions);
		}

		selectorArr = new int[loadedOptions.validKeys.Count];
		for(int i=0; i<loadedOptions.validKeys.Count; i++)
		{
			selectorArr[i] = 0;
		}
	}

	private void invalidateOptionCache()
	{
		if(optionsCache == null) { optionsCache = new Dictionary<ApplicationID, AvailableOptions>(); }
		optionsCache.Clear();
	}



	private void extractUserChoices()
	{
		if(optionParamStore == null)
		{
			optionParamStore = new Dictionary<string, string>();
		}


		List<string> opKeys = loadedOptions.validKeys;
		Dictionary<string,OptionInfo> opLookup = loadedOptions.validOptions;
		
		for(int i=0; i<opKeys.Count; i++)
		{
			string currOpKey = opKeys[i];
			OptionInfo currOpInfo = opLookup[currOpKey];

			string codeSelection = currOpKey + (""+ currOpInfo.opCodes[selectorArr[i]]);

			if(optionParamStore.ContainsKey(currOpKey))
			{
				optionParamStore[currOpKey] = codeSelection;
			}
			else
			{
				optionParamStore.Add(currOpKey,codeSelection);
			}
		}
	}

	private string buildParamString()
	{
		string retStr = "";

		extractUserChoices();


		if(optionParamStore == null)
		{
			retStr = defaultParamStrForConfig;
		}
		else
		{
			if(paramKeyOrder == null)
			{
				retStr = defaultParamStrForConfig;
			}
			else
			{
				int paramID = 0;
				for(int i=0; i<paramKeyOrder.Count; i++)
				{
					if(optionParamStore.ContainsKey(paramKeyOrder[i]))
					{
						if(paramID != 0) { retStr += "-"; }
						retStr += optionParamStore[paramKeyOrder[i]];
						optionParamStore.Remove(paramKeyOrder[i]);
						paramID++;
					}
				}

				List<string> remainingKeys = new List<string>(optionParamStore.Keys);
				for(int i=0; i<remainingKeys.Count; i++)
				{
					if(paramID != 0) { retStr += "-"; }
					retStr += optionParamStore[remainingKeys[i]];
					optionParamStore.Remove(remainingKeys[i]);
					paramID++;
				}
			}
		}

		return retStr;
	}




	

	/*public void forceCloseWindow()
	{
		notifyAllListeners("TeacherFeaturesWindow","Close",null);
		Destroy(transform.gameObject);
	}*/

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}

	Transform sfxPrefab = null;
	
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

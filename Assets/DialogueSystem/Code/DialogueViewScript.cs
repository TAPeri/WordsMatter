/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class DialogueViewScript : ILearnRWUIElement, CustomActionListener, IActionNotifier
{
	int person1NpcID;
	//string person1Name;

	bool continueIsVisible;
	bool exitIsVisible;
	bool playIsVisible;

	GameObject tmpObjToDestroy;
	MonoBehaviour tmpScriptToDestroy;


	void OnGUI()
	{
		if( ! hasInitGUIStyles)
		{
			this.prepGUIStyles();
			hasInitGUIStyles = true;
		}
		else
		{

			//GUI.Label(person1NameGUIBounds,person1Name,nameLabelGUIStyle);

			//GUI.Label(uiBounds["Person1Name"],textContent["Person1Name"],availableGUIStyles["FieldContent"]);

			if(continueIsVisible)
			{
				GUI.color = Color.clear;
				if(GUI.Button(uiBounds["ContinueBtnGuide"],textContent["ContinueBtnGuide"],availableGUIStyles["Button"]))
				{
					hideContinueButton();
					notifyAllListeners("DialogueViewScript","ContinuePressed",null);
				}
				GUI.color = Color.white;
			}

			if(exitIsVisible)
			{
				GUI.color = Color.clear;
				if(GUI.Button(uiBounds["ExitDialogBtn"],textContent["ExitDialogBtn"],availableGUIStyles["Button"]))
				{
					toggleExitButton(false);
					notifyAllListeners("DialogueViewScript","ExitPressed",null);
				}
			}

			if(playIsVisible)
			{
				GUI.color = Color.clear;
				if(GUI.Button(uiBounds["PlayButton"],textContent["PlayButton"],availableGUIStyles["Button"]))
				{
					toggleExitButton(false);
					notifyAllListeners("DialogueViewScript","PlayPressed",null);
				}
			}

		}
	}



	public void init(int para_person1NpcID, string para_person1Name)
	{
		person1NpcID = para_person1NpcID;
		//person1Name = para_person1Name;
		hideContinueButton();

		//transform.FindChild("DialogBarBackground").renderer.sortingOrder = 138;

		Transform dummyPortrait = transform.FindChild("Person1");
		PortraitHelper.replaceEntireDummyPortrait(dummyPortrait.gameObject,para_person1NpcID,0,para_person1Name,0.04f);

		/*Transform reqPerson1Prefab = Resources.Load<Transform>("Prefabs/Ghostbook/UnlockedProfilePics/UnlockedPortrait_"+para_person1NpcID);
		Transform dummyPortrait = transform.FindChild("Person1");
		GameObject nwPerson1Portrait = WorldSpawnHelper.initObjWithinWorldBounds(reqPerson1Prefab,"Person1",dummyPortrait.renderer.bounds,new bool[] {false,true,false});
		nwPerson1Portrait.renderer.sortingOrder = 140;
		nwPerson1Portrait.transform.parent = transform;
		nwPerson1Portrait.transform.localScale = new Vector3(0.9f,0.9f,1f);
		Destroy(dummyPortrait.gameObject);


		GameObject namePlaque1 = nwPerson1Portrait.transform.FindChild("Name").gameObject;
		namePlaque1.name = "Person1Name";

		Transform genericWordBoxPrefab = Resources.Load<Transform>("Prefabs/GenericWordBox");
		GameObject nameWordBox = WordBuilderHelper.buildWordBox(99,para_person1Name,
		                               CommonUnityUtils.get2DBounds(namePlaque1.renderer.bounds),
		                               namePlaque1.transform.position.z,upAxisArr,genericWordBoxPrefab);

		Destroy(nameWordBox.transform.FindChild("Board").gameObject);
		nameWordBox.transform.FindChild("Text").renderer.sortingOrder = 1500;

		nameWordBox.transform.parent = nwPerson1Portrait.transform;
		Destroy(namePlaque1);*/



		//Transform dummyPortrait2 = transform.FindChild("Person2");
		//GameObject mainAv = GameObject.Find("MainAvDiagClone");
		//GameObject diagPlayerAv = ((Transform) Instantiate(mainAv.transform,new Vector3(dummyPortrait2.position.x,dummyPortrait2.position.y,dummyPortrait2.position.z),Quaternion.identity)).gameObject;
		//diagPlayerAv.name = "MainAvDiagClone2";
		//diagPlayerAv.transform.localScale = new Vector3(2,2,2);
		//diagPlayerAv.transform.parent = transform;
		//CommonUnityUtils.setSortingOrderOfEntireObject(diagPlayerAv,150);

		//Animator aniScript = diagPlayerAv.GetComponent<Animator>();
		//aniScript.speed = 1;
		//aniScript.Play("Idle");
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "EnterScene")
		{
			// Init and register items.
			/*string[] elementNames = new string[]{"Person1Name"};
			string[] elementContent = new string[]{person1Name};
			bool[] destroyGuideArr = new bool[]{true};
			int[] textElementTypeArr = new int[]{0};
			prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,nwPerson1Portrait.name);*/
			

			
			string[] elementNames = new string[]{"ContinueBtnGuide","ExitDialogBtn","PlayButton"};
			string[] elementContent = new string[]{"CONTINUE","EXIT","PLAY"};
			bool[] destroyGuideArr = new bool[]{false,false,false};
			int[] textElementTypeArr = new int[]{0,0,0};
			prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);
		}
		else if(para_eventID == "ExitScene")
		{
			Destroy(this.gameObject);
		}
		else if(para_eventID == "BubbleCreated")
		{
			tmpObjToDestroy = (GameObject) para_eventData;
		}

		notifyAllListeners(para_sourceID,para_eventID,para_eventData);
	}

	public void enterScene()
	{

		this.respondToEvent("DialogueViewScript.enterScene","EnterScene",null);
		/*Transform bkground = transform.FindChild("DialogBarBackground");

		triggerSoundAtCamera("DialogBarSlideOpen");

		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3] { transform.position.x, transform.position.y + bkground.renderer.bounds.size.y, transform.position.z }, 0.4f, true }));
		batchLists.Add(batch1);
		aniMang.registerListener("DialogueViewScript",this);
		aniMang.init("EnterScene",batchLists);*/
	}

	public void exitScene()
	{
		//Transform bkground = transform.FindChild("DialogBarBackground");

		triggerSoundAtCamera("DialogBarSlideClose");

		if(tmpScriptToDestroy is NarrativeDiagScript) { Destroy(tmpScriptToDestroy); }

		notifyAllListeners("DialogueViewScript","ExitScene",null);

		/*CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3] { transform.position.x, transform.position.y - bkground.renderer.bounds.size.y, transform.position.z }, 0.4f, true }));
		batchLists.Add(batch1);
		aniMang.registerListener("DialogueViewScript",this);
		aniMang.init("ExitScene",batchLists);*/
	}

	public void showContinueButton()
	{
		continueIsVisible = true;
		/*Transform continueBtn = transform.FindChild("ContinueBtn");
		if(continueBtn != null)
		{
			continueBtn.renderer.enabled = true;
			continueBtn.GetComponent<Animator>().enabled = true;
		}*/
		Transform continueBtnGuide = transform.FindChild("ContinueBtnGuide");
		if(continueBtnGuide != null)
		{
			continueBtnGuide.renderer.enabled = true;
		}
	}

	public void hideContinueButton()
	{
		continueIsVisible = false;
		/*Transform continueBtn = transform.FindChild("ContinueBtn");
		if(continueBtn != null)
		{
			continueBtn.renderer.enabled = false;
			continueBtn.GetComponent<Animator>().enabled = false;
		}*/
		Transform continueBtnGuide = transform.FindChild("ContinueBtnGuide");
		if(continueBtnGuide != null)
		{
			continueBtnGuide.renderer.enabled = false;
		}
	}

	public void toggleExitButton(bool para_state)
	{
		exitIsVisible = para_state;
		Transform exitBtn = transform.FindChild("ExitDialogBtn");
		
		if(exitBtn != null)
		{
			List<SpriteRenderer> recSRends = CommonUnityUtils.getSpriteRendsOfChildrenRecursively(exitBtn.gameObject);
			if(recSRends != null)
			{
				for(int i=0; i<recSRends.Count; i++)
				{
					SpriteRenderer tmpSRend = recSRends[i];
					if(tmpSRend != null)
					{
						tmpSRend.enabled = para_state;
					}
				}
			}
		}

		togglePlayButton(para_state);
	}

	public void togglePlayButton(bool para_state)
	{
		playIsVisible = para_state;
		Transform exitBtn = transform.FindChild("PlayButton");

		if(exitBtn != null)
		{
			List<SpriteRenderer> recSRends = CommonUnityUtils.getSpriteRendsOfChildrenRecursively(exitBtn.gameObject);
			if(recSRends != null)
			{
				for(int i=0; i<recSRends.Count; i++)
				{
					SpriteRenderer tmpSRend = recSRends[i];
					if(tmpSRend != null)
					{
						tmpSRend.enabled = para_state;
					}
				}
			}
		}
	}

	public int getPerson1ID()
	{
		return person1NpcID;
	}



	public void executeCommand(DialogueViewCommand para_command)
	{
		// Destroy current bubble.
		DestroyImmediate(tmpObjToDestroy);
		if(tmpScriptToDestroy != null) { DestroyImmediate(tmpScriptToDestroy); }

		// Create next bubble.
		DialogueViewType diagType = para_command.getDiagType();

		if(para_command != null)
		{
			toggleExitButton(para_command.needsQuitBtn());
		}

		if(diagType == DialogueViewType.NARRATIVE_VIEW)
		{

			Debug.Log("NARRATIVE COMMAND");
			NarrativeDiagCommand castCommand = (NarrativeDiagCommand) para_command;

			NarrativeDiagScript nds = transform.gameObject.AddComponent<NarrativeDiagScript>();
			tmpScriptToDestroy = nds;
			nds.registerListener("MainDiagController",this);
			nds.init(castCommand);
		}
		else if((diagType == DialogueViewType.INVENTORY_TRANSFER_1_TO_2)
			||(diagType == DialogueViewType.INVENTORY_TRANSFER_2_TO_1))
		{
			InventoryDiagCommand castCommand = (InventoryDiagCommand) para_command;

			InventoryDiagScript ids = transform.gameObject.AddComponent<InventoryDiagScript>();
			tmpScriptToDestroy = ids;
			ids.registerListener("MainDiagController",this);
			ids.init(castCommand);
		}
		else if((diagType == DialogueViewType.WALK_TO_ACTIVITY)
			||(diagType == DialogueViewType.ENTER_ACTIVITY)
			||(diagType == DialogueViewType.SOMEONE_SENT_YOU))
		{

			TmpStruct ts = null;
			if(diagType == DialogueViewType.WALK_TO_ACTIVITY)
			{
				WalkToActivityDiagCommand castCommand = (WalkToActivityDiagCommand) para_command;

				Transform acOwnerPrefab = Resources.Load<Transform>("Prefabs/Ghostbook/SmallHeads/SmallHeads_"+castCommand.getActivityMasterOwnerID());
				Transform acIconPrefab = Resources.Load<Transform>("Prefabs/Ghostbook/ActivitySymbols/SmallSymbols_"+castCommand.getActivityKey());
				ts =  prepSimpleElementSequenceObjects("WalkToActivityBubbleLeft", new string[] {"footsteps","ActivityOwnerSmallHead","DestActivity"}, new Dictionary<string, Transform>() {{"ActivityOwnerSmallHead",acOwnerPrefab},{"DestActivity",acIconPrefab}});
			}
			else if(diagType == DialogueViewType.ENTER_ACTIVITY)
			{
				EnterActivityDiagCommand castCommand = (EnterActivityDiagCommand) para_command;
				
				Transform acIconPrefab = Resources.Load<Transform>("Prefabs/Ghostbook/ActivitySymbols/SmallSymbols_"+castCommand.getActivityKey());
				ts = prepSimpleElementSequenceObjects("EnterActivityBubbleLeft", new string[] {"DestActivity","ExclamationMark"}, new Dictionary<string, Transform>() {{"DestActivity",acIconPrefab}});
			}
			else //if(diagType == DialogueViewType.SOMEONE_SENT_YOU)
			{
				SomeoneSentYouDiagCommand castCommand = (SomeoneSentYouDiagCommand) para_command;

				Transform senderPrefab = Resources.Load<Transform>("Prefabs/Ghostbook/SmallHeads/SmallHeads_"+castCommand.getSenderID());
				Transform acIconPrefab = Resources.Load<Transform>("Prefabs/Ghostbook/ActivitySymbols/SmallSymbols_"+castCommand.getActivityKey());
				ts = prepSimpleElementSequenceObjects("SomeoneSentYouBubbleLeft",new string[] {"SenderSmallHead","plus","InventoryIcon","QuestionMark"},new Dictionary<string, Transform>() {{"SenderSmallHead",senderPrefab},{"InventoryIcon",acIconPrefab}});
			}



			DiagElementSequenceScript dess = ts.currentBubble.AddComponent<DiagElementSequenceScript>();
			dess.registerListener("MainDiagController",this);
			dess.init(ts.currentBubble,ts.sequenceElements);
		
			ts.currentBubble.transform.parent = transform;
			ts.currentBubble.SetActive(true);
		}
		else if(diagType == DialogueViewType.GENERIC_ICON_VIEW)
		{
			GenericIconDiagCommand castCommand = (GenericIconDiagCommand) para_command;

			GenericIconDiagScript gds = transform.gameObject.AddComponent<GenericIconDiagScript>();
			tmpScriptToDestroy = gds;
			gds.registerListener("MainDiagController",this);
			tmpObjToDestroy = gds.init(castCommand);
			if(tmpObjToDestroy != null) { tmpObjToDestroy.transform.parent = transform; }
		}
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

	private TmpStruct prepSimpleElementSequenceObjects(string para_bubbleGuideName, string[] para_sequenceItemNames, Dictionary<string,Transform> para_prefabsForReplacement)
	{
		Transform bubbleGuide = transform.FindChild(para_bubbleGuideName);
		GameObject currentBubble = ((Transform) Instantiate(bubbleGuide,bubbleGuide.position,bubbleGuide.rotation)).gameObject;
		currentBubble.name = "CurrentBubble";
		currentBubble.transform.parent = transform;


		List<GameObject> createdSequenceObjs = new List<GameObject>();

		if(para_sequenceItemNames != null)
		{
			for(int i=0; i<para_sequenceItemNames.Length; i++)
			{
				string seqItemName = para_sequenceItemNames[i];

				Transform guideObj = currentBubble.transform.Find(seqItemName);


				if(para_prefabsForReplacement.ContainsKey(seqItemName))
				{
					Transform replacementPrefab = para_prefabsForReplacement[seqItemName];
					GameObject nwObj = WorldSpawnHelper.initObjWithinWorldBounds(replacementPrefab,"Chosen_"+seqItemName,guideObj.renderer.bounds,upAxisArr);
					nwObj.renderer.sortingOrder = 150;
					nwObj.transform.parent = currentBubble.transform;
					nwObj.SetActive(false);
					createdSequenceObjs.Add(nwObj);
					Destroy(guideObj.gameObject);
				}
				else
				{
					guideObj.gameObject.SetActive(false);
					createdSequenceObjs.Add(guideObj.gameObject);
				}
			}
		}

		TmpStruct retData = new TmpStruct(currentBubble,createdSequenceObjs);
		return retData;
	}
	

	private void triggerSoundAtCamera(string para_soundFileName)
	{
		GameObject camGObj = Camera.main.gameObject;

		Transform  sfxPrefab = Resources.Load<Transform>("Prefabs/SFxBox");
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = 1f;
		audS.Play();
	}


	class TmpStruct
	{
		public GameObject currentBubble;
		public List<GameObject> sequenceElements;

		public TmpStruct(GameObject para_currentBubble, List<GameObject> para_sequenceElements)
		{
			currentBubble = para_currentBubble;
			sequenceElements = para_sequenceElements;
		}
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

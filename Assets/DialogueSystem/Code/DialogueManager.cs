/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour, CustomActionListener, IActionNotifier
{

	AbsDiagController diagController;

	Transform diagBarInstance;
	DialogueViewScript diagViewScriptInstance;

	int person1NpcID;
	string person1Name;

	Dictionary<string,List<System.Object>> paramsForBranches;


	string latestSequenceName = "";

	DiagFlowCommand queuedFlowCommand = null;
	bool checkForQueuedFlowCommandFlag = false;

	bool isAbruptExit = false;
	bool playExit = false;

	// NOTE:
	// Method for each possible conversation type.
	// Each method should then create a sequence of commands which make up the dialogue sequence.
	// These commands are iterated over and/or chosen by this class, the DialogueManager (which acts as a Flow Controller),
	// but are executed by the DialogueViewScript, which acts as the builder for constructing speech bubbles based on commands.


	void Update()
	{
		if(checkForQueuedFlowCommandFlag)
		{
			handleFlowCommand(queuedFlowCommand);
			checkForQueuedFlowCommandFlag = false;
		}
	}



	private void initDialogViewBoard()
	{
		Transform diagBar = Resources.Load<Transform>("Prefabs/Dialogue/DialogBar");
		//float totWorldHeightForDiagBack = diagBar.FindChild("DialogBarBackground").gameObject.renderer.bounds.size.y;

		Rect cam2DWorldBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,true);

		Vector3 outOfScreenBarSpawnPt = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y- (cam2DWorldBounds.height/2f),Camera.main.transform.position.z + 2f);
		//Vector3 outOfScreenBarSpawnPt = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y - (cam2DWorldBounds.height/2f) - (totWorldHeightForDiagBack/2f),Camera.main.transform.position.z + 2f);
		diagBarInstance = (Transform) Instantiate(diagBar,outOfScreenBarSpawnPt,diagBar.rotation);
		diagBarInstance.name = "DiagBar";


		DialogueViewScript dvs = diagBarInstance.gameObject.AddComponent<DialogueViewScript>();
		dvs.registerListener("DialogueManager",this);
		dvs.init(person1NpcID,person1Name);
		//dvs.enterScene();
		diagViewScriptInstance = dvs;

		//TextAudioManager taMang = 
		TextAudioManager tam = diagBarInstance.gameObject.AddComponent<TextAudioManager>();
		tam.init();
		Debug.Log("Added Text Audio Manager");
	}

	public void setBasicParams(int para_person1NpcID, string para_person1Name, Dictionary<string,List<System.Object>> para_paramsForBranches)
	{
		person1NpcID = para_person1NpcID;
		person1Name = para_person1Name;
		paramsForBranches = para_paramsForBranches;

		if(diagBarInstance == null)
		{
			initDialogViewBoard();
		}
	}


	public void narrativeDialogue(int para_person1NpcID, List<string> para_narrativeParts)
	{
		person1NpcID = para_person1NpcID;
		person1Name = "";
		paramsForBranches = null;
		initDialogViewBoard();

		diagController = new FirstMeetingDiagController(person1NpcID,para_narrativeParts);
		diagController.prepController();

		diagViewScriptInstance.enterScene();
		latestSequenceName = "EnterMyOwnActivity";

	}



	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{

		UnityEngine.Debug.Log("Dialogue Manager: "+para_sourceID+" "+para_eventID);
		if(para_eventID == "EnterScene")
		{
			runNextCommand();
		}
		else if(para_eventID == "ExitScene")
		{
			DialogReturnData retData = new DialogReturnData(person1NpcID,person1Name,latestSequenceName);

			if(isAbruptExit)
			{

				notifyAllListeners("DialogueManager","DialogueAbruptExit",null);
			}
			else if(playExit)
			{

				notifyAllListeners("DialogueManager","PlayOwn",null);


			}else{


				notifyAllListeners("DialogueManager","DialogueEnded",retData);
			}

			Destroy(diagBarInstance.gameObject);
			Destroy(diagViewScriptInstance);
			Destroy(this);
		}
		else if(para_eventID == "ExitPressed")
		{
			isAbruptExit = true;
			diagViewScriptInstance.exitScene();
		}else if(para_eventID == "PlayPressed")
		{
			playExit = true;
			diagViewScriptInstance.exitScene();

		}else
		{
			DiagFlowCommand flowCommand = diagController.reactToControlOutcome(para_sourceID,para_eventID,para_eventData);
			queuedFlowCommand = flowCommand;
			checkForQueuedFlowCommandFlag = true;
			//handleFlowCommand(flowCommand);
		}
	}

	private void handleFlowCommand(DiagFlowCommand para_flowCommand)
	{
		checkForQueuedFlowCommandFlag = false;

		DiagFlowCommand flowCommand = para_flowCommand;

		if(flowCommand != null)
		{
			if(flowCommand is DFCShowContinue)
			{
				diagViewScriptInstance.showContinueButton();
			}
			else if(flowCommand is DFCMoveToNextCommand)
			{
				diagController.dequeueCommand();
				runNextCommand();
			}
			else if(flowCommand is DFCBranchToNewConv)
			{
				DFCBranchToNewConv castFlowCommand = (DFCBranchToNewConv) flowCommand;
				string nwConvSequenceName = castFlowCommand.getNwConvSequenceName();
				UnityEngine.Debug.Log("Do: "+nwConvSequenceName);

				List<System.Object> branchingParms = paramsForBranches[nwConvSequenceName];

				if(nwConvSequenceName == "CharacterIntroNarrative")
				{
					latestSequenceName = "CharacterIntroNarrative";
					trigger_CharacterIntroNarrativeDiag((List<string>) branchingParms[0]);
					runNextCommand();
				}
				else if(nwConvSequenceName == "HandOverInventoryItem")
				{
					latestSequenceName = "HandOverInventoryItem";
					trigger_HandOverInventoryItemDiag((int) branchingParms[0], (ApplicationID) branchingParms[1]);
					runNextCommand();
				}
				else if(nwConvSequenceName == "SendToOtherActivity")
				{
					/*if(((int) branchingParms[1]) == person1NpcID)
					{
						// THIS OVERRIDES the "SendToOtherActivity" branch command because
						// this "if" has detected that the quest giver and the destination activity owner are the same person.
						latestSequenceName = "EnterMyOwnActivity";
						trigger_EnterMyOwnActivityDiag((ApplicationID) branchingParms[0]);
						runNextCommand();
					}
					else
					{*/
						latestSequenceName = "SendToOtherActivity";
						trigger_SendToOtherActivityDiag((ApplicationID) branchingParms[0],(int) branchingParms[1]);
						runNextCommand();
				//	}
				}
				else if(nwConvSequenceName == "EnterMyOwnActivity")
				{
					latestSequenceName = "EnterMyOwnActivity";
					trigger_EnterMyOwnActivityDiag((ApplicationID) branchingParms[0]);
					runNextCommand();
				}
			}
		}
	}

	private void runNextCommand()
	{
		DialogueViewCommand nxtCommand = diagController.getCurrentCommand();
		if(nxtCommand != null)
		{
			diagViewScriptInstance.executeCommand(nxtCommand);
		}
		else
		{
			Debug.Log("REMOVED AUTOMATIC EXIT");
			//diagViewScriptInstance.exitScene();
		}
	}

	// Exposed to the outside.
	public void trigger_FirstMeetingDiag(List<string> para_narrativeParts)
	{
		diagController = new FirstMeetingDiagController(person1NpcID,para_narrativeParts);
		diagController.prepController();
		latestSequenceName = "EnterMyOwnActivity";
	}
	
	public void trigger_SendToOtherActivityDiag(ApplicationID para_destAppID, int para_activityMasterOwnerID)
	{
		diagController = new SendToOtherActivityDiagController(para_destAppID,para_activityMasterOwnerID);
		diagController.prepController();
	}
	
	public void trigger_RestateActivitySendOffDiag(ApplicationID para_destAppID, int para_activityMasterOwnerID)
	{
		diagController = new RestateActivitySendOffDiagController(para_destAppID,para_activityMasterOwnerID);
		diagController.prepController();
	}
	
	public void trigger_HandOverInventoryItemDiag(int para_senderNpcID, ApplicationID para_destAppID)
	{
		diagController = new HandOverInventoryItemDiagController(para_senderNpcID,para_destAppID);
		diagController.prepController();
		latestSequenceName = "EnterMyOwnActivity";
	}
	
	public void trigger_CharacterIntroNarrativeDiag(List<string> para_narrativeParts)
	{
		diagController = new CharacterIntroNarrativeDiagController(person1NpcID,para_narrativeParts);
		diagController.prepController();
	}
	
	public void trigger_GreetingWithRelevantInvItem(ApplicationID para_destAppID)
	{
		diagController = new GreetingWithRelevantInvItemDiagController(para_destAppID);
		diagController.prepController();
	}
	
	public void trigger_GreetingWhileNoCurrentQuest(ApplicationID para_ownerAppID)
	{
		diagController = new GreetingWhileNoCurrentQuestDiagController(para_ownerAppID);
		diagController.prepController();
	}
	
	public void trigger_GreetingSecCharNoCurrentQuest()
	{
		diagController = new GreetingSecondaryCharacterNoCurrentQuestDiagController();
		diagController.prepController();
	}
	
	public void trigger_EnterMyOwnActivityDiag(ApplicationID para_activityKey)
	{
		diagController = new EnterMyOwnActivityDiagController(para_activityKey);
		diagController.prepController();
		latestSequenceName = "EnterMyOwnActivity";
	}
	



	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

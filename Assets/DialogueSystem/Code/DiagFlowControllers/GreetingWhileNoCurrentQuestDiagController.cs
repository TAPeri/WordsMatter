/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class GreetingWhileNoCurrentQuestDiagController : AbsDiagController
{

	//ApplicationID ownerActivityID;

	public GreetingWhileNoCurrentQuestDiagController(ApplicationID para_ownerActivityID)
	{
		//ownerActivityID = para_ownerActivityID;
	}

	public override void prepController()
	{
		Transform infoIcon = Resources.Load<Transform>("Prefabs/Dialogue/InfoIcon");
		Transform errandHelpIcon = Resources.Load<Transform>("Prefabs/Dialogue/ErrandHelpIcon");
		Transform activityZonePrefab = Resources.Load<Transform>("Prefabs/Dialogue/OwnerActivityIcon");//"Prefabs/Ghostbook/ActivitySymbols/SmallSymbols_"+ownerActivityID);
		
		commandQueue = new List<DialogueViewCommand>();
		commandQueue.Add(new GenericIconDiagCommand("GreetingWhileNoCurrentQuest",1,new Transform[]{infoIcon,errandHelpIcon,activityZonePrefab},new bool[]{true,true,true},false,false,true));
	}

	public override DiagFlowCommand reactToControlOutcome(string para_sourceID, string para_eventID, System.Object para_eventData)
	{

		UnityEngine.Debug.Log("Pressed: "+para_eventID);
		DiagFlowCommand retData = base.checkForBasicReaction(para_sourceID,para_eventID,para_eventData);
		if(retData == null)
		{
			if(para_eventID == "ButtonPressed-0")
			{
				retData = new DFCBranchToNewConv("CharacterIntroNarrative");
			}
			else if(para_eventID == "ButtonPressed-1")
			{
				retData = new DFCBranchToNewConv("SendToOtherActivity");
			}
			else if(para_eventID == "ButtonPressed-2")
			{
				retData = new DFCBranchToNewConv("EnterMyOwnActivity");
			}
		}
		return retData;
	}
}
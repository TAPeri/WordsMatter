/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class GreetingSecondaryCharacterNoCurrentQuestDiagController : AbsDiagController
{
	public override void prepController()
	{
		Transform errandHelpIcon = Resources.Load<Transform>("Prefabs/Dialogue/ErrandHelpIcon");
		
		commandQueue = new List<DialogueViewCommand>();
		commandQueue.Add(new GenericIconDiagCommand("GreetingWhileNoCurrentQuest",1,new Transform[]{errandHelpIcon},new bool[]{true},false,false,true));
	}
	
	public override DiagFlowCommand reactToControlOutcome(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		DiagFlowCommand retData = base.checkForBasicReaction(para_sourceID,para_eventID,para_eventData);
		if(retData == null)
		{
			if(para_eventID == "ButtonPressed-0")
			{
				retData = new DFCBranchToNewConv("SendToOtherActivity");
			}
		}
		return retData;
	}
}

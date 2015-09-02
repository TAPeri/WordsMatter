/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class GreetingWithRelevantInvItemDiagController : AbsDiagController
{
		
	ApplicationID invItemTypeID;

	public GreetingWithRelevantInvItemDiagController(ApplicationID para_destActivityID)
	{
		invItemTypeID = para_destActivityID;
	}

	public override void prepController()
	{
		Transform infoIcon = Resources.Load<Transform>("Prefabs/Dialogue/InfoIcon");
		Transform activityObjectPrefab = Resources.Load<Transform>("Prefabs/Ghostbook/ActivityItems/ActivityItem_"+invItemTypeID);

		commandQueue = new List<DialogueViewCommand>();
		commandQueue.Add(new GenericIconDiagCommand("GreetingWithRelevantInvItem",1,new Transform[]{infoIcon,activityObjectPrefab},new bool[]{true,true},false,false,true));
	}

	public override DiagFlowCommand reactToControlOutcome(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		DiagFlowCommand retData = base.checkForBasicReaction(para_sourceID,para_eventID,para_eventData);
		if(retData == null)
		{
			if(para_eventID == "ButtonPressed-0")
			{
				retData = new DFCBranchToNewConv("CharacterIntroNarrative");
			}
			else if(para_eventID == "ButtonPressed-1")
			{
				retData = new DFCBranchToNewConv("HandOverInventoryItem");
			}
		}
		return retData;
	}
}
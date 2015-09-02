/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class RestateActivitySendOffDiagController : AbsDiagController
{
	ApplicationID destActivityID;
	int activityMasterOwnerID;
	
	
	public RestateActivitySendOffDiagController(ApplicationID para_destActivityID, int para_activityMasterOwnerID)
	{
		destActivityID = para_destActivityID;
		activityMasterOwnerID = para_activityMasterOwnerID;
		commandQueue = null;
	}

	public override void prepController()
	{
		//Transform exclamationMarkPrefab = Resources.Load<Transform>("Prefabs/Dialogue/ExclamationMark");
		//Transform activityObjectPrefab = Resources.Load<Transform>("Prefabs/Ghostbook/ActivityItems/ActivityItem_"+destActivityID);
		
		commandQueue = new List<DialogueViewCommand>();
		commandQueue.Add(new WalkToActivityDiagCommand("InstructPlayerToGoToActivity",activityMasterOwnerID,destActivityID,false,false));
	}
	
	public override DiagFlowCommand reactToControlOutcome(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		// Nothing extra to add.
		DiagFlowCommand retData = base.checkForBasicReaction(para_sourceID,para_eventID,para_eventData);
		if(retData != null)
		{
			
		}
		return retData;
	}	
}

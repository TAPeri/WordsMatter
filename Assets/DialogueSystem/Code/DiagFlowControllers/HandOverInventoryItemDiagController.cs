/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class HandOverInventoryItemDiagController : AbsDiagController
{
	int senderNpcID;
	ApplicationID destActivityID;
		
	public HandOverInventoryItemDiagController(int para_senderNpcID, ApplicationID para_destActivityID)
	{
		senderNpcID = para_senderNpcID;
		destActivityID = para_destActivityID;
	}

	public override void prepController()
	{
		Transform activityObjectPrefab = Resources.Load<Transform>("Prefabs/Ghostbook/ActivityItems/ActivityItem_"+destActivityID);

		commandQueue = new List<DialogueViewCommand>();
		commandQueue.Add(new InventoryDiagCommand("InvHandOver",activityObjectPrefab,false,false,true));
		commandQueue.Add(new SomeoneSentYouDiagCommand("SomeoneSentYou",senderNpcID,destActivityID,false,true));
		commandQueue.Add(new EnterActivityDiagCommand("EnterActivity",destActivityID,false,false));
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

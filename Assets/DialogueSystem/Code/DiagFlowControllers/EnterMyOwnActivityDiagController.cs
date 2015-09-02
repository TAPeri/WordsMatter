/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class EnterMyOwnActivityDiagController : AbsDiagController
{

	ApplicationID activityKey;

	public EnterMyOwnActivityDiagController(ApplicationID para_activityKey)
	{
		activityKey = para_activityKey;
	}

	public override void prepController()
	{		
		commandQueue = new List<DialogueViewCommand>();
		commandQueue.Add(new EnterActivityDiagCommand("EnterActivity",activityKey,false,false));
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

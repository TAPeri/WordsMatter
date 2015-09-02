/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class CharacterIntroNarrativeDiagController : AbsDiagController
{
	int person1NpcID;
	List<string> narrativeParts;

	public CharacterIntroNarrativeDiagController(int para_person1NpcID, List<string> para_narrativeParts)
	{
		person1NpcID = para_person1NpcID;
		narrativeParts = para_narrativeParts;
	}

	public override void prepController()
	{
		commandQueue = new List<DialogueViewCommand>();
		for(int i=0; i<narrativeParts.Count; i++)
		{
			bool validStr = false;

			string tmpStr = narrativeParts[i];
			if(tmpStr != null)
			{
				tmpStr = tmpStr.Trim();
				if(tmpStr != "")
				{
					validStr = true;
				}
			}
			if(validStr)
			{
				commandQueue.Add(new NarrativeDiagCommand("NarrativePart-"+i,tmpStr,"Char_"+person1NpcID+"*"+"Bio_"+i,true,true));
			}
		}
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

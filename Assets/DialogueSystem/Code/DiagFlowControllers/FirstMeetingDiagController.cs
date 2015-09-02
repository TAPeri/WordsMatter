/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class FirstMeetingDiagController : AbsDiagController
{
	int person1NpcID;
	List<string> narrativeParts;


	public FirstMeetingDiagController(int para_person1NpcID,
	                                  				  List<string> para_narrativeParts)
	{
		person1NpcID = para_person1NpcID;
		narrativeParts = para_narrativeParts;
	}
	
	public override void prepController()
	{
		//Transform exclamationMarkPrefab = Resources.Load<Transform>("Prefabs/Dialogue/ExclamationMark");

		commandQueue = new List<DialogueViewCommand>();
		//commandQueue.Add(new GenericIconDiagCommand("CharacterAlerted",0,new Transform[]{exclamationMarkPrefab},null,true,false,true));
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
		//commandQueue.Add(new EnterActivityDiagCommand("EnterActivity",destAppID,false,true));
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

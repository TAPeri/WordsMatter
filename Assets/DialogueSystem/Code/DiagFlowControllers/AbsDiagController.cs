/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public abstract class AbsDiagController
{
	protected List<DialogueViewCommand> commandQueue;

	public void dequeueCommand()
	{
		if(commandQueue != null)
		{
			if(commandQueue.Count > 0)
			{
				commandQueue.RemoveAt(0);
			}
		}
	}

	public DialogueViewCommand getCurrentCommand()
	{
		DialogueViewCommand retData = null;
		if(commandQueue != null)
		{
			if(commandQueue.Count > 0)
			{
				retData = commandQueue[0];
			}
		}
		return retData;
	}




	protected DiagFlowCommand checkForBasicReaction(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		// Nothing. No branching to other dialogue sequences are needed.
		if(para_eventID == "BubbleCreated")
		{
			DialogueViewCommand currentCommand = commandQueue[0];

			if(currentCommand.needsContinueBtn())
			{
				return (new DFCShowContinue());
			}
			else
			{
				return (new DFCMoveToNextCommand());
			}
		}
		else if(para_eventID == "ContinuePressed")
		{
			return (new DFCMoveToNextCommand());
		}
		else
		{
			return null;
		}
	}


	public abstract void prepController();
	public abstract DiagFlowCommand reactToControlOutcome(string para_sourceID, string para_eventID, System.Object para_eventData);
}

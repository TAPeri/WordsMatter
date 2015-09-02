/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class EnterActivityDiagCommand : DialogueViewCommand
{
	ApplicationID activityKey;

	public EnterActivityDiagCommand(string para_commandName,
	                                ApplicationID para_activityKey,
	                                				   bool para_needsContinueBtn,
	                                				   bool para_needsQuitBtn)
		:base(para_commandName,DialogueViewType.ENTER_ACTIVITY,para_needsContinueBtn,para_needsQuitBtn)
	{
		activityKey = para_activityKey;
	}

	public ApplicationID getActivityKey() { return activityKey; }
}

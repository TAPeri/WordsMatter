/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class WalkToActivityDiagCommand : DialogueViewCommand
{
	int activityMasterOwner_id;
	ApplicationID activityKey;

	public WalkToActivityDiagCommand(string para_commandName,
														  int para_activityMasterOwner_id,
	                                 ApplicationID para_activityKey,
	                                 					  bool para_needsContinueBtn,
	                                 					  bool para_needsQuitBtn)
		:base(para_commandName,DialogueViewType.WALK_TO_ACTIVITY,para_needsContinueBtn,para_needsQuitBtn)
	{
		activityMasterOwner_id = para_activityMasterOwner_id;
		activityKey = para_activityKey;
	}

	public int getActivityMasterOwnerID() { return activityMasterOwner_id; }
	public ApplicationID getActivityKey() { return activityKey; }
}

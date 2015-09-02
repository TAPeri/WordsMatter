/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class SomeoneSentYouDiagCommand : DialogueViewCommand
{
	int senderID;
	ApplicationID acKey;

	public SomeoneSentYouDiagCommand(string para_commandName, int para_senderID, ApplicationID para_acKey, bool para_needsContinueBtn, bool para_needsQuitBtn)
		:base(para_commandName,DialogueViewType.SOMEONE_SENT_YOU,para_needsContinueBtn,para_needsQuitBtn)
	{
		senderID = para_senderID;
		acKey = para_acKey;
	}

	public int getSenderID() { return senderID; }
	public ApplicationID getActivityKey() { return acKey; }
}

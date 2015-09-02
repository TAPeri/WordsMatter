/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class NarrativeDiagCommand : DialogueViewCommand
{
	string voiceOverSearchKey;
	string narrativeText;

	public NarrativeDiagCommand(string para_commandName,
	                            				 string para_narrativeText,
	                            				 string para_voiceOverSearchKey,
	                              				 bool para_needsContinueBtn,
	                            				 bool para_needsQuitBtn)
		:base(para_commandName,DialogueViewType.NARRATIVE_VIEW,para_needsContinueBtn,para_needsQuitBtn)
	{
		narrativeText = para_narrativeText;
		voiceOverSearchKey = para_voiceOverSearchKey;
	}

	public string getNarrativeText() { return narrativeText; }
	public string getVoiceOverSearchKey() { return voiceOverSearchKey; }
}
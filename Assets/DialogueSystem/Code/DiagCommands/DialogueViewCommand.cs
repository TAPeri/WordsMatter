/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class DialogueViewCommand
{
	protected string commandName;
	protected DialogueViewType diagType;

	protected bool needsContinueBtnFlag;
	protected bool needsQuitBtnFlag;

	// Subclasses should add attributes which will be used to construct the view in the view script.



	public DialogueViewCommand(string para_commandName,
	                           DialogueViewType para_diagType,
	                           bool para_needsContinueBtn,
	                           bool para_needsQuitBtn)
	{
		commandName = para_commandName;
		diagType = para_diagType;
		needsContinueBtnFlag = para_needsContinueBtn;
		needsQuitBtnFlag = para_needsQuitBtn;
	}

	public string getCommandName() { return commandName; }
	public DialogueViewType getDiagType() { return diagType; }
	public bool needsContinueBtn() { return needsContinueBtnFlag; }
	public bool needsQuitBtn() { return needsQuitBtnFlag; }
}

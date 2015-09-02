/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class GenericIconDiagCommand : DialogueViewCommand
{
	int speakerIndex;
	Transform[] iconPrefabs;
	bool[] pressableIconsFlagArr;

	bool performPopInSequence;


	public GenericIconDiagCommand(string para_commandName,
	                              					 int para_speakerIndex,
	                              					 Transform[] para_iconPrefabs,
	                              					  bool[] para_pressableIconsFlagArr,
	                              					  bool para_performPopInSequence,
	                              					  bool para_needsContinueBtn,
	                              					  bool para_needsQuitBtn)
		:base(para_commandName,DialogueViewType.GENERIC_ICON_VIEW,para_needsContinueBtn,para_needsQuitBtn)
	{
		speakerIndex = para_speakerIndex;
		iconPrefabs = para_iconPrefabs;
		pressableIconsFlagArr = para_pressableIconsFlagArr;
		performPopInSequence = para_performPopInSequence;
	}
	
	public int getSpeakerIndex() { return speakerIndex; }
	public Transform[] getIconPrefabs() { return iconPrefabs; }
	public bool[] getPressableIconsFlagArr() { return pressableIconsFlagArr; }
	public bool needsToPerformPopInSequence() { return performPopInSequence; }
}

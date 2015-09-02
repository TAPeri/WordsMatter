/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OptionInfo
{
	public string opKey; // Eg. M,A,S etc.
	public List<int> opCodes; // Eg. 1,2,3,4 etc. which merge with the op key to become Eg. M1,M2,M3,M4 etc.
	public string[] opReadableSelections; // Displayed on the UI.
	public string opReadableTitle; // Displayed on the UI.
	
	public OptionInfo(string para_opKey,
	                  List<int> para_opCodes,
	                  string[] para_opReadableSelections,
	                  string para_opReadableTitle)
	{
		opKey = para_opKey;
		opCodes = para_opCodes;
		opReadableSelections = para_opReadableSelections;
		opReadableTitle = para_opReadableTitle;
	}
}
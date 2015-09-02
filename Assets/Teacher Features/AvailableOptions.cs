/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class AvailableOptions
{
	public List<string> validKeys;
	public Dictionary<string,OptionInfo> validOptions;
	
	public AvailableOptions(List<string> para_validKeys,
	                        Dictionary<string,OptionInfo> para_validOptions)
	{
		validKeys = para_validKeys;
		validOptions = para_validOptions;
	}
	
	public void cleanUp()
	{
		if(validKeys != null) { validKeys.Clear(); }
		if(validOptions != null) { validOptions.Clear(); }
	}
}
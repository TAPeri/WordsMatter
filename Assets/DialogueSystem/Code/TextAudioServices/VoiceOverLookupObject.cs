/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class VoiceOverLookupObject
{
	public Dictionary<string,string> keyValueMap;

	public string getValue(string para_key)
	{
		string retData = null;
		if(keyValueMap != null)
		{
			if(keyValueMap.ContainsKey(para_key))
			{
				retData = keyValueMap[para_key];
			}
		}
		return retData;
	}
}

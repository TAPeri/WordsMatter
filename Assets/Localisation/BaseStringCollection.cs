/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BaseStringCollection : StringCollection
{
	public Dictionary<string,string> keyToStrMap;

	public BaseStringCollection()
	{
		// For Json Only.
	}

	public BaseStringCollection(Dictionary<string,string> para_map)
	{
		keyToStrMap = para_map;
	}

	public new string getString(int para_dirLevel, string[] para_splitDirPath)
	{
		string retStr = null;
		if(keyToStrMap != null)
		{
			string reqKey = para_splitDirPath[para_dirLevel];
			if(keyToStrMap.ContainsKey(reqKey))
			{
				retStr = keyToStrMap[reqKey];
			}
		}
		return retStr;
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BranchStringCollection : StringCollection
{
	public Dictionary<string,BaseStringCollection> strBaseCollections;
	//public Dictionary<string,BranchStringCollection> subDirectoryMap;

	public BranchStringCollection()
	{
		// For Json Only.
	}

	public BranchStringCollection(Dictionary<string,BaseStringCollection> para_strCollection,
								  Dictionary<string,BranchStringCollection> para_subDirectoryMap)
	{
		strBaseCollections = para_strCollection;
		//subDirectoryMap = para_subDirectoryMap;
	}

	public new string getString(int para_dirLevel, string[] para_splitDirPath)
	{
		string retStr = "STRING NOT FOUND";

		string reqKey = para_splitDirPath[para_dirLevel];
		//bool foundStr = false;

		if(strBaseCollections != null)
		{
			if(strBaseCollections.ContainsKey(reqKey))
			{
				retStr = strBaseCollections[reqKey].getString(para_dirLevel+1,para_splitDirPath);
				//foundStr = true;
			}
		}

		/*if(( ! foundStr)&&(subDirectoryMap != null))
		{
			if(subDirectoryMap.ContainsKey(reqKey))
			{
				retStr = subDirectoryMap[reqKey].getString(para_dirLevel+1,para_splitDirPath);
			}
		}*/

		return retStr;
	}
}

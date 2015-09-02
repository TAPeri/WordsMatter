/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

[System.Serializable]
public class LocalisationSet
{
	// Changed from protected to public due to JSON serialiser access.

	public string langCodeStr;
	public LanguageCode langCodeEnum;
	public string langAlphabetLower;
	public string langAlphabetUpper;
	public List<string> langKL_TabletOrderLower;
	public List<string> langKL_TabletOrderUpper;
	public List<string> langKL_AlphaOrderLower;
	public List<string> langKL_AlphaOrderUpper;
	public BranchStringCollection localisedStrings;

	// The key must be the path to the string as dictated by the localisation set.
	// The sections in the path must be separated by *.
	// See the EnglishLocalisationFile or GreekLocalisationFile for an example of the path structure.
	public string getLocalisedString(string para_key)
	{
		string retData = null;
		string[] splitPath = para_key.Split('*');
		retData = localisedStrings.getString(0,splitPath);
		return retData;
	}

	public string getAlphabetLower() { return langAlphabetLower; }
	public string getAlphabetUpper() { return langAlphabetUpper; }
	public List<string> getKeyboardLayoutTabLower() { return langKL_TabletOrderLower; }
	public List<string> getKeyboardLayoutTabUpper() { return langKL_TabletOrderUpper; }
	public List<string> getKeyboardLayoutAlphaLower() { return langKL_AlphaOrderLower; }
	public List<string> getKeyboardLayoutAlphaUpper() { return langKL_AlphaOrderUpper; }
}

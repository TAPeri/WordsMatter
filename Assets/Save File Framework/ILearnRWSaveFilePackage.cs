/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
[System.Serializable]
public class ILearnRWSaveFilePackage
{
	SaveVersioningInfo gameVersionInfo;
	string otherData;

	public ILearnRWSaveFilePackage()
	{
		// For serialisers only.
	}

	public ILearnRWSaveFilePackage(SaveVersioningInfo para_saveVersion, string para_otherData)
	{
		gameVersionInfo = para_saveVersion;
		otherData = para_otherData;
	}

	public SaveVersioningInfo getGameVersionInfo() { return gameVersionInfo; }
	public string getOtherData() { return otherData; }
}
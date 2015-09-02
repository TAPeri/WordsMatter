/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class ActivitySessionMetaData
{
	ApplicationID acID;
	string characterHelperName;
	string languageArea;
	string difficulty;

	int questGiverID;
	int langAreaID;
	int diffIndexForLangArea;

	// Float between 0 and 1.
	// Corresponds to the progress of a single particular activity instance.
	float progress;

	Mode activityLauncher;
	string launcherDetails;

	System.DateTime startTimestamp;
	System.DateTime endTimestamp;
	

	public ActivitySessionMetaData(string para_characterHelperName,
	                               ApplicationID para_acID,
	                               string para_languageArea,
	                               string para_difficulty,
	                               int para_questGiverID,
	                               int para_langAreaID,
	                               int para_diffIndexForLangArea,
	                               Mode al,
	                               string ld)
	{
		launcherDetails = ld;
		activityLauncher = al;
		acID = para_acID;
		characterHelperName = para_characterHelperName;
		languageArea = para_languageArea;
		difficulty = para_difficulty;

		questGiverID = para_questGiverID;
		langAreaID = para_langAreaID;
		diffIndexForLangArea = para_diffIndexForLangArea;

		progress = 0;
		startTimestamp = System.DateTime.Now;
		endTimestamp = System.DateTime.Now;
	}

	public void recordActivityStart()
	{
		startTimestamp = System.DateTime.Now;
	}

	public void recordActivityEnd()
	{
		endTimestamp = System.DateTime.Now;
	}

	public void setProgress(float para_progVal)
	{
		progress = para_progVal;
		if(progress < 0) { progress = 0; }
		if(progress > 1) { progress = 1; }
	}

	public ApplicationID getApplicationID() { return acID; }
	public string getCharacterHelperName() { return characterHelperName; }
	public string getLanguageArea() { return languageArea; }
	public string getDifficulty() { return difficulty; }
	public int getQuestGiverID() { return questGiverID; }
	public int getLangAreaID() { return langAreaID; }
	public int getDiffIndexForLangArea() { return diffIndexForLangArea; }
	public float getProgress() { return progress; }
	public System.DateTime getActivityStartTimestamp() { return startTimestamp; }
	public System.DateTime getActivityEndTimestamp() { return endTimestamp; }

	public string getLauncherDetails(){ return launcherDetails;}
	public Mode getLauncherApp(){return activityLauncher;}
}

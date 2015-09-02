/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
[System.Serializable]
public class SaveVersioningInfo
{
	// Date Info.
	int dayValue;
	int monthValue;
	int yearValue;
	int versionForDay;

	string otherData;

	public SaveVersioningInfo()
	{
		// For serialisers only.
	}

	public SaveVersioningInfo(int para_dayValue,
	                          			   int para_monthValue,
	                           			   int para_yearValue,
	                          			   int para_versionForDay,
	                          			   string para_otherData)
	{
		dayValue = para_dayValue;
		monthValue = para_monthValue;
		yearValue = para_yearValue;
		versionForDay = para_versionForDay;
		otherData = para_otherData;
	}

	public int getDayValue() { return dayValue; }
	public int getMonthValue() { return monthValue; }
	public int getYearValue() { return yearValue; }
	public int getVersionForDay() { return versionForDay; }
	public string getOtherData() { return otherData; }
}

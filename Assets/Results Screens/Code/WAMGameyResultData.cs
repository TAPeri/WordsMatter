/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections;

public class WAMGameyResultData : GameyResultData
{
	int numFedCorrectMonkies;
	int numMissedCorrectMonkies;
	int numFedWrongMonkies;
	int time_minuteComponent;
	int time_secondComponent;

	public WAMGameyResultData(int para_numFedCorrectMonkies,
	                          					int para_numMissedCorrectMonkies,
	                          					int para_numFedWrongMonkies,
												int para_timeMinuteComponent,
	                          					int para_timeSecondComponent)
	{
		numFedCorrectMonkies = para_numFedCorrectMonkies;
		numMissedCorrectMonkies = para_numMissedCorrectMonkies;
		numFedWrongMonkies = para_numFedWrongMonkies;
		time_minuteComponent = para_timeMinuteComponent;
		time_secondComponent = para_timeSecondComponent;
	}

	public int getNumFedCorrectMonkies() { return numFedCorrectMonkies; }
	public int getNumMissedCorrectMonkies() { return numMissedCorrectMonkies; }
	public int getNumFedWrongMonkies() { return numFedWrongMonkies; }
	public string getTimeString() { return ""+time_minuteComponent+"m "+time_secondComponent+"s"; }
}

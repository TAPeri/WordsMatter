/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class TDGameyResultData : GameyResultData
{

	int numCorrectTrains;
	int numIncorrectAttempts;
	int time_minuteComponent;
	int time_secondComponent;

	public TDGameyResultData(int para_numCorrectTrains,
	                         				int para_numIncorrectAttempts,
	                         				int para_timeMinutesComponent,
	                         				int para_timeSecondsComponent)
	{
		numCorrectTrains = para_numCorrectTrains;
		numIncorrectAttempts = para_numIncorrectAttempts;
		time_minuteComponent = para_timeMinutesComponent;
		time_secondComponent = para_timeSecondsComponent;
	}

	public int getNumCorrectTrains() { return numCorrectTrains; }
	public int getNumIncorrectAttempts() { return numIncorrectAttempts; }
	public string getTimeString() { return ""+time_minuteComponent+"m "+time_secondComponent+"s"; }	
}

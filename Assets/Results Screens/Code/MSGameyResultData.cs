/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections;

public class MSGameyResultData : GameyResultData
{
	int numCorrectAttempts;
	int numIncorrectAttempts;
	int time_minuteComponent;
	int time_secondComponent;

	public MSGameyResultData(int para_numCorrectAttempts,
	                         				 int para_numIncorrectAttempts,
	                         				 int para_timeMinuteComponent,
	                         				 int para_timeSecondComponent)
	{
		numCorrectAttempts = para_numCorrectAttempts;
		numIncorrectAttempts = para_numIncorrectAttempts;
		time_minuteComponent = para_timeMinuteComponent;
		time_secondComponent = para_timeSecondComponent;
	}

	public int getNumCorrectAttempts() { return numCorrectAttempts; }
	public int getNumIncorrectAttempts() { return numIncorrectAttempts; }
	public string getTimeString() { return ""+time_minuteComponent+"m "+time_secondComponent+"s"; }
}

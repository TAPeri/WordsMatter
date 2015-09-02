/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class SHGameyResultData : GameyResultData
{
	int numCorrectSentences;
	int numIncorrectTries;
	int finalMusPeepCount;
	int time_minuteComponent;
	int time_secondComponent;

	public SHGameyResultData(int para_numCorrectSentences,
	                         int para_numIncorrectTries,
	                         int para_finalMusPeepCount,
	                         int para_timeMinuteComponent,
	                         int para_timeSecondComponent)
	{
		numCorrectSentences = para_numCorrectSentences;
		numIncorrectTries = para_numIncorrectTries;
		finalMusPeepCount = para_finalMusPeepCount;
		time_minuteComponent = para_timeMinuteComponent;
		time_secondComponent = para_timeSecondComponent;
	}

	public int getNumCorrectSentences() { return numCorrectSentences; }
	public int getNumIncorrectTries() { return numIncorrectTries; }
	public int getFinalMusPeepCount() { return finalMusPeepCount; }
	public string getTimeString() { return ""+time_minuteComponent+"m "+time_secondComponent+"s"; }
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class BBGameyResultData : GameyResultData
{
	int numOfCompleteCorrectBridges;
	int numBrokenBridges;
	int time_minuteComponent;
	int time_secondComponent;

	public BBGameyResultData(int para_numOfCompleteCorrectBridges,
	                         				int para_numBrokenBridges,
	                         				int para_timeMinuteComponent,
	                         				int para_timeSecondComponent)
	{
		numOfCompleteCorrectBridges = para_numOfCompleteCorrectBridges;
		numBrokenBridges = para_numBrokenBridges;
		time_minuteComponent = para_timeMinuteComponent;
		time_secondComponent = para_timeSecondComponent;
	}

	public int getNumOfCompleteCorrectBridges() { return numOfCompleteCorrectBridges; }
	public int getNumBrokenBridges() { return numBrokenBridges; }
	public string getTimeString() { return ""+time_minuteComponent+"m "+time_secondComponent+"s"; }
}

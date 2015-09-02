/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections;

public class HarvestGameyResultData : GameyResultData
{
	int numCorrectHarvests;
	int numMachinesBroken;
	int time_minuteComponent;
	int time_secondComponent;

	public HarvestGameyResultData(int para_numCorrectHarvests,
	                              int para_numMachinesBroken,
	                              int para_timeMinuteComponent,
	                              int para_timeSecondComponent)
	{
		numCorrectHarvests = para_numCorrectHarvests;
		numMachinesBroken = para_numMachinesBroken;
		time_minuteComponent = para_timeMinuteComponent;
		time_secondComponent = para_timeSecondComponent;
	}

	public int getNumCorrectHarvests() { return numCorrectHarvests; }
	public int getNumMachinesBroken() { return numMachinesBroken; }
	public string getTimeString() { return ""+time_minuteComponent+"m "+time_secondComponent+"s"; }
}

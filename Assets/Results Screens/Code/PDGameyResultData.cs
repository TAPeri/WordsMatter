/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class PDGameyResultData : GameyResultData
{
	int numPackagesLostToMonkies;
	int numCorrectDeliveries;
	int numWrongDeliveries;
	int time_minuteComponent;
	int time_secondComponent;

	public PDGameyResultData(int para_numPackagesLostToMonkies,
	                         				int para_numCorrectDeliveries,
	                         				int para_numWrongDeliveries,
	                         				int para_timeMinutesComponent,	
	                         				int para_timeSecondsComponent)
	{
		numPackagesLostToMonkies = para_numPackagesLostToMonkies;
		numCorrectDeliveries = para_numCorrectDeliveries;
		numWrongDeliveries = para_numWrongDeliveries;
		time_minuteComponent = para_timeMinutesComponent;
		time_secondComponent = para_timeSecondsComponent;
	}

	public int getNumPackagesLostToMonkies() { return numPackagesLostToMonkies; }
	public int getNumCorrectDeliveries() { return numCorrectDeliveries; }
	public int getNumWrongDeliveries() { return numWrongDeliveries; }
	public string getTimeString() { return ""+time_minuteComponent+"m "+time_secondComponent+"s"; }
}

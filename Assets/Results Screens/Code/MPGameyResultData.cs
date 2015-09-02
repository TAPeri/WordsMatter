/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class MPGameyResultData : GameyResultData
{
	int numCompletePaths;
	int numMissteps;
	int numRotations;
	int time_minuteComponent;
	int time_secondComponent;

	public MPGameyResultData(int para_numCompletePaths,
	                         int para_numMissteps,
	                         int para_numRotations,
	                         int para_timeMinuteComponent,
	                         int para_timeSecondComponent)
	{
		numCompletePaths = para_numCompletePaths;
		numMissteps = para_numMissteps;
		numRotations = para_numRotations;
		time_minuteComponent = para_timeMinuteComponent;
		time_secondComponent = para_timeSecondComponent;
	}

	public int getNumCompletePaths() { return numCompletePaths; }
	public int getNumMissteps() { return numMissteps; }
	public int getRotations() { return numRotations; }
	public string getTimeString() { return ""+time_minuteComponent+"m "+time_secondComponent+"s"; }
}

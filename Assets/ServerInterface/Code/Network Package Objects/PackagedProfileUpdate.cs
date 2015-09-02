/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */[System.Serializable]

public class PackagedProfileUpdate{
	
	public int category, index, previousSeverity, newSeverity, previousWorkingIndex, newWorkingIndex;

	public PackagedProfileUpdate()
	{

	}
		
	public PackagedProfileUpdate(int category, int index, int previousSeverity,
		                           int newSeverity, int previousWorkingIndex, int newWorkingIndex) {
			this.category = category;
			this.index = index;
			this.previousSeverity = previousSeverity;
			this.newSeverity = newSeverity;
			this.previousWorkingIndex = previousWorkingIndex;
			this.newWorkingIndex = newWorkingIndex;
		}
}

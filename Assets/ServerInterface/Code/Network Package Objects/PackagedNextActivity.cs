/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */[System.Serializable]

public class PackagedNextActivity{

	public string[] activity;
	public int category;
	public int index;
	public string[] level;


	public PackagedNextActivity()
	{
		// Empty constructor required for JSON converter.
	}

	public PackagedNextActivity(string[] a, int c, int i,string[] l){

		activity = a;
		category = c;
		index = i;
		level = l;
	}

	public int getDifficulty(){
		return index;
	}

	public int getLanguageArea(){

		return category;
	}

	public ApplicationID[] getActivities(){
		ApplicationID[] output = new ApplicationID[activity.Length];

		for (int i=0;i<output.Length;i++){
			output[i] = (ApplicationID) System.Enum.Parse(typeof(ApplicationID), activity[i]);
		}
		return output;
	}

	public string[] getLevel(){
		return level;
	}



}

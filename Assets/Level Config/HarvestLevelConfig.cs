/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class HarvestLevelConfig : ILevelConfig
{
	public string harvestingWord;
	public string[] machineDescriptions;
	public int[] correctMachines;

	public int languageArea = -1;
	public int difficulty = -1;


	public HarvestLevelConfig(string para_harvestingWord,
	                          string[] para_machineDescriptions,
	                          int[] para_correctMachines,int languageArea,int difficulty)
	{
		
		this.difficulty = difficulty;
		this.languageArea = languageArea;

		UnityEngine.Debug.Log(para_harvestingWord);
		foreach(string pm in para_machineDescriptions){
			UnityEngine.Debug.Log("->"+pm);
		}
		foreach(int i in para_correctMachines)
			UnityEngine.Debug.Log("->"+i);


		harvestingWord = para_harvestingWord;
		machineDescriptions = para_machineDescriptions;
		correctMachines = para_correctMachines;
	}

	public bool machineIsCorrect(int para_machineIndex)
	{
		bool retFlag = false;
		for(int i=0; i<correctMachines.Length; i++)
		{
			if(correctMachines[i] == para_machineIndex)
			{
				retFlag = true;
				break;
			}
		}
		return retFlag;
	}

	public string getSolutionString()
	{
		string solutionStr = "Word: '"+harvestingWord+"'\t\t";
		string categoryStr = "Categories: ";
		for(int i=0; i<correctMachines.Length; i++)
		{
			categoryStr += ("'" + machineDescriptions[correctMachines[i]] + "'\t");
		}
		solutionStr += categoryStr;
		return solutionStr;
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class WAMLevelOutcome : LevelOutcome
{
	Dictionary<string,int> fedCorrectMonkies;
	Dictionary<string,int> missedCorrectMonkies;
	Dictionary<string,int> fedIncorrectMonkies;

	public WAMLevelOutcome(bool para_isPossitive)
		:base(para_isPossitive)
	{
		fedCorrectMonkies = new Dictionary<string, int>();
		missedCorrectMonkies = new Dictionary<string, int>();
		fedIncorrectMonkies = new Dictionary<string, int>();
	}

	public void addFedCorrectMonkey(string para_monkeyWord)
	{
		if(fedCorrectMonkies == null) { fedCorrectMonkies = new Dictionary<string, int>(); }

		if( ! fedCorrectMonkies.ContainsKey(para_monkeyWord))
		{
			fedCorrectMonkies.Add(para_monkeyWord,0);
		}
		fedCorrectMonkies[para_monkeyWord]++;
	}

	public void addMissedCorrectMonkey(string para_monkeyWord)
	{
		if(missedCorrectMonkies == null) { missedCorrectMonkies = new Dictionary<string, int>(); }
		
		if( ! missedCorrectMonkies.ContainsKey(para_monkeyWord))
		{
			missedCorrectMonkies.Add(para_monkeyWord,0);
		}
		missedCorrectMonkies[para_monkeyWord]++;
	}

	public void addFedIncorrectMonkey(string para_monkeyWord)
	{
		if(fedIncorrectMonkies == null) { fedIncorrectMonkies = new Dictionary<string, int>(); UnityEngine.Debug.Log("????");}
		try{
		if( ! fedIncorrectMonkies.ContainsKey(para_monkeyWord))
		{
				UnityEngine.Debug.Log("Add "+para_monkeyWord);

			fedIncorrectMonkies.Add(para_monkeyWord,0);
		}
		fedIncorrectMonkies[para_monkeyWord]++;
		}catch(System.Exception ex){
			UnityEngine.Debug.Log(para_monkeyWord);
			UnityEngine.Debug.Log(ex.Message);
		}
	}

	public Dictionary<string,int> getFedCorrectMonkeyData() { return fedCorrectMonkies; }
	public Dictionary<string,int> getMissedCorrectMonkeyData() { return missedCorrectMonkies; }
	public Dictionary<string,int> getFedIncorrectMonkeyData() { return fedIncorrectMonkies; }
}

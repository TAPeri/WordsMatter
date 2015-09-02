/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class MSLevelOutcome : LevelOutcome
{
	Dictionary<int,bool> pairsWithIncorrectAttempts;

	public MSLevelOutcome(bool para_isPositive)
		:base(para_isPositive)
	{
		pairsWithIncorrectAttempts = new Dictionary<int, bool>();
	}

	public void addIncorrectAttempt(int para_pairIndex)
	{
		if(pairsWithIncorrectAttempts == null) { pairsWithIncorrectAttempts = new Dictionary<int, bool>(); }

		if( ! pairsWithIncorrectAttempts.ContainsKey(para_pairIndex))
		{
			pairsWithIncorrectAttempts.Add(para_pairIndex,true);
		}
	}

	public bool isPairWithIncorrectAttempts(int para_pairIndex)
	{
		bool retFlag = false;
		if(pairsWithIncorrectAttempts != null)
		{
			if(pairsWithIncorrectAttempts.ContainsKey(para_pairIndex))
			{
				retFlag = pairsWithIncorrectAttempts[para_pairIndex];
			}
		}
		return retFlag;
	}
}

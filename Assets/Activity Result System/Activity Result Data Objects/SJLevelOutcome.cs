/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class SJLevelOutcome : LevelOutcome
{
	List<int> playerSplitPattern;

	public SJLevelOutcome(bool para_isPositive)
		:base(true)
	{
		playerSplitPattern = null;
	}

	public SJLevelOutcome(bool para_isPositive, List<int> para_playerSplitPattern)
		:base(false)
	{
		playerSplitPattern = para_playerSplitPattern;
	}

	public List<int> getPlayerSplitPattern() { return playerSplitPattern; }
}

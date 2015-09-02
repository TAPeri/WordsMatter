/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class SHLevelOutcome : LevelOutcome
{
	Dictionary<int,string> playerSelectedHoleItems;

	public SHLevelOutcome(bool para_isPositive, Dictionary<int,string> para_playerSelectedHoleItems)
		:base(para_isPositive)
	{
		playerSelectedHoleItems = para_playerSelectedHoleItems;
	}

	public Dictionary<int,string> getPlayerSelectedHoleItems() { return playerSelectedHoleItems; }
}

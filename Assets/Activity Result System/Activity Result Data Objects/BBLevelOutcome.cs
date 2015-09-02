/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class BBLevelOutcome : LevelOutcome
{

	List<IndexRegion> playerHighlights;
	

	public BBLevelOutcome(bool para_isPositive, List<IndexRegion> para_playerHighlights)
		:base(para_isPositive)
	{
		playerHighlights = para_playerHighlights;
	}

	public List<IndexRegion> getPlayerHighlights() { return playerHighlights; }
}

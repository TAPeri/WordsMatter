/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
// Extend this if it is necessary to record more indepth
// outcome data about a level in a particular activity.
public class LevelOutcome
{
	bool isPositive;

	public LevelOutcome(bool para_isPositive)
	{
		isPositive = para_isPositive;
	}

	public bool isPositiveOutcome()
	{
		return isPositive;
	}
}

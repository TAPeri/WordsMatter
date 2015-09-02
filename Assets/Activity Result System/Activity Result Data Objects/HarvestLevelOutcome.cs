/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class HarvestLevelOutcome : LevelOutcome
{
	List<int> machinesGivenGoodInput;
	List<int> machinesGivenBadInput;

	public HarvestLevelOutcome(bool para_isPositive,
	                           List<int> para_machinesGivenGoodInput,
	                           List<int> para_machinesGivenBadInput)
		:base(para_isPositive)
	{
		machinesGivenGoodInput = para_machinesGivenGoodInput;
		machinesGivenBadInput = para_machinesGivenBadInput;
	}

	public List<int> getMachinesGivenGoodInput() { return machinesGivenGoodInput; }
	public List<int> getMachinesGivenBadInput() { return machinesGivenBadInput; }
}
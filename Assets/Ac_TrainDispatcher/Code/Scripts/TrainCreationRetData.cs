/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using UnityEngine;

public class TrainCreationRetData
{
	public Rect trainMaxWorldBounds;
	public int numCarriagesUsed;
	public int numCarriagesInStorageBay;

	public TrainCreationRetData(Rect para_trainMaxWorldBounds, int para_numCarriagesUsed, int para_numCarriagesInStorageBay)
	{
		trainMaxWorldBounds = para_trainMaxWorldBounds;
		numCarriagesUsed = para_numCarriagesUsed;
		numCarriagesInStorageBay = para_numCarriagesInStorageBay;
	}
}

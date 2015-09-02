/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class ClothingConfigIterator
{
	ClothingConfig config;
	string[] categoryOrder = {"Head","Body","Leg"};
	int counter = 0;


	public ClothingConfigIterator(ClothingConfig para_config)
	{
		config = para_config;
	}

	public bool hasNext()
	{
		return (counter < categoryOrder.Length);
	}

	public string[] getNextClothingInfo()
	{
		string itemID = config.getClothing(categoryOrder[counter]);
		string[] retData = new string[2] { categoryOrder[counter], itemID };
		counter++;
		return retData;
	}
}

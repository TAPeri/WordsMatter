/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

[System.Serializable]
public class ClothingConfig
{
	Dictionary<string,string> gearByCategorySetup;

	public ClothingConfig()
	{
		// For serialisers only.
	}

	public void setClothing(string para_category, string para_clothingID)
	{
		if(gearByCategorySetup == null) { gearByCategorySetup = new Dictionary<string, string>(); }

		if(gearByCategorySetup.ContainsKey(para_category))
		{
			gearByCategorySetup[para_category] = para_clothingID;
		}
		else
		{
			gearByCategorySetup.Add(para_category,para_clothingID);
		}
	}

	public string getClothing(string para_category)
	{
		string retClothingID = null;
		if(gearByCategorySetup != null)
		{
			if(gearByCategorySetup.ContainsKey(para_category))
			{
				retClothingID = gearByCategorySetup[para_category];
			}
		}
		return retClothingID;
	}

	public ClothingConfigIterator getIterator()
	{
		ClothingConfigIterator iter = new ClothingConfigIterator(this);
		return iter;
	}
}

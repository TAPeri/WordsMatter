/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerAvatarSettings
{
	string gender;
	ClothingConfig clothingSettings;

	public PlayerAvatarSettings()
	{
		// For serialisers only.
	}

	public PlayerAvatarSettings(string para_gender,
	                            			 ClothingConfig para_clothingSettings)
	{

		//Debug.Log("GENDER: "+para_gender);
		gender = para_gender;
		clothingSettings = para_clothingSettings;
	}

	public string getGender() { return gender; }
	public ClothingConfig getClothingSettings() { return clothingSettings; }


	public void initWithDefaultState()
	{
		gender = "Male";
		clothingSettings = new ClothingConfig();
		string basicPrefix = "AV01";
		clothingSettings.setClothing("Head",basicPrefix);
		clothingSettings.setClothing("Body",basicPrefix);
		clothingSettings.setClothing("Leg",basicPrefix);
	}
}
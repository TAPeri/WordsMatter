/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ClothingManager
{
	static ClothingManager instance = null;

	public static APTable apTable;
	public static APCategoryTable apCategoryTable;

	public static Dictionary<string,Sprite> loadedClothingSprites;

	
	private ClothingManager(ClothingSize para_size)
	{
		// Singletons only.
		loadTables(para_size);
		loadClothingSprites(para_size);
	}

	public static ClothingManager getInstance(ClothingSize para_size)
	{
		if(instance == null)
		{
			// Create instance.
			instance = new ClothingManager(para_size);
		}
		return instance;
	}

	public static void disposeInstance()
	{
		if(instance != null)
		{
			instance = null;
		}
	}

	public ClothingCatalog loadNGetFullClothingCatalog(ClothingSize para_size)
	{
		List<string> clothingCharOrder = new List<string>();


		int counter = 1;
		bool itemFound = true;
		while(itemFound)
		{
			string tmpName = "MainAvatar";
			if(counter <= 9) { tmpName += ("0"+counter); } else { tmpName += counter; }

			Transform tmpTrans = Resources.Load<Transform>("Prefabs/Avatars/"+tmpName);
			if(tmpTrans == null)
			{
				itemFound = false;
			}
			else
			{
				clothingCharOrder.Add(tmpName);
			}
			
			counter++;
		}

		
		
		ClothingCatalog clothingCatalog = new ClothingCatalog();
		if(clothingCharOrder != null)
		{
			for(int i=0; i<clothingCharOrder.Count; i++)
			{
				string reqCharName = clothingCharOrder[i];
				
				string charSpritePrefix = "";
				if(reqCharName.Contains("MainAvatar"))
				{
					charSpritePrefix = "AV";
					if(para_size == ClothingSize.BIG) { charSpritePrefix = "Big_AV"; }
					int tmpNumID = int.Parse(reqCharName.Split(new string[]{"MainAvatar"},System.StringSplitOptions.None)[1]);
					if(tmpNumID <= 9) { charSpritePrefix += "0"; }
					charSpritePrefix += tmpNumID;
				}

				clothingCatalog.addHeadGear(charSpritePrefix);				
				clothingCatalog.addBodyGear(charSpritePrefix);
				clothingCatalog.addLegGear(charSpritePrefix);
			}
		}

		return clothingCatalog;
	}


	private void loadTables(ClothingSize para_size)
	{
		// Get APTable Data.

		string apcsvtableLocation = "APTableFolder/";
		string apcatcsvtableLocation = "APTableFolder/";
		switch(para_size)
		{
			case ClothingSize.BIG:	apcsvtableLocation += "BigAvatars/AvatarPieceTree_Big";
									apcatcsvtableLocation += "BigAvatars/APCategories_Big";
									break;

			default:				apcsvtableLocation += "SmallAvatars/AvatarPieceTree_Small";
									apcatcsvtableLocation += "SmallAvatars/APCategories_Small";
									break;
		}

		CSVTableHelper csvHelper = new CSVTableHelper();
		CSVTable extractedAPCSVTable = csvHelper.loadCSVTable("AvatarPieceTree",apcsvtableLocation);
		CSVTable extractedAPCatCSVTable = csvHelper.loadCSVTable("APCategories",apcatcsvtableLocation);
		
		apTable = new APTable();
		apCategoryTable = new APCategoryTable();
		apTable.buildFromCSVTable(extractedAPCSVTable);
		apCategoryTable.buildFromCSVTable(extractedAPCatCSVTable);
	}

	private void loadClothingSprites(ClothingSize para_size)
	{
		loadedClothingSprites = new Dictionary<string, Sprite>();

		//(BIG CLOTHING) Big_AV01, Big_AV02
		//(SMALL CLOTHING) AV_Spritesheet_01, AV_Spritesheet_02

		if(para_size == ClothingSize.BIG)
		{
			for(int i=0; i<10; i++)
			{
				int artNum = (i+1);
				string numSuffix = "";
				if(artNum < 10) { numSuffix += "0"; }
				numSuffix += artNum;

				Sprite[] tmpSpriteArr = Resources.LoadAll<Sprite>("Prefabs/Avatars/Big_AVs_Sprites/Big_AV"+numSuffix);
				if(tmpSpriteArr != null)
				{
					for(int k=0; k<tmpSpriteArr.Length; k++)
					{
						string nxtKey = tmpSpriteArr[k].name;
						if(loadedClothingSprites.ContainsKey(nxtKey))
						{
							loadedClothingSprites[nxtKey] = tmpSpriteArr[k];
						}
						else
						{
							loadedClothingSprites.Add(nxtKey,tmpSpriteArr[k]);
						}
					}
				}
			}
		}
		else
		{
			// Default uses Small.
			for(int i=0; i<5; i++)
			{
				int artNum = (i+1);
				string numSuffix = "";
				if(artNum < 10) { numSuffix += "0"; }
				numSuffix += artNum;

				Sprite[] tmpSpriteArr = Resources.LoadAll<Sprite>("Prefabs/Avatars/AV_Spritesheet_"+numSuffix);
				if(tmpSpriteArr != null)
				{
					for(int k=0; k<tmpSpriteArr.Length; k++)
					{
						loadedClothingSprites.Add(tmpSpriteArr[k].name,tmpSpriteArr[k]);
					}
				}
			}
		}
	}
}

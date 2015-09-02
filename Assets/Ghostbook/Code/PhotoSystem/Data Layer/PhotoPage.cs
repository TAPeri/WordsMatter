/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

[System.Serializable]
public class PhotoPage
{
	string pageName;
	public string longPageName = "";
	string langAreaDifficultyComboKey;
	Dictionary<int,Photo> photoCollection;
	string humanReadableDescription;
	
	public PhotoPage(string para_pageName,
	                  		  string para_langAreaDifficultyComboKey,
	                  		  Dictionary<int,Photo> para_photoCollection)
	{

		UnityEngine.Debug.LogError("DEPRECATED");

		pageName = para_pageName;
		langAreaDifficultyComboKey = para_langAreaDifficultyComboKey;
		photoCollection = para_photoCollection;
		if(photoCollection == null) { photoCollection = new Dictionary<int,Photo>(); }
	}


	public PhotoPage(string para_pageName,
	                 string para_langAreaDifficultyComboKey,
	                 Dictionary<int,Photo> para_photoCollection,
	                 string humanReadable)
	{
		pageName = para_pageName;
		langAreaDifficultyComboKey = para_langAreaDifficultyComboKey;
		photoCollection = para_photoCollection;
		if(photoCollection == null) { photoCollection = new Dictionary<int,Photo>(); }
		humanReadableDescription = humanReadable;
	}


	public string getPageName() { return pageName; }
	public string getLangAreaDifficultyComboKey() { return langAreaDifficultyComboKey; }
	public Dictionary<int,Photo> getAvailablePhotos() { return photoCollection; }
	public int getNumAvailablePhotos() { return photoCollection.Count; }
	public string getExplanation(){return humanReadableDescription;}

	public int getLangArea()
	{
		int retLA = -1;
		if(langAreaDifficultyComboKey != null)
		{
			retLA = int.Parse(langAreaDifficultyComboKey.Split('*')[0]);
		}
		return retLA;
	}

	public int getDifficulty()
	{
		int retDiff = -1;
		if(langAreaDifficultyComboKey != null)
		{
			retDiff = int.Parse(langAreaDifficultyComboKey.Split('*')[1]);
		}
		return retDiff; 
	}

	/*public void setLongName(string name){
		longPageName = name;
	}*/
	
	public void addPhoto(Photo para_photo, int para_stickPosition)
	{
		if(para_photo != null)
		{
			if(photoCollection == null) { photoCollection = new Dictionary<int, Photo>(); }
			photoCollection.Add(para_stickPosition,para_photo);
		}
	}
	
	public void removePhoto(int para_position)
	{
		if(photoCollection != null)
		{
			if(photoCollection.ContainsKey(para_position))
			{
				photoCollection.Remove(para_position);
			}
		}
	}
	
	public bool isPhotoStickPosVacant(int para_stickPosition)
	{
		bool isVacantSpot = false;
		if(photoCollection != null)
		{
			if(( ! photoCollection.ContainsKey(para_stickPosition))&&(para_stickPosition >=0)&&(para_stickPosition <= 3))
			{
				isVacantSpot = true;
			}
		}
		return isVacantSpot;
	}
}
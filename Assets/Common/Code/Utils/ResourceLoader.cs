/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ResourceLoader 
{
	public static Dictionary<string,Texture2D> cacheTextures = null;
	private const string commonTexDirectory = "Textures/Common/";


	public static void initialise()
	{
		cacheTextures = new Dictionary<string, Texture2D>();
		cacheTextures.Add("UnknownTexture", Resources.Load<Texture2D>(commonTexDirectory + "UnknownTexture"));
	}


	public static Texture2D getTexture(string paraTextureID, string paraDir)
	{
		return getTexture(paraDir + paraTextureID);
	}

	public static Texture2D getTexture(string para_fullPathLocation)
	{
		Texture2D retTexture = null;

		if(cacheTextures == null) { initialise(); }

		string[] splitPath = para_fullPathLocation.Split('/');
		string texName = splitPath[splitPath.Length-1];
		
		if(cacheTextures.ContainsKey(texName))
		{
			retTexture = cacheTextures[texName];
			retTexture.name = texName;
		}
		else
		{
			Texture2D nwTexture = (Texture2D) Resources.Load<Texture2D>(para_fullPathLocation);
			
			if(nwTexture == null)
			{			
				retTexture = cacheTextures["UnknownTexture"];	
				retTexture.name = "UnknownTexture";
			}
			else
			{
				cacheTextures.Add(texName,nwTexture);
				retTexture = nwTexture;
				retTexture.name = texName;
			}
		}
		
		return retTexture;
	}

	public static TextureCollection createTextureMapArr(List<string> para_fullPathLocations)
	{
		List<Texture2D> tmpTexList = new List<Texture2D>();
		List<int> indexMap = new List<int>();

		Dictionary<string,int> seenTextures = new Dictionary<string,int>();
		for(int i=0; i<para_fullPathLocations.Count; i++)
		{
			string tmpFPath = para_fullPathLocations[i];

			if(seenTextures.ContainsKey(tmpFPath))
			{
				indexMap.Add(seenTextures[tmpFPath]);
			}
			else
			{
				Texture2D nwTex = getTexture(para_fullPathLocations[i]);
				tmpTexList.Add(nwTex);
				seenTextures.Add(tmpFPath,tmpTexList.Count-1);
				indexMap.Add(tmpTexList.Count-1);
			}
		}


		Texture2D[] retTexArr = tmpTexList.ToArray();
		int[] retIndexMap = indexMap.ToArray();

		TextureCollection tCol = new TextureCollection(retTexArr,retIndexMap);
		return tCol;
	}

	public static void flushAllCaches()
	{
		cacheTextures.Clear();
	}
}
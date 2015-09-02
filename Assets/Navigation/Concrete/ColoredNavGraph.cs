/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

using Color = UnityEngine.Color;

public class ColoredNavGraph : BasicNavGraph
{

	Dictionary<Color,int> colorToNodeType;
	Dictionary<string,Color> nameToColor;



	public ColoredNavGraph(ColorGraphTypeInfo para_typeInfo)
		:base()
	{
		this.init(para_typeInfo.typeNames,para_typeInfo.typeColors);
	}

	public ColoredNavGraph(string[] para_typeNames, Color[] para_typeColors)
		:base()
	{
		this.init(para_typeNames,para_typeColors);
	}


	private void init(string[] para_typeNames, Color[] para_typeColors)
	{
		colorToNodeType = new Dictionary<Color, int>();
		nameToColor = new Dictionary<string,Color>();
		for(int i=0; i<para_typeNames.Length; i++)
		{
			string currTypeName = para_typeNames[i];
			Color currTypeColor;
			if(i < para_typeColors.Length) { currTypeColor = para_typeColors[i]; } else { currTypeColor = Color.magenta; }
			
			nameToColor.Add(currTypeName,currTypeColor);
			colorToNodeType.Add(currTypeColor,i);
		}
	}



	// *** Get Color Methods. ***

	public Color getColorByTypeID(int para_typeID)
	{
		Color retColor = Color.magenta;
		foreach(KeyValuePair<Color,int> pair in colorToNodeType)
		{
			if(pair.Value == para_typeID)
			{
				retColor = pair.Key;
				break;
			}
		}
		return retColor;
	}

	public Color getColorByTypeName(string para_typeName)
	{
		Color retColor = Color.magenta;
		if((nameToColor != null)
		&&(nameToColor.ContainsKey(para_typeName)))
		{
			retColor = nameToColor[para_typeName];
		}
		return retColor;
	}



	// *** Get Type ID Methods. ***

	public int getTypeIDByColor(Color para_typeColor)
	{
		int retTypeID = -1;
		if((colorToNodeType != null)
		&&(colorToNodeType.ContainsKey(para_typeColor)))
		{
			retTypeID = colorToNodeType[para_typeColor];
		}
		return retTypeID;
	}

	public int getTypeIDByName(string para_typeName)
	{
		int retTypeID = -1;
		if((nameToColor != null)
		&&(nameToColor.ContainsKey(para_typeName)))
		{
			if((colorToNodeType != null)
			&&(colorToNodeType.ContainsKey(nameToColor[para_typeName])))
			{
				retTypeID = colorToNodeType[nameToColor[para_typeName]];
			}
		}
		return retTypeID;
	}


	// *** Get Type Name Methods ***

	public string getTypeNameByTypeID(int para_typeID)
	{
		string retName = null;


		Color reqColor = Color.magenta;
		bool foundColor = false;

		foreach(KeyValuePair<Color,int> pair in colorToNodeType)
		{
			if(pair.Value == para_typeID)
			{
				reqColor = pair.Key;
				foundColor = true;
				break;
			}
		}


		if(foundColor)
		{
			//bool foundName = false;

			foreach(KeyValuePair<string,Color> pair in nameToColor)
			{
				if(pair.Value == reqColor)
				{
					retName = pair.Key;
					foundColor = true;
					break;
				}
			}
		}


		return retName;
	}

	public string getTypeNamebyColor(Color para_typeColor)
	{
		string retName = null;

		foreach(KeyValuePair<string,Color> pair in nameToColor)
		{
			if(pair.Value == para_typeColor)
			{
				retName = pair.Key;
				break;
			}
		}

		return retName;
	}

	public List<string> getAllTypeNames()
	{
		List<string> retList = null;
		if(nameToColor != null)
		{
			retList = new List<string>(nameToColor.Keys);
		}
		return retList;
	}


	public Dictionary<int,Color> createTypeIDToColorMap()
	{
		Dictionary<int,Color> retMap = new Dictionary<int, Color>();

		foreach(KeyValuePair<Color,int> pair in colorToNodeType)
		{
			if( ! retMap.ContainsKey(pair.Value))
			{
				retMap.Add(pair.Value,pair.Key);
			}
			retMap[pair.Value] = pair.Key;
		}

		return retMap;
	}


}

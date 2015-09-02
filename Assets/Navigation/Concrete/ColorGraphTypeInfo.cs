/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using Color = UnityEngine.Color;

public class ColorGraphTypeInfo
{
	public string[] typeNames;
	public Color[] typeColors;

	public ColorGraphTypeInfo(string[] para_typeNames,
	                          Color[] para_typeColors)
	{
		typeNames = para_typeNames;
		typeColors = para_typeColors;
	}
}

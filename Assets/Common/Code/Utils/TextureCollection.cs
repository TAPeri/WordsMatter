/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;

public class TextureCollection
{
	public Texture2D[] texArr;
	public int[] indexArr;

	public TextureCollection(Texture2D[] para_texArr,
	                         int[] para_indexArr)
	{
		texArr = para_texArr;
		indexArr = para_indexArr;
	}
}
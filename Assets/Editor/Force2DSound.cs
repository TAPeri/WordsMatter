/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEditor;
using UnityEngine;

public class Force2DSound : AssetPostprocessor
{
	public void OnPreprocessAudio()
	{
		AudioImporter ai = assetImporter as AudioImporter;
		ai.threeD = false;
	}
}
/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class DifficultyMetaData
{
	string langAreaIDDiffIndexCombo;
	string name;
	string explanation;

	public DifficultyMetaData(string para_langAreaIDDiffIndexCombo, string para_name)
	{
		Debug.LogError("DEPRECATED");
		langAreaIDDiffIndexCombo = para_langAreaIDDiffIndexCombo;
		name = para_name;
	}

	public DifficultyMetaData(string para_langAreaIDDiffIndexCombo, string para_name, string para_explanation)
	{
		langAreaIDDiffIndexCombo = para_langAreaIDDiffIndexCombo;
		name = para_name;
		explanation = para_explanation;
	}

	public string getLangAreaIDDiffIndexCombo() { return langAreaIDDiffIndexCombo; }
	public string getName() { return name; }
	public string getExplanation() { return explanation; }
}
/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class FancyLabelTestScript : MonoBehaviour {

	FancyLabelTools fancyTools;
	string text;

	bool initGStyles = false;
	GUIStyle chosenLabelStyle;

	void Start()
	{
		fancyTools = new FancyLabelTools();

		/*text = "";
		text += "#FFDDDDFF"; // Light red
		text += "Fireball";
		text += "#!"; // revert to default color
		text += "\n\nHurls a ball of fire that ";
		text += "#x"; // bold font
		text += "explodes";
		text += "#n"; // normal font
		text += " on contact\n";
		text +="and damages all nearby enemies.\n\n";
		text += "#FF6666FF"; // red
		text += "#x"; // bold font
		text += "8";
		text += "#!"; // revert to default color
		text += "#n"; // normal font
		text += " to ";
		text += "#FF6666FF"; // red
		text += "#x"; // bold font
		text += "12";
		text += "#n"; // normal font
		text += "#!"; // revert to default color
		text += "#i"; // italic font
		text += " fire";
		text += "#n"; // normal font
		text += " damage";*/

		text = "";
		//text = "Hello there dude Hello there dude Hello there dude Hello there dude Hello there dude Hello there dude Hello there dude Hello there dude Hello there dude";
		text += "The dog ";
		text += "#66FF66FF";
		text += "#x";
		text += "slipped";
		text += "#!";
		text += "after the cat";
	}

	void OnGUI()
	{
		if( ! initGStyles)
		{
			chosenLabelStyle = new GUIStyle(GUI.skin.label);
			chosenLabelStyle.fontSize = 40;

			initGStyles = true;
		}
		else
		{
			fancyTools.FancyLabel(new Rect(0,0,500,500),text,GUI.skin.label.font,null,GUI.skin.label.font,TextAlignment.Left,chosenLabelStyle);
		}
	}
	
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;


public class ActivityInfoWindow : HorizImageNTextPopup
{
	string txtActivityTitle;
	string txtActivityHowTo;
	string txtGoal;


	void Start()
	{
		this.loadTextures();
		this.prepUIBounds();
	}

	void OnGUI()
	{
		if( ! hasInitGUIStyles)
		{
			this.prepGUIStyles();
			hasInitGUIStyles = true;
		}
		else
		{
			GUI.DrawTexture(uiBounds["Window"],availableTextures["Window"]);
			GUI.DrawTexture(uiBounds["Image"],availableTextures["Image"]);
			GUI.Label(uiBounds["ActivityTitle"],txtActivityTitle,availableGUIStyles["CentredLabel"]);
			GUI.Label(uiBounds["ActivityHowTo"],txtActivityHowTo,availableGUIStyles["LeftLabel"]);
			//GUI.Label(uiBounds["ActivityWinTitle"],"[ Goal ]",availableGUIStyles["LeftLabel"]);
			GUI.Label(uiBounds["ActivityWinDetails"],txtGoal,availableGUIStyles["LeftLabel"]);
			if(GUI.Button(uiBounds["OkButton"],"OK"))
			{
				notifyAllListeners(windowName,"OK",null);
				Destroy(this);
			}
		}
	}


	public new void init(string para_activityName, string para_howToText, string para_goalText)
	{
		base.init("ActivityInfoWindow","Textures/Common/InfoSkull","");

		txtActivityTitle = para_activityName;
		txtActivityHowTo = "[How to play]\n"+para_howToText;
		txtGoal = "[Goal]\n"+para_goalText;

		base.triggerSoundAtCamera("SFX_InfoScreen",3f);
	}


	protected new void prepUIBounds()
	{
		base.prepUIBounds();

		Rect textAreaBounds = uiBounds["TextArea"];

		float activityTitle_Width = textAreaBounds.width;
		float activityTitle_Height = textAreaBounds.height * 0.10f;
		float activityTitle_X = textAreaBounds.x;
		float activityTitle_Y = textAreaBounds.y;

		float activityHowTo_Width = textAreaBounds.width;
		float activityHowTo_Height = textAreaBounds.height * 0.60f;
		float activityHowTo_X = textAreaBounds.x;
		float activityHowTo_Y = activityTitle_Y + activityTitle_Height;

		/*float activityWinTitle_Width = textAreaBounds.width;
		float activityWinTitle_Height = textAreaBounds.height * 0.10f;
		float activityWinTitle_X = textAreaBounds.x;
		float activityWinTitle_Y = activityHowTo_Y + activityHowTo_Height;*/

		float activityWinDetails_Width = textAreaBounds.width;
		float activityWinDetails_Height = textAreaBounds.height * 0.25f;
		float activityWinDetails_X = textAreaBounds.x;
		float activityWinDetails_Y = activityHowTo_Y + activityHowTo_Height;//activityWinTitle_Y + activityWinTitle_Height;

		uiBounds.Add("ActivityTitle",new Rect(activityTitle_X,activityTitle_Y,activityTitle_Width,activityTitle_Height));
		uiBounds.Add("ActivityHowTo",new Rect(activityHowTo_X,activityHowTo_Y,activityHowTo_Width,activityHowTo_Height));
		//uiBounds.Add("ActivityWinTitle",new Rect(activityWinTitle_X,activityWinTitle_Y,activityWinTitle_Width,activityWinTitle_Height));
		uiBounds.Add("ActivityWinDetails",new Rect(activityWinDetails_X,activityWinDetails_Y,activityWinDetails_Width,activityWinDetails_Height));
	}

	protected new void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string,GUIStyle>(); hasInitGUIStyles = true;

		/*Screen.height 650 =  -->  fontsize 20
		Screen.height = >*/

		int reqFontSize = (int) ((Screen.height * 20f)/650f);

		GUIStyle centredLabel = new GUIStyle(GUI.skin.label);
		centredLabel.alignment = TextAnchor.MiddleCenter;
		centredLabel.fontSize = reqFontSize;// 20;

		GUIStyle leftAlignedLabel = new GUIStyle(GUI.skin.label);
		leftAlignedLabel.alignment = TextAnchor.MiddleLeft;
		leftAlignedLabel.fontSize = reqFontSize;//20;
		
		availableGUIStyles.Add("CentredLabel",centredLabel);
		availableGUIStyles.Add("LeftLabel",leftAlignedLabel);
	}
}

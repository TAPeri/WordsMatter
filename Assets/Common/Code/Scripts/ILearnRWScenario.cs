/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public abstract class ILearnRWScenario : MonoBehaviour
{

	protected Dictionary<string,Rect> uiBounds;
	protected Dictionary<string,Texture2D> availableTextures;
	protected Dictionary<string,GUIStyle> availableGUIStyles;
	protected bool hasInitGUIStyles = false;




	/*void OnGUI()
	{
		if( ! hasInitGUIStyles)
		{
			prepGUIStyles();
			hasInitGUIStyles = true;
		}
		else
		{
			// Draw GUI Components.
		}
	}*/




	protected void loadTextures() { availableTextures = new Dictionary<string,Texture2D>(); }
	protected void prepUIBounds() { uiBounds = new Dictionary<string,Rect>(); }
	protected void prepGUIStyles() { availableGUIStyles = new Dictionary<string,GUIStyle>(); hasInitGUIStyles = true; }
}

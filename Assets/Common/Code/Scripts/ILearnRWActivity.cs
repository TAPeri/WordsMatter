/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public abstract class ILearnRWActivity : MonoBehaviour
{
	Dictionary<string,Rect> uiBounds;
	Dictionary<string,Texture2D> availableTextures;
	Dictionary<string,GUIStyle> availableGUIStyles;
	//bool hasInitGUIStyles = false;
	//bool hasInitialised = false;


	public abstract void initWorld();			// **** Create any scene elements which must be spawned programmatically before the activity starts. ****
	public abstract void createLevel();			// **** Refresh the scenario and spawn/create the material for the next level in the activity. 		 ****
	public abstract void loadTextures();		// **** Setup 'availableTextures'. Perform 'Resource.Load' loading here only. 						 ****
	public abstract void prepUIBounds();		// **** Setup 'uiBounds'. 																			 ****
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class ActivityLaunchParams
{
	public ApplicationID appID;
	public int difficulty;
	public int languageArea;
	public string userID;
	public int evaluation_mode;
	public string challenge;
	public Mode mode;
	public string modeDetails;

	public ActivityLaunchParams(ApplicationID _appID,
	                            int _difficulty,
	                            int _languageArea,
	                            string _userID,
	                            int _evaluation_mode,
	                            string _challenge,
	                            Mode _mode,
	                            string _modeDetais)
	{
		appID = _appID;
		difficulty = _difficulty;
		languageArea = _languageArea;
		userID = _userID;
		evaluation_mode = _evaluation_mode;
		challenge = _challenge;
		mode = _mode;
		modeDetails = _modeDetais;
	}

}

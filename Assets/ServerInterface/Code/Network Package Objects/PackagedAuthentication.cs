/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */[System.Serializable]

public class PackagedAuthentication{

	public string auth;
	public string refresh;
	
	public PackagedAuthentication()
	{
		// Empty constructor required for JSON converter.
	}
	
	public string getAuth() { return auth; }
	public string getRefresh() { return refresh; }
}

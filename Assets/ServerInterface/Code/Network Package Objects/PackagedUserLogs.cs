/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
[System.Serializable]


public class PackagedUserLogs {

	
	public PackagedUserLogs()
	{
		// Empty constructor required for JSON converter.
	}

	public PackagedUserLogs(int p, UserLog[] r, int tp){ page = p; results = r; totalPages = tp;}

	public int page;
	public UserLog[] results;
	public int totalPages;

	public int getPage(){return page;}
	public UserLog[] getLogs(){return results;}
	public int getTotalPages(){return totalPages;}

	
}

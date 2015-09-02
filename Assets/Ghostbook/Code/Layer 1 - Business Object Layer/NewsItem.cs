/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
[System.Serializable]
public class NewsItem
{
	protected int newsID;
	protected int newsType;//deprecated
	protected string dateTimestamp;//deprecated
	protected string newsText;

	public NewsItem(int para_newsID,
	                  			int para_newsType,
	                  			string para_dateTimestamp,
	                  			string para_newsText)
	{
		newsID = para_newsID;
		newsType = para_newsType;
		dateTimestamp = para_dateTimestamp;
		newsText = para_newsText;
	}

	public int getNewsID() { return newsID; }
	public int getNewsType() { UnityEngine.Debug.LogError("Deprecated");return newsType; }
	public string getDateTimestamp() { UnityEngine.Debug.LogError("Deprecated");return dateTimestamp; }
	public virtual string getNewsText() { return newsText; }
}
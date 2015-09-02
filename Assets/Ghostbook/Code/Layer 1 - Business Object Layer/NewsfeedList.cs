/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

[System.Serializable]
public class NewsfeedList
{
	int nxtID;
	List<NewsItem> newsItems;

	public NewsfeedList()
	{
		// For serialisers only.
	}

	public NewsfeedList(bool para_dummyFlag)
	{
		nxtID = 0;
		newsItems = new List<NewsItem>();
	}

	public void addNewsItem(int para_newsType,
					                      string para_newsText)
	{
		if(newsItems == null) { newsItems = new List<NewsItem>(); }
		NewsItem nwItem = new NewsItem(nxtID,para_newsType,System.DateTime.Now.ToString("dd\\/MM\\/yyyy h\\:mm tt"),para_newsText);
		newsItems.Add(nwItem);
		nxtID++;
	}

	public void addNewsItemPastActivity(ApplicationID para_acPlayed, int para_langArea, int para_difficulty, int para_questGiverID,string level, string date)
	{
		if(newsItems == null) { newsItems = new List<NewsItem>(); }
		NIPastActivity nwItem = new NIPastActivity(nxtID,2,System.DateTime.Now.ToString("dd\\/MM\\/yyyy h\\:mm tt"),para_acPlayed,para_langArea,para_difficulty,para_questGiverID, level,  date);
		newsItems.Add(nwItem);
		nxtID++;
	}

	public void eraseAllItems()
	{
		if(newsItems == null) { newsItems = new List<NewsItem>(); }
		else { newsItems.Clear(); }
	}

	public List<NewsItem> getAllNewsItems() { return newsItems; }
}
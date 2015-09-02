/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public interface INewsfeedServices
{
	List<NewsItem> getNewsItems();
	void addNewsItem(int para_newsType, string para_newsText);
	void addNewsItemPastActivity(ApplicationID para_acPlayed, int para_langArea, int para_difficulty, int para_questGiverID,string level, string date);
	void eraseAllItems();
}

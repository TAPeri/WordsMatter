/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class HashingBucket
{
	HashSet<int> items;

	public HashingBucket()
	{
		items = new HashSet<int>();
	}

	public HashSet<int> getAllItems()
	{
		return items;
	}

	public bool containsItem(int para_itemID)
	{
		return (items.Contains(para_itemID));
	}

	public void addItem(int para_itemID)
	{
		if( ! containsItem(para_itemID))
		{
			items.Add(para_itemID);
		}
	}


	/*public bool containsItem(int para_itemID)
	{
		bool retFlag = false;
		if((items != null)&&(items.Count > 0))
		{
			for(int i=0; i<items.Count; i++)
			{
				if(items[i] == para_itemID)
				{
					retFlag = true;
					break;
				}
			}
		}
		return retFlag;
	}*/
}

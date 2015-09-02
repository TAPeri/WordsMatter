/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class ClothingCatalogIterator
{
	protected string iteratorName;
	protected List<string> clothesList;
	protected int counter = -1;

	public ClothingCatalogIterator(string para_iteratorName, List<string> para_catalogSection)
	{
		iteratorName = para_iteratorName;
		clothesList = para_catalogSection;
		counter = -1;
	}

	public string getIteratorName() { return iteratorName; }

	public virtual bool hasPrevious()
	{
		if(clothesList != null)
		{
			if(counter > 0)
			{
				return true;
			}
		}

		return false;
	}

	public virtual bool hasNext()
	{
		if(clothesList != null)
		{
			if((counter >= -1)&&(counter < (clothesList.Count-1)))
			{
				return true;
			}
		}

		return false;
	}

	public virtual string getNext()
	{
		if(hasNext())
		{
			counter++;
			return clothesList[counter];
		}
		else{ return null; }
	}

	public virtual string getPrevious()
	{
		if(hasPrevious())
		{
			counter--;
			return clothesList[counter];
		}
		else { return null; }
	}

	public string getCurrentItem()
	{
		return clothesList[counter];
	}

	public bool directAccessToItem(string para_itemName)
	{
		bool successFlag = false;
		if(clothesList != null)
		{
			for(int i=0; i<clothesList.Count; i++)
			{
				string tmpName = clothesList[i];
				if(tmpName != null)
				{
					if((tmpName == para_itemName)
						||(tmpName == ("Big_"+para_itemName)))
					{
						successFlag = true;
						counter = i;
						break;
					}
				}
			}
		}
		return successFlag;
	}
}

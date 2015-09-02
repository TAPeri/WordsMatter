/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class SpecialCatalogIterator : ClothingCatalogIterator
{
	int incStep = 1;


	public SpecialCatalogIterator(string para_iteratorName, List<string> para_catalogSection, int para_startIndex, int para_incStep)
		:base(para_iteratorName,para_catalogSection)
	{
		counter = para_startIndex;
		incStep = para_incStep;
	}


	public override bool hasPrevious()
	{
		if(clothesList != null)
		{
			if((counter-incStep) >= 0)
			{
				return true;
			}
		}
		
		return false;
	}
	
	public override bool hasNext()
	{
		if(clothesList != null)
		{
			int tmpRes = counter+incStep;
			if((tmpRes >= 0)&&(tmpRes < clothesList.Count))
			{
				return true;
			}
		}
		
		return false;
	}
	
	public override string getNext()
	{
		if(hasNext())
		{
			counter += incStep;
			return clothesList[counter];
		}
		else{ return null; }
	}
	
	public override string getPrevious()
	{
		if(hasPrevious())
		{
			counter -= incStep;
			return clothesList[counter];
		}
		else { return null; }
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class IndexRegion
{
	public int startIndex;
	public int endIndex;

	public IndexRegion(int para_startIndex, int para_endIndex)
	{
		startIndex = para_startIndex;
		endIndex = para_endIndex;
	}

	public bool contains(IndexRegion para_reg2)
	{
		return 
			
			((para_reg2.startIndex >= startIndex)
			 &&(para_reg2.startIndex <= endIndex)
			 &&(para_reg2.endIndex >= startIndex)
			 &&(para_reg2.endIndex <= endIndex));
	}

	public int getRegionLength()
	{
		return ((endIndex - startIndex)+1);
	}
}

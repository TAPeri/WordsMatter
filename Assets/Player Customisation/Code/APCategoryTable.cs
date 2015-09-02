/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class APCategoryTable : GBTable<APCategoryTableRow>
{



	public int getPKeyUsingCategoryName(string para_categoryName)
	{
		int retPKey = -1;

		if(rowData != null)
		{
			for(int i=0; i<rowData.Count; i++)
			{
				APCategoryTableRow tmpRow = rowData[i];
				if(tmpRow.getCategoryName() == para_categoryName)
				{
					retPKey = tmpRow.getBodyCategoryID();
					break;
				}
			}
		}

		return retPKey;
	}

	public int[] getTopLevelPieces(int para_id)
	{
		int[] retArr = null;
		APCategoryTableRow reqRow = findAPCategoryTableRowByID(para_id);
		if(reqRow != null) { retArr = reqRow.getTopLevelPieces(); }
		return retArr;
	}

	public int[] getSinglePieces(int para_id)
	{
		int[] retArr = null;
		APCategoryTableRow reqRow = findAPCategoryTableRowByID(para_id);
		if(reqRow != null) { retArr = reqRow.getSinglePieces(); }
		return retArr;
	}


	private APCategoryTableRow findAPCategoryTableRowByID(int para_id)
	{
		APCategoryTableRow reqRow = null;
		
		if(rowData != null)
		{
			for(int i=0; i<rowData.Count; i++)
			{
				APCategoryTableRow tmpRow = rowData[i];
				if(tmpRow.getBodyCategoryID() == para_id)
				{
					reqRow = rowData[i];
					break;
				}
			}
		}
		
		return reqRow;
	}
}

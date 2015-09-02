/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class APTable : GBTable<APTableRow>
{


	public APTableRow findAPTableRow(int para_id)
	{
		APTableRow reqRow = null;
		
		if(rowData != null)
		{
			for(int i=0; i<rowData.Count; i++)
			{
				APTableRow tmpRow = rowData[i];
				if(tmpRow.getAvatarPieceID() == para_id)
				{
					reqRow = rowData[i];
					break;
				}
			}
		}
		
		return reqRow;
	}
}

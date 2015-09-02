/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class CSVTable : CustomTable<CSVTableRow>
{
	public CSVTable(string para_tableName,
					string[] para_columnHeaders,
	                List<CSVTableRow> para_rowData)
		:base(para_tableName,para_columnHeaders,para_rowData)
	{
		//
	}
}

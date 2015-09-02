/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class CustomTable<R> where R:ICustomTableRow
{
	protected string tableName;
	protected string[] columnHeaders;
	protected List<R> rowData;

	public CustomTable()
	{

	}

	public CustomTable(string para_tableName,
	                   string[] para_columnHeaders,
	                   List<R> para_rowData)
	{
		tableName = para_tableName;
		columnHeaders = para_columnHeaders;
		rowData = para_rowData;
	}
	
	public string getTableName() { return tableName; }
	public string[] getAllColumnHeaders() { return columnHeaders; }
	public List<R> getAllRows() { return rowData; }
	
	public int getColumnCount()
	{
		return ((columnHeaders == null) ? -1 : columnHeaders.Length);
	}
	
	public int getRowCount()
	{
		return ((rowData == null) ? -1 : rowData.Count);
	}

	public string getColumnHeader(int para_colIndex)
	{
		return ((columnHeaders == null)||(para_colIndex < 0)||(para_colIndex >= columnHeaders.Length)) ? null : columnHeaders[para_colIndex];
	}

	public int findColumn(string para_columnName)
	{
		int retIndex = -1;

		if((columnHeaders != null)&&(para_columnName != null))
		{
			for(int i=0; i<columnHeaders.Length; i++)
			{
				if(columnHeaders[i] == para_columnName)
				{
					retIndex = i;
					break;
				}
			}
		}

		return retIndex;
	}

	public R getSingleRow(int para_rowIndex)
	{
		return ((rowData == null)||(para_rowIndex < 0)||(para_rowIndex >= rowData.Count)) ? default(R) : rowData[para_rowIndex];
	}
}

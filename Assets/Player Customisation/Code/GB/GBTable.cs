/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class GBTable<R> : CustomTable<R> where R:GBTableRow,new()
{
	public GBTable()
	{

	}

	public GBTable(string para_tableName,
	               string[] para_columnHeaders,
	               List<R> para_rowData)
		:base(para_tableName,para_columnHeaders,para_rowData)
	{
		//
	}

	public void setTableName(string para_tableName) { tableName = para_tableName; }
	public void setColumnHeaders(string[] para_columnHeaders) { columnHeaders = para_columnHeaders; }
	public void setRows(List<R> para_rows) { rowData = para_rows; }


	public virtual void buildFromCSVTable(CSVTable para_csvTable)
	{
		if(para_csvTable != null)
		{
			tableName = para_csvTable.getTableName();
			columnHeaders = para_csvTable.getAllColumnHeaders();
			
			rowData = new List<R>();
			int totRows = para_csvTable.getRowCount();
			
			for(int i=0; i<totRows; i++)
			{
				R nwRow = new R();
				nwRow.buildFromCSVRow(para_csvTable.getSingleRow(i));
				rowData.Add(nwRow);
			}
		}
	}

	public virtual CSVTable createCSVTable()
	{
		string csvTableName = tableName;
		string[] csvColumnHeaders = columnHeaders;
		List<CSVTableRow> csvRowData = new List<CSVTableRow>();

		if(rowData != null)
		{
			for(int i=0; i<rowData.Count; i++)
			{
				csvRowData.Add(rowData[i].generateCSVRow());
			}
		}

		CSVTable retTable = new CSVTable(csvTableName,csvColumnHeaders,csvRowData);
		return retTable;
	}


}

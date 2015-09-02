/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.IO;
using System.Collections.Generic;

public class CSVTableHelper
{
	public CSVTable loadCSVTable(string para_tableName, string para_fullFilePath)
	{
		UnityEngine.TextAsset ta = (UnityEngine.TextAsset) UnityEngine.Resources.Load(para_fullFilePath,typeof(UnityEngine.TextAsset));
		StringReader sr = new StringReader(ta.text);


		List<string> columnHeaders = new List<string>();
		List<CSVTableRow> dataRows = new List<CSVTableRow>();

		int counter = 0;
		string csvLine = sr.ReadLine();
		while(csvLine != null)
		{
			List<string> parsedLine = parseCSVStringToList(csvLine);

			if(counter == 0)
			{
				// Store column headers.
				columnHeaders = parsedLine;
			}
			else
			{
				// Store next data row.
				dataRows.Add(new CSVTableRow(parsedLine));
			}

			counter++;
			csvLine = sr.ReadLine();
		}


		sr.Close();
		sr.Dispose();


		// Fill Table.
		CSVTable nwTable = new CSVTable(para_tableName,columnHeaders.ToArray(),dataRows);
		return nwTable;
	}


	public void saveCSVTable(string para_fullFilePath, CSVTable para_table)
	{
		StreamWriter sw = File.CreateText(para_fullFilePath);

		// Apply Column Header Row.
		string[] columnHeaders = para_table.getAllColumnHeaders();
		if(columnHeaders != null)
		{
			string columnHeaderCSVRow = "";
			for(int i=0; i<columnHeaders.Length; i++)
			{
				string nxtColumnHeader = columnHeaders[i];
				if(nxtColumnHeader != null)
				{
					columnHeaderCSVRow += nxtColumnHeader;
				}

				if(i < (columnHeaders.Length-1))
				{
					columnHeaderCSVRow += ",";
				}
			}

			sw.WriteLine(columnHeaderCSVRow);
		}

		// Apply Data Rows.
		List<CSVTableRow> rowData = para_table.getAllRows();
		if(rowData != null)
		{
			for(int i=0; i<rowData.Count; i++)
			{
				CSVTableRow nxtCSVRow = rowData[i];
				string commaDelimRow = nxtCSVRow.getCommaDelimStr();
				sw.WriteLine(commaDelimRow);
			}
		}

		sw.Close();
		sw.Dispose();
	}

	private List<string> parseCSVStringToList(string para_csvString)
	{
		List<string> retList = null;
		string[] splitStrs = para_csvString.Split(',');
		retList = new List<string>(splitStrs);
		return retList;
	}
}

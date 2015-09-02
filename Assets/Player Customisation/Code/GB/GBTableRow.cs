/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public abstract class GBTableRow : ICustomTableRow
{
	public GBTableRow() {}

	public abstract void buildFromCSVRow(CSVTableRow para_csvRow);
	public abstract CSVTableRow generateCSVRow();

	public int[] convertStrIntoIntArr(string para_str)
	{
		string[] retParts = para_str.Split('-');
		
		if(retParts == null)
		{
			return null;
		}
		else
		{
			List<int> tmpList = new List<int>();
			for(int i=0; i<retParts.Length; i++)
			{
				string tmpStr = retParts[i];
				if(tmpStr != null)
				{
					tmpStr = tmpStr.Trim();
					if(tmpStr != "")
					{
						tmpList.Add(int.Parse(tmpStr));
					}
				}
			}
			
			return tmpList.ToArray();
		}
	}


	public string convertIntArrToDelimStr(int[] para_intArr, string para_delimiter)
	{
		string retStr = "";

		if(para_intArr != null)
		{
			for(int i=0; i<para_intArr.Length; i++)
			{
				retStr += (""+para_intArr[i]);

				if(i < (para_intArr.Length-1))
				{
					retStr += para_delimiter;
				}
			}
		}

		return retStr;
	}
}

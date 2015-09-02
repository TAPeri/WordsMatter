/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class CSVTableRow : ICustomTableRow
{
	List<string> elements;

	public CSVTableRow(List<string> para_elements)
	{
		elements = para_elements;
	}

	public string getCommaDelimStr()
	{
		string retStr = "";

		if(elements != null)
		{
			for(int i=0; i<elements.Count; i++)
			{
				string nxtElement = elements[i];
				if(nxtElement != null)
				{
					retStr += nxtElement;
				}

				if(i < (elements.Count-1))
				{
					retStr += ",";
				}
			}
		}

		return retStr;
	}

	public List<string> getElements() { return elements; }
}

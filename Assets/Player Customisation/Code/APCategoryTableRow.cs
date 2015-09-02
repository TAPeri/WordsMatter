/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class APCategoryTableRow : GBTableRow
{

	int bodyCategoryID;
	string categoryName;
	int[] topLevelPieces;
	int[] singlePieces;


	public override void buildFromCSVRow(CSVTableRow para_csvRow)
	{
		List<string> csvElements = para_csvRow.getElements();


		bodyCategoryID = int.Parse(csvElements[0]);
		categoryName = csvElements[1];
		topLevelPieces = convertStrIntoIntArr(csvElements[2]);
		singlePieces = convertStrIntoIntArr(csvElements[3]);
	}

	public override CSVTableRow generateCSVRow ()
	{
		List<string> elementList = new List<string>() { ""+bodyCategoryID, categoryName };
		elementList.Add(convertIntArrToDelimStr(topLevelPieces,"-"));
		elementList.Add(convertIntArrToDelimStr(singlePieces,"-"));

		CSVTableRow retCsvRow = new CSVTableRow(elementList);
		return retCsvRow;
	}

	public int getBodyCategoryID() { return bodyCategoryID; }
	public string getCategoryName() { return categoryName; }
	public int[] getTopLevelPieces() { return topLevelPieces; }
	public int[] getSinglePieces() { return singlePieces; }
}

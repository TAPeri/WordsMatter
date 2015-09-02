/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class APTableRow : GBTableRow
{

	int avatarPieceID;
	string gObjName;
	string spriteNameSuffix;
	int[] childPieces;


	public override void buildFromCSVRow(CSVTableRow para_csvRow)
	{
		List<string> csvElements = para_csvRow.getElements();
		
		avatarPieceID = int.Parse(csvElements[0]);
		gObjName = csvElements[1];
		spriteNameSuffix = csvElements[2];

		string[] retParts = csvElements[3].Split('-');

		if(retParts == null)
		{
			childPieces = null;
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

			childPieces = tmpList.ToArray();
		}
	}

	public override CSVTableRow generateCSVRow ()
	{
		List<string> elementList = new List<string>() { ""+avatarPieceID, gObjName, spriteNameSuffix, convertIntArrToDelimStr(childPieces,"-") };
		CSVTableRow retCsvRow = new CSVTableRow(elementList);
		return retCsvRow;
	}

	public int getAvatarPieceID() { return avatarPieceID; }
	public string getGObjName() { return gObjName; }
	public string getSpriteNameSuffix() { return spriteNameSuffix; }
	public int[] getChildPieces() { return childPieces; }
}

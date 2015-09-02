/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class PDBCAddPhoto : PhotoDatabaseCommand
{
	public int questGiverID;
	public int activityOwnerID;
	public ApplicationID activityKey;
//	public int activityKey;
	public int langAreaID;
	public int diffIndexInLangArea;
	public int photoDiffPosition;
	public string[] text;

	public PDBCAddPhoto(int para_questGiverID, int para_activityOwnerID, ApplicationID para_activityKey, int para_langAreaID, int para_diffIndexInLangArea, int para_photoDiffPosition,string[] text)
	{

		questGiverID = para_questGiverID;
		activityOwnerID = para_activityOwnerID;
		activityKey = para_activityKey;
		langAreaID = para_langAreaID;
		diffIndexInLangArea = para_diffIndexInLangArea;
		photoDiffPosition = para_photoDiffPosition;
		this.text = text;
	}
}

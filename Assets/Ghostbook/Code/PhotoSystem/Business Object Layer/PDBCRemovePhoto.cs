/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class PDBCRemovePhoto : PhotoDatabaseCommand
{
	public int questGiverID;
	public int activityOwnerID;
	public ApplicationID activityKey;
	public int langAreaID;
	public int diffIndexInLangArea;
	public int photoDiffPosition;

	public PDBCRemovePhoto(int para_questGiverID, int para_activityOwnerID, ApplicationID para_activityKey, int para_langAreaID, int para_diffIndexInLangArea, int para_photoDiffPosition)
	{
		questGiverID = para_questGiverID;
		activityOwnerID = para_activityOwnerID;
		activityKey = para_activityKey;
		langAreaID = para_langAreaID;
		diffIndexInLangArea = para_diffIndexInLangArea;
		photoDiffPosition = para_photoDiffPosition;
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using UnityEngine;

public class PhotoRecorder
{
	// The job of the PhotoRecorder is to create parameters for Photo objects.
	// Rendering of Photo objects can then be performed by PhotoVisualiser.

	public Photo buildNewPhotoData(int para_questGiverID,
	                                 				int para_activityOwnerID,
	                                 				ApplicationID para_activityKey,
	                                 				int para_langAreaID,
	                                 				int para_difficultyIndex)
	{
		// The photo will fall under the quest giver's album.
		GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
		//IGBDifficultyReference diffRefMat = gbMang.getDifficultyReferenceMaterial();


		// Extract player avatar details.
		GameObject poRef = PersistentObjMang.getInstance();
		DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
		PlayerAvatarSettings playerAvSettings = (PlayerAvatarSettings) ds.getData("PlayerAvatarSettings");

		// Add background randomisation when assets become available.
		int backgroundID = Random.Range(0,2);

		// Get difficulty name.
		string diffName = gbMang.createDifficultyShortDescription(para_langAreaID,para_difficultyIndex);
		if(diffName == null) { diffName = "N/A"; }


		Photo nwPhoto = new Photo(para_activityKey,
		                          				  backgroundID,
		                          				  new PhotoCharacterElement(para_questGiverID,Random.Range(1,4),null),
		                                          new PhotoCharacterElement(para_activityOwnerID,Random.Range(1,4),null),
		                                          playerAvSettings,
		                          				  Random.Range(1,4),
		                                          diffName,
		                                          null);

		return nwPhoto;
	}
}

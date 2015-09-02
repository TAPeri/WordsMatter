/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

[System.Serializable]
public class Photo
{
	// Backgrounds are grouped by Type: Eg. backgrounds representing the Junkyard activity, the Harvest activity etc.
	// This number must correspond with the Activity PKey as defined in the GB database table files. (See Resources Folder).
	// Special Note: Negative IDs are considered whacky photos. There is a special set of backgrounds for these.
	int photoAreaTypeID;

	// A specific background ID refers to a particular index within a particular background type.
	int photoAreaBackgroundID;

	// Character Info.
	PhotoCharacterElement questGiverElement;
	PhotoCharacterElement activityOwnerElement;

	// Player avatar settings for the photo.
	PlayerAvatarSettings playerAvSettings;
	int playerAvPoseID;

	// The specific text containing the difficulty name.
	string boardText;

	// The normalised bounds of the text board.
	float[] boardNormBounds;

	string dateNTimeStamp;



	/*public Photo(ActivityID para_photoAreaTypeID,
	             int para_photoAreaBackgroundID,
	             PhotoCharacterElement para_questGiverElement,
	             PhotoCharacterElement para_activityOwnerElement,
	             PlayerAvatarSettings para_playerAvSettings,
	             int para_playerAvPoseID,
	             string para_boardText,
	             float[] para_boardNormBounds)
	{
		
		photoAreaTypeID = (int)para_photoAreaTypeID;

		
		photoAreaBackgroundID = para_photoAreaBackgroundID;
		questGiverElement = para_questGiverElement;
		activityOwnerElement = para_activityOwnerElement;
		playerAvSettings = para_playerAvSettings;
		playerAvPoseID = para_playerAvPoseID;
		boardText = para_boardText;
		boardNormBounds = para_boardNormBounds;
		
		dateNTimeStamp = ""+System.DateTime.Now;//+" "+System.DateTime.Now.TimeOfDay;
	}*/




	
	public Photo(ApplicationID para_photoAreaTypeID,
	             int para_photoAreaBackgroundID,
	             PhotoCharacterElement para_questGiverElement,
	             PhotoCharacterElement para_activityOwnerElement,
	             PlayerAvatarSettings para_playerAvSettings,
	             int para_playerAvPoseID,
	             string para_boardText,
	             float[] para_boardNormBounds)
	{


		//ApplicationID is converted to backgroundIDs for consistency with old save files
		switch(para_photoAreaTypeID){
			case ApplicationID.SERENADE_HERO: photoAreaTypeID=0;break;
			case ApplicationID.DROP_CHOPS: photoAreaTypeID=1;break;
			case ApplicationID.MOVING_PATHWAYS: photoAreaTypeID=2;break;
			case ApplicationID.HARVEST: photoAreaTypeID=3;break;
			case ApplicationID.WHAK_A_MOLE: photoAreaTypeID=4;break;
			case ApplicationID.MAIL_SORTER: photoAreaTypeID=5;break;
			case ApplicationID.EYE_EXAM: photoAreaTypeID=6;break;
			case ApplicationID.TRAIN_DISPATCHER: photoAreaTypeID=7;break;
			case ApplicationID.ENDLESS_RUNNER: photoAreaTypeID=8;break;
			default: photoAreaTypeID=-1;break;
		}

		photoAreaBackgroundID = para_photoAreaBackgroundID;
		questGiverElement = para_questGiverElement;
		activityOwnerElement = para_activityOwnerElement;
		playerAvSettings = para_playerAvSettings;
		playerAvPoseID = para_playerAvPoseID;
		boardText = para_boardText;
		boardNormBounds = para_boardNormBounds;

		dateNTimeStamp = ""+System.DateTime.Now;//+" "+System.DateTime.Now.TimeOfDay;
	}

	public int getPhotoAreaTypeID() { return photoAreaTypeID; }
	public int getPhotoAreaBackgroundID() { return photoAreaBackgroundID; }
	public PhotoCharacterElement getQuestGiverInfo() { return questGiverElement; }
	public PhotoCharacterElement getActivityOwnerInfo() { return activityOwnerElement; }
	public PlayerAvatarSettings getPlayerAvatarInfo() { return playerAvSettings; }
	public int getPlayerAvatarPoseID() { return playerAvPoseID; }
	public string getBoardText() { return boardText; }
	public float[] getBoardNormBounds() { return boardNormBounds; }
	public string getDateTimeStampStr() { return dateNTimeStamp; }
}
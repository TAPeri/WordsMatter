/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
[System.Serializable]
public class PhotoCharacterElement
{
	int characterID;
	int poseID;
	float[] normSpawnPhotoCoords;

	public PhotoCharacterElement(int para_characterID, int para_poseID, float[] para_normSpawnPhotoCoords)
	{
		characterID = para_characterID;
		poseID = para_poseID;
		normSpawnPhotoCoords = para_normSpawnPhotoCoords;
	}

	public int getCharacterID() { return characterID; }
	public int getPoseID() { return poseID; }
	public float[] getNormSpawnPhotoCoords() { return normSpawnPhotoCoords; }
}
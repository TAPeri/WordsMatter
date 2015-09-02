/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

[System.Serializable]
public class ContactSlot
{
	int characterID;
	CharacterStatus status;
	int numBioSectionsUnlocked;

	PhotoAlbum album;
	
	// If available, the enc identifies the current language, difficulty and other parameters associated with this character at a specific moment in time.
	// An enc can be seen as an errand configuration for that character which may be presented to the player.
	// Character encs are updated depending on specific available errands.
	Encounter enc;


	public ContactSlot()
	{
		// Empty constructor for serialisers.
	}

	public ContactSlot(int para_charID,
	                   CharacterStatus para_status,
	                   int para_numBioSectionsUnlocked,
	                   List<DifficultyMetaData> para_associatedDifficulties)
	{
		characterID = para_charID;
		status = para_status;
		numBioSectionsUnlocked = para_numBioSectionsUnlocked;
		album= new PhotoAlbum(para_charID,para_associatedDifficulties);
		enc = null;
	}

	public int getCharacterID() { return characterID; }
	public CharacterStatus getStatus() { return status; }
	public void setStatus(CharacterStatus para_status) { status = para_status; }
	public int getNumOfUnlockedBioSections() { return numBioSectionsUnlocked; }
	public void setUnlockedBioSections(int para_numBioSections) { numBioSectionsUnlocked = para_numBioSections; if(numBioSectionsUnlocked < 0) { numBioSectionsUnlocked = 0; } }

	public void unlockBioSection()
	{
		numBioSectionsUnlocked++;
	}

	public void setEncounterForCharacter(Encounter para_enc) { enc = para_enc; }
	public Encounter getAvailableCharacterEncounter() { return enc; }

	public PhotoAlbum getPhotoAlbum() { return album; }
	public void setPhotoAlbum(PhotoAlbum aux) { album = aux; }
}
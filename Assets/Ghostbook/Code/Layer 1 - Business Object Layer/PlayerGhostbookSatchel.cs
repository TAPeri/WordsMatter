/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

[System.Serializable]
public class PlayerGhostbookSatchel
{
	ContactList contacts;
	EventsList events;
	NewsfeedList newsfeed;

	bool encounterRebootNotNeeded;

	public bool rebootNeeded(){
		return !encounterRebootNotNeeded;
	}

	public void encounterRebooted(){
		encounterRebootNotNeeded = true;
	}

	public PlayerGhostbookSatchel()
	{
		// For serialisers only.
	}

	// Only to be used on the first time.
	public PlayerGhostbookSatchel(List<int> para_allCharacterIDs,
	                              Dictionary<int,List<DifficultyMetaData>> para_npcToDiffDataList)
	{
		contacts = new ContactList(para_allCharacterIDs,para_npcToDiffDataList);
		events = new EventsList(true);
		newsfeed = new NewsfeedList(true);
	}

	public ContactList getContactList() { return contacts; }
	public EventsList getEventList() { return events; }
	public NewsfeedList getNewsfeedList() { return newsfeed; }

	public void setContactList(ContactList para_cl) { contacts = para_cl; }
	public void setEventList(EventsList para_el) { events = para_el; }
	public void setNewsfeedList(NewsfeedList para_nl) { newsfeed = para_nl; }

	public void unlockAllCharacters() { contacts.setAllToStatus(CharacterStatus.UNLOCKED); }
}
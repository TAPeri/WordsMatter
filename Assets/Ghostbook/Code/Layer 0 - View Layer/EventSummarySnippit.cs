/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class EventSummarySnippit
{
	EventSlot eventInfo;
	string eventText;
	string readableLangArea;
	string readableDifficulty;

	public EventSummarySnippit(EventSlot para_eventSlot,
	                           string para_eventText,
	                           string para_readableLangArea,
	                           string para_readableDifficulty)
	{
		eventInfo = para_eventSlot;
		eventText = para_eventText;
		readableLangArea = para_readableLangArea;
		readableDifficulty = para_readableDifficulty;
	}

	public int getQuestID() { return eventInfo.getQuestID(); }
	public int getQuestGiverCharID() { return eventInfo.getQuestGiverCharID(); }
	public ApplicationID getApplicationID() { return eventInfo.getApplicationID(); }
	public string getEventText() { return eventText; }
	public string getReadableLA() { return readableLangArea; }
	public string getReadableDiff() { return readableDifficulty; }
	public Encounter getRelatedEncData() { return eventInfo.getEncounter(); }
}

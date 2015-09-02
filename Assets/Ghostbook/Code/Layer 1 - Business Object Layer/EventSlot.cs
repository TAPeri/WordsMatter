/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
[System.Serializable]
public class EventSlot
{
	// These attributes are needed for the quest bar.
	int questID;
	int questGiverCharID;
	int questReceiverCharID;
	ActivityID acID;//depreacted

	// This holds extra info and used as needed.
	Encounter enc;

	public EventSlot()
	{
		// For serialisers only.
	}


	/*public EventSlot(int para_questID,
	                 int para_questGiverCharID,
	                 ActivityID para_acID,
	                 Encounter para_enc)
	{
		questID = para_questID;
		questGiverCharID = para_questGiverCharID;
		acID = para_acID;
		enc = para_enc;
	}*/

	public EventSlot(int para_questID,
	                 int para_questGiverCharID,
	                 int para_questReceiverCharID,

	                 ApplicationID para_acID,
	                 Encounter para_enc)
	{
		questID = para_questID;
		questGiverCharID = para_questGiverCharID;
		questReceiverCharID = para_questReceiverCharID;
		//acID = null;
		enc = para_enc;
	}

	public int getQuestID() { return questID; }
	public int getQuestGiverCharID() { return questGiverCharID; }
	public int getQuestReceiverCharID() { return questReceiverCharID; }
	public ApplicationID getApplicationID() { return enc.getLocation(); }
//	public ActivityID getActivityID() { return acID; }
	public Encounter getEncounter() { return enc; }
}

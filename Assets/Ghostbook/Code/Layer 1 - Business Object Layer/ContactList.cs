/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

[System.Serializable]
public class ContactList
{
	public Dictionary<int,ContactSlot> charIDToSlotData;
	List<int> orderedList;


	public ContactList()
	{
		// Used for serialisers only.
	}

	// This constructor is to be used only on the first time.
	public ContactList(List<int> para_allCharacterIDs,
	                   Dictionary<int,List<DifficultyMetaData>> para_npcToDiffDataList)
	{
		charIDToSlotData = new Dictionary<int, ContactSlot>();
		orderedList = new List<int>();

		if(para_allCharacterIDs != null)
		{
			for(int i=0; i<para_allCharacterIDs.Count; i++)
			{
				int tmpCharID = para_allCharacterIDs[i];
				orderedList.Add(tmpCharID);

				List<DifficultyMetaData> associatedDifficultiesForNpc = new List<DifficultyMetaData>();
				if(para_npcToDiffDataList != null)
				{
					if(para_npcToDiffDataList.ContainsKey(tmpCharID))
					{
						associatedDifficultiesForNpc = para_npcToDiffDataList[tmpCharID];
					}
				}

				charIDToSlotData.Add(tmpCharID,new ContactSlot(tmpCharID,CharacterStatus.LOCKED,0,associatedDifficultiesForNpc));
			}
		}
	}


	public void updateCharStatus(int para_charID, CharacterStatus para_nwStatus)
	{
		if(charIDToSlotData != null)
		{
			if(charIDToSlotData.ContainsKey(para_charID))
			{
				ContactSlot tmpSlot = charIDToSlotData[para_charID];
				tmpSlot.setStatus(para_nwStatus);
			}
		}
	}

	public void setAllToStatus(CharacterStatus para_status)
	{
		if((orderedList != null)&&(charIDToSlotData != null))
		{
			foreach(KeyValuePair<int,ContactSlot> pair in charIDToSlotData)
			{
				pair.Value.setStatus(para_status);
			}
		}
	}

	public void unlockBioSection(int para_charID)
	{
		if(charIDToSlotData != null)
		{
			if(charIDToSlotData.ContainsKey(para_charID))
			{
				ContactSlot tmpSlot = charIDToSlotData[para_charID];
				tmpSlot.unlockBioSection();
			}
		}
	}

	public List<ContactSlot> getAllSlotsInOrder()
	{
		List<ContactSlot> retList = new List<ContactSlot>();

		if((orderedList != null)&&(charIDToSlotData != null))
		{
			for(int i=0; i<orderedList.Count; i++)
			{
				int tmpID = orderedList[i];
				if(charIDToSlotData.ContainsKey(tmpID))
				{
					retList.Add(charIDToSlotData[tmpID]);
				}
			}
		}

		return retList;
	}

	public Dictionary<CharacterStatus,List<int>> getContactsByStatus()
	{
		Dictionary<CharacterStatus,List<int>> retData = new Dictionary<CharacterStatus, List<int>>();

		if(charIDToSlotData != null)
		{
			foreach(KeyValuePair<int,ContactSlot> pair in charIDToSlotData)
			{
				CharacterStatus tmpStatus = pair.Value.getStatus();
				if( ! retData.ContainsKey(tmpStatus))
				{
					retData.Add(tmpStatus,new List<int>());
				}
				retData[tmpStatus].Add(pair.Key);
			}
		}
		
		return retData;
	}
	
	public List<int> getContactsByStatus(CharacterStatus para_status)
	{
		List<int> retList = new List<int>();

		if(charIDToSlotData != null)
		{
			foreach(KeyValuePair<int,ContactSlot> pair in charIDToSlotData)
			{
				if(pair.Value.getStatus() == para_status)
				{
					retList.Add(pair.Key);
				}
			}
		}
		
		return retList;
	}

	public ContactSlot getContactByCharID(int para_charID)
	{
		ContactSlot reqSlot = null;

		if(charIDToSlotData != null)
		{
			if(charIDToSlotData.ContainsKey(para_charID))
			{
				reqSlot = charIDToSlotData[para_charID];
			}
		}

		return reqSlot;
	}


}
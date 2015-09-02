/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

[System.Serializable]
public class EventsList
{
	int nxtAvailableID;
	List<EventSlot> availableEvents;

	public EventsList()
	{
		// For serialisers only.
	}

	public EventsList(bool para_dummyFlag)
	{
		nxtAvailableID = 0;
		availableEvents = new List<EventSlot>(); 
	}


	public bool isEmpty()
	{
		bool retFlag = true;

		if(availableEvents != null)
		{
			if(availableEvents.Count > 0)
			{
				retFlag = false;
			}
		}

		return retFlag;
	}

	public bool isFull()
	{
		bool retFlag = true;

		if(availableEvents != null)
		{
			if(availableEvents.Count < 4)
			{
				retFlag = false;
			}
		}

		return retFlag;
	}

	public void addEvent(int para_questGiverCharID,
	                     int para_questReceiverCharID,
	                     ApplicationID para_activityID,
	                     Encounter para_encFullData)
	{
		//UnityEngine.Debug.LogWarning("ActivityID deprecated");
		if(availableEvents == null) { 
			availableEvents = new List<EventSlot>(); }
		availableEvents.Add(new EventSlot(nxtAvailableID,para_questGiverCharID,para_questReceiverCharID,para_activityID,para_encFullData));
		nxtAvailableID++;
	}

	public void removeEventAtIndex(int para_index)
	{
		if(availableEvents != null)
		{
			if((para_index >= 0)&&(para_index < availableEvents.Count))
			{
				availableEvents.RemoveAt(para_index);
			}
		}
	}

	public void reset(){
		availableEvents = new List<EventSlot>();

		nxtAvailableID = 0;
	}

	
	public EventSlot getEvent(int para_eventSlotIndex)
	{
		EventSlot reqSlot = null;
		if((availableEvents != null)&&(para_eventSlotIndex > 0)&&(para_eventSlotIndex < availableEvents.Count))
		{
			reqSlot = availableEvents[para_eventSlotIndex];
		}
		return reqSlot;
	}

	public List<EventSlot> getAllEventSlots()
	{
		return availableEvents;
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public interface IEventsServices
{
	List<EventSummarySnippit> getAvailableEvents();
	void addEvent(int para_questGiverID, int para_questReceiverID,ApplicationID para_acID, Encounter para_encFullData);
	void clearList();
}

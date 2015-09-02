/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public interface IActionNotifier
{
	void unregisterListener(string para_listenerName);
	void registerListener(string para_listenerName, CustomActionListener para_listener);
	void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData);	
}

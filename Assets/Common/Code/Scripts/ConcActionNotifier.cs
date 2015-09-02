/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using System.Collections.Generic;

public class ConcActionNotifier : IActionNotifier
{
	Dictionary<string,CustomActionListener> listenerMap;
	HashSet<string> nextTurn_keysToRemove = null;
	Dictionary<string,CustomActionListener> nextTurn_keysToAdd = null;


	// Action Notifier Methods.
	public void registerListener(string para_name, CustomActionListener para_listener)
	{		
		if(nextTurn_keysToAdd == null) { nextTurn_keysToAdd = new Dictionary<string, CustomActionListener>(); }
		if( ! nextTurn_keysToAdd.ContainsKey(para_name))
		{
			nextTurn_keysToAdd.Add(para_name,para_listener);
		}
	}
	
	public void unregisterListener(string para_name)
	{
		if(nextTurn_keysToRemove == null) { nextTurn_keysToRemove = new HashSet<string>(); }
		if( ! nextTurn_keysToRemove.Contains(para_name))
		{
			nextTurn_keysToRemove.Add(para_name);
		}
	}
	
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(listenerMap == null) { listenerMap = new Dictionary<string, CustomActionListener>(); }

		if(listenerMap != null)
		{
			// Prevents desynch if some script unregisters a listener whilst notifyAllListeners is still happening.


			if(nextTurn_keysToRemove != null)
			{
				foreach(string tmpKey in nextTurn_keysToRemove)
				{
					if(listenerMap.ContainsKey(tmpKey))
					{
						listenerMap.Remove(tmpKey);
					}
				}
				nextTurn_keysToRemove.Clear();
			}

			if(nextTurn_keysToAdd != null)
			{
				foreach(KeyValuePair<string,CustomActionListener> pair in nextTurn_keysToAdd)
				{
					if( ! listenerMap.ContainsKey(pair.Key))
					{
						listenerMap.Add(pair.Key,pair.Value);
					}
					else
					{
						listenerMap[pair.Key] = pair.Value;
					}
				}
				nextTurn_keysToAdd.Clear();
			}


			// Actually perform the notifications.

			foreach(KeyValuePair<string,CustomActionListener> pair in listenerMap)
			{
				pair.Value.respondToEvent(para_sourceID,para_eventID,para_eventData);
			}
		}
	}
}

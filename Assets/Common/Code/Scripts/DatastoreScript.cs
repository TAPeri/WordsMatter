/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

using UnityEngine;
using System.Collections.Generic;

public class DatastoreScript : MonoBehaviour
{
	Dictionary<string,System.Object> store = new Dictionary<string, object>();


	public bool containsData(string para_key)
	{
		return store.ContainsKey(para_key);
	}

	public System.Object getData(string para_key)
	{
		if(store == null) { return null; }
		else
		{
			if(store.ContainsKey(para_key))
			{
				return store[para_key];
			}
			else
			{
				return null;
			}
		}
	}

	public void insertData(string para_key, System.Object para_data)
	{
		if(store == null) { store = new Dictionary<string, System.Object>(); }

		if( ! store.ContainsKey(para_key))
		{
			store.Add(para_key,para_data);
		}
		else
		{
			store[para_key] = para_data;
		}
	}

	public void removeData(string para_key)
	{
		if(store != null)
		{
			if(store.ContainsKey(para_key))
			{
				store.Remove(para_key);
			}
		}
	}
}

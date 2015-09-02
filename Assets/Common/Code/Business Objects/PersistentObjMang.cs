/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

using UnityEngine;

public class PersistentObjMang
{
	public static GameObject instance = null;

	private PersistentObjMang() { }

	public static void createInstance()
	{
		GameObject tmpObj = GameObject.Find("PersistentObj");
		if(tmpObj != null)
		{
			instance = tmpObj;
		}
		else
		{	
			// Basic object shell.
			instance = new GameObject("PersistentObj");
			instance.transform.position = new Vector3(0,0,0);

			// Persistent object components.
			// (Other components may be attached from within other scripts once you obtain an instance via getInstance())
			//instance.AddComponent<ServerComRequester>();
			instance.AddComponent<SoundPlayerScript>();
			DatastoreScript ds = instance.AddComponent<DatastoreScript>();

			// Store versioning info in memory. (Primarily needed by the saving system to detect old or new save data).
			int dayValue = 6;//23;
			int monthValue = 1;//10;
			int yearValue = 2015;//2014;
			int versionForDay = 0;

			ds.insertData("GameVersionData",new SaveVersioningInfo(dayValue,monthValue,yearValue,versionForDay,""));

			// Make sure that the object is not destroyed when new scenes are loaded.
			UnityEngine.MonoBehaviour.DontDestroyOnLoad(instance);
		}
	}

	public static GameObject getInstance()
	{
		if(instance == null)
		{
			createInstance();
		}
		return instance;
	}	
}

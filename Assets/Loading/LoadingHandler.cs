/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class LoadingHandler : MonoBehaviour, CustomActionListener
{

	float initialWait_Sec = 3;
	float postWait_Sec = 4;
	int state = 0;

	void Start()
	{
		// Spawn the loading window.
		Transform loadingWindowPrefab = Resources.Load<Transform>("Prefabs/LoadingWindow");
		Rect origPrefab2DBounds = CommonUnityUtils.get2DBounds(loadingWindowPrefab.FindChild("WindowBounds").renderer.bounds);
		GameObject nwLoadingWindow = WorldSpawnHelper.initWorldObjAndBlowupToScreen(loadingWindowPrefab,origPrefab2DBounds);
		nwLoadingWindow.transform.position = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,Camera.main.transform.position.z + 3f);
		//LoadingWindow lwScript = nwLoadingWindow.AddComponent<LoadingWindow>();
		nwLoadingWindow.AddComponent<LoadingWindow>();


		performInitialWait();
	}
	


	// 3 sec.
	private void performInitialWait()
	{
		Debug.Log("Performing Initial Wait");
		DelayForInterval dfi = transform.gameObject.AddComponent<DelayForInterval>();
		dfi.registerListener("LoadingHandler",this);
		dfi.init(initialWait_Sec);
	}

	// 3 sec.
	private void performUnloadProcedure()
	{
		Debug.Log("Performing Unload Procedure");

		// Unity based method to deallocate any unused loaded assets.
		Resources.UnloadUnusedAssets();

		// .NET based method to force garbage collection. (Not guaranteed to work fully on all OS).
		System.GC.Collect();

		performPostWait();
	}

	// 3 sec.
	private void performPostWait()
	{
		Debug.Log("Performing Post Wait");

		DelayForInterval dfi = transform.gameObject.AddComponent<DelayForInterval>();
		dfi.registerListener("LoadingHandler",this);
		dfi.init(postWait_Sec);
	}

	private void performLoadNextScene()
	{
		Debug.Log("About to load next scene");

		GameObject poRef = PersistentObjMang.getInstance();
		DatastoreScript ds = poRef.GetComponent<DatastoreScript>();

		if(ds.containsData("NextSceneToLoad"))
		{
			string nextSceneName = (string) ds.getData("NextSceneToLoad");
			ds.removeData("NextSceneToLoad");

			Debug.Log("Loading Next Scene: "+nextSceneName);
			Application.LoadLevel(nextSceneName);
		}
		else
		{
			// Missing Scene Info!
			Debug.LogError("Missing next scene name");
		}
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "DelayEnd")
		{
			if(state == 0)
			{
				state++;
				performUnloadProcedure();
			}
			else
			{
				performLoadNextScene();
			}
		}
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class IntroNarrativeScenario : MonoBehaviour, CustomActionListener
{

	void Start()
	{
		Transform introNarrativeWindowPrefab = Resources.Load<Transform>("Prefabs/IntroNarrativeWindow");
		Rect origPrefab2DBounds = CommonUnityUtils.get2DBounds(introNarrativeWindowPrefab.FindChild("WindowBounds").renderer.bounds);
		GameObject nwIntroNarrativeWindow = WorldSpawnHelper.initWorldObjAndBlowupToScreen(introNarrativeWindowPrefab,origPrefab2DBounds);
		nwIntroNarrativeWindow.transform.position = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,Camera.main.transform.position.z + 3f);
		
		//string langCodeAsStr = System.Enum.GetName(typeof(LanguageCode),LocalisationMang.langCode);
		
		IntroNarrativeWindow inwScript = nwIntroNarrativeWindow.AddComponent<IntroNarrativeWindow>();
		inwScript.registerListener("AcScen",this);
		inwScript.init("IntroNarrativeWindow",true);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_sourceID == "IntroNarrativeWindow")
		{
			if(para_eventID == "AllDone")
			{
				// Load the world view scene.

				GameObject poRef = PersistentObjMang.getInstance();
				DatastoreScript ds = poRef.GetComponent<DatastoreScript>();

				string nxtSceneName = "WorldView";
				if(ds.containsData("NextSceneName"))
				{
					string tmpSceneName = (string) ds.getData("NextSceneName");
					if(tmpSceneName != null)
					{
						tmpSceneName = tmpSceneName.Trim();
						if(tmpSceneName != "")
						{
							nxtSceneName = tmpSceneName;
						}
					}
					ds.removeData("NextSceneName");
				}
				
				ds.insertData("NextSceneToLoad",nxtSceneName);
				Application.LoadLevel("LoadingScene");
			}
		}
	}
}
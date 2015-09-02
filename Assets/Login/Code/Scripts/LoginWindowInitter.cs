/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using UnityEngine;
using System.Collections;

public class LoginWindowInitter : MonoBehaviour
{


	void Start()
	{
		Transform loginWindowPrefab = Resources.Load<Transform>("Prefabs/LoginWindow");
		Rect origPrefab2DBounds = CommonUnityUtils.get2DBounds(loginWindowPrefab.FindChild("WindowPane").renderer.bounds);
		GameObject nwLoginWindow = WorldSpawnHelper.initWorldObjAndBlowupToScreen(loginWindowPrefab,origPrefab2DBounds,new Rect(0,0,1,1));
		nwLoginWindow.transform.position = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,Camera.main.transform.position.z + 3f);
		nwLoginWindow.AddComponent<LoginHandlerV2>();
		Destroy(this);
	}
}

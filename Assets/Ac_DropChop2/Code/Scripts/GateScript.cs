/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class GateScript : MonoBehaviour, CustomActionListener, IActionNotifier
{

	bool firstTime = true;
	Vector3[] closePosArr;
	Vector3[] openPosArr;

	int actionsHeard;


	public void openGate() { toggleGate(true); }
	public void closeGate() { toggleGate(false); }

	public void toggleGate(bool para_orderGateOpen)
	{
		actionsHeard = 0;

		GameObject leftGate = GameObject.Find("PitLeft");
		GameObject rightGate = GameObject.Find("PitRight");
		List<GameObject> gateObjs = new List<GameObject>() { leftGate, rightGate };
		if(firstTime) { calculateOpenAndClosePositions(gateObjs); }


		for(int i=0; i<gateObjs.Count; i++)
		{
			GameObject gate = gateObjs[i];
			Vector3 reqDestPos;
			string aniName = "GateOpenAni";
			if(para_orderGateOpen) { reqDestPos = openPosArr[i]; aniName = "GateOpenAni"; }
			else { reqDestPos = closePosArr[i]; aniName = "GateCloseAni"; }

			CustomAnimationManager aniMang = gate.AddComponent<CustomAnimationManager>();
			List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
			List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
			batch1.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3] {reqDestPos.x,reqDestPos.y,reqDestPos.z}, 0.5f, true }));
			batchLists.Add(batch1);
			aniMang.registerListener("GateScript",this);
			aniMang.init(aniName,batchLists);
		}
	}




	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if((para_eventID == "GateOpenAni")||(para_eventID == "GateCloseAni"))
		{
			actionsHeard++;

			if(actionsHeard >= 2)
			{
				actionsHeard = 0;
				notifyAllListeners(para_sourceID,para_eventID,para_eventData);
			}
		}
	}

	private void calculateOpenAndClosePositions(List<GameObject> para_gateSections)
	{
		bool[] upAxisArr = new bool[3] {false,true,false};
		
		Vector2 screenCentrePoint = new Vector2(Screen.width/2f,Screen.height/2f);
		Rect screenCentreBounds = new Rect(screenCentrePoint.x - 2,screenCentrePoint.y - 2, 4, 4);
		Rect centreWorldBounds = WorldSpawnHelper.getGuiToWorldBounds(screenCentreBounds,1,upAxisArr);


		openPosArr = new Vector3[para_gateSections.Count];
		closePosArr = new Vector3[para_gateSections.Count];

		for(int i=0; i<para_gateSections.Count; i++)
		{
			Transform tmpObj = para_gateSections[i].transform;

			Vector3 openPos = new Vector3(tmpObj.position.x,tmpObj.position.y,tmpObj.position.z);


			float xVal = 0;
			Vector3 centreWorldPt = new Vector3(centreWorldBounds.x + (centreWorldBounds.width/2f),
			                                    centreWorldBounds.y - (centreWorldBounds.height/2f),
			                                    tmpObj.position.z);


			if(centreWorldPt.x >= tmpObj.position.x) { xVal = centreWorldPt.x - (tmpObj.renderer.bounds.size.x/2f); }
			else { xVal = centreWorldPt.x + (tmpObj.renderer.bounds.size.x/2f); }


			Vector3 closePos = new Vector3(xVal, tmpObj.position.y, tmpObj.position.z);

			openPosArr[i] = openPos;
			closePosArr[i] = closePos;
		}

		firstTime = false;
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

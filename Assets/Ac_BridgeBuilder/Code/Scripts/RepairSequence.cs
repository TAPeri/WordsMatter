/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class RepairSequence : MonoBehaviour, CustomActionListener, IActionNotifier
{
	Rect maxGameArea;
	Vector3 engiStartPos;
	Vector3 engiEndPos;

	Transform clampPrefab;
	int[] correctPosArr;


	bool last;

	public void init(Rect para_maxGameArea,
	                 Vector3 para_engiStartPos,
	                 Vector3 para_engiEndPos,
	                 Transform para_clampPrefab,
	                 int[] para_correctPosArr,
	                 bool last)
	{

		this.last = last;
		maxGameArea = para_maxGameArea;
		engiStartPos = para_engiStartPos;
		engiEndPos = para_engiEndPos;
		clampPrefab = para_clampPrefab;
		correctPosArr = para_correctPosArr;

		// STEP 1: Move camera to engineer location. (Cam is bounded to game area).

		Rect camWorld2DBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,false);
		Vector3 currCamPos = Camera.main.transform.position;
		Vector3 camMoveToLoc = new Vector3(currCamPos.x,currCamPos.y,currCamPos.z);
		camMoveToLoc.x = (maxGameArea.x + camWorld2DBounds.width/2f);


		if(Mathf.Abs(currCamPos.x - camMoveToLoc.x) != 0)
		{
			//MoveToLocation mtl = Camera.main.transform.gameObject.AddComponent<MoveToLocation>();
			//mtl.registerListener("RepairSequence",this);
			//mtl.initScript(camMoveToLoc,5f);
			TeleportToLocation ttl = Camera.main.transform.gameObject.AddComponent<TeleportToLocation>();
			ttl.registerListener("RepairSequence",this);
			ttl.init(camMoveToLoc);
		}
		else
		{
			this.respondToEvent(Camera.main.name,"MoveToLocation",null);
		}
	}


	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_sourceID == Camera.main.name)
		{
			if((para_eventID == "MoveToLocation")||(para_eventID == "TeleportToLocation"))
			{
				// STEP 2: Trigger engineer to start repairs.
				//Debug.Log("Camera has moved to location");

				GameObject engi = GameObject.Find("Engineer");
				
				BoundedCamFollow bcf = Camera.main.gameObject.AddComponent<BoundedCamFollow>();
				bcf.init(engi,new bool[3] { true, false, false }, maxGameArea);
				
				EngiScript engiScrp = engi.GetComponent<EngiScript>();
				engiScrp.registerListener("RepairSequence",this);
				engiScrp.init(engiStartPos,engiEndPos,clampPrefab,correctPosArr,this.last);
			}
		}
		else if(para_sourceID == "Engineer")
		{


			if(para_eventID == "EngiAtRightCliff")
			{

				DestroyImmediate(Camera.main.gameObject.GetComponent<BoundedCamFollow>());
				notifyAllListeners("Repair","EngiAtRightCliff",null);
				this.unregisterListener("AcScen");
				Debug.Log("Repair received from the engineer "+para_eventID+". I should die now");

				DestroyImmediate(this);
			}else if(para_eventID == "EngiAtLeftCliff")
			{
				DestroyImmediate(Camera.main.gameObject.GetComponent<BoundedCamFollow>());
				notifyAllListeners("Repair","EngiAtLeftCliff",null);
				this.unregisterListener("AcScen");

				Debug.Log("Repair received from the engineer "+para_eventID+". I should die now");

				DestroyImmediate(this);
			}
		}
	}



	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class EngiScript : MonoBehaviour, CustomActionListener, IActionNotifier
{
	Vector3 leftCliffPos;
	Vector3 rightCliffPos;
	float engiWalkSpeed = 10f;//6f;
	//float repairTimePerSupport_Sec = 1f;
	//int state = 0;

	Transform clampPrefab;

	int[] repairLocations;
	int jobIndex;

	bool currRepairJobUnnecessary;

	List<GameObject> relevantRepairItems;

	int[] correctPosArr;


	bool finishRight = false;
	
	public void init(Vector3 para_engiStartPt,
	                 Vector3 para_engiDestPt,
	                 Transform para_clampPrefab,
	                 int[] para_correctPosArr,
	                 bool finishRight)
	{

		this.finishRight = finishRight;
		leftCliffPos = para_engiStartPt;
		rightCliffPos = para_engiDestPt;

		clampPrefab = para_clampPrefab;
		correctPosArr = para_correctPosArr;

		HighlightInputScript his = GameObject.Find("GlobObj").GetComponent<HighlightInputScript>();
		repairLocations = his.getHighlightLocations();
		jobIndex = -1;


		if(repairLocations.Length>0)
		{
			moveToNextRepairSite();
		}
		else
		{
			if(finishRight)
				moveToRightCliff();
			else
				moveToLeftCliff();

		}


	}
	

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{


		//Debug.Log();
		if(para_eventID == "MoveToNextRepairSite")
		{
			// Trigger repair.
			repairCurrentBridgeSegment();
		}
		else if(para_eventID == "SupportRepairProcedure")
		{
			// Trigger end of repair effect.
			applyRepairFinishedEffect();
		}
		else if(para_eventID == "SupportRepairEndEffect")
		{
			// If more supports to repair. Move to the next support.
			if(jobIndex < (repairLocations.Length-1))
			{
				moveToNextRepairSite();
			}
			else
			{
				if(finishRight)
					moveToRightCliff();
				else
					moveToLeftCliff();
			}
		}
		else if(para_eventID == "MoveToRightCliff")
		{
			Animator characterAni = transform.gameObject.GetComponent<Animator>();
			characterAni.speed = 0;
			characterAni.Play("SideWalkL_Wrench",0,1);
			

			notifyAllListeners("Engineer","EngiAtRightCliff",null);				
		}
		else if(para_eventID == "MoveToLeftCliff")
		{
			Animator characterAni = transform.gameObject.GetComponent<Animator>();
			characterAni.speed = 0;
			characterAni.Play("SideWalkR_Wrench",0,1);
			
			
			//Vector3 tmpAngles = transform.localEulerAngles;
			//tmpAngles.y += 180;
			//transform.localEulerAngles = tmpAngles;
			
			notifyAllListeners("Engineer","EngiAtLeftCliff",null);				
		}
		else if(para_eventID == "MoveToNewLevel")
		{
			Animator characterAni = transform.gameObject.GetComponent<Animator>();
			//characterAni.speed = 0;
			characterAni.Play("Idle",0,1);

			notifyAllListeners("Engineer","MoveToNewLevel",null);
		}

	}


	private void moveToNextRepairSite()
	{
		jobIndex++;

		int nxtLetterIndex = repairLocations[jobIndex];

		Transform aux =  GameObject.Find("Bridge").transform.FindChild("BridgeSurface").transform.FindChild("Tile*"+nxtLetterIndex);

		if (aux==null)
			aux = GameObject.Find("ErrorOverlay").transform.FindChild("ErrorTile"+nxtLetterIndex);

		GameObject reqLetterTile = aux.gameObject;
		Vector3 midPt = new Vector3(reqLetterTile.renderer.bounds.center.x,transform.position.y,transform.position.z);

		currRepairJobUnnecessary = (! arrContainsInt(correctPosArr,nxtLetterIndex));

		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",1,new List<System.Object>() { new float[3] {midPt.x,midPt.y,midPt.z}, engiWalkSpeed }));
		batchLists.Add(batch1);
		aniMang.registerListener("EngiScript",this);
		aniMang.init("MoveToNextRepairSite",batchLists);
		
		Animator characterAni = transform.gameObject.GetComponent<Animator>();
		characterAni.speed = 1;
		characterAni.Play("SideWalkR_Wrench");
	}

	private void repairCurrentBridgeSegment()
	{
		int nxtLetterIndex = repairLocations[jobIndex];
		GameObject bridgeObj = GameObject.Find("Bridge");
		//GameObject letterTile = bridgeObj.transform.FindChild("BridgeSurface").transform.FindChild("Tile*"+nxtLetterIndex).gameObject;
		GameObject supportCollectionForTile = bridgeObj.transform.Find("BridgeSupports").transform.FindChild("Support*"+nxtLetterIndex).gameObject;

		if(relevantRepairItems == null) { relevantRepairItems = new List<GameObject>(); }
		relevantRepairItems.Clear();

		for(int i=0; i<supportCollectionForTile.transform.childCount; i++)
		{
			Transform tmpChild = supportCollectionForTile.transform.GetChild(i);
			if((tmpChild.name == "MainPipe")||(tmpChild.name == "HexConnector"))
			{
				relevantRepairItems.Add(tmpChild.gameObject);
			}
		}


		string reqAniName = "Clamping";
		if( ! currRepairJobUnnecessary)
		{
			reqAniName = "Clamping";

			for(int i=0; i<relevantRepairItems.Count; i++)
			{
				GameObject tmpItem = relevantRepairItems[i];
				if(tmpItem.name == "MainPipe")
				{
					Rect mainPipeBounds = CommonUnityUtils.get2DBounds(tmpItem.renderer.bounds);
					Vector3 clampSpawnPt = new Vector3(mainPipeBounds.x + (mainPipeBounds.width/2f), mainPipeBounds.y - (clampPrefab.renderer.bounds.size.y/2f),tmpItem.transform.position.z - 0.2f);
					Transform nwClamp = (Transform) Instantiate(clampPrefab,clampSpawnPt,Quaternion.identity);
					nwClamp.parent = tmpItem.transform;
				}
			}
		}
		else
		{
			reqAniName = "Shrug";
		}

		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("TriggerAnimation",2, new List<System.Object>() { reqAniName ,5f}));//Animation sped up by 3
//		batch1.Add(new AniCommandPrep("TriggerAnimation",1, new List<System.Object>() { reqAniName }));
		batchLists.Add(batch1);
		aniMang.registerListener("EngiScript",this);
		aniMang.init("SupportRepairProcedure",batchLists);
	}

	private void applyRepairFinishedEffect()
	{

		relevantRepairItems.Clear();

		respondToEvent("EngiScript","SupportRepairEndEffect",null);
	}

	public void moveToRightCliff()
	{

		Debug.Log("Going right!");
		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",1,new List<System.Object>() { new float[3] {rightCliffPos.x,rightCliffPos.y,rightCliffPos.z}, engiWalkSpeed }));
		batchLists.Add(batch1);
		aniMang.registerListener("EngiScript",this);
		aniMang.init("MoveToRightCliff",batchLists);
		
		Animator characterAni = transform.gameObject.GetComponent<Animator>();
		characterAni.speed = 1;
		characterAni.Play("SideWalkR_Wrench");
	}

	public void moveToLeftCliff()
	{
		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",1,new List<System.Object>() { new float[3] {leftCliffPos.x,leftCliffPos.y,leftCliffPos.z}, engiWalkSpeed }));
		batchLists.Add(batch1);
		aniMang.registerListener("EngiScript",this);
		aniMang.init("MoveToLeftCliff",batchLists);
		
		Animator characterAni = transform.gameObject.GetComponent<Animator>();
		characterAni.speed = 1;
		characterAni.Play("SideWalkL_Wrench");
	}

	public void moveToNewLevel(int para_currLvLID)
	{
		GameObject cliffLeftObj = GameObject.Find("CliffLeft-"+para_currLvLID);
		Vector3 nwEngineerStartPos = new Vector3(cliffLeftObj.renderer.bounds.center.x,transform.position.y,transform.position.z);

		Vector3 tmpAngles = transform.localEulerAngles;
		tmpAngles.y = 0;
		transform.localEulerAngles = tmpAngles;

		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",1,new List<System.Object>() { new float[3] {nwEngineerStartPos.x,nwEngineerStartPos.y,nwEngineerStartPos.z}, engiWalkSpeed }));
		batchLists.Add(batch1);
		aniMang.registerListener("EngiScript",this);
		aniMang.init("MoveToNewLevel",batchLists);
		
		Animator characterAni = transform.gameObject.GetComponent<Animator>();
		characterAni.speed = 1;
		characterAni.Play("SideWalkR_Wrench");
	}

	public bool arrContainsInt(int[] para_arr, int para_reqInt)
	{
		bool retFlag = false;
		if(para_arr != null)
		{
			for(int i=0; i<para_arr.Length; i++)
			{
				if(para_arr[i] == para_reqInt)
				{
					retFlag = true;
					break;
				}
			}
		}
		return retFlag;
	}

	
	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

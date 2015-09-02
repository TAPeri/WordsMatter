/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class PassengerHubController : MonoBehaviour, CustomActionListener, IActionNotifier
{

	bool firstTime = true;
	bool isBusy = false;
	GameObject passengerTemplateObj = null;
	GameObject passengerCrowd = null;

	Dictionary<int,int> passToCarriageLookup = new Dictionary<int, int>();
	Dictionary<int,int> carriageCountLookup = new Dictionary<int, int>();
	
	HashSet<int> goodCarriages;

	Dictionary<int,Vector3> passToInitialPos = new Dictionary<int, Vector3>();
	Dictionary<string,GameObject> passToGObj = new Dictionary<string, GameObject>();
	Dictionary<int,Vector3> carrToBasePos = new Dictionary<int, Vector3>();

	List<int> boardedPassengers = new List<int>();

	int numBusyPassengers = 0;

	int lvlConfigReqCarriageCount = 0;

	Dictionary<string,Sprite> smallHeadsLookup;

	public void init(Sprite[] para_smallHeadSprites)
	{
		smallHeadsLookup = new Dictionary<string, Sprite>();
		for(int i=0; i<para_smallHeadSprites.Length; i++)
		{
			Sprite tmpSprt = para_smallHeadSprites[i];

			if( ! smallHeadsLookup.ContainsKey(tmpSprt.name))
			{
				smallHeadsLookup.Add(tmpSprt.name,tmpSprt);
			}
		}
	}

	public void noticeNewTrainSpawn(int para_trainID, int para_lvlConfigReqCarriageCount)
	{
		if( ! isBusy)
		{
			lvlConfigReqCarriageCount = para_lvlConfigReqCarriageCount;

			if(para_trainID == 0)
			{
				passengerCrowd = GameObject.Find("PassengerCrowd");
				
				List<int> crowdPositionIDs = new List<int>();
				for(int i=0; i<12; i++)
				{
					crowdPositionIDs.Add(i);
				}
				addPassengers(crowdPositionIDs);
			}
			else
			{
				List<int> passengersToReplace = new List<int>();
				for(int i=0; i<boardedPassengers.Count; i++)
				{
					passengersToReplace.Add(boardedPassengers[i]);
				}
				boardedPassengers.Clear();
				addPassengers(passengersToReplace);
			}
		}
	}

	public void launchNewBoardingAttempt(HashSet<int> para_goodCarriages)
	{
		if( ! isBusy)
		{
			goodCarriages = para_goodCarriages;
			//boardedPassengers.Clear();
			triggerAllPassengers_walkToCarriageForInspection();
		}
	}


	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{

		//Debug.LogWarning(para_eventID);

		if(para_eventID == "PassengerCrowdEnter")
		{
			GameObject mergerGroup = GameObject.Find(para_sourceID);
			setAnimForAllMembersOfCrowd(mergerGroup,"Idle_R");

			Transform mGroupTrans = mergerGroup.transform;
			List<Transform> childObjs = new List<Transform>();
			for(int i=0; i<mGroupTrans.childCount; i++)
			{
				childObjs.Add(mGroupTrans.GetChild(i));
			}
			for(int i=0; i<childObjs.Count; i++)
			{
				childObjs[i].parent = passengerCrowd.transform;
			}
			Destroy(mergerGroup);

			assignPassengersToCarriages();
		}
		else if(para_eventID == "WalkToCarriage")
		{
			int reqPassID = int.Parse(para_sourceID.Split('-')[1]);
			int reqCarrID = passToCarriageLookup[reqPassID];
			
			if(goodCarriages.Contains(reqCarrID))
			{
				triggerPassenger_boardCarriage(para_sourceID);
			}
			else
			{
				triggerPassenger_incorrectCarriageEffect(para_sourceID);
			}
		}
		else if(para_eventID == "BoardCarriage")
		{
			triggerPassenger_sitDown(para_sourceID);
		}
		else if(para_eventID == "ReturnToPlatform")
		{
			numBusyPassengers--;
			if(numBusyPassengers <= 0)
			{
				numBusyPassengers = 0;
				// Report that all busy passengers have returned.
				notifyAllListeners("PHubController","BoardingAttemptEnd",null);
			}
		}
	}

	private void addPassengers(List<int> para_crowdPositionIDs)
	{
		Dictionary<string,Transform> characterCache = new Dictionary<string, Transform>();
		List<string> availableCharNames = new List<string>() { "SecChar-0", "SecChar-1", "SecChar-2" };

		if(firstTime)
		{
			passengerTemplateObj = GameObject.Find("PassengersTemplate");
		}
		else
		{
			passengerTemplateObj.SetActive(true);
		}

		GameObject passengerMergerGroup = new GameObject("PassengerMergerGroup");


		for(int i=0; i<para_crowdPositionIDs.Count; i++)
		{
			int nxtPosID = para_crowdPositionIDs[i];
			Transform passengerPlaceholder = passengerTemplateObj.transform.FindChild("P"+nxtPosID);

			if(passengerPlaceholder != null)
			{
				string reqPrefabName = availableCharNames[Random.Range(0,availableCharNames.Count)];
				Transform reqPrefab;
				if(characterCache.ContainsKey(reqPrefabName))
				{
					reqPrefab = characterCache[reqPrefabName];
				}
				else
				{
					Transform loadedPrefab = Resources.Load<Transform>("Prefabs/Avatars/"+reqPrefabName);
					characterCache.Add(reqPrefabName,loadedPrefab);
					reqPrefab = loadedPrefab;
				}

				Vector3 passengerSpawnPt = passengerPlaceholder.position;
				Transform nwPassenger = (Transform) Instantiate(reqPrefab,passengerSpawnPt,Quaternion.identity);
				nwPassenger.name = "Pass-"+nxtPosID;
				nwPassenger.parent = passengerMergerGroup.transform;


				CommonUnityUtils.setSortingLayerOfEntireObject(nwPassenger.gameObject,"Pass_"+nxtPosID);
			}
		}

		passengerTemplateObj.SetActive(false);
		firstTime = false;

		triggerPassengerGroup_enterStation(passengerMergerGroup);
	}

	


	private void triggerPassengerGroup_enterStation(GameObject para_groupObj)
	{
		setAnimForAllMembersOfCrowd(para_groupObj,"SideWalkR");

		Vector3 groupDestination = new Vector3(para_groupObj.transform.position.x + 4f,para_groupObj.transform.position.y,para_groupObj.transform.position.z);

		CustomAnimationManager aniMang = para_groupObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3]{groupDestination.x,groupDestination.y,groupDestination.z}, 3f, true }));
		batchLists.Add(batch1);
		aniMang.registerListener("PassengerHubController",this);
		aniMang.init("PassengerCrowdEnter",batchLists);
	}
	

	private void triggerAllPassengers_walkToCarriageForInspection()
	{
		GameObject trainObj = GameObject.Find("Train");

		passToInitialPos.Clear();
		numBusyPassengers = 0;

		foreach(KeyValuePair<int,int> pair in passToCarriageLookup)
		{
			Transform reqPassenger = passengerCrowd.transform.FindChild("Pass-"+pair.Key);
			Transform reqCarriage = trainObj.transform.FindChild("Carriage-"+pair.Value);

			if((reqPassenger != null)&&(reqCarriage != null))
			{
				// Select carriage walk to position.
				Rect carriageBounds = CommonUnityUtils.get2DBounds(reqCarriage.renderer.bounds);

				Vector3 walkToPos = new Vector3(Random.Range(carriageBounds.x,carriageBounds.x + (carriageBounds.width * 0.4f)),
				                                carriageBounds.y - carriageBounds.height,
				                                reqPassenger.position.z);

				passToInitialPos.Add(pair.Key,new Vector3(reqPassenger.position.x,reqPassenger.position.y,reqPassenger.position.z));

				Destroy(reqPassenger.GetComponent<MoveToLocation>());
				Destroy(reqPassenger.GetComponent<CustomAnimationManager>());

				numBusyPassengers++;

				reqPassenger.GetComponent<Animator>().Play("SideWalkR");

				CustomAnimationManager aniMang = reqPassenger.gameObject.AddComponent<CustomAnimationManager>();
				List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
				List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
				batch1.Add(new AniCommandPrep("MoveToLocation",1,new List<System.Object>() { new float[3]{walkToPos.x,walkToPos.y,walkToPos.z}, 8f }));
//				batch1.Add(new AniCommandPrep("MoveToLocation",1,new List<System.Object>() { new float[3]{walkToPos.x,walkToPos.y,walkToPos.z}, 3f }));
				batchLists.Add(batch1);
				aniMang.registerListener("PHCont",this);
				aniMang.init("WalkToCarriage",batchLists);
			}
		}

		if(numBusyPassengers == 0)
		{
			notifyAllListeners("PHubController","FailedToAttemptBoarding",null);
		}
	}

	
	private void triggerPassenger_incorrectCarriageEffect(string para_passName)
	{
		GameObject reqPassObj = passToGObj[para_passName];

		Vector3 retPos = passToInitialPos[int.Parse(para_passName.Split('-')[1])];

		CustomAnimationManager aniMang = reqPassObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("TriggerAnimation",2,new List<System.Object>() { "Shrug" ,6f}));//2f//with mode!= 1, last parameter is animation speed
		List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
		batch2.Add(new AniCommandPrep("TriggerAnimation",1,new List<System.Object>() { "SideWalkL" }));
		batch2.Add(new AniCommandPrep("MoveToLocation",1,new List<System.Object>() { new float[3]{retPos.x,retPos.y,retPos.z}, 8f }));//5f//distance per second
		List<AniCommandPrep> batch3 = new List<AniCommandPrep>();
		batch3.Add(new AniCommandPrep("TriggerAnimation",2,new List<System.Object>() { "Idle_R" ,50f}));
		batchLists.Add(batch1);
		batchLists.Add(batch2);
		batchLists.Add(batch3);
		aniMang.registerListener("PHCont",this);
		aniMang.init("ReturnToPlatform",batchLists);
	}

	private void triggerPassenger_boardCarriage(string para_passName)
	{
		GameObject reqPassObj = passToGObj[para_passName];

		int reqPassID = int.Parse(para_passName.Split('-')[1]);
		int reqCarrID = passToCarriageLookup[reqPassID];

		Vector3 carrBase;
		if(carrToBasePos.ContainsKey(reqCarrID))
		{
			carrBase = carrToBasePos[reqCarrID];
		}
		else
		{
			GameObject trainObj = GameObject.Find("Train");
			Transform reqCarriage = trainObj.transform.FindChild("Carriage-"+reqCarrID);
			Rect carr2DBounds = CommonUnityUtils.get2DBounds(reqCarriage.renderer.bounds);

			Vector3 tmpBase = new Vector3(carr2DBounds.x + (carr2DBounds.width * 0.48f),carr2DBounds.y - carr2DBounds.height,reqPassObj.transform.position.z);
			carrToBasePos.Add(reqCarrID,tmpBase);
			carrBase = tmpBase;
		}

		string reqWalkAnim = "SideWalkR";
		if(reqPassObj.transform.position.x >= carrBase.x)
		{
			reqWalkAnim = "SideWalkL";
		}


		CustomAnimationManager aniMang = reqPassObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("TriggerAnimation",2,new List<System.Object>() { reqWalkAnim,5f }));//...
		batch1.Add(new AniCommandPrep("MoveToLocation",1,new List<System.Object>() { new float[3]{carrBase.x,carrBase.y,carrBase.z}, 8f, true }));//1f
		List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
		batch2.Add(new AniCommandPrep("TriggerAnimation",2,new List<System.Object>() { "BackWalk" ,5f}));
		batch2.Add(new AniCommandPrep("MoveToLocation",1,new List<System.Object>() { new float[3]{carrBase.x,carrBase.y + 0.55f,carrBase.z}, 8f, true }));//1f
		batchLists.Add(batch1);
		batchLists.Add(batch2);
		aniMang.registerListener("PHCont",this);
		aniMang.init("BoardCarriage",batchLists);
	}

	/*private void triggerPassenger_waitOnPlatform(string para_passName)
	{
		GameObject reqPassObj = passToGObj[para_passName];
	}*/

	private void triggerPassenger_sitDown(string para_passName)
	{
		GameObject reqPassObj = passToGObj[para_passName];

		int reqPassID = int.Parse(para_passName.Split('-')[1]);
		int reqCarrID = passToCarriageLookup[reqPassID];

		GameObject trainObj = GameObject.Find("Train");
		Transform reqCarriage = trainObj.transform.FindChild("Carriage-"+reqCarrID);

		int currCarrCount = carriageCountLookup[reqCarrID];
		int reqSeatingID = (4 - currCarrCount) + 1;

		Transform reqSeatingObj = reqCarriage.FindChild("Passenger_"+reqSeatingID);
		SpriteRenderer sRend = reqSeatingObj.GetComponent<SpriteRenderer>();

		Transform reqTorsoBack = reqPassObj.transform.FindChild("Torso").FindChild("TorsoBack");
		string passSpriteID = (reqTorsoBack.GetComponent<SpriteRenderer>().sprite.name).Split('_')[0];
		sRend.sprite = smallHeadsLookup[passSpriteID+"_Head_Sd"]; // eg. SC01_Head_Sd.
		sRend.enabled = true;

		carriageCountLookup[reqCarrID]--;

		passToCarriageLookup.Remove(reqPassID);
		passToInitialPos.Remove(reqPassID);
		passToGObj.Remove(para_passName);
		reqPassObj.transform.parent = reqCarriage;
		Destroy(reqPassObj);

		boardedPassengers.Add(reqPassID);

		if(carriageCountLookup[reqCarrID] <= 0)
		{
			carriageCountLookup[reqCarrID] = 0;
			// Report that carriage is correct and full.
			trainObj.GetComponent<TrainScript>().performCorrectLockdownOnCarriage(reqCarrID);
			notifyAllListeners("PHubController","CorrectCarriageFilled",reqCarrID);
		}

		numBusyPassengers--;
		if(numBusyPassengers <= 0)
		{
			numBusyPassengers = 0;
			// Report that all busy passengers have returned.
			notifyAllListeners("PHubController","BoardingAttemptEnd",null);
		}
	}

	private void setAnimForAllMembersOfCrowd(GameObject para_groupObj, string para_animName)
	{
		if(para_groupObj != null)
		{
			Transform crowdTrans = para_groupObj.transform;
			for(int i=0; i<crowdTrans.childCount; i++)
			{
				Transform nxtPerson = crowdTrans.GetChild(i);
				Animator aniScript = nxtPerson.GetComponent<Animator>();
				if(aniScript != null)
				{
					aniScript.Play(para_animName);
				}
			}
		}
	}

	private void assignPassengersToCarriages()
	{
		passToCarriageLookup.Clear();
		carriageCountLookup.Clear();
		passToInitialPos.Clear();
		passToGObj.Clear();
		carrToBasePos.Clear();


		List<int> useablePassengers = new List<int>();
		for(int i=0; i<passengerCrowd.transform.childCount; i++)
		{
			Transform tmpPass = passengerCrowd.transform.GetChild(i);
			int tmpPassPosID = int.Parse(tmpPass.name.Split('-')[1]);
			useablePassengers.Add(tmpPassPosID);
		}

		// Ensure 1 person per carriage.
		for(int i=0; i<lvlConfigReqCarriageCount; i++)
		{
			int reqCarriageID = i;
			int randIndex = Random.Range(0,useablePassengers.Count);
			int reqPassID = useablePassengers[randIndex];
			passToCarriageLookup.Add(reqPassID,reqCarriageID);
			passToGObj.Add("Pass-"+reqPassID,passengerCrowd.transform.FindChild("Pass-"+reqPassID).gameObject);
			carriageCountLookup.Add(reqCarriageID,1);
			useablePassengers.RemoveAt(randIndex);
		}

		// Add any extra passengers.
		if(useablePassengers.Count > 0)
		{
			for(int i=0; i<lvlConfigReqCarriageCount; i++)
			{
				int reqCarriageID = i;
				int numPassOnCarriage = carriageCountLookup[reqCarriageID];
				if(numPassOnCarriage < 4)
				{
					int remPlaces = 4 - numPassOnCarriage;

					while((remPlaces > 0)&&(useablePassengers.Count > 0))
					{
						int randIndex = Random.Range(0,useablePassengers.Count);
						int reqPassID = useablePassengers[randIndex];
						passToCarriageLookup.Add(reqPassID,reqCarriageID);
						passToGObj.Add("Pass-"+reqPassID,passengerCrowd.transform.FindChild("Pass-"+reqPassID).gameObject);
						carriageCountLookup[reqCarriageID]++;

						useablePassengers.RemoveAt(randIndex);
						remPlaces--;
					}
				}
			}
		}
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

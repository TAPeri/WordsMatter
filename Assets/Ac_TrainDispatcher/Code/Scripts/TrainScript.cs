/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class TrainScript : MonoBehaviour, CustomActionListener, IActionNotifier
{
	Transform trainFrontPrefab;
	Transform trainCarriagePrefab;
	Transform wordBoxPrefab;

	Vector3 trainFrontParkPos;

	bool[] upAxisArr = new bool[3] { false, true, false };

	int numOfCarriages;


	// Return world bounds of entire train.
	public TrainCreationRetData createTrain(string[] para_carriageItems,
										                        Vector3 para_trainFrontParkPos,
										                        Rect para_screenBoundsInWorldCoords,
										                        List<Transform> para_prefabList)
	{
		trainFrontPrefab = para_prefabList[0];
		trainCarriagePrefab = para_prefabList[1];
		wordBoxPrefab = para_prefabList[2];

		trainFrontParkPos = para_trainFrontParkPos;



		Vector3 trainOutOfScreenSpawnPt = new Vector3(para_screenBoundsInWorldCoords.xMax + (trainFrontPrefab.renderer.bounds.size.x/2f),
		                                              trainFrontParkPos.y,
		                                              trainFrontParkPos.z);

		transform.position = trainOutOfScreenSpawnPt;

		Transform trainFront = (Transform) Instantiate(trainFrontPrefab,trainOutOfScreenSpawnPt,trainFrontPrefab.rotation);
		trainFront.name = "TrainFront";
		//trainFront.parent = transform;




		int maxStorageBay = 10;

		int totTrainCarriages = para_carriageItems.Length;
		int potentialExtraCarriages = maxStorageBay - para_carriageItems.Length;
		if(potentialExtraCarriages > 0)
		{
			int extraCarriages = Random.Range(1,potentialExtraCarriages);
			totTrainCarriages += extraCarriages;
		}
		else
		{
			// No extra carriages.
		}
		numOfCarriages = totTrainCarriages;



		Rect nxtCarriageBounds = new Rect(trainFront.position.x + (trainFront.renderer.bounds.size.x/2f) - 0.1f,
		                                  trainFront.position.y + (trainCarriagePrefab.renderer.bounds.size.y/2f),
		                                  trainCarriagePrefab.renderer.bounds.size.x,
		                                  trainCarriagePrefab.renderer.bounds.size.y);

		List<GameObject> carriages = new List<GameObject>();

		numOfCarriages = 1;
		for(int i=0; i<numOfCarriages; i++)
		{

			GameObject nwCarriage = createNewCarriage(i,nxtCarriageBounds,trainOutOfScreenSpawnPt.z);

			carriages.Add(nwCarriage);
			nxtCarriageBounds.x += nxtCarriageBounds.width - 0.15f;
		}




		Rect maxTrainWorldBounds = CommonUnityUtils.get2DBounds(CommonUnityUtils.findMaxBounds(new List<GameObject>() { trainFront.gameObject, carriages[carriages.Count-1] }));

		trainFront.parent = transform;
		for(int i=0; i<carriages.Count; i++)
		{
			carriages[i].transform.parent = transform;
		}


		TrainCreationRetData retData = new TrainCreationRetData(maxTrainWorldBounds,numOfCarriages,maxStorageBay - numOfCarriages);
//		TrainCreationRetData retData = new TrainCreationRetData(maxTrainWorldBounds,numOfCarriages,maxStorageBay - numOfCarriages);
		return retData;
	}

	public void enterTrainStation()
	{
		GameObject.Find("Lights").GetComponent<Animator>().Play("LightsStop");

		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> cmdBatches = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",1,new List<System.Object>() { new float[3] { trainFrontParkPos.x, trainFrontParkPos.y, trainFrontParkPos.z }, 8f }));//3f
		cmdBatches.Add(batch1);
		aniMang.registerListener("TrainScript",this);
		aniMang.init("EnterTrainStation",cmdBatches);
	}

	private void haltTrain()
	{
		for(int i=0; i<transform.childCount; i++)
		{
			Transform tmpChild = transform.GetChild(i);
			Animator tmpAniScript = tmpChild.gameObject.GetComponent<Animator>();
			if(tmpAniScript != null)
			{
				tmpAniScript.speed = 0;
			}
		}

		//transform.FindChild("TrainFront").FindChild("Smoke").gameObject.SetActive(false);
	}


	public Vector3 addCarriage()
	{
		//Rect cam2DBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,false);
	
		GameObject trainFront = transform.FindChild("TrainFront").gameObject;

		GameObject rightMostCarriage = transform.FindChild("Carriage-"+(numOfCarriages-1)).gameObject;
		Rect rightMostCarriageWorldBounds = CommonUnityUtils.get2DBounds(rightMostCarriage.renderer.bounds);

		GameObject appendObj = new GameObject("AppendObject");
		appendObj.transform.position = new Vector3(rightMostCarriageWorldBounds.xMax + (trainCarriagePrefab.renderer.bounds.size.x * 2f),trainFront.transform.position.y,trainFront.transform.position.z);






		Vector3 endPos = new Vector3(rightMostCarriageWorldBounds.center.x + rightMostCarriageWorldBounds.width - 0.15f,
		                             				 appendObj.transform.position.y,
		                             				 appendObj.transform.position.z);



		Rect nwCarriageSpawnWorldBounds = new Rect(appendObj.transform.position.x - (trainCarriagePrefab.renderer.bounds.size.x/2f),
		                                           							  appendObj.transform.position.y + (trainCarriagePrefab.renderer.bounds.size.y/2f),
		                                           							  trainCarriagePrefab.renderer.bounds.size.x,
		                                           							  trainCarriagePrefab.renderer.bounds.size.y);

		GameObject nwCarriage = this.createNewCarriage(numOfCarriages,nwCarriageSpawnWorldBounds,trainFront.transform.position.z);
		nwCarriage.transform.parent = appendObj.transform;


		
		MoveToLocation mtl = appendObj.AddComponent<MoveToLocation>();
		mtl.registerListener("TrainScript",this);
		mtl.initScript(endPos,5f);

		return endPos;
	}

	public void detachCarriage(int para_carriageID)
	{
		Transform reqCarriageTrans = transform.FindChild("Carriage-"+para_carriageID);

		if(reqCarriageTrans != null)
		{
			GameObject detachCollection = new GameObject("DetachCollection");
			detachCollection.transform.position = new Vector3(reqCarriageTrans.position.x,reqCarriageTrans.position.y,reqCarriageTrans.position.z);
			reqCarriageTrans.parent = detachCollection.transform;


			int nxtID = para_carriageID+1;

			bool foundTailCarriage = true;
			while(foundTailCarriage)
			{
				Transform nxtCarriage = transform.FindChild("Carriage-"+nxtID);
				if(nxtCarriage != null)
				{
					nxtCarriage.parent = detachCollection.transform;
					nxtID++;
				}
				else
				{
					foundTailCarriage = false;
				}
			}



			// Trigger detach movement out of screen.

			numOfCarriages -= detachCollection.transform.childCount;
			notifyAllListeners("TrainScript","DetachmentStart",null);


			Rect cam2DBounds = WorldSpawnHelper.getCameraViewWorldBounds(1,false);

			Vector3 endPos = new Vector3(cam2DBounds.xMax + (trainCarriagePrefab.renderer.bounds.size.x/2f),
			   							 detachCollection.transform.position.y,
			                             detachCollection.transform.position.z);

			MoveToLocation mtl = detachCollection.AddComponent<MoveToLocation>();
			mtl.registerListener("TrainScript",this);
			mtl.initScript(endPos,5f);

		}


	}

	public void performCorrectLockdownOnCarriage(int para_carriageID)
	{
		Transform reqCarriageTrans = transform.FindChild("Carriage-"+para_carriageID);
		
		if(reqCarriageTrans != null)
		{
			SpriteRenderer sRend = reqCarriageTrans.GetComponent<SpriteRenderer>();
			sRend.color = Color.green;
		}
	}

	public void leaveStation()
	{
		GameObject.Find("Lights").GetComponent<Animator>().Play("LightsGo");

		Rect leftMostBackdropBounds = CommonUnityUtils.get2DBounds(GameObject.Find("BackdropCollection").transform.FindChild("Backdrop*0").gameObject.renderer.bounds);
		Rect maxTrainBounds = getMaxTrainBounds();

		Transform trainFront = transform.FindChild("TrainFront");

		Vector3 trainEndPos = new Vector3(leftMostBackdropBounds.x - maxTrainBounds.width,
		                                  					trainFront.position.y,
		                                  					trainFront.position.z);


		trainFront.GetComponent<Animator>().speed = 1;
		for(int i=0; i<numOfCarriages; i++)
		{
			Transform tmpChild = transform.FindChild("Carriage-"+i);
			Animator tmpAniScript = tmpChild.gameObject.GetComponent<Animator>();
			if(tmpAniScript != null)
			{
				tmpAniScript.speed = 1;
			}
		}


		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> cmdBatches = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",1,new List<System.Object>() { new float[3] { trainEndPos.x, trainEndPos.y, trainEndPos.z }, 8f }));//3f
		cmdBatches.Add(batch1);
		aniMang.registerListener("TrainScript",this);
		aniMang.init("LeaveTrainStation",cmdBatches);
	}


	public Rect getMaxTrainBounds()
	{
		GameObject trainFront = transform.FindChild("TrainFront").gameObject;
		GameObject lastCarriage = transform.FindChild("Carriage-"+(numOfCarriages-1)).gameObject;
		Rect maxTrainWorldBounds = CommonUnityUtils.get2DBounds(CommonUnityUtils.findMaxBounds(new List<GameObject>() { trainFront, lastCarriage }));
		return maxTrainWorldBounds;
	}

	public string[] getCarriageItems()
	{
		List<string> carriageItems = new List<string>();

		for(int i=0; i<numOfCarriages; i++)
		{
			GameObject reqCarriage = transform.FindChild("Carriage-"+i).gameObject;
			TextMesh reqTMesh = reqCarriage.transform.FindChild("TextInputObj").transform.FindChild("Text").gameObject.GetComponent<TextMesh>();
			carriageItems.Add(reqTMesh.text.Trim());
		}

		return carriageItems.ToArray();
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "EnterTrainStation")
		{
			haltTrain();
			notifyAllListeners("TrainScript","TrainParked",null);
		}
		else if(para_eventID == "LeaveTrainStation")
		{
			notifyAllListeners("TrainScript","TrainLeftStation",null);
			Destroy(transform.gameObject);
		}
		else if(para_sourceID == "DetachCollection")
		{
			GameObject detachCollection = GameObject.Find("DetachCollection");
			int numOfCarriagesDetached = detachCollection.transform.childCount;

			Destroy(detachCollection);
			notifyAllListeners("TrainScript","DetachmentEnd",numOfCarriagesDetached);
		}
		else if(para_sourceID == "AppendObject")
		{
			Transform appendObjTrans = GameObject.Find("AppendObject").transform;

			for(int i=0; i<appendObjTrans.childCount; i++)
			{
				Transform tmpChild = appendObjTrans.GetChild(i);
				Animator tmpAniScript = tmpChild.gameObject.GetComponent<Animator>();
				if(tmpAniScript != null)
				{
					tmpAniScript.speed = 0;
				}

				tmpChild.parent = transform;
				numOfCarriages++;
			}

			Destroy(appendObjTrans.gameObject);
			notifyAllListeners("TrainScript","AppendEnd",null);
		}
	}


	private GameObject createNewCarriage(int para_carriageIndex, Rect para_carriageSpawnBounds, float para_carriageZVal)
	{
		int carriageID = para_carriageIndex;

		// Create carraige base object.
		GameObject nwCarriage = WorldSpawnHelper.initObjWithinWorldBounds(trainCarriagePrefab,"Carriage-"+carriageID,para_carriageSpawnBounds,para_carriageZVal,upAxisArr);
		
		
		// Create carraige number.
		GameObject carriageNumberGuide = nwCarriage.transform.FindChild("CarriageNumber").gameObject;
		Rect carriageNumber2DBounds = CommonUnityUtils.get2DBounds(carriageNumberGuide.renderer.bounds);
		
		GameObject nwCarriageNumber = WordBuilderHelper.buildWordBox(99,""+(carriageID+1),carriageNumber2DBounds,carriageNumberGuide.transform.position.z,upAxisArr,wordBoxPrefab);
		nwCarriageNumber.name = "CarNum";
		nwCarriageNumber.transform.parent = nwCarriage.transform;
		Destroy(nwCarriageNumber.transform.FindChild("Board").gameObject);
		
		
		// Create text input area.
		GameObject textAreaGuide = nwCarriage.transform.FindChild("TextArea").gameObject;
		Rect textArea2DBounds = CommonUnityUtils.get2DBounds(textAreaGuide.renderer.bounds);
		
		GameObject nwTextInputObj = WordBuilderHelper.buildWordBox(99," ",textArea2DBounds,textAreaGuide.transform.position.z,upAxisArr,wordBoxPrefab);
		nwTextInputObj.name = "TextInputObj";
		WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() { nwTextInputObj },0.04f);
		nwTextInputObj.transform.parent = nwCarriage.transform;
		Destroy(nwTextInputObj.transform.FindChild("Board").gameObject);


		// Disable selector highlight.
		nwCarriage.transform.FindChild("CarriageSelection").renderer.enabled = false;

		return nwCarriage;
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

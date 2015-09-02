/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class DeliveryChuteScript : MonoBehaviour, CustomActionListener, IActionNotifier
{

	bool firstTime = true;
	Vector3 offScreenPos;
	Vector3 junkyardCentrePos;

	float enterSequenceInSec = 10f;
	float hurryEnterSequenceInSec = 3f;
	float exitSequenceInSec = 3f;

	float growScale;
	Rect destGrowRect;

	bool hurryFlag = false;


	string word;
	GridProperties dropGridWorldGProp;
	Transform textPlanePrefab;
	Transform splitDetectorPrefab;
	Transform[] itemToSplitPrefabArr;
	Transform framePrefab;
	Transform glowPrefab;
	Transform nettingPrefab;


	public void performFetchWordSequence(string para_word,
	                                     GridProperties para_dropGridWorldGProp,
	                                     Transform para_textPlanePrefab,
	                                     Transform para_splitDetectorPrefab,
	                                     Transform[] para_itemToSplitPrefabArr,
	                                     Transform para_framePrefab,
	                                     Transform para_glowPrefab,
	                                     Transform para_nettingPrefab)
	{
		word = para_word;
		dropGridWorldGProp = para_dropGridWorldGProp;
		textPlanePrefab = para_textPlanePrefab;
		splitDetectorPrefab = para_splitDetectorPrefab;
		itemToSplitPrefabArr = para_itemToSplitPrefabArr;
		framePrefab = para_framePrefab;
		glowPrefab = para_glowPrefab;
		nettingPrefab = para_nettingPrefab;

		hurryFlag = false;

		exitScene();
	}

	private void enterScene()
	{
		notifyAllListeners("BigPipe","EnterStart",null);
		toggleChuteMovement(true);
	}

	private void exitScene() { toggleChuteMovement(false); }


	private void emitNewWord(string para_word,
	                         GridProperties para_dropGridWorldGProp,
	                         Transform para_textPlanePrefab,
	                         Transform para_splitDetectorPrefab,
	                         Transform[] para_itemToSplitPrefabArr,
	                         Transform para_framePrefab,
	                         Transform para_glowPrefab)
	{

		GameObject chuteGObj = GameObject.Find("BigPipe");

		int wordLength = para_word.Length;
		GridProperties dropGrid_WorldGProp = para_dropGridWorldGProp;

		GameObject emitAreaGOBj = GameObject.Find("WordEmitArea");
		Rect emitAreaWorld = CommonUnityUtils.get2DBounds(emitAreaGOBj.renderer.bounds);

		float totalBlockWidth = ((dropGrid_WorldGProp.cellWidth + dropGrid_WorldGProp.borderThickness) * wordLength);
		float totalBlockHeight = dropGrid_WorldGProp.cellHeight;
		
		growScale = Mathf.Min( (emitAreaWorld.width/totalBlockWidth), (emitAreaWorld.height/totalBlockHeight) );
		float destWidth = (totalBlockWidth * growScale);
		float destHeight = (totalBlockHeight * growScale);
		destGrowRect = new Rect(emitAreaWorld.x + (emitAreaWorld.width/2f) - (destWidth/2f), emitAreaWorld.y - (emitAreaWorld.height/2f) + (destHeight/2f),destWidth,destHeight);


		GameObject backgroundGObj = GameObject.Find("Background");
		Rect background2DBounds = CommonUnityUtils.get2DBounds(backgroundGObj.renderer.bounds);
		chuteGObj.transform.position = new Vector3(background2DBounds.x - (destGrowRect.width/2f),
		                                           chuteGObj.transform.position.y,
		                                           chuteGObj.transform.position.z);

		Vector3 spawnPosForWord = new Vector3(chuteGObj.transform.position.x,
		                                      chuteGObj.transform.position.y - (chuteGObj.renderer.bounds.size.y/2f) - (destGrowRect.height/2f),
		                                      chuteGObj.transform.position.z);





		// Spawn word.
		GameObject nwConveyorWordGObj = WordFactoryYUp.spawnSplittableWordBoard(para_word,
		                                                                        spawnPosForWord,
		                                                                        dropGrid_WorldGProp.cellWidth,
		                                                                        dropGrid_WorldGProp.borderThickness,
		                                                                        -1,
		                                                                        new bool[] {true,true,true},
		new bool[] {false,true,false},
		para_textPlanePrefab,
		para_splitDetectorPrefab,
		para_itemToSplitPrefabArr,
		null,
		para_framePrefab,
		para_glowPrefab);
		

		Vector3 tmpLocalScale = nwConveyorWordGObj.transform.localScale;
		tmpLocalScale.x *= growScale;
		tmpLocalScale.y *= growScale;
		tmpLocalScale.z *= growScale;
		nwConveyorWordGObj.transform.localScale = tmpLocalScale;


		SplittableBlockUtil.setStateOfSplitDetectsForWord(nwConveyorWordGObj,false);


		GameObject wordHolder = new GameObject("WordHolder");
		nwConveyorWordGObj.transform.parent = wordHolder.transform;



		// Apply netting to the word.
		GameObject nettingParent = new GameObject("Netting");
		nettingParent.transform.position = new Vector3(spawnPosForWord.x,spawnPosForWord.y,spawnPosForWord.z);
		int numOfReqNetting = wordLength++;
		float totRequiredNettingWidth = destGrowRect.width;
		float netting_X = spawnPosForWord.x - (totRequiredNettingWidth/2f);
		float netting_Y = spawnPosForWord.y + (destGrowRect.height/2f);
		float netting_Height = destGrowRect.height;
		float netting_Width = netting_Height;
		Rect nettingBounds = new Rect(netting_X,netting_Y,netting_Width,netting_Height);

		for(int i=0; i<numOfReqNetting; i++)
		{
			GameObject nettingChild = WorldSpawnHelper.initObjWithinWorldBounds(nettingPrefab,"Netting-"+i,nettingBounds,spawnPosForWord.z - 1.2f,new bool[] {false,true,false});
			nettingChild.transform.parent = nettingParent.transform;
			nettingBounds.x += nettingBounds.width;
		}






		// Attach to harness.
		Transform harnessGObj = chuteGObj.transform.FindChild("Harness");
		if(harnessGObj == null) { harnessGObj = (new GameObject("Harness")).transform; harnessGObj.parent = chuteGObj.transform; }
		Transform oldWordHolder = harnessGObj.FindChild("WordHolder");
		if(oldWordHolder != null) { Destroy(oldWordHolder.gameObject); }
		Transform oldNetting = harnessGObj.FindChild("Netting");
		if(oldNetting != null) { Destroy(oldNetting.gameObject); }
		wordHolder.transform.parent = harnessGObj;
		nettingParent.transform.parent = harnessGObj;


		//caMang.init("PipeEmergeSequence",cmdBatchList);

		respondToEvent("DeliveryChute","DeliveryChuteEmitWord",null);
	}

	



	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "DeliveryChuteExit")
		{
			emitNewWord(word,
			            dropGridWorldGProp,
			            textPlanePrefab,
			            splitDetectorPrefab,
			            itemToSplitPrefabArr,
			            framePrefab,
			            glowPrefab);
		}
		else if(para_eventID == "DeliveryChuteEmitWord")
		{
			enterScene();
		}
		else if(para_eventID == "DeliveryChuteEnter")
		{
			// Remove netting.
			releaseWord();
			removeNetting();

			notifyAllListeners(para_sourceID,para_eventID,para_eventData);
		}
	}

	public GameObject getAttachedWordGObj()
	{
		GameObject retObj = null;
		GameObject chute = GameObject.Find("BigPipe");
		if(chute != null)
		{
			Transform harness = chute.transform.FindChild("Harness");
			if(harness != null)
			{
				Transform wordHolder = harness.transform.FindChild("WordHolder");
				if(wordHolder != null)
				{
					retObj = wordHolder.GetChild(0).gameObject;
				}
			}
		}
		return retObj;
	}

	public float getGrowScale()
	{
		return growScale;
	}

	public void hurryUp()
	{
		hurryFlag = true;
		GameObject chuteGObj = GameObject.Find("BigPipe");
		Destroy(chuteGObj.GetComponent<MoveToLocation>());

		float percLeft = Vector3.Distance(chuteGObj.transform.position,junkyardCentrePos)/Vector3.Distance(offScreenPos,junkyardCentrePos);
		hurryEnterSequenceInSec = 3f * percLeft;

		toggleChuteMovement(true);
	}

	public void setPauseState(bool para_pause)
	{
		// Replace this later with customanimang pause method.
		MoveToLocation mtl = GameObject.Find("BigPipe").GetComponent<MoveToLocation>();
		if(mtl != null)
		{
			mtl.enabled = !para_pause;
		}
	}

	private void releaseWord()
	{
		GameObject attachedWordGObj = getAttachedWordGObj();
		GameObject nettingObj = GameObject.Find("BigPipe").transform.FindChild("Harness").FindChild("Netting").gameObject;
		float reqY = destGrowRect.y - (destGrowRect.height/2f);

		List<GameObject> tmpList = new List<GameObject>() { attachedWordGObj, nettingObj };

		for(int i=0; i<tmpList.Count; i++)
		{
			Vector3 tmpPos = tmpList[i].transform.position;
			tmpPos.y = reqY;
			tmpList[i].transform.position = tmpPos;
		}
	}

	private void removeNetting()
	{
		GameObject nettingObj = GameObject.Find("BigPipe").transform.FindChild("Harness").FindChild("Netting").gameObject;

		CustomAnimationManager aniMang = nettingObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] {1,1,1,0}, 2f }));
		List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
		batch2.Add(new AniCommandPrep("DestroyObject",1,new List<System.Object>() {}));
		batchLists.Add(batch1);
		batchLists.Add(batch2);
		aniMang.init("NettingRemoval",batchLists);
	}

	private void toggleChuteMovement(bool para_orderChuteEnter)
	{
		GameObject chuteGObj = GameObject.Find("BigPipe");
		if(firstTime) { performPrep(); }
		

		Vector3 reqDestPos;
		string aniName = "DeliveryChuteEnter";
		float totAniTimeInSec = enterSequenceInSec;
		if(para_orderChuteEnter) { reqDestPos = junkyardCentrePos; aniName = "DeliveryChuteEnter"; totAniTimeInSec = enterSequenceInSec; }
		else { reqDestPos = offScreenPos; aniName = "DeliveryChuteExit"; totAniTimeInSec = exitSequenceInSec; }
		if(hurryFlag) { totAniTimeInSec = hurryEnterSequenceInSec; }
		
		CustomAnimationManager aniMang = chuteGObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3] {reqDestPos.x,reqDestPos.y,reqDestPos.z}, totAniTimeInSec, true }));
		batchLists.Add(batch1);
		aniMang.registerListener("DeliveryChuteScript",this);
		aniMang.init(aniName,batchLists);
	}

	private void performPrep()
	{
		if(firstTime)
		{
			Transform chute = GameObject.Find("BigPipe").transform;

			junkyardCentrePos = new Vector3(chute.position.x,chute.position.y,chute.position.z);

			GameObject backgroundGObj = GameObject.Find("Background");
			Rect backgroundWorld2DBounds = CommonUnityUtils.get2DBounds(backgroundGObj.renderer.bounds);

			offScreenPos = new Vector3(backgroundWorld2DBounds.x - (chute.gameObject.renderer.bounds.size.x),chute.position.y, chute.position.z);
		}
		firstTime = false;
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

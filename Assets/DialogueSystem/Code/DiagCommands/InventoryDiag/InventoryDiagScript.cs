/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class InventoryDiagScript : MonoBehaviour, CustomActionListener, IActionNotifier
{


	GameObject currentBubble;

	Vector3 dragRestPosition;
	GameObject draggableInvItemObj;

	GameObject globObjToDestroy;
	DragScript ds;

	Transform sfxPrefab;

	GameObject itemRepObj;
	Vector3 pointerHandDestPt;

	GameObject srcInventory;
	GameObject destInventory;


	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "DragStart")
		{
			triggerSoundAtCamera("sfx_ObjectGrab");
			itemRepObj.transform.localScale +=  new Vector3(0.1f,0.1f,0.1f);
			srcInventory.GetComponent<SpriteRenderer>().color = Color.white;
		}
		else if(para_eventID == "DragRelease")
		{
			if(ds.getNumPotentialOwnersForDragObj(para_sourceID) <= 0)
			{
				triggerSoundAtCamera("sfx_ObjectRelease");
				itemRepObj.transform.localScale -= new Vector3(0.1f,0.1f,0.1f);
				srcInventory.GetComponent<SpriteRenderer>().color = Color.gray;
				TeleportToLocation ttl = draggableInvItemObj.AddComponent<TeleportToLocation>();
				ttl.init(dragRestPosition);
			}
		}
		else if(para_eventID == "HoleFilled")
		{
			Destroy(ds);
			Destroy(globObjToDestroy);

			itemRepObj.transform.localScale -= new Vector3(0.1f,0.1f,0.1f);
			srcInventory.GetComponent<SpriteRenderer>().color = Color.white;
			destInventory.GetComponent<SpriteRenderer>().color = Color.gray;
			Transform pointerHandRemnant = currentBubble.transform.FindChild("PointerHand");
			if(pointerHandRemnant != null) { Destroy(pointerHandRemnant.gameObject); }
			performInventoryGainAnimation();
		}
		else if(para_eventID == "PointerHandAni")
		{
			//ds.enabled = true;
			//Destroy(currentBubble.transform.FindChild("PointerHand").gameObject);
		}
		else if(para_eventID == "InventoryGainAnimation")
		{
			notifyAllListeners("InventoryDiagScript","BubbleCreated",currentBubble);
			//Destroy(this);
		}
	}
	
	public void init(InventoryDiagCommand para_commandData)
	{

		Transform bubbleGuide = transform.FindChild("InventoryTransferBubble");
		currentBubble = ((Transform) Instantiate(bubbleGuide,bubbleGuide.position,bubbleGuide.rotation)).gameObject;
		currentBubble.name = "CurrentBubble";
		currentBubble.transform.parent = transform;
		currentBubble.SetActive(true);

		GameObject person1Inventory = currentBubble.transform.FindChild("Person1Inventory").gameObject;
		GameObject person2Inventory = currentBubble.transform.FindChild("Person2Inventory").gameObject;


		srcInventory = person1Inventory;
		destInventory = person2Inventory;
		if(para_commandData.getDiagType() == DialogueViewType.INVENTORY_TRANSFER_2_TO_1)
		{
			srcInventory = person2Inventory;
			destInventory = person1Inventory;

			GameObject transferArrow = GameObject.Find("TransferArrow");
			Vector3 tmpAngles = transferArrow.transform.eulerAngles;
			tmpAngles.y = 180;
			transferArrow.transform.eulerAngles = tmpAngles;
		}
		srcInventory.GetComponent<SpriteRenderer>().color = Color.gray;


		GameObject globObj = GameObject.Find("GlobObj");
		if(globObj == null)
		{
			globObj = new GameObject("GlobObj");
			globObjToDestroy = globObj;
		}

		ds = globObj.AddComponent<DragScript>();
		ds.registerListener("InventoryDiagScript",this);
		//ds.enabled = false;


		GameObject srcItemPlatform = srcInventory.transform.FindChild("ItemArea").gameObject;
		GameObject destItemPlatform = destInventory.transform.FindChild("ItemArea").gameObject;

		itemRepObj = WorldSpawnHelper.initObjWithinWorldBounds(para_commandData.getInventoryItemPrefab(),"ItemToDrag",srcItemPlatform.renderer.bounds,new bool[]{false,true,false});
		itemRepObj.renderer.sortingOrder = 150;
		itemRepObj.transform.parent = srcItemPlatform.transform;

		srcItemPlatform.layer = LayerMask.NameToLayer("Draggable");
		dragRestPosition = srcItemPlatform.transform.position;
		draggableInvItemObj = srcItemPlatform;

		HoleScript hs = destItemPlatform.AddComponent<HoleScript>();
		hs.registerListener("InventoryDiagScript",this);





		Transform pointerHandTrans = currentBubble.transform.FindChild("PointerHand");

		Vector3 pointerHandSrcPt = new Vector3(srcItemPlatform.transform.position.x + (pointerHandTrans.renderer.bounds.size.x/2f),srcItemPlatform.transform.position.y - (pointerHandTrans.renderer.bounds.size.y/2f),pointerHandTrans.position.z);
		pointerHandDestPt = new Vector3(destItemPlatform.transform.position.x + (pointerHandTrans.renderer.bounds.size.x/2f),destItemPlatform.transform.position.y - (pointerHandTrans.renderer.bounds.size.y/2f),pointerHandTrans.position.z);



		CustomAnimationManager aniMang = pointerHandTrans.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3]{pointerHandSrcPt.x,pointerHandSrcPt.y,pointerHandSrcPt.z},1f,true }));
		List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
		batch2.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3]{pointerHandDestPt.x,pointerHandDestPt.y,pointerHandDestPt.z},1f,true }));
		List<AniCommandPrep> batch3 = new List<AniCommandPrep>();
		batch3.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] {0,0,0,0}, 2f }));
		List<AniCommandPrep> batch4 = new List<AniCommandPrep>();
		batch4.Add(new AniCommandPrep("DestroyObject",1,new List<System.Object>()));
		batchLists.Add(batch1);
		batchLists.Add(batch2);
		batchLists.Add(batch3);
		batchLists.Add(batch4);
		aniMang.registerListener("InventoryDiagScript",this);
		aniMang.init("PointerHandAni",batchLists);
	}

	private void performInventoryGainAnimation()
	{
		CustomAnimationManager aniMang = transform.gameObject.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>() { 2f }));
		batchLists.Add(batch1);
		aniMang.registerListener("InventoryDragScript",this);
		aniMang.init("InventoryGainAnimation",batchLists);
	}

	public void triggerSoundAtCamera(string para_soundFileName)
	{
		GameObject camGObj = Camera.main.gameObject;

		if(sfxPrefab == null) { sfxPrefab = Resources.Load<Transform>("Prefabs/SFxBox"); }
		GameObject nwSFX = ((Transform) Instantiate(sfxPrefab,camGObj.transform.position,Quaternion.identity)).gameObject;
		AudioSource audS = (AudioSource) nwSFX.GetComponent(typeof(AudioSource));
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.volume = 0.5f;
		audS.Play();
	}
	
	
	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

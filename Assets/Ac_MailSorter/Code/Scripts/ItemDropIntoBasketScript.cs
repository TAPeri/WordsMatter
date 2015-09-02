/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ItemDropIntoBasketScript : MonoBehaviour, CustomActionListener, IActionNotifier
{
	GameObject basketObj;
	GameObject parcelObj;
	Transform basketBasePrefab;
	bool autocorrect;

	public void init(GameObject para_relatedParcelObj, GameObject para_basketObj, Transform para_basketBasePrefab, bool para_autocorrect)
	{
		parcelObj = para_relatedParcelObj;
		basketObj = para_basketObj;
		basketBasePrefab = para_basketBasePrefab;
		autocorrect = para_autocorrect;

		parcelObj.transform.parent = null;


		// STEP 1: Detach parcel from conveyors.
		GameObject mainConveyorObj = GameObject.Find("ConveyerRight").gameObject;
		ConveyorScript cs1 = mainConveyorObj.GetComponent<ConveyorScript>();
		cs1.detachObject(parcelObj.name);
		
		GameObject returnConveyorObj = GameObject.Find("ReturnConveyerLeft");
		ConveyorScript cs2 = returnConveyorObj.GetComponent<ConveyorScript>();
		cs2.detachObject(parcelObj.name);
		

		// STEP 2: Remove rigidbodies and colliders from the parcel.
		Destroy(parcelObj.rigidbody);
		Destroy(parcelObj.collider);
		parcelObj.name = "DroppedP-"+(parcelObj.name.Split('-')[1]);
		parcelObj.layer = LayerMask.NameToLayer("Default");


		// STEP 3: Perform the shrink animation.
		CustomAnimationManager aniMang = parcelObj.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		batch1.Add(new AniCommandPrep("GrowOrShrink",1,new List<System.Object>() { new float[3] { 0,0,0 }, 5f }));//1f
		//batch1.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] {0,0,0,0}, 0.1f }));
		batchLists.Add(batch1);
		aniMang.registerListener("ItemDropIntoBasketScript",this);
		aniMang.init("ParcelDisappearAni",batchLists);
	}


	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "ParcelDisappearAni")
		{
			// STEP 4: Reposition parcel on top of the basket.

			Vector3 basketCentre = basketObj.transform.position;
			Vector3 aboveBasketPt = new Vector3(basketCentre.x, basketCentre.y + 1f, basketCentre.z + 0.1f);
			parcelObj.transform.position = aboveBasketPt;


			// STEP 5: Rotate object so it enters the basket vertically.
			if(parcelObj.renderer.bounds.size.x >= parcelObj.renderer.bounds.size.y)
			{
				Vector3 eAngles = parcelObj.transform.localEulerAngles;
				eAngles.z = 90;
				parcelObj.transform.localEulerAngles = eAngles;
			}
			else
			{
				Vector3 eAngles = parcelObj.transform.localEulerAngles;
				eAngles.z = 0;
				parcelObj.transform.localEulerAngles = eAngles;
			}

		
			// STEP 6: Perform grow animation.
			CustomAnimationManager aniMang = parcelObj.AddComponent<CustomAnimationManager>();
			List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
			List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
			batch1.Add(new AniCommandPrep("GrowOrShrink",1,new List<System.Object>() { new float[3] { 1,1,1 }, 5f }));//1f
			//batch1.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] {0,0,0,1}, 0.1f }));
			batchLists.Add(batch1);
			aniMang.registerListener("ItemDropIntoBasketScript",this);
			aniMang.init("ParcelAppearAni",batchLists);

			// Wait for appear ani to finish
		}
		else if(para_eventID == "ParcelAppearAni")
		{
			Bounds basketBounds = basketObj.renderer.bounds;

			// STEP 7: Init basket base collider.
			Vector3 basketBaseSpawnPos = new Vector3(basketBounds.center.x,basketBounds.center.y - (basketBounds.size.y/2f) + (basketBasePrefab.renderer.bounds.size.y),basketBounds.center.z);
			Transform nwBasketBase = (Transform) Instantiate(basketBasePrefab,basketBaseSpawnPos,Quaternion.identity);
			BasketBaseColliderScript bbcs = nwBasketBase.GetComponent<BasketBaseColliderScript>();
			bbcs.registerListener("ItemDropIntoBasketScript",this);


			// STEP 8: Attach rigid body to parcel.
			parcelObj.AddComponent<BoxCollider>();
			Rigidbody rb = parcelObj.AddComponent<Rigidbody>();
			rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

			// Wait for parcel to hit basket base.
		}
		else if(para_eventID == "BasketReceivedItem")
		{
			// STEP 9: Remove rigid body and collider from parcel.
			Destroy(parcelObj.collider);
			Destroy(parcelObj.rigidbody);

			// STEP 10: Attach parcel to the basket.
			parcelObj.transform.parent = basketObj.transform;

			// STEP 11: Send message that procedure is complete.
			System.Object[] retData = new System.Object[2];
			retData[0] = parcelObj;
			retData[1] = basketObj;

			if(autocorrect)
				notifyAllListeners("ItemDropIntoBasketScript","BasketReceiveCompleteAutoCorrect",retData);

			else
				notifyAllListeners("ItemDropIntoBasketScript","BasketReceiveComplete",retData);
			Destroy(this);
		}
	}



	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}

}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class PhotoVisualiser
{


	// NOTE: The "producePhotoRender" methods do not produce an image file but creates the photo in 3D space.
	//       Future requirements for producing and saving image files should be done by adding a separate method here.


	public GameObject producePhotoRender(string para_photoGObjStringName,
	                               					Photo para_photoDetails,
	                                     			float para_maxFontCharSize,
	                               					GameObject para_photoSpawnGuide,
	                               					bool para_destroyPhotoSpawnGuide)
	{
		Vector3 reqPhotoRotations = new Vector3(para_photoSpawnGuide.transform.localEulerAngles.x,
		                                        para_photoSpawnGuide.transform.localEulerAngles.y,
		                                        para_photoSpawnGuide.transform.localEulerAngles.z);
		para_photoSpawnGuide.transform.localEulerAngles = new Vector3(0,0,0);
		float reqPhotoWidth = para_photoSpawnGuide.renderer.bounds.size.x;
		float reqPhotoHeight = para_photoSpawnGuide.renderer.bounds.size.y;
		Vector3 reqPhotoCentrePt = para_photoSpawnGuide.transform.position;

		GameObject nwRender = producePhotoRender(para_photoGObjStringName,para_photoDetails,para_maxFontCharSize,reqPhotoCentrePt,reqPhotoWidth,reqPhotoHeight,reqPhotoRotations);

		if(para_destroyPhotoSpawnGuide)
		{
			MonoBehaviour.Destroy(para_photoSpawnGuide);
		}

		return nwRender;
	}


	public GameObject producePhotoRender(string para_photoGObjStringName,
												   Photo para_photoDetails,
	                                     		   float para_maxFontCharSize,
	                               				   Vector3 para_photoSpawnPt,
					                               float para_photoWidth,
					                               float para_photoHeight,
					                               Vector3 para_photoRotAngles)				                               
	{

		// Extract Details.
		int areaTypeID = para_photoDetails.getPhotoAreaTypeID();
		int areaBackgroundID = para_photoDetails.getPhotoAreaBackgroundID();
		PhotoCharacterElement questGiverInfo = para_photoDetails.getQuestGiverInfo();
		PhotoCharacterElement activityOwnerInfo = para_photoDetails.getActivityOwnerInfo();
		PlayerAvatarSettings playerAvSettings = para_photoDetails.getPlayerAvatarInfo();
		string boardText = para_photoDetails.getBoardText();





		// Select and create photo backbone.
		string areaTypeIDString = ""+areaTypeID;
		string areaBackgroundIDString = ""+areaBackgroundID;
		if((areaTypeID < 0)||(areaTypeID > 8))
		{
			areaTypeIDString = "W";
		}

		if(areaBackgroundID < 0)
		{
			areaBackgroundIDString = "0";
		}

		string pathToPhotoGuide = "Prefabs/PhotoBackbones/Area_"+areaTypeIDString+"/Area"+areaTypeIDString+"_Setup"+areaBackgroundIDString;
		Transform reqPhotoBackbone = Resources.Load<Transform>(pathToPhotoGuide);

		if(reqPhotoBackbone == null)
		{
			// Do something if photo guide not found.
			return  null;
		}
		else
		{
			Rect worldSpawnBounds = new Rect(para_photoSpawnPt.x - (para_photoWidth/2f),
			                                 para_photoSpawnPt.y + (para_photoHeight/2f),
			                                 para_photoWidth,
			                                 para_photoHeight);

			GameObject guideBackground = reqPhotoBackbone.FindChild("PhotoBackground").gameObject;
			float nwXScale = worldSpawnBounds.width / guideBackground.renderer.bounds.size.x;
			float nwYScale = worldSpawnBounds.height / guideBackground.renderer.bounds.size.y;

			//GameObject nwPhotoBackbone = WorldSpawnHelper.initObjWithinWorldBounds(reqPhotoBackbone,para_photoGObjStringName,worldSpawnBounds,para_photoSpawnPt.z,new bool[]{false,true,false});
			GameObject nwPhotoBackbone = ((Transform) MonoBehaviour.Instantiate(reqPhotoBackbone,para_photoSpawnPt,Quaternion.identity)).gameObject;
			nwPhotoBackbone.name = para_photoGObjStringName;


			// Find,replace and setup characters in the photo.

			Transform questGiverGuide = nwPhotoBackbone.transform.FindChild("QuestGiver");
			Transform activityOwnerGuide = nwPhotoBackbone.transform.FindChild("ActivityOwner");
			Transform playerAvatarGuide = nwPhotoBackbone.transform.FindChild("PlayerAvatar");
			Transform noticeBoardTAGuide = nwPhotoBackbone.transform.FindChild("PhotoNoticeBoard").FindChild("BoardTextArea");
			questGiverGuide.parent = null;
			activityOwnerGuide.parent = null;
			playerAvatarGuide.parent = null;
			noticeBoardTAGuide.parent = null;
			questGiverGuide.name += "Guide";
			activityOwnerGuide.name += "Guide";
			playerAvatarGuide.name += "Guide";

			//GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();


			int questGiverID = questGiverInfo.getCharacterID();
			int activityOwnerID = activityOwnerInfo.getCharacterID();

			Transform questGiverPrefab;
			if(questGiverID < 9)
			{
				string artNumber = "0"+(questGiverID+1);
				questGiverPrefab = Resources.Load<Transform>("Prefabs/AvatarsBigVersions/Big_PCs/Big_PC"+artNumber);
			}
			else
			{
				int tmpArtNum = ((questGiverID - 9)+1);
				string artNumber = ""+tmpArtNum;
				if(tmpArtNum < 10) { artNumber = "0" + artNumber; }
				questGiverPrefab = Resources.Load<Transform>("Prefabs/AvatarsBigVersions/Big_SCs/Big_SC"+artNumber);
			}

			Transform activityOwnerPrefab = null;
			if(questGiverID != activityOwnerID)
			{
				string artNumber = "0"+(activityOwnerID+1);
				activityOwnerPrefab = Resources.Load<Transform>("Prefabs/AvatarsBigVersions/Big_PCs/Big_PC"+artNumber);
			}

			Transform playerAvatarBasePrefab = Resources.Load<Transform>("Prefabs/AvatarsBigVersions/Big_AVs/Big_AV01"); //Resources.Load<Transform>("Prefabs/Avatars/MainAvatar01");
			Transform wordBoxPrefab = Resources.Load<Transform>("Prefabs/GenericWordBox");


			// Render player avatar in photo.
			Transform nwPlayerAvObj = (Transform) MonoBehaviour.Instantiate(playerAvatarBasePrefab,playerAvatarGuide.position,Quaternion.identity);
			nwPlayerAvObj.name = "PlayerAvatar";

			MonoBehaviour.DestroyImmediate(nwPlayerAvObj.GetComponent<Animator>());
			Animator nwPlayerAni = nwPlayerAvObj.gameObject.AddComponent<Animator>();
			nwPlayerAni.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Prefabs/Animation/Poses/AVBigController");

			ClothingApplicator cApp = nwPlayerAvObj.gameObject.AddComponent<ClothingApplicator>();
			cApp.setSubject(nwPlayerAvObj.gameObject,nwPlayerAni.runtimeAnimatorController);
			cApp.applyClothingConfig(playerAvSettings.getClothingSettings(),ClothingSize.BIG);
			MonoBehaviour.Destroy(cApp);

			nwPlayerAvObj.localScale = playerAvatarGuide.transform.localScale;


			nwPlayerAvObj.GetComponent<Animator>().Play("AVPhotoPose_"+para_photoDetails.getPlayerAvatarPoseID());
			nwPlayerAvObj.parent = nwPhotoBackbone.transform;

			// Render quest giver in photo.
			Transform nwQuestGiverObj = (Transform) MonoBehaviour.Instantiate(questGiverPrefab,questGiverGuide.position,Quaternion.identity);
			nwQuestGiverObj.name = "QuestGiver";
			nwQuestGiverObj.GetComponent<Animator>().Play("SCPhotoPose_"+questGiverInfo.getPoseID());
			nwQuestGiverObj.localScale = questGiverGuide.transform.localScale;
			nwQuestGiverObj.parent = nwPhotoBackbone.transform;

			// Render activity owner in photo.
			Transform nwActivityOwnerObj = null;
			if(activityOwnerPrefab != null)
			{
				nwActivityOwnerObj = (Transform) MonoBehaviour.Instantiate(activityOwnerPrefab,activityOwnerGuide.position,Quaternion.identity);
				nwActivityOwnerObj.name = "ActivityOwner";
				nwActivityOwnerObj.GetComponent<Animator>().Play("SCPhotoPose_"+activityOwnerInfo.getPoseID());
				nwActivityOwnerObj.localScale = activityOwnerGuide.localScale;
				nwActivityOwnerObj.parent = nwPhotoBackbone.transform;
			}

			// Render board text.
			GameObject nwBoardTextObj = WordBuilderHelper.buildWordBox(99,boardText,CommonUnityUtils.get2DBounds(noticeBoardTAGuide.renderer.bounds),noticeBoardTAGuide.transform.position.z,new bool[]{false,true,false},wordBoxPrefab);
			nwBoardTextObj.name = "PhotoBoardText";
			WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() {nwBoardTextObj},para_maxFontCharSize);
			MonoBehaviour.Destroy(nwBoardTextObj.transform.FindChild("Board").gameObject);
			nwBoardTextObj.transform.FindChild("Text").renderer.sortingOrder = 200;
			nwBoardTextObj.transform.parent = nwPhotoBackbone.transform;



			// Apply rotation.
			nwPhotoBackbone.transform.eulerAngles = new Vector3(para_photoRotAngles.x,para_photoRotAngles.y,para_photoRotAngles.z);

			// Apply scale.
			nwPhotoBackbone.transform.localScale = new Vector3(nwXScale,nwYScale,1);

			// Set speed of all animators to 0.
			if(nwQuestGiverObj.GetComponent<Animator>() != null) { nwQuestGiverObj.GetComponent<Animator>().speed = 0; }
			if(nwPlayerAvObj.GetComponent<Animator>() != null) { nwPlayerAvObj.GetComponent<Animator>().speed = 0; }
			if((nwActivityOwnerObj != null)&&(nwActivityOwnerObj.GetComponent<Animator>() != null)) { nwActivityOwnerObj.GetComponent<Animator>().speed = 0; }


			// TMP - Remove This Later - Remove any shadow child objects.
			if(nwQuestGiverObj != null) { removeShadowObjects(nwQuestGiverObj.gameObject); }
			if(nwActivityOwnerObj != null) { removeShadowObjects(nwActivityOwnerObj.gameObject); }
			//if(nwPlayerAvObj != null) { removeShadowObjects(nwPlayerAvObj); }

			// TMP - Remove This Later - Update all character rend order so that they appear in front of everything.
			CommonUnityUtils.setSortingOrderOfEntireObject(nwQuestGiverObj.gameObject,502);
			if(nwActivityOwnerObj != null)
			{
				CommonUnityUtils.setSortingOrderOfEntireObject(nwActivityOwnerObj.gameObject,502);
			}
			CommonUnityUtils.setSortingOrderOfEntireObject(nwPlayerAvObj.gameObject,502);


			// Destroy guides.
			MonoBehaviour.Destroy(questGiverGuide.gameObject);
			MonoBehaviour.Destroy(activityOwnerGuide.gameObject);
			MonoBehaviour.Destroy(playerAvatarGuide.gameObject);
			MonoBehaviour.Destroy(noticeBoardTAGuide.gameObject);

			return nwPhotoBackbone;
		}
	}

	// Might have to readd poses here aswell.
	public void makeAllCharactersStill(GameObject para_photoObj)
	{
		List<string> itemsToCheck = new List<string>() { "QuestGiver","PlayerAvatar","ActivityOwner" };

		for(int i=0; i<itemsToCheck.Count; i++)
		{
			Transform tmpItem = para_photoObj.transform.FindChild(itemsToCheck[i]);
			if(tmpItem != null)
			{
				if(tmpItem.GetComponent<Animator>() != null) { tmpItem.GetComponent<Animator>().speed = 0; }
			}
		}
	}

	private void removeShadowObjects(GameObject para_gObj)
	{
		Transform reqTrans = para_gObj.transform;

		List<string> childObjsToDestroy = new List<string>();
		for(int i=0; i<reqTrans.childCount; i++)
		{
			Transform reqChild = reqTrans.GetChild(i);
			if((reqChild.name.Contains("shadow"))||(reqChild.name.Contains("Shadow"))||(reqChild.name.Contains("SHADOW")))
			{
				childObjsToDestroy.Add(reqChild.name);
			}
		}

		for(int i=0; i<childObjsToDestroy.Count; i++)
		{
			MonoBehaviour.Destroy(reqTrans.FindChild(childObjsToDestroy[i]).gameObject);
		}
	}
}
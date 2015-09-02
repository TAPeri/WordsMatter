/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

// Should be attached directly on the bystander object of choice.
public class WaitAndCallStateScript : MonoBehaviour, CustomActionListener
{
	// Step 1: Move to the board edge.
	// Step 2: Create exclamation mark bubble.
	// Step 3: Create and float instruction sphere.
	// Step 4: Instruct main script that player can now move.

	string targetSide;
	Transform exclamationMarkPrefab;
	GameObject instructionTile;
	Rect finalBoundsForInstructionTile;

	GameObject tmpEMarkInstance;


	public void init(string para_targetSide,
					 Vector3 para_ptNearBoardEdge,
	                 Transform para_exclamationMarkPrefab,
	                 GameObject para_instructionTile,
	                 Rect para_finalBoundsForInstructionTile)
	{
		targetSide = para_targetSide;
		exclamationMarkPrefab = para_exclamationMarkPrefab;
		instructionTile = para_instructionTile;
		finalBoundsForInstructionTile = para_finalBoundsForInstructionTile;

		moveToBoardEdge(para_ptNearBoardEdge);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "PathComplete")
		{
			NewCharacterNavMovement cnm = transform.GetComponent<NewCharacterNavMovement>();
			cnm.unregisterListener("WaitAndCall");
			showExclamationMark();
		}
		else if(para_eventID == "DelayEnd")
		{
			Destroy(tmpEMarkInstance);
			createInstructionSphere();
		}
		else if(para_eventID == "InstructionSphereAni")
		{
			instructionTile.transform.parent = Camera.main.transform;
			sendPlayerCanMoveSignal();
		}
	}

	private void moveToBoardEdge(Vector3 para_ptNearBoardEdge)
	{
		NewCharacterNavMovement cnm = transform.GetComponent<NewCharacterNavMovement>();
		if(cnm == null)
		{
			cnm = transform.gameObject.AddComponent<NewCharacterNavMovement>();
			cnm.doNotDestroy = true;
		}
		cnm.registerListener("WaitAndCall",this);
		cnm.moveToWorldPoint(para_ptNearBoardEdge,2,false);
	}

	private void showExclamationMark()
	{
		Vector3 exclamationMarkSpawnPt = new Vector3(transform.position.x + 0.2f,transform.position.y + 0.9f,transform.position.z);
		Transform nwMark = (Transform) Instantiate(exclamationMarkPrefab,exclamationMarkSpawnPt,Quaternion.identity);
		nwMark.name = "BystanderBubble";
		nwMark.parent = transform;
		tmpEMarkInstance = nwMark.gameObject;

		NewCharacterNavMovement cnm = transform.GetComponent<NewCharacterNavMovement>();
		string faceDir = "S";
		if(targetSide == "T") { faceDir = "S"; }
		if(targetSide == "L") { faceDir = "E"; }
		if(targetSide == "R") { faceDir = "W"; }
		
		// Apply set idle left, right or south here.
		cnm.faceDirection(faceDir);
		
		DelayForInterval dfi = transform.gameObject.AddComponent<DelayForInterval>();
		dfi.registerListener("WaitAndCall",this);
		dfi.init(1f);
	}

	private void createInstructionSphere()
	{
		instructionTile.transform.parent = null;
		instructionTile.renderer.enabled = true;

		Transform textChild = instructionTile.transform.FindChild("InstructionText");
		if(textChild != null) { textChild.FindChild("Text").renderer.enabled = true; }
		else { instructionTile.transform.FindChild("TtsIcon").renderer.enabled = true; }

		//float sphereWidth = 0.4f;
		//Rect sphereStartBounds = new Rect(transform.position.x - (sphereWidth/2f),transform.position.y + (sphereWidth/2f),sphereWidth,sphereWidth);
		//Rect sphereDestBounds = finalBoundsForInstructionTile;

		Vector3 destPos = new Vector3(finalBoundsForInstructionTile.x + (finalBoundsForInstructionTile.width/2f),
		                              finalBoundsForInstructionTile.y - (finalBoundsForInstructionTile.height/2f),
		                              transform.position.z);

		CustomAnimationManager aniMang = instructionTile.AddComponent<CustomAnimationManager>();
		List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
		List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
		//batch1.Add(new AniCommandPrep("ResizeToWorldSize",1,new List<System.Object>() { new float[3]{sphereDestBounds.width,sphereDestBounds.height,1}, new float[3]{sphereStartBounds.width,sphereStartBounds.height,1} }));
		batch1.Add(new AniCommandPrep("InjectLocalScale",1,new List<System.Object>() { new float[3]{0.2f,0.2f,1}}));
		batch1.Add(new AniCommandPrep("TeleportToLocation",1,new List<System.Object>() { new float[3]{transform.position.x,transform.position.y, transform.position.z} }));
		List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
		batch2.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3]{transform.position.x,transform.position.y + 0.4f,transform.position.z}, 0.2f, true }));
		List<AniCommandPrep> batch3 = new List<AniCommandPrep>();
		batch3.Add(new AniCommandPrep("MoveToLocation",2,new List<System.Object>() { new float[3]{destPos.x,destPos.y,destPos.z}, 0.4f, true }));
		batch3.Add(new AniCommandPrep("GrowProportionateToDistance",1,new List<System.Object>() { new float[3]{destPos.x,destPos.y,destPos.z},new float[3]{1,1,1} }));
		batchLists.Add(batch1);
		batchLists.Add(batch2);
		batchLists.Add(batch3);
		aniMang.registerListener("WaitAndCall",this);
		aniMang.init("InstructionSphereAni",batchLists);
	}

	private void sendPlayerCanMoveSignal()
	{
		GameObject.Find("GlobObj").GetComponent<AcMovingPathwaysScenario>().endWaitProcedure();
		Destroy(this);
	}
}

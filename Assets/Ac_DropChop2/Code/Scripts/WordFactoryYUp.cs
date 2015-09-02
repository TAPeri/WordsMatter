/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class WordFactoryYUp : MonoBehaviour
{
	public static GameObject spawnSplittableWordBoard(string para_word,
													  Vector3 para_spawnLoc,
													  float para_letterWidthInWorldSpace,
													  float para_spacingWidthWorldSpace,
													  float para_maxWidthInWorldSpace,
													  bool[] para_freezePositionFlags,
													  bool[] para_wordUpAxis,
													  Transform para_textPlanePrefab,
													  Transform para_splitDetectorPrefab,
													  Transform[] para_sliceableObjPrefabArr,
													  Transform para_foregroundPlanePrefab,
													  Transform para_framePrefab,
	                                                  Transform para_glowBoxPrefab)
	{
		
		
		
		// **** Create master object. ****
		GameObject masterWordGObj = new GameObject();
		masterWordGObj.name = "MasterWordGObj:"+"0"+"-"+(para_word.Length-1);
		masterWordGObj.tag = "MasterObj";
		masterWordGObj.transform.position = para_spawnLoc;
		masterWordGObj.AddComponent(typeof(Rigidbody));
		
		Rigidbody rBod = (Rigidbody) masterWordGObj.GetComponent(typeof(Rigidbody));
		rBod.constraints = RigidbodyConstraints.FreezeRotation;
		if(para_freezePositionFlags[0])
			rBod.constraints = rBod.constraints | RigidbodyConstraints.FreezePositionX;	
		if(para_freezePositionFlags[1])
			rBod.constraints = rBod.constraints | RigidbodyConstraints.FreezePositionY;
		if(para_freezePositionFlags[2])
			rBod.constraints = rBod.constraints | RigidbodyConstraints.FreezePositionZ;
		// ********************************
		
		
		
		
		// **** Determine word plane bounds. ****
		GameObject wordOverlayGObj = new GameObject();
		wordOverlayGObj.name = "WordOverlay";
		string wordStr = para_word;
		
		
		float widthOfWordPlane;
		float widthOfLetterCell;
		float widthOfSpacer;
		float widthOfSplitDetect;
		
		
		float maxWidthInWorldSpace =  para_maxWidthInWorldSpace;
		if(maxWidthInWorldSpace < 0)
		{
			widthOfLetterCell = para_letterWidthInWorldSpace;
			widthOfSplitDetect = widthOfLetterCell * 0.5f;
			widthOfSpacer = para_spacingWidthWorldSpace;
			widthOfWordPlane = (widthOfLetterCell * (wordStr.Length)) + (widthOfSpacer * (wordStr.Length-1));
		}
		else
		{
			widthOfLetterCell = para_letterWidthInWorldSpace;
			widthOfSplitDetect = widthOfLetterCell * 0.5f;
			widthOfSpacer = para_spacingWidthWorldSpace;
			widthOfWordPlane = (widthOfLetterCell * (wordStr.Length)) + (widthOfSpacer * (wordStr.Length-1));
			
			
			if(widthOfWordPlane > maxWidthInWorldSpace)
			{
				widthOfWordPlane = maxWidthInWorldSpace;
				widthOfLetterCell = widthOfWordPlane/(wordStr.Length * 1.0f);
				widthOfSpacer = para_spacingWidthWorldSpace;
				widthOfSplitDetect = widthOfLetterCell * 0.5f;
			}
		}
		
		
		Vector3 wordPlaneLeftEdge = para_spawnLoc - new Vector3(widthOfWordPlane/2f,0,0);
		// ********************************
		
		
		
		// **** Spawn letter planes ****
		Vector3 nxtLetterPlaneSpawnPt = wordPlaneLeftEdge + new Vector3(widthOfLetterCell/2f,0f,-(para_sliceableObjPrefabArr[0].localScale.z/2f));
		Vector3 creepVect = new Vector3(widthOfLetterCell + widthOfSpacer,0f,0f);
		float[] rotVals = new float[3] {0,0,0};
		if(para_wordUpAxis[1])
		{
			nxtLetterPlaneSpawnPt = wordPlaneLeftEdge + new Vector3(widthOfLetterCell/2f,0f,-(para_sliceableObjPrefabArr[0].localScale.z/2f));
			rotVals = new float[3] {0,0,0};
		}
		else if(para_wordUpAxis[2])
		{
			nxtLetterPlaneSpawnPt = wordPlaneLeftEdge + new Vector3(widthOfLetterCell/2f,(para_sliceableObjPrefabArr[0].localScale.y/2f),0f);
			rotVals = new float[3] {90,0,0};
		}
		
		
		for(int i=0; i<wordStr.Length; i++)
		{
			char nxtLetter = wordStr[i];
			
			
			GameObject nwLetterGObj = ((Transform) Instantiate(para_textPlanePrefab,nxtLetterPlaneSpawnPt,Quaternion.identity)).gameObject;
			nwLetterGObj.name = "Letter-"+i;

			nwLetterGObj.renderer.sortingOrder = 101;
			
			nwLetterGObj.transform.localEulerAngles = new Vector3(rotVals[0],rotVals[1],rotVals[2]);
			
			Vector3 tmpScale = nwLetterGObj.transform.localScale;
			tmpScale.x = widthOfLetterCell;
			tmpScale.y = widthOfLetterCell;
			tmpScale.z = widthOfLetterCell;
			nwLetterGObj.transform.localScale = tmpScale;
			
			TextMesh tMesh = (TextMesh) (nwLetterGObj.GetComponent(typeof(TextMesh)));
			tMesh.text = ""+nxtLetter;
			tMesh.renderer.material.color = Color.black;
			
			
			nwLetterGObj.transform.parent = wordOverlayGObj.transform;
			
			nxtLetterPlaneSpawnPt += creepVect;
		}
		// ********************************
		
		
		
		// **** Spawn split detectors ****
		Vector3 nxtSplitDetSpawnPt = wordPlaneLeftEdge + new Vector3(widthOfLetterCell + widthOfSpacer,0,-(para_sliceableObjPrefabArr[0].localScale.z/2f));
		if(para_wordUpAxis[1])
		{
			nxtSplitDetSpawnPt = wordPlaneLeftEdge + new Vector3(widthOfLetterCell,0,-(para_sliceableObjPrefabArr[0].localScale.z/2f));
		}
		else if(para_wordUpAxis[2])
		{
			nxtSplitDetSpawnPt = wordPlaneLeftEdge + new Vector3(widthOfLetterCell,(para_sliceableObjPrefabArr[0].localScale.y/2f),0f);
		}
		
		
		for(int i=0; i<(wordStr.Length-1); i++)
		{
			GameObject nwSplitDetectGObj = ((Transform) Instantiate(para_splitDetectorPrefab,nxtSplitDetSpawnPt,Quaternion.identity)).gameObject;
			nwSplitDetectGObj.name = "SplitDet-"+i;
			
			nwSplitDetectGObj.transform.localEulerAngles = new Vector3(rotVals[0],rotVals[1],rotVals[2]);
			
			Vector3 tmpScale = nwSplitDetectGObj.transform.localScale;
			tmpScale.x = widthOfSplitDetect;
			tmpScale.y = widthOfSplitDetect * 2f;
			tmpScale.z = widthOfSplitDetect * 10f;
			nwSplitDetectGObj.transform.localScale = tmpScale;
			
			nwSplitDetectGObj.transform.parent = wordOverlayGObj.transform;
			
			nxtSplitDetSpawnPt += creepVect;
		}
		
		wordOverlayGObj.transform.parent = masterWordGObj.transform;
		// ********************************
		
		
			
		
		// **** Create design centric object associated with word. Eg. A wooden log. ****
		Vector3 nxtBlockSpawnPt = wordPlaneLeftEdge + new Vector3(widthOfLetterCell/2f,0,0);		
		for(int i=0; i<wordStr.Length; i++)
		{
			Transform reqPrefab = null;
			if(i == 0) { reqPrefab = para_sliceableObjPrefabArr[0]; }
			else if(i == (wordStr.Length-1)) { reqPrefab = para_sliceableObjPrefabArr[2]; }
			else { reqPrefab = para_sliceableObjPrefabArr[1]; }

			GameObject nwDesignGObj = ((Transform) Instantiate(reqPrefab,nxtBlockSpawnPt,Quaternion.identity)).gameObject;
			nwDesignGObj.name = "WordDesignRelatedGObj-"+i;
			
			nwDesignGObj.transform.localEulerAngles = new Vector3(rotVals[0],rotVals[1],rotVals[2]);
						
			Vector3 tmpScale = nwDesignGObj.transform.localScale;
			tmpScale.x = widthOfLetterCell / reqPrefab.renderer.bounds.size.x;
			tmpScale.y = widthOfLetterCell / reqPrefab.renderer.bounds.size.y;
			tmpScale.z = widthOfLetterCell / reqPrefab.renderer.bounds.size.z;
			nwDesignGObj.transform.localScale = tmpScale;
			
			//Destroy(nwDesignGObj.GetComponent(typeof(BoxCollider)));
		
			nwDesignGObj.transform.parent = masterWordGObj.transform;
				
			nxtBlockSpawnPt += creepVect;
		}
		// ********************************
		
		
		// **** Create spacers. ****
		Vector3 nxtSpacerSpawnPt = wordPlaneLeftEdge + new Vector3(widthOfLetterCell+(widthOfSpacer/2f),0,0);		
		for(int i=0; i<(wordStr.Length-1); i++)
		{
			GameObject nwSpacerGObj = ((Transform) Instantiate(para_sliceableObjPrefabArr[1],nxtSpacerSpawnPt,Quaternion.identity)).gameObject;
			nwSpacerGObj.name = "Spacer-"+i;
			
			nwSpacerGObj.transform.localEulerAngles = new Vector3(rotVals[0],rotVals[1],rotVals[2]);
						
			Vector3 tmpScale = nwSpacerGObj.transform.localScale;
			tmpScale.x = widthOfSpacer;
			tmpScale.y = widthOfLetterCell * 0.80f;
			tmpScale.z = widthOfLetterCell;
			nwSpacerGObj.transform.localScale = tmpScale;
			
			Destroy (nwSpacerGObj.GetComponent(typeof(BoxCollider)));
		
			nwSpacerGObj.transform.parent = masterWordGObj.transform;
				
			nxtSpacerSpawnPt += creepVect;
		}
		// ********************************
		


		/*// **** Create glowbox ****
		Vector3 glowSpawnPt = wordPlaneLeftEdge + new Vector3(widthOfWordPlane/2f,0,0) + new Vector3(0,0, ((widthOfLetterCell/2f) + (widthOfLetterCell * 0.01f)));
		GameObject glowObj = ((Transform) Instantiate(para_glowBoxPrefab,glowSpawnPt,Quaternion.identity)).gameObject;
		glowObj.name = "TheGlow";
		GameObject glowChild = (para_glowBoxPrefab.FindChild("GlowBox")).gameObject;
		Vector3 tmpScale2 = glowObj.transform.localScale;
		tmpScale2.x = (widthOfWordPlane) / (glowChild.renderer.bounds.size.x + 0.6f);
		tmpScale2.y = (widthOfLetterCell + (widthOfLetterCell * 0.2f)) / (glowChild.renderer.bounds.size.y + 0.6f);
		tmpScale2.z = (widthOfWordPlane + (widthOfWordPlane * 0.1f)) / (glowChild.renderer.bounds.size.z);
		glowObj.transform.localScale = tmpScale2;
		glowObj.transform.parent = masterWordGObj.transform;
		// *********************************/

		
		// **** Create frame ****
		//WordFactoryYUp.createFrame(masterWordGObj,wordStr.Length,widthOfLetterCell,widthOfWordPlane,wordPlaneLeftEdge,creepVect,para_framePrefab);
		// ********************************
		
		
		
		// Return finished constructed object.
		return masterWordGObj;
	}
	
	
	
	/*public static List<GameObject> splitEntireWord(string para_wordGObjName, int[] para_splitPositions, Transform para_framePrefab)
	{
		GameObject parentWordGObj = GameObject.Find(para_wordGObjName);
		
		int currSplitPos = para_splitPositions[0];
		GameObject currGObjToSplit = parentWordGObj;
		List<GameObject> retSplitList = new List<GameObject>();
		List<GameObject> nwSplits = null;
		int currTraversed = 0;
		for(int i=0; i<para_splitPositions.Length; i++)
		{
			nwSplits = WordFactoryYUp.performSplitOnWordBoard(ref currGObjToSplit,currSplitPos,para_framePrefab);
			retSplitList.Add(nwSplits[0]);
			currTraversed += (para_splitPositions[i]+1);
			
			if(i < (para_splitPositions.Length-1))
			{
				currGObjToSplit = nwSplits[1];
				currSplitPos = para_splitPositions[i+1] - currTraversed;
			}
		}
		
		return retSplitList;
	}*/
	
		
	public static List<GameObject> performSplitOnWordBoard(ref GameObject para_hitSplitDetector, Transform para_framePrefab, Transform para_glowBoxPrefab)
	{
		GameObject currMasterOverlay = para_hitSplitDetector.transform.parent.gameObject;
		GameObject currMasterMaster = currMasterOverlay.transform.parent.gameObject;
		
		int splitIndex = int.Parse(para_hitSplitDetector.name.Split('-')[1]);
		
		return WordFactoryYUp.performSplitOnWordBoard(ref currMasterMaster,splitIndex,para_framePrefab,para_glowBoxPrefab);
	}
	
	public static List<GameObject> performSplitOnWordBoard(ref GameObject para_wordBoardToSplit, int para_splitPos, Transform para_framePrefab, Transform para_glowBoxPrefab)
	{	
		GameObject currMasterMaster = para_wordBoardToSplit;
		GameObject currMasterOverlay = (currMasterMaster.transform.FindChild("WordOverlay")).gameObject;
		
			
		string[] splitMName = currMasterMaster.name.Split(':');
		string[] wordLetterStartNStop = splitMName[1].Split('-');
		int masterWordStart = int.Parse(wordLetterStartNStop[0]);
		int masterWordEnd = int.Parse(wordLetterStartNStop[1]);
		
		int half1_wordStart = masterWordStart;
		int half1_wordEnd = para_splitPos;
		
		int half2_wordStart = para_splitPos+1;
		int half2_wordEnd = masterWordEnd;
		
		
		List<GameObject> retSplitObjs = new List<GameObject>();
		GameObject half1GObj = createSplitHalf(currMasterMaster,currMasterOverlay,half1_wordStart,half1_wordEnd,para_framePrefab,para_glowBoxPrefab);
		GameObject half2GObj = createSplitHalf(currMasterMaster,currMasterOverlay,half2_wordStart,half2_wordEnd,para_framePrefab,para_glowBoxPrefab);
		retSplitObjs.Add(half1GObj);
		retSplitObjs.Add(half2GObj);
		
		
		/*// Create Exploader:
		Instantiate(exploaderPrefab,hitInfo.collider.gameObject.transform.position,Quaternion.identity);
		Transform nwSparks = (Transform) Instantiate(sparksPrefab,hitInfo.collider.gameObject.transform.position + new Vector3(0,0,-1),Quaternion.identity);
		nwSparks.localScale = new Vector3(5,5,5);
		triggerSoundAtCamera("wood chop 1");//triggerSoundAtCamera("pop");*/
		
		// Destroy old objects.
		Destroy(currMasterMaster); // Brings down everything with it.	
		
		return retSplitObjs;
	}
	
	
	public static GameObject createSplitHalf(GameObject para_mmObj,
							   			     GameObject para_wordOverlayObj,
											 int para_halfWStart,
											 int para_halfWEnd,
											 Transform para_framePrefab,
	                                         Transform para_glowBoxPrefab)
	{
		// 1. World Overlay: Text Meshes
		// 	       		     Split Detectors
		// 2. Design Object
		
		GameObject currMasterMaster = para_mmObj;
		GameObject currMasterOverlay = para_wordOverlayObj;
		
		int half_wordStart = para_halfWStart;
		int half_wordEnd = para_halfWEnd;
		
		
		
		
		GameObject fC = currMasterMaster.transform.FindChild("WordDesignRelatedGObj-"+half_wordStart).gameObject;
		float leftMostPosX = fC.transform.position.x - (fC.transform.renderer.bounds.size.x/2f);
		Vector3 leftMostPos = new Vector3(leftMostPosX,fC.transform.position.y,fC.transform.position.z);
		int half_length = half_wordEnd - half_wordStart + 1;
		float wordCellWidth = (fC.transform.renderer.bounds.size.x);
		float totSizeOfHalf = (wordCellWidth * half_length) + ((wordCellWidth * 0.1f) * (half_length-1));
		Vector3 half_centrePos = leftMostPos + new Vector3((totSizeOfHalf)/2f,0,0);
		
		
		GameObject half_masterWordGObj = new GameObject();
		string parentPrefix = (para_mmObj.name.Split(':')[0]);
		half_masterWordGObj.name = parentPrefix+":"+half_wordStart+"-"+half_wordEnd;// "MasterWordGObj:"+half_wordStart+"-"+half_wordEnd;
		half_masterWordGObj.tag = "MasterObj";
		half_masterWordGObj.transform.position = half_centrePos;
		//half_masterWordGObj.AddComponent(typeof(DestroyOnFloorCol));
		half_masterWordGObj.AddComponent(typeof(Rigidbody));
		Rigidbody rBod = (Rigidbody) half_masterWordGObj.GetComponent(typeof(Rigidbody));
		//rBod.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;  //| RigidbodyConstraints.FreezePositionY;
		rBod.constraints = currMasterMaster.rigidbody.constraints;
		
		
		GameObject half_wordOverlayGObj = new GameObject();
		half_wordOverlayGObj.name = "WordOverlay";
		
		// Adjust Letter Plane Parent References.
		for(int i=half_wordStart; i<half_wordEnd+1; i++)
		{
			GameObject lPaneGObj = currMasterOverlay.transform.FindChild("Letter-"+i).gameObject;
			lPaneGObj.transform.parent = half_wordOverlayGObj.transform;
		}
		
		// Adjust Split Detector and Spacer Parent References.
		//float widthOfLetterCell = fC.transform.renderer.bounds.size.x;
		//float widthOfSpacer = widthOfLetterCell * 0.1f;
		for(int i=half_wordStart; i<half_wordEnd; i++)
		{
			GameObject splitDetGObj = currMasterOverlay.transform.FindChild("SplitDet-"+i).gameObject;
			splitDetGObj.transform.parent = half_wordOverlayGObj.transform;
			
			GameObject spacerGObj = currMasterMaster.transform.FindChild("Spacer-"+i).gameObject;
			if(spacerGObj != null)
			{
				spacerGObj.transform.parent = half_masterWordGObj.transform;
				//widthOfSpacer = spacerGObj.transform.renderer.bounds.size.x;
			}
		}
		
		half_wordOverlayGObj.transform.parent = half_masterWordGObj.transform;
		
		// Adjust Design Objects Parent References.
		for(int i=half_wordStart; i<half_wordEnd+1; i++)
		{
			GameObject designGObj = currMasterMaster.transform.FindChild("WordDesignRelatedGObj-"+i).gameObject;
			designGObj.transform.parent = half_masterWordGObj.transform;


		}
		

		
		
	
		return half_masterWordGObj;
	}
	
	public static void createFrame(GameObject para_parentObj,
								 int para_wordLength,
								 float para_widthOfLetterCell,
								 float para_widthOfWordPlane,
								 Vector3 para_wordPlaneLeftEdge,
								 Vector3 para_creepVect,
								 Transform para_framePrefab)
	{
		//Transform framePrefab = para_framePrefab;
		GameObject masterWordGObj = para_parentObj;
		int wordLength = para_wordLength;
		float widthOfLetterCell = para_widthOfLetterCell;
		float widthOfWordPlane = para_widthOfWordPlane;
		Vector3 wordPlaneLeftEdge = para_wordPlaneLeftEdge;
		Vector3 creepVect = para_creepVect;
		
		
		// **** Create frame ****
		GameObject frameGObj = new GameObject("Frame");
		
		float frameBorderThicknessPerc = 0.1f;
		float frameBorderThickness = widthOfLetterCell * frameBorderThicknessPerc;
		
		// Top border:
		Vector3 frameTopLeft = wordPlaneLeftEdge + new Vector3(0,0,- ((widthOfLetterCell/2f) + (widthOfLetterCell * 0.01f))) + new Vector3(0,(widthOfLetterCell/2f),0);
		
		List<Rect> frameItems = new List<Rect>();
		frameItems.Add(new Rect(frameTopLeft.x,frameTopLeft.y,widthOfWordPlane,frameBorderThickness));
		frameItems.Add(new Rect(frameTopLeft.x,frameTopLeft.y,frameBorderThickness,widthOfLetterCell));
		frameItems.Add(new Rect(frameTopLeft.x + widthOfWordPlane - frameBorderThickness,frameTopLeft.y,frameBorderThickness,widthOfLetterCell));
		frameItems.Add(new Rect(frameTopLeft.x,frameTopLeft.y - widthOfLetterCell + frameBorderThickness,widthOfWordPlane,frameBorderThickness));
		
		Vector3 nxtFrameSpacerSpawnPt = frameTopLeft + new Vector3(widthOfLetterCell,0,0);		
		for(int i=0; i<(wordLength-1); i++)
		{
			frameItems.Add(new Rect(nxtFrameSpacerSpawnPt.x,nxtFrameSpacerSpawnPt.y,frameBorderThickness,widthOfLetterCell));	
			nxtFrameSpacerSpawnPt += creepVect;
		}
		
		for(int i=0; i<frameItems.Count; i++)
		{
			GameObject nwFramePart = WorldSpawnHelper.initObjWithinWorldBounds(para_framePrefab,1,1,"FramePart",frameItems[i],null,frameTopLeft.z,new bool[3]{false,true,false});
			Vector3 tmpScale = nwFramePart.transform.localScale;
			tmpScale.z = 0.1f;
			nwFramePart.transform.localScale = tmpScale;
			nwFramePart.transform.parent = frameGObj.transform;
		}
		
		frameGObj.transform.parent = masterWordGObj.transform;	
		// ********************************
	}
}

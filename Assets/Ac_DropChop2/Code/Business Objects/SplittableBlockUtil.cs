/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class SplittableBlockUtil
{
	// Block Structure:
	//
	// []  Main Game Object Parent 					(eg: "Block-2:0-1")
	// 	   - Spacer list  							(eg: "Spacer-0")
	//	   - World related obj list 				(eg: "WordDesignRelatedGObj-0")
	//	   [] Word Overlay Child Object 			("WordOverlay").
	//	   		- Letter Pane list 					(eg: "Letter-0")
	//			- Split Detector list 				(eg: "SplitDet-0")
	//



	public static void setStateOfSplitDetectsForWord(string para_wordBlockName, bool para_state)
	{
		GameObject reqGObj = GameObject.Find(para_wordBlockName);
		setStateOfSplitDetectsForWord(reqGObj,para_state);
	}

	public static void setStateOfSplitDetectsForWord(GameObject para_wordBlockGObj, bool para_state)
	{
		GameObject reqGObj = para_wordBlockGObj;
		GameObject wordOverlayGObj = (reqGObj.transform.FindChild("WordOverlay")).gameObject;
		
		bool continueFlag = true;
		int counter = 0;
		while(continueFlag)
		{
			Transform splitDetObjTrans = wordOverlayGObj.transform.FindChild("SplitDet-"+counter);
			if(splitDetObjTrans != null)
			{
				splitDetObjTrans.gameObject.SetActive(para_state);
			}
			else
			{
				continueFlag = false;
			}
			counter++;
		}
	}	

	

	/*private void autoSplitWordSegmentRecursively(string para_wordSegID, List<int> para_validSplitLocations)
	{
		if((para_validSplitLocations == null)||(para_validSplitLocations.Count == 0))
		{
			return;
		}
		else
		{
			string suffix = para_wordSegID.Split(':')[1];
			string[] suffixParts = suffix.Split('-');
			int[] parsedLetterRange = new int[2] { int.Parse(suffixParts[0]), int.Parse(suffixParts[1]) };
			
			if(parsedLetterRange[0] != parsedLetterRange[1])
			{
				for(int i=0; i<para_validSplitLocations.Count; i++)
				{
					if((para_validSplitLocations[i] >= parsedLetterRange[0])&&(para_validSplitLocations[i] < parsedLetterRange[1]))
					{
						List<GameObject> nwSplitObjs = triggerCorrectSplitConsequences(para_wordSegID,para_validSplitLocations[i]);
						
						for(int k=0; k<nwSplitObjs.Count; k++)
						{
							autoSplitWordSegmentRecursively(nwSplitObjs[k].name,para_validSplitLocations);
						}
						break;
					}
				}
			}
		}
	}*/



	public static Bounds getSplitDetectWorldBounds(string para_wordSegID, int para_splitDetID)
	{
		Bounds retBnds = new Bounds();

		GameObject reqSplitDet = ((GameObject.Find(para_wordSegID).transform.FindChild("WordOverlay")).FindChild("SplitDet-"+para_splitDetID)).gameObject;
		if(reqSplitDet != null)
		{
			retBnds = reqSplitDet.renderer.bounds;
		}

		return retBnds;
	}


	public static void applyColorToBlock(GameObject para_obj, Color para_nwColor)
	{
		List<SpriteRenderer> reqList = getSpriteRendsOfChildrenRecursively(para_obj);
		for(int i=0; i<reqList.Count; i++)
		{
			reqList[i].color = para_nwColor;
		}
	}

	public static List<SpriteRenderer> getSpriteRendsOfChildrenRecursively(GameObject para_tmpObj)
	{
		List<SpriteRenderer> localList = new List<SpriteRenderer>();
		
		SpriteRenderer tmpSRend = null;
		tmpSRend = para_tmpObj.GetComponent<SpriteRenderer>();
		if(tmpSRend != null)
		{
			localList.Add(tmpSRend);
		}
		
		for(int i=0; i<para_tmpObj.transform.childCount; i++)
		{
			List<SpriteRenderer> tmpRecList = getSpriteRendsOfChildrenRecursively((para_tmpObj.transform.GetChild(i)).gameObject);
			localList.AddRange(tmpRecList);
		}			
		
		return localList;
	}

}

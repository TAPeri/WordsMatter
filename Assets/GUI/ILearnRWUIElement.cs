/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ILearnRWUIElement : ILearnRWScenario
{
	protected Dictionary<string,string> textContent;
	Transform wordBoxPrefab;
	protected bool[] upAxisArr = {false,true,false};

	protected int uiRender_baseSortingLayer = 5000;


	private void init()
	{
		textContent = new Dictionary<string, string>();
		wordBoxPrefab = Resources.Load<Transform>("Prefabs/GenericWordBox");
	}

	protected int fixRenderSortingForElements(List<string[]> para_layers) { return fixRenderSortingForElements(para_layers,uiRender_baseSortingLayer,null); }
	protected int fixRenderSortingForElements(List<string[]> para_layers, int para_startingLayerID) { return fixRenderSortingForElements(para_layers,para_startingLayerID,null); }
	protected int fixRenderSortingForElements(List<string[]> para_layers, int para_startingLayerID, string para_pathToGameObj)
	{
		List<int> sortingOrderList = new List<int>();
		for(int i=0; i<para_layers.Count; i++)
		{
			if(i==0)
			{
				sortingOrderList.Add(para_startingLayerID);
			}
			else
			{
				sortingOrderList.Add(sortingOrderList[i-1]+1);
			}
		}

		Transform reqParentGObj = getReqParentGObj(para_pathToGameObj);

		if(reqParentGObj != null)
		{
			for(int i=0; i<sortingOrderList.Count; i++)
			{
				string[] layerItems = para_layers[i];

				for(int j=0; j<layerItems.Length; j++)
				{
					string tmpItemName = layerItems[j];
					if(tmpItemName != null)
					{
						Transform tmpItem = reqParentGObj.FindChild(tmpItemName);
						if(tmpItem != null)
						{
							tmpItem.gameObject.renderer.sortingOrder = sortingOrderList[i];
						}
					}
				}
			}
		}

		int nxtAvailableLayerID = uiRender_baseSortingLayer;
		if((sortingOrderList != null)&&(sortingOrderList.Count > 0)) { nxtAvailableLayerID = sortingOrderList[sortingOrderList.Count-1] + 1; }
		return nxtAvailableLayerID;
	}

	protected void prepTextElements(string[] para_elementNames,
	                                string[] para_elementContent,
	                                bool[] para_destroyGuideFlags,
	                                int[] para_textElementTypeArr,
	                                string para_pathToGameObj)
	{
		for(int i=0; i<para_elementNames.Length; i++)
		{
			if(para_textElementTypeArr[i] == 0)
			{
				populateGUIElement(para_elementNames[i],para_elementContent[i],para_destroyGuideFlags[i],para_pathToGameObj);
			}
			else
			{
				populateWorldTextElement(para_elementNames[i],para_elementContent[i],para_destroyGuideFlags[i],para_pathToGameObj);
			}
		}
	}

	protected void populateGUIElement(string para_elementName,
	                                  string para_elementContent,
	                                  bool para_destroyGuide,
	                                  string para_pathToGameObj)
	{
		if(textContent == null) { this.init(); }

		Transform reqParentGObj = getReqParentGObj(para_pathToGameObj);
		if(reqParentGObj != null)
		{
			Transform elementGObj = reqParentGObj.FindChild(para_elementName);
			if(elementGObj != null)
			{
				//Debug.Log(para_elementName);
				//elementGObj.parent = null;
				Rect elementGUIBounds = WorldSpawnHelper.getWorldToGUIBounds(elementGObj.renderer.bounds,upAxisArr);
				if(uiBounds == null) { uiBounds = new Dictionary<string, Rect>(); }
				if(uiBounds.ContainsKey(para_elementName)) { uiBounds[para_elementName] = elementGUIBounds; } else { uiBounds.Add(para_elementName,elementGUIBounds); }
				if(para_destroyGuide) { Destroy(elementGObj.gameObject); }

				if( ! textContent.ContainsKey(para_elementName)) { textContent.Add(para_elementName,para_elementContent); }

				//elementGObj.parent = reqParentGObj.transform;
			}else{
				Debug.Log("Not found: "+reqParentGObj.name+"/"+para_elementName);
			}
		}else{
			Debug.LogError(para_pathToGameObj+" not found");
		}
	}
	
	protected void populateWorldTextElement(string para_elementName,
	                                        string para_elementContent,
	                                        bool para_destroyGuide,
	                                        string para_pathToGameObj)
	{
		if(textContent == null) { this.init(); }

		Transform reqParentGObj = getReqParentGObj(para_pathToGameObj);
		if(reqParentGObj != null)
		{
			Transform elementGObj = reqParentGObj.FindChild(para_elementName);
			if(elementGObj != null)
			{
				Transform oldTE = transform.FindChild(para_elementName+"-WB");
				if(oldTE != null) { Destroy(oldTE.gameObject); }

				Rect world2DBounds = CommonUnityUtils.get2DBounds(elementGObj.renderer.bounds);
				float zVal = elementGObj.transform.position.z;
				GameObject nwWordBox = WordBuilderHelper.buildWordBox(-99,para_elementContent,world2DBounds,zVal,upAxisArr,wordBoxPrefab);
				nwWordBox.name = para_elementName+"-WB";
				nwWordBox.layer = reqParentGObj.gameObject.layer;
				Destroy(nwWordBox.transform.FindChild("Board").gameObject);
				nwWordBox.transform.FindChild("Text").renderer.sortingOrder = uiRender_baseSortingLayer + 5;
				nwWordBox.transform.FindChild("Text").gameObject.layer = reqParentGObj.gameObject.layer;
				nwWordBox.transform.parent = transform;

				Rect elementGUIBounds = WorldSpawnHelper.getWorldToGUIBounds(elementGObj.renderer.bounds,upAxisArr);
				if(uiBounds == null) { uiBounds = new Dictionary<string, Rect>(); }
				if(uiBounds.ContainsKey(para_elementName)) { uiBounds[para_elementName] = elementGUIBounds; } else { uiBounds.Add(para_elementName,elementGUIBounds); }

				if(para_destroyGuide) { Destroy(elementGObj.gameObject); }

				if( ! textContent.ContainsKey(para_elementName)) { textContent.Add(para_elementName,para_elementContent); }
			}
		}
	}

	private Transform getReqParentGObj(string para_pathToGObj)
	{
		Transform retData = null;

		if((para_pathToGObj == null)||(para_pathToGObj == ""))
		{
			retData = transform;
		}
		else
		{
			Transform tmpTrans = transform;
			string[] pathItems = para_pathToGObj.Split('*');
			for(int i=0; i<pathItems.Length; i++)
			{
				tmpTrans = tmpTrans.FindChild(pathItems[i]);
				if(tmpTrans == null)
				{
					break;
				}
			}

			retData = tmpTrans;
		}

		return retData;
	}
}

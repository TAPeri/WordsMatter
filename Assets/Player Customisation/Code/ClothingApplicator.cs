/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ClothingApplicator : MonoBehaviour
{

	GameObject subjectGObj;
	RuntimeAnimatorController subjectAniController;


	void OnDestroy()
	{
		ClothingManager.disposeInstance();
	}


	public void setSubject(GameObject para_avatarGObj,
	                       RuntimeAnimatorController para_aniController)
	{
		subjectGObj = para_avatarGObj;
		subjectAniController = para_aniController;
	}

	public void applyClothingConfig(ClothingConfig para_cConfig, ClothingSize para_size)
	{
		if(para_cConfig != null)
		{
			ClothingConfigIterator iter = para_cConfig.getIterator();

			while(iter.hasNext())
			{
				string[] catNItemPair = iter.getNextClothingInfo();
				string tmpCategoryName = catNItemPair[0];
				string tmpClothingSelectionPrefix = catNItemPair[1];

				if(para_size == ClothingSize.BIG)
				{
					tmpClothingSelectionPrefix = "Big_"+tmpClothingSelectionPrefix;
				}

				applyClothingByCategory(tmpCategoryName,tmpClothingSelectionPrefix,para_size);
			}
		}
	}

	// WARNING: For some reason this method cannot be used outside and must be set to private.
	// Call applyClothingConfig above from an external script (even if only one clothing category needs to be applied).
	private void applyClothingByCategory(string para_categoryName, string para_clothingPrefix, ClothingSize para_size)
	{
		string spritePrefix = para_clothingPrefix;
		ClothingManager.getInstance(para_size); // Keep this line in case getInstance was not called previously.
		Dictionary<string,Sprite> loadedSprites = ClothingManager.loadedClothingSprites;


		List<PCObjSpriteCombo> reqPieces = getCategoryPieces(para_categoryName,para_size);
		

		GameObject mainAvatar = subjectGObj;
		Animator tmpAni = mainAvatar.gameObject.GetComponent<Animator>();
		tmpAni.runtimeAnimatorController = null;
		if(reqPieces != null)
		{
			for(int i=0; i<reqPieces.Count; i++)
			{
				PCObjSpriteCombo tmpPieceInfo = reqPieces[i];
				Transform tmpChild = deepChildFind(mainAvatar.transform,tmpPieceInfo.gObjName);
				if(tmpChild != null)
				{
					SpriteRenderer tmpSRend = tmpChild.gameObject.GetComponent<SpriteRenderer>();
					if(tmpSRend != null)
					{
						//if(tmpSRend.sprite != null)
						//{
							//Debug.Log(spritePrefix + tmpPieceInfo.spriteSuffix);

							Sprite reqNewSprite = null;
							string reqNewSpriteKey = spritePrefix + tmpPieceInfo.spriteSuffix;

							if(loadedSprites.ContainsKey(reqNewSpriteKey))
							{
								reqNewSprite = loadedSprites[reqNewSpriteKey];
							}
							else
							{
								reqNewSpriteKey = "AV01" + tmpPieceInfo.spriteSuffix;
								if(para_size == ClothingSize.BIG) { reqNewSpriteKey = "Big_AV01" + tmpPieceInfo.spriteSuffix; }
								reqNewSprite = loadedSprites[reqNewSpriteKey];
							}

							tmpSRend.sprite = reqNewSprite;

						//}
					}
				}
			}
		}
		tmpAni.runtimeAnimatorController = subjectAniController;
	}

	// Tmp.
	public void toggleClothingVisibility(string para_modelSpriteName, string para_categoryName, ClothingSize para_size)
	{
		List<PCObjSpriteCombo> tmpPieceList = getCategoryPieces(para_categoryName,para_size);
		
		if(tmpPieceList != null)
		{
			for(int i=0; i<tmpPieceList.Count; i++)
			{
				string gObjName = tmpPieceList[i].gObjName;
				Transform reqChild = deepChildFind(subjectGObj.transform,gObjName);
				if(reqChild != null)
				{
					SpriteRenderer tmpSRend = reqChild.gameObject.GetComponent<SpriteRenderer>();
					if(tmpSRend != null) { tmpSRend.enabled = true; }
				}
			}
		}
	}


	private List<PCObjSpriteCombo> getCategoryPieces(string para_categoryName, ClothingSize para_size)
	{
		ClothingManager.getInstance(para_size);

		int categoryPKey = ClothingManager.apCategoryTable.getPKeyUsingCategoryName(para_categoryName);
		int[] topLevelPieces = ClothingManager.apCategoryTable.getTopLevelPieces(categoryPKey);
		int[] singlePieces = ClothingManager.apCategoryTable.getSinglePieces(categoryPKey);
		
		
		//HashSet<int> seenPieces = new HashSet<int>();
		List<PCObjSpriteCombo> tmpPieceList = new List<PCObjSpriteCombo>();
		if(topLevelPieces != null)
		{
			for(int i=0; i<topLevelPieces.Length; i++)
			{
				int currTLPiece = topLevelPieces[i];
				List<PCObjSpriteCombo> listToMerge = recursivlyConstructEffectData(currTLPiece,para_size);
				if(listToMerge != null)
				{
					tmpPieceList.AddRange(listToMerge);
				}
			}
		}
		
		if(singlePieces != null)
		{
			for(int i=0; i<singlePieces.Length; i++)
			{
				int currSingPiece = singlePieces[i];
				APTableRow reqRow = ClothingManager.apTable.findAPTableRow(currSingPiece);
				if(reqRow != null)
				{
					tmpPieceList.Add(new PCObjSpriteCombo(reqRow.getGObjName(),reqRow.getSpriteNameSuffix()));
				}
			}
		}
		
		return tmpPieceList;
	}

	private List<PCObjSpriteCombo> recursivlyConstructEffectData(int para_pieceID, ClothingSize para_size)
	{
		List<PCObjSpriteCombo> retList = new List<PCObjSpriteCombo>();

		ClothingManager.getInstance(para_size);

		APTableRow reqRow = ClothingManager.apTable.findAPTableRow(para_pieceID);
		if(reqRow == null)
		{
			return retList;
		}
		else
		{
			retList.Add(new PCObjSpriteCombo(reqRow.getGObjName(),reqRow.getSpriteNameSuffix()));
			
			int[] childPieceIDs = reqRow.getChildPieces();
			if(childPieceIDs != null)
			{
				for(int i=0; i<childPieceIDs.Length; i++)
				{
					List<PCObjSpriteCombo> listToMerge = recursivlyConstructEffectData(childPieceIDs[i],para_size);
					retList.AddRange(listToMerge);
				}
			}
		}
		
		return retList;
	}

	private Transform deepChildFind(Transform parentObj, string para_childName)
	{
		Transform retTrans = null;
		
		if(parentObj == null)
		{
			return retTrans;
		}
		else
		{
			if(parentObj.name == para_childName)
			{
				return parentObj;
			}
			else
			{
				int childrenCount = parentObj.childCount;
				for(int i=0; i<childrenCount; i++)
				{
					retTrans = deepChildFind(parentObj.GetChild(i),para_childName);
					if(retTrans != null)
					{
						break;
					}
				}
			}
		}
		
		return retTrans;
	}

	public void toggleAllSpriteRends(Transform para_obj, bool para_spriteRendState)
	{
		List<SpriteRenderer> sRendList = getSpriteRendsOfChildrenRecursively(para_obj.gameObject);
		if(sRendList != null)
		{
			for(int i=0; i<sRendList.Count; i++)
			{
				sRendList[i].enabled = para_spriteRendState;
			}
		}
	}
	
	private List<SpriteRenderer> getSpriteRendsOfChildrenRecursively(GameObject para_tmpObj)
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

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class WordBuilderHelper
{






	public static float getCharacterSizeForBounds(Bounds para_itemWorldBounds, int para_textCharLength) { return getCharacterSizeForBounds(para_itemWorldBounds.size.x,para_textCharLength); }
	public static float getCharacterSizeForBounds(Rect para_itemWorldBounds, int para_textCharLength)   { return getCharacterSizeForBounds(para_itemWorldBounds.width,para_textCharLength);  }
	public static float getCharacterSizeForBounds(float para_widthBounds, int para_textCharLength)
	{
		float reqMaxWidth = para_widthBounds;
		float normalTextWidth = 0.7f * para_textCharLength;
		float normalTextCharacterSize = 0.1f;			
		float reqCharacterSize = (reqMaxWidth * normalTextCharacterSize)/normalTextWidth;

		return reqCharacterSize;
	}



	public static GameObject buildWordBoxWithSound(int para_wBoxID,
	                                      string para_wordText,
	                                      Rect para_worldBounds,
	                                      float para_depthCoord,
	                                      bool[] para_upAxisArr,
	                                      Transform para_wordBoxPrefab,
	                                      AudioClip TTSword,
	                                      bool only_sound)
	{

		GameObject nwWordBox;
		if(only_sound){

			nwWordBox = buildWordBox( para_wBoxID, " ", para_worldBounds, para_depthCoord,   para_upAxisArr, para_wordBoxPrefab);

		}else{
			nwWordBox = buildWordBox( para_wBoxID, para_wordText, para_worldBounds, para_depthCoord,   para_upAxisArr, para_wordBoxPrefab);

		}

		AudioSource audioSource = nwWordBox.AddComponent<AudioSource>();
		audioSource.clip = TTSword; 
		audioSource.Play();

		return nwWordBox;

	}



	public static GameObject buildWordBox(int para_wBoxID,
	                                      string para_wordText,
	                                      Rect para_worldBounds,
	                                      float para_depthCoord,
	                                      bool[] para_upAxisArr,
	                                      Transform para_wordBoxPrefab)
	{
		GameObject nwWordBox = WorldSpawnHelper.initObjWithinWorldBounds(para_wordBoxPrefab,1,1,"WordBox"+para_wBoxID,para_worldBounds,null,para_depthCoord,para_upAxisArr);
		TextNBoardScript tnbScript = (TextNBoardScript) nwWordBox.GetComponent(typeof(TextNBoardScript));
		tnbScript.init(para_wordText,Color.black,Color.white);

		
		Vector3 scaleCopy = new Vector3(nwWordBox.transform.localScale.x,nwWordBox.transform.localScale.y,nwWordBox.transform.localScale.z);
		nwWordBox.transform.localScale = new Vector3(1,1,1);
		
		Transform boardChild = (Transform) nwWordBox.transform.FindChild("Board");
		boardChild.localScale = scaleCopy;
		BoxCollider bCol = (BoxCollider) nwWordBox.GetComponent(typeof(BoxCollider));
		if(bCol != null)
		{
			bCol.size = scaleCopy;
		}
		
		
		Transform textChild = (Transform) nwWordBox.transform.FindChild("Text");
		
		textChild.transform.parent = null;
		Vector3 boardSize = boardChild.renderer.bounds.size;
		//float defaultLetterWidth = 0.7f;
		
		//int logicalMaxLetters = (int) (boardSize.x/(boardSize.y * 1.0f));
		
		
		textChild.transform.parent = null;
		textChild.transform.localScale = new Vector3(1,1,1);
		//textChild.transform.parent = transform;
		


		float reqTextCharacterSize = getCharacterSizeForBounds(boardSize.x,para_wordText.Length);
		TextMesh tMesh = textChild.GetComponent<TextMesh>();
		tMesh.characterSize = reqTextCharacterSize;
		
		
		
		textChild.transform.parent = nwWordBox.transform;
		
		
		
		
		return nwWordBox;
	}



	public static float getMinFontCharSizeForWordBoxes(List<GameObject> para_wordBoxes)
	{
		float minFontCharSize = -1;

		if(para_wordBoxes != null)
		{
			for(int i=0; i<para_wordBoxes.Count; i++)
			{
				GameObject tmpObj = para_wordBoxes[i];
				if(tmpObj != null)
				{
					Transform textChild = tmpObj.transform.FindChild("Text");
					if(textChild != null)
					{
						TextMesh tMesh = textChild.gameObject.GetComponent<TextMesh>();
						if(tMesh != null)
						{
							if(i == 0)
							{
								minFontCharSize = tMesh.characterSize;
							}
							else
							{
								if(tMesh.characterSize < minFontCharSize)
								{
									minFontCharSize = tMesh.characterSize;
								}
							}
						}
					}
				}
			}
		}

		return minFontCharSize;
	}

	public static float setBoxesToUniformTextSize(List<GameObject> para_gObjs, float para_maxDesiredFontCharSize)
	{

		float smallestFontCharSize = para_maxDesiredFontCharSize;

		for(int i=0; i<para_gObjs.Count; i++)
		{
			Transform textChild = para_gObjs[i].transform.FindChild("Text");
			if(textChild != null)
			{
				TextMesh tMesh = textChild.gameObject.GetComponent<TextMesh>();
				if(tMesh.characterSize < smallestFontCharSize)
				{
					smallestFontCharSize = tMesh.characterSize;
				}
			}
		}


		for(int i=0; i<para_gObjs.Count; i++)
		{
			Transform textChild = para_gObjs[i].transform.FindChild("Text");
			if(textChild != null)
			{
				TextMesh tMesh = textChild.gameObject.GetComponent<TextMesh>();
				tMesh.characterSize = smallestFontCharSize;
			}
		}

		return smallestFontCharSize;
	}

	public static float setBoxesToTextSizeInRange(List<GameObject> para_gObjs, float min,float max)
	{

		float smallestFontCharSize = -1;
		
		for(int i=0; i<para_gObjs.Count; i++)
		{
			Transform textChild = para_gObjs[i].transform.FindChild("Text");
			if(textChild != null)
			{
				TextMesh tMesh = textChild.gameObject.GetComponent<TextMesh>();

				if(smallestFontCharSize<0)
					smallestFontCharSize = tMesh.characterSize;
				//Debug.Log("Font "+i+": "+tMesh.characterSize);

				if(tMesh.characterSize < smallestFontCharSize)
				{
					smallestFontCharSize = tMesh.characterSize;
				}
			}
		}
		if(smallestFontCharSize<min)
			smallestFontCharSize = min;
		else if(smallestFontCharSize>max)
			smallestFontCharSize = max;

		for(int i=0; i<para_gObjs.Count; i++)
		{
			Transform textChild = para_gObjs[i].transform.FindChild("Text");
			if(textChild != null)
			{
				TextMesh tMesh = textChild.gameObject.GetComponent<TextMesh>();
					tMesh.characterSize = smallestFontCharSize;
			}
		}
		
		return smallestFontCharSize;
	}

}

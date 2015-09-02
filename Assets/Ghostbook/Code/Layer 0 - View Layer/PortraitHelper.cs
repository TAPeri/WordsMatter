/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class PortraitHelper : MonoBehaviour
{





	public static void replaceEntireDummyPortrait(GameObject para_portraitToReplace, int para_reqCharID, float para_reqBarPercentage, string para_reqBarText)
	{
		replaceEntireDummyPortrait(para_portraitToReplace,para_reqCharID,para_reqBarPercentage,para_reqBarText,-1);
	}

	public static void replaceEntireDummyPortrait(GameObject para_portraitToReplace, int para_reqCharID, float para_reqBarPercentage, string para_reqBarText, float para_overrideCharSize)
	{
		if(para_portraitToReplace != null)
		{
			updatePortraitPhoto(para_portraitToReplace,para_reqCharID);
			updatePortraitProgressBar(para_portraitToReplace,para_reqBarPercentage);
			updatePortraitTextLabel(para_portraitToReplace,para_reqBarText,para_overrideCharSize);
		}
	}

	public static void updatePortraitPhoto(GameObject para_portraitToReplace, int para_reqCharID)
	{
		if(para_portraitToReplace != null)
		{
			Transform reqPortraitPrefab = Resources.Load<Transform>("Prefabs/Ghostbook/UnlockedProfilePics/UnlockedPortrait_"+para_reqCharID);
			if(reqPortraitPrefab != null)
			{
				para_portraitToReplace.GetComponent<SpriteRenderer>().sprite = reqPortraitPrefab.GetComponent<SpriteRenderer>().sprite;
			}
		}
	}

	public static void updatePortraitProgressBar(GameObject para_portraitToReplace, float para_reqBarPercentage)
	{
		updatePortraitProgressBar(para_portraitToReplace,para_reqBarPercentage,Color.clear);
	}

	public static void updatePortraitProgressBar(GameObject para_portraitToReplace, float para_reqBarPercentage, Color para_overrideColor)
	{
		if(para_portraitToReplace != null)
		{
			float percVal = para_reqBarPercentage;
			if(percVal > 1) { percVal = 1; }
			if(percVal <= 0) { percVal = 0.0001f; }


			Transform nameGuide = para_portraitToReplace.transform.FindChild("Name");
			Rect nameGuideBounds = CommonUnityUtils.get2DBounds(nameGuide.renderer.bounds);
			nameGuide.collider.enabled = false;

			Transform nameBar = para_portraitToReplace.transform.FindChild("NameBar");
			Transform friendshipMeter = nameBar.FindChild("FriendshipMeter");
			Rect friendshipMeterOrigBounds = CommonUnityUtils.get2DBounds(friendshipMeter.renderer.bounds);
			
			Rect desiredBoundsForProgBar = new Rect(friendshipMeterOrigBounds.x,friendshipMeterOrigBounds.y,nameGuideBounds.width * percVal,friendshipMeterOrigBounds.height);
			
			Vector3 tmpscale = friendshipMeter.localScale;
			tmpscale.x *= (desiredBoundsForProgBar.width/friendshipMeterOrigBounds.width);
			friendshipMeter.localScale = tmpscale;

			Vector3 tmpPos = friendshipMeter.position;
			tmpPos.x = desiredBoundsForProgBar.x + (desiredBoundsForProgBar.width);  // Not width/2f because sprite pivot is set to the mid of the right edge.
			friendshipMeter.position = tmpPos;


			SpriteRenderer sRendForFriendship = friendshipMeter.GetComponent<SpriteRenderer>();
			if(para_overrideColor == Color.clear)
			{
				if(percVal < 0.33f)
				{
					// Red
					sRendForFriendship.color = ColorUtils.convertColor(255,0,0); // Red.
				}
				else if(percVal < 0.66f)
				{
					// Orange.
					sRendForFriendship.color = ColorUtils.convertColor(255,131,0); // Orange.
				}
				else if(percVal < 1f)
				{
					// Yellow.
					sRendForFriendship.color = ColorUtils.convertColor(255,215,0); // Gold.
				}
				else if(percVal >= 1f)
				{
					// Green.
					sRendForFriendship.color = Color.green;
				}
			}
			else
			{
				sRendForFriendship.color = para_overrideColor;
			}
		}
	}



	public static void updatePortraitTextLabel(GameObject para_portraitToReplace, string para_text)
	{
		updatePortraitTextLabel(para_portraitToReplace,para_text,-1);
	}

	public static void updatePortraitTextLabel(GameObject para_portraitToReplace, string para_text, float para_overrideCharSize)
	{
		if(para_portraitToReplace != null)
		{
			Transform nameGuide = para_portraitToReplace.transform.FindChild("Name");
			Rect nameGuideBounds = CommonUnityUtils.get2DBounds(nameGuide.renderer.bounds);
			nameGuide.renderer.enabled = false;
			nameGuide.collider.enabled = false;


			foreach (Transform child in para_portraitToReplace.transform.FindChild("NameBar")){//Destroys more than 1
				if(child.name=="TextBanner")
					Destroy(child.gameObject);

			}

			//Transform oldTextBanner = para_portraitToReplace.transform.FindChild("NameBar").FindChild("TextBanner");
			//if(oldTextBanner != null) { Destroy(oldTextBanner.gameObject); }
			//if(oldTextBanner != null) { DestroyImmediate(oldTextBanner.gameObject); }

			Transform genericWordBox = Resources.Load<Transform>("Prefabs/GenericWordBox");
			GameObject nwWordBox = WordBuilderHelper.buildWordBox(99,para_text,nameGuideBounds,nameGuide.position.z,new bool[]{false,true,false},genericWordBox);
			nwWordBox.name = "TextBanner";
			Destroy(nwWordBox.transform.FindChild("Board").gameObject);
			if(para_overrideCharSize > 0)
			{
				nwWordBox.transform.FindChild("Text").GetComponent<TextMesh>().characterSize = para_overrideCharSize;
			}
			nwWordBox.transform.FindChild("Text").renderer.sortingOrder = 601;
			//nwWordBox.transform.FindChild("Text").renderer.sortingOrder = 6000;
			//nwWordBox.transform.FindChild("Text").renderer.sortingLayerName = "SpriteGUI";

			nwWordBox.transform.parent = para_portraitToReplace.transform.FindChild("NameBar");
		}
	}

	public static void setCharSizeForTextLabel(GameObject para_portrait, float para_charSizeForText)
	{
		if(para_portrait != null)
		{
			Transform tBanner = para_portrait.transform.FindChild("NameBar").FindChild("TextBanner");
			if(tBanner != null)
			{
				tBanner.FindChild("Text").GetComponent<TextMesh>().characterSize = para_charSizeForText;
			}
		}
	}


	public static void applyCorrectSealSprite(GameObject para_sealSource, float para_percCompletion)
	{
		if(para_sealSource != null)
		{
			float percComp = para_percCompletion;
			if(percComp > 1) { percComp = 1; }
			if(percComp < 0) { percComp = 0; }

			Sprite reqSealSprite = Resources.Load<Sprite>("Sprites/RedSeal");
			if(percComp < 0.33f)
			{
				// Red.
				reqSealSprite = Resources.Load<Sprite>("Sprites/RedSeal");
			}
			else if(percComp < 0.66f)
			{
				// Orange.
				reqSealSprite = Resources.Load<Sprite>("Sprites/OrangeSeal");
			}
			else if(percComp < 1f)
			{
				// Yellow.
				reqSealSprite = Resources.Load<Sprite>("Sprites/YellowSeal");
			}
			else if(percComp >= 1f)
			{
				// Green.
				reqSealSprite = Resources.Load<Sprite>("Sprites/GreenSeal");
			}

			para_sealSource.GetComponent<SpriteRenderer>().sprite = reqSealSprite;
		}
	}



}

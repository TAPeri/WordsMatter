/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class KeyboardScript : MonoBehaviour, CustomActionListener
{

	List<Rect[]> uiKeyBounds;
	TextMesh currTMeshRef;
	bool[] upAxisArr = new bool[3] { false, true, false };

	bool fingerDown;

	bool btnSelected;
	int[] selBtnCoords;
	//Color selectColor;

	bool keyboardEnabled;


	void OnStart()
	{
		currTMeshRef = null;
		fingerDown = false;
		btnSelected = false;
		selBtnCoords = new int[2];
		//selectColor = Color.green;
		keyboardEnabled = false;
	}
	

	void OnGUI()
	{
		if(keyboardEnabled)
		{
			// Used to place invisible button overlay.
			// (May have to remove this and use raycasts instead if Unity GUI performance is bad).

			GUI.color = Color.clear;

			for(int r=0; r<uiKeyBounds.Count; r++)
			{
				Rect[] tmpRow = uiKeyBounds[r];

				for(int c=0; c<tmpRow.Length; c++)
				{
					if(GUI.RepeatButton(tmpRow[c],""))
					{
						bool enterIf = false;

						if((Application.platform == RuntimePlatform.Android)
						   ||(Application.platform == RuntimePlatform.IPhonePlayer))
						{
							enterIf = ( ! btnSelected);
						}
						else
						{
							enterIf = ((fingerDown)&&( ! btnSelected));
						}

						if(enterIf)
						{
							btnSelected = true;

							if(selBtnCoords == null) { selBtnCoords = new int[2]; }
							selBtnCoords[0] = c;
							selBtnCoords[1] = r;

							// Init hold ani.
							Transform reqKeyTrans = transform.FindChild("Key*"+r+"-"+c);
							//SpriteRenderer sRend = reqKeyTrans.gameObject.GetComponent<SpriteRenderer>();
							//sRend.color = selectColor;
							Vector3 locScale = reqKeyTrans.localScale;
							locScale.x = locScale.x/2f;
							locScale.y = locScale.y/2f;
							locScale.z = locScale.z/2f;
							reqKeyTrans.localScale = locScale;

							fingerDown = true;
						}
					}
				}
			}


			bool tmpFlag = false;
			if((Application.platform == RuntimePlatform.Android)
			   ||(Application.platform == RuntimePlatform.IPhonePlayer))
			{
				tmpFlag = (Input.touches.Length > 0);
			}
			else
			{
				tmpFlag = (Input.GetMouseButton(0));
			}

			//if((fingerDown)&&(!tmpFlag)) { Debug.Log("FingerDown Now False"); }
			//else if((!fingerDown)&&(tmpFlag)) { Debug.Log("FingerDown Now True"); }
			fingerDown = tmpFlag;

			if( ! fingerDown)
			{
				if(btnSelected)
				{
					Transform reqKeyTrans = transform.FindChild("Key*"+selBtnCoords[1]+"-"+selBtnCoords[0]);
					reqKeyTrans.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
					Vector3 locScale = reqKeyTrans.localScale;
					locScale.x = locScale.x * 2f;
					locScale.y = locScale.y * 2f;
					locScale.z = locScale.z * 2f;
					reqKeyTrans.localScale = locScale;


					if(currTMeshRef != null)
					{
						TextMesh reqKeyTMesh = reqKeyTrans.FindChild("WordBox").FindChild("Text").GetComponent<TextMesh>();
						//if(currTMeshRef.text.Length < 10)
						//{
							string tmpTextToAdd = reqKeyTMesh.text;

							if(LocalisationMang.langCode == LanguageCode.GR)
							{
								// Check if special greek accent letters.
								if(currTMeshRef.text.Length > 0)
								{
									if(currTMeshRef.text[currTMeshRef.text.Length-1] == '´')
									{
										if(currTMeshRef.text.Length == 1) { currTMeshRef.text = ""; }
										else { currTMeshRef.text = currTMeshRef.text.Substring(0,currTMeshRef.text.Length-1); }

										List<char> acuteAccChars = new List<char>() { 'α','ε','η','ι','ο','υ','ω' };
										List<char> acuteAccChangeChars = new List<char>() { 'ά','έ','ή','ί','ό','ύ','ώ' };
										for(int i=0; i<acuteAccChars.Count; i++)
										{
											if(acuteAccChars[i] == reqKeyTMesh.text[0])
											{
												tmpTextToAdd = ""+acuteAccChangeChars[i];
												break;
											}
										}

									}
									else if(currTMeshRef.text[currTMeshRef.text.Length-1] == '¨')
									{
										if(currTMeshRef.text.Length == 1) { currTMeshRef.text = ""; }
										else { currTMeshRef.text = currTMeshRef.text.Substring(0,currTMeshRef.text.Length-1); }

										List<char> diAccChars = new List<char>() { 'ι','υ' };
										List<char> diAccChangeChars = new List<char>() { 'ϊ','ϋ' };
										for(int i=0; i<diAccChars.Count; i++)
										{
											if(diAccChars[i] == reqKeyTMesh.text[0])
											{
												tmpTextToAdd = ""+diAccChangeChars[i];
												break;
											}
										}

									}
								}
							}



							currTMeshRef.text += tmpTextToAdd;
						//}

						// Trigger teleport camera to focus on text mesh.
						teleportNFocusOnTextDest();
					}
				}
				
				btnSelected = false;
			}




		}

	}



	public void initKeyboard(List<string> para_keyboardLayout,
	                         Rect para_maxWorldBoundsForKeyboard,
	                         Transform para_keyboardKeyPrefab,
	                         Transform para_wordBoxPrefab,
	                         Transform backButton)
	{

		// Create keyboard additional world objects.
		// (This script should be attached to a blank Keyboard game object.

		float columnPadding = 0.05f;
		float rowPadding = 0.1f;


		int largestRowSize = 0;
		for(int i=0; i<para_keyboardLayout.Count; i++)
		{
			if(para_keyboardLayout[i].Length > largestRowSize)
			{
				largestRowSize = para_keyboardLayout[i].Length;
			}
		}


		uiKeyBounds = new List<Rect[]>();

		float totalReqWidth = ((largestRowSize * 1.0f) * para_keyboardKeyPrefab.renderer.bounds.size.x) + (((largestRowSize-1) * 1.0f) * columnPadding);
		float totalReqHeight = ((para_keyboardLayout.Count * 1.0f) * para_keyboardKeyPrefab.renderer.bounds.size.y) + (((para_keyboardLayout.Count-1)*1.0f) * rowPadding);
		float row_X = para_maxWorldBoundsForKeyboard.center.x - (totalReqWidth/2f);
		float row_Y = para_maxWorldBoundsForKeyboard.center.y + (totalReqHeight/2f);

		for(int r=0; r<para_keyboardLayout.Count; r++)
		{
			string reqRow = para_keyboardLayout[r];
			Rect[] tmpUIArr = new Rect[reqRow.Length];

			float rowWidth = ((reqRow.Length * 1.0f) * para_keyboardKeyPrefab.renderer.bounds.size.x) + (((reqRow.Length-1) * 1.0f) * columnPadding);
			row_X = para_maxWorldBoundsForKeyboard.center.x - (rowWidth/2f);
			row_Y = para_maxWorldBoundsForKeyboard.y - (para_maxWorldBoundsForKeyboard.height/2f) + (totalReqHeight/2f) - (r * (para_keyboardKeyPrefab.renderer.bounds.size.y + rowPadding));

			Rect keyboardTileBounds = new Rect(row_X,row_Y,para_keyboardKeyPrefab.renderer.bounds.size.x,para_keyboardKeyPrefab.renderer.bounds.size.y);

			for(int c=0; c<reqRow.Length; c++)
			{
				GameObject nwKeyBase = WorldSpawnHelper.initObjWithinWorldBounds(para_keyboardKeyPrefab,"Key*"+r+"-"+c,keyboardTileBounds,-1f,upAxisArr);
				nwKeyBase.transform.localScale = para_keyboardKeyPrefab.transform.localScale;
				GameObject nwKeyWordBox = WordBuilderHelper.buildWordBox(99,""+reqRow[c],keyboardTileBounds,-1,upAxisArr,para_wordBoxPrefab);
				nwKeyWordBox.name = "WordBox";
				Destroy(nwKeyWordBox.transform.FindChild("Board").gameObject);
				nwKeyWordBox.transform.FindChild("Text").renderer.sortingLayerName = nwKeyBase.renderer.sortingLayerName;
				nwKeyWordBox.transform.FindChild("Text").renderer.sortingOrder = 200;

				WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() { nwKeyWordBox },0.06f);

				nwKeyWordBox.transform.parent = nwKeyBase.transform;
				nwKeyBase.transform.parent = transform;

				tmpUIArr[c] = WorldSpawnHelper.getWorldToGUIBounds(keyboardTileBounds,-1,upAxisArr);

				keyboardTileBounds.x += (keyboardTileBounds.width + columnPadding);
			}

			if(r==0){

				//Rect box = WorldSpawnHelper.getWorldToGUIBounds(backButton.transform.renderer.bounds,upAxisArr);

				Debug.LogWarning(backButton.transform.renderer.bounds.extents.x+" "+backButton.transform.renderer.bounds.extents.y);
				Vector3 tmp = backButton.position;
				tmp.x = keyboardTileBounds.xMax;
				tmp.y = keyboardTileBounds.y-backButton.transform.renderer.bounds.extents.y;

				backButton.position = tmp;
				backButton.parent = this.transform;

			}

			uiKeyBounds.Add(tmpUIArr);
		}


		//selectColor = Color.green;

		// Attach input detector.
		//clickDetector = transform.gameObject.AddComponent<ClickDetector>();
		//clickDetector.registerListener("Keyboard",this);
	}


	public void enableKeyboard(TextMesh para_destTMesh)
	{
		currTMeshRef = para_destTMesh;
		keyboardEnabled = true;
	}

	public void disableKeyboard()
	{
		currTMeshRef = null;
		keyboardEnabled = false;
	}

	public void applyBackspace()
	{
		if((keyboardEnabled)&&(currTMeshRef != null))
		{
			if(currTMeshRef.text.Length > 0)
			{
				currTMeshRef.text = (currTMeshRef.text.Substring(0,currTMeshRef.text.Length-1));
			}
		}
	}


	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{

	}


	private void teleportNFocusOnTextDest()
	{

		if(currTMeshRef != null)
		{
			Vector3 focusTextPos = currTMeshRef.transform.position;

			// Teleport camera.
			TeleportToLocation ttl = Camera.main.gameObject.AddComponent<TeleportToLocation>();
			ttl.init(new Vector3(focusTextPos.x,Camera.main.gameObject.transform.position.y,Camera.main.gameObject.transform.position.z));
		}
	}

}

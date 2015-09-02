/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class HighlightInputScript : MonoBehaviour
{

	public Transform wrenchPrefab;
	public Transform clampPrefab;

	Vector2 clickPos;
	bool fingerDown;
	
	char[] highlightStateArr;


	char currMode;
	Color currColorEffect;
	
	int availableHighlightPoints;
	//int totalPossibleHighlightPoints;
	
	Dictionary<string,Rect> uiBounds;

	GUIStyle counterGUIStyle;
	bool preppedGUIStyles;

	//bool goVisible;

	bool inputOn;


	void Start()
	{
		clickPos = new Vector2();
		fingerDown = false;
		currMode = '1';
		currColorEffect = Color.white;
		//goVisible = false;
		inputOn = true;
	}
	
	void Update()
	{
		// Highlight Detection.

		if(inputOn)
		{
			
			bool clickDetected = false;
			if((Application.platform == RuntimePlatform.Android)
			   ||(Application.platform == RuntimePlatform.IPhonePlayer))
			{
				if(Input.touches.Length == 1)
				{
					clickPos.x = Input.touches[0].position.x;
					clickPos.y = Input.touches[0].position.y;
					clickDetected = true;
				}
			}
			else
			{
				if(Input.GetMouseButton(0))
				{
					clickPos.x = Input.mousePosition.x;
					clickPos.y = Input.mousePosition.y;
					clickDetected = true;
				}
			}
			
			
			
			if(clickDetected)
			{



				// Raycast to check for tile hit.
				
				RaycastHit hitInfo;
				if(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(clickPos.x,clickPos.y,0)),out hitInfo))
				{
					if(hitInfo.collider.gameObject.name.Contains("Tile"))
					{

						//Debug.Log(hitInfo.collider.gameObject.name);
						
						string[] aux = hitInfo.collider.gameObject.name.Split('*');
						if(aux.Length==1){//ErrorTile
							aux = hitInfo.collider.gameObject.name.Split('e');//ErrorTileXXX
						}

						int letterIndex = int.Parse(aux[1]);

						if((letterIndex >= 0)&&(letterIndex < highlightStateArr.Length))
						{

							bool validAction = true;

							// i.e. a new drag session is started.
							if( ! fingerDown) 
							{
								if(highlightStateArr[letterIndex] == '1')
								{
									currMode = '0';
									currColorEffect = Color.white;
								}
								else
								{
									currMode = '1';
									currColorEffect = Color.yellow;
								}
							}


							if((currMode == '1')&&(highlightStateArr[letterIndex] == '0'))
							{
								if(availableHighlightPoints <= 0)
								{
									// No more highlight points!
									validAction = false;
								}
							}

								
							if(validAction)
							{

								if((highlightStateArr[letterIndex] == '1')&&(currMode == '0'))
								{
									availableHighlightPoints++;

									updateCounterDisplay();


									removeClamp(hitInfo.collider.name);

								}

								if((highlightStateArr[letterIndex] == '0')&&(currMode == '1'))
								{
									availableHighlightPoints--; 
									if(availableHighlightPoints < 0) { availableHighlightPoints = 0; }
				

									updateCounterDisplay();

									addClamp(hitInfo.collider.name);
								}


								highlightStateArr[letterIndex] = currMode;
								SpriteRenderer sRend = hitInfo.collider.gameObject.GetComponent<SpriteRenderer>();
								sRend.color = currColorEffect;
							}
						}

					}
				}


				if(!fingerDown)
				{
					fingerDown = true;
				}
			}
			else
			{
				fingerDown = false;
			}
		}
	}

	public void addClamp(string name){

		GameObject bridgeObj = GameObject.Find("Bridge");
		GameObject supportCollectionForTile = bridgeObj.transform.Find("BridgeSupports").transform.FindChild(name.Replace("Tile","Support")).gameObject;

		for(int i=0; i<supportCollectionForTile.transform.childCount; i++)
		{
			Transform tmpChild = supportCollectionForTile.transform.GetChild(i);
			if((tmpChild.name == "MainPipe"))
			{
				Rect mainPipeBounds = CommonUnityUtils.get2DBounds(tmpChild.renderer.bounds);
				Vector3 clampSpawnPt = new Vector3(mainPipeBounds.x + (mainPipeBounds.width/2f), mainPipeBounds.y - (clampPrefab.renderer.bounds.size.y/2f),tmpChild.transform.position.z - 0.2f);
				Transform nwClamp = (Transform) Instantiate(clampPrefab,clampSpawnPt,Quaternion.identity);
				nwClamp.parent = tmpChild.transform;			
			}
		}

	}

	public void removeClamp(string name){
		GameObject bridgeObj = GameObject.Find("Bridge");
		GameObject supportCollectionForTile = bridgeObj.transform.Find("BridgeSupports").transform.FindChild(name.Replace("Tile","Support")).gameObject;
		
		for(int i=0; i<supportCollectionForTile.transform.childCount; i++)
		{
			Transform tmpChild = supportCollectionForTile.transform.GetChild(i);
			if((tmpChild.name == "MainPipe"))
			{
				if(tmpChild.childCount==1){//Has a clamp

					Transform clamp = tmpChild.GetChild(0);
					Destroy(clamp.gameObject);

				}		
			}
		}
		
	}

	public void clearHighlights()
	{
		if(highlightStateArr != null)
		{
			for(int i=0; i<highlightStateArr.Length; i++)
			{
				highlightStateArr[i] = '0';
			}
		}
	}

	public void resetHighlightStateArr(int para_highlightStateArrLength, int para_totNumOfHighlightReq)
	{
		highlightStateArr = new char[para_highlightStateArrLength];
		for(int i=0; i<highlightStateArr.Length; i++)
		{
			highlightStateArr[i] = '0';
		}

		availableHighlightPoints = para_totNumOfHighlightReq;
		//totalPossibleHighlightPoints = availableHighlightPoints;

		createWrenchCollection(availableHighlightPoints);

		uiBounds = new Dictionary<string, Rect>();
		uiBounds.Add("CounterArea",WorldSpawnHelper.getWorldToGUIBounds(Camera.main.transform.FindChild("HighlightCountArea").transform.renderer.bounds,new bool[3] {false,true,false}));
	}

	public void autoHighlightPositions(int[] para_reqPositions)
	{
		Transform bridgeSurface = GameObject.Find("Bridge").transform.FindChild("BridgeSurface");

		for(int i=0; i<para_reqPositions.Length; i++)
		{
			highlightStateArr[para_reqPositions[i]] = '1';
			Transform reqTile = bridgeSurface.FindChild("Tile*"+para_reqPositions[i]);
			SpriteRenderer sRend = reqTile.GetComponent<SpriteRenderer>();
			sRend.color = currColorEffect;
		}
	}

	public string getHighlightBinString()
	{
		string retStr = "";
		for(int i=0; i<highlightStateArr.Length; i++)
		{
			retStr += highlightStateArr[i];
		}
		return retStr;
	}

	public int[] getHighlightLocations()
	{
		string binStr = this.getHighlightBinString();

		List<int> locs = new List<int>();
		for(int i=0; i<binStr.Length; i++)
		{
			if(binStr[i] == '1')
			{
				locs.Add(i);
			}
		}
		return locs.ToArray();
	}

	public void setInputOnState(bool para_state)
	{
		inputOn = para_state;
	}


	private void setGoBtnVisibility(bool para_state)
	{
		GameObject.Find("GlobObj").GetComponent<AcBridgeBuilderScenario>().setGoBtnVisibility(para_state);
		//goVisible = para_state;
	}


	private void createWrenchCollection(int para_maxWrenches)
	{
		Transform wrenchCollection = Camera.main.transform.FindChild("WrenchCollection");
		if(wrenchCollection != null) { Destroy(wrenchCollection.gameObject); }

		GameObject wrenchCollectionObj = new GameObject("WrenchCollection");
		wrenchCollectionObj.transform.parent = Camera.main.transform;
		wrenchCollection = wrenchCollectionObj.transform;

		GameObject highlightCountArea = Camera.main.transform.FindChild("HighlightCountArea").gameObject;
		Rect highlightCountAreaBounds = CommonUnityUtils.get2DBounds(highlightCountArea.renderer.bounds);

		Vector3 nxtWrenchSpawnPt = new Vector3(highlightCountAreaBounds.x + highlightCountAreaBounds.width - (wrenchPrefab.renderer.bounds.size.x/2f),highlightCountArea.transform.position.y,highlightCountArea.transform.position.z);
		float spacing = wrenchPrefab.renderer.bounds.size.x * 1.2f;
		for(int i=0; i<para_maxWrenches; i++)
		{
			Transform nwWrench = (Transform) Instantiate(wrenchPrefab,nxtWrenchSpawnPt,Quaternion.identity);
			nwWrench.name = "Wrench-"+i;
			nwWrench.parent = wrenchCollection;

			nxtWrenchSpawnPt.x -= spacing;
		}
	}

	private void updateCounterDisplay()
	{
		GameObject wrenchCollection = GameObject.Find("WrenchCollection");


		for(int i=0; i<wrenchCollection.transform.childCount; i++)
		{
			bool rendEnableStatus = false;
			if(i < availableHighlightPoints)
			{
				rendEnableStatus = true;
			}

			wrenchCollection.transform.FindChild("Wrench-"+i).renderer.enabled = rendEnableStatus;
		}
	}
}

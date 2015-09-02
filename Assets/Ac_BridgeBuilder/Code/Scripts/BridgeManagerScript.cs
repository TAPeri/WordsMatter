/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class BridgeManagerScript : MonoBehaviour
{
	public Transform letterTilePrefab;
	public Transform pipeBigPrefab;
	public Transform pipeMidPrefab;
	public Transform pipeSmallPrefab;
	public Transform connectorPrefab;
	public Transform wordBoxPrefab;
	public Transform sparksPrefab;

	bool[] upAxisArr = new bool[3] { false, true, false };


	string bridgeWord;
	int bridgeLengthInTiles;
	Rect bridge2DBounds;

	int prePadding;
	int postPadding;

	List<int> collapsedSegments;

	GameObject bridgeRef;
	GameObject bridgeSurfaceRef;
	//GameObject bridgeWordOverlayRef;
	GameObject bridgeSupportsRef;





	public void init(Transform para_letterTilePrefab,
	                 Transform para_pipeBigPrefab,
	                 Transform para_pipeMidPrefab,
	                 Transform para_pipeSmallPrefab,
	                 Transform para_connectorPrefab,
	                 Transform para_wordBoxPrefab,
	                 Transform para_sparksPrefab)
	{
		letterTilePrefab = para_letterTilePrefab;
		pipeBigPrefab = para_pipeBigPrefab;
		pipeMidPrefab = para_pipeMidPrefab;
		pipeSmallPrefab = para_pipeSmallPrefab;
		connectorPrefab = para_connectorPrefab;
		wordBoxPrefab = para_wordBoxPrefab;
		sparksPrefab = para_sparksPrefab;

		collapsedSegments = new List<int>();
	}


	public Rect constructBridge(int para_bridgeID, string para_bridgeWord)
	{

		bridgeRef = new GameObject("Bridge");
		bridgeSurfaceRef = new GameObject("BridgeSurface");
		//bridgeWordOverlayRef = new GameObject("BridgeWordOverlay");
		bridgeSupportsRef = new GameObject("BridgeSupports");
		
		GameObject cliffLeftObj = GameObject.Find("CliffLeft-"+para_bridgeID);
		Bounds cliffLeftBounds = cliffLeftObj.renderer.bounds;
		
		
		bridgeWord = para_bridgeWord;
		bridgeLengthInTiles = bridgeWord.Length;



		// Construct any necessary pre-padding.
		prePadding = 0;
		postPadding = 0;
		if(bridgeLengthInTiles < 10)
		{
			int rem = 10 - bridgeLengthInTiles;
			prePadding = (int) (rem/2f);
			postPadding = rem - prePadding;
		}


		
		// Build bridge surface + supports.
		Rect letterTileSpawnBounds = new Rect(cliffLeftBounds.center.x + (cliffLeftBounds.size.x/2f) - (letterTilePrefab.renderer.bounds.size.x),
		                                      cliffLeftBounds.center.y + (cliffLeftBounds.size.y/2f),
		                                      letterTilePrefab.renderer.bounds.size.x,
		                                      letterTilePrefab.renderer.bounds.size.y);


		
		bridge2DBounds = new Rect(letterTileSpawnBounds.x,
		                          letterTileSpawnBounds.y,
		                          letterTileSpawnBounds.width * ((bridgeLengthInTiles + prePadding + postPadding) * 1.0f),
		                          letterTilePrefab.renderer.bounds.size.y + pipeBigPrefab.renderer.bounds.size.y);






		int index = (0 - prePadding);
		for(int i=0; i<prePadding; i++)
		{
			buildBridgeSegment(index,"Tile*"+index,"");
			index++;
		}

		// Construct supports with word letters.
		for(int i=0; i<bridgeLengthInTiles; i++)
		{
			buildBridgeSegment(index,"Tile*"+index,""+bridgeWord[i]);		
			letterTileSpawnBounds.x += letterTileSpawnBounds.width;
			index++;
		}

		// Construct any necessary pre-padding.
		for(int i=0; i<postPadding; i++)
		{
			buildBridgeSegment(index,"Tile*"+index,"");
			index++;
		}

		
		bridgeSurfaceRef.transform.parent = bridgeRef.transform;
		//bridgeWordOverlayRef.transform.parent = bridgeRef.transform;
		bridgeSupportsRef.transform.parent = bridgeRef.transform;
		
		

		
		return bridge2DBounds;
	}


	public void collapseSupport(int para_supportIndex)
	{
		List<GameObject> gObjs = getRelevantObjsRelatedToSegment(para_supportIndex,true,true);

		for(int i=0; i<gObjs.Count; i++)
		{
			if(gObjs[i].GetComponent<Rigidbody>() == null)
			{
				gObjs[i].AddComponent<Rigidbody>();
			}
		}

		List<GameObject> detachPtObjs = getSegmentDetachPoints(para_supportIndex);
		for(int i=0; i<detachPtObjs.Count; i++)
		{
			GameObject tmpDetachPt = detachPtObjs[i];
			Vector3 sparksSpawnPt = new Vector3(tmpDetachPt.transform.position.x,
			                                    tmpDetachPt.transform.position.y,
			                                    tmpDetachPt.transform.position.z - 0.5f);

			Transform nwSparks = (Transform) Instantiate(sparksPrefab,sparksSpawnPt,sparksPrefab.rotation);
			nwSparks.renderer.sortingOrder = 50;
		}

		collapsedSegments.Add(para_supportIndex);
	}

	public void restoreEntireBridge()
	{
		Destroy(GameObject.Find("ErrorOverlay"));

		for(int i=0; i<bridgeLengthInTiles; i++)
		{
			restoreBridgeSegment(i);
		}
	}

	public void restoreBridgeSegment(int para_supportIndex)
	{
		destroyBridgeSegment(para_supportIndex);

		string content = "";
		if((para_supportIndex < 0)||(para_supportIndex >= bridgeWord.Length))
		{
			content = "";
		}
		else
		{
			content = ""+bridgeWord[para_supportIndex];
		}

		buildBridgeSegment(para_supportIndex,"Tile*"+para_supportIndex,content);
	}

	public void destroyOldBridge()
	{
		Destroy(GameObject.Find("Bridge"));
	}

	public void destroyCollapsedSegments()
	{
		for(int i=0; i<collapsedSegments.Count; i++)
		{
			destroyBridgeSegment(collapsedSegments[i]);
		}
		collapsedSegments.Clear();
	}

	public void createErrorOverlay()
	{
		destroyCollapsedSegments();

		GameObject errorOverlayObj = new GameObject("ErrorOverlay");

		Rect errorTileBounds = new Rect(bridge2DBounds.x + (prePadding * letterTilePrefab.renderer.bounds.size.x),bridge2DBounds.y,letterTilePrefab.renderer.bounds.size.x,letterTilePrefab.renderer.bounds.size.y);

		for(int i=0; i<bridgeLengthInTiles; i++)
		{
			if(bridgeSupportsRef.transform.FindChild("Support*"+i) == null)
			{
				errorTileBounds.x = (bridge2DBounds.x + (prePadding * letterTilePrefab.renderer.bounds.size.x) + (i * errorTileBounds.width));




				GameObject nwErrorTile = WorldSpawnHelper.initObjWithinWorldBounds(letterTilePrefab,"ErrorTile-"+i,errorTileBounds,-0.1f,upAxisArr);
				SpriteRenderer sRend = nwErrorTile.GetComponent<SpriteRenderer>();
				sRend.color = new Color(1f,0.6f,0.6f,0.6f);


				GameObject nwErrorTileWordBox = WordBuilderHelper.buildWordBox(99,""+bridgeWord[i],errorTileBounds,-0.2f,upAxisArr,wordBoxPrefab);
				nwErrorTileWordBox.name = "ErrorTileWordBox-"+i;
				nwErrorTileWordBox.transform.FindChild("Board").gameObject.SetActive(false);
				nwErrorTileWordBox.transform.FindChild("Text").renderer.sortingOrder = 1;
				
				WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() { nwErrorTileWordBox },0.12f);
				
				nwErrorTileWordBox.transform.parent = nwErrorTile.transform;
				nwErrorTile.transform.parent = errorOverlayObj.transform;
			}
		}
	}

	public void colourBridgeTile(int para_tileIndex, Color para_color)
	{
		Transform reqTile = bridgeSurfaceRef.transform.FindChild("Tile*"+para_tileIndex);
		if(reqTile != null)
		{
			reqTile.GetComponent<SpriteRenderer>().color = para_color;
		}
	}

	public int getWordLength() { return bridgeLengthInTiles; }


	private void destroyBridgeSegment(int para_supportIndex)
	{
		List<GameObject> gObjs = getRelevantObjsRelatedToSegment(para_supportIndex,true,true);
		for(int i=0; i<gObjs.Count; i++)
		{
			DestroyImmediate(gObjs[i]);
		}
		gObjs.Clear();

		Transform reqSupport = bridgeSupportsRef.transform.FindChild("Support*"+para_supportIndex);
		if(reqSupport != null)
		{
			DestroyImmediate(reqSupport.gameObject);
		}
	}


	

	private void buildBridgeSegment(int para_supportIndex, string para_tileName, string para_contents)
	{
		int segmentIndex = para_supportIndex;


		float baseWordLoc = bridge2DBounds.x + (prePadding * letterTilePrefab.renderer.bounds.size.x);
		float tileTLXLoc = baseWordLoc;
		if(segmentIndex < 0)
		{
			tileTLXLoc = baseWordLoc - (Mathf.Abs(segmentIndex) * letterTilePrefab.renderer.bounds.size.x);
		}
		else
		{
			tileTLXLoc = baseWordLoc + (segmentIndex * letterTilePrefab.renderer.bounds.size.x);
		}

		Rect letterTileSpawnBounds = new Rect(tileTLXLoc,
		                                      bridge2DBounds.y,
		                                      letterTilePrefab.renderer.bounds.size.x,
		                                      letterTilePrefab.renderer.bounds.size.y);


		// Surface Tile.
		GameObject nwTile = WorldSpawnHelper.initObjWithinWorldBounds(letterTilePrefab,para_tileName,letterTileSpawnBounds,-0.1f,upAxisArr);


		// Word Box.
		GameObject tileWordBox = WordBuilderHelper.buildWordBox(99,""+para_contents,letterTileSpawnBounds,-0.2f,upAxisArr,wordBoxPrefab);
		tileWordBox.name = "TileWordBox*"+segmentIndex;
		tileWordBox.transform.FindChild("Board").gameObject.SetActive(false);
		tileWordBox.transform.FindChild("Text").renderer.sortingOrder = 1;
		
		WordBuilderHelper.setBoxesToUniformTextSize(new List<GameObject>() { tileWordBox },0.12f);
		
		tileWordBox.transform.parent = nwTile.transform;
		nwTile.transform.parent = bridgeSurfaceRef.transform;
		
		
		
		
		
		// Support.
		List<GameObject> supportObjs = new List<GameObject>();
		
		// Support: Select Pipe.
		Transform nwPipe = null;
		if((segmentIndex ==0)||(segmentIndex == (bridgeLengthInTiles-1)))
		{
			// Small Pipe.
			Vector3 pipeSpawnPt = new Vector3(letterTileSpawnBounds.x + (letterTileSpawnBounds.width/2f),
			                                  letterTileSpawnBounds.y - letterTileSpawnBounds.height - (pipeSmallPrefab.renderer.bounds.size.y/2f),
			                                  nwTile.transform.position.z);
			
			nwPipe = (Transform) Instantiate(pipeSmallPrefab,pipeSpawnPt,Quaternion.identity);
		}
		else
		{
			// Large Pipe.
			Vector3 pipeSpawnPt = new Vector3(letterTileSpawnBounds.x + (letterTileSpawnBounds.width/2f),
			                                  letterTileSpawnBounds.y - letterTileSpawnBounds.height - (pipeBigPrefab.renderer.bounds.size.y/2f),
			                                  nwTile.transform.position.z);
			
			nwPipe = (Transform) Instantiate(pipeBigPrefab,pipeSpawnPt,Quaternion.identity);
		}
		nwPipe.name = "MainPipe";
		supportObjs.Add(nwPipe.gameObject);
		
		
		
		// Support: Select Hex Connector position.
		Transform nwConnector = null;
		if(segmentIndex%2 == 0)
		{
			Vector3 connectorSpawnPt = new Vector3(letterTileSpawnBounds.x + (letterTileSpawnBounds.width/2f),
			                                       letterTileSpawnBounds.y - letterTileSpawnBounds.height - (pipeSmallPrefab.renderer.bounds.size.y),
			                                       nwTile.transform.position.z);
			
			nwConnector = (Transform) Instantiate(connectorPrefab,connectorSpawnPt,Quaternion.identity);
		}
		else
		{
			Vector3 connectorSpawnPt = new Vector3(letterTileSpawnBounds.x + (letterTileSpawnBounds.width/2f),
			                                       letterTileSpawnBounds.y - letterTileSpawnBounds.height - (pipeBigPrefab.renderer.bounds.size.y/2f),
			                                       nwTile.transform.position.z);
			
			nwConnector = (Transform) Instantiate(connectorPrefab,connectorSpawnPt,Quaternion.identity);
		}
		nwConnector.name = "HexConnector";
		supportObjs.Add(nwConnector.gameObject);
		
		
		
		// Support: Connecting Pipes. (Connect to the previous support).
		if(segmentIndex != (0-prePadding))
		{
			GameObject previousConnector = bridgeSupportsRef.transform.FindChild("Support*"+(segmentIndex-1)).FindChild("HexConnector").gameObject;
			
			
			Vector3 connectingPipeSpawnPt = new Vector3(letterTileSpawnBounds.x + (letterTileSpawnBounds.width/2f) - (letterTileSpawnBounds.width/2f),
			                                            (nwConnector.position.y + previousConnector.transform.position.y)/2f,
			                                            nwTile.transform.position.z);
			
			Transform nwConnectingPipe = (Transform) Instantiate(pipeSmallPrefab,connectingPipeSpawnPt,Quaternion.identity);
			nwConnectingPipe.name = "ConnectingPipe";
			
			float zRotAngle = 0;
			if(segmentIndex%2 != 0) { zRotAngle = 60f;	} else { zRotAngle = (60f * 5f); }
			
			Vector3 tmpAngles = nwConnectingPipe.localEulerAngles;
			tmpAngles.z = zRotAngle;
			nwConnectingPipe.localEulerAngles = tmpAngles;
			
			supportObjs.Add(nwConnectingPipe.gameObject);
		}


		GameObject columnSupp = new GameObject("Support*"+segmentIndex);
		for(int k=0; k<supportObjs.Count; k++)
		{
			supportObjs[k].transform.parent = columnSupp.transform;
		}
		columnSupp.transform.parent = bridgeSupportsRef.transform;



		// If the next support already exists then recreate the right connector.
		Transform nxtSupport = bridgeSupportsRef.transform.FindChild("Support*"+(segmentIndex+1));
		if(nxtSupport != null)
		{
			Transform rPipe = nxtSupport.FindChild("ConnectingPipe");
			if(rPipe == null)
			{
				GameObject previousConnector = nwConnector.gameObject;
				GameObject rConnector = nxtSupport.FindChild("HexConnector").gameObject;
				
				
				Vector3 connectingPipeSpawnPt = new Vector3(letterTileSpawnBounds.x + (letterTileSpawnBounds.width/2f) + (letterTileSpawnBounds.width/2f),
				                                            (rConnector.transform.position.y + previousConnector.transform.position.y)/2f,
				                                            nwTile.transform.position.z);
				
				Transform nwConnectingPipe = (Transform) Instantiate(pipeSmallPrefab,connectingPipeSpawnPt,Quaternion.identity);
				nwConnectingPipe.name = "ConnectingPipe";
				
				float zRotAngle = 0;
				if((segmentIndex+1)%2 != 0) { zRotAngle = 60f;	} else { zRotAngle = (60f * 5f); }
				
				Vector3 tmpAngles = nwConnectingPipe.localEulerAngles;
				tmpAngles.z = zRotAngle;
				nwConnectingPipe.localEulerAngles = tmpAngles;
				
				nwConnectingPipe.transform.parent = nxtSupport;
			}
		}
		
		

	}


	private List<GameObject> getRelevantObjsRelatedToSegment(int para_supportIndex,
	                                                         bool para_includeLeftConnectors,
	                                                         bool para_includeRightConnectors)
	{
		List<GameObject> retList = new List<GameObject>();
		Transform reqTileTrans = bridgeSurfaceRef.transform.FindChild("Tile*"+para_supportIndex);
		if(reqTileTrans != null)
		{
			retList.Add(reqTileTrans.gameObject);
		}

		Transform reqSupport = bridgeSupportsRef.transform.FindChild("Support*"+para_supportIndex);
		if(reqSupport != null)
		{
			for(int i=0; i<reqSupport.childCount; i++)
			{
				Transform tmpChild = reqSupport.GetChild(i);

				if(tmpChild.name == "ConnectingPipe")
				{
					if(para_includeLeftConnectors)
					{
						retList.Add(tmpChild.gameObject);
					}
				}
				else
				{
					retList.Add(tmpChild.gameObject);
				}
			}
		}

		// Right connector.
		if(para_includeRightConnectors)
		{
			if(para_supportIndex < (bridgeLengthInTiles-1))
			{
				Transform nxtSupport = bridgeSupportsRef.transform.FindChild("Support*"+(para_supportIndex+1));
				if(nxtSupport != null)
				{
					Transform rightConnector = nxtSupport.FindChild("ConnectingPipe");
					if(rightConnector != null)
					{
						retList.Add(rightConnector.gameObject);
					}
				}
			}
		}

		return retList;
	}


	private List<GameObject> getSegmentDetachPoints(int para_supportIndex)
	{
		List<GameObject> retList = new List<GameObject>();

		Transform prevSupport = bridgeSupportsRef.transform.FindChild("Support*"+(para_supportIndex-1));
		if(prevSupport != null)
		{
			retList.Add(prevSupport.FindChild("HexConnector").gameObject);
		}

		Transform nxtSupport = bridgeSupportsRef.transform.FindChild("Support*"+(para_supportIndex+1));
		if(nxtSupport != null)
		{
			retList.Add(nxtSupport.FindChild("HexConnector").gameObject);
		}

		return retList;
	}
}

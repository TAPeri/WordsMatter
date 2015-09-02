/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class VehicleDamageColliderScript : MonoBehaviour
{
	HashSet<int> correctIndexes;
	HashSet<int> playerSelections;
	HashSet<int> collapsedItems;
	BridgeManagerScript bridgeMangRef;

	int mistakeCounter = 0;
	int wordLength;

	void Start()
	{
		AcBridgeBuilderScenario acScen = GameObject.Find("GlobObj").GetComponent<AcBridgeBuilderScenario>();
		correctIndexes = convertIntArrToSet(acScen.getCorrectSelections());
		playerSelections = convertIntArrToSet(acScen.getPlayerSelections());
		collapsedItems = new HashSet<int>();


		GameObject bridgeObj = GameObject.Find("Bridge");
		GameObject bridgeSurfaceObj = bridgeObj.transform.FindChild("BridgeSurface").gameObject;

		for(int i=0; i<bridgeSurfaceObj.transform.childCount; i++)
		{
			Transform tmpTile = bridgeSurfaceObj.transform.GetChild(i);
			BoxCollider bc = tmpTile.gameObject.GetComponent<BoxCollider>();
			bc.isTrigger = true;
		}

		bridgeMangRef = GameObject.Find("GlobObj").GetComponent<BridgeManagerScript>();
		wordLength = bridgeMangRef.getWordLength();
	}



	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.name.Contains("Tile"))
		{

			//Debug.Log(collider.gameObject.name);

			string[] aux = collider.gameObject.name.Split('*');
			if(aux.Length==1){//ErrorTile
				aux = collider.gameObject.name.Split('e');//ErrorTileXXX
			}
			int colTileIndex = int.Parse(aux[1]);

			if((colTileIndex >= 0)&&(colTileIndex < wordLength))
			{

				if((correctIndexes.Contains(colTileIndex))
				&&( ! playerSelections.Contains(colTileIndex))
				&&( ! collapsedItems.Contains(colTileIndex)))
				{
					bridgeMangRef.collapseSupport(colTileIndex);
					collapsedItems.Add(colTileIndex);
					mistakeCounter++;
				}
				else if((playerSelections.Contains(colTileIndex))&&( ! correctIndexes.Contains(colTileIndex)))
				{
					bridgeMangRef.colourBridgeTile(colTileIndex,new Color(1f,0.6f,0.6f));
					mistakeCounter++;
				}
				else if((playerSelections.Contains(colTileIndex))&&(correctIndexes.Contains(colTileIndex)))
				{
					bridgeMangRef.colourBridgeTile(colTileIndex,new Color(0.6f,1f,0.6f));
				}
			}

		}
	}

	void OnDestroy()
	{
		AcBridgeBuilderScenario acScen = GameObject.Find("GlobObj").GetComponent<AcBridgeBuilderScenario>();
		acScen.setMistakeCount(mistakeCounter);
	}

	public int getMistakeCount()
	{
		return mistakeCounter;
	}

	private HashSet<int> convertIntArrToSet(int[] para_arr)
	{
		HashSet<int> nwSet = new HashSet<int>();
		for(int i=0; i<para_arr.Length; i++)
		{
			if( ! nwSet.Contains(para_arr[i]))
			{
				nwSet.Add(para_arr[i]);
			}
		}
		return nwSet;
	}
}

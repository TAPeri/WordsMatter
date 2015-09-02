/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class SpatialHasher : MonoBehaviour
{

	// Key = "c,r"
	GridProperties hasherGProp;
	Dictionary<string,HashingBucket> bucketMap;
	int totItemCount;

	public SpatialHasher(GridProperties para_hasherGProp)
	{
		hasherGProp = para_hasherGProp;
		bucketMap = new Dictionary<string, HashingBucket>();
		totItemCount = 0;
	}

	public void insertItem(int para_itemID, Vector2 para_itemCentre)
	{
		int[] bucketCoords = hashPoint(para_itemCentre);
		string bucketKey = getBucketKeyFromBucketCoords(bucketCoords);
		insertToBucket(para_itemID,bucketKey);
	}

	public void insertItem(int para_itemID, Rect para_itemBounds)
	{
		List<string> relevantBuckets = hashArea(para_itemBounds);
		for(int i=0; i<relevantBuckets.Count; i++)
		{
			insertToBucket(para_itemID,relevantBuckets[i]);
		}
	}

	// Warning does not filter out items from a relevant bucket but which are out side of the range bounds.
	public List<int> areaQuery(Rect para_rangeBounds)
	{

		HashSet<int> reqItemIDs = new HashSet<int>();

		List<string> relevantBuckets = hashArea(para_rangeBounds);
		for(int i=0; i<relevantBuckets.Count; i++)
		{
			string bucketKey = relevantBuckets[i];
			if(bucketMap.ContainsKey(bucketKey))
			{
				HashingBucket reqBucket = bucketMap[bucketKey];
				HashSet<int> bucketItems = reqBucket.getAllItems();

				foreach(int tmpItem in bucketItems)
				{
					if( ! reqItemIDs.Contains(tmpItem))
					{
						reqItemIDs.Add(tmpItem);
					}
				}
			}
		}


		List<int> retList = new List<int>(reqItemIDs);
		return retList;
	}

	public int getTotalItemCount()
	{
		return totItemCount;
	}




	private void insertToBucket(int para_itemID, string para_bucketID)
	{
		if( ! bucketMap.ContainsKey(para_bucketID))
		{
			bucketMap.Add(para_bucketID,new HashingBucket());
		}

		HashingBucket reqBucket = bucketMap[para_bucketID];
		if( ! reqBucket.containsItem(para_itemID))
		{
			reqBucket.addItem(para_itemID);
			bucketMap[para_bucketID] = reqBucket;
			totItemCount++;
		}
	}
	
	private int[] hashPoint(Vector2 para_pt)
	{
		int reqColumn = (int) ((para_pt.x-hasherGProp.x)/hasherGProp.cellWidth);
		int reqRow = (int) ((hasherGProp.y - para_pt.y)/hasherGProp.cellHeight);
		int[] retCoords = new int[2] { reqColumn, reqRow };
		return retCoords;
	}

	private List<string> hashArea(Rect para_area)
	{
		Vector2 pointTL = new Vector2(para_area.x,para_area.y);
		Vector2 pointBR = new Vector2(para_area.x + para_area.width, para_area.y - para_area.height);
		
		int[] bucketTL = hashPoint(pointTL);
		int[] bucketBR = hashPoint(pointBR);
		
		List<string> relevantBuckets = new List<string>();
		for(int r=bucketTL[1]; r<(bucketBR[1]+1); r++)
		{
			for(int c=bucketTL[0]; c<(bucketBR[0]+1); c++)
			{
				relevantBuckets.Add(getBucketKeyFromBucketCoords(c,r));
			}
		}

		return relevantBuckets;
	}

	private string getBucketKeyFromBucketCoords(int[] para_bucketCoords)
	{
		string retKey = "NONE";
		if(para_bucketCoords.Length >= 2)
		{
			retKey = getBucketKeyFromBucketCoords(para_bucketCoords[0],para_bucketCoords[1]);
		}
		return retKey;
	}

	private string getBucketKeyFromBucketCoords(int para_bucketX, int para_bucketY)
	{
		string retKey = ""+para_bucketX+","+para_bucketY;
		return retKey;
	}

	private int[] getBucketCoordsFromBucketKey(string para_bucketKey)
	{
		int[] retCoords = null;
		try
		{
			string[] strArr = para_bucketKey.Split(',');
			int col = int.Parse(strArr[0]);
			int row = int.Parse(strArr[1]);
			retCoords = new int[2] { col, row };
		}
		catch(System.Exception ex)
		{
			Debug.LogError(ex.Message);
			// Error.
		}
		return retCoords;
	}
	
}

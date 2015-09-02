/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class QuestBarScript : MonoBehaviour
{


	public void init(int para_questGiverNpcID,
	                 int para_receiverNpcID,
	                 ApplicationID para_destActivityIndex)
	{
		Transform oldQuestGiverHead = transform.FindChild("QuestGiverSmallHead");
		Transform oldReceiverHead = transform.FindChild("ReceiverSmallHead");
		Transform oldInventoryItem = transform.FindChild("InventoryItem");
		if(oldQuestGiverHead != null) { Destroy(oldQuestGiverHead.gameObject); }
		if(oldReceiverHead != null) { Destroy(oldReceiverHead.gameObject); }
		if(oldInventoryItem != null) { Destroy(oldInventoryItem.gameObject); }

		GameObject questGiverHeadGuide = transform.FindChild("QuestGiverHeadGuide").gameObject;
		GameObject receiverHeadGuide = transform.FindChild("ReceiverHeadGuide").gameObject;
		GameObject inventoryItemArea = transform.FindChild("InventoryItemArea").gameObject;

		UnityEngine.Debug.Log("From "+para_questGiverNpcID+" to "+ para_receiverNpcID+" at "+para_destActivityIndex);
		Transform questGiverSmallHeadPrefab = Resources.Load<Transform>("Prefabs/Ghostbook/SmallHeads/SmallHeads_"+para_questGiverNpcID);
		Transform receiverSmallHeadPrefab = Resources.Load<Transform>("Prefabs/Ghostbook/SmallHeads/SmallHeads_"+para_receiverNpcID);
		Transform inventoryItemPrefab = Resources.Load<Transform>("Prefabs/Ghostbook/ActivitySymbols/SmallSymbols_"+para_destActivityIndex);

		bool[] upAxisArr = new bool[]{false,true,false};
		GameObject nwQuestGiverSmallHead = WorldSpawnHelper.initObjWithinWorldBounds(questGiverSmallHeadPrefab,"QuestGiverSmallHead",questGiverHeadGuide.renderer.bounds,upAxisArr);
		GameObject nwReceiverSmallHead = WorldSpawnHelper.initObjWithinWorldBounds(receiverSmallHeadPrefab,"ReceiverSmallHead",receiverHeadGuide.renderer.bounds,upAxisArr);
		GameObject nwInventoryItem = WorldSpawnHelper.initObjWithinWorldBounds(inventoryItemPrefab,"InventoryItem",inventoryItemArea.renderer.bounds,upAxisArr);

		nwQuestGiverSmallHead.transform.parent = transform;
		nwReceiverSmallHead.transform.parent = transform;
		nwInventoryItem.transform.parent = transform;

		nwQuestGiverSmallHead.renderer.sortingOrder = 135;
		nwReceiverSmallHead.renderer.sortingOrder = 135;
		nwInventoryItem.renderer.sortingOrder = 135;

		Destroy(this);
	}
}

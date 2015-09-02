/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class InventoryDiagCommand : DialogueViewCommand
{
	Transform inventoryItemPrefab;

	public InventoryDiagCommand(string para_commandName,
	                            				 Transform para_inventoryItemPrefab,
	                            				  bool para_inventoryTransferType1to2,
	                            				  bool para_needsContinueBtn,
	                            				  bool para_needsQuitBtn)
		:base(para_commandName,DialogueViewType.INVENTORY_TRANSFER_1_TO_2,para_needsContinueBtn,para_needsQuitBtn)
	{
		if(para_inventoryTransferType1to2) {  diagType = DialogueViewType.INVENTORY_TRANSFER_1_TO_2; } else { diagType = DialogueViewType.INVENTORY_TRANSFER_2_TO_1; }
		inventoryItemPrefab = para_inventoryItemPrefab;
	}

	public Transform getInventoryItemPrefab() { return inventoryItemPrefab; }
}

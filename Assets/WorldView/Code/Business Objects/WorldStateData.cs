/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

[System.Serializable]
public class WorldStateData 
{
	int[] playerAvatarCell;
	List<ActivityID> unlockedMapActivities;//Deprecated

	// This does not necessarily mean that the characters are unlocked in ghostbook.
	List<int> worldVisibleNpcIDs;


	public int[] getPlayerAvatarCell() { return playerAvatarCell; }
	private List<ActivityID> getUnlockedMapActivities() { UnityEngine.Debug.LogError("Deprecated");return unlockedMapActivities; }
	public List<int> getWorldVisibleNpcIDs() { return worldVisibleNpcIDs; }

	public void setPlayerAvatarCell(int[] para_cell)
	{
		playerAvatarCell = para_cell;
	}

	/*public void addUnlockedMapActivity(ActivityID para_nwAc)
	{
		if(unlockedMapActivities == null) { unlockedMapActivities = new HashSet<ActivityID>(); }
		unlockedMapActivities.Add(para_nwAc);
	}*/

	public void addWorldVisibleNpcID(int para_npcID)
	{
		if(worldVisibleNpcIDs == null) { worldVisibleNpcIDs = new List<int>(); }
		if( ! worldVisibleNpcIDs.Contains(para_npcID))
		{
			worldVisibleNpcIDs.Add(para_npcID);
		}
	}

	public bool isNpcVisibleInWorld(int para_npcID)
	{
		bool retFlag = false;
		if(worldVisibleNpcIDs != null)
		{
			if(worldVisibleNpcIDs.Contains(para_npcID))
			{
				retFlag = true;
			}
		}
		return retFlag;
	}

	public void initWithDefaultState()
	{
		playerAvatarCell = null;
		unlockedMapActivities = new List<ActivityID>();
		worldVisibleNpcIDs = new List<int>();
		worldVisibleNpcIDs.Add(2);
		worldVisibleNpcIDs.Add(5);
		worldVisibleNpcIDs.Add(7);
	}
}
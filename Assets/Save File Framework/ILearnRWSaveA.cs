/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
[System.Serializable]
public class ILearnRWSaveA : ILearnRWSaveFilePackage
{
	PlayerAvatarSettings playerAvatarSettings;
	WorldStateData worldStateData;
	PlayerGhostbookSatchel playerGBSatchelState;



	public ILearnRWSaveA()
	{

		// For serialisers only.
	}

	public ILearnRWSaveA(SaveVersioningInfo para_saveVersioningData,
	                     			  PlayerAvatarSettings para_playerAvSettings,
	                     			  WorldStateData para_worldStateData,
	                     PlayerGhostbookSatchel para_gbSatchelState):base(para_saveVersioningData,"")
	{


		playerAvatarSettings = para_playerAvSettings;
		worldStateData = para_worldStateData;
		playerGBSatchelState = para_gbSatchelState;
	}

	public PlayerAvatarSettings getPlayerAvatarSettings() { return playerAvatarSettings; }
	public WorldStateData getWorldStateData() { return worldStateData; }
	public PlayerGhostbookSatchel getPlayerGBSatchelState() { return playerGBSatchelState; }
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */[System.Serializable]
public class ILearnRWSaveB : ILearnRWSaveFilePackage
{

	PlayerAvatarSettings playerAvatarSettings;
	LightSatchel satchel;
	
	public ILearnRWSaveB()
	{
		
		// For serialisers only.
	}
	
	public ILearnRWSaveB(SaveVersioningInfo para_saveVersioningData,
	                     PlayerAvatarSettings para_playerAvSettings,
	                     LightSatchel para_satchel):base(para_saveVersioningData,"")
	{

		playerAvatarSettings = para_playerAvSettings;
		satchel = para_satchel;

	}
	
	public PlayerAvatarSettings getPlayerAvatarSettings() { return playerAvatarSettings; }

	public LightSatchel getPlayerGBSatchelState(){ return satchel;}


}
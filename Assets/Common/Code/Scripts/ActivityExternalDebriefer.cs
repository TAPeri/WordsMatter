/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ActivityExternalDebriefer : MonoBehaviour, CustomActionListener, IActionNotifier
{
	
	// Outcome vars.
	//ApplicationID acID;
	//bool outcomeFlag;
	ActivitySessionMetaData acMetaData;


	LocalPhotoDatabaseUpdater localPhotoDatabaseUpdaterScript;
	List<PhotoDatabaseCommand> enactedPhotoCommands;
	List<PhotoDatabaseCommand> tmpRemoveCommands;
	GhostbookManagerLight gbMang;
	ProgressScript control;
	
	//Inititates creation of characters and photo additions
	public void debrief(ApplicationID acID, bool outcomeFlag, bool activityHadAbruptExit,ActivitySessionMetaData  acMetaData,GhostbookManagerLight gbMang,List<PackagedProfileUpdate> update,int[][] severities,ProgressScript c ){

		this.control = c;
		this.gbMang = gbMang;
		//this.acID = acID;
		//this.outcomeFlag = outcomeFlag;
		this.acMetaData = acMetaData;


		enactedPhotoCommands = LocalPhotoDatabaseUpdater.obtainNecessarySeverityUpdates(
			acMetaData,
			outcomeFlag,
			gbMang,
			update,
			severities);

		//Characters are unlocked before the activities; ProgressScript will request unlocking if there is an event request that needs it
		//issueNextCharacterUnlockWindow();

		tmpRemoveCommands = new List<PhotoDatabaseCommand>();
		issueNextPhotoRelatedNotification();//following pop ups are processed with respondToEvent

	}

	List<int> potentialGBCharsToUnlock;
	List<string> potentialGBCharsNames;
	public void unlockCharacters(List<int> ids,List<string> names,ProgressScript c ){

		this.control = c;

		potentialGBCharsToUnlock = ids;
		potentialGBCharsNames = names;

		issueNextCharacterUnlockWindow();
	}



	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_sourceID == "CharacterUnlockWindow")
		{
			if(para_eventID == "Close")
			{
				potentialGBCharsToUnlock.RemoveAt(0);
				potentialGBCharsNames.RemoveAt(0);
				issueNextCharacterUnlockWindow();
			}
		}
		else if((para_sourceID == "PhotoAddedWindow")||(para_sourceID == "PhotoRemovedWindow"))
		{
			if(para_eventID == "Close")
			{
				enactedPhotoCommands.RemoveAt(0);
				issueNextPhotoRelatedNotification();
			}
		}
	}



	private void issueNextPhotoRelatedNotification()
	{
		if((enactedPhotoCommands == null)||(enactedPhotoCommands.Count <= 0))
		{
			LocalPhotoDatabaseUpdater.tmpRemovePhotos(tmpRemoveCommands,gbMang);
			control.updatesPopUpsDone();
			DestroyImmediate(this);

		}
		else
		{
			// Show photo added or photo removed popup.

			PhotoDatabaseCommand nxtCommand = enactedPhotoCommands[0];
			if(nxtCommand is PDBCAddPhoto)
			{
				showPhotoAddedPopup((PDBCAddPhoto) nxtCommand);
			}
			else if(nxtCommand is PDBCRemovePhoto)
			{
				PDBCRemovePhoto castCommand = (PDBCRemovePhoto) nxtCommand;
				showPhotoRemovedPopup(castCommand);
				tmpRemoveCommands.Add(new PDBCRemovePhoto(castCommand.questGiverID,castCommand.activityOwnerID,castCommand.activityKey,castCommand.langAreaID,castCommand.diffIndexInLangArea,castCommand.photoDiffPosition));
			}
			else
			{
				Debug.LogError("UNKNOWN TYPE");
				enactedPhotoCommands.RemoveAt(0);
				issueNextPhotoRelatedNotification();
			}
		}
	}

	private void showPhotoAddedPopup(PDBCAddPhoto para_addCommandParams)
	{
		Transform photoAddedWindowPrefab = Resources.Load<Transform>("Prefabs/PhotoAddedWindow");
		Rect origPrefab2DBounds = CommonUnityUtils.get2DBounds(photoAddedWindowPrefab.FindChild("WindowPane").renderer.bounds);
		GameObject nwPhotoAddedWindow = WorldSpawnHelper.initWorldObjAndBlowupToScreen(photoAddedWindowPrefab,origPrefab2DBounds,new Rect(0,0,0.75f,0.75f));
		nwPhotoAddedWindow.transform.position = new Vector3(0,0,Camera.main.transform.position.z + 3f);
		nwPhotoAddedWindow.transform.parent = Camera.main.transform;
		PhotoAddedWindow pawScript = nwPhotoAddedWindow.AddComponent<PhotoAddedWindow>();
		pawScript.registerListener("ActivityExternalDebriefer",this);
		pawScript.init(para_addCommandParams,acMetaData.getCharacterHelperName(),para_addCommandParams.text);
	}

	private void showPhotoRemovedPopup(PDBCRemovePhoto para_removeCommandParams)
	{
		Transform photoRemovedWindowPrefab = Resources.Load<Transform>("Prefabs/PhotoRemovedWindow");
		Rect origPrefab2DBounds = CommonUnityUtils.get2DBounds(photoRemovedWindowPrefab.FindChild("WindowPane").renderer.bounds);
		GameObject nwPhotoRemovedWindow = WorldSpawnHelper.initWorldObjAndBlowupToScreen(photoRemovedWindowPrefab,origPrefab2DBounds,new Rect(0,0,0.75f,0.75f));
		nwPhotoRemovedWindow.transform.position = new Vector3(0,0,Camera.main.transform.position.z + 3f);
		nwPhotoRemovedWindow.transform.parent = Camera.main.transform;
		PhotoRemovedWindow prwScript = nwPhotoRemovedWindow.AddComponent<PhotoRemovedWindow>();
		prwScript.registerListener("ActivityExternalDebriefer",this);
		prwScript.init(para_removeCommandParams,acMetaData.getCharacterHelperName());
	}


	private void issueNextCharacterUnlockWindow()
	{
		if((potentialGBCharsToUnlock == null)||(potentialGBCharsToUnlock.Count <= 0))
		{
			control.characterUnlockDone();
			DestroyImmediate(this);
		}
		else
		{
			showGhostbookCharacterUnlockPopup(potentialGBCharsToUnlock[0],potentialGBCharsNames[0]);
		}
	}
	
	private void showGhostbookCharacterUnlockPopup(int para_charID, string para_charName)
	{
		Transform charUnlockWindowPrefab = Resources.Load<Transform>("Prefabs/CharacterUnlockedWindow");
		Rect origPrefab2DBounds = CommonUnityUtils.get2DBounds(charUnlockWindowPrefab.FindChild("WindowPane").renderer.bounds);
		GameObject nwCharUnlockWindow = WorldSpawnHelper.initWorldObjAndBlowupToScreen(charUnlockWindowPrefab,origPrefab2DBounds,new Rect(0,0,0.75f,0.75f));
		nwCharUnlockWindow.transform.position = new Vector3(0,0,Camera.main.transform.position.z + 3f);
		nwCharUnlockWindow.transform.parent = Camera.main.transform;
		CharacterUnlockWindow cuwScript = nwCharUnlockWindow.AddComponent<CharacterUnlockWindow>();
		cuwScript.registerListener("ActivityExternalDebriefer",this);
		cuwScript.init(para_charID,para_charName);
	}



	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

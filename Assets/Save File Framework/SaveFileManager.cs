/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SaveFileManager 
{

	public static bool loadSaveFromPlayerDownload(string para_downloadedGameState,LanguageCode language, LiteracyProfile profile)
	{
		bool successFlag = false;

		LocalisationMang.init(language);
		string gameSaveStr = para_downloadedGameState;

		try
		{
			ILearnRWSaveFilePackage savePackage = ObjectSerializerHelper.deserialiseObjFromString<ILearnRWSaveFilePackage>(gameSaveStr);
			SaveVersioningInfo saveVersionInfo =  savePackage.getGameVersionInfo();

			GameObject poRef = PersistentObjMang.getInstance();
			DatastoreScript ds = poRef.GetComponent<DatastoreScript>();

			GhostbookManagerLight gbMang;

			if((saveVersionInfo==null)||(saveVersionInfo.getYearValue()==2014)){


				ILearnRWSaveA castSave = (ILearnRWSaveA) savePackage;


				PlayerAvatarSettings tmpPASettings = castSave.getPlayerAvatarSettings();
				if(tmpPASettings == null)
				{

					tmpPASettings = new PlayerAvatarSettings();
					tmpPASettings.initWithDefaultState();
				}

				ds.insertData("PlayerAvatarSettings",tmpPASettings);


				PlayerGhostbookSatchel oldSatchel = castSave.getPlayerGBSatchelState();
				gbMang = new GhostbookManagerLight(oldSatchel);


			}else{
				ILearnRWSaveB castSave = (ILearnRWSaveB) savePackage;


				PlayerAvatarSettings tmpPASettings = castSave.getPlayerAvatarSettings();
				if(tmpPASettings == null)
				{
					
					tmpPASettings = new PlayerAvatarSettings();
					tmpPASettings.initWithDefaultState();
				}
				
				ds.insertData("PlayerAvatarSettings",tmpPASettings);

				LightSatchel satchel = castSave.getPlayerGBSatchelState();
				gbMang = new GhostbookManagerLight(satchel);

			}



			List<int[]> updates = gbMang.syncWithProfile(profile);

			foreach(int[] up in updates){
				Debug.Log("Sync: "+up[0]+" "+up[1]+" "+up[2]);
			}

			ds.insertData("IsUsingSaveFile",true);

			ds.insertData("GBMang",gbMang);

			successFlag = true;
			Debug.Log("SaveManager: Success in LOADING player file.");
		}
		catch(System.Exception ex)
		{

			Debug.Log(ex.ToString());
			Debug.Log(ex.Message);
			GameObject poRef = PersistentObjMang.getInstance();
			DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
			ds.removeData("IsUsingSaveFile");
			ds.removeData("PlayerAvatarSettings");
			ds.removeData("WorldViewState");
			ds.insertData("FirstTime",true);

			Debug.LogError("SaveManager: Failed to LOAD player file.");
			successFlag = false;
		}

		return successFlag;
	}

	public static bool saveGameStateToServer(WorldViewServerCommunication para_wvsCom)
	{
		bool successFlag = false;

		try
		{

			GameObject poRef = PersistentObjMang.getInstance();
			DatastoreScript ds = poRef.GetComponent<DatastoreScript>();

			SaveVersioningInfo saveVersionInfo = (SaveVersioningInfo) ds.getData("GameVersionData");
			PlayerAvatarSettings playerAvSettings = (PlayerAvatarSettings) ds.getData("PlayerAvatarSettings");
			//WorldStateData worldstate = (WorldStateData) ds.getData("WorldViewState");
			GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
			LightSatchel gbSatchelState = gbMang.getSatchelState();

			ILearnRWSaveB saveFile = new ILearnRWSaveB(saveVersionInfo,playerAvSettings,gbSatchelState);

			string saveFileAsString = "";
			saveFileAsString = ObjectSerializerHelper.serialiseObjToString<ILearnRWSaveB>(saveFile);
			para_wvsCom.saveProgress(saveFileAsString);
			successFlag = true;


		//	ILearnRWSaveFilePackage savePackage = ObjectSerializerHelper.deserialiseObjFromString<ILearnRWSaveFilePackage>(saveFileAsString);
			
		//	SaveVersioningInfo saveVersionInfo2 =  savePackage.getGameVersionInfo();
		//	ILearnRWSaveA castSave = (ILearnRWSaveA) savePackage;



			Debug.Log("SaveManager: Success in SAVING player file.");


		}
		catch(System.Exception ex)
		{
			Debug.LogError("SaveManager: Failed to SAVE player file.");
			Debug.LogError(ex.ToString());
			successFlag = false;
		}

		return successFlag;
	}

	/*public static bool printOutSaveString(WorldViewServerCommunication para_wvsCom)
	{
		bool successFlag = false;
		
		GameObject poRef = PersistentObjMang.getInstance();
		DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
		SaveVersioningInfo saveVersionInfo = (SaveVersioningInfo) ds.getData("GameVersionData");
		PlayerAvatarSettings playerAvSettings = (PlayerAvatarSettings) ds.getData("PlayerAvatarSettings");
		WorldStateData worldstate = (WorldStateData) ds.getData("WorldViewState");
		GhostbookManager gbMang = GhostbookManager.getInstance();
		PlayerGhostbookSatchel gbSatchelState = gbMang.getSatchelState();
		
		
		ILearnRWSaveA saveFile = new ILearnRWSaveA(saveVersionInfo,playerAvSettings,worldstate,gbSatchelState);
		
		string saveFileAsString = "";
		saveFileAsString = ObjectSerializerHelper.serialiseObjToString<ILearnRWSaveA>(saveFile);
		
		Debug.Log("Save File String Here: '"+saveFileAsString+"'EndOfF");


		
		
		return successFlag;
	}

	public static bool restoreStateFromString()
	{
		TextAsset ta = Resources.Load<TextAsset>("DummySStateFile");
		string tmpSS = ta.text;

		ILearnRWSaveFilePackage savePackage = ObjectSerializerHelper.deserialiseObjFromString<ILearnRWSaveFilePackage>(tmpSS);
		SaveVersioningInfo saveVersionInfo =  savePackage.getGameVersionInfo();
		ILearnRWSaveA castSave = (ILearnRWSaveA) savePackage;
		
		GameObject poRef = PersistentObjMang.getInstance();
		DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
		ds.insertData("PlayerAvatarSettings",castSave.getPlayerAvatarSettings());
		ds.insertData("WorldViewState",castSave.getWorldStateData());
		GhostbookManager gbMang = GhostbookManager.getInstance();
		gbMang.setSatchelState(castSave.getPlayerGBSatchelState());

		return true;
	}*/

}

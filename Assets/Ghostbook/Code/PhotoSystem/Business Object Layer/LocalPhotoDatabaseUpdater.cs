/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */


// This class is called by the ActivityExternalDebriefer after an activity is completed and the game has returned to the world view.
// The main aim of the LocalPhotoDatabaseUpdater is to use the get severities server service, compare the data with the state of the player's local photo database
// and then perform necessary updates if the server instructs that photos should be given (in the case of increase in player success and therefore lowering of severity for a particular Language Area and Difficulty)
// or removed/untagged (in the case of decrease in player success and therefore increasing severity for a particular Language Area and Difficulty).

using UnityEngine;
using System.Collections.Generic;

public class LocalPhotoDatabaseUpdater
{

	static public List<PhotoDatabaseCommand> obtainNecessarySeverityUpdates(ActivitySessionMetaData acMetaData, bool para_otherData,GhostbookManagerLight gbMang,List<PackagedProfileUpdate> updateList,int[][] severities)
	{

		int usedLangArea = acMetaData.getLangAreaID();
		int usedDiffForLangArea = acMetaData.getDiffIndexForLangArea();
		bool outcome =  para_otherData;


		List<PhotoDatabaseCommand> commandQueue = null;

		//First check if the language area played had one photo already
		PhotoAlbum album = gbMang.getPhotoAlbumForCharacter(acMetaData.getQuestGiverID());
		PhotoPage reqPage = album.findSpecificPage(usedLangArea,usedDiffForLangArea);

		ApplicationID acPkey = acMetaData.getApplicationID();
		int activityOwnerID = LocalisationMang.getOwnerNpcOfActivity(acPkey);

		commandQueue = new List<PhotoDatabaseCommand>();

		if(reqPage.getNumAvailablePhotos() == 0)//The first picture is free
			{
				if(outcome == true)
				{
					// This lang-diff combo has been played for the first time. Create the first photo automatically.
					// (Server side starts all severities at value 3 (value 0 meaning no literacy problems) and value 3 means 1 photo).
					// Due to this setup on the server side, the first photo in every photo page never gets removed once created.


				string[] sectionText = LocalisationMang.getBioSection(acMetaData.getQuestGiverID(),0);

				PDBCAddPhoto tmp = new PDBCAddPhoto(acMetaData.getQuestGiverID(),activityOwnerID,acPkey,usedLangArea,usedDiffForLangArea,0,sectionText);
				commandQueue.Add(tmp);
				gbMang.addPhoto(tmp);
			}

		}

		//Add more photos based on server update
		if(updateList != null)
			{
				Debug.Log(updateList.Count);
					for(int i=0; i<updateList.Count; i++)
					{
						PackagedProfileUpdate tmpItem = updateList[i];

						int tmpNPCid = -1;
				
						PhotoAlbum tmpAlbum = null;
						PhotoPage tmpPage = null;


						if(tmpItem == null)//Provides sync of the profile and picture
						{
							Debug.Log("Null update");

							tmpNPCid = gbMang.getNpcIDForLangAreaDifficulty(usedLangArea,usedDiffForLangArea);
							tmpAlbum = gbMang.getPhotoAlbumForCharacter(tmpNPCid);
							tmpPage = tmpAlbum.findSpecificPage(usedLangArea,usedDiffForLangArea);


							int severityPhoto = 4-tmpPage.getNumAvailablePhotos();
							if (severityPhoto==4)
								severityPhoto--;

							tmpItem = new PackagedProfileUpdate(usedLangArea, usedDiffForLangArea,severityPhoto, severities[usedLangArea][usedDiffForLangArea] , -1,-1);

						}else{


							tmpNPCid = gbMang.getNpcIDForLangAreaDifficulty(tmpItem.category,tmpItem.index);
							tmpAlbum = gbMang.getPhotoAlbumForCharacter(tmpNPCid);
							tmpPage = tmpAlbum.findSpecificPage(tmpItem.category,tmpItem.index);
							//Update the profile
							Debug.Log("Copy of the profile updated");
							severities[usedLangArea][usedDiffForLangArea] = tmpItem.newSeverity;

						}

						int reqPhotoDiff = 4 - tmpItem.newSeverity - tmpPage.getNumAvailablePhotos();

						/*4 -> 0 : 0
						4 -> 1: -1
						4 -> 2: -2
						4 -> 3: -3

						3 -> 0: +1
						3 -> 3: -2

						0 -> 3: +1
						0 -> 0: +4
*/

						Debug.Log("Severity:"+tmpItem.newSeverity+" NumPhotos:"+tmpPage.getNumAvailablePhotos()+" Add:"+reqPhotoDiff);

						if(reqPhotoDiff != 0)
						{

								if(reqPhotoDiff < 0)
								{
								int numPhotosToRemove = Mathf.Abs(reqPhotoDiff);
									int adding = tmpPage.getNumAvailablePhotos();
									for(int k=0; k<numPhotosToRemove; k++)
									{
										int reqRemovePos = adding-1-k;
										if(reqRemovePos >= 0)
										{
											if( ! tmpPage.isPhotoStickPosVacant(reqRemovePos))
											{
												commandQueue.Add(new PDBCRemovePhoto(tmpNPCid,activityOwnerID,acPkey,tmpItem.category,tmpItem.index,reqRemovePos));
											// NOTE: We do not remove photos here because the popups will need the photos.
											// Once popups are done, they will then call tmpRemovePhotos() below.
										}
										}
									}
								}
								else if(reqPhotoDiff > 0)
								{
									int numPhotosToAdd = Mathf.Abs(reqPhotoDiff);
									int adding = tmpPage.getNumAvailablePhotos();

									List<PDBCAddPhoto> gbUpdates = new List<PDBCAddPhoto>();

									for(int k=0; k<numPhotosToAdd; k++)
									{
										int reqStickPos = adding+k;
										if(reqStickPos <= 3)
										{
											if(tmpPage.isPhotoStickPosVacant(reqStickPos))
											{

												int sectionIdx = -1;
												for(int j=0;j<7;j++){
													if(!gbMang.getContactBioSectionStatus(tmpNPCid  ,j)){
														gbMang.unlockSection(tmpNPCid );
														sectionIdx = j;
														break;
													}
												}

												if(sectionIdx == -1)
													sectionIdx = UnityEngine.Random.Range(0,7);


												string[] sectionText = LocalisationMang.getBioSection(tmpNPCid,sectionIdx);
									

												gbUpdates.Add(new PDBCAddPhoto(tmpNPCid,activityOwnerID,acPkey,tmpItem.category,tmpItem.index,reqStickPos,sectionText));
												commandQueue.Add(gbUpdates[gbUpdates.Count-1]);
											}
										}
									}

									foreach(PDBCAddPhoto addCommand in gbUpdates)
										gbMang.addPhoto(addCommand);
								}
							}
						
					}
			}else{

				Debug.LogError("Update list was null (quit activity)");

			}

		return commandQueue;

	}

	

	static public void tmpRemovePhotos(List<PhotoDatabaseCommand> para_commands,GhostbookManagerLight gbMang)
	{
		if(para_commands != null)
		{
			if(para_commands.Count > 0)
			{
				for(int i=0; i<para_commands.Count; i++)
				{
					PhotoDatabaseCommand nxtCommand = para_commands[i];

					if(nxtCommand is PDBCRemovePhoto)
					{
						PDBCRemovePhoto castRemoveCommand = (PDBCRemovePhoto) nxtCommand;
						gbMang.removePhoto(castRemoveCommand);
					}
					
				}
			}
		}

	}


}

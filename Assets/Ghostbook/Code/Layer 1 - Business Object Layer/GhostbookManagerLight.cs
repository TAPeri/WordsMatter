/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;
using UnityEngine;

public class GhostbookManagerLight 
{
	// Singleton instance.
	private static GhostbookManagerLight uniqueGBInstance = null;

	EventsList events;
	NewsfeedList newsfeed;

	LightSatchel satchel;			//Basic permanent information
	DifficultiesDescription profile;//Profile information from the server

	string[] NPCgameNames; //Name of the characters in the game indexed by ID
	Dictionary<string,int> NPCserverNamesID;//Character IDs indexed by their server name

	List<int> unlockedCharacters; //Indexes of unlocked character
	bool[][] unlockedDifficulties;


	int[] characterLanguageAreas; //Language area for each character
	List<int>[] characterDifficulties; //List of difficulties for each character

	//Encounter[] encounters;//Game parameters attached to each character

	List<int> supportedClusters;

	List<ApplicationID> availableActivities;

	//Characters unlocked
	public List<int> visibleCharacters(){
		return unlockedCharacters;
	}


	public string getNPCserverName(int charID){
		foreach (KeyValuePair<string, int> pair in NPCserverNamesID)
		{
			if(pair.Value==charID)
				return pair.Key;
		}

		return "";

	}


	public static GhostbookManagerLight getInstance()
	{
		if(uniqueGBInstance == null)
		{
			UnityEngine.GameObject poRef = PersistentObjMang.getInstance();
			DatastoreScript ds = poRef.GetComponent<DatastoreScript>();
			
			if(ds.containsData("GBMang"))
			{
				GhostbookManagerLight extractedData = (GhostbookManagerLight) ds.getData("GBMang");
				if(extractedData != null)
				{
					uniqueGBInstance = extractedData;
				}
				else
				{
					uniqueGBInstance = new GhostbookManagerLight();
					ds.insertData("GBMang",uniqueGBInstance);
				}
			}
			else
			{
				uniqueGBInstance = new GhostbookManagerLight();
				ds.insertData("GBMang",uniqueGBInstance);
			}
		}
		
		return uniqueGBInstance;
	}


	public GhostbookManagerLight(){
		Debug.Log("GB: Brand new");

		performLoadProcedure();

		List<string[]> npcNames = LocalisationMang.getBothNPCnames();

		satchel = new LightSatchel(npcNames.Count,profile.getDifficulties());

		//unlockAllCharacters();
		
		initUnlockedCharactersAndDifficulties();


		//unlockAllRelatedDifficulties(supportedClusters[2]);//unlock all previous and 2 ahead


	}

	public GhostbookManagerLight(LightSatchel newSatchel  ){
		Debug.Log("GB: Loaded from Light");

		performLoadProcedure();
		
		satchel = newSatchel;

		initUnlockedCharactersAndDifficulties();


	}


	public GhostbookManagerLight( PlayerGhostbookSatchel oldSatchel  ){
		Debug.Log("GB: Loaded from Heavy");


		List<string[]> npcNames = LocalisationMang.getBothNPCnames();


		performLoadProcedure();
		satchel = new LightSatchel(npcNames.Count,profile.getDifficulties());

		if(oldSatchel==null){
			Debug.Log("GB: Brand new");


//			unlockAllRelatedDifficulties(supportedClusters[0]);

		}else{

			//unlock all clusters
			for(int lA = 0; lA < profile.getDifficulties().Length;lA++){

				for(int diff = 0; diff < profile.getDifficulties()[lA].Length;diff++){
					Difficulty difficulty = profile.getDifficulties()[lA][diff];
					if(!satchel.getUnlockedClusters().Contains((byte)difficulty.cluster)){
						satchel.getUnlockedClusters().Add((byte)difficulty.cluster);
					}

				}
			}


			//unlock all bio sections

			for(int i = 0; i< npcNames.Count;i++)
				for(int j = 0;j<7;j++)//7 bio sections
					satchel.unlockBio(j);


			//Add old photos
			List<ContactSlot> characters = oldSatchel.getContactList().getAllSlotsInOrder();

			foreach(ContactSlot contact in characters){

				PhotoAlbum album = contact.getPhotoAlbum();

				foreach(PhotoPage page in album.getAllAvailablePages()){


					if(page.getNumAvailablePhotos()>0){
						if(page.getLangArea()<satchel.getPhotos().Length){
							if(page.getDifficulty() < satchel.getPhotos()[page.getLangArea()].Length){
							     foreach(Photo photo in page.getAvailablePhotos().Values){
									satchel.getPhotos()[page.getLangArea()][page.getDifficulty()].Add(photo);
								}

							}
						}

					}

				}

			}


		}

		initUnlockedCharactersAndDifficulties();

	}

	public LightSatchel getSatchelState(){
		return satchel;
	}

	public void unlockSection(int charID){
		satchel.unlockBio(charID);
	}

	private void performLoadProcedure()
	{

		List<string[]> npcNames = LocalisationMang.getBothNPCnames();

		NPCgameNames = new string[npcNames.Count];
		NPCserverNamesID = new Dictionary<string,int>();

		characterLanguageAreas = new int[npcNames.Count] ;
		characterDifficulties  = new List<int>[npcNames.Count]; 
		//encounters = new Encounter[npcNames.Count]; 

		for(int i = 0; i <npcNames.Count;i++){

			NPCgameNames[i] = npcNames[i][0];
			NPCserverNamesID[npcNames[i][1]] = i;


			characterDifficulties[i] = new List<int>();

		}


		profile = WorldViewServerCommunication.userProfile.getLiteracyProfile().getDifficultiesDescription();

		for(int lA = 0;lA<profile.getDifficulties().Length;lA++){
			for(int diff = 0; diff<profile.getDifficulties()[lA].Length;diff++){

				Difficulty difficulty = profile.getDifficulties()[lA][diff];

				characterLanguageAreas[ NPCserverNamesID[difficulty.character] ] = lA;
				characterDifficulties[ NPCserverNamesID[difficulty.character] ].Add(diff);

			}
		}


		events = new EventsList(true);
		newsfeed = new NewsfeedList(true);

		//UnityEngine.GameObject poRef = PersistentObjMang.getInstance();
		//DatastoreScript ds = poRef.GetComponent<DatastoreScript>();

	}


	public List<int[]> syncWithProfile(LiteracyProfile profile){

		//int[][] severities){

		List<int[]> listUpdates = new List<int[]>();

		List<byte> clusters = satchel.getUnlockedClusters();

		for(int i = 0; i<profile.getUserSeveritites().getSeverities().Length;i++){

			for(int j = 0; j<profile.getUserSeveritites().getSeverities()[i].Length;j++){

				int empty = 3;
				if(profile.getDifficultiesDescription().getLanguageAreas()[i].getSeverityType().Equals("binary"))//zeroToThree
					empty = 1;

				if(profile.getUserSeveritites().getSeverities()[i][j]<empty){

					if(!clusters.Contains((byte)profile.getDifficultiesDescription().getDifficulties()[i][j].cluster)){
						listUpdates.Add(new int[]{profile.getDifficultiesDescription().getDifficulties()[i][j].cluster,i,j});
						unlockAllRelatedDifficulties(profile.getDifficultiesDescription().getDifficulties()[i][j].cluster);
					}

				}
			}

		}

		return listUpdates;
	}


	/******************************************  ******  *************************************************/
	/******************************************  ******  *************************************************/
	/*****************************************  CHARACTERS  *************************************************/
	/******************************************  ******  *************************************************/
	/******************************************  ******  *************************************************/

	public List<ApplicationID> getAvailableActivities(){

		return availableActivities;
	}
	



	public string getNameForLangArea(int lA){


		return profile.getLanguageAreas()[lA].getType().getUrl();
//		return profile.getLanguageAreas()[lA].getURI();
	}

	public int numberOfLanguageAreas(){
		return profile.getDifficulties().Length;
	}

	public List<int> getAllNpcsForLangArea(int lA){

		List<int> output = new List<int>();
		for(int i = 0; i<characterLanguageAreas.Length;i++){

			if(characterLanguageAreas[i]==lA)
				output.Add(i);
		}
		return output;
	}


	public int getNpcIDForLangAreaDifficulty(int lA,int diff){


		//Debug.Log("Language area "+lA+"*"+diff+" corresponds to NPC "+NPCserverNamesID[ profile.getDifficulties()[lA][diff].character  ]);
		return NPCserverNamesID[ profile.getDifficulties()[lA][diff].character  ];

	}

	public int getLangAreaForNPCID(int charID){

		return characterLanguageAreas[charID];
	}


	public List<int> getDiffForNPCID(int charID){

		return characterDifficulties[charID];

	}


	public bool difficultyUnlocked(int la,int diff){
		return unlockedDifficulties[la][diff];
	}

	public void unlockSingleDifficulty(int lA,int diff){
		Debug.LogWarning("Difficulty unlocked without cluster");
		unlockedDifficulties[lA][diff] = true;
	}

	private List<int[]> unlockDifficulties(int cluster){


		List<int[]> unlocks = new List<int[]>();

		for(int lA = 0; lA < profile.getDifficulties().Length;lA++){

			for(int diff = 0; diff<  profile.getDifficulties()[lA].Length;diff++){
				
				if( (byte)profile.getDifficulties()[lA][diff].cluster==cluster){
					unlocks.Add(new int[]{lA,diff});
					unlockedDifficulties[lA][diff] = true;
					/*if(!unlockedCharacters.Contains(NPCserverNamesID[profile.getDifficulties()[lA][diff].character])){
						unlockedCharacters.Add(NPCserverNamesID[profile.getDifficulties()[lA][diff].character]);
					}*/
				}
			}
			
		}

		return unlocks;

	}

	private void initUnlockedCharactersAndDifficulties(){

		unlockedCharacters = new List<int>();
		unlockedDifficulties = new bool[profile.getDifficulties().Length][];
		availableActivities = new List<ApplicationID>();

		supportedClusters = new List<int>();

		//string listClusters = "";

		for(int lA = 0; lA < profile.getDifficulties().Length;lA++){
			unlockedDifficulties[lA] = new bool[profile.getDifficulties()[lA].Length];

			for(int diff = 0; diff<  profile.getDifficulties()[lA].Length;diff++){

				if(!supportedClusters.Contains(profile.getDifficulties()[lA][diff].cluster)){//Adds all clusters in order

					bool inserted = false;
					for(int i = 0;i<supportedClusters.Count;i++){
						if(supportedClusters[i]>profile.getDifficulties()[lA][diff].cluster){

							supportedClusters.Insert(i,profile.getDifficulties()[lA][diff].cluster);
							inserted = true;
							break;
						}
					}
					if(!inserted)
						supportedClusters.Add(profile.getDifficulties()[lA][diff].cluster);
				}

				/*listClusters += profile.getDifficulties()[lA][diff].cluster+" ";
				foreach(string a in profile.getDifficulties()[lA][diff].descriptions)
					listClusters+= a+",";
				listClusters+="\n";*/

				if( satchel.getUnlockedClusters().Contains((byte)profile.getDifficulties()[lA][diff].cluster)){

					unlockedDifficulties[lA][diff] = true;
					unlockCharacter(NPCserverNamesID[profile.getDifficulties()[lA][diff].character]);

				}else{
					unlockedDifficulties[lA][diff] = false;

				}
			}


		}

		//Debug.Log(listClusters);

		if(satchel.getUnlockedClusters().Count<3)//force unlock at least 5 clusters
			unlockAllRelatedDifficulties(supportedClusters[0]);//unlock all previous and 2 ahead

		Debug.Log(satchel.getUnlockedClusters().Count+" clusters unlocked");




	}




	//List of characters (ID,name,status)
	public List<ContactPortraitSnippit> getContactPortraitSnippitsInOrder()
	{
		Debug.LogWarning("No order");
		List<ContactPortraitSnippit> reqSnippits = new List<ContactPortraitSnippit>();

		for(int i=0; i<NPCgameNames.Length; i++){

			if(unlockedCharacters.Contains(i))  reqSnippits.Add(new ContactPortraitSnippit(i,NPCgameNames[i],CharacterStatus.UNLOCKED));
			else 								reqSnippits.Add(new ContactPortraitSnippit(i,NPCgameNames[i],CharacterStatus.LOCKED));

		}
		
		return reqSnippits;
	}


	//Information for a character album (status, short descriptions, ID, unlocked difficulties, short descriptions)
	public ContactPageInfoSnippit getContactPageInfoSnippit(int para_charID)
	{

		CharacterStatus status = CharacterStatus.LOCKED;
		if(unlockedCharacters.Contains(para_charID))
		   status = CharacterStatus.UNLOCKED;


		ContactPortraitSnippit pSnippit = new ContactPortraitSnippit(para_charID,NPCgameNames[para_charID],status);

		PhotoAlbum pPA = createAlbum(para_charID);

		string characterShortDescription = LocalisationMang.getFullExtensiveBio(para_charID).Split('\n')[0];
		ContactPageInfoSnippit pageSnippit = new ContactPageInfoSnippit(pSnippit,characterShortDescription,pPA);
		
		return pageSnippit;
	}



	public string createExplanation(int lA,int diff){
		
		string difficultyShortDescription = profile.getDifficulties()[lA][diff].getDescriptionsToString();
		if(profile.getDifficulties()[lA][diff].humanReadableDescription.Contains("<>"))
			difficultyShortDescription = profile.getDifficulties()[lA][diff].humanReadableDescription.Replace("<","").Split('>')[1].Trim();
		

		return difficultyShortDescription;
		
	}

	public string createDifficultyShortDescription(int lA,int diff){

		string difficultyShortDescription = profile.getDifficulties()[lA][diff].getDescriptionsToString();
		if(profile.getDifficulties()[lA][diff].humanReadableDescription.Contains("<>"))
			difficultyShortDescription = profile.getDifficulties()[lA][diff].humanReadableDescription.Replace("<","").Split('>')[0].Trim();


		return difficultyShortDescription;

	}

	private PhotoAlbum createAlbum(int para_charID){


		List<DifficultyMetaData> difficulties = new List<DifficultyMetaData>();

		int lA = characterLanguageAreas[ para_charID] ;

		//Debug.Log("Creating album for character "+para_charID+" and LA "+lA);

		foreach(int diff in characterDifficulties[para_charID]){
			
			if(unlockedDifficulties[lA][diff]){

				difficulties.Add(new DifficultyMetaData(lA+"*"+diff, createDifficultyShortDescription(lA,diff),createExplanation(lA,diff)));
				//Debug.Log(lA+"*"+diff+" -> "+difficultyShortDescription);
				
			}
		}
		
		
		PhotoAlbum pPA = new PhotoAlbum(para_charID, difficulties  );

		foreach(PhotoPage page in pPA.getAllAvailablePages()){

			int i = 0;
			foreach(Photo photo in satchel.getPhotos()[page.getLangArea()][page.getDifficulty()]){

				page.addPhoto(photo,i++);
			}
		}

		return pPA;

	}


	public CharacterStatus getContactStatus(int para_charID)
	{

		if(unlockedCharacters.Contains(para_charID))
			return CharacterStatus.UNLOCKED;
		else
			return CharacterStatus.LOCKED;

	}

	public bool getContactBioSectionStatus(int charID,int section){
		return satchel.isBioUnlocked(charID,section);
	}

	public bool unlockCharacter(int para_charID){

		if(!unlockedCharacters.Contains(para_charID)){
			unlockedCharacters.Add(para_charID);
			if(LocalisationMang.checkIfNpcIsMainChar(para_charID))//Main character
				availableActivities.Add(LocalisationMang.getMainApplicationID(para_charID));
			return true;
		}else{
			return false;
		}
	}

	public bool unlockCharacterForLocation(int para_charID){

		Debug.LogWarning("A character without unlocked difficulties is being unlocked");
		//unlockSingleDifficulty(characterLanguageAreas[para_charID],characterDifficulties[para_charID][0]);
		return unlockCharacter(para_charID);
	}


	public List<int[]> unlockAllRelatedDifficulties(int lA, int diff){//Unlocks all previous clusters + 2 ahead
			
			int cluster = profile.getDifficulties()[lA][diff].cluster;
		return unlockAllRelatedDifficulties(cluster);
	}

	private List<int[]> unlockAllRelatedDifficulties(int cluster){//Unlocks all previous clusters + 2 ahead
		List<int[]> output = new List<int[]>();

			int clusterIdx = supportedClusters.IndexOf(cluster);
			clusterIdx += 2;

			if(clusterIdx>=supportedClusters.Count){
				clusterIdx =supportedClusters.Count-1; 
			}

			for(int i = 0; i<= clusterIdx; i++){
				if(satchel.unlockCluster(supportedClusters[i])){//if it was locked
					List<int[]> tmp = unlockDifficulties(supportedClusters[i]);
					foreach(int[] pair in tmp)
						output.Add(pair);
				}

			}

			return output;

	}


	/*public List<int[]> setEncounterInfoForCharacter(int para_charID, Encounter para_enc, bool unlockCluster)
	{
		Debug.LogWarning("ONLY PROGRESS SCRIPT");

		encounters[para_charID] = para_enc;


		if(unlockCluster){
			int cluster = profile.getDifficulties()[para_enc.getLanguageArea()][para_enc.getDifficulty()].cluster;

			satchel.unlockCluster(cluster);
			return unlockDifficulties(cluster);

		}else
			return new List<int[]>();



	}*/



/*	public Encounter getEncounterInfoForCharacter(int para_charID)
	{
		return encounters[para_charID];

	}*/



	/******************************************  ******  *************************************************/
	/******************************************  ******  *************************************************/
	/*****************************************  PHOTOS   *************************************************/
	/******************************************  ******  *************************************************/
	/******************************************  ******  *************************************************/
	




	public PhotoAlbum getPhotoAlbumForCharacter(int para_charID)
	{

		return createAlbum(para_charID);

	}

	
	public void addPhoto(int para_questGiverID,
	                     int para_activityOwnerID,
	                     ApplicationID para_activityKey,
	                     int para_langAreaID,
	                     int para_difficultyIndexInLangArea,
	                     int para_photoDifficultyPosition)
	{

		if(  satchel.getPhotos()[para_langAreaID][para_difficultyIndexInLangArea].Count < 4)
		{
			PhotoRecorder photoRecorder = new PhotoRecorder();
			Photo photo = photoRecorder.buildNewPhotoData(para_questGiverID,para_activityOwnerID,para_activityKey,para_langAreaID,para_difficultyIndexInLangArea);

			satchel.getPhotos()[para_langAreaID][para_difficultyIndexInLangArea].Add(photo);
		}
	}

	public void addPhoto(PDBCAddPhoto para_addPhotoCommand)
	{
		addPhoto(para_addPhotoCommand.questGiverID,
		         para_addPhotoCommand.activityOwnerID,
		         para_addPhotoCommand.activityKey,
		         para_addPhotoCommand.langAreaID,
		         para_addPhotoCommand.diffIndexInLangArea,
		         para_addPhotoCommand.photoDiffPosition);
	}

	public void removePhoto(int para_questGiverID,
	                        int para_langAreaID,
	                        int para_difficultyIndexInLangArea,
	                        int para_photoDifficultyPosition)
	{

		if(satchel.getPhotos()[para_langAreaID][para_difficultyIndexInLangArea].Count>para_photoDifficultyPosition)
			satchel.getPhotos()[para_langAreaID][para_difficultyIndexInLangArea].RemoveAt(para_photoDifficultyPosition);
		else
			Debug.LogError("Wrong photo index");

	}


	public void removePhoto(PDBCRemovePhoto para_removePhotoCommand)
	{
		removePhoto(para_removePhotoCommand.questGiverID,
		            para_removePhotoCommand.langAreaID,
		            para_removePhotoCommand.diffIndexInLangArea,
		            para_removePhotoCommand.photoDiffPosition);
	}


	public Photo getPhoto(int para_charID, int para_langAreaID, int para_diffIndexInLangArea, int para_photoPosition)
	{
		if(satchel.getPhotos()[para_langAreaID][para_diffIndexInLangArea].Count>para_photoPosition)
			return satchel.getPhotos()[para_langAreaID][para_diffIndexInLangArea][para_photoPosition];
		else
			return null;

	}

	/******************************************  ******  *************************************************/
	/******************************************  ******  *************************************************/
	/******************************************  EVENTS  *************************************************/
	/******************************************  ******  *************************************************/
	/******************************************  ******  *************************************************/



	// IEventsServices.
	public List<EventSummarySnippit> getAvailableEvents()
	{
		List<EventSummarySnippit> retList = new List<EventSummarySnippit>();
		
		List<EventSlot> eSlots = events.getAllEventSlots();

		for(int i=0; i<eSlots.Count; i++)
		{
			EventSlot tmpSlot = eSlots[i];
			string acShorthand = LocalisationMang.getActivityShorthand(tmpSlot.getApplicationID());
			int reqLA = tmpSlot.getEncounter().getLanguageArea();
			int reqDiff = tmpSlot.getEncounter().getDifficulty();

			string readableLA = getNameForLangArea(reqLA);

			string readableDiff = createDifficultyShortDescription(reqLA,reqDiff);


			retList.Add(new EventSummarySnippit(tmpSlot, LocalisationMang.getString(acShorthand+"*"+"EventPop"),readableLA,readableDiff));
		}
		
		return retList;
	}
	
	public void addEvent(int para_questGiverID, int para_questReceiverID,ApplicationID para_acID, Encounter para_encFullData)
	{

		events.addEvent(para_questGiverID, para_questReceiverID, para_acID,para_encFullData);
	}
	
	public void clearEventList(){
		events.reset();
	}
	
	
	public void removeEventAtIndex(int para_index)
	{
		events.removeEventAtIndex(para_index);
	}
	
	
	public void removeAllEvents()
	{
		events.reset();
	}

	/******************************************  ******  *************************************************/
	/******************************************  ******  *************************************************/
	/*****************************************  NEWSFEED  *************************************************/
	/******************************************  ******  *************************************************/
	/******************************************  ******  *************************************************/


	// INewsfeedServices.
	public List<NewsItem> getNewsItems()
	{
		return newsfeed.getAllNewsItems();

	}
	
	public void addNewsItem(int para_newsType, string para_newsText)
	{
		newsfeed.addNewsItem(para_newsType,para_newsText);

	}
	
	public void addNewsItemPastActivity(ApplicationID para_acPlayed, int para_langArea, int para_difficulty, int para_questGiverID,string level, string date)
	{
		newsfeed.addNewsItemPastActivity(para_acPlayed,para_langArea,para_difficulty,para_questGiverID,level,date);

	}
	
	public void eraseNewsItems()
	{
		newsfeed.eraseAllItems();

	}


}
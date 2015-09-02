/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using System.Collections;
using System.Collections.Generic;

public interface IServerServices
{
	// These words do not need to make sense when read collectively.

	bool connectedWithTeacher();
	
	GetRoutine requestAuthentication(string student_name,string student_pass);
	GetRoutine requestAuthentication(string student_name,string student_pass,string teacher_name,string teacher_pass);
	GetRoutine requestRefreshAuthentication();
	
	GetRoutine requestUserProfile(string userID);
	GetRoutine requestUserDetails();


	GetRoutine requestSavefile();

	GetRoutine requestServerVersion();
	
	GetRoutine requestUserLogs(
		string para_timestart,
		string para_timeend,
		int para_page,
		string[] para_tags,
		string para_applicationID,
		string para_sessionID);

	GetRoutine requestNewsFeed(
		string para_timestart,
		string para_timeend,
		int para_page,
		string[] para_tags,
		string para_applicationID,
		string para_sessionID);
	
	GetRoutine requestLevel(string language,ApplicationID appID, int languageArea,int difficulty, int index);
	string loadLevel();

	GetRoutine requestNextActivity(string userID);
	GetRoutine requestNextActivity(string userID,int languageArea,int difficulty);
	GetRoutine requestNextActivity(string userID,int languageArea,int difficulty,string game);
	GetRoutine requestNextActivity(string userID,string character);
	GetRoutine requestNextActivity(string userID,string character,string game);

	GetRoutine requestNextWords(string app, int number_words,int difficultyID, int languageArea, string userID,int evaluation_mode,string level);
	
	GetRoutine requestProfileUpdate(int difficulty,int languageArea,string userID);
	
	
	PackagedUserProfile getUserProfile();
	PackagedUserDetails getUserDetails();
	PackagedUserLogs getUserLogs();
	PackagedUserLogs getNewsFeed();
	PackagedUserLogs getSaveFiles();

	
	PackagedServerInfo getServerInfo();
	
	
	List<PackagedNextWord> getNextWords();
	List<PackagedNextActivity> getNextActivity();
	
	GetRoutine errorHandler(GetRoutine error);
	
	void logData(UserLogNoTimestamp log);
	GetRoutine flushLogs();
	
	List<PackagedProfileUpdate> getProfileUpdate();
	
}

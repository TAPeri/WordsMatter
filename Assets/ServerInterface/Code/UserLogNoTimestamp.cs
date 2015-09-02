/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using System;

[System.Serializable]
public class UserLogNoTimestamp {
	
	
	public UserLogNoTimestamp(){
		
	}
	
	public UserLogNoTimestamp(string _username, ApplicationID _applicationID, Tag _tag){
		
		username = _username;
		applicationId = _applicationID.ToString();
		tag =_tag.ToString();


		timestamp = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
		//timestamp = (long)UnityEngine.Time.realtimeSinceStartup;//System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
		//		timeServer = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
		//timestamp = System.BitConverter.GetBytes(System.DateTime.Now.ToBinary());
	//	timestamp = System.DateTime.Now.ToString();//.ToBinary();
	}
	
	
	public void setDifficulty(int difficulty, int languageArea){
		
		problem_category = languageArea;
		problem_index = difficulty;
	}
	
	public void setMode(Mode _mode){
		mode = _mode.ToString ();
		
	}
	
	public void setLevel(string _level){
		level = _level;
		
	}
	
	
	public void setValue(string details){
		
		value = details;
	}
	
	
	public void setWord(string details){
		
		word = details;
	}
	
	public void setDuration(int _duration){
		
		duration = (long)_duration;
	}
	
	public void setSupervisor(string _supervisor){
		supervisor = _supervisor;
	}
	
	
	public string username;
	public string tag;
	public string value;
	public string applicationId;
	public string timestamp;
	//public string serverTime;
	public string word;
	public double duration;
	public string level;
	public string mode;
	public int problem_category;
	public int problem_index;
	
	public string supervisor;
	
	
	/*results":[{
 * "username":"joe_t",
 * "tag":"SAVEFILE",
 * "value":"AcDropChop;",
 * "applicationId":"GAME_WORLD",
 * "timestamp":"2014-06-16T09:59:21+02:00",
 * "word":null,
 * "duration":0.0,
 * "level":null,
 * "mode":null,
 * "problem_category":0,
 * "problem_index":0}
*/
	public Tag getTag(){
		
		return (Tag)System.Enum.Parse (typeof(Tag),tag);
	}
	
	public ApplicationID getApplicationID(){
		return (ApplicationID)System.Enum.Parse (typeof(ApplicationID),applicationId);
	}
	
	public string getUsername(){
		
		return username;
	}
	
	public string getValue(){
		
		return value;
	}
	
	public int getLanguageArea(){
		
		return problem_category;
	}
	
	public int getDifficulty(){
		
		return problem_index;
	}
	
	public string getLevel(){
		
		return level;
	}
	
	
	public Mode getMode(){
		
		return (Mode) System.Enum.Parse(typeof(Mode),mode);
	}
	
	public string logToString(){
		
		return username+" "+tag+" "+value+" "+applicationId+" "+" "+word+" "+duration+" "+level+" "+mode+" "+problem_category+" "+problem_index;
		
	}
}





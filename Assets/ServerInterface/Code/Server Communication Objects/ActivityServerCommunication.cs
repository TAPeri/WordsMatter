/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivityServerCommunication : GameServerCommunication {

	
	private GetRoutine difficulty_query;
	private GetRoutine words_query;
	
	int languageArea = -1;
	int difficulty = -1;
	string level = "0";//int challenge;
	Mode mode;
	
	
	int number_words;
	int evaluation_mode;

	//static int counter = 0;

	/*public void OnGUI(){

		if (Input.GetKey("up")){
			Application.CaptureScreenshot("Screenshot"+counter+".png");
			Debug.Log("Screenshot"+counter+".png");
			counter++;
		}
	}*/
	
	public void setAppID(ApplicationID _appID){
		Debug.Log("Name! "+appID);
		appID = _appID;
		
	}
	
	
	public string getLevel(){
		return level;
	}
	

	public void setActivityParameters(ApplicationID _appID, int _difficulty, int _languageArea, string _userID, int _evaluation_mode, string _challenge, Mode _mode){


		appID = _appID;

		LevelParameters param = new LevelParameters(_challenge);

		number_words = param.batchSize;
		languageArea = _languageArea;
		difficulty = _difficulty;
		level = _challenge;
		mode = _mode;
		userID = _userID;
		//challenge = _challenge;
		evaluation_mode = _evaluation_mode;
		
		loading_status = new ConnectionError("Loading of words have to be invoked",RoutineStatus.IDLE); 
		
	}
	
	public void load(){
		Debug.Log("Request words!");
		loading_status = server.requestNextWords(appID.ToString(), number_words,difficulty,languageArea, userID, evaluation_mode,level);
		
	}
	

	
	public void startRound(string value){
		Debug.Log("Start "+appID+" from "+value);
		UserLogNoTimestamp log = basicLog(Tag.APP_ROUND_SESSION_START,appID);
		log.setDifficulty(difficulty,languageArea);
		log.setMode(mode);
		log.setLevel(level);
		log.setValue(value);
		
		server.logData(log);
	}
	
	public List<GetRoutine> endRound(){
		
		UserLogNoTimestamp log = basicLog(Tag.APP_ROUND_SESSION_END,appID);
		log.setDifficulty(difficulty,languageArea);
		log.setMode(mode);
		log.setLevel(level);

		server.logData(log);
		GetRoutine flushLogsRoutine = server.flushLogs();
		GetRoutine requestProfileUpdateRoutine = server.requestProfileUpdate(difficulty,languageArea,userID);

		List<GetRoutine> retList = new List<GetRoutine>();
		retList.Add(flushLogsRoutine);
		retList.Add(requestProfileUpdateRoutine);
		return retList;
	}


	public void wordDisplayed(string word, int newLanguageArea,int newDifficulty){
		
		UserLogNoTimestamp log = basicLog(Tag.WORD_DISPLAYED,appID);
		log.setDifficulty(newDifficulty,newLanguageArea);
		log.setMode(mode);
		log.setLevel(level);
		log.setWord(word);
		
		server.logData(log);
		
	}


	public void wordSolvedCorrectly(string word,bool correct,string details, int newLanguageArea,int newDifficulty){
		
		UserLogNoTimestamp log;
		if (correct){
			
			log = basicLog(Tag.WORD_SUCCESS,appID);
			
		}else{
			
			log = basicLog(Tag.WORD_FAILED,appID);
			
		}
		
		log.setDifficulty(newDifficulty,newLanguageArea);
		log.setValue(details);
		
		log.setMode(mode);
		log.setLevel(level);
		log.setWord(word);
		server.logData(log);
		
	}

	

	public List<PackagedNextWord> loadSentences(){
		List<PackagedNextWord> aux = server.getNextWords();

		if(loading_status.status()==RoutineStatus.ERROR)
			return new List<PackagedNextWord>();
		if (WorldViewServerCommunication.tts!=null){
			List<string> words = new List<string>();
			foreach (PackagedNextWord sentence in aux){
				words.Add(sentence.getAnnotatedSentence().theSentence.Replace("{","").Replace("}",""));
				foreach (string filler in sentence.getAnnotatedSentence().fillerWords){
					words.Add (filler);
				}
				
				/*foreach(string word in sentence.getAnnotatedSentence().theSentence.Split(' ')){
					
					if(word.StartsWith("{"))
						words.Add (word.Replace("{","").Replace("}",""));
				}*/
				
			}
			
			WorldViewServerCommunication.tts.fetch(words.ToArray());

		}
		return aux;
		
		
	}
	
	
	
	public List<PackagedNextWord> loadWords(){

		if(loading_status.status()==RoutineStatus.ERROR)
			return new List<PackagedNextWord>();

		List<PackagedNextWord> aux = server.getNextWords();


		if (WorldViewServerCommunication.tts!=null){
			
			string [] words = new string[aux.Count];
			
			for (int i=0;i<words.Length;i++){
				
				words[i] = aux[i].getAnnotatedWord().getWord();
			}
			WorldViewServerCommunication.tts.fetch(words);
			
		}
		return aux;
		
		
	}
	
	public int[] getDifficulty(){
		
		return new int[2]{languageArea , difficulty};
		
	}
	
	
	
	
}

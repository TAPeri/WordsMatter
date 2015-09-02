/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class WAMLevelConfigGeneratorServer : ILevelConfigGenerator {

	WAMLevelConfig[] lvlArr;
	int counter;
	string instruction = "";
	
	public string getInstruction(){ return instruction;}
	public int getConfigCount() { if(lvlArr == null) { return -1; } else { return lvlArr.Length; } }

	public WAMLevelConfigGeneratorServer(ActivityServerCommunication para_serverCommunication)
	{


		//Debug.LogWarning( "More than one pattern per game?" );

		List<PackagedNextWord> list = null;
		LevelParameters level = new LevelParameters("");
		
		if(para_serverCommunication!=null){
			list = para_serverCommunication.loadWords();
			level = new LevelParameters(para_serverCommunication.getLevel());
			instruction = LocalisationMang.instructions(para_serverCommunication.getDifficulty()[0],para_serverCommunication.getLevel(),ApplicationID.WHAK_A_MOLE);
		}

		if (list==null){
			WorldViewServerCommunication.setError("Did not find words");
			return;
		}else if(list.Count==0){
			WorldViewServerCommunication.setError("List of words is empty");
			return;
		}else{
			
			foreach(PackagedNextWord w in list){
				if (w.getAnnotatedWord().getWordProblems().Count==0){
					WorldViewServerCommunication.setError("Word '"+w.getAnnotatedWord().getWord()+"' do not have problems");
					return;
				}else if(w.getAnnotatedWord().getWordProblems()[0].matched.Length==0){
					WorldViewServerCommunication.setError("Word '"+w.getAnnotatedWord().getWord()+"' do not have problems");
					return;
				}
			}
		}
	
		//List<WAMLevelConfig> list_levels = new List<WAMLevelConfig>();

		try{

			bool patternSound = (level.ttsType==TtsType.SPOKEN2WRITTEN);

			if(WorldViewServerCommunication.tts==null){
				patternSound = false;
			}

			List<PackagedNextWord> words = new List<PackagedNextWord>();
			List<PackagedNextWord> fillers = new List<PackagedNextWord>();

			foreach( PackagedNextWord nw in list){

				if(nw.getFiller()){ fillers.Add(nw);
				}else{ words.Add(nw); }
			}

	
			string[] deckA = new string[words.Count];
			string[] deckB = new string[fillers.Count];

			string pattern = "";
			if(words.Count==0){
				WorldViewServerCommunication.setError("Correct words not available for difficulty ("+para_serverCommunication.getDifficulty()[0]+","+para_serverCommunication.getDifficulty()[1]);
				return;		
			}

			int languageArea = words[0].getAnnotatedWord().getWordProblems()[0].category;
			int difficulty = words[0].getAnnotatedWord().getWordProblems()[0].index;
			string description = WorldViewServerCommunication.userProfile.userProblems.getDifficultiesDescription().getDifficulties()[languageArea][difficulty].getDescriptionsToString();


			if(level.mode==0){//DEPRECATED

				string phoneme = "";
			
				foreach(string desc in description.Split (',')){
					if (desc.Contains("-")){
						if (phoneme=="")
							phoneme += desc.Split('-')[1];
						else
							phoneme += " / "+desc.Split('-')[1];
					}
				}
			
				if(phoneme==""){
					patternSound = false;
					pattern = description;
				}else
					pattern = "/"+phoneme+"/";


				for (int i=0;i<deckA.Length;i++){

					deckA[i] = WorldViewServerCommunication.userProfile.userProblems.getDifficultiesDescription().getDifficulties()[languageArea][difficulty].descriptions[0].Split('-')[0];
				}

				for (int i=0;i<deckB.Length;i++){
				
					int fillerLanguageArea = fillers[i].getAnnotatedWord().getWordProblems()[0].category;
					int fillerDifficulty = fillers[i].getAnnotatedWord().getWordProblems()[0].index;
				
					deckB[i] = WorldViewServerCommunication.userProfile.userProblems.getDifficultiesDescription().getDifficulties()[fillerLanguageArea][fillerDifficulty].descriptions[0].Split('-')[0];
				
				}

			}else if (level.mode==1){//EN, vowels & consonants, monkeys with words, phoneme on the pattern


				string phoneme = "";
				
				foreach(string desc in description.Split (',')){
					if (desc.Contains("-")){
						if (phoneme=="")
							phoneme += desc.Split('-')[1];
						else
							phoneme += " / "+desc.Split('-')[1];
					}
				}

				if(!patternSound){
					GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
					pattern = gbMang.createExplanation(languageArea,difficulty);//Human readable description
				}else{
					if(phoneme==""){
						patternSound = false;
						GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
						pattern = gbMang.createExplanation(languageArea,difficulty);//Human readable description
					}else{
						pattern = "/"+phoneme+"/";
						if(!WorldViewServerCommunication.tts.test(pattern)){//Phonemes are prerecorded
							patternSound = false;
							GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
							pattern = gbMang.createExplanation(languageArea,difficulty);//Human readable description
						}
					}
				}
				for (int i=0;i<deckA.Length;i++){
					deckA[i] = words[i].getAnnotatedWord().getWord();
				}
				
				for (int i=0;i<deckB.Length;i++){
					deckB[i] = fillers[i].getAnnotatedWord().getWord();
				}
				
				
			}else if(level.mode==2){//Suffix, EN, pattern is the suffix

				//pattern = "-"+WorldViewServerCommunication.userProfile.userProblems.getDifficultiesDescription().getDifficulties()[languageArea][difficulty].descriptions[0].Split('-')[0];

				for (int i=0;i<deckA.Length;i++){
					deckA[i] = words[i].getAnnotatedWord().getWord();
				}

				for (int i=0;i<deckB.Length;i++){
					deckB[i] = fillers[i].getAnnotatedWord().getWord();
				}

				MatchedData problem = words[0].getAnnotatedWord().getWordProblems()[0].matched[0];
				pattern = "-"+words[0].getAnnotatedWord().getWord().Substring(problem.start,problem.end-problem.start);

			}else if(level.mode==3){//Prefix, EN, use 

				//pattern = WorldViewServerCommunication.userProfile.userProblems.getDifficultiesDescription().getDifficulties()[languageArea][difficulty].descriptions[0].Split('-')[0]+"-";
			
				for (int i=0;i<deckA.Length;i++){
					deckA[i] = words[i].getAnnotatedWord().getWord();
				}
			
				for (int i=0;i<deckB.Length;i++){
					deckB[i] = fillers[i].getAnnotatedWord().getWord();
				}	

				MatchedData problem = words[0].getAnnotatedWord().getWordProblems()[0].matched[0];
				pattern = words[0].getAnnotatedWord().getWord().Substring(problem.start,problem.end-problem.start)+"-";


			}else if (level.mode==4){//COnfusing letters (EN) and letter similarity (GR); letter to letter



				int size =level.batchSize/2;


				deckA = new string[size];


				if(para_serverCommunication.language==LanguageCode.EN){//get the pair of confusing letters from the difficulty description, e.g. b/d
					deckB = new string[1];

					deckA[0] = WorldViewServerCommunication.userProfile.userProblems.getDifficultiesDescription().getDifficulties()[languageArea][difficulty].descriptions[0].Split('/')[0];
					deckB[0] = WorldViewServerCommunication.userProfile.userProblems.getDifficultiesDescription().getDifficulties()[languageArea][difficulty].descriptions[0].Split('/')[1];

					for(int k=1;k<deckA.Length;k++){

						deckA[k] = deckA[0];
					}
				}else{

					int letter = Random.Range(0,description.Split(',').Length);//The description are the letters separated by commas a,A,e

					deckB = new string[description.Split(',').Length-1];

					int i = 0;
					for(int ii = 0;ii<description.Split(',').Length;ii++){
						if (ii == letter){
							deckA[0] = description.Split(',')[letter];
						}else{
							deckB[i++] = description.Split(',')[ii];
						}
					}
				}

				pattern = deckA[0];

			}else if (level.mode==5){//GR, prefixes and derivational; pattern is a word

				pattern = words[0].getAnnotatedWord().getWord();

				deckA = new string[deckA.Length-1];
				for (int i=0;i<deckA.Length;i++){
					deckA[i] = words[i+1].getAnnotatedWord().getWord();
				}

				for (int i=0;i<deckB.Length;i++){
					deckB[i] = fillers[i].getAnnotatedWord().getWord();
				}

		
			}else if (level.mode==6){//GR, letter similarity; pattern is a the first letter of correct words

				for (int i=0;i<deckA.Length;i++){
					deckA[i] = words[i].getAnnotatedWord().getWord();
				}
				
				for (int i=0;i<deckB.Length;i++){
					deckB[i] = fillers[i].getAnnotatedWord().getWord();
				}


				if(patternSound){
					//TODO: record Greek phonemes
					pattern = words[0].getAnnotatedWord().getWord().Substring(0,1);
					//pattern ="/"+ words[0].getAnnotatedWord().getGraphemesPhonemes()[0].phoneme+"/";
					if(! WorldViewServerCommunication.tts.test(pattern)){
						patternSound = false;
						pattern = words[0].getAnnotatedWord().getWord().Substring(0,1);//first letter
					}
				}else{
					pattern = words[0].getAnnotatedWord().getWord().Substring(0,1);//first letter
				}

			}else if(level.mode==7){//EN with confusing and GR with GP correspondence, pattern is the grapheme/phoneme of the correct words


				MatchedData problem = words[0].getAnnotatedWord().getWordProblems()[0].matched[0];
				pattern = words[0].getAnnotatedWord().getWord().Substring(problem.start,problem.end-problem.start);//grapheme

				for (int i=0;i<deckA.Length;i++){
					deckA[i] = words[i].getAnnotatedWord().getWord();
				}
				
				for (int i=0;i<deckB.Length;i++){
					deckB[i] = fillers[i].getAnnotatedWord().getWord();
				}
			}

			if(patternSound){//This should be handled above, additional precaution
				if(WorldViewServerCommunication.tts!=null){
					if(!WorldViewServerCommunication.tts.test(pattern))
						patternSound = false;
				}
			}

			lvlArr = new WAMLevelConfig[1]{ new WAMLevelConfig(pattern ,deckA,deckB ,patternSound,level.speed,languageArea,difficulty)};

		}catch(System.Exception e){
		
			WorldViewServerCommunication.setError(e.Message);
			return;
		}

		counter = 0;
	}

	public void reboot(){
		counter = 0;
	}
	
	public ILevelConfig getNextLevelConfig(System.Object para_extraInfo)
	{
		int reqIndex = counter % lvlArr.Length;
		WAMLevelConfig reqConfig = lvlArr[reqIndex];
		counter++;
		return reqConfig;
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class MPLevelConfigGeneratorServer : ILevelConfigGenerator {
	
	MPLevelConfig[] lvlArr;
	int counter;

	int[] minBoardDims;
	int[] maxBoardDims; 
	string[] potentialItems;

	string instruction = "";


	public string getInstruction(){ return instruction;}
	public int getConfigCount() { if(lvlArr == null) { return -1; } else { return lvlArr.Length; } }

	public void reboot(){
		counter = 0;
	}

	public MPLevelConfigGeneratorServer(ActivityServerCommunication para_serverCommunication)
	{


		List<PackagedNextWord> list = null;
		LevelParameters level = new LevelParameters("");
		
		if(para_serverCommunication!=null){
			list = para_serverCommunication.loadWords();
			level = new LevelParameters(para_serverCommunication.getLevel());
			instruction = LocalisationMang.instructions(para_serverCommunication.getDifficulty()[0],para_serverCommunication.getLevel(),ApplicationID.MOVING_PATHWAYS);

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
		


		int num_levels = level.accuracy;
		lvlArr = new MPLevelConfig[num_levels];


		List<string> correctItems = new List<string>();
		List<string> fillerItems = new List<string>();
		string pattern = "";

		bool isSound = (level.ttsType == TtsType.SPOKEN2WRITTEN);

		if((WorldViewServerCommunication.tts==null))
			isSound = false;

		try{

			int lA = list[0].getAnnotatedWord().getWordProblems()[0].category;
			int diff = list[0].getAnnotatedWord().getWordProblems()[0].index;
			string description = WorldViewServerCommunication.userProfile.userProblems.getDifficultiesDescription().getDifficulties()[lA][diff].getDescriptionsToString();


			foreach(PackagedNextWord word in list){

				if(!word.getFiller()){
					lA = word.getAnnotatedWord().getWordProblems()[0].category;
					diff = word.getAnnotatedWord().getWordProblems()[0].index;
					description = WorldViewServerCommunication.userProfile.userProblems.getDifficultiesDescription().getDifficulties()[lA][diff].getDescriptionsToString();

				}
			}

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
					isSound = false;
					pattern = description;
				}else
					pattern = "/"+phoneme+"/";



				for (int i=0;i<list.Count;i++){

					lA = list[i].getAnnotatedWord().getWordProblems()[0].category;
					diff = list[i].getAnnotatedWord().getWordProblems()[0].index;
					string grapheme =WorldViewServerCommunication.userProfile.userProblems.getDifficultiesDescription().getDifficulties()[lA][diff].descriptions[0].Split('-')[0];

					if(list[i].getFiller()){
						Debug.Log(grapheme);
						fillerItems.Add(grapheme);
					}else{ 						
						correctItems.Add(grapheme); 
					}
				}

			}else if(level.mode==1){//confusing//consonants/vowels Greek, letter to letter and confusin EN

				if(para_serverCommunication.language==LanguageCode.EN){
					string a = description.Split('/')[0];
					string b = description.Split('/')[1];

					if(Random.Range(0.0f,1.0f)>0.5){

						correctItems.Add(a);
						fillerItems.Add (b);
						pattern = a;

					}else{

						correctItems.Add(b);
						fillerItems.Add (a);
						pattern = b;

					}

					if(isSound)
						if(!WorldViewServerCommunication.tts.test(pattern)){//Letter name
							isSound = false;
						}

				}else{

					for (int i=0;i<list.Count;i++){
						MatchedData problem = list[i].getAnnotatedWord().getWordProblems()[0].matched[0];
						string grapheme = list[i].getAnnotatedWord().getWord().Substring(problem.start,problem.end-problem.start);

						if(list[i].getFiller()){
							fillerItems.Add(grapheme);
						}else{
							correctItems.Add(grapheme);
						}
					}
					pattern = correctItems[0];

					if(isSound)
						if(!WorldViewServerCommunication.tts.test(pattern)){//Letter name
							isSound = false;
						}

				}

			}else if(level.mode==2){//EN (vowels & consonants) GR (consonants & letter similarity)

				if(para_serverCommunication.language==LanguageCode.EN){

					string phoneme = "";
				
					foreach(string desc in description.Split (',')){
						if (desc.Contains("-")){
							if (phoneme=="")
								phoneme += desc.Split('-')[1];
							else
								phoneme += " / "+desc.Split('-')[1];
						}
					}
				

					if(!isSound){
						GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
						pattern = gbMang.createExplanation(lA,diff);//Human readable description

					}else{
						if(phoneme==""){
							isSound = false;
							GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
							pattern = gbMang.createExplanation(lA,diff);//Human readable description
						}else{
							pattern = "/"+phoneme+"/";
							if(!WorldViewServerCommunication.tts.test(pattern)){//Phonemes are prerecorded
								isSound = false;
								GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
								pattern = gbMang.createExplanation(lA,diff);//Human readable description
							}				
						}
					}

					for (int i=0;i<list.Count;i++){
						if(list[i].getFiller()){
							fillerItems.Add(list[i].getAnnotatedWord().getWord());
						}else{
							correctItems.Add(list[i].getAnnotatedWord().getWord());
						}
					}

				}else{

					for (int i=0;i<list.Count;i++){
						if(list[i].getFiller()){
							fillerItems.Add(list[i].getAnnotatedWord().getWord());
						}else{
							correctItems.Add(list[i].getAnnotatedWord().getWord());
							//pattern = "/"+list[i].getAnnotatedWord().getGraphemesPhonemes()[0].phoneme+"/";
						}
					}
					pattern = correctItems[0].Substring(0,1);//first letter

					if(isSound){
						if(!WorldViewServerCommunication.tts.test(pattern)){//Letter ma,e
							isSound = false;
						}
					}

				}
			}else if(level.mode==3){//GP correspondence, GR


				for (int i=0;i<list.Count;i++){
					if(list[i].getFiller()){
						fillerItems.Add(list[i].getAnnotatedWord().getWord());
					}else{
						correctItems.Add(list[i].getAnnotatedWord().getWord());
					
						MatchedData problem = list[i].getAnnotatedWord().getWordProblems()[0].matched[0];
						string grapheme = list[i].getAnnotatedWord().getWord().Substring(problem.start,problem.end-problem.start);
						pattern = grapheme;

					}
				}

				if(isSound){
					if(!WorldViewServerCommunication.tts.test(pattern)){//TODO: probably should record the phonemes
						isSound = false;
					}
				}


			}else if(level.mode==4){//Greek vowels, contain letter

				for (int i=0;i<list.Count;i++){
					if(list[i].getFiller()){
						fillerItems.Add(list[i].getAnnotatedWord().getWord());
					}else{
						correctItems.Add(list[i].getAnnotatedWord().getWord());
					//pattern = "/"+list[i].getAnnotatedWord().getGraphemesPhonemes()[0].phoneme+"/";

						MatchedData problem = list[i].getAnnotatedWord().getWordProblems()[0].matched[0];
						pattern = list[i].getAnnotatedWord().getWord().Substring(problem.start,problem.end-problem.start);
					}
				}


				if(isSound){
					if(!WorldViewServerCommunication.tts.test(pattern)){//TODO: needs phonemes prerecorded
						isSound = false;
					}
				}

		}


		int[] chosenBoardDim = new int[2];


		if(level.speed==0){
			chosenBoardDim[0] = 5;
			chosenBoardDim[1] = 5;
			
		}else if(level.speed==1){
			chosenBoardDim[0] = 8;
			chosenBoardDim[1] = 8;
		}else{
			chosenBoardDim[0] = 12;
			chosenBoardDim[1] = 12;
		}


			if(isSound){//This should be handled above, additional precaution
				if(WorldViewServerCommunication.tts!=null){
					if(!WorldViewServerCommunication.tts.test(pattern))
						isSound = false;
				}
			}

		for (int j = 0;j<num_levels;j++){
				//Debug.Log(j);
			lvlArr[j] = new MPLevelConfig(chosenBoardDim,correctItems.ToArray(),fillerItems.ToArray(),pattern,isSound,list[0].getAnnotatedWord().getWordProblems()[0].category,list[0].getAnnotatedWord().getWordProblems()[0].index);

		}

	}catch(System.Exception e){
		
		WorldViewServerCommunication.setError(e.Message);
		return;
	}

		counter = 0;
	}
	
	
	public ILevelConfig getNextLevelConfig(System.Object para_extraInfo)
	{
		int reqIndex = counter % lvlArr.Length;
		MPLevelConfig reqConfig = lvlArr[reqIndex];
		counter++;
		return reqConfig;
	}
	
}
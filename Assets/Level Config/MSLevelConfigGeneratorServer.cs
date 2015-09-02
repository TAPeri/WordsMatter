/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class MSLevelConfigGeneratorServer : ILevelConfigGenerator {

	MSLevelConfig[] lvlArr;
	int counter;
	string instruction = "";


	public string getInstruction(){ return instruction;}
	public int getConfigCount() { if(lvlArr == null) { return -1; } else { return lvlArr.Length; } }

	bool contains_that_one(string[] array, string text){

		foreach(string a in array)
			if (a!=null)
				if(a.Contains(text))
				return true;

		return false;

	}

	public void reboot(){
		counter = 0;
	}

	public MSLevelConfigGeneratorServer(ActivityServerCommunication para_serverCommunication)
	{
		List<PackagedNextWord> list = null;
		LevelParameters level = new LevelParameters("");
		
		if(para_serverCommunication!=null){
			list = para_serverCommunication.loadWords();
			level = new LevelParameters(para_serverCommunication.getLevel());
			instruction = LocalisationMang.instructions(para_serverCommunication.getDifficulty()[0],para_serverCommunication.getLevel(),ApplicationID.MAIL_SORTER);

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


		if(WorldViewServerCommunication.tts==null){
			level.ttsType = TtsType.WRITTEN2WRITTEN;

		}

		List<MSLevelConfig> lvlList = new List<MSLevelConfig>(); 
		int idx = 0;
		try{

			while(idx<list.Count){

			string[] deckA = new string[level.accuracy+1];
			string[] deckB = new string[level.accuracy+1];

			int[] languageAreas = new int[level.accuracy+1];
			int[] difficulties = new int[level.accuracy+1];


			for (int j =0;j<deckA.Length;j++){

				int languageArea = list[idx+j].getAnnotatedWord().getWordProblems()[0].category;
				int  difficulty = list[idx+j].getAnnotatedWord().getWordProblems()[0].index;

				string description = WorldViewServerCommunication.userProfile.userProblems.getDifficultiesDescription().getDifficulties()[languageArea][difficulty].getDescriptionsToString();

				languageAreas[j] = languageArea;
				difficulties[j] = difficulty;

				if(level.mode==0){//EN consonants & vowels
					deckB[j] = list[idx+j].getAnnotatedWord().getWord();

					string phoneme = "";
					
					foreach(string desc in description.Split (',')){
						if (desc.Contains("-")){
							if (phoneme=="")
								phoneme += desc.Split('-')[1];
							else
								phoneme += " / "+desc.Split('-')[1];
						}
					}

						if(level.ttsType==TtsType.WRITTEN2WRITTEN){
							GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
							deckA[j] = gbMang.createExplanation(languageAreas[j],difficulties[j]);//Human readable description


						}else{
					
					if(phoneme==""){
							level.ttsType = TtsType.WRITTEN2WRITTEN;

							GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
							deckA[j] = gbMang.createExplanation(languageAreas[j],difficulties[j]);//Human readable description

					}else{
							if(!WorldViewServerCommunication.tts.test("/"+phoneme+"/")){//Phonemes are prerecorded
								level.ttsType = TtsType.WRITTEN2WRITTEN;
								GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
								deckA[j] = gbMang.createExplanation(languageAreas[j],difficulties[j]);//Human readable description
							}else{

								deckA[j] = "/"+phoneme+"/";
							}

					}

						}

				}else if(level.mode==1){//Suffix
					deckB[j] = list[idx+j].getAnnotatedWord().getWord();

					//deckB[j] = "-"+list[idx+j].getAnnotatedWord().getSuffix();//getSyllables()[list[idx+j].getAnnotatedWord().getSyllables().Length-1];
					if(para_serverCommunication.language==LanguageCode.EN)
						deckA[j] = "-"+WorldViewServerCommunication.userProfile.userProblems.getDifficultiesDescription().getDifficulties()[languageArea][difficulty].descriptions[0].Split('-')[0];
					else{

							int start = list[idx+j].getAnnotatedWord().getWordProblems()[0].matched[0].start;
							int end = list[idx+j].getAnnotatedWord().getWordProblems()[0].matched[0].end;
		
							//Debug.Log(list[idx+j].getAnnotatedWord().getWord());
							deckA[j] = "-"+list[idx+j].getAnnotatedWord().getWord().Substring(start,end-start);
					}

				}else if(level.mode==2){//Prefix
					deckB[j] = list[idx+j].getAnnotatedWord().getWord();
					deckA[j] = WorldViewServerCommunication.userProfile.userProblems.getDifficultiesDescription().getDifficulties()[languageArea][difficulty].descriptions[0].Split('-')[0]+"-";
				}else if(level.mode==3){//GR GP or EN blends

					deckB[j] = list[idx+j].getAnnotatedWord().getWord();

					//Grapheme
					deckA[j] = list[idx+j].getAnnotatedWord().getWord().Substring(list[idx+j].getAnnotatedWord().getWordProblems()[0].matched[0].start,list[idx+j].getAnnotatedWord().getWordProblems()[0].matched[0].end-list[idx+j].getAnnotatedWord().getWordProblems()[0].matched[0].start);

					//TODO: blends should use recorded sounds

					if(level.ttsType==TtsType.WRITTEN2SPOKEN){//GR
						if(WorldViewServerCommunication.tts!=null){
								bool correct = true;
								foreach(string s in deckA){
									if(!WorldViewServerCommunication.tts.test(s)){
										correct = false;
										break;
									}
								}
								if(!correct){
									level.ttsType=TtsType.WRITTEN2WRITTEN;
								}
						}else{
								level.ttsType=TtsType.WRITTEN2WRITTEN;

						}
					}
				}
			}



			idx += deckA.Length;

			for(int i =0;i<deckA.Length;i++){
				int j = Random.Range(0,deckA.Length);

				string a = deckA[i];
				deckA[i] =deckA[j];
				deckA[j] = a;

				a = deckB[i];
				deckB[i] =deckB[j];
				deckB[j] = a;

				int la = languageAreas[i];
				languageAreas[i] = languageAreas[j];

				languageAreas[j] = la;

				int d = difficulties[i];
				difficulties[i] = difficulties[j];
				difficulties[j] = d;


			}

			lvlList.Add(new MSLevelConfig(deckB,deckA,level.ttsType,level.speed,languageAreas,difficulties));

		}
		}catch(System.Exception e){

			WorldViewServerCommunication.setError(e.Message);
			return;
		}
		lvlArr = lvlList.ToArray();
		counter = 0;


	}


	public ILevelConfig getNextLevelConfig(System.Object para_extraInfo)
	{
		int reqIndex = counter % lvlArr.Length;
		MSLevelConfig reqConfig = lvlArr[reqIndex];
		counter++;
		return reqConfig;
	}

}

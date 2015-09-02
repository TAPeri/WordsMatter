/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class BBLevelConfigGeneratorServer : ILevelConfigGenerator
{


	public void reboot(){
		counter = 0;
	}

	BBLevelConfig[] hardCodedArr;
	int counter;
	string instruction = "";


	public string getInstruction(){ return instruction;}
	public int getConfigCount() { if(hardCodedArr == null) { return -1; } else { return hardCodedArr.Length; } }

	// Note: Assumes that server communication object was previously polled by the activity script to check when it has RoutineStatus.READY.
	//       the script has confirmed that the generator can go ahead.
	public BBLevelConfigGeneratorServer(ActivityServerCommunication para_serverCommunication)
	{

		List<PackagedNextWord> list = null;
		LevelParameters level = new LevelParameters("");

		if(para_serverCommunication!=null){
			list = para_serverCommunication.loadWords();
			level = new LevelParameters(para_serverCommunication.getLevel());

			instruction = LocalisationMang.instructions(para_serverCommunication.getDifficulty()[0],para_serverCommunication.getLevel(),ApplicationID.EYE_EXAM);


		}

		if (list==null){
			WorldViewServerCommunication.setError("Did not find words");
			return;
		}else if(list.Count==0){
			WorldViewServerCommunication.setError("List of words is empty");
			return;
		}else{
			if ((level.mode!=0))//Words must have a difficulty unless we are only using syllables
			
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

		List<BBLevelConfig> reqList = new List<BBLevelConfig>();

		try{

		for(int i=0; i<list.Count; i++)
		{
			AnnotatedWord tmpAWord = list[i].annotatedWord;
			string bridgeWord = tmpAWord.getWord();


			if(level.mode==0){//(deprecated for EN) GR syllable division

				List<int> wordSylls = tmpAWord.getSyllSplitPositions();

				
				int syllableIndex = UnityEngine.Random.Range(0,wordSylls.Count);

				if(syllableIndex==0){

					HighlightDesc hd= new HighlightDesc(0,wordSylls[0]);

					if(WorldViewServerCommunication.userProfile.language=="EN")
							reqList.Add(new BBLevelConfig(bridgeWord,new List<HighlightDesc>() {hd},"First syllable",TtsType.WRITTEN2WRITTEN, tmpAWord.getWordProblems()[0].category,tmpAWord.getWordProblems()[0].index));
					else
							reqList.Add(new BBLevelConfig(bridgeWord,new List<HighlightDesc>() {hd},"πρώτη συλλαβή",TtsType.WRITTEN2WRITTEN, tmpAWord.getWordProblems()[0].category,tmpAWord.getWordProblems()[0].index));
					
				}else if(syllableIndex==wordSylls.Count-1){

					HighlightDesc hd= new HighlightDesc(wordSylls[wordSylls.Count-1]+1,bridgeWord.Length-1);
					if(WorldViewServerCommunication.userProfile.language=="EN")
						
							reqList.Add(new BBLevelConfig(bridgeWord,new List<HighlightDesc>() {hd},"Last syllable",TtsType.WRITTEN2WRITTEN, tmpAWord.getWordProblems()[0].category,tmpAWord.getWordProblems()[0].index));
					
					else
							reqList.Add(new BBLevelConfig(bridgeWord,new List<HighlightDesc>() {hd},"Τελευταία συλλαβή",TtsType.WRITTEN2WRITTEN, tmpAWord.getWordProblems()[0].category,tmpAWord.getWordProblems()[0].index));
					
				}else{

					HighlightDesc hd= new HighlightDesc(wordSylls[syllableIndex-1]+1,wordSylls[syllableIndex]-1);
					string position = "";
					
					if(WorldViewServerCommunication.userProfile.language=="EN"){
						
						switch(syllableIndex){
						
						case 1: position = "Second";break;
						case 2: position = "Third";break;
						case 3: position = "Fourth";break;
						case 4: position = "Fifth";break;
						case 5: position = "Sixth";break;
						}
							reqList.Add(new BBLevelConfig(bridgeWord,new List<HighlightDesc>() {hd},position+" syllable",TtsType.WRITTEN2WRITTEN, tmpAWord.getWordProblems()[0].category,tmpAWord.getWordProblems()[0].index));
					}else{
						
						switch(syllableIndex){
						case 1: position = "δεύτερη";break;
						case 2: position = "τρίτη";break;
						case 3: position = "τέταρτη";break;
							
						}
							reqList.Add(new BBLevelConfig(bridgeWord,new List<HighlightDesc>() {hd},position+" συλλαβή",TtsType.WRITTEN2WRITTEN, tmpAWord.getWordProblems()[0].category,tmpAWord.getWordProblems()[0].index));
						
						
					}
				}
				
			}else if(level.mode==6){//GR inflexional and prefixes and letter similarity

				MatchedData[] matches = tmpAWord.getWordProblems()[0].matched;

				List<HighlightDesc> listMatches = new List<HighlightDesc>();
				string pattern = tmpAWord.getWord().Substring(matches[0].start,matches[0].end-matches[0].start);
				foreach (MatchedData match in matches)
					if(tmpAWord.getWord().Substring(match.start,match.end-match.start)==pattern)
						listMatches.Add( new HighlightDesc(match.start,match.end-1) );
				
				reqList.Add(new BBLevelConfig(bridgeWord,listMatches,pattern,level.ttsType, tmpAWord.getWordProblems()[0].category,tmpAWord.getWordProblems()[0].index));


			}else{//EN 

				int languageArea = tmpAWord.getWordProblems()[0].category;
				int difficulty = tmpAWord.getWordProblems()[0].index;
				MatchedData[] matches = tmpAWord.getWordProblems()[0].matched;


				string description = WorldViewServerCommunication.userProfile.userProblems.getDifficultiesDescription().getDifficulties()[languageArea][difficulty].getDescriptionsToString();


				if(level.mode==4){//vowel or consonants, TTS 
					
					string phoneme = "";
					
					foreach(string pattern in description.Split (',')){
						if (pattern.Contains("-")){
							if (phoneme=="")
								phoneme += pattern.Split('-')[1];
							else
								phoneme += " / "+pattern.Split('-')[1];
						}
					}
					
					List<HighlightDesc> highlights = new List<HighlightDesc>();
					
					foreach(MatchedData match in matches){
						highlights.Add(new HighlightDesc(match.start,match.end-1));
					}
					
					string pattern2 = "";
					if(level.ttsType==TtsType.WRITTEN2WRITTEN){
							GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
							pattern2 = gbMang.createExplanation(languageArea,difficulty);//Human readable description
					}else{
						if(phoneme==""){
								level.ttsType=TtsType.WRITTEN2WRITTEN;
								GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
								pattern2 = gbMang.createExplanation(languageArea,difficulty);//Human readable description
						}else{
								pattern2 = "/"+phoneme+"/";
								if(!WorldViewServerCommunication.tts.test(pattern2)){//Phonemes are prerecorded
									level.ttsType=TtsType.WRITTEN2WRITTEN;
									GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();
									pattern2 = gbMang.createExplanation(languageArea,difficulty);//Human readable description
								}
						}
					}

					reqList.Add(new BBLevelConfig(bridgeWord,highlights ,pattern2,level.ttsType, tmpAWord.getWordProblems()[0].category,tmpAWord.getWordProblems()[0].index));
					
					
				}else if(level.mode==1){//EN suffix

					List<HighlightDesc> listMatches = new List<HighlightDesc>();

					foreach (MatchedData match in matches)
						listMatches.Add( new HighlightDesc(match.start,match.end-1) );

						reqList.Add(new BBLevelConfig(bridgeWord,listMatches,"-"+description,level.ttsType, tmpAWord.getWordProblems()[0].category,tmpAWord.getWordProblems()[0].index));

				}else if(level.mode==2){//EN prefix

					List<HighlightDesc> listMatches = new List<HighlightDesc>();
					
					foreach (MatchedData match in matches)
						listMatches.Add( new HighlightDesc(match.start,match.end-1) );
					
					reqList.Add(new BBLevelConfig(bridgeWord,listMatches,description+"-",level.ttsType, tmpAWord.getWordProblems()[0].category,tmpAWord.getWordProblems()[0].index));


				}else{//5, EN confusing letters


					List<HighlightDesc> listMatches = new List<HighlightDesc>();

					description = bridgeWord.Substring(matches[0].start,matches[0].end-matches[0].start);

					foreach (MatchedData match in matches){
						if(bridgeWord.Substring(match.start,match.end-match.start).Equals(description))
						       listMatches.Add( new HighlightDesc(match.start,match.end-1) );
					}
					reqList.Add(new BBLevelConfig(bridgeWord,listMatches,description,level.ttsType, tmpAWord.getWordProblems()[0].category,tmpAWord.getWordProblems()[0].index));

				}
			}
		}
		
		hardCodedArr = reqList.ToArray();


		for(int j=1;j<hardCodedArr.Length;j++){
			int r_j = Random.Range(1,hardCodedArr.Length);
			BBLevelConfig aux = hardCodedArr[r_j];
			hardCodedArr[r_j] = hardCodedArr[j];
			hardCodedArr[j] = aux;
		}

	}catch(System.Exception e){
		
		WorldViewServerCommunication.setError(e.Message);
		return;
	}
		
		
		counter = 0;
	}


	public ILevelConfig getNextLevelConfig(System.Object para_extraInfo)
	{
		int reqIndex = counter % hardCodedArr.Length;
		BBLevelConfig reqConfig = hardCodedArr[reqIndex];
		counter++;
		return reqConfig;
	}

}

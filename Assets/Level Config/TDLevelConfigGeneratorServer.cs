/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class TDLevelConfigGeneratorServer : ILevelConfigGenerator
{

	TDLevelConfig[] hardCodedArr;
	int counter;

	string instruction = "";

	public void reboot(){
		counter = 0;
	}
	public string getInstruction(){ return instruction;}
	public int getConfigCount() { if(hardCodedArr == null) { return -1; } else { return hardCodedArr.Length; } }

	// Note: Assumes that server communication object was previously polled by the activity script to check when it has RoutineStatus.READY.
	//       the script has confirmed that the generator can go ahead.
	public TDLevelConfigGeneratorServer(ActivityServerCommunication para_serverCommunication)
	{

		List<PackagedNextWord> list = null;
		LevelParameters level = new LevelParameters("");
		
		if(para_serverCommunication!=null){
			list = para_serverCommunication.loadWords();
			level = new LevelParameters(para_serverCommunication.getLevel());
			instruction = LocalisationMang.instructions(para_serverCommunication.getDifficulty()[0],para_serverCommunication.getLevel(),ApplicationID.TRAIN_DISPATCHER);

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
		

		
		List<TDLevelConfig> reqList = new List<TDLevelConfig>();

		try{


		for(int i=0; i<list.Count; i++)
		{
			AnnotatedWord tmpAWord = list[i].annotatedWord;
			string[] syllables;

			if(level.mode==0){//Syllables
				
				syllables = tmpAWord.getSyllables();
		
				
			}else if(level.mode==1){//Suffix
				syllables = new string[2];


				syllables[0] = tmpAWord.getWord().Substring(0,tmpAWord.getWordProblems()[0].matched[0].start);
				syllables[1] = tmpAWord.getWord().Substring(tmpAWord.getWordProblems()[0].matched[0].start,tmpAWord.getWordProblems()[0].matched[0].end-tmpAWord.getWordProblems()[0].matched[0].start);

			}else if(level.mode==2){//Prefix
				syllables = new string[2];

				syllables[0] = tmpAWord.getWord().Substring(0,tmpAWord.getWordProblems()[0].matched[0].end);
				syllables[1] = tmpAWord.getWord().Substring(tmpAWord.getWordProblems()[0].matched[0].end,tmpAWord.getWord().Length-tmpAWord.getWordProblems()[0].matched[0].end);

			}else{//Graphemes
				
				syllables = new string[tmpAWord.getGraphemesPhonemes().Count];
				
				for(int j=0;j<syllables.Length;j++){
					
					syllables[j] = tmpAWord.getGraphemesPhonemes()[j].grapheme;
					
				}
				

			}
			
			reqList.Add(new TDLevelConfig(tmpAWord.getWord(),syllables, tmpAWord.getWordProblems()[0].category,tmpAWord.getWordProblems()[0].index ,level.speed));
			
		}


		
		hardCodedArr = reqList.ToArray();

		for(int j=1;j<hardCodedArr.Length;j++){
			int r_j = Random.Range(1,hardCodedArr.Length);
			TDLevelConfig aux = hardCodedArr[r_j];
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
		TDLevelConfig reqConfig = hardCodedArr[reqIndex];
		counter++;
		return reqConfig;
	}
}

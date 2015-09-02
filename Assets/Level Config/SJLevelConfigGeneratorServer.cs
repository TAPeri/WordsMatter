/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class SJLevelConfigGeneratorServer : ILevelConfigGenerator
{
	SJLevelConfig[] lvlArr;
	int counter = 0;
	string instruction = "";
	bool error = false;

	public void reboot(){
		counter = 0;
	}

	public string getInstruction(){ return instruction;}
	public int getConfigCount() { if(lvlArr == null) { return -1; } else { return lvlArr.Length; } }

	// Note: Assumes that server communication object was previously polled by the activity script to check when it has RoutineStatus.READY.
	//       the script has confirmed that the generator can go ahead.
	public SJLevelConfigGeneratorServer(ActivityServerCommunication para_serverCommunication)
	{

		List<PackagedNextWord> list = null;
		LevelParameters level = new LevelParameters("");

		if(para_serverCommunication!=null){
			list = para_serverCommunication.loadWords();
			level = new LevelParameters(para_serverCommunication.getLevel());
			instruction = LocalisationMang.instructions(para_serverCommunication.getDifficulty()[0],para_serverCommunication.getLevel(),ApplicationID.DROP_CHOPS);


				/*	TextAsset ta = (TextAsset) Resources.Load("Localisation_Files/"+para_serverCommunication.language.ToString()+"/Instructions_"+para_serverCommunication.language.ToString(),typeof(TextAsset));
			if(ta != null){

			string text = ta.text;
			
			foreach(string line in text.Split('\n')){
				
				string[] values = line.Split(',');
				
				if(System.Convert.ToInt32(values[0])==(int)ApplicationID.DROP_CHOPS){
					
					if(System.Convert.ToInt32(values[1])==level.mode){
						Debug.Log(line);
						
						if((TtsType)System.Enum.Parse(typeof(TtsType), values[2])==level.ttsType){
							
							instruction = values[3];
						}
					}
				}
			}
			}
			if (instruction==""){
				instruction = "Instructions not available";
			}*/
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
		
		if (error){
			list = WordDBCreator.createListPackagedNextWords("DROP_CHOPS");
			level = new LevelParameters(WordDBCreator.getLevel("DROP_CHOPS"));
		}


		bool useTTS = level.ttsType!=TtsType.WRITTEN2WRITTEN;


		List<SJWordItem> reqList = new List<SJWordItem>();
		//List<string> not_repeated = new List<string>();

		try{


		for(int i=0; i<list.Count; i++)
		{
			AnnotatedWord tmpAWord = list[i].annotatedWord;
			int[] syllables;
			bool[] openSyllables;

			if(level.mode==0){//Syllables

				syllables = tmpAWord.getSyllSplitPositions().ToArray();
				/*for (int j=0;j<syllables.Length;j++){
					syllables[j]--;
				}*/
				openSyllables = new bool[syllables.Length];
				for(int j = 0;j<openSyllables.Length;j++){
					string cvSyllable = tmpAWord.cvform.Split('-')[j+1];
					if (cvSyllable[cvSyllable.Length-1]=='v')
						openSyllables[j]=true;
					else
						openSyllables[j]=false;
				}

			}else if(level.mode==1){//Suffix
				syllables = new int[1];
				openSyllables = new bool[2];//all false

			
				syllables[0] = tmpAWord.getWordProblems()[0].matched[0].start-1;

			}else if(level.mode==2){//Prefix
				syllables = new int[1];
				openSyllables = new bool[2];//all false


				syllables[0] = tmpAWord.getWordProblems()[0].matched[0].end-1;

			}else{//Graphemes

				syllables = new int[tmpAWord.getGraphemesPhonemes().Count-1];

				openSyllables = new bool[syllables.Length+1];//all false


				syllables[0] = tmpAWord.getGraphemesPhonemes()[0].grapheme.Length-1;

				for(int j=1;j<syllables.Length;j++){

					syllables[j] = tmpAWord.getGraphemesPhonemes()[j].grapheme.Length+syllables[j-1];

				}


			}

			reqList.Add(new SJWordItem(i,tmpAWord.getWord(),syllables,openSyllables,tmpAWord.getWordProblems()[0].category,tmpAWord.getWordProblems()[0].index));

		}
			                      


		lvlArr = new SJLevelConfig[reqList.Count];
		for(int i=0; i<reqList.Count; i++)
		{
			lvlArr[i] = new SJLevelConfig(reqList[i],useTTS,level.speed);
		}

		for(int j=1;j<lvlArr.Length;j++){
			int r_j = Random.Range(1,lvlArr.Length);
			SJLevelConfig aux = lvlArr[r_j];
			lvlArr[r_j] = lvlArr[j];
			lvlArr[j] = aux;
		}
	}catch(System.Exception e){
		
		WorldViewServerCommunication.setError(e.Message);
		return;
	}

	}
	
	public ILevelConfig getNextLevelConfig(System.Object para_extraInfo)
	{
		int reqIndex = counter % lvlArr.Length;
		SJLevelConfig reqConfig = lvlArr[reqIndex];
		counter++;
		return reqConfig;
	}
}

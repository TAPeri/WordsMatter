/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class PDLevelConfigGeneratorServer : ILevelConfigGenerator {

	PDLevelConfig[] hardCodedArr;
	int counter;
	
	string instruction = "";
	//bool error;
	
	public string getInstruction(){ return instruction;}
	public int getConfigCount() { if(hardCodedArr == null) { return -1; } else { return hardCodedArr.Length; } }

	public PDLevelConfigGeneratorServer(ActivityServerCommunication para_serverCommunication)
	{


		List<PackagedNextWord> list = null;
		LevelParameters level = new LevelParameters("");
		
		if(para_serverCommunication!=null){
			list = para_serverCommunication.loadWords();
			level = new LevelParameters(para_serverCommunication.getLevel());
			instruction = LocalisationMang.instructions(para_serverCommunication.getDifficulty()[0],para_serverCommunication.getLevel(),ApplicationID.ENDLESS_RUNNER);

			/*TextAsset ta = (TextAsset) Resources.Load("Localisation_Files/"+para_serverCommunication.language.ToString()+"/Instructions_"+para_serverCommunication.language.ToString(),typeof(TextAsset));
			if(ta != null){

			string text = ta.text;
			
			foreach(string line in text.Split('\n')){
				
				string[] values = line.Split(',');
				
				if(System.Convert.ToInt32(values[0])==(int)ApplicationID.ENDLESS_RUNNER){
					
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
				instruction = "";
			}*/
		}

		if (list==null){
			WorldViewServerCommunication.setError("Did not find words");
			return;
		}else if(list.Count==0){
			WorldViewServerCommunication.setError("List of words is empty");
			return;
		}


		//if (error){
		//	list = WordDBCreator.createListPackagedNextWords("ENDLESS_RUNNER");
		//	level = new LevelParameters(WordDBCreator.getLevel("ENDLESS_RUNNER"));
		//}


		List<PDLevelConfig> hardCodedList = new List<PDLevelConfig>();

		try{


			for (int i=0;i<list.Count;i+=level.accuracy){

				List<string> words = new List<string>();
				List<int> knocks = new List<int>();
				List<int> langArea = new List<int>();
				List<int> diff = new List<int>();


				for(int j=i;j<list.Count;j++){

					words.Add(list[j].annotatedWord.getWord());
					langArea.Add(list[j].annotatedWord.getWordProblems()[0].category);
					diff.Add(list[j].annotatedWord.getWordProblems()[0].index);

					if(level.mode==0){
						knocks.Add(list[j].annotatedWord.getSyllables().Length);
					}else{
						knocks.Add(list[j].annotatedWord.getGraphemesPhonemes().Count);
					}

					if (words.Count==level.accuracy)
						break;
				}

				hardCodedList.Add( new PDLevelConfig(words.ToArray(),knocks.ToArray(),langArea.ToArray(),diff.ToArray(),words.Count*(4-level.speed)));

			}

			hardCodedArr = hardCodedList.ToArray();
		for(int j=1;j<hardCodedArr.Length;j++){
			int r_j = Random.Range(1,hardCodedArr.Length);
			PDLevelConfig aux = hardCodedArr[r_j];
			hardCodedArr[r_j] = hardCodedArr[j];
			hardCodedArr[j] = aux;
		}

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
		int reqIndex = counter % hardCodedArr.Length;
		PDLevelConfig reqConfig = hardCodedArr[reqIndex];
		counter++;
		return reqConfig;
	}
}

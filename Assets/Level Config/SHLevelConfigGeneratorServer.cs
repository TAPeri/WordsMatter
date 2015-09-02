/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class SHLevelConfigGeneratorServer : ILevelConfigGenerator
{
	SHeroLevelConfig[] lvlArr;
	int counter = 0;
	string instruction = "";
	bool error = false;

	public string getInstruction(){ return instruction;}
	public int getConfigCount() { if(lvlArr == null) { return -1; } else { return lvlArr.Length; } }

	// Note: Assumes that server communication object was previously polled by the activity script to check when it has RoutineStatus.READY.
	//       the script has confirmed that the generator can go ahead.
	public SHLevelConfigGeneratorServer(ActivityServerCommunication para_serverCommunication)
	{
		List<PackagedNextWord> list = null;
		LevelParameters level = new LevelParameters("");
		
		if(para_serverCommunication!=null){
			list = para_serverCommunication.loadSentences();
//			list = para_serverCommunication.loadWords();
			level = new LevelParameters(para_serverCommunication.getLevel());

			instruction = LocalisationMang.instructions(para_serverCommunication.getDifficulty()[0],para_serverCommunication.getLevel(),ApplicationID.SERENADE_HERO);


			/*TextAsset ta = (TextAsset) Resources.Load("Localisation_Files/"+para_serverCommunication.language.ToString()+"/Instructions_"+para_serverCommunication.language.ToString(),typeof(TextAsset));
			if(ta != null){

			string text = ta.text;
			
			foreach(string line in text.Split('\n')){
				
				string[] values = line.Split(',');
				
				if(System.Convert.ToInt32(values[0])==(int)ApplicationID.SERENADE_HERO){
					
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

		int speed = level.speed;


		if (list==null){
			WorldViewServerCommunication.setError("Did not find words");
			return;
		}else if(list.Count==0){
			WorldViewServerCommunication.setError("List of words is empty");
			return;
		}
		
		if (error){
			list = WordDBCreator.createListPackagedNextWords("SERENADE_HERO");
			level = new LevelParameters(WordDBCreator.getLevel("SERENADE_HERO"));
		}


		lvlArr = new SHeroLevelConfig[list.Count];

		try{

		for(int i=0; i<list.Count; i++)
		{

			AnnotatedSentence sentence = list[i].getAnnotatedSentence();

			bool isWord = false;
			string[] text;
			List<int> indexes = new List<int>();

			isWord = true;
			text = sentence.theSentence.Split(new char[] { '{', '}' }, System.StringSplitOptions.RemoveEmptyEntries);

			List<int> positions = new List<int>();
			int j = -1;

				Debug.Log(sentence.theSentence);
			while ((j = sentence.theSentence.IndexOf('{', j+1)) != -1)
				{
					positions.Add(j);
					//Debug.Log(j);
				}

			int total_length = 0;


			for( j = 0;j<text.Length;j++){
					if (positions.Contains(total_length)){
						indexes.Add(j);
						total_length +=2; //Opening and closing brakets
					}

					total_length += text[j].Length;
			}


			
				Debug.Log(sentence.theSentence+" "+indexes.Count+" "+sentence.fillerWords.Count);
				for(int jj =0;jj<sentence.fillerWords.Count;jj++){
					
					Debug.Log(sentence.fillerWords[jj]);
					
				}



				if(sentence.fillerWords.Count>4){
					List<string> needHaveFillers = new List<string>();//The last fillers are the correct syllables

					for(int jj=0;jj<indexes.Count;jj++){
						needHaveFillers.Add(sentence.fillerWords[sentence.fillerWords.Count-1]);
						sentence.fillerWords.Remove(needHaveFillers[jj]);
					}

					while((sentence.fillerWords.Count+indexes.Count)>4){
						sentence.fillerWords.RemoveAt(Random.Range(0,sentence.fillerWords.Count));
						Debug.Log("Remove! "+sentence.fillerWords.Count);
					}

					foreach(string needFiller in needHaveFillers)
						sentence.fillerWords.Add(needFiller);

				}


			for(int jj =0;jj<sentence.fillerWords.Count;jj++){

				int k = Random.Range(0,sentence.fillerWords.Count);
				sentence.fillerWords.Add(sentence.fillerWords[k]);
				sentence.fillerWords.Remove(sentence.fillerWords[k]);

			}







			Debug.Log(text+" "+list[i].getAnnotatedSentence().languageArea+" "+list[i].getAnnotatedSentence().difficulty);

			lvlArr[i] = new SHeroLevelConfig(text,indexes.ToArray(),sentence.fillerWords.ToArray(),isWord,speed, list[i].getAnnotatedSentence().languageArea,list[i].getAnnotatedSentence().difficulty  );
		}

		for(int j=1;j<lvlArr.Length;j++){
			int r_j = Random.Range(1,lvlArr.Length);
			SHeroLevelConfig aux = lvlArr[r_j];
			lvlArr[r_j] = lvlArr[j];
			lvlArr[j] = aux;
		}
	}catch(System.Exception e){
		
		WorldViewServerCommunication.setError(e.Message);
		return;
	}



	}

	public void reboot(){
		counter = 0;
	}

	public ILevelConfig getNextLevelConfig(System.Object para_extraInfo)
	{
		int reqIndex = counter % lvlArr.Length;
		SHeroLevelConfig reqConfig = lvlArr[reqIndex];
		counter++;
		return reqConfig;	
	}
}

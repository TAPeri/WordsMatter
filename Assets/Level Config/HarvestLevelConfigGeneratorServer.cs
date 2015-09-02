/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class HarvestLevelConfigGeneratorServer : ILevelConfigGenerator {

	public void reboot(){
		counter = 0;
	}

	HarvestLevelConfig[] lvlArr;
	int counter;
	
	string instruction = "";

	//bool error;
	
	public string getInstruction(){ return instruction;}
	public int getConfigCount() { if(lvlArr == null) { return -1; } else { return lvlArr.Length; } }

	public HarvestLevelConfigGeneratorServer(ActivityServerCommunication para_serverCommunication)
	{


		List<PackagedNextWord> list = null;
		LevelParameters level = new LevelParameters("");
		 
		if(para_serverCommunication!=null){
			list = para_serverCommunication.loadWords();
			level = new LevelParameters(para_serverCommunication.getLevel());
			instruction = LocalisationMang.instructions(para_serverCommunication.getDifficulty()[0],para_serverCommunication.getLevel(),ApplicationID.HARVEST);


			/*TextAsset ta = (TextAsset) Resources.Load("Localisation_Files/"+para_serverCommunication.language.ToString()+"/Instructions_"+para_serverCommunication.language.ToString(),typeof(TextAsset));
			if(ta != null){

			string text = ta.text;
			
			foreach(string line in text.Split('\n')){
				
				string[] values = line.Split(',');
				
				if(System.Convert.ToInt32(values[0])==(int)ApplicationID.HARVEST){
					
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

		/*if (error){
			list = WordDBCreator.createListPackagedNextWords("HARVEST");
			level = new LevelParameters(WordDBCreator.getLevel("HARVEST"));
		}*/


		lvlArr = new HarvestLevelConfig[list.Count];

		int idx = 0;


		List<string> patterns = new List<string>();

		try{

		Difficulty[][] profileDifficulties = WorldViewServerCommunication.userProfile.userProblems.getDifficultiesDescription().getDifficulties();


		if (level.mode==0){
			
			patterns.Add("Open syllable");
			//patterns.Add("Muuuuuu");
			patterns.Add("Closed syllable");
			//patterns.Add("Muuuuuu");


			foreach(PackagedNextWord nw in list){
				
				List<int> matches = new List<int>();
				
				string[] cvForm = nw.getAnnotatedWord().getCVForm().Split(new char[] {'-'}, System.StringSplitOptions.RemoveEmptyEntries);		
				if (cvForm[0][cvForm[0].Length-1]=='v'){
					matches.Add(0);
				}else{
					matches.Add(1);
					
				}

				/*if(cvForm.Length>1){
					
					if (cvForm[1][cvForm[1].Length-1]=='v')
						matches.Add(2);
					else
						matches.Add(3);
				}*/
				
				lvlArr[idx++] = new HarvestLevelConfig(nw.getAnnotatedWord().getWord(),patterns.ToArray(),matches.ToArray(), nw.getAnnotatedWord().getWordProblems()[0].category,nw.getAnnotatedWord().getWordProblems()[0].index);
			}

		}else if(level.mode==1){
			
			
			//int lA = 0;
			foreach(PackagedNextWord nw in list){
				//foreach(WordProblemInfo wp in nw.getAnnotatedWord().getWordProblems()){
				WordProblemInfo wp = nw.getAnnotatedWord().getWordProblems()[0];
				Debug.Log(nw.getAnnotatedWord().getWord()+" "+profileDifficulties[ wp.category ][ wp.index  ].character);

				string problem = profileDifficulties[ wp.category ][ wp.index  ].character;//getDescriptionsToString();
				if(!patterns.Contains(problem)){
					patterns.Add(problem);
					//lA = wp.category;
				}
				//}
			}
			
			
			/*List<string> candidates = new List<string>();
			
			for(int i = 0; i < profileDifficulties[lA].Length;i++){
				
				if (!candidates.Contains(profileDifficulties[lA][i].character)){
					candidates.Add(profileDifficulties[lA][i].character);
				}
			}
			
			while((patterns.Count<4)&(candidates.Count>0)){
				
				
				int index = Random.Range(0,candidates.Count);
				string candidateCharacter = candidates[index];
				
				patterns.Add(candidateCharacter);
				
				candidates.Remove(candidateCharacter);
				
			}*/
			
			List<HarvestLevelConfig> array = new List<HarvestLevelConfig>();

			//while(patterns.Count<4)
			//	patterns.Add("Muuuuuu");
			
			foreach(PackagedNextWord nw in list){
				
				List<int> matches = new List<int>();
				
				WordProblemInfo wp = nw.getAnnotatedWord().getWordProblems()[0];
				string problem = profileDifficulties[ wp.category ][ wp.index  ].character;//getDescriptionsToString();
				
				for(int i = 0;i<patterns.Count;i++){
					
					if(patterns[i]==problem){
						matches.Add(i);
					}
				}

				if (matches.Count>0)
					array.Add(new HarvestLevelConfig(nw.getAnnotatedWord().getWord(),patterns.ToArray(),matches.ToArray(),nw.getAnnotatedWord().getWordProblems()[0].category,nw.getAnnotatedWord().getWordProblems()[0].index));

			}

			lvlArr = array.ToArray();
			

		}else if(level.mode==2){

			foreach(PackagedNextWord nw in list){//confusing lettershapes
				//foreach(WordProblemInfo wp in nw.getAnnotatedWord().getWordProblems()){
				
				WordProblemInfo wp = nw.getAnnotatedWord().getWordProblems()[0];
				
				string problem = profileDifficulties[ wp.category ][ wp.index  ].getDescriptionsToString();

				if(!patterns.Contains(problem.Split('/')[0])){
					patterns.Add(problem.Split('/')[0]);
				}

				if(!patterns.Contains(problem.Split('/')[1])){
					patterns.Add(problem.Split('/')[1]);
				}
				//}
			}

			while(patterns.Count<4){
				
				int lA = list[0].getAnnotatedWord().getWordProblems()[0].category;
				int i = Random.Range(0,profileDifficulties[lA].Length);
				
				string problem = profileDifficulties[ lA ][ i  ].getDescriptionsToString();
				
				if(!patterns.Contains(problem.Split('/')[0])){
					patterns.Add(problem.Split('/')[0]);
				}
				
				if(!patterns.Contains(problem.Split('/')[1])){
					patterns.Add(problem.Split('/')[1]);
				}
			}



			foreach(PackagedNextWord nw in list){

				List<int> matches = new List<int>();
			
				for(int i = 0;i<patterns.Count;i++){

					if(nw.getAnnotatedWord().getWord().IndexOf(patterns[i])>-1){
						matches.Add(i);
					}
				}
				lvlArr[idx++] = new HarvestLevelConfig(nw.getAnnotatedWord().getWord(),patterns.ToArray(),matches.ToArray(),nw.getAnnotatedWord().getWordProblems()[0].category,nw.getAnnotatedWord().getWordProblems()[0].index);


			}

	

		}else if(level.mode==3){//gr//NOT USED


			foreach(PackagedNextWord nw in list){
				//foreach(WordProblemInfo wp in nw.getAnnotatedWord().getWordProblems()){
				
				string problem =   nw.getAnnotatedWord().getSyllables().Length.ToString();

				if(!patterns.Contains(problem)){
					patterns.Add(problem);
				}
				if (patterns.Count==4)
					break;
				//}
			}

			if(patterns.Count<4){
				for (int i=2;i<6;i++){

					if(!patterns.Contains(i.ToString()))
						patterns.Add(i.ToString());

					if (patterns.Count==4)
							break;

				}
			}


			foreach(PackagedNextWord nw in list){
				
				List<int> matches = new List<int>();
				
				string problem = nw.getAnnotatedWord().getSyllables().Length.ToString();

				for(int i = 0;i<patterns.Count;i++){
					
					if(patterns[i]==problem){
						matches.Add(i);
					}
				}
				if (matches.Count!=0)
					lvlArr[idx++] = new HarvestLevelConfig(nw.getAnnotatedWord().getWord(),patterns.ToArray(),matches.ToArray(),nw.getAnnotatedWord().getWordProblems()[0].category,nw.getAnnotatedWord().getWordProblems()[0].index);
			}


		}else if(level.mode==4){//Function words
			
			
			foreach(PackagedNextWord nw in list){
				//foreach(WordProblemInfo wp in nw.getAnnotatedWord().getWordProblems()){
				
				string problem =   nw.getAnnotatedWord().getType();
				
				if(!patterns.Contains(problem)){
					patterns.Add(problem);
				}
				if (patterns.Count==4)
					break;
				//}
			}

			//while(patterns.Count<4){
					
			//			patterns.Add("Muuuuuu");
		
			//}
			
			
			foreach(PackagedNextWord nw in list){
				
				List<int> matches = new List<int>();
				
				string problem = nw.getAnnotatedWord().getType();
				
				for(int i = 0;i<patterns.Count;i++){
					
					if(patterns[i]==problem){
						matches.Add(i);
					}
				}
				if (matches.Count!=0)
					lvlArr[idx++] = new HarvestLevelConfig(nw.getAnnotatedWord().getWord(),patterns.ToArray(),matches.ToArray(),nw.getAnnotatedWord().getWordProblems()[0].category,nw.getAnnotatedWord().getWordProblems()[0].index);
			}
			
			
		}else if(level.mode==5){

			List<string> problems = new List<string>();
			List<int> wordsCount = new List<int>();
			
			foreach(PackagedNextWord nw in list){
				foreach(WordProblemInfo wp in nw.getAnnotatedWord().getWordProblems()){
				
					string problem = nw.getAnnotatedWord().getWord().Substring(wp.matched[0].start,wp.matched[0].end-wp.matched[0].start);
				
					if(!problems.Contains(problem)){
						//Debug.Log("New! "+nw.getAnnotatedWord().getWord()+" "+problem);
						problems.Add(problem);
						wordsCount.Add (1);
					}else{
						//Debug.Log("Old! "+nw.getAnnotatedWord().getWord()+" "+problem);

						next_string_pattern = problem;
						int i = problems.FindIndex(FindString);
						wordsCount[i] = wordsCount[i]+1;
					
					}
				}
				
			}
			
			
			List<string> validPatterns = new List<string>();
			List<string> needRepresentative = new List<string>();
			List<string> bench = new List<string>();

			for(int i =0;i<problems.Count;i++){
				
				if( wordsCount[i]>1){
					validPatterns.Add(problems[i]);
					needRepresentative.Add(problems[i]);
				}else{

					bench.Add(problems[i]);
				}
				if (validPatterns.Count==4)
					break;
				
			}

			//Debug.Log(validPatterns.Count);

			foreach(string w in bench){
				if(validPatterns.Count<4){
					validPatterns.Add(w);
					needRepresentative.Add(w);
				}else
					break;
			}


			while(validPatterns.Count<4){
				string newPattern = validPatterns[Random.Range(0,validPatterns.Count)];
				validPatterns.Add(newPattern);
				needRepresentative.Add(newPattern);
			}
			//Debug.Log(validPatterns.Count);

			List<PackagedNextWord> words = new List<PackagedNextWord>();
			
			foreach(PackagedNextWord nw in list){

				bool representative = false;
				foreach(WordProblemInfo wp in nw.getAnnotatedWord().getWordProblems()){
				
					string problem = nw.getAnnotatedWord().getWord().Substring(wp.matched[0].start,wp.matched[0].end-wp.matched[0].start);

					if(needRepresentative.Contains(problem)){
						patterns.Add(nw.getAnnotatedWord().getWord());
						needRepresentative.Remove(problem);
						representative = true;
						break;
					}
				}
				if(!representative)
					words.Add(nw);

			}


			List<HarvestLevelConfig> array = new List<HarvestLevelConfig>();
			//Debug.Log(patterns.Count);
			
			foreach(PackagedNextWord nw in words){
				
				List<int> matches = new List<int>();
				for(int i = 0; i<validPatterns.Count;i++){

					foreach(WordProblemInfo wp in nw.getAnnotatedWord().getWordProblems()){
					
						string problem = nw.getAnnotatedWord().getWord().Substring(wp.matched[0].start,wp.matched[0].end-wp.matched[0].start);

						if (validPatterns[i]==problem){
							matches.Add(i);
							//Debug.Log("break?");
							break;
						}

					}
				}
				//Debug.Log(nw.getAnnotatedWord().getWord()+" "+matches.Count);
				if(matches.Count>0){
					array.Add(new HarvestLevelConfig(nw.getAnnotatedWord().getWord(),patterns.ToArray(),matches.ToArray(),nw.getAnnotatedWord().getWordProblems()[0].category,nw.getAnnotatedWord().getWordProblems()[0].index));
					if(array.Count==level.batchSize)
						break;
				}
			}
			lvlArr = array.ToArray();


		}else if(level.mode==6){

			List<int> numberSyllables = new List<int>();
			List<int> wordsCount = new List<int>();

			foreach(PackagedNextWord nw in list){
				//foreach(WordProblemInfo wp in nw.getAnnotatedWord().getWordProblems()){
				
				int syllables =   nw.getAnnotatedWord().getSyllables().Length;

				if(!numberSyllables.Contains(syllables)){
					numberSyllables.Add(syllables);
					wordsCount.Add (1);
				}else{
					next_int_pattern = syllables;
					int i = numberSyllables.FindIndex(FindInt);

					wordsCount[i] = wordsCount[i]+1;

				}

			}


			List<int> validPatterns = new List<int>();
			List<int> bench = new List<int>();
			List<int> needRepresentative = new List<int>();

			for(int i =0;i<numberSyllables.Count;i++){

				if( wordsCount[i]>1){
					//validPatterns.Add(numberSyllables[i]);
					needRepresentative.Add(numberSyllables[i]);
				}else{
						
					bench.Add(numberSyllables[i]);

				}
					
				if (needRepresentative.Count==4)
						break;
					
			}
				
				//Debug.Log(validPatterns.Count);
				
				foreach(int w in bench){
					if(needRepresentative.Count<4){
						//validPatterns.Add(w);
						needRepresentative.Add(w);
					}else
						break;
				}


			while(needRepresentative.Count<4){
				int newPattern = needRepresentative[Random.Range(0,needRepresentative.Count)];
				//validPatterns.Add(newPattern);
				needRepresentative.Add(newPattern);
			}

			List<PackagedNextWord> words = new List<PackagedNextWord>();
			
			//string[] patternsArray = new string[needRepresentative.Count];

			foreach(PackagedNextWord nw in list){
				//foreach(WordProblemInfo wp in nw.getAnnotatedWord().getWordProblems()){
				
				int syllables = nw.getAnnotatedWord().getSyllables().Length;

				if(needRepresentative.Contains(syllables)){
					validPatterns.Add(syllables);
					patterns.Add(nw.getAnnotatedWord().getWord());
					needRepresentative.Remove(syllables);
				}else if(validPatterns.Contains(syllables)){
					words.Add(nw);
					//Debug.Log("WORD "+nw.getAnnotatedWord().getWord());
				}

			}


			List<HarvestLevelConfig> array = new List<HarvestLevelConfig>();
			foreach(PackagedNextWord nw in words){

				List<int> matches = new List<int>();
				int syllables = nw.getAnnotatedWord().getSyllables().Length;

				for(int i = 0; i<validPatterns.Count;i++)
					if (validPatterns[i]==syllables)
						matches.Add(i);

				if(matches.Count>0){
					array.Add(new HarvestLevelConfig(nw.getAnnotatedWord().getWord(),patterns.ToArray(),matches.ToArray(),nw.getAnnotatedWord().getWordProblems()[0].category,nw.getAnnotatedWord().getWordProblems()[0].index));
					if(array.Count==level.batchSize)
						break;
				}

			}


			lvlArr = array.ToArray();



		}


		for(int i= 1;i<lvlArr.Length;i++){
			
			int j = Random.Range(1,lvlArr.Length);
			
			HarvestLevelConfig a = lvlArr[i];
			lvlArr[i] = lvlArr[j];
			lvlArr[j] = a;
			
		}	


	}catch(System.Exception e){
		
		WorldViewServerCommunication.setError(e.Message);
		return;
	}

		

		counter = 0;
	}


	private static string next_string_pattern = "";
	private static int next_int_pattern = -1;

	private static bool FindString(string pattern)
	{
		
		if (pattern == next_string_pattern)
		{
			return true;
		}
		else
		{
			return false;
		}
		
	}

	private static bool FindInt(int pattern)
	{
		
		if (pattern == next_int_pattern)
		{
			return true;
		}
		else
		{
			return false;
		}
		
	}
	
	public ILevelConfig getNextLevelConfig(System.Object para_extraInfo)
	{
		int reqIndex = counter % lvlArr.Length;
		HarvestLevelConfig reqConfig = lvlArr[reqIndex];
		counter++;
		return reqConfig;
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class WordDBCreator
{


	static public string getLevel(string app){
		Debug.LogError("Offline word creator");

		switch(app){
		case "DROP_CHOPS": return "M0-A2-S0-B10-W0-F0-T1";
			
		case "SERENADE_HERO": return "M0-A3-S0-B10-W0-F3-T0";
			
		case "MAIL_SORTER": return "M1-A2-S0-B5-W0-F3-T0";
			
		case "MOVING_PATHWAYS": return "M1-A1-S0-B10-W0-F3-T0";	
			
		case "WHAK_A_MOLE": return "M3-A10-S0-B10-W0-F3-T0";
			
		case "HARVEST": return "M0-A0-S0-B5-W1-F1-T0";
			
		case "TRAIN_DISPATCHER": return "M0-A0-S0-B5-W1-F0-T0";
			
		case "EYE_EXAM": return "M0-A3-S0-B10-W1-F0-T0";
			
		case "ENDLESS_RUNNER": return "M0-A0-S2-B5-W1-F0-T0";

			
		}
		return "M0-A0-S2-B5-W1-F0-T0";
		

		
	}


	static public List<PackagedNextWord> createListPackagedNextWords(string app){

		/*if(lan == LanguageCode.GR){
			return WordDBCreatorGR.createListPackagedNextWords(app, 0,0,0,"",0,"")
			

		}*/
		string file = "Localisation_Files/EN/Offline_";
		
		switch(app){
		case "DROP_CHOPS": file+="SJ_EN";break;
			
		case "SERENADE_HERO": file+="SH_EN";break;
			
		case "MAIL_SORTER": file+="MS_EN";break;
			
		case "MOVING_PATHWAYS": file+="MP_EN";break;
			
		case "WHAK_A_MOLE": file+="WAM_EN";break;
			
		case "HARVEST": file+="HARVEST_EN";break;
			
		case "TRAIN_DISPATCHER": file+="TD_EN";break;
			
		case "EYE_EXAM": file+="BB_EN";break;
			
		case "ENDLESS_RUNNER": file+="PD_EN";break;
			
		default: Debug.Log("Hard Coded for " +app+" not available");break;
			
			
		}
		
		
		TextAsset ta = (TextAsset) Resources.Load(file,typeof(TextAsset));
		
		return JsonHelper.deserialiseObject<List<PackagedNextWord>>(ta.text);


	}

	static public List<PackagedNextWord> createListPackagedNextWords(string app, int number_words,int difficultyID,int languageArea,string userID,int evaluation_mode,string challenge)
	{

		return createListPackagedNextWords(app);


		
		/*



		switch(app){


		case "DROP_CHOPS":
			return SJwords();
		case "SERENADE_HERO":
			return SHsentences();
		case "MAIL_SORTER":
			return MSwords();
		case "MOVING_PATHWAYS":
			return MPwords();
		case "WHAK_A_MOLE":
			return WAMwords();
		case "HARVEST":
			return Harvestwords();
		case "TRAIN_DISPATCHER":
			return TDwords();
		case "EYE_EXAM":
			return BBwords();
		case "ENDLESS_RUNNER":
			return PDwords();
		default:
			Debug.Log("Hard Coded for " +app+" not available");
			return new List<PackagedNextWord>();
		}*/


	}

	static public List<PackagedNextWord> PDwords(){


		return new List<PackagedNextWord>{
			new PackagedNextWord( new  AnnotatedWord("teacher",new List<int>{2}  ),false),
			new PackagedNextWord( new  AnnotatedWord("item",new List<int>{0}  ),false),
			new PackagedNextWord( new  AnnotatedWord("watermelon",new List<int>{1,4,6} ),false),
			new PackagedNextWord( new  AnnotatedWord("acorn",new List<int>{0}  ),false),
			new PackagedNextWord( new  AnnotatedWord("computer",new List<int>{2,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("table",new List<int>{1}  ),false),
			new PackagedNextWord( new  AnnotatedWord("bicycle",new List<int>{1,3}  ),false),
			new PackagedNextWord( new  AnnotatedWord("helicopter",new List<int>{1,3,6}  ),false),
			new PackagedNextWord( new  AnnotatedWord("laptop",new List<int>{2}  ),false)
		};



	}

	static public List<PackagedNextWord> TDwords(){


		return new List<PackagedNextWord>{
			new PackagedNextWord( new  AnnotatedWord("teacher",new List<int>{2}  ),false),
			new PackagedNextWord( new  AnnotatedWord("helicopter",new List<int>{1,3,6}  ),false),
			new PackagedNextWord( new  AnnotatedWord("small",new List<int>{}  ),false),
			new PackagedNextWord( new  AnnotatedWord("item",new List<int>{0}  ),false),
			new PackagedNextWord( new  AnnotatedWord("meant",new List<int>{}  ),false),
			new PackagedNextWord( new  AnnotatedWord("watermelon",new List<int>{1,4,6} ),false),
			new PackagedNextWord( new  AnnotatedWord("acorn",new List<int>{0}  ),false),
			new PackagedNextWord( new  AnnotatedWord("computer",new List<int>{2,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("table",new List<int>{1}  ),false),
			new PackagedNextWord( new  AnnotatedWord("bicycle",new List<int>{1,3}  ),false),
			new PackagedNextWord( new  AnnotatedWord("laptop",new List<int>{2}  ),false)
		};

	}


	static public List<PackagedNextWord> BBwords(){

		return new List<PackagedNextWord>{
			new PackagedNextWord( new  AnnotatedWord("decoder",new List<int>{1,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("preordered",new List<int>{2,4,7}  ),false),
			new PackagedNextWord( new  AnnotatedWord("incoming",new List<int>{1,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("misunderstanding",new List<int>{2,4,7,12}  ),false)
		};


	}


	static public List<PackagedNextWord> MPwords(){
		
		return new List<PackagedNextWord>{

			new PackagedNextWord( new  AnnotatedWord("b","single letter") , false    ),
			new PackagedNextWord( new  AnnotatedWord("d","single letter") , false    ),
			new PackagedNextWord( new  AnnotatedWord("q","single letter") , false    ),
			new PackagedNextWord( new  AnnotatedWord("p","single letter") , false    )
		};


	}

	static public List<PackagedNextWord> Harvestwords(){
		
		return new List<PackagedNextWord>{

			new PackagedNextWord( new  AnnotatedWord("calling","-ing",new string[]{"call","ing"}) , false       ),
			new PackagedNextWord( new  AnnotatedWord("caring","-ing",new string[]{"caring"}) , false       ),
			new PackagedNextWord( new  AnnotatedWord("beginner","-er",new string[]{"be","gin","ner"}) , false    ),

			new PackagedNextWord( new  AnnotatedWord("taking","-ing",new string[]{"tak","ing"}) , false       ),
			new PackagedNextWord( new  AnnotatedWord("caller","-er",new string[]{"call","er"}) , false       )
		


		};

	}



	static public List<PackagedNextWord> WAMwords(){
					
			return new List<PackagedNextWord>{
				
				new PackagedNextWord( new  AnnotatedWord("jumped","Suffix 'ed'") , false       ),
				new PackagedNextWord( new  AnnotatedWord("chased","Suffix 'ed'") , false       ),
				new PackagedNextWord( new  AnnotatedWord("pushed","Suffix 'ed'") , false       ),
				new PackagedNextWord( new  AnnotatedWord("climbed","Suffix 'ed'") , false       ),
				new PackagedNextWord( new  AnnotatedWord("called","Suffix 'ed'") , false       ),
				new PackagedNextWord( new  AnnotatedWord("danced","Suffix 'ed'") , false       ),

				new PackagedNextWord( new  AnnotatedWord("jumper","Suffix 'ed'") , true       ),
				new PackagedNextWord( new  AnnotatedWord("chaser","Suffix 'ed'") , true       ),
				new PackagedNextWord( new  AnnotatedWord("pusher","Suffix 'ed'") , true       ),
			new PackagedNextWord( new  AnnotatedWord("climber","Suffix 'ed'") , true       ),
			new PackagedNextWord( new  AnnotatedWord("caller","Suffix 'ed'") , true       ),
			new PackagedNextWord( new  AnnotatedWord("dancer","Suffix 'ed'") , true       ),



			new PackagedNextWord( new  AnnotatedWord("redo","Prefix 're'") , false       ),
			new PackagedNextWord( new  AnnotatedWord("rerun","Prefix 're'") , false       ),
			new PackagedNextWord( new  AnnotatedWord("remake","Prefix 're'") , false       ),
			new PackagedNextWord( new  AnnotatedWord("reorganise","Prefix 're'") , false       ),
			new PackagedNextWord( new  AnnotatedWord("resell","Prefix 're'") , false       ),
			new PackagedNextWord( new  AnnotatedWord("reopen","Prefix 're'") , false       ),


			new PackagedNextWord( new  AnnotatedWord("doer","Prefix 're'") , true       ),
			new PackagedNextWord( new  AnnotatedWord("runner","Prefix 're'") , true       ),
			new PackagedNextWord( new  AnnotatedWord("maker","Prefix 're'") , true       ),
			new PackagedNextWord( new  AnnotatedWord("organiser","Prefix 're'") , true       ),
			new PackagedNextWord( new  AnnotatedWord("seller","Prefix 're'") , true       ),
			new PackagedNextWord( new  AnnotatedWord("opener","Prefix 're'") , true       )




			};

	}


	static public List<PackagedNextWord> MSwords(){

		return new List<PackagedNextWord>{
			new PackagedNextWord( new  AnnotatedWord("calling","-ing",new string[]{"call","ing"}) , false       ),
			new PackagedNextWord( new  AnnotatedWord("caller","-er",new string[]{"call","er"}) , false       ),
			new PackagedNextWord( new  AnnotatedWord("called","-ed",new string[]{"called"}) , false       ),

			new PackagedNextWord( new  AnnotatedWord("caring","-ing",new string[]{"caring"}) , false       ),
			new PackagedNextWord( new  AnnotatedWord("careless","-less",new string[]{"care","less"}) , false       ),
			new PackagedNextWord( new  AnnotatedWord("carefull","-full",new string[]{"care","full"}) , false       ),

			new PackagedNextWord( new  AnnotatedWord("taken","-en",new string[]{"tak","en"}) , false       ),
			new PackagedNextWord( new  AnnotatedWord("taking","-ing",new string[]{"tak","ing"}) , false       ),
			new PackagedNextWord( new  AnnotatedWord("takes"," -s ",new string[]{"takes"}) , false       ),

		};


	}

	static public List<PackagedNextWord> SHsentences(){



		return new List<PackagedNextWord>{

			new PackagedNextWord(  new  AnnotatedSentence("You have to complete this {sentence}",new List<string>{"write","read"} ) , false   ),
			new PackagedNextWord(  new  AnnotatedSentence("I don't think {you're} right",new List<string>{"your","you"} ) , false   ),
			new PackagedNextWord(  new  AnnotatedSentence("It was {their} idea",new List<string>{ "they're","they" }) , false   ),
			new PackagedNextWord(  new  AnnotatedSentence("The dog {ran} after the cat",new List<string>{ "run","ruin" }) , false   ),
			new PackagedNextWord(  new  AnnotatedSentence("Superman does {good} , you are doing {well}",new List<string>{"goodness"}) , false   ),
			new PackagedNextWord(  new  AnnotatedSentence("We all {thought} the same thing",new List<string>{ "though","through" }) , false   ),
			new PackagedNextWord(  new  AnnotatedSentence("Leave the book {on} the table",new List<string>{ "in","onto" }) , false   ),
			new PackagedNextWord(  new  AnnotatedSentence("Last month we {read} two books",new List<string>{ "red","bread" }) , false   )


		};

	}
		//static public List<PackagedNextWord> createListPackagedNextWords()
	static public List<PackagedNextWord> SJwords()
		{

		return new List<PackagedNextWord>{
			new PackagedNextWord( new  AnnotatedWord("helicopter",new List<int>{1,3,6}  ),false),
			new PackagedNextWord( new  AnnotatedWord("misunderstanding",new List<int>{2,4,7,12}   ),false),
			new PackagedNextWord( new  AnnotatedWord("theory",new List<int>{2,3}  ),false),
			new PackagedNextWord( new  AnnotatedWord("thermometer",new List<int>{3,5,7}  ),false),
			new PackagedNextWord( new  AnnotatedWord("computer",new List<int>{2,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("table",new List<int>{1}  ),false),
			new PackagedNextWord( new  AnnotatedWord("watermelon",new List<int>{1,4,6} ),false),
			new PackagedNextWord( new  AnnotatedWord("laptop",new List<int>{2}  ),false)
		};

		/*TextAsset ta = (TextAsset) Resources.Load("SegmentationWords",typeof(TextAsset));
		string[] lines = ta.text.Split(new string[] { "\r\n" },System.StringSplitOptions.None);
		
		
		int currDiffIndicator = 1;


		List<PackagedNextWord> list = new List<PackagedNextWord>();


		bool nwSetComplete = false;
		
		for(int k=0; k<lines.Length; k++)
		{
			string readData = lines[k];
			
			int tmpInt = 0;
			if(int.TryParse(readData,out tmpInt))
			{
				if(nwSetComplete)
				{
					// We have a complete word set.
					return list;//single difficulty
				}				
				
				currDiffIndicator = tmpInt;
				nwSetComplete = true;
			}
			else
			{
				// Parse and add a new word to the current word set.
				
				string[] splitStrs = readData.Split(':');
				
				string nwWordTextRep = splitStrs[0];
				string[] syllableSplitPos = splitStrs[1].Split(',');
				
				List<int> ssPosIntList = new List<int>();
				for(int i=0; i<syllableSplitPos.Length; i++)
				{
					ssPosIntList.Add(int.Parse(syllableSplitPos[i]));	
				}
				
				AnnotatedWord reqW = new AnnotatedWord(nwWordTextRep,ssPosIntList);
				list.Add (  new PackagedNextWord(  reqW    ,  false)   );

			}
		}
		
		return list;//single difficulty
		*/

	}
		
	
}

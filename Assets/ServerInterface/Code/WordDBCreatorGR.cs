/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class WordDBCreatorGR
{



	static public List<PackagedNextWord> createListPackagedNextWords(string app, int number_words,int difficultyID,int languageArea,string userID,int evaluation_mode,string challenge)
	{

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
		}


	}

	static public List<PackagedNextWord> PDwords(){




		return new List<PackagedNextWord>{
			new PackagedNextWord( new  AnnotatedWord("ομάδα",new List<int>{0,2}  ),false),
			new PackagedNextWord( new  AnnotatedWord("πιέζω",new List<int>{1,2}  ),false),
			new PackagedNextWord( new  AnnotatedWord("συμβάλλω",new List<int>{2,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("φιγούρα",new List<int>{1,4} ),false),
			new PackagedNextWord( new  AnnotatedWord("φαγοπότι",new List<int>{1,3,5}  ),false),
			new PackagedNextWord( new  AnnotatedWord("διάβασμα",new List<int>{2,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("βουβάλι",new List<int>{2,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("γυμνάζω",new List<int>{1,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("ναρθηκας",new List<int>{2,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("χρειάζομαι",new List<int>{3,4,6}  ),false)
		};



	}

	static public List<PackagedNextWord> TDwords(){


			
				
				
				
		
		return new List<PackagedNextWord>{
			new PackagedNextWord( new  AnnotatedWord("φουγαρο",new List<int>{2,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("κειμενο",new List<int>{2,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("ψιθυρος",new List<int>{1,3}  ),false),
			new PackagedNextWord( new  AnnotatedWord("στελνω",new List<int>{3}  ),false),
			new PackagedNextWord( new  AnnotatedWord("πριζα",new List<int>{2}  ),false),

		};

	}


	static public List<PackagedNextWord> BBwords(){

		return new List<PackagedNextWord>{
			new PackagedNextWord( new  AnnotatedWord("βαρκάρης",new List<int>{3}  ),false),
			new PackagedNextWord( new  AnnotatedWord("χρωματοπωλείο",new List<int>{9}  ),false),
			new PackagedNextWord( new  AnnotatedWord("αγωνίζομαι",new List<int>{5}  ),false),

		};


	}


	static public List<PackagedNextWord> MPwords(){
		
		return new List<PackagedNextWord>{

			new PackagedNextWord( new  AnnotatedWord("φ","single letter") , false    ),
			new PackagedNextWord( new  AnnotatedWord("β","single letter") , false    ),
			new PackagedNextWord( new  AnnotatedWord("θ","single letter") , false    )

		};


	}

	static public List<PackagedNextWord> Harvestwords(){
		
		return new List<PackagedNextWord>{


				

			new PackagedNextWord( new  AnnotatedWord("βαρκάρης","-άρης",new string[]{"βαρ","κά","ρής"}) , false    ),
			new PackagedNextWord( new  AnnotatedWord("σκουπιδιάρης","-άρης'",new string[]{"σκού","πι","διά","ρης"}) , false    ),
			new PackagedNextWord( new  AnnotatedWord("ξενοδοχείο","-είο",new string[]{"ξε","νο","δο","χεί","ο"}) , false    ),
			new PackagedNextWord( new  AnnotatedWord("παντοπωλείο","-είο'",new string[]{"πα","ντο","πω","λεί","ο"}) , false    ),

	
		};

	}




	static public List<PackagedNextWord> WAMwords(){
					
			return new List<PackagedNextWord>{
				
					
					
			new PackagedNextWord( new  AnnotatedWord("βαρκάρης","-άρης") , false       ),
			new PackagedNextWord( new  AnnotatedWord("σκουπιδιάρης","-άρης") , false       ),
			new PackagedNextWord( new  AnnotatedWord("παιχνιδιάρης","-άρης") , false       ),
			new PackagedNextWord( new  AnnotatedWord("γκρινιάρης","-άρης") , false       ),
			new PackagedNextWord( new  AnnotatedWord("βαρκάρης","-άρης") , false       ),
			new PackagedNextWord( new  AnnotatedWord("παιχνιδιάρης","-άρης") , false       ),



			new PackagedNextWord( new  AnnotatedWord("αγωνίζομαι","-άρης") , true       ),
			new PackagedNextWord( new  AnnotatedWord("εργάζομαι","-άρης") , true       ),
			new PackagedNextWord( new  AnnotatedWord("συλλογίζομαι","-άρης") , true       ),
			new PackagedNextWord( new  AnnotatedWord("χειρίζομαι","-άρης") , true       ),
			new PackagedNextWord( new  AnnotatedWord("συλλογίζομαι","-άρης") , true       ),
			new PackagedNextWord( new  AnnotatedWord("εργάζομαι","-άρης") , true       ),



			new PackagedNextWord( new  AnnotatedWord("χρωματοπωλείο","είο") , false       ),
			new PackagedNextWord( new  AnnotatedWord("ξενοδοχείο","είο") , false       ),
			new PackagedNextWord( new  AnnotatedWord("παντοπωλείο","είο") , false       ),
			new PackagedNextWord( new  AnnotatedWord("δισκοπωλείο","είο") , false       ),
			new PackagedNextWord( new  AnnotatedWord("ξενοδοχείο","είο") , false       ),
			new PackagedNextWord( new  AnnotatedWord("χρωματοπωλείο","είο") , false       ),


			new PackagedNextWord( new  AnnotatedWord("αγωνίζομαι","είο") , true       ),
			new PackagedNextWord( new  AnnotatedWord("εργάζομαι","είο") , true       ),
			new PackagedNextWord( new  AnnotatedWord("συλλογίζομαι","είο") , true       ),
			new PackagedNextWord( new  AnnotatedWord("χειρίζομαι","είο") , true       ),
			new PackagedNextWord( new  AnnotatedWord("εργάζομαι","είο") , true       ),
			new PackagedNextWord( new  AnnotatedWord("χειρίζομαι","είο") , true       )



					
					
					

			};

	}


	static public List<PackagedNextWord> MSwords(){

		return new List<PackagedNextWord>{
			new PackagedNextWord( new  AnnotatedWord("βαρκάρης","-ρης'",new string[]{"βαρ","κά","ρης"}) , false       ),
			new PackagedNextWord( new  AnnotatedWord("ξενοδοχείο","-είο",new string[]{"ξε-νο-δο-χεί-ο"}) , false       ),
			new PackagedNextWord( new  AnnotatedWord("αγωνίζομαι","-ομαι",new string[]{"α","γω","νί","ζο","μαι"}) , false       ),


			new PackagedNextWord( new  AnnotatedWord("σκουπιδιάρης","-άρης",new string[]{"σκού","πι","διά","ρης"}) , false       ),
			new PackagedNextWord( new  AnnotatedWord("παντοπωλείο","-είο",new string[]{"πα","ντο","πω","λεί","ο"}) , false       ),
			new PackagedNextWord( new  AnnotatedWord("εργάζομαι","-ομαι",new string[]{"ερ","γά","ζο","μαι"}) , false       ),


			new PackagedNextWord( new  AnnotatedWord("γκρινιάρης","-άρης",new string[]{"γκρι","νιά","ρης"}) , false       ),
			new PackagedNextWord( new  AnnotatedWord("δισκοπωλείο","-είο",new string[]{"δι","σκο","πω","λεί","ο"}) , false       ),
			new PackagedNextWord( new  AnnotatedWord("χειρίζομαι","-ομαι",new string[]{"χει","ρί","ζο","μαιe"}) , false       ),


		};


	}

	static public List<PackagedNextWord> SHsentences(){



		return new List<PackagedNextWord>{

			new PackagedNextWord(  new  AnnotatedSentence("Ο πατέρ {ας} της πουλάει καναπ {έδες}",new List<string>{"α","ές"} ) , false   ),
			new PackagedNextWord(  new  AnnotatedSentence("Το αγόρ {ι} φοβάται τις αλεπ {ούδες}",new List<string>{"ια","ές"} ) , false   ),
			new PackagedNextWord(  new  AnnotatedSentence("Ένα παιδ {άκι} ήταν πάνω στο δέντρ {ο}",new List<string>{ "ή","ού" }) , false   ),
			new PackagedNextWord(  new  AnnotatedSentence("Τα παιδ {ιά} μπήκαν στις τάξ {εις} τους",new List<string>{ "ί","ης" }) , false   ),
			new PackagedNextWord(  new  AnnotatedSentence("Οι εργάτ {ες} του δημαρχεί {ου}",new List<string>{"ης","ων"}) , false   ),


		};

	}
		//static public List<PackagedNextWord> createListPackagedNextWords()
	static public List<PackagedNextWord> SJwords()
		{

		
		return new List<PackagedNextWord>{
			new PackagedNextWord( new  AnnotatedWord("ομάδα",new List<int>{0,2}  ),false),
			new PackagedNextWord( new  AnnotatedWord("πιέζω",new List<int>{1,2}  ),false),
			new PackagedNextWord( new  AnnotatedWord("συμβάλλω",new List<int>{2,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("φιγούρα",new List<int>{1,4} ),false),
			new PackagedNextWord( new  AnnotatedWord("φαγοπότι",new List<int>{1,3,5}  ),false),
			new PackagedNextWord( new  AnnotatedWord("διάβασμα",new List<int>{2,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("βουβάλι",new List<int>{2,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("γυμνάζω",new List<int>{1,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("ναρθηκας",new List<int>{2,4}  ),false),
			new PackagedNextWord( new  AnnotatedWord("χρειάζομαι",new List<int>{3,4,6}  ),false)
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

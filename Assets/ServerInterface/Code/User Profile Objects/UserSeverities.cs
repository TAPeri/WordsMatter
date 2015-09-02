/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using System.Collections.Generic;

[System.Serializable]


public class UserSeverities {

	public int[] systemIndices;
	// the array teacherIndices contains the teacher's index about the user
	public int[] teacherIndices;
	// the matrix severities contains the severities to the problems of the user
	public int[][] severities;
	
	// Use this for initialization
	public UserSeverities(){
		
	}

	public UserSeverities(string language){

		if (language=="EN"){


			severities = new int[][]{//Language areas  
				
				new int[]{ 0,2,3,1,2,3,3,3,3,1,2,3,1,2,3,3,2,3,1,2,3,1,2,3,3,3,3,1,2,3,3,2,3,1,2,3,1},//Syllable division [Junkyard, Moving Pathways, Harvest, Bridge Builder, Train:  1,2,3,3,3]
				new int[]{ 2,3,3,3,2,3,3,3,2,3,3,3,3,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3},//Vowel sounds [Moving Pathways, Harvest, Whack-A-Monkey, Mail Sorter, Bridge Builder:  2,3,3,3,3]
				new int[]{ 0,1,3,3,3,0,1,3,3,3,0,1,3,3,3,0,1,3,3,3,3,1,3,3,3,0,1,3,3,3,0,1,3,3,3,0,3,3,3,3,0,3,3,3,3,0,1,3,3,3,0,1,3,3,3,0,1,3,3},//Suffixing [all games except Moving Pathways:  0,1,3,3,3,3,3]
				new int[]{ 0,1,3,3,3,0,1,3,3,3,0,1,3,3,3,0,1,3,3,3,0,1,3,3,3,0,1,3,3,3,0,1,3,3,3,0,1,3,3,3},//Prefixing [all games except Moving Pathways: 0,1,3,3,3,3,3]
				new int[]{ 2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2},//Phon-Graph [Moving Pathways, Harvest, Whack-A-Monkey, Mail Sorter, Train: 2,3,3,3,3]
				new int[]{ 3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},//Letter Patterns [Harvest, Whack-A-Monkey, Mail Sorter: 3,3,3]
				
				//Letter names
				//Confusing letter shapes ?
				
			};

			systemIndices = new int[5];
			teacherIndices = new int[5];


		}else{
			//Greek		
			severities = new int[][]{  
				new int[]{1,2,3,3,3,3,1,2,3,3,2,3,1,2,3,1,2,3,3,2},//Syllable Division [Junkyard, Moving Pathways, Harvest, Bridge Builder, Train: 1,2,3,3,3]
				new int[]{2,3,3,2,3,3,2,3,3,2,3,3},//Consonants [Moving Pathways, Mail Sorter, Bridge Builder: 2,3,3]
				new int[]{2,3,3,2,3},//Vowels [Moving Pathways, Harvest, Bridge Builder: 2,3,3]
				new int[]{0,1,3,3,3,3,1,3,3,3,3,1,3},//Derivational Suffixing [all games except Moving Pathways: 0,1,3,3,3,3,3]
				new int[]{0,3,3,3,3,3,0,3,0,3,3,3,0,3,3,3,0},//Inflectional/Grammatical Suffixing [all games except Moving Pathways and DropChops/Junkyard: 0,3,3,3,3,3]
				new int[]{0,1,3,3,3,1},//Prefixing [all games except Moving Pathways and Whack-A-Monkey: 0,1,3,3,3,3]
				new int[]{2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3},//Regular/Irregular:Consonant clusters [Moving Pathways, Harvest, Whack-A-Monkey, Mail Sorter: 2,3,3,3]
				new int[]{0,2,3,3,0,2,3,3,0,2},//Grammar/Function Words [Serenade Hero, Moving Pathways, Harvest, Train: 0,2,3,3]
				new int[]{2,3,3,2,3,3,2,3}//Visual Similarity [Moving Pathways, Whack-A-Monkey, Mail Sorter: 2,3,3]
			};

			systemIndices = new int[9];
			teacherIndices = new int[9];
		}

	}

	public UserSeverities( int[] s, int[] t, int[][] se){ systemIndices = s; teacherIndices = t; severities = se;}


	public int[][] getSeverities(){return severities;}

}

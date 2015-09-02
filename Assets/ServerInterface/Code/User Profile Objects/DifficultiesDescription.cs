/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */[System.Serializable]

public class DifficultiesDescription {
	
	public LanguageArea[] problemsIndex;
	// the problems matrix contains all possible problems for a language
	public Difficulty[][] problems;
	
	public string language;

	public DifficultiesDescription()
	{
		// Empty constructor required for JSON converter.
	}

	public DifficultiesDescription(string language){

		if (language=="EN"){
			problemsIndex = new LanguageArea[5];

			problems = new Difficulty[][]{
				new Difficulty[37],//Syllable division [Junkyard, Moving Pathways, Harvest, Bridge Builder, Train:  1,2,3,6,7]
				new Difficulty[72],//Vowel sounds [Moving Pathways, Harvest, Whack-A-Monkey, Mail Sorter, Bridge Builder:  2,3,4,5,6]
				new Difficulty[59],//Suffixing [all games except Moving Pathways:  0,1,3,4,5,6,7]
				new Difficulty[40],//Prefixing [all games except Moving Pathways: 0,1,3,4,5,6,7]
				new Difficulty[57],//Phon-Graph [Moving Pathways, Harvest, Whack-A-Monkey, Mail Sorter, Train: 2,3,4,5,7]
				new Difficulty[127]//Letter Patterns [Harvest, Whack-A-Monkey, Mail Sorter: 3,4,5]
			};


		}else{
			problemsIndex = new LanguageArea[9];

			problems = new Difficulty[][]{
				new Difficulty[20],//Syllable Division [Junkyard, Moving Pathways, Harvest, Bridge Builder, Train: 1,2,3,6,7]
				new Difficulty[12],//Consonants [Moving Pathways, Mail Sorter, Bridge Builder: 2,5,6]
				new Difficulty[5],//Vowels [Moving Pathways, Harvest, Bridge Builder: 2,3,6]
				new Difficulty[13],//Derivational Suffixing [all games except Moving Pathways: 0,1,3,4,5,6,7]
				new Difficulty[17],//Inflectional/Grammatical Suffixing [all games except Moving Pathways and DropChops/Junkyard: 0,3,4,5,6,7]
				new Difficulty[6],//Prefixing [all games except Moving Pathways and Whack-A-Monkey: 0,1,3,5,6,7]
				new Difficulty[26],//Regular/Irregular:Consonant clusters [Moving Pathways, Harvest, Whack-A-Monkey, Mail Sorter: 2,3,4,5]
				new Difficulty[10],//Grammar/Function Words [Serenade Hero, Moving Pathways, Harvest, Train: 0,2,3,7]
				new Difficulty[8]//Visual Similarity [Moving Pathways, Whack-A-Monkey, Mail Sorter: 2,4,5]
			};
		}

	}

	public DifficultiesDescription( LanguageArea[] p, Difficulty[][] d, LanguageCode l){ problemsIndex=p;problems=d;language=l.ToString();}


	public LanguageArea[] getLanguageAreas(){return problemsIndex;}
	public Difficulty[][] getDifficulties(){return problems;}
	public LanguageCode getLanguage(){return (LanguageCode)System.Enum.Parse(typeof(LanguageCode), language);}


}
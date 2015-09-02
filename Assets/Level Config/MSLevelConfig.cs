/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class MSLevelConfig : ILevelConfig
{
	string[] parcelWords;
	string[] postmenWords;

	bool useTTS = false;
	int speed = 0;

	public int[] languageAreas = new int[]{-1};
	public int[] difficulties = new int[]{-1};

	public MSLevelConfig(string[] para_parcelWords,
	                     string[] para_postmenWords,
	                     TtsType ttsT,
	                     int defaultSpeed,int[] languageArea,int[] difficulty)
	{
		
		this.difficulties = difficulty;
		this.languageAreas = languageArea;
		parcelWords = para_parcelWords;
		postmenWords = para_postmenWords;

		speed = defaultSpeed;
		if (WorldViewServerCommunication.tts!=null){

			if ((ttsT == TtsType.SPOKEN2WRITTEN) || (ttsT == TtsType.WRITTEN2SPOKEN))//the second is Greek
				useTTS = true;

		}

	}

	public string[] getParcelWords() { return parcelWords; }
	public string[] getPostmenWords() { return postmenWords; }
	public int getSpeed(){return speed;}
	public bool getUseTtsFlag() { return useTTS; }


	public bool isCorrectParcelPostmanCombination(string para_parcelText, string para_postmanText)
	{
		int parcelTextIndex = findTextIndex(para_parcelText,parcelWords);
		if(parcelTextIndex != -1)
		{
			int postmanTextIndex = findTextIndex(para_postmanText,postmenWords);

			return (parcelTextIndex == postmanTextIndex);
		}

		return false;
	}

	public int getUnjumbledIndexForPostman(string para_postmanText)
	{
		return findTextIndex(para_postmanText,postmenWords);
	}

	private int findTextIndex(string para_text, string[] para_arr)
	{
		int retIndex = -1;

		for(int i=0; i<para_arr.Length; i++)
		{
			if(System.String.Equals(para_arr[i], para_text, System.StringComparison.Ordinal))
			{
				retIndex = i;
				break;
			}
		}

		return retIndex;
	}
}

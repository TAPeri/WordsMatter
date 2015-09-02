/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class LevelParameters {
	
	
	public int wordLevel;
	public FillerType fillerType;
	public int batchSize;
	public int speed;
	public int accuracy;
	public TtsType ttsType;
	public int mode;
	public int amountDistractors =1;
	public int amountTricky = 0;

	public LevelParameters(string level){
		
		string[] parameters = level.Split('-');
		
		foreach(string parameter in parameters){
			if(parameter.StartsWith("W")){
				wordLevel = System.Convert.ToInt32(parameter.Substring(1));
				
			}else if(parameter.StartsWith("F")){
				fillerType = (FillerType)System.Convert.ToInt32(parameter.Substring(1));
				
			}else if(parameter.StartsWith("B")){
				batchSize = System.Convert.ToInt32(parameter.Substring(1));
				
			}else if(parameter.StartsWith("S")){
				speed = System.Convert.ToInt32(parameter.Substring(1));
				
			}else if(parameter.StartsWith("A")){
				accuracy = System.Convert.ToInt32(parameter.Substring(1));
				
			}else if(parameter.StartsWith("T")){
				ttsType = (TtsType)System.Convert.ToInt32(parameter.Substring(1));

			}else if(parameter.StartsWith("M")){
				mode = System.Convert.ToInt32(parameter.Substring(1));
			}else if(parameter.StartsWith("X")){
				amountTricky = System.Convert.ToInt32(parameter.Substring(1));
			}else if(parameter.StartsWith("D")){
				amountDistractors = System.Convert.ToInt32(parameter.Substring(1));
			}
			
		}
		
	}

	public string toString(){
		//UnityEngine.Debug.LogError("PATCH");
		string output = "";


		output+= "M"+mode;
		output+= "-A"+accuracy;
		output+= "-S"+speed;
//		output+= "-B8";
		output+= "-B"+batchSize;
//		output +="-W0";
		output +="-W"+wordLevel;
		output+= "-F"+(int)fillerType;
		output+= "-T"+(int)ttsType;
		output+= "-X"+amountTricky;
		output+= "-D"+amountDistractors;

		return output;
	}



	
}
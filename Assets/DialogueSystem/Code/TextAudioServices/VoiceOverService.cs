/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class VoiceOverService
{

	VoiceOverLookupObject metaObj = null;


	public bool isServiceAvailable()
	{
		return true;
	}

	public List<AudioClip> extractContent(string para_reqParamStr,bool para_maleSetting)
	{


		List<AudioClip> retList = new List<AudioClip>();

		//try
	//	{
			string voiceOverMainFolder = "Localisation_Files/"+System.Enum.GetName(typeof(LanguageCode),LocalisationMang.langCode)+"/Dialogue/";
			string voiceOverMetaDataFilePath = voiceOverMainFolder + "VoiceOverMap";

			if(metaObj == null)
			{
				TextAsset ta = (TextAsset) Resources.Load(voiceOverMetaDataFilePath,typeof(TextAsset));
				string jsonStr = ta.text;
				metaObj = JsonHelper.deserialiseObject<VoiceOverLookupObject>(jsonStr);
			}

			string valueStr = metaObj.getValue(para_reqParamStr);

			string[] splitDataArr1 = valueStr.Split(',');
			string additionalFilePrefix = splitDataArr1[0];

			string[] splitDataArr2 = splitDataArr1[1].Split('*');
			int[] fileIDs = new int[splitDataArr2.Length];
			for(int i=0; i<splitDataArr2.Length; i++)
			{
				string tmpStr = splitDataArr2[i];
				if(tmpStr != null)
				{
					tmpStr = tmpStr.Trim();
					if(tmpStr != "")
					{
						fileIDs[i] = int.Parse(tmpStr);
					}
				}
			}

			string[] searchKeyParts = para_reqParamStr.Split('*');

		string genderStr = "Male";
		if( ! para_maleSetting) { genderStr = "Female"; }
		
		string pathToRequiredFolder = voiceOverMainFolder + genderStr+"/";



			pathToRequiredFolder += (searchKeyParts[0] + "/");
			/*for(int i=0; i<searchKeyParts.Length; i++)
			{
				pathToRequiredFolder += (searchKeyParts[i] + "/");
			}*/


			for(int i=0; i<fileIDs.Length; i++)
			{
				int reqID = fileIDs[i];
				string reqIDStr = ""+reqID;
				if(reqID < 10) { reqIDStr = "0"+reqIDStr; }



			string reqFileFullPath;
			if(LocalisationMang.langCode == LanguageCode.EN){
				reqFileFullPath = pathToRequiredFolder + genderStr +"_"+additionalFilePrefix+"_"+reqIDStr;

			}else{

				if(para_maleSetting){
					reqFileFullPath = pathToRequiredFolder +additionalFilePrefix +"_D-"+reqIDStr;

				}else{
					reqFileFullPath = pathToRequiredFolder +additionalFilePrefix +"_L-"+reqIDStr;

				}
			}

				AudioClip nwClip = Resources.Load<AudioClip>(reqFileFullPath);
				retList.Add(nwClip);
			}
	//	}
	//	catch(System.Exception ex)
	//	{
	//		Debug.Log(ex.StackTrace);
	//	}



		return retList;
	}
}
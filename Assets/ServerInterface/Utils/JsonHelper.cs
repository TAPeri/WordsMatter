/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using LitJson;
using System.Collections.Generic;

// Helper class which invokes the chosen JSON library.
public class JsonHelper
{

	public static string serialiseObject<T>(T para_objToSerialise)
	{
		string reqStr = JsonMapper.ToJson(para_objToSerialise);
		return reqStr;
	}
	
	public static T deserialiseObject<T>(string para_jsonStr)
	{
		T retObj = JsonMapper.ToObject<T>(para_jsonStr);
		return retObj;
	}
}
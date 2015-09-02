/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;



public class ObjectSerializerHelper
{


	public static string serialiseObjToString<T>(T para_obj)
	{
		using (MemoryStream stream = new MemoryStream())
		{
			BinaryFormatter binForm = new BinaryFormatter();
			binForm.Serialize(stream, para_obj);
			stream.Flush();
			stream.Position = 0;
			return System.Convert.ToBase64String(stream.ToArray());
		}
	}

	public static T deserialiseObjFromString<T>(string para_obj)
	{
		byte[] b = System.Convert.FromBase64String(para_obj);
		using (MemoryStream stream = new MemoryStream(b))
		{
			BinaryFormatter binForm = new BinaryFormatter();
			stream.Seek(0, SeekOrigin.Begin);
			return (T)binForm.Deserialize(stream);
		}
	}


	/*public static string deserialiseObjFromString<T>(string para_serStr)
	{
		BinaryFormatter binForm = new BinaryFormatter();
		StringReader sr = new StringReader();
		return (T) binForm.Deserialize(sr);
	}*/
	

	/*public static string serialiseObjToString<T>(T para_obj)
	{
		if(!para_obj.GetType().IsSerializable)
		{
			return null;
		}
		
		using(MemoryStream stream = new MemoryStream())
		{
			new BinaryFormatter().Serialize(stream, para_obj);
			return  Convert.ToBase64String(stream.ToArray());
		}
	}

	public static T DeserializeObject<T>(string para_str)
	{
		byte[] bytes = Convert.FromBase64String(para_str);

		System.Object retObj = null;
		using (MemoryStream stream = new MemoryStream(bytes))
		{
			retObj = (new BinaryFormatter().Deserialize(stream));
		}
		return retObj;
	}*/


	public static void serialiseObjToFile<T>(string para_fullFilePath, T para_obj)
	{
		BinaryFormatter binForm = new BinaryFormatter();
		Stream outStrm = new FileStream(para_fullFilePath,FileMode.Create,FileAccess.Write);
		binForm.Serialize(outStrm,para_obj);
		outStrm.Close();
		outStrm.Dispose();
	}

	public static System.Object deserialiseObjFromFile(string para_fullFilePath)
	{
		BinaryFormatter binForm = new BinaryFormatter();
		Stream inStrm = new FileStream(para_fullFilePath,FileMode.Open,FileAccess.Read);
		System.Object reqObj = binForm.Deserialize(inStrm);
		return reqObj;
	}

}

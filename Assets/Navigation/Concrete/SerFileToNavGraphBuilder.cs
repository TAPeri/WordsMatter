/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

public class SerFileToNavGraphBuilder : INavGraphBuilder
{
	string filePath;

	public SerFileToNavGraphBuilder(string para_filePath)
	{
		filePath = para_filePath;
	}

	public NavGraph constructGraph()
	{
		NavGraph retGraph = null;

		System.Object fileObj = ObjectSerializerHelper.deserialiseObjFromFile(filePath);
		if(fileObj != null)
		{
			retGraph = (BasicNavGraph) fileObj;
		}

		return retGraph;
	}
}

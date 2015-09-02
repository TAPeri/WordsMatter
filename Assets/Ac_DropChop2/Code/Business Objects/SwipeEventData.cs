/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class SwipeEventData : EventData
{
	public string masterWordGObjName;
	public string splitDetectorObjName;

	public SwipeEventData(string para_masterWordGObjName, string para_splitDetectorObjName)
	{
		masterWordGObjName = para_masterWordGObjName;
		splitDetectorObjName = para_splitDetectorObjName;
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class DialogReturnData
{
	int person1NpcID;
	string person1Name;
	string latestSequenceName;

	public DialogReturnData(int para_person1NpcID, string para_person1Name, string para_latestSequenceName)
	{
		person1NpcID = para_person1NpcID;
		person1Name = para_person1Name;
		latestSequenceName = para_latestSequenceName;
	}

	public int getPerson1NpcId() { return person1NpcID; }
	public string getPerson1Name() { return person1Name; }
	public string getLatestSequenceName() { return latestSequenceName; }
}

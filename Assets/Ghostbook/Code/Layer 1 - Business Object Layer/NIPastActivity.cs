/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

[System.Serializable]
public class NIPastActivity : NewsItem
{
	ApplicationID acPlayedCorrect = ApplicationID.APP_ID_NOT_SET_UP;
	ActivityID acPlayed;//deprecated
	int langArea;
	int difficulty;
	int questGiverID;
	string level;
	string date;

	/*public NIPastActivity(int para_newsID, int para_newsType, string para_dateTimestamp,
	                      ActivityID para_acPlayed, int para_langArea, int para_difficulty, int para_questGiverID,string level,string date)
		:base(para_newsID,para_newsType,para_dateTimestamp,"")
	{
		
		UnityEngine.Debug.LogError("Deprecated");
		
		acPlayed = para_acPlayed;
		langArea = para_langArea;
		difficulty = para_difficulty;
		questGiverID = para_questGiverID;
		newsText = null;
		level = level;
		date = date;
	}*/



	public NIPastActivity(int para_newsID, int para_newsType, string para_dateTimestamp,
									ApplicationID para_acPlayed, int para_langArea, int para_difficulty, int para_questGiverID,string level,string date)
		:base(para_newsID,para_newsType,para_dateTimestamp,"")
	{
		this.acPlayedCorrect = para_acPlayed;
		this.langArea = para_langArea;
		this.difficulty = para_difficulty;
		this.questGiverID = para_questGiverID;
		this.newsText = null;
		this.level = level;
		this.date = date;
	}






	public ApplicationID getActivityPlayed() { if(acPlayedCorrect==ApplicationID.APP_ID_NOT_SET_UP){ UnityEngine.Debug.LogError("Deprecated: old log"); return ApplicationID.MAIL_SORTER; }else return acPlayedCorrect; }
	public int getLangArea() { return langArea; }
	public int getDifficulty() { return difficulty; }
	public int getQuestGiverID() { return questGiverID; }
	public string getLevel(){return level;}
	public string getDate(){return date;}

	private string generateNewsText()
	{
		string retNewsText = "";

		GhostbookManagerLight gbMang = GhostbookManagerLight.getInstance();


		//int acPKey = acRefMat.getAcPKey_byAcIDEnum(acPlayed);
		//string acName = acRefMat.getActivityName(acPKey);

		//retNewsText += "----- News Item "+newsID+": "+dateTimestamp+" -----\n";
		retNewsText += "\t"+date+"\n";
		retNewsText += "\t"+LocalisationMang.translate("Quest giver")+": "+LocalisationMang.getNPCnames()[questGiverID]+"\n";
		retNewsText +=  "\t"+LocalisationMang.translate("Activity played")+": "+LocalisationMang.getActivityShorthand(acPlayedCorrect)+"\n";
		retNewsText +=  "\t"+LocalisationMang.translate("Language area")+": "+gbMang.getNameForLangArea(langArea)+"\n";
		retNewsText +=  "\t"+LocalisationMang.translate("Difficulty")+": "+gbMang.createDifficultyShortDescription(langArea,difficulty)+"\n";
		//retNewsText += "Level: "+level+"\n";

		return retNewsText;
	}

	public override string getNewsText()
	{
		if(newsText == null) { newsText = generateNewsText(); }
		return newsText;
	}
}
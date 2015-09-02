/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class ActivityResult
{
	// Remember: An activity session is made up of a list of levels which are shown to players.

	ActivitySessionMetaData acMetaData;
	List<ILevelConfig> presentedContentList;
	List<LevelOutcome> outcomeList;
	GameyResultData gameyData;
	
	public ApplicationID getAcID() { return acMetaData.getApplicationID(); }
	public ActivitySessionMetaData getAcMetaData()  { return acMetaData; }
	public List<ILevelConfig> getPresentedContent() { return presentedContentList; }
	public List<LevelOutcome> getOutcomeList() { return outcomeList; }
	public int getNumOfResultEntries() { return presentedContentList.Count; }
	public GameyResultData getGameyData() { return gameyData; }

	public void saveActivitySessionMetaData(ActivitySessionMetaData para_acMetaData, GameyResultData para_gData)
	{
		acMetaData = para_acMetaData;
		gameyData = para_gData;
	}

	public void addPresentedContent(ILevelConfig para_presentedConfig)
	{
		if(presentedContentList == null) { presentedContentList = new List<ILevelConfig>(); }
		presentedContentList.Add(para_presentedConfig);
	}

	public void addOutcomeEntry(LevelOutcome para_levelOutcome)
	{

		if(outcomeList == null) { outcomeList = new List<LevelOutcome>(); }
		outcomeList.Add(para_levelOutcome);
	}

	public List<ILevelConfig> getAllCorrectItems()
	{
		List<ILevelConfig> correctItems = new List<ILevelConfig>();

		if((presentedContentList != null)&&(outcomeList != null))
		{
			for(int i=0; i<presentedContentList.Count; i++)
			{
				if(i < outcomeList.Count)
				{
					LevelOutcome lo = outcomeList[i];
					if(lo.isPositiveOutcome())
					{
						correctItems.Add(presentedContentList[i]);
					}
				}
			}
		}

		return correctItems;
	}
}

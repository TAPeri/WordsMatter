/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ScoreBoardScript : MonoBehaviour, CustomActionListener, IActionNotifier
{
	int currStarCount = 0;
	int currWrongCount = 0;
	int maxWrong = 3;

	Color[] colorLevels = new Color[] { Color.white, Color.green, Color.red };
	int currStarLevel = 0;
	int starsPerLevel = 5;

	GameObject latestUpdatedStar;
	GameObject latestUpdatedCross;


	public void addStar()
	{

		Transform scoreBoard = GameObject.Find("Scoreboard").transform;

		int starID = (currStarCount%starsPerLevel);
		Transform reqStar = scoreBoard.FindChild("Star-"+starID);
		if(reqStar != null)
		{
			Vector3 tmpStarScale = reqStar.localScale;
			tmpStarScale.x = 5;
			tmpStarScale.y = 5;
			tmpStarScale.z = 1;
			reqStar.localScale = tmpStarScale;

			currStarLevel = (currStarCount / starsPerLevel);
			Color reqStarColor = colorLevels[(currStarLevel % colorLevels.Length)];
			reqStar.GetComponent<SpriteRenderer>().color = reqStarColor;
			reqStar.GetComponent<SpriteRenderer>().sortingOrder++;

			latestUpdatedStar = reqStar.gameObject;

			CustomAnimationManager aniMang = reqStar.gameObject.AddComponent<CustomAnimationManager>();
			List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
			List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
			batch1.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>() { 1f } ));
			List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
			batch2.Add(new AniCommandPrep("GrowOrShrink",1,new List<System.Object>() { new float[3] { 1,1,1 }, 3f }));
			List<AniCommandPrep> batch3 = new List<AniCommandPrep>();
			batch3.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>() { 0.5f } ));

			/*batch1.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>() { 3f } ));
			List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
			batch2.Add(new AniCommandPrep("InjectLocalScale",1,new List<System.Object>() { new float[3] {1f, 1f, 1f}}));*/
			//batch2.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] {1,1,1,1}, 0.1f }));
			batchLists.Add(batch1);
			batchLists.Add(batch2);
			batchLists.Add(batch3);
			aniMang.registerListener("Scoreboard",this);
			aniMang.init("NewStarAni",batchLists);

			currStarCount++;
		}
	}

	public void addWrongCross()
	{
		if(currWrongCount < maxWrong)
		{
			Transform scoreBoard = GameObject.Find("Scoreboard").transform;

			Transform reqCross = scoreBoard.FindChild("BadCross-"+currWrongCount);
			if(reqCross != null)
			{
				Vector3 tmpStarScale = reqCross.localScale;
				tmpStarScale.x = 2f;
				tmpStarScale.y = 2f;
				tmpStarScale.z = 1;
				reqCross.localScale = tmpStarScale;

				reqCross.GetComponent<SpriteRenderer>().color = Color.red;
				reqCross.GetComponent<SpriteRenderer>().sortingOrder++;

				latestUpdatedCross = reqCross.gameObject;
								
				CustomAnimationManager aniMang = reqCross.gameObject.AddComponent<CustomAnimationManager>();
				List<List<AniCommandPrep>> batchLists = new List<List<AniCommandPrep>>();
				List<AniCommandPrep> batch1 = new List<AniCommandPrep>();
				batch1.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>() { 0.5f } ));
				List<AniCommandPrep> batch2 = new List<AniCommandPrep>();
				batch2.Add(new AniCommandPrep("GrowOrShrink",1,new List<System.Object>() { new float[3] { 1,1,1 }, 3f }));
				List<AniCommandPrep> batch3 = new List<AniCommandPrep>();
				batch3.Add(new AniCommandPrep("DelayForInterval",1,new List<System.Object>() { 0.5f } ));

				//batch2.Add(new AniCommandPrep("ColorTransition",1,new List<System.Object>() { new float[4] {1,1,1,1}, 0.1f }));
				batchLists.Add(batch1);
				batchLists.Add(batch2);
				batchLists.Add(batch3);
				aniMang.registerListener("Scoreboard",this);
				aniMang.init("NewCrossAni",batchLists);

				currWrongCount++;
			}
		}
	}

	public void reset()
	{
		resetStars();
		resetCrosses();
	}

	private void resetStars()
	{
		currStarCount = 0;

		Transform scoreBoard = GameObject.Find("Scoreboard").transform;

		Color plainColor = ColorUtils.convertColor(21,21,21,47);

		int tmpCounter = 0;
		bool foundStar = true;
		while(foundStar)
		{
			Transform tmpStar = scoreBoard.FindChild("Star-"+tmpCounter);
			if(tmpStar == null)
			{
				foundStar = false;
			}
			else
			{
				tmpStar.GetComponent<SpriteRenderer>().color = plainColor;
			}
			
			tmpCounter++;
		}
	}

	public void resetCrosses()
	{
		currWrongCount = 0;

		Transform scoreBoard = GameObject.Find("Scoreboard").transform;
		
		Color plainColor = ColorUtils.convertColor(21,21,21,47);

		int tmpCounter = 0;
		bool foundCross = true;
		while(foundCross)
		{
			Transform tmpCross = scoreBoard.FindChild("BadCross-"+tmpCounter);
			if(tmpCross == null)
			{
				foundCross = false;
			}
			else
			{
				tmpCross.GetComponent<SpriteRenderer>().color = plainColor;
			}
			
			tmpCounter++;
		}
	}


	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "NewStarAni")
		{
			latestUpdatedStar.GetComponent<SpriteRenderer>().sortingOrder--;
			notifyAllListeners("Scoreboard","ScoreboardUpdate",null);
		}
		else if(para_eventID == "NewCrossAni")
		{
			latestUpdatedCross.GetComponent<SpriteRenderer>().sortingOrder--;
		}
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

[System.Serializable]
public class PhotoAlbum
{
	public int ownerID;
	public List<PhotoPage> availablePages;
	
	public PhotoAlbum(int para_ownerID, List<DifficultyMetaData> para_associatedDifficulties)
	{
		ownerID = para_ownerID;
		availablePages = new List<PhotoPage>();
		
		if(para_associatedDifficulties != null)
		{
			for(int i=0; i<para_associatedDifficulties.Count; i++)
			{
				DifficultyMetaData dmd = para_associatedDifficulties[i];
				if(dmd != null)
				{
					availablePages.Add(new PhotoPage(dmd.getName(),dmd.getLangAreaIDDiffIndexCombo(),new Dictionary<int,Photo>(),dmd.getExplanation()));
				}
			}
		}


	}
	
	public List<PhotoPage> getAllAvailablePages()
	{
		return availablePages;
	}
	
	public PhotoPage findSpecificPage(int para_langAreaID, int para_difficultyIndex)
	{
		PhotoPage reqPage = null;
		//UnityEngine.Debug.Log("Searching... "+para_langAreaID+"*"+para_difficultyIndex);

		string queryStr = ""+para_langAreaID+"*"+para_difficultyIndex;
		for(int i=0; i<availablePages.Count; i++)
		{
			PhotoPage tmpPage = availablePages[i];
			//UnityEngine.Debug.Log("Owner: "+ownerID+" has "+tmpPage.getLangAreaDifficultyComboKey());

			string tmpPageLADiffKey = tmpPage.getLangAreaDifficultyComboKey();
			
			if(tmpPageLADiffKey == queryStr)
			{
				reqPage = tmpPage;
				break;
			}
		}
		
		return reqPage;
	}
	
	public int getOwnerID() { return ownerID; }

	public float getNormalisedPercentageCompletion()
	{
		float percCompletion = 0;

		if(availablePages != null)
		{
			for(int i=0; i<availablePages.Count; i++)
			{
				percCompletion += availablePages[i].getNumAvailablePhotos();
			}

			percCompletion = percCompletion / ((4 * availablePages.Count) * 1.0f);

			if(availablePages.Count == 0)
			{
				percCompletion = 1;
			}
			else if(percCompletion == float.NaN)
			{
				percCompletion = 1;
			}
		}

		return percCompletion;
	}
}
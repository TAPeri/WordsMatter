/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class MPResultViewer : ActivityResultViewer
{

	ApplicationID acPKey;
	bool[] pageInitFlags;


	new void Start()
	{
		base.Start();

	}
	
	new void OnGUI()
	{
		base.OnGUI();

		if(currPage == 0)
		{
			GUI.color = Color.black;
			GUI.Label(uiBounds["FieldTitle-0"],textContent["FieldTitle-0"],availableGUIStyles["FieldTitle"]);
			GUI.Label(uiBounds["FieldTitle-1"],textContent["FieldTitle-1"],availableGUIStyles["FieldTitle"]);
			GUI.Label(uiBounds["FieldTitle-2"],textContent["FieldTitle-2"],availableGUIStyles["FieldTitle"]);
			GUI.Label(uiBounds["FieldTitle-3"],textContent["FieldTitle-3"],availableGUIStyles["FieldTitle"]);
			GUI.color = Color.green;
			GUI.Label(uiBounds["FieldContent-0"],textContent["FieldContent-0"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.red;
			GUI.Label(uiBounds["FieldContent-1"],textContent["FieldContent-1"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.black;
			GUI.Label(uiBounds["FieldContent-2"],textContent["FieldContent-2"],availableGUIStyles["FieldContent"]);
			GUI.Label(uiBounds["FieldContent-3"],textContent["FieldContent-3"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.white;
		}
	}


	public override void buildPage(int para_pageID, GameObject para_pageParent)
	{
		if(pageInitFlags == null) { pageInitFlags = new bool[]{false,false}; }
		
		if(para_pageID == 0)
		{
			string reqPageObjName = "ResultScreens_A";
			Transform reqResultScreenPage = transform.FindChild(reqPageObjName);
			Transform subPageTemplate = reqResultScreenPage.FindChild("Pages").FindChild("Ac"+acPKey);
			subPageTemplate.gameObject.SetActive(true);
			
			if(pageInitFlags[0] == false)
			{
				// Load necessary items.
				string fullPathToTemplate = reqPageObjName + "*" + "Pages" + "*" + ("Ac"+acPKey);
				
				
				// Init Items.
				MPGameyResultData gameyData = (MPGameyResultData) acResultData.getGameyData();

				int numCompletePaths = gameyData.getNumCompletePaths();
				int numMissteps = gameyData.getNumMissteps();
				int numRotations = gameyData.getRotations();
				string timeStr = gameyData.getTimeString();
				
				
				string[] elementNames   = {"FieldTitle-0","FieldTitle-1","FieldTitle-2","FieldTitle-3","FieldContent-0","FieldContent-1","FieldContent-2","FieldContent-3"};
				string[] elementContent = {LocalisationMang.translate("Paths completed"),LocalisationMang.translate("Tile missteps"),LocalisationMang.translate("Rotations"),LocalisationMang.translate("Time"),""+numCompletePaths,""+numMissteps,""+numRotations,timeStr};
				bool[] destroyGuideArr = {false,false,false,false,false,false,false,false};
				int[] textElementTypeArr = {0,0,0,0,0,0,0,0};
				prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,fullPathToTemplate);
				
				pageInitFlags[0] = true;
			}
		}
		else if(para_pageID == 1)
		{
			// MP does not have a second page with teacher info. Go directly to the last page (If next button pressed) or to the first page (If previous button pressed).
			// See page redirector below.
		}
	}

	public override int pageRedirector(int para_fromPage, int para_toPage)
	{
		// MP does not have a second page with teacher info. Go directly to the last page (If next button pressed) or to the first page (If previous button pressed).
		if(para_toPage == 1)
		{
			if(para_fromPage == 0) { return 2; }
			else { return 0; }
		}

		return para_toPage;
	}

	public override void init(ActivityResult para_acResultData)
	{
		base.init(para_acResultData);
		
		//GhostbookManager gbMang = GhostbookManager.getInstance();
		//IGBActivityReference acRef = gbMang.getActivityReferenceMaterial();
		acPKey = acResultData.getAcID();
	}	
}

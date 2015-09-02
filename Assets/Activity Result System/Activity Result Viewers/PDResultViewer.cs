/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class PDResultViewer : ActivityResultViewer
{
	
	ApplicationID acPKey;
	bool[] pageInitFlags;

	PDResTable resTable;

	float columnEqualGuiWidth;
	Vector2 tableScrollPos;
	Vector2 tmpVect;

	
	new void Start()
	{
		base.Start();

		tableScrollPos = new Vector2();
		tmpVect = new Vector2();		
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
			GUI.color = Color.black;
			GUI.Label(uiBounds["FieldContent-0"],textContent["FieldContent-0"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.green;
			GUI.Label(uiBounds["FieldContent-1"],textContent["FieldContent-1"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.red;
			GUI.Label(uiBounds["FieldContent-2"],textContent["FieldContent-2"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.black;
			GUI.Label(uiBounds["FieldContent-3"],textContent["FieldContent-3"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.white;
		}
		else if(currPage == 1)
		{
			// Render table
			
			if(resTable != null)
			{
				
				
				if((Application.platform == RuntimePlatform.Android)
				   ||(Application.platform == RuntimePlatform.IPhonePlayer))
				{
					if(Input.touchCount == 1)
					{
						tmpVect.x = Input.touches[0].position.x;
						tmpVect.y = Screen.height - Input.touches[0].position.y;
						
						if(uiBounds["TableScrollArea"].Contains(tmpVect))
						{
							tableScrollPos.x += (Input.touches[0].deltaPosition.x * 1f);
							tableScrollPos.y += (Input.touches[0].deltaPosition.y * 1f);
						}
					}
				}
				
				
				
				
				GUILayout.BeginArea(uiBounds["TableScrollArea"]);
				tableScrollPos = GUILayout.BeginScrollView(tableScrollPos,GUIStyle.none,GUI.skin.verticalScrollbar);
				
				// Headers.
				GUILayout.BeginHorizontal();
				GUI.color = Color.black;
				for(int i=0; i<resTable.headerNames.Count; i++)
				{
					GUILayout.Label(resTable.headerNames[i],availableGUIStyles["FieldTitle"],GUILayout.Width(columnEqualGuiWidth));
				}
				GUI.color = Color.white;
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Box("",availableGUIStyles["SeparatorBox"],GUILayout.Height(10));
				GUILayout.EndHorizontal();
				
				// Data rows.
				for(int i=0; i<resTable.rows.Count; i++)
				{
					PDResRow currRow = resTable.rows[i];
					if(currRow != null)
					{
						GUILayout.BeginHorizontal();
						for(int k=0; k<currRow.rowContent.Count; k++)
						{
							PDResCell currCell = currRow.rowContent[k];
							string cellText = "";
							if(currCell != null)
							{
								cellText = currCell.cellContent;
								if((k == 0)||(k == 1))
								{
									GUI.color = Color.black;
									GUILayout.Label(cellText,availableGUIStyles["FieldContent"],GUILayout.Width(columnEqualGuiWidth));
								}
								else if(k == 2)
								{
									//GUI.color = Color.red;
									GUILayout.Button(LocalisationMang.translate("Revise"),availableGUIStyles["ReviseButton"],GUILayout.Width(columnEqualGuiWidth));
								}
								
							}
							else
							{
								GUI.color = Color.black;
								GUILayout.Label("",availableGUIStyles["FieldContent"],GUILayout.Width(columnEqualGuiWidth));
							}
							GUI.color = Color.white;
						}
						GUILayout.EndHorizontal();
					}
				}
				
				GUILayout.EndScrollView();
				GUILayout.EndArea();
			}
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
				PDGameyResultData gameyData = (PDGameyResultData) acResultData.getGameyData();

				int numPackagesLostToMonkies = gameyData.getNumPackagesLostToMonkies();
				int numCorrectDeliveries = gameyData.getNumCorrectDeliveries();
				int numWrongDeliveries = gameyData.getNumWrongDeliveries();
				string timeStr = gameyData.getTimeString();
				
				
				string[] elementNames   = {"FieldTitle-0","FieldTitle-1","FieldTitle-2","FieldTitle-3","FieldContent-0","FieldContent-1","FieldContent-2","FieldContent-3"};
				string[] elementContent = {LocalisationMang.translate("Packages stolen"),LocalisationMang.translate("Correct deliveries"),LocalisationMang.translate("Wrong deliveries"),LocalisationMang.translate("Time"),""+numPackagesLostToMonkies,""+numCorrectDeliveries,""+numWrongDeliveries,timeStr};
				bool[] destroyGuideArr = {false,false,false,false,false,false,false,false};
				int[] textElementTypeArr = {0,0,0,0,0,0,0,0};
				prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,fullPathToTemplate);
				
				pageInitFlags[0] = true;
			}
		}
		else if(para_pageID == 1)
		{
			string reqPageObjName = "ResultScreens_B";
			Transform reqResultScreenPage = transform.FindChild(reqPageObjName);
			Transform subPageTemplate = reqResultScreenPage.FindChild("Pages").FindChild("Ac"+acPKey);
			subPageTemplate.gameObject.SetActive(true);
			
			if(pageInitFlags[1] == false)
			{
				// Load necessary items.
				string fullPathToTemplate = reqPageObjName + "*" + "Pages" + "*" + ("Ac"+acPKey);
				
				
				
				
				// Init Items.
				resTable = new PDResTable();
				
				
				
				List<PDResColumn> columnData = new List<PDResColumn>();
				columnData.Add(new PDResColumn());
				columnData.Add(new PDResColumn());
				columnData.Add(new PDResColumn());
				
				
				List<ILevelConfig> presentedContent = acResultData.getPresentedContent();
				List<LevelOutcome> outcomeList = acResultData.getOutcomeList();
				for(int i=0; i<presentedContent.Count; i++)
				{
					PDLevelConfig tmpContent = (PDLevelConfig) presentedContent[i];
					PDLevelOutcome tmpOutcome = (PDLevelOutcome) outcomeList[i];
					
					
					if(i == 0)
					{
						// Add headers.
						resTable.addHeaderName(LocalisationMang.translate("Item"));
						                       resTable.addHeaderName(LocalisationMang.translate("Knocks"));
						resTable.addHeaderName("");
					}

					for(int j = 0; j<tmpContent.getParcelWord().Length;j++){

						columnData[0].addCell(new PDResCell(tmpContent.getParcelWord()[j]));
						columnData[1].addCell(new PDResCell(""+tmpContent.getCorrectNumOfKnocks()[j]));

						if(tmpOutcome.isPositiveOutcome()){

							columnData[2].addCell(null);

						}
						else if(j<tmpOutcome.getNumCorrectWords())
						{
							columnData[2].addCell(null);
						}
						else
						{
							columnData[2].addCell(new PDResCell(""));
						}

					}
				}
				
				
				// Convert columns to rows.
				bool canMakeMoreRows = true;
				int dataRowIndex = 0;
				while(canMakeMoreRows)
				{
					canMakeMoreRows = false;
					
					List<PDResCell> nwRowData = new List<PDResCell>();
					for(int i=0; i<columnData.Count; i++)
					{
						List<PDResCell> columnContent = columnData[i].columnContent;
						if((columnContent != null)&&(dataRowIndex < (columnContent.Count)))
						{
							nwRowData.Add(columnContent[dataRowIndex]);
							canMakeMoreRows = true;
						}
						else
						{
							nwRowData.Add(null);
						}
					}
					
					if(canMakeMoreRows)
					{
						PDResRow nwRow = new PDResRow(nwRowData);
						resTable.addRow(nwRow);
					}
					
					dataRowIndex++;
				}
				
				
				
				
				string[] elementNames   = {"TableScrollArea"};
				string[] elementContent = {"Table Scroll Area"};
				bool[] destroyGuideArr = {true};
				int[] textElementTypeArr = {0};
				prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,fullPathToTemplate);
				
				
				columnEqualGuiWidth = ((uiBounds["TableScrollArea"].width * 0.9f) / (3 * 1.0f));
				float minimumColumnWidth = (uiBounds["TableScrollArea"].width * 0.30f);
				if(columnEqualGuiWidth <= minimumColumnWidth)
				{
					columnEqualGuiWidth = minimumColumnWidth;
				}
				
				
				pageInitFlags[1] = true;
			}
		}
	}
	
	public override void init(ActivityResult para_acResultData)
	{
		base.init(para_acResultData);
		
		//GhostbookManager gbMang = GhostbookManager.getInstance();
		//IGBActivityReference acRef = gbMang.getActivityReferenceMaterial();
		acPKey = acResultData.getAcID();
	}





	class PDResTable
	{
		public List<string> headerNames;
		public List<PDResRow> rows;
		
		public PDResTable()
		{
			rows = new List<PDResRow>();
			headerNames = new List<string>();
		}
		
		public void addHeaderName(string para_hName)
		{
			if(headerNames == null) { headerNames = new List<string>(); }
			headerNames.Add(para_hName);
		}
		
		public void addRow(PDResRow para_nwRow)
		{
			if(rows == null) { rows = new List<PDResRow>(); }
			rows.Add(para_nwRow);
		}
	}
	
	class PDResRow
	{
		public List<PDResCell> rowContent;
		
		public PDResRow()
		{
			rowContent = new List<PDResCell>();
		}
		
		public PDResRow(List<PDResCell> para_cells)
		{
			rowContent = para_cells;
		}
		
		public void addCell(PDResCell para_nwCell)
		{
			if(rowContent == null) { rowContent = new List<PDResCell>(); }
			rowContent.Add(para_nwCell);
		}
	}
	
	class PDResColumn
	{
		public List<PDResCell> columnContent;
		
		public PDResColumn()
		{
			columnContent = new List<PDResCell>();
		}
		
		public void addCell(PDResCell para_nwCell)
		{
			if(columnContent == null) { columnContent = new List<PDResCell>(); }
			columnContent.Add(para_nwCell);
		}
	}
	
	class PDResCell
	{
		public string cellContent;
		
		public PDResCell(string para_cellContent)
		{
			cellContent = para_cellContent;
		}
	}


}

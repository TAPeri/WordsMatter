/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class SJResultViewer : ActivityResultViewer
{
	
	ApplicationID acPKey;
	bool[] pageInitFlags;

	Color darkGreen = ColorUtils.convertColor(0,200,0);

	SJResTable resTable;

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
			GUI.Label(uiBounds["FieldContent-0"],textContent["FieldContent-0"],availableGUIStyles["FieldContent"]);
			GUI.color = darkGreen;
			GUI.Label(uiBounds["FieldContent-1"],textContent["FieldContent-1"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.red;
			GUI.Label(uiBounds["FieldContent-2"],textContent["FieldContent-2"],availableGUIStyles["FieldContent"]);
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
					SJResRow currRow = resTable.rows[i];
					if(currRow != null)
					{
						GUILayout.BeginHorizontal();
						for(int k=0; k<currRow.rowContent.Count; k++)
						{
							SJResCell currCell = currRow.rowContent[k];
							string cellText = "";
							if(currCell != null)
							{
								cellText = currCell.cellContent;
								if(k == 0)
								{
									GUI.color = Color.black;
									GUILayout.Label(cellText,availableGUIStyles["FieldContent"],GUILayout.Width(columnEqualGuiWidth));
								}
								else
								{
									//GUI.color = Color.red;
									GUILayout.Button(cellText,availableGUIStyles["ReviseButton"],GUILayout.Width(columnEqualGuiWidth));
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
				SJGameyResultData gameyData = (SJGameyResultData) acResultData.getGameyData();
				
				int numLinesCompleted = gameyData.numOfLines;
				int numCorrectSplits = gameyData.numCorrectSplits;
				int numIncorrectSplits = gameyData.numWrongSplits;
				
				string[] elementNames   = {"FieldTitle-0","FieldTitle-1","FieldTitle-2","FieldContent-0","FieldContent-1","FieldContent-2"};
				string[] elementContent = {LocalisationMang.translate("Lines completed"),LocalisationMang.translate("Correct splits"),LocalisationMang.translate("Incorrect splits"),""+numLinesCompleted,""+numCorrectSplits,""+numIncorrectSplits};
				bool[] destroyGuideArr = {false,false,false,false,false,false};
				int[] textElementTypeArr = {0,0,0,0,0,0};
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
				resTable = new SJResTable();
				
				
				
				List<SJResColumn> columnData = new List<SJResColumn>();
				columnData.Add(new SJResColumn());
				columnData.Add(new SJResColumn());
				
				
				List<ILevelConfig> presentedContent = acResultData.getPresentedContent();
				List<LevelOutcome> outcomeList = acResultData.getOutcomeList();
				for(int i=0; i<presentedContent.Count; i++)
				{
					SJLevelConfig tmpContent = (SJLevelConfig) presentedContent[i];
					SJLevelOutcome tmpOutcome = (SJLevelOutcome) outcomeList[i];

					List<int> correctSplitPositions = new List<int>(tmpContent.getWordItem().getSyllSplitPositions());

					string presentedWord = tmpContent.getWordItem().getWordAsString();
					string presentedWordFancy = tmpContent.createFormattedSplitString(presentedWord,correctSplitPositions);

					
					if(i == 0)
					{
						// Add headers.
						resTable.addHeaderName(LocalisationMang.translate("Success"));
						resTable.addHeaderName(LocalisationMang.translate("Revise"));
					}
					
					if(tmpOutcome.isPositiveOutcome())
					{
						columnData[0].addCell(new SJResCell(presentedWordFancy));
					}
					else
					{
						columnData[1].addCell(new SJResCell(presentedWordFancy));
					}
					
					
				}
				
				
				// Convert columns to rows.
				bool canMakeMoreRows = true;
				int dataRowIndex = 0;
				while(canMakeMoreRows)
				{
					canMakeMoreRows = false;
					
					List<SJResCell> nwRowData = new List<SJResCell>();
					for(int i=0; i<columnData.Count; i++)
					{
						List<SJResCell> columnContent = columnData[i].columnContent;
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
						SJResRow nwRow = new SJResRow(nwRowData);
						resTable.addRow(nwRow);
					}
					
					dataRowIndex++;
				}
				
				
				
				
				string[] elementNames   = {"TableScrollArea"};
				string[] elementContent = {"Table Scroll Area"};
				bool[] destroyGuideArr = {true};
				int[] textElementTypeArr = {0};
				prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,fullPathToTemplate);
				
				
				columnEqualGuiWidth = ((uiBounds["TableScrollArea"].width * 0.9f) / (2 * 1.0f));
				float minimumColumnWidth = (uiBounds["TableScrollArea"].width * 0.45f);
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
		

		acPKey = acResultData.getAcID();
	}






	class SJResTable
	{
		public List<string> headerNames;
		public List<SJResRow> rows;
		
		public SJResTable()
		{
			rows = new List<SJResRow>();
			headerNames = new List<string>();
		}
		
		public void addHeaderName(string para_hName)
		{
			if(headerNames == null) { headerNames = new List<string>(); }
			headerNames.Add(para_hName);
		}
		
		public void addRow(SJResRow para_nwRow)
		{
			if(rows == null) { rows = new List<SJResRow>(); }
			rows.Add(para_nwRow);
		}
	}
	
	class SJResRow
	{
		public List<SJResCell> rowContent;
		
		public SJResRow()
		{
			rowContent = new List<SJResCell>();
		}
		
		public SJResRow(List<SJResCell> para_cells)
		{
			rowContent = para_cells;
		}
		
		public void addCell(SJResCell para_nwCell)
		{
			if(rowContent == null) { rowContent = new List<SJResCell>(); }
			rowContent.Add(para_nwCell);
		}
	}
	
	class SJResColumn
	{
		public List<SJResCell> columnContent;
		
		public SJResColumn()
		{
			columnContent = new List<SJResCell>();
		}
		
		public void addCell(SJResCell para_nwCell)
		{
			if(columnContent == null) { columnContent = new List<SJResCell>(); }
			columnContent.Add(para_nwCell);
		}
	}
	
	class SJResCell
	{
		public string cellContent;
		
		public SJResCell(string para_cellContent)
		{
			cellContent = para_cellContent;
		}
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class SHResultViewer : ActivityResultViewer
{
	
	ApplicationID acPKey;
	bool[] pageInitFlags;

	float[] columnGUIWidths;

	SHResTable resTable;
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
			GUI.color = Color.green;
			GUI.Label(uiBounds["FieldContent-0"],textContent["FieldContent-0"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.red;
			GUI.Label(uiBounds["FieldContent-1"],textContent["FieldContent-1"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.black;
			GUI.Label(uiBounds["FieldContent-2"],textContent["FieldContent-2"],availableGUIStyles["FieldContent"]);
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
					GUILayout.Label(resTable.headerNames[i],availableGUIStyles["FieldTitle"],GUILayout.Width(columnGUIWidths[i]));
				}
				GUI.color = Color.white;
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Box("",availableGUIStyles["SeparatorBox"],GUILayout.Height(10));
				GUILayout.EndHorizontal();
				
				// Data rows.
				for(int i=0; i<resTable.rows.Count; i++)
				{
					SHResRow currRow = resTable.rows[i];
					if(currRow != null)
					{
						GUILayout.BeginHorizontal();
						for(int k=0; k<currRow.rowContent.Count; k++)
						{
							SHResCell currCell = currRow.rowContent[k];
							string cellText = "";
							if(currCell != null)
							{
								cellText = currCell.cellContent;
								if(k == 0)
								{
									GUI.color = Color.black;
									GUILayout.Label(cellText,availableGUIStyles["FieldContent"],GUILayout.Width(columnGUIWidths[0]));
								}
								else if(k == 1)
								{
									//GUI.color = Color.red;
									GUILayout.Button(LocalisationMang.translate("Revise"),availableGUIStyles["ReviseButton"],GUILayout.Width(columnGUIWidths[1]));
								}
								else
								{
									GUI.color = Color.black;
									GUILayout.Label("",availableGUIStyles["FieldContent"],GUILayout.Width(columnGUIWidths[k]));
								}
								
							}
							else
							{
								GUI.color = Color.black;
								GUILayout.Label("",availableGUIStyles["FieldContent"],GUILayout.Width(columnGUIWidths[k]));
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
				SHGameyResultData gameyData = (SHGameyResultData) acResultData.getGameyData();
				
				int numCorrectSentences = gameyData.getNumCorrectSentences();
				int numIncorrectTries = gameyData.getNumIncorrectTries();
				int finalMusPeepCount = gameyData.getFinalMusPeepCount();
				string timeStr = gameyData.getTimeString();
				
				
				string[] elementNames   = {"FieldTitle-0","FieldTitle-1","FieldTitle-2","FieldTitle-3","FieldContent-0","FieldContent-1","FieldContent-2","FieldContent-3"};
				string[] elementContent = {LocalisationMang.translate("Correct words"),LocalisationMang.translate("Incorrect tries"),LocalisationMang.translate("Final person count"),LocalisationMang.translate("Time"),""+numCorrectSentences,""+numIncorrectTries,""+finalMusPeepCount,timeStr};
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
				resTable = new SHResTable();
				
				
				
				List<SHResColumn> columnData = new List<SHResColumn>();
				columnData.Add(new SHResColumn());
				columnData.Add(new SHResColumn());
				
				
				List<ILevelConfig> presentedContent = acResultData.getPresentedContent();
				List<LevelOutcome> outcomeList = acResultData.getOutcomeList();
				for(int i=0; i<presentedContent.Count; i++)
				{
					SHeroLevelConfig tmpContent = (SHeroLevelConfig) presentedContent[i];
					SHLevelOutcome tmpOutcome = (SHLevelOutcome) outcomeList[i];
					
					if(i == 0)
					{
						// Add headers.
						resTable.addHeaderName(LocalisationMang.translate("Items"));
						resTable.addHeaderName("");
					}

					columnData[0].addCell(new SHResCell(tmpContent.getLevelSentence(false)));

					if(tmpOutcome.isPositiveOutcome())
					{
						columnData[1].addCell(null);
					}
					else
					{
						columnData[1].addCell(new SHResCellRevise());
					}
					
					
				}
				
				
				// Convert columns to rows.
				bool canMakeMoreRows = true;
				int dataRowIndex = 0;
				while(canMakeMoreRows)
				{
					canMakeMoreRows = false;
					
					List<SHResCell> nwRowData = new List<SHResCell>();
					for(int i=0; i<columnData.Count; i++)
					{
						List<SHResCell> columnContent = columnData[i].columnContent;
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
						SHResRow nwRow = new SHResRow(nwRowData);
						resTable.addRow(nwRow);
					}
					
					dataRowIndex++;
				}
				
				
				
				
				string[] elementNames   = {"TableScrollArea"};
				string[] elementContent = {"Table Scroll Area"};
				bool[] destroyGuideArr = {true};
				int[] textElementTypeArr = {0};
				prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,fullPathToTemplate);
				

				columnGUIWidths = new float[2];
				columnGUIWidths[0] = ((uiBounds["TableScrollArea"].width * 0.9f) * 0.8f);
				columnGUIWidths[1] = ((uiBounds["TableScrollArea"].width * 0.9f) * 0.2f);

				
				
				pageInitFlags[1] = true;
			}
		}
	}
	
	public override void init(ActivityResult para_acResultData)
	{
		base.init(para_acResultData);
		

		acPKey = acResultData.getAcID();
	}	





	class SHResTable
	{
		public List<string> headerNames;
		public List<SHResRow> rows;
		
		public SHResTable()
		{
			rows = new List<SHResRow>();
			headerNames = new List<string>();
		}
		
		public void addHeaderName(string para_hName)
		{
			if(headerNames == null) { headerNames = new List<string>(); }
			headerNames.Add(para_hName);
		}
		
		public void addRow(SHResRow para_nwRow)
		{
			if(rows == null) { rows = new List<SHResRow>(); }
			rows.Add(para_nwRow);
		}
	}
	
	class SHResRow
	{
		public List<SHResCell> rowContent;
		
		public SHResRow()
		{
			rowContent = new List<SHResCell>();
		}
		
		public SHResRow(List<SHResCell> para_cells)
		{
			rowContent = para_cells;
		}
		
		public void addCell(SHResCell para_nwCell)
		{
			if(rowContent == null) { rowContent = new List<SHResCell>(); }
			rowContent.Add(para_nwCell);
		}
	}
	
	class SHResColumn
	{
		public List<SHResCell> columnContent;
		
		public SHResColumn()
		{
			columnContent = new List<SHResCell>();
		}
		
		public void addCell(SHResCell para_nwCell)
		{
			if(columnContent == null) { columnContent = new List<SHResCell>(); }
			columnContent.Add(para_nwCell);
		}
	}
	
	class SHResCell
	{
		public string cellContent;
		
		public SHResCell(string para_cellContent)
		{
			cellContent = para_cellContent;
		}
	}

	class SHResCellRevise : SHResCell
	{
		public SHResCellRevise()
			:base("")
		{

		}
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class BBResultViewer : ActivityResultViewer
{
	
	ApplicationID acPKey;
	bool[] pageInitFlags;

	List<BBResTable> availableTables;

	float columnEqualGuiWidth;
	Vector2 tableScrollPos;
	Vector2 tmpVect;

	
	new void  Start()
	{
		base.Start();

		tableScrollPos = new Vector2();
		tmpVect = new Vector2();
	}
	
	new void  OnGUI()
	{
		base.OnGUI();

		if(currPage == 0)
		{
			GUI.color = Color.black;
			GUI.Label(uiBounds["FieldTitle-0"],textContent["FieldTitle-0"],availableGUIStyles["FieldTitle"]);
			GUI.Label(uiBounds["FieldTitle-1"],textContent["FieldTitle-1"],availableGUIStyles["FieldTitle"]);
			GUI.Label(uiBounds["FieldTitle-2"],textContent["FieldTitle-2"],availableGUIStyles["FieldTitle"]);
			GUI.color = Color.green;
			GUI.Label(uiBounds["FieldContent-0"],textContent["FieldContent-0"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.red;
			GUI.Label(uiBounds["FieldContent-1"],textContent["FieldContent-1"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.black;
			GUI.Label(uiBounds["FieldContent-2"],textContent["FieldContent-2"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.white;
		}
		else if(currPage == 1)
		{
			// Render all available tables.
			
			if(availableTables != null)
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
				
				
				for(int t=0; t<availableTables.Count; t++)
				{
					BBResTable resTable = availableTables[t];
					
					
					// Start render next table.
					
					if(resTable != null)
					{
						
						// Table title.
						GUILayout.BeginHorizontal();
						GUI.color = Color.black;
						GUILayout.Label(resTable.tableTitle,availableGUIStyles["FieldTitle"]);
						GUI.color = Color.white;
						GUILayout.EndHorizontal();
						
						
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
							BBResRow currRow = resTable.rows[i];
							if(currRow != null)
							{
								GUILayout.BeginHorizontal();
								for(int k=0; k<currRow.rowContent.Count; k++)
								{
									BBResCell currCell = currRow.rowContent[k];
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
						
					}
					
					// End render table procedure.
					
					
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
				BBGameyResultData gameyData = (BBGameyResultData) acResultData.getGameyData();

				int numCompleteCorrectBridges = gameyData.getNumOfCompleteCorrectBridges();
				int numBrokenBridges = gameyData.getNumBrokenBridges();
				string timeStr = gameyData.getTimeString();
				
				
				string[] elementNames   = {"FieldTitle-0","FieldTitle-1","FieldTitle-2","FieldContent-0","FieldContent-1","FieldContent-2"};
				string[] elementContent = {LocalisationMang.translate("Full correct bridges"),LocalisationMang.translate("Broken bridges"),LocalisationMang.translate("Time"),""+numCompleteCorrectBridges,""+numBrokenBridges,timeStr};
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
				
				availableTables = new List<BBResTable>();


				// Table to column lookup.
				Dictionary<string, List<BBResColumn>> tableLookup = new Dictionary<string, List<BBResColumn>>();
				
				
				List<ILevelConfig> presentedContent = acResultData.getPresentedContent();
				List<LevelOutcome> outcomeList = acResultData.getOutcomeList();
				
				for(int i=0; i<presentedContent.Count; i++)
				{
					BBLevelConfig tmpContent = (BBLevelConfig) presentedContent[i];
					BBLevelOutcome tmpOutcome = (BBLevelOutcome) outcomeList[i];
					
					string contentDescription = tmpContent.getDescription();
					List<BBResColumn> workbenchTableColumns = null;
					if(tableLookup.ContainsKey(contentDescription))
					{
						workbenchTableColumns = tableLookup[contentDescription];
					}
					else
					{
						workbenchTableColumns = new List<BBResColumn>();
						workbenchTableColumns.Add(new BBResColumn());
						workbenchTableColumns.Add(new BBResColumn());
						tableLookup.Add(contentDescription,workbenchTableColumns);
					}



					//List<IndexRegion> playerHighlights = tmpOutcome.getPlayerHighlights();

					string cellText = "";
					cellText = produceFancyText(tmpContent.getBridgeWord(),tmpContent.getCorrectHighlightAreas());
					if(tmpOutcome.isPositiveOutcome())
					{
						workbenchTableColumns[0].addCell(new BBResCell(cellText));
					}
					else
					{
						workbenchTableColumns[1].addCell(new BBResCell(cellText));
					}

					tableLookup[contentDescription] = workbenchTableColumns;					
				}

				// Compress all tables.
				List<string> lookupKeys = new List<string>(tableLookup.Keys);
				for(int i=0; i<lookupKeys.Count; i++)
				{
					List<BBResColumn> tmpTableColumns = tableLookup[lookupKeys[i]];

					BBResTable workbenchTable = new BBResTable(lookupKeys[i]);
					workbenchTable.addHeaderName(LocalisationMang.translate("Success"));
					workbenchTable.addHeaderName(LocalisationMang.translate("Revise"));

					// Convert columns to rows.
					bool canMakeMoreRows = true;
					int dataRowIndex = 0;
					while(canMakeMoreRows)
					{
						canMakeMoreRows = false;
						
						List<BBResCell> nwRowData = new List<BBResCell>();
						for(int k=0; k<tmpTableColumns.Count; k++)
						{
							List<BBResCell> columnContent = tmpTableColumns[k].columnContent;
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
							BBResRow nwRow = new BBResRow(nwRowData);
							workbenchTable.addRow(nwRow);
						}
						
						dataRowIndex++;
					}

					availableTables.Add(workbenchTable);
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
		
		//GhostbookManager gbMang = GhostbookManager.getInstance();
		//IGBActivityReference acRef = gbMang.getActivityReferenceMaterial();
		acPKey = acResultData.getAcID();
	}


	private string produceFancyText(string para_word, List<HighlightDesc> para_regions)
	{
		string fancyText = "";
		
		if((para_regions == null)||(para_regions.Count == 0))
		{
			fancyText = para_word;
		}
		else
		{
			int posIndex = 0;
			
			for(int i=0; i<para_regions.Count; i++)
			{
				IndexRegion currRegion = para_regions[i];
				
				while(posIndex < currRegion.startIndex)
				{
					fancyText += para_word[posIndex];
					posIndex++;
				}
				
				fancyText += "["; 
				while(posIndex <= currRegion.endIndex)
				{
					fancyText += para_word[posIndex];
					posIndex++;
				}
				fancyText += "]";
			}
			
			if(posIndex < para_word.Length)
			{
				while(posIndex < para_word.Length)
				{
					fancyText += para_word[posIndex];
					posIndex++;
				}
			}
		}
		
		return fancyText;
	}



	/*private string produceFancyText(string para_word, List<HighlightDesc> para_regions)
	{
		string fancyText = "";

		if((para_regions == null)||(para_regions.Count == 0))
		{
			fancyText = para_word;
		}
		else
		{
			int posIndex = 0;

			for(int i=0; i<para_regions.Count; i++)
			{
				IndexRegion currRegion = para_regions[i];

				while(posIndex < currRegion.startIndex)
				{
					fancyText += para_word[posIndex];
					posIndex++;
				}

				fancyText += "#DDFFDDFF"; // Green
				while(posIndex <= currRegion.endIndex)
				{
					fancyText += para_word[posIndex];
					posIndex++;
				}
				fancyText += "#!";
			}

			if(posIndex < para_word.Length)
			{
				fancyText += "#!";
				while(posIndex < para_word.Length)
				{
					fancyText += para_word[posIndex];
					posIndex++;
				}
			}

			fancyText += "#!";
		}

		return fancyText;
	}*/



	class BBResTable
	{
		public string tableTitle;
		public List<string> headerNames;
		public List<BBResRow> rows;
		
		public BBResTable(string para_tableTitle)
		{
			tableTitle = para_tableTitle;
			rows = new List<BBResRow>();
			headerNames = new List<string>();
		}
		
		public void addHeaderName(string para_hName)
		{
			if(headerNames == null) { headerNames = new List<string>(); }
			headerNames.Add(para_hName);
		}
		
		public void addRow(BBResRow para_nwRow)
		{
			if(rows == null) { rows = new List<BBResRow>(); }
			rows.Add(para_nwRow);
		}
	}
	
	class BBResRow
	{
		public List<BBResCell> rowContent;
		
		public BBResRow()
		{
			rowContent = new List<BBResCell>();
		}
		
		public BBResRow(List<BBResCell> para_cells)
		{
			rowContent = para_cells;
		}
		
		public void addCell(BBResCell para_nwCell)
		{
			if(rowContent == null) { rowContent = new List<BBResCell>(); }
			rowContent.Add(para_nwCell);
		}
	}
	
	class BBResColumn
	{
		public List<BBResCell> columnContent;
		
		public BBResColumn()
		{
			columnContent = new List<BBResCell>();
		}
		
		public void addCell(BBResCell para_nwCell)
		{
			if(columnContent == null) { columnContent = new List<BBResCell>(); }
			columnContent.Add(para_nwCell);
		}
	}
	
	class BBResCell
	{
		public string cellContent;
		
		public BBResCell(string para_cellContent)
		{
			cellContent = para_cellContent;
		}
	}
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class WAMResultViewer : ActivityResultViewer
{
	
	ApplicationID acPKey;
	bool[] pageInitFlags;

	List<WAMResTable> availableTables;

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
			GUI.color = Color.green;
			GUI.Label(uiBounds["FieldContent-0"],textContent["FieldContent-0"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.black;
			GUI.Label(uiBounds["FieldContent-1"],textContent["FieldContent-1"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.red;
			GUI.Label(uiBounds["FieldContent-2"],textContent["FieldContent-2"],availableGUIStyles["FieldContent"]);
			GUI.color = Color.black;
			GUI.Label(uiBounds["FieldContent-3"],textContent["FieldContent-3"],availableGUIStyles["FieldContent"]);
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
					WAMResTable resTable = availableTables[t];


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
							WAMResRow currRow = resTable.rows[i];
							if(currRow != null)
							{
								GUILayout.BeginHorizontal();
								for(int k=0; k<currRow.rowContent.Count; k++)
								{
									WAMResCell currCell = currRow.rowContent[k];
									string cellText = "";
									if(currCell != null)
									{
										cellText = currCell.cellContent;
									}

									GUI.color = Color.black;
									GUILayout.Label(cellText,availableGUIStyles["FieldContent"],GUILayout.Width(columnEqualGuiWidth));

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
				WAMGameyResultData gameyData = (WAMGameyResultData) acResultData.getGameyData();
				
				int numFedCorrectMonkies = gameyData.getNumFedCorrectMonkies();
				int numMissedCorrectMonkies = gameyData.getNumMissedCorrectMonkies();
				int numFedWrongMonkies = gameyData.getNumFedWrongMonkies();
				string timeStr = gameyData.getTimeString();

				
				string[] elementNames   = {"FieldTitle-0","FieldTitle-1","FieldTitle-2","FieldTitle-3","FieldContent-0","FieldContent-1","FieldContent-2","FieldContent-3"};
				string[] elementContent = {LocalisationMang.translate("Fed correct monkey"),LocalisationMang.translate("Missed correct monkey"),LocalisationMang.translate("Fed incorrect monkey"),LocalisationMang.translate("Time"),""+numFedCorrectMonkies,""+numMissedCorrectMonkies,""+numFedWrongMonkies,timeStr};
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

				availableTables = new List<WAMResTable>();



				List<ILevelConfig> presentedContent = acResultData.getPresentedContent();
				List<LevelOutcome> outcomeList = acResultData.getOutcomeList();

				for(int i=0; i<presentedContent.Count; i++)
				{
					WAMLevelConfig tmpContent = (WAMLevelConfig) presentedContent[i];
					WAMLevelOutcome tmpOutcome = (WAMLevelOutcome) outcomeList[i];


					WAMResTable workbenchTable = new WAMResTable(tmpContent.descriptionLabel);

					List<WAMResColumn> columnData = new List<WAMResColumn>();

					// Add headers.
					workbenchTable.addHeaderName(LocalisationMang.translate("Fed correct monkey"));
					workbenchTable.addHeaderName(LocalisationMang.translate("Missed correct monkey"));
					workbenchTable.addHeaderName(LocalisationMang.translate("Fed wrong monkey"));

					// Add column data.
					WAMResColumn fedCorrectCol = new WAMResColumn();
					Dictionary<string,int> fedCorrectData = tmpOutcome.getFedCorrectMonkeyData();
					if(fedCorrectData != null)
					{
						foreach(KeyValuePair<string,int> pair in fedCorrectData)
						{
							Debug.Log("Correct Item: "+pair.Key + " x"+pair.Value);
							fedCorrectCol.addCell(new WAMResCell(pair.Key + " x"+ pair.Value));
						}
					}

					WAMResColumn missedMonkCol = new WAMResColumn();
					Dictionary<string,int> missedMonkData = tmpOutcome.getMissedCorrectMonkeyData();
					if(missedMonkData != null)
					{
						foreach(KeyValuePair<string,int> pair in missedMonkData)
						{
							Debug.Log("Missed Item: "+pair.Key + " x"+pair.Value);
							missedMonkCol.addCell(new WAMResCell(pair.Key + " x"+ pair.Value));
						}
					}

					WAMResColumn fedWrongCol = new WAMResColumn();
					Dictionary<string,int> fedWrongData = tmpOutcome.getFedIncorrectMonkeyData();
					if(fedWrongData != null)
					{
						foreach(KeyValuePair<string,int> pair in fedWrongData)
						{
							Debug.Log("Wrong Item: "+pair.Key + " x"+pair.Value);
							fedWrongCol.addCell(new WAMResCell(pair.Key + " x"+ pair.Value));
						}
					}

					columnData.Add(fedCorrectCol);
					columnData.Add(missedMonkCol);
					columnData.Add(fedWrongCol);


					// Convert columns to rows.
					bool canMakeMoreRows = true;
					int dataRowIndex = 0;
					while(canMakeMoreRows)
					{
						canMakeMoreRows = false;
						
						List<WAMResCell> nwRowData = new List<WAMResCell>();
						for(int k=0; k<columnData.Count; k++)
						{
							List<WAMResCell> columnContent = columnData[k].columnContent;
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
							WAMResRow nwRow = new WAMResRow(nwRowData);
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
		

		acPKey = acResultData.getAcID();
	}



	class WAMResTable
	{
		public string tableTitle;
		public List<string> headerNames;
		public List<WAMResRow> rows;
		
		public WAMResTable(string para_tableTitle)
		{
			tableTitle = para_tableTitle;
			rows = new List<WAMResRow>();
			headerNames = new List<string>();
		}
		
		public void addHeaderName(string para_hName)
		{
			if(headerNames == null) { headerNames = new List<string>(); }
			headerNames.Add(para_hName);
		}
		
		public void addRow(WAMResRow para_nwRow)
		{
			if(rows == null) { rows = new List<WAMResRow>(); }
			rows.Add(para_nwRow);
		}
	}
	
	class WAMResRow
	{
		public List<WAMResCell> rowContent;
		
		public WAMResRow()
		{
			rowContent = new List<WAMResCell>();
		}
		
		public WAMResRow(List<WAMResCell> para_cells)
		{
			rowContent = para_cells;
		}
		
		public void addCell(WAMResCell para_nwCell)
		{
			if(rowContent == null) { rowContent = new List<WAMResCell>(); }
			rowContent.Add(para_nwCell);
		}
	}
	
	class WAMResColumn
	{
		public List<WAMResCell> columnContent;
		
		public WAMResColumn()
		{
			columnContent = new List<WAMResCell>();
		}
		
		public void addCell(WAMResCell para_nwCell)
		{
			if(columnContent == null) { columnContent = new List<WAMResCell>(); }
			columnContent.Add(para_nwCell);
		}
	}
	
	class WAMResCell
	{
		public string cellContent;
		
		public WAMResCell(string para_cellContent)
		{
			cellContent = para_cellContent;
		}
	}
}

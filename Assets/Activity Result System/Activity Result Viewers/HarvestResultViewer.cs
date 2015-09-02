/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class HarvestResultViewer : ActivityResultViewer, CustomActionListener
{
	
	ApplicationID acPKey;
	bool[] pageInitFlags;

	HarvestResTable resTable = null;
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

		if(guiOnStatus)
		{
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
						HarvestResRow currRow = resTable.rows[i];
						if(currRow != null)
						{
							GUILayout.BeginHorizontal();
							for(int k=0; k<currRow.rowContent.Count; k++)
							{
								HarvestResCell currCell = currRow.rowContent[k];
								string cellText = "";
								if(currCell != null)
								{
									cellText = currCell.cellContent;
									if(currCell.isCorrect)
									{
										GUI.color = Color.black;
										GUILayout.Label(cellText,availableGUIStyles["FieldContent"],GUILayout.Width(columnEqualGuiWidth));
									}
									else
									{
										//GUI.color = Color.red;
										if(GUILayout.Button(cellText,availableGUIStyles["ReviseButton"],GUILayout.Width(columnEqualGuiWidth)))
										{
											// Trigger popup here.
											guiOnStatus = false;
											string presentedStr = createPresentedString(currCell.lvlconfigIndex);
											string playerChoicesStr = createPlayerChoicesString(currCell.lvlconfigIndex);
											string correctChoicesStr = createCorrectChoicesString(currCell.lvlconfigIndex);
											string message = presentedStr +"\n"+ playerChoicesStr +"\n"+ correctChoicesStr;
											createMessageWindow(LocalisationMang.translate("Revise"),message,"HarvestResultViewer",this);
										}
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
				HarvestGameyResultData gameyData = (HarvestGameyResultData) acResultData.getGameyData();

				int numCorrectHarvests = gameyData.getNumCorrectHarvests();
				int numMachinesBroken = gameyData.getNumMachinesBroken();
				string timeStr = gameyData.getTimeString();
				
				
				string[] elementNames   = {"FieldTitle-0","FieldTitle-1","FieldTitle-2","FieldContent-0","FieldContent-1","FieldContent-2"};
				string[] elementContent = {LocalisationMang.translate("Correct harvests"),LocalisationMang.translate("Broken machines"),LocalisationMang.translate("Time"),""+numCorrectHarvests,""+numMachinesBroken,timeStr};
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
				resTable = new HarvestResTable();



				List<HarvestResColumn> columnData = new List<HarvestResColumn>();

				
				List<ILevelConfig> presentedContent = acResultData.getPresentedContent();
				List<LevelOutcome> outcomeList = acResultData.getOutcomeList();

				Debug.Log(presentedContent.Count);
				Debug.Log(outcomeList.Count);

				for(int i=0; i<presentedContent.Count; i++)
				{
					HarvestLevelConfig tmpContent = (HarvestLevelConfig) presentedContent[i];
					HarvestLevelOutcome tmpOutcome = (HarvestLevelOutcome) outcomeList[i];


					if(i == 0)
					{
						// Add headers.
						for(int k=0; k<tmpContent.machineDescriptions.Length; k++)
						{
							resTable.addHeaderName(tmpContent.machineDescriptions[k]);
							columnData.Add(new HarvestResColumn());
						}
					}

					// Add column data.
					int[] levelCorrectMachines = tmpContent.correctMachines;
					for(int k=0; k<levelCorrectMachines.Length; k++)
					{

						bool playerGotThisCorrect = false;

						int tmpCorrectMachineIndex = levelCorrectMachines[k];

						if(tmpOutcome.isPositiveOutcome())
						{
							playerGotThisCorrect = true;
						}
						else
						{
							if(tmpOutcome.getMachinesGivenGoodInput().Contains(tmpCorrectMachineIndex))
							{
								playerGotThisCorrect = true;
							}
						}

						columnData[tmpCorrectMachineIndex].addCell(new HarvestResCell(i,tmpContent.harvestingWord,playerGotThisCorrect));
					}


				}


				// Convert columns to rows.
				bool canMakeMoreRows = true;
				int dataRowIndex = 0;
				while(canMakeMoreRows)
				{
					canMakeMoreRows = false;

					List<HarvestResCell> nwRowData = new List<HarvestResCell>();
					for(int i=0; i<columnData.Count; i++)
					{
						List<HarvestResCell> columnContent = columnData[i].columnContent;
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
						HarvestResRow nwRow = new HarvestResRow(nwRowData);
						resTable.addRow(nwRow);
					}

					dataRowIndex++;
				}




				string[] elementNames   = {"TableScrollArea"};
				string[] elementContent = {"Table Scroll Area"};
				bool[] destroyGuideArr = {true};
				int[] textElementTypeArr = {0};
				prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,fullPathToTemplate);


				columnEqualGuiWidth = ((uiBounds["TableScrollArea"].width * 0.9f) / (columnData.Count * 1.0f));
				float minimumColumnWidth = (uiBounds["TableScrollArea"].width * 0.25f);
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


	public string createPresentedString(int para_levelConfigIndex)
	{
		List<ILevelConfig> presentedContent = acResultData.getPresentedContent();
		//List<LevelOutcome> outcomeList = acResultData.getOutcomeList();
		
		HarvestLevelConfig reqConfig = (HarvestLevelConfig) presentedContent[para_levelConfigIndex];
		//HarvestLevelOutcome reqOutcome = (HarvestLevelOutcome) outcomeList[para_levelConfigIndex];

		string retStr = "You were presented with:\n";
		retStr += "'"+reqConfig.harvestingWord+"'\n";
		return retStr;
	}

	public string createPlayerChoicesString(int para_levelConfigIndex)
	{
		List<ILevelConfig> presentedContent = acResultData.getPresentedContent();
		List<LevelOutcome> outcomeList = acResultData.getOutcomeList();

		HarvestLevelConfig reqConfig = (HarvestLevelConfig) presentedContent[para_levelConfigIndex];
		HarvestLevelOutcome reqOutcome = (HarvestLevelOutcome) outcomeList[para_levelConfigIndex];

		string retStr = "You chose: \n";
		List<int> machinesGivenGoodInput = reqOutcome.getMachinesGivenGoodInput();
		if(machinesGivenGoodInput != null)
		{
			for(int i=0; i<machinesGivenGoodInput.Count; i++)
			{
				retStr += reqConfig.machineDescriptions[machinesGivenGoodInput[i]];
				retStr += "\n";
			}
		}
		List<int> machinesGivenBadInput = reqOutcome.getMachinesGivenBadInput();
		if(machinesGivenBadInput != null)
		{
			for(int i=0; i<machinesGivenBadInput.Count; i++)
			{
				retStr += reqConfig.machineDescriptions[machinesGivenBadInput[i]];
				retStr += "\n";
			}
		}

		return retStr;		
	}

	public string createCorrectChoicesString(int para_levelConfigIndex)
	{
		List<ILevelConfig> presentedContent = acResultData.getPresentedContent();
		//List<LevelOutcome> outcomeList = acResultData.getOutcomeList();
		
		HarvestLevelConfig reqConfig = (HarvestLevelConfig) presentedContent[para_levelConfigIndex];
		//HarvestLevelOutcome reqOutcome = (HarvestLevelOutcome) outcomeList[para_levelConfigIndex];

		string retStr = "The correct answer is: \n";

		if(reqConfig.correctMachines != null)
		{
			for(int i=0; i<reqConfig.correctMachines.Length; i++)
			{
				retStr += reqConfig.machineDescriptions[reqConfig.correctMachines[i]];
				retStr += "\n";
			}
		}

		return retStr;
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_sourceID == "MessageWindow")
		{
			if(para_eventID == "OK")
			{
				guiOnStatus = true;
			}
		}
	}



	class HarvestResTable
	{
		public List<string> headerNames;
		public List<HarvestResRow> rows;

		public HarvestResTable()
		{
			rows = new List<HarvestResRow>();
			headerNames = new List<string>();
		}

		public void addHeaderName(string para_hName)
		{
			if(headerNames == null) { headerNames = new List<string>(); }
			headerNames.Add(para_hName);
		}

		public void addRow(HarvestResRow para_nwRow)
		{
			if(rows == null) { rows = new List<HarvestResRow>(); }
			rows.Add(para_nwRow);
		}
	}

	class HarvestResRow
	{
		public List<HarvestResCell> rowContent;

		public HarvestResRow()
		{
			rowContent = new List<HarvestResCell>();
		}

		public HarvestResRow(List<HarvestResCell> para_cells)
		{
			rowContent = para_cells;
		}

		public void addCell(HarvestResCell para_nwCell)
		{
			if(rowContent == null) { rowContent = new List<HarvestResCell>(); }
			rowContent.Add(para_nwCell);
		}
	}

	class HarvestResColumn
	{
		public List<HarvestResCell> columnContent;

		public HarvestResColumn()
		{
			columnContent = new List<HarvestResCell>();
		}

		public void addCell(HarvestResCell para_nwCell)
		{
			if(columnContent == null) { columnContent = new List<HarvestResCell>(); }
			columnContent.Add(para_nwCell);
		}
	}

	class HarvestResCell
	{
		public string cellContent;
		public bool isCorrect;
		public int lvlconfigIndex;

		public HarvestResCell(int para_lvlConfigIndex, string para_cellContent, bool para_isCorrect)
		{
			lvlconfigIndex = para_lvlConfigIndex;
			cellContent = para_cellContent;
			isCorrect = para_isCorrect;
		}
	}
}
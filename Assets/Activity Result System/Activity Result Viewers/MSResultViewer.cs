/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class MSResultViewer : ActivityResultViewer
{
	
	ApplicationID acPKey;
	bool[] pageInitFlags;


	MSResTable resTable;

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

			// Start render next table.
			
			if(resTable != null)
			{
				
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
					MSResRow currRow = resTable.rows[i];
					if(currRow != null)
					{
						GUILayout.BeginHorizontal();
						for(int k=0; k<currRow.rowContent.Count; k++)
						{
							MSResCell currCell = currRow.rowContent[k];
							string cellText = "";
							if(currCell != null)
							{
								cellText = currCell.cellContent;

								if(currCell is MSResCellRevise)
								{
									// Create Revise Button.
									GUILayout.Button(LocalisationMang.translate("Revise"),availableGUIStyles["ReviseButton"],GUILayout.Width(columnEqualGuiWidth));
								}
								else
								{
									GUI.color = Color.black;
									GUILayout.Label(cellText,availableGUIStyles["FieldContent"],GUILayout.Width(columnEqualGuiWidth));
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


			GUILayout.EndScrollView();
			GUILayout.EndArea();

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
				MSGameyResultData gameyData = (MSGameyResultData) acResultData.getGameyData();
				
				int numCorrectAttempts = gameyData.getNumCorrectAttempts();
				int numIncorrectAttempts = gameyData.getNumIncorrectAttempts();
				string timeStr = gameyData.getTimeString();
				
				
				string[] elementNames   = {"FieldTitle-0","FieldTitle-1","FieldTitle-2","FieldContent-0","FieldContent-1","FieldContent-2"};
				string[] elementContent = {LocalisationMang.translate("Correct attempts"),LocalisationMang.translate("Incorrect attempts"),LocalisationMang.translate("Time"),""+numCorrectAttempts,""+numIncorrectAttempts,timeStr};
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
				

				
				
				
				List<ILevelConfig> presentedContent = acResultData.getPresentedContent();
				List<LevelOutcome> outcomeList = acResultData.getOutcomeList();

				resTable = new MSResTable("");
				resTable.addHeaderName(LocalisationMang.translate("Postman"));
				resTable.addHeaderName(LocalisationMang.translate("Parcel"));
				resTable.addHeaderName("");

				for(int i=0; i<presentedContent.Count; i++)
				{
					MSLevelConfig tmpContent = (MSLevelConfig) presentedContent[i];
					MSLevelOutcome tmpOutcome = (MSLevelOutcome) outcomeList[i];

					if(i > 0)
					{
						MSResRow blankRow = new MSResRow(new List<MSResCell>() { null,null,null });
						resTable.addRow(blankRow);
					}

					string[] postmenWords = tmpContent.getPostmenWords();
					string[] parcelWords = tmpContent.getParcelWords();

					for(int k=0; k<postmenWords.Length; k++)
					{
						MSResRow nwDataRow = new MSResRow();

						nwDataRow.addCell(new MSResCell(postmenWords[k]));
						nwDataRow.addCell(new MSResCell(parcelWords[k]));

						if(tmpOutcome.isPairWithIncorrectAttempts(k))
						{
							nwDataRow.addCell(new MSResCellRevise());
						}
						else
						{
							nwDataRow.addCell(null);
						}

						resTable.addRow(nwDataRow);
					}
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



	class MSResTable
	{
		public string tableTitle;
		public List<string> headerNames;
		public List<MSResRow> rows;
		
		public MSResTable(string para_tableTitle)
		{
			tableTitle = para_tableTitle;
			rows = new List<MSResRow>();
			headerNames = new List<string>();
		}
		
		public void addHeaderName(string para_hName)
		{
			if(headerNames == null) { headerNames = new List<string>(); }
			headerNames.Add(para_hName);
		}
		
		public void addRow(MSResRow para_nwRow)
		{
			if(rows == null) { rows = new List<MSResRow>(); }
			rows.Add(para_nwRow);
		}
	}
	
	class MSResRow
	{
		public List<MSResCell> rowContent;
		
		public MSResRow()
		{
			rowContent = new List<MSResCell>();
		}
		
		public MSResRow(List<MSResCell> para_cells)
		{
			rowContent = para_cells;
		}
		
		public void addCell(MSResCell para_nwCell)
		{
			if(rowContent == null) { rowContent = new List<MSResCell>(); }
			rowContent.Add(para_nwCell);
		}
	}
	
	class MSResColumn
	{
		public List<MSResCell> columnContent;
		
		public MSResColumn()
		{
			columnContent = new List<MSResCell>();
		}
		
		public void addCell(MSResCell para_nwCell)
		{
			if(columnContent == null) { columnContent = new List<MSResCell>(); }
			columnContent.Add(para_nwCell);
		}
	}
	
	class MSResCell
	{
		public string cellContent;
		
		public MSResCell(string para_cellContent)
		{
			cellContent = para_cellContent;
		}
	}

	class MSResCellRevise : MSResCell
	{
		public MSResCellRevise()
			:base("")
		{

		}
	}
}

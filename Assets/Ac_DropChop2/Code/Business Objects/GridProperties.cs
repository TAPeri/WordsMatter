/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class GridProperties
{
	
	// Top Left Location.
	public float x;
	public float y;
	public float z;
	//public float[] topLeftLoc;
	
	public float borderThickness;
	public float cellWidth;
	public float cellHeight;
	public int widthInCells;
	public int heightInCells;
	public float totalWidth;
	public float totalHeight;
	
	public GridProperties(float[] para_topLeftLoc,
						  float para_borderThickness,
						  float para_cellWidth,
						  float para_cellHeight,
						  int para_widthInCells,
						  int para_heightInCells)
	{
		//topLeftLoc = para_topLeftLoc;
		x = para_topLeftLoc[0];
		y = para_topLeftLoc[1];
		z = para_topLeftLoc[2];
		
		borderThickness = para_borderThickness;
		cellWidth = para_cellWidth;
		cellHeight = para_cellHeight;
		widthInCells = para_widthInCells;
		heightInCells = para_heightInCells;
		
		totalWidth = (cellWidth * widthInCells) + (borderThickness * (widthInCells+1));
		totalHeight = (cellHeight * heightInCells) + (borderThickness * (heightInCells+1));
	}
	
	public GridProperties(UnityEngine.Rect para_bounds,
						  int para_gridWidthInCells,
						  int para_gridHeightInCells,
						  float para_borderPerc,
						  float para_depthVal)
	{
		
		// Calculate adjustments.
		
		float widthPerCellBorderCombo_Pix = (para_bounds.width / ((para_gridWidthInCells) * 1.0f));
		float heightPerCellBorderCombo_Pix = (para_bounds.height / ((para_gridHeightInCells) * 1.0f));
		
		float widthPerCell = widthPerCellBorderCombo_Pix * (1-para_borderPerc);
		float heightPerCell = heightPerCellBorderCombo_Pix * (1-para_borderPerc);
		float borderWidth = widthPerCellBorderCombo_Pix * para_borderPerc;
		float borderHeight = heightPerCellBorderCombo_Pix * para_borderPerc;
		
		float setCellSize = widthPerCell;
		float setBorderThickness = borderWidth;
		if(heightPerCellBorderCombo_Pix < widthPerCellBorderCombo_Pix) { setCellSize = heightPerCell; setBorderThickness = borderHeight;}
		widthPerCell = setCellSize;
		heightPerCell = setCellSize;
		float finalBorderThickness = setBorderThickness;
		//float finalCellBorderComboSize = setCellSize + setBorderThickness;
		
		
		
		// Set adjustments.
		
		borderThickness = finalBorderThickness;		
		cellWidth = widthPerCell;
		cellHeight = heightPerCell;
		widthInCells = para_gridWidthInCells;
		heightInCells = para_gridHeightInCells;
		totalWidth = (widthInCells * cellWidth) + ((widthInCells+1) * borderThickness);
		totalHeight = (heightInCells * cellHeight) + ((heightInCells+1) * borderThickness);
		
		/*topLeftLoc = new float[3] {
			(para_bounds.x + (para_bounds.width/2f)) - (totalWidth/2f),
			(para_bounds.y + (para_bounds.height/2f)) - (totalHeight/2f),
			para_depthVal
		};*/
		
		//x = (para_bounds.x + (para_bounds.width/2f)) - (totalWidth/2f);
		//y = (para_bounds.y + (para_bounds.height/2f)) - (totalHeight/2f);
		//z = para_depthVal;		
		
		x = para_bounds.x;
		y = para_bounds.y;
		z = para_depthVal;
	}

	public int[] hashPointToCell(float[] para_2DPt, bool para_originYIsMax)
	{
		int[] retHash = null;

		float highestY = this.y + this.totalHeight;
		float lowestY = this.y;
		if(para_originYIsMax) { highestY = this.y; lowestY = this.y - this.totalHeight; }


		if((para_2DPt[0] >= this.x)&&(para_2DPt[0] < (this.totalWidth))
		&&(para_2DPt[1] >= lowestY)&&(para_2DPt[1] < highestY))
		{
			retHash = new int[2];
			retHash[0] = (int) ((UnityEngine.Mathf.Abs(para_2DPt[0] - this.x)/this.totalWidth) * this.widthInCells);
			retHash[1] = (int) ((UnityEngine.Mathf.Abs(para_2DPt[1] - this.y)/this.totalHeight) * this.heightInCells);
		}

		return retHash;
	}

	public float[] getCellCentre(int[] para_cellCoords, bool para_originYIsMax)
	{
		float[] retCentre = null;

		if((para_cellCoords[0] >= 0)&&(para_cellCoords[0] < this.widthInCells)
		&&(para_cellCoords[1] >= 0)&&(para_cellCoords[1] < this.heightInCells))
		{
			retCentre = new float[2];
			retCentre[0] = this.x + (this.cellWidth * para_cellCoords[0]) + (this.cellWidth/2f);
			if(para_originYIsMax)
			{
				retCentre[1] = this.y - (this.cellHeight * para_cellCoords[1]) - (this.cellHeight/2f);
			}
			else
			{
				retCentre[1] = this.y + (this.cellHeight * para_cellCoords[1]) - (this.cellHeight/2f);
			}
		}

		return retCentre;
	}

	/*public float[] getEquivalentPoint(ref GridProperties para_gProp2, float[] para_ptInGProp2, bool para_yIsOnMax)
	{
		float[] normalisedCoord = new float[2] { (Mathf.Abs(para_ptInGProp2[0] - para_gProp2.x))/para_gProp2.totalWidth, (Mathf.Abs(para_ptInGProp2[1] - para_gProp2.y))/para_gProp2.totalHeight };
		float[] equivalentPt = new float[2] { this.x + (this.totalWidth * normalisedCoord[0]), this.y + (this.totalHeight * normalisedCoord[1]) };
		if(para_yIsOnMax) { equivalentPt[1] = this.y - (this.totalHeight * normalisedCoord[1]); }
		return equivalentPt;
	}*/
}

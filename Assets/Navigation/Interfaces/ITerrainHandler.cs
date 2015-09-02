/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public interface ITerrainHandler
{
	bool constructTerrainStructures(List<System.Object> para_dataToUse);
	bool isCellTraversible(int[] para_cellCoords);
	int getNavNodeIDForCell(int[] para_cellCoords);
}

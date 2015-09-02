/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class ExternalParams
{
	public ApplicationID acIdOverride;
	public bool hasAcIdOverride;
	public int questGiverIdOverride;
	public int langAreaOverride;
	public int difficultyOverride;
	public string levelOverride;
	public bool startImmediatelyWithoutDiag;
	
	public ExternalParams(ApplicationID para_acIdOverride,
	                      int para_questGiverIdOverride,
						  int para_langAreaOverride, 
	                      int para_difficultyOverride,
	                      string para_levelOverride,
	                      bool para_startImmediatelyWithoutDiag)
	{
		acIdOverride = para_acIdOverride;
		hasAcIdOverride = true;
		questGiverIdOverride = para_questGiverIdOverride;
		langAreaOverride = para_langAreaOverride;
		difficultyOverride = para_difficultyOverride;
		levelOverride = para_levelOverride;
		startImmediatelyWithoutDiag = para_startImmediatelyWithoutDiag;
	}

	public ExternalParams(int para_questGiverIdOverride,
	                      int para_langAreaOverride,
	                      int para_difficultyOverride,
	                      string para_levelOverride,
	                      bool para_startImmediatelyWithoutDiag)
	{
		hasAcIdOverride = false;
		questGiverIdOverride = para_questGiverIdOverride;
		langAreaOverride = para_langAreaOverride;
		difficultyOverride = para_difficultyOverride;
		levelOverride = para_levelOverride;
		startImmediatelyWithoutDiag = para_startImmediatelyWithoutDiag;
	}
}

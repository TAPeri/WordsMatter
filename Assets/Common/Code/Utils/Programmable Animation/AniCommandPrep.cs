/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using System.Collections.Generic;

public class AniCommandPrep
{
	public string commandStr;
	public int mode;
	public List<System.Object> parameters;

	public AniCommandPrep(string para_commandStr, int para_mode, List<System.Object> para_parameters)
	{
		commandStr = para_commandStr;
		mode = para_mode;
		parameters = para_parameters;
	}
}
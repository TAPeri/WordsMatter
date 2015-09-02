/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public class BasicNavGraph : NavGraph
{

	public BasicNavGraph()
		:base(new BasicNodeCollection(),
		      new BasicEdgeCollection(),
		      new WorldAStarSearch())
	{
		
	}
}
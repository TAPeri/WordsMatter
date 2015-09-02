/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public interface INavAlgorithm
{
	List<int> searchForPath(ref NavGraph para_graph, NavNode para_sourceNode, NavNode para_destNode);
	List<int> searchForPath(ref NavGraph para_graph, NavNode para_sourceNode, NavNode para_destNode, HashSet<int> para_untraversibleTypes);
}

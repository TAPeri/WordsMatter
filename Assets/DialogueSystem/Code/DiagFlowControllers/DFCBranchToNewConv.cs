/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public class DFCBranchToNewConv : DiagFlowCommand
{
	string nwConvSequenceName;

	public DFCBranchToNewConv(string para_nwConvSequenceName)
	{
		nwConvSequenceName = para_nwConvSequenceName;
	}

	public string getNwConvSequenceName() { return nwConvSequenceName; }
}

/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

public interface ILevelConfigGenerator
{
	ILevelConfig getNextLevelConfig(System.Object para_extraInfo);

	string getInstruction();
	int getConfigCount();
	void reboot();
}

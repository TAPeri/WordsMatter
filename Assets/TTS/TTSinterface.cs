/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;

public interface TTSinterface {

	void init(int pitchRate,int speechRate,string language);

	void fetch(string[] text);
	AudioClip say(string word);

	bool test(string text);

	bool loading();

	void clearCache();

}

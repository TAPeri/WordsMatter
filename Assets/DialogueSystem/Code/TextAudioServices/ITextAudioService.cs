/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
public interface ITextAudioService : IActionNotifier
{
	bool isServiceAvailable();
	bool canProcessContent(string para_reqParamStr);
	bool processContent(string para_reqParamStr, string para_listenerName, CustomActionListener para_listener);
}
/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class FaderScript : MonoBehaviour, CustomActionListener, IActionNotifier
{
	bool destroyFaderOnEnd;

	public void init(Color para_startColor, Color para_fadeToColor, float para_effectDuration, bool para_destroyFaderOnEnd)
	{
		destroyFaderOnEnd = para_destroyFaderOnEnd;

		SpriteRenderer sRend = transform.GetComponent<SpriteRenderer>();
		sRend.color = para_startColor;
		sRend.enabled = true;

		ColorTransition ct = transform.gameObject.AddComponent<ColorTransition>();
		ct.registerListener("FaderScript",this);
		ct.init(para_fadeToColor,para_effectDuration);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "ColorTransition")
		{
			notifyAllListeners(transform.name,"FadeEffectDone",null);
			if(destroyFaderOnEnd)
			{
				Destroy(transform.gameObject);
			}
		}
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

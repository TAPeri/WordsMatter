/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class TmpTestScript : MonoBehaviour, CustomActionListener
{
	void Start()
	{
		SlideshowInfoWindow slideshowWind = transform.gameObject.AddComponent<SlideshowInfoWindow>();
		slideshowWind.registerListener("AcScen",this);
		slideshowWind.init("SlideshowWindow","Textures/SlideshowSlides/"+"Ac_SJ",new string[] { "Text 1", "Text 2", "Text 3", "Text 4" });

	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{

	}
}

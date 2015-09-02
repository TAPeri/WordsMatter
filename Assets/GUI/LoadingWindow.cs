/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class LoadingWindow : ILearnRWUIElement
{


	void Start()
	{
		// Switch on dimer object.
		GameObject dimScreen = transform.FindChild("DimScreen").gameObject;
		dimScreen.renderer.enabled = true;
		dimScreen.renderer.material.color = Color.black;
		
		// Init text items.
		string[] elementNames   = {"LoadingText","LoadingBullets"};
		string[] elementContent = {"Loading","..."};
		bool[] destroyGuideArr = {true,true};
		int[] textElementTypeArr = {1,1};
		
		prepTextElements(elementNames,elementContent,destroyGuideArr,textElementTypeArr,null);

		transform.FindChild("LoadingText-WB").FindChild("Text").GetComponent<TextMesh>().renderer.sortingLayerName = "SpriteGUI";
		transform.FindChild("LoadingText-WB").FindChild("Text").GetComponent<TextMesh>().renderer.sortingOrder = 500;
		//transform.FindChild("LoadingBullets-WB").FindChild("Text").GetComponent<TextMesh>().renderer.sortingLayerName = "SpriteGUI";
		//transform.FindChild("LoadingBullets-WB").FindChild("Text").GetComponent<TextMesh>().renderer.sortingOrder = 500;

		CircleTimerScript timerScript = transform.FindChild("CircleTimer").FindChild("Timer").gameObject.GetComponent<CircleTimerScript>();
		timerScript.init(6);

		Destroy(this);
	}
}

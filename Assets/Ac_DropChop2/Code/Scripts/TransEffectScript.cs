/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class TransEffectScript : MonoBehaviour, IActionNotifier
{

	bool isDone;
	Vector4 endColor;
	Vector4 changeVect;
	Vector4 tmpCurrCol;
	List<SpriteRenderer> sRendList;


	void Start()
	{
		endColor = new Vector4(0,1,0,1);


		string suffix = transform.name.Split(':')[1];
		string[] suffixParts = suffix.Split('-');
		int startLetterIndex = int.Parse(suffixParts[0]);
		int endLetterIndex = int.Parse(suffixParts[1]);

		sRendList = new List<SpriteRenderer>();
		
		for(int i=startLetterIndex; i<(endLetterIndex+1); i++)
		{
			SpriteRenderer tmpSRend = (transform.FindChild("WordDesignRelatedGObj-"+i)).GetComponent<SpriteRenderer>();
			sRendList.Add(tmpSRend);
		}

		if(sRendList.Count > 0)
		{
			SpriteRenderer tmpSRend = sRendList[0];
			Vector4 currColor = new Vector4(tmpSRend.color.r,tmpSRend.color.g,tmpSRend.color.b,tmpSRend.color.a);
			changeVect = endColor - currColor;

			float effectDurationInSec = 2f;
			changeVect = changeVect/effectDurationInSec;
		}

		tmpCurrCol = new Vector4();
		isDone = false;
	}


	void Update()
	{
		for(int i=0; i<sRendList.Count; i++)
		{
			Color currCol = sRendList[i].color;

			Vector4 incVect = changeVect * Time.deltaTime;

			tmpCurrCol.x = currCol.r;
			tmpCurrCol.y = currCol.g;
			tmpCurrCol.z = currCol.b;
			tmpCurrCol.w = currCol.a;


			if(Vector4.Distance(endColor,tmpCurrCol) < incVect.magnitude)
			{
				currCol.r = endColor.x;
				currCol.g = endColor.y;
				currCol.b = endColor.z;
				currCol.a = endColor.w;
				isDone = true;
			}
			else
			{
				currCol.r += incVect.x;
				currCol.g += incVect.y;
				currCol.b += incVect.z;
				currCol.a += incVect.w;
			}

			sRendList[i].color = currCol;
		}


		if(isDone)
		{
			notifyAllListeners("WordTransitionEffect","EffectCompleted",null);
			Destroy(this);
		}
	}


	/*List<SpriteRenderer> sRendList;
	Color endColor;
	float colValPerSec = 0.5f;
	bool isDone;
	
	void Start()
	{
		endColor = new Color(0,1,0,1);

		string suffix = transform.name.Split(':')[1];
		string[] suffixParts = suffix.Split('-');
		int startLetterIndex = int.Parse(suffixParts[0]);
		int endLetterIndex = int.Parse(suffixParts[1]);


		sRendList = new List<SpriteRenderer>();

		for(int i=startLetterIndex; i<(endLetterIndex+1); i++)
		{
			SpriteRenderer tmpSRend = (transform.FindChild("WordDesignRelatedGObj-"+i)).GetComponent<SpriteRenderer>();
			tmpSRend.color = endColor;
			Color tmpCol = tmpSRend.color;
			tmpCol.a = 0;
			tmpSRend.color = tmpCol;
			sRendList.Add(tmpSRend);
		}

		isDone = false;
	}

	void Update()
	{
		float incVal = (colValPerSec * Time.deltaTime);

		for(int i=0; i<sRendList.Count; i++)
		{
			Color tmpCol = sRendList[i].color;
			tmpCol.a += incVal;
			if(tmpCol.a >= 1) { tmpCol.a = 1; isDone = true; }
			sRendList[i].color = tmpCol;
		}

		if(isDone)
		{
			notifyAllListeners("WordTransitionEffect","EffectCompleted",null);
			Destroy(this);
		}
	}*/


	
	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}

}

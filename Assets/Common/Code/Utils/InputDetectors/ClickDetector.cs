/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ClickDetector : AbsInputDetector
{

	//bool inSession;
	Vector2 clickPos;
	bool fingerDown;
	int numClicksForSession;
	float maxDelayBetweenClicks_Sec;
	float timeOfLastEntry;
	bool checkForEventThrow;
	bool hasSetDelayExternally;

	Vector2 avrgSessionClick;


	float invalidScreenDistBetweenClicks;
	Vector3 clickStartScreenPt;
	Vector3 clickReleaseScreenPt;



	void Start()
	{
		//inSession = false;
		clickPos = new Vector2(0,0);
		fingerDown = false;
		numClicksForSession = 0;
		if( ! hasSetDelayExternally) { maxDelayBetweenClicks_Sec = 0.2f; }
		avrgSessionClick = new Vector2(0,0);
		checkForEventThrow = false;


		float invalidWorldDistBetweenClicks = 1f;
		Vector3 tmpV1 = new Vector3(0,0,0);
		Vector3 tmpV2 = new Vector3(invalidWorldDistBetweenClicks,0,0);
		invalidScreenDistBetweenClicks = Vector3.Distance(Camera.main.WorldToScreenPoint(tmpV1),Camera.main.WorldToScreenPoint(tmpV2));

	}

	void Update()
	{

		if( ! inputPaused)
		{

			bool clickDetected = false;
			if((Application.platform == RuntimePlatform.Android)
			   ||(Application.platform == RuntimePlatform.IPhonePlayer))
			{
				if(Input.touches.Length == 1)
				{
					clickPos.x = Input.touches[0].position.x;
					clickPos.y = Input.touches[0].position.y;
					clickDetected = true;
				}
			}
			else
			{
				if(Input.GetMouseButton(0))
				{
					clickPos.x = Input.mousePosition.x;
					clickPos.y = Input.mousePosition.y;
					clickDetected = true;
				}
			}



			if(clickDetected)
			{
				if(!fingerDown)
				{
					fingerDown = true;

					//inSession = true;

					clickStartScreenPt.x = clickPos.x;
					clickStartScreenPt.y = clickPos.y;

					if(numClicksForSession == 0)
					{
						avrgSessionClick.x = clickPos.x;
						avrgSessionClick.y = clickPos.y;
					}
					else
					{
						avrgSessionClick.x = (avrgSessionClick.x + clickPos.x)/2f;
						avrgSessionClick.y = (avrgSessionClick.y + clickPos.y)/2f;

						//float clickDuration = Time.time - timeOfLastEntry;
						//float tmpI = 0;
					}

					numClicksForSession++;
					timeOfLastEntry = Time.time;
				}
			}
			else
			{
				if(fingerDown)
				{
					clickReleaseScreenPt.x = clickPos.x;
					clickReleaseScreenPt.y = clickPos.y;


					// Check 1: Deter interference with scrolling.
					// If the input point has moved significantly from the finger down point then this is a scroll not a click.
					bool invalidFlag = false;
					if(Vector2.Distance(clickStartScreenPt,clickReleaseScreenPt) >= invalidScreenDistBetweenClicks)
					{
						invalidFlag = true;
					}

					if( ! invalidFlag)
					{
						checkForEventThrow = true;
					}
					else
					{
						resetSession();
					}
				}

				if(checkForEventThrow)
				{
					if((Time.time - timeOfLastEntry) >= maxDelayBetweenClicks_Sec)
					{
						// Send event notification.
						System.Object[] eventData = new System.Object[2];
						eventData[0] = new float[2] { avrgSessionClick.x, avrgSessionClick.y };
						eventData[1] = numClicksForSession;

						notifyAllListeners("InputDetector","ClickEvent",eventData);

						resetSession();
					}
				}

				fingerDown = false;
			}
		}
	}

	public override void toggleInputStatus(bool para_inputOn)
	{
		inputPaused = !para_inputOn;
	}

	public void setMaxDelayBetweenClicks(float para_sec)
	{
		maxDelayBetweenClicks_Sec = para_sec;
		hasSetDelayExternally = true;
	}

	private void resetSession()
	{
		clickPos = new Vector2(0,0);
		fingerDown = false;
		numClicksForSession = 0;
		avrgSessionClick = new Vector2(0,0);
		//inSession = false;
		checkForEventThrow = false;
	}
}

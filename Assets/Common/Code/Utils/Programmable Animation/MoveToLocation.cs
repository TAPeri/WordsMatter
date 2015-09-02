/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class MoveToLocation : AbsCustomAniCommand
{
	Vector3 startPt;
	Vector3 destination;
	Vector3 movVect;
	float increment;


	// Used if a single distance interval is repeated indefinitely.
	int sendMarkerUpdates;
	float markerIntervalDist;
	Vector3 lastIntervalPoint;

	// Used if intervals are marked by specific distances.
	float totReqDistance;
	float totalDistanceTravelled;
	float[] normDistIntervalPts;
	int currIntervalPointIndex;



	void Update()
	{
		increment = (movVect.magnitude * Time.deltaTime);

		if(Vector3.Distance(transform.position,destination) > increment)
		{
			transform.position += (movVect * (Time.deltaTime));

			if(sendMarkerUpdates == 0)
			{
				// No marker updates required by listeners.
			}
			else
			{

				if(sendMarkerUpdates == 1)
				{
					float distFromLastInterval = Vector3.Distance(transform.position,lastIntervalPoint);

					if(distFromLastInterval >= markerIntervalDist)
					{
						lastIntervalPoint = new Vector3(transform.position.x,transform.position.y,transform.position.z);
						notifyAllListeners(transform.name,"MoveIntervalUpdate",null);
					}
				}
				else if(sendMarkerUpdates == 2)
				{
					if(currIntervalPointIndex < normDistIntervalPts.Length)
					{
						totalDistanceTravelled = Vector3.Distance(startPt,transform.position);

						float normDistTravelled = totalDistanceTravelled / totReqDistance;
						if(normDistTravelled >= normDistIntervalPts[currIntervalPointIndex])
						{
							currIntervalPointIndex++;
							notifyAllListeners(transform.name,"MoveIntervalUpdate",currIntervalPointIndex-1);
						}
					}
				}
			}
		}
		else
		{
			performExit();
		}
	}

	public void initScript(Vector3 para_destination, float para_distPerSec)
	{
		startPt = new Vector3(transform.position.x,transform.position.y,transform.position.z);
		destination = para_destination;
		totReqDistance = Vector3.Distance(startPt,destination);
		movVect = (Vector3.Normalize(para_destination - transform.position));
		movVect *= para_distPerSec;

		sendMarkerUpdates = 0; // i.e. no interval updates.
		markerIntervalDist = 0;
	}

	public void initScript(Vector3 para_destination, float para_timeToCompleteInSec, bool para_dummyOverrideVar)
	{
		startPt = new Vector3(transform.position.x,transform.position.y,transform.position.z);
		destination = para_destination;
		totReqDistance = Vector3.Distance(startPt,destination);

		float currDistToTarget = Vector3.Distance(transform.position,para_destination);
		float distPerSec = currDistToTarget/para_timeToCompleteInSec;
		movVect = (Vector3.Normalize(para_destination - transform.position));
		movVect *= distPerSec;

		sendMarkerUpdates = 0; // i.e. no interval updates.
		markerIntervalDist = 0;
	}





	public void requestIntervalUpdates(float para_markerIntervalDist)
	{
		sendMarkerUpdates = 1;
		markerIntervalDist = para_markerIntervalDist;
		lastIntervalPoint = new Vector3(transform.position.x,transform.position.y,transform.position.z);
	}

	public void requestIntervalUpdates(float[] para_normDistIntervals)
	{
		sendMarkerUpdates = 2;
		totalDistanceTravelled = 0;
		normDistIntervalPts = para_normDistIntervals;
		currIntervalPointIndex = 0;
	}
	

	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		if(para_prep.mode == 1)
		{
			float[] p_destinationVect = (float[]) para_prep.parameters[0];
			float p_distPerSec = (float) para_prep.parameters[1];

			this.initScript(new Vector3(p_destinationVect[0],p_destinationVect[1],p_destinationVect[2]),p_distPerSec);
		}
		else if(para_prep.mode == 2)
		{
			float[] p_destinationVect = (float[]) para_prep.parameters[0];
			float p_timeToCompleteInSec = (float) para_prep.parameters[1];
			bool p_dummyParam = (bool) para_prep.parameters[2];

			this.initScript(new Vector3(p_destinationVect[0],p_destinationVect[1],p_destinationVect[2]),p_timeToCompleteInSec,p_dummyParam);
		}

		return true;
	}


	public void changeETAintermediate(float para_secToDest){


		//normal distance to current destination minus normalised distance till current position
		float normDistToIntermDest = normDistIntervalPts[currIntervalPointIndex]-(totalDistanceTravelled / totReqDistance);//(Vector3.Distance(startPt,transform.position)/Vector3.Distance(startPt,destination));
		float distToIntermDest = normDistToIntermDest*totReqDistance;
		//Debug.Log("Distance: "+distToIntermDest+" from current " + (totalDistanceTravelled / totReqDistance)+" to "+normDistIntervalPts[currIntervalPointIndex]);
		movVect = Vector3.Normalize(movVect);
		float distPerSec = distToIntermDest / para_secToDest;
		Debug.Log("Go to "+currIntervalPointIndex+" "+normDistToIntermDest);

		//Debug.Log("Distance per second: "+distPerSec);
		movVect *= distPerSec;
	}

	public void changeETASpeed(float para_secToDest){

		//float distToDest = Vector3.Distance(transform.position,destination);
		movVect *= para_secToDest;

	}


	public void changeETA(float para_secToDest)
	{
		movVect = Vector3.Normalize(movVect);
		float distToDest = Vector3.Distance(transform.position,destination);
		float distPerSec = distToDest / para_secToDest;
		movVect *= distPerSec;
	}


	private void performExit()
	{
		transform.position = destination;
		notifyAllListeners(transform.name,"MoveToLocation",null);
		Destroy(this);
	}
}

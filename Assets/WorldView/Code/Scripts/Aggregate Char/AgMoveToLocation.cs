/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class AgMoveToLocation // : AbsCustomAniCommand
{

	/*Dictionary<string,AgMoveStateType1> type1States;
	Dictionary<string,AgMoveStateType2> type2States;
	Dictionary<string,AgMoveStateType3> type3States;



	void Update()
	{

		if(type1States != null)
		{
			foreach(KeyValuePair<string,AgMoveStateType1> pair in type1States)
			{
				AgMoveStateType1
				
				increment = (movVect.magnitude * Time.deltaTime);
			}
		}
		else if(type2States != null)
		{

		}
		else if(type3States != null)
		{

		}




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
							notifyAllListeners(transform.name,"MoveIntervalUpdate",currIntervalPointIndex);
							currIntervalPointIndex++;
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





	// STATE OBJECTS.

	// To be extended.
	class AgMoveState : IActionNotifier
	{
		public GameObject gObjRef;

		public AgMoveState(GameObject para_gObj)
		{
			gObjRef = para_gObj;
		}

		// Action Notifier Methods.
		IActionNotifier acNotifier = new ConcActionNotifier();
		public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
		public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
		public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
	}

	// Simple move attributes.
	class AgMoveStateType1 : AgMoveState
	{
		public Vector3 startPt;
		public Vector3 destination;
		public Vector3 movVect;
		public float increment;

		public AgMoveStateType1(GameObject para_gObj, Vector3 para_startPt, Vector3 para_destination, Vector3 para_movVect, float para_increment)
			:base(para_gObj)
		{
			startPt = para_startPt;
			destination = para_destination;
			movVect = para_movVect;
			increment = para_increment;
		}
	}

	// Used if a single distance interval is repeated indefinitely.
	class AgMoveStateType2 : AgMoveStateType1
	{
		public int sendMarkerUpdates;
		public float markerIntervalDist;
		public Vector3 lastIntervalPoint;

		public AgMoveStateType2(GameObject para_gObj, Vector3 para_startPt, Vector3 para_destination, Vector3 para_movVect, float para_increment,
		                        int para_sendMarkerUpdates, float para_markerIntervalDist, Vector3 para_lastIntervalPoint)
			:base(para_gObj,para_startPt,para_destination,para_movVect,para_increment)
		{
			sendMarkerUpdates = para_sendMarkerUpdates;
			markerIntervalDist = para_markerIntervalDist;
			lastIntervalPoint = para_lastIntervalPoint;
		}
	}

	// Used if intervals are marked by specific distances.
	class AgMoveStateType3 : AgMoveStateType1
	{
		public float totReqDistance;
		public float totalDistanceTravelled;
		public float[] normDistIntervalPts;
		public int currIntervalPointIndex;

		public AgMoveStateType3(GameObject para_gObj, Vector3 para_startPt, Vector3 para_destination, Vector3 para_movVect, float para_increment,
		                        float para_totReqDistance, float para_totalDistanceTravelled, float[] para_normDistIntervalPts, int para_currIntervalPointIndex)
			:base(para_gObj,para_startPt,para_destination,para_movVect,para_increment)
		{
			totReqDistance = para_totReqDistance;
			totalDistanceTravelled = para_totalDistanceTravelled;
			normDistIntervalPts = para_normDistIntervalPts;
			currIntervalPointIndex = para_currIntervalPointIndex;
		}
	}*/
}

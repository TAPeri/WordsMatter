/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class WaiterScript : MonoBehaviour, IActionNotifier
{
	int currIndex = 0;
	List<GetRoutine> routineList;
	GetRoutine currRoutine;




	void Update()
	{

		if(currRoutine==null){
			Debug.LogError("Error waiting for server: no routine");
			informTaskEnd(currIndex,false);
			proceedToNextTask();
		}

		RoutineStatus cStatus = currRoutine.status();
		if(cStatus == RoutineStatus.ERROR)
		{
			Debug.LogError("Error waiting for server");
			informTaskEnd(currIndex,false);
			proceedToNextTask();
		}
		else if(cStatus == RoutineStatus.IDLE)
		{
			Debug.LogError("Error wating for server: Server request routine is Idle?");
			informTaskEnd(currIndex,false);
			proceedToNextTask();
		}
		else if(cStatus == RoutineStatus.READY)
		{
			informTaskEnd(currIndex,true);
			proceedToNextTask();
		}

		if(cStatus == RoutineStatus.WAIT)
		{
			// waiting.
		}
	}

	public void init(List<GetRoutine> para_waitList)
	{
		routineList = para_waitList;
		if(routineList != null)
		{
			currRoutine = routineList[0];
		}
	}

	private void informTaskEnd(int para_taskID, bool para_successFlag)
	{
		List<System.Object> retData = new List<System.Object>();
		retData.Add(para_taskID);
		retData.Add(para_successFlag);
		notifyAllListeners("WaiterScript","SingleTaskDone",retData);
	}

	private void proceedToNextTask()
	{
		bool canProceed = false;

		if(currIndex < (routineList.Count-1))
		{
			currIndex++;
			bool foundNext = false;
			while(! foundNext)
			{
				if(currIndex >= routineList.Count)
				{
					break;
				}
				else if(routineList[currIndex] != null)
				{
					currRoutine = routineList[currIndex];
					foundNext = true;
					canProceed = true;
					break;
				}
				else
				{
					currIndex++;
				}
			}
		}

		if( ! canProceed)
		{
			notifyAllListeners("WaiterScript","AllDone",null);
			Destroy(this);
		}
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

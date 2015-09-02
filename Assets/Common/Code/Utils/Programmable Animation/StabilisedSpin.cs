/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class StabilisedSpin : AbsCustomAniCommand
{


	float anglePerSec;
	float haltAtMaxRot;
	float numRotsSoFar;
	
	//float origAngle;
	float currRotAngle;
	
	float currRoundAngle;


	// Fixed items.
	List<GameObject> stabilisedObjs;
	List<GameObject> stabObjParents;
	





	void Update()
	{
		if(numRotsSoFar >= haltAtMaxRot)
		{
			float rem = haltAtMaxRot % 2f;
			Vector3 angleVect = transform.localEulerAngles;
			angleVect.z = (int) (360f - (360f * rem));
			transform.localEulerAngles = angleVect;

			for(int i=0; i<stabilisedObjs.Count; i++)
			{
				if((stabilisedObjs[i] != null)
				&&(stabObjParents[i] != null))
				{
					if(stabObjParents[i].name == "TmpHinge")
					{
						stabilisedObjs[i].transform.parent = transform;
						Destroy(stabObjParents[i]);
					}
					else
					{
						stabilisedObjs[i].transform.parent = stabObjParents[i].transform;
					}
				}
			}
			
			notifyAllListeners(transform.name,"SpinComplete",null);
			Destroy(this);
		}
		else
		{
			if(currRotAngle >= currRoundAngle)
			{
				currRotAngle = 360f - currRotAngle;
				
				if(currRoundAngle == 360f)
				{
					numRotsSoFar++;
				}
				else
				{
					numRotsSoFar += (currRoundAngle/360f);
				}
				
				if((haltAtMaxRot - numRotsSoFar) < 1)
				{
					currRoundAngle = (360f * (haltAtMaxRot - numRotsSoFar));
				}
			}
			else
			{

				/*List<Vector3> displacementVects = new List<Vector3>();
				for(int i=0; i<stabilisedObjs.Count; i++)
				{
					if((stabilisedObjs[i] != null)
					&&(stabObjParents[i] != null))
					{
						//displacementVects.Add( stabilisedObjs[i].transform.position);// - stabObjParents[i].transform.position );
						stabilisedObjs[i].transform.parent = null;
					}
				}*/



				currRotAngle += (anglePerSec * Time.deltaTime);
				
				Vector3 angleVect = transform.localEulerAngles;
				angleVect.z = 360f - currRotAngle;
				transform.localEulerAngles = angleVect;



				for(int i=0; i<stabilisedObjs.Count; i++)
				{
					if((stabilisedObjs[i] != null)
					   &&(stabObjParents[i] != null))
					{
						stabilisedObjs[i].transform.position = stabObjParents[i].transform.position;// + displacementVects[i];
						//stabilisedObjs[i].transform.parent = stabObjParents[i].transform;
					}
				}




			}
		}
	}
	
	
	
	public void init(float para_rotationsPerSecond, float para_haltAtMaxRot, string[] para_namesOfStabObjs)
	{
		anglePerSec = (para_rotationsPerSecond * 360.0f);
		haltAtMaxRot = para_haltAtMaxRot;
		numRotsSoFar = 0;
		currRotAngle = 0;
		//origAngle = transform.localEulerAngles.z;
		
		if(para_haltAtMaxRot >= 1f) { currRoundAngle = 360f; } else { currRoundAngle = 360f * para_haltAtMaxRot; } 

		//DataStore ds = performRecursiveObjSearch(transform.gameObject,new List<string>(para_namesOfStabObjs));
		//stabilisedObjs = ds.stabilisedObjList;
		//stabilisedObjsAngles = ds.stabilisedObjAngles;

		stabilisedObjs = performRecursiveObjSearch(transform.gameObject,new List<string>(para_namesOfStabObjs));

		stabObjParents = new List<GameObject>();
		for(int i=0; i<stabilisedObjs.Count; i++)
		{
			if(stabilisedObjs[i] != null)
			{
				if(stabilisedObjs[i].transform.parent.name == transform.name)
				{
					GameObject tmpHinge = new GameObject("TmpHinge");
					tmpHinge.transform.position = stabilisedObjs[i].transform.position;
					tmpHinge.transform.parent = transform;
					stabObjParents.Add(tmpHinge.gameObject);
				}
				else
				{
					stabObjParents.Add(stabilisedObjs[i].transform.parent.gameObject);
				}
				stabilisedObjs[i].transform.parent = null;
			}
		}
	}
	
	public override bool initViaCommandPrep(AniCommandPrep para_prep)
	{
		float p_rotationsPerSecond = (float) para_prep.parameters[0];
		float p_haltAtMaxRot = (float) para_prep.parameters[1];
		string[] p_namesOfStabObjs = (string[]) para_prep.parameters[2];
		this.init(p_rotationsPerSecond,p_haltAtMaxRot,p_namesOfStabObjs);
		return true;
	}



	private List<GameObject> performRecursiveObjSearch(GameObject para_tmpObj, List<string> para_matchingNames)
	{
		//DataStore ds = new DataStore();
		List<GameObject> localList = new List<GameObject>();

		if(para_matchingNames.Contains(para_tmpObj.name))
		{
			localList.Add(para_tmpObj);
		}

		
		for(int i=0; i<para_tmpObj.transform.childCount; i++)
		{
			List<GameObject> tmpRecList = performRecursiveObjSearch((para_tmpObj.transform.GetChild(i)).gameObject,para_matchingNames);
			localList.AddRange(tmpRecList);
		}			
		
		return localList;
	}


	class DataStore
	{

	}

}

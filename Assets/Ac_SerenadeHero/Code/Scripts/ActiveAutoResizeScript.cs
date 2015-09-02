/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ActiveAutoResizeScript : MonoBehaviour
{
	
	List<Vector3> pointsToNotice;
	int axisToNotice;
	Vector3 ptOnAxis;

	Vector3 restPos;

	Vector3 startWorldScale;
	Vector3 destWorldScale;

	float startFontCharacterSize;
	float destFontCharacterSize;

	bool usePoints;
	bool useAxis;

	Vector3 prevPos;

	List<TextMesh> textMToUpdate;

	bool in2DMode;


	void Start()
	{
		prevPos = new Vector3(transform.position.x,transform.position.y,transform.position.z);
		textMToUpdate = new List<TextMesh>();
		textMToUpdate = getTextMeshOfChildrenRecursively(this.gameObject);
	}


	void Update()
	{


		float minDistanceToTarget = -1f;
		float distFromRestToDest = -1f;

		if(usePoints)
		{
			Vector3 minDestinationPt = Vector3.zero;

			for(int i=0; i<pointsToNotice.Count; i++)
			{
				if(in2DMode)
				{
					Vector3 tmpVar = pointsToNotice[i];
					tmpVar.z = transform.position.z;
					pointsToNotice[i] = tmpVar;
				}

				float tmpDist = Vector3.Distance(pointsToNotice[i],transform.position);
				
				if((minDistanceToTarget == -1)||(tmpDist < minDistanceToTarget))
				{
					minDistanceToTarget = tmpDist;
					minDestinationPt = pointsToNotice[i];
				}
			}



			distFromRestToDest = Vector3.Distance(restPos,minDestinationPt);
		}
		else if(useAxis)
		{
			switch(axisToNotice)
			{
				case 0:	minDistanceToTarget = Mathf.Abs(transform.position.x - ptOnAxis.x); break;
				case 1: minDistanceToTarget = Mathf.Abs(transform.position.y - ptOnAxis.y); break;
				case 2: minDistanceToTarget = Mathf.Abs(transform.position.z - ptOnAxis.z); break;
				default: minDistanceToTarget = -1; Debug.Log("ActiveAutoResizeScript Error"); break;
			}

			Vector3 minDestinationPt = ptOnAxis;

			if(in2DMode)
			{
				minDestinationPt.z = restPos.z;
			}

			distFromRestToDest = Vector3.Distance(restPos,minDestinationPt);
		}






		
		if((distFromRestToDest != 0)
		&&(Vector3.Equals(prevPos,transform.position) == false))
		{
			float percFromRest = (1-(minDistanceToTarget/distFromRestToDest));
			
			
			Vector3 diffScaleVect = (destWorldScale - startWorldScale);

			
			// LocalScale (1,1,1) --> startWorldScale (size vect)
			//      ?             --> reqRenderSizeVect
			Vector3 reqRenderSizeVect = (startWorldScale + (diffScaleVect * percFromRest));
			Vector3 defaultScaleOfItem = new Vector3(1,1,1);
			Vector3 reqScale = divideVecElements(multiplyVecElements(reqRenderSizeVect,defaultScaleOfItem),startWorldScale);
			
			Vector3 oldScale = transform.localScale;
			reqScale.z = oldScale.z;
			transform.localScale = reqScale;




			// Update size of any text on the object.
			if((textMToUpdate != null)&&(textMToUpdate.Count > 0))
			{
				for(int i=0; i<textMToUpdate.Count; i++)
				{
					TextMesh tmesh = textMToUpdate[i];
					tmesh.transform.parent = null;
					tmesh.transform.localScale = defaultScaleOfItem;
					tmesh.transform.parent = transform;


					float changeFactorFontCharacterSize = startFontCharacterSize + ((destFontCharacterSize - startFontCharacterSize) * percFromRest);
					tmesh.characterSize = changeFactorFontCharacterSize;
				}
			}


			prevPos.x = transform.position.x;
			prevPos.y = transform.position.y;
			prevPos.z = transform.position.z;
		}

	}

	private List<TextMesh> getTextMeshOfChildrenRecursively(GameObject para_tmpObj)
	{
		List<TextMesh> localList = new List<TextMesh>();
		
		TextMesh tmpTMesh = null;
		tmpTMesh = para_tmpObj.GetComponent<TextMesh>();
		if(tmpTMesh != null)
		{
			localList.Add(tmpTMesh);
		}
		
		for(int i=0; i<para_tmpObj.transform.childCount; i++)
		{
			List<TextMesh> tmpRecList = getTextMeshOfChildrenRecursively((para_tmpObj.transform.GetChild(i)).gameObject);
			localList.AddRange(tmpRecList);
		}			
		
		return localList;
	}

	public void init(List<Vector3> para_ptsToNotice,
	                 Vector3 para_startWorldScale,
	                 Vector3 para_destWorldScale,
	                 float para_startFontCharacterSize,
	                 float para_destFontCharacterSize,
	                 bool para_2dMode)
	{
		restPos = new Vector3(transform.position.x,transform.position.y,transform.position.z);

		usePoints = true;
		useAxis = false;
		pointsToNotice = para_ptsToNotice;
		axisToNotice = -1;
		ptOnAxis = Vector3.zero;
		startWorldScale = para_startWorldScale;
		destWorldScale = para_destWorldScale;
		startFontCharacterSize = para_startFontCharacterSize;
		destFontCharacterSize = para_destFontCharacterSize;
		in2DMode = para_2dMode;
	}

	public void init(int para_axisToNotice,
	                 Vector3 para_ptOnAxis,
	                 Vector3 para_startWorldScale,
	                 Vector3 para_destWorldScale,
	                 float para_startFontCharacterSize,
	                 float para_destFontCharacterSize,
	                 bool para_2dMode)
	{
		restPos = new Vector3(transform.position.x,transform.position.y,transform.position.z);

		usePoints = false;
		useAxis = true;
		pointsToNotice = null;
		axisToNotice = para_axisToNotice;
		ptOnAxis = para_ptOnAxis;
		startWorldScale = para_startWorldScale;
		destWorldScale = para_destWorldScale;
		startFontCharacterSize = para_startFontCharacterSize;
		destFontCharacterSize = para_destFontCharacterSize;
		in2DMode = para_2dMode;
	}


	private Vector3 multiplyVecElements(Vector3 para_v1, Vector3 para_v2)
	{
		Vector3 retVec = new Vector3( para_v1.x * para_v2.x,
		                             para_v1.y * para_v2.y,
		                             para_v1.z * para_v2.z );
		return retVec;
	}
	
	private Vector3 divideVecElements(Vector3 para_v1, Vector3 para_v2)
	{
		Vector3 retVec = new Vector3( para_v1.x / para_v2.x,
		                             para_v1.y / para_v2.y,
		                             para_v1.z / para_v2.z );
		return retVec;
	}
}

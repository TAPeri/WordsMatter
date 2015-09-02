/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class HoleScript : MonoBehaviour, CustomActionListener, IActionNotifier
{

	HashSet<string> potentialParkers;
	string parkerName;
	bool isOccupied;


	void Start()
	{
		potentialParkers = new HashSet<string>();
		parkerName = null;
		isOccupied = false;
	}
	

	void Update()
	{

	}


	void OnTriggerEnter(Collider other)
	{
		GameObject colGObj = other.gameObject;
		handleEnterContact(ref colGObj);
	}
	
	void OnTriggerExit(Collider other)
	{
		GameObject colGObj = other.gameObject;
		handleExitContact(ref colGObj);
	}

	void OnCollisionEnter(Collision col)
	{
		GameObject colGObj = col.gameObject;
		handleEnterContact(ref colGObj);
		handleRegistering(colGObj.name);
	}

	void OnCollisionExit(Collision col)
	{
		GameObject colGObj = col.gameObject;
		handleExitContact(ref colGObj);
	}

	public void reset()
	{
		if(potentialParkers != null) { potentialParkers.Clear(); } else { potentialParkers = new HashSet<string>(); }

		if(parkerName != null)
		{
			// Create HoleEmptied event.
			string holeName = transform.gameObject.name;
			string[] tmpParams = new string[2] {parkerName,holeName};
			notifyAllListeners(transform.name,"HoleEmptied",tmpParams);
		}

		parkerName = null;
		isOccupied = false;
	}


	// This part needs cleaning up. Tmp hack for now to allow for automatic return to hole if the player just picks up the item but releases it again without leaving the hole bounds.
	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "DragStart")
		{
			if(isOccupied)
			{
				if((parkerName != null)&&(parkerName == para_sourceID))
				{
					//manualDisownProcedure();
					if(parkerName != null)
					{
						// Create HoleEmptied event.
						string holeName = transform.gameObject.name;
						string[] tmpParams = new string[2] {parkerName,holeName};
						notifyAllListeners(transform.name,"HoleEmptied",tmpParams);
					}

				}
			}
		}
		else if(para_eventID == "DragRelease")
		{
			if( ! isOccupied)
			{
				if(potentialParkers.Contains(para_sourceID))
				{
					handleRegistering(para_sourceID);
				}
			}
			else
			{
				if(parkerName == para_sourceID)
				{
					GameObject nwParker = GameObject.Find(para_sourceID);
					Vector3 nwParkerPos = nwParker.transform.position;
					bool parkerHasRenderer = false;
					Rect nwParker2DWorldBounds = new Rect(-1,-1,-1,-1);
					if(nwParker.renderer != null)
					{
						parkerHasRenderer = true;
						nwParker2DWorldBounds = CommonUnityUtils.get2DBounds(nwParker.renderer.bounds);
					}
					else if(nwParker.collider != null)
					{
						parkerHasRenderer = true;
						nwParker2DWorldBounds = CommonUnityUtils.get2DBounds(nwParker.collider.bounds);
					}
					Rect hole2DWorldBounds = CommonUnityUtils.get2DBounds(transform.renderer.bounds);

					bool isValidMatch = false;
					if(parkerHasRenderer)
					{
						// Adjust ys because Unity Rect assumes that x, y, width, height where y is the min y pt.
						nwParker2DWorldBounds.y = (nwParker2DWorldBounds.y - nwParker2DWorldBounds.height);
						hole2DWorldBounds.y = (hole2DWorldBounds.y - hole2DWorldBounds.height);

						if(nwParker2DWorldBounds.Overlaps(hole2DWorldBounds,true))
						{
							isValidMatch = true;
						}
					}
					else
					{
						// Adjust ys because Unity Rect assumes that x, y, width, height where y is the min y pt.
						hole2DWorldBounds.y = (hole2DWorldBounds.y - hole2DWorldBounds.height);

						if(hole2DWorldBounds.Contains(nwParkerPos))
						{
							isValidMatch = true;
						}
					}

					if(isValidMatch)
					{
						if( ! potentialParkers.Contains(para_sourceID))
						{
							potentialParkers.Add(para_sourceID);
						}
						//isOccupied = false;
						handleRegistering(para_sourceID);
						// Create HoleFilled event.
						string fillerName = para_sourceID;
						string holeName = transform.gameObject.name;
						string[] tmpParams = new string[2] {fillerName,holeName};
						notifyAllListeners(transform.name,"HoleFilled",tmpParams);
					}
				}

			}
		}
	}


	private void handleEnterContact(ref GameObject para_colObj)
	{
		GameObject colGObj = para_colObj;
		//Debug.Log("CONTACT");
		if(colGObj.layer == LayerMask.NameToLayer("Draggable"))//(colGObj.tag == "WordBox")
		{
			if( ! isOccupied)
			{
				if(potentialParkers != null)
				{
					if( ! potentialParkers.Contains(colGObj.name))
					{
						potentialParkers.Add(colGObj.name);
						DragScript dScrpt = (GameObject.Find("GlobObj").GetComponent<DragScript>());
						dScrpt.incrementDragObjOwnerAmount(colGObj.name);
						dScrpt.registerListener(this.gameObject.name,this);
					}
					
					//transform.renderer.material.color = Color.green;
				}
			}
		}
	}

	private void handleExitContact(ref GameObject para_colObj)
	{
		GameObject colGObj = para_colObj;

		//Debug.Log(" EXIT CONTACT");


		if(colGObj.layer == LayerMask.NameToLayer("Draggable"))// .tag == "WordBox")
		{

			//if(para_colObj.name.Contains("WordPoolEntry"))//Ignore this function on serenade hero
			//	return;

			if((potentialParkers.Contains(colGObj.name))||(parkerName == colGObj.name))
			{

				DragScript dScript = (GameObject.Find("GlobObj").GetComponent<DragScript>());

				dScript.decrementDragObjOwnerAmount(colGObj.name);
				dScript.unregisterListener(this.gameObject.name);
				
				if(potentialParkers.Contains(colGObj.name))
				{
					Debug.Log("Remove name from potentialParkers "+colGObj.name);

					potentialParkers.Remove(colGObj.name);
				}
				
				if(parkerName == colGObj.name)
				{

					Debug.Log("Parker name! "+colGObj.name);


					if(parkerName != null)
					{
						Debug.Log("Hole emptied!");

						// Create HoleEmptied event.
						string holeName = transform.gameObject.name;
						string[] tmpParams = new string[2] {parkerName,holeName};
						notifyAllListeners(transform.name,"HoleEmptied",tmpParams);
					}

					parkerName = null;
					isOccupied = false;
				}

				//Destroy(dScript);

			}else{

				Debug.Log("No potential parkers");
			}
			
			transform.renderer.material.color = Color.gray;

		}
	}

	private void handleRegistering(string para_objName)
	{
		GameObject parkerObj = GameObject.Find(para_objName);
		Vector3 dockingLocation = transform.position;
		dockingLocation.z = parkerObj.transform.position.z;
		parkerObj.transform.position = dockingLocation;
			
		if( ! isOccupied)
		{	
			// Create HoleFilled event.
			string fillerName = para_objName;
			string holeName = transform.gameObject.name;
			string[] tmpParams = new string[2] {fillerName,holeName};
			notifyAllListeners(transform.name,"HoleFilled",tmpParams);
		}

		parkerName = para_objName;
		potentialParkers.Clear();
		isOccupied = true;
	}


	public void manualDisownProcedure()
	{
		if(potentialParkers != null)
		{
			if(potentialParkers.Contains(parkerName))
			{
				potentialParkers.Remove(parkerName);
			}
		}

		if(parkerName != null)
		{
			// Create HoleEmptied event.
			string holeName = transform.gameObject.name;
			string[] tmpParams = new string[2] {parkerName,holeName};
			notifyAllListeners(transform.name,"HoleEmptied",tmpParams);
		}

		parkerName = null;
		isOccupied = false;
	}

	public bool isHoleInLimbo()
	{
		if(parkerName != null)
		{
			DragScript dScrpt = (GameObject.Find("GlobObj").GetComponent<DragScript>());
			if(dScrpt != null)
			{
				string dragObjName = dScrpt.getNameOfDragObj();
				if((dragObjName != null)&&(dragObjName == parkerName))
				{
					return true;
				}
			}
		}

		return false;
	}


	public string getParkerName()
	{
		return parkerName;
	}

	void OnDestroy()
	{
		GameObject globObj = GameObject.Find("GlobObj");
		if(globObj != null)
		{
			DragScript dScrpt = (globObj.GetComponent<DragScript>());
			if(dScrpt != null)
			{
				/*if(parkerName != null)
				{
					dScrpt.decrementDragObjOwnerAmount(parkerName);
				}*/

				dScrpt.unregisterListener(this.gameObject.name);
			}
		}
	}


	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}
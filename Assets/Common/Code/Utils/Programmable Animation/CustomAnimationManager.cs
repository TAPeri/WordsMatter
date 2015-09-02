/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class CustomAnimationManager : MonoBehaviour, CustomActionListener, IActionNotifier
{
	string animationSequenceName;
	bool hasDispatchedBatch;
	List<List<AniCommandPrep>> commands;
	int numAcksReceived;
	
	List<AbsCustomAniCommand> spawnedScriptRefs;

	void Start()
	{
		spawnedScriptRefs = new List<AbsCustomAniCommand>();
	}
	
	void Update()
	{
		if(commands.Count == 0)
		{
			// Notify all that animation/command chain is done.
			notifyAllListeners(transform.name,animationSequenceName,null);
			Destroy(this);
		}
		else
		{
			if( ! hasDispatchedBatch)
			{
				dispatchNextBatch();
				hasDispatchedBatch = true;
			}
			else
			{
				// Is waiting for command batch to complete.
			}
		}
	}

	public void init(string para_animationSequenceName, List<List<AniCommandPrep>> para_commands)
	{
		animationSequenceName = para_animationSequenceName;
		commands = para_commands;
		hasDispatchedBatch = false;
		numAcksReceived = 0;
	}

	public void init(List<List<AniCommandPrep>> para_commands)
	{
		init("CustomAniComplete",para_commands);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		numAcksReceived++;
		if(numAcksReceived >= commands[0].Count)
		{
			commands.RemoveAt(0);
			numAcksReceived = 0;
			hasDispatchedBatch = false;
		}
	}

	/// <summary>
	/// WARNING: Intentionally does not return a notification to listeners.
	/// </summary>
	public void forceHalt()
	{
		for(int i=0; i<spawnedScriptRefs.Count; i++)
		{
			if(spawnedScriptRefs[i] != null)
			{
				Destroy(spawnedScriptRefs[i]);
			}
		}
		spawnedScriptRefs.Clear();
		Destroy(this);
	}
	
	private void dispatchNextBatch()
	{
		for(int i=0; i<commands[0].Count; i++)
		{
			dispatchNextCommand(0,i);
		}
	}



	private void dispatchNextCommand(int para_batchIndex, int para_commandIndex)
	{
		AniCommandPrep reqCommand = (commands[para_batchIndex])[para_commandIndex];
		//Debug.LogWarning("Add animation: "+reqCommand.commandStr);

		MonoBehaviour tmpCommandScript = (MonoBehaviour) transform.gameObject.AddComponent(reqCommand.commandStr);
		if(tmpCommandScript == null)
		{
			Debug.LogError("CustomAnimationManager: Command Script was NULL. Could not find '"+reqCommand.commandStr+"'");
		}
		else
		{


			AbsCustomAniCommand castCommand = tmpCommandScript as AbsCustomAniCommand;
			if(castCommand == null)
			{
				Debug.LogError("CustomAnimationManager: Script '"+reqCommand.commandStr+"' does not extend AbsCustomAniCommand!");
			}
			else
			{
				bool successFlag = castCommand.initViaCommandPrep(reqCommand);

				if(! successFlag)
				{
					Debug.LogError("CustomAnimationManager: Valid command but initialisation failed!");
				}
				else
				{

					// Register listener.
					castCommand.registerListener("CustomAnimationManager",this);
					spawnedScriptRefs.Add(castCommand);
				}
			}
		}
	}



	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}

}

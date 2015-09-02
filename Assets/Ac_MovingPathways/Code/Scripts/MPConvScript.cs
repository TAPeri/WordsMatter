/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class MPConvScript : MonoBehaviour, CustomActionListener, IActionNotifier
{
	Transform[] playerBubbleSequence;
	Transform[] bystanderBubbleSequence;
	int playerTalkIndex;
	int bystanderTalkIndex;

	GameObject playerAv;
	GameObject bystander;

	GameObject currBubble;

	int talker = 0;

	public void init(Transform[] para_playerTalkSequence,
	                 	   Transform[] para_bystanderTalkSequence,
	                 	   GameObject para_playerAv,
	                 	   GameObject para_bystander)
	{
		playerBubbleSequence = para_playerTalkSequence;
		bystanderBubbleSequence = para_bystanderTalkSequence;
		playerTalkIndex = 1;
		bystanderTalkIndex = 1;

		playerAv = para_playerAv;
		bystander = para_bystander;

		showPlayerBubble();
	}

	private void showPlayerBubble()
	{
		talker = 0;
		showBubble(playerAv,playerBubbleSequence[playerTalkIndex]);
		playerTalkIndex++;
	}

	public void showBystanderBubble()
	{
		talker = 1;
		showBubble(bystander,bystanderBubbleSequence[bystanderTalkIndex]);
		bystanderTalkIndex++;
	}
	
	public void endConversation()
	{
		Destroy(currBubble);
		notifyAllListeners("MPConvScript","PlayerBystanderConvFinished",null);
		Destroy(this);
	}

	public void respondToEvent(string para_sourceID, string para_eventID, System.Object para_eventData)
	{
		if(para_eventID == "DelayEnd")
		{

			if((talker == 0)&&(bystanderTalkIndex < (bystanderBubbleSequence.Length)))
		    {
				showBystanderBubble();
			}
			else if((talker == 1)&&(playerTalkIndex < (playerBubbleSequence.Length)))
			{
				showPlayerBubble();
			}
			else
			{
				endConversation();
			}

		}
	}


	private void showBubble(GameObject para_subject, Transform para_bubblePrefab)
	{
		Destroy(currBubble);

		Vector3 bubbleSpawnPt = new Vector3(para_subject.transform.position.x + 0.2f,para_subject.transform.position.y + 0.9f,para_subject.transform.position.z);
		Transform nwMark = (Transform) Instantiate(para_bubblePrefab,bubbleSpawnPt,Quaternion.identity);
		nwMark.name = "Bubble";

		currBubble = nwMark.gameObject;

		DelayForInterval dfi = transform.gameObject.AddComponent<DelayForInterval>();
		dfi.registerListener("WaitAndCall",this);
		dfi.init(1f);
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

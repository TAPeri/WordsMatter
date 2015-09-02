/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class DialogScenarioScript : MonoBehaviour, IActionNotifier
{

	
	Dictionary<string,Rect> uiBounds;
	Dictionary<string,Texture2D> availableTextures;
	Dictionary<string,GUIStyle> availableStyles;
	bool hasInitGUIStyles = false;
	
	float dialog_Z = -0.5f;
	bool displayFinalQuestion = true;
	string labelTextStr;
	GameObject dialogBubbleRef;

	void Start()
	{
		loadTextures();
		prepUIBounds();
	}


	public void init(string para_receiverObj, string para_expertObj, Transform para_dialogBubblePrefab, string para_labelTextStr)
	{
		//GameObject receiverGObj = GameObject.Find(para_receiverObj);
		GameObject expertGObj = GameObject.Find(para_expertObj);

		Vector3 dialogBubbleSpot = expertGObj.transform.position + new Vector3(- (expertGObj.transform.renderer.bounds.size.x/2f), (expertGObj.transform.renderer.bounds.size.y) + (expertGObj.transform.renderer.bounds.size.y * 0.1f));
		dialogBubbleSpot.z = dialog_Z;

		Transform dialogBubble = (Transform) Instantiate(para_dialogBubblePrefab,dialogBubbleSpot,Quaternion.identity);
		dialogBubble.name = "DialogBubble";
		dialogBubbleRef = dialogBubble.gameObject;

		SoundPlayerScript sps = transform.GetComponent<SoundPlayerScript>();
		sps.triggerSoundAtCamera("pop");

		labelTextStr = para_labelTextStr;
	}



	void OnGUI()
	{
		if( ! hasInitGUIStyles)
		{
			availableStyles = new Dictionary<string,GUIStyle>();

			GUIStyle largeCentreLabel = new GUIStyle(GUI.skin.label);
			largeCentreLabel.fontSize = 30;
			largeCentreLabel.alignment = TextAnchor.MiddleCenter;

			GUIStyle largeCentreBtn = new GUIStyle(GUI.skin.button);
			largeCentreBtn.fontSize = 30;

			availableStyles.Add("LargeCLabel",largeCentreLabel);
			availableStyles.Add("LargeCBtn",largeCentreBtn);

			hasInitGUIStyles = true;
		}
		else
		{
			if(displayFinalQuestion)
			{

				GUI.DrawTexture(uiBounds["BottomBar"],availableTextures["BlackTex"]);

				GUI.DrawTexture(uiBounds["YesBtn"],availableTextures["GreenTex"]);
				if(GUI.Button(uiBounds["YesBtn"],"YES",availableStyles["LargeCBtn"]))
				{
					notifyAllListeners("DialogScript","DialogEnd",true);
					Destroy(this);
				}

				GUI.DrawTexture(uiBounds["NoBtn"],availableTextures["RedTex"]);
				if(GUI.Button(uiBounds["NoBtn"],"NO",availableStyles["LargeCBtn"]))
				{
					notifyAllListeners("DialogScript","DialogEnd",false);
					Destroy(this);
				}


				GUI.Label(uiBounds["TextLabel"],labelTextStr,availableStyles["LargeCLabel"]);
			}
		}
	}

	void OnDestroy()
	{
		Destroy(dialogBubbleRef);
	}

	private void prepUIBounds()
	{
		uiBounds = new Dictionary<string,Rect>();

		float bottomBar_Width = Screen.width;
		float bottomBar_Height = Screen.height * 0.25f;
		float bottomBar_X = 0;
		float bottomBar_Y = Screen.height - bottomBar_Height;

		float yesBtn_X = bottomBar_X;
		float yesBtn_Y = bottomBar_Y;
		float yesBtn_Width = bottomBar_Height;
		float yesBtn_Height = bottomBar_Height;

		float noBtn_Width = bottomBar_Height;
		float noBtn_Height = bottomBar_Height;
		float noBtn_X = Screen.width - bottomBar_Height;
		float noBtn_Y = bottomBar_Y;

		float textLabel_X = yesBtn_X + yesBtn_Width;
		float textLabel_Y = bottomBar_Y;
		float textLabel_Width = bottomBar_Width - (yesBtn_Width + noBtn_Width);
		float textLabel_Height = bottomBar_Height;


		uiBounds.Add("BottomBar",new Rect(bottomBar_X,bottomBar_Y,bottomBar_Width,bottomBar_Height));
		uiBounds.Add("YesBtn",new Rect(yesBtn_X,yesBtn_Y,yesBtn_Width,yesBtn_Height));
		uiBounds.Add("NoBtn",new Rect(noBtn_X,noBtn_Y,noBtn_Width,noBtn_Height));
		uiBounds.Add("TextLabel",new Rect(textLabel_X,textLabel_Y,textLabel_Width,textLabel_Height));
	}

	private void loadTextures()
	{
		availableTextures = new Dictionary<string,Texture2D>();

		Texture2D blackTex = new Texture2D(1,1);
		blackTex.SetPixel(0,0,Color.black);
		blackTex.Apply();

		Texture2D greenTex = new Texture2D(1,1);
		greenTex.SetPixel(1,1,new Color(76f/255f,153f/255f,0));
		greenTex.Apply();

		Texture2D redTex = new Texture2D(1,1);
		redTex.SetPixel(1,1,new Color(153f/255f,0,0));
		redTex.Apply();

		availableTextures.Add("BlackTex",blackTex);
		availableTextures.Add("GreenTex",greenTex);
		availableTextures.Add("RedTex",redTex);
	}



	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

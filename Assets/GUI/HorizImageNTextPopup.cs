/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class HorizImageNTextPopup : ILearnRWScenario, IActionNotifier
{
	protected string windowName;
	protected string imgPath;
	protected string text;


	void Start()
	{
		this.loadTextures();
		this.prepUIBounds();
	}

	void OnGUI()
	{
		if( ! hasInitGUIStyles)
		{
			prepGUIStyles();
			hasInitGUIStyles = true;
		}
		else
		{
			GUI.DrawTexture(uiBounds["Window"],availableTextures["Window"]);
			GUI.DrawTexture(uiBounds["Image"],availableTextures["Image"]);
			GUI.Label(uiBounds["TextArea"],text,availableGUIStyles["LargeCentredLabel"]);
			if(GUI.Button(uiBounds["OkButton"],"OK"))
			{
				notifyAllListeners(windowName,"OK",null);
				Destroy(this);
			}
		}
	}

	protected new void prepUIBounds()
	{
		uiBounds = new Dictionary<string,Rect>();

		float window_Width = Screen.width * 0.80f;
		float window_Height = Screen.height * 0.50f;
		float window_X = (Screen.width/2f) - (window_Width/2f);
		float window_Y = (Screen.height/2f) - (window_Height/2f);


		float padding = 10f;

		float contentBox_Width = window_Width - (padding * 2f);
		float contentBox_Height = window_Height - (padding * 2f);
		float contentBox_X = window_X + (window_Width/2f) - (contentBox_Width/2f);
		float contentBox_Y = window_Y + (window_Height/2f) - (contentBox_Height/2f);


		float img_X = contentBox_X;
		float img_Y = contentBox_Y;
		float img_Width = contentBox_Height;
		float img_Height = img_Width;

		float txtArea_X = img_X + img_Width + padding;
		float txtArea_Y = contentBox_Y;
		float txtArea_Width = contentBox_Width - img_Width - padding;
		float txtArea_Height = contentBox_Height * 0.75f;


		float okBtn_Width = txtArea_Width * 0.25f;
		float okBtn_Height = contentBox_Height * 0.25f;
		float okBtn_X = txtArea_X + (txtArea_Width/2f) - (okBtn_Width/2f);
		float okBtn_Y = txtArea_Y + txtArea_Height;


		uiBounds.Add("Window",new Rect(window_X,window_Y,window_Width,window_Height));
		uiBounds.Add("ContentBox",new Rect(contentBox_X,contentBox_Y,contentBox_Width,contentBox_Height));
		uiBounds.Add("Image",new Rect(img_X,img_Y,img_Width,img_Height));
		uiBounds.Add("TextArea",new Rect(txtArea_X,txtArea_Y,txtArea_Width,txtArea_Height));
		uiBounds.Add("OkButton",new Rect(okBtn_X,okBtn_Y,okBtn_Width,okBtn_Height));
	}



	protected new void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string,GUIStyle>(); hasInitGUIStyles = true;

		GUIStyle largeCentredLabel = new GUIStyle(GUI.skin.label);
		largeCentredLabel.alignment = TextAnchor.MiddleCenter;
		largeCentredLabel.fontSize = 20;
		
		availableGUIStyles.Add("LargeCentredLabel",largeCentredLabel);
	}

	protected new void loadTextures()
	{
		availableTextures = new Dictionary<string,Texture2D>();
		
		Texture2D grayTex = new Texture2D(1,1);
		grayTex.SetPixel(0,0,Color.gray);
		grayTex.Apply();

		Texture2D blackTex = new Texture2D(1,1);
		blackTex.SetPixel(0,0,Color.black);
		blackTex.Apply();

		Texture2D imgTex = Resources.Load<Texture2D>(imgPath);

		availableTextures.Add("DefaultTex",grayTex);
		availableTextures.Add("Window",blackTex);
		availableTextures.Add("Image",imgTex);
	}

	public void init(string para_windowName, string para_imgPath, string para_text)
	{
		windowName = para_windowName;
		imgPath = para_imgPath;
		text = para_text;
	}

	protected void triggerSoundAtCamera(string para_soundFileName, float para_secondsUntilShutoff)
	{
		GameObject camGObj = Camera.main.gameObject;
		AudioSource audS = camGObj.AddComponent<AudioSource>();
		audS.clip = (AudioClip) Resources.Load("Sounds/"+para_soundFileName,typeof(AudioClip));
		audS.loop = false;
		audS.volume = 0.5f;
		audS.Play();

		// FIXME : Plug
		//DestroyAfterTime dat = camGObj.AddComponent<DestroyAfterTime>();
		//dat.delaySec = para_secondsUntilShutoff;
	}

	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

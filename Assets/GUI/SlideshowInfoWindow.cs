/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class SlideshowInfoWindow : ILearnRWScenario, IActionNotifier
{
	string windowName = "SlideshowWindow";

	int currSlideID;
	Sprite[] slideshowImages;
	string[] slideshowText;

	Transform tutorialContentPane;
	bool showProgressBar = true;



	void Start()
	{
		this.loadTextures();
		this.prepUIBounds();

		tutorialContentPane = transform.FindChild("TutorialContentPane");

		currSlideID = 0;
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
			GUI.DrawTexture(uiBounds["EntireScreen"],availableTextures["FadeGrayTex"]);
			GUI.DrawTexture(uiBounds["Window"],availableTextures["Window"]);
			//GUI.DrawTexture(uiBounds["Img"],slideshowImages[currSlideID]);

			if( ! showProgressBar)
			{
				if(currSlideID < slideshowText.Length)
				{
					GUI.Label(uiBounds["InstructionTextArea"],slideshowText[currSlideID],availableGUIStyles["LargeCentredLabel"]);
				}
				else
				{
					GUI.Label(uiBounds["InstructionTextArea"],"",availableGUIStyles["LargeCentredLabel"]);
				}
			}
			else
			{
				float progPerc = (currSlideID+1)/(slideshowImages.Length * 1.0f);
				Rect fullProgBarArea = uiBounds["ProgressBarArea"];
				Rect progBounds = new Rect(fullProgBarArea.x, fullProgBarArea.y, progPerc * fullProgBarArea.width, fullProgBarArea.height);
				GUI.DrawTexture(progBounds,availableTextures["ProgressBarTex"]);
			}


			if(currSlideID > 0)	{ GUI.color = Color.white; }else{ GUI.color = Color.gray; }
			if(GUI.Button(uiBounds["BtnPrevious"],availableTextures["Previous"]))
			{
				if(currSlideID > 0)
				{
					notifyAllListeners(windowName,"Previous",null);
					currSlideID--;
					tutorialContentPane.gameObject.GetComponent<SpriteRenderer>().sprite = slideshowImages[currSlideID];
				}
			}


			GUI.color = Color.white;
			if(GUI.Button(uiBounds["BtnClose"],availableTextures["Close"]))
			{
				notifyAllListeners(windowName,"Close",null);
				Destroy(this);
			}


			if(currSlideID < (slideshowImages.Length-1)) { GUI.color = Color.white; }else{ GUI.color = Color.gray; }
			if(GUI.Button(uiBounds["BtnNext"],availableTextures["Next"]))
			{
				if(currSlideID < (slideshowImages.Length-1))
				{
					notifyAllListeners(windowName,"Next",null);
					currSlideID++;
					tutorialContentPane.gameObject.GetComponent<SpriteRenderer>().sprite = slideshowImages[currSlideID];
				}
			}
		}
	}


	public void init(string para_windowName, string[] para_imgPaths, string[] para_slideTextArr)
	{
		windowName = para_windowName;
		slideshowText = para_slideTextArr;

		slideshowImages = new Sprite[para_imgPaths.Length];
		for(int i=0; i<para_imgPaths.Length; i++)
		{
			string reqPath = para_imgPaths[i];
			slideshowImages[i] = Resources.Load<Sprite>(reqPath);
		}
	}

	public void init(string para_windowName, string para_imgDirectory, string[] para_slideTextArr)
	{
		windowName = para_windowName;
		slideshowText = para_slideTextArr;


		List<Sprite> tmpImages = new List<Sprite>();

		int nxtSlideID = 1;
		string nxtSlideImgFileName = "slide_";
		bool foundNewSlide = true;
		do
		{
			nxtSlideImgFileName = "slide_" + nxtSlideID;
			Sprite nwImg = Resources.Load<Sprite>(para_imgDirectory + "/" + nxtSlideImgFileName);
			if(nwImg != null)
			{
				tmpImages.Add(nwImg);
			}
			else
			{
				foundNewSlide = false;
			}

			nxtSlideID++;
		}
		while(foundNewSlide);

		slideshowImages = tmpImages.ToArray();
	}



	protected new void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string,GUIStyle>(); hasInitGUIStyles = true;
		
		GUIStyle largeCentredLabel = new GUIStyle(GUI.skin.label);
		largeCentredLabel.alignment = TextAnchor.MiddleCenter;
		largeCentredLabel.fontSize = 20;
		
		availableGUIStyles.Add("LargeCentredLabel",largeCentredLabel);
	}

	protected new void prepUIBounds()
	{
		uiBounds = new Dictionary<string,Rect>();


		float window_Width = Screen.width * 0.50f;
		float window_Height = Screen.height * 0.80f;
		float window_X = (Screen.width/2f) - (window_Width/2f);
		float window_Y = (Screen.height/2f) - (window_Height/2f);
		
		
		float padding = 10f;
		
		float contentBox_Width = window_Width - (padding * 2f);
		float contentBox_Height = window_Height - (padding * 2f);
		float contentBox_X = window_X + (window_Width/2f) - (contentBox_Width/2f);
		float contentBox_Y = window_Y + (window_Height/2f) - (contentBox_Height/2f);
		
		
		float img_X = contentBox_X;
		float img_Y = contentBox_Y;
		float img_Width = contentBox_Width;
		float img_Height = (img_Width/4f) * 3f; // 4:3 ratio.
		
		float txtArea_X = contentBox_X;
		float txtArea_Y = img_Y + img_Height;
		float txtArea_Width = contentBox_Width;
		float txtArea_Height = (contentBox_Height - img_Height) * 0.4f;

		float progressBarArea_Width = txtArea_Width;
		float progressBarArea_Height = txtArea_Height/2f;
		float progressBarArea_X = txtArea_X;
		float progressBarArea_Y = txtArea_Y + (txtArea_Height/2f) - (progressBarArea_Height/2f);
		

		float btnArea_X = contentBox_X;
		float btnArea_Y = txtArea_Y + txtArea_Height;
		float btnArea_Width = contentBox_Width;
		float btnArea_Height = (contentBox_Height - img_Height) * 0.6f;

		float btnPrevious_X = btnArea_X;
		float btnPrevious_Y = btnArea_Y;
		float btnPrevious_Width = btnArea_Width/3f;
		float btnPrevious_Height = btnArea_Height;

		float btnClose_X = btnPrevious_X + btnPrevious_Width;
		float btnClose_Y = btnArea_Y;
		float btnClose_Width = btnArea_Width/3f;
		float btnClose_Height = btnArea_Height;

		float btnNext_X = btnClose_X + btnClose_Width;
		float btnNext_Y = btnArea_Y;
		float btnNext_Width = btnArea_Width/3f;
		float btnNext_Height = btnArea_Height;

		uiBounds.Add("EntireScreen",new Rect(0,0,Screen.width,Screen.height));
		uiBounds.Add("Window",new Rect(window_X,window_Y,window_Width,window_Height));
		uiBounds.Add("ContentBox",new Rect(contentBox_X,contentBox_Y,contentBox_Width,contentBox_Height));
		uiBounds.Add("Img",new Rect(img_X,img_Y,img_Width,img_Height));
		uiBounds.Add("InstructionTextArea",new Rect(txtArea_X,txtArea_Y,txtArea_Width,txtArea_Height));
		uiBounds.Add("ProgressBarArea",new Rect(progressBarArea_X,progressBarArea_Y,progressBarArea_Width,progressBarArea_Height));
		uiBounds.Add("BtnPrevious",new Rect(btnPrevious_X,btnPrevious_Y,btnPrevious_Width,btnPrevious_Height));
		uiBounds.Add("BtnClose",new Rect(btnClose_X,btnClose_Y,btnClose_Width,btnClose_Height));
		uiBounds.Add("BtnNext",new Rect(btnNext_X,btnNext_Y,btnNext_Width,btnNext_Height));
	}

	protected new void loadTextures()
	{
		availableTextures = new Dictionary<string,Texture2D>();
		
		Texture2D grayTex = new Texture2D(1,1);
		grayTex.SetPixel(0,0,Color.gray);
		grayTex.Apply();

		Texture2D fadeGrayTex = new Texture2D(1,1);
		fadeGrayTex.SetPixel(0,0,new Color(0,0,0,0.7f));
		fadeGrayTex.Apply();

		Texture2D blackTex = new Texture2D(1,1);
		blackTex.SetPixel(0,0,Color.black);
		blackTex.Apply();

		Texture2D progressTex = new Texture2D(1,1);
		progressTex.SetPixel(0,0,Color.green);
		progressTex.Apply();


		availableTextures.Add("DefaultTex",grayTex);
		availableTextures.Add("Window",blackTex);
		availableTextures.Add("ProgressBarTex",progressTex);
		availableTextures.Add("FadeGrayTex",fadeGrayTex);
		availableTextures.Add("Previous",Resources.Load<Texture2D>("Textures/Common/PreviousButton"));
		availableTextures.Add("Close",Resources.Load<Texture2D>("Textures/Common/CloseButton"));
		availableTextures.Add("Next",Resources.Load<Texture2D>("Textures/Common/NextButton"));
	}



	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

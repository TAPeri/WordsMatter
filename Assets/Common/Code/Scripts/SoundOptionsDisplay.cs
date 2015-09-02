/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class SoundOptionsDisplay : MonoBehaviour, IActionNotifier
{



	Dictionary<string,Rect> uiBounds;
	Dictionary<string,Texture2D> availableTextures;
	Dictionary<string,GUIStyle> availableGUIStyles;
	bool hasInitGUIStyles = false;
	
	
	float[] musicVolumePresets = {0,0.25f,0.50f,0.75f,1f};
	float musicVolumePerc = 0.5f;
	float oldSiderVal = 0.5f;


	string noteStr =
			"These options only effect game music.\n"
			+"Results may vary depending on device speakers.";


	List<AudioSource> tmpAudSrcList;
	float maxClampVolume;


		
	void Start()
	{
		loadTextures();
		prepUiBounds();
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

			GUI.DrawTexture(uiBounds["PopupWindow"],availableTextures["BlackTex"]);
			GUI.color = Color.white;
			GUI.DrawTexture(uiBounds["TitleBarIcon"],availableTextures["MusicControlBox"]);
			GUI.Label(uiBounds["TitleBarText"],"MUSIC VOLUME SETTINGS",availableGUIStyles["LargeCentredLabel"]);
			if(GUI.Button(uiBounds["TitleBarCloseIcon"],availableTextures["CloseIcon"]))
			{	
				updateKnownAudSrces();

				Destroy(GameObject.Find("MaskQuad"));
				notifyAllListeners("SoundOptionDisplay","SoundOptionDisplayClose",musicVolumePerc);

				SoundPlayerScript sps = transform.GetComponent<SoundPlayerScript>();
				sps.triggerSoundAtCamera("blop");

				this.enabled = false;
			}

			GUI.DrawTexture(uiBounds["InternalBox"],availableTextures["GrayTex"]);
			//GUI.Label(uiBounds["NoteLabel"],"Note",availableGUIStyles["LargeCentredLabel"]);
			GUI.color = Color.black;
			GUI.Label(uiBounds["NoteText"],noteStr,availableGUIStyles["LargeCentredLabel"]);   
			GUI.Label(uiBounds["MusicPresetLabel"],"Presets",availableGUIStyles["LargeCentredLabel"]);
			GUI.Label(uiBounds["MusicVolumeSliderLabel"],"Slider",availableGUIStyles["LargeCentredLabel"]);
			GUI.color = Color.white;
			musicVolumePerc = GUI.HorizontalSlider(uiBounds["MusicVolumeSlider"],musicVolumePerc,0,1f,availableGUIStyles["SliderStyle"],availableGUIStyles["SliderThumbStyle"]);
			if(oldSiderVal != musicVolumePerc)
			{
				updateKnownAudSrces();
				oldSiderVal = musicVolumePerc;
			}


			for(int i=0; i<musicVolumePresets.Length; i++)
			{
				if(GUI.Button(uiBounds["MusicVolumePresetIcon-"+i],""+i))
				{
					// Set preset.
					musicVolumePerc = musicVolumePresets[i];
					updateKnownAudSrces();
				}
			}



		}


	}


	public void init(List<AudioSource> para_tmpAudScrList)
	{
		this.init(para_tmpAudScrList,1);
	}

	public void init(List<AudioSource> para_tmpAudSrcList, float para_maxClampVolume)
	{
		tmpAudSrcList = para_tmpAudSrcList;
		maxClampVolume = para_maxClampVolume;

		GameObject tmpQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
		tmpQuad.name = "MaskQuad";
		Rect wBounds = WorldSpawnHelper.getGuiToWorldBounds(new Rect(0,0,Screen.width,Screen.height),Camera.main.transform.position.z + 1f,new bool[3] {false,true,false});
		tmpQuad.transform.position = new Vector3(wBounds.x + (wBounds.width/2f),wBounds.y - (wBounds.height/2f),Camera.main.transform.position.z + 1f);
		tmpQuad.transform.localScale = new Vector3(wBounds.width/tmpQuad.transform.renderer.bounds.size.x,wBounds.height/tmpQuad.transform.renderer.bounds.size.y,tmpQuad.transform.localScale.z);
		tmpQuad.renderer.material.color = Color.gray;

		// Popup sound.
		//SoundPlayerScript sps = GameObject.Find("PersistentObj").GetComponent<SoundPlayerScript>();
		SoundPlayerScript sps = transform.GetComponent<SoundPlayerScript>();
		sps.triggerSoundAtCamera("pop");
	}


	private void updateKnownAudSrces()
	{
		for(int i=0; i<tmpAudSrcList.Count; i++)
		{
			tmpAudSrcList[i].volume = (musicVolumePerc * maxClampVolume);
		}
	}



	private void prepUiBounds()
	{
		uiBounds = new Dictionary<string, Rect>();
		
		float popupWindow_Width = Screen.width * 0.75f;
		float popupWindow_Height = Screen.width * 0.3f;
		float popupWindow_X = (Screen.width/2f) - (popupWindow_Width/2f);
		float popupWindow_Y = (Screen.height/2f) - (popupWindow_Height/2f);


		float titleBar_X = popupWindow_X;
		float titleBar_Y = popupWindow_Y;
		float titleBar_Width = popupWindow_Width;
		float titleBar_Height = popupWindow_Height * 0.2f;
		
		float titleIcon_X = titleBar_X;
		float titleIcon_Y = titleBar_Y;
		float titleIcon_Width = titleBar_Height;
		float titleIcon_Height = titleBar_Height;
		
		float titleBarClose_Width = titleBar_Height;
		float titleBarClose_Height = titleBar_Height;
		float titleBarClose_X = titleBar_X + titleBar_Width - titleBarClose_Width;
		float titleBarClose_Y = titleBar_Y;
		
		float titleLabel_X = titleIcon_X + titleIcon_Width;
		float titleLabel_Y = titleIcon_Y;
		float titleLabel_Width = titleBar_Width - (titleIcon_Width + titleBarClose_Width);
		float titleLabel_Height = titleBar_Height;




		float padding = popupWindow_Height * 0.05f;



		float internalBox_X = titleBar_X + padding;
		float internalBox_Y = titleBar_Y + titleBar_Height;
		float internalBox_Width = (popupWindow_Width - (padding*2f));
		float internalBox_Height = (popupWindow_Height - titleBar_Height - (padding));
	

		float contentsBox_X = internalBox_X + padding;
		float contentsBox_Y = internalBox_Y + padding;
		float contentsBox_Width = internalBox_Width - (padding*2f);
		float contentsBox_Height = internalBox_Height - (padding*2f);


		float noteBox_X = contentsBox_X;
		float noteBox_Y = contentsBox_Y;
		float noteBox_Width = contentsBox_Width;
		float noteBox_Height = (contentsBox_Height - (padding*2f)) * 0.33f;

		float noteLabel_X = noteBox_X;
		float noteLabel_Y = noteBox_Y;
		float noteLabel_Width = noteBox_Width * 0.25f;
		float noteLabel_Height = noteBox_Height;

		float noteText_X = noteLabel_X + noteLabel_Width;
		float noteText_Y = noteBox_Y;
		float noteText_Width = noteBox_Width - noteLabel_Width;
		float noteText_Height = noteBox_Height;

		

		float musicPresetsBox_X = contentsBox_X;
		float musicPresetsBox_Y = noteBox_Y + noteBox_Height + padding;
		float musicPresetsBox_Width = contentsBox_Width;
		float musicPresetsBox_Height = (contentsBox_Height - padding) * 0.33f;

		float presetLabel_X = musicPresetsBox_X;
		float presetLabel_Y = musicPresetsBox_Y;
		float presetLabel_Width = musicPresetsBox_Width * 0.25f;
		float presetLabel_Height = musicPresetsBox_Height;
		 

		float iconWidth = (musicPresetsBox_Width - presetLabel_Width)/(musicVolumePresets.Length * 1.0f);

		float musicPresetIcon_X = presetLabel_X + presetLabel_Width;
		float musicPresetIcon_Y = musicPresetsBox_Y;
		float musicPresetIcon_Width = iconWidth;
		float musicPresetIcon_Height = musicPresetsBox_Height;
		for(int i=0; i<musicVolumePresets.Length; i++)
		{
			uiBounds.Add("MusicVolumePresetIcon-"+i,new Rect(musicPresetIcon_X,musicPresetIcon_Y,musicPresetIcon_Width,musicPresetIcon_Height));
			musicPresetIcon_X += musicPresetIcon_Width;
		}


		float musicVolumeSliderBox_X = contentsBox_X;
		float musicVolumeSliderBox_Y = musicPresetsBox_Y + musicPresetsBox_Height + padding;
		float musicVolumeSliderBox_Width = contentsBox_Width;
		float musicVolumeSliderBox_Height = (contentsBox_Height - padding) * 0.33f;

		float musicVolumeSliderLabel_X = musicVolumeSliderBox_X;
		float musicVolumeSliderLabel_Y = musicVolumeSliderBox_Y;
		float musicVolumeSliderLabel_Width = musicVolumeSliderBox_Width * 0.25f;
		float musicVolumeSliderLabel_Height = musicVolumeSliderBox_Height;

		float musicVolumeSlider_X = musicVolumeSliderLabel_X + musicVolumeSliderLabel_Width;
		float musicVolumeSlider_Y = musicVolumeSliderBox_Y;
		float musicVolumeSlider_Width = musicVolumeSliderBox_Width - musicVolumeSliderLabel_Width;
		float musicVolumeSlider_Height = musicVolumeSliderBox_Height;


		

		

		uiBounds.Add("PopupWindow",new Rect(popupWindow_X,popupWindow_Y,popupWindow_Width,popupWindow_Height));
		uiBounds.Add("TitleBar",new Rect(titleBar_X,titleBar_Y,titleBar_Width,titleBar_Height));
		uiBounds.Add("TitleBarIcon",new Rect(titleIcon_X,titleIcon_Y,titleIcon_Width,titleIcon_Height));
		uiBounds.Add("TitleBarText",new Rect(titleLabel_X,titleLabel_Y,titleLabel_Width,titleLabel_Height));
		uiBounds.Add("TitleBarCloseIcon",new Rect(titleBarClose_X,titleBarClose_Y,titleBarClose_Width,titleBarClose_Height));

		uiBounds.Add("InternalBox",new Rect(internalBox_X,internalBox_Y,internalBox_Width,internalBox_Height));
		uiBounds.Add("NoteLabel",new Rect(noteLabel_X,noteLabel_Y,noteLabel_Width,noteLabel_Height));
		uiBounds.Add("NoteText",new Rect(noteText_X,noteText_Y,noteText_Width,noteText_Height));
		uiBounds.Add("MusicPresetLabel",new Rect(presetLabel_X,presetLabel_Y,presetLabel_Width,presetLabel_Height));
		uiBounds.Add("MusicVolumeSliderLabel",new Rect(musicVolumeSliderLabel_X,musicVolumeSliderLabel_Y,musicVolumeSliderLabel_Width,musicVolumeSliderLabel_Height));
		uiBounds.Add("MusicVolumeSlider",new Rect(musicVolumeSlider_X,musicVolumeSlider_Y,musicVolumeSlider_Width,musicVolumeSlider_Height));
	}

	private void loadTextures()
	{
		availableTextures = new Dictionary<string, Texture2D>();
		
		Texture2D blackTex = new Texture2D(1,1);
		blackTex.SetPixel(0,0,Color.black);
		blackTex.Apply();

		Texture2D grayTex = new Texture2D(1,1);
		grayTex.SetPixel(0,0,Color.gray);
		grayTex.Apply();

		availableTextures.Add("BlackTex",blackTex);
		availableTextures.Add("GrayTex",grayTex);

		availableTextures.Add("CloseIcon",Resources.Load<Texture2D>("Textures/Ghostbook/UI/closeicon"));
		availableTextures.Add("MusicControlBox",(Texture2D) Resources.Load("Textures/Common/" + "music_control",typeof(Texture2D)));
	}

	private void prepGUIStyles()
	{
		availableGUIStyles = new Dictionary<string, GUIStyle>();
		
		GUIStyle largeCentredLabel = new GUIStyle(GUI.skin.label);
		largeCentredLabel.alignment = TextAnchor.MiddleCenter;
		largeCentredLabel.fontSize = 20;

		GUIStyle sliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
		//sliderStyle.normal.background = availableTextures["BlackTex"];
		//sliderStyle.active.background = availableTextures["BlackTex"];
		sliderStyle.fixedHeight = uiBounds["MusicVolumeSlider"].height;

		GUIStyle sliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);
		//sliderThumbStyle.normal.background = availableTextures["GrayTex"];
		//sliderThumbStyle.active.background = availableTextures["GrayTex"];
		sliderThumbStyle.fixedHeight = uiBounds["MusicVolumeSlider"].height;


		availableGUIStyles.Add("LargeCentredLabel",largeCentredLabel);
		availableGUIStyles.Add("SliderStyle",sliderStyle);
		availableGUIStyles.Add("SliderThumbStyle",sliderThumbStyle);
	}




	// Action Notifier Methods.
	IActionNotifier acNotifier = new ConcActionNotifier();
	public void registerListener(string para_name, CustomActionListener para_listener) { acNotifier.registerListener(para_name,para_listener); }
	public void unregisterListener(string para_name) { acNotifier.unregisterListener(para_name); }
	public void notifyAllListeners(string para_sourceID, string para_eventID, System.Object para_eventData)	{ acNotifier.notifyAllListeners(para_sourceID,para_eventID,para_eventData);	}
}

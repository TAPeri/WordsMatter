/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using UnityEngine;

public class DetectLeaks : MonoBehaviour 
{
	private static DetectLeaks instance;
	
	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this);
		}
	}
	
	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}
	
	void OnGUI() 
	{
		if(GUILayout.Button("Unload Unused Assets"))
		{
			Resources.UnloadUnusedAssets();
		}
		
		if(GUILayout.Button("Mono Garbage Collect"))
		{
			System.GC.Collect();    
		}
		
		if(GUILayout.Button("List Loaded Textures"))
		{   
			ListLoadedTextures();
		}
		
		if(GUILayout.Button("List Loaded Sounds"))
		{
			ListLoadedAudio();
		}
		
		if(GUILayout.Button("List Loaded GameObjects"))
		{
			ListLoadedGameObjects();
		}
	}
	
	
	private void ListLoadedTextures()
	{
		Object[] textures = Resources.FindObjectsOfTypeAll(typeof(Texture));
		
		string list = string.Empty;
		
		for(int i = 0; i < textures.Length; i++)
		{
			if(textures[i].name == string.Empty)
			{
				continue;
			}
			
			list += (i.ToString() + ". " + textures[i].name + "\n");
			
			if(i == 500)
			{
				Debug.Log(list);
				list = string.Empty;
			}
		}
		
		Debug.Log(list);
	}
	
	private void ListLoadedAudio()
	{
		Object[] sounds = Resources.FindObjectsOfTypeAll(typeof(AudioClip));
		
		string list = string.Empty;
		
		for(int i = 0; i < sounds.Length; i++)
		{   
			if(sounds[i].name == string.Empty)
			{
				continue;
			}
			list += (i.ToString() + ". " + sounds[i].name + "\n");
		}
		
		Debug.Log(list);
	}
	
	private void ListLoadedGameObjects()
	{
		Object[] gos = Resources.FindObjectsOfTypeAll(typeof(GameObject));
		
		string list = string.Empty;
		
		for(int i = 0; i < gos.Length; i++)
		{
			if(gos[i].name == string.Empty)
			{
				continue;
			}
			list += (i.ToString() + ". " + gos[i].name + "\n");
		}
		
		Debug.Log(list);
	}
}
/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TTS_googletranslate : MonoBehaviour,TTSinterface
{

	private string url = "http://translate.google.com/translate_tts";
	

	private Dictionary<string,AudioClip> cache = new Dictionary<string,AudioClip>();


	private List<string> requests = new List<string>();

	string language = "en";
	string exception;
	

	public void fetch(string[] text){

		for(int i=0;i<text.Length;i++){

			if(!cache.ContainsKey(TTS_android.translate(text[i])))
				if(!requests.Contains(TTS_android.translate(text[i]))){
					requests.Add(TTS_android.translate(text[i]));
					StartCoroutine(retrieve(TTS_android.translate(text[i]),i));
			
				}
		}
	}


	IEnumerator retrieve(string text,int i)
	{ 

		WWW www = new WWW (url + "?tl="+language+"&q="+text.Replace(" ","%20").Replace("/",""));//WWW.EscapeURL(text));
		//Debug.Log(url + "?tl="+language+"&q="+text.Replace(" ","%20").Replace("/",""));
		yield return www;
		exception += www.error;
		//Debug.LogError(exception);
		
		//AudioClip b = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
		AudioClip b = www.GetAudioClip(false, false, AudioType.MPEG);
		//AudioClip c = www.audioClip;
		//word.clip = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
		
		if(!cache.ContainsKey(text))
			cache.Add(text,b);

		www.Dispose();
		requests.Remove(text);

		yield return null;

	}


	public bool test(string text){

		if(text.Contains("/")){
			
			int idx = System.Array.IndexOf(TTS_android.SAMPA,TTS_android.translate(text).Replace("/",""));
			if(idx>-1){
				
				return (Resources.Load("Sounds/Phonemes/"+TTS_android.translate(text).Replace("/",""))!=null);
			}
			
		}/*else if(cache.ContainsKey(TTS_android.translate(text))){

			if(cache[TTS_android.translate(text)]!= null)
				if(cache[TTS_android.translate(text)].length>0)
					return true;
		}else if(requests.Contains(TTS_android.translate(text))){

			return true;

		}else{

			fetch(new string[]{text});
			return true;
		}*/

		return false;
	}


	public AudioClip say(string text){

		if(text.Contains("/")){
			
			int idx = System.Array.IndexOf(TTS_android.SAMPA,TTS_android.translate(text).Replace("/",""));
			if(idx>-1){
				
				return Resources.Load("Sounds/Phonemes/"+TTS_android.translate(text).Replace("/","")) as AudioClip;
			}
			
		}else if(cache.ContainsKey(TTS_android.translate(text))){
			return cache[TTS_android.translate(text)];
		}
		
		return null;
	}

	public void init(int pitchRate,int speechRate,string language){
		Debug.Log(language);
		if (language=="EN"){
			this.language = "en";
		}else if (language=="GR"){
			this.language = "el";
		}else
			this.language = language;

		List<string> initialWords = new List<string>();
		if(language=="EN")
			foreach(string a in TTS_android.letters_EN)
				initialWords.Add(a.Replace(".",""));
		
		else
			foreach(string a in TTS_android.letters_GR)
				initialWords.Add(a.Replace(".",""));
		


		this.fetch(initialWords.ToArray());

	}

	public void clearCache(){
		
		foreach(string key in cache.Keys){
			Destroy(cache[key]);
		}
		cache.Clear();
		
	}

	



	public bool loading(){
		return (requests.Count>0);
	}



}

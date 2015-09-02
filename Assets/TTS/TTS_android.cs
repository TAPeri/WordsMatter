/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TTS_android : MonoBehaviour,TTSinterface {

	static public string[] letters_EN = new string[]{"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"};
	static public string[] letters_GR = new string[]{"α","β","γ","δ","ε","ζ","η","θ","ι","κ","λ","μ","ν","ξ","ο","π","ρ","σ","τ","υ","φ","χ","ψ","ω"};
	//static string[] letters_EN_phonemes = new string[]{"/a/","/b/","/c/","/d/","/e/","/f/","/g/","/h/","/i/","/j/","/k/","/l/","/m/","/n/","/o/","/p/","/q/","/r/","/s/","/t/","/u/","/v/","/w/","/x/","/y/","/z/"};
	static string[] IPA = new string[]{"b","d","ð","dʒ","f","ɡ","h","j","k","l","m","n","ŋ","p","r","s","ʃ","t","θ","tʃ","v","w","z","ʒ", "ɜː","ə","əʊ","æ","ɑː","aɪ","ɑr","aʊ","ɛ","ɛər","eɪ","ɪ","iː","ɪər","juː","ɔː","ɔɪ","ɒ","ʊ","uː","ʊər","ʌ","iː","ɪn","aɪn","ʒɪn","ɪs","i_C_e","a_C_e","o_C_e","u_C_e","ice","e_C_e","y_C_e","aɪC","e"};//,"ɜr"
	//static string[] DILP = new string[]{"b","d","dh","j","f","g","h","y","k","l","m","n","ng","p","r","s","sh","t","th","ch","v","w","z","zh","e͡r","ə","ō","ă","ah","ī","a͡r","ow","ě","ār","ā","ǐ","ē","ēr","ū","aw","oi","ǒ","o͝o","o͞o","oor","ǔ"};
	//static string[] ilearn = new string[]{"b","d","TH","j","f","g","h","y","k","l","m","n","ng","p","r","s","sh","t","th","ch","v","w","z","zh","er","ə","oh","a","ah","aɪ","N/A","ow","e","air","ei","ɪ","ee","ier","yoo","aw","oi","ɒ","u","uu","oor","uh"};
	static public string[] SAMPA = new string[]{"b","d","D","dZ","f","g","h","j","k","l","m","n","N","p","r","s","S","t","Th","tS","v","w","z","Z","3,,","@","@U","{","A,,","aI","Ar","aU","E","E@","eI","I","i,,","I@","ju,,","O,,","OI","Q","U","u,,","U@","V","i,,","I","aI","I","I","aI","eI","@U","ju,,","I","i,,","aI","aI","e"};


	List<string> loadingWords;

	bool enabledTTS = false;



	public static string translate(string text){
		string aux = text.Replace(".",",").Replace("/","");
		if(System.Array.IndexOf(IPA,aux)>-1)
				aux = SAMPA[ System.Array.IndexOf(IPA,aux)];


		if(text.Contains("/"))
			return "/"+aux+"/";
		else
			return aux;


	}

	//cd Applications/adt-bundle-mac-x86_64-20140321/sdk/platform-tools/
	//./adb logcat -s Unity


	private Dictionary<string,string> clip = new Dictionary<string,string>();
	private Dictionary<string,AudioClip> cache = new Dictionary<string,AudioClip>();

	public void init(int pitchRate,int speechRate,string language){
		
		loadingWords = new List<string>();
		#if UNITY_ANDROID
		
		
		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.ilearnrw.wordsmatter.Main"))
		{
			using (AndroidJavaObject activity = javaClass.GetStatic<AndroidJavaObject>("mContext"))
			{
				if (language=="EN"){
					activity.Call("init",pitchRate,speechRate,"en_uk",Application.persistentDataPath);
					int stat = activity.Call<int>("isAvailable","en_uk");
					enabledTTS = (stat>-2);
					Debug.Log("Language available "+"en_uk "+stat);
					
				}else if (language=="GR"){
					activity.Call("init",pitchRate,speechRate,"el_gr",Application.persistentDataPath);
					int stat = activity.Call<int>("isAvailable","el_gr");
					enabledTTS = (stat>-2);
					Debug.Log("Language available "+"el_gr "+stat);
				}
				
			}
		}
		#endif
		
		if(!enabledTTS)
			return;
		
		List<string> initialWords = new List<string>();
		if(language=="EN")
			foreach(string a in letters_EN)
				initialWords.Add(a.Replace(".",""));
		
		else
			foreach(string a in letters_GR)
				initialWords.Add(a.Replace(".",""));
		

		this.fetch(initialWords.ToArray());
		
	}



	public void clearCache(){

		foreach(string key in cache.Keys){
			Destroy(cache[key]);
		}
		cache.Clear();

	}

	



	public bool test(string text){
		if(text.Contains("/")){
			
			int idx = System.Array.IndexOf(SAMPA,translate(text).Replace("/",""));
			if(idx>-1){
				
				return (say(text)!=null);
				
			}else{

				return false;
			}
			
		}else if(enabledTTS){

			if(loadingWords.Contains(translate(text))){

				return true;//not yet loaded, but getting there
			}else if(cache.ContainsKey(translate(text))){

				return true;//is ready
			}else if(clip.ContainsKey(translate(text))){

				StartCoroutine(addToCache(translate(text)));
				return true;//file exists but is not currently on the cache
			}else{
				fetch(new string[]{text});
				return true;//since TTS is working, the word is ordered...

			}

		}else
			return false;
	}


	public AudioClip say(string text){

		if(text.Contains("/")){

			int idx = System.Array.IndexOf(SAMPA,translate(text).Replace("/",""));
			if(idx>-1){

				return Resources.Load("Sounds/Phonemes/"+translate(text).Replace("/","")) as AudioClip;

			}

		}else if(cache.ContainsKey(translate(text))){
			return cache[translate(text)];
		}

		return null;

	}

/*	#if UNITY_ANDROID
	AndroidJavaClass javaClass;
	AndroidJavaObject activity;
	#endif*/


	public void fetch(string[] text){
		fetch(text,false);
	}
	
	
	public void fetch(string[] words, bool clearCache){
		if(!enabledTTS)
			return;

		List<string> text = new List<string>();
		List<string> toCache = new List<string>();
		for(int i = 0; i< words.Length;i++){
			if(words[i].Contains("/")){//Phonemes are prefabs
				continue;
			}else if(!loadingWords.Contains(translate(words[i]))){//not loading already

				if(!cache.ContainsKey(translate(words[i]))){//not loaded already

					if(!text.Contains(translate(words[i])) &&!toCache.Contains(translate(words[i])) ){//precaution for repeated text on the same batch

						if(clip.ContainsKey(translate(words[i]))){//file exists, simply load into cache
							toCache.Add(translate(words[i]));
							StartCoroutine(addToCache(translate(words[i])));

						}else{//synthesize new word

							text.Add(translate(words[i]));

						}
					}

				}
			}
		}
		

		if(text.Count>0){
			foreach(string f in text)
				loadingWords.Add(f);
			
			string[] file = sendTTSrequest(text.ToArray());//retrive the names of the files, and call the TTS synthesiser
			StartCoroutine(receiveTTSfilesAndLoadToCache(file,text.ToArray(),clearCache));//wait until Android is done, and loads the cache with sounds
		}
	}



	string[] sendTTSrequest(string[] text){

		#if UNITY_ANDROID

			using (AndroidJavaClass javaClass = new AndroidJavaClass("com.ilearnrw.wordsmatter.Main"))
			{
				using (AndroidJavaObject activity = javaClass.GetStatic<AndroidJavaObject>("mContext"))
				{

					return activity.Call<string[]>("say_store",text,"wav");
				}
			}
		#endif

		#if !UNITY_ANDROID
				return null;
		#endif


	}


	IEnumerator receiveTTSfilesAndLoadToCache(string[] files,string[] text,bool clearCache){

		loadingThreads++;

		#if UNITY_ANDROID

		using (AndroidJavaClass javaClass = new AndroidJavaClass("com.ilearnrw.wordsmatter.Main"))
		{
			using (AndroidJavaObject activity = javaClass.GetStatic<AndroidJavaObject>("mContext"))
			{
				while(! activity.Call<bool>("get_status")){
					//Debug.Log("Not yet");
					yield return new WaitForSeconds(0.2f); 
				}
			}
		}

		#endif
		#if !UNITY_ANDROID
		return null;
		#endif

		if(clearCache){
			foreach(string key in cache.Keys)
				if(cache[key]!=null)
					Destroy(cache[key]);

			cache.Clear();
		}

		for (int i=0;i<files.Length;i++){

			if(!clip.ContainsKey(text[i])){
				clip.Add(text[i],files[i]);
			}

			addToCache(text[i]);

			loadingWords.Remove(text[i]);
			//System.IO.File.Delete(files[i]);

		}

		loadingThreads--;

	}

	IEnumerator addToCache(string alias){

		if(!cache.ContainsKey(alias)){

			WWW www = new WWW("file://"+clip[alias]);
		
			yield return www;
		
			cache.Add(alias,ConvertWavToClip(www.bytes, clip[alias].Split('/')[ clip[alias].Split('/').Length-1  ] ));
		
			www.Dispose();
		}
		yield return null;
	}


	int loadingThreads = 0;



	public bool loading(){
		return (loadingWords.Count>0);
	}

	private AudioClip ConvertWavToClip(byte[] array,string name) 
	{

		//string riff = System.Text.Encoding.UTF8.GetString(array,0,4);
		//int chunkSize = System.BitConverter.ToInt32(array, 4);//16756
		//string wave = System.Text.Encoding.UTF8.GetString(array,8,4);

		//string fmt = System.Text.Encoding.UTF8.GetString(array,12,4);
		//int subChunk1 = System.BitConverter.ToInt32(array, 16);//16
		//int byteRate = System.BitConverter.ToInt32(array, 28);// bytes/second
		//string data = System.Text.Encoding.UTF8.GetString(array,36,4);

		try{
		int channels = System.BitConverter.ToUInt16(array, 22);//1 for mono, 2 for stereo
		int sampleRate = System.BitConverter.ToInt32(array, 24);//samples/second

		int blockAlign = System.BitConverter.ToUInt16(array, 32);//atomic unit of data (bytes per sample* channels)
		int bitsPerSample = System.BitConverter.ToUInt16(array, 34);

		int subChunk2 = System.BitConverter.ToInt32(array, 40);//size

		int bytesPerSample = bitsPerSample/8;


		AudioClip output = AudioClip.Create(name, subChunk2/bytesPerSample, channels, sampleRate, false,false);

		float[] floatArr = new float[subChunk2/bytesPerSample];
		int k = 0;
		int offset = 44;//Bytes on the header

		for (int i = offset; i < array.Length; i+=blockAlign) 
		{

			for (int j=0;j<channels;j++){
				if (bytesPerSample==2){
					floatArr[k] = System.BitConverter.ToInt16(array, (i+(j*bytesPerSample)))/(float)System.Int16.MaxValue;//32768f;//32760f;
				}else if(bytesPerSample==4){
					floatArr[k] = System.BitConverter.ToInt32(array, (i+(j*bytesPerSample)))/(float)System.Int32.MaxValue;//32768f;//32760f;
					//floatArr[k] = System.BitConverter.ToSingle(array, i * 4);/// 0x80000000;
				}
				k++;
				//if (System.BitConverter.IsLittleEndian) 
				//	System.Array.Reverse(array, i, 2);

			}
		}

		output.SetData(floatArr,0);

			return output;

		}catch(System.Exception ex){
			if(array!=null)
				Debug.Log("Audio length: "+array.Length);

			Debug.LogError(ex.Message+" "+name);
			return null;
		}
		/*message += 	"Length: "+floatArr.Length +
				"Frequency: "+output.frequency+ 
				" Samples: "+output.samples+
				" Channels: "+output.channels+
				" Min: "+MinValue(floatArr)+
				" Max: "+MaxValue(floatArr)+"\n";*/

//		return null;
	}

	//string exception;
	

}

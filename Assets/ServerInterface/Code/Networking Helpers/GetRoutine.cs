/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
//using System;
using System.Collections;
using System.Collections.Generic;
//using System.Text;
//using System.IO;

public enum RoutineStatus{ERROR,WAIT,READY,IDLE}

public abstract class GetRoutine{
	
	public abstract RoutineStatus status();
	public abstract string getError();
}


public class ConnectionError : GetRoutine{
	
	RoutineStatus flag = RoutineStatus.ERROR;
	string error;
	
	public override RoutineStatus status(){
		return flag;
	}
	
	public override string getError(){
		return error;
	}
	
	public ConnectionError(string _error, RoutineStatus _flag){
		flag  = _flag;
		error = _error;
		//Debug.Log(server_url);
	}
}


public class GetRoutine<T> : GetRoutine {
	
	
	string error = "";
	T package;
	RoutineStatus flag = RoutineStatus.IDLE;
	string server_url;
	WWW www;
	
	private float elapsedTime = -1f;
	
	string auth_username = "api";
	string auth_password = "api";
	
	List<string> requests;
	List<string> responses;
	
	bool verbose = false;
	
	public GetRoutine(string _server_url,List<string> requests,List<string> responses){
		this.requests = requests;
		this.responses = responses;
		
		if (this.requests!=null & this.responses!=null){
			
			this.verbose = true;
		}
		
		server_url = _server_url;
		//Debug.Log(server_url);
	}
	
	private Hashtable buildHeader()
	{
		Hashtable headers = new Hashtable();
		headers.Add("Authorization","Basic "+System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(auth_username+":"+auth_password)));
		headers.Add("content-type","application/json");
		return headers;
	}
	
	public override RoutineStatus status(){
		
		if (elapsedTime>-1){
			
			if (Time.time-elapsedTime>60.0){//if we have waited for more than 20 seconds
				Debug.LogError("Too long!");
				
				www.Dispose();
				package = default(T);
				error = "Connection lost: more than 60 seconds waiting for reply";
				WorldViewServerCommunication.setError("Connection lost: more than 60 seconds waiting for reply");

				flag = RoutineStatus.ERROR;	
				if(verbose){
					responses.Add(error);
				}
			}
		}
		return flag;
		
	}
	
	public override string getError(){
		return error;
	}
	
	public T getPackage(){
		
		if (flag == RoutineStatus.READY){
			flag = RoutineStatus.IDLE;
		}
		return package;
		
	}
	
	
	public IEnumerator get(string parameters,bool verbose){
		
		www = new WWW(server_url+parameters,null,buildHeader());	
		flag = RoutineStatus.WAIT;
		
		if(verbose){
			requests.Add("Get:"+server_url+parameters);
			UnityEngine.Debug.LogWarning("Get:"+server_url+parameters);
		}
		
		elapsedTime = Time.time;
		yield return www;
		elapsedTime = -1.0f;
		
		
		
		
		if(www.error == null){
			
			string requestedText = "";
			
			try{
				requestedText = www.text;//.Replace("\"2014-06-16T14:10:34+02:00\"","1399238616000");//.Replace("1399238616000","\""+System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")+"\"").Replace("1402671780000","\""+System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")+"\"").Replace("1402918241000","\""+System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")+"\"");
				
				
			}catch(UnityException e){
				Debug.LogError("Get could not retrieve the text: "+server_url+parameters);

				WorldViewServerCommunication.setError(e.Message+"\n"+server_url+parameters);

				if(verbose){responses.Add("Fail:"+e.Message);}
				
			}
			
			
			//yield return new WaitForSeconds(5);
			try{
				package = JsonHelper.deserialiseObject<T>(requestedText);
				error = "0 Success";
				flag = RoutineStatus.READY;
				
				if(verbose){
					responses.Add("Ok:"+requestedText);
				}
				//Debug.LogWarning(requestedText);
				
			}catch{
				
				//Debug.LogError("Not the expected JSON structure: "+requestedText);
				//package = JsonHelper.deserialiseObject<T>(www.text);
				flag = RoutineStatus.ERROR;
				error = "Not the expected JSON structure: "+requestedText;
				if(verbose){
					responses.Add("Fail:"+error);
				}

				WorldViewServerCommunication.setError(error+"\n"+server_url+parameters);


			}
			
			
			
		}else{
			package=default(T);

			WorldViewServerCommunication.setError("Get failed: "+www.error+"\n"+server_url+parameters);
			Debug.LogError(server_url+parameters);
			error = www.error;
			flag = RoutineStatus.ERROR;
			if(verbose){
				responses.Add("Fail:"+error);
			}
			
		}
		
		
	}
	
	
	public IEnumerator get(string parameters){
		
		return get(parameters,this.verbose);
	}
	
	
	public IEnumerator post(string parameters, string jsonStr){
		
		return post (parameters,jsonStr,this.verbose);
		
	}
	
	public IEnumerator post(string parameters, string jsonStr, bool verbose){
		
		byte[] dataArr = System.Text.Encoding.Unicode.GetBytes(jsonStr);
		
		Hashtable headers = buildHeader();
		
		foreach(string header in parameters.Replace("?","").Split('&')){
			headers.Add(header.Split('=')[0],header.Split('=')[1]);
		}
		
		
		www = new WWW(server_url+parameters ,dataArr,headers);
		
		if(verbose){
			requests.Add("Post:"+ server_url+parameters+" JSON:"+jsonStr);
			Debug.LogWarning("Post:"+ server_url+parameters+" JSON:"+jsonStr);
		}
		
		elapsedTime = Time.time;
		yield return www;
		elapsedTime = -1f;
		
		
		if(www.error == null){
			
			//package = JsonHelper.deserialiseObject<T>(www.text);
			//Debug.Log("Post OK "+www.text);
			/*flag = RoutineStatus.READY;
			
			if(verbose){
				responses.Add("Ok:"+ www.text);
				Debug.LogWarning("Ok:"+ www.text);
			}
			Debug.LogWarning("Ok:"+ www.text);
			*/
			string text = "nothing";
			try{
				text = www.text;
				package = JsonHelper.deserialiseObject<T>(text);
				error = "0 Success";
				flag = RoutineStatus.READY;
				
				if(verbose){
					responses.Add("Ok:"+www.text);
				}
				
			}catch(System.Exception ex){
				
				//Debug.LogError("Not the expected JSON structure: "+requestedText);
				//package = JsonHelper.deserialiseObject<T>(www.text);
				flag = RoutineStatus.ERROR;
				error = "Not the expected JSON structure: "+text+" "+ex.ToString();
				WorldViewServerCommunication.setError("Not the expected JSON structure: "+text+" "+ex.ToString());

				if(verbose){
					responses.Add("Fail:"+error);
				}


				Debug.LogError("Fail Post:"+ server_url+parameters+" "+error);
				
			}

			
		}else{
			package=default(T);
			//Debug.LogError("Post failed "+server_url+" "+www.error);
			WorldViewServerCommunication.setError("Post failed: "+www.error+"\n"+server_url+parameters);

			Debug.LogError(jsonStr);
			error = www.error;
			flag = RoutineStatus.ERROR;
			
			if(verbose){
				responses.Add("Fail:"+ www.error);
				Debug.LogWarning("Fail:"+ www.error);
			}
			
		}
		
		
	}
	
	
	
	
}

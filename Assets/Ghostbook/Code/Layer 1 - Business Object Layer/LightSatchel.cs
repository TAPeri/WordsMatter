/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

[System.Serializable]
public class LightSatchel{ 
	
	List<byte> unlockedClusters;
	byte[] unlockedBios;//Number of bios per character
	List<Photo>[][] photos;


	public LightSatchel(int numberCharacters,Difficulty[][] profile){
		unlockedClusters = new List<byte>();//no clusters unlocked
		unlockedBios = new byte[numberCharacters];//all start at 0

		photos = new List<Photo>[profile.Length][];
		for(int i=0;i<photos.Length;i++){
			photos[i] = new List<Photo>[profile[i].Length];
			for(int j = 0;j<profile[i].Length;j++){

				photos[i][j] = new List<Photo>();
			}
		}

	}


	public List<Photo>[][] getPhotos(){
		return photos;
	}

	public List<byte> getUnlockedClusters(){
		return unlockedClusters;
	}


	public int unlockBio(int character){
		if(unlockedBios[character]<7)
			unlockedBios[character]++;
		return unlockedBios[character];
	}

	public bool isBioUnlocked(int character,int section){
		if(section<=unlockedBios[character])
			return true;
		else
			return false;
	}

	public bool unlockCluster(int cluster){

		if(!unlockedClusters.Contains((byte)cluster)){
			unlockedClusters.Add((byte)cluster);
			return true;
		}else{
			return false;
		}
	}






}

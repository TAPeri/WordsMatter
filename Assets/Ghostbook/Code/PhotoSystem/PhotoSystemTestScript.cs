/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class PhotoSystemTestScript : MonoBehaviour
{


	void Start()
	{
		GameObject photoDestinationGuide = GameObject.Find("PhotoDestination");
		PhotoVisualiser photoVisualiser = new PhotoVisualiser();
		
		ClothingConfig playerClothingConfig = new ClothingConfig();
		playerClothingConfig.setClothing("Head","AV010");
		playerClothingConfig.setClothing("Body","AV04");
		playerClothingConfig.setClothing("Leg","AV01");
		
		Photo tmpPhotoDetails = new Photo((ApplicationID)System.Enum.ToObject(typeof(ApplicationID),0),0,
		                                  new PhotoCharacterElement(8,Random.Range(1,4),null),
		                                  new PhotoCharacterElement(0,Random.Range(1,4),null),
		                                  new PlayerAvatarSettings("Male",playerClothingConfig),
		                                  Random.Range(1,4),
		                                  "Test Photo",
		                                  null);
		
		photoVisualiser.producePhotoRender("TestPhoto",tmpPhotoDetails,0.05f,photoDestinationGuide,true);
	}

}
/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class ContactPageInfoSnippit
{
	ContactPortraitSnippit portraitSnippit;
	string shortDescription;
	PhotoAlbum photoAlbum;

	public ContactPageInfoSnippit(ContactPortraitSnippit para_portraitSnippit,
	                              				 string para_shortDescription,
	                              				 PhotoAlbum para_photoAlbum)
	{
		portraitSnippit = para_portraitSnippit;
		shortDescription = para_shortDescription;
		photoAlbum = para_photoAlbum;
	}

	public int getCharID() { return portraitSnippit.charID; }
	public string getName() { return portraitSnippit.name; }
	public CharacterStatus getStatus() { return portraitSnippit.status; }
	public string getShortDescription() { return shortDescription; }
	public PhotoAlbum getPhotoAlbum() { return photoAlbum; }
}
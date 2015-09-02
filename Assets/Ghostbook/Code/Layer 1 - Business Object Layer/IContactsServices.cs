/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */
using System.Collections.Generic;

public interface IContactsServices
{
	List<ContactPortraitSnippit> getContactPortraitSnippitsInOrder();
	ContactPageInfoSnippit getContactPageInfoSnippit(int para_charID);
	//List<string> getContactDetailedBio(int para_charID);
	void unlockCharacter(int para_char);
	CharacterStatus getContactStatus(int para_charID);
}

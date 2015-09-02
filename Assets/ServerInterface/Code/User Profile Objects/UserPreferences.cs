/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */[System.Serializable]

public class UserPreferences {
	
		public int fontSize;

		public UserPreferences()
		{
			// Empty constructor required for JSON converter.
		}
		
		public int getFontSize() { return fontSize; }

}
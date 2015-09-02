/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */[System.Serializable]


public class LanguageAreaCategory{
	
	// Use this for initialization
	public LanguageAreaCategory(){
		
	}

	public string url;//always to lowercase
	
	public LanguageAreaCategory(string url) {
		this.url = url.ToLower().Trim();
	}
	public string getUrl() {
		return url;
	}
	public void setUrl(string url) {
		this.url = url.ToLower().Trim();
	}
	
/*	@Override
	public bool equals(Object obj) {
		Category c = (Category)obj;
		return c.getUrl().trim().equalsIgnoreCase(this.url.trim());
	}*/

	public bool isSubCategory(LanguageAreaCategory c){
		return url.StartsWith(c.getUrl()) && !(c.getUrl()).StartsWith(url);
	}
	
}
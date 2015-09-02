/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections.Generic;

public class ClothingCatalog
{
	List<string> availableHeadGear;
	List<string> availableBodyGear;
	List<string> availableLegGear;

	public ClothingCatalog()
	{
		availableHeadGear = new List<string>();
		availableBodyGear = new List<string>();
		availableLegGear = new List<string>();
	}

	public void addHeadGear(string para_headGearName) { availableHeadGear.Add(para_headGearName); }
	public void addBodyGear(string para_bodyGearName) { availableBodyGear.Add(para_bodyGearName); }
	public void addLegGear(string para_legGearName) { availableLegGear.Add(para_legGearName); }

	public ClothingCatalogIterator getHeadGearIterator() { return (new ClothingCatalogIterator("Head",availableHeadGear)); }
	public ClothingCatalogIterator getBodyGearIterator() { return (new ClothingCatalogIterator("Body",availableBodyGear)); }
	public ClothingCatalogIterator getLegGearIterator() { return (new ClothingCatalogIterator("Leg",availableLegGear)); }

	public ClothingCatalogIterator getSpecialisedHeadGearIterator(int para_startIndex, int para_incStep) { return (new SpecialCatalogIterator("Head",availableHeadGear,para_startIndex,para_incStep)); }
}
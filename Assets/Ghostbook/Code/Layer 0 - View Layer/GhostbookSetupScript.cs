/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class GhostbookSetupScript : MonoBehaviour
{


	void Update()
	{
		Debug.LogError("DEPRECATED");
		/*GameObject poRef = PersistentObjMang.getInstance();


		GhostBookDisplay ngbd_src = transform.gameObject.GetComponent<GhostBookDisplay>();
		GhostBookDisplay ngbd_dest = poRef.AddComponent<GhostBookDisplay>();

		ngbd_dest.contactsTabPrefab = ngbd_src.contactsTabPrefab;

//		ngbd_dest.detailedContactTabPrefab = ngbd_src.detailedContactTabPrefab;


		ngbd_dest.eventsTabPrefab = ngbd_src.eventsTabPrefab;
		ngbd_dest.newsFeedTabPrefab = ngbd_src.newsFeedTabPrefab;
		ngbd_dest.ghostBookWordBoxPrefab = ngbd_src.ghostBookWordBoxPrefab;
		ngbd_dest.textBannerNarrowPrefab = ngbd_src.textBannerNarrowPrefab;
		ngbd_dest.textBannerThickPrefab = ngbd_src.textBannerThickPrefab;
		ngbd_dest.newsIconPrefab = ngbd_src.newsIconPrefab;

		ngbd_dest.unlockedCharacterFaces = ngbd_src.unlockedCharacterFaces;
		ngbd_dest.lockedCharacterFaces = ngbd_src.lockedCharacterFaces;
		ngbd_dest.characterSmallFaces = ngbd_src.characterSmallFaces;
		ngbd_dest.activitySymbols = ngbd_src.activitySymbols;

		ngbd_dest.photoPageCompletionStatusIcons = ngbd_src.photoPageCompletionStatusIcons;

		ngbd_dest.albumInnerPrefab = ngbd_src.albumInnerPrefab;

		ngbd_dest.tmpSprite = ngbd_src.tmpSprite;
		ngbd_dest.maskShaderMaterial = ngbd_src.maskShaderMaterial;
		ngbd_dest.maskShaderMaterialForText = ngbd_src.maskShaderMaterialForText;

		ngbd_dest.setVisibility(false);*/

		Destroy(this.gameObject);
	}
}

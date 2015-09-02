/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class RefreshTextMesh : MonoBehaviour
{
	void Start ()
	{
		TextMesh t = (TextMesh) this.GetComponent(typeof(TextMesh));
		t.richText = false;
		t.richText = true;
		Destroy (this);
	}
}

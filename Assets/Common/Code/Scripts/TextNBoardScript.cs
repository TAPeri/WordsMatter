/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;

public class TextNBoardScript : MonoBehaviour
{
	
	public bool spin_reveal;
	public bool spin_hide;
	float spinAnglePerSecSpeed = 360;
	Vector3 angles;

	public bool destroyAfterInit;
	
	
	void Start()
	{
		spin_reveal = false;
		spin_hide = false;
	}
	
	void Update()
	{
		if(destroyAfterInit)
		{
			Destroy(this);
		}

		if(spin_reveal)
		{
			float diff = (spinAnglePerSecSpeed * Time.deltaTime);
			angles.x += diff;
			transform.eulerAngles = angles;
			
			if(angles.x >= 180)
			{
				angles.x = 180;
				transform.eulerAngles = angles;
				spin_reveal = false;
			}
		}
		else if(spin_hide)
		{
			float diff = (spinAnglePerSecSpeed * Time.deltaTime);
			angles.x -= diff;
			transform.eulerAngles = angles;
			
			if(angles.x <= 0)
			{
				angles.x = 0;
				transform.eulerAngles = angles;
				spin_hide = false;
			}	
		}
	}
	
	
	
	public void init(string para_text, Color para_textColor, Color para_boardColor)
	{
		init(para_text,para_textColor,para_boardColor,null);	
	}
	
	public void init(string para_text, Color para_textColor, string para_boardTextureName)
	{
		Texture2D reqTex = (Texture2D) Resources.Load(para_boardTextureName,typeof(Texture2D));
		init(para_text,para_textColor,Color.black,reqTex);	
	}
	
	public void init(string para_text, Color para_textColor, Texture2D para_boardTexture)
	{
		init(para_text,para_textColor,Color.black,para_boardTexture);	
	}

	public string getText()
	{
		Transform textChild = transform.FindChild("Text");
		TextMesh tMesh = (TextMesh) textChild.GetComponent(typeof(TextMesh));
		return tMesh.text;
	}
	
	public void setText(string para_nwText)
	{
		Transform textChild = transform.FindChild("Text");
		TextMesh tMesh = (TextMesh) textChild.GetComponent(typeof(TextMesh));
		tMesh.text = para_nwText;
	}
	
	public void setTextColor(Color para_nwTextColor)
	{
		Transform textChild = transform.FindChild("Text");
		TextMesh tMesh = (TextMesh) textChild.GetComponent(typeof(TextMesh));
		
		// Init both text mesh script colour and the material colour. IMP: Set to the same value.
		tMesh.color = para_nwTextColor;
		tMesh.renderer.material.color = para_nwTextColor;
	}

	public void setBoardColor(int para_r255, int para_g255, int para_b255, int para_a255)
	{
		setBoardColor(new Color( para_r255/255f, para_g255/255f, para_b255/255f, para_a255/255f));
	}

	public void setBoardColor(float para_r1, float para_g1, float para_b1, float para_a1)
	{
		setBoardColor(new Color(para_r1,para_g1,para_b1,para_a1));
	}

	public void setBoardColor(Color para_nwBoardColor)
	{
		Transform boardChild = transform.FindChild("Board");
		boardChild.renderer.material.color = para_nwBoardColor;
	}
	
	public void setBoardTexture(Texture2D para_nwBoardTexture)
	{
		Transform boardChild = transform.FindChild("Board");
		boardChild.renderer.material.mainTexture = para_nwBoardTexture;
	}
	
	public void setBoardTexture(string para_nwBoardTextureName)
	{
		Transform boardChild = transform.FindChild("Board");
		Texture2D reqTex = (Texture2D) Resources.Load(para_nwBoardTextureName,typeof(Texture2D));
		boardChild.renderer.material.mainTexture = reqTex;
	}
	
	public void triggerDefault_CorrectStatusVisuals()
	{
		//this.setTextColor(Color.white);
		this.setBoardColor(Color.green);
	}
	
	public void triggerDefault_WrongStatusVisuals()
	{
		this.setTextColor(Color.white);
		this.setBoardColor(Color.red);
	}
	
	
	public void spinAndReveal()
	{
		spin_reveal = true;
		spin_hide = false;
		
		angles = transform.eulerAngles;
		angles.x = 0;
	}
	
	public void spinAndHide()
	{
		spin_reveal = false;
		spin_hide = true;
		
		angles = transform.eulerAngles;
		angles.x = 180;
	}	
		
	private void init(string para_text,
	                  Color para_textColor,
	                  Color para_boardColor,
	                  Texture2D para_boardTexture)
	{
		Transform textChild = transform.FindChild("Text");
		TextMesh tMesh = (TextMesh) textChild.GetComponent(typeof(TextMesh));
		tMesh.text = para_text;
		// Init both text mesh script colour and the material colour. IMP: Set to the same value.
		tMesh.color = para_textColor;
		tMesh.renderer.material.color = para_textColor;
		
		Transform boardChild = transform.FindChild("Board");

		boardChild.localScale = new Vector3((1.5f*(para_text.Length)) * (boardChild.renderer.bounds.extents.x),1,(textChild.renderer.bounds.extents.z)/(boardChild.renderer.bounds.extents.z));
		if(para_boardTexture == null)
		{
			boardChild.renderer.material.color = para_boardColor;
		}
		else
		{
			boardChild.renderer.material.mainTexture = para_boardTexture;
		}
	}
}
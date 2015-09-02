/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

public class ColorUtils
{

	public static UnityEngine.Color convertColor(int[] para_vals)
	{
		if(para_vals.Length == 3)
		{
			return convertColor(para_vals[0],para_vals[1],para_vals[2],255); // a:255 = solid colour
		}
		else if(para_vals.Length == 4)
		{
			return convertColor(para_vals[0],para_vals[1],para_vals[2],para_vals[3]);
		}
		else
		{
			return UnityEngine.Color.magenta;
		}							
	}

	public static UnityEngine.Color convertColor(int para_r, int para_g, int para_b)
	{
		return convertColor(para_r,para_g,para_b,255);
	}

	public static UnityEngine.Color convertColor(int para_r, int para_g, int para_b, int para_a)
	{
		return (new UnityEngine.Color(para_r/255.0f,para_g/255.0f,para_b/255.0f,para_a/255.0f)); 
	}
}

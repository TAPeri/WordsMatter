/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */

// This class code was written by user Dafu : http://forum.unity3d.com/threads/fancylabel-multicolor-and-multifont-label.9549/
// Accessed for ILearnRW: 22nd Sept 2014.


using UnityEngine;
using System.Collections;

public class FancyLabelTools
{





	// Recognized color string sequences:
	// #aabbccdd - Change current color to the one specified by the hex string
	//             red green blue alpha
	// #!        - Revert back to the original color that was used before this function call
	// #n        - normal font
	// #x        - bold font
	// #i        - italic font
	public void FancyLabel( Rect rect, string text,
	                       Font normalFont, Font boldFont, Font italicFont,
	                       TextAlignment alignment,
	                       GUIStyle labelGStyle) {
		int     i1 = 0, i2 = 0;
		bool    done = false;
		bool    newLine = false;
		Color   originalColor = GUI.contentColor;
		Color   textColor = new Color( originalColor.r,
		                              originalColor.g,
		                              originalColor.b,
		                              originalColor.a );
		
		//bool    leftSpace = false, rightSpace = false, topSpace = false, bottomSpace = false;
		
		Font    defaultFont = GUI.skin.font;
		Font    newFont = null;
		
		GUIStyle fontStyle = new GUIStyle(labelGStyle);//new GUIStyle(testStyle);
		
		// Start with normal font
		if ( normalFont != null ) {
			fontStyle.font = normalFont;
		} else {
			fontStyle.font = defaultFont;          
		}
		
		// NOTE: Lowering this padding reduces the line spacing
		// May need to adjust per font
		fontStyle.padding.bottom = -5;
		
		//GUILayout.BeginArea( rect );
		GUILayout.BeginVertical( GUILayout.ExpandHeight( true ),
		                        GUILayout.Width( rect.height ),
		                        GUILayout.MinWidth( rect.height ) );
		GUILayout.BeginHorizontal( GUILayout.ExpandWidth( true ),
		                          GUILayout.Width( rect.width ),
		                          GUILayout.MinWidth( rect.width ) );
		// Insert flexible space on the left if Center or Right aligned
		if ( alignment == TextAlignment.Right || alignment == TextAlignment.Center) {
			GUILayout.FlexibleSpace();
		}
		
		while ( !done ) {
			
			int skipChars = 0;
			int firstEscape, firstNewline;
			//int firstDoubleEscape;

			firstEscape = text.IndexOf("#", i2);
			firstNewline = text.IndexOf("\n", i2);
			
			if ( (firstEscape != -1) && ( firstNewline == -1 || firstEscape < firstNewline ) ) {
				i1 = firstEscape;
			} else {
				i1 = firstNewline;
			}
			
			// We're at the end, set the index to the end of the
			// string and signal an end
			if ( i1 == -1 ) {
				i1 = text.Length - 1;
				done = true;
			}
			
			fontStyle.normal.textColor = textColor;
			if ( newFont != null ) {
				fontStyle.font = newFont;
				newFont = null;
			}
			
			// If the next character is # then we have a ## sequence
			// We want to point one of the # so advance the index by
			// one to include the first #
			if ( !done ) {
				if ( text.Substring( i1, 1 ) == "#" ) {
					if ( ((text.Length - i1) >= 2) && (text.Substring(i1 + 1, 1) == "#" )) {
						skipChars = 2;
					}
					// Revert to original color sequence
					else if (  ((text.Length - i1) >= 2) &&  (text.Substring(i1 + 1, 1) == "!" )) {
						textColor = new Color( originalColor.r,
						                      originalColor.g,
						                      originalColor.b,
						                      originalColor.a );
						i1--;
						skipChars = 3;
					}
					// Set normal font
					else if (  ((text.Length - i1) >= 2) &&  (text.Substring(i1 + 1, 1) == "n" )) {
						if ( normalFont != null ) {
							newFont = normalFont;
						} else {
							newFont = defaultFont;
						}
						i1--;
						skipChars = 3;
					}
					// Set bold font
					else if (  ((text.Length - i1) >= 2) &&  (text.Substring(i1 + 1, 1) == "x" )) {
						if ( boldFont != null ) {
							newFont = boldFont;
						} else {
							newFont = defaultFont;
						}
						i1--;
						skipChars = 3;
					}
					// Set italic font
					else if (  ((text.Length - i1) >= 2) &&  (text.Substring(i1 + 1, 1) == "i" )) {
						if ( italicFont != null ) {
							newFont = italicFont;
						} else {
							newFont = defaultFont;
						}
						i1--;
						skipChars = 3;
					}
					//  New color sequence
					else if ( (text.Length - i1) >= 10 ) {
						string rText = text.Substring( i1 + 1, 2 );
						string gText = text.Substring( i1 + 3, 2 );
						string bText = text.Substring( i1 + 5, 2 );
						string aText = text.Substring( i1 + 7, 2 );
						
						float r = HexStringToInt( rText ) / 255.0f;
						float g = HexStringToInt( gText ) / 255.0f;
						float b = HexStringToInt( bText ) / 255.0f;
						float a = HexStringToInt( aText ) / 255.0f;
						
						if ( r < 0 || g < 0 || b < 0 || a < 0 ) {
							Debug.Log("Invalid color sequence");
							return;
						}
						
						textColor = new Color( r, g, b, a );
						skipChars = 10;
						// Move back one character so that we don't print the #
						i1--;
					} else {
						Debug.Log("Invalid # escape sequence");
						return;
					}
				} else if ( ((text.Length - i1) >= 1) &&  (text.Substring( i1, 1 ) == "\n" )) {
					newLine = true;
					i1--;
					skipChars = 2;
				} else {
					Debug.Log("Invalid escape sequence");
					return;
				}
			}
			
			string textPiece = text.Substring( i2, i1 - i2 + 1 );          
			GUILayout.Label( textPiece, fontStyle );
			
			// Unity seems to cut off the trailing spaces in the label, he have
			// to add them manually here
			// Figure out how many trailing spaces there are
			int spaces = textPiece.Length - textPiece.TrimEnd(' ').Length;
			
			// NOTE: Add the proper amount of gap for trailing spaces.
			// the length of space is a questimate here,
			// may need to be adjusted for different fonts
			GUILayout.Space( spaces * 5.0f );
			
			if ( newLine ) {
				// Create a new line by ending the horizontal layout
				if ( alignment == TextAlignment.Left || alignment == TextAlignment.Center) {
					GUILayout.FlexibleSpace();
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal( GUILayout.ExpandWidth( true ),
				                          GUILayout.Width( rect.width ),
				                          GUILayout.MinWidth( rect.width ) );         
				if ( alignment == TextAlignment.Right || alignment == TextAlignment.Center) {
					GUILayout.FlexibleSpace();
				}
				newLine = false;
			}
			
			// Store the last index
			i2 = i1 + skipChars;
		}
		if ( alignment == TextAlignment.Left || alignment == TextAlignment.Center) {
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		//GUILayout.EndArea();       
	}


	int HexStringToInt(string hexString) {

		int value = 0;
		int digitValue = 1;
		hexString = hexString.ToUpper();
		char[] hexDigits = hexString.ToCharArray(0, hexString.Length);
		
		for ( int i = hexString.Length - 1; i >= 0; i-- ) {
			int digit = 0;
			if ( (hexDigits[i] >= '0') && (hexDigits[i] <= '9' )) {
				digit = hexDigits[i] - '0';
			} else if ( (hexDigits[i] >= 'A') &&  (hexDigits[i] <= 'F' )) {
				digit = hexDigits[i] - 'A' + 10;               
			} else {
				// Not a hex string
				return -1;
			}
			
			value += digit * digitValue;
			digitValue *= 16;
		}

		return value;
	}




	/*// EXAMPLE:

	string text = "";
	text += "#FFDDDDFF"; // Light red
	text += "Fireball";
	text += "#!"; // revert to default color
	text += "\n\nHurls a ball of fire that ";
	text += "#x"; // bold font
	text += "explodes";
	text += "#n"; // normal font
	text += " on contact\n";
	text +="and damages all nearby enemies.\n\n";
	text += "#FF6666FF"; // red
	text += "#x"; // bold font
	text += "8";
	text += "#!"; // revert to default color
	text += "#n"; // normal font
	text += " to ";
	text += "#FF6666FF"; // red
	text += "#x"; // bold font
	text += "12";
	text += "#n"; // normal font
	text += "#!"; // revert to default color
	text += "#i"; // italic font
	text += " fire";
	text += "#n"; // normal font
	text += " damage";
	
	Globals.gGUIGlobal().FancyLabel( labelRect,
	                                text,
	                                Globals.gGUIGlobal().defaultFont,                                       Globals.gGUIGlobal().defaultBoldFont,
	                                Globals.gGUIGlobal().defaultItalicFont,
	                                TextAlignment.Center);*/


}

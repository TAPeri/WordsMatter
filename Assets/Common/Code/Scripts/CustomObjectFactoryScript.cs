/*
 * Copyright (c) 2015, iLearnRW. Licensed under Modified BSD Licence. See licence.txt for details.
 */using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomObjectFactoryScript : MonoBehaviour 
{
	const float segmentHeight = 8;
	
	Dictionary<string,Texture2D> loadedTextures;
	
    public Transform testElement;
	


    private GameObject render3DObj(string para_ObjName,
	                               string para_ParentName,                // NULL or "" mean no parent.
	                               UnityEngine.Color para_RenderColor,
								   Texture2D para_Tex,
	                               List<Vector3> para_ObjVertices,
	                               int para_NumOfVertsOnSameEdge)
    {			
		//float unitScale = 1;
		
		// Initial empty game object and empty mesh setup.
		GameObject nwGObj = new GameObject();
		nwGObj.name = para_ObjName;
		nwGObj.AddComponent(typeof(MeshRenderer));
		
		
		

        //nwGObj.renderer.material.mainTexture = (Texture2D) Resources.Load("floorTexEg", typeof(Texture2D));
        //nwGObj.renderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
		
	
		nwGObj.AddComponent(typeof(MeshFilter));
		MeshFilter meshFilter = (MeshFilter) nwGObj.GetComponent(typeof(MeshFilter));
		
		if (meshFilter==null)
		{
			nwGObj.AddComponent(typeof(MeshFilter));
			meshFilter = (MeshFilter) nwGObj.GetComponent(typeof(MeshFilter));
		}


		
				
		// Create Vertices
		List<Vector3> vertList = new List<Vector3>();
		vertList.AddRange(para_ObjVertices);
	
		
		
		
		
		
		
		/*// Shift every vertex so that the origin of the model mesh is World Pt (0,0,0)
		float averageCentreZ = ((highestColCeiling+lowestColFloor)/(2*1.0f))*unitScale;
		Vector3 shiftDownVect = new Vector3(0,0,-averageCentreZ);
		
		Vector3 shiftDepthVect = new Vector3(0,(float) (para_Thickness/2f),0);
		Vector3 centringVect = shiftLeftVect + shiftDownVect + shiftDepthVect;
				
		for(int i=0; i<vertList.Count; i++)
		{
			vertList[i] = vertList[i] + centringVect;	
		}*/
			
		
		
		// Add vertices to mesh vertex buffer.
		Mesh mesh = meshFilter.mesh;
		if (mesh == null)
		{
			meshFilter.mesh = new Mesh();
			mesh = meshFilter.mesh;
		}
		
		mesh.Clear();
		mesh.vertices = vertList.ToArray();


        //mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };
	
			
		// Create triangles (Via Index Buffer).
		int numOfTrianglesNeeded = vertList.Count - 2;
		int[] tmpTriangleArr = new int[(numOfTrianglesNeeded * 3)];
		int currTriangleID = 0;

        Debug.Log("Num of Triangles Needed: " + numOfTrianglesNeeded);


        int halfVerts = para_NumOfVertsOnSameEdge;

		// Create top 3D layer.
		for(int i=0; i<(halfVerts-1); i++)
		{
			// Add triangle 1.
			int baseIndex = currTriangleID * 3;
			tmpTriangleArr[baseIndex] = i;
            tmpTriangleArr[baseIndex + 1] = (i + halfVerts);
            tmpTriangleArr[baseIndex + 2] = (i + (halfVerts + 1));
		
			currTriangleID++;
			
			// Add triangle 2.
			baseIndex = currTriangleID * 3;
			tmpTriangleArr[baseIndex] = i;
            tmpTriangleArr[baseIndex + 1] = (i + (halfVerts + 1));
            tmpTriangleArr[baseIndex + 2] = i + 1;
		
			currTriangleID++;
		}
		
		
	
	 	mesh.triangles = tmpTriangleArr;
		
		
		//Textures and Colours
		if(para_Tex == null)
		{
			// Apply Colour.
			if(para_RenderColor.a != 1)
			{
				nwGObj.renderer.material.shader = Shader.Find("Particles/Alpha Blended");
				nwGObj.renderer.material.SetColor("_TintColor",para_RenderColor);
			}
			else
			{
				nwGObj.renderer.material.SetColor("_Color",para_RenderColor);
			}	
		}
		else
		{
			// Apply Texture.
			
			// Calculate length.
			float totLength = 0;
			for(int i=0; i<halfVerts-1; i++)
			{
				Vector3 tmpVert1 = vertList[i];
				Vector3 tmpVert2 = vertList[i+1];
				
				totLength += (tmpVert2-tmpVert1).magnitude;
			}
			
			
			Vector2[] tmpUVVect = new Vector2[vertList.Count];
			
			float currX = 0;
			float currY = 0;
			
			float currDistBuildUp = 0;
			
			tmpUVVect[0] = new Vector2(0,0);
			for(int i=0; i<halfVerts-1; i++)
			{
				Vector3 tmpVert1 = vertList[i];
				Vector3 tmpVert2 = vertList[i+1];
				
				float tmpDist = (tmpVert2-tmpVert1).magnitude;
				currDistBuildUp += tmpDist;
				
				float normalisedVal = currDistBuildUp/totLength;
				currX = normalisedVal;
				
				tmpUVVect[i+1] = new Vector2(currX,currY);
			}
			
			currX = 0;
			currY = 1;
			
			currDistBuildUp = 0;
			
			tmpUVVect[halfVerts] = new Vector2(0,1);
			for(int i=halfVerts; i<vertList.Count-1; i++)
			{
				Vector3 tmpVert1 = vertList[i];
				Vector3 tmpVert2 = vertList[i+1];
				
				float tmpDist = (tmpVert2-tmpVert1).magnitude;
				currDistBuildUp += tmpDist;
				
				float normalisedVal = currDistBuildUp/totLength;
				currX = normalisedVal;
				
				tmpUVVect[i+1] = new Vector2(currX,currY);
			}
			
			
			mesh.uv = tmpUVVect;
			
			float lengthwiseTexRepeat = totLength/10f;
			
			nwGObj.renderer.material.mainTexture = para_Tex;
			nwGObj.renderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
			nwGObj.renderer.material.mainTextureScale = new Vector2(lengthwiseTexRepeat,1);
		}
		
		
		
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		
        if(para_ParentName != null)
        {
            if(para_ParentName != "")
            {
                nwGObj.transform.parent = GameObject.Find(para_ParentName).transform;
            }
        }
        
		nwGObj.transform.position = new Vector3(0,0,0);	
		
		return nwGObj;
    }
	
	
	
	public GameObject render3DObjWithHeight(string para_ObjName,
		                              		string para_ParentName,                // NULL or "" mean no parent.
			                                Color para_RenderColor,
											bool para_alphaChannelRequired,
											string para_markingText,
			                                List<Vector3> para_ObjVertices,
			                                int para_NumOfVertsOnSameEdge,
										    float para_Height,
										    HashSet<RenderDirections> para_rendDirs)
	{		
		//float unitScale = 1;
		
		// Initial empty game object and empty mesh setup.
		GameObject nwGObj = new GameObject();
		nwGObj.name = para_ObjName;
		nwGObj.AddComponent(typeof(MeshRenderer));
		
		
		
		
		
		
	
		nwGObj.AddComponent(typeof(MeshFilter));
		MeshFilter meshFilter = (MeshFilter) nwGObj.GetComponent(typeof(MeshFilter));
		
		if (meshFilter==null)
		{
			nwGObj.AddComponent(typeof(MeshFilter));
			meshFilter = (MeshFilter) nwGObj.GetComponent(typeof(MeshFilter));
		}
		
		
			
		// Create Vertices
		List<Vector3> vertList = new List<Vector3>();
		vertList.AddRange(para_ObjVertices);
		
	
		
		
		int lastTopLayerIndex = vertList.Count-1;
		int numOfTopLayerVertices = vertList.Count;
	
		// 3D related:
		// Add second layer of vertices.
		if(para_Height != 0)
		{
			List<Vector3> modelBottomLayerVertices = new List<Vector3>();
			for(int i=0; i<vertList.Count; i++)
			{
				Vector3 topLayerVert = vertList[i];
				modelBottomLayerVertices.Add(new Vector3(topLayerVert.x,para_Height,topLayerVert.z));
			}
			vertList.AddRange(modelBottomLayerVertices);
		}
		
		
		// Find Centroid.
		//Vector3 centroidVect = calculateCentroid(vertList);
		
		
		/*// Shift every vertex so that the origin of the model mesh is World Pt (0,0,0)
		float averageCentreZ = ((highestColCeiling+lowestColFloor)/(2*1.0f))*unitScale;
		Vector3 shiftDownVect = new Vector3(0,0,-averageCentreZ);
		
		Vector3 shiftDepthVect = new Vector3(0,(float) (para_Thickness/2f),0);
		Vector3 centringVect = shiftLeftVect + shiftDownVect + shiftDepthVect;
				
		for(int i=0; i<vertList.Count; i++)
		{
			vertList[i] = vertList[i] + centringVect;	
		}*/
			
		/*// New version for centering object at the world origin Pt (0,0,0).
		Vector3 centroidVect = calculateCentroid(vertList);
		for(int i=0; i<vertList.Count; i++)
		{
			vertList[i] = vertList[i] - centroidVect;	
		}*/
			
		
		
		// Add vertices to mesh vertex buffer.
		Mesh mesh = meshFilter.mesh;
		if (mesh == null)
		{
			meshFilter.mesh = new Mesh();
			mesh = meshFilter.mesh;
		}
		
		mesh.Clear();
		mesh.vertices = vertList.ToArray();
		
			
		
		
		
		// Create triangles (Via Index Buffer).
		int numOfTrianglesNeeded = (vertList.Count - 4);
		int numOfLayerBindingTrianglesNeeded = vertList.Count;
		numOfTrianglesNeeded += numOfLayerBindingTrianglesNeeded;
		int[] tmpTriangleArr = new int[(numOfTrianglesNeeded * 3)];
		int currTriangleID = 0;
		
		int halfVerts = para_NumOfVertsOnSameEdge;
		
		
	
		// Create bottom 3D layer.
		if(para_rendDirs.Contains(RenderDirections.BOTTOM))
		{
			for(int i=0; i<(halfVerts-1); i++)
			{
				// Add triangle 1.
				int baseIndex = currTriangleID * 3;
				tmpTriangleArr[baseIndex] = i;
				tmpTriangleArr[baseIndex + 1] = (i + halfVerts);
				tmpTriangleArr[baseIndex + 2] = (i + (halfVerts + 1));
			
				currTriangleID++;
				
				// Add triangle 2.
				baseIndex = currTriangleID * 3;
				tmpTriangleArr[baseIndex] = i;
				tmpTriangleArr[baseIndex + 1] = (i + (halfVerts + 1));
				tmpTriangleArr[baseIndex + 2] = i+1;
			
				currTriangleID++;
			}
		}
	
	
		// Create top 3D layer.
		if(para_rendDirs.Contains(RenderDirections.TOP))
		{
			for(int i=(lastTopLayerIndex+1); i<((lastTopLayerIndex+1)+(halfVerts-1)); i++)
			{
				// Add triangle 1.
				int baseIndex = currTriangleID * 3;
				tmpTriangleArr[baseIndex] = i;
				tmpTriangleArr[baseIndex + 1] = (i + (halfVerts + 1));
				tmpTriangleArr[baseIndex + 2] = (i + halfVerts);
							
				currTriangleID++;
				
				// Add triangle 2.
				baseIndex = currTriangleID * 3;
				tmpTriangleArr[baseIndex] = i;
				tmpTriangleArr[baseIndex + 1] = i+1;
				tmpTriangleArr[baseIndex + 2] = (i + (halfVerts + 1));
			
				currTriangleID++;
			}
		}
	
	
		// Join top and bottom 3D layers.
		
		if(para_rendDirs.Contains(RenderDirections.SIDE1))
		{
			for(int i=0; i<(halfVerts-1); i++)
			{
				// Add triangle 1.
				int baseIndex = currTriangleID * 3;
				tmpTriangleArr[baseIndex] = i;
				tmpTriangleArr[baseIndex + 1] = (i + (numOfTopLayerVertices + 1));
				tmpTriangleArr[baseIndex + 2] = (i + numOfTopLayerVertices);
							
				currTriangleID++;
				
				// Add triangle 2.
				baseIndex = currTriangleID * 3;
				tmpTriangleArr[baseIndex] = i;
				tmpTriangleArr[baseIndex + 1] = i+1;
				tmpTriangleArr[baseIndex + 2] = (i + (numOfTopLayerVertices + 1));
			
				currTriangleID++;
			}
		}
	
		if(para_rendDirs.Contains(RenderDirections.SIDE2))
		{
			for(int i=(halfVerts); i<((halfVerts-1)+(halfVerts)); i++)
			{
				// Add triangle 1.
				int baseIndex = currTriangleID * 3;
				tmpTriangleArr[baseIndex] = i;
				tmpTriangleArr[baseIndex + 1] = (i + numOfTopLayerVertices);
				tmpTriangleArr[baseIndex + 2] = (i + (numOfTopLayerVertices + 1));
							
				currTriangleID++;
				
				// Add triangle 2.
				baseIndex = currTriangleID * 3;
				tmpTriangleArr[baseIndex] = i;
				tmpTriangleArr[baseIndex + 1] = (i + (numOfTopLayerVertices + 1));
				tmpTriangleArr[baseIndex + 2] = i+1;
			
				currTriangleID++;
			}
		}
	
		
		// Close off the sides.
		
		int bIndex;
		int tmpIndex;
		if(para_rendDirs.Contains(RenderDirections.FRONT))
		{
			bIndex = currTriangleID * 3;
			tmpIndex = halfVerts-1;
			tmpTriangleArr[bIndex] = tmpIndex;
			tmpTriangleArr[bIndex + 1] = vertList.Count-1;
			tmpTriangleArr[bIndex + 2] = (tmpIndex + numOfTopLayerVertices);
			currTriangleID++;	
		
			bIndex = currTriangleID * 3;
			tmpTriangleArr[bIndex] = tmpIndex;
			tmpTriangleArr[bIndex + 1] = lastTopLayerIndex;
			tmpTriangleArr[bIndex + 2] = vertList.Count-1;
			currTriangleID++;	
		}
	
		if(para_rendDirs.Contains(RenderDirections.BACK))
		{
			bIndex = currTriangleID * 3;
			tmpIndex = 0;
			tmpTriangleArr[bIndex] = tmpIndex;
			tmpTriangleArr[bIndex + 1] = tmpIndex + numOfTopLayerVertices;
			tmpTriangleArr[bIndex + 2] = tmpIndex + (numOfTopLayerVertices + halfVerts);
			currTriangleID++;
		
			bIndex = currTriangleID * 3;
			tmpTriangleArr[bIndex] = tmpIndex;
			tmpTriangleArr[bIndex + 1] = tmpIndex + (numOfTopLayerVertices + halfVerts);
			tmpTriangleArr[bIndex + 2] = tmpIndex + halfVerts;
			currTriangleID++;
		}
	
	
	 	mesh.triangles = tmpTriangleArr;
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		
		
		if(para_ParentName != null)
        {
            if(para_ParentName != "")
            {
                nwGObj.transform.parent = GameObject.Find(para_ParentName).transform;
            }
        }
        
		nwGObj.transform.position = new Vector3(0,0,0);	
		
		
		
		if((para_RenderColor.a != 1)||(para_alphaChannelRequired))
		{
			nwGObj.renderer.material.shader = Shader.Find("Custom/TransparentColorShader");
			nwGObj.renderer.material.SetColor("_Color",para_RenderColor);
			
			//nwGObj.renderer.material.shader = Shader.Find("Transparent/Diffuse");
			//nwGObj.renderer.material.SetColor("_Color",para_RenderColor);
			//nwGObj.renderer.material.shader = Shader.Find("Mobile/Particles/Alpha Blended");
			//nwGObj.renderer.material.color = para_RenderColor;
			//nwGObj.renderer.material.SetColor("_TintColor",para_RenderColor);
			//nwGObj.renderer.material.SetColor("_Color",para_RenderColor);
		}
		else
		{
			nwGObj.renderer.material.SetColor("_Color",para_RenderColor);
		}
		
		
		
		
		
		return nwGObj;
	}
	
	/*
	public GameObject render3DObjWithHeight_SeparateFaces(string para_ObjName,
		                              		string para_ParentName,                // NULL or "" mean no parent.
			                                UnityEngine.Color para_RenderColor,
			                                List<Vector3> para_ObjVertices,
											List<Vector3> para_ObjNormals,
											List<int> para_NormalIndexBuffer,
			                                int para_NumOfVertsOnSameEdge,
										    float para_Height,
										    HashSet<RenderDirections> para_rendDirs,
											SettingItems para_settingData,
											string para_segTypePrefixStr)
	{
		Dictionary<RenderDirections,UnityEngine.Color> colorMap = new Dictionary<RenderDirections, UnityEngine.Color>();
		foreach(RenderDirections rDir in para_rendDirs)
		{
			colorMap.Add(rDir,para_RenderColor);	
		}
		
		return render3DObjWithHeight_SeparateFaces(para_ObjName,para_ParentName,colorMap,para_ObjVertices,para_ObjNormals,para_NormalIndexBuffer,para_NumOfVertsOnSameEdge,para_Height,para_rendDirs,para_settingData,para_segTypePrefixStr);
	}
	
	
	public GameObject render3DObjWithHeight_SeparateFaces(string para_ObjName,
		                              		string para_ParentName,                // NULL or "" mean no parent.
			                                Dictionary<RenderDirections,UnityEngine.Color> para_RenderColorMap,
			                                List<Vector3> para_ObjVertices,
											List<Vector3> para_ObjNormals,
											List<int> para_NormalIndexBuffer,
			                                int para_NumOfVertsOnSameEdge,
										    float para_Height,
										    HashSet<RenderDirections> para_rendDirs,
											SettingItems para_settingData,
											string para_segTypePrefixStr)
	{		
		float unitScale = 1;
		
		// Initial empty game object and empty mesh setup.
		GameObject nwGObj = new GameObject();
		nwGObj.name = para_ObjName;
		nwGObj.AddComponent(typeof(MeshRenderer));
		
		//if(para_RenderColor.a != 1)
		//{
		//	nwGObj.renderer.material.shader = Shader.Find("Particles/Alpha Blended");
		//	nwGObj.renderer.material.SetColor("_TintColor",para_RenderColor);
		//}
		//else
		//{
		//	nwGObj.renderer.material.SetColor("_Color",para_RenderColor);
		//}
		
		
	
		nwGObj.AddComponent(typeof(MeshFilter));
		MeshFilter meshFilter = (MeshFilter) nwGObj.GetComponent(typeof(MeshFilter));
		
		if (meshFilter==null)
		{
			nwGObj.AddComponent(typeof(MeshFilter));
			meshFilter = (MeshFilter) nwGObj.GetComponent(typeof(MeshFilter));
		}
		
		
			
		// Create Vertices
		List<Vector3> vertList = new List<Vector3>();
		vertList.AddRange(para_ObjVertices);
		
	
		
		
		int lastTopLayerIndex = vertList.Count-1;
		int numOfTopLayerVertices = vertList.Count;
	
		// 3D related:
		// Add second layer of vertices.
		List<Vector3> modelBottomLayerVertices = new List<Vector3>();
		for(int i=0; i<vertList.Count; i++)
		{
			Vector3 topLayerVert = vertList[i];
			Vector3 reqNormal = para_ObjNormals[para_NormalIndexBuffer[i]];
			Vector3 nwPt = topLayerVert + (reqNormal * para_Height);
			modelBottomLayerVertices.Add(nwPt);
			//modelBottomLayerVertices.Add(new Vector3(topLayerVert.x,para_Height,topLayerVert.z));
		}
		vertList.AddRange(modelBottomLayerVertices);
		
		
		
		// Shift every vertex so that the origin of the model mesh is World Pt (0,0,0)
		//float averageCentreZ = ((highestColCeiling+lowestColFloor)/(2*1.0f))*unitScale;
		//Vector3 shiftDownVect = new Vector3(0,0,-averageCentreZ);
		//
		//Vector3 shiftDepthVect = new Vector3(0,(float) (para_Thickness/2f),0);
		//Vector3 centringVect = shiftLeftVect + shiftDownVect + shiftDepthVect;
		//		
		//for(int i=0; i<vertList.Count; i++)
		//{
		//	vertList[i] = vertList[i] + centringVect;	
		//}
			
		
		
		
		
			
		
		
		
		
		
		int halfVerts = para_NumOfVertsOnSameEdge;
		
		
	
		// Create bottom 3D layer.
		if(para_rendDirs.Contains(RenderDirections.BOTTOM))
		{
			List<Vector3> childMergedVectList = new List<Vector3>();
			List<Vector3> childTopVectList = new List<Vector3>();
			List<Vector3> childBotVectList = new List<Vector3>();
			HashSet<int> encounteredIDSet = new HashSet<int>();
			
			for(int i=0; i<(halfVerts-1); i++)
			{
				if(encounteredIDSet.Contains(i) == false) { childTopVectList.Add(vertList[i]); encounteredIDSet.Add(i); }
				if(encounteredIDSet.Contains(i+1) == false) { childTopVectList.Add(vertList[i+1]); encounteredIDSet.Add(i+1); }
				if(encounteredIDSet.Contains(i+halfVerts) == false) { childBotVectList.Add(vertList[i+halfVerts]); encounteredIDSet.Add(i+halfVerts); }
				if(encounteredIDSet.Contains(i+halfVerts+1) == false) { childBotVectList.Add(vertList[i+halfVerts+1]); encounteredIDSet.Add(i+halfVerts+1); }
			}
			
			childMergedVectList.AddRange(childTopVectList);
			childMergedVectList.AddRange(childBotVectList);
			
			string floorTex_MainID = para_settingData.floorTextures[para_segTypePrefixStr+"_MAIN"];
			Texture2D reqFloorTex = getGameTexture(floorTex_MainID);
			
			render3DObj("BOTTOM",para_ObjName,para_RenderColorMap[RenderDirections.BOTTOM],reqFloorTex,childMergedVectList,childMergedVectList.Count/2);
		}
	
	
		// Create top 3D layer.
		if(para_rendDirs.Contains(RenderDirections.TOP))
		{
			List<Vector3> childMergedVectList = new List<Vector3>();
			List<Vector3> childTopVectList = new List<Vector3>();
			List<Vector3> childBotVectList = new List<Vector3>();
			HashSet<int> encounteredIDSet = new HashSet<int>();
			
			for(int i=(lastTopLayerIndex+1); i<((lastTopLayerIndex+1)+(halfVerts-1)); i++)
			{
				if(encounteredIDSet.Contains(i) == false) { childBotVectList.Add(vertList[i]); encounteredIDSet.Add(i); }
				if(encounteredIDSet.Contains(i+1) == false) { childBotVectList.Add(vertList[i+1]); encounteredIDSet.Add(i+1); }
				if(encounteredIDSet.Contains(i+halfVerts) == false) { childTopVectList.Add(vertList[i+halfVerts]); encounteredIDSet.Add(i+halfVerts); }
				if(encounteredIDSet.Contains(i+halfVerts+1) == false) { childTopVectList.Add(vertList[i+halfVerts+1]); encounteredIDSet.Add(i+halfVerts+1); }
			}
			
			childMergedVectList.AddRange(childTopVectList);
			childMergedVectList.AddRange(childBotVectList);
			
			string ceilingTex_MainID = para_settingData.wallTextures["Ceiling_MAIN"];
			Texture2D reqCeilingTex = getGameTexture(ceilingTex_MainID);
			
			render3DObj("TOP",para_ObjName,para_RenderColorMap[RenderDirections.TOP],reqCeilingTex,childMergedVectList,childMergedVectList.Count/2);
		}
	
	
		// Join top and bottom 3D layers.
		
		if(para_rendDirs.Contains(RenderDirections.SIDE1))
		{
			List<Vector3> childMergedVectList = new List<Vector3>();
			List<Vector3> childTopVectList = new List<Vector3>();
			List<Vector3> childBotVectList = new List<Vector3>();
			HashSet<int> encounteredIDSet = new HashSet<int>();
			
			for(int i=0; i<(halfVerts-1); i++)
			{
				if(encounteredIDSet.Contains(i) == false) { childBotVectList.Add(vertList[i]); encounteredIDSet.Add(i); }
				if(encounteredIDSet.Contains(i+1) == false) { childBotVectList.Add(vertList[i+1]); encounteredIDSet.Add(i+1); }
				if(encounteredIDSet.Contains(i+numOfTopLayerVertices) == false) { childTopVectList.Add(vertList[i+numOfTopLayerVertices]); encounteredIDSet.Add(i+numOfTopLayerVertices); }
				if(encounteredIDSet.Contains(i+numOfTopLayerVertices+1) == false) { childTopVectList.Add(vertList[i+numOfTopLayerVertices+1]); encounteredIDSet.Add(i+numOfTopLayerVertices+1); }
			}
			
			childMergedVectList.AddRange(childTopVectList);
			childMergedVectList.AddRange(childBotVectList);
			
			string wallTex_MainID = para_settingData.wallTextures[para_segTypePrefixStr+"_MAIN"];
			Texture2D reqWallTex = getGameTexture(wallTex_MainID);
			
			render3DObj("SIDE1",para_ObjName,para_RenderColorMap[RenderDirections.SIDE1],reqWallTex,childMergedVectList,childMergedVectList.Count/2);
		}
	
		if(para_rendDirs.Contains(RenderDirections.SIDE2))
		{
			List<Vector3> childMergedVectList = new List<Vector3>();
			List<Vector3> childTopVectList = new List<Vector3>();
			List<Vector3> childBotVectList = new List<Vector3>();
			HashSet<int> encounteredIDSet = new HashSet<int>();
			
			for(int i=(halfVerts); i<((halfVerts-1)+(halfVerts)); i++)
			{
				if(encounteredIDSet.Contains(i) == false) { childTopVectList.Add(vertList[i]); encounteredIDSet.Add(i); }
				if(encounteredIDSet.Contains(i+1) == false) { childTopVectList.Add(vertList[i+1]); encounteredIDSet.Add(i+1); }
				if(encounteredIDSet.Contains(i+numOfTopLayerVertices) == false) { childBotVectList.Add(vertList[i+numOfTopLayerVertices]); encounteredIDSet.Add(i+numOfTopLayerVertices); }
				if(encounteredIDSet.Contains(i+numOfTopLayerVertices+1) == false) { childBotVectList.Add(vertList[i+numOfTopLayerVertices+1]); encounteredIDSet.Add(i+numOfTopLayerVertices+1); }
			}
			
			childMergedVectList.AddRange(childTopVectList);
			childMergedVectList.AddRange(childBotVectList);
			
			string wallTex_MainID = para_settingData.wallTextures[para_segTypePrefixStr+"_MAIN"];
			Texture2D reqWallTex = getGameTexture(wallTex_MainID);
			
			render3DObj("SIDE2",para_ObjName,para_RenderColorMap[RenderDirections.SIDE2],reqWallTex,childMergedVectList,childMergedVectList.Count/2);
		}
	
		
		// Close off the sides.
		
		int bIndex;
		int tmpIndex;
		if(para_rendDirs.Contains(RenderDirections.FRONT))
		{
			tmpIndex = halfVerts-1;
			
			List<Vector3> childMergedVectList = new List<Vector3>();
			List<Vector3> childTopVectList = new List<Vector3>();
			List<Vector3> childBotVectList = new List<Vector3>();
			
			childTopVectList.Add(vertList[tmpIndex + numOfTopLayerVertices]);
			childTopVectList.Add(vertList[vertList.Count-1]);
			childBotVectList.Add(vertList[tmpIndex]);
			childBotVectList.Add(vertList[lastTopLayerIndex]);
			
			childMergedVectList.AddRange(childTopVectList);
			childMergedVectList.AddRange(childBotVectList);
			
			string wallTex_MainID = para_settingData.wallTextures[para_segTypePrefixStr+"_MAIN"];
			Texture2D reqWallTex = getGameTexture(wallTex_MainID);
			
			render3DObj("FRONT",para_ObjName,para_RenderColorMap[RenderDirections.FRONT],reqWallTex,childMergedVectList,childMergedVectList.Count/2);
		}
	
		if(para_rendDirs.Contains(RenderDirections.BACK))
		{
			tmpIndex = 0;
			
			List<Vector3> childMergedVectList = new List<Vector3>();
			List<Vector3> childTopVectList = new List<Vector3>();
			List<Vector3> childBotVectList = new List<Vector3>();
			
			childTopVectList.Add(vertList[tmpIndex]);
			childTopVectList.Add(vertList[tmpIndex + halfVerts]);
			childBotVectList.Add(vertList[tmpIndex + numOfTopLayerVertices]);
			childBotVectList.Add(vertList[tmpIndex + numOfTopLayerVertices + halfVerts]);
			
			
			childMergedVectList.AddRange(childTopVectList);
			childMergedVectList.AddRange(childBotVectList);
			
			string wallTex_MainID = para_settingData.wallTextures[para_segTypePrefixStr+"_MAIN"];
			Texture2D reqWallTex = getGameTexture(wallTex_MainID);
			
			render3DObj("BACK",para_ObjName,para_RenderColorMap[RenderDirections.BACK],reqWallTex,childMergedVectList,childMergedVectList.Count/2);
		}
	
		
	 	
		
		if(para_ParentName != null)
        {
            if(para_ParentName != "")
            {
                nwGObj.transform.parent = GameObject.Find(para_ParentName).transform;
            }
        }
        
		nwGObj.transform.position = new Vector3(0,0,0);	
		
		
		return nwGObj;
	}
	
	*/
	
	private Texture2D getGameTexture(string para_texID)
	{
		Texture2D reqTex;
		
		if(loadedTextures == null)
		{
			loadedTextures = new Dictionary<string, Texture2D>();	
		}
		
		if(loadedTextures.ContainsKey(para_texID))
		{
			reqTex = loadedTextures[para_texID];	
		}
		else
		{
			reqTex = (Texture2D) Resources.Load(para_texID,typeof(Texture2D));
			loadedTextures.Add(para_texID,reqTex);
		}	
		
		return reqTex;
	}

	private Vector3 calculateCentroid(List<Vector3> para_vertices)
	{
		float centroid_X = 0;
		float centroid_Y = 0;
		float centroid_Z = 0;
		for(int i=0; i<para_vertices.Count; i++)
		{
			centroid_X += para_vertices[i].x;
			centroid_Y += para_vertices[i].y;
			centroid_Z += para_vertices[i].z;
		}
		centroid_X = centroid_X/para_vertices.Count;
		centroid_Y = centroid_Y/para_vertices.Count;
		centroid_Z = centroid_Z/para_vertices.Count;
		
		Vector3 centroidVect = new Vector3(centroid_X,centroid_Y,centroid_Z);
		return centroidVect;
	}
}

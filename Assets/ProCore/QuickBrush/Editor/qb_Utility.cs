//	QuickBrush: Prefab Placement Tool
//	by PlayTangent
//	all rights reserved
//	www.ProCore3d.com

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// save as .qbt special binary
public static class qb_Utility
{
	
	public static void SaveTemplate(qb_Template template, string directory) // Save the current brush to memory  
	{
		string fileName = directory + "/Templates/" + template.brushName + ".qbt";

		Stream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
		//Debug.Log("SavingTemplate");
		Hashtable propertyTable = CreatePropertyTable(template);
		
		BinaryFormatter formatter = new BinaryFormatter();
		formatter.Serialize(stream, propertyTable);
		
		stream.Close();
	}
	
	static Hashtable CreatePropertyTable(qb_Template template)	//converts brush class to a hash table of values
	{
		Hashtable propertyTable = new Hashtable();
		
		propertyTable.Add("BrushName",template.brushName);
		
	#region Brush Settings Vars
		propertyTable.Add("BrushRadius",template.brushRadius);
	
		propertyTable.Add("BrushRadiusMin",template.brushRadiusMin);
	
		propertyTable.Add("BrushRadiusMax",template.brushRadiusMax);
	
		propertyTable.Add("BrushSpacing",template.brushSpacing);
	
		propertyTable.Add("BrushSpacingMin",template.brushSpacingMin);
	
		propertyTable.Add("BrushSpacingMax",template.brushSpacingMax);
		
		propertyTable.Add("ScatterRadius",template.scatterRadius);
	#endregion
				
	#region Rotation Settings Vars
		propertyTable.Add("AlignToNormal",template.alignToNormal);
	
		propertyTable.Add("FlipNormalAlign",template.flipNormalAlign);
	
		propertyTable.Add("AlignToStroke",template.alignToStroke);
	
		propertyTable.Add("FlipStrokeAlign",template.flipStrokeAlign);
	
		propertyTable.Add("RotationRangeMinX",template.rotationRangeMin.x);
		propertyTable.Add("RotationRangeMinY",template.rotationRangeMin.y);
		propertyTable.Add("RotationRangeMinZ",template.rotationRangeMin.z);
	
		propertyTable.Add("RotationRangeMaxX",template.rotationRangeMax.x);
		propertyTable.Add("RotationRangeMaxY",template.rotationRangeMax.y);
		propertyTable.Add("RotationRangeMaxZ",template.rotationRangeMax.z);
	#endregion
				
	#region Position Settings Vars
		propertyTable.Add("PositionOffsetX",template.positionOffset.x);
		propertyTable.Add("PositionOffsetY",template.positionOffset.y);
		propertyTable.Add("PositionOffsetZ",template.positionOffset.z);
	#endregion
				
	#region Scale Settings Vars	
		//The minimum and maximum possible scale
		propertyTable.Add("ScaleMin",template.scaleMin);
	
		propertyTable.Add("ScaleMax",template.scaleMax);
			
		//The minimum and maximum current scale range setting
		propertyTable.Add("ScaleRandMinX",template.scaleRandMin.x);
		propertyTable.Add("ScaleRandMinY",template.scaleRandMin.y);
		propertyTable.Add("ScaleRandMinZ",template.scaleRandMin.z);
	
		propertyTable.Add("ScaleRandMaxX",template.scaleRandMax.x);
		propertyTable.Add("ScaleRandMaxY",template.scaleRandMax.y);
		propertyTable.Add("ScaleRandMaxZ",template.scaleRandMax.z);
	
		propertyTable.Add("ScaleRandMinUniform",template.scaleRandMinUniform);
	
		propertyTable.Add("ScaleRandMaxUniform",template.scaleRandMaxUniform);
	
		propertyTable.Add("ScaleUniform",template.scaleUniform);
	#endregion	
				
	#region Sorting Vars  
		//Selection
		propertyTable.Add("PaintToSelection",template.paintToSelection);
	
		//Layers
		propertyTable.Add("PaintToLayer",template.paintToLayer);
	
		propertyTable.Add("LayerIndex",template.layerIndex);
		
		propertyTable.Add("GroupObjects",template.groupObjects);
		
		propertyTable.Add("GroupIndex",template.groupIndex);
		
	#endregion
		
	#region Eraser Vars	
		propertyTable.Add("EraseByGroup",template.eraseByGroup);
		propertyTable.Add("EraseBySelected",template.eraseBySelected);
	#endregion
			
		//New Prefab Save
		string prefabGroupString = string.Empty;
		
		for(int i = 0; i < template.prefabGroup.Length; i ++)
		{
			prefabGroupString += "/" + AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(template.prefabGroup[i].prefab)) + "-" + template.prefabGroup[i].weight.ToString();
		}
		
		propertyTable.Add("PrefabGUIDList",prefabGroupString);
	
		return propertyTable;
	}
	

	
	public static qb_TemplateSignature LoadTemplateSignature(string fileLocationString)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		Stream stream = new FileStream(fileLocationString, FileMode.Open, FileAccess.Read, FileShare.Read);
		
		Hashtable propertyTable = (Hashtable) formatter.Deserialize(stream);
		stream.Close();	
	
		qb_TemplateSignature signature = new qb_TemplateSignature();
		
		signature.directory = fileLocationString;
		signature.name = (string) propertyTable["BrushName"];
		
		return signature;
	}
	
	/*
	public static qb_Template LoadTemplate(string fileLocationString)
	{
		//brush properties pulled from disk and re-assembled in new qb_Brush class instance 
		BinaryFormatter formatter = new BinaryFormatter();
		Stream stream = new FileStream(fileLocationString, FileMode.Open, FileAccess.Read, FileShare.Read);
		
		Hashtable propertyTable = (Hashtable) formatter.Deserialize(stream);
		stream.Close();	
	
		qb_Template template = new qb_Template();//ScriptableObject.CreateInstance<qb_Template>();//new qb_Template();
		
		template.brushName = (string) propertyTable["BrushName"];
		
	#region Brush Settings Vars
		template.brushRadius = (float) propertyTable["BrushRadius"];
	
		template.brushRadiusMin = (float) propertyTable["BrushRadiusMin"];
	
		template.brushRadiusMax =  (float) propertyTable["BrushRadiusMax"];
	
		template.brushSpacing = (float) propertyTable["BrushSpacing"];
	
		template.brushSpacingMin = (float) propertyTable["BrushSpacingMin"];
	
		template.brushSpacingMax = (float) propertyTable["BrushSpacingMax"];
		
		template.scatterRadius = (float) propertyTable["ScatterRadius"];
	#endregion
				
	#region Rotation Settings Vars
		template.alignToNormal = (bool) propertyTable["AlignToNormal"];
	
		template.flipNormalAlign = (bool) propertyTable["FlipNormalAlign"];
	
		template.alignToStroke = (bool) propertyTable["AlignToStroke"];
	
		template.flipStrokeAlign = (bool) propertyTable["FlipStrokeAlign"];
	
		template.rotationRangeMin.x = (float) propertyTable["RotationRangeMinX"];
		template.rotationRangeMin.y = (float) propertyTable["RotationRangeMinY"];
		template.rotationRangeMin.z = (float) propertyTable["RotationRangeMinZ"];
	
		template.rotationRangeMax.x = (float) propertyTable["RotationRangeMaxX"];
		template.rotationRangeMax.y = (float) propertyTable["RotationRangeMaxY"];
		template.rotationRangeMax.z = (float) propertyTable["RotationRangeMaxZ"];
	#endregion
				
	#region Position Settings Vars
		template.positionOffset.x =	(float) propertyTable["PositionOffsetX"];
		template.positionOffset.y = (float) propertyTable["PositionOffsetY"];
		template.positionOffset.z = (float) propertyTable["PositionOffsetZ"];
	#endregion
				
	#region Scale Settings Vars	
		//The minimum and maximum possible scale
		template.scaleMin = (float) propertyTable["ScaleMin"];
		template.scaleMax = (float) propertyTable["ScaleMax"];
			
		//The minimum and maximum current scale range setting
		template.scaleRandMin.x = (float) propertyTable["ScaleRandMinX"];
		template.scaleRandMin.y = (float) propertyTable["ScaleRandMinY"];
		template.scaleRandMin.z = (float) propertyTable["ScaleRandMinZ"];
	
		template.scaleRandMax.x = (float) propertyTable["ScaleRandMaxX"];
		template.scaleRandMax.y = (float) propertyTable["ScaleRandMaxY"];
		template.scaleRandMax.z = (float) propertyTable["ScaleRandMaxZ"];
	
		template.scaleRandMinUniform = (float) propertyTable["ScaleRandMinUniform"];
	
		template.scaleRandMaxUniform = (float) propertyTable["ScaleRandMaxUniform"];
	
		template.scaleUniform = (bool) propertyTable["ScaleUniform"];
	#endregion	
				
	#region Sorting Vars  
		//Selection
		template.paintToSelection = (bool) propertyTable["PaintToSelection"];
	
		//Layers
		template.paintToLayer = (bool) propertyTable["PaintToLayer"];
	
		template.layerIndex = (int) propertyTable["LayerIndex"];
	#endregion
		
	#region Eraser Vars	
		template.eraseByGroup = (bool) propertyTable["EraseByGroup"];
		template.eraseBySelected = (bool) propertyTable["EraseBySelected"];
	#endregion
		
	#region Repopulate the Prefab List
		string prefabGroupString = (string) propertyTable["PrefabGUIDList"];
		qb_PrefabObject[] prefabGroup = new qb_PrefabObject[0];

		
		string[] prefabStringList = new string[0];
		List<UnityEngine.Object> newPrefabs = new List<UnityEngine.Object>();
		
		if(prefabGroupString != string.Empty)
		{
			//first clear out any items that are in the prefab list now
			prefabGroup = new qb_PrefabObject[0];
			//then retreive and split the saved prefab guids into a list
			prefabStringList = prefabGroupString.Split('/');//string;
		}
		
		foreach(string GUIDstring in prefabStringList)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(GUIDstring);
			Object item = AssetDatabase.LoadAssetAtPath(assetPath,typeof(Object));
			
			if(item != null)
				newPrefabs.Add(item);
		}
		
		prefabGroup = new qb_PrefabObject[0]; //clear out

		if(newPrefabs.Count > 0)
		{
			foreach(UnityEngine.Object newPrefab in newPrefabs)
			{
				ArrayUtility.Add(ref prefabGroup,new qb_PrefabObject(newPrefab,1f));
			}
		}
		
		template.prefabGroup = prefabGroup;
	#endregion
		
		template.live = true;
		
		return template;
		
	}
	*/
	/*
	public static qb_Template LoadTemplate(string fileLocationString)
	{
		//brush properties pulled from disk and re-assembled in new qb_Brush class instance 
		BinaryFormatter formatter = new BinaryFormatter();
		Stream stream = new FileStream(fileLocationString, FileMode.Open, FileAccess.Read, FileShare.Read);
		
		Hashtable propertyTable = (Hashtable) formatter.Deserialize(stream);
		stream.Close();	
	
		qb_Template template = new qb_Template();//ScriptableObject.CreateInstance<qb_Template>();//new qb_Template();
		
		
		
		template.brushName =		(string) GetProperty<string>(ref propertyTable,"BrushName",template.brushName);// (string) propertyTable["BrushName"];
		
	#region Brush Settings Vars
		template.brushRadius =		(float) GetProperty<float>(ref propertyTable,"BrushRadius",template.brushRadius);//(float) propertyTable["BrushRadius"];
	
		template.brushRadiusMin =	(float) GetProperty<float>(ref propertyTable,"BrushRadiusMin",template.brushRadiusMin);//(float) propertyTable["BrushRadiusMin"];
	
		template.brushRadiusMax =	(float) GetProperty<float>(ref propertyTable,"BrushRadiusMax",template.brushRadiusMax);//(float) propertyTable["BrushRadiusMax"];
	
		template.brushSpacing =		(float) GetProperty<float>(ref propertyTable,"BrushSpacing",template.brushSpacing);//(float) propertyTable["BrushSpacing"];
	
		template.brushSpacingMin =	(float) GetProperty<float>(ref propertyTable,"BrushSpacingMin",template.brushSpacingMin);//(float) propertyTable["BrushSpacingMin"];
	
		template.brushSpacingMax =	(float) GetProperty<float>(ref propertyTable,"BrushSpacingMax",template.brushSpacingMax);//(float) propertyTable["BrushSpacingMax"];
		
		template.scatterRadius =	(float) GetProperty<float>(ref propertyTable,"ScatterRadius",template.scatterRadius);//(float) propertyTable["ScatterRadius"];
	#endregion
				
	#region Rotation Settings Vars
		template.alignToNormal =	(bool) GetProperty<bool>(ref propertyTable,"AlignToNormal",template.alignToNormal);//(bool) propertyTable["AlignToNormal"];
	
		template.flipNormalAlign =	(bool) GetProperty<bool>(ref propertyTable,"FlipNormalAlign",template.flipNormalAlign);//(bool) propertyTable["FlipNormalAlign"];
	
		template.alignToStroke =	(bool) GetProperty<bool>(ref propertyTable,"AlignToStroke",template.alignToStroke);//(bool) propertyTable["AlignToStroke"];
	
		template.flipStrokeAlign =	(bool) GetProperty<bool>(ref propertyTable,"FlipStrokeAlign",template.flipStrokeAlign);//(bool) propertyTable["FlipStrokeAlign"];
	
		template.rotationRangeMin.x = (float) GetProperty<float>(ref propertyTable,"RotationRangeMinX",template.rotationRangeMin.x);//(float) propertyTable["RotationRangeMinX"];
		template.rotationRangeMin.y = (float) GetProperty<float>(ref propertyTable,"RotationRangeMinY",template.rotationRangeMin.y);//(float) propertyTable["RotationRangeMinY"];
		template.rotationRangeMin.z = (float) GetProperty<float>(ref propertyTable,"RotationRangeMinZ",template.rotationRangeMin.z);//(float) propertyTable["RotationRangeMinZ"];
	
		template.rotationRangeMax.x = (float) GetProperty<float>(ref propertyTable,"RotationRangeMaxX",template.rotationRangeMax.x);//(float) propertyTable["RotationRangeMaxX"];
		template.rotationRangeMax.y = (float) GetProperty<float>(ref propertyTable,"RotationRangeMaxY",template.rotationRangeMax.y);//(float) propertyTable["RotationRangeMaxY"];
		template.rotationRangeMax.z = (float) GetProperty<float>(ref propertyTable,"RotationRangeMaxZ",template.rotationRangeMax.z);//(float) propertyTable["RotationRangeMaxZ"];
	#endregion
				
	#region Position Settings Vars
		template.positionOffset.x =	(float) GetProperty<float>(ref propertyTable,"PositionOffsetX",template.positionOffset.x);//(float) propertyTable["PositionOffsetX"];
		template.positionOffset.y = (float) GetProperty<float>(ref propertyTable,"PositionOffsetY",template.positionOffset.y);//(float) propertyTable["PositionOffsetY"];
		template.positionOffset.z = (float) GetProperty<float>(ref propertyTable,"PositionOffsetZ",template.positionOffset.z);//(float) propertyTable["PositionOffsetZ"];
	#endregion
				
	#region Scale Settings Vars	
		//The minimum and maximum possible scale
		template.scaleMin =			(float) GetProperty<float>(ref propertyTable,"ScaleMin",template.scaleMin);//(float) propertyTable["ScaleMin"];
		template.scaleMax =			(float) GetProperty<float>(ref propertyTable,"ScaleMax",template.scaleMax);//(float) propertyTable["ScaleMax"];
			
		//The minimum and maximum current scale range setting
		template.scaleRandMin.x =	(float) GetProperty<float>(ref propertyTable,"ScaleRandMinX",template.scaleRandMin.x);//(float) propertyTable["ScaleRandMinX"];
		template.scaleRandMin.y =	(float) GetProperty<float>(ref propertyTable,"ScaleRandMinY",template.scaleRandMin.y);//(float) propertyTable["ScaleRandMinY"];
		template.scaleRandMin.z =	(float) GetProperty<float>(ref propertyTable,"ScaleRandMinZ",template.scaleRandMin.z);//(float) propertyTable["ScaleRandMinZ"];
	
		template.scaleRandMax.x =	(float) GetProperty<float>(ref propertyTable,"ScaleRandMaxX",template.scaleRandMax.x);//(float) propertyTable["ScaleRandMaxX"];
		template.scaleRandMax.y =	(float) GetProperty<float>(ref propertyTable,"ScaleRandMaxY",template.scaleRandMax.y);//(float) propertyTable["ScaleRandMaxY"];
		template.scaleRandMax.z =	(float) GetProperty<float>(ref propertyTable,"ScaleRandMaxZ",template.scaleRandMax.z);//(float) propertyTable["ScaleRandMaxZ"];
	
		template.scaleRandMinUniform = (float) GetProperty<float>(ref propertyTable,"ScaleRandMinUniform",template.scaleRandMinUniform);//(float) propertyTable["ScaleRandMinUniform"];
		template.scaleRandMaxUniform = (float) GetProperty<float>(ref propertyTable,"ScaleRandMaxUniform",template.scaleRandMaxUniform);//(float) propertyTable["ScaleRandMaxUniform"];
	
		template.scaleUniform = 	(bool) GetProperty<bool>(ref propertyTable,"ScaleUniform",template.scaleUniform);//(bool) propertyTable["ScaleUniform"];
	#endregion	
				
	#region Sorting Vars  
		//Selection
		template.paintToSelection = (bool) GetProperty<bool>(ref propertyTable,"PaintToSelection",template.paintToSelection);//(bool) propertyTable["PaintToSelection"];
	
		//Layers
		template.paintToLayer =		(bool) GetProperty<bool>(ref propertyTable,"PaintToLayer",template.paintToLayer);//(bool) propertyTable["PaintToLayer"];
	
		template.layerIndex =		(int) GetProperty<int>(ref propertyTable,"LayerIndex",template.layerIndex);//(int) propertyTable["LayerIndex"];
	#endregion
		
	#region Eraser Vars	
		template.eraseByGroup =		(bool) GetProperty<bool>(ref propertyTable,"EraseByGroup",template.eraseByGroup);//(bool) propertyTable["EraseByGroup"];
		template.eraseBySelected =	(bool) GetProperty<bool>(ref propertyTable,"EraseBySelected",template.eraseBySelected);//(bool) propertyTable["EraseBySelected"];
	#endregion
		
	#region Repopulate the Prefab List
		string prefabGroupString =	(string) GetProperty<string>(ref propertyTable,"PrefabGUIDList",string.Empty);// (string) propertyTable["PrefabGUIDList"];
		qb_PrefabObject[] prefabGroup = new qb_PrefabObject[0];

		string[] prefabStringList = new string[0];
		List<UnityEngine.Object> newPrefabs = new List<UnityEngine.Object>();
		
		if(prefabGroupString != string.Empty)
		{
			//first clear out any items that are in the prefab list now
			prefabGroup = new qb_PrefabObject[0];
			//then retreive and split the saved prefab guids into a list
			prefabStringList = prefabGroupString.Split('/');//string;
		}
		
		foreach(string GUIDstring in prefabStringList)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(GUIDstring);
			Object item = AssetDatabase.LoadAssetAtPath(assetPath,typeof(Object));
			
			if(item != null)
				newPrefabs.Add(item);
		}
		
		prefabGroup = new qb_PrefabObject[0]; //clear out

		if(newPrefabs.Count > 0)
		{
			foreach(UnityEngine.Object newPrefab in newPrefabs)
			{
				ArrayUtility.Add(ref prefabGroup,new qb_PrefabObject(newPrefab,1f));
			}
		}
		
		template.prefabGroup = prefabGroup;
	#endregion
		
		template.live = true;
		
		return template;
		
	}
	*/
	
	public static qb_Template LoadTemplate(string fileLocationString)
	{
		//brush properties pulled from disk and re-assembled in new qb_Brush class instance 
		BinaryFormatter formatter = new BinaryFormatter();
		Stream stream = new FileStream(fileLocationString, FileMode.Open, FileAccess.Read, FileShare.Read);
		
		Hashtable propertyTable = (Hashtable) formatter.Deserialize(stream);
		stream.Close();	
	
		qb_Template template = new qb_Template();//ScriptableObject.CreateInstance<qb_Template>();//new qb_Template();
		
		
		
		template.brushName =		(string) GetProperty<string>(ref propertyTable,"BrushName",template.brushName);// (string) propertyTable["BrushName"];
		
	#region Brush Settings Vars
		template.brushRadius =		(float) GetProperty<float>(ref propertyTable,"BrushRadius",template.brushRadius);//(float) propertyTable["BrushRadius"];
	
		template.brushRadiusMin =	(float) GetProperty<float>(ref propertyTable,"BrushRadiusMin",template.brushRadiusMin);//(float) propertyTable["BrushRadiusMin"];
	
		template.brushRadiusMax =	(float) GetProperty<float>(ref propertyTable,"BrushRadiusMax",template.brushRadiusMax);//(float) propertyTable["BrushRadiusMax"];
	
		template.brushSpacing =		(float) GetProperty<float>(ref propertyTable,"BrushSpacing",template.brushSpacing);//(float) propertyTable["BrushSpacing"];
	
		template.brushSpacingMin =	(float) GetProperty<float>(ref propertyTable,"BrushSpacingMin",template.brushSpacingMin);//(float) propertyTable["BrushSpacingMin"];
	
		template.brushSpacingMax =	(float) GetProperty<float>(ref propertyTable,"BrushSpacingMax",template.brushSpacingMax);//(float) propertyTable["BrushSpacingMax"];
		
		template.scatterRadius =	(float) GetProperty<float>(ref propertyTable,"ScatterRadius",template.scatterRadius);//(float) propertyTable["ScatterRadius"];
	#endregion
				
	#region Rotation Settings Vars
		template.alignToNormal =	(bool) GetProperty<bool>(ref propertyTable,"AlignToNormal",template.alignToNormal);//(bool) propertyTable["AlignToNormal"];
	
		template.flipNormalAlign =	(bool) GetProperty<bool>(ref propertyTable,"FlipNormalAlign",template.flipNormalAlign);//(bool) propertyTable["FlipNormalAlign"];
	
		template.alignToStroke =	(bool) GetProperty<bool>(ref propertyTable,"AlignToStroke",template.alignToStroke);//(bool) propertyTable["AlignToStroke"];
	
		template.flipStrokeAlign =	(bool) GetProperty<bool>(ref propertyTable,"FlipStrokeAlign",template.flipStrokeAlign);//(bool) propertyTable["FlipStrokeAlign"];
	
		template.rotationRangeMin.x = (float) GetProperty<float>(ref propertyTable,"RotationRangeMinX",template.rotationRangeMin.x);//(float) propertyTable["RotationRangeMinX"];
		template.rotationRangeMin.y = (float) GetProperty<float>(ref propertyTable,"RotationRangeMinY",template.rotationRangeMin.y);//(float) propertyTable["RotationRangeMinY"];
		template.rotationRangeMin.z = (float) GetProperty<float>(ref propertyTable,"RotationRangeMinZ",template.rotationRangeMin.z);//(float) propertyTable["RotationRangeMinZ"];
	
		template.rotationRangeMax.x = (float) GetProperty<float>(ref propertyTable,"RotationRangeMaxX",template.rotationRangeMax.x);//(float) propertyTable["RotationRangeMaxX"];
		template.rotationRangeMax.y = (float) GetProperty<float>(ref propertyTable,"RotationRangeMaxY",template.rotationRangeMax.y);//(float) propertyTable["RotationRangeMaxY"];
		template.rotationRangeMax.z = (float) GetProperty<float>(ref propertyTable,"RotationRangeMaxZ",template.rotationRangeMax.z);//(float) propertyTable["RotationRangeMaxZ"];
	#endregion
				
	#region Position Settings Vars
		template.positionOffset.x =	(float) GetProperty<float>(ref propertyTable,"PositionOffsetX",template.positionOffset.x);//(float) propertyTable["PositionOffsetX"];
		template.positionOffset.y = (float) GetProperty<float>(ref propertyTable,"PositionOffsetY",template.positionOffset.y);//(float) propertyTable["PositionOffsetY"];
		template.positionOffset.z = (float) GetProperty<float>(ref propertyTable,"PositionOffsetZ",template.positionOffset.z);//(float) propertyTable["PositionOffsetZ"];
	#endregion
				
	#region Scale Settings Vars	
		//The minimum and maximum possible scale
		template.scaleMin =			(float) GetProperty<float>(ref propertyTable,"ScaleMin",template.scaleMin);//(float) propertyTable["ScaleMin"];
		template.scaleMax =			(float) GetProperty<float>(ref propertyTable,"ScaleMax",template.scaleMax);//(float) propertyTable["ScaleMax"];
			
		//The minimum and maximum current scale range setting
		template.scaleRandMin.x =	(float) GetProperty<float>(ref propertyTable,"ScaleRandMinX",template.scaleRandMin.x);//(float) propertyTable["ScaleRandMinX"];
		template.scaleRandMin.y =	(float) GetProperty<float>(ref propertyTable,"ScaleRandMinY",template.scaleRandMin.y);//(float) propertyTable["ScaleRandMinY"];
		template.scaleRandMin.z =	(float) GetProperty<float>(ref propertyTable,"ScaleRandMinZ",template.scaleRandMin.z);//(float) propertyTable["ScaleRandMinZ"];
	
		template.scaleRandMax.x =	(float) GetProperty<float>(ref propertyTable,"ScaleRandMaxX",template.scaleRandMax.x);//(float) propertyTable["ScaleRandMaxX"];
		template.scaleRandMax.y =	(float) GetProperty<float>(ref propertyTable,"ScaleRandMaxY",template.scaleRandMax.y);//(float) propertyTable["ScaleRandMaxY"];
		template.scaleRandMax.z =	(float) GetProperty<float>(ref propertyTable,"ScaleRandMaxZ",template.scaleRandMax.z);//(float) propertyTable["ScaleRandMaxZ"];
	
		template.scaleRandMinUniform = (float) GetProperty<float>(ref propertyTable,"ScaleRandMinUniform",template.scaleRandMinUniform);//(float) propertyTable["ScaleRandMinUniform"];
		template.scaleRandMaxUniform = (float) GetProperty<float>(ref propertyTable,"ScaleRandMaxUniform",template.scaleRandMaxUniform);//(float) propertyTable["ScaleRandMaxUniform"];
	
		template.scaleUniform = 	(bool) GetProperty<bool>(ref propertyTable,"ScaleUniform",template.scaleUniform);//(bool) propertyTable["ScaleUniform"];
	#endregion	
				
	#region Sorting Vars  
		//Selection
		template.paintToSelection = (bool) GetProperty<bool>(ref propertyTable,"PaintToSelection",template.paintToSelection);//(bool) propertyTable["PaintToSelection"];
	
		//Layers
		template.paintToLayer =		(bool) GetProperty<bool>(ref propertyTable,"PaintToLayer",template.paintToLayer);//(bool) propertyTable["PaintToLayer"];
	
		template.layerIndex =		(int) GetProperty<int>(ref propertyTable,"LayerIndex",template.layerIndex);//(int) propertyTable["LayerIndex"];
		
		template.groupObjects =		(bool) GetProperty<bool>(ref propertyTable,"GroupObjects",template.groupObjects);
		
		template.groupIndex =		(int) GetProperty<int>(ref propertyTable,"GroupIndex",template.groupIndex);
	#endregion
		
	#region Eraser Vars	
		template.eraseByGroup =		(bool) GetProperty<bool>(ref propertyTable,"EraseByGroup",template.eraseByGroup);//(bool) propertyTable["EraseByGroup"];
		template.eraseBySelected =	(bool) GetProperty<bool>(ref propertyTable,"EraseBySelected",template.eraseBySelected);//(bool) propertyTable["EraseBySelected"];
	#endregion
		
	#region Repopulate the Prefab List
		string prefabGroupString =	(string) GetProperty<string>(ref propertyTable,"PrefabGUIDList",string.Empty);// (string) propertyTable["PrefabGUIDList"];
		qb_PrefabObject[] prefabGroup = new qb_PrefabObject[0];

		string[] prefabStringList = new string[0];
		List<UnityEngine.Object> newPrefabs = new List<UnityEngine.Object>();
		
		if(prefabGroupString != string.Empty)
		{
			//first clear out any items that are in the prefab list now
			prefabGroup = new qb_PrefabObject[0];
			//then retreive and split the saved prefab guids into a list
			prefabStringList = prefabGroupString.Split('/');//string;
			
		}
		
		foreach(string prefabString in prefabStringList)
		{
			
			if(prefabString == string.Empty)
				continue;

			int splitIndex = prefabString.IndexOf("-");
			string GUIDstring = string.Empty;
			string weightString = string.Empty;
				
			if(prefabString.Contains("-"))
			{	
				GUIDstring = prefabString.Substring(0,splitIndex);
				weightString = prefabString.Substring(splitIndex + 1);
			}
			
			else
			{
				GUIDstring = prefabString;
			}
			
			if(GUIDstring == string.Empty)
				continue;
			
			float itemWeight = 1f;
			if(weightString != null && weightString != string.Empty)
				itemWeight = System.Convert.ToSingle(weightString);
			
			string assetPath = AssetDatabase.GUIDToAssetPath(GUIDstring);
			Object item = AssetDatabase.LoadAssetAtPath(assetPath,typeof(Object));
			
			if(item != null)
			{
				newPrefabs.Add(item);
				
				ArrayUtility.Add(ref prefabGroup,new qb_PrefabObject(item,itemWeight));
			}
		}
		/*
		if(newPrefabs.Count > 0)
		{
			foreach(UnityEngine.Object newPrefab in newPrefabs)
			{
				ArrayUtility.Add(ref prefabGroup,new qb_PrefabObject(newPrefab,1f));
			}
		}
		*/
		template.prefabGroup = prefabGroup;
	#endregion
		
		template.live = true;
		
		return template;
		
	}	
	//A property getter to safeguard loading of obsolete files - returns default value if key is not present
	public static T GetProperty<T>(ref Hashtable propertyTable, string propertyKey, T defaultValue)
	{
		T result;
		
		if(propertyTable.ContainsKey(propertyKey))
			result =(T) propertyTable[propertyKey];
		
		else
			result = defaultValue;
		
		return result;
			
	}
	
	public static qb_TemplateSignature[] GetTemplateFileSignatures(string directory)
	{
		string[] directories = GetTemplateFileDirectories(directory + "/Templates/");
		
		qb_TemplateSignature[] signatures = new qb_TemplateSignature[directories.Length];
		
		for(int i = 0; i < signatures.Length; i++)
		{
			qb_TemplateSignature signature = LoadTemplateSignature(directories[i]);
			
			signatures[i] = signature;
		}
		
		return signatures;
	}
	
	static qb_Template[] GetSavedBrushes(string directory)
	{
		string[] directories = GetTemplateFileDirectories(directory);
		qb_Template[] brushes = new qb_Template[directories.Length];
		
		for(int i = 0; i < directories.Length; i++)
		{
			qb_Template brush = LoadTemplate(directories[i]);	
			brushes[i] = brush;
		}
		
		return brushes;
	}
	
	//Get brush files saved in the qb directory
	static string[] GetTemplateFileDirectories(string directory)
	{
		string[] directories = Directory.GetFiles(directory, "*.qbt");
		
		return directories;
	}
	
}

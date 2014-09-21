//	QuickBrush: Prefab Placement Tool
//	by PlayTangent
//	all rights reserved
//	www.ProCore3d.com

using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class qb_Template //: ScriptableObject
{
	public	bool	live = 					false; //Sets whether the template is active in the toolbar - allows blank with values but no active file
	
	public	string	brushName =				string.Empty;
	
#region Brush Settings Vars
	public	float	brushRadius	=			0.5f;
	
	public	float	brushRadiusMin =		0.2f;
	
	public	float	brushRadiusMax =		5f;
	
	public	float	brushSpacing	=		0.2f;
	
	public	float 	brushSpacingMin =		0.02f;
	
	public	float	brushSpacingMax =		2f;
	
	public	float	scatterRadius =			0f;
#endregion
			
#region Rotation Settings Vars
	public	bool	alignToNormal =			true;
	
	public	bool	flipNormalAlign =		false;
	
	public	bool	alignToStroke =			true;
	
	public	bool	flipStrokeAlign =		false;
	
	public	Vector3	rotationRangeMin =		new Vector3(0f,0f,0f);
		
	public	Vector3 rotationRangeMax =		new Vector3(0f,0f,0f);
#endregion
			
#region Position Settings Vars
	public	Vector3	positionOffset =		Vector3.zero;
#endregion
			
#region Scale Settings Vars	
	//The minimum and maximum possible scale
	public	float	scaleMin =				0.1f;
	
	public	float	scaleMax =				3f;
		
	//The minimum and maximum current scale range setting
	public	Vector3	scaleRandMin = 			new Vector3(1f,1f,1f);
	
	public	Vector3	scaleRandMax = 			new Vector3(1f,1f,1f);	
	
	public	float	scaleRandMinUniform =	1f;
	
	public	float	scaleRandMaxUniform =	1f;
	
	public	bool	scaleUniform =			true;
#endregion	
			
#region Sorting Vars  
	//Selection
	public	bool	paintToSelection =		false;

	//Layers
	public	bool	paintToLayer =			false;
	
	public	int		layerIndex =			0;
	
	public	bool	groupObjects =			false;
	
	public	int		groupIndex = 			0;
#endregion

#region Eraser Vars
	public	bool	eraseByGroup =			false;
	
	public 	bool	eraseBySelected =		false;
#endregion
	
	public	qb_PrefabObject[] prefabGroup =	new qb_PrefabObject[0];
	
	public	bool 	dirty = 				false;
	
}

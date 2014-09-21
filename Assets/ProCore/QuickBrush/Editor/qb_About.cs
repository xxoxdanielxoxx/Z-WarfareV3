//	QuickBrush: Prefab Placement Tool
//	by PlayTangent
//	all rights reserved
//	www.ProCore3d.com

using UnityEngine;
using UnityEditor;
using System.Collections;

public class qb_About : qb_Window
{
	[MenuItem ("Tools/QuickBrush/About")]
	public static void ShowWindow()
	{
		window = EditorWindow.GetWindow<qb_About>(false,"QB About");

	 	window.position = new Rect(50,50,400,180);
		window.maxSize = new Vector2(400,180);
		window.minSize = new Vector2(400,180);
	}
	
	public const string RELEASE_VERSION = "1.0.6-RC1";
	const string BUILD_DATE = "06-28-2014";
	
	void OnGUI()
	{
		BuildStyles();
		
		EditorGUILayout.Space();

		EditorGUILayout.BeginVertical();
			
			MenuListItem(false,true,"QuickBrush" + RELEASE_VERSION);
			
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Version Number:");
				EditorGUILayout.LabelField(RELEASE_VERSION);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Build Date:");
				EditorGUILayout.LabelField(BUILD_DATE);
			EditorGUILayout.EndHorizontal();
				
		EditorGUILayout.EndVertical();
	}
}
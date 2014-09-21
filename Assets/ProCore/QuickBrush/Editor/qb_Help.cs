//	QuickBrush: Prefab Placement Tool
//	by PlayTangent
//	all rights reserved
//	www.ProCore3d.com

using UnityEngine;
using UnityEditor;
using System.Collections;

public class qb_Help : qb_Window
{
	[MenuItem ("Tools/QuickBrush/Help")]
	public static void ShowWindow()
	{
		window = EditorWindow.GetWindow<qb_Help>(false,"QB Help");
		
		window.position = new Rect(50,50,400,600);
		window.maxSize = new Vector2(400,600);
		window.minSize = new Vector2(400,300);
    }
	
	static Vector2 sliderVal;
	
	void OnGUI()
	{
		BuildStyles();
		
		EditorGUILayout.Space();
		
		sliderVal = EditorGUILayout.BeginScrollView(sliderVal,false,false);
			
			MenuListItem(false,true,"Complete documentation & info at:");
			MenuListItem(false,true,"http://www.proCore3d.com/quickBrush/");
		EditorGUILayout.LabelField("Basic Usage",labelStyle_bold);
		
		EditorGUILayout.BeginVertical(menuBlockStyle);
			MenuListItem(true,"To begin, create a new brush template by clicking on the arrow symbol above one of the brush template slots near the bottom of the QuickBrush window and selecting 'New Template' from the dropdown that appears");
			MenuListItem(true,"Once you have saved some templates, this drop down will also contain the names of your saved templates, allowing you to select a template to load.");
			MenuListItem(true,"To save a new brush template to disk, type a name into the text field located directly above the template slots and then click on the save icon next to the field");
			MenuListItem(true,"When several slots contain brushe templates, you can switch between the templates by clicking on the large slot icon. The template name is displayed in the help box at the bottom of the QB window when the cursor is placed over any of the slots");
			MenuListItem(true,"To set up your brush, drag and drop a prefab (or prefabs) onto the outlined region near the top of the QuickBrush window. This adds it to the prefab list pane");
			MenuListItem(true,"Once in the list pane, each prefab item has a slider, a preview window, and two overlaid controls");
			MenuListItem(true,"The Slider dictates how likely each prefab in the list is to be spawned vs the others when using the brush to place objects");
			MenuListItem(true,"Clicking the 'Red X' overlaid button removes the item from the list");
			MenuListItem(true,"Clicking the 'Green Checkmark' toggles whether an object will be placed exclussively.");
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Painting Controls",labelStyle_bold);
		
		EditorGUILayout.BeginVertical(menuBlockStyle);
			MenuListItem(true,"The brush is toggled ON by holding down Ctrl/Cmd");
			MenuListItem(true,"To switch between painting and erasing, press the X key while holding down Ctrl/Cmd");
			MenuListItem(true,"To paint (or erase), click and drag with the mouse while holding Ctrl/Cmd");
			MenuListItem(true,"An additional percision placement mode is available by simultaneously holding down Shift and Ctr/Cmd");
			MenuListItem(true,"While precision placing, clicking the mouse spawns a single prefab. Dragging the cursor while still holding down the mouse allows you to scale and rotate the object being placed");
			MenuListItem(true,"To chose the object which will be percision placed using this control, toggle the green checkmark overlaying the preview pane for the prefab in the prefab list pane.");
			MenuListItem(true,"If no object is selected, QuickBrush will use the first item in the list (the one on the left)");
		EditorGUILayout.EndVertical();

		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField("FAQ (will expand with feedback)",labelStyle_bold);
		
		EditorGUILayout.BeginVertical(menuBlockStyle);
			
			EditorGUILayout.LabelField("Q: When I paint, my objects stack on top of one another instead of painting onto my chosen surface. What gives?",labelStyle);
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("A: It is likely that your prefabs have colliders and are layered such that QuickBrush is painting them onto one another. Use the layer settings in order to paint to a different layer than the one your prefabs are on.",labelStyle);
			EditorGUILayout.Space();
		
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.EndScrollView();
	}


	
}
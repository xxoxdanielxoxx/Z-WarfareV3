//	QuickBrush: Prefab Placement Tool
//	by PlayTangent
//	all rights reserved
//	www.ProCore3d.com

/**
 * A waterfall effect for defines.
 */
#if UNITY_4_5 || UNITY_4_5_0 || UNITY_4_5_1 || UNITY_4_5_2 || UNITY_4_5_3 || UNITY_4_5_4 || UNITY_4_5_5 || UNITY_4_5_6 || UNITY_4_5_7 || UNITY_4_5_8 || UNITY_4_5_9 || UNITY_4_6 || UNITY_4_6_0 || UNITY_4_6_1 || UNITY_4_6_2 || UNITY_4_6_3 || UNITY_4_6_4 || UNITY_4_6_5 || UNITY_4_6_6 || UNITY_4_6_7 || UNITY_4_6_8 || UNITY_4_6_9 || UNITY_4_7 || UNITY_4_7_0 || UNITY_4_7_1 || UNITY_4_7_2 || UNITY_4_7_3 || UNITY_4_7_4 || UNITY_4_7_5 || UNITY_4_7_6 || UNITY_4_7_7 || UNITY_4_7_8 || UNITY_4_7_9 || UNITY_4_8 || UNITY_4_8_0 || UNITY_4_8_1 || UNITY_4_8_2 || UNITY_4_8_3 || UNITY_4_8_4 || UNITY_4_8_5 || UNITY_4_8_6 || UNITY_4_8_7 || UNITY_4_8_8 || UNITY_4_8_9
#define UNITY_4_5
#endif
#if UNITY_4_3 || UNITY_4_3_0 || UNITY_4_3_1 || UNITY_4_3_2 || UNITY_4_3_3 || UNITY_4_3_4 || UNITY_4_3_5 || UNITY_4_3_6 || UNITY_4_3_7 || UNITY_4_3_8 || UNITY_4_3_9 || UNITY_4_4 || UNITY_4_4_0 || UNITY_4_4_1 || UNITY_4_4_2 || UNITY_4_4_3 || UNITY_4_4_4 || UNITY_4_4_5 || UNITY_4_4_6 || UNITY_4_4_7 || UNITY_4_4_8 || UNITY_4_4_9 || UNITY_4_5 || UNITY_4_5_0 || UNITY_4_5_1 || UNITY_4_5_2 || UNITY_4_5_3 || UNITY_4_5_4 || UNITY_4_5_5 || UNITY_4_5_6 || UNITY_4_5_7 || UNITY_4_5_8 || UNITY_4_5_9 || UNITY_4_6 || UNITY_4_6_0 || UNITY_4_6_1 || UNITY_4_6_2 || UNITY_4_6_3 || UNITY_4_6_4 || UNITY_4_6_5 || UNITY_4_6_6 || UNITY_4_6_7 || UNITY_4_6_8 || UNITY_4_6_9 || UNITY_4_7 || UNITY_4_7_0 || UNITY_4_7_1 || UNITY_4_7_2 || UNITY_4_7_3 || UNITY_4_7_4 || UNITY_4_7_5 || UNITY_4_7_6 || UNITY_4_7_7 || UNITY_4_7_8 || UNITY_4_7_9 || UNITY_4_8 || UNITY_4_8_0 || UNITY_4_8_1 || UNITY_4_8_2 || UNITY_4_8_3 || UNITY_4_8_4 || UNITY_4_8_5 || UNITY_4_8_6 || UNITY_4_8_7 || UNITY_4_8_8 || UNITY_4_8_9
#define UNITY_4_3
#elif UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
#define UNITY_4
#elif UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_3_6 || UNITY_3_5_7 || UNITY_3_8
#define UNITY_3
#endif

using UnityEngine;
using UnityEditor;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class qb_Painter : EditorWindow
{
	[MenuItem ("Tools/QuickBrush/ToolBar")]
	public static void ShowWindow()
	{
		window = EditorWindow.GetWindow<qb_Painter>(false,"QuickBrush");
		
		window.position = new Rect(50,50,284,600);
		window.minSize = new Vector2(284f,300f);
		window.maxSize = new Vector2(284f,800f);
    }
	
	#region Variable Declarations
	static qb_Painter				window;
	
	static string 					directory;
	
	static BrushMode 				brushMode;
	
	static bool						brushDirection =		true;		//Positive or negative - Indicates whether we are placing or erasing
	
	static bool						placementModifier = 	false;
	
	static SceneView.OnSceneFunc 	onSceneGUIFunc;
	
	
	static Texture2D				removePrefabXTexture_normal;
	static Texture2D				removePrefabXTexture_hover;
	static Texture2D				addPrefabTexture;
	static Texture2D				addPrefabFieldTexture;
	static Texture2D				selectPrefabCheckTexture_normal;
	static Texture2D				selectPrefabCheckTexture_hover;
	static Texture2D				prefabFieldBackgroundTexture;
	static Texture2D				brushIcon_Active;
	static Texture2D				brushIcon_Inactive;
	static Texture2D				eraserIcon_Active;
	static Texture2D				eraserIcon_Inactive;
	static Texture2D				placementIcon_Active;
	//static Texture2D				saveBrushIcon;
	//static Texture2D				saveBrushIcon_hover;
	static Texture2D				loadBrushIcon;
	static Texture2D				loadBrushIcon_hover;
	static Texture2D				saveIcon;				
	static Texture2D				saveIcon_hover;
	static Texture2D				clearBrushIcon;
	static Texture2D				clearBrushIcon_hover;
	static Texture2D				savedBrushIcon;
	static Texture2D				savedBrushIcon_Active;
	static Texture2D				resetSliderIcon;
	static Texture2D				resetSliderIcon_hover;
	#endregion
	
	
	void OnEnable()
	{
		window = this;
		onSceneGUIFunc = this.OnSceneGUI;
		SceneView.onSceneGUIDelegate += onSceneGUIFunc;
		
		directory = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));		
		
		if(directory != string.Empty)
		{
			directory = directory.Replace("/Editor/qb_Painter.cs","");
		}
		
		string skinPath = directory + "/Resources/Skin/";
		
		addPrefabTexture			=		Resources.LoadAssetAtPath(skinPath + "qb_addPrefabIcon.tga", typeof(Texture2D)) as Texture2D;
		addPrefabFieldTexture		=		Resources.LoadAssetAtPath(skinPath + "qb_addPrefabField.tga", typeof(Texture2D)) as Texture2D;
		removePrefabXTexture_normal	=		Resources.LoadAssetAtPath(skinPath + "qb_removePrefabXIcon_normal.tga", typeof(Texture2D)) as Texture2D;
		removePrefabXTexture_hover 	=		Resources.LoadAssetAtPath(skinPath + "qb_removePrefabXIcon_hover.tga", typeof(Texture2D)) as Texture2D;
		
		selectPrefabCheckTexture_normal =	Resources.LoadAssetAtPath(skinPath + "qb_selectPrefabCheck_normal.tga", typeof(Texture2D)) as Texture2D;
		selectPrefabCheckTexture_hover 	=	Resources.LoadAssetAtPath(skinPath + "qb_selectPrefabCheck_hover.tga", typeof(Texture2D)) as Texture2D;
		prefabFieldBackgroundTexture 	=	Resources.LoadAssetAtPath(skinPath + "qb_prefabFieldBackground.tga", typeof(Texture2D)) as Texture2D;
		
		brushIcon_Active 		=			Resources.LoadAssetAtPath(skinPath + "qb_brushIcon_Active.tga", typeof(Texture2D)) as Texture2D;
		brushIcon_Inactive 		=			Resources.LoadAssetAtPath(skinPath + "qb_brushIcon_Inactive.tga", typeof(Texture2D)) as Texture2D;
		
		eraserIcon_Active		=			Resources.LoadAssetAtPath(skinPath + "qb_eraserIcon_Active.tga", typeof(Texture2D)) as Texture2D;
		eraserIcon_Inactive		=			Resources.LoadAssetAtPath(skinPath + "qb_eraserIcon_Inactive.tga", typeof(Texture2D)) as Texture2D;		
		
		placementIcon_Active	=			Resources.LoadAssetAtPath(skinPath + "qb_placementIcon_Active.tga", typeof(Texture2D)) as Texture2D;
		
		savedBrushIcon			=			Resources.LoadAssetAtPath(skinPath + "qb_SavedBrushIcon.tga", typeof(Texture2D)) as Texture2D;
		savedBrushIcon_Active	=			Resources.LoadAssetAtPath(skinPath + "qb_SavedBrushIcon_Active.tga", typeof(Texture2D)) as Texture2D;
		
		//This 
		//saveBrushIcon			=			Resources.LoadAssetAtPath(skinPath + "qb_SaveBrushIcon.tga", typeof(Texture2D)) as Texture2D;
		//saveBrushIcon_hover		=			Resources.LoadAssetAtPath(skinPath + "qb_SaveBrushIcon_hover.tga", typeof(Texture2D)) as Texture2D;
		loadBrushIcon			=			Resources.LoadAssetAtPath(skinPath + "qb_LoadBrushIcon.tga", typeof(Texture2D)) as Texture2D;
		loadBrushIcon_hover		=			Resources.LoadAssetAtPath(skinPath + "qb_LoadBrushIcon_hover.tga", typeof(Texture2D)) as Texture2D;;
		
		saveIcon				=			Resources.LoadAssetAtPath(skinPath + "qb_SaveIcon.tga", typeof(Texture2D)) as Texture2D;
		saveIcon_hover			=			Resources.LoadAssetAtPath(skinPath + "qb_SaveIcon_hover.tga", typeof(Texture2D)) as Texture2D;
		
		clearBrushIcon			= 			Resources.LoadAssetAtPath(skinPath + "qb_ClearBrushIcon.tga", typeof(Texture2D)) as Texture2D;
		clearBrushIcon_hover	=			Resources.LoadAssetAtPath(skinPath + "qb_ClearBrushIcon_hover.tga", typeof(Texture2D)) as Texture2D;

		resetSliderIcon			=			Resources.LoadAssetAtPath(skinPath + "qb_ResetSliderIcon.tga", typeof(Texture2D)) as Texture2D;
		resetSliderIcon_hover	=			Resources.LoadAssetAtPath(skinPath + "qb_ResetSliderIcon_hover.tga", typeof(Texture2D)) as Texture2D;
		
		UpdateGroups();
		
	//	UpdateTemplateSignatures();
		
		EnableMenu();
		ClearForm();
		
	}
	
	void OnDisable()
	{
		DisableMenu();
	}
		
	void ClearForm()
	{
		Repaint();
	}
	
	void EnableMenu()
	{
		brushDirection = true;
		brushMode = BrushMode.Off;
//		liveTemplate = new qb_Template();//ScriptableObject.CreateInstance<qb_Template>();//
	}
	
	void DisableMenu()
	{
		brushDirection = true;
		brushMode = BrushMode.Off;
/*		brushSettingsFoldout = false;
		sortingFoldout = false;
		rotationFoldout = false;
		positionFoldout = false;
		scaleFoldout = false;
		eraserFoldout = false;
//		selectedPrefabIndex = -1;
*/	
	}
	
#region Foldouts
	[SerializeField] private bool			brushSettingsFoldout = 	false;
	[SerializeField] private bool			rotationFoldout = 		false;
	[SerializeField] private bool			scaleFoldout = 			false;
	[SerializeField] private bool			positionFoldout = 		false;
	[SerializeField] private bool			sortingFoldout =		false;
	[SerializeField] private bool			eraserFoldout = 		false;
#endregion

#region Live Vars
	//Templates
//	[SerializeField] static qb_Template[]	testTemplates =			new qb_Template[6];
	[SerializeField] private qb_Template[]	brushTemplates =		new qb_Template[6];
	[SerializeField] private qb_Template	liveTemplate =			new qb_Template();
	[SerializeField] private int			templateIndex =			-1;
	
	static qb_TemplateSignature[]			templateSignatures;
	
	private bool			clearSelection = 		true;
	
	//Painting
	static bool 			paintable =				false;			//set by the mouse raycast to control whether an object can be painted on
	static qb_Stroke		curStroke;
	static qb_Point			cursorPoint =			new qb_Point();
	
	//Groups
	static List<qb_Group> 	groups = 				new List<qb_Group>();
	static List<string>		groupNames = 			new List<string>();
	
	static qb_Group			curGroup;
	static string			newGroupName = 			"";
	
	//Layers
	private LayerMask 		layersMasked =			0;
	
	//Prefab Section
	private Vector2			prefabFieldScrollPosition;
	private Object			newPrefab;
	private Object			previousPrefab;
	private List<int>		removalList;
	private int				selectedPrefabIndex = 	-1;
	
	//Menu Scrolling
	static Vector2 			topScroll =				Vector2.zero;

	//Tool Tip
	private string			curTip =				string.Empty;
	private bool			drawCurTip =			false; //this is set false on each redraw and checked at the end of OnGUI - set by DoTipCheck if a control with a tip is currently moused over
#endregion	
	
	void OnGUI()
	{
		UpdateGroups();
		BuildStyles();
		drawCurTip = false;

		EditorGUILayout.Space();
		
	EditorGUILayout.BeginVertical(masterVerticalStyle,GUILayout.Width(280)); //Begin Master Vertical
		
		EditorGUILayout.BeginHorizontal();//Brush Toggles Section Begin
		
			Texture2D brushIndicatorTexture = null;
			
			switch(brushMode)
			{
				case BrushMode.Off:
					if(brushDirection == true)
					{
						brushIndicatorTexture = brushIcon_Inactive;
					}
					
					if(brushDirection == false)
					{
						brushIndicatorTexture = eraserIcon_Inactive;
					}
				break;
				
				case BrushMode.On:
					
				if(placementModifier)
					brushIndicatorTexture = placementIcon_Active;

				else
				{
					if(brushDirection == true)
					{
						brushIndicatorTexture = brushIcon_Active;
					}
					
					if(brushDirection == false)
					{
						brushIndicatorTexture = eraserIcon_Active;
					}
				}
				break;
			}

			GUILayout.Label(brushIndicatorTexture,picLabelStyle,GUILayout.Width(32),GUILayout.Height(32));
			DoTipCheck("Brush On/Off Indicator");
			
		EditorGUI.BeginDisabledGroup(true);
			GUILayout.Label("Use Brush:" + System.Environment.NewLine + "Precise Place:" + System.Environment.NewLine + "Toggle Eraser:",tipLabelStyle,GUILayout.Width(90),GUILayout.Height(34)); DoTipCheck("Brush On/Off Indicator" + System.Environment.NewLine + "hold ctrl to paint");
			GUILayout.Label("ctrl+click/drag mouse"+ System.Environment.NewLine + "ctrl+shift+click/drag mouse" + System.Environment.NewLine + "ctrl+x" ,tipLabelStyle,GUILayout.Width(146),GUILayout.Height(32)); DoTipCheck("Brush On/Off Indicator" + System.Environment.NewLine + "hold ctrl to paint");
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndHorizontal(); // Brush Toggles Section End
		
		
		#region Prefab Picker	
				List<Object> newPrefabs = new List<Object>(); 
				removalList = new List<int>();
		
	EditorGUI.BeginDisabledGroup(liveTemplate.live == false);//Overall Disable Start
		
				EditorGUILayout.BeginHorizontal();
						
				if(liveTemplate.prefabGroup.Length == 0)
				{
					EditorGUILayout.BeginVertical(GUILayout.Height(78));
					newPrefabs = PrefabDragBox(274,addPrefabFieldTexture,"Drag & Drop Prefabs Here");	DoTipCheck("Drag & Drop Prefab Here To Add");
					EditorGUILayout.EndVertical();
				}
				
				else
				{
					newPrefabs = PrefabDragBox(30,addPrefabTexture,"");		DoTipCheck("Drag & Drop Prefab Here To Add");
				
					prefabFieldScrollPosition = EditorGUILayout.BeginScrollView(prefabFieldScrollPosition,GUILayout.Height(78), GUILayout.Width(246));
					EditorGUILayout.BeginHorizontal();
					//Prefab Objects can be dragged or selected in this horizontal list
			
					for(int i = 0; i < liveTemplate.prefabGroup.Length; i++)
					{
					EditorGUI.BeginChangeCheck();

						EditorGUILayout.BeginHorizontal(prefabFieldStyle,GUILayout.Height(60), GUILayout.Width(70));
						
						liveTemplate.prefabGroup[i].weight = GUILayout.VerticalSlider(liveTemplate.prefabGroup[i].weight,1f,0.001f,prefabAmountSliderStyle,prefabAmountSliderThumbStyle,GUILayout.Height(50)); DoTipCheck("Likelyhood that this object will be placed vs the others in the list");
						
					if(EditorGUI.EndChangeCheck())
					{
						liveTemplate.dirty = true;
					}
				
						EditorGUILayout.BeginVertical();
	
	                    Texture2D previewTexture = null;
				
						#if UNITY_3_5
							previewTexture =  EditorUtility.GetAssetPreview(liveTemplate.prefabGroup[i].prefab);
						#else
							previewTexture = AssetPreview.GetAssetPreview(liveTemplate.prefabGroup[i].prefab);
	                    #endif
	
							prefabPreviewWindowStyle.normal.background = previewTexture;//previewTexture;
				
							GUILayout.Label("",prefabPreviewWindowStyle,GUILayout.Height(50),GUILayout.Width(50));
							
							Rect prefabButtonRect = GUILayoutUtility.GetLastRect();
							Rect xControlRect = new Rect(prefabButtonRect.xMax - 14,prefabButtonRect.yMin,14,14);
							
							if(GUI.Button(xControlRect,"",prefabRemoveXStyle))
							{	removalList.Add(i);
								Event.current.Use();
							} DoTipCheck("'Red X' = remove prefab from list" + System.Environment.NewLine + "'Green Check' = mark to place exclusively");
							
							Rect checkControlRect = new Rect(prefabButtonRect.xMax - 14,prefabButtonRect.yMax - 14,14,14);
							
							if(selectedPrefabIndex == i)
								prefabSelectCheckStyle.normal.background = selectPrefabCheckTexture_hover;
							
							else
								prefabSelectCheckStyle.normal.background = selectPrefabCheckTexture_normal;
							
							
							if(GUI.Button(checkControlRect,"",prefabSelectCheckStyle))
							{
								if(selectedPrefabIndex != i)
									selectedPrefabIndex = i;
					
								else
									selectedPrefabIndex = -1;
					
								liveTemplate.dirty = true;
								clearSelection = true;
							} //DoTipCheck("Click to mark object as selected - it will be placed exclusively");
							
						EditorGUILayout.EndVertical();
					
						EditorGUILayout.EndHorizontal();
					}
			
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndScrollView();
				}
		
				EditorGUILayout.EndHorizontal();
		
			if(removalList.Count > 0)
			{
				foreach(int index in removalList)
				{
					if(selectedPrefabIndex > index)
					{
						selectedPrefabIndex -= 1;
					}
			
					else if(selectedPrefabIndex == index)
					{
						selectedPrefabIndex = -1;
					}
				
					ArrayUtility.RemoveAt(ref liveTemplate.prefabGroup,index);
				}
			
				liveTemplate.dirty = true;
				clearSelection = true;
//				EditorUtility.SetDirty(prefabGroup);
			}
		
 			if(newPrefabs.Count > 0)
			{
				foreach(Object newPrefab in newPrefabs)
				{
					ArrayUtility.Add(ref liveTemplate.prefabGroup,new qb_PrefabObject(newPrefab,1f));
				}
//				added = true;
//			    EditorUtility.SetDirty(prefabGroup);
			}
	
		#endregion
		
		
		topScroll = EditorGUILayout.BeginScrollView(topScroll,GUILayout.Width(280));
		EditorGUILayout.BeginVertical(GUILayout.Width(260));
		
		#region	Stroke Properties
			brushSettingsFoldout = EditorGUILayout.Foldout(brushSettingsFoldout,"Brush Settings:"); DoTipCheck("Brush and Stroke settings");
		
			if(brushSettingsFoldout == true)
			{
			EditorGUI.BeginChangeCheck();

			
                EditorGUILayout.BeginVertical(menuBlockStyle,GUILayout.Width(260));
								
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Brush Radius",GUILayout.Width(100)); DoTipCheck("The Size of the brush");
						liveTemplate.brushRadius = EditorGUILayout.Slider(liveTemplate.brushRadius,liveTemplate.brushRadiusMin,liveTemplate.brushRadiusMax); DoTipCheck("The Size of the brush");
					EditorGUILayout.EndHorizontal();
			
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Min",GUILayout.Width(70)); DoTipCheck("Minimum Slider Value");
						float tryRadiusMin = EditorGUILayout.FloatField(liveTemplate.brushRadiusMin,floatFieldCompressedStyle);  DoTipCheck("Minimum Slider Value");
						liveTemplate.brushRadiusMin = tryRadiusMin < liveTemplate.brushRadiusMax ? tryRadiusMin : liveTemplate.brushRadiusMax;
			
						EditorGUILayout.LabelField("Max",GUILayout.Width(70)); DoTipCheck("Maximum Slider Value");
						float tryRadiusMax = EditorGUILayout.FloatField(liveTemplate.brushRadiusMax,floatFieldCompressedStyle); DoTipCheck("Maximum Slider Value");
						liveTemplate.brushRadiusMax = tryRadiusMax > liveTemplate.brushRadiusMin ? tryRadiusMax : liveTemplate.brushRadiusMin;
					EditorGUILayout.EndHorizontal();
			
			
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Scatter Amount",GUILayout.Width(100)); DoTipCheck("How closely should scattering match brush radius");
						liveTemplate.scatterRadius = EditorGUILayout.Slider(liveTemplate.scatterRadius,0f,1f); DoTipCheck("How closely should scattering match total brush radius");
					EditorGUILayout.EndHorizontal();
			
				EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(menuBlockStyle,GUILayout.Width(260));
			
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Stroke Spacing",GUILayout.Width(100)); DoTipCheck("Distance between brush itterations");
						liveTemplate.brushSpacing = EditorGUILayout.Slider(liveTemplate.brushSpacing,liveTemplate.brushSpacingMin,liveTemplate.brushSpacingMax); DoTipCheck("Distance between brush itterations");
					EditorGUILayout.EndHorizontal();
			
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Min",GUILayout.Width(70)); DoTipCheck("Minimum Slider Value");
						float trySpacingMin = EditorGUILayout.FloatField(liveTemplate.brushSpacingMin,floatFieldCompressedStyle); DoTipCheck("Minimum Slider Value");
						liveTemplate.brushSpacingMin = trySpacingMin < liveTemplate.brushSpacingMax ? trySpacingMin : liveTemplate.brushSpacingMax;
			
						EditorGUILayout.LabelField("Max",GUILayout.Width(70)); DoTipCheck("Maximum Slider Value");
						float trySpacingMax = EditorGUILayout.FloatField(liveTemplate.brushSpacingMax,floatFieldCompressedStyle); DoTipCheck("Maximum Slider Value");
						liveTemplate.brushSpacingMax = trySpacingMax > liveTemplate.brushSpacingMin ? trySpacingMax : liveTemplate.brushSpacingMin;
					EditorGUILayout.EndHorizontal();
			
				EditorGUILayout.EndVertical();
			
			if(EditorGUI.EndChangeCheck())
			{
				liveTemplate.dirty = true;
			}
		
			}
		#endregion
		
		EditorGUILayout.Space();

		sortingFoldout = EditorGUILayout.Foldout(sortingFoldout,"Sorting Settings:"); DoTipCheck("Grouping and Layer settings");
		if(sortingFoldout == true)
		{
		EditorGUI.BeginChangeCheck();

		#region Layers		
            EditorGUILayout.BeginVertical(menuBlockStyle,GUILayout.Width(260));
				//A toggle determining whether to isolate painting to specific layers
				liveTemplate.paintToLayer = EditorGUILayout.Toggle("Paint to Layer", liveTemplate.paintToLayer,EditorStyles.toggle); DoTipCheck("Restrict painting to specific layers?");
				//A dropdown where the user can check off which layers to paint to
				EditorGUI.BeginDisabledGroup(!liveTemplate.paintToLayer);
				liveTemplate.layerIndex = EditorGUILayout.MaskField("Choose Layers",liveTemplate.layerIndex,GetLayerList(),EditorStyles.layerMaskField); DoTipCheck("Choose the layers to paint onto");
				layersMasked = liveTemplate.layerIndex;

				EditorGUI.EndDisabledGroup();
				
				liveTemplate.paintToSelection = EditorGUILayout.Toggle("Restrict to Selection", liveTemplate.paintToSelection); DoTipCheck("Restrinct painting to selected objects in the scene - stacks with Layer Settings");

			EditorGUILayout.EndVertical();
		#endregion	
			
		#region Groups
            EditorGUILayout.BeginVertical(menuBlockStyle,GUILayout.Width(260));
						
			liveTemplate.groupObjects = EditorGUILayout.Toggle("Group Placed Objects",liveTemplate.groupObjects); DoTipCheck("Parent placed objects to an in-scene group object?");
			
			EditorGUI.BeginDisabledGroup(!liveTemplate.groupObjects);
			liveTemplate.groupIndex = EditorGUILayout.Popup("Choose Existing Group",liveTemplate.groupIndex,groupNames.ToArray()); DoTipCheck("Pick a previously created group already in the scene");
			
		
			if(groups.Count != 0)
			{
				if(groups.Count > liveTemplate.groupIndex)
					curGroup = groups[liveTemplate.groupIndex];
			
				else
				{
					liveTemplate.groupIndex = 0;
					liveTemplate.groupObjects = false;
				}
			}
			
		if(EditorGUI.EndChangeCheck())
		{
			liveTemplate.dirty = true;
		}
			
				EditorGUILayout.BeginHorizontal();
					newGroupName = EditorGUILayout.TextField("Name New Group",newGroupName,GUILayout.Width(210)); DoTipCheck("Enter a name for a new group you'd like to add");
			
					EditorGUI.BeginDisabledGroup(newGroupName == "");
						if(GUILayout.Button("Add",GUILayout.Width(38)))
						{
							clearSelection = true;
				
							if(GroupWithNameExists(newGroupName))
							{
								EditorUtility.DisplayDialog("Group Name Conflict","A Group named '" + newGroupName + "' already exists. Please choose a different name for your new group." ,"Ok");
								newGroupName = "";
							}
							else
							{
								CreateGroup(newGroupName);
								newGroupName = "";
							}
				
						}  DoTipCheck("Create your newly named group in the scene");
					EditorGUI.EndDisabledGroup();
			
				EditorGUILayout.EndHorizontal();
			
			EditorGUI.EndDisabledGroup();
	
			EditorGUILayout.EndVertical();
		#endregion
		}
		
		EditorGUILayout.Space();
		
		#region Rotation
		rotationFoldout = EditorGUILayout.Foldout(rotationFoldout,"Object Rotation:"); DoTipCheck("Settings for Offsetting the rotation of placed objects");
		if(rotationFoldout == true)
		{
		EditorGUI.BeginChangeCheck();

				EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginVertical(menuBlockStyle,GUILayout.Width(260));
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Align to Surface",GUILayout.Width(130)); DoTipCheck("Placed objects orient based on the surface normals of the painting surface");
						liveTemplate.alignToNormal = EditorGUILayout.Toggle(liveTemplate.alignToNormal,GUILayout.Width(14)); DoTipCheck("Placed objects orient based on the surface normals of the painting surface");
							GUILayout.Space(42);
						EditorGUI.BeginDisabledGroup(!liveTemplate.alignToNormal);
						EditorGUILayout.LabelField("Flip",GUILayout.Width(40)); DoTipCheck("Flip object's up axis along the surface");
							liveTemplate.flipNormalAlign = EditorGUILayout.Toggle(liveTemplate.flipNormalAlign,GUILayout.Width(14)); DoTipCheck("Flip object's up axis along the surface");		
						EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
						
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Align to Stroke",GUILayout.Width(130)); DoTipCheck("Placed objects face in the direction of painting");
						liveTemplate.alignToStroke = EditorGUILayout.Toggle(liveTemplate.alignToStroke,GUILayout.Width(14)); DoTipCheck("Placed objects face in the direction of painting");
							GUILayout.Space(42);
						EditorGUI.BeginDisabledGroup(!liveTemplate.alignToStroke);
							EditorGUILayout.LabelField("Flip",GUILayout.Width(40)); DoTipCheck("Flip object's forward axis along the stroke");
							liveTemplate.flipStrokeAlign = EditorGUILayout.Toggle(liveTemplate.flipStrokeAlign,GUILayout.Width(14)); DoTipCheck("Flip object's forward axis along the stroke");
						EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
			
					EditorGUILayout.EndVertical();
			
				EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical(menuBlockStyle,GUILayout.Width(260));
			
								picButtonStyle.normal.background = resetSliderIcon;
								picButtonStyle.hover.background = resetSliderIcon_hover;
			
						EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Offset X",GUILayout.Width(106)); DoTipCheck("Limits (in degrees) to randomly offset object rotation around the X axis");
							//rotationRange.x = EditorGUILayout.Slider(rotationRange.x,0f,180f); DoTipCheck("Upper limit (in degrees) to randomly offset object rotation around the X axis");
								EditorGUILayout.BeginHorizontal(saveIconContainerStyle);
								if(GUILayout.Button("",picButtonStyle,GUILayout.Width(16),GUILayout.Height(16)))
								{	liveTemplate.rotationRangeMin.x = 0f;
									liveTemplate.rotationRangeMax.x = 0f;
									liveTemplate.dirty = true;
									clearSelection = true;

								} DoTipCheck("Reset slider to 0");
								EditorGUILayout.EndHorizontal();
							//EditorGUILayout.LabelField(liveTemplate.rotationRangeMin.x.ToString("000"),GUILayout.Width(30)); DoTipCheck("Limits (in degrees) to randomly offset object rotation around the X axis");
							float inputRotMinX = (float)EditorGUILayout.IntField((int)liveTemplate.rotationRangeMin.x, GUILayout.Width(32));
							liveTemplate.rotationRangeMin.x = inputRotMinX < liveTemplate.rotationRangeMax.x ? inputRotMinX : liveTemplate.rotationRangeMax.x; DoTipCheck("Limits (in degrees) to randomly offset object rotation around the X axis");
								EditorGUILayout.MinMaxSlider(ref liveTemplate.rotationRangeMin.x,ref liveTemplate.rotationRangeMax.x,-180f,180); DoTipCheck("Limits (in degrees) to randomly offset object rotation around the X axis");
								liveTemplate.rotationRangeMin.x = (float)System.Math.Round(liveTemplate.rotationRangeMin.x,0);
								liveTemplate.rotationRangeMax.x = (float)System.Math.Round(liveTemplate.rotationRangeMax.x,0);
							float inputRotMaxX = (float)EditorGUILayout.IntField((int)liveTemplate.rotationRangeMax.x, GUILayout.Width(32));
							liveTemplate.rotationRangeMax.x = inputRotMaxX > liveTemplate.rotationRangeMin.x ? inputRotMaxX : liveTemplate.rotationRangeMin.x; DoTipCheck("Limits (in degrees) to randomly offset object rotation around the X axis");						
							//EditorGUILayout.LabelField(liveTemplate.rotationRangeMax.x.ToString("000"),GUILayout.Width(30)); DoTipCheck("Limits (in degrees) to randomly offset object rotation around the X axis");
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Offset Y (up)",GUILayout.Width(106)); DoTipCheck("Limits (in degrees) to randomly offset object rotation around the Y axis");
							//rotationRange.y = EditorGUILayout.Slider(rotationRange.y,0f,180f); DoTipCheck("Upper limit (in degrees) to randomly offset object rotation around the Y axis");
								EditorGUILayout.BeginHorizontal(saveIconContainerStyle);
								if(GUILayout.Button("",picButtonStyle,GUILayout.Width(16),GUILayout.Height(16)))
								{	liveTemplate.rotationRangeMin.y = 0f;
									liveTemplate.rotationRangeMax.y = 0f;
									liveTemplate.dirty = true;
									clearSelection = true;

								} DoTipCheck("Reset slider to 0");
							EditorGUILayout.EndHorizontal();
							//EditorGUILayout.LabelField(liveTemplate.rotationRangeMin.y.ToString("000"),GUILayout.Width(30)); DoTipCheck("Limits (in degrees) to randomly offset object rotation around the Y axis");
							float inputRotMinY = (float)EditorGUILayout.IntField((int)liveTemplate.rotationRangeMin.y, GUILayout.Width(32));
							liveTemplate.rotationRangeMin.y = inputRotMinY < liveTemplate.rotationRangeMax.y ? inputRotMinY : liveTemplate.rotationRangeMax.y; DoTipCheck("Limits (in degrees) to randomly offset object rotation around the Y axis");		
								EditorGUILayout.MinMaxSlider(ref liveTemplate.rotationRangeMin.y,ref liveTemplate.rotationRangeMax.y,-180f,180); DoTipCheck("Limits (in degrees) to randomly offset object rotation around the Y axis");
								liveTemplate.rotationRangeMin.y = (float)System.Math.Round(liveTemplate.rotationRangeMin.y,0);
								liveTemplate.rotationRangeMax.y = (float)System.Math.Round(liveTemplate.rotationRangeMax.y,0);
							float inputRotMaxY = (float)EditorGUILayout.IntField((int)liveTemplate.rotationRangeMax.y, GUILayout.Width(32));
							liveTemplate.rotationRangeMax.y = inputRotMaxY > liveTemplate.rotationRangeMin.y ? inputRotMaxY : liveTemplate.rotationRangeMin.y; DoTipCheck("Limits (in degrees) to randomly offset object rotation around the Y axis");				
							//EditorGUILayout.LabelField(liveTemplate.rotationRangeMax.y.ToString("000"),GUILayout.Width(30)); DoTipCheck("Limits (in degrees) to randomly offset object rotation around the Y axis");
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Offset Z",GUILayout.Width(106)); DoTipCheck("Limits (in degrees) to randomly offset object rotation around the Z axis");
							//rotationRange.z = EditorGUILayout.Slider(rotationRange.z,0f,180f); DoTipCheck("Upper limit (in degrees) to randomly offset object rotation around the Z axis");
								EditorGUILayout.BeginHorizontal(saveIconContainerStyle);
								if(GUILayout.Button("",picButtonStyle,GUILayout.Width(16),GUILayout.Height(16)))
								{	liveTemplate.rotationRangeMin.z = 0f;
									liveTemplate.rotationRangeMax.z = 0f;
									liveTemplate.dirty = true;
									clearSelection = true;
				
								} DoTipCheck("Reset slider to 0");
								EditorGUILayout.EndHorizontal();
							//EditorGUILayout.LabelField(liveTemplate.rotationRangeMin.z.ToString("000"),GUILayout.Width(30)); DoTipCheck("Limits (in degrees) to randomly offset object rotation around the Z axis");
							float inputRotMinZ = (float)EditorGUILayout.IntField((int)liveTemplate.rotationRangeMin.z, GUILayout.Width(32));
							liveTemplate.rotationRangeMin.z = inputRotMinZ < liveTemplate.rotationRangeMax.z ? inputRotMinZ : liveTemplate.rotationRangeMax.z; DoTipCheck("Limits (in degrees) to randomly offset object rotation around the Z axis");					
								EditorGUILayout.MinMaxSlider(ref liveTemplate.rotationRangeMin.z,ref liveTemplate.rotationRangeMax.z,-180f,180); DoTipCheck("Limits (in degrees) to randomly offset object rotation around the Z axis");
								liveTemplate.rotationRangeMin.z = (float)System.Math.Round(liveTemplate.rotationRangeMin.z,0);
								liveTemplate.rotationRangeMax.z = (float)System.Math.Round(liveTemplate.rotationRangeMax.z,0);
							float inputRotMaxZ = (float)EditorGUILayout.IntField((int)liveTemplate.rotationRangeMax.z, GUILayout.Width(32));
							liveTemplate.rotationRangeMax.z = inputRotMaxZ > liveTemplate.rotationRangeMin.z ? inputRotMaxZ : liveTemplate.rotationRangeMin.z; DoTipCheck("Limits (in degrees) to randomly offset object rotation around the Z axis");									
							//EditorGUILayout.LabelField(liveTemplate.rotationRangeMax.z.ToString("000"),GUILayout.Width(30)); DoTipCheck("Limits (in degrees) to randomly offset object rotation around the Z axis");

						EditorGUILayout.EndHorizontal();
			
					EditorGUILayout.EndVertical();
	
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.EndVertical();
			
			if(EditorGUI.EndChangeCheck())
			{
				liveTemplate.dirty = true;
			}
			}
		#endregion
			
		EditorGUILayout.Space();
		
		#region Position
		positionFoldout = EditorGUILayout.Foldout(positionFoldout,"Object Position:"); DoTipCheck("Settings for Offsetting the rotation of placed objects");
		if(positionFoldout == true)
		{
		EditorGUI.BeginChangeCheck();
			
			EditorGUILayout.BeginVertical(menuBlockStyle,GUILayout.Width(260));
			
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Offset X", GUILayout.Width(130)); DoTipCheck("Offset final placement position along local X axis");
					liveTemplate.positionOffset.x = EditorGUILayout.FloatField(liveTemplate.positionOffset.x); DoTipCheck("Offset final placement position along local X axis");
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();	
					EditorGUILayout.LabelField("Offset Y (Up/Down)", GUILayout.Width(130)); DoTipCheck("Offset final placement position along local Y axis");
					liveTemplate.positionOffset.y = EditorGUILayout.FloatField(liveTemplate.positionOffset.y); DoTipCheck("Offset final placement position along local Y axis");
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Offset Z", GUILayout.Width(130)); DoTipCheck("Offset final placement position along local Z axis");
					liveTemplate.positionOffset.z = EditorGUILayout.FloatField(liveTemplate.positionOffset.z); DoTipCheck("Offset final position placement along local Z axis");
				EditorGUILayout.EndHorizontal();
				
			EditorGUILayout.EndHorizontal();
			
		if(EditorGUI.EndChangeCheck())
		{
			liveTemplate.dirty = true;
		}
		}
		#endregion 
		
		EditorGUILayout.Space();
		
		#region Scale		
		scaleFoldout = EditorGUILayout.Foldout(scaleFoldout,"Object Scale:",EditorStyles.foldout); DoTipCheck("Settings for Offsetting the scale of placed objects");
		if(scaleFoldout == true)
		{
		EditorGUI.BeginChangeCheck();
			
				EditorGUILayout.BeginHorizontal();

					liveTemplate.scaleUniform = EditorGUILayout.Toggle(liveTemplate.scaleUniform,toggleButtonStyle,GUILayout.Width(15)); DoTipCheck("Placed models are scaled the same on all axes");
					GUILayout.Label("Uniform Scale"); DoTipCheck("Placed models are scaled the same on all axes");
				
				EditorGUILayout.EndHorizontal();
					
				EditorGUI.BeginDisabledGroup(!liveTemplate.scaleUniform);
				
					EditorGUILayout.BeginHorizontal(GUILayout.Width(260));

                        EditorGUILayout.BeginVertical(menuBlockStyle,GUILayout.Width(78));
							
							EditorGUILayout.LabelField(liveTemplate.scaleRandMinUniform.ToString("0.00") + " to " + liveTemplate.scaleRandMaxUniform.ToString("0.00"),GUILayout.Width(78)); DoTipCheck("Random Scaling Range Split Slider (Min/Max)");
							EditorGUILayout.MinMaxSlider(ref liveTemplate.scaleRandMinUniform,ref liveTemplate.scaleRandMaxUniform,liveTemplate.scaleMin,liveTemplate.scaleMax); DoTipCheck("Random Scaling Range Split Slider (Min/Max)");
							
							liveTemplate.scaleRandMinUniform = (float)System.Math.Round(liveTemplate.scaleRandMinUniform,2);
							liveTemplate.scaleRandMaxUniform = (float)System.Math.Round(liveTemplate.scaleRandMaxUniform,2);			
							liveTemplate.scaleRandMinUniform = Mathf.Clamp(liveTemplate.scaleRandMinUniform,liveTemplate.scaleMin,liveTemplate.scaleMax);
							liveTemplate.scaleRandMaxUniform = Mathf.Clamp(liveTemplate.scaleRandMaxUniform,liveTemplate.scaleMin,liveTemplate.scaleMax);
					
						if(liveTemplate.scaleUniform)
						{	
							liveTemplate.scaleRandMin = new Vector3(liveTemplate.scaleRandMinUniform,liveTemplate.scaleRandMinUniform,liveTemplate.scaleRandMinUniform);
							liveTemplate.scaleRandMax = new Vector3(liveTemplate.scaleRandMaxUniform,liveTemplate.scaleRandMaxUniform,liveTemplate.scaleRandMaxUniform);
						}
					
						EditorGUILayout.EndVertical();
				
				EditorGUI.EndDisabledGroup();

                        EditorGUILayout.BeginVertical(menuBlockStyle, GUILayout.Width(170), GUILayout.MaxWidth(170));
							EditorGUILayout.BeginHorizontal();
								EditorGUILayout.LabelField("Min",GUILayout.Width(108)); DoTipCheck("Slider Minimum Value");
								liveTemplate.scaleMin = EditorGUILayout.FloatField(liveTemplate.scaleMin,floatFieldCompressedStyle); DoTipCheck("Slider Minimum Value");
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();
								EditorGUILayout.LabelField("Max",GUILayout.Width(108)); DoTipCheck("Slider Maximum Value");
								liveTemplate.scaleMax = EditorGUILayout.FloatField(liveTemplate.scaleMax,floatFieldCompressedStyle); DoTipCheck("Slider Maximum Value");
							EditorGUILayout.EndHorizontal();
			
							liveTemplate.scaleMin = (float)System.Math.Round(liveTemplate.scaleMin,2);
							liveTemplate.scaleMax = (float)System.Math.Round(liveTemplate.scaleMax,2);
						EditorGUILayout.EndVertical();
					
					EditorGUILayout.EndHorizontal();
		
	//-------------------------
				//EditorGUILayout.Space();
	//-------------------------
				
				EditorGUILayout.BeginHorizontal();

					liveTemplate.scaleUniform = !EditorGUILayout.Toggle(!liveTemplate.scaleUniform,toggleButtonStyle,GUILayout.Width(15)); DoTipCheck("Placed models are scaled separately on each axis");
					GUILayout.Label("Per Axis Scale"); DoTipCheck("Placed models are scaled separately on each axis");
				
				EditorGUILayout.EndHorizontal();
			
				EditorGUI.BeginDisabledGroup(liveTemplate.scaleUniform);
					
					EditorGUILayout.BeginHorizontal();
			
                        EditorGUILayout.BeginVertical(menuBlockStyle);
							liveTemplate.scaleRandMin.x = Mathf.Clamp(liveTemplate.scaleRandMin.x,liveTemplate.scaleMin,liveTemplate.scaleMax);
							EditorGUILayout.LabelField("X",GUILayout.Width(76));
							EditorGUILayout.LabelField(liveTemplate.scaleRandMin.x.ToString("0.00") + " to " + liveTemplate.scaleRandMax.x.ToString("0.00"),GUILayout.Width(76)); DoTipCheck("X Axis Random Scaling Range Split Slider (Min/Max)");
							EditorGUILayout.MinMaxSlider(ref liveTemplate.scaleRandMin.x,ref liveTemplate.scaleRandMax.x,liveTemplate.scaleMin,liveTemplate.scaleMax); DoTipCheck("X Axis Random Scaling Range Split Slider (Min/Max)");
							
							liveTemplate.scaleRandMin.x = (float)System.Math.Round(liveTemplate.scaleRandMin.x,2);
							liveTemplate.scaleRandMax.x = (float)System.Math.Round(liveTemplate.scaleRandMax.x,2);
							liveTemplate.scaleRandMin.x = Mathf.Clamp(liveTemplate.scaleRandMin.x,liveTemplate.scaleMin,liveTemplate.scaleMax);
							liveTemplate.scaleRandMax.x = Mathf.Clamp(liveTemplate.scaleRandMax.x,liveTemplate.scaleMin,liveTemplate.scaleMax);
						EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical(menuBlockStyle);
							liveTemplate.scaleRandMin.y = Mathf.Clamp(liveTemplate.scaleRandMin.y,liveTemplate.scaleMin,liveTemplate.scaleMax);
							EditorGUILayout.LabelField("Y",GUILayout.Width(76));
							EditorGUILayout.LabelField(liveTemplate.scaleRandMin.y.ToString("0.00") + " to " + liveTemplate.scaleRandMax.y.ToString("0.00"),GUILayout.Width(76)); DoTipCheck("Y Axis Random Scaling Range Split Slider (Min/Max)");
							EditorGUILayout.MinMaxSlider(ref liveTemplate.scaleRandMin.y,ref liveTemplate.scaleRandMax.y,liveTemplate.scaleMin,liveTemplate.scaleMax); DoTipCheck("Y Axis Random Scaling Range Split Slider (Min/Max)");
	
							liveTemplate.scaleRandMin.y = (float)System.Math.Round(liveTemplate.scaleRandMin.y,2);
							liveTemplate.scaleRandMax.y = (float)System.Math.Round(liveTemplate.scaleRandMax.y,2);
							liveTemplate.scaleRandMin.y = Mathf.Clamp(liveTemplate.scaleRandMin.y,liveTemplate.scaleMin,liveTemplate.scaleMax);
							liveTemplate.scaleRandMax.y = Mathf.Clamp(liveTemplate.scaleRandMax.y,liveTemplate.scaleMin,liveTemplate.scaleMax);
						EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical(menuBlockStyle);
							liveTemplate.scaleRandMin.z = Mathf.Clamp(liveTemplate.scaleRandMin.z,liveTemplate.scaleMin,liveTemplate.scaleMax);
							EditorGUILayout.LabelField("Z",GUILayout.Width(76));
							EditorGUILayout.LabelField(liveTemplate.scaleRandMin.z.ToString("0.00") + " to " + liveTemplate.scaleRandMax.z.ToString("0.00"),GUILayout.Width(76)); DoTipCheck("Z Axis Random Scaling Range Split Slider (Min/Max)");
							EditorGUILayout.MinMaxSlider(ref liveTemplate.scaleRandMin.z,ref liveTemplate.scaleRandMax.z,liveTemplate.scaleMin,liveTemplate.scaleMax); DoTipCheck("Z Axis Random Scaling Range Split Slider (Min/Max)");
	
							liveTemplate.scaleRandMin.z = (float)System.Math.Round(liveTemplate.scaleRandMin.z,2);
							liveTemplate.scaleRandMax.z = (float)System.Math.Round(liveTemplate.scaleRandMax.z,2);
							liveTemplate.scaleRandMin.z = Mathf.Clamp(liveTemplate.scaleRandMin.z,liveTemplate.scaleMin,liveTemplate.scaleMax);
							liveTemplate.scaleRandMax.z = Mathf.Clamp(liveTemplate.scaleRandMax.z,liveTemplate.scaleMin,liveTemplate.scaleMax);
						EditorGUILayout.EndVertical();

					EditorGUILayout.EndHorizontal();
					
				EditorGUI.EndDisabledGroup();
			
		if(EditorGUI.EndChangeCheck())
		{
			liveTemplate.dirty = true;
		}
		}
		#endregion
		
		EditorGUILayout.Space();
		
		#region Eraser Options
		eraserFoldout = EditorGUILayout.Foldout(eraserFoldout,"Eraser Settings:",EditorStyles.foldout); DoTipCheck("Settings for limiting the Eraser");
		if(eraserFoldout == true)
		{
		EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginVertical(menuBlockStyle,GUILayout.Width(260));

			liveTemplate.eraseByGroup =		EditorGUILayout.Toggle("Erase by Group", liveTemplate.eraseByGroup,EditorStyles.toggle);				DoTipCheck("Restrict Eraser to objects in currently selected group");
			liveTemplate.eraseBySelected =	EditorGUILayout.Toggle("Erase selected Prefab", liveTemplate.eraseBySelected,EditorStyles.toggle);		DoTipCheck("Restrict Eraser to checked prefab");
			
			EditorGUILayout.EndVertical();
			
		if(EditorGUI.EndChangeCheck())
		{
			liveTemplate.dirty = true;
		}
		}
		#endregion
	
		EditorGUILayout.EndVertical(); //Overall Vertical Container End
		EditorGUILayout.EndScrollView(); // Overall Scroll View End


		EditorGUILayout.Space();
	
		#region Templates
		
		EditorGUILayout.BeginVertical(GUILayout.Width(280));
			
			EditorGUILayout.BeginHorizontal(menuBlockStyle,GUILayout.Width(276),GUILayout.Height(22));
				picButtonStyle.hover.background =	saveIcon_hover;
				picButtonStyle.normal.background =	saveIcon;
			
				EditorGUILayout.LabelField("Name: ",GUILayout.Width(60));DoTipCheck("Name Template");
			
				EditorGUI.BeginChangeCheck();
				GUILayout.FlexibleSpace();

				liveTemplate.brushName = EditorGUILayout.TextField(liveTemplate.brushName); DoTipCheck("Name Template");
				
				if(EditorGUI.EndChangeCheck())
				{
					liveTemplate.dirty = true;
				}
		
				EditorGUILayout.BeginHorizontal(saveIconContainerStyle);
				
					EditorGUI.BeginDisabledGroup(liveTemplate.dirty == false);
						if(GUILayout.Button("",picButtonStyle,GUILayout.Width(16),GUILayout.Height(16)))
						{
							if(liveTemplate.brushName == string.Empty)
								EditorUtility.DisplayDialog("File Name Not Set","A template file must be named before it can be saved","Ok");
							
							else
							{
								SaveSettings(liveTemplate);
								liveTemplate.dirty = false;
							}
			
							clearSelection = true;
					
						}DoTipCheck("Save Template to File");
					EditorGUI.EndDisabledGroup();
				
				EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.EndHorizontal();
			
			//EditorGUILayout.Space();
			
		EditorGUI.EndDisabledGroup(); //Overall Disable End
	
			EditorGUILayout.BeginHorizontal(brushStripStyle,GUILayout.Width(276));
				
				for(int i = 0; i < 6; i++)
				{
					if(brushTemplates[i] != null)
					{
						if(brushTemplates[i].live == false)
							brushTemplates[i] = null;
					}
				
					EditorGUILayout.BeginVertical(brushSlotContainerStyle,GUILayout.Width(32)); //Begin Slot
					
						EditorGUILayout.BeginHorizontal();	//Begin Slot Operations Duo
				
								picButtonStyle.hover.background =	loadBrushIcon_hover;
								picButtonStyle.normal.background =	loadBrushIcon;
								if(GUILayout.Button("",picButtonStyle, GUILayout.Width(16), GUILayout.Height(16)))
								{	
									RenderTemplateMenu(Event.current, i);
								
								}DoTipCheck("Assign Template File to Slot" + (i + 1).ToString("00"));
								
							EditorGUI.BeginDisabledGroup(brushTemplates[i] == null);
				
								picButtonStyle.hover.background =	clearBrushIcon_hover;
								picButtonStyle.normal.background =	clearBrushIcon;
				
								if(GUILayout.Button("", picButtonStyle, GUILayout.Width(16), GUILayout.Height(16)))
								{
									ClearSlot(i);
									
									if(templateIndex == i)
									{
										ClearLiveTemplate();
										templateIndex = -1;
									}
					
								}DoTipCheck("Clear Slot " + (i + 1).ToString("00"));
					
							EditorGUI.EndDisabledGroup();
					
						EditorGUILayout.EndHorizontal();	//End Slot Operations Duo
				
				
						EditorGUI.BeginDisabledGroup(brushTemplates[i] == null);// || brushTemplates[i].live == false);//!brushStateArray[i]);
							
							if(templateIndex != i)
							{
								picButtonStyle.hover.background =	savedBrushIcon;
								picButtonStyle.normal.background =	savedBrushIcon;
							}
							else
							{
								picButtonStyle.hover.background =	savedBrushIcon_Active;
								picButtonStyle.normal.background =	savedBrushIcon_Active;
							}
				
							if(GUILayout.Button((i + 1).ToString("00"), picButtonStyle, GUILayout.Width(32), GUILayout.Height(32)))
							{
								SwitchToTemplate(i);
							}
				
							if(clearSelection == true)
							{
								clearSelection = false;
								EditorGUIUtility.hotControl = 0;
	  							EditorGUIUtility.keyboardControl = 0;
							}
					
						EditorGUI.EndDisabledGroup();
					
						//string slotName = brushTemplates[i] != null ? brushTemplates[i].brushName : "Empty";
						
						string slotName = brushTemplates[i] == null ? "Empty" : brushTemplates[i].brushName == string.Empty ? "Unnamed Template" :  brushTemplates[i].brushName ;
						DoTipCheck("Brush Slot " + (i + 1).ToString("00") + ": " + slotName);
				
					EditorGUILayout.EndVertical();	//End Slot
				}
				
			EditorGUILayout.EndHorizontal();
			
			if(GUILayout.Button("Restore Defaults"))
			{
				RestoreTemplateDefaults();
				
			}DoTipCheck("Restore Default Presets to the current template slot");
			
			string tipToDraw = drawCurTip ? curTip : string.Empty;
				
			EditorGUILayout.HelpBox(tipToDraw,MessageType.Info);
		
			EditorGUILayout.EndVertical();
			
		EditorGUILayout.EndVertical();//Master Vertical End
		#endregion
		
	Repaint();

	}
	
	static void DoTipCheck(string entry)
	{
		if(/*Event.current.type == EventType.Repaint &&*/ GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
		{
			window.curTip = entry;
			window.drawCurTip = true;
		}
	}
	
	private void RenderTemplateMenu(Event curEvent, int slotIndex)
	{
		UpdateTemplateSignatures();

		// Now create the menu, add items and show it
		GenericMenu menu = new GenericMenu ();

			menu.AddItem(new GUIContent("New Template"), false, TemplateMenuItemCallback, new KeyValuePair<int,int>(-1,slotIndex));
		
		for(int i = 0; i < templateSignatures.Length; i++)
		{
			menu.AddItem(new GUIContent(templateSignatures[i].name), false, TemplateMenuItemCallback, new KeyValuePair<int,int>(i,slotIndex));
		}
	
		menu.ShowAsContext ();

        curEvent.Use();
	}
	
	private void TemplateMenuItemCallback(object obj)
	{
		KeyValuePair<int,int> pair = (KeyValuePair<int,int>)obj;
		
		int fileIndex = pair.Key;
		int slotIndex = pair.Value;
		
		if(fileIndex == -1)
		{
			qb_Template newTemplate = new qb_Template();// ScriptableObject.CreateInstance<qb_Template>();
			brushTemplates[slotIndex] = newTemplate;
			newTemplate.live = true;
		}
		
		else
		{
			brushTemplates[slotIndex] = qb_Utility.LoadTemplate(templateSignatures[fileIndex].directory);
		}		
		
		
		SwitchToTemplate(slotIndex);
	}
	
	static private void DrawBrushGizmo( RaycastHit mouseRayHit)
	{
		if(placing == false)
		{
			if(mouseRayHit.collider != null)
			{
				Handles.color = Color.white;
				Handles.DrawWireDisc(mouseRayHit.point,mouseRayHit.normal,window.liveTemplate.brushRadius);
				Handles.color = Color.blue;
				Handles.DrawWireDisc(mouseRayHit.point,mouseRayHit.normal,window.liveTemplate.scatterRadius * window.liveTemplate.brushRadius);
			}
		}
		
		else
		{
			if(placingObject != null)
			{
				Handles.color = Color.green;
				Handles.DrawWireDisc(placingObject.transform.position,placingUpVector, Vector3.Distance(placingPlanePoint,placingObject.transform.position));
				Handles.DrawSolidDisc(placingPlanePoint,placingUpVector,0.1f);
				Handles.DrawPolyLine(new Vector3[2]{placingObject.transform.position,placingPlanePoint});
				Handles.ArrowCap(0,placingPlanePoint + ((placingPlanePoint - placingObject.transform.position).normalized * -0.5f),Quaternion.LookRotation(placingPlanePoint - placingObject.transform.position,placingUpVector),1f);
			}
		}
		
	}
	
	private enum BrushMode
	{
		Off,
		On
	}

	static bool placing;	//currently placing an object - ie have spawned object and not yet stopped modifying scale / rotation
	static bool painting; 	//in stroke

	public void OnSceneGUI(SceneView sceneView)
	{
		/* Rules pseudo code
		//if ALT not down
		//{
			//if mouse click when shift down -		begin PlaceMode
			//if drag while PlaceMode -				scale and rotate (calculate and execute scale and rotation on stored object)
			//if mouse up while placing -			end PlaceMode (if shift is still held down it will pop back on based on the stuff above)
			//if mouse right down while placing -	remove stored object from scene
			
			//if click when shift up -				begin StrokeMode
			//if drag while StrokeMode -			Paint stuff
			//if mouse up while StokeMode -			end StrokeMode
		//}
		*/
		
		bool altDown = false;
		bool shiftDown = false;
		bool ctrlDown = false;
//		bool xDown = false;

		if(Event.current.control)
		{
			ctrlDown = true;
			sceneView.Focus();
		}
		
		if(ctrlDown == false)
		{
			brushMode = BrushMode.Off;
			EndStroke();
			EndPlaceStroke();
			return;
		}
		
		else
			brushMode = BrushMode.On;
		
	//	if(xDown)
	//		brushDirection = !brushDirection;
		
		
		RaycastHit mouseRayHit = new RaycastHit();
		mouseRayHit = DoMouseRaycast();		
		
		DrawBrushGizmo(mouseRayHit);
		HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
		
		//Default Mode is Paint, from there we can 
		if(Event.current.alt)
		{
			//modify mode to Camera Navigation
			//if we are currently painting end stroke
			//if we are currently placing commit
			altDown = true;
		}
		
		if(Event.current.shift) //might want to force end 
		{
			//modify mode to Place
			shiftDown = true;
			placementModifier = true;
		}
		
		else 
			placementModifier = false;
		
		if(GetKeyUp == KeyCode.X)
		{
			brushDirection = !brushDirection;
		}
				
		if(altDown == false)
		{
			switch(Event.current.type)
			{
				
				case EventType.mouseUp:
					if(Event.current.button == 0)
					{
						if(brushMode == BrushMode.On)
						{
							if(painting == true)
							{
								EndStroke();
							}
					
							if(placing == true)
							{
								EndPlaceStroke();
							}
						}
					}
				break;
					
				case EventType.mouseDown:
					if(Event.current.button == 0)
					{	
						if(brushMode == BrushMode.On)
						{
							if(!shiftDown) 
							{
								BeginStroke();
							
								if(paintable)
								{
									Paint(mouseRayHit);
									Event.current.Use();
								}
								Tools.current = Tool.None;
							}
							
							else //shiftDown
							{
								if(paintable)
								{
									BeginPlaceStroke();
									Event.current.Use();
								}
								Tools.current = Tool.None;
							}
						}
					}
				break;
					
				case EventType.mouseDrag:
				if(Event.current.button == 0)
				{
					if(brushMode == BrushMode.On)
					{
						if(placing == true)
						{
							//if(paintable)
							//{
								UpdatePlace(sceneView);
								Event.current.Use();
								Tools.current = Tool.None; //make sure needed?
							//}
						}
					
						else if(painting == true)
						{
							if(paintable)
							{
								Paint(mouseRayHit);
								Event.current.Use();
								Tools.current = Tool.None; //make sure needed?
							}
						}
				
					}
				}
					HandleUtility.Repaint();
				break;
				
				case EventType.mouseMove:
					HandleUtility.Repaint();
				break;

			}
		}
		
		CalculateCursor(mouseRayHit);
		
		//This repaint is important to make lines and indicators not hang around for more frames
		sceneView.Repaint();

	}

	static void BeginStroke()
	{
		curStroke = new qb_Stroke();
		painting = true;
	}
	
	static void EndStroke()
	{
		curStroke = null;
		painting = false;
	}
	
	static void UpdateStroke()
	{
		//use the calculated stored cursor position to check distance from previous point on the stroke
		
		//if the calculated cursor position is at or beyond the BrushSpacingDistance from the last point in the stroke
		//add a point to the stroke 
		
		if(curStroke.GetCurPoint() == null) //there is no cur point, we are starting the stroke 
		{
			qb_Point nuPoint = curStroke.AddPoint(cursorPoint.position,cursorPoint.upVector,cursorPoint.dirVector);
			DoBrushIterration(nuPoint);
		}
		
		else
		{
			float distanceFromLastPt = Vector3.Distance(cursorPoint.position, curStroke.GetCurPoint().position);
			Vector3 strokeDirection = cursorPoint.position - curStroke.GetCurPoint().position;

			if(distanceFromLastPt >= window.liveTemplate.brushSpacing)
			{
				//Debug.DrawRay(cursorPoint.position,strokeDirection * strokeDirection.magnitude * -1f,Color.red);
				qb_Point newPoint = curStroke.AddPoint(cursorPoint.position,cursorPoint.upVector,strokeDirection.normalized);
				DoBrushIterration(newPoint);
			}
		}
	}
	
	static void DoBrushIterration(qb_Point newPoint) // do whatever needs to be done on the bruh itteration
	{		
		//if brush is positive
			//do a paint itteration
		if(brushDirection == true)
			PlaceGeo(newPoint);
			
		//if brush is negative
			//do an erase itteration
		else
			EraseGeo(newPoint);
			
		//later, we'll need another case for a vertex color brush, probably just an additional layer rather than exclusive
			
	}
	
	static GameObject placingObject;
	static Vector3 placingUpVector;
		
	static void BeginPlaceStroke()
	{
		curStroke = new qb_Stroke();
		qb_Point nuPoint = curStroke.AddPoint(cursorPoint.position,cursorPoint.upVector,cursorPoint.dirVector);//cursorPoint.dirVector);

		placingObject = PlaceObject(nuPoint);//PlaceGeo(nuPoint);
		
		if(placingObject != null)
		{
			placing = true;
			placingUpVector = placingObject.transform.up;
		}
	}
	
	static void EndPlaceStroke()
	{
		curStroke = null;
		
		//release from placing mode
		placing = false;
	}

	static Vector3 placingPlanePoint = Vector3.zero;
	
	static void UpdatePlace(SceneView sceneView)
	{
		if(placingObject != null)
		{	
			Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			Vector3 mouseWorldPoint = mouseRay.GetPoint(0f);

			placingPlanePoint = GetLinePlaneIntersectionPoint(mouseWorldPoint,mouseRay.direction,placingObject.transform.position,placingUpVector); //contact;//Vector3.Project(placingObjectRay.direction,placingUpVector);
			
			Vector3 difVector = placingPlanePoint - placingObject.transform.position;
			
			Bounds placingObjectBounds = TryGetObjectBounds(placingObject);
			
			float modifiedScale = 1f;
			
		//	if(placingObjectBounds == null)
		//		modifiedScale = (difVector.magnitude * 2f);
			
		//	else
		//		modifiedScale = (difVector.magnitude * 2f) / ((Mathf.Max(placingObject.renderer.bounds.extents.x , placingObject.renderer.bounds.extents.x) * 2) / placingObject.transform.localScale.x);
				
			modifiedScale = (difVector.magnitude * 2f) / /*((Mathf.Max(placingObjectBounds.extents.x , placingObjectBounds.extents.y) * 2)*/(placingObjectBounds.size.magnitude / placingObject.transform.localScale.x);
			
			placingObject.transform.rotation = Quaternion.LookRotation(difVector,placingUpVector);//this has to be rotation relative to the original placement rotation along its disk in the direction of the mouse pointer
			placingObject.transform.localScale = new Vector3(modifiedScale,modifiedScale,modifiedScale);//This has to be the distance between the screen cursor's position and the screen bound position of the object's placement point
		}
	}
	
	static Bounds TryGetObjectBounds(GameObject topObject)
	{
		//We need to itterate through the object's hierarchy and determine if it has any kind of object with bounds
		//if yes	return cumulative bounds
		//if no		return null
		
		//So, we itterate through the hierarchy to find any renderers or meshes to get bounds from
		//Then we combine all bounds to get a total
		Bounds combinedBounds = new Bounds(topObject.transform.position,new Vector3(1f,1f,1f));
		
		if(topObject.GetComponent(typeof(MeshRenderer)))
			combinedBounds = topObject.renderer.bounds;
		
	    Renderer[] renderers = topObject.GetComponentsInChildren<MeshRenderer>() as Renderer[];// as Renderer[];
	    
		foreach(Renderer render in renderers)
		{
	    	//if (render != renderer) 
				combinedBounds.Encapsulate(render.bounds);
	    }
		
		return combinedBounds;
	}
	
	static Vector3 GetLinePlaneIntersectionPoint(Vector3 rayOrigin, Vector3 rayDirection, Vector3 pointOnPlane, Vector3 planeNormal)
	{
		float epsilon = 0.0000001f;
		Vector3 contact = Vector3.zero;
		
		
		Vector3 ray =  (rayOrigin + (rayDirection * 1000f)) - rayOrigin; 
		//Vector3 ray = rayOrigin - (rayDirection * Mathf.Infinity); 
		
		Vector3 difVector = rayOrigin - pointOnPlane;
		
		float dot = Vector3.Dot(planeNormal,ray);
		
		if(Mathf.Abs(dot) > epsilon)
		{
			float fac = -Vector3.Dot(planeNormal,difVector) / dot;
			Vector3 fin = ray * fac;
			
			contact = rayOrigin + fin;
		}
	
		return contact;

	}
	
	static Vector3 GetFlattenedDirection(Vector3 worldVector, Vector3 flattenUpVector)
	{
		Vector3 flattened = Vector3.Cross(flattenUpVector, worldVector);
		Vector3 diskDirection = Vector3.Cross(flattened,flattenUpVector);
		
		return diskDirection;
	}
		
	static Object PickRandPrefab()
	{
		
		float totalWeight = 0f;
		for(int i = 0; i < window.liveTemplate.prefabGroup.Length ; i++)
		{
			totalWeight += window.liveTemplate.prefabGroup[i].weight;
		}
		
		float randomNumber = Random.Range(0f,totalWeight);
		
		float weightSum = 0f;
		int chosenIndex = 0;
		
		for(int x = 0; x < window.liveTemplate.prefabGroup.Length; x++)
		{
			weightSum += window.liveTemplate.prefabGroup[x].weight;

			if(randomNumber < weightSum)//prefabGroup[x].weight)
			{
				chosenIndex = x;
				break;
			}
		}
		
		return window.liveTemplate.prefabGroup[chosenIndex].prefab;

	}			
	
	static void Paint(RaycastHit mouseRayHit) //This function is called when the stroke reaches its next step - We feed it the hit from the latest Raycast
	{
		//were only here if the cursor is over a paintable object and the mouse button is pressed
		CalculateCursor(mouseRayHit);
		
		UpdateStroke(); 
	}
	
	static Object objectToSpawn;
	private static void PlaceGeo(qb_Point newPoint)
	{
	//-1 : if there are no prefabs in the queue. Do not paint
		if(window.liveTemplate.prefabGroup.Length == 0)
			return;

	//0	: declare function variables
		Vector3 spawnPosition = Vector3.zero;
		Quaternion spawnRotation = Quaternion.identity;
		//Vector3 spawnScale = new Vector3(1f,1f,1f);
		Vector3 upVector = Vector3.up;
		Vector3 placeUpVector = Vector3.up;
		Vector3 forwardVector = Vector3.forward; //blank filled - this value should never end up being used
			
	//1 : if there is more than one prefab in the queue, pick one using the randomizer
		if(window.liveTemplate.prefabGroup.Length > 0)
		{
			if(window.selectedPrefabIndex != -1)
			{
				if(window.liveTemplate.prefabGroup.Length > window.selectedPrefabIndex && window.liveTemplate.prefabGroup[window.selectedPrefabIndex] != null)
					objectToSpawn = window.liveTemplate.prefabGroup[window.selectedPrefabIndex].prefab;
			
				else
				{
					window.selectedPrefabIndex = -1;
					return;
				}
			}
			
			else
				objectToSpawn = PickRandPrefab();
		}
		
		else
			return;
		
	//2 : use the current point in the stroke to Get a random point around its upVector Axis
		Vector3 castPosition = GetRandomPointOnDisk(newPoint.upVector);//Vector3.zero;
		
	//3 : use the random disk point to cast down along the upVector of the stroke point
		Vector3 rayDir = -newPoint.upVector;
		//RaycastHit hit;
		
		qb_RaycastResult result = DoPlacementRaycast(castPosition + (rayDir * -0.02f), rayDir);
				
	//4 : if cast successful, get cast point and normal - if cast is unsuccessful, return...<---
		if(result.success == true)
		{
			spawnPosition = result.hit.point;
			
			if(window.liveTemplate.alignToNormal == true)
			{
				upVector = result.hit.normal;
				placeUpVector = upVector;
				
				if(window.liveTemplate.flipNormalAlign)
					placeUpVector *= -1f;
					
				forwardVector = GetFlattenedDirection(Vector3.forward,upVector);
			}
			
			forwardVector = GetFlattenedDirection(Vector3.forward,upVector);
			
			
			if(window.liveTemplate.alignToStroke == true)
			{
				forwardVector = GetFlattenedDirection(curStroke.GetCurPoint().dirVector,upVector);
				
				if(window.liveTemplate.flipStrokeAlign)
					forwardVector *= -1f;
			}
		}
		
		else
			return;
		
	//5 : instantiate the prefab
		GameObject newObject = null;

		newObject = PrefabUtility.InstantiatePrefab(objectToSpawn) as GameObject;
		qb_Object marker = newObject.AddComponent<qb_Object>();//.hideFlags = HideFlags.HideInInspector;
		marker.hideFlags = HideFlags.HideInInspector;
		Undo.RegisterCreatedObjectUndo(newObject,"QB Place Object");

	//6 : use settings to scale, rotate, and place the object
		spawnRotation = GetSpawnRotation(upVector,forwardVector);
		
		newObject.transform.position = spawnPosition;
		newObject.transform.rotation = spawnRotation;
		newObject.transform.position += (newObject.transform.right * window.liveTemplate.positionOffset.x) + (upVector.normalized * window.liveTemplate.positionOffset.y) + (newObject.transform.forward * window.liveTemplate.positionOffset.z);
		Vector3 randomScale;
		
		if(window.liveTemplate.scaleUniform == true)
		{	
			float randomScaleUni = Random.Range(window.liveTemplate.scaleRandMinUniform,window.liveTemplate.scaleRandMaxUniform);
			randomScale = new Vector3(randomScaleUni,randomScaleUni,randomScaleUni);
		}
				
		else
			randomScale = new Vector3(Random.Range(window.liveTemplate.scaleRandMin.x,window.liveTemplate.scaleRandMax.x),Random.Range(window.liveTemplate.scaleRandMin.y,window.liveTemplate.scaleRandMax.y),Random.Range(window.liveTemplate.scaleRandMin.z,window.liveTemplate.scaleRandMax.z));
		

	//7 : If we have a group, add the object to the group
		if(window.liveTemplate.groupObjects == true && curGroup != null)
		{
			curGroup.AddObject(newObject);
		}
		
	//8 : Scaling is applied after grouping to avoid float error
		newObject.transform.localScale = new Vector3(randomScale.x,randomScale.y,randomScale.z);//Random.Range(scaleMin.x,scaleMax.x),Random.Range(scaleMin.y,scaleMax.y),Random.Range(scaleMin.z,scaleMax.z));//spawnScale;

//		qb_ObjectContainer.GetInstance().AddObject(newObject);

	}
	
	private static GameObject PlaceObject(qb_Point newPoint)
	{
	//-1 : if there are no prefabs in the queue. Do not place
		if(window.liveTemplate.prefabGroup.Length == 0)
			return null;
			
		if(window.liveTemplate.prefabGroup[0] == null)
			return null;

	//0	: declare function variables
		Vector3 spawnPosition = Vector3.zero;
		Quaternion spawnRotation = Quaternion.identity;
		Vector3 upVector = Vector3.up;
		Vector3 placeUpVector = Vector3.up;
			//1 : if there is more than one prefab in the queue, pick one
		
		if(window.selectedPrefabIndex != -1)
		{
			if(window.liveTemplate.prefabGroup.Length > window.selectedPrefabIndex && window.liveTemplate.prefabGroup[window.selectedPrefabIndex] != null)
				objectToSpawn = window.liveTemplate.prefabGroup[window.selectedPrefabIndex].prefab;
		
			else
				window.selectedPrefabIndex = -1;
				
		}

		else
		{
			if(window.liveTemplate.prefabGroup.Length > 0 && window.liveTemplate.prefabGroup[0] != null)
				objectToSpawn = window.liveTemplate.prefabGroup[0].prefab;
			
			else
			{
				//window.selectedPrefabIndex = -1;
				return null;
			}
		}
		//else return null;
		
	//2 : use the current point in the stroke to Get a random point around its upVector Axis
		Vector3 castPosition = newPoint.position;
		
	//3 : use the random disk point to cast down along the upVector of the stroke point
		Vector3 rayDir = -newPoint.upVector;
		
		qb_RaycastResult result = DoPlacementRaycast(castPosition, rayDir);
				
	//4 : if cast successful, get cast point and normal - if cast is unsuccessful, return...<---
		if(result.success == true)
		{
			spawnPosition = result.hit.point;
			
			if(window.liveTemplate.alignToNormal == true)
			{
				upVector = result.hit.normal;
				placeUpVector = upVector;
				
				if(window.liveTemplate.flipNormalAlign)
					placeUpVector *= -1f;
			}

		}
		
		else
			return null;
		
	//5 : instantiate the prefab
		GameObject newObject = null;

		newObject = PrefabUtility.InstantiatePrefab(objectToSpawn) as GameObject;
		qb_Object marker = newObject.AddComponent<qb_Object>();//.hideFlags = HideFlags.HideInInspector;
		marker.hideFlags = HideFlags.HideInInspector;
		Undo.RegisterCreatedObjectUndo(newObject,"QB Place Object");

	//6 : use settings to scale, rotate, and place the object
		if(window.liveTemplate.alignToNormal)
		{
			spawnRotation = Quaternion.LookRotation(curStroke.GetCurPoint().dirVector,placeUpVector);
		}
		
		else
		{
			spawnRotation = Quaternion.LookRotation(Vector3.forward,Vector3.up);
		}
		
		newObject.transform.position = spawnPosition;
		newObject.transform.rotation = spawnRotation;
		newObject.transform.position += (newObject.transform.right * window.liveTemplate.positionOffset.x) + (upVector.normalized * window.liveTemplate.positionOffset.y) + (newObject.transform.forward * window.liveTemplate.positionOffset.z);
	//7 : If we have a group, add the object to the group
		if(window.liveTemplate.groupObjects == true && curGroup != null)
		{
			curGroup.AddObject(newObject);
		}
		
//		qb_ObjectContainer.GetInstance().AddObject(newObject);
		
		return newObject;
	}
	
	private static void EraseGeo(qb_Point newPoint)
	{
//		qb_ObjectContainer objectContainer = qb_ObjectContainer.GetInstance();
		
		GameObject[] objects = window.GetObjects();
		List<int> removalList = new List<int>();
		
		object curPrefab = null;
		bool eraseSelected = false;
		bool eraseGrouped = false;
		
		if(window.liveTemplate.eraseBySelected == true)
			if(window.selectedPrefabIndex != -1)
			{
				//AssetDatabase.GUIDToAssetPath(GUIDstring);
				curPrefab = window.liveTemplate.prefabGroup[window.selectedPrefabIndex].prefab;
				eraseSelected = true;
			}

		if(window.liveTemplate.eraseByGroup == true)
		{	
			if(curGroup != null)
			{
				eraseGrouped = true;
			}
		}
		
		bool addToList;
		for(int i = 0; i < objects.Length; i++)
		{
			addToList = true;
			
			if(Vector3.Distance(objects[i].transform.position, newPoint.position) < window.liveTemplate.brushRadius)
			{

				if(eraseSelected == true)
				{
					//if the current object's prefab is the curPrefab
					if(PrefabUtility.GetPrefabParent(objects[i]) != curPrefab)
						addToList = false;
					
				}
				//if group erase is on
				if(eraseGrouped == true)
					if(objects[i].transform.parent != curGroup.transform)
					{
						addToList = false;
					}
				if(addToList == true)
					removalList.Add(i);
			}
		}
		
		if(removalList.Count > 0)
			window.EraseObjects(removalList);
	}
	
	private static qb_RaycastResult DoPlacementRaycast(Vector3 castPosition,Vector3 rayDirection)
	{
		RaycastHit hit;
		bool success = false;
		
		Physics.Raycast(castPosition + (-0.1f * rayDirection),rayDirection,out hit,float.MaxValue);
		
		if(hit.collider != null)
		{
			success = true;
			
			if(window.liveTemplate.paintToLayer == true)
			{
				//if(hit.collider.gameObject.layer != window.layerIndex)
				if( (1 << hit.collider.gameObject.layer & window.liveTemplate.layerIndex) == 0)
					success = false;
			}
			
			if(window.liveTemplate.paintToSelection == true)
			{
				
				Transform[] selectedObjects = Selection.transforms;
				bool contains = ArrayUtility.Contains(selectedObjects,hit.collider.transform);
				
				if(!contains)
					success = false;
			}
		}
		
		qb_RaycastResult result = new qb_RaycastResult(success,hit);

		return result;
	}
	
	private static Vector3 GetRandomPointOnDisk(Vector3 upVector)
	{
		float angle = Random.Range(0f,1f) * Mathf.PI;
		Vector2 direction = new Vector2((float)Mathf.Cos(angle), (float)Mathf.Sin(angle));
		
		Vector3 direction3D = new Vector3(direction.x,0f,direction.y);
		Vector3 flattened = Vector3.Cross(upVector, direction3D);
		Vector3 diskDirection = Vector3.Cross(flattened,upVector);
		
		float distanceFromCenter = (window.liveTemplate.scatterRadius * window.liveTemplate.brushRadius)* Random.Range(0.0f,1.0f);
		Vector3 randomPoint = curStroke.GetCurPoint().position + (diskDirection.normalized * distanceFromCenter);

		return randomPoint;
	}
	
	//private static Quaternion GetPlacementSpawnRotation()
	//{
		
	//}
	
	private static Quaternion GetSpawnRotation(Vector3 upVector,Vector3 forwardVector)
	{	
		//based on rotation range about axis
		//Quaternion rotation = Quaternion.AngleAxis(baseRotationY,upVector);
		//Vector3 upVector = curStroke.GetCurPoint().upVector;
		Quaternion rotation = Quaternion.identity;
		Vector3 rotationOffset = Vector3.zero;

		if(upVector.magnitude != 0 && forwardVector.magnitude != 0)
		{
			if(upVector != Vector3.zero && forwardVector != Vector3.zero)
				rotation = Quaternion.LookRotation(forwardVector,upVector);
		}

		if(placing)
			return rotation;
			
			rotationOffset = new Vector3(Random.Range(window.liveTemplate.rotationRangeMin.x,window.liveTemplate.rotationRangeMax.x),Random.Range(window.liveTemplate.rotationRangeMin.y,window.liveTemplate.rotationRangeMax.y),Random.Range(window.liveTemplate.rotationRangeMin.z,window.liveTemplate.rotationRangeMax.z));
		
		rotation = rotation * Quaternion.Euler(rotationOffset);
		
		return rotation;
	}
	
	private static RaycastHit DoMouseRaycast() //Does a Raycast from mouse position and returns the hit - the main thread uses it to draw a handle and paint
	{
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		RaycastHit hit;
		
		if(window.liveTemplate.paintToLayer == true && window.liveTemplate.layerIndex != -1)
			Physics.Raycast(ray,out hit,float.MaxValue, window.layersMasked);//1 << window.layerIndex);
		
		else
			Physics.Raycast(ray,out hit,float.MaxValue);

				
		if(hit.collider != null)
		{	
			paintable = true;
			
			if(window.liveTemplate.paintToSelection == true)
			{
				Transform hitObject = hit.collider.transform;
				Transform[] selectedObjects = Selection.transforms;
				bool contains = ArrayUtility.Contains(selectedObjects,hitObject);
				
				if(!contains)
				{
					hit = new RaycastHit();
					paintable = false;
				}
			}
		}
		
		else
			paintable = false;
		
		return hit;
	}

	private static void CalculateCursor(RaycastHit mouseRayHit)
	{
		Vector3 upVector = Vector3.zero;
		Vector3 forwardVector = Vector3.zero;
		Vector3 positionVector = Vector3.zero;
		
		if(mouseRayHit.collider != null)
		{
			upVector = mouseRayHit.normal;
			positionVector = mouseRayHit.point;
			forwardVector = GetFlattenedDirection(Vector3.forward,upVector); //placement needs a direction to work with- we have no stroke direction yet, so we use flattened forward
		}

		cursorPoint.UpdatePoint(positionVector,upVector,forwardVector);
	}
	
	private static void CreateGroup(string groupName)
	{
		GameObject newGroupObject = new GameObject("QB_Group_" + groupName);
		curGroup = newGroupObject.AddComponent<qb_Group>();
		curGroup.groupName = groupName;
		groups.Add(curGroup);
		groupNames.Add(groupName);
	}
	
	private static void UpdateGroups() //updates the groups and groupNames arrays based on what is in the scene
	{
		qb_Group[] groupsInScene =  GameObject.FindObjectsOfType(typeof(qb_Group)) as qb_Group[];
		
		groups.Clear();
		groupNames.Clear();
		
		for(int i = 0; i < groupsInScene.Length; i++)
		{
			groups.Add(groupsInScene[i]);
			groupNames.Add(groupsInScene[i].groupName);
		}
		
	}
	
	private static bool GroupWithNameExists(string groupName)
	{
		qb_Group[] groupsInScene =  GameObject.FindObjectsOfType(typeof(qb_Group)) as qb_Group[];
		
		bool exists = false;
		
		for(int i = 0; i < groupsInScene.Length; i++)
		{
			if(groupsInScene[i].groupName == groupName)
				exists = true;
		}
		
		return exists;					
	}
				
	//This check is called whenever the tool needs to verify that the groups it has on record still exist in the scene
	//Rather than doing the full update groups loop, this is cheaper and is only concerned with groups that might have been deleted
	private static void CheckGroupsValid() 
	{
		List<int> removalList = new List<int>();
		
		for(int i = 0; i < groups.Count; i++)
		{
			if(groups[i] == null)
				removalList.Add(i);
		}
		
		for(int x = 0; x < removalList.Count; x++)
		{
			groups.RemoveAt(removalList[x]);
			window.Repaint();

		}
		
		if(groups.Count == 0)
		{
			window.liveTemplate.groupObjects = false;
			window.Repaint();
		}
		
	}

	static List<Object> PrefabDragBox(int width,Texture2D texture, string text)
	{
		List<Object> draggedObjects = new List<Object>();
		
		// Draw the controls

		window.prefabAddButtonStyle.normal.background = texture;

			GUILayout.Label(text,window.prefabAddButtonStyle,GUILayout.Width(width),GUILayout.MinWidth(width),GUILayout.Height(60),GUILayout.MinHeight(60));		
				 
		Rect lastRect = GUILayoutUtility.GetLastRect();

		
		// Handle events
		Event evt = Event.current;
		switch (evt.type)
		{
			case EventType.DragUpdated:
			// Test against rect from last repaint
			if (lastRect.Contains(evt.mousePosition))
			{
				// Change cursor and consume the event
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				evt.Use();
			}
			break;
			
			case EventType.DragPerform:
			// Test against rect from last repaint
			if (lastRect.Contains(evt.mousePosition))
			{
		
				foreach(Object draggedObject in DragAndDrop.objectReferences)
				{
					if(draggedObject.GetType() == typeof(GameObject))	//Debug.Log(draggedObject.GetType());
					{
						draggedObjects.Add(draggedObject);
						window.liveTemplate.dirty = true;
						window.clearSelection = true;
					}
				}
				// Change cursor and consume the event and drag
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				DragAndDrop.AcceptDrag();
				evt.Use();
			}
			break;
		}
		
		return draggedObjects;
	}

	void OnDestroy()
	{
		brushMode = BrushMode.Off;
		SceneView.onSceneGUIDelegate -= onSceneGUIFunc;
//		window = null;
	}
	
	public KeyCode GetKeyUp { get { return Event.current.type == EventType.KeyUp ? Event.current.keyCode : KeyCode.None; } }

	#region Temp Erasing
	public GameObject[] sceneObjects = new GameObject[0];
	public void EraseObjects(List<int> indexList)
	{	
		List<GameObject> removalList = new List<GameObject>();

		foreach(int index in indexList)
		{
			removalList.Add(sceneObjects[index]);
		}

		foreach(GameObject obj in removalList) 
		{
			ArrayUtility.Remove(ref sceneObjects,obj);
			EraseObject(obj);
		}		
	}
	
	public void EraseObject(GameObject obj)
	{
		
		#if UNITY_4_3
			Undo.DestroyObjectImmediate(obj);
		#else
			Undo.RegisterSceneUndo("Erased Object");
			//Undo.RegisterUndo(obj, "Erased Object");
			GameObject.DestroyImmediate(obj);
		#endif
		
	}

	public void VerifyObjects()
	{
		qb_Object[] objs = Object.FindObjectsOfType(typeof(qb_Object)) as qb_Object[];
		sceneObjects = new GameObject[objs.Length];

		for(int i = 0; i < sceneObjects.Length; i++)
		{
			sceneObjects[i] = objs[i].gameObject;
		}
	}
	
	public GameObject[] GetObjects()
	{
		VerifyObjects();
		return sceneObjects;
	}
	#endregion

////////SAVE-ABLE BRUSHES
	
	//Save curent window settings to a file 
	private void SaveSettings(qb_Template template)
	{
		if(template.brushName == string.Empty)
			return; //this should be a popup
		
		qb_Utility.SaveTemplate(template, directory);
		UpdateTemplateSignatures();
		AssetDatabase.Refresh();
	}

	private void ClearLiveTemplate()
	{			
		clearSelection = true;
		liveTemplate = new qb_Template();//ScriptableObject.CreateInstance<qb_Template>();		
	}
	
	private void RestoreTemplateDefaults()
	{
		string	slotName = string.Empty;
		bool 	slotLive = false;
		
		if(liveTemplate != null)
		{
			slotName = liveTemplate.brushName;
			slotLive = liveTemplate.live;
		}
		
		clearSelection = true;
		liveTemplate = new qb_Template();//ScriptableObject.CreateInstance<qb_Template>();//new qb_Template();
		liveTemplate.brushName = slotName;
		liveTemplate.live = slotLive;
		liveTemplate.dirty = true;
	}
	
	//Load settings into the window - from the file associated with the currently selected slot
	private void SwitchToTemplate(int slotIndex)
	{
		if(brushTemplates[slotIndex] == null)
			return;
		
		clearSelection = true;
		templateIndex = slotIndex;
		liveTemplate = brushTemplates[slotIndex];
	}
	
	private void UpdateTemplateSignatures()
	{
		templateSignatures = qb_Utility.GetTemplateFileSignatures(directory);
	}

	static void ClearSlots()
	{
		for(int i = 0; i < 6; i++)
		{
			ClearSlot(i);
		}
	}

	static void ClearSlot(int slotIndex)
	{
		window.brushTemplates[slotIndex] = null;
	}
	
	//Save the correlation (by name) of which templates are in which slots
	private void SaveSlotAssignments()
	{
		string prefix = "qb_SlotAssignment_";
		
		for(int i = 0; i < 6; i++)
		{
			if(brushTemplates[i] != null && brushTemplates[i].brushName != string.Empty)
				EditorPrefs.SetString(prefix + i.ToString(),brushTemplates[i].brushName);			
		}
	}
	
	//Try to load the templates from the previous session into the slots in which they were previously located
	private void LoadSlotAssignments()
	{
		string prefix = "qb_SlotAssignment_";
		string[] names = new string[6];
		
		for(int i = 0; i < 6; i++)
		{
			if(brushTemplates[i] != null && brushTemplates[i].brushName != string.Empty)
			{
				names[i] = EditorPrefs.GetString(prefix + i.ToString(),string.Empty);
			}
		}
		
		for(int i = 0; i < 6; i++)
		{
			if(names[i] != null)
				brushTemplates[i] = qb_Utility.LoadTemplate(directory + names[i]);
		}
		
		
	}

	
	[SerializeField] private GUIStyle prefabAmountSliderStyle;
	[SerializeField] private GUIStyle prefabAmountSliderThumbStyle;
	[SerializeField] private GUIStyle toggleButtonStyle;
	[SerializeField] private GUIStyle prefabPreviewWindowStyle;
	[SerializeField] private GUIStyle prefabSelectCheckStyle;
	[SerializeField] private GUIStyle prefabRemoveXStyle;
	[SerializeField] private GUIStyle prefabFieldStyle;
	[SerializeField] private GUIStyle floatFieldCompressedStyle;
	[SerializeField] private GUIStyle prefabAddButtonStyle;
    [SerializeField] private GUIStyle picLabelStyle;
	[SerializeField] private GUIStyle menuBlockStyle;
	[SerializeField] private GUIStyle masterVerticalStyle;
	[SerializeField] private GUIStyle tipLabelStyle;
	[SerializeField] private GUIStyle picButtonStyle;
	[SerializeField] private GUIStyle brushSlotContainerStyle;
	[SerializeField] private GUIStyle brushStripStyle;
	
	[SerializeField] private GUIStyle shortToggleStyle;
	[SerializeField] private GUIStyle saveIconContainerStyle;

	
    private void SetStyleParameters()
	{
		window.masterVerticalStyle.margin.left = 0;
		window.masterVerticalStyle.margin.right = 0;
		window.masterVerticalStyle.padding.left = 0;
		window.masterVerticalStyle.padding.left = 0;
		
		window.prefabAmountSliderStyle.margin.top = 4;

		window.prefabAmountSliderThumbStyle.fixedHeight = 10;
		
		window.prefabPreviewWindowStyle.margin.top = 4;
		
		window.prefabRemoveXStyle.normal.background = removePrefabXTexture_normal;
		window.prefabRemoveXStyle.hover.background = removePrefabXTexture_hover;
		
		window.prefabSelectCheckStyle.normal.background = selectPrefabCheckTexture_normal;
		window.prefabSelectCheckStyle.hover.background = selectPrefabCheckTexture_hover;
		
		window.prefabAddButtonStyle.margin.top = 0;
        window.prefabAddButtonStyle.margin.bottom = 0;
        window.prefabAddButtonStyle.margin.left = 4;
        window.prefabAddButtonStyle.margin.right = 0;
        window.prefabAddButtonStyle.fixedHeight = 60;
		window.prefabAddButtonStyle.normal.background = addPrefabTexture;
		
		window.prefabFieldStyle.padding.left = 1;
		window.prefabFieldStyle.padding.right = 1;
        window.prefabFieldStyle.margin.top = 0;
		window.prefabFieldStyle.margin.left = 7;
		window.prefabFieldStyle.margin.right = 2;

        window.prefabFieldStyle.fixedHeight = 60;
        window.prefabFieldStyle.fixedWidth = 72;
		window.prefabFieldStyle.normal.background = prefabFieldBackgroundTexture;
		
		window.floatFieldCompressedStyle.fixedWidth = 50;
		window.floatFieldCompressedStyle.stretchWidth = false;
		
        window.picLabelStyle.padding.left = 0;
        window.picLabelStyle.padding.top = 0;
        window.picLabelStyle.padding.bottom = 0;
        window.picLabelStyle.padding.right = 0;
        window.picLabelStyle.margin.left = 4;
		
        window.picButtonStyle.padding.left = 0;
        window.picButtonStyle.padding.top = 4;
        window.picButtonStyle.padding.bottom = 0;
        window.picButtonStyle.padding.right = 0;
        window.picButtonStyle.margin.bottom = 0;
        window.picButtonStyle.margin.top = 0;
		window.picButtonStyle.margin.left = 0;
		window.picButtonStyle.margin.right = 0;
		window.picButtonStyle.alignment = TextAnchor.UpperCenter;
		window.picButtonStyle.fontStyle = FontStyle.Bold;
		window.picButtonStyle.hover.textColor = Color.black;
		window.picButtonStyle.normal.textColor = Color.black;
		
		window.brushSlotContainerStyle.padding.left = 0;
		window.brushSlotContainerStyle.padding.right = 0;
		window.brushSlotContainerStyle.padding.top = 0;
		window.brushSlotContainerStyle.padding.bottom = 0;
		window.brushSlotContainerStyle.margin.left = 10;//12;
		window.brushSlotContainerStyle.margin.right = 12;//12;
		
		window.saveIconContainerStyle.padding.left = 0;
		window.saveIconContainerStyle.padding.right = 0;
		window.saveIconContainerStyle.padding.top = 0;
		window.saveIconContainerStyle.padding.bottom = 0;
		window.saveIconContainerStyle.margin.left = 2;//12;
		window.saveIconContainerStyle.margin.right = 2;//12;
		window.saveIconContainerStyle.margin.top = 3;
		window.saveIconContainerStyle.margin.bottom = 3;//12;

		
		window.menuBlockStyle.margin.right = 0;
		
		window.brushStripStyle.margin.right = 0;
		
		window.tipLabelStyle.fontSize = 9;
		window.tipLabelStyle.padding.top = 0;
		

		window.shortToggleStyle.overflow = new RectOffset(80,0,-3,0);
	}

    private void BuildStyles()
    {
		window.masterVerticalStyle = new GUIStyle(EditorStyles.label);
        window.prefabAmountSliderStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).verticalSlider);
        window.prefabAmountSliderThumbStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).verticalSliderThumb);
        window.toggleButtonStyle = new GUIStyle(EditorStyles.radioButton); //new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).toggle);//
        window.floatFieldCompressedStyle = new GUIStyle(EditorStyles.textField); //new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).textField);//
        window.prefabPreviewWindowStyle = new GUIStyle(EditorStyles.label); //new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).button);
        window.prefabRemoveXStyle = new GUIStyle(EditorStyles.label); //new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).label);
        window.prefabSelectCheckStyle = new GUIStyle(EditorStyles.label); //new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).label);
        window.prefabAddButtonStyle = new GUIStyle(EditorStyles.miniButton); //new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).button);
        window.prefabFieldStyle = new GUIStyle(EditorStyles.label); //new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).textField);
        window.picLabelStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).label);//new GUIStyle(EditorStyles.miniButton); //new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).button);
        window.picButtonStyle = new GUIStyle(EditorStyles.label);
		window.menuBlockStyle = new GUIStyle(EditorStyles.textField);//new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).textArea);//new GUIStyle(EditorStyles.textField); //new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).textField);
		window.tipLabelStyle = new GUIStyle(EditorStyles.label);
		window.brushSlotContainerStyle = new GUIStyle(EditorStyles.label);
		window.saveIconContainerStyle = new GUIStyle(EditorStyles.label);
		window.brushStripStyle = new GUIStyle(EditorStyles.label);
		window.shortToggleStyle = new GUIStyle(EditorStyles.toggle);
        SetStyleParameters();
    }
	
	static string[] GetLayerList()
	{
		string[] layerArray = new string[32];
		
		for(int i = 0; i < 32; i++)
		{
			string name = LayerMask.LayerToName(i);
			if(name == string.Empty)
				name = "    ";//"undefined";
			
			layerArray[i] = name;
		}
		
		return layerArray;
	}

}
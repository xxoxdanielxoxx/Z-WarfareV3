using UnityEngine;
using UnityEditor;

/// <summary>
/// Navmesh Creation Assisstant
/// Made for the Josh in all of us.
/// </summary>
public class NCA : EditorWindow 
{
	int m_walkLayer 	= 0;
	int m_climbLayer 	= 0;
	int m_noPassLayer 	= 0;

	[MenuItem("Z Warfare/Navmesh Creation Assistant")]
	public static void OpenWindow()
	{
		EditorWindow.GetWindow(typeof(NCA));
	}

	void OnGUI()
	{
		// Set the layers that we will automatically feed into the Navmesh
		GUILayout.Label("Navmesh Layers", EditorStyles.boldLabel);
		m_walkLayer = EditorGUILayout.LayerField("Walkable", m_walkLayer);
		m_climbLayer = EditorGUILayout.LayerField("Climbable", m_climbLayer);
		m_noPassLayer = EditorGUILayout.LayerField("Obstacles", m_noPassLayer);
		GUILayout.Space (15);

		// Confirm and cancel button
		if (GUI.Button(new Rect(position.width/4, position.height-23, position.width/4, 20 ), "Process Enviroment"))
		{
			// Go through every object in the scene and see what Navmesh layer should I add it to
			object[] objectArray = GameObject.FindObjectsOfType(typeof (GameObject));
			foreach (object obj in objectArray)
			{
				GameObject gameObj = (GameObject) obj;

				if (gameObj.layer == m_walkLayer)
				{
					GameObjectUtility.SetStaticEditorFlags(gameObj, StaticEditorFlags.NavigationStatic);
					GameObjectUtility.SetNavMeshLayer(gameObj, 0);
				}
				else if (gameObj.layer == m_climbLayer)
				{
					GameObjectUtility.SetStaticEditorFlags(gameObj, StaticEditorFlags.NavigationStatic);
					GameObjectUtility.SetNavMeshLayer(gameObj, 2);
				}
				else if (gameObj.layer == m_noPassLayer)
				{
					GameObjectUtility.SetStaticEditorFlags(gameObj, StaticEditorFlags.NavigationStatic);
					GameObjectUtility.SetNavMeshLayer(gameObj, 1);
				}
				else // In none of the targetted layers. If it has a collider than make it non walkable
				{
					if (gameObj.collider != null)
					{
						GameObjectUtility.SetStaticEditorFlags(gameObj, StaticEditorFlags.NavigationStatic);
						GameObjectUtility.SetNavMeshLayer(gameObj, 1);
					}
					if (gameObj.transform.parent != null)
					{
						if (gameObj.transform.parent.collider != null)
						{
							GameObjectUtility.SetStaticEditorFlags(gameObj, StaticEditorFlags.NavigationStatic);
							GameObjectUtility.SetNavMeshLayer(gameObj, 1);
						}
					}
				}
			}
			
			Debug.Log("Object processing completed. Use the Navigation window to finalize Navmesh parameters and to build the Navmesh.");
		}
		if (GUI.Button(new Rect(position.width/2, position.height-23, position.width/4, 20 ), "Cancel"))
		{
			Close();
		}
		if (GUI.Button(new Rect(position.width-position.width/4, position.height-23, position.width/4, 20 ), "Clean Navmesh"))
		{
			object[] objectArray = GameObject.FindObjectsOfType(typeof (GameObject));
			foreach (object obj in objectArray)
			{
				GameObject gameObj = (GameObject) obj;
				if (GameObjectUtility.AreStaticEditorFlagsSet(gameObj, StaticEditorFlags.NavigationStatic))
				{
					GameObjectUtility.SetStaticEditorFlags(gameObj, GameObjectUtility.GetStaticEditorFlags(gameObj) ^ StaticEditorFlags.NavigationStatic);
				}
			}

			Debug.Log("Cleaning completed. None of the objects in the scene is marked as a Navigation Static Object");
		}
	}
}

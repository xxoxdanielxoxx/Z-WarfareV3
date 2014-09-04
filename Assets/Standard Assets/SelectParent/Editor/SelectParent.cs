/*! \mainpage
\author Stig Olavsen <stig.olavsen@randomrnd.com>
\author http://www.RandomRnD.com
\date © 2011-May-01
\version 1.0.2011.05.01
 
\section About
This is a small editor extension for Unity, which adds an item in the
Edit menu called "Select Parent". This item is mapped to the hotkey <em>P</em>,
and lets you easily select the parent object of your currently selected
object in the Unity editor. You can also select the root of the tree
with <em>Shift-P</em>. 
 
After seeing a presentation from the London Unity User Group meeting
(http://blogs.unity3d.com/2011/05/03/london-unity-user-group-luug-meet-v1-0/) 
by Quickfingers (http://www.quickfingers.net/) I also decided to add
a component you can attach to your gameobjects that will automatically
select their parent, to avoid accidentaly selecting for example the mesh
when you really want to select the parent gameobject.

To use this, simply add the component found under <em>Component --> Select Parent -->
Always Select My Parent</em>, or just add the AlwaysSelectMyParent.cs script to
your object. Since you sometimes will want to select these objects anyway,
toggle the functionality from the <em>Edit --> Always Select Parent</em> menu. You can
also toggle it on and off using the hotkey <em>Alt+P</em>.

The component is an empty class, and so should not affect performance of
your final build.

\section License
  Copyright (C) 2011 Stig Olavsen

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
     
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
     
  3. This notice may not be removed or altered from any source distribution.

  Stig Olavsen <stig.olavsen@randomrnd.com>

 */
 

/*! 
 * \file
 * \author Stig Olavsen <stig.olavsen@randomrnd.com>
 * \date © 2011-May-01
 * \brief Function definition for Select Parent menu item
 * \details Adds a menu item called "Select Parent" in the Edit menu
 * activated by the P key. This lets you select the parent object of
 * your current selection with a single keypress.
 */

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


#if UNITY_EDITOR
/// <summary>
/// Editor class for enabling the Select Parent function
/// </summary>
public class SelectParent : Editor
{
	/// <summary>
	/// Boolean to hold the current status for if we should
	/// select the parent of all AlwaysSelectMyParent objects.
	/// </summary>
	private static bool alwaysSelectParent = false;
	
	/// <summary>
	/// Function for selecting the parent transform
	/// </summary>
	[MenuItem("Edit/Select Parent _p", false, 200001)]
	private static void SelectParentMenuitem()
	{
		if (Selection.activeTransform != null && 
		    Selection.activeTransform.parent != null)
		{
			Selection.activeTransform = Selection.activeTransform.parent;
		}
	}
	
	/// <summary>
	/// Function for selecting the root transform
	/// </summary>
	[MenuItem("Edit/Select Root #p", false, 200002)]
	private static void SelectRootMenuItem()
	{
		if (Selection.activeTransform != null &&
		    Selection.activeTransform.root != null)
		{
			Selection.activeTransform = Selection.activeTransform.root;
		}
	}	
	
	/// <summary>
	/// Function for turning Always Select Parent On
	/// </summary>
	[MenuItem("Edit/Always Select Parent/On", false, 200003)]
	private static void EnableSelectParent()
	{
		if (!alwaysSelectParent)
		{
			EditorApplication.update += SelectParents;
		}
		alwaysSelectParent = true;
	}

	/// <summary>
	/// Validation Function for turning Always Select Parent On
	/// </summary>
	[MenuItem("Edit/Always Select Parent/On", true, 200003)]
	private static bool EnableSelectParentValidate()
	{
		return (!alwaysSelectParent);
	}
	
	/// <summary>
	/// Function for turning Always Select Parent Off
	/// </summary>
	[MenuItem("Edit/Always Select Parent/Off", false, 200004)]
	private static void DisableSelectParent()
	{
		if (alwaysSelectParent)
		{
			EditorApplication.update -= SelectParents;
		}
		alwaysSelectParent = false;
	}

	/// <summary>
	/// Validation Function for turning Always Select Parent Off
	/// </summary>
	[MenuItem("Edit/Always Select Parent/Off", true, 200004)]
	private static bool DisableSelectParentValidate()
	{
		return alwaysSelectParent;
	}
	
	/// <summary>
	/// Function to toggle the state of Always Select Parent (On/Off)
	/// </summary>
	[MenuItem("Edit/Always Select Parent/Toggle &p", false, 200005)]
	private static void ToggleSelectParent()
	{
		if (!alwaysSelectParent)
		{
			EnableSelectParent();
		}
		else
		{
			DisableSelectParent();
		}
	}
	
	/// <summary>
	/// Function that is added to the editors update callback.
	/// This will select the parent of the current transform if
	/// it has the AlwaysSelectMyParent component added.
	/// </summary>
	private static void SelectParents()
	{
		Transform t = Selection.activeTransform;
		if (t != null)
		{
			if (t.GetComponent<AlwaysSelectMyParent>() != null)
			{
				if (t.parent != null)
				{
					Selection.activeTransform = t.parent;
				}
			}
		}
	}
}
#endif
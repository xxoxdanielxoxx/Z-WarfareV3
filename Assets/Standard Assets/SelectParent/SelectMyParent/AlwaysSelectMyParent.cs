
/*! 
 * \file
 * \author Stig Olavsen <stig.olavsen@randomrnd.com>
 * \date © 2011-May-01
 * \brief Always Select Parent class definition
 * \details Empty component that is used for the Always Select Parent
 * functionality.
 */

using System;
using System.Collections;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
/// <summary>
/// This is an empty class, only used  to determine if
/// the parent should be selected.
/// </summary>
[AddComponentMenu("Select Parent/Always Select My Parent")]
public class AlwaysSelectMyParent : MonoBehaviour
{

}
#endif
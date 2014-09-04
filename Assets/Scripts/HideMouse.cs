using UnityEngine;
using System.Collections;

public class HideMouse : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.F2))
		{
			if (Screen.showCursor)
			{
				Debug.Log ("Cursor locked and hidden. Press F2 to reenable.");
				Screen.lockCursor = true;
				Screen.showCursor = false;
			}
			else
			{
				Debug.Log ("Cursor locked and hidden. Press F2 to reenable.");
				Screen.lockCursor = false;
				Screen.showCursor = true;
			}
				
		}
		
	}
}

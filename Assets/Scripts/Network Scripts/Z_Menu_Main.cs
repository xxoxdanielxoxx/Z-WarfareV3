using UnityEngine;
using System.Collections;

public class Z_Menu_Main : MonoBehaviour 
{
	private bool m_bShowMenu = false;
	private Rect m_MyWindowRect;
	
	void Awake()
	{
		m_MyWindowRect = new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 200);
	}
	/// <summary>
	/// Enables the Main menu buttons
	/// </summary>
	public void EnableMenu()
	{
		m_bShowMenu = true;
	}

	void OnGUI()
	{
		//print ("Gooey!!!" + m_bShowMenu);
		if (!m_bShowMenu)
		{
			return;
		}
		
		m_MyWindowRect = GUILayout.Window(0, m_MyWindowRect, windowGUI, "Z Warfare");
	}

	void windowGUI(int id)
	{
		
		GUILayout.Space(10);
		
		if (!PhotonNetwork.connected)
		{
			GUILayout.Label("Not connected");
			if (GUI.Button(new Rect(50, 60, 200, 20), "Retry"))
			{
				PhotonNetwork.ConnectUsingSettings("1.0");
			}
		}
		else
		{
			if (GUI.Button(new Rect(50, 60, 200, 20), "Create a game"))
			{
				m_bShowMenu = false;
				Z_Menu_Manager.GetSinglton().OpenMenu(MenuPanals.PREGAMELOBBY);
			}

			if (GUI.Button(new Rect(50, 90, 200, 20), "Select a game to join"))
			{
				m_bShowMenu = false;
				Z_Menu_Manager.GetSinglton().OpenMenu(MenuPanals.SELECTIONLOBBY);
			}
			//Space Break
			if (GUI.Button(new Rect(50, 150, 200, 20), "Credits"))
			{
				Debug.Log(" Under Construction");
			}
			if (GUI.Button(new Rect(50, 180, 200, 20), "Options"))
			{
				Debug.Log(" Under Construction");
			}
		}
	}
}

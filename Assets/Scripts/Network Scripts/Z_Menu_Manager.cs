using UnityEngine;
using System.Collections;

public enum MenuPanals{ NAME, MAIN, SELECTIONLOBBY, PREGAMELOBBY, OPTIONS, CREDITS};

public class Z_Menu_Manager : MonoBehaviour 
{
	/// <summary>
	/// Read ONLY in the Editor to see what panal you are Currently In 
	/// </summary>
	public MenuPanals m_eMenuPanal;
	
	//refrence to other Menus
	private Z_Menu_SelectionLobby m_SelectionLobbyScript;
	private Z_Menu_PreGameLobby   m_PreGameLobbyScript;
	private Z_Menu_Main           m_MainMenuScript;

	private static Z_Menu_Manager SP;
	private bool m_bRequirePlayerName = false;
	private string m_sPlayerNameInput = "";
	
	// Use this for initialization
	void Awake () 
	{
		SP = this;
        m_sPlayerNameInput = PlayerPrefs.GetString ("Player Name" + Application.platform, "");
		m_bRequirePlayerName = true;

		//Get Scripts attached to the GO
		m_SelectionLobbyScript = GetComponent<Z_Menu_SelectionLobby> ();
		m_MainMenuScript       = GetComponent<Z_Menu_Main> ();
		m_PreGameLobbyScript   = GetComponent<Z_Menu_PreGameLobby> ();

		OpenMenu (MenuPanals.MAIN);
		PhotonNetwork.ConnectUsingSettings("1.0");
	}

	void OnGUI()
	{
		if (m_bRequirePlayerName)
		{
			GUILayout.Window(9, new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 100), NameMenu, "Please enter a name:");
		}
	}

	public void OpenMenu(MenuPanals newMenu)
	{
		m_eMenuPanal = newMenu;

		if (m_bRequirePlayerName)
		{
			return;
		}
				
		if (newMenu == MenuPanals.PREGAMELOBBY)
		{
			m_PreGameLobbyScript.EnableLobby();
		}
		else if (newMenu == MenuPanals.SELECTIONLOBBY)
		{
			m_SelectionLobbyScript.EnableMenu();
		}
		else if (newMenu == MenuPanals.MAIN)
		{
			m_MainMenuScript.EnableMenu();
		}
	}
	void NameMenu(int id)
	{
		GUILayout.BeginVertical();
		GUILayout.Space(10);
		
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(10);
		GUILayout.Label("Please enter your name");
		GUILayout.Space(10);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(10);
		m_sPlayerNameInput = GUILayout.TextField(m_sPlayerNameInput);
		GUILayout.Space(10);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(10);

		if (m_sPlayerNameInput.Length >= 1)
		{

			if (GUILayout.Button("Save"))
			{
				m_bRequirePlayerName = false;
				PlayerPrefs.SetString("playerName" + Application.platform, m_sPlayerNameInput);
				PhotonNetwork.playerName = m_sPlayerNameInput;
				OpenMenu(MenuPanals.MAIN);
			}
		}
		else
		{
			GUILayout.Label("Enter a name to continue...");
		}
		GUILayout.Space(10);
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
	}

	public static Z_Menu_Manager GetSinglton()
	{
		return SP;
	}
}

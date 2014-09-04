using UnityEngine;
using System.Collections;

public class Z_Menu_SelectionLobby : MonoBehaviour 
{
	public GUISkin m_Skin;
	private Rect m_WindowRect1;
	private Rect m_WindowRect2;
	
	private string m_sErrorMessage = "";
	public GUIStyle m_CustomButton;
	
	private bool m_bShowMenu;
	
	private Vector2 m_vJoinScrollPosition;
	private string m_sJoinRoomName = "";

	private string     m_sRoomPassword = "";
	private bool       m_bPasswordWindow = false;
	private RoomInfo   m_lockedRoom;

	void Awake()
	{
		m_WindowRect1 = new Rect(Screen.width / 2 - 305, Screen.height / 2 - 140, 400, 280);
		m_WindowRect2 = new Rect (Screen.width / 2 - 100, Screen.height / 2 - 30, 200, 80);

		ResetPasswordMembers ();
	}
	
	/// <summary>
	/// Enables the Selection Menu
	/// </summary>
	public void EnableMenu()
	{
		m_bShowMenu = true;
	}
	
	void OnJoinedRoom()
	{
		m_bShowMenu = false;
		Z_Menu_PreGameLobby.GetSingleton ().EnableLobby ();
	}
	
	void OnGUI()
	{
		// Only SHow the Menu if m_bShowMenu  is True
		if (!m_bShowMenu)
		{
			return;
		}
		
		//Back to main menu Button
		if (GUI.Button(new Rect(40, 10, 150, 20), "Back to main menu"))
		{
			m_bShowMenu = false;
			Z_Menu_Manager.GetSinglton().OpenMenu(MenuPanals.MAIN);
			ResetPasswordMembers();
		}
		
		//Print out Erros
		if (m_sErrorMessage != null && m_sErrorMessage != "")
		{
			GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 30, 200, 60), "Error");
			GUI.Label(new Rect(Screen.width / 2 - 90, Screen.height / 2 - 15, 180, 50), m_sErrorMessage);
			if (GUI.Button(new Rect(Screen.width / 2 + 40, Screen.height / 2 + 5, 50, 20), "Close"))
			{
				m_sErrorMessage = "";
			}
		}
		
		//Hide windows on error
		if (m_sErrorMessage == "")
		{ 
			if(m_bPasswordWindow)
				m_WindowRect2 = GUILayout.Window(0, m_WindowRect2, PasswordWindow, "Enter the Room's Password");
			else
			{
				m_WindowRect1 = GUILayout.Window(1, m_WindowRect1, listGUI, "Join a game via the list");
			}
		}
		
	}
	void listGUI(int id)
	{
		GUILayout.BeginVertical();
		GUILayout.Space(6);
		GUILayout.EndVertical();
		
		//Masterlist
		GUILayout.Label("Game list:");

		GUILayout.Space(2);
		GUILayout.BeginHorizontal();
		GUILayout.Space(24);
		
		GUILayout.Label("Host", GUILayout.Width(200));
		GUILayout.Label("Players", GUILayout.Width(55));
		GUILayout.Label("Ping", GUILayout.Width(30));
		GUILayout.Label ("Password", GUILayout.Width (60));
		GUILayout.EndHorizontal();
		
		
		m_vJoinScrollPosition = GUILayout.BeginScrollView(m_vJoinScrollPosition);
		foreach (RoomInfo room in PhotonNetwork.GetRoomList())
		{
			if (!room.open) continue;


			//Need to Check RACE CONDITIONS
			GUILayout.BeginHorizontal();
			if (  (room.playerCount<room.maxPlayers || room.maxPlayers == 0) &&
			    GUILayout.Button("" + room.name, GUILayout.Width(200)))
			{
				//Password Check
				if(room.customProperties.Count > 0)
				{
					m_lockedRoom = room;
					m_bPasswordWindow = true;
				}
				else
				{
					PhotonNetwork.JoinRoom(room.name);    
				}
			}
			GUILayout.Label(room.playerCount + "/" + room.maxPlayers, GUILayout.Width(45));

			GUILayout.Label(PhotonNetwork.GetPing()+ "" , GUILayout.Width(30));

			if(room.customProperties.Count > 0)
			{
				GUILayout.Label("Yes");
			}
			else
			{
				GUILayout.Label("No");
			}

			GUILayout.EndHorizontal();
		}
		if (PhotonNetwork.GetRoomList().Length == 0)
		{
			GUILayout.Label("No games running right now, you should host one.");
		}
		GUILayout.EndScrollView();
		
		GUILayout.Label( PhotonNetwork.GetRoomList().Length + " total servers" );

		//DIRECT JOIN
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Join by name:");
		GUILayout.Space(5);
		GUILayout.Label("Host name");
		m_sJoinRoomName = (GUILayout.TextField(m_sJoinRoomName + "", GUILayout.Width(50)) + "");
		
		if (GUILayout.Button("Connect"))
		{
			foreach (RoomInfo room in PhotonNetwork.GetRoomList())
			{
				if (!room.open) continue;
			
				if(m_sJoinRoomName == room.name)
				{
					if(room.customProperties.Count > 0)
					{
						m_lockedRoom = room;
						m_bPasswordWindow = true;
					}
					else
					{
						PhotonNetwork.JoinRoom(m_sJoinRoomName);    
					}
				}
			}
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(4);
	}

	void PasswordWindow(int id)
	{
		GUILayout.BeginVertical();
		GUILayout.Space(10);
		GUI.FocusWindow(0);
		m_sRoomPassword = (GUILayout.TextField(m_sRoomPassword + "", GUILayout.Width(180)) + "");

		GUILayout.EndVertical();

		GUILayout.Space(2);
		GUILayout.BeginHorizontal();
		GUILayout.Space(10);

		if (GUILayout.Button ("Cancel"))
		{
			ResetPasswordMembers();
		}
		GUILayout.Space(50);

		if (GUILayout.Button ("Connect" )) 
		{
			if(m_lockedRoom.customProperties.ContainsKey(m_sRoomPassword))
				PhotonNetwork.JoinRoom(m_lockedRoom.name); 

			ResetPasswordMembers();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	void ResetPasswordMembers()
	{
		m_bPasswordWindow = false;
		m_sRoomPassword = "";
	}
}

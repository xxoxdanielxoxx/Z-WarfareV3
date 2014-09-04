using UnityEngine;
using System.Collections;

public class Z_Menu_PreGameLobby : Photon.MonoBehaviour 
{
	static Z_Menu_PreGameLobby SP;
	
	bool m_bLaunchingGame = false;
	bool m_bShowMenu = false;
	
	private const int c_iServerMaxPlayers = 4;
	private string    m_sServerTitle = "Loading..";
	
	//Host Settings
	public string m_sGAMESCENE;
	
	private string m_sHostSetting_title = "No server title";
	private int    m_iHostSetting_players = 4;

	private bool   m_bPasswordProtected = false;
	private string m_sPassword = "";

	void Awake () 
	{
		m_bShowMenu = false;
		SP = this;
	}
	/// <summary>
	/// Enables the lobby. Shows GUI Menu
	/// </summary>
	public void EnableLobby()
	{
		m_bLaunchingGame = false;
		m_bShowMenu = true;
	}
	
	void OnGUI()
	{
		//Only Show the Menu if its true
		if (!m_bShowMenu)
			return;
		
		//Back to main menu
		if (GUI.Button(new Rect(40, 10, 150, 20), "Back to main menu"))
		{
			LeaveLobby();
		}
		if (m_bLaunchingGame)
		{
			LaunchingGameGUI();
		}
		//First set player count, server name and password option	
		else if (!PhotonNetwork.isMasterClient && !PhotonNetwork.isNonMasterClientInRoom)
		{
			CreationSettings();
		}
		else
		{
			//Show the lobby		
			ShowLobby();
		}
	}
	
	/// <summary>
	/// Leaves the lobby. Disconnect from host, or shutduwn host Resets Values
	/// </summary>
	void LeaveLobby()
	{
		PhotonNetwork.LeaveRoom ();
		
		Z_Menu_Manager.GetSinglton ().OpenMenu (MenuPanals.MAIN);	
		m_bShowMenu = false;

		m_sPassword = "";
		m_bPasswordProtected = false;
	}
	

	void CreationSettings()
	{
		GUI.BeginGroup(new Rect(Screen.width / 2 - 175, Screen.height / 2 - 75 - 50, 350, 150));
		GUI.Box(new Rect(0, 0, 350, 150), "Host options");
		
		GUI.Label(new Rect(10, 20, 150, 20), PhotonNetwork.player.name + " 's Z-Lobby");
		m_sHostSetting_title =  PhotonNetwork.player.name;   //GUI.TextField(new Rect(175, 20, 160, 20), m_sHostSetting_title);
		
		GUI.Label(new Rect(10, 40, 150, 20), "Max. players (2-4)");
		m_iHostSetting_players = int.Parse(GUI.TextField(new Rect(175, 40, 160, 20), m_iHostSetting_players + ""));
		
		m_bPasswordProtected =  GUI.Toggle(new Rect(10, 60, 200, 20), m_bPasswordProtected,"Optional: Password Locked");

		//Password Stuff
		GUI.enabled = m_bPasswordProtected;
		GUI.Label(new Rect(10, 80, 150, 20),"Password");
		m_sPassword = GUI.TextField (new Rect (175, 80, 160, 20), m_sPassword + "" );
		GUI.enabled = true;


		if (GUI.Button(new Rect(100, 115, 150, 20), "Go to lobby"))
		{
			StartHost(m_iHostSetting_players, m_sHostSetting_title, m_sPassword);
		}
		GUI.EndGroup();
	}

	//AWARE will Get a Random Server Name if one already exist: FIX Needs To check servername first if it exist then creates
	void StartHost(int players, string serverName, string password)
	{
		m_sServerTitle = serverName;

		RoomOptions options = new RoomOptions();

		options.maxPlayers = Mathf.Clamp(players, 1, 4); 
		options.isOpen = true;
		options.isVisible = true;

		//if password exists set custom properties
		if (password.Length > 0) 
		{

			options.customRoomProperties = new ExitGames.Client.Photon.Hashtable () { { password, 1 } }; // C0 might be 0 or 1
			options.customRoomPropertiesForLobby = new string[] { password }; // this makes "C0" available in the lobby
		}

		TypedLobby sqlLobby = new TypedLobby ();
		PhotonNetwork.CreateRoom (serverName, options, sqlLobby);

	}

	void ShowLobby()
	{
		string players = "";
		int currentPlayerCount = 0;
		foreach (PhotonPlayer playerInstance in PhotonNetwork.playerList)
		{
			players = playerInstance.name + "\n" + players;
			currentPlayerCount++;
		}
		
		GUI.BeginGroup(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 200, 400, 180));
		GUI.Box(new Rect(0, 0, 400, 200), "Game lobby");

		GUI.Label(new Rect(10, 40, 150, 20), "Host");
		GUI.Label(new Rect(150, 40, 100, 100), m_sServerTitle);
		
		GUI.Label(new Rect(10, 60, 150, 20), "Players");
		GUI.Label(new Rect(150, 60, 100, 100), currentPlayerCount + "/" + c_iServerMaxPlayers);
		
		GUI.Label(new Rect(10, 80, 150, 20), "Current players");
		GUI.Label(new Rect(150, 80, 100, 100), players);
		
		if (PhotonNetwork.isMasterClient)
		{
			if (GUI.Button(new Rect(25, 140, 150, 20), "Start the game"))
			{
				HostLaunchGame();
			}
		}
		else
		{
			GUI.Label(new Rect(25, 140, 200, 40), "Waiting for the server to start the game..");
		}
		
		GUI.EndGroup();
	}
	
	//Called on masterclient
	void OnCreatedRoom()
	{
		photonView.RPC("SetServerSettings", PhotonTargets.OthersBuffered, m_sHostSetting_title);
	}

	void OnMasterClientSwitched(PhotonPlayer newMaster)
	{
		Debug.Log("The old masterclient left, we have a new masterclient: " + newMaster); 
		if (PhotonNetwork.player == newMaster)
		{
			photonView.RPC("SetServerSettings", PhotonTargets.OthersBuffered, m_sServerTitle);
		}       
	}

	[RPC]
	void SetServerSettings(string newSrverTitle)
	{
		m_sServerTitle = newSrverTitle;
	}

	void HostLaunchGame()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		//Close the room for future connections
		PhotonNetwork.room.open = false;
		PhotonNetwork.room.visible = false;

		photonView.RPC("LaunchGame", PhotonTargets.All);
	}

	[RPC]
	void LaunchGame()
	{
		m_bLaunchingGame = true;
	}

	void LaunchingGameGUI()
	{
		if(m_sGAMESCENE == null)
		{
			Debug.Log("Go in the inspector and Type in the Next Scene, Make Sure its in the Build settings and Spell Correctly");	
		}
		
		//Show loading progress, ADD LOADINGSCREEN?
		GUI.Box(new Rect(Screen.width / 4 + 180, Screen.height / 2 - 30, 280, 50), "");
		if (Application.CanStreamedLevelBeLoaded(m_sGAMESCENE))
		{
			GUI.Label(new Rect(Screen.width / 4 + 200, Screen.height / 2 - 25, 285, 150), "Loaded, starting the game!");
			PhotonNetwork.LoadLevel((m_sGAMESCENE));
		}
		else
		{
			GUI.Label(new Rect(Screen.width / 4 + 200, Screen.height / 2 - 25, 285, 150), "Starting..Loading the game: " + (Mathf.Floor(Application.GetStreamProgressForLevel(m_sGAMESCENE)) * 100) + " %");
		}
	}

	public static Z_Menu_PreGameLobby GetSingleton()
	{
		return SP;
	}
	
}

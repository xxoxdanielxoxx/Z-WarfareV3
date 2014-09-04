using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChatEntry
{
	public string name = "";
	public string text = "";
}

public class Z_Chat : Photon.MonoBehaviour 
{
	public static Z_Chat SP;
	public static bool m_bUsingChat = false;	//Can be used to determine if we need to stop player movement since we're chatting
	
	public bool m_bShowChat = false;			//Show/Hide the chat
	
	public const int c_iMAXCHATENTRIES = 4;

	//Private vars used by the script
	private string m_sInputField = "";
	
	private Vector2 m_vScrollPosition;
	private int m_iWidth = 500;
	private int m_iHeight = 185;
	private Rect m_rWindow;
	private float m_fLastUnfocus = 0;



	private List<ChatEntry> m_lChatEntries = new List<ChatEntry>();

	void Awake()
	{
		m_rWindow = new Rect(Screen.width / 2 - m_iWidth / 2, Screen.height - m_iHeight + 5, m_iWidth, m_iHeight);
		SP = this;
		
		//We get the name from the masterserver example, if you entered your name there ;).
		string playerName = PlayerPrefs.GetString("playerName"+Application.platform, "");
		if (playerName == null || playerName == "")
		{
			playerName = "RandomName" + Random.Range(1, 999);
		}
		PhotonNetwork.playerName = playerName;
	}

	//Client function
	void OnJoinedRoom()
	{
		SetShowChatWindow(true);
		AddGameChatMessage(PhotonNetwork.player.name + " joined the chat", true);
	}
	
	void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient)
		{
			AddGameChatMessage("Player disconnected: " + player, true);
		}
	}

	void OnDisconnectedFromPhoton()
	{
		SetShowChatWindow(false);
	}

	public void SetShowChatWindow(bool show)
	{
		m_bShowChat = show;
		m_sInputField = "";
		m_lChatEntries = new List<ChatEntry>();
	}

	void OnGUI()
	{
		if (!m_bShowChat || PhotonNetwork.room == null)
		{
			return;
		}

		if (Event.current.type == EventType.keyDown && Event.current.character == '\n')//inputField.Length <= 0)
		{
			if (m_fLastUnfocus + 0.25f < Time.realtimeSinceStartup)
			{
				if (!m_bUsingChat)
					StartCoroutine(StartUsingChat());
				else
					StopUsingChat();
			}
		}

		if (m_bUsingChat && Input.GetKey(KeyCode.Escape))
		{
			StopUsingChat();
		}
		
		m_rWindow = GUI.Window(5, m_rWindow, GlobalChatWindow, "");
	}

	IEnumerator StartUsingChat()
	{
		m_bUsingChat = true;
		yield return 0;
	}

	void StopUsingChat()
	{
		m_sInputField = "";
		GUI.UnfocusWindow();//Deselect chat
		m_fLastUnfocus = Time.realtimeSinceStartup;
		m_bUsingChat = false;
	}
	
	void GlobalChatWindow(int id)
	{
		
		GUILayout.BeginVertical();
		GUILayout.Space(10);
		GUILayout.EndVertical();
		
		// Begin a scroll view. All rects are calculated automatically - 
		// it will use up any available screen space and make sure contents flow correctly.
		// This is kept small with the last two parameters to force scrollbars to appear.
		m_vScrollPosition = GUILayout.BeginScrollView(m_vScrollPosition);
		
		foreach (ChatEntry entry in m_lChatEntries)
		{
			GUILayout.BeginHorizontal();
			if ((entry as ChatEntry).name == "")
			{//Game message
				GUILayout.Label((entry as ChatEntry).text);
			}
			else
			{
				GUILayout.Label((entry as ChatEntry).name + ": " + (entry as ChatEntry).text);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(3);
			
		}
		// End the scrollview we began above.
		GUILayout.EndScrollView();
		
		
		if (Event.current.type == EventType.keyDown && Event.current.character == '\n' && m_sInputField.Length > 0)
		{
			HitEnter(m_sInputField);
		}
		
		if (m_bUsingChat)
		{
			GUI.SetNextControlName("Chat input field");
			m_sInputField = GUILayout.TextField(m_sInputField, GUILayout.Height(24));
			
			if (GUI.GetNameOfFocusedControl() != "Chat input field")
			{
				//GUI.FocusWindow(1);
				GUI.FocusControl("Chat input field");
			}
		}
		else
		{
			GUILayout.Space(23 + 5);
		}
		//If we DO have typing focus, make sure Chat.usingchat is set!
		if (GUI.changed && !m_bUsingChat)
		{
			StartCoroutine(StartUsingChat());
		}
		
		//GUI.DragWindow ();
		
		if (Input.GetKeyDown("mouse 0"))
		{
			if (m_bUsingChat)
			{
				StopUsingChat();
			}
		}
	}

	void HitEnter(string msg)
	{
		msg = msg.Replace("\n", "");
		photonView.RPC("ApplyGlobalChatText", PhotonTargets.All, msg, false);
		m_sInputField = ""; //Clear line
	}

	[RPC]
	public void ApplyGlobalChatText(string msg, bool systemMessage, PhotonMessageInfo info)
	{
		ChatEntry entry = new ChatEntry();
		if(!systemMessage) entry.name = info.sender.name;
		entry.text = msg;
		
		m_lChatEntries.Add(entry);
		
		//Remove old entries
		if (m_lChatEntries.Count > c_iMAXCHATENTRIES)
		{
			m_lChatEntries.RemoveAt(0);
		}
		
		m_vScrollPosition.y = 1000000;
	}

	//Add game messages etc
	public void AddGameChatMessage(string str, bool systemMessage)
	{
		photonView.RPC("ApplyGlobalChatText", PhotonTargets.All, str, systemMessage);
	}
}

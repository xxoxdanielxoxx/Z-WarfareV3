using UnityEngine;
using System.Collections;

public class Z_GameManager : Photon.MonoBehaviour 
{
	bool m_bOnlinePlay;

	static Z_GameManager s_GameManager;

	//Network Initialization
	public Z_Netowork_PlayerSpawner m_NetPlayerSpawner;
	public Z_NetworkAuthoritativePlayerSpawner m_ZNet_AuthPlayerSpawner;

	PhotonView  m_PhotonView;


	// Set To Defualt to true, just in case we are in offline mode
	bool m_bIsHost = true;

	public int m_iNumOfPlayers = 0;

	void Awake()
	{
		Initialize ();
	}

	void Initialize()
	{
		m_NetPlayerSpawner = GetComponent<Z_Netowork_PlayerSpawner> ();
		m_ZNet_AuthPlayerSpawner = GetComponent<Z_NetworkAuthoritativePlayerSpawner> ();

		m_PhotonView = GetComponent<PhotonView> ();
	

		//Online Check
		if (m_PhotonView == null)
			OnlineModeOff();	
		else
			OnlineModeOn();

		//Do the Ready Check and Assign the Host
		if(IsOnline())
		{
			m_bIsHost = PhotonNetwork.isMasterClient;


			if(m_bIsHost)
			{
				ReadyCheck();
			}
			else
			{
				m_PhotonView.RPC("ReadyCheck",PhotonTargets.MasterClient);	
			}

			//Start Spawning Players
			m_ZNet_AuthPlayerSpawner.Initialize(m_bIsHost);
		
		}
		else
		{
			//Activate Non Network Player Spawner
		}
	}


	[RPC]
	void ReadyCheck()
	{
		m_iNumOfPlayers++;

		//WORK in progress

//
//		if (m_iNumOfPlayers == PhotonNetwork.countOfPlayers) 
//		{
//			photonView.RPC ("SpawnLocalPlayer", PhotonTargets.OthersBuffered);
//			SpawnLocalPlayer();
//			
//			AIMaster m = (AIMaster)FindObjectOfType(typeof(AIMaster));
//			StartCoroutine( m.StartAI());
//		}
	} 

	public bool IsHost()
	{
		return m_bIsHost;
	}

	public void OnlineModeOff()
	{
		m_bOnlinePlay = false;
	}

	public void OnlineModeOn()
	{
		m_bOnlinePlay = true;
	}

	public bool IsOnline()
	{
		return m_bOnlinePlay; 
	}



	/// <summary>
	/// Get this Single Static instance.
	/// </summary>
	public static Z_GameManager Get()
	{
		return s_GameManager;
	}
}

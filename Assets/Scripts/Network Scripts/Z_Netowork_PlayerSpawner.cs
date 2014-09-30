//Authortative Toggle
//#define auth  
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Z_Netowork_PlayerSpawner : Photon.MonoBehaviour 
{
	class PlayerInfo4
	{
		public PhotonPlayer networkPlayer;        
		public Transform transform;
		
		public bool IsLocal()
		{
			return networkPlayer.isLocal;
		}
	}
	
	public static Z_Netowork_PlayerSpawner SP;
	
	public Transform playerPrefab;
	
	private List<PlayerInfo4> playerList = new List<PlayerInfo4>();
	private PlayerInfo4 localPlayerInfo;

	private int m_iCountOfplayers = 0;
	
	void Awake()
	{
		SP = this;
		m_iCountOfplayers = 0;
		
		if (PhotonNetwork.isMasterClient) 
			ReadyCheck ();
		 
		else 
		{
			photonView.RPC("ReadyCheck",PhotonTargets.MasterClient);	
		}


	}

	//////////////////////////////
	// Manage players
	
	[RPC]
	void AddPlayer(PhotonPlayer networkPlayer)
	{
		Debug.Log("Player Loaded " + networkPlayer );
		if (GetPlayer(networkPlayer) != null)
		{
			Debug.LogError("AddPlayer: Player already exists!");
			return;
		}
		
		PlayerInfo4 pla = new PlayerInfo4();
		pla.networkPlayer = networkPlayer;
		playerList.Add(pla);
		
		if (pla.IsLocal())
		{
			if (localPlayerInfo != null) { Debug.LogError("localPlayerInfo already set?"); }
			localPlayerInfo = pla;
		}
	}
	
	
	void RemovePlayer(PhotonPlayer networkPlayer)
	{
		Debug.Log("RemovePlayer " + networkPlayer);
		PlayerInfo4 thePlayer = GetPlayer(networkPlayer);
		
		if (thePlayer.transform)
		{
			Destroy(thePlayer.transform.gameObject);
		}
		playerList.Remove(thePlayer);
	}
	
	
	PlayerInfo4 GetPlayer(PhotonPlayer networkPlayer)
	{
		foreach (PlayerInfo4 pla in playerList)
		{
			if (pla.networkPlayer == networkPlayer)
			{
				return pla;
			}
		}
		return null;
	}
	
	////////////////////////////
	// STARTUP: Spawn own player, Message is sent from the host to all the clients, inlcuding himself
	[RPC ]
	void SpawnLocalPlayer()
	{

		//Get random spawnpoint //FIX THIS WHERE PLAYERS GET THEROE OWN SPAWN
		GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("playerSpawn");
		GameObject theGO = spawnPoints[Random.Range(0, spawnPoints.Length)];
		Vector3 pos = theGO.transform.position;
		Quaternion rot = theGO.transform.rotation;
		
		//Manually allocate PhotonViewID
		int id1 = PhotonNetwork.AllocateViewID();
		
		photonView.RPC("AddPlayer", PhotonTargets.OthersBuffered, PhotonNetwork.player);
		AddPlayer (PhotonNetwork.player);

		//Spawns myself Remote to others
		photonView.RPC("SpawnOnNetwork", PhotonTargets.OthersBuffered, pos, rot, id1, PhotonNetwork.player);

		//Spawns  myself Locallaly
		SpawnOnNetwork (pos, rot, id1, PhotonNetwork.player);
	}
	
	[RPC]
	void SpawnOnNetwork(Vector3 pos, Quaternion rot, int id1, PhotonPlayer np)
	{
		Transform newPlayer = Instantiate(playerPrefab, pos, rot) as Transform;
		//Set transform
		PlayerInfo4 pNode = GetPlayer(np);
		pNode.transform = newPlayer;

		//Set photonview ID everywhere!
		newPlayer.GetComponent<PhotonView> ().viewID = id1;
		
		if (pNode.IsLocal())
			localPlayerInfo = pNode;
	}

	//On all clients: When a remote client disconnects, cleanup
	void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		PlayerInfo4 pNode = GetPlayer(player);
		if (pNode != null)
		{
			//string playerNameLeft= pNode.name;
			//I.e.: Chat.SP.addGameChatMessage(playerNameLeft+" left the game");
		}
		RemovePlayer(player);
	}

	[RPC]
    void ReadyCheck()
	{
		m_iCountOfplayers++;

		if (m_iCountOfplayers == PhotonNetwork.countOfPlayers) 
		{
			photonView.RPC ("SpawnLocalPlayer", PhotonTargets.OthersBuffered);
			SpawnLocalPlayer();

			AIMaster m = (AIMaster)FindObjectOfType(typeof(AIMaster));
			StartCoroutine( m.StartAI());
		}
	}
}

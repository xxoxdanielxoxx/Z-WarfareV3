using UnityEngine;
using System.Collections;

public class Z_Network_ZombieSpawner : Photon.MonoBehaviour 
{
	//Set in the inspector
	public Transform m_RegZombiePrefab;

	static Z_Network_ZombieSpawner s_ZS;

	void Awake()
	{
		s_ZS = this;
	}

	public static Z_Network_ZombieSpawner Get()
	{
		return s_ZS;
	}

	
	//For Host To Call
	public void SummonRemoteZombies()
	{
		ZombieAI[] zombies = AIMaster.m_Reference.m_Zombs;
		if (zombies == null) 
		{
			Debug.Log("Error AI MASTER's Zombies are NULL");
			return ;		
		}

		if (photonView == null) {
			Debug.Log("Offline / Or Photon View is not atached");
				return;
		}


		for(int i = 0; i < zombies.Length; ++ i)
		{
			//Manually allocate PhotonViewID
			int id1 = PhotonNetwork.AllocateViewID();
			zombies[i].gameObject.GetPhotonView().viewID = id1;
			//Debug.Log( " Zombie ID = " + id1);

			//Get The POS And ROT
			Vector3 pos 	= zombies[i].transform.position;
			Quaternion rot 	= zombies[i].transform.rotation;


			//ERROR, doesnt work on the EXE need to Log to txt countof players
//			Debug.Log("Player Count = " + PhotonNetwork.countOfPlayers);
//			if (PhotonNetwork.countOfPlayers <= 1) 
//			{
//				Debug.Log("Single Player ");
//				return; 
//			}

			//Transmit the Clients the ID, and have them Spawn the Zombies
			//NOTE ADD TYPE
			photonView.RPC("SpawnRemoteZombiesNetwork", PhotonTargets.OthersBuffered, pos, rot, id1);
		}
	}

	[RPC]
	public void SpawnRemoteZombiesNetwork(Vector3 pos, Quaternion rot, int id1)
	{
		Debug.Log("Clients call to spawn Zombies, ID = " + id1);

		Transform remoteZombie = Instantiate(m_RegZombiePrefab, pos, rot) as Transform;

		remoteZombie.GetComponent<PhotonView>().viewID = id1 ;
	}


}

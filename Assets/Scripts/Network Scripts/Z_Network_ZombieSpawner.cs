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

		if (PhotonNetwork.isNonMasterClientInRoom) 
		{
			AIMaster m = (AIMaster)FindObjectOfType(typeof(AIMaster));
			m.enabled = false;
		} 
	}

	public static Z_Network_ZombieSpawner Get()
	{
		return s_ZS;
	}

	
	//For Host To Call
	public void SummonRemoteZombies()
	{
		AIMaster m = (AIMaster)FindObjectOfType(typeof(AIMaster));

		ZombieAI[] zombies = m.m_Zombs;
		if (zombies == null) 
		{
			Debug.Log("Error AI MASTER's Zombies are NULL");
			return ;		
		}


		for(int i = 0; i < zombies.Length; ++ i)
		{
			//Manually allocate PhotonViewID
			int id1 = PhotonNetwork.AllocateViewID();

			Debug.Log( " Zombie ID = " + id1);
			//Set ID


			zombies[i].gameObject.GetPhotonView().viewID = id1;

			//id1 = zombies[i].gameObject.GetPhotonView().viewID; 


			//Get The POS And ROT
			Vector3 pos 	= zombies[i].transform.position;
			Quaternion rot 	= zombies[i].transform.rotation;


			if (PhotonNetwork.countOfPlayers <= 1) 
			{
				Debug.Log("Single Player ");
				return; 
			}

			//Transmit the Clients the ID, and have them Spawn the Zombies
			//NOTE ADD TYPE
			photonView.RPC("SpawnRemoteZombiesNetwork", PhotonTargets.OthersBuffered, pos, rot, id1);
		}
	}

	[RPC]
	void SpawnRemoteZombiesNetwork(Vector3 pos, Quaternion rot, int id1)
	{
		Transform remoteZombie = Instantiate(m_RegZombiePrefab, pos, rot) as Transform;

		//Set photonview ID everywhere!
		SetPhotonViewIDs(remoteZombie.gameObject, id1);
	
	}

	//When a PhotonView instantiates it has viewID=0 and is unusable.
	//We need to assign the right viewID -on all players(!)- for it to work
	void SetPhotonViewIDs(GameObject go, int id1)
	{
		PhotonView[] nViews = go.GetComponentsInChildren<PhotonView>();
		nViews[0].viewID = id1;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// This script is intended to act as a bridge that ensures all indivual AI scripts can access data from each other;
/// it will also run, create, and manage all the scripts dedicated to AI.
/// 
/// Simply attach this script to any gameObject in the scene and it will take care of the rest.
/// </summary>

public class AIMaster : Photon.MonoBehaviour 
{
	/// <summary>
	/// IF it is Checked You are in DEBUG MODE and its NOT NETWORKED/// </summary>
	public bool m_bOFFLINEMODE = true;
	private bool m_bIsActive = false; 
	// Varibles I should get from GameManager later
	public List<GameObject> players;
	public int wave;

	// Reference to all the zombie prefabs
	public GameObject	m_GroupRef;		// Group Object 
	public GameObject	m_ZombieStd;	// Standard zombie
	public GameObject	m_ZombieRun;	// Runner zombie
	public GameObject	m_ZombieCom;	// Commander zombie
	public GameObject	m_ZombieTnk;	// Tank zombie

	// Spawner varibles
	public int 	 m_iSpawnLimitMax; 	// Maximum number of zombies a spawner can spawn
	public int 	 m_iSpawnLimitMin;	// Minimum number of zombies a spawner can spawn
	public int 	 m_fSpawnDist;		// How far away the spawner can be before it is disregarded for use (If no spawner is in range than waves will increment quickly. POTATO!)
	public float m_fSpawnWaveMult;	// Multipler to the number range of how many zombies can be spawned this wave
	public float m_fSpawnDownTime;	// After a spawner revives a zombie, how long to wait until another zombie is spawned



	[HideInInspector]
	public bool m_SpawnNow;

	// Group varibles
	public int m_iGroupLimit;		// How many zombies can be in each group at max


	// Master varibles
	[HideInInspector]
	public int m_iGroupCount 		= -1;	// Index of groups there are, +1 to this value for total number
	public int m_iZombieWaveLimit 	= 0;	// CHANGE THIS! This is now how many to spawn this wave
	public int m_iMaxActiveZomb 	= 0;	// The highest number of zombies that can be active at once
	
	int m_iZombiesLeft  	= 0;	// Number of zombies left in this wave
	int m_iActiveZombies	= -1; 	// How many zombies are wandering around

	uint m_ZombieBits 		= 0;	// These bits represent which zombies are alive

	float	m_fSpawnCD;		// How long until this spawner can spawn another zombie

	// Data arrays and lists
	public List<GroupAI> m_Groups = new List<GroupAI>();
	List<Spawner> 		 m_Spawns = new List<Spawner>();
	public ZombieAI[] 	 m_Zombs  = new ZombieAI[32];

	#region Getter/Setters
	/* ARRR! Here be Gettahs n' Settahs! */

	public int ZombiesActive
	{
		get {return m_iActiveZombies;}
		set {m_iActiveZombies = value;}
	}

	#endregion

	#region Initial Functions


	void Awake()
	{
		if (m_bOFFLINEMODE) 
		{
			StartCoroutine(SpawnZombies())	;
		
		}
	}

	/* These functions are called as the scene is loaded and NEVER during gameplay  */

	/// <summary>
	/// Awake this instance. Initialize any internal varible that I will need to track
	/// </summary>
	public IEnumerator SpawnZombies()
	{
		yield return new WaitForSeconds (5); // HACK FIX!!!! DO IT THE CORRECT WAY!!!!!


			m_SpawnNow = true;
			m_fSpawnCD = m_fSpawnDownTime;

			// Start by initizaling the groups
			GroupAI.FindMaster(this);
			GroupAI.s_iCap = m_iGroupLimit;

			Vector3 nonActiveSpawn = new Vector3(0,5.0f,0);
			m_iActiveZombies = 0;

			// Spawn all possible zombies at once on load up
			for (int i = 0; i < 32; ++i)
			{
				m_Zombs[i] = (ZombieAI)((GameObject)GameObject.Instantiate(m_ZombieStd, nonActiveSpawn, Quaternion.identity)).GetComponent("ZombieAI");
				m_Zombs[i].m_iMasterID = i;
			}

			if(!m_bOFFLINEMODE)
				Z_Network_ZombieSpawner.Get ().SummonRemoteZombies ();

			//Activate all the Spanwnners now that AI Master is ready

			Spawner[]  ZombieSpawns = FindObjectsOfType(typeof(Spawner)) as Spawner[];
			foreach (Spawner zs in ZombieSpawns)
			{
				zs.Activate();
			}
			m_bIsActive = true;
	
	}

	/// <summary>
	/// Will add the spawner passed in to the AIMaster list of spawners for future reference.
	/// </summary>
	/// <param name="_spawner">The spawner to start keeping track of.</param>
	public void SpawnerCheckIn(Spawner _spawner)
	{
		// Set varibles to default
		if (!_spawner.m_OverrideMaster)
		{
			_spawner.m_iMaxSpawn = m_iSpawnLimitMax;
			_spawner.m_iMinSpawn = m_iSpawnLimitMin;
			_spawner.m_fWaveMult = m_fSpawnWaveMult;
			_spawner.m_fDistance = m_fSpawnDist;
		}

		// Give spawners a reference to Zombie prefabs
		if (Spawner.m_zombieRef == null)
		{
			Spawner.m_zombieRef = m_ZombieStd;
		}

		m_Spawns.Add(_spawner);
	}

	#endregion

	#region AI Management Functions
	/*  These Functions will affect the other AI scripts or tweak values in the master */

	public void CreateNewGroup()
	{
		m_Groups.Add((GroupAI)ScriptableObject.CreateInstance("GroupAI"));
		
		++m_iGroupCount;
		m_Groups[m_iGroupCount].Init();
	}

	/// <summary>
	/// Called in the ZombieAI's LoseHealth(int) function if health is < 0.
	/// This function will keep the zombies coming after one gets popped.
	/// </summary>
	/// <param name="_zomb">The zombie that died.</param>
	public void ZombieRespawn(int _i)
	{
		float dist = float.MaxValue;
		int closestIdx = -1;

		for (int i = 0; i < m_Spawns.Count; ++i)
		{
			float tDist = (m_Zombs[_i].gameObject.transform.position - m_Spawns[i].gameObject.transform.position).magnitude;

			if (tDist < dist)
			{
				dist = tDist;
				closestIdx = i;
			}
		}

		if (closestIdx > -1)	// A spawner was found, and it's the closest one to where this zombie died!
		{
			m_Spawns[closestIdx].Spawn(_i);
		}
	}

	public void ZombieDeath(int _deadID)
	{
		--m_iZombiesLeft;

		uint mask = 0xFFFFFFFE;
		mask = (mask << _deadID) | (mask >> (32 - _deadID));	// Rotate the bits so the 0 bit is in the right place

		m_ZombieBits = m_ZombieBits & mask;	// Set the correct zombie as dead
		m_Zombs [_deadID].Redeath ();	// Zombie can now start really being dead
	}

	#endregion
	
	#region Frame by Frame Fucntions
	/* These are functions that are called every single frame. Expect these functions to call other functions */ 

	// Update is called once per frame
	void Update () 
	{
		if (!m_bIsActive)
			return;
		if (players.Count == 0)
			return;
		if (!m_bOFFLINEMODE) 
		{
			if (PhotonNetwork.isNonMasterClientInRoom)
				return;
		}

		// Check if the wave is completed and a new one will have to be generated
		if (m_iZombiesLeft <= 0)
		{
			// Up the wave count and ready spawn related data
			++wave;
			m_SpawnNow = false;
		}

	SpawningUpdate:

		if (m_SpawnNow)
		{
			if (m_ZombieBits < uint.MaxValue)	// If any zombie has be marked as dead, go through the zombie bits and figure which one is dead. Then revive
			{
				if ((m_iActiveZombies+1) > m_iZombieWaveLimit)
					goto OtherUpdates;

				uint mask = 0x00000001;

				// Get the first zombie in the array so I can tell an appropiate spawner to spawn it
				for (int i = 0; i < 32; ++i)
				{
					if ((m_ZombieBits & mask) == 0)	// This zombie has been marked as dead (0) in the bits
					{
						// Find appropiate spawner	// Potato: For now, I just always use the first spawner.
  						m_Spawns[0].Spawn(i);
						m_ZombieBits = m_ZombieBits | mask;	// Set the bit as alive (1)
						++m_iActiveZombies;
						return;	// More often than not only 1 zombie will be dead right?
					}
					
					mask = (mask << 1);	// Move the set bit by 1 to the left. This is to check the next zombie bit
				}
			} 
		}
		else // Can't spawn just now, count down the wave delay timer
		{
			if (true)	// Potato: Later this will be the timer check to seperate waves
			{
				m_SpawnNow = true;
				m_iZombieWaveLimit = wave*10;
				m_iZombiesLeft = m_iZombieWaveLimit;
				m_iActiveZombies = 0;
			}
		}

	OtherUpdates:

		// ::: DEBUG TEST ::: 
		// Press 1-4 to raise the threat of that player to all living zombies by 10
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			for (int i = 0; i < 32; ++i)
			{
				if (m_Zombs[i].Health > -1)
				{
					m_Zombs[i].DEBUG_ADD_THREAT(0, 10);
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			for (int i = 0; i < 32; ++i)
			{
				if (m_Zombs[i].Health > -1)
				{
					m_Zombs[i].DEBUG_ADD_THREAT(1, 10);
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			for (int i = 0; i < 32; ++i)
			{
				if (m_Zombs[i].Health > -1)
				{
					m_Zombs[i].DEBUG_ADD_THREAT(2, 10);
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			for (int i = 0; i < 32; ++i)
			{
				if (m_Zombs[i].Health > -1)
				{
					m_Zombs[i].DEBUG_ADD_THREAT(3, 10);
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			m_Groups[0].RemoveZombie(m_Groups[0].m_Zombie[1]);
		}
		// :::
	}

	// Handle all the movement Updates here
	void FixedUpdate()
	{
		for (int i = 0; i < m_Groups.Count; ++i) // Update every group
		{
			m_Groups[i].UpdateGroup();
		}
	
	}
	#endregion

}

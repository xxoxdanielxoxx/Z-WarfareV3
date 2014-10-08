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

	public GameObject[] players;
	public int wave;

	private byte[] districtCount;
	private byte[] maxDistrictCount;

	// A static reference to this class incase it is needed by any other script
	public static AIMaster m_Reference;

	// Reference to all the zombie prefabs
	public GameObject	m_ZombieStd;	// Standard zombie
	public GameObject	m_ZombieRun;	// Runner zombie
	public GameObject	m_ZombieCom;	// Commander zombie
	public GameObject	m_ZombieTnk;	// Tank zombie

	// Spawner behavioural varibles
	public int 	 m_iSpawnLimitMax; 	// Maximum number of zombies a spawner can spawn
	public int 	 m_iSpawnLimitMin;	// Minimum number of zombies a spawner can spawn
	public int 	 m_fSpawnDist;		// How far away the spawner can be before it is disregarded for use (If no spawner is in range than waves will increment quickly. POTATO!)
	public float m_fSpawnWaveMult;	// Multipler to the number range of how many zombies can be spawned this wave

	// Spawner/Wave logical parameters
	[Range (0.1f, 10.0f)]
	public float	m_fWaveDelay = 2.5f;	// How much time inbetween waves

	[Range (0.1f, 5.0f)]
	public float 	m_fSpawnDelay = 1.0f;	// How long until the spawners reactivates

	[HideInInspector]
	public bool m_SpawnNow;
	
	private float	m_fWaveTimer;	// This will tick down to the next wave
	private float	m_fSpawnTimer;	// This will tick down to the next spawning group

	private bool	m_bWaveCD;		// If true than start the timer to start the next wave
	private bool	m_bSpawnCD;		// If true than start the timer to start spawning the next set of zombies
	


	// Group varibles
	public int m_iGroupLimit;		// How many zombies can be in each group at max


	// Master varibles
	[HideInInspector]
	public int m_iGroupCount 		= -1;	// Index of groups there are, +1 to this value for total number
	public int m_iZombieWaveLimit 	= 0;	// CHANGE THIS! This is now how many to spawn this wave
	public int m_iMaxActiveZomb 	= 0;	// The highest number of zombies that can be active at once

	[Range(0.1f, 0.9f)]
	public float	m_fZombieAngerThres = 0.5f;


	
	int m_iZombiesLeft  	= 0;	// Number of zombies left in this wave
	int m_iActiveZombies	= -1; 	// How many zombies are wandering around

	uint m_ZombieBits 		= 0;	// These bits represent which zombies are alive

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

	public bool IsZombieAlive(int _zombIdx)
	{
		uint mask = 0x00000001;
		uint bits = m_ZombieBits;
		bits = (bits << _zombIdx);	// Move the set bit by to the index of the zombie we need to check if it's alive

		if ((bits & mask) == mask)
			return true;
		else
			return false;
	}

	#endregion

	#region Initial Functions


	void Awake()
	{
		if (m_bOFFLINEMODE) 
		{
			StartCoroutine(StartAI())	;

		}
	}

	/* These functions are called as the scene is loaded and NEVER during gameplay  */

	/// <summary>
	/// Awake this instance. Initialize any internal varible that I will need to track
	/// </summary>
	public IEnumerator StartAI()
	{
		yield return new WaitForSeconds (5); // HACK FIX!!!! DO IT THE CORRECT WAY!!!!!

		if (m_Reference == null)
			m_Reference = this;


		PlayerIDManager.Get().Activate();
		players = new GameObject[4];
		districtCount = new byte[4];
		maxDistrictCount = new byte[4];
		players = PlayerIDManager.Get ().GetALLPlayers ();


		m_SpawnNow 		= false;
		m_bSpawnCD 		= false;
		m_bWaveCD 		= true;
		m_fSpawnTimer	= m_fSpawnDelay;
		m_fWaveTimer  	= m_fWaveDelay;

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
			m_Zombs[i].SetThreatThreashold(m_fZombieAngerThres);
		}

		if(!m_bOFFLINEMODE)
			Z_Network_ZombieSpawner.Get().SummonRemoteZombies();

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
		m_Groups[m_Groups.Count-1].Init(m_Groups.Count-1);
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
		--m_iActiveZombies;

		uint mask = 0xFFFFFFFE;
		mask = (mask << _deadID) | (mask >> (32 - _deadID));	// Rotate the bits so the 0 bit is in the right place

		m_ZombieBits = m_ZombieBits & mask;	// Set the correct zombie as dead
		m_Zombs [_deadID].Redeath ();	// Zombie can now start really being dead

		if (m_iZombiesLeft <= 0)	// Killed the last thing in the wave
		{
			m_bSpawnCD  = false;
			m_bWaveCD 	= true;
		}
		else 	// Wave still going on!
		{
			m_bSpawnCD = true;
		}
	}

	#endregion
	
	#region Frame by Frame Fucntions
	/* These are functions that are called every single frame. Expect these functions to call other functions */ 

	// Update is called once per frame
	void Update () 
	{
		if (!m_bIsActive)
			return;
		if (players.Length == 0)
			return;
		if (!m_bOFFLINEMODE) 
		{
			if (PhotonNetwork.isNonMasterClientInRoom)
				return;
		}

		// Check if the delay between waves is completed and a new one will have to be generated
		if (m_fWaveTimer <= 0)
		{
			// Up the wave count and ready spawn related data
			++wave;
	
			m_iZombieWaveLimit = wave*10;
			m_iZombiesLeft = m_iZombieWaveLimit;
			m_iActiveZombies = 0;
			m_fWaveTimer = m_fWaveDelay;

			m_SpawnNow = true;
			m_bWaveCD = false;
		}

		if (m_SpawnNow)
		{
			if (m_iActiveZombies >= m_iZombiesLeft)
			{
				m_SpawnNow = false;
				return;
			}

			if (m_ZombieBits < uint.MaxValue)	// If any zombie has be marked as dead, go through the zombie bits and figure which one is dead. Then revive
			{

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
			if (m_iActiveZombies == 0 && m_bWaveCD == false)
				m_bSpawnCD = true;

			if (m_bSpawnCD)
				m_fSpawnTimer -= Time.deltaTime;

			if (m_bWaveCD)
				m_fWaveTimer -= Time.deltaTime;

			if (m_fSpawnTimer < 0.0f)
			{
				m_SpawnNow = true;
				m_bSpawnCD = false;
				m_fSpawnTimer = m_fSpawnDelay;
			}
		}
	}

	// Handle all the movement Updates here
	void FixedUpdate()
	{
		for (int i = 0; i < m_Groups.Count; ++i) // Update every group
		{
			m_Groups[i].UpdateGroup();	// I actually don't think this is being used right now. Consider what I can use a group's center point for
		}
	
	}
	#endregion

}

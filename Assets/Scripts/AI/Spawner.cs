using UnityEngine;
using System.Collections;

/// <summary>
/// Attach this script to as many empty object you want to turn into spawn points.
/// 
/// Spawns a ton of zombies for the game.
/// </summary>

public class Spawner : MonoBehaviour 
{
	static AIMaster 	m_Master = null;
	public static  GameObject 	m_zombieRef = null;
	public static  GameObject	m_runnerRef = null;
	public static  GameObject	m_commdrRef = null;
	public static  GameObject	m_tankRef   = null;

	public bool	m_Active		 = true;	// Can this spawner even spawn at all?
	public bool m_OverrideMaster = false;	// Allows this spawner to ignore parameters set by AIMaster at build time

	// Parameters for spawning
	public int		m_iMaxSpawn;	// Spawn no more than this many zombies
	public int  	m_iMinSpawn;	// Spawn at least this many zombies
	public float	m_fWaveMult;	// The influence the wave number will have on this spawner
	public float	m_fDistance;	// Maximum distance this spawn can be from a player before it's ignored

	// Spawning Grid
	int 	m_BitArrayGrid = 0x00;		// Use a bit array to represent the space around the spawner that is open
	float	m_fGridResW;	// The width  of each cell in the grid. Make sure the player can fit in here!
	float	m_fGridResH;	// The height of each cell in the grid. Make sure the player can fit in here!
	

	public void Activate()
	{
		m_fGridResW = 1.2f;
		m_fGridResH = 1.2f;

		if (m_Master == null)
		{ 
			m_Master = (AIMaster)FindObjectOfType(typeof(AIMaster));
		}

		m_Master.SpawnerCheckIn(this);

		// Set up the grid. This is important so I know where to spawn things
		Vector3 cellPosOrg = this.gameObject.transform.position;
		cellPosOrg.x -= m_fGridResW * 3;
		cellPosOrg.z -= m_fGridResH * 3;
		Vector3 cellPos = cellPosOrg; 
	}

	/// <summary>
	/// Spawn a zombie marked as dead at this spawner.
	/// </summary>
	/// <param name="_i">Index in the Master zombie array of which zombie to spawn.</param>
	/// <param name="_group">If a group is passed in, this is the group the Zombie will join.</param>
	public void Spawn(int _i)
	{
		if (!m_Active)
			return;

		if (_i >= 32) 
		{
			return;
		}

		m_Master.m_Zombs[_i].gameObject.transform.position = this.gameObject.transform.position;
		
		
		// =================== Here's the code Stephen added ==========================
		
		//m_Master.m_Zombs[_i].Rebirth();
		
		m_Master.m_Zombs[_i].Rebirth(GetComponent<DistrictProperties>().GetDistrict());
		// =================== Here's the code Stephen added ==========================
	}
}

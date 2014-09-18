using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// DO NOT PLACE IN THE SCENE. AIMaster will take care of using this class. Any parameters can also be changed
/// through AIMaster. Later I might make an editor tool if it gets messy to track AIMaster parameters
/// </summary>

// Group up zombies to have them move in formation, and to save cycles by calcuating nav mesh pathing
// for a number of zombies at once.
public class GroupAI : ScriptableObject
{
	static public AIMaster	m_Master;	// Reference to the AIMaster

	public static int s_iCap;	// Limit of how many Zombies can be added to any group

	public Vector3	m_GroupCenter;	// The point in between all the zombies

	public int 		m_iPopulation;	// How many zombies are currently in this group
	public int		m_iIndex;		// The index of this group in the master array for quick comparisons

	public float	m_fPopRatio;	// Percent of filled capacity

	public List<ZombieAI> m_Zombie;	// A dynamic list of all the zombies in this group
	

	public static void FindMaster(AIMaster _master)
	{
		if (m_Master == null)
		{
			m_Master = _master;
		}
	}

	// Use this for initialization
	public void Init(int _mIdx) 
	{
		m_iIndex = _mIdx;

		m_Zombie = new List<ZombieAI>();
		m_iPopulation = -1;
		m_GroupCenter = Vector3.zero;
	}

	public void AddZombie(ZombieAI _zomb)
	{
		m_Zombie.Add(_zomb);
		_zomb.m_Group = this;
		++m_iPopulation;

		m_fPopRatio = (float)m_iPopulation/(float)s_iCap;
	}

	public void RemoveZombie (ZombieAI _zomb)
	{
		m_Zombie.Remove(_zomb);
		_zomb.m_Group = null;
		--m_iPopulation;
		
		m_fPopRatio = (float)m_iPopulation/(float)s_iCap;

		if (m_iPopulation <= 0)
		{
			// Now kill the other group
			m_Master.m_Groups.Remove(this);
			GameObject.Destroy(this);	// POTATO! Recycle this later
		}
	}

	public void MergeGroup(GroupAI _other)
	{
		if (this.m_fPopRatio < 0.5f && _other.m_fPopRatio < 0.5f)	// Both are under 50% full
		{
			// Go through the other zombie list and throw them all into this group
			for (int i = 0; i < _other.m_iPopulation; ++i)
			{
				// Add the other to this group
				AddZombie(_other.m_Zombie[i]);
			}

			// Now kill the other group
			m_Master.m_Groups.Remove(_other);
			GameObject.Destroy(_other);	// POTATO! Recycle this later
		}
	}

	/// <summary>
	/// Call once per frame in the AIMaster Fixed Update.
	/// </summary>
	public void UpdateGroup()
	{
		ZombieGroupCenter();
	}


	public void ZombieGroupCenter()
	{

		m_GroupCenter = Vector3.zero;
		for (int i = 0; i < m_iPopulation; ++i) 
		{
			m_GroupCenter += m_Zombie[i].gameObject.transform.position;
		}

		if (m_iPopulation > 0) 
			m_GroupCenter /= this.m_iPopulation;
		
	}
}

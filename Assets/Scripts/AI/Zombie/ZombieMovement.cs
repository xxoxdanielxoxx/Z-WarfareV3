using UnityEngine;
using System.Collections;

public class ZombieMovement : MonoBehaviour 
{
	/* Some good values for the parameters here (Use as guidelines, not gospel)
	 * 
	 * m_fRepelDist = 1.5f;
	 * m_fAtrctDist = 0;
	 * m_fGroupDist = 0;
	 * 
	 * m_fRepelPow = 0.015f;
	 * m_fAtrctPow = 0;
	 * 
	 * m_fSpeed = .025f;	
	 * m_fRunSpeed = .05f;
	 * m_fWanderTimer = 5f;
	 */


	// Flocking values
	public float 	m_fRepelDist;	// If the distance to another zombie in the group is less than this, repel
	public float 	m_fAtrctDist;	// If something good is in this distance, move to it
	public float	m_fGroupDist;	// If too far from it's group, drop it and add it to a closer one
	
	public float	m_fRepelPow;	// Strength of the repeling force
	public float	m_fAtrctPow;	// Strength of the attraction force
	
	public float 	m_fSpeed;		// Nav mesh Zombie walking speed
	public float	m_fRunSpeed;	// Nav mesh Zombie running speed (at the target when it has one)
	public float	m_fWanderTimer;


	float	m_fWanderT;
	
	Vector3 m_Dir = Vector3.zero;
	Vector3 m_Wandir = Vector3.zero;

	public NavMeshAgent agent;

	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<NavMeshAgent>();
	}

	public void Reset(Vector3 _target)
	{
		// Set up pathfinding
		agent.enabled = true;
		
		agent.SetDestination(_target);
		agent.Stop();
		m_fWanderT = m_fWanderTimer;
		agent.speed = m_fSpeed;
	}

	/// <summary>
	/// Makes sure the position the zombie is heading towards makes sense when in wander mode
	/// </summary>
	public void WanderTargetUpdate()
	{
		if (m_fWanderTimer > m_fWanderT) // Not time to change the wander direction
		{ 
			m_fWanderT += Time.fixedDeltaTime;
		} 
		else // Now it is!
		{
			m_fWanderT = Random.Range(0.0f, m_fWanderT*0.33f);
			Vector3 pathDir = agent.steeringTarget - this.transform.position;
			m_Wandir = (pathDir.normalized + Random.insideUnitSphere*1.05f).normalized;
		}
	}

	public void Flocking(GroupAI _zGroup)
	{
		Vector3 repelVec = Vector3.zero;		// Add up all repeling forces here
		m_Dir = Vector3.zero;					// Final move vector
		
		// Go through every zombie in the group and get the vector to it
		for (int i = 0; i < _zGroup.m_iPopulation; ++i)
		{
			Vector3 toZombie = _zGroup.m_Zombie[i].gameObject.transform.position - this.gameObject.transform.position;
			
			if ( toZombie.magnitude < m_fRepelDist)	// If the other zombie is within the repel distance
			{
				float distFactor = (m_fRepelDist - toZombie.magnitude);	// How much the distance should factor in
				if (distFactor < 0)
					distFactor = 0;
				
				// Do the repeling
				repelVec -= (toZombie * distFactor)*5;	// Add up all the repelling forces
			}
		}
		
		m_Dir = repelVec;
		
		// Now apply a force to pull the zombie group together
		Vector3 groupVec = this.transform.position - _zGroup.m_GroupCenter;
		float groupDist = groupVec.magnitude * 0.15f;	// Distance from the group of zombies (modified for the proper attractive force)
		
		m_Dir -= groupVec.normalized * groupDist;
	}

	/// <summary>
	/// Updates the path so the zombies can react to player actions. Will adjust the path when players
	/// move or generate more aggro.
	/// </summary>
	public void PathUpdate(ZombieThreatLogic _zThreat, Vector3 _pos)
	{
		
		if (_zThreat.m_AggrOn)	// Locked onto a target, ignore the Threat and just head straight to target
		{
			agent.SetDestination(GroupAI.m_Master.players[_zThreat.m_TargetIdx].transform.position);	// And tell it to target this new point!
			return;
		}
		
		Vector3 mid = _pos;	// Recalculate the mid point since the player might've moved
		Vector3 offset = Vector3.zero;	// This offset will let the zombies move closer to high threat players

		if (GroupAI.m_Master.players.Count > 1)
		{
			// Divide the sumation to get the percent of hate a player has, which will directly translate to threat
			for (int i = 0; i < GroupAI.m_Master.players.Count; ++i)
			{
				offset += _zThreat.m_Threat[i] * (GroupAI.m_Master.players[i].transform.position - mid);
			}
		}
		
		agent.SetDestination(mid + offset);	// And tell it to target this new point!

		//if (agent.steeringTarget != GroupAI.m_Master.players[0].transform.position)
		{
			Debug.DrawLine(this.gameObject.transform.position, agent.steeringTarget, Color.red, 1.0f);
		}

		//agent.Stop();
		
		//agent.Move((m_Wandir + m_Dir).normalized * m_fSpeed);
	}

	public void NavMeshSpeed(float _val)
	{
		agent.speed = _val;	// Give the zombie a running speed
	}
}

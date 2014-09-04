using UnityEngine;
using System.Collections;

public class ZombieAI : MonoBehaviour 
{
	// Start off doing the flockings, but I guess I need some framework done. Get the zombies added to groups

	// Need a reference to this zombie's group
	public GroupAI m_Group;

	ZombieStateMachine 	m_StateScript;
	ZombieHealth		m_HealthScript;

	ZombieMovement		m_MovementScript;


	ZombieThreatLogic		m_ThreatLogic;
	ZombieThreatRecognition	m_ThreatRecon;

	[HideInInspector]
	public int m_iMasterID;

	public ZombieStateMachine GetStateMachine()
	{
		return m_StateScript;
	}

	public ZombieHealth GetHealth()
	{
		return m_HealthScript;
	}

	public ZombieMovement GetMovement()
	{
		return m_MovementScript;
	}

	public ZombieThreatLogic GetThreatLogic()
	{
		return m_ThreatLogic;
	}

	public ZombieThreatRecognition GetThreatRec()
	{
		return m_ThreatRecon;
	}


	// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

	public void DEBUG_ADD_THREAT(int _idx, uint _amount)
	{
		m_ThreatLogic.m_Hatred[_idx] += _amount;	// Remember to remove this function later once player can shoot
		m_ThreatLogic.CalculateThreat();
	}

	// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

	public int Health
	{
		get { return m_HealthScript.m_iHealth; }
		set { m_HealthScript.m_iHealth = value;}
	}

	void Awake()
	{
		m_ThreatRecon	 = GetComponent<ZombieThreatRecognition>();
		m_StateScript 	 = GetComponent<ZombieStateMachine> ();
		m_ThreatLogic    = GetComponent<ZombieThreatLogic>  ();
		m_MovementScript = GetComponent  <ZombieMovement>   ();
		m_HealthScript   = GetComponent  <ZombieHealth>     ();

		m_StateScript.Init(this);
		m_ThreatRecon.Init(this);
	}

	void Start()
	{
		if (m_ThreatRecon.enabled && m_HealthScript.enabled && m_ThreatLogic.enabled && m_StateScript.enabled)
			m_ThreatRecon.Init(this);
	}

	/// <summary>
	/// Averages out the position of all the players to get a middle point between them all
	/// </summary>
	/// <returns> The point between all the players. </returns>
	Vector3 PlayerPositionMiddle()
	{
		Vector3 avgPos = Vector3.zero;	// Start at (0,0,0)

		if (GroupAI.m_Master.players.Count == 1)
						return GroupAI.m_Master.players [0].transform.position;

		for (int i = 0; i < GroupAI.m_Master.players.Count; ++i)
		{
			avgPos += GroupAI.m_Master.players[i].transform.position;	// Get a summation of all the player pos
		}
		avgPos /= (GroupAI.m_Master.players.Count-1);	// Divide by the number of players
		
		return avgPos;	// And this is the average
	}

	public void Rebirth()
	{
		// Init stats
		m_HealthScript.m_iHealth = 99;

		if (m_StateScript.enabled)
			m_StateScript.Reset();

		if (m_MovementScript.enabled)
			m_MovementScript.Reset (PlayerPositionMiddle());

		if (m_ThreatLogic.enabled)
			m_ThreatLogic.Reset();
	}

	public void Redeath()
	{
		transform.position = new Vector3 (0,-15f, 0);

		if (m_MovementScript.enabled)
		m_MovementScript.agent.enabled = false;
	}

	void FixedUpdate()
	{
		if (m_StateScript.m_State == ZombieStates.Dead)
		{
			//transform.position = new Vector3(0, -500.0f, 0);
			return;
		}

		// If health is no a negative number then it's still alive so do things
		if (m_HealthScript.m_iHealth >= 0)	
		{
			if (m_StateScript.m_State == ZombieStates.Wander)
			{
				// This is the default state of the zombies. In this state zombies will move a at slow pace
				//towards the players though not directly at the players unless they are bunched up (due to the calculation)
				//will transition to the runnning state when the zombie has over 70% threat on one target

				if (m_Group == null)
				{
					m_StateScript.m_State = ZombieStates.LFG;
					return;
				}
				
				m_MovementScript.Flocking(m_Group);
				m_MovementScript.WanderTargetUpdate();
				m_MovementScript.PathUpdate(m_ThreatLogic, PlayerPositionMiddle());
				
				// Divide the sumation to get the percent of hate a player has, which will directly translate to threat
				for (byte i = 0; i < GroupAI.m_Master.players.Count; ++i)
				{
					if (m_ThreatLogic.m_Threat[i] >= 0.7f)
					{
						m_StateScript.m_State = ZombieStates.Run;	// Set the state to Run
						m_MovementScript.NavMeshSpeed(m_MovementScript.m_fRunSpeed); // Go at a nice pace :^)
						m_Group.RemoveZombie(this);	// Get it off of the group list
						m_ThreatLogic.m_TargetIdx = i;	// Keep track of who I'm charging towards
					}
				}
			}
			else if (m_StateScript.m_State == ZombieStates.Run)
			{
				// The zombie will charge at a high threat target. When it gets close enough it will never change targets
				// The way the zombie moves should change as well; Zombie should move towards the player with intent to kill

				m_MovementScript.PathUpdate(m_ThreatLogic, PlayerPositionMiddle());

				// If the zombie gets close to it's target then the state should transition to lock on
				float dist = (this.gameObject.transform.position - GroupAI.m_Master.players[m_ThreatLogic.m_TargetIdx].transform.position).magnitude;
				if (dist < 5.5f)
				{
					m_StateScript.m_State = ZombieStates.Lock;
					m_ThreatLogic.m_AggrOn = true;
				}
			}
			else if (m_StateScript.m_State == ZombieStates.Lock)
			{
				// Either kill the target, or die.
				// Ignore all hate, otherwise same as run?

				m_MovementScript.PathUpdate(m_ThreatLogic, PlayerPositionMiddle());

			}
			else if (m_StateScript.m_State == ZombieStates.LFG)
			{
				// Figure out what I want to do to have this zombie find another group
			}
		}
	}
}

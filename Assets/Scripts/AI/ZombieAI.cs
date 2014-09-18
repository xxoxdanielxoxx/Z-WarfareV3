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

		if (GroupAI.m_Master.players.Length == 1)
			return GroupAI.m_Master.players [0].transform.position;

		for (int i = 0; i < GroupAI.m_Master.players.Length; ++i)
		{
			avgPos += GroupAI.m_Master.players[i].transform.position;	// Get a summation of all the player pos
		}
		avgPos /= (GroupAI.m_Master.players.Length);	// Divide by the number of players

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
				for (byte i = 0; i < GroupAI.m_Master.players.Length; ++i)
				{
					if (m_ThreatLogic.m_Threat[i] >= 0.7f && m_ThreatLogic.m_Hatred[i] > 10)
					{
						m_StateScript.m_State = ZombieStates.Run;	// Set the state to Run
						m_MovementScript.NavMeshSpeed(m_MovementScript.m_fRunSpeed); // Go at a nice pace :^)
						m_ThreatLogic.m_TargetIdx = i;	// Keep track of who I'm charging towards

						if (m_Group != null)
							m_Group.RemoveZombie(this);	// Get it off of the group list
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
				// Potato Idea: Hey, what if the zombies in this state avoided the player. 
				//   Stay within sight of the group, but try to hide from the player and try to get in range of other zombies

				AIMaster master = AIMaster.m_Reference;

				// Figure out what I want to do to have this zombie find another group
				for (int i = 0; i < 32; ++i)
				{
					if ( master.IsZombieAlive(i))	// The zombie we're checking is alive
					{
						ZombieAI zomb = master.m_Zombs[i];

						if ((zomb.transform.position - this.transform.position).magnitude < 30)	// The two zombies are in a close distance of each other. Group up
						{
							if (zomb.m_Group == null)	// The zombie we're checking doesn't have a group
							{
								if (this.m_Group != null)	// if I do then add the other zombie to this group
								{
									m_Group.AddZombie(zomb);

									break;
								}
								else 	// If I don't either than create a new group and add both the zombies
								{
									master.CreateNewGroup();
									m_Group = master.m_Groups[master.m_Groups.Count-1];
									m_Group.AddZombie(this);
									m_Group.AddZombie(zomb);

									break;
								}
							}
							else 	// The other zombie has a group!
							{
								if (this.m_Group != null)	// I do too, so we'll merge the two groups
								{
									if (zomb.m_Group.m_iIndex != this.m_Group.m_iIndex)	// Make sure they aren't the same group
									{
										zomb.m_Group.MergeGroup(m_Group);	// Since I'm the one looking for a group, I'll join the other group
									
										break;
									}
								}
								else 	// I don't have a group, I need to be added to the other zombie group
								{
									zomb.m_Group.AddZombie(this);

									break;
								}
							}
							m_StateScript.m_State = ZombieStates.Wander;	// Something got added to a group so stop looking!
						}
					}

					if (master.ZombiesActive <= i-1)
					{
						break;
					}
				}
			}
		}
	}
}

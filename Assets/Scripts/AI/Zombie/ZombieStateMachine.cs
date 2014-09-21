using UnityEngine;
using System.Collections;

public enum ZombieStates
{
	Wander 	= 0,
	Run 	= 1,
	Lock	= 2,
	LFG		= 3,
	Attack	= 4,
	Dying	= 5,
	Dead	= 6,
};

public class ZombieStateMachine : MonoBehaviour 
{
	ZombieAI m_behaviour;


	// The state of the zombie
	public ZombieStates m_State;

	public void Init(ZombieAI _zombAI)
	{
		m_behaviour  = _zombAI;
	}

	void Awake()
	{
		m_State = ZombieStates.Dead;

	}

	// Use this for initialization
	public void Reset () 
	{
		m_State = ZombieStates.LFG;
	}

	public void ChangeState(ZombieStates _state)
	{
		if (m_behaviour == null)
		{
			ChangeStateRemoteSafe(_state);
			return;
		}

		switch(_state)
		{
		case ZombieStates.Wander:

			goto default;

		case ZombieStates.Run:

			goto default;

		case ZombieStates.Lock:

			goto default;

		case ZombieStates.LFG:

			goto default;

		case ZombieStates.Attack:

			goto default;

		case ZombieStates.Dying:

			goto default;

		case ZombieStates.Dead:
			AIMaster.m_Reference.ZombieDeath(m_behaviour.m_iMasterID);

			goto default;

		default:
			m_State = _state;
			return;
		}
	}

	void ChangeStateRemoteSafe(ZombieStates _state)
	{
		switch(_state)
		{
		case ZombieStates.Wander:
			
			goto default;
			
		case ZombieStates.Run:
			
			goto default;
			
		case ZombieStates.Lock:
			
			goto default;
			
		case ZombieStates.LFG:
			
			goto default;
			
		case ZombieStates.Attack:
			
			goto default;
			
		case ZombieStates.Dying:
			
			goto default;
			
		case ZombieStates.Dead:
			
			goto default;
			
		default:
			m_State = _state;
			return;
		}
	}
}
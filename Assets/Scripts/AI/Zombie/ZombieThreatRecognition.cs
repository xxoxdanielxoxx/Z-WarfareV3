using UnityEngine;
using System.Collections;

public class ZombieThreatRecognition : MonoBehaviour 
{
	ZombieAI m_behaviour;
	ZombieThreatLogic	m_logic;

	public void Init(ZombieAI _zombAI)
	{
		m_behaviour  = _zombAI;
		m_logic = _zombAI.GetThreatLogic ();
	}
	
	// this used to be OnTriggerEnter(Collider _other), now a function that is run by the bullet upon collision
	// needed to be a function because Stephen is bad at programming, code within functionhas not been changed -Stephen
	public void HitByBullet(GameObject _other)
	{
		if (_other.tag == "Bullet")
		{
			Bullet bullet = _other.GetComponent<Bullet>();
			if (bullet != null)
			{
				int pIdx = PlayerIDManager.Get().FindPlayerIndex( bullet.GetPlayerID());
				
				if (!m_behaviour.GetHealth().TakeDamage(bullet.GetDamage()))
				{
					// Zombie is dead
					m_behaviour.GetStateMachine().ChangeState(ZombieStates.Dead);	// Potato: There's no dying animation, go directly to dead state. Do not pass GO.
				}
				else
				{
					if (m_behaviour.m_Group != null)
					{
						// Zombie is pissed
						for (int i = 0; i < m_behaviour.m_Group.m_Zombie.Count; ++i)
						{
							m_behaviour.m_Group.m_Zombie[i].GetThreatLogic().BuildHate((uint)bullet.GetDamage()/4, pIdx);
						}
					}

					m_logic.BuildHate((uint)bullet.GetDamage(), pIdx);
				}
				Destroy(_other.gameObject);
			}
		}
	}

	public void SoundDetected(Vector3 inputPosition, int inputPlayerID, float inputThreat)
	{
		// Function for "actions", i.e. sounds that cause threat to the zombies where:
		// inputPosition = the sound's position
		// inputPlayerID = the playerID of the sound
		// inputThreat = the threat value of the sound
		// Threat has already been calculated, so apply as you see fit
		// Debug.Log ("sound detected. playerID: " + inputPlayerID);
		
		int pIdx = PlayerIDManager.Get().FindPlayerIndex(inputPlayerID);

		if (pIdx < 4)
		{
			// One of the players pissed me off
			m_logic.BuildHate((uint)inputThreat, pIdx);
		}
		else
		{
			// Enviromental hazard (IE. grenade)
		}
	}
}

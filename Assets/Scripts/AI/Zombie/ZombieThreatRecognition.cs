using UnityEngine;
using System.Collections;

public class ZombieThreatRecognition : MonoBehaviour 
{
	ZombieAI m_behaviour;

	public void Init(ZombieAI _zombAI)
	{
		m_behaviour  = _zombAI;
	}

	public void OnTriggerEnter(Collider _other)
	{

		if (_other.tag == "Bullet")
		{
			Bullet bullet = _other.GetComponent<Bullet>();
			if (bullet != null)
			{
		
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
							m_behaviour.m_Group.m_Zombie[i].GetThreatLogic().BuildHate((uint)bullet.GetDamage()/4, bullet.GetPlayerID());
						}
					}

					m_behaviour.GetThreatLogic().BuildHate((uint)bullet.GetDamage(), bullet.GetPlayerID());
				}

				Destroy(_other.gameObject);

			}
		}
	}
}

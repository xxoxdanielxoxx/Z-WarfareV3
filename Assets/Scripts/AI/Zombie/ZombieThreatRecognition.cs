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
				Destroy(_other.gameObject);

			}
		}
	}
}

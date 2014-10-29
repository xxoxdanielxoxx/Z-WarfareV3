using UnityEngine;
using System.Collections;

public class ZombieHealth : MonoBehaviour 
{
	public int 		m_iHealth = -1;
	public bool		m_bWince = false;

	private float t = 0;

	public bool TakeDamage(int _dmg)
	{
		m_iHealth -= _dmg;
		m_bWince = true;
		t = 0.0f;
		
		//this.gameObject.renderer.material.color = Color.red;

		if (m_iHealth < 0)
			return false;

		return true; // Returns true if this zombie survived the hit
	}

	void Update()
	{

	}

	void TakenDamage()
	{
		m_bWince = false;

	}

}

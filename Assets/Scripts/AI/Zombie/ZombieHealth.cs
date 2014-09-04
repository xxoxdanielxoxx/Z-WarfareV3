using UnityEngine;
using System.Collections;

public class ZombieHealth : MonoBehaviour 
{
	public int 		m_iHealth = -1;

	private float t = 1;

	bool m_bChange = false;
	public bool TakeDamage(int _dmg)
	{
		m_iHealth -= _dmg;
		m_bChange = true;
		t = 0.1f;
		
		this.gameObject.renderer.material.color = Color.red;

		if (m_iHealth < 0)
			return false;

		return true; // Returns true if this zombie survived the hit
	}

	void Update()
	{
		if(m_bChange)
		{

			t -= Time.deltaTime;
			if (t < 0)
			{
				White();
			}
		}

	}

	void White()
	{
		this.gameObject.renderer.material.color = Color.white;
		m_bChange = false;
	}

}

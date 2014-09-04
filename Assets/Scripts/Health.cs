using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour 
{
	public float m_fHealth = 100;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_fHealth <= 0)
		{
			Destroy (gameObject);
		}
	}
	
	public void TakeDamage(float damage)
	{
		m_fHealth -= damage;
	}
}

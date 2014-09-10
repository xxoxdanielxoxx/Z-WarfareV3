using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour 
{
	public float m_fHealth = 100;
	private bool m_bHasMedkit = false;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_fHealth <= 0)
		{
			Debug.Log (gameObject.name + " died");
		}
	}
	
	public void TakeDamage(float damage)
	{
		m_fHealth -= damage;
	}
	
	public float GetHealth()
	{
		return m_fHealth;
	}
	public void SetHealth(float input)
	{
		m_fHealth = input;
	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Pickup" && other.transform.parent.parent.tag == "Medkit" && m_bHasMedkit == false)
		{
			if (!GetComponent<Medkit>())
			{
				gameObject.AddComponent("Medkit");
				other.GetComponentInParent<Medkit>().MedkitTaken();
			}
			
		}
	}
}

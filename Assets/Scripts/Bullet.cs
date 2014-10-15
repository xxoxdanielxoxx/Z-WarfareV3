using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{

	public enum BulletType
	{
		Pistol,
		Rifle,
		Shotgun,
		Sniper,
		None
	}
	// m_eBullet
	
	private BulletType m_eBullet;
	private float m_fBulletSpeed = 1.0f;
	private int m_iDamage = 10;
	private Ray m_ray;
	private RaycastHit m_rHit;
	private float m_fDistanceTraveled = 0;
	private float m_fBulletRange = 100;
	private int m_iPlayerID;

	// Use this for initialization
	void Start ()
	{
		tag = "Bullet";
		// automatically destroy the bullet after 2 seconds. a bullet still hasn't hit anything
		// after 2 seconds of travel time, something has gone wrong
		Destroy (gameObject, 2);
	}
	
	void Update ()
	{
		BulletBehavior();
	}
	
	void BulletBehavior()
	{
		m_ray = new Ray(transform.position, transform.forward);
		Physics.Raycast (m_ray, out m_rHit, m_fBulletSpeed);
		//if (!m_rHit.collider || m_rHit.collider.tag == "WeaponSpawner")
		//{
			//it is still live and flying, move bullet to new physics position;
		m_fDistanceTraveled += Vector3.Distance (transform.position, m_ray.GetPoint(m_fBulletSpeed));
		transform.position = m_ray.GetPoint(m_fBulletSpeed);

		if (m_fDistanceTraveled >= m_fBulletRange)
		{
			Destroy (gameObject);
		}

		if (m_rHit.transform && m_rHit.transform.GetComponent<ZombieHealth>())
		{
			// we hit a zombie
			if (Physics.Raycast (m_ray, m_fBulletSpeed, 0x00001000))
			{
				// Head shot
				m_rHit.transform.GetComponent<ZombieThreatRecognition>().BulletToTheHead(this.gameObject);
			}
			else if (Physics.Raycast (m_ray, m_fBulletSpeed, 0x00002000))
			{
				// Body shot
				m_rHit.transform.GetComponent<ZombieThreatRecognition>().HitByBullet(this.gameObject);
			}
			Destroy (gameObject);
		}
		//}
//		else
//		{
//			transform.position = m_rHit.point;
//			Health dBB = m_rHit.collider.GetComponent<Health>();
//			if (dBB)
//				dBB.TakeDamage (m_iDamage);
//			//Destroy (gameObject);
//				
//		}
	}

	public void SetPlayerID(int ID)
	{
		m_iPlayerID = ID;
	}
	
	public void SetBulletSpeed(float input)
	{
		m_fBulletSpeed = input;
	}
	
	public void SetDamage(int input)
	{
		m_iDamage = input;
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, 0.2f);
	}

	public int GetDamage()
	{
		return m_iDamage;
	}

	public int GetPlayerID()
	{
		return m_iPlayerID;
	}


	
	// bullets have position, direction, damage, speed
	// position and direction are determined upon firing, damage is determined by prefab
	// we'll have four different prefabs
	// use public enum to denote which bullet type it is
}

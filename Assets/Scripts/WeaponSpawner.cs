using UnityEngine;
using System.Collections;

// this script is no longer used. use ItemSpawner as it is more general
public class WeaponSpawner : MonoBehaviour 
{
	public enum Gun
	{
		Pistol,
		Shotgun,
		Rifle,
		Sniper
	}
	
	public Gun m_eGun = Gun.Pistol;
	public float m_fTimerInitialDelay = 2;
	public float m_fTimer = 5;
	private float m_fInitialGunTimer;
	public bool m_spawnOnce = false;
	private bool m_bGunSpawned = false;

	// Use this for initialization
	void Start () 
	{
		m_fInitialGunTimer = m_fTimer;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_spawnOnce && m_bGunSpawned)
		{
			Destroy (gameObject);
		}
		
		if (!m_bGunSpawned)
		{
			m_fTimerInitialDelay -= Time.deltaTime;
			if (m_fTimerInitialDelay <= 0)
			{
				m_fTimerInitialDelay = 0;
				m_fTimer -= Time.deltaTime;
				if (m_fTimer <= 0)
				{
					m_fTimer = m_fInitialGunTimer;
					m_bGunSpawned = true;
					//SpawnGun ();
				}
			}
		}
	}
	
	/*private void SpawnGun()
	{
		GameObject newGun;
		
		if (m_eGun == Gun.Pistol)
		{
			newGun = (GameObject) Instantiate(Resources.Load ("PickupPistol"), transform.position, transform.rotation);
		}
		else if (m_eGun == Gun.Shotgun)
		{
			newGun = (GameObject) Instantiate(Resources.Load ("PickupShotgun"), transform.position, transform.rotation);
		}
		else if (m_eGun == Gun.Rifle)
		{
			newGun = (GameObject) Instantiate(Resources.Load ("PickupRifle"), transform.position, transform.rotation);
		}
		else
		{
			newGun = (GameObject) Instantiate(Resources.Load ("PickupSniper"), transform.position, transform.rotation);
		}
		newGun.GetComponent<AmmoManager>().MaxAmmo();
		newGun.GetComponent<GunProperties>().SetItemSpawner(gameObject);
		newGun.GetComponent<GunProperties>().MaxAmmo();
	}*/
	
	public void ChangeWeapon(Gun input)
	{
		m_eGun = input;
	}
	
	public void RemoveSpawner()
	{
		Destroy(gameObject);
	}

	public void SetGun(Gun input)
	{
		m_eGun = input;
	}
	
	public void GunTaken()
	{
		m_bGunSpawned = false;
	}
}

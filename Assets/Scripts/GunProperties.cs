using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunProperties : MonoBehaviour 
{
	public enum Gun
	{
		Pistol,
		Rifle,
		Shotgun,
		Sniper,
		None
	}

	private Gun m_eGun = Gun.Pistol;
	private GameObject m_bulletSocket;
	public float m_fRandomSpread = 5;
	private int m_iNumShotgunPellets = 8;
	public float m_fFireRate = 0.25f;
	private float m_fFireRateTimer;
	public float m_fBulletSpeed = 4.0f;
	public int m_iDamage = 10;
	private GameObject m_WeaponSpawner;
	public int m_iAmmo = 0;
	public int m_iMagSize = 20;
	private int m_iMaxPistol = 17;
	private int m_iMaxShotgun = 8;
	private int m_iMaxRifle = 40;
	private int m_iMaxSniper = 5;
	private bool m_bReloading = false;
	public float m_fReloadTime = 2.0f;
	
	// Use this for initialization
	void Start ()
	{
		if (gameObject.tag == "Pistol")
			m_eGun = Gun.Pistol;
		else if (tag == "Shotgun")
			m_eGun = Gun.Shotgun;
		else if (tag == "Rifle")
			m_eGun = Gun.Rifle;
		else if (tag == "Sniper")
			m_eGun = Gun.Sniper;
		
		if (gameObject.tag == "Pistol")
			m_iMagSize = m_iMaxPistol;
		else if (tag == "Shotgun")
			m_iMagSize = m_iMaxShotgun;
		else if (tag == "Rifle")
			m_iMagSize = m_iMaxRifle;
		else if (tag == "Sniper")
			m_iMagSize = m_iMaxSniper;
		
		m_fFireRateTimer = m_fFireRate;
	}
	
	// Update is called once per frame
	void Update ()
	{
		m_fFireRateTimer -= Time.deltaTime;
		
		if (Input.GetKeyDown (KeyCode.R))
			if (!m_bReloading)
				StartCoroutine(Reload());
	}
	
	public void Shoot()
	{
		if (m_fFireRateTimer <= 0)
		{
			if (m_iAmmo > 0 && !m_bReloading)
			{
				m_fFireRateTimer = m_fFireRate;
				if (m_eGun == Gun.Shotgun)
				{
					//Transform bulletTrajectory = m_GBulletSocket.transform;
					//Quaternion bulletRotation = m_bulletSocket.transform.rotation;
					for (int i = 0; i < m_iNumShotgunPellets; i++) 
					{
						Quaternion bulletRotation = new Quaternion(m_bulletSocket.transform.rotation.x, m_bulletSocket.transform.rotation.y, m_bulletSocket.transform.rotation.z, m_bulletSocket.transform.rotation.w);
						bulletRotation.eulerAngles += new Vector3(Random.Range(-m_fRandomSpread, m_fRandomSpread), Random.Range (-m_fRandomSpread, m_fRandomSpread), Random.Range (-m_fRandomSpread, m_fRandomSpread));
						GameObject bullet = (GameObject) Instantiate (Resources.Load ("Bullet"), m_bulletSocket.transform.position, bulletRotation);
						bullet.GetComponent<Bullet>().SetDamage(m_iDamage);
						bullet.GetComponent<Bullet>().SetBulletSpeed(m_fBulletSpeed);
					}
				}
				else
				{
					GameObject bullet = (GameObject) Instantiate (Resources.Load ("Bullet"), m_bulletSocket.transform.position, m_bulletSocket.transform.rotation);
					bullet.GetComponent<Bullet>().SetDamage(m_iDamage);
					bullet.GetComponent<Bullet>().SetBulletSpeed(m_fBulletSpeed);
				}
				m_iAmmo--;
				// do gun recoil
				//MouseLook mouseLook = GetComponentInParent<MouseLook>();
			}
			else
			{
				if (!m_bReloading)
					StartCoroutine(Reload());
			}

		}
	}
	
	private IEnumerator Reload()
	{
		m_bReloading = true;
		AmmoManager m_ammoManager = GetComponent<AmmoManager>();
		int m_iAmmoNeeded = m_iMagSize - m_iAmmo;
		
		if (m_iAmmo < m_iMagSize)
		{
			if (m_ammoManager.GetAmmo() > 0)
			{
				yield return new WaitForSeconds(m_fReloadTime);
				m_bReloading = false;
				if (m_ammoManager.GetAmmo() >= m_iAmmoNeeded)
				{
					m_iAmmo += m_iAmmoNeeded;
					m_ammoManager.SetAmmo(m_ammoManager.GetAmmo() - m_iAmmoNeeded);
				}
				else
				{
					m_iAmmo = m_ammoManager.GetAmmo();
					m_ammoManager.SetAmmo(0);
				}
			}
			else
			{
				m_bReloading = false;
			}
		}
	}
	
	public void MaxAmmo()
	{
		if (gameObject.tag == "Pistol")
			m_iMagSize = m_iMaxPistol;
		else if (tag == "Shotgun")
			m_iMagSize = m_iMaxShotgun;
		else if (tag == "Rifle")
			m_iMagSize = m_iMaxRifle;
		else if (tag == "Sniper")
			m_iMagSize = m_iMaxSniper;
		m_iAmmo = m_iMagSize;
	}
	
	public Gun GetGunType()
	{
		return m_eGun;
	}
	
	public void SetAmmo(int input)
	{
		m_iAmmo = input;
	}
	
	public int GetAmmo()
	{
		return m_iAmmo;
	}
	
	public void SetBulletSocket(GameObject input)
	{
		m_bulletSocket = input;
	}
	
	public void SetWeaponSpawner(GameObject input)
	{
		m_WeaponSpawner = input;
	}
	
	public GameObject GetWeaponSpawner()
	{
		return m_WeaponSpawner;
	}
	
	public void GunPickupTaken()
	{
		if (m_WeaponSpawner)
			m_WeaponSpawner.GetComponent<WeaponSpawner>().GunTaken();
		Destroy (gameObject);
	}
}

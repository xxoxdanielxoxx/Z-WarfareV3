using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GUNTYPE
{
	Pistol,
	Rifle,
	Shotgun,
	Sniper,
	None,
	Grenade
}

public class GunProperties : Photon.MonoBehaviour 
{
	/*
	public enum Gun
	{
		Pistol,
		Rifle,
		Shotgun,
		Sniper,
		None
	}
	*/

	private GUNTYPE m_eGun = GUNTYPE.Pistol;
	private GameObject m_bulletSocket;
	public float m_fRandomSpread = 5;
	private int m_iNumShotgunPellets = 8;
	public float m_fFireRate = 0.25f;
	private float m_fFireRateTimer;
	public float m_fBulletSpeed = 4.0f;
	public int m_iDamage = 10;
	public GameObject m_ItemSpawner;
	public int m_iAmmo = 0;
	public int m_iMagSize = 20;
	private int m_iMaxPistol = 17;
	private int m_iMaxShotgun = 8;
	private int m_iMaxRifle = 40;
	private int m_iMaxSniper = 5;
	private bool m_bReloading = false;
	public float m_fReloadTime = 2.0f;
	
	[Range (1, 100)]
	public float m_fSoundRadius = 10.0f;
	
	// Use this for initialization
	void Start ()
	{
		if (gameObject.tag == "Pistol")
			m_eGun = GUNTYPE.Pistol;
		else if (tag == "Shotgun")
			m_eGun = GUNTYPE.Shotgun;
		else if (tag == "Rifle")
			m_eGun = GUNTYPE.Rifle;
		else if (tag == "Sniper")
			m_eGun = GUNTYPE.Sniper;
		
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
		// just realized that this can cause pickups on the ground to "reload", but doesn't matter as the gun is destroyed and a new one is instantiated
	}
	
	public void Shoot()
	{
		if (m_fFireRateTimer <= 0)
		{
			if (m_iAmmo > 0 && !m_bReloading)
			{
				m_fFireRateTimer = m_fFireRate;
				int playerID =  transform.root.GetComponent<PlayerProperties>().m_iID;
				PhotonView pv = transform.root.GetComponent<PhotonView>();


				//Debug.Log(GetComponent<PlayerProperties>());
				//Debug.Log(GetComponent<PlayerProperties>().m_iID);

				if (m_eGun == GUNTYPE.Shotgun)
				{


					//Transform bulletTrajectory = m_GBulletSocket.transform;
					//Quaternion bulletRotation = m_bulletSocket.transform.rotation;
					for (int i = 0; i < m_iNumShotgunPellets; i++) 
					{
						Quaternion bulletRotation = new Quaternion(m_bulletSocket.transform.rotation.x, m_bulletSocket.transform.rotation.y, m_bulletSocket.transform.rotation.z, m_bulletSocket.transform.rotation.w);
						bulletRotation.eulerAngles += new Vector3(Random.Range(-m_fRandomSpread, m_fRandomSpread), Random.Range (-m_fRandomSpread, m_fRandomSpread), Random.Range (-m_fRandomSpread, m_fRandomSpread));
					
						InstantiateBullet( m_bulletSocket.transform.position, bulletRotation, m_iDamage, m_fBulletSpeed, playerID );
						
						//Create Networked Bullet 
						if(pv != null)
							pv.RPC("InstantiateNetworkBullet", PhotonTargets.Others, m_bulletSocket.transform.position, bulletRotation, m_iDamage, m_fBulletSpeed, playerID );
					}
				}
				else
				{
					
					InstantiateBullet(m_bulletSocket.transform.position, m_bulletSocket.transform.rotation, m_iDamage, m_fBulletSpeed, playerID);
					//Create Networked Bullet
					if(pv != null)
						pv.RPC("InstantiateNetworkBullet", PhotonTargets.Others, m_bulletSocket.transform.position, m_bulletSocket.transform.rotation, m_iDamage, m_fBulletSpeed, playerID );
				}
				
				GameObject emitThreat = (GameObject) Instantiate(Resources.Load ("ThreatEmitter"), transform.position, transform.rotation);
				emitThreat.GetComponent<ThreatEmitter>().SetValues(transform.position, playerID, m_fSoundRadius, m_iDamage);
				
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

	//Creats a local Bullet
	void InstantiateBullet(Vector3 pos, Quaternion rot, int dmg, float spd, int ID)
	{

		Debug.Log ("Creating Bullet");
		GameObject bullet = (GameObject) Instantiate (Resources.Load ("Bullet"), pos, rot);
		bullet.GetComponent<Bullet>().SetDamage(m_iDamage);
		bullet.GetComponent<Bullet>().SetBulletSpeed(m_fBulletSpeed);
		bullet.GetComponent<Bullet>().SetPlayerID(ID);
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
	
	public GUNTYPE GetGunType()
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
	
	public void SetItemSpawner(GameObject input)
	{
		m_ItemSpawner = input;
	}
	
	public GameObject GetItemSpawner()
	{
		return m_ItemSpawner;
	}
	
	public void GunPickupTaken()
	{
		if (m_ItemSpawner)
		{
			m_ItemSpawner.GetComponent<ItemSpawner>().ItemTaken();
		}
		Destroy (gameObject);
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, m_fSoundRadius);
	}
}

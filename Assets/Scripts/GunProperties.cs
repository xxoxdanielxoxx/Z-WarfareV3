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
		// just realized that this can cause pickups on the ground to "reload", but doesn't matter as the gun is destroyed and a new one is instantiated, so it shouldn't become an issue
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

				if (m_eGun == GUNTYPE.Shotgun)
				{
					for (int i = 0; i < m_iNumShotgunPellets; i++) 
					{
						InstantiateBullet(playerID, pv);
						InstantiateThreatEmitter();
					}
				}
				else
				{
					InstantiateBullet(playerID, pv);
					InstantiateThreatEmitter();
				}
				
				//This is not Network Safe
				//GameObject emitThreat = (GameObject) Instantiate(Resources.Load ("ThreatEmitter"), transform.position, transform.rotation);
				//emitThreat.GetComponent<ThreatEmitter>().SetValues(transform.position, playerID, m_fSoundRadius, m_iDamage);
				
				m_iAmmo--;
				// do gun recoil
				//MouseLook mouseLook = GetComponentInParent<MouseLook>();
			}
			else
			{
				// we're out of ammo, player automatically reloads upon trying to shoot
				if (!m_bReloading)
					StartCoroutine(Reload());
			}

		}
	}

	void InstantiateThreatEmitter()
	{
		GameObject threatEmitter = (GameObject)Instantiate (Resources.Load("ThreatEmitter"), transform.position, transform.rotation);
		threatEmitter.GetComponent<ThreatEmitter>().SetValues(transform.position, transform.root.GetComponent<PlayerProperties>().m_iID, m_fSoundRadius, m_iDamage);
	}
	
	//Creates a local and network Bullet
	void InstantiateBullet(int ID, PhotonView pv)
	{
		Quaternion bulletRotation = new Quaternion(m_bulletSocket.transform.rotation.x, m_bulletSocket.transform.rotation.y, m_bulletSocket.transform.rotation.z, m_bulletSocket.transform.rotation.w);
		bulletRotation.eulerAngles += new Vector3(Random.Range(-m_fRandomSpread, m_fRandomSpread), Random.Range (-m_fRandomSpread, m_fRandomSpread), Random.Range (-m_fRandomSpread, m_fRandomSpread));
		
		//Debug.Log ("Creating Bullet");
		Bullet bullet = ((GameObject) Instantiate (Resources.Load ("Bullet"), m_bulletSocket.transform.position, bulletRotation)).GetComponent<Bullet>();
		bullet.SetDamage(m_iDamage);
		bullet.SetBulletSpeed(m_fBulletSpeed);
		bullet.SetPlayerID(ID);
		
		//Create Networked Bullet
		if(pv != null)
			pv.RPC("InstantiateNetworkBullet", PhotonTargets.Others, m_bulletSocket.transform.position, m_bulletSocket.transform.rotation, m_iDamage, m_fBulletSpeed, ID);
		
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
					// take all available bullets, tell ammoManager that it is empty
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
		// this is checking to see if was dropped from an item spawner, therefore it's not in a player's hands and doesn't make any potential noise
		if (!m_ItemSpawner)
			Gizmos.DrawWireSphere(transform.position, m_fSoundRadius);
	}
}

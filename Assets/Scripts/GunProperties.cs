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
	private int m_iMaxPistol = 17;
	private int m_iMaxShotgun = 8;
	private int m_iMaxRifle = 40;
	private int m_iMaxSniper = 5;
	private bool m_bReloading = false;
	public float m_fReloadTime = 2.0f;
	private AmmoManager m_ammoManager;
	
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
		
		m_fFireRateTimer = m_fFireRate;
		
		m_ammoManager = GetComponent<AmmoManager>();
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
			if (m_ammoManager.GetMagAmmo() > 0 && !m_bReloading)
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
				
				// tell ammo manager to update the mag's ammo count
				m_ammoManager.SetMagAmmo(m_ammoManager.GetMagAmmo() - 1);
				// do gun recoil
				//MouseLook mouseLook = GetComponentInParent<MouseLook>();
				
				//This is not Network Safe
				//GameObject emitThreat = (GameObject) Instantiate(Resources.Load ("ThreatEmitter"), transform.position, transform.rotation);
				//emitThreat.GetComponent<ThreatEmitter>().SetValues(transform.position, playerID, m_fSoundRadius, m_iDamage);
			}
			else
			{
				// we're out of ammo, player automatically reloads upon trying to shoot
				// need to check again so that we don't end up starting dozens of coroutines
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
		// shotgun will need its own reload logic because players will expect to be able to load one shell at a time
		m_bReloading = true;
		if (!m_ammoManager.InvEmpty() && !m_ammoManager.MagFull())
		{
			yield return new WaitForSeconds(m_fReloadTime);
			m_bReloading = false;
			m_ammoManager.Reload();
		}
		else
			m_bReloading = false;
	}
	
	public GUNTYPE GetGunType()
	{
		return m_eGun;
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

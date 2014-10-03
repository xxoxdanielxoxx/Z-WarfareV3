using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour 
{
	public enum Item
	{
		Pistol,
		Shotgun,
		Rifle,
		Sniper,
		Medkit
	}
	
	public Item m_eItem = Item.Pistol;
	public float m_fTimerInitialDelay = 2;
	public float m_fTimer = 5;
	private float m_fInitialItemTimer;
	public bool m_spawnOnce = false;
	private bool m_bItemSpawned = false;

	// Use this for initialization
	void Start () 
	{
		m_fInitialItemTimer = m_fTimer;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_spawnOnce && m_bItemSpawned)
		{
			Destroy (gameObject);
		}
		
		if (!m_bItemSpawned)
		{
			m_fTimerInitialDelay -= Time.deltaTime;
			if (m_fTimerInitialDelay <= 0)
			{
				m_fTimerInitialDelay = 0;
				m_fTimer -= Time.deltaTime;
				if (m_fTimer <= 0)
				{
					m_fTimer = m_fInitialItemTimer;
					m_bItemSpawned = true;
					if (m_eItem != Item.Medkit) // meant for future if we have more pickups than medkits
						SpawnGun();
					else
						SpawnItem();
				}
			}
		}
	}
	
	private void SpawnGun()
	{
		GameObject newGun;
		
		if (m_eItem == Item.Pistol)
			newGun = (GameObject) Instantiate(Resources.Load ("PickupPistol"), transform.position, transform.rotation);
		else if (m_eItem == Item.Shotgun)
			newGun = (GameObject) Instantiate(Resources.Load ("PickupShotgun"), transform.position, transform.rotation);
		else if (m_eItem == Item.Rifle)
			newGun = (GameObject) Instantiate(Resources.Load ("PickupRifle"), transform.position, transform.rotation);
		else
			newGun = (GameObject) Instantiate(Resources.Load ("PickupSniper"), transform.position, transform.rotation);
		newGun.GetComponent<AmmoManager>().Start();
		newGun.GetComponent<AmmoManager>().MaxAmmo();
		newGun.GetComponent<GunProperties>().SetItemSpawner(gameObject);
	}

	private void SpawnItem()
	{
		// currently only for medkit
		GameObject newItem;
		newItem = (GameObject) Instantiate (Resources.Load ("PickupMedkit"), transform.position, transform.rotation);
		newItem.GetComponent<Medkit>().SetItemSpawner(gameObject);
	}
	
	public void RemoveSpawner()
	{
		Destroy(gameObject);
	}

	public void SetItem(Item input)
	{
		m_eItem = input;
	}
	
	public void ItemTaken()
	{
		m_bItemSpawned = false;
	}
}

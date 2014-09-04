using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour 
{
	public enum Gun
	{
		Pistol,
		Shotgun,
		Rifle,
		Sniper,
		None
	};
	
	public enum Slot
	{
		Primary,
		Secondary
	}
	public GameObject m_pistolPrefab;
	public GameObject m_shotgunPrefab;
	public GameObject m_riflePrefab;
	public GameObject m_sniperPrefab;
	public Gun m_primary = Gun.Pistol;
	public Gun m_secondary = Gun.None;
	private Gun m_prevPrimary = Gun.Pistol;
	private Gun m_prevSecondary = Gun.None;
	private Slot m_currentGun;
	Ray m_ray;
	// Use this for initialization
	void Start () 
	{
		m_currentGun = Slot.Primary;
		m_pistolPrefab.SetActive(false);
		m_shotgunPrefab.SetActive(false);
		m_riflePrefab.SetActive(false);
		m_sniperPrefab.SetActive(false);
		if (m_primary == Gun.Pistol)
		{
			m_pistolPrefab.SetActive(true);
		}
		if (m_primary == Gun.Shotgun)
		{
			m_shotgunPrefab.SetActive(true);
		}
		if (m_primary == Gun.Rifle)
		{
			m_riflePrefab.SetActive(true);
		}
		if (m_primary == Gun.Sniper)
		{
			m_sniperPrefab.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (m_primary != m_prevPrimary)
		{
			m_prevPrimary = m_primary;
			if (m_primary == Gun.Pistol)
			{
				m_pistolPrefab.SetActive(true);
				m_shotgunPrefab.SetActive(false);
				m_riflePrefab.SetActive(false);
				m_sniperPrefab.SetActive(false);
			}
			else if (m_primary == Gun.Shotgun)
			{
				m_pistolPrefab.SetActive(false);
				m_shotgunPrefab.SetActive(true);
				m_riflePrefab.SetActive(false);
				m_sniperPrefab.SetActive(false);
			}
			else if (m_primary == Gun.Rifle)
			{
				m_pistolPrefab.SetActive(false);
				m_shotgunPrefab.SetActive(false);
				m_riflePrefab.SetActive(true);
				m_sniperPrefab.SetActive(false);
			}
			else if (m_primary == Gun.Sniper)
			{
				m_pistolPrefab.SetActive(false);
				m_shotgunPrefab.SetActive(false);
				m_riflePrefab.SetActive(false);
				m_sniperPrefab.SetActive(true);
			}
		}
		if (m_secondary != m_prevSecondary)
		{
			m_prevSecondary = m_secondary;
			if (m_secondary == Gun.Pistol)
			{
				m_pistolPrefab.SetActive(true);
				m_shotgunPrefab.SetActive(false);
				m_riflePrefab.SetActive(false);
				m_sniperPrefab.SetActive(false);
			}
			else if (m_secondary == Gun.Shotgun)
			{
				m_pistolPrefab.SetActive(false);
				m_shotgunPrefab.SetActive(true);
				m_riflePrefab.SetActive(false);
				m_sniperPrefab.SetActive(false);
			}
			else if (m_secondary == Gun.Rifle)
			{
				m_pistolPrefab.SetActive(false);
				m_shotgunPrefab.SetActive(false);
				m_riflePrefab.SetActive(true);
				m_sniperPrefab.SetActive(false);
			}
			else if (m_primary == Gun.Sniper)
			{
				m_pistolPrefab.SetActive(false);
				m_shotgunPrefab.SetActive(false);
				m_riflePrefab.SetActive(false);
				m_sniperPrefab.SetActive(true);
			}
		}
		if (Input.GetKeyDown (KeyCode.Q))
		{
			SwitchWeapon();
		}
	}
	
	public void SwitchWeapon()
	{
		if (m_currentGun == Slot.Primary)
		{
			if (m_secondary != Gun.None)
			{
				m_pistolPrefab.SetActive(false);
				m_shotgunPrefab.SetActive(false);
				m_riflePrefab.SetActive(false);
				m_sniperPrefab.SetActive(false);
				
				m_currentGun = Slot.Secondary;
				
				if (m_secondary == Gun.Pistol)
				{
					m_pistolPrefab.SetActive(true);
				}
				else if (m_secondary == Gun.Shotgun)
				{
					m_shotgunPrefab.SetActive(true);
				}
				else if (m_secondary == Gun.Rifle)
				{
					m_riflePrefab.SetActive(true);
				}
				else if (m_primary == Gun.Sniper)
				{
					m_sniperPrefab.SetActive(true);
				}
			}
		}
		else
		{
			m_pistolPrefab.SetActive(false);
			m_shotgunPrefab.SetActive(false);
			m_riflePrefab.SetActive(false);
			m_sniperPrefab.SetActive(false);
			
			m_currentGun = Slot.Primary;
			
			if (m_primary == Gun.Pistol)
			{
				m_pistolPrefab.SetActive(true);
			}
			else if (m_primary == Gun.Shotgun)
			{
				m_shotgunPrefab.SetActive(true);
			}
			else if (m_primary == Gun.Rifle)
			{
				m_riflePrefab.SetActive(true);
			}
			else if (m_primary == Gun.Sniper)
			{
				m_sniperPrefab.SetActive(true);
			}
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "WeaponSpawner")
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				Camera m_mainCamera = GetComponentInChildren<Camera>();
				m_ray = m_mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
				RaycastHit m_rayHit = new RaycastHit();
				Physics.Raycast (m_ray, out m_rayHit, 100);
				if (m_rayHit.collider.tag == "Pistol" || m_rayHit.collider.tag == "Shotgun" || m_rayHit.collider.tag == "Rifle" || m_rayHit.collider.tag == "Sniper")
				{
					m_pistolPrefab.SetActive (false);
					m_shotgunPrefab.SetActive(false);
					m_riflePrefab.SetActive(false);
					m_sniperPrefab.SetActive(false);
					
					if (m_rayHit.collider.tag == "Pistol")
					{
						m_pistolPrefab.SetActive(true);
						
						if (m_currentGun == Slot.Primary && m_secondary == Gun.None)
						{
							other.GetComponent<WeaponSpawner>().RemoveSpawner();
							m_currentGun = Slot.Secondary;
							m_secondary = Gun.Pistol;
						}
						else if (m_currentGun == Slot.Primary)
						{
							if (m_primary != Gun.Pistol)
							{
								if (m_primary == Gun.Shotgun)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Shotgun);
								}
								else if (m_primary == Gun.Rifle)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Rifle);
								}
								else if (m_primary == Gun.Sniper)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Sniper);
								}
								
								m_primary = Gun.Pistol;
							}
						}
						else
						{
							if (m_secondary != Gun.Pistol)
							{
								if (m_secondary == Gun.Shotgun)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Shotgun);
								}
								else if (m_secondary == Gun.Rifle)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Rifle);
								}
								else if (m_secondary == Gun.Sniper)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Sniper);
								}
								
								m_secondary = Gun.Pistol;
							}
						}
					}
					if (m_rayHit.collider.tag == "Shotgun")
					{
						m_shotgunPrefab.SetActive(true);
						
						if (m_currentGun == Slot.Primary && m_secondary == Gun.None)
						{
							other.GetComponent<WeaponSpawner>().RemoveSpawner();
							m_currentGun = Slot.Secondary;
							m_secondary = Gun.Shotgun;
						}
						else if (m_currentGun == Slot.Primary)
						{
							if (m_primary != Gun.Shotgun)
							{
								if (m_primary == Gun.Pistol)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Pistol);
								}
								else if (m_primary == Gun.Rifle)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Rifle);
								}
								else if (m_primary == Gun.Sniper)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Sniper);
								}
								
								m_primary = Gun.Shotgun;
							}
						}
						else
						{
							if (m_secondary != Gun.Shotgun)
							{
								if (m_secondary == Gun.Pistol)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Pistol);
								}
								else if (m_secondary == Gun.Rifle)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Rifle);
								}
								else if (m_secondary == Gun.Sniper)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Sniper);
								}
								
								m_secondary = Gun.Shotgun;
							}
						}
					}
					if (m_rayHit.collider.tag == "Rifle")
					{
						m_riflePrefab.SetActive(true);
						
						if (m_currentGun == Slot.Primary && m_secondary == Gun.None)
						{
							other.GetComponent<WeaponSpawner>().RemoveSpawner();
							m_currentGun = Slot.Secondary;
							m_secondary = Gun.Rifle;
						}
						else if (m_currentGun == Slot.Primary)
						{
							if (m_primary != Gun.Rifle)
							{
								if (m_primary == Gun.Pistol)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Pistol);
								}
								else if (m_primary == Gun.Shotgun)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Shotgun);
								}
								else if (m_primary == Gun.Sniper)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Sniper);
								}
								
								m_primary = Gun.Rifle;
							}
						}
						else
						{
							if (m_secondary != Gun.Pistol)
							{
								if (m_secondary == Gun.Pistol)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Pistol);
								}
								else if (m_secondary == Gun.Shotgun)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Shotgun);
								}
								else if (m_secondary == Gun.Sniper)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Sniper);
								}
								
								m_secondary = Gun.Rifle;
							}
						}
					}
					if (m_rayHit.collider.tag == "Sniper")
					{
						m_sniperPrefab.SetActive(true);
						
						if (m_currentGun == Slot.Primary && m_secondary == Gun.None)
						{
							other.GetComponent<WeaponSpawner>().RemoveSpawner();
							m_currentGun = Slot.Secondary;
							m_secondary = Gun.Sniper;
						}
						else if (m_currentGun == Slot.Primary)
						{
							if (m_primary != Gun.Sniper)
							{
								if (m_primary == Gun.Shotgun)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Shotgun);
								}
								else if (m_primary == Gun.Rifle)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Rifle);
								}
								else if (m_primary == Gun.Pistol)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Pistol);
								}
								
								m_primary = Gun.Sniper;
							}
						}
						else
						{
							if (m_secondary != Gun.Sniper)
							{
								if (m_secondary == Gun.Pistol)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Pistol);
								}
								else if (m_secondary == Gun.Shotgun)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Shotgun);
								}
								else if (m_secondary == Gun.Rifle)
								{
									other.GetComponent<WeaponSpawner>().ChangeWeapon(WeaponSpawner.Gun.Rifle);
								}
								
								m_secondary = Gun.Sniper;
							}
						}
					}
				}
				
			}
		}
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(m_ray);
	}
}

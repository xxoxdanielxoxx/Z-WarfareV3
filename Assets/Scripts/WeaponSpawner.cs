using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class WeaponSpawner : MonoBehaviour 
{
	public enum Gun
	{
		Pistol,
		Shotgun,
		Rifle,
		Sniper
	}

	public GameObject m_pistolPrefab;
	public GameObject m_shotgunPrefab;
	public GameObject m_riflePrefab;
	public GameObject m_sniperPrefab;
	private Gun m_prevGun = Gun.Pistol;
	public Gun m_gun = Gun.Pistol;
	
	// Use this for initialization
	void Start () 
	{
		m_pistolPrefab.SetActive(true);
		m_shotgunPrefab.SetActive(false);
		m_riflePrefab.SetActive(false);
		m_sniperPrefab.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_gun != m_prevGun)
		{
			if (m_gun == Gun.Pistol)
			{
				m_prevGun = Gun.Pistol;
				m_pistolPrefab.SetActive(true);
				m_shotgunPrefab.SetActive(false);
				m_riflePrefab.SetActive(false);
				m_sniperPrefab.SetActive(false);
			}
			else if (m_gun == Gun.Shotgun)
			{
				m_prevGun = Gun.Shotgun;
				m_pistolPrefab.SetActive(false);
				m_shotgunPrefab.SetActive(true);
				m_riflePrefab.SetActive(false);
				m_sniperPrefab.SetActive(false);
			}
			else if (m_gun == Gun.Rifle)
			{
				m_prevGun = Gun.Rifle;
				m_pistolPrefab.SetActive(false);
				m_shotgunPrefab.SetActive(false);
				m_riflePrefab.SetActive(true);
				m_sniperPrefab.SetActive(false);
			}
			else if (m_gun == Gun.Sniper)
			{
				m_prevGun = Gun.Sniper;
				m_pistolPrefab.SetActive(false);
				m_shotgunPrefab.SetActive(false);
				m_riflePrefab.SetActive(false);
				m_sniperPrefab.SetActive(true);
			}
		}
	}
	
	public void ChangeWeapon(Gun input)
	{
		m_gun = input;
	}
	
	public void RemoveSpawner()
	{
		Destroy(gameObject);
	}
}

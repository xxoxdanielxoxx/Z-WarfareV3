using UnityEngine;
using System.Collections;

public class AmmoManager : MonoBehaviour 
{
	private int m_iMaxAmmoPistol = 100;
	private int m_iMaxAmmoShotgun = 100;
	private int m_iMaxAmmoRifle = 100;
	private int m_iMaxAmmoSniper = 100;
	
	public int m_iAmmo = 0;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (GetComponent<GunProperties>().GetWeaponSpawner() &&
			m_iAmmo <= 0)
			GetComponent<GunProperties>().GunPickupTaken();
	}
	
	public void SetAmmo(int input)
	{
		m_iAmmo = input;
	}
	
	public int GetAmmo()
	{
		return m_iAmmo;
	}
	
	public void MaxAmmo()
	{
		if (tag == "Pistol")
			m_iAmmo = m_iMaxAmmoPistol;
		else if (tag == "Shotgun")
			m_iAmmo = m_iMaxAmmoShotgun;
		else if (tag == "Rifle")
			m_iAmmo = m_iMaxAmmoRifle;
		else
			m_iAmmo = m_iMaxAmmoSniper;
	}
	
	public void FillUpAmmo(AmmoManager otherAmmo)
	{
		int m_iAmmoNeeded;
		if (tag == "Pistol" && otherAmmo.tag == "Pistol")
		{
			m_iAmmoNeeded = m_iMaxAmmoPistol - m_iAmmo;
			
			if (otherAmmo.GetAmmo() >= m_iAmmoNeeded)
			{
				m_iAmmo += m_iAmmoNeeded;
				otherAmmo.SetAmmo(otherAmmo.GetAmmo() - m_iAmmoNeeded);
			}
			else
			{
				m_iAmmo += otherAmmo.GetAmmo();
				otherAmmo.SetAmmo(0);
			}
		}
		else if (tag == "Shotgun" && otherAmmo.tag == "Shotgun")
		{
			m_iAmmoNeeded = m_iMaxAmmoRifle - m_iAmmo;
			
			if (otherAmmo.GetAmmo() >= m_iAmmoNeeded)
			{
				m_iAmmo += m_iAmmoNeeded;
				otherAmmo.SetAmmo(otherAmmo.GetAmmo() - m_iAmmoNeeded);
			}
			else
			{
				m_iAmmo += otherAmmo.GetAmmo();
				otherAmmo.SetAmmo(0);
			}
		}
		else if (tag == "Rifle" && otherAmmo.tag == "Rifle")
		{
			m_iAmmoNeeded = m_iMaxAmmoRifle - m_iAmmo;
			
			if (otherAmmo.GetAmmo() >= m_iAmmoNeeded)
			{
				m_iAmmo += m_iAmmoNeeded;
				otherAmmo.SetAmmo(otherAmmo.GetAmmo() - m_iAmmoNeeded);
			}
			else
			{
				m_iAmmo += otherAmmo.GetAmmo();
				otherAmmo.SetAmmo(0);
			}
		}
		else if (tag == "Sniper" && otherAmmo.tag == "Sniper")
		{
			m_iAmmoNeeded = m_iMaxAmmoSniper - m_iAmmo;
			
			if (otherAmmo.GetAmmo() >= m_iAmmoNeeded)
			{
				m_iAmmo += m_iAmmoNeeded;
				otherAmmo.SetAmmo(otherAmmo.GetAmmo() - m_iAmmoNeeded);
			}
			else
			{
				m_iAmmo += otherAmmo.GetAmmo();
				otherAmmo.SetAmmo(0);
			}
		}
	}
}

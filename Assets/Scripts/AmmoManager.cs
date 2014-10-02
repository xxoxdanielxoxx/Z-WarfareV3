using UnityEngine;
using System.Collections;

public class AmmoManager : MonoBehaviour 
{
	private int m_iMaxAmmoPistol = 100;
	private int m_iMaxAmmoShotgun = 100;
	private int m_iMaxAmmoRifle = 100;
	private int m_iMaxAmmoSniper = 100;
	public int m_iInvAmmo = 0;
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (GetComponent<GunProperties>().GetItemSpawner() && m_iInvAmmo <= 0)
			GetComponent<GunProperties>().GunPickupTaken();
	}
	
	public void SetAmmo(int input)
	{  
		m_iInvAmmo = input;
	}
	
	public int GetAmmo()
	{
		return m_iInvAmmo;
	}
	
	public void MaxAmmo()
	{
		if (tag == "Pistol")
			m_iInvAmmo = m_iMaxAmmoPistol;
		else if (tag == "Shotgun")
			m_iInvAmmo = m_iMaxAmmoShotgun;
		else if (tag == "Rifle")
			m_iInvAmmo = m_iMaxAmmoRifle;
		else
			m_iInvAmmo = m_iMaxAmmoSniper;
	}
	
	public void FillUpAmmo(AmmoManager otherAmmo)
	{
		int ammoNeeded;
		if (tag == "Pistol" && otherAmmo.tag == "Pistol")
		{
			ammoNeeded = m_iMaxAmmoPistol - m_iInvAmmo;
			
			if (otherAmmo.GetAmmo() >= ammoNeeded)
			{
				m_iInvAmmo += ammoNeeded;
				otherAmmo.SetAmmo(otherAmmo.GetAmmo() - ammoNeeded);
			}
			else
			{
				// grab all the available bullets from the pickup, tell the pickup it's empty
				m_iInvAmmo += otherAmmo.GetAmmo();
				otherAmmo.SetAmmo(0);
			}
		}
		else if (tag == "Shotgun" && otherAmmo.tag == "Shotgun")
		{
			ammoNeeded = m_iMaxAmmoRifle - m_iInvAmmo;
			
			if (otherAmmo.GetAmmo() >= ammoNeeded)
			{
				m_iInvAmmo += ammoNeeded;
				otherAmmo.SetAmmo(otherAmmo.GetAmmo() - ammoNeeded);
			}
			else
			{
				m_iInvAmmo += otherAmmo.GetAmmo();
				// grab all the available bullets from the pickup, tell the pickup it's empty
				otherAmmo.SetAmmo(0);
			}
		}
		else if (tag == "Rifle" && otherAmmo.tag == "Rifle")
		{
			ammoNeeded = m_iMaxAmmoRifle - m_iInvAmmo;
			
			if (otherAmmo.GetAmmo() >= ammoNeeded)
			{
				m_iInvAmmo += ammoNeeded;
				otherAmmo.SetAmmo(otherAmmo.GetAmmo() - ammoNeeded);
			}
			else
			{
				m_iInvAmmo += otherAmmo.GetAmmo();
				// grab all the available bullets from the pickup, tell the pickup it's empty
				otherAmmo.SetAmmo(0);
			}
		}
		else if (tag == "Sniper" && otherAmmo.tag == "Sniper")
		{
			ammoNeeded = m_iMaxAmmoSniper - m_iInvAmmo;
			
			if (otherAmmo.GetAmmo() >= ammoNeeded)
			{
				m_iInvAmmo += ammoNeeded;
				otherAmmo.SetAmmo(otherAmmo.GetAmmo() - ammoNeeded);
			}
			else
			{
				m_iInvAmmo += otherAmmo.GetAmmo();
				// grab all the available bullets from the pickup, tell the pickup it's empty
				otherAmmo.SetAmmo(0);
			}
		}
	}
}

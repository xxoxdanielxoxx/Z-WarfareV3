using UnityEngine;
using System.Collections;

public class AmmoManager : MonoBehaviour 
{
	// list of ammo counts
	private int m_iMaxAmmoPistol = 17;
	private int m_iMaxAmmoShotgun = 8;
	private int m_iMaxAmmoRifle = 40;
	private int m_iMaxAmmoSniper = 5;
	private int m_iMaxInvAmmoPistol = 80;
	private int m_iMaxInvAmmoShotgun = 30;
	private int m_iMaxInvAmmoRifle = 100;
	private int m_iMaxInvAmmoSniper = 20;
	
	public int m_iMagAmmo = 0;
	public int m_iInvAmmo = 0;
	
	private int m_iMagSize = 0;
	private int m_iInvSize = 0;
	
	// Use this for initialization
	public void Start () 
	{
		// initializing what ammo counts this gun should use
		// technically shotguns don't have mags, but whatever, still fits the analogy
		if (gameObject.tag == "Pistol")
		{
			m_iMagSize = m_iMaxAmmoPistol;
			m_iInvSize = m_iMaxInvAmmoPistol;
		}
		else if (tag == "Shotgun")
		{
			m_iMagSize = m_iMaxAmmoShotgun;
			m_iInvSize = m_iMaxInvAmmoShotgun;
		}
		else if (tag == "Rifle")
		{
			m_iMagSize = m_iMaxAmmoRifle;
			m_iInvSize = m_iMaxInvAmmoRifle;
		}
		else
		{
			m_iMagSize = m_iMaxAmmoSniper;
			m_iInvSize = m_iMaxInvAmmoSniper;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (GetComponent<GunProperties>().GetItemSpawner() && m_iInvAmmo <= 0 && m_iMagAmmo <= 0)
			GetComponent<GunProperties>().GunPickupTaken();
	}
	
	public void Reload()
	{		
		int ammoNeeded = m_iMagSize - m_iMagAmmo;
		
		if (m_iInvAmmo >= m_iMagSize)
		{
			m_iMagAmmo += ammoNeeded;
			m_iInvAmmo -= ammoNeeded;
		}
		else
		{
			// take all the bullets from the inventory
			m_iMagAmmo = m_iInvAmmo;
			m_iInvAmmo = 0;
		}
	}
	
	public void SetMagAmmo(int input)
	{
		m_iMagAmmo = input;
	}
	
	public int GetMagAmmo()
	{
		return m_iMagAmmo;
	}
	
	public void SetInvAmmo(int input)
	{  
		m_iInvAmmo = input;
	}
	
	public int GetInvAmmo()
	{
		return m_iInvAmmo;
	}
	
	public void MaxAmmo()
	{
		m_iMagAmmo = m_iMagSize;
		m_iInvAmmo = m_iInvSize;
	}
	
	public bool InvEmpty()
	{
		if (m_iInvAmmo > 0)
			return false;
		else
			return true;
	}
	
	public bool MagFull()
	{
		if (m_iMagAmmo >= m_iMagSize)
			return true;
		else
			return false;
	}
	
	public void FillUpAmmo(AmmoManager otherAmmo)
	{
		int ammoNeeded;
		if (tag == "Pistol" && otherAmmo.tag == "Pistol")
			ammoNeeded = m_iMaxInvAmmoPistol - m_iInvAmmo;
		else if (tag == "Shotgun" && otherAmmo.tag == "Shotgun")
			ammoNeeded = m_iMaxInvAmmoRifle - m_iInvAmmo;
		else if (tag == "Rifle" && otherAmmo.tag == "Rifle")
			ammoNeeded = m_iMaxInvAmmoRifle - m_iInvAmmo;
		else
			ammoNeeded = m_iMaxInvAmmoSniper - m_iInvAmmo;
		
		if (otherAmmo.GetInvAmmo() >= ammoNeeded)
		{
			m_iInvAmmo += ammoNeeded;
			otherAmmo.SetInvAmmo(otherAmmo.GetInvAmmo() - ammoNeeded);
		}
		else
		{
			int moreAmmo = ammoNeeded - otherAmmo.GetInvAmmo();
			m_iInvAmmo += otherAmmo.GetInvAmmo();
			otherAmmo.SetInvAmmo(0);
			if (otherAmmo.GetMagAmmo() >= moreAmmo)
			{
				m_iInvAmmo += moreAmmo;
				otherAmmo.SetMagAmmo(otherAmmo.GetMagAmmo() - moreAmmo);
			}
			else
			{
				m_iMagAmmo += otherAmmo.GetMagAmmo();
				otherAmmo.SetMagAmmo(0);
			}
		}
	}
	
	public int DepleteAmmo(GUNTYPE inputGUNTYPE, int inputAmmo)
	{
		// used for WeaponManager, for picking up ammo for inventory weapon
		// returns the number of bullets that the inventory weapon will receive
		int ammoNeeded;
		if (tag == "Pistol" && inputGUNTYPE == GUNTYPE.Pistol)
			ammoNeeded = m_iMaxInvAmmoPistol - inputAmmo;
		else if (tag == "Shotgun" && inputGUNTYPE == GUNTYPE.Shotgun)
			ammoNeeded = m_iMaxInvAmmoShotgun - inputAmmo;
		else if (tag == "Rifle" && inputGUNTYPE == GUNTYPE.Rifle)
			ammoNeeded = m_iMaxInvAmmoRifle - inputAmmo;
		else
			ammoNeeded = m_iMaxAmmoSniper - inputAmmo;
			
		if (m_iInvAmmo >= ammoNeeded)
		{
			m_iInvAmmo -= ammoNeeded;
			return ammoNeeded;
		}
		else
		{
			int moreAmmo = ammoNeeded - m_iInvAmmo;
			int totalAmmoGiven = 0;
			totalAmmoGiven += m_iInvAmmo;
			m_iInvAmmo = 0;
			if (m_iMagAmmo >= moreAmmo)
			{
				m_iMagAmmo -= moreAmmo;
				totalAmmoGiven += moreAmmo;
			}
			else
			{
				totalAmmoGiven += m_iMagAmmo;
				m_iMagAmmo = 0;
			}
			return totalAmmoGiven;
		}
	}
}

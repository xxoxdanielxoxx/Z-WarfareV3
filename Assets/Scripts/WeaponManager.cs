using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour 
{	
	public enum Slot
	{
		Gun1,
		Gun2
	}
	
	private GUNTYPE m_ePrimary = GUNTYPE.Pistol;
	private GUNTYPE m_eSecondary = GUNTYPE.None;
	private Slot m_currentGun;
	private GameObject m_heldGun;
	private bool m_bCanPickUp = false;
	private List<Collider> m_availablePickups;
	private float m_fHoldETimer = 0.5f;
	private float m_fInitialHoldETimer = 0.5f;
	private bool m_bInPickupZone = false;
	
	private int m_iPrimaryAmmo;
	private int m_iPrimaryReserveAmmo;
	private int m_iSecondaryAmmo;
	private int m_iSecondaryReserveAmmo;
	
	// Use this for initialization
	void Start () 
	{
		m_availablePickups = new List<Collider>();
		
		m_currentGun = Slot.Gun1;
		FirstGun(m_ePrimary);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Q))
			SwitchWeapon();
		if (Input.GetKey (KeyCode.E) && m_bInPickupZone)
		{
			m_fHoldETimer -= Time.deltaTime;
			if (m_fHoldETimer <= 0)
			{
				m_bCanPickUp = true;
				m_fHoldETimer = m_fInitialHoldETimer;
			}
		}
		else
		{
			m_fHoldETimer = m_fInitialHoldETimer;
		}
	}
	
	public void SwitchWeapon()
	{
		if (m_currentGun == Slot.Gun1)
		{
			if (m_eSecondary != GUNTYPE.None)
			{
				m_currentGun = Slot.Gun2;
				SwitchGun();
			}
		}
		else
		{
			m_currentGun = Slot.Gun1;
			SwitchGun();
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Pickup")
		{
			m_bInPickupZone = true;
			HandlePickup();
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Pickup")
		{
			m_bInPickupZone = false;
		}
	}
	
	private void HandlePickup()
	{
		GetAmmo();
		
		if (m_bCanPickUp)
		{
			m_bCanPickUp = false;
			Transform closestGun = FindClosestGun();
			if (closestGun)
			{
				GUNTYPE tempGun = closestGun.GetComponent<GunProperties>().GetGunType();
				GUNTYPE otherGun;
				if (tempGun == GUNTYPE.Pistol)
					otherGun = GUNTYPE.Pistol;
				else if (tempGun == GUNTYPE.Shotgun)
					otherGun = GUNTYPE.Shotgun;
				else if (tempGun == GUNTYPE.Rifle)
					otherGun = GUNTYPE.Rifle;
				else
					otherGun = GUNTYPE.Sniper;
				
				if (m_currentGun == Slot.Gun1)
				{
					if (m_eSecondary == GUNTYPE.None)
					{
						// we aren't carrying a secondary weapon, so switch to second gun, pick it up
						m_currentGun = Slot.Gun2;
						m_eSecondary = otherGun;
						PickUpGunSpecial(closestGun.gameObject);
					}
					else
					{
						// we're going to swap the gun in our hands with the gun on the ground
						DropGun(m_ePrimary);
						m_ePrimary = otherGun;
						PickUpGun(closestGun.gameObject);
					}
				}
				else
				{
					DropGun(m_eSecondary);
					m_eSecondary = otherGun;
					PickUpGun(closestGun.gameObject);
				}
				// tell the weapon spawner that we've taken the gun
				closestGun.GetComponent<GunProperties>().GunPickupTaken();
			}
		}
	}
	
	// NOTE: This function is deprecated, does not do intendend behavior anymore. Use SetRemoteWeaponSwitch() instead.
	//public void SetRemoteWeaponSwitch(Slot input)
	//{
	//	m_currentGun = input;
	//}
	
	public void SetRemoteWeaponSwitch()
	{
		SwitchWeapon();
	}
	
	private GameObject InstantiateGun(GUNTYPE input)
	{
		GameObject newGun;
		if (input == GUNTYPE.Pistol)
			newGun = (GameObject)Instantiate(Resources.Load ("Pistol"));
		else if (input == GUNTYPE.Shotgun)
			newGun = (GameObject)Instantiate(Resources.Load ("Shotgun"));
		else if (input == GUNTYPE.Rifle)
			newGun = (GameObject)Instantiate(Resources.Load ("Rifle"));
		else
			newGun = (GameObject)Instantiate(Resources.Load ("Sniper"));
		return newGun;
	}
	
	private GameObject InstantiatePickup(GUNTYPE input)
	{
		GameObject newGun;
		if (input == GUNTYPE.Pistol)
			newGun = (GameObject) Instantiate(Resources.Load("PickupPistol"), transform.position, transform.rotation);
		else if (input == GUNTYPE.Shotgun)
			newGun = (GameObject) Instantiate(Resources.Load ("PickupShotgun"), transform.position, transform.rotation);
		else if (input == GUNTYPE.Rifle)
			newGun = (GameObject) Instantiate(Resources.Load ("PickupRifle"), transform.position, transform.rotation);
		else
			newGun = (GameObject) Instantiate(Resources.Load ("PickupSniper"), transform.position, transform.rotation);
		return newGun;
	}
	
	private void FirstGun(GUNTYPE input)
	{
		m_heldGun = InstantiateGun(input);
		m_heldGun.GetComponent<AmmoManager>().Start();
		m_heldGun.GetComponent<AmmoManager>().MaxAmmo();
		m_heldGun.transform.parent = gameObject.transform.FindChild("Main Camera/Gun");
		m_heldGun.transform.localPosition = Vector3.zero;
		m_heldGun.transform.localEulerAngles = Vector3.zero;
		m_heldGun.GetComponentInChildren<GunProperties>().SetBulletSocket(m_heldGun.transform.parent.transform.Find("BulletSocket").gameObject);
	}
	
	private void DropGun(GUNTYPE input)
	{
		GameObject droppedGun;
		droppedGun = InstantiatePickup(input);
		droppedGun.GetComponent<AmmoManager>().SetMagAmmo(m_heldGun.GetComponent<AmmoManager>().GetMagAmmo());
		droppedGun.GetComponent<AmmoManager>().SetInvAmmo(m_heldGun.GetComponent<AmmoManager>().GetInvAmmo());
	}
	
	private void PickUpGun(GameObject input)
	{
		// we need to replace the values for the current gun with this gun
		if (m_currentGun == Slot.Gun1)
		{
			Destroy (m_heldGun);
			m_heldGun = InstantiateGun(m_ePrimary);
		}
		else
		{
			Destroy (m_heldGun);
			m_heldGun = InstantiateGun(m_eSecondary);
		}
		m_heldGun.GetComponent<AmmoManager>().SetMagAmmo(input.GetComponent<AmmoManager>().GetMagAmmo());
		m_heldGun.GetComponent<AmmoManager>().SetInvAmmo(input.GetComponent<AmmoManager>().GetInvAmmo());
		m_heldGun.transform.parent = gameObject.transform.FindChild("Main Camera/Gun");
		m_heldGun.transform.localPosition = Vector3.zero;
		m_heldGun.transform.localEulerAngles = Vector3.zero;
		m_heldGun.GetComponentInChildren<GunProperties>().SetBulletSocket(m_heldGun.transform.parent.transform.Find("BulletSocket").gameObject);
	}
	
	private void PickUpGunSpecial(GameObject input)
	{
		// this is a special case just for when the player doesn't have two weapons yet, so he must only have a primary
		m_iPrimaryAmmo = m_heldGun.GetComponent<AmmoManager>().GetMagAmmo();
		m_iPrimaryReserveAmmo = m_heldGun.GetComponent<AmmoManager>().GetInvAmmo();
		
		Destroy (m_heldGun);
		
		m_heldGun = InstantiateGun(m_eSecondary);
		m_heldGun.GetComponent<AmmoManager>().SetMagAmmo(input.GetComponent<AmmoManager>().GetMagAmmo());
		m_heldGun.GetComponent<AmmoManager>().SetInvAmmo(input.GetComponent<AmmoManager>().GetInvAmmo());
		m_heldGun.transform.parent = gameObject.transform.FindChild("Main Camera/Gun");
		m_heldGun.transform.localPosition = Vector3.zero;
		m_heldGun.transform.localEulerAngles = Vector3.zero;
		m_heldGun.GetComponentInChildren<GunProperties>().SetBulletSocket(m_heldGun.transform.parent.transform.Find("BulletSocket").gameObject);
	}
	
	private void SwitchGun()
	{
		// this function is designed for switching between the two guns on the character, all info is known on the object
		if (m_currentGun == Slot.Gun1)
		{
			m_iSecondaryAmmo = m_heldGun.GetComponent<AmmoManager>().GetMagAmmo();
			m_iSecondaryReserveAmmo = m_heldGun.GetComponent<AmmoManager>().GetInvAmmo();
		}
		else
		{
			m_iPrimaryAmmo = m_heldGun.GetComponent<AmmoManager>().GetMagAmmo();
			m_iPrimaryReserveAmmo = m_heldGun.GetComponent<AmmoManager>().GetInvAmmo();
		}
		
		Destroy (m_heldGun);
		
		if (m_currentGun == Slot.Gun1)
		{
			m_heldGun = InstantiateGun(m_ePrimary);
			m_heldGun.GetComponent<AmmoManager>().SetMagAmmo(m_iPrimaryAmmo);
			m_heldGun.GetComponent<AmmoManager>().SetInvAmmo(m_iPrimaryReserveAmmo);
		}
		else
		{
			m_heldGun = InstantiateGun(m_eSecondary);
			m_heldGun.GetComponent<AmmoManager>().SetMagAmmo(m_iSecondaryAmmo);
			m_heldGun.GetComponent<AmmoManager>().SetInvAmmo(m_iSecondaryReserveAmmo);
		}
		m_heldGun.transform.parent = gameObject.transform.FindChild("Main Camera/Gun");
		m_heldGun.transform.localPosition = Vector3.zero;
		m_heldGun.transform.localEulerAngles = Vector3.zero;
		m_heldGun.GetComponentInChildren<GunProperties>().SetBulletSocket(m_heldGun.transform.parent.transform.Find("BulletSocket").gameObject);
	}
	
	private Transform FindClosestGun()
	{
		// this function is for finding the closest valid gun to pick up
		// we need to do two things: get a list of closeby guns that ARE NOT currently in the player inventory, then find out which gun in the list is closest
		Collider[] pickupColliders = Physics.OverlapSphere(transform.position, 3.2f, LayerMask.GetMask("PickupLayer"));
		// 3.2 is a size based on the current size of the Pickup colliders as of 9-4-14
		m_availablePickups.Clear();
		GUNTYPE otherGun;
		foreach (Collider weapon in pickupColliders)
		{
			GUNTYPE tempGun = weapon.GetComponentInParent<GunProperties>().GetGunType();
			if (tempGun == GUNTYPE.Pistol)
				otherGun = GUNTYPE.Pistol;
			else if (tempGun == GUNTYPE.Shotgun)
				otherGun = GUNTYPE.Shotgun;
			else if (tempGun == GUNTYPE.Rifle)
				otherGun = GUNTYPE.Rifle;
			else
				otherGun = GUNTYPE.Sniper;
			
			if (otherGun != m_ePrimary && otherGun != m_eSecondary)
			{
				// this is a gun that is not in our inventory, so it can potentially be picked up, add it to the list
				m_availablePickups.Add(weapon);
			}
		}	
		
		if (m_availablePickups.Count > 0)
		{
			// with this list, we need to find out which gun is the closest to the player
			Collider closestPickup = m_availablePickups[0];
			float m_fShortestDistance = Vector3.Distance(transform.position, closestPickup.transform.position);
			
			foreach (Collider weapon in m_availablePickups)
			{
				if (Vector3.Distance(transform.position, weapon.transform.position) < m_fShortestDistance)
					closestPickup = weapon;
			}
			return closestPickup.transform.parent;
		}
		return null;
	}
	
	private void GetAmmo()
	{
		Collider[] pickupColliders = Physics.OverlapSphere(transform.position, 1.5f, LayerMask.GetMask("PickupLayer"));
		GUNTYPE otherGun;
		foreach (Collider otherWeapon in pickupColliders)
		{
			// this is checking to see if there is indeed a GunProperties script on the pickup object
			if (otherWeapon.GetComponentInParent<GunProperties>())
			{
				GUNTYPE tempGun = otherWeapon.GetComponentInParent<GunProperties>().GetGunType();
				if (tempGun == GUNTYPE.Pistol)
					otherGun = GUNTYPE.Pistol;
				else if (tempGun == GUNTYPE.Shotgun)
					otherGun = GUNTYPE.Shotgun;
				else if (tempGun == GUNTYPE.Rifle)
					otherGun = GUNTYPE.Rifle;
				else
					otherGun = GUNTYPE.Sniper;
				
				if ((m_currentGun == Slot.Gun1 && otherGun == m_ePrimary) || (m_currentGun == Slot.Gun2 && otherGun == m_eSecondary))
				{
					// the player is currently already carring that weapon and has it out, get ammo from the pickup
					GetComponentInChildren<AmmoManager>().FillUpAmmo(otherWeapon.transform.parent.GetComponentInParent<AmmoManager>());
				}
				else if ((m_currentGun == Slot.Gun1 && otherGun == m_eSecondary) || (m_currentGun == Slot.Gun2 && otherGun == m_ePrimary))
				{
					// not very elegant way to do it, but works
					// the player has that gun in their inventory, so get ammo for it 
					int moreBullets;
					if (m_currentGun == Slot.Gun1)
					{
						// we know we're carrying our primary, so get ammo for secondary
						moreBullets = otherWeapon.GetComponentInParent<AmmoManager>().DepleteAmmo(m_eSecondary, m_iSecondaryReserveAmmo);
						m_iSecondaryReserveAmmo += moreBullets;
					}
					else
					{	
						// we know we're carrying our secondary, get ammo for primary
						moreBullets = otherWeapon.GetComponentInParent<AmmoManager>().DepleteAmmo(m_ePrimary, m_iPrimaryReserveAmmo);
						Debug.Log ("moreBullets = " + moreBullets);
						m_iPrimaryReserveAmmo += moreBullets;
						Debug.Log ("m_iPrimaryReserveAmmo = " + m_iPrimaryReserveAmmo);
					}
				}
				         
			}
		}
	}
	
	public void SetGunActive(bool input)
	{
		m_heldGun.SetActive(input);
	}
}

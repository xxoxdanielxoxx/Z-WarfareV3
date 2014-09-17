using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		Gun1,
		Gun2
	}
	
	private Gun m_ePrimary = Gun.Pistol;
	private Gun m_eSecondary = Gun.None;
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
			if (m_eSecondary != Gun.None)
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
				GunProperties.Gun m_eTempGun = closestGun.GetComponent<GunProperties>().GetGunType();
				Gun otherGun;
				if (m_eTempGun == GunProperties.Gun.Pistol)
					otherGun = Gun.Pistol;
				else if (m_eTempGun == GunProperties.Gun.Shotgun)
					otherGun = Gun.Shotgun;
				else if (m_eTempGun == GunProperties.Gun.Rifle)
					otherGun = Gun.Rifle;
				else
					otherGun = Gun.Sniper;
				
				if (m_currentGun == Slot.Gun1)
				{
					if (m_eSecondary == Gun.None)
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
	public void SetRemoteWeaponSwitch(Slot input)
	{
		m_currentGun = input;
	}
	
	public void SetRemoteWeaponSwitch()
	{
		SwitchWeapon();
	}
	
	private void FirstGun(Gun input)
	{
		if (input == Gun.Pistol)
			m_heldGun = (GameObject)Instantiate(Resources.Load ("Pistol"));
		else if (input == Gun.Shotgun)
			m_heldGun = (GameObject)Instantiate(Resources.Load ("Shotgun"));
		else if (input == Gun.Rifle)
			m_heldGun = (GameObject)Instantiate(Resources.Load ("Rifle"));
		else
			m_heldGun = (GameObject)Instantiate(Resources.Load ("Sniper"));
			
		m_heldGun.GetComponent<GunProperties>().MaxAmmo();
		m_heldGun.GetComponent<AmmoManager>().MaxAmmo();
		m_heldGun.transform.parent = gameObject.transform.FindChild("Main Camera/Gun");
		m_heldGun.transform.localPosition = Vector3.zero;
		m_heldGun.transform.localEulerAngles = Vector3.zero;
		m_heldGun.GetComponentInChildren<GunProperties>().SetBulletSocket(m_heldGun.transform.parent.transform.Find("BulletSocket").gameObject);
	}
	
	private void DropGun(Gun input)
	{
		GameObject droppedGun;
		if (input == Gun.Pistol)
			droppedGun = (GameObject) Instantiate(Resources.Load("PickupPistol"), transform.position, transform.rotation);
		else if (input == Gun.Shotgun)
			droppedGun = (GameObject) Instantiate(Resources.Load ("PickupShotgun"), transform.position, transform.rotation);
		else if (input == Gun.Rifle)
			droppedGun = (GameObject) Instantiate(Resources.Load ("PickupRifle"), transform.position, transform.rotation);
		else if (input == Gun.Sniper)
			droppedGun = (GameObject) Instantiate(Resources.Load ("PickupSniper"), transform.position, transform.rotation);
		else
			droppedGun = null;
		droppedGun.GetComponent<GunProperties>().SetAmmo(m_heldGun.GetComponent<GunProperties>().GetAmmo());
		droppedGun.GetComponent<AmmoManager>().SetAmmo(m_heldGun.GetComponent<AmmoManager>().GetAmmo());
	}
	
	private void PickUpGun(GameObject m_input)
	{
		// we need to replace the values for the current gun with this gun
		if (m_currentGun == Slot.Gun1)
		{
			Destroy (m_heldGun);
			if (m_ePrimary == Gun.Pistol)
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Pistol"));
			else if (m_ePrimary == Gun.Shotgun)
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Shotgun"));
			else if (m_ePrimary == Gun.Rifle)
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Rifle"));
			else
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Sniper"));
			m_heldGun.GetComponent<GunProperties>().SetAmmo(m_input.GetComponent<GunProperties>().GetAmmo());
			m_heldGun.GetComponent<AmmoManager>().SetAmmo(m_input.GetComponent<AmmoManager>().GetAmmo());
		}
		else
		{
			Destroy (m_heldGun);
			if (m_eSecondary == Gun.Pistol)
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Pistol"));
			else if (m_eSecondary == Gun.Shotgun)
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Shotgun"));
			else if (m_eSecondary == Gun.Rifle)
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Rifle"));
			else
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Sniper"));
			m_heldGun.GetComponent<GunProperties>().SetAmmo(m_input.GetComponent<GunProperties>().GetAmmo());
			m_heldGun.GetComponent<AmmoManager>().SetAmmo(m_input.GetComponent<AmmoManager>().GetAmmo());
		}
		m_heldGun.transform.parent = gameObject.transform.FindChild("Main Camera/Gun");
		m_heldGun.transform.localPosition = Vector3.zero;
		m_heldGun.transform.localEulerAngles = Vector3.zero;
		m_heldGun.GetComponentInChildren<GunProperties>().SetBulletSocket(m_heldGun.transform.parent.transform.Find("BulletSocket").gameObject);
	}
	
	private void PickUpGunSpecial(GameObject m_input)
	{
		// this is a special case just for when the player doesn't have two weapons yet, so he must only have a primary
		m_iPrimaryAmmo = m_heldGun.GetComponent<GunProperties>().GetAmmo();
		m_iPrimaryReserveAmmo = m_heldGun.GetComponent<AmmoManager>().GetAmmo();
		
		Destroy (m_heldGun);
		
		if (m_eSecondary == Gun.Pistol)
			m_heldGun = (GameObject) Instantiate(Resources.Load ("Pistol"));
		else if (m_eSecondary == Gun.Shotgun)
			m_heldGun = (GameObject) Instantiate(Resources.Load ("Shotgun"));
		else if (m_eSecondary == Gun.Rifle)
			m_heldGun = (GameObject) Instantiate(Resources.Load ("Rifle"));
		else
			m_heldGun = (GameObject) Instantiate(Resources.Load ("Sniper"));
		m_heldGun.GetComponent<GunProperties>().SetAmmo(m_input.GetComponent<GunProperties>().GetAmmo());
		m_heldGun.GetComponent<AmmoManager>().SetAmmo(m_input.GetComponent<AmmoManager>().GetAmmo());
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
			m_iSecondaryAmmo = m_heldGun.GetComponent<GunProperties>().GetAmmo();
			m_iSecondaryReserveAmmo = m_heldGun.GetComponent<AmmoManager>().GetAmmo();
		}
		else
		{
			m_iPrimaryAmmo = m_heldGun.GetComponent<GunProperties>().GetAmmo();
			m_iPrimaryReserveAmmo = m_heldGun.GetComponent<AmmoManager>().GetAmmo();
		}
		
		Destroy (m_heldGun);
		
		if (m_currentGun == Slot.Gun1)
		{
			if (m_ePrimary == Gun.Pistol)
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Pistol"));
			else if (m_ePrimary == Gun.Shotgun)
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Shotgun"));
			else if (m_ePrimary == Gun.Rifle)
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Rifle"));
			else
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Sniper"));
			m_heldGun.GetComponent<GunProperties>().SetAmmo(m_iPrimaryAmmo);
			m_heldGun.GetComponent<AmmoManager>().SetAmmo(m_iPrimaryReserveAmmo);
		}
		else
		{
			if (m_eSecondary == Gun.Pistol)
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Pistol"));
			else if (m_eSecondary == Gun.Shotgun)
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Shotgun"));
			else if (m_eSecondary == Gun.Rifle)
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Rifle"));
			else
				m_heldGun = (GameObject) Instantiate(Resources.Load ("Sniper"));
			m_heldGun.GetComponent<GunProperties>().SetAmmo(m_iSecondaryAmmo);
			m_heldGun.GetComponent<AmmoManager>().SetAmmo(m_iSecondaryReserveAmmo);
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
		Collider[] m_pickupColliders = Physics.OverlapSphere(transform.position, 3.2f, LayerMask.GetMask("PickupLayer"));
		// 3.2 is a size based on the current size of the Pickup colliders as of 9-4-14
		m_availablePickups.Clear();
		Gun otherGun;
		foreach (Collider weapon in m_pickupColliders)
		{
			GunProperties.Gun m_eTempGun = weapon.GetComponentInParent<GunProperties>().GetGunType();
			if (m_eTempGun == GunProperties.Gun.Pistol)
				otherGun = Gun.Pistol;
			else if (m_eTempGun == GunProperties.Gun.Shotgun)
				otherGun = Gun.Shotgun;
			else if (m_eTempGun == GunProperties.Gun.Rifle)
				otherGun = Gun.Rifle;
			else
				otherGun = Gun.Sniper;
			
			if (!(otherGun == Gun.Pistol && m_ePrimary == Gun.Pistol) &&
			    !(otherGun == Gun.Pistol && m_eSecondary == Gun.Pistol) &&
			    !(otherGun == Gun.Shotgun && m_ePrimary == Gun.Shotgun) &&
			    !(otherGun == Gun.Shotgun && m_eSecondary == Gun.Shotgun) &&
			    !(otherGun == Gun.Rifle && m_ePrimary == Gun.Rifle) &&
			    !(otherGun == Gun.Rifle && m_eSecondary == Gun.Rifle) &&
			    !(otherGun == Gun.Sniper && m_ePrimary == Gun.Sniper) &&
			    !(otherGun == Gun.Sniper && m_eSecondary == Gun.Sniper))
			{
				// this is a gun that is not in our inventory, so it can potentially be picked up, add it to the list
				m_availablePickups.Add(weapon);
			}
		}	
		
		if (m_availablePickups.Count > 0)
		{
			// with this list, we need to find out which gun is the closest to the player
			Collider m_closestPickup = m_availablePickups[0];
			float m_fShortestDistance = Vector3.Distance(transform.position, m_closestPickup.transform.position);
			
			foreach (Collider weapon in m_availablePickups)
			{
				if (Vector3.Distance(transform.position, weapon.transform.position) < m_fShortestDistance)
					m_closestPickup = weapon;
			}
			return m_closestPickup.transform.parent;
		}
		return null;
	}
	
	private void GetAmmo()
	{
		Collider[] m_pickupColliders = Physics.OverlapSphere(transform.position, 1.5f, LayerMask.GetMask("PickupLayer"));
		Gun otherGun;
		foreach (Collider weapon in m_pickupColliders)
		{
			if (weapon.GetComponentInParent<GunProperties>())
			{
				// this is checking to see if there is indeed a GunProperties script on the pickup object
				GunProperties.Gun m_eTempGun = weapon.GetComponentInParent<GunProperties>().GetGunType();
				if (m_eTempGun == GunProperties.Gun.Pistol)
					otherGun = Gun.Pistol;
				else if (m_eTempGun == GunProperties.Gun.Shotgun)
					otherGun = Gun.Shotgun;
				else if (m_eTempGun == GunProperties.Gun.Rifle)
					otherGun = Gun.Rifle;
				else
					otherGun = Gun.Sniper;
				
				if (otherGun == Gun.Pistol && m_ePrimary == Gun.Pistol ||
				    otherGun == Gun.Pistol && m_eSecondary == Gun.Pistol ||
				    otherGun == Gun.Shotgun && m_ePrimary == Gun.Shotgun ||
				    otherGun == Gun.Shotgun && m_eSecondary == Gun.Shotgun ||
				    otherGun == Gun.Rifle && m_ePrimary == Gun.Rifle ||
				    otherGun == Gun.Rifle && m_eSecondary == Gun.Rifle ||
				    otherGun == Gun.Sniper && m_ePrimary == Gun.Sniper ||
				    otherGun == Gun.Sniper && m_eSecondary == Gun.Sniper)
				{
					// the player is currently already carring that weapon, which means they can get ammo
					// get ammo from the pickup
					GetComponentInChildren<AmmoManager>().FillUpAmmo(weapon.transform.parent.GetComponentInParent<AmmoManager>());
				}
			}
		}
	}
	
	public void SetGunActive(bool input)
	{
		m_heldGun.SetActive(input);
	}
}

using UnityEngine;
using System.Collections;

public class Medkit : MonoBehaviour 
{
	// medkit will heal the player if F is held
	// once F is held long enough, medkit is used
	// if F is released before timer is done, it does not get used
	// medkit will also heal any objects within a certain distance in the PlayerLayer (other players)
	
	// player is rooted in place while healing, CanControl = false on CharacterController
	// deactivate gun
	// start timer, play animation
	// once timer is finished, do Heal() to player and any players close enough
	// stop animation
	// reactivate gun, CanControl = true
	// reset timer back to original time
	// if key is released before finished, stop animation, reactivate gun, reset timer, CanControl=true
	
	// health script keeps track of whether or not a healthkit can be picked up
	
	private float m_fHealTimer;
	private float m_fHealTimerReset = 3.0f;
	private float m_fHealRadius = 10.0f;
	private int m_iMedkitHealVal = 80;
	private GameObject m_ItemSpawner;
	
	// Use this for initialization
	void Start () 
	{
		m_fHealTimer = m_fHealTimerReset;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!m_ItemSpawner)
		{
			if (Input.GetKey (KeyCode.F))
			{
				m_fHealTimer -= Time.deltaTime;
				
				// in healing state
				// play animation
				GetComponent<CharacterMotor>().canControl = false;
				GetComponent<WeaponManager>().SetGunActive(false);
				
				if (m_fHealTimer <= 0)
				{
					Debug.Log ("Heal!");
					// end healing state
					m_fHealTimer = m_fHealTimerReset;
					Heal();
					GetComponent<CharacterMotor>().canControl = true;
					GetComponent<WeaponManager>().SetGunActive(true);
				}
			}
			else if (Input.GetKeyUp (KeyCode.F))
			{	
				// end healing state
				m_fHealTimer = m_fHealTimerReset;
				GetComponent<CharacterMotor>().canControl = true;
				GetComponent<WeaponManager>().SetGunActive(true);
				Debug.Log ("Heal cancelled");
			}
		}
	}
	
	void Heal()
	{
		Collider [] otherPlayers = Physics.OverlapSphere(transform.position, m_fHealRadius, LayerMask.GetMask("PlayerLayer"));
		GetComponent<Health>().SetHealth(GetComponent<Health>().GetHealth() + m_iMedkitHealVal);
		foreach (Collider other in otherPlayers)
		{
			other.GetComponent<Health>().SetHealth(GetComponent<Health>().GetHealth() + m_iMedkitHealVal);
		}
		Destroy (this);
		// remove this script from player
	}
	
	public void SetItemSpawner(GameObject input)
	{
		m_ItemSpawner = input;
	}
	
	public void MedkitTaken()
	{
		m_ItemSpawner.GetComponent<ItemSpawner>().ItemTaken();
		Debug.Log ("Medkit picked up");
		// IMPORTANT George probably wants this to be handled differently to ensure networking sync between clients ------------
		Destroy (gameObject);
	}
}

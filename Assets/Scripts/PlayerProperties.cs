using UnityEngine;
using System.Collections;

public class PlayerProperties : Photon.MonoBehaviour 
{
	//public int m_iHealth;
	// Use this for initialization
	public enum Guns
	{
		Pistol,
		Rifle,
		Shotgun,
		Sniper,
		None
	}

	//THIS IS THE HONOR SYSTEM DONT CHANGE THIS VALUE
	public int m_iID;

	public Guns gun;
	void Start () 
	{
		gun = Guns.None;
		//m_iHealth = GameMaster.Instance.getHealth();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//m_iHealth = GameMaster.Instance.getHealth();


	}

	[RPC]
	public void NetworkSetPlayerID(int id)
	{
		m_iID = id;
	}
}

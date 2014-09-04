using UnityEngine;
using System.Collections;

public class PlayerProperties : MonoBehaviour 
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
}

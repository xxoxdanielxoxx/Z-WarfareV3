using UnityEngine;
using System.Collections;

public class PlayerIDManager : MonoBehaviour 
{

	GameObject[] m_Players;
	static PlayerIDManager s_PlayerIDManager;

	void Awake()
	{
		if (s_PlayerIDManager == null)
			s_PlayerIDManager = this;
		else
			return;
	}

	/// <summary>
	/// Delages New IDS to Players, Should Only Be Called Once. IDS Are allocated Simple: Its just there Index to the Array Size 0 - 4
	/// </summary>
	public void Activate()
	{
		m_Players = new GameObject[4];
		m_Players = GameObject.FindGameObjectsWithTag("Player");

		for (int i = 0; i < m_Players.Length; ++i)
		{
			m_Players[i].GetComponent<PlayerProperties>().m_iID = i;	
		}
	}
	
	public GameObject FindPlayer(int ID)
	{
		GameObject n = null;

		for (int i = 0; i < m_Players.Length; ++i)
		{
			if(m_Players[i].GetComponent<PlayerProperties>().m_iID == ID)
				return m_Players[i];
		}
		Debug.LogError ("Player wasnt found");

		return n;
	}

	public GameObject[] GetALLPlayers()
	{
		return m_Players;
	}


	public  static PlayerIDManager Get()
	{
		return s_PlayerIDManager;
	}
}

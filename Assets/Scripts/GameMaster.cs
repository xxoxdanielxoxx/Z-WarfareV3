using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour 
{
	
	private static GameMaster instance;
	public int m_iHealth = 100;
	
	public static GameMaster Instance
	{
		
		get
		{
			if (instance == null)
			{
				instance = new GameObject("GameMaster").AddComponent<GameMaster>();
			}
			return instance;
		}
	}
	
	public void OnApplicationQuit()
	{
		instance = null;
	}
	
	public void StartState()
	{
		// set default variables
		m_iHealth = 100;
		Application.LoadLevel (1);
	}
		
	public int getHealth()
	{
		return m_iHealth;
	}
	
	public  void loseHealth(int value)
	{
		m_iHealth -= value;
	}
}

using UnityEngine;
using System.Collections;

public class ZombieThreatLogic : MonoBehaviour 
{
	public bool 	m_AggrOn;

	// Threat tracking
	public uint[] 	m_Hatred = new uint[8];	// 0-3 are the players, 4 is the enviroment aggro
	public float[]	m_Threat = new float[8];
	public byte		m_TargetIdx;	// Index of the highest threat target. 100 if there is no target

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Reset()
	{
		// Set up threat system
		for (int i = 0; i < 8;  ++i) 
		{
			m_Hatred[i] = 1;
		}
		m_Hatred[7] = 0;

		// Set up threat system
		for (int i = 0; i < 8;  ++i) 
		{
			m_Threat[i] = 1;
		}
		m_Threat[7] = 0;
		
		m_TargetIdx = 100;
		m_AggrOn = false;
	}

	public void CalculateThreat()
	{
		uint threatSum = 0;
		
		// Add up the threat of each player
		for (int i = 0; i < GroupAI.m_Master.players.Length;  ++i) 
		{
			threatSum += m_Hatred[i];
		}
		
		// Divide the sumation to get the percent of hate a player has, which will directly translate to threat
		for (byte i = 0; i < GroupAI.m_Master.players.Length; ++i)
		{
			m_Threat[i] = (float)m_Hatred[i]/(float)threatSum;
		}
	}
}
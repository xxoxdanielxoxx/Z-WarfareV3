using UnityEngine;
using System.Collections;

public class ZombieThreatLogic : MonoBehaviour 
{


	public bool 	m_AggrOn;

	// Threat tracking
	public uint[] 	m_Hatred = new uint[8];	// 0-3 are the players, 4+ is the enviroment aggro
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
		// Set up HATE system
		for (int i = 0; i < 4;  ++i) 
		{
			// Default player hate is 1
			m_Hatred[i] = 1;
		}
		for (int i = 0; i < 4;  ++i) 
		{
			// Default enviromental hate is 0
			m_Hatred[i+4] = 0;
		}

		// Set up THREAT system
		for (int i = 0; i < 8;  ++i) 
		{
			m_Threat[i] = 0;
		}
		
		m_TargetIdx = 100;
		m_AggrOn = false;
	}

	public void BuildHate(uint _hate, int _pIdx)
	{
		m_Hatred [_pIdx] += _hate;

		Debug.Log("The spooky guy is player "+_pIdx);

		CalculateThreat ();
	}

	public void CalculateThreat() // Potato: Does this even count up grenade threat? 
	{
		uint threatSum = 0;
		
		// Add up the threat of each player and extra
		for (int i = 0; i < 8;  ++i) 
		{
			threatSum += m_Hatred[i];
		}
		
		// Divide the sumation to get the percent of hate a player has, which will directly translate to threat
		for (byte i = 0; i < 8; ++i)
		{
			m_Threat[i] = (float)m_Hatred[i]/(float)threatSum;
		}
	}
}
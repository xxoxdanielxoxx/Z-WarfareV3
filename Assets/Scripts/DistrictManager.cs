using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	Manages turning off and on blockers and item drop timers
*/
public class DistrictManager : MonoBehaviour 
{
	private AIMaster m_aiMaster;
	public GameObject m_districtTwoBlocker;
	public GameObject m_districtThreeBlocker;
	public GameObject m_districtFourBlocker;
	public List<DistrictProperties> m_playerDistrictPos;
	public float m_fD1ItemTimer;
	public float m_fD1ItemTimerReset = 60;
	
	// called in update once, I am predicting that this function will run late enough that everything will be assigned correctly
	void Reset()
	{
		m_aiMaster = GetComponent<AIMaster>();
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject gObj in players)
		{
			m_playerDistrictPos.Add (gObj.GetComponent<DistrictProperties>());
		}
		m_fD1ItemTimer = m_fD1ItemTimerReset;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// I only expect Reset() to be called once because m_aiMaster should never be null after it is assigned
		if (m_aiMaster == null)
			Reset ();
		
		// not sure if it's a wise idea to check if the blocker's currently active, I did it to make sure debug.log only happens once
		// shouldn't be a problem later
		if (m_aiMaster.wave == 3 && m_districtThreeBlocker.activeSelf)
		{
			Debug.Log ("Woohoo! The survivors have reached wave 3! Opening District Three");
			m_districtThreeBlocker.SetActive(false);
		}
		
		if (PlayersInOneDistrict() && m_playerDistrictPos[0].GetDistrict() == DistrictProperties.District.One)
		{
			m_fD1ItemTimer -= Time.deltaTime;
			if (m_fD1ItemTimer <= 0)
			{
				m_fD1ItemTimer = m_fD1ItemTimerReset;
			}
		}
	}
	
	bool PlayersInOneDistrict()
	{
		bool testDistrict = true;
		for (int i = 0; i < m_playerDistrictPos.Count; i++)
		{
			if (!testDistrict)
				break;
			else if (i + 1 < m_playerDistrictPos.Count)
			{
				if (m_playerDistrictPos[i].GetDistrict() != m_playerDistrictPos[i+1].GetDistrict())
				{
					testDistrict = false;
				}
			}
		}
		return testDistrict;
	}
	
	
}

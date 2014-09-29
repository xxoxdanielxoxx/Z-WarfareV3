using UnityEngine;
using System.Collections;

public class DistrictProperties : MonoBehaviour 
{
	public enum District
	{
		One,
		Two,
		Three,
		Four,
		None
	}
	
	public District m_district;
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "DistrictOne")
		{
			if (m_district != District.One)
				Debug.Log (gameObject + " entered District One"); 
			m_district = District.One;
			
		}
		else if (other.tag == "DistrictTwo")
		{
			if (m_district != District.Two)
				Debug.Log (gameObject + " entered District Two");
			m_district = District.Two;
		}
		else if (other.tag == "DistrictThree")
		{
			if (m_district != District.Three)
				Debug.Log (gameObject + " entered District Three");
			m_district = District.Three;
		}
		else if (other.tag == "DistricFour")
		{
			if (m_district != District.Four)
				Debug.Log (gameObject + " entered District Four");
			m_district = District.Four;
		}
	}
	
	public void SetDistrict(District input)
	{
		m_district = input;
		Debug.Log (gameObject + " set to District " + m_district);
	}
	
	public District GetDistrict()
	{
		return m_district;
	}
}

using UnityEngine;
using System.Collections;

public class ThreatEmitter : MonoBehaviour 
{
	private float m_fRadius;
	private Vector3 m_position;
	private int m_iPlayerID;
	private int m_iBaseThreat;
	private bool m_valuesSet = false;
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (m_valuesSet)
		{
			DetectZombies();
			Destroy (gameObject);
		}
	}
	
	public void SetValues(Vector3 inputPosition, int inputPlayerID, float inputRadius, int inputDamage)
	{
		m_position = inputPosition;
		m_iPlayerID = inputPlayerID;
		m_fRadius = inputRadius;
		m_iBaseThreat = inputDamage;
		m_valuesSet = true;
	}
	
	void DetectZombies()
	{
		// this will find all zombies nearby
		Collider[] overlapSphere = Physics.OverlapSphere(transform.position, m_fRadius, LayerMask.GetMask("Zombie"));
		int debugCount = 0;
		if (overlapSphere.Length > 0)
		{
			foreach (Collider zombie in overlapSphere)
			{
				// This is being checked because it is possible for child objects in layer Zombie to be counted. We only want gameObjects that actually have the ZombieThreatRecognition script on it
				if (zombie.GetComponent<ZombieThreatRecognition>())
				{
					float distanceFromZomb = Vector3.Distance(transform.position, zombie.transform.position);
					float threat = CalculateThreat(distanceFromZomb);
					zombie.gameObject.GetComponent<ZombieThreatRecognition>().SoundDetected(m_position, m_iPlayerID, threat);
					debugCount++;
				}
			}
		}
		//Debug.Log ("zombies detected: " + debugCount);		
	}
	
	float CalculateThreat(float zombDistance)
	{
		// returns approx 1 if close to player, using full base threat, returns 0 if at edge of threat emitter radius
		return m_iBaseThreat * ((m_fRadius - zombDistance) / m_fRadius);
	}
}
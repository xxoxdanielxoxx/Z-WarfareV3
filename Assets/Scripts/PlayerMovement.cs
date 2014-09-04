using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	CharacterMotor m_characterMotor;
	public float m_fSprintModifierFwd = 1.5f;
	public float m_fWalkModifier = 0.5f;
	private float m_initialSpeed;
	
	// Use this for initialization
	void Start () 
	{
		m_characterMotor = GetComponent<CharacterMotor>();
		m_initialSpeed = m_characterMotor.movement.maxForwardSpeed;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.V))
		{
			m_characterMotor.movement.maxForwardSpeed *= m_fSprintModifierFwd;
		}
		if (Input.GetKeyUp (KeyCode.V))
		{
			m_characterMotor.movement.maxForwardSpeed = m_initialSpeed;
		}
		if (Input.GetKeyDown (KeyCode.C))
		{
			m_characterMotor.movement.maxForwardSpeed *= m_fWalkModifier;
		}
		if (Input.GetKeyUp (KeyCode.C))
		{
			m_characterMotor.movement.maxForwardSpeed = m_initialSpeed;
		}
	}
}
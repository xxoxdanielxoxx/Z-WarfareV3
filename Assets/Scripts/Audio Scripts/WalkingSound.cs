using UnityEngine;
using System.Collections;

public class WalkingSound : MonoBehaviour {
	/*  get the audio Source that will play the 3d sound. 
	 * (just drag and drop the object that has the audio source)*/
	public AudioSource m_audioSource;
	//this means that the object will need to be a character controller
	public CharacterController m_charCon;
	public Transform m_transform;

	public LayerMask m_hitLayer;
	//used to make the steps play apart
	public float m_footTimer;

	//will be geting a tag frpm the planes or objects the character is walking over
	private string m_tag;
	private float m_volume;
	private float m_time;

	// Use this for initialization
	void Start () {
		m_charCon = GetComponent<CharacterController>();
		m_transform = this.transform;
		m_volume = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {

		//when the character starts moving it will start making the steps sounds
		if(m_charCon.velocity.x != 0.0f || m_charCon.velocity.z != 0.0f)
		{
			//every .4 secs it will make a step
			m_time -= Time.deltaTime;
			if(m_time <= 0.0f)
			{
				m_audioSource.PlayOneShot(GetAudio(), m_volume);
				m_time = m_footTimer;
			}
		}
	}

	//getter function
	AudioClip GetAudio()
	{
		RaycastHit hit;

		Debug.DrawRay(m_transform.position + new Vector3 (0.0f, .5f, 0.0f), -Vector3.up * 50.0f);

		if(Physics.Raycast(m_transform.position + new Vector3 (0.0f, .5f, 0.0f), -Vector3.up, out hit, Mathf.Infinity, m_hitLayer))
		{
			m_tag = hit.collider.tag.ToLower();
		}

		if(m_tag == "wood")
		{
			return SoundBank.Get().GetWoodSteps();
		}
		else if(m_tag == "dirt")
		{
			return SoundBank.Get().GetDirtSteps();
		}
		else if(m_tag == "concrete")
		{
			m_volume = 0.8f;
			return SoundBank.Get().GetConcreteSteps();
		}
		return null;

	}
}

using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	//static singleton
	private static AudioManager s_audioMan;
	//wasn't sure what to refrence if I should look for a script or just the gameobject it self
	public GameObject m_manager;
	public GameObject m_player;
		
	void Awake()
	{
		//checks if there a other instance(s_audioMan) that is conflicting
		if(s_audioMan != null && s_audioMan != this)
		{
			//if that is the case it will destroy the other instance(s_audioMan)
			Destroy(gameObject);
		}

		//save the instance
		s_audioMan = this;
	}

	void Start()
	{
		m_manager = GameObject.Find("GameMaster");
		m_player   = GameObject.Find("First Person Controller");
	}

	void Update()
	{
		//logic for playing the sound will go in here.

		//testing purposes to see if the sound bank is working
		if(Input.GetKey(KeyCode.T))
		{
			Debug.Log("hit");
			audio.clip = SoundBank.Get().GetTestSound();
			audio.Play();

		}
	}

	//allows you to use the audio manager anywhere in the project
	public static AudioManager Get()
	{
		return s_audioMan;
	}

}

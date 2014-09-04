using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundBank : MonoBehaviour {

	private static SoundBank s_soundBank;
	//holds a list of Audioclips where all 2d sounds will be stored 
	public List<AudioClip> m_2dSoundClips;
	//going to  make threes different list for the the footsteps sound
	public List<AudioClip> m_dirtSteps;
	public List<AudioClip> m_concreteSteps;
	public List<AudioClip> m_woodSteps;

	void Awake()
	{
		s_soundBank = this;
	}

	public static SoundBank Get()
	{
		return s_soundBank;
	}

	//for testing purposes
	public AudioClip GetTestSound()
	{
		return m_2dSoundClips[0];
	}

	//getters for the footsteps.
	public AudioClip GetDirtSteps()
	{
		return m_dirtSteps[0];
	}

	public AudioClip GetConcreteSteps()
	{
		return m_concreteSteps[0];
	}

	public AudioClip GetWoodSteps()
	{
		return m_woodSteps[0];
	}
}

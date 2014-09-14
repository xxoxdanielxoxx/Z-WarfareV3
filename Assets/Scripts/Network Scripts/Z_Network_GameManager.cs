using UnityEngine;
using System.Collections;

public class Z_Network_GameManager : Photon.MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnDisconnectedFromPhoton()
	{
		//Load main menu
		Screen.lockCursor = false;
		Application.LoadLevel((Application.loadedLevel - 1));
	}
}

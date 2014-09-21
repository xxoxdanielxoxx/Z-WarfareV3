using UnityEngine;
using System.Collections;

public class Z_Network_PlayerScriptManager : Photon.MonoBehaviour 
{
	public string m_sName;


	void Start()
	{

		// This is our own player
		if (photonView.isMine)
		{
			Z_Network_RigidBody RB = GetComponent<Z_Network_RigidBody>()as Z_Network_RigidBody ;
			//RB.enabled = false;

			Destroy(GameObject.Find("LevelCamera"));
			

		}
		// This is just some remote controlled player, don't execute direct
		// user input on this. DO enable multiplayer controll
		else
		{            
			//m_sName += msg.sender.name; 
			
			//////TURN OFF

			//Turn off Camera
			transform.FindChild("Main Camera").gameObject.active = false;
			
			//Turn off input
			FPSInputController input = GetComponent<FPSInputController>() as FPSInputController;
			input.enabled = false;
			
			//Turn off mouse
			MouseLook mouse = GetComponent<MouseLook>() as MouseLook;
			mouse.enabled = false;

			//Turn off Character Controller
			CharacterMotor  cm = GetComponent<CharacterMotor>()as CharacterMotor;
			cm.enabled = false;
		}
	}

	


	[RPC]
	public void InstantiateNetworkBullet(Vector3 pos, Quaternion rot, int dmg, float spd, int ID)
	{

		Debug.Log ("Creating a Network  Bullet");
		GameObject bullet = (GameObject) Instantiate (Resources.Load ("Bullet"), pos, rot);
		bullet.GetComponent<Bullet>().SetDamage(dmg);
		bullet.GetComponent<Bullet>().SetBulletSpeed(spd);
		bullet.GetComponent<Bullet>().SetPlayerID(ID);

	}


	


}

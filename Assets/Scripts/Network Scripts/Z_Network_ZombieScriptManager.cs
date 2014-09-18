//#define offline 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Z_Network_ZombieScriptManager : Photon.MonoBehaviour 
{

	ZombieAI 			m_ZombieAIScript;
	ZombieStateMachine 	m_ZombieStateMachine;
	ZombieThreatLogic  	m_ZombieThreatLogic;
	ZombieMovement 	   	m_ZombieMovement;
	NavMeshAgent 		m_NavMeshAgent;

	Z_Network_RigidBody        m_NetRigidBody;
	private ZombieStates       m_ePrevZombieState;

	private bool m_bIsHost;

	/*
		allocoate the ID
		So That the Host is the Owner Of this Object.

	 */


	void Awake()
	{


		//Host or Client
		m_bIsHost = PhotonNetwork.isMasterClient;
#if offline
		PhotonNetwork.offlineMode = true;
		m_bIsHost = PhotonNetwork.offlineMode;
#endif


		//Get all the Scripts
		m_ZombieAIScript 	 = GetComponent<ZombieAI> ();
		m_ZombieStateMachine = GetComponent<ZombieStateMachine> ();
		m_ZombieThreatLogic  = GetComponent<ZombieThreatLogic> ();
		m_ZombieMovement 	 = GetComponent<ZombieMovement> ();
		m_NetRigidBody		 = GetComponent<Z_Network_RigidBody>();
		m_NavMeshAgent 		 = GetComponent<NavMeshAgent> ();


		//Logg the Prev State
		m_ePrevZombieState = m_ZombieStateMachine.m_State;

		//Turn off scripts if we are the Clients
		if (m_bIsHost) 
		{
			//m_NetRigidBody.enabled = false;
			m_ZombieStateMachine.m_bIsHost = true;
		} 
		else 
		{
			m_ZombieMovement.enabled    = false;
			m_ZombieThreatLogic.enabled = false;
			m_ZombieAIScript.enabled    = false;
			m_NavMeshAgent.enabled    	= false;

			m_ZombieStateMachine.m_bIsHost = false;
		}

	}

	void BroadcastStates()
	{
	  	//If true we are a single player game and no reason to broadcast 
		if (PhotonNetwork.playerList.Length < 2)
			return;

		if (m_bIsHost) 
		{
			//If we changed states, Broad cast this to the clients
			if(m_ePrevZombieState != m_ZombieStateMachine.m_State)
			{
				//photonView.RPC("ReceiverStates", PhotonTargets.Others, (int)m_ZombieStateMachine.m_State);
				//I need add a Genral Netowork Start
			}
		}

		m_ePrevZombieState = m_ZombieStateMachine.m_State;
	}  

	[RPC]
	void ReceiverStates(int newState)
	{
		m_ZombieStateMachine.m_State = (ZombieStates)newState;
	}


	// Update is called once per frame
	void Update () 
	{
		BroadcastStates ();
	}
}

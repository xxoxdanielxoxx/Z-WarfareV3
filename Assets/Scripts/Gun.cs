using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gun : MonoBehaviour 
{
	public enum GunType
	{
		Pistol,
		Rifle,
		Shotgun,
		Sniper,
		None
	}

	public GunType m_eGunType = GunType.Pistol;
	public GameObject m_bulletPrefab;
	public GameObject m_bulletSocket;
	public float m_fRandomSpread = 5;
	private int m_iNumShotgunPellets = 8;
	public float m_fFireRate = 0.25f;
	private float m_fFireRateTimer;
	public float m_fBulletSpeed = 4.0f;
	public int m_iDamage = 10;
	// Use this for initialization
	void Start () 
	{
		m_fFireRateTimer = m_fFireRate;
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_fFireRateTimer -= Time.deltaTime;
	}

	public void Shoot()
	{
		if (m_fFireRateTimer <= 0)
		{
			m_fFireRateTimer = m_fFireRate;
			if (m_eGunType == GunType.Shotgun)
			{
				//Transform bulletTrajectory = m_GBulletSocket.transform;
				//Quaternion bulletRotation = m_bulletSocket.transform.rotation;
				for (int i = 0; i < m_iNumShotgunPellets; i++) 
				{
					Quaternion bulletRotation = new Quaternion(m_bulletSocket.transform.rotation.x, m_bulletSocket.transform.rotation.y, m_bulletSocket.transform.rotation.z, m_bulletSocket.transform.rotation.w);
					bulletRotation.eulerAngles += new Vector3(Random.Range(-m_fRandomSpread, m_fRandomSpread), Random.Range (-m_fRandomSpread, m_fRandomSpread), Random.Range (-m_fRandomSpread, m_fRandomSpread));
					GameObject bullet = (GameObject) Instantiate (m_bulletPrefab, m_bulletSocket.transform.position, bulletRotation);
					bullet.GetComponent<Bullet>().setDamage(m_iDamage);
					bullet.GetComponent<Bullet>().setBulletSpeed(m_fBulletSpeed);
				}
			}
			else
			{
				GameObject bullet = (GameObject) Instantiate (m_bulletPrefab, m_bulletSocket.transform.position, m_bulletSocket.transform.rotation);
				bullet.GetComponent<Bullet>().setDamage(m_iDamage);
				bullet.GetComponent<Bullet>().setBulletSpeed(m_fBulletSpeed);
			}
			MouseLook mouseLook = GetComponentInParent<MouseLook>();


		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController : MonoBehaviour 
{
	private int lines = 6;
	private int enemiesPerLine = 6;
	private const float MOTHERSHIP_INTERVAL = 4.0f;

	private const float H_GAP = 0.8f;
	private const float V_GAP = 0.7f;
	
	private int frameCounter = 0;
	private int speed = 15;
	private float shootingInterval = 2.0f;
	
	private float startX;
	private float startY = 3.0f;

	private Vector2 directionVector;
	private Vector2 motherShipDirectionVector;

	private GameObject MotherShip;

	public GameObject MotherShipPrefab;
	public GameObject Enemy1Prefab;
	public GameObject Enemy2Prefab;
	public GameObject Enemy3Prefab;

	void Awake () 
	{
		directionVector = Vector2.right;
	}
	
	void Update () 
	{
		if (GameManager.Instance.isGamePaused) return;

		if (gameObject.transform.childCount == 0 && !MotherShip)
			GameManager.Instance.EndGame (true);

		if (frameCounter%25 == 0)
		{
			transform.Translate (directionVector * speed * Time.deltaTime);

			float myWidth = (enemiesPerLine - 1) * H_GAP;

			if (transform.position.x > GameManager.Instance.hBound - myWidth/2 ||
				transform.position.x < - (GameManager.Instance.hBound - myWidth/2))
			{
				transform.position = new Vector2 (transform.position.x, transform.position.y - V_GAP);
				directionVector.x *= -1;
				speed += 2;
				shootingInterval -= 0.2f;

				if (transform.childCount > 0 && (transform.GetChild(transform.childCount - 1).transform.position.y + transform.position.y < - Camera.main.orthographicSize))
					GameManager.Instance.EndGame (false);	
			}

			if (MotherShip) 
			{
				MotherShip.transform.Translate (motherShipDirectionVector * speed * 1.5f * Time.deltaTime);

				if (MotherShip.transform.position.x > GameManager.Instance.hBound + MotherShip.GetComponent<SpriteRenderer> ().bounds.size.x ||
					MotherShip.transform.position.x < - (GameManager.Instance.hBound + MotherShip.GetComponent<SpriteRenderer> ().bounds.size.x))
				{
					CancelInvoke ("MotherShipMovement");
					CancelInvoke ("MotherShipShooting");
					Destroy (MotherShip);
				}
			}
		}

		frameCounter++;
	}

	public void CreateEnemies ()
	{
		startX =  - ( (enemiesPerLine - 1) * H_GAP)/2;

		for (int i = 0; i < enemiesPerLine * lines; i++)
		{
			GameObject prefab;

			if (i < enemiesPerLine * 2)
				prefab = Enemy1Prefab;
			else if (i < enemiesPerLine * 4)
				prefab = Enemy2Prefab;
			else
			 	prefab = Enemy3Prefab;

			Instantiate (prefab, new Vector2 (startX + i%enemiesPerLine * H_GAP, startY - i/enemiesPerLine * V_GAP), Quaternion.identity, gameObject.transform);
		}
	}

	public void StartShooting ()
	{
		CancelInvoke ("Shooting");
		InvokeRepeating ("Shooting", shootingInterval, shootingInterval);
	}

	private void Shooting ()
	{
		if (gameObject.transform.childCount == 0) return;
			
		gameObject.transform.GetChild (Random.Range (0, gameObject.transform.childCount - 1)).GetComponent<Enemy> ().canShoot = true;
	}

	public void MotherShipAppearence ()
	{
		CancelInvoke ("MotherShipMovement");
		InvokeRepeating ("MotherShipMovement", MOTHERSHIP_INTERVAL, MOTHERSHIP_INTERVAL);
	}

	private void MotherShipMovement ()
	{
		if (MotherShip || GameManager.Instance.isGamePaused) return;

		int rand = Random.Range (0, 10);
		if (rand < 4)
		{
			float dirRand = Random.Range (-1.0f, 1.0f);
			motherShipDirectionVector = dirRand > 0 ? Vector2.right : Vector2.left;
			
			MotherShip = Instantiate (MotherShipPrefab, 
									 new Vector2 (dirRand > 0 ? -GameManager.Instance.hBound : GameManager.Instance.hBound, 
									 			  gameObject.transform.GetChild (0).transform.position.y + V_GAP), 
									 Quaternion.identity);

			CancelInvoke ("MotherShipShooting");
			InvokeRepeating ("MotherShipShooting", shootingInterval, shootingInterval);
		}
	}

	public void MotherShipShooting ()
	{
		if (MotherShip) 
			MotherShip.GetComponent<Enemy> ().canShoot = true;
	}

	public void PauseEnemies (bool isPaused)
	{
		if (isPaused)
		{
			CancelInvoke ("MotherShipShooting");
			CancelInvoke ("Shooting");
		}
		else
		{
			InvokeRepeating ("MotherShipShooting", shootingInterval, shootingInterval);
			InvokeRepeating ("Shooting", shootingInterval, shootingInterval);
		}
	}

	public void DestroyAllEnemiesAndBullets ()
	{
		Destroy (MotherShip);

		for (int i = gameObject.transform.childCount; i > 0; i--)
		{
			Destroy (gameObject.transform.GetChild (i-1).gameObject);
		}

		GameObject[] Bullets = GameObject.FindGameObjectsWithTag ("Bullet");
		foreach (GameObject bullet in Bullets)
		{
			Destroy (bullet);
		}
	}

	public void increaseEnemiesNumber(int level)
	{
		enemiesPerLine += level;
		lines += level;
	}
}

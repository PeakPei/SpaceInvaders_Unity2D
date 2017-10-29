using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController : MonoBehaviour 
{
	//Constants
	private const float H_GAP = 0.8f;
	private const float V_GAP = 0.7f;
	private const float MOTHERSHIP_INTERVAL = 4.0f;
	private const float MOTHERSHIP_SPEED_DELTA = 1.5f;	
	private const int ENEMIES_TYPE_4_LINES_NUM = 1;
	private const int ENEMIES_TYPE_3_LINES_NUM = 1;
	private const int ENEMIES_TYPE_2_LINES_NUM = 2;
    private const string MOTHERSHIP_MOVEMENT = "MotherShipMovement";
    private const string SHOOTING = "Shooting";
    private const string BULLETS_TAG = "Bullet";

	//Variables
    private int lines = 6;
	private int enemiesPerLine = 6;
	private int frameCounter = 0;
	private int speed = 15;
	private float shootingInterval = 2.0f;
	
	private float startX;
	private float startY = 3.0f;

	private Vector2 directionVector;
	private Vector2 motherShipDirectionVector;

	private GameObject Mothership;

	//Prefabs
	public GameObject MotherShipPrefab;
	public GameObject Enemy1Prefab;
	public GameObject Enemy2Prefab;
	public GameObject Enemy3Prefab;
	public GameObject Enemy4Prefab;

	void Awake () 
	{
		directionVector = Vector2.right;
	}
	
	void Update () 
	{
		if (GameManager.Instance.IsGamePaused) return;

		if (transform.childCount == 0 && !Mothership)
			GameManager.Instance.EndGame (true);

		if (frameCounter%Model.ENEMIES_UPDATE_FRAMES_DELTA == 0)
		{
			transform.Translate (directionVector * speed * Time.deltaTime);

			float myWidth = (enemiesPerLine - 1) * H_GAP;

			if (transform.position.x > Model.HBound - myWidth/2 ||
				transform.position.x < - (Model.HBound - myWidth/2))
			{
				transform.position = new Vector2 (transform.position.x, transform.position.y - V_GAP);
				directionVector.x *= -1;
				speed += 2;
				shootingInterval -= 0.2f;
				Model.bulletsSpeed += 0.2f;				
			}

			if (Mothership) 
				Mothership.transform.Translate (motherShipDirectionVector * speed * MOTHERSHIP_SPEED_DELTA * Time.deltaTime);
			
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).GetComponent<Enemy>().UpdateSprite();
			}
		}

		frameCounter++;
	}

	void LateUpdate()
	{
		if (transform.childCount > 0 && (transform.GetChild(transform.childCount - 1).transform.position.y + transform.position.y < -Camera.main.orthographicSize))
					GameManager.Instance.EndGame (false);	

		if (Mothership) 
		{
			if (Mothership.transform.position.x > Model.HBound + Mothership.GetComponent<SpriteRenderer> ().bounds.size.x ||
				Mothership.transform.position.x < - (Model.HBound + Mothership.GetComponent<SpriteRenderer> ().bounds.size.x))
			{
				CancelInvoke (MOTHERSHIP_MOVEMENT);
				Destroy (Mothership);
			}
		}
	}

	public void CreateEnemies ()
	{
		startX =  - ( (enemiesPerLine - 1) * H_GAP)/2;

		for (int i = 0; i < enemiesPerLine * lines; i++)
		{
			GameObject prefab;

			if (i < enemiesPerLine * ENEMIES_TYPE_4_LINES_NUM)
				prefab = Enemy4Prefab;
			else if (i < enemiesPerLine * (ENEMIES_TYPE_4_LINES_NUM + ENEMIES_TYPE_3_LINES_NUM))
				prefab = Enemy3Prefab;
			else if (i < enemiesPerLine * (ENEMIES_TYPE_4_LINES_NUM + ENEMIES_TYPE_3_LINES_NUM + ENEMIES_TYPE_2_LINES_NUM))
			 	prefab = Enemy2Prefab;
			else
				prefab = Enemy1Prefab;

			Instantiate (prefab, new Vector2 (startX + i%enemiesPerLine * H_GAP, startY - i/enemiesPerLine * V_GAP), Quaternion.identity, transform);
		}
	}

	public void StartShooting ()
	{
		CancelInvoke (SHOOTING);
		InvokeRepeating (SHOOTING, shootingInterval, shootingInterval);
	}

	private void Shooting ()
	{
		if (transform.childCount == 0) return;
			
		transform.GetChild (Random.Range (0, transform.childCount - 1)).GetComponent<Enemy> ().canShoot = true;
	}

	public void MotherShipAppearence ()
	{
		CancelInvoke (MOTHERSHIP_MOVEMENT);
		InvokeRepeating (MOTHERSHIP_MOVEMENT, MOTHERSHIP_INTERVAL, MOTHERSHIP_INTERVAL);
	}

	private void MotherShipMovement ()
	{
		if (Mothership || GameManager.Instance.IsGamePaused) return;

		int rand = Random.Range (0, 10);
		if (rand < 4)
		{
			float dirRand = Random.Range (-1.0f, 1.0f);
			motherShipDirectionVector = dirRand > 0 ? Vector2.right : Vector2.left;
			
			Mothership = Instantiate (MotherShipPrefab, 
									 new Vector2 (dirRand > 0 ? -Model.HBound : Model.HBound, 
									 			  transform.GetChild (0).transform.position.y + V_GAP), 
									 Quaternion.identity);
		}
	}

	public void PauseEnemies (bool isPaused)
	{
		if (isPaused)
			CancelInvoke (SHOOTING);
		else
			InvokeRepeating (SHOOTING, shootingInterval, shootingInterval);
	}

	public void DestroyAllEnemiesAndBullets ()
	{
		Destroy (Mothership);

		for (int i = transform.childCount; i > 0; i--)
		{
			Destroy (transform.GetChild (i-1).gameObject);
		}

		GameObject[] Bullets = GameObject.FindGameObjectsWithTag (BULLETS_TAG);
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

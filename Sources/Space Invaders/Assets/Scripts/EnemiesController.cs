using UnityEngine;

public class EnemiesController : MonoBehaviour 
{
	//Constants
	private const float H_GAP = 0.8f;
	private const float V_GAP = 0.7f;
	private const float MOTHERSHIP_INTERVAL = 4.0f;
	private const float MOTHERSHIP_SPEED_DELTA = 1.5f;	
	private const int START_LINES = 5;
	private const int START_ENEMIES_PER_LINE = 5;	
	private const int ENEMIES_TYPE_4_LINES_NUM = 1;
	private const int ENEMIES_TYPE_3_LINES_NUM = 1;
	private const int ENEMIES_TYPE_2_LINES_NUM = 2;
    private const string MOTHERSHIP_MOVEMENT = "MotherShipMovement";
    private const string SHOOTING = "Shooting";    

	//Variables
    private int lines = START_LINES;
	private int enemiesPerLine = START_ENEMIES_PER_LINE;
	private int frameCounter = 0;
	private int speed = 15;
	private float shootingInterval = 2.0f;
	
	private float startX;
	private float startY = 3.0f;

	private float mothershipWidth;

	private Vector2 directionVector;
	private Vector2 motherShipDirectionVector;
	private Vector2 enemiesRelativeCenter;

	private Bounds enemiesBounds;

	private GameObject Mothership;

    //Prefabs
    public GameObject MotherShipPrefab;
	public GameObject Enemy1Prefab;
	public GameObject Enemy2Prefab;
	public GameObject Enemy3Prefab;
	public GameObject Enemy4Prefab;

	void Awake () 
	{
		directionVector = Random.Range (-1.0f, 1.0f) > 0 ? Vector2.right : Vector2.left;
	}
	
	void Update () 
	{
		if (GameManager.Instance.IsGamePaused) return;

		if (transform.childCount == 0 && !Mothership)
			GameManager.Instance.EndGame (true);

		frameCounter++;	

		if (frameCounter%Model.ENEMIES_UPDATE_FRAMES_DELTA == 0)
		{			
			transform.Translate (directionVector * speed * Time.deltaTime);

			if (Mothership) 
				Mothership.transform.Translate (motherShipDirectionVector * speed * MOTHERSHIP_SPEED_DELTA * Time.deltaTime);		

			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).GetComponent<Enemy>().UpdateSprite();
			}
		}
	}

	void FixedUpdate()
	{
		if (GameManager.Instance.IsGamePaused) return;

		if (frameCounter%Model.ENEMIES_UPDATE_FRAMES_DELTA == 0)
		{
			if (IsEnemiesReachedHBounds()) MoveEnemiesToNextLine ();	

			if (Mothership) 
			{
				if (Mothership.transform.position.x + mothershipWidth > Model.HBound ||
					Mothership.transform.position.x - mothershipWidth < -Model.HBound)
				{
					CancelInvoke (MOTHERSHIP_MOVEMENT);
					Destroy (Mothership);
				}
			}
		}		
	}

	public void CreateEnemies ()
	{
		startX =  - ( (enemiesPerLine - 1) * H_GAP)/2;

		for (int i = 0; i < enemiesPerLine * lines; i++)
		{
			GameObject prefab;

			//will be changed when levels loading will be available
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

		UpdateBounds ();
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
			Mothership = Instantiate (MotherShipPrefab, Vector2.zero, Quaternion.identity);
			mothershipWidth = Mothership.GetComponent<SpriteRenderer> ().bounds.extents.x;

			float dirRand = Random.Range (-1.0f, 1.0f);
			motherShipDirectionVector = dirRand > 0 ? Vector2.right : Vector2.left;

			Mothership.transform.position = new Vector2 (dirRand > 0 ? -Model.HBound + mothershipWidth : Model.HBound - mothershipWidth, 
									 			  transform.GetChild (0).transform.position.y + V_GAP);
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

		GameObject[] Bullets = GameObject.FindGameObjectsWithTag (Model.BULLETS_TAG);
		foreach (GameObject bullet in Bullets)
		{
			Destroy (bullet);
		}
	}

	public void UpdateEnemiesNumber(int level)
	{
		lines = START_LINES + level;
		enemiesPerLine = START_ENEMIES_PER_LINE + level;
	}

	public void UpdateBounds ()
	{
		enemiesBounds = new Bounds(transform.position, Vector3.zero);
		
		foreach (Renderer r in GetComponentsInChildren<Renderer>()) 
		{
			enemiesBounds.Encapsulate(r.bounds);
		}

		enemiesRelativeCenter = enemiesBounds.center - transform.position;
	}

	private bool IsEnemiesReachedHBounds ()
	{
		if ((directionVector == Vector2.right && transform.position.x + enemiesRelativeCenter.x + enemiesBounds.extents.x + H_GAP > Model.HBound) ||
			(directionVector == Vector2.left && transform.position.x + enemiesRelativeCenter.x - enemiesBounds.extents.x - H_GAP < - Model.HBound))
			return true;			
		return false;
	}

	private bool IsEnemiesReachedVBounds ()
	{
		if (transform.childCount > 0 && transform.position.y + enemiesRelativeCenter.y - enemiesBounds.extents.y - V_GAP < Model.PLAYERS_START_POS.y)
			return true;
		return false;
	}

	private void MoveEnemiesToNextLine ()
	{
		transform.position = new Vector2 (transform.position.x, transform.position.y - V_GAP);
		
		if (IsEnemiesReachedVBounds()) GameManager.Instance.EndGame (false); 
		else
		{
			directionVector.x *= -1;
			speed++;
			shootingInterval -= 0.1f;
			Model.bulletsSpeed += 0.1f;
		}
	}
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
	private const float GAME_START_DELAY = 3.0f;
	private const int LIVES_MAX_NUM = 3;

	private static GameManager instance = null;
	
	private GameObject PlayerInstance;
	private GameObject EnemiesControllerInstance;

	private int score;
	private int lives;

	private int level = 0;

	private float gameWidth;
	public float hBound;

	public bool isGamePaused = false;
	public bool isGameStartWaitingForDelay = false;

	public GameObject PlayerPrefab;
	public GameObject EnemiesControllerPrefab;

	public Button btnStart;
	public Button btnReStart;
	public Button btnReturn;

	public Text Score;
	public Text Lives;
	public Text WinGameOver;
	
	public GameObject startScreen;
	public GameObject gameScreen;
	public GameObject winGameOverScreen;
	public GameObject pauseScreen;
    
    public static GameManager Instance
	{
		get 
		{ 
			if (instance == null)
				instance = new GameObject ("GameManager").AddComponent<GameManager>();
			return instance;
		}
	}
	
	void Awake()
	{
		if (instance)
			DestroyImmediate(gameObject);
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}		

		gameWidth = (int) Camera.main.orthographicSize * Screen.width / Screen.height * 2;
		hBound = gameWidth/2;

		btnStart.onClick.AddListener (ResetGame);
		btnReStart.onClick.AddListener (ResetGame);
		btnReturn.onClick.AddListener(ReturnToNextLevel);

		startScreen.SetActive(true);
		gameScreen.SetActive(false);
		winGameOverScreen.SetActive(false);
		pauseScreen.SetActive (false);
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape) && gameScreen.activeSelf && !isGameStartWaitingForDelay)
		{
			isGamePaused = !isGamePaused;
			if (EnemiesControllerInstance) EnemiesControllerInstance.GetComponent<EnemiesController>().PauseEnemies (isGamePaused);
			pauseScreen.SetActive (isGamePaused);
		}
	}

	private void ResetGame ()
	{
		score = 0;
		lives = LIVES_MAX_NUM;

		UpdateScore ();
		UpdateLives();

		EnemiesControllerInstance = Instantiate(EnemiesControllerPrefab, Vector2.zero, Quaternion.identity);

		GameSetup ();
	}

	IEnumerator StartNewGame ()
	{
		isGameStartWaitingForDelay = isGamePaused = true;

		yield return new WaitForSeconds (GAME_START_DELAY);

		isGameStartWaitingForDelay = isGamePaused = false;

		EnemiesControllerInstance.GetComponent<EnemiesController>().StartShooting ();
		EnemiesControllerInstance.GetComponent<EnemiesController>().MotherShipAppearence();
	}

	public void ReturnToNextLevel()
	{
		level++;
		
		EnemiesControllerInstance = Instantiate(EnemiesControllerPrefab, Vector2.zero, Quaternion.identity);
		EnemiesControllerInstance.GetComponent<EnemiesController>().increaseEnemiesNumber (level);

		GameSetup();
	}

	private void GameSetup ()
	{
		InitPlayer ();		
		EnemiesControllerInstance.GetComponent<EnemiesController>().CreateEnemies ();

		startScreen.SetActive(false);
		gameScreen.SetActive(true);
		winGameOverScreen.SetActive(false);
		pauseScreen.SetActive (false);

		StopAllCoroutines();
		StartCoroutine(StartNewGame());
	}

	private void UpdateScore ()
	{
		Score.text = "SCORE: " + score;
	}

	private void UpdateLives ()
	{
		Lives.text = "LIVES: " + lives;
	}

	private void InitPlayer ()
	{
		PlayerInstance = Instantiate (PlayerPrefab, 
							new Vector2 (0f, -(Camera.main.orthographicSize - 1)), 
							Quaternion.identity);
	}

	public void OnEnemyHit (int points)
	{
		score += points;
		UpdateScore();
	}

	public void OnPlayerHit ()
	{
		if (lives > 1)
		{
			lives--;
			UpdateLives ();
			InitPlayer ();
		}
		else
			EndGame (false);
	}

	public void EndGame (bool isWin)
	{
		isGamePaused = false;

		startScreen.SetActive(false);
		gameScreen.SetActive(false);
		winGameOverScreen.SetActive(true);
		pauseScreen.SetActive (false);

		btnReturn.gameObject.SetActive(isWin);

		StopAllCoroutines();

		EnemiesControllerInstance.GetComponent<EnemiesController>().DestroyAllEnemiesAndBullets ();

		Destroy(PlayerInstance);
		Destroy(EnemiesControllerInstance);

		if (isWin)
			WinGameOver.text = "YOU WON!\n YOUR SCORE IS: " + score;
		else
			WinGameOver.text = "GAME OVER";
	}
}
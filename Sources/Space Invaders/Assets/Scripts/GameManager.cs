using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
	//Constants
	private const float GAME_START_DELAY = 3.0f;
	private const int LIVES_MAX_NUM = 3;
    private const string SCORE_TEXT = "SCORE: ";
    private const string LIVES_TEXT = "LIVES: ";
    private const string WIN_TEXT = "YOU WON!\n YOUR SCORE IS: ";
    private const string GAME_OVER_TEXT = "GAME OVER";
    private static GameManager instance = null;
	
	//Variables
	private bool isGamePaused = false;
    private bool isGameStartWaitingForDelay = false;

	private int score;
	private int lives;
	private int level = 0;

	private float elapsedSecondsFromLevelStart;

	private GameObject PlayerInstance;
	private GameObject EnemiesControllerInstance;

	//Prefabs
	public GameObject PlayerPrefab;
	public GameObject EnemiesControllerPrefab;

	public Button btnStart;
	public Button btnReStart;
	public Button btnReturn;

	public Text CountDown;
	public Text Score;
	public Text Lives;
	public Text WinGameOver;
	
	public GameObject StartScreen;
	public GameObject GameStartCountDown;
	public GameObject GameScreen;
	public GameObject WinGameOverScreen;
	public GameObject PauseScreen;
    
    public static GameManager Instance
	{
		get 
		{ 
			if (instance == null)
				instance = new GameObject ("GameManager").AddComponent<GameManager>();
			return instance;
		}
	}

    public bool IsGamePaused { get {return isGamePaused;} }

    void Awake()
	{
		if (instance)
			DestroyImmediate(gameObject);
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		btnStart.onClick.AddListener (ResetGame);
		btnReStart.onClick.AddListener (ResetGame);
		btnReturn.onClick.AddListener(ReturnToNextLevel);

		StartScreen.SetActive(true);
		GameStartCountDown.SetActive(false);
		GameScreen.SetActive(false);
		WinGameOverScreen.SetActive(false);
		PauseScreen.SetActive (false);
	}

	void Update()
	{
		if (GameStartCountDown.activeSelf)
		{
			CountDown.text = Mathf.Ceil(GAME_START_DELAY - elapsedSecondsFromLevelStart).ToString();
			CountDown.color = new Color (CountDown.color.r, CountDown.color.g, CountDown.color.b, (GAME_START_DELAY - elapsedSecondsFromLevelStart) / GAME_START_DELAY);
			elapsedSecondsFromLevelStart += Time.deltaTime;
		}
		else if (Input.GetKeyDown (KeyCode.Escape) && GameScreen.activeSelf && !isGameStartWaitingForDelay)
		{
			isGamePaused = !isGamePaused;
			if (EnemiesControllerInstance) EnemiesControllerInstance.GetComponent<EnemiesController>().PauseEnemies (isGamePaused);
			PauseScreen.SetActive (isGamePaused);
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

		GameStartCountDown.SetActive(false);

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
		PlayerInstance = Instantiate (PlayerPrefab, Model.PLAYERS_START_POS, Quaternion.identity);
		
		EnemiesControllerInstance.GetComponent<EnemiesController>().CreateEnemies ();

		StartScreen.SetActive(false);
		GameStartCountDown.SetActive(true);
		GameScreen.SetActive(true);
		WinGameOverScreen.SetActive(false);
		PauseScreen.SetActive (false);

		StopAllCoroutines();
		StartCoroutine(StartNewGame());
	}

	private void UpdateScore ()
	{
		Score.text = SCORE_TEXT + score;
	}

	private void UpdateLives ()
	{
		Lives.text = LIVES_TEXT + lives;
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
			PlayerInstance.gameObject.GetComponent<Player>().Explode();
		}
		else
			EndGame (false);
	}

	public void EndGame (bool isWin)
	{
		isGamePaused = false;
		elapsedSecondsFromLevelStart = 0;

		StartScreen.SetActive(false);
		GameStartCountDown.SetActive(false);
		GameScreen.SetActive(false);
		WinGameOverScreen.SetActive(true);
		PauseScreen.SetActive (false);

		btnReturn.gameObject.SetActive(isWin);

		StopAllCoroutines();

		EnemiesControllerInstance.GetComponent<EnemiesController>().DestroyAllEnemiesAndBullets ();

		Destroy(PlayerInstance);
		Destroy(EnemiesControllerInstance);

		if (isWin)
			WinGameOver.text = WIN_TEXT + score;
		else
			WinGameOver.text = GAME_OVER_TEXT;
	}
}
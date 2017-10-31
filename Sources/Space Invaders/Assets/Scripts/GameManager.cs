using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
	//Constants
	private const float GAME_START_DELAY = 3.0f;
	private const int LIVES_MAX_NUM = 3;
	private const int MAX_HIGH_SCORES = 10;
	private const string LEVEL_TEXT = "LEVEL: ";	
    private const string SCORE_TEXT = "SCORE: ";
    private const string LIVES_TEXT = "LIVES: ";
    private const string WIN_TEXT = "YOU WON!\n YOUR SCORE IS: ";
    private const string GAME_OVER_TEXT = "GAME OVER";
	private const string url = "http://space-invaders.bewebmaster.co.il/clients";
    private static GameManager instance = null;
	private enum GameState {MAIN, COUNT_DOWN, PAUSE, GAME, WIN_LOSE, HIGH_SCORES};
		
	//Variables
	private bool isGamePaused = false;
    private bool isGameStartWaitingForDelay = false;

	private int score;
	private int lives;
	private int level = 1;

	private float elapsedSecondsFromLevelStart;

	private GameObject PlayerInstance;
	private GameObject EnemiesControllerInstance;

	//Prefabs
	public GameObject PlayerPrefab;
	public GameObject EnemiesControllerPrefab;

	public Button btnStart;
	public Button btnHighScores;
	public Button btnNextLevel;
	public Button btnMainMenu;
	public Button btnAddNewResult;		
	public Button btnMainMenuFromHighScores;

	public Text CountDown;
	public Text Level;
	public Text Score;
	public Text Lives;
	public Text WinGameOver;
	public Text HighScores;
	public InputField Playername;
	
	public GameObject StartScreen;
	public GameObject GameStartCountDown;
	public GameObject PauseScreen;
	public GameObject GameScreen;
	public GameObject WinGameOverScreen;
	public GameObject HighScoresScreen;

	[Serializable]
	public class PlayersData
	{
		public List<PlayerData> playerDataList;
	}

	[Serializable]
	public class PlayerData
	{
		public string name;
		public string score;
	}
    
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

		btnStart.onClick.AddListener (GameSetup);
		btnHighScores.onClick.AddListener (ShowHighScores);
		btnMainMenu.onClick.AddListener (ResetGame);
		btnMainMenuFromHighScores.onClick.AddListener (ResetGame);
		btnNextLevel.onClick.AddListener(NextLevel);
		btnAddNewResult.onClick.AddListener(AddNewResult);

		ResetGame();
	}

	void Update()
	{
		if (isGameStartWaitingForDelay)
		{
			CountDown.text = Mathf.Ceil(GAME_START_DELAY - elapsedSecondsFromLevelStart).ToString();
			CountDown.color = new Color (CountDown.color.r, CountDown.color.g, CountDown.color.b, (GAME_START_DELAY - elapsedSecondsFromLevelStart) / GAME_START_DELAY);
			elapsedSecondsFromLevelStart += Time.deltaTime;
		}
		else if (Input.GetKeyDown (KeyCode.Escape) && GameScreen.activeSelf && !isGameStartWaitingForDelay)
		{
			isGamePaused = !isGamePaused;
			
			if (isGamePaused) SetGameState(GameState.PAUSE);
			else 			  SetGameState(GameState.GAME);
		}
	}

	private void SetGameState (GameState state)
	{
		StartScreen.SetActive(false);
		GameStartCountDown.SetActive(false);
		PauseScreen.SetActive (false);
		GameScreen.SetActive(false);
		WinGameOverScreen.SetActive(false);
		HighScoresScreen.SetActive(false);	

		isGamePaused = true;

		switch (state)
		{
			case GameState.MAIN:
				StartScreen.SetActive(true);	
				break;
			case GameState.COUNT_DOWN:
				GameStartCountDown.SetActive(true);
				GameScreen.SetActive(true);
				isGameStartWaitingForDelay = true;
				elapsedSecondsFromLevelStart = 0;
				break;
			case GameState.PAUSE:
				GameScreen.SetActive(true);
				PauseScreen.SetActive(true);
				if (EnemiesControllerInstance) EnemiesControllerInstance.GetComponent<EnemiesController>().PauseEnemies (isGamePaused);
				break;
			case GameState.GAME:
				GameScreen.SetActive(true);
				isGameStartWaitingForDelay = false;
				isGamePaused = false;
				break;
			case GameState.WIN_LOSE:
				WinGameOverScreen.SetActive(true);
				break;
			case GameState.HIGH_SCORES:
				HighScoresScreen.SetActive(true);	
				break;
		}
	}

	private void ResetGame ()
	{
		level = 1;
		score = 0;
		lives = LIVES_MAX_NUM;

		UpdateLevel ();
		UpdateScore ();
		UpdateLives();

		if (!PlayerInstance) PlayerInstance = Instantiate (PlayerPrefab, Model.PLAYERS_START_POS, Quaternion.identity, GameScreen.transform);
		else 				 PlayerInstance.transform.position = Model.PLAYERS_START_POS;

		if (!EnemiesControllerInstance) EnemiesControllerInstance = Instantiate (EnemiesControllerPrefab, Vector2.zero, Quaternion.identity);//, GameScreen.transform
		else 							EnemiesControllerInstance.transform.position = Vector2.zero;

		SetGameState (GameState.MAIN);
	}	

	private void GameSetup ()
	{
		EnemiesControllerInstance.GetComponent<EnemiesController>().UpdateEnemiesNumber (level);
		EnemiesControllerInstance.GetComponent<EnemiesController>().CreateEnemies ();

		StopAllCoroutines();
		StartCoroutine(StartNewGame());
	}

	IEnumerator StartNewGame ()
	{
		SetGameState (GameState.COUNT_DOWN);

		yield return new WaitForSeconds (GAME_START_DELAY);

		SetGameState (GameState.GAME);

		EnemiesControllerInstance.GetComponent<EnemiesController>().StartShooting ();
		EnemiesControllerInstance.GetComponent<EnemiesController>().MotherShipAppearence();
	}

	public void NextLevel()
	{
		level++;
		UpdateLevel ();
		GameSetup();
	}

	private void ShowHighScores ()
	{
		SetGameState(GameState.HIGH_SCORES);
		GetResultsList ();
	}

	private void UpdateLevel ()
	{
		Level.text = LEVEL_TEXT + level;
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
		StopAllCoroutines();
		SetGameState(GameState.WIN_LOSE);	

		EnemiesControllerInstance.GetComponent<EnemiesController>().DestroyAllEnemiesAndBullets ();

		btnNextLevel.gameObject.SetActive(isWin);
		Playername.gameObject.SetActive(!isWin);
		btnAddNewResult.gameObject.SetActive(!isWin);

		if (isWin)
			WinGameOver.text = WIN_TEXT + score;	
		else
			WinGameOver.text = GAME_OVER_TEXT;
	}

	void GetResultsList (string data = "") 
	{
		WWW www = new WWW(url);
        StartCoroutine(WaitForRequest(www, ShowResultsList));
    }

	void ShowResultsList (string data) 
	{
		SetGameState(GameState.HIGH_SCORES);
		
		HighScores.text = "Loading...";

		PlayersData playersData = JsonUtility.FromJson<PlayersData>(data);
		playersData.playerDataList.Sort((p1,p2)=>Convert.ToInt32(p1.score).CompareTo(Convert.ToInt32(p2.score)));
		playersData.playerDataList.Reverse();

		for (int i = 0; i < playersData.playerDataList.Count && i < MAX_HIGH_SCORES; i++)
		{
			HighScores.text += (i+1) + ".\t" + playersData.playerDataList[i].name + '\t' + playersData.playerDataList[i].score + '\n';
		}
    }

	void AddNewResult () 
	{
		WWWForm form = new WWWForm();
    	form.AddField("name", Playername.text);
        form.AddField("score", score.ToString());
        WWW www = new WWW(url, form);

        StartCoroutine(WaitForRequest(www, GetResultsList));
    }

    IEnumerator WaitForRequest(WWW www, Action<string> callback)
    {
        yield return www;

		if (string.IsNullOrEmpty(www.error))
        {
            callback(www.text);				
        } 
		else
		{
			#if UNITY_EDITOR
            	Debug.LogError("WWW Error: "+ www.error);
			#endif
        }
    }
}
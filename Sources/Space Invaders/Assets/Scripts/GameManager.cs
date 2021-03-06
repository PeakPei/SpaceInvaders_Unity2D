﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
	#region Constants
	private const float GAME_START_DELAY = 3.0f;
	private const int LIVES_MAX_NUM = 3;
	private const int MAX_HIGH_SCORES = 10;
	private const string LEVEL_TEXT = "LEVEL: ";	
    private const string SCORE_TEXT = "SCORE: ";
    private const string LIVES_TEXT = "LIVES: ";
    private const string WIN_TEXT = "YOU WON!\n YOUR SCORE IS: ";
    private const string GAME_OVER_TEXT = "GAME OVER";
	private const string START_NEW_GAME = "StartNewGame";
	private const string UPDATE_ENEMIES = "UpdateEnemies";
	private const string url = "http://space-invaders.bewebmaster.co.il/clients";
	private enum GameState {MAIN, COUNT_DOWN, PAUSE, GAME, WIN_LOSE, HIGH_SCORES};
	#endregion
		
	
	#region Variables	
    private bool isGameStartWaitingForDelay = false;

	private int score;
	private int lives;
	private int level = 1;

	private float elapsedSecondsFromLevelStart;

	private GameObject PlayerInstance;
	private GameObject EnemiesControllerInstance;

	private EnemiesController EnemiesControllerClass;
	private static GameManager instance = null;
	#endregion


	#region Prefabs
	[Header("Prefabs")]
	public GameObject PlayerPrefab;
	public GameObject EnemiesControllerPrefab;
	#endregion

	[Header("Buttons")]
	public Button btnStart;
	public Button btnHighScores;
	public Button btnNextLevel;
	public Button btnMainMenu;
	public Button btnAddNewResult;		
	public Button btnMainMenuFromHighScores;

	[Header("Text fields")]
	public Text CountDown;
	public Text Level;
	public Text Score;
	public Text Lives;
	public Text WinGameOver;
	public Text HighScoresListNames;
	public Text HighScoresListScores;
	public InputField Playername;
	
	[Header("Screens")]
	public GameObject StartScreen;
	public GameObject CountDownScreen;
	public GameObject PauseScreen;
	public GameObject GameScreen;
	public GameObject WinGameOverScreen;
	public GameObject HighScoresScreen;
	
	
	#region Properties
	public static GameManager Instance
	{
		get 
		{ 
			if (instance == null)
				instance = new GameObject ("GameManager").AddComponent<GameManager>();
			return instance;
		}
	}
	#endregion

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
		btnHighScores.onClick.AddListener (OnBtnHighScores);
		btnMainMenu.onClick.AddListener (ResetGame);
		btnMainMenuFromHighScores.onClick.AddListener (ResetGame);
		btnNextLevel.onClick.AddListener(NextLevel);
		btnAddNewResult.onClick.AddListener(AddNewHighScore);

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
			Model.IsGamePaused = !Model.IsGamePaused;
			
			if (Model.IsGamePaused) SetGameState(GameState.PAUSE);
			else 			 		SetGameState(GameState.GAME);

			if (EnemiesControllerInstance) EnemiesControllerClass.PauseEnemies (Model.IsGamePaused);
		}
	}

	private void SetGameState (GameState state)
	{
		StartScreen.SetActive(false);
		CountDownScreen.SetActive(false);
		PauseScreen.SetActive (false);
		GameScreen.SetActive(false);
		WinGameOverScreen.SetActive(false);
		HighScoresScreen.SetActive(false);	

		Model.IsGamePaused = true;

		switch (state)
		{
			case GameState.MAIN:
				StartScreen.SetActive(true);	
				break;
			case GameState.COUNT_DOWN:
				CountDownScreen.SetActive(true);
				GameScreen.SetActive(true);
				isGameStartWaitingForDelay = true;
				elapsedSecondsFromLevelStart = 0;
				break;
			case GameState.PAUSE:
				GameScreen.SetActive(true);
				PauseScreen.SetActive(true);
				break;
			case GameState.GAME:
				GameScreen.SetActive(true);
				isGameStartWaitingForDelay = false;
				Model.IsGamePaused = false;
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

		if (!EnemiesControllerInstance) 
		{
			EnemiesControllerInstance = Instantiate (EnemiesControllerPrefab, Vector2.zero, Quaternion.identity, GameScreen.transform);
			EnemiesControllerClass 	  = EnemiesControllerInstance.GetComponent<EnemiesController>();
		}
		else
			EnemiesControllerClass.Reset();

		SetGameState (GameState.MAIN);
	}	

	private void GameSetup ()
	{
		EnemiesControllerClass.Reset();
		EnemiesControllerClass.UpdateEnemiesNumber (level);
		EnemiesControllerClass.CreateEnemies ();

		StopAllCoroutines();
		StartCoroutine(START_NEW_GAME);
	}

	IEnumerator StartNewGame ()
	{
		SetGameState (GameState.COUNT_DOWN);

		yield return new WaitForSeconds (GAME_START_DELAY);

		SetGameState (GameState.GAME);

		EnemiesControllerClass.StartShooting ();
		EnemiesControllerClass.MotherShipAppearence();
	}

	private void NextLevel()
	{
		level++;
		UpdateLevel ();
		GameSetup();
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

		StopCoroutine(UPDATE_ENEMIES);
		StartCoroutine(UPDATE_ENEMIES);
	}

	IEnumerator UpdateEnemies ()
	{
		yield return new WaitForSeconds(Model.EXPLOSION_TIME_OUT);
		EnemiesControllerClass.UpdateBounds();
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

		EnemiesControllerClass.DestroyAllEnemiesAndBullets ();

		btnNextLevel.gameObject.SetActive(isWin);
		Playername.gameObject.SetActive(!isWin);
		btnAddNewResult.gameObject.SetActive(!isWin);

		if (isWin)
			WinGameOver.text = WIN_TEXT + score;	
		else
			WinGameOver.text = GAME_OVER_TEXT;
	}

	private void OnBtnHighScores ()
	{
		ShowHighScoresScreen();
		GetHighScoresList ();
	}

	private void ShowHighScoresScreen ()
	{
		SetGameState(GameState.HIGH_SCORES);

		HighScoresListNames.text = "Loading...";
		HighScoresListNames.alignment = TextAnchor.MiddleRight;
		HighScoresListScores.text = "";		
	}

	private void GetHighScoresList (string data = "") 
	{
		WWW www = new WWW(url);
        StartCoroutine(WaitForRequest(www, ShowHighScoresList));
    }

	private void ShowHighScoresList (string data) 
	{
		HighScoresListNames.text = "";
		HighScoresListNames.alignment = TextAnchor.MiddleLeft;

		PlayersData playersData = JsonUtility.FromJson<PlayersData>(data);
		playersData.playerDataList.Sort((p1,p2)=>Convert.ToInt32(p1.score).CompareTo(Convert.ToInt32(p2.score)));
		playersData.playerDataList.Reverse();

		for (int i = 0; i < playersData.playerDataList.Count && i < MAX_HIGH_SCORES; i++)
		{
			HighScoresListNames.text += (i+1) + ".\t" + playersData.playerDataList[i].name + '\n';
			HighScoresListScores.text += playersData.playerDataList[i].score + '\n';
		}
    }

	private void AddNewHighScore () 
	{
		WWWForm form = new WWWForm();
    	form.AddField("name", Playername.text);
        form.AddField("score", score.ToString());
        WWW www = new WWW(url, form);

        StartCoroutine(WaitForRequest(www, GetHighScoresList));
		ShowHighScoresScreen();
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
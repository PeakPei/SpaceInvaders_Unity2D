using UnityEngine;

public static class Model : object 
{
	#region Constants
	public const int ENEMIES_UPDATE_FRAMES_DELTA = 25;
	public const int BULLETS_UPDATE_FRAMES_DELTA = 15;

	public const int MOTHERSHIP_MIN_POINTS = 50;
	public const int MOTHERSHIP_MAX_POINTS = MOTHERSHIP_MIN_POINTS * 2;

	private const int START_BULLET_SPEED = 3;

	public const float EXPLOSION_TIME_OUT = .5f;
	
    public const string PLAYER_TAG = "Player";
	public const string ENEMIES_TAG = "Enemy";
	public const string BULLETS_TAG = "Bullet";
	#endregion
		
	
	#region Variables
	private static bool isGamePaused = false;
	public static float bulletsSpeed = START_BULLET_SPEED;
	#endregion


	#region Properties
	public static Vector2 ScreenLimit 
	{
		get 
		{
			return new Vector2 (Camera.main.orthographicSize * Screen.width / Screen.height, 
								Camera.main.orthographicSize);
		}
	}

	public static Vector2 PLAYERS_START_POS { get { return new Vector2(0f, -(ScreenLimit.y - 1));}}

	public static bool IsGamePaused { get {return isGamePaused;} 
							   		  set {isGamePaused = value;}}
	#endregion


	#region Methods
	public static void ResetBulletsSpeed()
	{
		bulletsSpeed = START_BULLET_SPEED;
	}
	#endregion
}
using UnityEngine;

public static class Model : object 
{
	public const int ENEMIES_UPDATE_FRAMES_DELTA = 25;
	public const int BULLETS_UPDATE_FRAMES_DELTA = 15;

	public const int MOTHERSHIP_MIN_POINTS = 50;
	public const int MOTHERSHIP_MAX_POINTS = MOTHERSHIP_MIN_POINTS * 2;

	public const float EXPLOSION_TIME_OUT = .5f;
	
    public const string PLAYER_TAG = "Player";
	public const string ENEMIES_TAG = "Enemy";
	public const string BULLETS_TAG = "Bullet";
	

	public static float bulletsSpeed = 3;

	public static float VBound { get { return Camera.main.orthographicSize;}}

	public static float HBound { get { return Camera.main.orthographicSize * Screen.width / Screen.height;}}

	public static Vector2 PLAYERS_START_POS { get { return new Vector2(0f, -(VBound - 1));}}
}
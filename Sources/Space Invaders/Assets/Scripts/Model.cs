using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Model : object 
{
	public const int ENEMIES_UPDATE_FRAMES_DELTA = 25;
	public const int BULLETS_UPDATE_FRAMES_DELTA = 15;

	public const int MOTHERSHIP_MIN_POINTS = 50;
	public const int MOTHERSHIP_MAX_POINTS = MOTHERSHIP_MIN_POINTS * 2;
	
	public static float bulletsSpeed = 3;

	public static float VBound { get { return Camera.main.orthographicSize;}}

	public static float HBound { get { return (int) Camera.main.orthographicSize * Screen.width / Screen.height;}}
}

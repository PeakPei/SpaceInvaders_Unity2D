using UnityEngine;

public class Enemy : MonoBehaviour 
{
	public bool canShoot = false;
	public int points;
	public GameObject Bullet;

	void Update () 
	{		
		if (canShoot)
		{
			Instantiate(Bullet, new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y), Quaternion.identity);
			canShoot = false;
		}
	}
}

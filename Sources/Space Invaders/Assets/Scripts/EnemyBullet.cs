using UnityEngine;

public class EnemyBullet : Bullet 
{
	void Start ()
	{
		directionVector = Vector2.down;
	}

	protected override void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			GameManager.Instance.OnPlayerHit();
			Destroy(other.gameObject);			
			Destroy(gameObject);
		}
	}
}

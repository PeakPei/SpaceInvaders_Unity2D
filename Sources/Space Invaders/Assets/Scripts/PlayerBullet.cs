using UnityEngine;

public class PlayerBullet : Bullet 
{
	void Start()
	{
		directionVector = Vector2.up;
	}

	protected override void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.CompareTag("Enemy"))
		{
			GameManager.Instance.OnEnemyHit(other.gameObject.GetComponent<Enemy>().GetPoints());
			Destroy(other.gameObject);
			Destroy(gameObject);
		}
	}
}
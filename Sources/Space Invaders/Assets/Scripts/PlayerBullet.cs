using UnityEngine;

public class PlayerBullet : Bullet 
{
	void Start()
	{
		directionVector = Vector2.up;
	}

	protected override void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.CompareTag(Model.ENEMIES_TAG))
		{
			GameManager.Instance.OnEnemyHit(other.gameObject.GetComponent<Enemy>().GetPoints());
			other.gameObject.GetComponent<Enemy>().Explode();
			
			Destroy(gameObject);
		}
	}
}
using UnityEngine;

public class EnemyBullet : Bullet 
{
    private const string PLAYER_TAG = "Player";

    void Start ()
	{
		directionVector = Vector2.down;
	}

	protected override void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag(PLAYER_TAG))
		{
			GameManager.Instance.OnPlayerHit();
			
			Destroy(gameObject);
		}
	}
}

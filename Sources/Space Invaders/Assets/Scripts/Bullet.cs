using UnityEngine;

public class Bullet : MonoBehaviour 
{
	private int frameCounter = 0;
	private SpriteRenderer spriteRenderer;
	protected Vector2 directionVector;

	void Awake ()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update () 
	{
		if (Model.IsGamePaused) return;

		transform.Translate (directionVector * Model.bulletsSpeed * Time.deltaTime);	

		if (frameCounter%Model.BULLETS_UPDATE_FRAMES_DELTA == 0)
			spriteRenderer.flipY = !spriteRenderer.flipY;

		frameCounter++;
	}
	
	void LateUpdate()
	{
		if (Model.IsGamePaused) return;
		
		if (transform.position.y >  Model.ScreenLimit.y ||
			transform.position.y < -Model.ScreenLimit.y)
				Destroy(gameObject);
	}

	protected virtual void OnTriggerEnter2D(Collider2D other){}
}
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
		transform.Translate (directionVector * Model.bulletsSpeed * Time.deltaTime);	

		if (frameCounter%Model.BULLETS_UPDATE_FRAMES_DELTA == 0)
			spriteRenderer.flipY = !spriteRenderer.flipY;

		frameCounter++;
	}
	
	void LateUpdate()
	{
		if (transform.position.y >  Model.VBound ||
			transform.position.y < -Model.VBound)
				Destroy(gameObject);
	}

	protected virtual void OnTriggerEnter2D(Collider2D other){}
}
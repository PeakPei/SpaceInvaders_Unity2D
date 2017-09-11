using UnityEngine;

public class Bullet : MonoBehaviour 
{
	private int speed = 5;
	private float vBound;

	protected Vector2 directionVector;

	void Awake ()
	{
		vBound = Camera.main.orthographicSize;
	}

	void Update () 
	{
		transform.Translate (directionVector * speed * Time.deltaTime);

		if (transform.position.y >  vBound ||
			transform.position.y < -vBound)
			Destroy(gameObject);
	}

	protected virtual void OnTriggerEnter2D(Collider2D other){}
}

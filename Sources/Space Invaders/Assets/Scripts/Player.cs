using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	private int speed = 5;

	public GameObject Bullet;
	
	void Update () 
	{
		if (GameManager.Instance.isGamePaused) return;
		
		if 		(Input.GetKey (KeyCode.RightArrow) && transform.position.x < Model.HBound + gameObject.GetComponent<SpriteRenderer>().bounds.size.x/2)
			transform.Translate (Vector2.right * speed * Time.deltaTime);
		else if (Input.GetKey (KeyCode.LeftArrow)  && transform.position.x > -(Model.HBound + gameObject.GetComponent<SpriteRenderer>().bounds.size.x/2))
			transform.Translate (Vector2.left * speed * Time.deltaTime);
		
		if (Input.GetKeyDown (KeyCode.Space))
			Instantiate(Bullet, new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y), Quaternion.identity);
	}	

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.CompareTag("Enemy"))
		{
			GameManager.Instance.EndGame (false);
		}
	}
}
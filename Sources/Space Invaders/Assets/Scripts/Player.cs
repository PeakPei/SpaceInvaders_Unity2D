using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	private int speed = 5;
	private float hBound;

	public GameObject Bullet;

	void Awake ()
	{
		hBound = GameManager.Instance.hBound - transform.localScale.x/2;
	}
	
	void Update () 
	{
		if (GameManager.Instance.isGamePaused) return;
		
		if 		(Input.GetKey (KeyCode.RightArrow) && transform.position.x < hBound + gameObject.GetComponent<SpriteRenderer>().bounds.size.x/2)
			transform.Translate (Vector2.right * speed * Time.deltaTime);
		else if (Input.GetKey (KeyCode.LeftArrow)  && transform.position.x > -(hBound + gameObject.GetComponent<SpriteRenderer>().bounds.size.x/2))
			transform.Translate (Vector2.left * speed * Time.deltaTime);
		
		if (Input.GetKeyDown (KeyCode.Space))
			Instantiate(Bullet, new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y), Quaternion.identity);
	}	
}
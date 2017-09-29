using UnityEngine;

public class Enemy : MonoBehaviour 
{
	private int currentSpriteID = 0;
	private SpriteRenderer spriteRenderer;

	public bool isMotherShip = false;
	public bool canShoot = false;
	public int points;

	public Sprite[] Sprites;	
	public GameObject Bullet;

	void Awake ()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();

		UpdateSprite();
	}
	
	void Update () 
	{		
		if (!isMotherShip && canShoot)
		{
			Instantiate(Bullet, new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y), Quaternion.identity);
			canShoot = false;
		}
	}

	public void UpdateSprite()
	{
		spriteRenderer.sprite = Sprites[currentSpriteID];

		currentSpriteID++;
		if (currentSpriteID == Sprites.Length) currentSpriteID = 0;
	}

	public int GetPoints ()
	{
		if (isMotherShip) points = Random.Range(Model.MOTHERSHIP_MIN_POINTS, Model.MOTHERSHIP_MAX_POINTS);

		Debug.Log(points);

		return points;
	}
}

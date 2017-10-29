using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{
	//Constants
    private const float EXPLOSION_TIME_OUT = 0.5f;

	//Variables
    private bool isExplode = false;
	private int currentSpriteStateID = 0; //Enemies have two sprite states for "animation" when moving
	private SpriteRenderer spriteRenderer;

	public bool isMotherShip = false;
	public bool canShoot = false;
	public int points;

	public Sprite[] StatesSprites;
	public Sprite ExplosionSprite;
	public GameObject Bullet;

    void Awake ()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();

		UpdateSprite();
	}
	
	void Update () 
	{		
		if (canShoot && !isMotherShip && !isExplode)
		{
			Instantiate(Bullet, new Vector2 (transform.position.x, transform.position.y), Quaternion.identity);
			canShoot = false;
		}
	}

	public void UpdateSprite()
	{
		if (isExplode) return;

		spriteRenderer.sprite = StatesSprites[currentSpriteStateID];

		currentSpriteStateID++;
		if (currentSpriteStateID == StatesSprites.Length) currentSpriteStateID = 0;
	}

	public int GetPoints ()
	{
		if (isMotherShip) points = Random.Range(Model.MOTHERSHIP_MIN_POINTS, Model.MOTHERSHIP_MAX_POINTS);

		return points;
	}

    public void Explode ()
    {
		isExplode = true;
		spriteRenderer.sprite = ExplosionSprite;
		Destroy(gameObject, EXPLOSION_TIME_OUT);
    }
}
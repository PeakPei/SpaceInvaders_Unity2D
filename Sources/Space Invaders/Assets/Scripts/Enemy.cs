using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{
	#region Variables
	private bool isExplode = false;
	private int currentSpriteStateID = 0; //Enemies have two sprite states for "animation" when moving
	private SpriteRenderer spriteRenderer;
	
	[SerializeField]
	private bool isMotherShip = false;
	[SerializeField]
	private int points;
	
	public bool canShoot = false;
	#endregion
	

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
		Destroy(gameObject, Model.EXPLOSION_TIME_OUT);
    }
}
using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	public Sprite dmgSprite; // Displays once wall is hit
	public int hp = 4;

	private SpriteRenderer spriteRenderer;


	// Use this for initialization
	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void DamageWall (int loss) {
		spriteRenderer.sprite = dmgSprite;	// Set sprite to dmg'd sprite
		hp -= loss;							// Subtract hp lost
		if (hp <= 0)
			gameObject.SetActive(false);	// Removes game object if destroyed
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactive {

	public bool isHeld = false;
	public bool isThrown = false;
	public bool isBouncing = false;
	public bool isAttacking = false;
	public bool flipped = false;

	public float xOffset;
	public float yOffset;
	public float zRotation;
	public float restingHeight;
	public float restingRotation;
	public int thrownDamage;
	public int thrownKnockback;
	public string type;

	protected int state = 0;

	public Sprite bloodySprite1;
	public Sprite bloodySprite2;
	public Sprite bloodySprite3;

	public BoxCollider2D pickupCollider;
	public Collider2D hitCollider;
	public GameObject frontHand;
	public GameObject backHand;

	protected AudioSource source;
	public AudioClip throwSound;
	public AudioClip throwImpact;
	public AudioClip craftSound;
	public AudioClip pickupSound;
	public AudioClip swapSound;

	public GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		source = gameObject.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (isBouncing) {
			Rigidbody2D itemBody = gameObject.GetComponent<Rigidbody2D> ();
			float xVel = itemBody.velocity.x;
			float yVel = itemBody.velocity.y;
			//Once it has come to rest
			if (Mathf.Approximately (xVel, 0.0f) && Mathf.Approximately (yVel, 0.0f)) {
				isBouncing = false;
				//Turn physics effects off for the item
				itemBody.bodyType = RigidbodyType2D.Kinematic;

				pickupCollider.enabled = true;
				hitCollider.enabled = false;
				gameObject.layer = 13;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (isThrown) {
			if (other.gameObject.tag == "Enemy") {
				if (throwImpact) {
					source.PlayOneShot (throwImpact);
				}
				other.gameObject.GetComponent<Enemy> ().takeHit (thrownDamage, thrownKnockback);
				if ((state == 0) && (bloodySprite1 != null)) {
					GetComponent<SpriteRenderer> ().sprite = bloodySprite1;
					state++;
				} else if ((state == 1) && (bloodySprite2 != null)) {
					GetComponent<SpriteRenderer> ().sprite = bloodySprite2;
					state++;
				} else if ((state == 2) && (bloodySprite3 != null)) {
					GetComponent<SpriteRenderer> ().sprite = bloodySprite3;
					state++;
				}
			}

			isThrown = false;
			isBouncing = true;
			gameObject.layer = 11;
		}
	}

	public void disableAnimator() {
		Animator anim = GetComponent<Animator> ();
		if (anim) {
			anim.enabled = false;
		}
	}

	public void enableAnimator() {
		Animator anim = GetComponent<Animator> ();
		if (anim) {
			anim.enabled = true;
		}
	}

	override public void disableHighlight() {
		StopCoroutine ("highlightGlow");
		GetComponent<SpriteOutline> ().enabled = false;
	}

	override public void enableHighlight() {
		GetComponent<SpriteOutline> ().enabled = true;
		StartCoroutine ("highlightGlow");
	}

	override public void updateHighlightColor() {
		GameObject heldItem = player.GetComponent<PlayerController> ().heldItem;
		if (heldItem && !RecipeBook.canCraft(type, heldItem.GetComponent<Item>().type)) {
			GetComponent<SpriteOutline> ().color = negativeColor;
		} else {
			GetComponent<SpriteOutline> ().color = positiveColor;
		}

	}

	public void flipItem() {
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
		flipped = !flipped;
	}

	public void pickupItem(bool playerFlipX) {
		if (pickupSound && source) {
			source.PlayOneShot (pickupSound);
		}
			
		if (playerFlipX != flipped)
		{
			flipItem ();
		}
			
		gameObject.layer = 16;

		//disable pickup trigger, enable hit trigger
		pickupCollider.enabled = false;
		hitCollider.enabled = true;

		enableAnimator ();

		frontHand.SetActive (true);
		backHand.SetActive (true);

		isHeld = true;
	}

	public void dropItem() {
		transform.parent = null;
		pickupCollider.enabled = true;
		hitCollider.enabled = false;
		isHeld = false;

		disableAnimator ();

		frontHand.SetActive (false);
		backHand.SetActive (false);

		moveToResting ();
	}

	public void moveToResting() {
		transform.position = new Vector2 (transform.position.x, restingHeight);
		transform.eulerAngles = new Vector3 (0, 0, restingRotation);
	}

	public void playCraftingSound() {
		//Since this object was just created, the source variable is not initialized yet
		//so we need to get the AudioSource directly
		if (craftSound) {
			gameObject.GetComponent<AudioSource>().PlayOneShot (craftSound);
		}
	}

	public void playSwappingSound() {
		if (craftSound) {
			source.PlayOneShot (swapSound);
		}
	}

	public void playThrowSound() {
		if (throwSound) {
			source.PlayOneShot (throwSound);
		}
	}

	public virtual void use() {

	}

	IEnumerator highlightGlow() {
		SpriteOutline outline = GetComponent<SpriteOutline> ();
		//Fade out the highlight
		for (float f = 1f; f >= 0.3; f -= 0.01f) {
			Color c = outline.color;
			c.a = f;
			outline.color = c;

			yield return null;
		}
		//Fade the highlight back in
		for (float f = 0.3f; f <= 1; f += 0.01f) {
			Color c = outline.color;
			c.a = f;
			outline.color = c;

			yield return null;
		}
		Color orig = outline.color;
		orig.a = 1;
		outline.color = orig;


		StartCoroutine ("highlightGlow");
	}
}

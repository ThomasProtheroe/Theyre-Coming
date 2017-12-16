using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	public bool isHeld = false;
	public bool isThrown = false;
	public bool isBouncing = false;
	public bool isAttacking = false;

	public float xOffset;
	public float yOffset;
	public float zRotation;
	public float restingHeight;
	public float restingRotation;

	public int thrownDamage;

	public BoxCollider2D pickupCollider;
	public BoxCollider2D hitCollider;

	public GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
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
				other.gameObject.GetComponent<Enemy> ().takeDamage (thrownDamage);
			}

			isThrown = false;
			isBouncing = true;
			gameObject.layer = 11;
		}
	}

	public void pickupItem(bool playerFlipX) {
		SpriteRenderer itemSprite = gameObject.GetComponent<SpriteRenderer> ();
		itemSprite.flipX = playerFlipX;

		//disable pickup trigger, enable hit trigger
		pickupCollider.enabled = false;
		hitCollider.enabled = true;

		//Make sure the animator knows which way we are facing when the item is picked up
		Animator anim = GetComponent<Animator> ();
		if (anim) {
			anim.SetBool ("FacingRight", !playerFlipX);
		}

		isHeld = true;
	}

	public void dropItem() {
		transform.parent = null;
		pickupCollider.enabled = true;
		hitCollider.enabled = false;
		isHeld = false;
		moveToResting ();
	}

	public void moveToResting() {
		transform.position = new Vector2 (transform.position.x, restingHeight);
		transform.eulerAngles = new Vector3 (0, 0, restingRotation);
	}

	public void flipItem() {
		Animator anim = gameObject.GetComponent<Animator> ();
		SpriteRenderer itemSprite = gameObject.GetComponent<SpriteRenderer> ();
		itemSprite.flipX = !itemSprite.flipX;
		if (anim) {
			anim.SetBool ("FacingRight", !itemSprite.flipX);
		}
	}

	public virtual void use() {

	}
}

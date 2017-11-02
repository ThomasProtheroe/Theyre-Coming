using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public int playerSpeed = 5;
	public int xThrowStrength = 500;
	public int yThrowStrength = 100;
	public float heldItemOffset = 0.4f;
	public GameObject floorItem;
	public GameObject heldItem;
	private float moveX;
	private Animator anim;
	private Rigidbody2D rigidBody;
	private SpriteRenderer playerSprite;

	void Start() {
		anim = gameObject.GetComponent<Animator> ();
		rigidBody = gameObject.GetComponent<Rigidbody2D> ();
		playerSprite = gameObject.GetComponent<SpriteRenderer> ();
	}

	// Update is called once per frame
	void Update () {
		playerMove ();
		checkPlayerInput ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Item") {
			floorItem = other.gameObject;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.tag == "Item") {
			floorItem = null;
		}
	}

	void checkPlayerInput() {
		if (floorItem) {
			checkPickup ();
		}
		if (heldItem) {
			checkDrop ();
			checkThrow ();
		}
	}

	void playerMove () {
		float moveInput = Input.GetAxis ("Horizontal");

		if (moveInput > 0.0f) {
			moveX = 1.0f;
		} else if (moveInput < 0.0f) {
			moveX = -1.0f;
		} else {
			moveX = 0.0f;
		}

		if (playerSprite.flipX == true && moveX > 0.0f) {
			flipPlayer ();
		} else if (playerSprite.flipX == false && moveX < 0.0f) {
			flipPlayer ();
		}

		rigidBody.velocity = new Vector2 (moveX * playerSpeed, rigidBody.velocity.y);

		if (moveX != 0.0f) {
			anim.SetInteger("State", 1);
		}
		else if (moveX == 0.0f) {
			anim.SetInteger("State", 0);
		}

		moveHeldItem ();
	}

	void flipPlayer() {
		playerSprite.flipX = !playerSprite.flipX;

		if (heldItem) {
			heldItem.GetComponent<ItemController> ().flipItem ();
		}
	}

	void checkThrow() {
		if (Input.GetButton("up") && Input.GetButtonDown("use")) {
			Rigidbody2D body = heldItem.GetComponent<Rigidbody2D>();
			heldItem.AddComponent (typeof(BoxCollider2D));

			//Set throw direction
			int tempThrowStrength;
			if (playerSprite.flipX) {
				tempThrowStrength = xThrowStrength * -1;
			} else {
				tempThrowStrength = xThrowStrength;
			}

			body.bodyType = RigidbodyType2D.Dynamic;
			body.AddForce (new Vector2 (tempThrowStrength, yThrowStrength));

			heldItem.GetComponent<ItemController> ().isThrown = true;
			heldItem = null;
		}
	}

	void checkPickup() {
		if (Input.GetButtonDown ("use")) {
			heldItem = floorItem;
		}
	}

	void checkDrop() {
		if (Input.GetButtonDown ("drop")) {
			heldItem.transform.position = new Vector2 (heldItem.transform.position.x, -2.77f);
			heldItem = null;
		}
	}

	void moveHeldItem() {
		//Debug.Log (heldItem);
		if (heldItem) {
			Transform playerPos = gameObject.transform;
			float newX = playerPos.position.x;
			float newY = playerPos.position.y;
			float heldItemOffset = 0.5f;

			if (playerSprite.flipX) {
				heldItemOffset *= -1;
			}
			newX += heldItemOffset;

			heldItem.transform.position = new Vector2 (newX, newY);
		}
	}	
}

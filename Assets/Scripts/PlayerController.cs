using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public int playerSpeed = 5;
	public int xThrowStrength = 500;
	public int yThrowStrength = 100;
	public int health = 5;

	public List<GameObject> nearItems = new List<GameObject>();
	public GameObject heldItem;
	public GameObject stashedItem;

	private float moveX;
	private Animator anim;
	private Rigidbody2D rigidBody;
	private SpriteRenderer playerSprite;
	private bool isAttacking;

	void Start() {
		anim = gameObject.GetComponent<Animator> ();
		rigidBody = gameObject.GetComponent<Rigidbody2D> ();
		playerSprite = gameObject.GetComponent<SpriteRenderer> ();
		isAttacking = false;
	}

	// Update is called once per frame
	void Update () {
		if (heldItem) {
			isAttacking = heldItem.GetComponent<Item> ().isAttacking;
		} else {
			isAttacking = false;
		}

		if (!isAttacking) {
			playerMove ();
			checkPlayerInput ();
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Item") {
			nearItems.Add (other.gameObject);
		} else if (other.gameObject.tag == "Enemy") {
			takeDamage (1);
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.tag == "Item") {
			nearItems.Remove(other.gameObject);
		}
	}

	void checkPlayerInput() {
		if (nearItems.Count != 0 && !heldItem) {
			checkPickup ();
		}
		if (heldItem) {
			//Since we are sharing a key for both, only check for item use if not throwing item
			if (!checkThrow ()) {
				checkUse ();
			}
			checkDrop ();
		}
		checkSwapItem ();
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
	}

	void flipPlayer() {
		playerSprite.flipX = !playerSprite.flipX;

		if (heldItem) {
			heldItem.GetComponent<Item> ().flipItem ();
			positionHeldItem ();
		}
	}

	void takeDamage(int damage) {
		health -= damage;
		Debug.Log (health);
		if (health <= 0) {
			gameOver ();
		}
	}

	void gameOver() {
		Destroy (gameObject);
	}

	void checkUse() {
		if (Input.GetButtonDown ("use")) {
			rigidBody.velocity = new Vector2 (0, 0);
			anim.SetInteger("State", 0);
			heldItem.GetComponent<Item>().use ();
		}
	}

	bool checkThrow() {
		if (Input.GetButton("up") && Input.GetButtonDown("use")) {
			heldItem.transform.parent = null;
			Rigidbody2D body = heldItem.GetComponent<Rigidbody2D>();

			//Set throw direction	
			int tempThrowStrength;
			if (playerSprite.flipX) {
				tempThrowStrength = xThrowStrength * -1;
			} else {
				tempThrowStrength = xThrowStrength;
			}

			Item item = heldItem.GetComponent<Item> ();
			item.isHeld = false;
			item.isThrown = true;

			body.bodyType = RigidbodyType2D.Dynamic;
			body.AddForce (new Vector2 (tempThrowStrength, yThrowStrength));

			heldItem = null;

			return true;
		}
		return false;
	}

	void checkPickup() {
		if (Input.GetButtonDown ("pickup")) {
			GameObject closest = nearItems [0];
			foreach (GameObject item in nearItems) {
				float dist = Vector3.Distance (transform.position, item.transform.position);
				float closestDist = Vector3.Distance (transform.position, closest.transform.position);
				if (dist < closestDist) {
					closest = item;
				}
			}

			nearItems.Remove (closest);
			heldItem = closest;
			heldItem.transform.parent = transform;
			Item itemController = heldItem.GetComponent<Item> ();
			Physics2D.IgnoreCollision (GetComponent<CapsuleCollider2D>(), itemController.hitCollider);

			itemController.pickupItem(playerSprite.flipX);

			//Set the items position and rotation
			positionHeldItem ();
		}
	}

	void checkSwapItem () {
		if (Input.GetButtonDown ("swap")) {
			if (stashedItem && heldItem) {
				swapStashedEquipped ();
			} else if (stashedItem) {
				equipStashedItem ();
			} else if (heldItem) {
				stashEquippedItem ();
			}
		}
	}

	void swapStashedEquipped () {
		GameObject tempItem = heldItem;
		equipStashedItem ();
		stashedItem = tempItem;
		stashedItem.SetActive (false);
	}

	void equipStashedItem() {
		stashedItem.SetActive (true);
		Item itemController = stashedItem.GetComponent<Item> ();
		Physics2D.IgnoreCollision (GetComponent<CapsuleCollider2D>(), itemController.hitCollider);
		heldItem = stashedItem;
		stashedItem = null;
		itemController.pickupItem(playerSprite.flipX);
		positionHeldItem ();
	}

	void stashEquippedItem() {
		stashedItem = heldItem;
		stashedItem.SetActive (false);
		heldItem = null;
	}

	void checkDrop() {
		if (Input.GetButtonDown ("drop")) {
			heldItem.GetComponent<Item> ().dropItem ();

			heldItem = null;
		}
	}

	void positionHeldItem() {
		if (heldItem) {
			float newX = transform.position.x;
			float newY = transform.position.y;
			float currentXOffset = heldItem.GetComponent<Item>().xOffset;
			float currentYOffset = heldItem.GetComponent<Item>().yOffset;
			float currentZRotation = heldItem.GetComponent<Item> ().zRotation;

			if (playerSprite.flipX) {
				currentXOffset *= -1;
				currentZRotation *= -1;
			}
			newX += currentXOffset;
			newY += currentYOffset;

			heldItem.transform.position = new Vector2 (newX, newY);
			heldItem.transform.eulerAngles = new Vector3 (0, 0, currentZRotation);
		}
	}	
}

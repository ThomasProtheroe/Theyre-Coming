using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public int playerSpeed = 5;
	public int xThrowStrength = 500;
	public int yThrowStrength = 100;
	public int health = 5;
	public bool isBusy = false;
	public bool isTargetable = true;
	public bool isInvulnerable = false;

	public List<GameObject> nearItems = new List<GameObject>();
	public List<GameObject> nearTransitions = new List<GameObject>();
	public GameObject heldItem;
	public GameObject heldItemParent;
	public GameObject handsParent;
	public GameObject stashedItem;
	public GameObject frontHand;
	public GameObject backHand;
	public Sprite deathSprite;

	private float moveX;
	private Animator anim;
	private Animator handsAnim;
	private Rigidbody2D rigidBody;
	private SpriteRenderer playerSprite;
	private bool isAttacking;
	private bool isDead;
	private bool handsFlipped;

	void Start() {
		anim = gameObject.GetComponent<Animator> ();
		handsAnim = handsParent.GetComponent<Animator> ();
		rigidBody = gameObject.GetComponent<Rigidbody2D> ();
		playerSprite = gameObject.GetComponent<SpriteRenderer> ();
		isAttacking = false;
		isDead = false;
	}

	// Update is called once per frame
	void Update () {
		if (heldItem) {
			isAttacking = heldItem.GetComponent<Item> ().isAttacking;
		} else {
			isAttacking = false;
		}

		if (!isAttacking && !isBusy && !isDead) {
			playerMove ();
			checkPlayerInput ();
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Item") {
			nearItems.Add (other.gameObject);
		} else if (other.gameObject.tag == "Enemy" && !isInvulnerable) {
			takeDamage (1);
			StartCoroutine ("damageFlash");
		} else if (other.gameObject.tag == "Transition") {
			nearTransitions.Add (other.gameObject);
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.tag == "Item") {
			nearItems.Remove(other.gameObject);
		} else if (other.gameObject.tag == "Transition") {
			nearTransitions.Remove(other.gameObject);
		}
	}

	void checkPlayerInput() {
		if (nearItems.Count != 0) {
			if (!heldItem) {
				checkPickup ();
			} else {
				checkCraft ();
			}
		}
		if (heldItem) {
			//Since we are sharing a key for both, only check for item use if not throwing item
			if (!checkThrow ()) {
				checkUse ();
			}
			checkDrop ();
		}
		if (nearTransitions.Count != 0) {
			checkTravel ();
		}
		checkSwapItem ();
	}

	void playerMove () {
		float moveInput = Input.GetAxis ("Horizontal");

		if (moveInput > 0.0f) {
			moveX = 1.0f;
			handsAnim.SetBool ("Walking", true);
		} else if (moveInput < 0.0f) {
			moveX = -1.0f;
			handsAnim.SetBool ("Walking", true);
		} else {
			moveX = 0.0f;
			handsAnim.SetBool ("Walking", false);
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
		} else {
			flipHands ();
		}
	}

	void flipHands() {
		Vector3 scale = handsParent.transform.localScale;
		scale.x *= -1;
		handsParent.transform.localScale = scale;
	}

	void alignHands() {
		if (handsFlipped != playerSprite.flipX) {
			flipHands ();
		}
	}

	void takeDamage(int damage) {
		health -= damage;
		if (health <= 0) {
			gameOver ();
		}
	}

	void gameOver() {
		isDead = true;
		transform.localScale = new Vector3 (1.2f, 1.2f, 1.0f);
		anim.SetTrigger ("Death");

		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		Debug.Log (enemies.Length);
		foreach (GameObject enemy in enemies) {
			enemy.GetComponent<Enemy> ().deactivate ();
		}
	}

	void checkUse() {
		if (Input.GetButtonDown ("use") && !Input.GetButton("down")) {
			rigidBody.velocity = new Vector2 (0, 0);
			anim.SetInteger("State", 0);
			heldItem.GetComponent<Item>().use ();
		}
	}

	bool checkThrow() {
		if (Input.GetButton("up") && Input.GetButtonDown("use")) {
			Item item = heldItem.GetComponent<Item> ();
			item.playThrowSound ();

			heldItem.transform.parent = null;
			item.frontHand.SetActive (false);
			item.backHand.SetActive (false);

			Rigidbody2D body = heldItem.GetComponent<Rigidbody2D>();

			heldItem.layer = 12;

			//Set throw direction	
			int tempThrowStrength;
			if (playerSprite.flipX) {
				tempThrowStrength = xThrowStrength * -1;
			} else {
				tempThrowStrength = xThrowStrength;
			}
				
			showPlayerHands ();

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
		if (Input.GetButtonDown ("interact")) {
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
			heldItem.transform.parent = heldItemParent.transform;
			Item itemController = heldItem.GetComponent<Item> ();

			itemController.pickupItem(playerSprite.flipX);
			hidePlayerHands ();

			//Set the items position and rotation
			positionHeldItem ();
		}
	}

	void checkCraft() {
		if (Input.GetButton ("down") && Input.GetButtonDown ("use")) {
			GameObject closest = nearItems [0];
			foreach (GameObject item in nearItems) {
				float dist = Vector3.Distance (transform.position, item.transform.position);
				float closestDist = Vector3.Distance (transform.position, closest.transform.position);
				if (dist < closestDist) {
					closest = item;
				}
			}

			//Try to combine the items
			string closestType = closest.GetComponent<Item> ().type;
			string heldType = heldItem.GetComponent<Item> ().type;
			GameObject product = RecipeBook.tryCraft (closestType, heldType);

			//If crafting is successful
			if (product) {
				product.GetComponent<Item> ().playCraftingSound();

				//destroy both ingredients
				nearItems.Remove (closest);
				Destroy(closest);
				Destroy (heldItem);

				//create new item and place in hands
				heldItem = product;
				heldItem.GetComponent<Item> ().pickupItem (playerSprite.flipX);
				heldItem.transform.parent = heldItemParent.transform;

				//Set the items position and rotation
				positionHeldItem ();
			}
		}
	}

	void checkSwapItem () {
		if (Input.GetButtonDown ("swap")) {
			if (heldItem) {
				heldItem.GetComponent<Item> ().playSwappingSound ();
			}

			if (stashedItem && heldItem) {
				swapStashedEquipped ();
			} else if (stashedItem) {
				equipStashedItem ();
			} else if (heldItem) {
				stashEquippedItem ();
			} else {
				return;
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
		heldItem = stashedItem;
		stashedItem = null;
		itemController.pickupItem(playerSprite.flipX);
		positionHeldItem ();

		hidePlayerHands ();
	}

	void stashEquippedItem() {
		stashedItem = heldItem;
		stashedItem.SetActive (false);
		heldItem = null;

		showPlayerHands ();
	}

	void checkDrop() {
		if (Input.GetButtonDown ("drop")) {
			heldItem.GetComponent<Item> ().dropItem ();
			heldItem.layer = 13;

			showPlayerHands ();

			heldItem = null;
		}
	}

	void checkTravel() {
		if (Input.GetButtonDown ("up")) {
			rigidBody.velocity = new Vector2 (0f, 0f);
			anim.SetInteger("State", 0);
			isBusy = true;
			isInvulnerable = true;

			GameObject closest = nearTransitions [0];
			foreach (GameObject item in nearTransitions) {
				float dist = Vector3.Distance (transform.position, item.transform.position);
				float closestDist = Vector3.Distance (transform.position, closest.transform.position);
				if (dist < closestDist) {
					closest = item;
				}
			}
				
			Transition transController = closest.GetComponent<Transition> ();
			transController.playerTravel ();
		}
	}

	void hidePlayerHands() {
		frontHand.SetActive (false);
		backHand.SetActive (false);
	}

	void showPlayerHands() {
		frontHand.SetActive (true);
		backHand.SetActive (true);
	}

	void positionHeldItem() {
		if (heldItem) {
			Item itemCon = heldItem.GetComponent<Item> ();
			float currentXOffset = itemCon.xOffset;
			float currentYOffset = itemCon.yOffset;
			float currentZRotation = itemCon.zRotation;

			if (playerSprite.flipX) {
				currentXOffset *= -1;
				currentZRotation *= -1;
			}

			heldItemParent.transform.localPosition = new Vector2 (currentXOffset, currentYOffset);
			heldItemParent.transform.localEulerAngles = new Vector3 (0, 0, currentZRotation);
			heldItem.transform.localPosition = Vector2.zero;
			heldItem.transform.localEulerAngles = Vector3.zero;
		}
	}	

	IEnumerator damageFlash() {
		//Turn red over time
		for (float f = 1f; f >= 0; f -= 0.2f) {
			Color c = playerSprite.material.color;
			c.g = f;
			c.b = f;
			playerSprite.material.color = c;

			yield return null;
		}

		//Turn back to original color
		for (float f = 0f; f <= 1; f += 0.2f) {
			Color c = playerSprite.material.color;
			c.g = f;
			c.b = f;
			playerSprite.material.color = c;

			yield return null;
		}
	}
}

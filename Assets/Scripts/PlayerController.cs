using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float playerSpeed = 5;
	public int xThrowStrength = 500;
	public int yThrowStrength = 100;
	public int health = 5;
	public bool isBusy = false;
	public bool isTargetable = true;
	public bool isInvulnerable = false;

	[HideInInspector]
	public List<GameObject> nearInteractives = new List<GameObject>();
	public GameObject heldItem;
	[HideInInspector]
	public GameObject heldItemParent;
	[HideInInspector]
	public GameObject handsParent;
	public GameObject stashedItem;
	[HideInInspector]
	public GameObject frontHand;
	[HideInInspector]
	public GameObject backHand;
	public GameObject craftingCloud;
	public Sprite deathSprite;
	public Area currentArea;
	public GameController gameCon;

	public AudioClip[] hitSounds;
	public AudioClip deathSound;

	[HideInInspector]
	public ItemSlot itemSlot1;
	[HideInInspector]
	public ItemSlot itemSlot2;
	[HideInInspector]
	public ItemSlot activeSlot;
	public DescriptionPanel descriptionPanel;
	[HideInInspector]
	public SpriteRenderer playerSprite;

	private Animator anim;
	private Animator handsAnim;
	private AudioSource source;
	private Rigidbody2D rigidBody;
	private GameObject closestInteractive;
	private GameObject beingCrafted;
	private float burnImmunityTimer;
	private float moveX;
	private bool isAttacking;
	private bool isDead;
	private bool isCrafting;
	private bool handsFlipped;

	void Start() {
		anim = gameObject.GetComponent<Animator> ();
		source = gameObject.GetComponent<AudioSource> ();
		handsAnim = handsParent.GetComponent<Animator> ();
		rigidBody = gameObject.GetComponent<Rigidbody2D> ();
		playerSprite = gameObject.GetComponent<SpriteRenderer> ();

		gameCon = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		itemSlot1 = GameObject.FindGameObjectWithTag ("ItemSlot1").GetComponent<ItemSlot> ();
		itemSlot2 = GameObject.FindGameObjectWithTag ("ItemSlot2").GetComponent<ItemSlot> ();
		activeSlot = itemSlot1;
		activeSlot.setActive (true);

		isAttacking = false;
		isDead = false;
		burnImmunityTimer = 0.0f;
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
			
		updateClosestInteractive ();

		if (burnImmunityTimer > 0.0f) {
			burnImmunityTimer -= Time.deltaTime;
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Item" || other.gameObject.tag == "Transition") {
			nearInteractives.Add (other.gameObject);
		} else if (other.gameObject.tag == "Enemy") {
			Enemy enemy = other.gameObject.GetComponent<Enemy> ();
			if (other == enemy.attackHitbox && !isInvulnerable && !enemy.hitPlayer) {
				enemy.hitPlayer = true;
				takeHit (enemy.attackDamage);
			}

		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.tag == "Item" || other.gameObject.tag == "Transition") {
			other.GetComponent<Interactive> ().disableHighlight ();
			nearInteractives.Remove(other.gameObject);
		}
	}

	void checkPlayerInput() {
		//TODO TESTING ONLY - to be replaced with pause menu
		if (Input.GetKeyDown("escape")) {
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit();
			#endif
		}

		if (nearInteractives.Count != 0) {
			checkTravel ();

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
		checkSwapItem ();
	}

	void updateClosestInteractive() {
		if (nearInteractives.Count == 0) {
			closestInteractive = null;
			return;
		}
		GameObject closest = nearInteractives [0];
		foreach (GameObject interactive in nearInteractives) {
			float dist = Vector3.Distance (transform.position, interactive.transform.position);
			float closestDist = Vector3.Distance (transform.position, closest.transform.position);
			if (dist < closestDist) {
				closest = interactive;
			}
		}

		Interactive interCon = closest.GetComponent<Interactive> ();
		interCon.updateHighlightColor ();

		if (closest == closestInteractive) {
			return;
		}

		if (closestInteractive) {
			closestInteractive.GetComponent<Interactive> ().disableHighlight ();
		}

		closestInteractive = closest;

		interCon.enableHighlight ();
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

		Vector3 scale = heldItemParent.transform.localScale;
		scale.x *= -1;
		heldItemParent.transform.localScale = scale;

		float newX = craftingCloud.transform.localPosition.x * -1;
		craftingCloud.transform.localPosition = new Vector2 (newX, craftingCloud.transform.localPosition.y);

		if (heldItem) {
			heldItem.GetComponent<Item> ().flipped = !heldItem.GetComponent<Item> ().flipped;
			positionHeldItem ();
		} else {
			flipHands ();
		}
	}

	void flipHands() {
		handsFlipped = !handsFlipped;
		Vector3 scale = handsParent.transform.localScale;
		scale.x *= -1;
		handsParent.transform.localScale = scale;
	}

	public void alignHands() {
		if (handsFlipped != playerSprite.flipX) {
			flipHands ();
		}
	}

	void takeDamage(int damage) {
		if (health > 0) {
			health -= damage;
			if (health <= 0) {
				source.PlayOneShot (deathSound);
				gameOver ();
			} else {
				playHitSound ();
			}
		}
	}

	void gameOver() {
		isDead = true;
		transform.localScale = new Vector3 (1.2f, 1.2f, 1.0f);
		anim.SetTrigger ("Death");
		hidePlayerHands ();

		gameCon.fadeToMenu ();
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
			item.disableAnimator ();
			item.playThrowSound ();

			heldItem.transform.parent = null;
			item.frontHand.SetActive (false);
			item.backHand.SetActive (false);

			Rigidbody2D body = heldItem.GetComponent<Rigidbody2D>();

			heldItem.layer = 12;

			//Set throw direction	
			int tempThrowStrength;
			if (playerSprite.flipX) {
				item.throwDirection = -1;
			} else {
				item.throwDirection = 1;
			}
			tempThrowStrength = xThrowStrength * item.throwDirection;
				
			alignHands ();
			showPlayerHands ();

			item.isHeld = false;
			item.isThrown = true;
			item.onThrow ();

			body.bodyType = RigidbodyType2D.Dynamic;
			body.AddForce (new Vector2 (tempThrowStrength, yThrowStrength));

			heldItem = null;

			//Update UI box
			activeSlot.setEmpty();

			return true;
		}
		return false;
	}

	void checkPickup() {
		if (Input.GetButtonDown ("interact") && closestInteractive && closestInteractive.tag == "Item") {
			nearInteractives.Remove (closestInteractive);
			heldItem = closestInteractive;
			heldItem.transform.parent = heldItemParent.transform;
			Item itemController = heldItem.GetComponent<Item> ();

			itemController.pickupItem(playerSprite.flipX);
			hidePlayerHands ();

			//Set the items position and rotation
			positionHeldItem ();

			//Update UI
			activeSlot.setImage(heldItem.GetComponent<SpriteRenderer>().sprite);
			if (itemController.description != "") {
				descriptionPanel.showDescription (itemController.description);
			}

		}
	}
		
	void checkCraft() {
		if (Input.GetButton ("down") && Input.GetButtonDown ("use")) {
			StartCoroutine ("craftItem");
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
			}

			switchActiveItemSlot ();
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
		itemController.flipped = playerSprite.flipX;
		itemController.pickupItem(playerSprite.flipX);

		positionHeldItem ();

		hidePlayerHands ();
	}

	void stashEquippedItem() {
		stashedItem = heldItem;
		stashedItem.SetActive (false);
		heldItem = null;

		alignHands ();
		showPlayerHands ();
	}

	void checkDrop() {
		if (Input.GetButtonDown ("drop")) {
			heldItem.GetComponent<Item> ().dropItem ();

			alignHands ();
			showPlayerHands ();

			heldItem = null;

			//Update UI box
			activeSlot.setEmpty();
			descriptionPanel.hideDescription ();
		}
	}

	void checkTravel() {
		if (Input.GetButtonDown ("interact") && closestInteractive && closestInteractive.tag == "Transition") {
			if (closestInteractive.GetComponent<Transition> ().inUse) {
				return;
			}

			rigidBody.velocity = new Vector2 (0f, 0f);
			anim.SetInteger("State", 0);
			handsAnim.SetBool ("Walking", false);
			isBusy = true;
			isInvulnerable = true;

			Transition transController = closestInteractive.GetComponent<Transition> ();
			transController.playerTravel ();
		}
	}

	public void hidePlayerHands() {
		frontHand.SetActive (false);
		backHand.SetActive (false);
	}

	public void showPlayerHands() {
		frontHand.SetActive (true);
		backHand.SetActive (true);
	}

	public void positionHeldItem() {
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

	private void switchActiveItemSlot() {
		activeSlot.setActive (false);

		if (activeSlot == itemSlot1) {
			activeSlot = itemSlot2;
		} else {
			activeSlot = itemSlot1;
		}

		activeSlot.setActive (true);
	}

	private void updateItemSlot(Sprite sprite) {
		if (sprite) {
			activeSlot.setImage (sprite);
		} else {
			activeSlot.setEmpty ();
		}
	}

	public void takeHit(int damage) {
		if (isCrafting) {
			StopCoroutine ("craftItem");
			Destroy (beingCrafted);
			craftingCloud.SetActive (false);
			isCrafting = false;
			isBusy = false;
		}

		takeDamage (damage);
		StartCoroutine ("damageFlash");
	}

	public void takeFireHit() {
		if (burnImmunityTimer > 0.0f) {
			return;
		}

		burnImmunityTimer = 1.0f;
		takeDamage (1);
		StartCoroutine ("damageFlash");
	}

	public GameObject getClosestInteractive() {
		return closestInteractive;
	}

	public Area getCurrentArea() {
		return currentArea;
	}

	public void setCurrentArea(Area area) {
		currentArea = area;
	}

	private void playHitSound() {
		source.PlayOneShot (hitSounds [Random.Range (0, hitSounds.Length - 1)]);
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

	IEnumerator craftItem() {
		rigidBody.velocity = Vector2.zero;
		GameObject closest = nearInteractives [0];
		foreach (GameObject item in nearInteractives) {
			float dist = Vector3.Distance (transform.position, item.transform.position);
			float closestDist = Vector3.Distance (transform.position, closest.transform.position);
			if (dist < closestDist) {
				closest = item;
			}
		}

		//Try to combine the items
		Item closestItem = closest.GetComponent<Item> ();
		Item equippedItem = heldItem.GetComponent<Item> ();
		beingCrafted = RecipeBook.tryCraft (closestItem.type, equippedItem.type);

		//Check for crafting sound overrides
		AudioClip overrideSound = null;
		if (equippedItem.craftOverrideSound) {
			overrideSound = equippedItem.craftOverrideSound;
		} else if (closestItem.craftOverrideSound) {
			overrideSound = closestItem.craftOverrideSound;
		}

		//If crafting is successful
		if (beingCrafted) {
			isBusy = true;
			isCrafting = true;

			Item prodItem = beingCrafted.GetComponent<Item> ();
			prodItem.playCraftingSound (overrideSound);
			beingCrafted.transform.position = new Vector2 (0.0f, -50.0f);
			craftingCloud.SetActive (true);

			yield return new WaitForSeconds(0.6f);

			//destroy both ingredients
			nearInteractives.Remove (closest);
			Destroy(closest);
			Destroy (heldItem);

			//create new item and place in hands
			heldItem = beingCrafted;
			Item itemCon = heldItem.GetComponent<Item> ();
			itemCon.pickupItem (playerSprite.flipX);
			heldItem.transform.parent = heldItemParent.transform;

			//Set the items position and rotation
			positionHeldItem ();
			craftingCloud.SetActive (false);
			isCrafting = false;
			isBusy = false;
			beingCrafted = null;

			//Update UI box
			activeSlot.setImage(heldItem.GetComponent<SpriteRenderer>().sprite);
			if (itemCon.description != "") {
				descriptionPanel.showDescription (itemCon.description);
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float playerSpeed;
	public float slowReduction;
	private float currentSpeed;
	public int xThrowStrength;
	public int yThrowStrength;
	public int maxHealth;
	//[HideInInspector]
	public int health;


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
	public ParticleSystem dripPS;
	public Sprite deathSprite;
	public Area currentArea;
	[HideInInspector]
	public GameController gameCon;

	public AudioClip[] hitSounds;
	public AudioClip deathSound;

	[HideInInspector]
	public ItemSlot itemSlot1;
	[HideInInspector]
	public ItemSlot itemSlot2;
	[HideInInspector]
	public ItemSlot activeSlot;
	[HideInInspector]
	public SpriteRenderer playerSprite;

	private Animator anim;
	private Animator handsAnim;
	private AudioSource source;
	private Rigidbody2D rigidBody;
	private GameObject closestInteractive;
	private GameObject beingCrafted;
	private float burnImmunityTimer;
	private float bileImmunityTimer;
	private float slowedTimer;
	private float moveX;
	[HideInInspector]
	public bool isBusy;
	[HideInInspector]
	public bool isTargetable;
	public bool isInvulnerable;
	[HideInInspector]
	public bool isAttacking;
	public bool isDead;
	private bool isCrafting;
	public bool isHealing;
	private bool handsFlipped;
	[SerializeField]
	private bool startLeft;

	//Player input storage
	private bool cinematicControl;
	public float moveInput;
	public float verticalInput;
	public bool useInput;
	public bool interactInput;
	public bool swapInput;
	public bool dropInput;

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

		health = maxHealth;
		currentSpeed = playerSpeed;
		isTargetable = true;
		burnImmunityTimer = 0.0f;
		bileImmunityTimer = 0.0f;
		slowedTimer = 0.0f;

		if (startLeft) {
			flipPlayer ();
		}
	}

	// Update is called once per frame
	void Update () {
		if (!cinematicControl) {
			getPlayerInput ();
		}

		if (heldItem) {
			isAttacking = heldItem.GetComponent<Item> ().isAttacking;
		} else {
			isAttacking = false;
		}

		if (isBusy) {
			checkInterrupt ();
		} else if (!isAttacking && !isDead) {
			playerMove ();
			checkPlayerInput ();
		}
			
		updateClosestInteractive ();

		if (burnImmunityTimer > 0.0f) {
			burnImmunityTimer -= Time.deltaTime;
		}
		if (bileImmunityTimer > 0.0f) {
			bileImmunityTimer -= Time.deltaTime;
		}
		if (slowedTimer > 0.0f) {
			slowedTimer -= Time.deltaTime;
			if (slowedTimer <= 0.0f) {
				//slow condition has ended
				endSlowed();
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Item" || other.gameObject.tag == "Transition" || other.gameObject.tag == "Interactive") {
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
		if (other.gameObject.tag == "Item" || other.gameObject.tag == "Transition" || other.gameObject.tag == "Interactive") {
			other.GetComponent<Interactive> ().disableHighlight ();
			nearInteractives.Remove(other.gameObject);
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.tag == "Enemy") {
			Enemy enemy = other.gameObject.GetComponent<Enemy> ();
			if (enemy.getIsBurning() && other == enemy.proximityHitbox && burnImmunityTimer <= 0.0f && !isInvulnerable) {
				takeFireHit ();
			}
		}
	}

	void getPlayerInput() {
		moveInput = Input.GetAxis ("Horizontal");
		verticalInput = Input.GetAxis ("Vertical");
		useInput = Input.GetButtonDown ("use");
		interactInput = Input.GetButtonDown ("interact");
		swapInput = Input.GetButtonDown ("swap");
		dropInput = Input.GetButtonDown ("drop");
	}

	void checkPlayerInput() {
		if (nearInteractives.Count != 0) {
			checkTravel ();
			checkInteract ();

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

	void checkInterrupt() {
		if (!interactInput) {
			return;
		}

		if (isHealing) {
			FirstAidStation station = closestInteractive.GetComponent<FirstAidStation> ();

			isBusy = false;
			isHealing = false;
			station.cancelUse ();
		}
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

		rigidBody.velocity = new Vector2 (moveX * currentSpeed, rigidBody.velocity.y);

		if (moveX != 0.0f) {
			anim.SetInteger("State", 1);
		}
		else if (moveX == 0.0f) {
			anim.SetInteger("State", 0);
		}
	}

	public void flipPlayer() {
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
		cancelCrafting ();
		cancelHealing ();

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
		if (heldItem) {
			heldItem.GetComponent<Item> ().dropItem ();
		}

		gameCon.pauseTimer ();
		gameCon.gameOver ();
	}


	void checkUse() {
		if (!heldItem.GetComponent<Item> ().usable) {
			return;
		}

		if (useInput && verticalInput == 0) {
			rigidBody.velocity = new Vector2 (0, 0);
			anim.SetInteger("State", 0);
			heldItem.GetComponent<Item>().use ();
		}
	}

	bool checkThrow() {
		if (verticalInput > 0 && useInput) {
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
			float tempThrowRotation;
			if (playerSprite.flipX) {
				item.throwDirection = -1;
			} else {
				item.throwDirection = 1;
			}
			tempThrowStrength = xThrowStrength * item.throwDirection;
			tempThrowRotation = item.throwRotation * item.throwDirection;
				
			alignHands ();
			showPlayerHands ();

			item.isHeld = false;
			item.isThrown = true;
			item.onThrow ();

			body.bodyType = RigidbodyType2D.Dynamic;
			body.AddForce (new Vector2 (tempThrowStrength, yThrowStrength));
			body.AddTorque (tempThrowRotation);

			heldItem = null;

			//Update UI box
			activeSlot.setEmpty();

			return true;
		}
		return false;
	}

	void checkPickup() {
		if (interactInput && closestInteractive && closestInteractive.tag == "Item") {
			nearInteractives.Remove (closestInteractive);
			heldItem = closestInteractive;
			heldItem.transform.parent = heldItemParent.transform;
			Item itemController = heldItem.GetComponent<Item> ();

			//Update UI
			updateItemSlot(itemController.getDisplaySprite());
			if (itemController.description != "") {
				gameCon.showDescription (itemController.description);
			}

			itemController.pickupItem(playerSprite.flipX);
			hidePlayerHands ();

			//Set the items position and rotation
			positionHeldItem ();
		}
	}
		
	void checkCraft() {
		if (verticalInput < 0 && (interactInput || useInput)) {
			StartCoroutine ("craftItem");
		}
	}

	void checkSwapItem () {
		if (swapInput) {
			if (heldItem) {
				heldItem.GetComponent<Item> ().playSwappingSound ();
			}

			switchActiveItemSlot ();

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
		if (dropInput) {
			heldItem.GetComponent<Item> ().dropItem ();

			emptyPlayerHands ();
		}
	}

	public void emptyPlayerHands() {
		alignHands ();
		showPlayerHands ();

		heldItem = null;

		//Update UI box
		activeSlot.setEmpty();
		gameCon.hideDescription ();
	}

	void checkInteract() {
		if (interactInput && closestInteractive) {
			Debug.Log (closestInteractive.name);
			if (closestInteractive.name == "FirstAidStation") {
				FirstAidStation station = closestInteractive.GetComponent<FirstAidStation> ();

				if (station.usesRemaining <= 0 || health == maxHealth) {
					return;
				}

				rigidBody.velocity = new Vector2 (0, 0);
				anim.SetInteger("State", 0);
				handsAnim.SetBool ("Walking", false);
				isBusy = true;
				isHealing = true;
				station.interact ();
			}
		}
	}

	void checkTravel() {
		if (interactInput && closestInteractive && closestInteractive.tag == "Transition") {
			Transition transController = closestInteractive.GetComponent<Transition> ();
			if (transController.inUse) {
				return;
			}

			if (transController.isLocked) {
				transController.failOpen();
				return;
			}

			rigidBody.velocity = new Vector2 (0f, 0f);
			anim.SetInteger("State", 0);
			handsAnim.SetBool ("Walking", false);
			isBusy = true;
			isInvulnerable = true;


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
		takeDamage (damage);
		StartCoroutine ("damageFlash");
	}

	public void takeBileHit(int damage) {
		setSlowed (0.25f);
		if (bileImmunityTimer > 0.0f) {
			return;
		}

		dripPS.Play ();
		CancelInvoke ("stopDrip");
		Invoke ("stopDrip", 3.0f);
			
		bileImmunityTimer = 0.6f;

		takeHit (damage);
	}

	public void takeFireHit() {
		if (burnImmunityTimer > 0.0f) {
			return;
		}

		burnImmunityTimer = 1.0f;
		takeDamage (1);
		StartCoroutine ("damageFlash");
	}

	public void setSlowed(float duration) {
		currentSpeed = playerSpeed * (1.0f - slowReduction);
		slowedTimer = duration;
	}

	public void endSlowed() {
		currentSpeed = playerSpeed;
		slowedTimer = 0.0f;
	}

	private void stopDrip() {
		if (dripPS.isPlaying) {
			dripPS.Stop ();
		}
	}

	public void heal(int healAmount) {
		health += healAmount;
		if (health > maxHealth) {
			health = maxHealth;
		}

		StartCoroutine ("healFlash");
	}

	private void cancelHealing() {
		if (isHealing) {
			isBusy = false;
			isHealing = false;

			FirstAidStation station = closestInteractive.GetComponent<FirstAidStation> ();
			station.cancelUse ();
		}
	}

	private void cancelCrafting() {
		if (isCrafting) {
			StopCoroutine ("craftItem");
			Destroy (beingCrafted);
			craftingCloud.SetActive (false);
			isCrafting = false;
			isBusy = false;
		}
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

	public void enableCinematicControl(bool flag) {
		cinematicControl = flag;
	}

	public void updateVolume(float volume) {
		source.volume = volume;
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

	IEnumerator healFlash() {
		//Turn red over time
		for (float f = 1f; f >= 0; f -= 0.2f) {
			Color c = playerSprite.material.color;
			c.r = f;
			c.b = f;
			playerSprite.material.color = c;

			yield return null;
		}

		//Turn back to original color
		for (float f = 0f; f <= 1; f += 0.2f) {
			Color c = playerSprite.material.color;
			c.r = f;
			c.b = f;
			playerSprite.material.color = c;

			yield return null;
		}
	}

	IEnumerator craftItem() {
		rigidBody.velocity = Vector2.zero;
		if (anim.GetInteger ("State") == 1) {
			anim.SetInteger ("State", 0);
		}
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
			gameCon.countItemCraft ();

			//Update UI box
			updateItemSlot(itemCon.getDisplaySprite());
			if (itemCon.description != "") {
				gameCon.showDescription (itemCon.description);
			}
		}
	}
}

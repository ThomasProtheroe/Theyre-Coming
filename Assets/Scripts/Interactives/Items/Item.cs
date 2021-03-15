using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactive {

	[HideInInspector]
	public bool isHeld = false;
	[HideInInspector]
	public bool isThrown = false;
	[HideInInspector]
	public bool isBouncing = false;
	[HideInInspector]
	public bool isAttacking = false;
	[HideInInspector]
	public bool flipped = false;
	[HideInInspector]
	public bool usable = false;
	private bool deparent = false;
	private bool isShoddy = false;

	private int craftingCostOverride;
	private int skipBounceCheck;

	[Header("Basic Attributes")]
	public float xOffset;
	public float yOffset;
	public float zRotation;
	public float restingHeight;
	public float restingRotation;
	public int thrownDamage;
	public int thrownKnockback;
	public float throwRotation;
	public float initialThrowRotation;
	public float initialThrowHeight;
	public int throwDirection = 0;
	public int tier = -1;
	public string type;
	public string itemName;
	public string description;
	public bool staticBreak = false;

	protected int state = 0;
	[SerializeField]
	protected int attackType;

	[Header("Sprites")]
	[SerializeField]
	protected List<GameObject> decorativeSprites;
	[SerializeField]
	protected Sprite displaySprite;
	[SerializeField]
	protected Sprite bloodySprite1;
	[SerializeField]
	protected Sprite bloodySprite2;
	[SerializeField]
	protected Sprite bloodySprite3;
	[SerializeField]
	protected Sprite bloodySprite4;

	public BoxCollider2D pickupCollider;
	public Collider2D hitCollider;
	public GameObject frontHand;
	public GameObject backHand;

	[Header("Sound Clips")]
	public AudioClip throwSound;
	public AudioClip throwImpact;
	public AudioClip craftSound;
	public AudioClip pickupSound;
	public AudioClip swapSound;
	public AudioClip breakSound;
	public AudioClip craftOverrideSound;
	public AudioClip craftingFanfare;

	[Header("Status Effects")]
	public bool inflictsBleed;
	public bool inflictsBlind;
	public bool inflictsBurning;
	public bool inflictsGib;

	[Header("Tutorial")]
	public TutorialPanel tutorialPanel;

	protected SpriteRenderer sprite;
	protected SoundController soundController;

	[HideInInspector]
	public GameObject player;
	[HideInInspector]
	public PlayerController playerCon;
	[HideInInspector]
	protected GameController gameController;

	[HideInInspector]
	public GameObject hiddenItem;

	// Use this for initialization
	protected virtual void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		playerCon = player.GetComponent<PlayerController> ();
		soundController = GameObject.FindGameObjectWithTag ("SoundController").GetComponent<SoundController> ();
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		//Don't escape newline characters in item description
		description = description.Replace ("\\n", "\n");
		sprite = GetComponent<SpriteRenderer> ();
	}

	public void manualStart() {
		Start ();
	}

	// Update is called once per frame
	protected virtual void Update () {
		if (deparent) {
			transform.parent = null;
			deparent = false;
		}

		//This god awful hack lets you skip checking if the item should come to rest for a certain number of frames. There is almost certainly a better way of doing this.
		if (skipBounceCheck > 0) {
			skipBounceCheck --;
			return;
		}

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

	protected void OnCollisionEnter2D(Collision2D other) {
		if (isThrown) {
			if (other.gameObject.tag == "Enemy") {
				if (throwImpact) {
					soundController.playPriorityOneShot (throwImpact);
				}
				float direction = transform.position.x - other.transform.position.x;
				other.gameObject.GetComponent<Enemy> ().takeHit (thrownDamage, thrownKnockback, direction, false, attackType);
				setBloodyState ();
			}

			isThrown = false;
			isBouncing = true;
			gameObject.layer = 11;

			foreach (SpriteRenderer sprite in gameObject.GetComponentsInChildren<SpriteRenderer> () ) {
				sprite.sortingLayerName = "Items";
			}
		}
	}

	protected virtual void setBloodyState() {
		if ((state == 0) && (bloodySprite1 != null)) {
			sprite.sprite = bloodySprite1;
			state++;
		} else if ((state == 1) && (bloodySprite2 != null)) {
			sprite.sprite = bloodySprite2;
			state++;
		} else if ((state == 2) && (bloodySprite3 != null)) {
			sprite.sprite = bloodySprite3;
			state++;
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

	public void setCraftingCostOverride(int cost) {
		craftingCostOverride = cost;
	}

	public void setSkipBounceCheck(int count) {
		skipBounceCheck = count;
	}

	override public void updateHighlightColor() {
		GameObject heldItem = playerCon.heldItem;
		bool canCraft = RecipeBook.canCraft(type, heldItem.GetComponent<Item>().type);
		if (heldItem) {
			if (!canCraft) {
				//Can't combine these items
				GetComponent<SpriteOutline> ().color = negativeColor;
			} else {
				int cost = RecipeBook.tryCraft (type, heldItem.GetComponent<Item>().type).product.GetComponent<Item> ().getCraftingCost();
				if (cost > (int)playerCon.stamina) {
					GetComponent<SpriteOutline> ().color = negativeColor;
				}
			}
		} else {
			GetComponent<SpriteOutline> ().color = positiveColor;
		}

	}

	public void flipItem() {
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
		flipped = !flipped;

		if (tutorialPanel != null) {
			tutorialPanel.flipPanel();
		}
	}

	public void pickupItem(bool playerFlipX, bool playSound=true) {
		if (hiddenItem != null) {
			hiddenItem.transform.parent = null;
			hiddenItem.SetActive(true);
		}

		//Handle decorative sprites attached to the item
		foreach (GameObject decorativeSprite in decorativeSprites) {
			decorativeSprite.GetComponent<DecorativeSprite> ().breakSprite();
		}
		if (decorativeSprites.Count > 0) {
			decorativeSprites.Clear ();
		}

		if (playSound && pickupSound) {
			soundController.playPriorityOneShot (pickupSound);
		}
			
		if (playerFlipX != flipped)
		{
			flipItem ();
		}
			
		gameObject.layer = 16;
		//Sometimes while manually instantiating prefabs Start() has not yet been called
		if (!playerCon) {
			Start ();
		}
		sprite.sortingLayerName = playerCon.playerSprite.sortingLayerName;
		sprite.sortingOrder = playerCon.playerSprite.sortingOrder + 1;

		//Handle items with multiple sprites
		foreach (SpriteRenderer childSprite in GetComponentsInChildren<SpriteRenderer> () ) {
			if (childSprite == frontHand.GetComponent<SpriteRenderer> () || childSprite == backHand.GetComponent<SpriteRenderer> () || childSprite == sprite) {
				continue;
			}
			childSprite.sortingLayerName = playerCon.playerSprite.sortingLayerName;
			childSprite.sortingOrder = sprite.sortingOrder - 1;
		}

		//disable pickup trigger, enable hit trigger
		pickupCollider.enabled = false;
		hitCollider.enabled = true;

		enableAnimator ();

		frontHand.SetActive (true);
		backHand.SetActive (true);

		isHeld = true;

		updateDurabilityIndicator ();

		//Display tutorial dialog if needed
		if (tutorialPanel != null) {
			tutorialPanel.showTutorialPanel();
			tutorialPanel.transform.parent = player.transform;
			tutorialPanel.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1.5f);
		}

		onPickup ();
	}

	public void dropItem() {
		transform.parent = null;
		pickupCollider.enabled = true;
		hitCollider.enabled = false;
		isHeld = false;

		disableAnimator ();

		frontHand.SetActive (false);
		backHand.SetActive (false);

		gameObject.layer = 13;
		moveToResting ();

		onDrop ();
	}

	public void moveToResting() {
		transform.position = new Vector2 (transform.position.x, restingHeight);
		transform.eulerAngles = new Vector3 (0, 0, restingRotation);

		sprite.sortingLayerName = "Background Items";
		sprite.sortingOrder = 5;

		//Handle items with multiple sprites
		foreach (SpriteRenderer childSprite in GetComponentsInChildren<SpriteRenderer> () ) {
			if (childSprite == frontHand.GetComponent<SpriteRenderer> () || childSprite == backHand.GetComponent<SpriteRenderer> () || childSprite == sprite) {
				continue;
			}
			childSprite.sortingLayerName = "Background Items";
			childSprite.sortingOrder = 4;
		}
	}

	public void playCraftingSound(AudioClip overrideSound=null) {
		//Since this object was just created, all variables are not initialized yet
		//so we need to get the SoundController directly
		SoundController sc = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
		if (overrideSound) {
			sc.playPriorityOneShot (overrideSound);
		} else if (craftSound) {
			sc.playPriorityOneShot (craftSound);
		}
	}

	public void playSwappingSound() {
		if (swapSound) {
			soundController.playPriorityOneShot (swapSound);
		}
	}

	public void playThrowSound() {
		if (throwSound) {
			soundController.playPriorityOneShot (throwSound);
		}
	}

	public virtual void use() {

	}

	public virtual void onTerrainImpact() {

	}

	public virtual void onPickup() {
		return;
	}

	public virtual void onDrop() {
		adoptTutorialPanel();

		return;
	}

	public virtual void onStash() {
		return;
	}

	public virtual void onThrow() {
		adoptTutorialPanel();
		
		return;
	}

	public void adoptTutorialPanel() {
		if (tutorialPanel != null) {
			tutorialPanel.transform.parent = gameObject.transform;
			tutorialPanel.transform.position = new Vector3(gameObject.transform.position.x, tutorialPanel.transform.position.y);
		}
	}

	public List<string> getStatusEffects() {
		List<string> statusEffects = new List<string> ();

		if (inflictsBleed) {
			statusEffects.Add ("bleed");
		}
		if (inflictsBurning) {
			statusEffects.Add ("burn");
		}
		if (inflictsBlind) {
			statusEffects.Add ("blind");
		}
		if (isShoddy) {
			statusEffects.Add ("shoddy");
		}

		return statusEffects;
	}

	public int getCraftingCost() {
		int baseCost;
		if (craftingCostOverride > 0) {
			baseCost = craftingCostOverride;
		} else {
			baseCost = Constants.STAMINA_COST_CRAFT_DEFAULT;
		}
		
		string phase = gameController.getPhase();
		int cost;
		if (phase == "downtime") {
			cost = baseCost;
		} else if (phase == "siege") {
			cost = Mathf.Floor(baseCost / 2);
		}

		return cost;
	}

	public virtual void breakItem() {
		bool breakImmed = onBreak ();

		playBreakSound ();

		Rigidbody2D body = GetComponent<Rigidbody2D>();

		if (isHeld) {
			isHeld = false;
			playerCon.heldItem = null;
			playerCon.activeSlot.setEmpty();

			//The animator sometimes changes the objects position on the frame it is 
			//disabled, so we deparent the item next frame instead for safety. Goddamn unity animators.
			deparent = true;

			disableAnimator ();
			frontHand.SetActive (false);
			backHand.SetActive (false);

			playerCon.alignHands ();
			playerCon.showPlayerHands ();

			body.velocity = Vector2.zero;
		}

		//If we don't want to play the break animation, destroy the object now
		if (breakImmed) {
			Destroy (gameObject);
			return;
		}

		gameObject.layer = 11;

		//Items with static break set will freeze in place while breaking
		if (!staticBreak) {
			int xBreakForce = Random.Range(-100, 100);
			int yBreakForce = Random.Range(60, 100);

			//Reset the x and y rotation as these can change during attack animations
			transform.eulerAngles = new Vector3 (0, 0, transform.rotation.z);

			body.bodyType = RigidbodyType2D.Dynamic;
			body.AddForce (new Vector2 (xBreakForce, yBreakForce));
			body.AddTorque (25.0f);
		}
			
		StartCoroutine ("beginSpriteFlash");
		StartCoroutine ("destroyAfterTime", 1.5f);
	}

	public void playBreakSound() {
		if (breakSound) {
			soundController.playPriorityOneShot (breakSound);
		}
	}

	public virtual void updateDurabilityIndicator() {
		playerCon.activeSlot.resetDurabilityIndicator();
	}

	public Sprite getDisplaySprite() {
		if (displaySprite != null) {
			return displaySprite;
		} 

		return sprite.sprite;
	}

	//Return true if item should be destroyed immediately (no animation)
	public virtual bool onBreak() {
		return false;
	}

	public virtual void onCraft() {
		
	}
		
	public virtual void onTravel() {

	}

	public virtual void onArrival() {
		
	}

	public virtual void makeShoddy() {
		isShoddy = true;
	}

	/**** Coroutines ****/ 
	IEnumerator destroyAfterTime(float time) {
		yield return new WaitForSeconds(time);

		Destroy (gameObject);
	}

	IEnumerator beginSpriteFlash() {
		while (true) {
			if (sprite.enabled) {
				sprite.enabled = false;
			} else {
				sprite.enabled = true;
			}

			yield return new WaitForSeconds (0.05f);
		}
	}
}

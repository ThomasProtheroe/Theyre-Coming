using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : Interactive {

	public Transition sibling;
	public AudioClip openSound;
	public AudioClip closeSound;

	[HideInInspector]
	public Trap readiedTrap;

	[SerializeField]
	private Collider2D safeZone;
	[SerializeField]
	private Transform[] buffers;
	private Animator anim;
	private GameObject player;
	private GameObject camera;
	private AudioSource source;
	private GameController gc;

	[SerializeField]
	private float safeZoneClearCooldown;
	private float safeZoneClearTimer;
	[SerializeField]
	private float enemyBufferOffset;

	public bool inUse;
	public bool inUseByPlayer;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		camera = GameObject.FindGameObjectWithTag("MainCamera");
		anim = gameObject.GetComponent<Animator> ();
		source = gameObject.GetComponent<AudioSource> ();
		gc = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		inUse = false;
	}

	void Update() {
		if (safeZoneClearTimer > 0) {
			safeZoneClearTimer -= Time.deltaTime;
		}
	}

	public void playerTravel() {
		StartCoroutine ("movePlayer");
	}

	public void enemyTravel(Enemy enemy) {
		StartCoroutine ("moveEnemy", enemy);
	}

	private void playOpenSound() {
		if (openSound) {
			source.clip = openSound;
			source.Play ();
		}
	}

	private void playCloseSound() {
		if (closeSound) {
			source.clip = closeSound;
			source.Play ();
		}
	}

	private void clearSafeZone() {
		Collider2D[] colliders = new Collider2D[200];
		safeZone.OverlapCollider(new ContactFilter2D(), colliders);
		foreach (Collider2D collider in colliders) {
			if (collider && collider.gameObject.tag == "Enemy") {
				//We need to activate any nearby enemies since they will have gone into hibernation while waiting for the player
				collider.gameObject.GetComponent<Enemy> ().activate();

				if (safeZoneClearTimer > 0) {
					continue;
				}

				if (!collider.gameObject.GetComponent<Enemy> ().getIsDead ()) {
					Transform buffer = buffers[UnityEngine.Random.Range(0, buffers.Length)];
					float offset = UnityEngine.Random.Range (enemyBufferOffset * -1, enemyBufferOffset);
					collider.transform.position = new Vector3 (buffer.position.x + offset, collider.transform.position.y, collider.transform.position.z);
				}
			}
		}

		safeZoneClearTimer = safeZoneClearCooldown;
		sibling.safeZoneClearTimer = sibling.safeZoneClearCooldown;
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
		if (inUse) {
			GetComponent<SpriteOutline> ().color = negativeColor;
		} else {
			GetComponent<SpriteOutline> ().color = positiveColor;
		}
	}

	IEnumerator movePlayer() {
		PlayerController playerCon = player.GetComponent<PlayerController> ();
		SpriteRenderer sprite = player.GetComponent<SpriteRenderer> ();
		SpriteRenderer itemSprite = null;
		SpriteRenderer[] handsSprites = null;
		Color originalItemColor = new Color(255f, 255f, 255f, 255f);

		inUseByPlayer = true;
		sibling.inUseByPlayer = true;

		playOpenSound ();

		if (playerCon.heldItem) {
			//Trigger item specific travel effects
			playerCon.heldItem.GetComponent<Item> ().onTravel();

			itemSprite = playerCon.heldItem.GetComponent<SpriteRenderer> ();
			originalItemColor = itemSprite.material.color;

			Item itemCon = playerCon.heldItem.GetComponent<Item> ();
			handsSprites = new SpriteRenderer[] {
				itemCon.frontHand.GetComponent<SpriteRenderer> (),
				itemCon.backHand.GetComponent<SpriteRenderer> ()
			};
		} else {
			handsSprites = playerCon.handsParent.GetComponentsInChildren<SpriteRenderer> ();
		}
		anim.SetTrigger ("Open");

		for (float f = 1f; f >= 0; f -= 0.03f) {
			//Fade out the player
			Color c = sprite.material.color;
			c.a = f;
			sprite.material.color = c;

			//Fade out any item they are holding
			if (playerCon.heldItem) {
				Color itemC = itemSprite.material.color;
				itemC.a = f;
				itemSprite.material.color = c;
			}

			//Fade out the player hand sprites
			Color handC = handsSprites [0].material.color;
			foreach (SpriteRenderer hand in handsSprites) {
				handC.a = f;
				hand.material.color = c;
			}

			yield return null;
		}

		yield return new WaitForSeconds (0.35f);

		sibling.clearSafeZone ();

		playCloseSound ();

		//Trigger item specific travel effects
		if (playerCon.heldItem) {
			playerCon.heldItem.GetComponent<Item> ().onArrival();
		}
			
		Vector2 destination = new Vector2(sibling.transform.position.x, player.transform.position.y);
		player.transform.position = destination;

		playerCon.setCurrentArea (sibling.gameObject.transform.parent.gameObject.GetComponent<Area>());

		Color orig = sprite.material.color;
		orig.a = 1.0f;
		sprite.material.color = orig;
		foreach (SpriteRenderer hand in handsSprites) {
			hand.material.color = orig;
		}
		if (itemSprite) {
			itemSprite.material.color = originalItemColor;
		}

		camera.transform.position = new Vector3 (player.transform.position.x, camera.transform.position.y, camera.transform.position.z);
		GameObject parallax = camera.transform.GetChild (0).gameObject;
		parallax.transform.localPosition = new Vector3 (0.0f, -1.5f, 10.0f);

		gc.clearCorpses ();

		inUseByPlayer = false;
		sibling.inUseByPlayer = false;

		playerCon.isBusy = false;
		playerCon.isInvulnerable = false;
	}

	IEnumerator moveEnemy(Enemy enemy) {
		inUse = true;
		sibling.inUse = true;
		enemy.deactivate ();
		SpriteRenderer sprite = enemy.gameObject.GetComponent<SpriteRenderer> ();

		anim.SetTrigger ("Open");
		sibling.GetComponent<Animator> ().SetTrigger ("Open");

		Color originalEnemyColor = sprite.material.color;

		for (float f = 1f; f >= 0; f -= 0.03f) {
			Color c = sprite.material.color;
			c.a = f;
			sprite.material.color = c;

			yield return null;
		}

		yield return new WaitForSeconds (0.35f);

		//Randomly offset the position they appear at the destination door
		float xOffset = Random.Range(-0.3f, 0.3f);
		Vector2 destination = new Vector2(sibling.transform.position.x + xOffset, enemy.gameObject.transform.position.y);

		enemy.gameObject.transform.position = destination;
		enemy.setCurrentArea (sibling.transform.parent.gameObject.GetComponent<Area> ());

		for (float f = 0f; f <= 1; f += 0.03f) {
			Color c = sprite.material.color;
			c.a = f;
			sprite.material.color = c;

			yield return null;
		}

		if (sibling.readiedTrap != null) {
			sibling.readiedTrap.trigger ();
		}

		enemy.activate ();
		inUse = false;
		sibling.inUse = false;
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

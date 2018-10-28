using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBoxTrap : Trap {
	
	public int damage;
	[SerializeField]
	private bool spawnFire;
	[SerializeField]
	private FireGlob fireGlob;
	[SerializeField]
	private int globCount;
	[SerializeField]
	private float fireLifetime;
	[SerializeField]
	private float maxYVel;
	[SerializeField]
	private float minYVel;
	[SerializeField]
	private float maxXVel;
	[SerializeField]
	private float minXVel;
	public AudioClip hitSound;

	private Transition parentTransition;
	private bool isFalling = false;
	private bool hitSoundPlayed = false;
	private bool playerHit = false;

	protected void OnTriggerEnter2D(Collider2D other) {
		if (isFalling) {
			if (other.gameObject.tag == "Enemy") {
				other.gameObject.GetComponent<Enemy> ().takeHit (damage, 0, 0, false, attackType);
				if (!hitSoundPlayed) {
					hitSoundPlayed = true;
					soundController.playPriorityOneShot (hitSound);
				}
			} else if (other.gameObject.tag == "Player") {
				if (!playerHit) {
					playerHit = true;
					playerCon.takeHit (1);
				}
			} else if (other.gameObject.tag == "Terrain") {
				isFalling = false;
				triggerCollider.enabled = false;
				hitCollider.enabled = true;
				breakItem ();
			}
		}
	}

	public override void deploy() {
		GameObject transitionObj = playerCon.getClosestInteractive ();
		if (transitionObj == null || transitionObj.tag != "Transition") {
			return;
		}

		parentTransition = transitionObj.GetComponent<Transition> ();
		parentTransition.readiedTrap = this;
		isDeployed = true;
		tag = "Trap";
			
		transform.parent = parentTransition.transform;
		hitCollider.enabled = false;
		isHeld = false;

		moveToDeployPos ();

		disableAnimator ();

		frontHand.SetActive (false);
		backHand.SetActive (false);

		gameObject.layer = 18;
		GetComponent<SpriteRenderer> ().sortingLayerName = "Background Items";

		playerCon.alignHands ();
		playerCon.showPlayerHands ();

		playerCon.heldItem = null;

		//Update UI box
		playerCon.activeSlot.setEmpty ();
	}

	public void moveToDeployPos () {
		transform.localPosition = new Vector2 (0, deployY);
		transform.eulerAngles = new Vector3 (0, 0, deployRotation);
	}

	override public void trigger(GameObject victim) {
		isFalling = true;
		triggerCollider.enabled = true;
		parentTransition.readiedTrap = null;
		transform.parent = null;
		GetComponent<SpriteRenderer> ().sortingLayerName = "Items";

		Rigidbody2D body = GetComponent<Rigidbody2D>();
		body.bodyType = RigidbodyType2D.Dynamic;
		body.gravityScale = 2.0f;
	}
		
	public override bool onBreak() {
		if (!spawnFire) {
			return false;
		}

		//Hack solution to collision problem when spawning fire globs
		//float newYPos = transform.position.y + 0.1f;

		for (int i = 0; i < globCount; i++) {
			FireGlob newGlob = Instantiate (fireGlob, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
			newGlob.friendlyFire = true;
			newGlob.lifetime = fireLifetime;

			Rigidbody2D rb = newGlob.GetComponent<Rigidbody2D> ();
			rb.velocity = new Vector2 (UnityEngine.Random.Range(minXVel, maxXVel), UnityEngine.Random.Range(minYVel, maxYVel));
		}

		//Return true so we don't play the breaking animation
		return true;
	}
}
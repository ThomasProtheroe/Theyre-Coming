using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBoxTrap : Trap {
	
	public int damage;
	[Header("Bleed Properties")]
	[SerializeField]
	private bool spawnBleedProjectiles;
	[SerializeField]
	private ParticleSystem bleedPS;
	[SerializeField]
	private BleedProjectile bleedProjectile;
	[SerializeField]
	private float projectileSpeed;
	[SerializeField]
	private float projectileLifetime;
	[SerializeField]
	private SpikeTrap spikeTrap;
	[Header("Molotov Properties")]
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
		if (spawnFire) {
			for (int i = 0; i < globCount; i++) {
				FireGlob newGlob = Instantiate (fireGlob, new Vector3(transform.position.x, transform.position.y + 0.25f, 0), Quaternion.identity);
				newGlob.friendlyFire = true;
				newGlob.lifetime = fireLifetime;

				Rigidbody2D rb = newGlob.GetComponent<Rigidbody2D> ();
				rb.velocity = new Vector2 (UnityEngine.Random.Range(minXVel, maxXVel), UnityEngine.Random.Range(minYVel, maxYVel));
			}

			//Return true so we don't play the breaking animation
			return true;
		}

		if (spawnBleedProjectiles) {
			bleedPS.Play ();

			BleedProjectile rightProjectile = Instantiate (bleedProjectile, new Vector3 (transform.position.x, transform.position.y), Quaternion.identity);
			rightProjectile.lifetime = projectileLifetime;
			rightProjectile.GetComponent<Rigidbody2D> ().velocity = new Vector2 (projectileSpeed, 0.0f); 
			rightProjectile.fire ();

			BleedProjectile leftProjectile = Instantiate (bleedProjectile, new Vector3 (transform.position.x, transform.position.y), Quaternion.identity);
			leftProjectile.lifetime = projectileLifetime;
			leftProjectile.GetComponent<Rigidbody2D> ().velocity = new Vector2 (projectileSpeed * -1, 0.0f);
			leftProjectile.fire ();

			//Spawn spike trap
			SpikeTrap newTrap = Instantiate (spikeTrap);
			newTrap.manualStart ();
			newTrap.transform.position = new Vector2 (transform.position.x, transform.position.y);
			newTrap.durability = 5;
			newTrap.moveToDeployPos ();
			newTrap.delayDeployment (1.5f);

			return false;
		}

		return false;
	}
}
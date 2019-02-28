using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteCarTrap : Trap {

	public AudioClip accelerationSound;
	public AudioClip stopSound;
	public AudioClip hitSound;
	protected List<Enemy> enemiesHit;
	protected Animator anim;

	[SerializeField]
	protected int damage;
	[SerializeField]
	protected int knockback;
	[SerializeField]
	protected float speed;
	[SerializeField]
	private bool inflictsBleeding;
	[SerializeField]
	private bool inflictsBurning;

	protected override void Start () {
		anim = GetComponentInChildren<Animator> ();
		anim.enabled = false;

		base.Start ();
	}

	protected void OnTriggerEnter2D(Collider2D other) {
		if (isDeployed) {
			if (other.gameObject.tag == "Enemy") {
				Enemy enemy = other.gameObject.GetComponent<Enemy> ();
				if (enemiesHit.Contains (enemy)) {
					return;
				}
					
				enemiesHit.Add (enemy);
				float direction = transform.position.x - other.transform.position.x;
				other.gameObject.GetComponent<Enemy> ().takeHit (damage, knockback, direction, false, attackType);
				if (inflictsBleeding) {
					enemy.setBleeding ();
				}
				if (inflictsBurning) {
					enemy.setBurning ();
				}

				if (UnityEngine.Random.Range (0, 3) == 1) {
					if ((state == 0) && (bloodySprite1 != null)) {
						GetComponent<SpriteRenderer> ().sprite = bloodySprite1;
						state++;
					} else if ((state == 1) && (bloodySprite2 != null)) {
						GetComponent<SpriteRenderer> ().sprite = bloodySprite2;
						state++;
					} else if ((state == 2) && (bloodySprite3 != null)) {
						GetComponent<SpriteRenderer> ().sprite = bloodySprite3;
						state++;
					}
				}

				soundController.playPriorityOneShot (hitSound);
				durability--;
				if (durability == 0) {
					breakItem ();
				}
			} else if (other.gameObject.tag == "AreaWall") {
				isDeployed = false;
				triggerCollider.enabled = false;
				pickupCollider.enabled = true;

				gameObject.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
				anim.enabled = false;

				soundController.stopEnvironmentalSound (accelerationSound);
				soundController.playPriorityOneShot (stopSound);

				gameObject.layer = 13;
				gameObject.tag = "Item";
			}
		}
		base.trigger ();
	}

	public override void deploy() {
		enemiesHit = new List<Enemy> ();

		transform.parent = null;
		pickupCollider.enabled = false;
		hitCollider.enabled = false;
		triggerCollider.enabled = true;
		isHeld = false;

		frontHand.SetActive (false);
		backHand.SetActive (false);

		gameObject.layer = 18;
		GetComponent<SpriteRenderer> ().sortingLayerName = "Items";
		moveToDeployPos ();

		isDeployed = true;
		tag = "Trap";

		playerCon.alignHands ();
		playerCon.showPlayerHands ();

		playerCon.heldItem = null;

		//Update UI box
		playerCon.activeSlot.setEmpty();

		int direction;
		if (playerCon.playerSprite.flipX) {
			direction = -1;
		} else {
			direction = 1;
		}

		anim.enabled = true;
		GetComponent<Rigidbody2D> ().velocity = new Vector2 (speed * direction, 0f);

		soundController.playEnvironmentalSound (accelerationSound);

		onDeploy ();
	}

	protected virtual void onDeploy() {

	}

	public void moveToDeployPos () {
		transform.position = new Vector2 (transform.position.x, deployY);
		transform.eulerAngles = new Vector3 (0, 0, deployRotation);
	}

	public override bool onBreak() {
		soundController.stopEnvironmentalSound (accelerationSound);

		return false;
	}
}

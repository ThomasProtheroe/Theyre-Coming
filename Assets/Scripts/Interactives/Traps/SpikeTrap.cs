using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : Trap {

	public int damage;
	public AudioClip hitSound;

	[SerializeField]
	private Sprite altDeploySprite;
	[SerializeField]
	private Sprite altBloodySprite1;
	[SerializeField]
	private Sprite altBloodySprite2;
	[SerializeField]
	private Sprite altBloodySprite3;

	void OnTriggerEnter2D (Collider2D other) {
		if (isDeployed) { 
			if ((other.gameObject.tag == "Enemy" && !other.isTrigger) || (other.gameObject.tag == "Player" && friendlyFire)) {
				trigger (other.gameObject);
			}
		}
	}

	override public void trigger(GameObject victim) {
		Enemy enemy = victim.GetComponent<Enemy> ();
		enemy.takeHit (damage, 0, 0, true, attackType);
		enemy.setBleeding ();
		soundController.playPriorityOneShot (hitSound);

		durability--;
		if (durability <= 0) {
			breakItem ();
		} else {
			setBloodyState ();
		}
	}

	public void delayDeployment(float delayTime) {
		isDeployed = false;
		Invoke ("deploy", delayTime);
	}

	public override void deploy() {
		transform.parent = null;
		hitCollider.enabled = false;
		pickupCollider.enabled = false;
		triggerCollider.enabled = true;

		bool autoDeploy = false;
		if (isHeld == false) {
			autoDeploy = true;
		}
		isHeld = false;

		disableAnimator ();

		frontHand.SetActive (false);
		backHand.SetActive (false);

		gameObject.layer = 18;
		GetComponent<SpriteRenderer> ().sortingLayerName = "Background Items";
		moveToDeployPos ();
		deployedArea = playerCon.getCurrentArea ().name;

		if (altDeploySprite != null && deployedArea == "Yard") {
			sprite.sprite = altDeploySprite;
		}

		isDeployed = true;
		tag = "Trap";

		if (autoDeploy == false)
		{
			playerCon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController> ();
			playerCon.alignHands ();
			playerCon.showPlayerHands ();

			playerCon.heldItem = null;

			//Update UI box
			playerCon.activeSlot.setEmpty();
		}

		playTrapDeploySound();
	}

	public void moveToDeployPos () {
		transform.position = new Vector2 (transform.position.x, deployY);
		transform.eulerAngles = new Vector3 (0, 0, deployRotation);
	}

	protected override void setBloodyState() {
		if (deployedArea == "Yard") {
			if ((state == 0) && (altBloodySprite1 != null)) {
				sprite.sprite = altBloodySprite1;
				state++;
			} else if ((state == 1) && (altBloodySprite2 != null)) {
				sprite.sprite = altBloodySprite2;
				state++;
			} else if ((state == 2) && (altBloodySprite3 != null)) {
				sprite.sprite = altBloodySprite3;
				state++;
			}
		} else {
			base.setBloodyState ();
		}
	}
}

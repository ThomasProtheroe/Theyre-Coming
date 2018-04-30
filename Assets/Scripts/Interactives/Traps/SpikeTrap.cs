using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : Trap {

	public int damage;
	public AudioClip hitSound;

	void OnTriggerEnter2D (Collider2D other) {
		if (isDeployed) { 
			if ((other.gameObject.tag == "Enemy" && !other.isTrigger) || (other.gameObject.tag == "Player" && friendlyFire)) {
				trigger (other.gameObject);
			}
		}
	}

	override public void trigger(GameObject victim) {
		Enemy enemy = victim.GetComponent<Enemy> ();
		enemy.takeHit (damage, 0, 0, true);
		enemy.setBleeding ();
		source.PlayOneShot (hitSound);


		durability--;
		if (durability <= 0) {
			breakItem ();
		} else {
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
	}

	public override void deploy() {
		transform.parent = null;
		hitCollider.enabled = false;
		triggerCollider.enabled = true;
		isHeld = false;

		disableAnimator ();

		frontHand.SetActive (false);
		backHand.SetActive (false);

		gameObject.layer = 18;
		GetComponent<SpriteRenderer> ().sortingLayerName = "Background Items";
		moveToDeployPos ();

		isDeployed = true;
		tag = "Trap";

		playerCon.alignHands ();
		playerCon.showPlayerHands ();

		playerCon.heldItem = null;

		//Update UI box
		playerCon.activeSlot.setEmpty();
	}

	public void moveToDeployPos () {
		transform.position = new Vector2 (transform.position.x, deployY);
		transform.eulerAngles = new Vector3 (0, 0, deployRotation);
	}
}

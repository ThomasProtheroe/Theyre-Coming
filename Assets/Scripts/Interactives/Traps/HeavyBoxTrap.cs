using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBoxTrap : Trap {
	
	public int damage;
	public AudioClip hitSound;

	private Transition parentTransition;
	private bool isFalling = false;
	private bool hitSoundPlayed = false;

	protected void OnTriggerEnter2D(Collider2D other) {
		if (isFalling) {
			if (other.gameObject.tag == "Enemy") {
				other.gameObject.GetComponent<Enemy> ().takeHit (damage, 0);
				if (!hitSoundPlayed) {
					hitSoundPlayed = true;
					source.PlayOneShot (hitSound);
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
		playerCon.descriptionPanel.hideDescription ();
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
	}
}
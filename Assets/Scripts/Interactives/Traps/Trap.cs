using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Item {

	public int durability;
	public bool friendlyFire;

	public float deployY;
	public float deployRotation;

	private bool isDeployed;

	public Collider2D triggerCollider;

	void OnTriggerEnter2D (Collider2D other) {
		if (isDeployed) { 
			if ((other.gameObject.tag == "Enemy" && !other.isTrigger) || (other.gameObject.tag == "Player" && friendlyFire)) {
				trigger (other.gameObject);
			}
		}
	}

	override public void use() {
		deploy ();
	}

	public void deploy() {
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

		PlayerController pc = player.GetComponent<PlayerController> ();
		pc.alignHands ();
		pc.showPlayerHands ();

		pc.heldItem = null;

		//Update UI box
		pc.activeSlot.setEmpty();
		pc.descriptionPanel.hideDescription ();
	}

	public void moveToDeployPos () {
		transform.position = new Vector2 (transform.position.x, deployY);
		transform.eulerAngles = new Vector3 (0, 0, deployRotation);
	}

	public virtual void trigger(GameObject other) {
		                                                                
	}
}

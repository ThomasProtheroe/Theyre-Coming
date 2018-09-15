﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanTrap : Trap {

	public int damage;
	public int knockback;
	[SerializeField]
	private AudioClip hitSound;
	[SerializeField]
	private AudioClip activeSound;
	[SerializeField]
	protected Animator anim;

	[Header("Status Effects")]
	public bool inflictsBleed;
	public bool inflictsBlind;
	public bool inflictsBurning;

	void OnTriggerStay2D (Collider2D other) {
		if (isDeployed) {
			if ((other.gameObject.tag == "Enemy" && !other.isTrigger) || (other.gameObject.tag == "Player" && friendlyFire)) {
				trigger (other.gameObject);
			}
		}
	}

	override public void trigger(GameObject victim) {
		Enemy enemy = victim.GetComponent<Enemy> ();

		float direction = transform.position.x - victim.transform.position.x;
		if (!enemy.takeHit (damage, knockback, direction, false, attackType)) {
			return;
		};
		soundController.playPriorityOneShot (hitSound);

		//Apply status effects
		if (inflictsBleed) {
			enemy.setBleeding ();
		}
		if (inflictsBlind) {
			enemy.setBlind ();
		}
		if (inflictsBurning) {
			enemy.setBurning ();
		}

		reduceDurability ();
	}

	public override void deploy() {
		transform.parent = null;
		hitCollider.enabled = false;
		pickupCollider.enabled = false;
		triggerCollider.enabled = true;
		isHeld = false;

		frontHand.SetActive (false);
		backHand.SetActive (false);

		gameObject.layer = 18;
		GetComponent<SpriteRenderer> ().sortingLayerName = "Background Items";
		moveToDeployPos ();
		anim.enabled = true;
		startActiveSound ();

		isDeployed = true;
		tag = "Trap";

		playerCon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController> ();
		playerCon.alignHands ();
		playerCon.showPlayerHands ();

		playerCon.heldItem = null;

		//Update UI box
		playerCon.activeSlot.setEmpty();
	}

	protected void reduceDurability() {
		durability--;

		if (durability <= 0) {
			breakItem ();
		} else {
			//Update UI
			updateDurabilityIndicator();

			if (durability < maxDurability * 0.5f) {
				for (int i = 0; i < anim.layerCount; i++) {
					anim.SetLayerWeight (i, 0.0f);
				}

				anim.SetLayerWeight (anim.GetLayerIndex("Blood 3"), 1.0f);
			} else if (durability < maxDurability * 0.7f) {
				for (int i = 0; i < anim.layerCount; i++) {
					anim.SetLayerWeight (i, 0.0f);
				}

				anim.SetLayerWeight (anim.GetLayerIndex("Blood 2"), 1.0f);
			} else if (durability < maxDurability * 0.9f) {
				for (int i = 0; i < anim.layerCount; i++) {
					anim.SetLayerWeight (i, 0.0f);
				}

				anim.SetLayerWeight (anim.GetLayerIndex("Blood 1"), 1.0f);
			}
		}
	}

	public void moveToDeployPos () {
		transform.position = new Vector2 (transform.position.x, deployY);
		transform.eulerAngles = new Vector3 (0, 0, deployRotation);
	}

	private void startActiveSound() {
		soundController.playEnvironmentalSound (activeSound);
	}

	private void stopActiveSound() {
		soundController.stopEnvironmentalSound (activeSound);
	}

	public override void onPickup() {
		disableAnimator ();
	}

	public override bool onBreak() {
		anim.enabled = false;

		return false;
	}
}
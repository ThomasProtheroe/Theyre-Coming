using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mulcher : Weapon {

	private bool isRunning;
	private bool isHitting;
	private bool playingHitSound;
	[SerializeField]
	private BoxCollider2D damageHitbox;
	private float bloodHitCount;
	[SerializeField]
	private Sprite idleSprite;

	[Header("Mulcher Sounds")]
	[SerializeField]
	private AudioClip turnOnSound;
	[SerializeField]
	private AudioClip runningSound;
	[SerializeField]
	private AudioClip turnOffSound;
	[SerializeField]
	private AudioClip hitStartSound;
	[SerializeField]
	private AudioClip hitMaintainSound;
	[SerializeField]
	private AudioClip hitStopSound;

	new void OnTriggerStay2D(Collider2D other) {
		if (!isRunning || other.gameObject.tag != "Enemy") {
			return;
		}

		if (!other.gameObject.GetComponent<Enemy>().isInvulnerable && !other.gameObject.GetComponent<Enemy>().getIsDead()) {
			//Mulcher uses the opposite direction from most weapons for the blood spray
			float direction = (player.transform.position.x - other.transform.position.x) * -1.0f;
			other.gameObject.GetComponent<Enemy> ().takeHit (attackDamage, knockback, direction, false, attackType, true);
			onEnemyImpact (other.gameObject);

			hitCount = 1;
			reduceDurability ();
		}
	}

	override public void use() {
		if (isRunning == false) {
			turnOn ();
		} else {
			turnOff ();
		}
	}

	private void turnOn() {
		if (isRunning) {
			return;
		}

		isRunning = true;
		damageHitbox.enabled = true;
		soundController.playPlayerItemSound (turnOnSound, false, runningSound);

		anim.SetBool ("Running", true);
	}

	private void turnOff() {
		if (!isRunning) {
			return;
		}

		isRunning = false;
		damageHitbox.enabled = false;
		soundController.playPlayerItemSound (turnOffSound);

		anim.SetBool ("Running", false);
	}

	protected override void reduceDurability() {
		durability--;

		if (durability <= 0) {
			breakItem ();
		} else {
			//Update UI
			updateDurabilityIndicator();
			setBloodyAnim ();
		}
	}

	public override void onDrop() {
		turnOff ();
	}

	public override void onThrow() {
		turnOff ();
	}

	public override void onStash() {
		turnOff ();
	}

	public override void onPickup() {
		setBloodyAnim ();
	}
		
	public override bool onBreak() {
		turnOff ();
		return base.onBreak ();
	}

	private void setBloodyAnim() {
		if ((durability < maxDurability * 0.7f) && (bloodySprite1 != null)) {
			for (int i = 0; i < anim.layerCount; i++) {
				anim.SetLayerWeight (i, 0.0f);
			}

			anim.SetLayerWeight (anim.GetLayerIndex("Blood 4"), 1.0f);
		} else if ((durability < maxDurability * 0.8f) && (bloodySprite2 != null)) {
			for (int i = 0; i < anim.layerCount; i++) {
				anim.SetLayerWeight (i, 0.0f);
			}

			anim.SetLayerWeight (anim.GetLayerIndex("Blood 3"), 1.0f);
		} else if ((durability < maxDurability * 0.9f) && (bloodySprite3 != null)) {
			for (int i = 0; i < anim.layerCount; i++) {
				anim.SetLayerWeight (i, 0.0f);
			}

			anim.SetLayerWeight (anim.GetLayerIndex("Blood 2"), 1.0f);
		} else if ((durability < maxDurability * 0.95f) && (bloodySprite4 != null)) {
			for (int i = 0; i < anim.layerCount; i++) {
				anim.SetLayerWeight (i, 0.0f);
			}

			anim.SetLayerWeight (anim.GetLayerIndex("Blood 1"), 1.0f);
		}
	}

}

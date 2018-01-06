using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item {

	public int attackDamage;
	public int durability;
	public int knockback;
	public int multiHit = 1;

	public bool isBroken;

	public Sprite bloodySprite1;
	public Sprite bloodySprite2;
	public Sprite bloodySprite3;

	public AudioClip swingSound;
	public AudioClip hitSound;
	public AudioClip breakSound;

	private int state = 0;
	private int hitCount = 0;

	void OnCollisionEnter2D(Collision2D other) {
		if (isThrown) {
			if (other.gameObject.tag == "Enemy") {
				other.gameObject.GetComponent<Enemy> ().takeHit (thrownDamage, knockback);
				onEnemyImpact (other.gameObject);
			} else {
				onTerrainImpact ();
			}

			isThrown = false;
			isBouncing = true;
			gameObject.layer = 11;
		}

		if (isAttacking && other.gameObject.tag == "Enemy" && !other.gameObject.GetComponent<Enemy>().getIsDead()) {
			if (!canHit ()) {
				return;
			}
			hitCount ++;
			other.gameObject.GetComponent<Enemy> ().takeHit (attackDamage, knockback);
			onEnemyImpact (other.gameObject);
		}
	}

	override public void use() {
		attack ();
	}

	public void attack() {
		Animator anim = GetComponent<Animator> ();
		anim.enabled = true;
		isAttacking = true;
		hitCount = 0;

		if (swingSound) {
			source.PlayOneShot (swingSound);
		}
		anim.SetTrigger ("Attack");
	}

	public void disableAnimator() {
		GetComponent<Animator> ().enabled = false;
		isAttacking = false;
	}

	bool canHit() {
		if (hitCount >= multiHit) {
			return false;
		}
		return true;
	}

	public void onEnemyImpact(GameObject enemy) {
		durability -= 1;
		if (isThrown && throwImpact) {
			source.PlayOneShot (throwImpact);
		} else if (!isThrown && hitSound) {
			source.PlayOneShot (hitSound);
		}
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

	public virtual void onTerrainImpact() {

	}

	public void breakItem() {
		bool breakImmed = onBreak ();

		if (breakSound) {
			source.PlayOneShot (breakSound);
		}

		if (isHeld) {
			player.GetComponent<PlayerController> ().heldItem = null;
			gameObject.transform.parent = null;
		}

		//If we don't want to play the break animation, destroy the object now
		if (breakImmed) {
			Destroy (gameObject);
			return;
		}

		Rigidbody2D body = GetComponent<Rigidbody2D>();

		gameObject.layer = 11;

		int xBreakForce = 120;
		int yBreakForce = 100;

		//TODO Randomize direction/force of xBreakForce

		isHeld = false;
		isBroken = true;

		body.bodyType = RigidbodyType2D.Dynamic;
		body.AddForce (new Vector2 (xBreakForce, yBreakForce));
		body.AddTorque (25.0f);

		StartCoroutine ("beginSpriteFlash");
		StartCoroutine ("destroyAfterTime", 1.5f);
	}

	//Return true if item should be destroyed immediately (no animation)
	public virtual bool onBreak() {
		return false;
	}

	/**** Coroutines ****/ 
	IEnumerator destroyAfterTime(float time) {
		yield return new WaitForSeconds(time);

		Destroy (gameObject);
	}

	IEnumerator beginSpriteFlash() {
		while (true) {
			SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer> ();
			if (sprite.enabled) {
				sprite.enabled = false;
			} else {
				sprite.enabled = true;
			}

			yield return new WaitForSeconds (0.05f);
		}
	}
}

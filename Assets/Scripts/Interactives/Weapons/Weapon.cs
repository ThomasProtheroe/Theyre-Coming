using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item {

	public int attackDamage;
	public int durability;
	public int knockback;
	public int multiHit = 1;
	public bool inflictsBleed;
	public bool instantAttack;

	public AudioClip swingSound;
	public AudioClip hitSound;

	private int hitCount = 0;
	private bool hitWindowActive = false;

	new void OnCollisionEnter2D(Collision2D other) {
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

		if (isAttacking && hitWindowActive && other.gameObject.tag == "Enemy" && !other.gameObject.GetComponent<Enemy>().isInvunlerable && !other.gameObject.GetComponent<Enemy>().getIsDead()) {
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
		isAttacking = true;
		hitCount = 0;
		anim.SetTrigger ("Attack");

		Collider2D[] colliders = new Collider2D[10];
		hitCollider.OverlapCollider(new ContactFilter2D(), colliders);
		foreach (Collider2D collider in colliders) {
			if (collider && instantAttack && collider.gameObject.tag == "Enemy" && !collider.gameObject.GetComponent<Enemy>().isInvunlerable&& !collider.gameObject.GetComponent<Enemy> ().getIsDead ()) {
				if (!canHit ()) {
					break;
				}
				hitCount ++;
				collider.gameObject.GetComponent<Enemy> ().takeHit (attackDamage, knockback);
				onEnemyImpact (collider.gameObject);
			}
		}
	}

	protected void playAttackSound() {
		if (swingSound) {
			source.PlayOneShot (swingSound);
		}
	}

	protected void startHitWindow() {
		hitWindowActive = true;
	}

	protected void endHitWindow() {
		hitWindowActive = false;
	}

	public void finishAttack() {
		isAttacking = false;
	}

	bool canHit() {
		if (hitCount >= multiHit) {
			return false;
		}
		return true;
	}

	public virtual void onEnemyImpact(GameObject enemy) {
		durability -= 1;

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

		if (inflictsBleed) {
			enemy.GetComponent<Enemy> ().setBleeding();
		}

		if (isThrown && throwImpact) {
			source.PlayOneShot (throwImpact);
		} else if (!isThrown && hitSound) {
			source.PlayOneShot (hitSound);
		}
	}
}

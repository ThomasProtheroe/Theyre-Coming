using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item {

	public int attackDamage;
	public int durability;
	public int knockback;

	public Sprite bloodySprite1;
	public Sprite bloodySprite2;
	public Sprite bloodySprite3;

	private int state = 0;
	private bool enemyHit = false;

	void Update() {
		Animator anim = GetComponent<Animator> ();
		//Debug.Log(anim.GetBool ("FacingRight"));
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (isThrown) {
			if (other.gameObject.tag == "Enemy") {
				other.gameObject.GetComponent<Enemy> ().takeHit (thrownDamage, knockback);
				onEnemyImpact (other.gameObject);
			}

			onTerrainImpact ();

			isThrown = false;
			isBouncing = true;
		}

		if (isAttacking && other.gameObject.tag == "Enemy" && !enemyHit && !other.gameObject.GetComponent<Enemy>().getIsDead()) {
			enemyHit = true;
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
		enemyHit = false;

		anim.SetTrigger ("Attack");
	}

	public void disableAnimator() {
		GetComponent<Animator> ().enabled = false;
		isAttacking = false;
	}

	public void onEnemyImpact(GameObject enemy) {
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
	}

	public virtual void onTerrainImpact() {

	}

	public void breakItem() {
		onBreak ();
		if (isHeld) {
			player.GetComponent<PlayerController> ().heldItem = null;
		}
		Destroy (gameObject);
	}

	public virtual void onBreak() {

	}
}

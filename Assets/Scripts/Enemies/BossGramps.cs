﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGramps : Enemy {
	[SerializeField]
	private int maxBurnDamage;

	[Header("Spit Attack")]
	[SerializeField]
	private ParticleSystem droolPS;
	[SerializeField]
	private ParticleSystem spitPS;
	[SerializeField]
	private BileProjectile projectile;
	[SerializeField]
	private float projectileSpeed;

	[Header("Leap Attack")]
	[SerializeField]
	private float leapXVelocity;
	[SerializeField]
	private float leapYVelocity;
	[SerializeField]
	private float leapDuration;
	[SerializeField]
	private Sprite leapFrame;
	[SerializeField]
	private Sprite leapPrepFrame;

	private int currentBurnDamage;
	private bool isSpitting;
	private bool isLeaping;

	//Behaviour control
	[Space(1)]
	[Header("Behaviour Parameters")]
	[SerializeField]
	private float attackAggroRange;

	protected override void Update() {

		base.Update ();
	}

	public override void takeAction() {
		if (isActive) {
			if (!isAttacking && !isDead) {
				if (player.GetComponent<PlayerController> ().getCurrentArea () == currentArea) {
					moveTowardsPlayer ();
					tryAttack ();
				} else {
					seekPlayer ();
				}
			}
		} else {
			stopMoving ();
		}
	}

	protected override void tryAttack() {
		if (distanceToPlayer <= attackRange) {
			attack ();
		} else if (distanceToPlayer <= attackAggroRange) {
			if (UnityEngine.Random.Range (0, 2) == 1) {
				spitAttack ();
			} else {
				leapAttack ();
			}
		}
	}

	public void setBlind() {
		//Zombie gramps needs no eyes!
		return;
	}

	public override void takeHit(int damage, int knockback, float direction, bool noBlood=false, int attackType=Constants.ATTACK_TYPE_UNTYPED) {
		if (isSpitting) {
			interruptSpitAttack ();
		} else if (isLeaping) {
			int newKnockback = interruptLeapAttack ();
			if (newKnockback != 0) {
				knockback = newKnockback;
			}
		}
			
		base.takeHit (damage, knockback, direction, noBlood, attackType);

		//TODO gramps jumps out of attack range after taking a hit. This way the player can't hammer him repeatedly with high speed weapons

	}

	public void takeFireHit(int damage) {
		if (burnImmunityTimer > 0.0f) {
			return;
		}

		playerAttackType = Constants.ATTACK_TYPE_FIRE;
		burnImmunityTimer = 1.0f;
		takeDamage (damage);
		setBurning ();
		playerAttackType = Constants.ATTACK_TYPE_UNTYPED;
	}


	public override void takeBurnDamage(int damage) { 
		takeDamage (damage);
		//Limits how much damage gramps can take from burning
		currentBurnDamage++;
		if (currentBurnDamage >= maxBurnDamage) {
			stopBurning ();
		}
	}

	private void spitAttack() {
		isSpitting = true;
		isAttacking = true;
		stopMoving ();
		StartCoroutine ("SpitAttack");
	}

	private void interruptSpitAttack() {
		if (!isSpitting) {
			return;
		}

		isSpitting = false;
		isAttacking = false;
		setStun (1.0f);
		StopCoroutine ("SpitAttack");
		anim.enabled = true;
	}

	private void fireBileProjectile() {
		BileProjectile bp = Instantiate (projectile, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);

		float speed = projectileSpeed;
		if (!enemySprite.flipX) {
			speed *= -1;
		}

		bp.GetComponent<Rigidbody2D> ().velocity = new Vector2 (speed, 0.0f);
		bp.lifetime = 0.65f;
	}

	private void leapAttack() {
		isLeaping = true;
		isAttacking = true;
		hitPlayer = false;
		isMoving = false;
		rigidBody.velocity = new Vector2 (0, rigidBody.velocity.y);
		StartCoroutine ("LeapAttack");
	}

	private int interruptLeapAttack() {
		if (!isLeaping) {
			return 0;
		}
			
		isLeaping = false;
		StopCoroutine ("LeapAttack");
		finishAttack ();

		int knockback = 0;
		if (transform.position.y > groundLevel) {
			isStunned = true;
			StartCoroutine ("Fall");
			knockback = 4;
		} else {
			knockback = 0;
			stopMoving ();
			anim.enabled = true;
		}

		return knockback;
	}

	IEnumerator LeapAttack() {
		//Prepare to leap
		deactivate();
		anim.enabled = false;
		enemySprite.sprite = leapPrepFrame;

		yield return new WaitForSeconds(1.0f);

		activateAttackHitbox ();
		enemySprite.sprite = leapFrame;

		int direction;
		if (enemySprite.flipX) {
			direction = 1;
		} else {
			direction = -1;
		}
		rigidBody.velocity = new Vector2 (leapXVelocity * direction, leapYVelocity);
		isMoving = true;

		yield return new WaitForSeconds(leapDuration * 0.4f);

		rigidBody.velocity = new Vector2 (rigidBody.velocity.x, 0.0f);

		yield return new WaitForSeconds(leapDuration * 0.2f);

		rigidBody.velocity = new Vector2 (rigidBody.velocity.x, leapYVelocity * -1);

		yield return new WaitForSeconds(leapDuration * 0.4f);

		rigidBody.velocity = Vector2.zero;

		anim.enabled = true;
		finishAttack ();
		isLeaping = false;
		isMoving = false;

		yield return new WaitForSeconds (2.0f);
		activate ();
	}

	IEnumerator Fall() {
		while(transform.position.y > groundLevel) {
			rigidBody.velocity = new Vector2 (rigidBody.velocity.x, rigidBody.velocity.y - 0.2f);
			yield return null;
		}

		rigidBody.velocity = new Vector2 (rigidBody.velocity.x, 0.0f);
		transform.position = new Vector3 (transform.position.x, groundLevel, transform.position.z);

		isMoving = false;
		anim.enabled = true;
		Invoke ("activate", 2.0f);
	}

	IEnumerator SpitAttack() {
		//Prepare to spit
		deactivate();
		droolPS.Play ();

		var sh = spitPS.shape;
		float spitDirection;
		if ((transform.position.x - player.transform.position.x) > 0) {
			spitDirection = 180;
		} else {
			spitDirection = 0;
		}

		yield return new WaitForSeconds (0.8f);

		anim.enabled = false;
		droolPS.Stop ();

		if ((transform.position.x - player.transform.position.x) > 0) {
			spitDirection = 180;
		} else {
			spitDirection = 0;
		}
		sh.rotation = new Vector3 (sh.rotation.x, spitDirection, sh.rotation.z);
		spitPS.Play ();

		while(spitPS.isEmitting) {
			fireBileProjectile ();
			yield return new WaitForSeconds(0.2f);
		}

		anim.enabled = true;

		yield return new WaitForSeconds (0.5f);

		activate ();
		isSpitting = false;
		isAttacking = false;
	}
}

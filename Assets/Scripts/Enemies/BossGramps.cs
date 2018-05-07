using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGramps : Enemy {

	[SerializeField]
	private ParticleSystem droolPS;
	[SerializeField]
	private ParticleSystem spitPS;
	[SerializeField]
	private BileProjectile projectile;
	[SerializeField]

	private float projectileSpeed;
	[SerializeField]
	private int maxBurnDamage;
	private int currentBurnDamage;
	private bool isSpitting;

	//Behaviour control
	[Space(1)]
	[Header("Behaviour Parameters")]
	[SerializeField]
	private float spitAggroRange;


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
			isMoving = false;
			rigidBody.velocity = new Vector2 (0, 0);
		}
	}

	protected override void tryAttack() {
		if (distanceToPlayer <= attackRange) {
			attack ();
		} else if (distanceToPlayer <= spitAggroRange) {
			spitAttack ();
		}
	}

	public void setBlind() {
		//Zombie gramps needs no eyes!
		return;
	}

	public override void takeHit(int damage, int knockback, float direction, bool noBlood=false, int attackType=Constants.ATTACK_TYPE_UNTYPED) {
		if (isSpitting) {
			interruptSpitAttack ();
		}

		//TODO Normal hit logic, but knockback is dependant on gramps' state
		int newKnockback = 0;
		base.takeHit (damage, newKnockback, direction, noBlood, attackType);

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

		yield return new WaitForSeconds (0.6f);

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

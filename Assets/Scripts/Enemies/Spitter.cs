using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitter : Enemy {
	private bool isSpitting;
	private bool spitAttackReady;

	[Header("Spit Attack")]
	[SerializeField]
	private ParticleSystem droolPS;
	[SerializeField]
	private ParticleSystem spitPS;
	[SerializeField]
	private BileProjectile projectile;
	[SerializeField]
	private float projectileSpeed;

	//Behaviour control
	[Header("Behaviour Parameters")]
	[SerializeField]
	private float spitCooldown;
	private float spitCooldownTimer;
	[SerializeField]
	private float guardRange;
	private Enemy guard;
    
	public override void takeAction() {
		if (isActive) {
			if (isBlind) {
				//Move randomly back and forth
				if (wanderTimer > 0) {
					wanderBlind ();
				} else {
					startWander ();
				}

				//Attack randomly
				if (wanderAttackTimer <= 0) {
					spitAttack ();
					wanderAttackTimer = UnityEngine.Random.Range (7.0f, 8.0f);
				}
			} else {
				if (!isDead) {
					//Attack player whenever possible
					if (player.GetComponent<PlayerController> ().getCurrentArea () == currentArea) {
						if (spitAttackReady) {
							moveTowardsPlayer ();
							tryAttack ();
						}
					} else {
						seekPlayer ();
					}
				}
			}
		} else {
			stopMoving ();
		}
	}

	protected override void checkExtraTimers() {
		if (!spitAttackReady) {
			spitCooldownTimer -= Time.deltaTime;
			if (spitCooldownTimer <= 0f) {
				spitAttackReady = true;
				isActive = true;
				anim.SetBool("Active", true);
			}
		}
	}

	protected override void tryAttack() {
		if (distanceToPlayer < attackRange && spitAttackReady && !isAttacking) {
			facePlayer();
			spitAttack();
		}
	}

	protected void spitAttack() {
		isSpitting = true;
		isAttacking = true;
		spitCooldownTimer = spitCooldown;
		spitAttackReady = false;
		stopMoving ();
		StartCoroutine ("SpitAttack");
	}

    protected override void onTakeHit()
    {
		if (isSpitting) {
			interruptSpitAttack();
		}
    }

	private void interruptSpitAttack() {
		if (!isSpitting) {
			return;
		}

		isSpitting = false;
		isAttacking = false;
		droolPS.Stop ();
		spitPS.Stop ();
		setStun (3.0f);
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
		playAttackSound();

		var sh = spitPS.shape;
		float spitDirection;
		if ((transform.position.x - player.transform.position.x) > 0) {
			spitDirection = 180;
		} else {
			spitDirection = 0;
		}

		yield return new WaitForSeconds (1.0f);

		anim.enabled = false;
		droolPS.Stop ();

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
		isActive = false;
		anim.SetBool("Active", false);
	}
}
	
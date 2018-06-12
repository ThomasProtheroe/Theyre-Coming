using System.Collections;
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

	[SerializeField]
	private bool isPreparing;
	[SerializeField]
	private float prepTimer;
	[SerializeField]
	private bool isSpitting;
	[SerializeField]
	private bool isLeaping;
	[SerializeField]
	private bool isRecovering;
	private string selectedAttack;

	//Behaviour control
	[Header("Behaviour Parameters")]
	[SerializeField]
	private float attackAggroRange;
	[SerializeField]
	private float prepRange;
	[SerializeField]
	private float prepDelay;
	[SerializeField]
	private float evadeDuration;

	public override void takeAction() {
		if (isActive) {
			if (!isDead) {
				if (isPreparing && distanceToPlayer < 2.0f && playerCon.isAttacking) {
					isPreparing = false;
					isRecovering = true;
					StartCoroutine("LeapEvade");
				}

				if (isPreparing && prepTimer > 0.0f) {
					prepTimer -= Time.deltaTime;
				}

				if (player.GetComponent<PlayerController> ().getCurrentArea () == currentArea) {
					if (isPreparing && prepTimer <= 0.0f) {
						//Prep has ended, make an attack
						isPreparing = false;
						anim.SetBool ("Active", true);
						if (UnityEngine.Random.Range (0, 2) == 1) {
							selectedAttack = "spit";
						} else {
							selectedAttack = "leap";
						}
					}

					if (!isPreparing && !isRecovering) {
						moveTowardsPlayer ();
						if (selectedAttack != null) {
							tryAttack ();
						} else {
							if (distanceToPlayer <= prepRange) {
								isPreparing = true;
								prepTimer = prepDelay;
								stopMoving ();
								anim.SetBool ("Active", false);
							}
						}
					}

				} else {
					if (isPreparing) {
						isPreparing = false;
						prepTimer = 0.0f;
					}
					if (selectedAttack != null) {
						selectedAttack = null;
					}
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
			if (selectedAttack == "leap") {
				leapAttack ();
			} else if (selectedAttack == "spit") {
				spitAttack ();
			}
		}
	}

	public new void setBlind(float duration=0f) {
		//Zombie gramps needs no eyes!
		return;
	}

	public override void takeHit(int damage, int knockback, float direction, bool noBlood=false, int attackType=Constants.ATTACK_TYPE_UNTYPED) {
		if (isPreparing) {
			isPreparing = false;
			prepTimer = 0.0f;
		}
		else if (isSpitting) {
			interruptSpitAttack ();
		} else if (isLeaping) {
			int newKnockback = interruptLeapAttack ();
			if (newKnockback != 0) {
				knockback = newKnockback;
			}
		} 

		if (distanceToPlayer <= 2.0f) {
			isRecovering = true;
		}
			
		base.takeHit (damage, knockback, direction, noBlood, attackType);
	}

	public override void takeThrowHit(int damage, int knockback, float direction, bool noBlood=false, int attackType=Constants.ATTACK_TYPE_UNTYPED) {
		if (isPreparing) {
			isPreparing = false;
			prepTimer = prepDelay / 2;
			anim.enabled = true;
			anim.SetTrigger ("Deflect");
			return;
		}

		takeHit (damage, knockback, direction, noBlood, attackType);
	}

	public override void takeFireHit(int damage) {
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

	protected override void onKnockbackEnd() {
		if (!isRecovering || isDead) {
			return;
		}
			
		StartCoroutine("LeapEvade");
	}

	private void spitAttack() {
		isSpitting = true;
		isAttacking = true;
		selectedAttack = null;
		stopMoving ();
		StartCoroutine ("SpitAttack");
	}

	private void interruptSpitAttack() {
		if (!isSpitting) {
			return;
		}

		isSpitting = false;
		isAttacking = false;
		droolPS.Stop ();
		spitPS.Stop ();
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
		selectedAttack = null;
		hitPlayer = false;
		stopMoving ();
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
			setStun (10.0f);
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

		stopMoving ();
		transform.position = new Vector3 (transform.position.x, groundLevel, transform.position.z);

		anim.enabled = true;
		finishAttack ();
		isLeaping = false;
	}

	IEnumerator Fall() {
		while(transform.position.y > groundLevel) {
			rigidBody.velocity = new Vector2 (rigidBody.velocity.x, rigidBody.velocity.y - 0.4f);
			yield return null;
		}

		rigidBody.velocity = new Vector2 (rigidBody.velocity.x, 0.0f);
		transform.position = new Vector3 (transform.position.x, groundLevel, transform.position.z);

		stopMoving ();
		anim.enabled = true;
		deactivate ();
		setStun (2.0f);
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

	IEnumerator LeapEvade () {
		setStun (3.0f);
		isInvunlerable = true;
		anim.enabled = false;

		facePlayer();
		enemySprite.sprite = leapPrepFrame;

		int direction;
		if (enemySprite.flipX) {
			direction = -1;
		} else {
			direction = 1;
		}

		//If gramps has his back to a wall he will leap past the player instead
		RaycastHit2D wall = Physics2D.Raycast(transform.position, new Vector2(direction, 0f), 4.0f, 1 << LayerMask.NameToLayer("Terrain"));
		if (wall.collider != null) {
			direction *= -1;
		}

		rigidBody.velocity = new Vector2 (leapXVelocity * direction, leapYVelocity);
		isMoving = true;

		yield return new WaitForSeconds(evadeDuration * 0.4f);

		rigidBody.velocity = new Vector2 (rigidBody.velocity.x, 0.0f);

		yield return new WaitForSeconds(evadeDuration * 0.2f);

		rigidBody.velocity = new Vector2 (rigidBody.velocity.x, leapYVelocity * -1);

		yield return new WaitForSeconds(evadeDuration * 0.4f);

		endStun ();
		stopMoving ();
		facePlayer();
		anim.SetBool ("Active", false);
		transform.position = new Vector3 (transform.position.x, groundLevel, transform.position.z);
		anim.enabled = true;
		isInvunlerable = false;
		isRecovering = false;
	}
}

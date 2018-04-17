using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float moveSpeed = 1.5f;
	public float attackRange = 0.8f;
	public float burnDamageInterval = 1.0f;
	public int health = 10;
	public bool isInvunlerable;

	public BoxCollider2D attackHitbox;
	public BoxCollider2D bodyHitbox;
	public BoxCollider2D proximityHitbox;
	public AudioSource walkingSource;
	public AudioSource vocalSource;
	public Area currentArea;

	private Animator anim;
	private AudioClip walkSound;
	private AudioClip prowlSound;
	private List<AudioClip> attackSounds;
	private Rigidbody2D rigidBody;
	private SpriteRenderer enemySprite;
	private GameObject player;

	[SerializeField]
	private ParticleSystem bloodSprayPS;
	[SerializeField]
	private ParticleSystem burningPS;
	[SerializeField]
	ParticleSystem[] burningDetailPS;

	private bool isActive;
	private bool isMoving;
	private bool isAttacking;
	private bool isStunned;
	private bool isBurning;
	private bool isDead;
	private float distanceToPlayer;
	private float burnTimer;
	private float burnImmunityTimer;

	// Use this for initialization
	void Start () {
		anim = gameObject.GetComponent<Animator> ();
		rigidBody = gameObject.GetComponent<Rigidbody2D> ();
		enemySprite = gameObject.GetComponent<SpriteRenderer> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		isMoving = false;
		isAttacking = false;
		burnTimer = 0.0f;
		burnImmunityTimer = 0.0f;

		//Let players pass through the enemy
		Physics2D.IgnoreCollision (player.GetComponent<CapsuleCollider2D>(), bodyHitbox);
	}
	
	// Update is called once per frame
	void Update () {
		if (player.transform.position.x > transform.position.x) {
			distanceToPlayer = player.transform.position.x - transform.position.x;
		} else {
			distanceToPlayer = transform.position.x - player.transform.position.x;
		}

		bool inAudioRange = false;
		if (distanceToPlayer < 22) {
			inAudioRange = true;
		}

		if (!isStunned) {
			if (isActive && !isAttacking && !isDead) {
				if (player.GetComponent<PlayerController> ().getCurrentArea () == currentArea) {
					moveTowardsPlayer ();
				} else {
					//Track player and move towards transition
					Transition target = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().findRouteToPlayer(currentArea);
					faceTarget (target.transform.position);
					move ();
					if (Vector2.Distance(target.gameObject.transform.position, gameObject.transform.position) < 0.5) {
						target.enemyTravel (GetComponent<Enemy>());
					}
				}
			} else {
				isMoving = false;
				rigidBody.velocity = new Vector2 (0, 0);
			}

			//Sound effect Control
			if (inAudioRange && !isAttacking) {
				if (!vocalSource.isPlaying) {
					startProwlSound ();
				}
				if (isMoving && !walkingSource.isPlaying) {
					startWalkSound ();
				} else if (!isMoving && walkingSource.isPlaying) {
					stopWalkSound ();
				}
			} else {
				if (vocalSource.isPlaying) {
					stopProwlSound ();
				}
				if (walkingSource.isPlaying) {
					stopWalkSound ();
				}
			}
		}

		//Periodic effects
		if (isBurning) {
			if (burnTimer > 0.0f) {
				burnTimer -= Time.deltaTime;
			} else {
				burnTimer = burnDamageInterval;
				takeDamage (1);
				Debug.Log ("burn damage tick");
			}

			if (burnImmunityTimer > 0.0f) {
				burnImmunityTimer -= Time.deltaTime;
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Enemy") {
			if (isBurning) {
				Enemy otherEnemy = other.GetComponent<Enemy> ();
				if (!otherEnemy.isBurning) {
					otherEnemy.setBurning ();
				}
			}
		}
	}

	private void moveTowardsPlayer () {
		facePlayer ();
		move ();
		if (distanceToPlayer <= attackRange) {
			attack ();
		}
	}

	public void activate() {
		anim.SetBool ("Active", true);
		isActive = true;
	}

	public void deactivate() {
		anim.SetBool ("Active", false);
		isActive = false;
	}

	void move() {
		if (distanceToPlayer > attackRange && !isMoving) {
			isMoving = true;
			float currentSpeed = moveSpeed;
			if (!enemySprite.flipX) {
				currentSpeed *= -1;
			}
			rigidBody.velocity = new Vector2 (currentSpeed, rigidBody.velocity.y);
		} else if (distanceToPlayer <= attackRange && isMoving){
			isMoving = false;
			rigidBody.velocity = new Vector2 (0, rigidBody.velocity.y);
		}
	}

	void facePlayer() {
		if (player.transform.position.x > transform.position.x && !enemySprite.flipX) {
			enemySprite.flipX = true;
			attackHitbox.offset = new Vector2 (attackHitbox.offset.x * -1, 0);
		} else if (player.transform.position.x < transform.position.x && enemySprite.flipX) {
			enemySprite.flipX = false;
			attackHitbox.offset = new Vector2 (attackHitbox.offset.x * -1, 0);
		}
	}

	void faceTarget(Vector2 target) {
		if (target.x > transform.position.x && !enemySprite.flipX) {
			enemySprite.flipX = true;
			attackHitbox.offset = new Vector2 (attackHitbox.offset.x * -1, 0);
		} else if (target.x < transform.position.x && enemySprite.flipX) {
			enemySprite.flipX = false;
			attackHitbox.offset = new Vector2 (attackHitbox.offset.x * -1, 0);
		}
	}

	void attack() {
		playAttackSound ();
		anim.SetTrigger ("Attack");
		isAttacking = true;
	}

	public void activateAttackHitbox() {
		attackHitbox.enabled = true;
		attackHitbox.transform.position = new Vector2(attackHitbox.transform.position.x - 0.05f, attackHitbox.transform.position.y);
	}

	public void finishAttack() {
		isAttacking = false;
		attackHitbox.enabled = false;
		attackHitbox.transform.position = new Vector2(attackHitbox.transform.position.x + 0.05f, attackHitbox.transform.position.y);
	}

	public void takeHit(int damage, int knockback) {
		stopProwlSound ();
		stopWalkSound ();
		bloodSprayPS.Play ();
		if (!getIsDead ()) {
			StartCoroutine ("setHitFrame", knockback);
		}
		takeDamage (damage);
	}

	public void takeDamage(int damage) {
		if (isDead) {
			return;
		}

		health -= damage;
		if (health <= 0) {
			killEnemy ();
		}
	}

	public void killEnemy() {
		if (isAttacking) {
			finishAttack ();
		}
		isDead = true;

		//Kill the burning detail particle systems before playing death animation (if active)
		if (isBurning) {
			foreach (ParticleSystem detailPS in burningDetailPS) {
				detailPS.Stop ();
			}
		}

		anim.SetTrigger ("Death");
	}

	public void destroyEnemy() {
		Destroy (gameObject);
	}

	public void takeFireHit(int damage) {
		if (burnImmunityTimer > 0.0f) {
			return;
		}

		burnImmunityTimer = 1.0f;
		takeDamage (damage);
		setBurning ();
	}

	public void setBurning () {
		if (!isBurning) {
			isBurning = true;
			burnTimer = burnDamageInterval;
			burningPS.Play ();
			foreach (ParticleSystem detailPS in burningDetailPS) {
				detailPS.Play ();
			}
		}
	}

	public bool getIsDead() {
		return isDead;
	}

	public Area getCurrentArea() {
		return currentArea;
	}

	public void setCurrentArea(Area area) {
		currentArea = area;
	}

	public void setProwlSound(AudioClip sound) {
		prowlSound = sound;
		vocalSource.clip = prowlSound;
	}

	public void addAttackSound(AudioClip[] sounds) {
		attackSounds = new List<AudioClip> ();
		attackSounds.AddRange(sounds);
	}

	public void setWalkSound(AudioClip sound) {
		walkSound = sound;
		walkingSource.clip = walkSound;
	}

	public void startProwlSound() {
		vocalSource.Play ();
	}

	public void stopProwlSound() {
		vocalSource.Pause ();
	}

	private void playAttackSound() {
		vocalSource.PlayOneShot (attackSounds[Random.Range(0, attackSounds.Count - 1)]);
	}

	public void startWalkSound() {
		walkingSource.Play ();
	}

	public void stopWalkSound() {
		walkingSource.Pause ();
	}

	/**** Coroutines ****/ 
	IEnumerator setHitFrame(int knockbackWeight) {
		isStunned = true;
		isMoving = false;
		stopProwlSound ();
		if (isAttacking) {
			finishAttack ();
		}

		anim.SetBool ("Knockback", true);

		if (knockbackWeight > 0) {
			knockbackEnemy (knockbackWeight);
		} else {
			rigidBody.velocity = new Vector2 (0, rigidBody.velocity.y);
		}

		float knockbackTime = 0.15f + ((float)knockbackWeight / 100.0f);  //Calculate the actual time based on knockback stat
		yield return new WaitForSeconds(knockbackTime);

		anim.SetBool ("Knockback", false);
		isStunned = false;
	}

	private void knockbackEnemy(int knockbackWeight) {
		float xVelocity = (float)knockbackWeight * 2.0f;
		if (enemySprite.flipX) {
			xVelocity *= -1;
		}
		rigidBody.velocity = new Vector2 (xVelocity, rigidBody.velocity.y);
	} 
}
	
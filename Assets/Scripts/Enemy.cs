using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public BoxCollider2D attackHitbox;
	public BoxCollider2D bodyHitbox;
	public BoxCollider2D proximityHitbox;
	[HideInInspector]
	public Area currentArea;

	private GameController gc;
	private Animator anim;
	[SerializeField]
	private AnimatorOverrideController blindController;
	[SerializeField]
	private RuntimeAnimatorController defaultController;
	private Rigidbody2D rigidBody;
	private SpriteRenderer enemySprite;
	private GameObject player;
	[SerializeField]
	private EnemyCorpse enemyCorpse;

	/* Audio Components */
	[HideInInspector]
	public SoundController soundCon;
	public AudioSource walkingSource;
	public AudioSource vocalSource;
	public AudioSource miscSource;
	private AudioClip walkSound;
	private AudioClip prowlSound;
	[SerializeField]
	private AudioClip burnSound;
	private List<AudioClip> attackSounds;

	/* Particle Systems */
	[SerializeField]
	private ParticleSystem bloodSprayPS;
	[SerializeField]
	private ParticleSystem bleedingPS;
	[SerializeField]
	private ParticleSystem burningPS;
	[SerializeField]
	private ParticleSystem[] burningDetailPS;

	public float moveSpeed = 1.5f;
	public float attackRange = 0.8f;
	public float burnDamageInterval = 1.0f;
	public float bleedDamageInterval = 1.0f;
	public int health = 10;
	public bool isInvunlerable;
	private bool isActive;
	private bool isMoving;
	private bool isAttacking;
	private bool isStunned;
	private bool isBurning;
	private bool isBleeding;
	private bool isBlind;
	private bool isDead;
	private float distanceToPlayer;

	private bool walkSoundPlaying;
	private bool prowlSoundPlaying;

	private float burnTimer = 0.0f;
	private float burnImmunityTimer = 0.0f;
	private float bleedTimer = 0.0f;
	private float blindTimer = 0.0f;
	private float blindDuration = 20.0f;

	// Use this for initialization
	void Start () {
		anim = gameObject.GetComponent<Animator> ();
		defaultController = anim.runtimeAnimatorController;
		rigidBody = gameObject.GetComponent<Rigidbody2D> ();
		enemySprite = gameObject.GetComponent<SpriteRenderer> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		soundCon = GameObject.FindGameObjectWithTag ("SoundController").GetComponent<SoundController> ();
		gc = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		isMoving = false;
		isAttacking = false;

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
			if (inAudioRange && !isAttacking && !isDead) {
				if (!prowlSoundPlaying) {
					startProwlSound ();
				}
				if (isMoving && !walkSoundPlaying) {
					startWalkSound ();
				} else if (!isMoving && walkSoundPlaying) {
					stopWalkSound ();
				}
			} else {
				if (prowlSoundPlaying) {
					stopProwlSound ();
				}
				if (walkSoundPlaying) {
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
			}

			if (burnImmunityTimer > 0.0f) {
				burnImmunityTimer -= Time.deltaTime;
			}
		}
		if (isBleeding) {
			if (bleedTimer > 0.0f) {
				bleedTimer -= Time.deltaTime;
			} else {
				bleedTimer = bleedDamageInterval;
				takeDamage (1);
				StartCoroutine ("bleedDamageFlash");
			}
		}
		if (isBlind) {
			if (blindTimer > 0.0f) {
				blindTimer -= Time.deltaTime;
			} else {
				endBlind ();
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

	void OnParticleCollision(GameObject other) {

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
		if (damage > 0) {
			var sh = bloodSprayPS.shape;
			var main = bloodSprayPS.main;
			int particleCount = 15;
			if (damage >= 10) {
				sh.arc = 120.0f;
				particleCount = 260;
				main.startLifetime = 1.5f;
			} else if (damage >= 7) {
				sh.arc = 60;
				particleCount = 120;
				main.startLifetime = 1.0f;
			} else {
				sh.arc = 20;
				particleCount = 20;
				main.startLifetime = 0.6f;
			}
			bloodSprayPS.Emit (particleCount);
		}
		if (!getIsDead ()) {
			StartCoroutine ("setHitFrame", knockback);
			takeDamage (damage);
		}
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

		if (walkSoundPlaying) {
			soundCon.stopEnemyWalk (walkSound);
		}
		if (prowlSoundPlaying) {
			soundCon.stopEnemyProwl (prowlSound);
		}

		//De-parent the blood Particle System so the particles dont disappear when the enemy is destroyed
		bloodSprayPS.transform.parent = null;
		bloodSprayPS.GetComponent<DestroyAfterTime> ().StartCoroutine ("destroyAfterTime", 2.0f);

		//Kill the burning detail particle systems before playing death animation (if active)
		if (isBurning) {
			foreach (ParticleSystem detailPS in burningDetailPS) {
				detailPS.Stop ();
			}
		}

		anim.SetTrigger ("Death");
	}

	public void destroyEnemy() {
		spawnCorpse ();
		Destroy (gameObject);
	}

	private void spawnCorpse() {
		EnemyCorpse corpse = Instantiate (enemyCorpse);
		if (isBurning) {
			corpse.setBurntSprite ();
		} else {
			corpse.setGenericSprite ();
		}

		corpse.positionOnGround (transform.position.x);

		gc.addEnemyCorpse (corpse, currentArea.name);
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
			playBurnSound ();
		}
	}

	public void setBleeding() {
		if (!isBleeding) {
			isBleeding = true;
			bleedTimer = bleedDamageInterval;
			bleedingPS.Play ();
		}
	}

	public void setBlind() {
		if (!isBlind) {
			isBlind = true;
			blindTimer = blindDuration;
			anim.runtimeAnimatorController = blindController;
		}
	}

	public void endBlind() {
		isBlind = false;
		anim.runtimeAnimatorController = defaultController;
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
	}

	public void addAttackSound(AudioClip[] sounds) {
		attackSounds = new List<AudioClip> ();
		attackSounds.AddRange(sounds);
	}

	public void setWalkSound(AudioClip sound) {
		walkSound = sound;
	}

	public void startProwlSound() {
		if (!prowlSoundPlaying) {
			prowlSoundPlaying = true;
			soundCon.playEnemyProwl (prowlSound);
		}
	}

	public void stopProwlSound() {
		if (prowlSoundPlaying) {
			prowlSoundPlaying = false;
			soundCon.stopEnemyProwl (prowlSound);
		}
	}

	public void startWalkSound() {
		if (!walkSoundPlaying) {
			walkSoundPlaying = true;
			soundCon.playEnemyWalk (walkSound);
		}
	}

	public void stopWalkSound() {
		if (walkSoundPlaying) {
			walkSoundPlaying = false;
			soundCon.stopEnemyWalk (walkSound);
		}
	}

	private void playAttackSound() {
		vocalSource.PlayOneShot (attackSounds[Random.Range(0, attackSounds.Count - 1)]);
	}

	private void playBurnSound() {
		miscSource.PlayOneShot (burnSound);
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

	IEnumerator bleedDamageFlash() {
		//Turn red over time
		for (float f = 1f; f >= 0; f -= 0.2f) {
			Color c = enemySprite.material.color;
			c.g = f;
			c.b = f;
			enemySprite.material.color = c;

			yield return null;
		}

		//Turn back to original color
		for (float f = 0f; f <= 1; f += 0.2f) {
			Color c = enemySprite.material.color;
			c.g = f;
			c.b = f;
			enemySprite.material.color = c;

			yield return null;
		}
	}

	private void knockbackEnemy(int knockbackWeight) {
		float xVelocity = (float)knockbackWeight * 2.0f;
		if (enemySprite.flipX) {
			xVelocity *= -1;
		}
		rigidBody.velocity = new Vector2 (xVelocity, rigidBody.velocity.y);
	} 
}
	
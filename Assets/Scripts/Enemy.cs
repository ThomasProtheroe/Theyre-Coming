using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float moveSpeed = 1.5f;
	public float attackRange = 0.8f;
	public int health = 10;

	public BoxCollider2D attackHitbox;
	public BoxCollider2D bodyHitbox;
	private Animator anim;
	private Rigidbody2D rigidBody;
	private SpriteRenderer enemySprite;
	private GameObject player;

	private bool isActive;
	private bool isMoving;
	private bool isAttacking;
	private bool isStunned;
	private bool isDead;
	private float distanceToPlayer;


	// Use this for initialization
	void Start () {
		anim = gameObject.GetComponent<Animator> ();
		rigidBody = gameObject.GetComponent<Rigidbody2D> ();
		enemySprite = gameObject.GetComponent<SpriteRenderer> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		isMoving = false;
		isAttacking = false;

		foreach (BoxCollider2D collider in gameObject.GetComponents<BoxCollider2D> ()) {
			if (collider.isTrigger) {
				collider.enabled = false;
				attackHitbox = collider;
			} else {
				bodyHitbox = collider;
			}
		}

		//Let players pass through the enemy
		Physics2D.IgnoreCollision (player.GetComponent<CapsuleCollider2D>(), bodyHitbox);

		activate ();
	}
	
	// Update is called once per frame
	void Update () {
		if (player.transform.position.x > transform.position.x) {
			distanceToPlayer = player.transform.position.x - transform.position.x;
		} else {
			distanceToPlayer = transform.position.x - player.transform.position.x;
		}

		if (!isStunned) {
			if (isActive && !isAttacking && !isDead) {
				facePlayer ();
				move ();
				if (distanceToPlayer <= attackRange) {
					attack ();
				}
			} else {
				isMoving = false;
				rigidBody.velocity = new Vector2 (0, 0);
			}
		}
	}

	void activate() {
		anim.SetBool ("Active", true);
		isActive = true;
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

	void attack() {
		anim.SetTrigger ("Attack");
		isAttacking = true;
	}

	public void activateAttackHitbox() {
		attackHitbox.enabled = true;
	}

	public void finishAttack() {
		isAttacking = false;
		attackHitbox.enabled = false;
	}

	public void takeHit(int damage, int knockback) {
		if (!getIsDead ()) {
			StartCoroutine ("setHitFrame", knockback);
		}
		takeDamage (damage);
	}

	public void takeDamage(int damage) {
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
		anim.SetTrigger ("Death");
	}

	public void destroyEnemy() {
		Destroy (gameObject);
	}

	public bool getIsDead() {
		return isDead;
	}

	/**** Coroutines ****/ 
	IEnumerator setHitFrame(int knockbackWeight) {
		isStunned = true;
		isMoving = false;
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
		Debug.Log(knockbackTime);
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
	
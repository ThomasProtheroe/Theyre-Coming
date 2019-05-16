using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public BoxCollider2D attackHitbox;
	public BoxCollider2D bodyHitbox;
	public BoxCollider2D proximityHitbox;
	[HideInInspector]
	public Area currentArea;

	protected Animator anim;
	protected GameController gc;
	protected Rigidbody2D rigidBody;
	protected SpriteRenderer enemySprite;
	protected GameObject player;
	protected PlayerController playerCon;
	[SerializeField]
	private EnemyCorpse enemyCorpse;
	public List<BloodSplatter> bsList = new List<BloodSplatter>();
	public List<AshPile> apList = new List<AshPile>();


    /* Audio Components */
    [HideInInspector]
	public SoundController soundCon;
	protected AudioClip walkSound;
	protected AudioClip prowlSound;
	[SerializeField]
	protected AudioClip igniteSound;
	[SerializeField]
	protected AudioClip burningSound;
	[SerializeField]
	protected AudioClip disitingrateSound;
	[SerializeField]
	protected AudioClip splashSound;
	[SerializeField]
	protected AudioClip attackImpactSound;
	protected List<AudioClip> attackSounds;

	/* Particle Systems */
	[SerializeField]
	protected ParticleSystem bloodSprayPS;
	[SerializeField]
	protected ParticleSystem splashPS;
	[SerializeField]
	protected ParticleSystem bleedingPS;
	[SerializeField]
	protected ParticleSystem burningPS;
	[SerializeField]
	protected ParticleSystem[] burningDetailPS;

	[SerializeField]
	protected List<GameObject> headGiblets;
	[SerializeField]
	protected List<GameObject> bodyGiblets;

	public float groundLevel;
	public float moveSpeed = 1.5f;
	public float attackRange = 0.8f;
	protected float distanceToPlayer;
	public float burnDamageInterval = 1.0f;
	public float bleedDamageInterval = 1.0f;
	public int health = 10;
	public int attackDamage = 1;
	protected int playerAttackType;
	public bool hitPlayer;
	public bool isInvulnerable;
	protected int woundState;

	protected bool isActive;
	protected bool isMoving;
	protected bool isAttacking;
	protected bool isStunned;
	protected bool isBurning;
	protected bool isBleeding;
	protected bool isBlind;
	protected bool isDead;
	protected bool isStaggered;
	protected bool isNotSplattering;
	protected bool gibOnDeath;

	protected bool walkSoundPlaying;
	protected bool prowlSoundPlaying;
	protected bool burnSoundPlaying;

	private float iFrameTimer = 0.0f;
	[SerializeField]
	private float iFrameDuration;
	private float wanderTimer = 0.0f;
	private int wanderDirection;
	private float wanderAttackTimer = 0.0f;
	protected float burnTimer = 0.0f;
	protected float burnImmunityTimer = 0.0f;
	protected float stunTimer;
	protected float bleedTimer = 0.0f;
	[SerializeField]
	private float blindDuration;
	private float blindTimer = 0.0f;
	private float blindMoveModifier = 0.3f;
    
        // Use this for initialization
    void Start () {
		anim = gameObject.GetComponent<Animator> ();
		rigidBody = gameObject.GetComponent<Rigidbody2D> ();
		enemySprite = gameObject.GetComponent<SpriteRenderer> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		playerCon = player.GetComponent<PlayerController> ();
		soundCon = GameObject.FindGameObjectWithTag ("SoundController").GetComponent<SoundController> ();
		gc = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		isMoving = false;
		isAttacking = false;
		woundState = Constants.ENEMY_WOUND_NONE;

      
		//Let players pass through the enemy
        Physics2D.IgnoreCollision (player.GetComponent<CapsuleCollider2D>(), bodyHitbox);

		StartCoroutine ("pushEnemiesAway");
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if (player.transform.position.x > transform.position.x) {
			distanceToPlayer = player.transform.position.x - transform.position.x;
		} else {
			distanceToPlayer = transform.position.x - player.transform.position.x;
		}

		bool inAudioRange = false;
		if (distanceToPlayer < 22) {
			inAudioRange = true;
		}

		if (!isStunned && !isAttacking && !isDead) {
			takeAction ();
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

		//Periodic effects
		if (isBurning) {
			if (burnTimer > 0.0f) {
				burnTimer -= Time.deltaTime;
			} else {
				burnTimer = burnDamageInterval;
				playerAttackType = Constants.ATTACK_TYPE_FIRE;
				takeBurnDamage (1);
				playerAttackType = Constants.ATTACK_TYPE_UNTYPED;
			}

			if (burnImmunityTimer > 0.0f) {
				burnImmunityTimer -= Time.deltaTime;
			}
		}
		if (isStunned) {
			if (stunTimer > 0.0f) {
				stunTimer -= Time.deltaTime;
			} else {
				endStun ();
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
			if (wanderTimer > 0 && !isAttacking) {
				wanderTimer -= Time.deltaTime;
			}
			if (wanderAttackTimer > 0 && !isAttacking) {
				wanderAttackTimer -= Time.deltaTime;
			}

			if (blindTimer > 0.0f) {
				blindTimer -= Time.deltaTime;
			} else {
				endBlind ();
			}
		}

		//iframe control
		if (iFrameTimer > 0.0f) {
			iFrameTimer -= Time.deltaTime;
		} else {
			endIFrames ();
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		//I suppose you could use this code, but I liked code close to other functions.
		//if (other.gameObject.tag == "NoSplatterZone") {
		//isNotSplattering = true;
		//}
		if (other.gameObject.tag == "Enemy") {
			Enemy enemy = other.gameObject.GetComponent<Enemy> ();
			if (enemy.isBlind && other == enemy.attackHitbox) {
				float direction = transform.position.x - other.transform.position.x;
				playAttackImpactSound();
				takeHit (4, 0, direction);
			}

			if (isBurning) {
				Enemy otherEnemy = other.GetComponent<Enemy> ();
				if (!otherEnemy.isBurning) {
					otherEnemy.setBurning ();
				}
			}
		} 
		if (other.gameObject.tag == "AreaWall") {
			float wallPosition = other.gameObject.transform.position.x;
			float diff = other.gameObject.GetComponent<BoxCollider2D> ().bounds.extents.x + bodyHitbox.bounds.extents.x;
			var direction = transform.position - other.transform.position;
			if (direction.x > 0) {
				direction.x = 1;
			} else {
				direction.x = -1;
			}
			rigidBody.velocity = new Vector2 (0.0f, rigidBody.velocity.y);
			transform.position = new Vector3 (wallPosition + (diff * direction.x), transform.position.y);
		}
	}

	public virtual void takeAction() {
		if (isActive && isBlind) {
			//Move randomly back and forth
			if (wanderTimer > 0) {
				wanderBlind ();
			} else {
				startWander ();
			}

			//Attack randomly
			if (wanderAttackTimer <= 0) {
				attack ();
				wanderAttackTimer = UnityEngine.Random.Range (2.0f, 3.0f);
			}
		} else if (isActive) {
			if (player.GetComponent<PlayerController> ().getCurrentArea () == currentArea) {
				moveTowardsPlayer ();
				tryAttack ();
			} else {
				seekPlayer ();
			}
		} else {
			stopMoving ();
		}
	}

	protected void moveTowardsPlayer () {
		facePlayer ();
		move ();
	}

	protected void seekPlayer() {
		//Track player and move towards transition
		Transition target = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().findRouteToPlayer(currentArea);
		faceTarget (target.transform.position);
		move ();
		if (Vector2.Distance(target.gameObject.transform.position, gameObject.transform.position) < 0.5) {
			if (target.inUseByPlayer) {
				deactivate ();
			} else {
				target.enemyTravel (GetComponent<Enemy>());
			}
		}
	}

	protected virtual void tryAttack() {
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
		if (distanceToPlayer > attackRange) {
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

	protected void stopMoving() {
		isMoving = false;
		rigidBody.velocity = Vector2.zero;
	}

	protected void facePlayer() {
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

	void startWander() {
		//Pick a direction to wander in
		wanderDirection = UnityEngine.Random.Range (0, 2);
		if (wanderDirection == 0) {
			wanderDirection = -1;
		}

		//Face the direction enemy is moving
		if (enemySprite.flipX && wanderDirection == -1) {
			enemySprite.flipX = false;
			attackHitbox.offset = new Vector2 (attackHitbox.offset.x * -1, 0);
		} else if (!enemySprite.flipX && wanderDirection == 1) {
			enemySprite.flipX = true;
			attackHitbox.offset = new Vector2 (attackHitbox.offset.x * -1, 0);
		}

		//Start wandering
		wanderTimer = UnityEngine.Random.Range (2.0f, 3.2f);
	}

	void wanderBlind() {
		isMoving = true;
		float currentSpeed = moveSpeed;
		currentSpeed *= wanderDirection;
		rigidBody.velocity = new Vector2 (currentSpeed, rigidBody.velocity.y);
	}

	protected void attack() {
		hitPlayer = false;
		stopMoving ();
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

	public virtual bool takeHit(int damage, int knockback, float direction, bool noBlood=false, int attackType=Constants.ATTACK_TYPE_UNTYPED, bool brutal=false) {
		if (isInvulnerable || getIsDead ()) {
			return false;
		}
			
		stopProwlSound ();
		stopWalkSound ();
		playerAttackType = attackType;
		if (damage > 0 && !noBlood) {
			var sh = bloodSprayPS.shape;
			var main = bloodSprayPS.main;
			int particleCount = 15;
			if (brutal) {
				sh.arc = 40.0f;
				particleCount = 60;
				main.startLifetime = 0.6f;
			} else if (damage >= 10) {
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
			float bloodDirection;
			if (direction > 0) {
				bloodDirection = 180;
			} else {
				bloodDirection = 0;
			}
			sh.rotation = new Vector3 (sh.rotation.x, bloodDirection, sh.rotation.z);
			bloodSprayPS.Emit (particleCount);
		}

		takeDamage (damage);
		if (direction > 0) {
			knockback *= -1;
		}

		startIFrames ();
		isStaggered = true;
		StartCoroutine ("setHitFrame", knockback);

		playerAttackType = Constants.ATTACK_TYPE_UNTYPED;
		return true;
	}

	public virtual void takeThrowHit(int damage, int knockback, float direction, bool noBlood=false, int attackType=Constants.ATTACK_TYPE_UNTYPED) {
		takeHit (damage, knockback, direction, noBlood, attackType);
	}

	public virtual void takeFireHit(int damage) {
		if (burnImmunityTimer > 0.0f) {
			return;
		}

		playerAttackType = Constants.ATTACK_TYPE_FIRE;
		burnImmunityTimer = 1.0f;
		takeDamage (damage);
		setBurning ();
		playerAttackType = Constants.ATTACK_TYPE_UNTYPED;
	}

	public void takeDamage(int damage) {
		if (isDead || isInvulnerable) {
			return;
		}
		health -= damage;
		if (health <= 0) {
			killEnemy ();
		} else {
			updateWoundState ();
			updateAnimLayer ();
		}
        if (damage >= 5 && health <= 0){
            createBloodSplatter();
        }
	}

	private void startIFrames() {
		isInvulnerable = true;
		iFrameTimer = iFrameDuration;
	}

	private void endIFrames() {
		isInvulnerable = false;
		iFrameTimer = 0.0f;
	}

	private void updateWoundState() {
		if (health <= 3) {
			woundState = Constants.ENEMY_WOUND_HEAVY;
		} else if (health <= 7) {
			woundState = Constants.ENEMY_WOUND_LIGHT;
		} else {
			woundState = Constants.ENEMY_WOUND_NONE;
		}
	}

	public void killEnemy() {
		onDeath ();

		if (isAttacking) {
			finishAttack ();
		}
		isDead = true;
		stopMoving ();

		//Track enemy kills
		gc.countEnemyKill(playerAttackType);
		gc.removeEnemy (this);

		stopProwlSound ();
		stopWalkSound ();

		//De-parent the blood Particle System so the particles dont disappear when the enemy is destroyed
		bloodSprayPS.transform.parent = null;
		bloodSprayPS.GetComponent<DestroyAfterTime> ().StartCoroutine ("destroyAfterTime", 2.0f);

		if (gibOnDeath) {
			dismemberEnemy ();
		}
		else if (isBurning) {
			//Kill the burning detail particle systems before playing death animation (if active)
			foreach (ParticleSystem detailPS in burningDetailPS) {
				detailPS.Stop ();
			}
			stopBurningSound ();

			//play disintigrate sound
			anim.SetTrigger ("BurnDeath");
			if (playerCon.currentArea == currentArea) {
				soundCon.playEnemyOneShot (disitingrateSound);
			}
		} else {
			anim.SetTrigger ("Death");
		}
	}

	protected virtual void onDeath() {
		return;
	}

	public void destroyEnemy() {
		//spawnCorpse ();
		Destroy (gameObject);
	}

	public void setGibOnDeath(bool gib) {
		gibOnDeath = gib;
	}

	public void dismemberEnemy() {
		//Randomly select the gibs to use
		List<GameObject> gibs = new List<GameObject> ();
		int gibCount = Random.Range (4, 7);
		for (int i = 0; i < gibCount; i++) {
			int gibIndex = Random.Range (0, bodyGiblets.Count);
			gibs.Add (bodyGiblets[gibIndex]);
			bodyGiblets.RemoveAt (gibIndex);
		}
		gibs.Add (headGiblets[Random.Range(0, headGiblets.Count)]);

		//De-parent gibs and launch them
		foreach (GameObject giblet in gibs) {
			giblet.SetActive (true);
			giblet.transform.parent = null;

			Rigidbody2D body = giblet.GetComponent<Rigidbody2D> ();
			body.bodyType = RigidbodyType2D.Dynamic;
			body.AddForce (new Vector2 (Random.Range(-800, 800), Random.Range(100, 700)));
			body.AddTorque (50.0f);

			giblet.GetComponent<Giblet> ().startBloodTrail ();
			giblet.GetComponent<DestroyAfterTime> ().StartCoroutine ("destroyAfterTime", Random.Range(5f, 10f));
		}

		destroyEnemy ();
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

	public void OnTriggerStay2D(Collider2D other){
		if (other.gameObject.tag == "NoSplatterZone") {
			isNotSplattering = true;
		}
	}
	public void OnTriggerExit2D(Collider2D other){
		if (other.gameObject.tag == "NoSplatterZone"){
				isNotSplattering = false;
		}
	}
	public void createBloodSplatter(){//(int damage=5)
	if (!isNotSplattering)
    {
		int i = UnityEngine.Random.Range (0, bsList.Count);
		BloodSplatter blood = (BloodSplatter) Instantiate(bsList[i]);

		blood.transform.position = transform.position;
		blood.transform.localScale =  Vector3.one * Random.Range(0.2f,0.6f);
		//Not happy with the sizing. I want something tide to weapon damage, but more realistic.
		//float newScale = 0.5f + (damage * 0.1f);
		//blood.transform.localScale = new Vector3(newScale, newScale, 0.5f);
    }
	}
	public void createAshPile()
	{
		int i = UnityEngine.Random.Range (0, apList.Count);
		AshPile ash = (AshPile) Instantiate(apList[i]);
		Vector3 offset = new Vector3 (0, -0.35f, 0);
		ash.transform.position = transform.position + offset;
		ash.transform.localScale =  Vector3.one * Random.Range(0.1f,0.2f);
	}
    public virtual void takeBurnDamage(int damage) { 
		takeDamage (damage);
	}

	public void setBurning () {
		if (!isBurning) {
			isBurning = true;
			burnTimer = burnDamageInterval;
			burningPS.Play ();
			foreach (ParticleSystem detailPS in burningDetailPS) {
				detailPS.Play ();
			}
			playIgniteSound ();
			if (!burnSoundPlaying) {
				startBurningSound ();
			}
		}
	}

	public void stopBurning () {
		if (!isBurning) {
			return;
		}
		isBurning = false;
		burnTimer = 0.0f;
		burningPS.Stop ();
		foreach (ParticleSystem detailPS in burningDetailPS) {
			detailPS.Stop ();
		}
		if (burnSoundPlaying) {
			stopBurningSound ();
		}
	}

	public void setStun(float duration) {
		if (isStunned) {
			return;
		}
		isStunned = true;
		stunTimer = duration;
		deactivate ();
	}

	public void endStun() {
		isStunned = false;
		stunTimer = 0.0f;
		activate ();
	}

	public void setBleeding() {
		if (!isBleeding) {
			isBleeding = true;
			bleedTimer = bleedDamageInterval;
			bleedingPS.Play ();
		}
	}

	public void setBlind(float duration=0f) {
		if (!isBlind) {
			isBlind = true;
			if (duration == 0f) {
				blindTimer = blindDuration;
			} else {
				blindTimer = duration;
			}

			moveSpeed -= blindMoveModifier;
			startWander ();
			wanderAttackTimer = UnityEngine.Random.Range (2.0f, 3.0f);
			updateAnimLayer ();
		} else {
			blindTimer = blindDuration;
		}
	}

	public void endBlind() {
		isBlind = false;
		isMoving = false;
		wanderTimer = 0.0f;
		moveSpeed += blindMoveModifier;
		updateAnimLayer ();
	}

	public void updateAnimLayer() {
		int animLayer = woundState;
		float weight = 0.0f;
		if (isBlind) {
			animLayer += 3;
		}

		for(int i = 0; i < anim.layerCount; i++) {
			if (i == animLayer) {
				weight = 1.0f;
			} else {
				weight = 0.0f;
			}

			anim.SetLayerWeight (i, weight);
		}
	}

	public void triggerSplash(Color color) {
		var main = splashPS.main;
		main.startColor = color;
		splashPS.Play ();
		playSplashSound ();
	}

	public bool getIsDead() {
		return isDead;
	}

	public bool getIsBurning() {
		return isBurning;
	}

	public bool getIsStaggered() {
		return isStaggered;
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

	public void startBurningSound() {
		if (!burnSoundPlaying) {
			burnSoundPlaying = true;
			soundCon.playBurning (burningSound);
		}
	}

	public void stopBurningSound() {
		if (burnSoundPlaying) {
			burnSoundPlaying = false;
			soundCon.stopBurning (burningSound);
		}
	}

	public void playAttackImpactSound() {
		soundCon.playEnemyOneShot (attackImpactSound);
	}

	private void playAttackSound() {
		if (player.GetComponent<PlayerController>().currentArea != currentArea) {
			return;
		}
		soundCon.playEnemyOneShot (attackSounds[Random.Range(0, attackSounds.Count)]);
	}

	private void playIgniteSound() {
		soundCon.playEnemyOneShot (igniteSound);
	}

	private void playSplashSound() {
		soundCon.playSpash (splashSound);
	}

	protected virtual void onKnockbackEnd() {
		isStaggered = false;
		setStun (0.3f);
	}

	/**** Coroutines ****/ 
	IEnumerator setHitFrame(int knockbackWeight) {
		float knockbackTime = 0.17f + ((float)knockbackWeight / 100.0f);  //Calculate the actual time based on knockback stat
		setStun (knockbackTime);
		isMoving = false;
		stopProwlSound ();
		if (isAttacking) {
			finishAttack ();
		}

		anim.SetBool ("Knockback", true);

		if (knockbackWeight != 0) {
			knockbackEnemy (knockbackWeight);
		} else {
			rigidBody.velocity = new Vector2 (0, rigidBody.velocity.y);
		}
			
		yield return new WaitForSeconds(knockbackTime);

		anim.SetBool ("Knockback", false);
		stopMoving ();

		onKnockbackEnd ();
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
		
	IEnumerator pushEnemiesAway() {
		Collider2D[] colliders = new Collider2D[100];
		ContactFilter2D filter = new ContactFilter2D ();
		filter.useTriggers = true;
		proximityHitbox.OverlapCollider(filter, colliders);
		foreach (Collider2D collider in colliders) {
			if (collider && collider.gameObject.tag == "Enemy" && collider == collider.gameObject.GetComponent<Enemy> ().proximityHitbox) {
				float diff = 0.01f;
				float direction = transform.position.x - collider.gameObject.transform.position.x;
				if (direction < 0) {
					diff *= -1;
				}
				transform.position = new Vector3(transform.position.x + diff, transform.position.y, transform.position.z);
			}
		}

		yield return new WaitForSeconds(0.1f);

		StartCoroutine ("pushEnemiesAway");
	}


	private void knockbackEnemy(int knockbackWeight) {
		float xVelocity = (float)knockbackWeight * 2.0f;
		rigidBody.velocity = new Vector2 (xVelocity, rigidBody.velocity.y);
	} 
}
	
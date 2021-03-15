using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item {

	[SerializeField]
	protected CapsuleCollider2D throwHitCollider;

	[Header("Basic Attributes")]
	public int attackDamage;
	public int durability;
	public float attackSpeed;
	public int knockback;
	protected int maxDurability;

	[Header("Special Attributes")]
	public int multiHit = 1;
	public int multiHitThrow;
	public bool instantAttack;
	public bool inflictsShockwave;
	public bool isVorpal;
	public float vorpalChance = 0;

	[Header("Weapon Sounds")]
	public AudioClip swingSound;
	public AudioClip hitSound;
	public AudioClip missSound;

	protected int hitCount = 0;
	private bool hitWindowActive = false;

	protected Animator anim;

	protected override void Start() {
		anim = GetComponent<Animator> ();	
		maxDurability = durability;
		if (attackSpeed == 0f) {
			attackSpeed = 1.0f;
		}
		usable = true;

		base.Start ();
	}

	new void OnCollisionEnter2D(Collision2D other) {
		if (isThrown) {
			if (other.gameObject.tag == "Enemy" && !other.gameObject.GetComponent<Enemy>().isInvulnerable && !other.gameObject.GetComponent<Enemy>().getIsDead()) {
				float direction = transform.position.x - other.transform.position.x;
				hitCount = 1;
				other.gameObject.GetComponent<Enemy> ().takeThrowHit (thrownDamage, knockback, direction, false, attackType);
				onEnemyImpact (other.gameObject);
				reduceDurability ();
			} else {
				onTerrainImpact ();
			}

			isThrown = false;
			isBouncing = true;
			gameObject.layer = 11;
		}
		if (isAttacking && hitWindowActive && other.gameObject.tag == "Enemy" && !other.gameObject.GetComponent<Enemy>().isInvulnerable && !other.gameObject.GetComponent<Enemy>().getIsDead()) {
			
			if (!canHit ()) {
				return;
			}
			hitCount ++;
			float direction = player.transform.position.x - other.transform.position.x;
			if (inflictsGib) {
				other.gameObject.GetComponent<Enemy> ().setGibOnDeath (true);
			}
			bool vorpalHit = false;
			bool noBlood = false;
			if (isVorpal && (UnityEngine.Random.Range(1,100) <= vorpalChance)) {
				hitCount --;
				vorpalHit = true;
				noBlood = true;
			}
			other.gameObject.GetComponent<Enemy> ().takeHit (attackDamage, knockback, direction, noBlood, attackType, false, vorpalHit);
			onEnemyImpact (other.gameObject);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (isThrown && multiHitThrow > 0 && other.gameObject.tag == "Enemy" && !other.gameObject.GetComponent<Enemy>().isInvulnerable && !other.gameObject.GetComponent<Enemy>().getIsDead()) {
			float direction = transform.position.x - other.transform.position.x;
			hitCount ++;
			other.gameObject.GetComponent<Enemy> ().takeThrowHit (thrownDamage, knockback, direction, false, attackType);
			onEnemyImpact (other.gameObject);

			if (hitCount >= multiHitThrow) {
				throwHitCollider.enabled = false;
				hitCollider.enabled = true;
				reduceDurability ();
			}
		} 

		if (isThrown && multiHitThrow > 0 && (other.gameObject.tag == "AreaWall" || other.gameObject.tag == "Terrain")) {
			throwHitCollider.enabled = false;
			hitCollider.enabled = true;
			reduceDurability ();
		}
	}

	override public void use() {
		attack ();
	}

	public void attack() {
		isAttacking = true;

		anim.SetTrigger ("Attack");

		onAttack ();
	}

	protected void playAttackSound() {
		if (swingSound) {
			soundController.playPriorityOneShot (swingSound);
		}
	}

	protected void startHitWindow() {
		hitWindowActive = true;
		//Hit any enemies inside the hitbox when it's activated
		hitCount = 0;
		Collider2D[] colliders = new Collider2D[50];
		hitCollider.OverlapCollider(new ContactFilter2D(), colliders);
		foreach (Collider2D collider in colliders) {
			if (collider && collider.gameObject.tag == "Enemy" && !collider.gameObject.GetComponent<Enemy>().isInvulnerable && !collider.gameObject.GetComponent<Enemy> ().getIsDead ()) {
				if (!canHit ()) {
					break;
				}
				hitCount ++;
				float direction = player.transform.position.x - collider.transform.position.x;
				collider.gameObject.GetComponent<Enemy> ().takeHit (attackDamage, knockback, direction, false, attackType);
				onEnemyImpact (collider.gameObject);
			}
		}
	}

	protected void endHitWindow() {
		hitWindowActive = false;
		anim.speed = attackSpeed;

		if (inflictsShockwave) {
			triggerShockwave ();
		}

		if (hitCount > 0) {
			reduceDurability ();
		} else {
			if (missSound) {
				soundController.playPriorityOneShot (missSound);
			}
		}
	}

	public void finishAttack() {
		isAttacking = false;
		anim.speed = 1.0f;
	}

	bool canHit() {
		if (hitCount >= multiHit) {
			return false;
		}
		return true;
	}

	protected virtual void onAttack() {
		return;
	}

	public override void onThrow() {
		if (multiHitThrow > 0) {
			hitCount = 0;
			hitCollider.enabled = false;
			throwHitCollider.enabled = true;
		}
		base.onThrow();
	}

	public override void makeShoddy() {
		maxDurability = (int)Mathf.Floor((float)maxDurability * 0.7f);
		if (durability > maxDurability) {
			durability = maxDurability;
		}

		base.makeShoddy();
	}

	public override void updateDurabilityIndicator() {
		playerCon.activeSlot.setDurabilityIndicator(((float)durability / maxDurability));
	}

	public virtual void onEnemyImpact(GameObject enemy) {
		if (inflictsBleed) {
			enemy.GetComponent<Enemy> ().setBleeding();
		}

		if (inflictsBlind) {
			enemy.GetComponent<Enemy> ().setBlind();
		}

		if (inflictsBurning) {
			enemy.GetComponent<Enemy> ().setBurning ();
		}

		if (isThrown && throwImpact) {
			soundController.playPriorityOneShot (throwImpact);
		} else if (!isThrown && hitSound) {
			soundController.playPriorityOneShot (hitSound);
		}
	}

	protected virtual void reduceDurability() {
		durability -= ((hitCount + 1) / 2);

		if (durability <= 0) {
			breakItem ();
		} else {
			//Update UI
			updateDurabilityIndicator();

			if (UnityEngine.Random.Range(0,3) == 1) {
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
	}

	private void triggerShockwave() {
		var list = new List<RaycastHit2D> ();
		list.AddRange (Physics2D.RaycastAll(transform.position, Vector2.right, 3f, 1 << LayerMask.NameToLayer("Enemy")));
		list.AddRange (Physics2D.RaycastAll(transform.position, Vector2.left, 3f, 1 << LayerMask.NameToLayer("Enemy")));

		RaycastHit2D[] enemies = list.ToArray ();

		foreach(RaycastHit2D collision in enemies) {
			float direction = transform.position.x - collision.transform.position.x;

			collision.transform.gameObject.GetComponent<Enemy> ().takeHit (0, 3, direction, true);
		}

		playerCon.gameCon.shakeCamera (0.4f, 0.15f);
	}
}

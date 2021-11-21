using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : MonoBehaviour {

	private PlayerController player;
	private int damage;
	private int knockback;
	[SerializeField]
	private bool instantAttack;

	[Header("Sounds")]
	public EnhancedAudioClip swingSound;
	public List<EnhancedAudioClip> hitSounds;
	private SoundController soundCon;

	protected int hitCount = 0;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		soundCon = GameObject.FindGameObjectWithTag ("SoundController").GetComponent<SoundController> ();
		damage = player.punchDamage;
		knockback = 0;
	}

	public void startHitWindow() {
		soundCon.playPriorityOneShot (swingSound);
		hitCount = 0;
		GetComponent<CircleCollider2D> ().enabled = true;

		Collider2D[] colliders = new Collider2D[50];
		GetComponent<CircleCollider2D> ().OverlapCollider(new ContactFilter2D(), colliders);
		foreach (Collider2D collider in colliders) {
			if (collider && collider.gameObject.tag == "Enemy" && !collider.gameObject.GetComponent<Enemy>().isInvulnerable && !collider.gameObject.GetComponent<Enemy> ().getIsDead ()) {
				if (!canHit ()) {
					break;
				}
				hitCount ++;
				float direction = player.transform.position.x - collider.transform.position.x;
				collider.gameObject.GetComponent<Enemy> ().takeHit (damage, knockback, direction, false, 0);
				onEnemyHit();
			}
		}
	}

	public void endHitWindow() {
		GetComponent<CircleCollider2D> ().enabled = false;
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Enemy" && !other.gameObject.GetComponent<Enemy>().isInvulnerable && !other.gameObject.GetComponent<Enemy>().getIsDead()) {
			if (!canHit ()) {
				return;
			}
			hitCount ++;
			float direction = player.transform.position.x - other.transform.position.x;
			other.gameObject.GetComponent<Enemy> ().takeHit (damage, knockback, direction, false, 0);
			onEnemyHit();
		}
	}

	void onEnemyHit() {
		if (hitSounds.Count > 0) {
			soundCon.playPriorityOneShot (hitSounds[Random.Range(0, hitSounds.Count)]);
		}
	}

	bool canHit() {
		if (hitCount >= 1) {
			return false;
		}
		return true;
	}
}

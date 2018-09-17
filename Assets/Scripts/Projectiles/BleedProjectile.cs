using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedProjectile : BaseProjectile {

	public int damage;
	public float lifetime;
	private List<Enemy> enemiesHit;

	public override void Start() {
		enemiesHit = new List<Enemy> ();

		base.Start ();
	}

	// Update is called once per frame
	void Update () {
		if (!isActive) {
			return;
		}
		
		lifetime -= Time.deltaTime;
		if (lifetime <= 0) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (!isActive) {
			return;
		}
		if (other.gameObject.tag == "Enemy") {
			Enemy enemy = other.gameObject.GetComponent<Enemy> ();
			if (enemiesHit.Contains (enemy)) {
				return;
			}

			hitTarget (enemy);
		} else if (other.gameObject.tag == "AreaWall" || other.gameObject.tag == "Terrain") {
			Destroy (gameObject);
		}
	}

	protected override bool hitTarget(Enemy target) {
		if (!target.isInvulnerable && !target.getIsDead ()) {
			float direction = player.transform.position.x - target.transform.position.x;
			target.takeHit (damage, knockback, direction, false, Constants.ATTACK_TYPE_PROJECTILE);
			target.setBleeding ();
			enemiesHit.Add (target);
			playImpactSound ();
		}

		return true;
	}
}

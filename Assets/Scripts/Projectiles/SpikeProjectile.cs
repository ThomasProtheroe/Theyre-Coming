using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeProjectile : BaseProjectile {

	public int damage;
	public int durability;

	void OnTriggerEnter2D (Collider2D other) {
		if (!isActive) {
			return;
		}

		if (other.gameObject.tag == "Enemy") {
			Enemy enemy = other.gameObject.GetComponent<Enemy> ();
			hitTarget (enemy);
		} else if (other.gameObject.tag == "AreaWall") {
			Destroy (gameObject);
		}
	}

	protected override bool hitTarget(Enemy target) {
		if (!target.isInvulnerable && !target.getIsDead ()) {
			float direction = player.transform.position.x - target.transform.position.x;
			target.takeHit (damage, knockback, direction, false, Constants.ATTACK_TYPE_PROJECTILE);
			durability--;
			playImpactSound ();
			if (durability <= 0) {
				Destroy (gameObject);
			}
		}

		return true;
	}
}

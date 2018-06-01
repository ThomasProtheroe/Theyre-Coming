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
			if (!enemy.isInvunlerable && !enemy.getIsDead ()) {
				float direction = transform.position.x - enemy.transform.position.x;
				enemy.takeHit (damage, knockback, direction, false, Constants.ATTACK_TYPE_PROJECTILE);
				durability--;
				if (durability <= 0) {
					Destroy (gameObject);
				}
			}
		} else if (other.gameObject.tag == "AreaWall") {
			Destroy (gameObject);
		}
	}
}

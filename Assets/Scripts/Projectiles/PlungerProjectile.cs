using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlungerProjectile : BaseProjectile {

	void OnTriggerEnter2D (Collider2D other) {
		if (!isActive) {
			return;
		}

		if (other.gameObject.tag == "Enemy") {
			Enemy enemy = other.gameObject.GetComponent<Enemy> ();
			if (!enemy.isInvunlerable && !enemy.getIsDead ()) {
				enemy.setBlind ();
				enemy.setStun (2.0f);
				float direction = transform.position.x - enemy.transform.position.x;
				enemy.takeHit (0, knockback, direction, true, Constants.ATTACK_TYPE_PROJECTILE);

				Destroy (gameObject);
			}
		} else if (other.gameObject.tag == "AreaWall") {
			Destroy (gameObject);
		}
	}
}

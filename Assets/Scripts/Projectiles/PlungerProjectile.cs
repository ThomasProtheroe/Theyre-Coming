using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlungerProjectile : BaseProjectile {

	[SerializeField]
	private float xOffset;
	[SerializeField]
	private float yOffset;
	private bool isAttached;
	private SpriteRenderer enemySprite;

	void Update () {
		if (!isAttached) {
			return;
		}

		if (enemySprite.flipX) {

		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (!isActive) {
			return;
		}

		if (other.gameObject.tag == "Enemy") {
			Enemy enemy = other.gameObject.GetComponent<Enemy> ();
			if (!enemy.isInvulnerable && !enemy.getIsDead ()) {
				enemy.setBlind (9999999f);
				enemy.setStun (2.0f);
				float direction = transform.position.x - enemy.transform.position.x;
				enemy.takeHit (0, knockback, direction, true, Constants.ATTACK_TYPE_PROJECTILE);

				isActive = false;
				isAttached = true;
				enemySprite = enemy.GetComponent<SpriteRenderer> ();
				GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
				transform.parent = enemy.transform;
				transform.localPosition = new Vector3 (xOffset, yOffset, 0.0f);
			}
		} else if (other.gameObject.tag == "AreaWall") {
			Destroy (gameObject);
		}
	}
}

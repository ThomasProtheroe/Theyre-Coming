using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStaff : FlamingBat {
	[Header("Projectile Attributes")]
	[SerializeField]
	private FireGlob fireGlob;
	[SerializeField]
	private float projectileSpeed;
	[SerializeField]
	private float fireLifetime;

	protected override void onAttack() {
		Invoke ("launchFireGlob", 0.1f);
		return;
	}

	private void launchFireGlob() {
		FireGlob newGlob = Instantiate (fireGlob, new Vector3(transform.position.x, transform.position.y + 0.25f, 0), Quaternion.identity);
		newGlob.friendlyFire = true;
		newGlob.lifetime = fireLifetime;
		newGlob.activeDamage = 2;
		newGlob.enableCollisions ();

		Rigidbody2D rb = newGlob.GetComponent<Rigidbody2D> ();
		float speed = projectileSpeed;
		if (playerCon.playerSprite.flipX) {
			speed *= -1;
		}
		Vector2 globVelocity = new Vector2 (speed, 2.5f);
		rb.velocity = globVelocity;

		hitCount = 1;
	}
}

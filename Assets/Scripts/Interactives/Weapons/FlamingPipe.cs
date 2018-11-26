using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamingPipe : FlamingBat {
	[SerializeField]
	private ParticleSystem flameBurstPS;
	[SerializeField]
	private FlameProjectile projectile;
	[SerializeField]
	private float projectileSpeed;

	protected override void onAttack() {
		Invoke ("playFlameBurst", 0.05f);
		Invoke ("fireProjectile", 0.1f);
		return;
	}

	private void fireProjectile() {
		FlameProjectile fp = Instantiate (projectile, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);

		float speed = projectileSpeed;
		if (playerCon.playerSprite.flipX) {
			speed *= -1;
		}

		fp.GetComponent<Rigidbody2D> ().velocity = new Vector2 (speed, 0.0f);
		fp.lifetime = 0.25f;
		fp.damage = 1;
	}

	private void playFlameBurst() {
		flameBurstPS.Play ();
	}
}

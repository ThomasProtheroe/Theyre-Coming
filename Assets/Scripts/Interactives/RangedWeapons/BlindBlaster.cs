using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindBlaster : RangedWeapon {

	public ParticleSystem stream;
	public BlindProjectile projectile;
	[SerializeField]
	private Color splashColor;

	private float nextProjectileTime;

	[SerializeField]
	private float projectileSpeed;

	public new void Update() {
		if (isAttacking) {
			nextProjectileTime -= Time.deltaTime;
			if (nextProjectileTime <= 0) {
				fireProjectile ();
				nextProjectileTime = 0.1f;
			}

			if (!stream.isEmitting) {
				isAttacking = false;
				if (ammunition <= 0) {
					setEmpty ();
				}
			}
		}

		base.Update ();
	}

	override public void fire() {
		isAttacking = true;
		ammunition -= 1;
		nextProjectileTime = 0.0f;
		stream.Play ();
		source.PlayOneShot (fireSound);
	}

	public new void setEmpty() {
		stream.transform.parent = null;
		stream.GetComponent<DestroyAfterTime> ().StartCoroutine ("destroyAfterTime", 1.0f);
		base.setEmpty ();
	}

	private void fireProjectile() {
		BlindProjectile bp = Instantiate (projectile, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);

		float speed = projectileSpeed;
		if (playerCon.playerSprite.flipX) {
			speed *= -1;
		}

		bp.GetComponent<Rigidbody2D> ().velocity = new Vector2 (speed, 0.0f);
		bp.lifetime = 0.7f;
		bp.color = splashColor;
	}
}

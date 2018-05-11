using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindSquirtGun : RangedWeapon {

	public ParticleSystem stream;
	public BlindProjectile projectile;
	[SerializeField]
	private Color splashColor;

	[SerializeField]
	private float projectileSpeed;

	public new void Update() {
		if (isAttacking) {
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
		stream.Play ();
		fireProjectile ();
		soundController.playPriorityOneShot (fireSound);

		base.fire ();
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
		bp.lifetime = 0.75f;
		bp.color = splashColor;
		bp.destroyOnContact = true;
	}
}

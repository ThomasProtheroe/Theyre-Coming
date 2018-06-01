using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomemadeCannon : RangedWeapon {

	public ParticleSystem smokePS;
	public ParticleSystem projectilePS;
	public BaseProjectile projectile;

	[SerializeField]
	private float projectileSpeed;

	override public void fire() {
		isAttacking = true;
		smokePS.Play ();
		if (projectilePS != null) {
			projectilePS.Play ();
		}

		soundController.playPriorityOneShot (fireSound);
		fireProjectile ();
		playerCon.gameCon.shakeCamera (0.2f, 0.1f);

		setEmpty ();

		base.fire ();

		Invoke ("stopAttacking", 0.5f);
	}

	public new void setEmpty() {
		smokePS.transform.parent = null;
		smokePS.GetComponent<DestroyAfterTime> ().StartCoroutine ("destroyAfterTime", 1.0f);
		if (projectilePS != null) {
			projectilePS.transform.parent = null;
			projectilePS.GetComponent<DestroyAfterTime> ().StartCoroutine ("destroyAfterTime", 1.0f);
		}

		base.setEmpty ();
	}

	private void fireProjectile() {
		float speed = projectileSpeed;
		if (playerCon.playerSprite.flipX) {
			speed *= -1;
		}

		projectile.isActive = true;
		projectile.transform.parent = null;
		projectile.GetComponent<Rigidbody2D> ().velocity = new Vector2 (speed, 0.0f);
	}
}

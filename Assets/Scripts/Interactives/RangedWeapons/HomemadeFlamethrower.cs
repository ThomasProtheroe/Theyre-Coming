using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomemadeFlamethrower : RangedWeapon {

	public ParticleSystem pilot;
	public ParticleSystem stream;
	public FlameProjectile projectile;

	private float nextProjectileTime;

	[SerializeField]
	private float projectileSpeed;


	public void Update() {
		if (isAttacking) {
			nextProjectileTime -= Time.deltaTime;
			if (nextProjectileTime <= 0) {
				fireProjectile ();
				nextProjectileTime = 0.1f;
			}

			if (!stream.isEmitting) {
				if (capacity <= 0) {
					//swap with empty weapon
				}
				isAttacking = false;
				pilot.Play ();
			}
		}
	}

	override public void fire() {
		isAttacking = true;
		nextProjectileTime = 0.0f;
		pilot.Stop ();
		stream.Play ();
	}

	private void fireProjectile() {
		FlameProjectile fp = Instantiate (projectile, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
		float speed = projectileSpeed;
		fp.GetComponent<Rigidbody2D> ().velocity = new Vector2 (speed, 0.0f);
	}

	override public void onPickup() {
		pilot.Play ();
	}

	override public void onDrop() {
		pilot.Stop ();
	}

	override public void onThrow() {
		pilot.Stop ();
	}
}
 
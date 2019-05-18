using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastProjectile : BaseProjectile {

	[SerializeField]
	private ParticleSystem fusePS;
	[SerializeField]
	private GameObject explosion;

	void OnTriggerEnter2D(Collider2D other) {
		if (!isActive) {
			return;
		}

		if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "AreaWall") {
			explode ();
		}
	}

	public override void fire() {
		fusePS.Play ();

		base.fire ();
	}

	protected override bool hitTarget(Enemy target) {
		if (!target.getIsDead ()) {
			explode ();
			return false;
		}

		return true;
	}

	private void explode() {
		Instantiate (explosion, transform.position, Quaternion.identity);

		Destroy (gameObject);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWheels : RemoteCarTrap {

	[SerializeField]
	private ParticleSystem fusePS;
	[SerializeField]
	private GameObject explosion;

	protected new void OnTriggerEnter2D(Collider2D other) {
		if (isDeployed) {
			if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "AreaWall") {
				explode ();
			}
		}
	}

	private void explode() {
		Instantiate (explosion, transform.position, Quaternion.identity);

		soundController.stopEnvironmentalSound (accelerationSound);
		breakItem ();
	}

	protected override void onDeploy() {
		var sh = fusePS.shape;
		var main = fusePS.main;
		var em = fusePS.emission;
		main.startLifetime = 1f;
		em.rateOverTimeMultiplier = 2500.0f;
		sh.position = new Vector3 (0.11f, sh.position.y, sh.position.z);
	}

	override public void onTravel() {
		fusePS.Stop ();
	}

	override public void onArrival() {
		fusePS.Play ();
	}

	public override bool onBreak() {
		fusePS.Stop ();
		soundController.stopEnvironmentalSound (accelerationSound);

		//De-parent the Particle System so the particles dont disappear when the item is destroyed
		fusePS.transform.parent = null;
		fusePS.GetComponent<DestroyAfterTime> ().StartCoroutine ("destroyAfterTime", 1.0f);

		return true;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWheels : RemoteCarTrap {

	[SerializeField]
	private ParticleSystem fusePS;
	[SerializeField]
	private ParticleSystem explosionPS;

	protected new void OnTriggerEnter2D(Collider2D other) {
		if (isDeployed) {
			if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "AreaWall") {
				explode ();
			}
		}
	}

	private void explode() {
		//Raycast to get all enemies/players, deal damage to them
		//TODO

		source.Stop ();
		breakItem ();
	}

	protected override void onDeploy() {
		var sh = fusePS.shape;
		var main = fusePS.main;
		var em = fusePS.emission;
		main.startLifetime = 2.5f;
		em.rateOverTimeMultiplier = 2500.0f;
		sh.position = new Vector3 (0.11f, sh.position.y, sh.position.z);
	}

	override public void onTravel() {
		fusePS.Stop ();
	}

	override public void onArrival() {
		fusePS.Play ();
	}

	public new bool onBreak() {
		fusePS.Stop ();
		source.Stop ();

		return false;
	}
}

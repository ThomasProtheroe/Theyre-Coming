using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamingBat : Weapon {

	public override void onTravel() {
		ParticleSystem[] flames = GetComponentsInChildren<ParticleSystem> ();
		foreach(ParticleSystem flame in flames) {
			flame.Stop ();
		}
	}

	public override void onArrival() {
		ParticleSystem[] flames = GetComponentsInChildren<ParticleSystem> ();
		foreach(ParticleSystem flame in flames) {
			flame.Play ();
		}
	}
}

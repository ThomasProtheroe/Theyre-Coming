using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomemadeFlamethrower : RangedWeapon {

	public ParticleSystem pilot;
	public ParticleSystem stream;

	override public void fire() {
		pilot.Stop ();
		stream.Play ();
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
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomemadeFlamethrower : RangedWeapon {

	public ParticleSystem pilot;
	public ParticleSystem stream;

	public void Update() {
		if (isAttacking && !stream.isEmitting) {
			isAttacking = false;
			pilot.Play ();
		}
	}

	override public void fire() {
		isAttacking = true;
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
 
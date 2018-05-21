using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlazingWheels : RemoteCarTrap {

	[SerializeField]
	private ParticleSystem firePS;

	override public void onTravel() {
		firePS.Stop ();
	}

	override public void onArrival() {
		firePS.Play ();
	}
}

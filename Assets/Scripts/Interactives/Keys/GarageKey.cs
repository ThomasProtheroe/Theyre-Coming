using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageKey : Item {

	[SerializeField]
	private Transition targetDoor;

	protected override void Start() {


		base.Start ();
	}
	
	public override void use() {
		GameObject closestInteractive = playerCon.getClosestInteractive ();
		if (closestInteractive.tag == "Transition") {
			unlockDoor (closestInteractive.GetComponent<Transition> ());
		}
	}

	public void unlockDoor(Transition door) {
		if (door == targetDoor) {
			door.unlock ();
			playerCon.emptyPlayerHands ();
			Destroy (gameObject);
		}
	}
}

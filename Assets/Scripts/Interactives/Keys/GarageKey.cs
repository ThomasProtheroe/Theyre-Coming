using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageKey : Item {

	[SerializeField]
	private LockedTransition targetDoor;

	// Use this for initialization
	void Start () {
		
	}
	
	public override void use() {

	}

	public void unlockDoor(LockedTransition door) {
		if (door == targetDoor) {
			door.unlock ();
			Destroy (gameObject);
		}
	}
}

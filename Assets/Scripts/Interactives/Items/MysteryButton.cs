using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryButton : Item {

	[SerializeField]
	private LabEntrance labEntrance;
	[SerializeField]
	private AudioClip useSound;

	protected override void Start() {
		usable = true;

		//FInd the lab entrance transition
		foreach (GameObject transitionObj in GameObject.FindGameObjectsWithTag ("Transition")) {
			LabEntrance transition = transitionObj.GetComponent<LabEntrance> ();
			if (transition != null) {
				labEntrance = transition;
			}
		}
			
		base.Start ();
	}

	public override void use ()
	{
		playUseSound ();
		if (playerCon.getCurrentArea () == labEntrance.gameObject.transform.parent.gameObject.GetComponent<Area> ()) {
			labEntrance.reveal ();
		}
	}

	public void drop() {
		Rigidbody2D body = GetComponent<Rigidbody2D>();

		pickupCollider.enabled = false;
		hitCollider.enabled = true;
		body.bodyType = RigidbodyType2D.Dynamic;
		isThrown = true;
		gameObject.layer = 12;
	}

	private void playUseSound() {
		soundController.playPriorityOneShot (useSound);
	}
}

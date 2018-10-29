using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabEntrance : Transition {

	[Header("Lab Specific Settings")]
	[SerializeField]
	private Collider2D entranceCollider;
	[HideInInspector]
	public bool isLabEntrance = true;

	public void reveal() {
		//unlock and activate sprite/collider
		unlock();
		GetComponent<SpriteRenderer> ().enabled = true;
		entranceCollider.enabled = true;

		//Play opening animation
		anim.SetTrigger ("Reveal");
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabEntrance : Transition {

	[Header("Lab Specific Settings")]
	[SerializeField]
	private Collider2D entranceCollider;
	[HideInInspector]
	public bool isLabEntrance = true;
	[SerializeField]
	private GameController gameCon;
	[SerializeField]
	private AudioClip revealSound;

	public void reveal() {
		//unlock and activate sprite/collider
		unlock();
		GetComponent<SpriteRenderer> ().enabled = true;
		entranceCollider.enabled = true;

		//Play opening animation and sound
		anim.SetTrigger ("Reveal");
		soundCon.playPriorityOneShot (revealSound);
	}

	public override void onPlayerArrival() {
		gameCon.activateAllEnemies ();
		gameCon.startTimer ();
	}

	public override void onPlayerTravel() {
		gameCon.miscFadeOut ();
		musicCon.fadeMusicOut ();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grenade : Throwable {

	[SerializeField]
	private float fuseDuration;
	[SerializeField]
	private GameObject explosion;
	[SerializeField]
	private Text timerText;
	[SerializeField]
	private AudioClip fuseSound;

	private bool isArmed;
	private float fuseTimer;

	// Update is called once per frame
	protected override void Update () {
		if (isArmed) {
			fuseTimer -= Time.deltaTime;

			if (fuseTimer <= 0.0f) {
				explode ();
			}
			
			timerText.text = Mathf.Ceil(fuseTimer).ToString();
		}

		base.Update ();
	}

	private void explode() {
		Instantiate (explosion, transform.position, Quaternion.identity);
	
		soundController.stopEnvironmentalSound (fuseSound);
		breakItem ();
	}

	//Return true if item should be destroyed immediately (no animation)
	public override bool onBreak() {
		return true;
	}

	private void arm() {
		isArmed = true;
		soundController.playEnvironmentalSound (fuseSound, true);
		fuseTimer = fuseDuration;
		timerText.gameObject.SetActive(true);
	}

	public override void onCraft() {
		arm();
	}
}

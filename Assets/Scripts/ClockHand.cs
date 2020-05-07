using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockHand : MonoBehaviour {

	[SerializeField]
	private SoundController soundCon;
	[SerializeField]
	private AudioClip tickSound;

	private bool canHear;
	private float timer;
	private int seconds;
	private int rotation;
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > seconds + 1) {
			moveHand();
			seconds ++;

			if (seconds == 60) {
				timer = 0f;
				seconds = 0;
			}
		}
	}

	void moveHand() {
		rotation -= 6;
		if (rotation > 360) {
			rotation = 0;
		}
		Quaternion target = Quaternion.Euler(0, 0, rotation);
		transform.rotation = target;

		if (canHear) {
			soundCon.playEnvironmentalSound(tickSound, false);
		}
	}
}

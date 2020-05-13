using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockHand : MonoBehaviour {

	[SerializeField]
	private List<AudioClip> tickSounds;
	private PlayerController player;
	private SoundController soundCon;
	[SerializeField]
	private Area area;
	
	private bool canHear;
	private float timer;
	private int seconds;
	private int rotation;
	private int tickCount;
	
	void Start() {
		tickCount = 0;
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController> ();
		soundCon = GameObject.FindGameObjectWithTag ("SoundController").GetComponent<SoundController> ();
	}

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

		if (player.getCurrentArea().name == "Livingroom") {
			canHear = true;
		} else {
			canHear = false;
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
			playTickSound();
		}
	}

	void playTickSound() {
		soundCon.playEnvironmentalSound(tickSounds[tickCount], false);
		tickCount ++;
		if (tickCount >= tickSounds.Count) {
			tickCount = 0;
		}
	}
}
